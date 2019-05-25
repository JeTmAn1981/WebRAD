Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Linq
Imports WhitTools.DataWarehouse
Imports WhitTools.eCommerce
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.File
Imports WhitTools.Filler
Imports WhitTools.DataTables
Imports WhitTools.Converter
Imports WhitTools.Validator
Imports WhitTools.Formatter
Imports WhitTools.GlobalEnum
Imports WhitTools.Repeaters
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.WebTeam
Imports WhitTools.SQL
Imports WhitTools.Encryption
Imports WhitTools.Workflow
Imports whittools.Utilities
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Ancillary
Imports Common.General.Pages
Imports Common.Webpages.Backend.Main
Imports Common.BuildSetup
Imports Common.Webpages.Main
Imports Common.sql.main
Imports Common.General.ProjectOperations
Imports Common.ProjectFiles
Imports Common.General.Logging
Imports System.Threading
Imports System.Reflection
Imports Common
Imports Common.General.Repeaters
Imports Common.SQL.StoredProcedure
Imports Common.General.ControlTypes
Imports Common.General.Folders
Imports Common.Webpages.ControlContent

Partial Public Class buildproject
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        stopwatch = New Stopwatch()
        stopwatch.Start()
        lastProgressMark = stopwatch.Elapsed

        Try
            SetupBuildProcess()

            UpdateProgress("Creating SQL", 0)
            ProcessSQL()

            UpdateProgress("Creating webpages", 0)

            DoWebpageCreationSetup()

            CreateWebpages()

            If CurrentProjectRequiresWhitworthLogin() Then
                UpdateMissingLoginInfo()
            End If
        Catch ex As Exception
            UpdateProgress("Exiting with error: " & ex.ToString(), 0)
        End Try

        stopwatch.Stop()
        UpdateProgress("Finished - " & stopwatch.Elapsed.ToString() & " elapsed", 0)
    End Sub

    Sub SetupBuildProcess()
        db = New WebRADEntities()
        Session("ProjectID") = ""
        SetImports()

        CreateLoginColumnTypes()

        GetProjectDetails()
        EliminateNull(projectDT)

        ' CleanUp()

        BindData()

        GetTopLevelControls()

        SetupLogger()
    End Sub

    Sub BindData()
        Dim sDepartmentname As String = ""

        GetProjectDetails()

        With projectDT.Rows(0)
            GetDepartmentInfo("", sDepartmentname, .Item("Department"), .Item("CustomDepartmentName"))

            lblEcommerce.Text = If(IsECommerceProject(), "Y", "N")

            isWorkflow = GetProjectOption("Workflow")

            projectName = RemoveNonAlphanumeric(.Item("PageTitle"))
            projectTitle = .Item("PageTitle")
            SQLServerName = .Item("SQLServerName")
            SQLDatabaseName = .Item("SQLDBName")
            SQLMainTableName = .Item("SQLMainTableName")

            sqlcnx = CreateSQLConnection(SQLDatabaseName, SQLServerName)

            lblDepartmentName.Text = sDepartmentname
            lblSQLMainTableName.Text = .Item("SQLMainTableName")
            lblSQLInsertStoredProcedureName.Text = .Item("SQLInsertStoredProcedureName")
            lblSQLUpdateStoredProcedureName.Text = .Item("SQLUpdateStoredProcedureName")
            lblControlList.Text = GetListofValues("select case when Heading  = '' then Name + ' (' + Prefix + ')' else Heading + ' (' + Prefix + ')' end ControlDetail from " & DT_WEBRAD_PROJECTCONTROLS & " C left outer join " & DT_WEBRAD_CONTROLTYPES & "  T on C.ControlType = T.ID  left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  D on T.DataType = D.ID Where C.ProjectID = " & GetProjectID() & " Order by position asc", "ControlDetail")
            txtNotes.Text = .Item("Notes")
        End With

        setProjectBuildVariables()
    End Sub


    Sub CleanUp()
        UpdateControlNames()
        RemoveInvalidCharactersFromTextLiterals()

        '  RemoveOrphanedDataSources()
    End Sub

    Private Shared Sub RemoveInvalidCharactersFromTextLiterals()
        currentProject.ProjectControls.Where(Function(pc) pc.ControlType1.DataType = N_TEXTLITERAL_DATA_TYPE).ToList().ForEach(Sub(pc)
                                                                                                                                   pc.Value = RemoveInvalidCharacters(pc.Value)
                                                                                                                               End Sub)

        db.SaveChanges()
    End Sub

    Private Shared Function RemoveInvalidCharacters(ByVal text As String)
        Return text.Replace("Â", "")
    End Function

    Sub RemoveOrphanedDataSources()
        For Each CurrentRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_DATASOURCEPARENTTYPES).Rows
            WhitTools.SQL.ExecuteNonQuery("DELETE FROM " & DT_WEBRAD_PROJECTDATASOURCES & " WHERE ParentType = " & CurrentRow.Item("ID") & " AND NOT ID IN (SELECT DataSourceID FROM " & CurrentRow.Item("Table") & " WHERE NOT DataSourceID IS NULL)", CreateSQLConnection("WebRAD"))
        Next
    End Sub


    Sub SaveNotes()
        Dim cmd As New SqlCommand("usp_UpdateProjectNotes", Common.General.Variables.cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@ProjectID", GetProjectID())
        cmd.Parameters.AddWithValue("@Notes", txtNotes.Text)

        WhitTools.SQL.ExecuteNonQuery(cmd)
    End Sub

    Protected Sub libControlDetails_Click(sender As Object, e As EventArgs) Handles libControlDetails.Click
        Redirect("controls.aspx?ID=" & GetProjectID() & "&PageID=" & GetFirstPage(GetProjectID()))
    End Sub

    Protected Sub libMainDetails_Click(sender As Object, e As EventArgs) Handles libMainDetails.Click
        Redirect("index.aspx?ID=" & GetProjectID())
    End Sub

    Protected Sub libFrontendDetails_Click(sender As Object, e As EventArgs) Handles libFrontendDetails.Click
        Redirect("frontend.aspx?ID=" & GetProjectID())
    End Sub

    Protected Sub libBackendDetails_Click(sender As Object, e As EventArgs) Handles libBackendDetails.Click
        Redirect("backend.aspx?ID=" & GetProjectID())
    End Sub

    
    Sub UpdateMissingLoginInfo()
        Dim sTableReference As String = SQLServerName & "." & SQLDatabaseName & ".dbo." & GetAncillaryProject("SQLMainTableName")
        Dim nUpdateNeeded As Integer = GetDataTable("SELECT CASE WHEN (SELECT COUNT(*) FROM " & sTableReference & " WHERE IDNumber IS NULL) > ((SELECT COUNT(*) FROM " & sTableReference & ") / 10) THEN 1 ELSE 0 END UpdateInfo").Rows(0).Item("UpdateInfo")

        Dim sColumns As String
        If nUpdateNeeded = 1 Then
            LoginColumnTypes.FindAll(Function(l) l.IncludeSelectStatement = True).ForEach(
                Sub(l)
                    sColumns &= l.ColumnName & " = (SELECT " & l.ColumnName & " FROM " & DV_USERINFO_V & " UI WHERE UI.Username = MT.Username),"
                End Sub)

            sColumns = Left(sColumns, sColumns.Length - 1)

            WriteLine("UPDATE MT SET " & sColumns & " FROM " & sTableReference & " MT")
            ' ExecuteNonQuery("UPDATE SET " & sColumns & " FROM " & sTableReference & " MT")

            If Common.Webpages.Backend.Main.getbackendoption(S_BACKEND_OPTION_ARCHIVE_VIEW) Then
                WriteLine("UPDATE MT SET " & sColumns & " FROM " & Replace(sTableReference, GetAncillaryProject("SQLMainTableName"), "Archive_" & GetAncillaryProject("SQLMainTableName")) & " MT")
                '    ExecuteNonQuery("UPDATE SET " & sColumns & " FROM " & Replace(sTableReference, GetAncillaryProject("SQLMainTableName"), "Archive_" & GetAncillaryProject("SQLMainTableName")) & " MT")
            End If
        End If
    End Sub


End Class

