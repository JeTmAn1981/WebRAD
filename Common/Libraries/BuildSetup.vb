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
Imports WhitTools.RulesAssignments
Imports WhitTools.Workflow
Imports Common.General.Main
Imports Common.General.ProjectOperations
Imports Common.General.Variables
Imports System.Threading
Imports System.Reflection
Imports Common.Webpages.Backend.Search
Imports WhitTools.Utilities

    Public Class BuildSetup
        Shared Sub GetProjectDetails()
        projectDT = GetDataTable("Select P.*,  DB.Name as SQLDBName, S.Name as SQLServerName From " & DT_WEBRAD_PROJECTS & "  P left outer join " & DT_WEBRAD_SQLDATABASES & "  DB on P.SQLDatabase = DB.ID left outer join " & DT_WEBRAD_SQLSERVERS & " S on DB.ServerID = S.ID Where P.ID = " & GetProjectID(), False)
        projectDT = ConvertDataTableColumnTypes(projectDT)
        EliminateNull(projectDT)
    End Sub

    Shared Sub GetTopLevelControls()
        dim projectID As Integer = GetProjectID()

        controlsDT = GetDataTable("Select * FROM " & DT_TOPLEVELPROJECTCONTROLS_V & " WHERE ProjectID = " & projectID & " AND NOT ControlType IS NULL Order by PageID asc, Position asc")
        SetSearchControls(projectID)
    End Sub


    Public Shared Sub SetImports()
        currentImportListing = GetImports()
    End Sub

    Private Shared Function GetImports() As String
        Dim importListing As String = ""

        AddSystemImports(importListing)
        importListing &= GetWhitToolsImports() & vbCrLf

        For Each currentRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " WHERE Type = " & N_ADDITIONALOPERATIONTYPE_IMPORTS & " AND PageID IN (SELECT ID FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & GetProjectID() & ")").Rows
            importListing &= currentRow.Item("AdditionalOperations") & vbCrLf
        Next

        Return importListing
    End Function

    Public Shared Function GetWhitToolsImports() As String
        Dim whitToolsImports = ""

        Dim excludedClasses As New List(Of String)(New String() {
                                                   "RoomContainer",
                                                   "Coll_CurrentRoomAssignment_Info_V",
                                                   "RoomAssignmentAdditionalInfo",
                                                   "DormOccupancy"})


        Dim WhitClasses() As Type = GetWhitToolsClasses()
        Dim hasInterfaces As List(Of Type) = New List(Of Type)()

        For Each currentClass As Type In WhitClasses
            If Not currentClass.FullName.Contains("$") And Not currentClass.FullName.Contains("+") And Not excludedClasses.Contains(currentClass.Name) Then
                whitToolsImports &= "Imports " & currentClass.FullName & vbCrLf
            End If
        Next

        Return whitToolsImports
    End Function

    Private Shared Sub AddSystemImports(ByRef importListing As String)
        importListing &= "Imports System.DateTime" & vbCrLf
        importListing &= "Imports System.Data" & vbCrLf
        importListing &= "Imports System.Data.SqlClient" & vbCrLf
        importListing &= "Imports System.IO" & vbCrLf
        importListing &= "Imports System.Web.Mail" & vbCrLf
        importListing &= "Imports System" & vbCrLf
        importListing &= "Imports System.Text.RegularExpressions" & vbCrLf
        importListing &= "Imports System.Web.Script.Services" & vbCrLf
        importListing &= "Imports System.Web.Services" & vbCrLf
        importListing &= "Imports System.Web.UI.WebControls" & vbCrLf
        importListing &= "Imports System.Linq" & vbCrLf
        importListing &= "Imports System.Collections.Generic" & vbCrLf
        importListing &= "Imports Microsoft.VisualBasic" & vbCrLf
        importListing &= "Imports Common" & vbCrLf
        importListing &= "Imports ClosedXML" & vbCrLf
        importListing &= "Imports WhitTools" & vbCrLf
    End Sub

    Shared Sub setProjectBuildVariables()
        Dim dtProjectBuild = GetLatestProjectBuild()

        If dtProjectBuild IsNot Nothing Then
            currentProjectBuild = dtProjectBuild

            With dtProjectBuild
                createFrontend = .Frontend
                createBackend = .Backend

                projectType = .Type
                isMVC = (.FormsType = "MVC")

                If projectType = "Test" Then
                    SQLServerName = "web3"
                    SQLDatabaseName = "Test2"
                    sqlcnx = CreateSQLConnection("Test2")
                End If

                pages = New List(Of ListItem)

                Dim currentItem As ListItem

                For Each currentRow As DataRow In GetDataTable("select ID, CASE WHEN (SELECT COUNT(*) FROM " & DT_WEBRAD_PROJECTBUILDPAGES & " where PageID = PP.ID AND buildid = " & .ID & ") > 0 then 'True' else 'False' end Selected from " & DT_WEBRAD_PROJECTPAGES & " PP where projectid=" & GetProjectID()).Rows
                    currentItem = New ListItem(currentRow.Item("ID"))
                    currentItem.Selected = currentRow.Item("Selected")
                    pages.Add(currentItem)
                Next
            End With
        End If
    End Sub

    Shared Function GetLatestProjectBuild() As ProjectBuild
        Dim builds = db.Projects.ToList().First(Function(project) project.ID = GetProjectID()).ProjectBuilds
        Return db.Database.SqlQuery(Of ProjectBuild)("SELECT TOP 1 * FROM ProjectBuilds Order By ID desc").FirstOrDefault

        Return currentProject.ProjectBuilds.OrderByDescending(Function(projectBuild) projectBuild.ID).FirstOrDefault
    End Function

    Public Shared Sub ShowRepeaterBackendOptions(sender As Object)
        With GetParentRepeaterItem(sender)
            CType(.FindControl("pnlShowBackendOptions"), Panel).Visible = CType(sender, CheckBox).Checked
        End With
    End Sub

    Public shared Sub ShowAncillaryMaintenance()
        Try
            CType(GetPageControlReference("pnlAncillaryMaintenance"),Panel).Visible = CType(GetPageControlReference("cblBackendOptions"),checkboxlist).Items.FindByText("Ancillary maintenance").Selected
        Catch ex As Exception

        End Try
     End Sub
    End Class
