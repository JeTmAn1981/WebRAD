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
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.DataSources
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.Controls
Imports Common.General.ProjectOperations
Imports Common.General.DataTypes
Imports Common.General.Folders
Imports Common.SQL.Main
Imports Common.General.Pages
Imports Common.ProjectFiles
Imports Common.BuildSetup
Imports Common.Webpages.Validation.Main
Imports Common.Webpages.BindData
Imports Common.Webpages.Frontend.Main
Imports Common.Webpages.Frontend.MultiPage
Imports Common.Webpages.Backend.Main
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Archive
Imports System.Threading
Imports System.Reflection
Imports System.IO.File

Imports WhitTools.Utilities
Imports System.Text

Namespace Webpages
    Public Class Main

        Shared Sub CreateWebpages()
            CopyWhitToolsLibrary()

            WriteFrontendPages()

            If BackendCreationAllowed() Then
                InitializeBackendNavigation()
                Backend.Main.WriteBackendPages()
                UpdateProgress("Creating and copying backend project files", 0)
                CreateAndCopyBackendProjectFiles()
            End If
        End Sub

        Private Shared Sub CopyWhitToolsLibrary()
            Dim copyCommand As String = "exec xp_cmdshell 'copy """ & S_WHITTOOLS_DIR & "WhitTools*.*"" """ & GetTemplatePath(True) & "Visual Studio Files\Standard\bin"" /Y'"

            logger.Info(copyCommand)

            WhitTools.SQL.ExecuteNonQuery(copyCommand, "", 3, True, Nothing, False, True, False)
        End Sub


        Private Shared Sub InitializeBackendNavigation()
            Backend.Navigation.mainNavigation = ""
            Backend.Navigation.CreateMainNavigation()
        End Sub

        Shared Sub DoWebpageCreationSetup()
            GetTopLevelControls()
            CorrectProjectFilePaths()
            SetupGlobalProjectVariables()
        End Sub

        Public Shared Sub SetupGlobalProjectVariables()
            With projectDT.Rows(0)
                projectName = RemoveNonAlphanumeric(projectDT.Rows(0).Item("PageTitle"))

                controlsDT = ConvertDataTableColumnTypes(controlsDT)
                EliminateNull(controlsDT)

                General.Main.GetDepartmentInfo(departmentLink, departmentName, .Item("Department"), .Item("CustomDepartmentName"), departmentUrl)
            End With
        End Sub

        Public Shared Sub SetupIndividualProjectVariables(ByVal type As String)
            General.Variables.isFrontend = (type = "Frontend")
            projectLocation = If(Left(projectDT.Rows(0).Item($"{type}Path"), 6) = "\\web1", "Frontend", "Backend")
            baseDir = S_PROJECTFILESPATH & projectName & $"\{type}\"

            If Not Directory.Exists(baseDir) Then
                Directory.CreateDirectory(baseDir)
            End If
        End Sub


        Shared Sub GetSuppliedData(ByRef sDTSupplied As String, ByRef sPostbackActions As StringBuilder, ByVal sTargetControlID As String, Optional ByVal bDTSuppliedExists As Boolean = False)
            Dim sDTSuppliedDeclaration, sDataTextField, sDataValueField As String
            Dim includePleaseSelect As Boolean
            For Each CurrentControl As DataRow In controlsDT.Rows
                'Possible bug here - changing parent is repeater check to only check immediate repeater parent and not all possible repeater parents
                'If ParentIsRepeaterControl(CurrentControl.Item("ID"), sTargetControlID, 0, "", "", "", True) And CStr(CurrentControl.Item("SelectionItems")) <> "" And CStr(CurrentControl.Item("SelectionItems")) <> "-1" Then
                includePleaseSelect = CurrentControl.Item("IncludePleaseSelect") = "1"

                If ParentIsRepeaterControl(CurrentControl.Item("ID"), sTargetControlID, 0, "", "", "", False) And CStr(CurrentControl.Item("SelectionItems")) <> "" And CStr(CurrentControl.Item("SelectionItems")) <> "-1" Then
                    GetDataSourceSelectString(CurrentControl.Item("DataSourceID"), sDataTextField, sDataValueField)

                    If sDTSupplied = "" Then
                        sDTSuppliedDeclaration = If(bDTSuppliedExists, "dtSupplied =", "dim dtSupplied as") & " new WhitTools.DataTablesSupplied" & vbCrLf & vbCrLf
                    End If

                    If CurrentControl.Item("SelectionItems") = "2" Then
                        sDTSupplied &= "dtSupplied.AddRow(""" & CurrentControl.Item("Prefix") & CurrentControl.Item("Name") & """, ""SQLSelect"", """ & GetDataSourceSelectString(CurrentControl.Item("DataSourceID")) & """, """ & sDataTextField & """, """ & sDataValueField & """)" & vbCrLf

                        'Data method data supply option chosen for this control
                    ElseIf CurrentControl.Item("SelectionItems") = "4" Then
                        Select Case CurrentControl.Item("DataMethod")
                            Case "1"
                                sDTSupplied &= "dtSupplied.AddRow(""" & CurrentControl.Item("Prefix") & CurrentControl.Item("Name") & """, ""FillStates"", """", """","""")" & vbCrLf
                            Case "3"
                                sDTSupplied = sDTSupplied & "dtSupplied.AddRow(""" & CurrentControl.Item("Prefix") & CurrentControl.Item("Name") & """, ""FillNumbers"", " & includePleaseSelect & ", """ & CurrentControl.Item("MinimumValue") & """, """ & CurrentControl.Item("MaximumValue") & """)" & vbCrLf
                            Case "4"
                                sDTSupplied = sDTSupplied & "dtSupplied.AddRow(""" & CurrentControl.Item("Prefix") & CurrentControl.Item("Name") & """, ""SQLSelect"", """ & CurrentControl.Item("OtherDataMethod").ToString().Replace("ControlNameHere", CurrentControl.Item("Prefix") & CurrentControl.Item("Name")) & """, """","""")" & vbCrLf
                            Case "5"
                                sDTSupplied &= "dtSupplied.AddRow(""" & CurrentControl.Item("Prefix") & CurrentControl.Item("Name") & """, ""FillDepartments"", """", """","""")" & vbCrLf
                            Case "6"
                                sDTSupplied &= "dtSupplied.AddRow(""" & CurrentControl.Item("Prefix") & CurrentControl.Item("Name") & """, ""FillProspectMajors"", """", """","""")" & vbCrLf
                            Case "7"
                                sDTSupplied = sDTSupplied & "dtSupplied.AddRow(""" & CurrentControl.Item("Prefix") & CurrentControl.Item("Name") & """, ""FillTimes"", 1/12, ""8:00 AM"", ""7:45 AM"")" & vbCrLf
                            Case "8"
                                sDTSupplied = sDTSupplied & "dtSupplied.AddRow(""" & CurrentControl.Item("Prefix") & CurrentControl.Item("Name") & """, ""FillMonths"", """", """", """")" & vbCrLf
                            Case "9"
                                sDTSupplied = sDTSupplied & "dtSupplied.AddRow(""" & CurrentControl.Item("Prefix") & CurrentControl.Item("Name") & """, ""FillResidenceHalls"", ""True"", ""1"", """")" & vbCrLf
                        End Select
                    End If
                End If
            Next

            If sDTSupplied <> "" Then
                sDTSupplied = sDTSuppliedDeclaration & sDTSupplied
            End If
        End Sub

        Shared Function GetRunAOPageHeader() As String
            Dim sRunAOPageHeader As String = ""
            Dim dtAdditionalOperations As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " WHERE Type = " & N_WEBRAD_AOPAGEHEADERTYPE & " AND PageID IN (SELECT ID FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & GetProjectID() & ")")

            For Each Currentrow As DataRow In dtAdditionalOperations.Rows
                With Currentrow
                    If .Item("AdditionalOperations") <> "" Then
                        If (General.Variables.isFrontend And BelongsToPage(pageNumber, .Item("PageID"))) Or Not General.Variables.isFrontend Then
                            sRunAOPageHeader &= vbCrLf & .Item("AdditionalOperations") & vbCrLf
                        End If
                    End If
                End With
            Next

            If ProjectIncludesProspectUserControl() Then
                sRunAOPageHeader &= "<%@ Register TagPrefix=""uc"" TagName=""" & GetProspectControl().Name & """ Src=""ProspectGathering/AdmissionsInfoNew.ascx"" %>"
            End If

            Return sRunAOPageHeader
        End Function


        Public Shared Sub GetCurrentOperations(ByRef pageBody As String)
            Dim dtAdditionalOperationTypes As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_ADDITIONALOPERATIONTYPES & " WHERE NOT ID IN (5," & N_ADDITIONALOPERATIONTYPE_IMPORTS & ")")

            For Each CurrentOperationType As DataRow In dtAdditionalOperationTypes.Rows
                AddOperationType(CurrentOperationType, pageBody)
            Next
        End Sub

        Private Shared Sub AddOperationType(CurrentOperationType As DataRow, ByRef pageBody As String)
            Dim sCurrentCall, sCurrentMethod As String

            For Each CurrentOperation As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " WHERE Type = " & CurrentOperationType.Item("ID") & " AND PageID IN (SELECT ID FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & GetProjectID() & ")").Rows
                AddAdditionalOperation(CurrentOperationType, CurrentOperation, sCurrentMethod, sCurrentCall)
            Next

            AddCertificationOperation(CurrentOperationType, sCurrentMethod)
            AddProspectOperations(CurrentOperationType, sCurrentMethod, sCurrentCall)

            If sCurrentMethod <> "" And CurrentOperationType.Item("ExecuteType") <> "Write" Then
                WrapOperationMethodCall(CurrentOperationType, sCurrentMethod, sCurrentCall)
            End If

            If CurrentOperationType.Item("ExecuteType") <> "Write" Then
                pageBody = MailFieldSubstitute(pageBody, "(" & CurrentOperationType.Item("MethodReference") & "Call)", sCurrentCall)
                pageBody = MailFieldSubstitute(pageBody, "(" & CurrentOperationType.Item("MethodReference") & "Method)", sCurrentMethod)
            Else
                pageBody = MailFieldSubstitute(pageBody, "(" & CurrentOperationType.Item("MethodReference") & ")", sCurrentMethod)
            End If

            sCurrentCall = ""
            sCurrentMethod = ""
        End Sub

        Private Shared Sub WrapOperationMethodCall(CurrentOperationType As DataRow, ByRef sCurrentMethod As String, ByRef sCurrentCall As String)
            sCurrentMethod = vbCrLf & "Sub " & CurrentOperationType.Item("MethodReference") & "()" & vbCrLf & sCurrentMethod & "End Sub" & vbCrLf
            sCurrentCall = vbCrLf & CurrentOperationType.Item("MethodReference") & "()" & vbCrLf
        End Sub

        Private Shared Sub AddAdditionalOperation(CurrentOperationType As DataRow, CurrentOperation As DataRow, ByRef sCurrentMethod As String, ByRef sCurrentCall As String)
            With CurrentOperation
                If .Item("AdditionalOperations") <> "" Then
                    If Not General.Variables.isFrontend Or isPrintable Or (General.Variables.isFrontend And BelongsToPage(pageNumber, .Item("PageID"))) Then
                        sCurrentMethod &= vbCrLf & .Item("AdditionalOperations") & vbCrLf
                    End If
                End If
            End With
        End Sub

        Private Shared Sub AddCertificationOperation(CurrentOperationType As DataRow, ByRef sCurrentMethod As String)
            If General.Variables.isFrontend And CurrentOperationType.Item("MethodReference") = "RunAOAfterPageSubmit" And pageNumber = GetPageCount() And Not DefaultCertificationPage() Then
                sCurrentMethod &= GetCertificationMethod()
            End If
        End Sub

        Private Shared Sub AddProspectOperations(CurrentOperationType As DataRow, ByRef sCurrentMethod As String, ByRef sCurrentCall As String)
            If ProjectIncludesProspectUserControl() And (CurrentOperationType.Item("MethodReference") = "RunAOPageAfterLoad" Or CurrentOperationType.Item("MethodReference") = "RunAOAfterPageSubmit" Or CurrentOperationType.Item("MethodReference") = "RunAOPageHeader") Then
                Dim prospectControl As ProjectControl = GetProspectControl()
                Dim sqlMainTableName As String = GetAncillaryProject("SQLMainTableName")

                If CurrentOperationType.Item("MethodReference") = "RunAOPageAfterLoad" Then

                    If Not General.Variables.isFrontend Then
                        sCurrentMethod = "LoadAdmissionsInfoControls(uc" & prospectControl.Name & ",cnx)" & vbCrLf & sCurrentMethod
                        sCurrentMethod = "LoadAdmissionsProspectInfo(uc" & prospectControl.Name & ", GetDataTable(""SELECT OAID FROM " & sqlMainTableName & " WHERE ID = "" & GetQueryString(""ID""), cnx).Rows(0).Item(""OAID""))" & vbCrLf & sCurrentMethod
                    End If
                ElseIf CurrentOperationType.Item("MethodReference") = "RunAOAfterPageSubmit" Then

                    If General.Variables.isFrontend Then
                        sCurrentMethod &= "Dim nOAID As Integer = SaveAdmissionsProspect(""" & prospectControl.ProspectCode & """)" & vbCrLf & vbCrLf
                        sCurrentMethod &= "With uc" & prospectControl.Name & vbCrLf
                        sCurrentMethod &= "ExecuteNonQuery(""UPDATE " & sqlMainTableName & " SET OAID = "" & nOAID & "", FirstName = '"" & CleanSQL(CType(.FindControl(""txtFirstName""), TextBox).Text) & ""', LastName = '"" & CleanSQL(CType(.FindControl(""txtLastName""), TextBox).Text) & ""', Email = '"" & CleanSQL(CType(.FindControl(""txtEmail""), TextBox).Text) & ""' WHERE ID = (SELECT IDENT_CURRENT('" & sqlMainTableName & "'))"", cnx)" & vbCrLf
                        sCurrentMethod &= "End With" & vbCrLf
                    Else
                        sCurrentMethod &= "Try" & vbCrLf
                        sCurrentMethod &= "UpdateAdmissionsProspect(uc" & prospectControl.Name & ", GetDataTable(""SELECT OAID FROM " & sqlMainTableName & " WHERE ID = "" & GetQueryString(""ID""),cnx).Rows(0).Item(""OAID""))" & vbCrLf & vbCrLf
                        sCurrentMethod &= "With uc" & prospectControl.Name & vbCrLf
                        sCurrentMethod &= "ExecuteNonQuery(""UPDATE " & sqlMainTableName & " SET FirstName = '"" & CleanSQL(CType(.FindControl(""txtFirstName""), TextBox).Text) & ""', LastName = '"" & CleanSQL(CType(.FindControl(""txtLastName""), TextBox).Text) & ""', Email = '"" & CleanSQL(CType(.FindControl(""txtEmail""), TextBox).Text) & ""' WHERE ID = "" & GetQuerystring(""ID""), cnx)" & vbCrLf
                        sCurrentMethod &= "End With" & vbCrLf
                        sCurrentMethod &= "Catch" & vbCrLf
                        sCurrentMethod &= "End Try" & vbCrLf
                    End If
                End If
            End If
        End Sub

        Private Shared Function GetProspectControl() As ProjectControl
            Return currentProject.ProjectControls.Where(Function(control) control.ControlType = N_PROSPECTGATHERING_USERCONTROL_CONTROL_TYPE).First()
        End Function

        Shared Function GetLoadDDLs() As String
            Dim loadDdLs, dataTextField, dataValueField As String

            AddAlreadySubmittedCheck(loadDdLs)
            AddWorkflowStepCompletedCheck(loadDdLs)
            AddPersonalInfoLoad(loadDdLs)
            AddControlDataLoads(loadDdLs, dataTextField, dataValueField)
            AddApplicationFinishedCheck(loadDdLs)

            Return loadDdLs
        End Function

        Private Shared Sub AddAlreadySubmittedCheck(ByRef sGetLoadDDLs As String)

            If pageNumber <> -1 Then
                sGetLoadDDLs &= "CheckAlreadySubmitted(" & GetUsernameReference(GetAncillaryProject("RequireLogin")) & ")" & vbCrLf
            End If
        End Sub

        Private Shared Sub AddApplicationFinishedCheck(ByRef sGetLoadDDLs As String)
            If Not DefaultCertificationPage() And pageNumber = GetPageCount() Then
                sGetLoadDDLs &= "CheckApplicationFinished(Common.GetCurrentusername())" & vbCrLf
            End If
        End Sub

        Public Shared Function GetWebServices()
            Dim webServices As String = ""

            For nCounter = 0 To controlsDT.Rows.Count - 1
                With controlsDT.Rows(nCounter)
                    If .Item("SupplyDataType") = "Autocomplete" And (BelongsToPage(pageNumber, .Item("PageID")) And Not (General.Variables.isFrontend And .Item("DisplayLocation") = "3") And (Not isSearch Or (isSearch And IsSearchControl(.Item("ID"))))) Then
                        AddAutocompleteWebService(webServices, controlsDT.Rows(nCounter))
                    End If
                End With
            Next

            If isInsert And CurrentProjectRequiresWhitworthLogin() Then
                AddUsernameAutocompleteWebService(webServices)
            End If

            Return webServices
        End Function


        Private Shared Sub AddAutocompleteWebService(ByRef webServices As String, ByVal currentData As DataRow)
            With currentData
                webServices &= "<ScriptMethod, WebMethod>" & vbCrLf
                webServices &= $"Public Shared Function Get{ .Item("Name")}AutocompleteData(ByVal prefixText As String, ByVal count As Integer) As IEnumerable" & vbCrLf
                webServices &= $"    Return (From currentRow In GetDataTable(""{GetDataSourceSelectString(.Item("DataSourceID"), "", "", "", "", "[" & currentData.Item("DataSourceColumn") & "] LIKE '%"" & prefixText & ""%'")}"",CreateSQLConnection(""" & GetSQLDatabaseName(GetCurrentProjectDT.Rows(0).Item("SQLDatabase")) & """)).Rows" & vbCrLf
                webServices &= $"Select currentRow.item(""{ .Item("DataSourceColumn")}"")).ToList()" & vbCrLf
                webServices &= "End Function" & vbCrLf & vbCrLf
            End With

        End Sub

        Private Shared Sub AddUsernameAutocompleteWebService(ByRef webServices As String)
            webServices &= "<ScriptMethod, WebMethod>" & vbCrLf
            webServices &= $"Public Shared Function GetUserAutocompleteData(ByVal prefixText As String, ByVal count As Integer) As IEnumerable" & vbCrLf
            webServices &= $"Return (From currentRow In GetDataTable(""SELECT * FROM adTelephone.dbo.UserInfo_V WHERE IDNumber in (select plID from adTelephone.dbo.PeopleListing where PLActive='1') AND [User] like '%"" & prefixtext & ""%' order by PLLName, plfName"", CreateSQLConnection(""" & GetSQLDatabaseName(GetCurrentProjectDT.Rows(0).Item("SQLDatabase")) & """)).Rows" & vbCrLf
            webServices &= $"Select currentRow.item(""User"")).ToList()" & vbCrLf
            webServices &= "End Function" & vbCrLf & vbCrLf
        End Sub

        Private Shared Sub AddControlDataLoads(ByRef sGetLoadDDLs As String, sDataTextField As String, sDataValueField As String)
            Dim nCounter As Integer
            Dim sCurrentPrefix As String
            Dim sIncludePleaseSelect As String

            For nCounter = 0 To controlsDT.Rows.Count - 1
                With controlsDT.Rows(nCounter)
                    If ControlDisplayAllowed(.Item("DisplayLocation")) AndAlso Not ParentIsRepeaterControl(.Item("ID")) And (BelongsToPage(pageNumber, .Item("PageID")) And (Not isSearch Or (isSearch And IsSearchControl(.Item("ID"))))) Then
                        sCurrentPrefix = .Item("Prefix")

                        sIncludePleaseSelect = If(.Item("IncludePleaseSelect") = "1" And ControlUsesPleaseSelectItem(.Item("ControlType")), True, False)

                        If CStr(.Item("SelectionItems")) <> "" Then
                            If CStr(.Item("SelectionItems")) = "2" Then
                                sGetLoadDDLs &= "FillListData(" & sCurrentPrefix & .Item("Name") & ",cnx,""" & GetDataSourceSelectString(controlsDT.Rows(nCounter).Item("DataSourceID"), sDataTextField, sDataValueField) & """,""" & sDataTextField & """,""" & sDataValueField & """," & If(.Item("IncludePleaseSelect") = "1", "True", "False") & If(.Item("IncludePleaseSelect") = "1", ",""""", "") & ")" & vbCrLf
                            ElseIf CStr(.Item("SelectionItems")) = "4" Then
                                Select Case .Item("DataMethod")
                                    Case "1"
                                        sGetLoadDDLs &= "FillStates(" & sCurrentPrefix & .Item("Name") & ")" & vbCrLf
                                    Case "3"
                                        sGetLoadDDLs &= "FillNumbers(" & sCurrentPrefix & .Item("Name") & "," & .Item("MinimumValue") & "," & .Item("MaximumValue") & "," & sIncludePleaseSelect & ")" & vbCrLf
                                    Case "4"
                                        sGetLoadDDLs &= .Item("OtherDataMethod").ToString().Replace("ControlNameHere", sCurrentPrefix & .Item("Name")) & vbCrLf
                                    Case "5"
                                        sGetLoadDDLs &= "FillDepartments(" & sCurrentPrefix & .Item("Name") & ")" & vbCrLf
                                    Case "6"
                                        sGetLoadDDLs &= "FillProspectMajors(" & sCurrentPrefix & .Item("Name") & "," & sIncludePleaseSelect & ")" & vbCrLf
                                    Case "7"
                                        sGetLoadDDLs &= "FillTimes(" & sCurrentPrefix & .Item("Name") & ",""" & GetTime("8:00 AM", .Item("MinimumValue")) & """,""" & GetTime("7:45 AM", .Item("MaximumValue")) & """, 1/12)" & vbCrLf
                                    Case "8"
                                        sGetLoadDDLs &= "FillMonths(" & sCurrentPrefix & .Item("Name") & ")" & vbCrLf
                                    Case "9"
                                        sGetLoadDDLs &= "FillResidenceHalls(" & sCurrentPrefix & .Item("Name") & "," & sIncludePleaseSelect & ",1)" & vbCrLf
                                    Case "10"
                                        sGetLoadDDLs &= "FillMajors(" & sCurrentPrefix & .Item("Name") & ",True,False," & sIncludePleaseSelect & ",True)" & vbCrLf
                                    Case "11"
                                        sGetLoadDDLs &= "FillTerms(" & sCurrentPrefix & .Item("Name") & "," & sIncludePleaseSelect & "," & .Item("MinimumValue") & "," & .Item("MaximumValue") & ")" & vbCrLf
                                End Select
                            End If
                        End If

                        If .Item("SupplyControlData") = "1" Then
                            If IsControlType(.Item("ControlType"), "Repeater") Then
                                sGetLoadDDLs &= GetSelectRepeaterData(controlsDT.Rows(nCounter), "", 0, True)
                            ElseIf .Item("SupplyDataType") = "DirectAssignment" Then
                                sGetLoadDDLs &= sCurrentPrefix & .Item("Name") & ".Text = GetDataTable(""" & GetDataSourceSelectString(.Item("DataSourceID")) & """, cnx).Rows(0).Item(""" & .Item("DataSourceColumn") & """)" & vbCrLf
                            End If
                        End If

                        If ControlTypeIsRepeater(.Item("ControlType")) And .Item("RepeaterAddRemove") = "1" And MinimumRequiredAboveZero(.Item("MinimumRequired")) And Not isPrintable Then
                            GetMinimumRepeaterItems("Nothing", sGetLoadDDLs, controlsDT.Rows(nCounter))

                            Dim dtChildRepeaters As DataTable = GetDataTable("Select * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " And ControlType = " & N_REPEATER_CONTROL_TYPE & " And Not ParentControlID Is NULL And RepeaterAddRemove = 1 And MinimumRequired > 0")

                            GetNestedRepeaterInitialItems(sGetLoadDDLs, controlsDT.Rows(nCounter), dtChildRepeaters)
                        End If
                    End If
                End With
            Next
        End Sub

        Private Shared Sub AddPersonalInfoLoad(ByRef getLoadDDLs As String)
            If CurrentProjectRequiresWhitworthLogin() Then
                If isInsert Then
                    getLoadDDLs &= "If GetQueryString(""Username"") <> """" Then" & vbCrLf
                    getLoadDDLs &= "LoadPersonalInfo()" & vbCrLf
                    getLoadDDLs &= "End If" & vbCrLf & vbCrLf
                ElseIf General.Variables.isFrontend And pageNumber <= 1 Then
                    getLoadDDLs &= "Try" & vbCrLf

                    Dim dtLoginColumns As DataTable = GetDataTable("Select * FROM " & DT_WEBRAD_PROJECTCOLUMNS & " PCL LEFT OUTER JOIN " & DT_WEBRAD_LOGINCOLUMNTYPES & " LCT On PCL.ColumnControlID = LCT.IDNumber WHERE Type = 'Login' AND ProjectID = " & GetProjectID())

                    getLoadDDLs &= "dim UserInfo = GetUserInfo(Common.GetCurrentUsername()).Rows(0)" & vbCrLf & vbCrLf


                    For Each Currentrow As DataRow In dtLoginColumns.Rows
                        With Currentrow
                            getLoadDDLs &= "lbl" & Currentrow.Item("ControlName") & ".Text = UserInfo.Item(""" & .Item("ControlName") & """)" & vbCrLf
                        End With
                    Next

                    getLoadDDLs &= " Catch" & vbCrLf

                    For Each Currentrow As DataRow In dtLoginColumns.Rows
                        With Currentrow
                            getLoadDDLs &= "lbl" & Currentrow.Item("ControlName") & ".Text = ""Unknown - Username "" & Common.GetCurrentUsername() & "")" & vbCrLf
                        End With
                    Next

                    getLoadDDLs &= "End Try" & vbCrLf & vbCrLf
                End If
            End If
        End Sub

        Private Shared Sub AddWorkflowStepCompletedCheck(ByRef sGetLoadDDLs As String)
            If isWorkflow And General.Variables.isFrontend And IsLastPage() Then
                sGetLoadDDLs &= GetCheckWorkflowStepAlreadyCompletedCall() & vbCrLf
            End If
        End Sub

        Public Shared Function GetCheckWorkflowStepAlreadyCompletedCall() As String
            Return "CheckWorkflowStepAlreadyCompleted(lblWorkflowStepID.Text)"
        End Function

        Public Shared Function GetTime(ByVal defaultTime As String, ByVal specifiedTime As String) As String
            Return If(specifiedTime <> "", specifiedTime, defaultTime)
        End Function

        Shared Function WrapInTableRow(ByRef sContent As String)
            Return "<tr><td colspan='100'>" & sContent & "</td></tr>" & vbCrLf
        End Function


        Shared Function GetCheckClosedMethod() As String
            Dim sCheckClosedMethod As String = ""

            If Backend.Main.GetBackendOption(S_BACKEND_OPTION_SCHEDULE) And General.Variables.isFrontend And Not isInsert Then
                sCheckClosedMethod = vbCrLf & vbCrLf & "Shared Sub CheckSchedule()" & vbCrLf
                sCheckClosedMethod &= "If Not " & GetAdminMembershipCheck() & " Then " & vbCrLf
                sCheckClosedMethod &= "dim dtDates as DataTable = GetDatatable(""SELECT * FROM " & GetScheduleTableName() & """,cnx)" & vbCrLf & vbCrLf
                sCheckClosedMethod &= "If dtDates.Rows.Count > 0 AndAlso Not IsBetweenDates(dtDates.Rows(0).Item(""OpenDate""),dtDates.Rows(0).Item(""CloseDate""),dtDates.Rows(0).Item(""OpenTime""),dtDates.Rows(0).Item(""CloseTime"")) Then" & vbCrLf
                sCheckClosedMethod &= "closedMessage = If(dtDates.Rows(0).Item(""Message"") <> """", dtDates.Rows(0).Item(""Message""), DEFAULT_CLOSED_MESSAGE)" & vbCrLf
                sCheckClosedMethod &= "messages.RedirectToMessage(MessageCode.Closed)" & vbCrLf
                sCheckClosedMethod &= "End If" & vbCrLf
                sCheckClosedMethod &= "End If" & vbCrLf
                sCheckClosedMethod &= "End Sub" & vbCrLf
            End If

            Return sCheckClosedMethod
        End Function

        Shared Function GetHeadData(ByRef ControlsDT As DataTable) As String
            Dim sEditorReferences As String = ""
            Dim sHeadData As String = ""

            If ControlsDT Is Nothing Then
                Return vbCrLf & "<meta name=""robots"" content=""noindex"">"
            Else
                For Each CurrentRow As DataRow In ControlsDT.Rows
                    With CurrentRow
                        Dim textMode = .Item("TextMode")
                        Dim richTextUser = .Item("RichTextUser")

                        If IsDataType(.Item("ControlType"), "Textbox") And .Item("TextMode") = "MultiLine" And .Item("RichTextUser") = "1" Then
                            sEditorReferences &= "CKEDITOR.replace('txt" & .Item("Name") & "');" & vbCrLf
                        End If
                    End With
                Next

                If sEditorReferences <> "" Then
                    sHeadData = "<script type=""text/javascript"" src=""/js/ckeditor/ckeditor.js""></script>" & vbCrLf
                    sHeadData &= "<script>" & vbCrLf
                    sHeadData &= "window.onload = function () {" & vbCrLf
                    sHeadData &= sEditorReferences
                    sHeadData &= "};" & vbCrLf
                    sHeadData &= "</script>"
                End If
            End If

            Return sHeadData
        End Function

        Public Shared Sub AddFormLoadLink(ByRef sFormLoadText As String, ByVal LinkName As String, Optional ByVal LinkURL As String = "")
            If LinkName <> "" Then
                sFormLoadText &= "CreateFormBreadcrumb(""" & LinkName & """, """ & LinkURL & """, False)" & vbCrLf
            End If
        End Sub

        Public Shared Sub AddMaintenanceHomeFormLoadLink(ByRef loadFormText As String)
            AddFormLoadLink(loadFormText, GetAncillaryProject("PageTitle") & " - "" & IIf(bArchive, ""Archive Listing"", ""Main Listing"") & """, """ & IIf(bArchive, ""archive" & GetAncillaryName() & """, ""index" & GetAncillaryName() & """) & """ & ".aspx")
        End Sub

        Public Shared Sub AddFormLoadHeadingCurrentPageLink(ByRef formLoadText As String, ByVal title As String, ByVal titleAddition As String)
            AddFormLoadLink(formLoadText, If(left(titleAddition, 3) = " - ", Right(titleAddition, titleAddition.Length - 3), titleAddition))
            AddFormLoadHeading(formLoadText, title & titleAddition)
        End Sub

        Public Shared Sub AddFormLoadHeading(ByRef sFormLoadText As String, ByVal sTitle As String)
            sFormLoadText &= "SetFormHeading(""" & sTitle & """)" & vbCrLf
        End Sub

        Public Shared Sub WriteWebConfig()
            Dim TemplatePath As String
            Dim pageBody As String

            TemplatePath = GetTemplatePath() & "WebRADWebConfig.eml"

            pageBody = GetMailFile(TemplatePath)
            pageBody = MailFieldSubstitute(pageBody, "(CustomErrorsMode)", If(projectType = "Test", "Off", "RemoteOnly"))
            pageBody = MailFieldSubstitute(pageBody, "(EFConnectionStrings)", GetEFConnectionStrings())

            If General.Variables.isFrontend AndAlso CurrentProjectRequiresNonWhitworthLogin() Then
                pageBody = MailFieldSubstitute(pageBody, "(FormsAuthenticationAuthorization)", GetFormsAuthenticationAuthorization())
                pageBody = MailFieldSubstitute(pageBody, "(AnonymousAccess)", GetAnonymousAccess())
            End If

            RemoveUnusedTemplateFields(pageBody)
            WriteAllText(baseDir & "\" & "web.config", pageBody)
        End Sub

        Private Shared Function GetEFConnectionStrings() As String
            Dim dbName = GetSQLDatabaseName()
            'Bug here, hardcoded web3 reference in despite allowing specification of another server (though other server is no longer being used, it could be an option in future)

            Dim connectionStrings As String = ""

            For Each currentrow As DataRow In GetDataTable("Select * From " & DT_WEBRAD_SQLDATABASES & " Where ServerID = 1 order by Name", cnx).Rows
                connectionStrings &= $"<add name=""{currentrow.Item("Name")}Entities"" connectionString=""metadata=res://*/Data.{currentrow.Item("Name")}.csdl|res://*/Data.{currentrow.Item("Name")}.ssdl|res://*/Data.{currentrow.Item("Name")}.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=web3;initial catalog={currentrow.Item("Name")};integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;"" providerName=""System.Data.EntityClient"" />" & vbCrLf
            Next

            Return connectionStrings
        End Function

        Private Shared Function GetFormsAuthenticationAuthorization() As String
            Dim authentication As String = "<authentication mode = ""Forms"" >" & vbCrLf

            authentication &= "<forms name=""." & currentProject.GetProjectNameAlphaNumericOnly() & """ loginUrl=""login.aspx"" protection=""All"" path=""/"" timeout=""3000""></forms>" & vbCrLf
            authentication &= "</authentication>" & vbCrLf
            authentication &= "<authorization>" & vbCrLf
            authentication &= " <deny users = ""?"" />" & vbCrLf
            authentication &= "<allow users=""*""/>" & vbCrLf
            authentication &= "</authorization>" & vbCrLf

            Return authentication
        End Function

        Private Shared Function GetAnonymousAccess()
            'Dim anonymousAccess As String = ""

            'anonymousAccess = vbCrLf & "<location path = ""newuser.aspx"">" & vbCrLf
            'anonymousAccess &= "<system.web>" & vbCrLf
            'anonymousAccess &= "<authorization>" & vbCrLf
            'anonymousAccess &= "<allow users=""*""/>" & vbCrLf
            'anonymousAccess &= "</authorization>" & vbCrLf
            'anonymousAccess &= "</system.web>" & vbCrLf
            'anonymousAccess &= "</location>" & vbCrLf

            'anonymousAccess = vbCrLf & "<location path = ""message.aspx"">" & vbCrLf
            'anonymousAccess &= "<system.web>" & vbCrLf
            'anonymousAccess &= "<authorization>" & vbCrLf
            'anonymousAccess &= "<allow users=""*""/>" & vbCrLf
            'anonymousAccess &= "</authorization>" & vbCrLf
            'anonymousAccess &= "</system.web>" & vbCrLf
            'anonymousAccess &= "</location>" & vbCrLf

            Dim anonymousAccess As New StringBuilder()


            anonymousAccess.Append(Environment.NewLine)

            anonymousAccess.AppendLine("<location path = ""newuser.aspx"">")
            anonymousAccess.AppendLine("<system.web>")
            anonymousAccess.AppendLine("<authorization>")
            anonymousAccess.AppendLine("<allow users=""*""/>")
            anonymousAccess.AppendLine("</authorization>")
            anonymousAccess.AppendLine("</system.web>")
            anonymousAccess.AppendLine("</location>")

            anonymousAccess.AppendLine("<location path = ""message.aspx"">")
            anonymousAccess.AppendLine("<system.web>")
            anonymousAccess.AppendLine("<authorization>")
            anonymousAccess.AppendLine("<allow users=""*""/>")
            anonymousAccess.AppendLine("</authorization>")
            anonymousAccess.AppendLine("</system.web>")
            anonymousAccess.AppendLine("</location>")

            Return anonymousAccess.ToString()
        End Function

        Public Shared Sub WritePostbackControlRegistration(ByRef pageBody As String)
            Dim registerPostbackControlsCall, registerPostbackControlsMethod As String

            GetPostbackControlRegistrations(registerPostbackControlsCall, registerPostbackControlsMethod)

            pageBody = MailFieldSubstitute(pageBody, "(RegisterPostbackControlsCall)", registerPostbackControlsCall)
            pageBody = MailFieldSubstitute(pageBody, "(RegisterPostbackControlsMethod)", registerPostbackControlsMethod)
        End Sub

    End Class
End Namespace

