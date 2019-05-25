Imports System.Data
Imports Common.General
Imports Microsoft.VisualBasic
Imports WhitTools.SQL
Imports WhitTools.DataTables
Imports WhitTools.Utilities
Imports WhitTools.Getter
Imports Common.General.Ancillary
Imports Common.General.Actions
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Controls
Imports Common.General.ControlTypes
Imports Common.General.ProjectOperations
Imports Common.General.Pages
Imports Common.General.Repeaters
Imports Common.General.DataTypes
Imports Common.Webpages.Validation
Imports Common.Webpages.ControlContent.Heading
Imports Common.Webpages.ControlContent.Attributes
Imports Common.Webpages.ControlContent.Enclosures

Namespace Webpages.ControlContent
    Public Class Main
        Shared Function GetBodyContent() As String
            Dim sContent As String
            Dim dt As New DataTable

            If CurrentProjectRequiresWhitworthLogin() And pageNumber <= 1 Then
                WriteIdentityLabelControls(sContent)
            End If
            WriteDataTableValues(controlsDT)
            For nCounter As Integer = 0 To controlsDT.Rows.Count - 1
                With controlsDT.Rows(nCounter)
                    If NoParentControl(.Item("ParentControlID")) And ControlWriteAllowed(controlsDT.Rows(nCounter)) Then
                        Dim controlName = .Item("Name")
                        Dim controlID = .Item("ID")
                        Call New ContentWriter(controlsDT.Rows(nCounter)).GetControlContent(sContent)
                    End If
                End With
            Next

            If isWorkflow And IsLastPage() Then
                sContent &= "<asp:label id=""lblWorkflowStepID"" runat=""server"" Visible=""False"" Text=""" & projectDT.Rows(0).Item("WorkflowStep") & """ />"
            End If

            If ProjectIncludesProspectUserControl()Then
                sContent &= "<asp:label id=""lblSessionID"" runat=""server"" visible=""false""></asp:label>" & vbcrlf
            End If
            
            Return sContent
        End Function

        Shared Function ControlWriteAllowed(ByRef controlData As DataRow) As Boolean
            Return (BelongsToPage(pageNumber, controlData.Item("PageID")) And ControlDisplayAllowed(controlData.Item("DisplayLocation")))
        End Function

        Shared Sub WriteIdentityLabelControls(ByRef content As String)
            Dim loginColumns As DataTable = GetDataTable("Select * FROM " & DT_WEBRAD_PROJECTCOLUMNS & " PCL LEFT OUTER JOIN " & DT_WEBRAD_LOGINCOLUMNTYPES & " LCT ON PCL.ColumnControlID = LCT.IDNumber WHERE Type = 'Login' AND ProjectID = " & GetProjectID() & " Order by [Order] asc ")

            If isInsert Then
                AddUserAutocompleteControl(content)
            End If

            If loginColumns.Rows.Count > 0 Then
                content &= "<h3>Current User's Information</h3>"
                content &= "<div class=""stack-container form-group"">" & vbCrLf

                For Each Currentrow As DataRow In loginColumns.Rows
                    With Currentrow
                        content &= "<asp:panel id=""" & .Item("ControlName") & "Container"" runat=""server"" CssClass=""stack""><div class=""IdentityLabel"">"
                        content &= "<label for=""lbl" & .Item("ControlName") & """>" & .Item("Heading") & "</label>" & vbCrLf
                        content &= "<asp:Label ID=""lbl" & .Item("ControlName") & """ Runat=""server""  Visible=""True""></asp:Label>" & vbCrLf
                        content &= "</div></asp:Panel>" & vbCrLf & vbCrLf
                    End With
                Next

                content &= "</div>" & vbCrLf & vbCrLf
            End If

        End Sub

        Private Shared Sub AddUserAutocompleteControl(ByRef content As String)
            content &= "<div class=""form-group"">" & vbCrLf
            content &= "<asp:customvalidator id=""cvUser"" runat=""server"" errormessage=""Temp"" CssClass=""" & GetErrorClass() & """ OnServerValidate=""cvUser_ServerValidate"" />" & vbCrLf
            content &= "<br />" & vbCrLf
            content &= "<label for=""txtUser"" class=""required"">User (enter name or ID)</label>" & vbCrLf
            content &= "<asp:textbox ID=""txtUser"" Runat=""server"" CssClass=""SlText"" MaxLength=""50"" autopostback=""True"" OnTextChanged=""txtUser_TextChanged"" />" & vbCrLf
            content &= "<ajaxToolkit:AutoCompleteExtender ServiceMethod=""GetUserAutocompleteData""  MinimumPrefixLength=""3"" CompletionInterval=""100"" EnableCaching=""true"" CompletionSetCount=""10"" TargetControlID=""txtUser"" ID=""AutoCompleteExtender1"" runat=""server"" FirstRowSelected = ""false""></ajaxToolkit:AutoCompleteExtender>" & vbCrLf
            content &= "</div>" & vbCrLf & vbCrLf
        End Sub
    End Class
End Namespace
