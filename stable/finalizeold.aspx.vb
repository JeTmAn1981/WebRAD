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
Imports Common.General.ProjectOperations
Imports Common.BuildSetup
Imports Common.General.Controls
Imports Common.General.Links
Imports Common.General.Pages
Imports Common.Webpages.ControlContent.Main
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

Partial Public Class finalize
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        
        Session("ProjectID") = ""
        CreateLoginColumnTypes()

        If Not Page.IsPostBack Then
            BindData()
        End If

        EliminateNull(projectDT)
        GetTopLevelControls()
        SetWhitToolsImports()

        'dim homephone,cellphone as string

        'Dim counter as integer = 1
        'for each currentrow as DataRow in getdatatable("select * from adadmissions.dbo.irdfix").Rows
        '    with currentrow
        '        homephone = FormatStringStripNonNumbers(.Item("HomePhone"),true)
        '        cellphone = FormatStringStripNonNumbers(.Item("MobilePhone"),true)
        '    End With


        '                    writeline($"homephone: {homephone}, cellphone:{cellphone}")

        '    homephone = getdatatable($"SELECT top 1 * FROM adadmissions.dbo.applicants where username='{currentrow.item("Email")}' ORDER BY oaID desc").rows(0).item("HomePhone")
        '    cellphone = getdatatable($"SELECT top 1 * FROM adadmissions.dbo.applicants where username='{currentrow.item("Email") }' ORDER BY oaID desc").rows(0).item("CellPhone")

        '    executenonquery($"UPDATE adadmissions.dbo.irdfix set homephone=master.dbo.ufn_formatphone('{homephone}'), mobilephone=master.dbo.ufn_formatphone('{cellphone}') where email='{currentrow.item("Email")}'")
        '    writeline($"fixed - homephone: {homephone}, cellphone:{cellphone}")
        'Next

        'common.Webpages.Main.DoWebpageCreationSetup()
        'Dim sContent as String
        'bnew = true
        'dim controldt as datatable = getdatatable($"SELECT * FROM { DT_TOPLEVELPROJECTCONTROLS_V} WHERE ID = 27621")

        'writeline(NoParentControl(controldt.Rows(0).Item("ParentControlID")) And ControlWriteAllowed(controldt.Rows(0)))
        ''call new ContentWriter(controldt.Rows(0)).GetControlContent(sContent)
        'writetextfile(scontent,"controltest.txt")
        'writeline(getdatatable("select * from projectcontrols where ProjectID=286  and id in (27527,27528,27531)",cnx).rows.count)
        'dim controls as new Hashtable()
        'For each currentrow as datarow in getdatatable("select * from projectcontrols where ProjectID=286  and id in (27527,27528,27531)",cnx).Rows
        '    writeline(currentrow.Item("ID"))
        '    copycontrol(currentrow,287,356,new Hashtable())
        'Next
        'writeline(ControlUsesFormGroup(4081))
        'dim scontent as string
        'dim cw = New ContentWriter(getdatatable("SELECT * FROM TopLevelProjectControls_v WHERE ID = 4081",cnx).Rows(0))
        '                    cw.GetControlContent(sContent)
        'WriteTextFile(scontent,"controltest.txt")
        'common.sql.main.CreateScheduleSQL()
        'Dim navigation as string
        'Webpages.Backend.Navigation.CreateMainNavigation()
        'writetextfile(Webpages.Backend.Navigation.mainNavigation,"navigation.txt")

            'Dim si as System.Diagnostics.Process = new System.Diagnostics.Process()
            'si.StartInfo.WorkingDirectory = "c:\\"
            'si.StartInfo.UseShellExecute = false
            'si.StartInfo.FileName = "robocopy.exe"
            'si.StartInfo.Arguments = "\\web1\c$\test\ \\web2\c$\ /E"
            'si.StartInfo.CreateNoWindow = true
            'si.StartInfo.RedirectStandardInput = true
            'si.StartInfo.RedirectStandardOutput = true
            'si.StartInfo.RedirectStandardError = true
            'si.Start()
            'dim  output as string= si.StandardOutput.ReadToEnd()
            'si.Close()
            'Response.Write(output)

        pageNumber = -1

        'dim content As string
        'dim cw as ContentWriter = New ContentWriter(getdatatable($"SELECT * FROM {DT_TOPLEVELPROJECTCONTROLS_V} WHERE ID = 28790").Rows(0))
        'cw.GetControlContent(content)
        'writetextfile(content,"content.txt")
        'writeline(GetControlColumnValue(-17,"ControlType"))
        'Dim saveAncillary As string
        'ancillary.SaveAncillaryUploadFiles(getdatatable($"SELECT * FROM {DT_TOPLEVELPROJECTCONTROLS_V} WHERE ID = 28808").Rows(0),saveAncillary,"nCurrentID")
        'writeline(saveAncillary)
        'ParentIsRepeaterControl(-17)
               
        'for each currentrow as DataRow In getdatatable($"SELECT * FROM {DT_TOPLEVELPROJECTCONTROLS_V} WHERE ID IN (28911,28912,28913,28914,28933)").Rows
        '    CopyControl(currentrow, 323, 388, new Hashtable())
        'Next

        'Dim body as String = GetBodyContent()
        'writetextfile(body,"body.txt")
        
    End Sub
    
    Sub NotifyMissingQuestions()
        dim questions as datatable
        dim facnx as sqlconnection = createsqlconnection("adFinancialAid")
        dim count as integer
        dim mailBody as string

        for each currentRow as datarow in getdatatable("select * from ScholarshipApplications where OldCertificationDate IS NULL AND CertificationDate >= '2015-11-25 15:54:12.133' AND NOT ID IN (SELECT ForeignID FROM ScholarshipApplicationAdditionalQuestionResponses)", facnx).rows
            questions = GetDataTable("exec [usp_GetScholarshipApplicationAdditionalQuestions] '" & currentRow.item("Username") & "'",facnx, True, "-1", "", False, True, True, False,false)

            If questions.rows.count > 0
                executenonquery("Update ScholarshipApplications SET Certification='0', OldCertificationDate = CertificationDate, Section2Complete='0' WHERE ID = " & currentRow.item("ID"),facnx)
                mailBody = "<p>Dear " & currentrow.Item("FirstName") & ":</p>I am sorry to inform you that there has been an error in the transmission of your returning student online scholarship application.  As a result of this, a small number of your application answers were not successfully transmitted.  The question(s) that were not transmitted are as follows:"        
                mailBody &= "<ul>"

                for each currentQuestion as datarow in questions.Rows
                    mailBody &= "<li>" & currentquestion.Item("Text") & "</li>"
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

                SendEmail(currentrow.item("Email"),"Whitworth Scholarship Application - Missing Answers",mailBody,"tryan@whitworth.edu","","tryan@whitworth.edu")

                'exit for
            End If
        Next
        End Sub

    Private Sub ImportFacultyReportData()

        dim adcnx as SqlConnection = CreateSQLConnection("AcademicDepartments")
        dim reportSQL,answerSQL as String
        dim currentReportID, currentCategoryID as Integer

        for each currentrow as datarow in getdatatable("SELECT * FROM FacultyActivityReport where username='kgrieves' and complete='0' order by DateSubmitted asc",adcnx).Rows
            with currentrow
                reportSQL = "INSERT INTO facultyScholarshipReports "
                reportSQL &= "(IDNumber,FirstName,LastName,Email,Class,"
                reportSQL &= "SendReportToDepartmentChair,SendReportToEmail,Username,CertificationDate, DateSubmitted) "

                reportSQL &= "SELECT (SELECT top 1 IDNumber FROM adTelephone.dbo.UserInfo_v WHERE Username = FacultyActivityReport.Username),"
                reportSQL &= "FirstName,LastName,(SELECT top 1 Email FROM adTelephone.dbo.UserInfo_v WHERE Username = FacultyActivityReport.Username)"
                reportSQL &= ",(SELECT top 1 Class FROM adTelephone.dbo.UserInfo_v WHERE Username = FacultyActivityReport.Username),"
                reportSQL &= "0,0,Username,DateSubmitted, DateSubmitted"
                reportSQL &= " FROM FacultyActivityReport WHERE ID = " & currentrow.Item("ID")

                executenonquery(reportSQL,adcnx)
                WriteLine(reportSQL)

                currentReportID = Executescalar("SELECT Ident_current('FacultyScholarshipReports')",adcnx)

                for each currentrow2 as DataRow in getdatatable("Select * FROM FacultyScholarshipReportAnswerTypeCategories",adcnx).Rows
                    writeline("INSERT INTO FacultyScholarshipReportCategories (ReportID, Description, Instructions, CategoryID) VALUES (" &  currentReportID & ",'" & cleansql(currentrow2.Item("Description")) & "','" & cleansql(currentrow2.Item("Instructions")) & "', " & currentrow2.Item("ID") & ")")
                    executenonquery("INSERT INTO FacultyScholarshipReportCategories (ReportID, Description, Instructions, CategoryID) VALUES (" &  currentReportID & ",'" & cleansql(currentrow2.Item("Description")) & "','" & cleansql(currentrow2.Item("Instructions")) & "', " & currentrow2.Item("ID") & ")" ,adcnx)

                    currentCategoryID = ExecuteScalar("SELECT IDENT_CUrrent('FacultyScholarshipReportCategories')",adcnx)

                    answerSQL = "INSERT INTO FacultyScholarshipReportQuestions "
                    answerSQL &= "(ReportCategoryID, Answer, Description, Format, Example) "
                    answerSQL &= "select " & currentcategoryid & ", Answer, Description, Format, Example from FacultyActivityReportAnswerTypes AT "
                    answerSQL &= "left outer join FacultyActivityReportAnswers A "
                    answerSQL &= "ON AT.ID = A.AnswerID And A.ReportID = " & currentrow.Item("ID")
                    answerSQL &= "where Active=1 And CategoryID = " & currentrow2.Item("ID")

                    ExecuteNonQuery(answerSQL,adcnx)
                    writeline(answerSQL)
                Next

                if .Item("Complete") = "1"
                    ExecuteNonQuery("Update FacultyScholarshipReports SET Section1Complete = '1', Section2Complete = '1', Certification='1', CertificationDate = getdate() WHERE ID = " & currentreportid,adcnx)
                    writeline("Update FacultyScholarshipReport SET Section1Complete = '1', Section2Complete = '1', Certification='1', CertificationDate = getdate() WHERE ID = " & currentreportid)
                else
                    ExecuteNonQuery("Update FacultyScholarshipReports SET Section1Complete = '0', Section2Complete = '0', Certification='0', CertificationDate = null WHERE ID = " & currentreportid,adcnx)
                    writeline("Update FacultyScholarshipReport SET Section1Complete = '0', Section2Complete = '0', Certification='0', CertificationDate = null WHERE ID = " & currentreportid)
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
        Dim sDepartmentname As String = ""

        GetProjectDetails()

        With projectDT.Rows(0)
            GetDepartmentInfo("", sDepartmentname, .Item("Department"), .Item("CustomDepartmentName"))

            lblPageTitle.Text = .Item("PageTitle")
            lblEcommerce.Text = If(IsECommerceProject(), "Y", "N")

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
            txtNotes.Text = .Item("Notes")
            End With

        FillListData(cblPages, GetDataTable("SELECT ID, CASE WHEN Purpose IS NULL THEN 'N/A' ELSE Purpose End Purpose FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & GetProjectID() & " ORDER BY ID ASC"), "Purpose", "ID", False)

        For nCounter As Integer = 0 To cblPages.Items.Count - 1
            cblPages.Items(nCounter).Text = "Page " & nCounter + 1 & " - " & cblPages.Items(nCounter).Text
        Next

        FillListData(cblBackendOptions, GetDataTable($"SELECT * FROM {DT_WEBRAD_BACKENDOPTIONTYPES} WHERE Display=1 AND IndividualPage = 1 And ID IN (Select Type FROM {DT_WEBRAD_PROJECTBACKENDOPTIONS} WHERE ProjectID = {GetProjectID()}) ORDER BY Name ASC"), "Name", "ID", False)
        cblBackendOptions.Items.Insert(0, New ListItem("Main listing page", N_BACKEND_OPTIONTYPE_MAINLISTING))
        cblBackendOptions.Items.Insert(0, New ListItem("Update page", N_BACKEND_OPTIONTYPE_UPDATE))


        setProjectBuildControls()
         GenerateLinks()        
    End Sub
    
    Sub SaveNotes()
        Dim cmd As New SqlCommand("usp_UpdateProjectNotes", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@ProjectID", GetProjectID())
        cmd.Parameters.AddWithValue("@Notes", txtNotes.Text)

        ExecuteNonQuery(cmd)
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
    End Sub

    Sub GenerateLinks()
        With projectDT.Rows(0)
            dim frontendLink as DirectoryLink = new FrontendLink(.Item("FrontendPath"),.Item("FrontendLink"),rblprojecttype.SelectedValue,.Item("RequireLogin"), GetPageCount() > 1)
            lblFrontendLink.Text = frontendLink.GenerateLink()
    
            dim backendLink as DirectoryLink = new BackendLink(.Item("BackendPath"),.Item("BackendLink"),rblprojecttype.SelectedValue,.Item("RequireLogin"), GetPageCount() > 1)
            lblBackendLink.Text = backendLink.GenerateLink()
        End With
    End Sub

End Class



