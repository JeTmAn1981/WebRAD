Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.Security
Imports System.Security.Principal
Imports System.IO
Imports System.Linq
Imports WhitTools.eCommerce
Imports WhitTools.Email
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.File
Imports WhitTools.Filler
Imports WhitTools.RulesAssignments
Imports WhitTools.DataTables
Imports WhitTools.Converter
Imports WhitTools.Validator
Imports WhitTools.ErrorHandler
Imports WhitTools.Formatter
Imports WhitTools.GlobalEnum
Imports WhitTools.Repeaters
'Imports WhitTools.SQL
Imports WhitTools.Encryption
Imports Common.SQL.StoredProcedure
Imports Common.Webpages.Main
Imports Common.General.Folders
Imports Common.General.Links
Imports Common.General.Ancillary
Imports Common.Webpages.Frontend.MultiPage
Imports Common.General.DataTypes
Imports Common.General.ControlTypes
imports Common.General.ProjectOperations
Imports Common.General.Pages
Imports Common.General.Repeaters
Imports Common.Webpages.Backend.Search
Imports Common.SQL.Main
Imports Common.General.Variables
'Imports Common
Imports WhitTools.Utilities

Namespace General
    Public Class Main
        Public Sub New()
            '            CreateLoginColumnTypes()
        End Sub

        Shared Function CheckSavedStatus(ByVal currentstatus As String, ByVal savedstatus As String) As Integer
            Try
                If savedstatus = "" Then
                    Return 1
                ElseIf currentstatus <> savedstatus Then
                    Return 2
                Else
                    Return 3
                End If
            Catch ex As Exception
                'Empty catch statement
            End Try
        End Function

        Shared Function GetCommonFunctions(ByVal SQLMainTable As String, ByVal requireLogin As String, ByVal multipleSubmissions As String, ByVal SQLAdditionalCertificationStatement As String, ByVal checkClosedMethod As String) As String
            Dim commonFunctions As String = ""

            commonFunctions &= "Public Const SESSION_USER_ID As String = """ & currentProject.GetProjectNameAlphaNumericOnly() & "_UserID""" & vbCrLf

            AddDepartmentBreadcrumbsMethod(commonFunctions)

            If GetPageCount() > 1 Then
                commonFunctions &= GetCheckApplicationFinishedMethod(SQLMainTable)
                commonFunctions &= GetCheckAlreadySubmittedMethod(SQLMainTable, multipleSubmissions, SQLAdditionalCertificationStatement)
                commonFunctions &= GetApplicationCopyMethod(SQLMainTable)
            End If

            If CurrentProjectRequiresLogin() Then
                commonFunctions &= GetCurrentUsernameOverload()
            End If


            currentProject.ProjectAncillaryMaintenances1.ToList().ForEach(Sub(ancillary)
                                                                              commonFunctions &= GetSubmissionColumnMethod(GetFormattedProjectName(False, ancillary.ShortName), ancillary.Project.SQLMainTableName)



                                                                          End Sub)

            commonFunctions &= GetSubmissionColumnMethod()

            If multipleSubmissions = "1" Then
                commonFunctions &= GetCheckReviewInformationMethod(SQLMainTable)
            End If

            commonFunctions &= checkClosedMethod
            commonFunctions &= GetApplicationIDMethod(SQLMainTable)
            commonFunctions &= GetGetAuthenticatorMethod()

            commonFunctions &= GetAdditionalOperations(currentProject)

            Return commonFunctions
        End Function

        Private Shared Function GetSubmissionColumnMethod(Optional ByVal projectName As String = "", Optional ByVal databaseTable As String = """ & MAIN_DATABASE_TABLE_NAME & """) As String
            Dim submissionColumnMethod As String = ""

            submissionColumnMethod &= "Public Shared Function Get" & projectName & "SubmissionColumn(ByVal ID As Integer, ByVal columnName As String) As String" & vbCrLf
            submissionColumnMethod &= "Return GetDataTable(""Select * FROM " & databaseTable & " WHERE ID = "" & ID, cnx).Rows(0).Item(columnName)" & vbCrLf
            submissionColumnMethod &= "End Function" & vbCrLf & vbCrLf

            Return submissionColumnMethod
        End Function

        Public Shared Function GetAdditionalOperations(project As Project) As String
            Dim additionalOperations As String = ""

            project.ProjectPages.ToList().ForEach(Sub(pp)
                                                      pp.ProjectAdditionalOperations.Where(Function(ao) ao.Type = N_ADDITIONALOPERATIONTYPE_COMMON).ToList().ForEach(Sub(ao)
                                                                                                                                                                         additionalOperations &= ao.AdditionalOperations
                                                                                                                                                                     End Sub)
                                                  End Sub)

            project.ProjectAncillaryMaintenances1.ToList().ForEach(Sub(ancillary)
                                                                       additionalOperations &= GetAdditionalOperations(db.Projects.FirstOrDefault(Function(p) p.ID = ancillary.AncillaryProjectID))
                                                                   End Sub)

            Return additionalOperations
        End Function

        Private Shared Function AddDepartmentBreadcrumbsMethod(ByRef sCommonFunctions As String) As String
            sCommonFunctions &= "Public Shared Sub AddDepartmentBreadcrumbs()" & vbCrLf

            AddDepartmentBreadcrumbLinks(sCommonFunctions)

            sCommonFunctions &= "End Sub" & vbCrLf

            Return sCommonFunctions
        End Function

        Public Shared Sub AddDepartmentBreadcrumbLinks(ByRef sCommonFunctions As String)
            Dim departmentBreadcrumbLinks As DataTable = GetDataTable($"SELECT * FROM {DT_ARA_DEPARTMENT_BREADCRUMBLINKS} WHERE DepartmentID = {projectDT.Rows(0).Item("Department")} Order By [Order] asc")

            If departmentBreadcrumbLinks.Rows.Count > 0 Then
                For Each currentRow As DataRow In departmentBreadcrumbLinks.Rows
                    AddFormLoadLink(sCommonFunctions, currentRow.Item("LinkName"), currentRow.Item("LinkURL"))
                Next
            Else
                AddFormLoadLink(sCommonFunctions, departmentName, departmentUrl)
            End If
        End Sub

        Shared Function GetUsernameReference(ByVal sRequireLogin As String) As String
            Return "Common.GetCurrentUsername()"
        End Function

        Shared Function GetDateSubmittedColumnReference() As String
            Return If(IsMultipageForm(), "CertificationDate", "DateSubmitted")
        End Function

        Shared Function GetListItemDefaultSelected(ByVal bInsert As Boolean, ByVal nRequired As Integer, ByVal sItemText As String, ByVal sItemValue As String) As String
            Return If(bInsert And nRequired = 1 And sItemValue = "0" And sItemText = "No", " Selected=""True""", "")
        End Function

        Shared Function GetQueryVariable(ByVal bSearch As Boolean) As String
            Return If(bSearch, "sSearchQuery", "sSelectQuery")
        End Function



        Shared Function GetCurrentUsernameOverload(ByVal nRequireLogin As Integer, ByVal bInsert As Boolean) As String
            Return If(nRequireLogin = "1" And Not bInsert, GetCurrentUsernameOverload(), "")
        End Function

        Shared Function GetCurrentUsernameOverload() As String
            Dim getUsernameMethod As String = ""

            getUsernameMethod &= "Shared Function GetCurrentUsername() As String" & vbCrLf

            If CurrentProjectRequiresWhitworthLogin() Then
                If projectType = "Test" Then
                    getUsernameMethod &= "      If True Then" & vbCrLf
                Else
                    getUsernameMethod &= "      If (bFrontend And" & GetAdminMembershipCheck() & ") Or Not bFrontend Then" & vbCrLf
                End If

                getUsernameMethod &= "            If GetQueryString(""Username"") = ""None"" Then" & vbCrLf
                getUsernameMethod &= "                SetSessionVariable(""Username"", """")" & vbCrLf
                getUsernameMethod &= "            ElseIf GetQueryString(""Username"") <> """" Then" & vbCrLf
                getUsernameMethod &= "                SetSessionVariable(""Username"", GetQueryString(""Username""))" & vbCrLf
                getUsernameMethod &= "            End If" & vbCrLf
                getUsernameMethod &= "" & vbCrLf
                getUsernameMethod &= "            If GetSessionVariable(""Username"") <> """" Then" & vbCrLf
                getUsernameMethod &= "                Return GetSessionVariable(""Username"")" & vbCrLf
                getUsernameMethod &= "            End If" & vbCrLf
                getUsernameMethod &= "        ElseIf Not bFrontend Then" & vbCrLf
                getUsernameMethod &= "            Return GetSessionVariable(""Username"")" & vbCrLf
                getUsernameMethod &= "        End If" & vbCrLf & vbCrLf
                getUsernameMethod &= "        Return WhitTools.Getter.GetCurrentUsername()" & vbCrLf
            Else
                getUsernameMethod &= "       Return GetSessionVariable(SESSION_USER_ID) " & vbCrLf
            End If

            getUsernameMethod &= "End Function" & vbCrLf

            Return getUsernameMethod
        End Function

        Shared Function GetSupervisorList() As String
            Return String.Join(",", currentProject.ProjectSupervisors.Where(Function(supervisor) supervisor.SupervisorType = "SingleUser").ToList().Select(Function(supervisor) """" & ExtractUsername(supervisor.SupervisorName) & """"))
        End Function

        Shared Function GetCustomScript() As String
            Dim sScript As String = ""

            For Each CurrentRow As DataRow In GetDataTable("Select * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " And Onchange = 1 And OnchangeBody <> '' ORDER BY Position asc").Rows
                sScript &= CurrentRow.Item("OnchangeBody") & vbCrLf & vbCrLf
            Next

            For Each CurrentRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " AND ControlType = " & N_REPEATER_CONTROL_TYPE & " AND LayoutType = " & S_LAYOUTTYPE_HORIZONTAL & "  ORDER BY Position asc").Rows
                sScript &= "ShowTablesawHeader('" & CurrentRow.Item("Name") & "');" & vbCrLf
            Next

            sScript &= "RemoveNegativeOnes();" & vbcrlf

            Return sScript
        End Function

        Public Shared Sub UpdateControlNames()
            UpdateProhibitedControlNames()
            'UpdateRedundantControlNames()
        End Sub

        Public Shared Sub UpdateRedundantControlNames()
            Dim dtRedundantNames As DataTable = GetDataTable("SELECT * FROM (select Name, COUNT(name) as namecount from " & DT_WEBRAD_PROJECTCONTROLS & " where ProjectID='" & GetProjectID() & "' group by name) as ControlNames where namecount > 1")
            Dim dtControls As DataTable
            Dim nAddIndex As Integer


            For Each currentRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID()).Rows
                If GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " AND Name = '" & WhitTools.SQL.CleanSQL(currentRow.Item("Name")) & "' AND NOT ID = " & currentRow.Item("ID")).Rows.Count > 0 Then

                End If
            Next

            For Each NameRow As DataRow In dtRedundantNames.Rows
                dtControls = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " AND Name = '" & WhitTools.SQL.CleanSQL(NameRow.Item("Name")) & "'")
                nAddIndex = 2

                For nCounter As Integer = 1 To dtControls.Rows.Count - 1
                    UpdateRedundantControlName(NameRow.Item("Name"), dtControls.Rows(nCounter).Item("ID"), nAddIndex)
                Next
            Next
        End Sub

        Public Shared Sub UpdateProhibitedControlNames()
            Dim nAddIndex As Integer

            If CurrentProjectRequiresWhitworthLogin() Then
                UpdateRedundantLoginControlNames()
            End If

            For Each CurrentRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROHIBITEDCOLUMNNAMES).Rows
                nAddIndex = 2

                For Each currentControl As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " AND Name = '" & CurrentRow.Item("Name") & "'").Rows
                    UpdateRedundantControlName(CurrentRow.Item("Name"), currentControl.Item("ID"), nAddIndex)
                Next
            Next
        End Sub

        Public Shared Sub UpdateRedundantLoginControlNames()
            Dim nAddIndex As Integer

            For Each currentColumn As LoginColumnType In LoginColumnTypes
                nAddIndex = 2

                For Each currentControl As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " AND Name = '" & currentColumn.ColumnName & "'").Rows
                    UpdateRedundantControlName(currentColumn.ColumnName, currentControl.Item("ID"), nAddIndex)
                Next
            Next
        End Sub

        Public Shared Sub UpdateRedundantControlName(ByVal sName As String, ByVal nControlID As Integer, ByRef nAddIndex As Integer)
            If GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " AND Name = '" & sName & nAddIndex & "'").Rows.Count = 0 Then
                'WriteLine("UPDATE " & DT_WEBRAD_PROJECTCONTROLS & " SET Name = Name + '" & nAddIndex & "' WHERE ID = " & nControlID)
                ExecuteNonQuery("UPDATE " & DT_WEBRAD_PROJECTCONTROLS & " SET Name = Name + '" & nAddIndex & "' WHERE ID = " & nControlID)
                nAddIndex += 1
            End If
        End Sub

        Shared Sub UpdateProgress(ByVal Message As String, ByVal nSpacers As Integer)
            WriteLine(String.Format("<script>try {{ parent.UpdateProgress('{0}',{1}); }} catch (ex) {{ }}</script>", Message, nSpacers))

            Dim timeElapsed = stopwatch.Elapsed

            logger.Info(Message & (timeElapsed - lastProgressMark).ToString() & " - elapsed since last update")
            lastProgressMark = timeElapsed

            GetCurrentPage().Response.Flush()
        End Sub

        
        
        Shared Sub WriteIdentityLabelDeclarations(ByRef sDesignerControls As String)
            Dim sTypePrefix, sType As String

            sTypePrefix = If(isSearch, "txt", "lbl")
            sType = If(isSearch, "Textbox", "Label")

            sDesignerControls &= "Protected WithEvents " & sTypePrefix & "FirstName As Global.System.Web.UI.WebControls." & sType & vbCrLf & vbCrLf
            sDesignerControls &= "Protected WithEvents " & sTypePrefix & "LastName As Global.System.Web.UI.WebControls." & sType & vbCrLf & vbCrLf
            sDesignerControls &= "Protected WithEvents " & sTypePrefix & "IDNumber As Global.System.Web.UI.WebControls." & sType & vbCrLf & vbCrLf
            sDesignerControls &= "Protected WithEvents " & sTypePrefix & "Email As Global.System.Web.UI.WebControls." & sType & vbCrLf & vbCrLf
            sDesignerControls &= "Protected WithEvents " & sTypePrefix & "Class As Global.System.Web.UI.WebControls." & sType & vbCrLf & vbCrLf
        End Sub

        Shared Sub GetDepartmentInfo(ByRef sDepartmentLink As String, ByRef sDepartmentName As String, ByVal nDepartment As Integer, Optional ByVal sCustomDepartmentName As String = "", Optional ByRef sDepartmentURL As String = "")
            Dim dt As New DataTable
            dt = GetDataTable("Select * from web3.Communications.dbo.ARA_Departments Where ID = " & nDepartment, Common.General.Variables.cnx)

            If dt.Rows.Count > 0 Then
                With dt.Rows(0)
                    Dim sHomepageURL As String

                    'If sProjectLocation = "Frontend" Then
                        sHomepageURL = .Item("HomepageURL")
                    'Else
                    '    sHomepageURL = .Item("HomepageURL2")
                    'End If

                    sDepartmentURL = sHomepageURL

                    If sHomepageURL <> "" Then
                        sDepartmentLink = Replace(GetFormattedLink(sHomepageURL, .Item("Department")), "<a ", "<a target=""_blank"" ")
                    End If

                    sDepartmentName = If(sCustomDepartmentName <> "", sCustomDepartmentName, .Item("Department"))
                End With
            End If
        End Sub

        public shared Function GetAdminMembershipCheck() As String
            Return "(IsWebTeamMember() Or Common.supervisors.Contains(WhitTools.Getter.GetCurrentUsername()))"
        End Function

        Public Shared Function ContainsCodeBlock(ByVal target As string) As boolean
                Return target.Contains("<%")
            End Function

        Public Shared Sub CreateLoginColumnTypes()
            Dim currentType As LoginColumnType
            LoginColumnTypes = New List(Of LoginColumnType)

            currentType = New LoginColumnType(N_IDNUMBER_CONTROLID)
            currentType.DisplayName = "Submission ID"
            currentType.ColumnName = "ID"
            currentType.IncludeSelectStatement = False
            currentType.SQLType = "Int"
            LoginColumnTypes.Add(currentType)

            currentType = New LoginColumnType(N_WHITWORTHID_CONTROLID)
            currentType.DisplayName = "Whitworth ID Number"
            currentType.ColumnName = "IDNumber"
            currentType.ControlType = N_TEXTBOX_CONTROL_TYPE
            currentType.ControlReference = "txtIDNumber"
            currentType.ControlMaxLength = 7
            currentType.ControlWidth = 50
            currentType.SQLType = "nvarchar(10)"
            LoginColumnTypes.Add(currentType)

            currentType = New LoginColumnType(N_FIRSTNAME_CONTROLID)
            currentType.DisplayName = "First Name"
            currentType.ColumnName = "FirstName"
            currentType.ControlType = N_TEXTBOX_CONTROL_TYPE
            currentType.ControlReference = "txtFirstName"
            currentType.ControlMaxLength = 50
            currentType.ControlWidth = 300
            currenttype.IncludeProspectUserControl = true
            LoginColumnTypes.Add(currentType)

            currentType = New LoginColumnType(N_LASTNAME_CONTROLID)
            currentType.DisplayName = "Last Name"
            currentType.ColumnName = "LastName"
            currentType.ControlType = N_TEXTBOX_CONTROL_TYPE
            currentType.ControlReference = "txtLastName"
            currenttype.IncludeProspectUserControl = true
            currentType.ControlMaxLength = 50
            currentType.ControlWidth = 300
            currenttype.IncludeProspectUserControl = true
            LoginColumnTypes.Add(currentType)

            currentType = New LoginColumnType(N_EMAIL_CONTROLID)
            currentType.DisplayName = "Email"
            currentType.ColumnName = "Email"
            currentType.ControlType = N_TEXTBOX_CONTROL_TYPE
            currentType.ControlReference = "txtEmail"
            currentType.ControlMaxLength = 50
            currentType.ControlWidth = 300
            currenttype.IncludeProspectUserControl = true
            LoginColumnTypes.Add(currentType)

            currentType = New LoginColumnType(N_PHONE_CONTROLID)
            currentType.DisplayName = "Phone"
            currentType.ColumnName = "Phone"
            currentType.ControlType = N_TEXTBOX_CONTROL_TYPE
            currentType.ControlReference = "txtPhone"
            currentType.ControlMaxLength = 15
            currentType.ControlWidth = 50
            currenttype.IncludeProspectUserControl = true
            LoginColumnTypes.Add(currentType)

            currentType = New LoginColumnType(N_CLASS_CONTROLID)
            currentType.DisplayName = "Class"
            currentType.ColumnName = "Class"
            currentType.ControlType = N_DROPDOWNLIST_CONTROL_TYPE
            currentType.ControlReference = "ddlClass"
            LoginColumnTypes.Add(currentType)

            currentType = New LoginColumnType(N_DATESUBMITTED_CONTROLID)
            currentType.DisplayName = "Date Submitted"
            currentType.ControlType = N_TEXTBOX_CONTROL_TYPE
            currentType.ControlReference = "txtDateSubmitted"
            currentType.ColumnName = GetDateSubmittedColumnReference()
            currentType.BackendDisplayValue = "<%# ctype(Container.Dataitem(""" & currentType.ColumnName & """),datetime).ToShortDateString %>"
            currentType.ControlMaxLength = 10
            currentType.ControlWidth = 100
            currentType.IncludeSelectStatement = False
            currentType.SQLType = "DateTime"
            LoginColumnTypes.Add(currentType)
        End Sub

        Public Shared LoginColumnTypes As List(Of LoginColumnType)
    End Class
End Namespace
