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
Imports WhitTools.Utilities
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
'Imports Common.Webpages.Validation
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.Webpages.Backend.Search
Imports Common.General.ProjectOperations
Imports Common.BuildSetup
Imports Common.ProjectFiles
Imports Common.General.Controls
Imports Common.General.Links
Imports Common.General.Pages
Imports Common.Webpages.ControlContent.Main
Imports Common.Webpages.Frontend.Main
Imports Common.Copy
Imports System.Threading
Imports System.Reflection
Imports Common
Imports Common.General
Imports Common.General.Ancillary
Imports Common.General.ControlTypes
Imports Common.Webpages.ControlContent.Attributes
Imports Common.General.Repeaters
Imports Common.Webpages.ControlContent
Imports CsQuery
Imports CsQuery.ExtensionMethods
Imports NLog.Config
Imports NLog.Targets
Imports System.Web.Services
Imports System.Collections
Imports System.Web.UI.WebControls

Partial Public Class finalizenewer
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        isSearch = False
        Session("ProjectID") = ""
        CreateLoginColumnTypes()
        Dim whittoolsclasses = GetWhitToolsClasses()

        If Not Page.IsPostBack Then
            BindData()
        End If

        EliminateNull(projectDT)
        GetTopLevelControls()

        pageNumber = -1

        'db.Projects.ToList().ForEach(Function(project)
        '                                 Dim lastBuild = project.ProjectBuilds.OrderByDescending(Function(build) build.ID).FirstOrDefault()

        '                                 If lastBuild IsNot Nothing Then
        '                                     lastBuild.ProjectBuildBackendAncillaryMaintenances.ToList().ForEach(
        '                                     Sub(ancillaryMaintenance)
        '                                         Dim ancillaryOptions = ancillaryMaintenance.ProjectAncillaryMaintenance.Project.ProjectBackendOptions.Where(Function(backendoption) backendoption.BackendOptionType.Display = 1 And backendoption.BackendOptionType.IndividualPage = 1)


        '                                         'Dim optionsToAdd = ancillaryOptions.Select(Function(ancillaryOption) New ProjectBuildBackendOption() With {.BuildID = lastBuild.ID, .ProjectID = ancillaryMaintenance.ProjectAncillaryMaintenance.Project.ID, .BackendOptionTypeID = ancillaryOption.BackendOptionType.ID}).ToList()
        '                                         'db.ProjectBuildBackendOptions.AddRange(optionsToAdd)

        '                                         db.ProjectBuildBackendOptions.Add(New ProjectBuildBackendOption() With {.BuildID = lastBuild.ID, .ProjectID = ancillaryMaintenance.ProjectAncillaryMaintenance.Project.ID, .BackendOptionTypeID = 16})
        '                                         db.ProjectBuildBackendOptions.Add(New ProjectBuildBackendOption() With {.BuildID = lastBuild.ID, .ProjectID = ancillaryMaintenance.ProjectAncillaryMaintenance.Project.ID, .BackendOptionTypeID = 17})
        '                                     End Sub)
        '                                 End If
        '                             End Function)

        'db.SaveChanges()
    End Sub


    Public Shared copiedControls As Boolean = False
    Sub NotifyMissingQuestions()
        Dim questions As DataTable
        Dim facnx As SqlConnection = CreateSQLConnection("adFinancialAid")
        Dim count As Integer
        Dim mailBody As String

        For Each currentRow As DataRow In GetDataTable("select * from ScholarshipApplications where OldCertificationDate IS NULL AND CertificationDate >= '2015-11-25 15:54:12.133' AND NOT ID IN (SELECT ForeignID FROM ScholarshipApplicationAdditionalQuestionResponses)", facnx).Rows
            questions = GetDataTable("exec [usp_GetScholarshipApplicationAdditionalQuestions] '" & currentRow.Item("Username") & "'", facnx, True, "-1", "", False, True, True, False, False)

            If questions.Rows.Count > 0 Then
                ExecuteNonQuery("Update ScholarshipApplications SET Certification='0', OldCertificationDate = CertificationDate, Section2Complete='0' WHERE ID = " & currentRow.Item("ID"), facnx)
                mailBody = "<p>Dear " & currentRow.Item("FirstName") & ":</p>I am sorry to inform you that there has been an error in the transmission of your returning student online scholarship application.  As a result of this, a small number of your application answers were not successfully transmitted.  The question(s) that were not transmitted are as follows:"
                mailBody &= "<ul>"

                For Each currentQuestion As DataRow In questions.Rows
                    mailBody &= "<li>" & currentQuestion.Item("Text") & "</li>"
                Next

                mailBody &= "</ul>"

                mailBody &= "<p>You may use the link below to log in to your application and submit your answer(s) to these question(s).  You will not have to re-enter any information other than the question(s) mentioned above as the rest of your application was received successfully.  Once you have written your answer(s) and clicked the Submit button at the bottom of the page, the process is complete.  Please do your best to submit your answer(s) as quickly as you can because they may help you qualify for financial aid you would not have received otherwise.</p>"
                mailBody &= "<p><a href='https://www.whitworth.edu/Administration/FinancialAid/Forms/ScholarshipApplicationReturning/section2.aspx'>Scholarship Application</a></p>"
                mailBody &= "<p>I offer my sincere apologies for any work you may have to do to recreate your answer(s).  Please contact me if you encounter any difficulties filling out the form or do not see the question(s) mentioned above listed on the form.</p>"
                mailBody &= "<p>Tom Ryan</p>"
                mailBody &= "<p>Senior Web Programmer/Analyst, Information Systems</p>"
                mailBody &= "<p>Whitworth University</p>"
                mailBody &= "<p>300 W. Hawthorne Road</p>"
                mailBody &= "<p>Spokane, WA 99251</p>"
                mailBody &= "<p>Phone: 509-777-4695</p>"
                mailBody &= "<p>Fax: 509-777-3786</p>"
                mailBody &= "<p><a href=""http://www.whitworth.edu/"">www.whitworth.edu</a></p>"

                'mailbody &= "Update ScholarshipApplications SET Certification='0', OldCertificationDate = CertificationDate, CertificationDate = NULL, Section2Complete='0' WHERE ID = " & currentRow.item("ID")  & "<br />"
                'mailbody &= currentrow.Item("Username")

                SendEmail(currentRow.Item("Email"), "Whitworth Scholarship Application - Missing Answers", mailBody, "tryan@whitworth.edu", "", "tryan@whitworth.edu")

                'exit for
            End If
        Next
    End Sub

    Private Sub ImportFacultyReportData()

        Dim adcnx As SqlConnection = CreateSQLConnection("AcademicDepartments")
        Dim reportSQL, answerSQL As String
        Dim currentReportID, currentCategoryID As Integer

        For Each currentrow As DataRow In GetDataTable("SELECT * FROM FacultyActivityReport where username='kgrieves' and complete='0' order by DateSubmitted asc", adcnx).Rows
            With currentrow
                reportSQL = "INSERT INTO facultyScholarshipReports "
                reportSQL &= "(IDNumber,FirstName,LastName,Email,Class,"
                reportSQL &= "SendReportToDepartmentChair,SendReportToEmail,Username,CertificationDate, DateSubmitted) "

                reportSQL &= "SELECT (SELECT top 1 IDNumber FROM adTelephone.dbo.UserInfo_v WHERE Username = FacultyActivityReport.Username),"
                reportSQL &= "FirstName,LastName,(SELECT top 1 Email FROM adTelephone.dbo.UserInfo_v WHERE Username = FacultyActivityReport.Username)"
                reportSQL &= ",(SELECT top 1 Class FROM adTelephone.dbo.UserInfo_v WHERE Username = FacultyActivityReport.Username),"
                reportSQL &= "0,0,Username,DateSubmitted, DateSubmitted"
                reportSQL &= " FROM FacultyActivityReport WHERE ID = " & currentrow.Item("ID")

                ExecuteNonQuery(reportSQL, adcnx)


                currentReportID = ExecuteScalar("SELECT Ident_current('FacultyScholarshipReports')", adcnx)

                For Each currentrow2 As DataRow In GetDataTable("Select * FROM FacultyScholarshipReportAnswerTypeCategories", adcnx).Rows
                    ExecuteNonQuery("INSERT INTO FacultyScholarshipReportCategories (ReportID, Description, Instructions, CategoryID) VALUES (" & currentReportID & ",'" & CleanSQL(currentrow2.Item("Description")) & "','" & CleanSQL(currentrow2.Item("Instructions")) & "', " & currentrow2.Item("ID") & ")", adcnx)

                    currentCategoryID = ExecuteScalar("SELECT IDENT_CUrrent('FacultyScholarshipReportCategories')", adcnx)

                    answerSQL = "INSERT INTO FacultyScholarshipReportQuestions "
                    answerSQL &= "(ReportCategoryID, Answer, Description, Format, Example) "
                    answerSQL &= "select " & currentCategoryID & ", Answer, Description, Format, Example from FacultyActivityReportAnswerTypes AT "
                    answerSQL &= "left outer join FacultyActivityReportAnswers A "
                    answerSQL &= "ON AT.ID = A.AnswerID And A.ReportID = " & currentrow.Item("ID")
                    answerSQL &= "where Active=1 And CategoryID = " & currentrow2.Item("ID")

                    ExecuteNonQuery(answerSQL, adcnx)

                Next

                If .Item("Complete") = "1" Then
                    ExecuteNonQuery("Update FacultyScholarshipReports SET Section1Complete = '1', Section2Complete = '1', Certification='1', CertificationDate = getdate() WHERE ID = " & currentReportID, adcnx)
                    WriteLine("Update FacultyScholarshipReport SET Section1Complete = '1', Section2Complete = '1', Certification='1', CertificationDate = getdate() WHERE ID = " & currentReportID)
                Else
                    ExecuteNonQuery("Update FacultyScholarshipReports SET Section1Complete = '0', Section2Complete = '0', Certification='0', CertificationDate = null WHERE ID = " & currentReportID, adcnx)
                    WriteLine("Update FacultyScholarshipReport SET Section1Complete = '0', Section2Complete = '0', Certification='0', CertificationDate = null WHERE ID = " & currentReportID)
                End If
            End With
        Next
    End Sub

    Sub CreateDetails()
        Dim dtColumns As DataTable = GetDataTable("select * from information_schema.columns where TABLE_NAME='controltypedetails' AND NOT column_Name in ('ID','ControlID','RequirementsProfile','ControlType')", cnx)

        For Each Currentrow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLTYPEDETAILS, False).Rows
            For Each CurrentRow2 As DataRow In dtColumns.Rows
                If Not IsDBNull(Currentrow(CurrentRow2.Item("column_name"))) Then
                    ExecuteNonQuery("INSERT INTO ControlTypeDetailValues (ControlTypeID, DetailTypeID, Value) VALUES (" & Currentrow.Item("ControlID") & ",(SELECT ID FROM ControlTypeDetailTypes WHERE Name='" & CurrentRow2.Item("column_name") & "'),'" & CleanSQL(Currentrow(CurrentRow2.Item("column_name"))) & "')", cnx)
                    'WriteLine("INSERT INTO ControlTypeDetailValues (ControlTypeID, DetailTypeID, Value) VALUES (" & Currentrow.Item("ControlID") & ",(SELECT ID FROM ControlTypeDetailTypes WHERE Name='" & CurrentRow2.Item("column_name") & "'),'" & Currentrow(CurrentRow2.Item("column_name")) & "')")
                End If

            Next
        Next

    End Sub

    Sub GetProjectDetails()
        projectDT = GetDataTable(cnx, "Select P.*,  DB.Name as SQLDBName, S.Name as SQLServerName From " & DT_WEBRAD_PROJECTS & "  P left outer join " & DT_WEBRAD_SQLDATABASES & "  DB on P.SQLDatabase = DB.ID left outer join " & DT_WEBRAD_SQLSERVERS & " S on DB.ServerID = S.ID Where P.ID = " & GetProjectID(), False)
        projectDT = ConvertDataTableColumnTypes(projectDT)
        EliminateNull(projectDT)
    End Sub


    Sub BindData()
        GetProjectDetails()

        SetMainDetails()
        SetPageDetails()
        SetBackendOptions(cblBackendOptions, GetProjectID())
        SetAncillaryMaintenance()
        SetProjectBuildControls()

        GenerateLinks()
    End Sub

    Private Sub SetAncillaryMaintenance()
        Dim ancillaryMaintenanceProjects = currentProject.ProjectAncillaryMaintenances1.ToArray()

        rptAncillaryMaintenance.DataSource = ancillaryMaintenanceProjects
        rptAncillaryMaintenance.DataBind()

        For ncounter As Integer = 0 To ancillaryMaintenanceProjects.Count - 1
            SetBackendOptions(CType(rptAncillaryMaintenance.Items(ncounter).FindControl("cblBackendOptions"), CheckBoxList), ancillaryMaintenanceProjects(ncounter).AncillaryProjectID)
        Next

        'amList.ForEach(Function(ancillaryMaintenance)
        '                   SetBackendOptions()
        '               End Function)
    End Sub

    Private Sub SetBackendOptions(backendOptionsList As CheckBoxList, projectID As Integer)
        FillListData(backendOptionsList, GetDataTable($"SELECT * FROM {DT_WEBRAD_BACKENDOPTIONTYPES} WHERE Display=1 AND IndividualPage = 1 And ID IN (Select Type FROM {DT_WEBRAD_PROJECTBACKENDOPTIONS} WHERE ProjectID = {projectID}) ORDER BY Name ASC"), "Name", "ID", False)

        backendOptionsList.Items.Insert(0, New ListItem("Main listing page", N_BACKEND_OPTIONTYPE_MAINLISTING))
        backendOptionsList.Items(0).Selected = False
        backendOptionsList.Items.Insert(0, New ListItem("Update page", N_BACKEND_OPTIONTYPE_UPDATE))
        backendOptionsList.Items(0).Selected = False
    End Sub

    Private Sub SetPageDetails()
        FillListData(cblPages, GetDataTable("SELECT ID, CASE WHEN Purpose IS NULL THEN 'N/A' ELSE Purpose End Purpose FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & GetProjectID() & " ORDER BY ID ASC"), "Purpose", "ID", False)

        For nCounter As Integer = 0 To cblPages.Items.Count - 1
            cblPages.Items(nCounter).Text = "Page " & nCounter + 1 & " - " & cblPages.Items(nCounter).Text
        Next

        chkIncludeStatusPage.Visible = cblPages.Items.Count > 1
    End Sub

    Private Function SetMainDetails() As String
        Dim sDepartmentname As String = ""

        With projectDT.Rows(0)
            GetDepartmentInfo("", sDepartmentname, .Item("Department"), .Item("CustomDepartmentName"))

            lblPageTitle.Text = .Item("PageTitle")
            lblEcommerce.Text = If(IseCommerceProject(), "Y", "N")

            lblSQLServer.Text = .Item("SQLServerName")
            SQLServerName = .Item("SQLServerName")
            lblSQLDatabase.Text = .Item("SQLDBName")
            SQLDatabaseName = .Item("SQLDBName")

            sqlcnx = CreateSQLConnection(SQLDatabaseName, SQLServerName)

            lblDepartmentName.Text = sDepartmentname
            lblSQLMainTableName.Text = .Item("SQLMainTableName")
            lblSQLInsertStoredProcedureName.Text = .Item("SQLInsertStoredProcedureName")
            lblSQLUpdateStoredProcedureName.Text = .Item("SQLUpdateStoredProcedureName")
            lblControlList.Text = GetListofValues("select case when Heading  = '' then Name + ' (' + Prefix + ')' else Heading + ' (' + Prefix + ')' end ControlDetail from " & DT_WEBRAD_PROJECTCONTROLS & " C left outer join " & DT_WEBRAD_CONTROLTYPES & "  T on C.ControlType = T.ID  left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  D on T.DataType = D.ID Where C.ProjectID = " & GetProjectID() & " Order by position asc", "ControlDetail")
        End With

        Return sDepartmentname
    End Function

    Sub ShowAncillaryMaintenance()
        Try
            pnlAncillaryMaintenance.Visible = cblBackendOptions.Items.FindByText("Ancillary maintenance").Selected
        Catch ex As Exception

        End Try
    End Sub

    Sub GetDepartmentInfo(ByRef sDepartmentLink As String, ByRef sDepartmentName As String, ByVal nDepartment As Integer, Optional ByVal sCustomDepartmentName As String = "")
        Dim dt As New DataTable
        dt = GetDataTable(cnx, "Select * from web3.Communications.dbo.ARA_Departments Where ID = " & nDepartment)

        If dt.Rows.Count > 0 Then
            With dt.Rows(0)
                Dim sHomepageURL As String

                If projectLocation = "Frontend" Then
                    sHomepageURL = .Item("HomepageURL")
                Else
                    sHomepageURL = .Item("HomepageURL2")
                End If

                If sHomepageURL <> "" Then
                    sDepartmentLink = Replace(GetFormattedLink(sHomepageURL, .Item("Department")), "<a ", "<a target=""_blank"" ")
                End If

                sDepartmentName = If(sCustomDepartmentName <> "", sCustomDepartmentName, .Item("Department"))
            End With
        End If
    End Sub

    Protected Sub rblProjectType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblProjectType.SelectedIndexChanged
        projectType = rblProjectType.SelectedValue
        GenerateLinks()
        'SaveBuild()
    End Sub

    Sub GenerateLinks()
        With projectDT.Rows(0)
            Dim frontendLink As DirectoryLink = New FrontendLink(.Item("FrontendPath"), .Item("FrontendLink"), rblProjectType.SelectedValue, .Item("RequireLogin"), GetPageCount() > 1)
            lblFrontendLink.Text = frontendLink.GenerateLink()

            Dim backendLink As DirectoryLink = New BackendLink(.Item("BackendPath"), .Item("BackendLink"), rblProjectType.SelectedValue, .Item("RequireLogin"), GetPageCount() > 1)
            lblBackendLink.Text = backendLink.GenerateLink()
        End With
    End Sub

    Protected Sub cblBackendOptions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cblBackendOptions.SelectedIndexChanged
        ShowAncillaryMaintenance()
    End Sub



    Protected Sub chkBackendOptionsAll_CheckedChanged(sender As Object, e As EventArgs)
        For Each currentItem In cblBackendOptions.Items
            currentItem.Selected = chkBackendOptionsAll.Checked
        Next

        ShowAncillaryMaintenance()
    End Sub

    Protected Sub chkAncillaryMaintenanceAll_CheckedChanged(sender As Object, e As EventArgs)
        For Each currentItem As RepeaterItem In rptAncillaryMaintenance.Items
            CType(currentItem.FindControl("chkSelect"), CheckBox).Checked = chkAncillaryMaintenanceAll.Checked
            chkSelect_CheckedChanged(CType(currentItem.FindControl("chkSelect"), CheckBox), Nothing)
        Next
    End Sub

    Protected Sub chkBackendOptionsAll_CheckedChanged1(sender As Object, e As EventArgs)

    End Sub

    Protected Sub chkBOAll_CheckedChanged(sender As Object, e As EventArgs)
        With GetParentRepeaterItem(sender)
            For Each currentItem As ListItem In CType(.FindControl("cblBackendOptions"), CheckBoxList).Items
                currentItem.Selected = CType(sender, CheckBox).Checked
            Next
        End With
    End Sub

    Protected Sub chkSelect_CheckedChanged(sender As Object, e As EventArgs)
        ShowRepeaterBackendOptions(sender)
    End Sub


    Sub SetProjectBuildControls()
        Dim dtProjectBuild = GetLatestProjectBuild()

        If dtProjectBuild IsNot Nothing Then
            With dtProjectBuild
                chkCreateFrontend.Checked = .Frontend
                chkCreateBackend.Checked = .Backend
                chkIncludeStatusPage.Checked = .IncludeStatusPage

                SetItemSelected(rblProjectType, .Type)
                SetItemSelected(rblFormsType, .FormsType)

                If cblPages.Items.Count > 0 Then
                    Dim dtPages As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTBUILDPAGES & " WHERE BuildID = " & .ID & " ORDER BY ID DESC")

                    SetListControlItemSelected(cblPages, "", True, dtPages, "PageID")
                End If

                If cblBackendOptions.Items.Count > 0 Then
                    Dim dtBackendOptions As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTBUILDBACKENDOPTIONS & " WHERE ProjectID = " & currentProject.ID & " AND BuildID = " & .ID & " ORDER BY ID DESC")

                    SetListControlItemSelected(cblBackendOptions, "", True, dtBackendOptions, "BackendOptionTypeID")

                    SetAncillaryMaintenance(.ID)
                Else
                    ShowAncillaryMaintenance()
                End If
            End With
        Else
            For nCounter As Integer = 0 To cblPages.Items.Count - 1
                cblPages.Items(nCounter).Selected = True
            Next

            For Each currentitem As ListItem In cblBackendOptions.Items
                currentitem.Selected = True
            Next
        End If

    End Sub

    Sub SetAncillaryMaintenance(ByVal buildID As Integer)
        Dim ancillaryMaintenance As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTBUILDBACKENDANCILLARYMAINTENANCE & " WHERE BuildID = " & buildID & " ORDER BY ID DESC")

        For Each currentRow As DataRow In ancillaryMaintenance.Rows
            For Each currentItem As RepeaterItem In CType(GetPageControlReference("rptAncillaryMaintenance"), Repeater).Items
                If CType(currentItem.FindControl("lblAncillaryProjectMaintenanceID"), Label).Text = currentRow.Item("AncillaryMaintenanceID") Then
                    CType(currentItem.FindControl("chkSelect"), CheckBox).Checked = True
                    ShowRepeaterBackendOptions(CType(currentItem.FindControl("chkSelect"), CheckBox))

                    Dim dtBackendOptions As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTBUILDBACKENDOPTIONS & " WHERE BuildID = " & buildID & " and ProjectID = " & CType(currentItem.FindControl("lblAncillaryProjectID"), Label).Text & " ORDER BY ID DESC")

                    SetListControlItemSelected(CType(currentItem.FindControl("cblBackendOptions"), CheckBoxList), "", True, dtBackendOptions, "BackendOptionTypeID")

                    Dim selectedCount As Integer = 0

                    For Each optionItem As ListItem In CType(currentItem.FindControl("cblBackendOptions"), CheckBoxList).Items
                        If optionItem.Selected Then
                            selectedCount += 1
                        End If
                    Next

                    If selectedCount = CType(currentItem.FindControl("cblBackendOptions"), CheckBoxList).Items.Count Then
                        CType(currentItem.FindControl("chkBOAll"), CheckBox).Checked = True

                    End If
                End If
            Next
        Next

        ShowAncillaryMaintenance()
    End Sub

End Class



