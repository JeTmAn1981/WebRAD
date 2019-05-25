Imports System.Data
Imports Common.General
Imports General
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
Imports Common.Webpages.ControlContent
Imports Common.Webpages.Validation
Imports Common.Webpages.Validation.Main
Imports Common.Webpages.ControlContent.Heading
Imports Common.Webpages.ControlContent.Attributes
Imports Common.Webpages.ControlContent.Main
Imports System.Data.SqlClient

Namespace Webpages.ControlContent
    Public Class ContentWriter
        Private currentControl As DataRow
        Private currentToolkitType, currentPrefix, currentControlType, currentValueAttribute, parentControlID As String
        Private repeaterHasColumns As Boolean
        Private childControls, childControlTypes As DataTable
        Private repeaterLayoutType As String
        Private ew As EnclosuresWriter

        Public Sub New(ByVal control As DataRow)
            currentControl = control
            ew = New EnclosuresWriter(currentControl)
        End Sub

        Sub GetControlContent(ByRef sContent As String, Optional ByVal sNewRepeaterRow As String = "")
            Try
                With currentControl
                    SetupControlData(sContent)

                    AddContentBeforeControlDefinition(sContent)
                    AddControlDefinition(sContent)
                    AddContentAfterControlDefinition(sContent, sNewRepeaterRow)
                End With
            Catch ex As Exception
                logger.Error(ex.ToString & " - " & currentControl.Item("ParentControlID") & " - " & currentControl.Item("Name") & "<br /><Br />")
            End Try
        End Sub

        Private Sub AddContentBeforeControlDefinition(ByRef content As String)
            AddTableColumnOpening(content)
            ew.AddEnclosureOpenings(content)
            AddFloatLayoutMarkup(content)
            AddFileUploadAdditionalControls(content)
            AddValidators(content)
            AddHeading(content)
            AddRepeaterLayoutOpeningMarkup(content)
        End Sub

        Private Sub AddContentAfterControlDefinition(ByRef sContent As String, sNewRepeaterRow As String)
            AddLabel(sContent)
            AddRepeaterLayoutClosingMarkup(sContent)
            AddVerificationControl(sContent)
            AddFileUploadButton(sContent)
            AddAutoCompleteControl(sContent)
            AddFloatLayoutMarkup(sContent, False)
            ew.AddEnclosureClosings(sContent)
            AddHorizontalRepeaterLayoutClose(sContent, sNewRepeaterRow)
            AddCodeLinebreaks(sContent)
        End Sub

        Private Sub AddLabel(ByRef sContent As String)
            If GetControlDataType(currentControl.Item("controlType")) = N_CHECKBOX_DATATYPE And (IsInsideHorizontalRepeater(currentControl.Item("ID")) Or currentControl.Item("Text") = "") Then
                AddControlLabel(currentControl, sContent)
            End If
        End Sub

        Private Sub AddFileUploadButton(ByRef sContent As String)
            Dim isFileUpload = IsFileUploadControl(currentControl.Item("ControlType"))

            If IsFileUploadControl(currentControl.Item("ControlType")) And Not isArchive Then
                sContent &= "<div class=""stack"">" & vbCrLf
                sContent &= "<asp:button id=""btnUpload" & currentControl.Item("Name") & """ runat=""server"" CausesValidation=""false"" OnClick=""btnUpload" & currentControl.Item("Name") & "_OnClick"" CssClass=""button"" text=""Upload"" />" & vbCrLf
                sContent &= "</div>" & vbCrLf
                sContent &= "</div>" & vbCrLf
            End If
        End Sub

        Private Sub AddPrintHelper(ByRef sContent As String)
            If GetControlDataType(currentControl.Item("ControlType")) = N_TEXTBOX_DATA_TYPE And Not isPrintable Then

                With currentControl
                    sContent &= "<div id=""" & General.Controls.GetControlName(.Item("ID")) & "PrintHelper"" class=""PrintOnly""></div>" & vbCrLf
                End With
            End If
        End Sub

        Private Sub SetChildControls()
            With currentControl
                Dim selectString = "Select CT.*, '" & CleanSQL(.Item("Name")) & "' as Name, '" & CleanSQL(.Item("Heading")) & "' as Heading, CT.ID as ControlType, ToolkitType, Description, Prefix, ValueAttribute,ValidatorMessage From " & DT_WEBRAD_CONTROLTYPES & "  CT left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  DT on CT.DataType = DT.ID Where ParentControlTypeID = " & .Item("ControlType")
                Dim controlName = .Item("Name")

                Dim cmd As New SqlCommand(selectString, cnx)
                Dim dataSet As New DataSet()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = cmd
                adapter.Fill(dataSet)
                childControlTypes = dataSet.Tables(0)

                'childControlTypes = GetDataTable(selectString, General.Variables.cnx)
                childControls = GetImmediateChildControls(.Item("ID"))
            End With
        End Sub

        Private Sub AddHorizontalRepeaterLayoutClose(ByRef sContent As String, sNewRepeaterRow As String)
            With currentControl
                If ParentIsRepeaterControl(.Item("ID"), -1, 0, parentControlID) Then
                    If repeaterLayoutType = S_LAYOUTTYPE_HORIZONTAL And Not ParentIsControlType(.Item("ID"), "Panel") And .Item("Visible") = "1" Then
                        sContent &= "</td>" & vbCrLf

                        If sNewRepeaterRow <> "" Then
                            sContent &= sNewRepeaterRow & vbCrLf
                        End If
                    End If
                End If
            End With
        End Sub

        Private Sub AddAutoCompleteControl(ByRef sContent As String)
            With currentControl
                If .Item("SupplyDataType") = "Autocomplete" Then
                    sContent &= $"<ajaxToolkit:AutoCompleteExtender ServiceMethod=""Get{ .Item("Name")}AutocompleteData"""
                    sContent &= "    MinimumPrefixLength=""2"""
                    sContent &= " CompletionInterval=""100"" EnableCaching=""true"" CompletionSetCount=""10"""
                    sContent &= $" TargetControlID=""txt{ .Item("Name")}"""
                    sContent &= $" ID=""{ .Item("Name")}AutoCompleteExtender"" runat=""server"" FirstRowSelected = ""false"">"
                    sContent &= "</ajaxToolkit:AutoCompleteExtender>"
                End If
            End With
        End Sub

        Private Sub AddVerificationControl(ByRef sContent As String)
            With currentControl
                If .Item("RequireVerification") = "1" And Variables.isFrontend Then
                    sContent &= "<br />" & vbCrLf
                    sContent &= "<asp:CustomValidator ID=""cvVerify" & .Item("Name") & """ runat=""server"" OnServerValidate=""" & GetRepeaterHandlerReference(.Item("ID")) & "cvVerify" & .Item("Name") & "_ServerValidate"" CssClass=""" & GetErrorClass() & """ ErrorMessage=""Sorry, your entry for Verify " & .Item("Heading") & " did not match what you entered above.  Please try again.""></asp:CustomValidator>" & vbCrLf
                    sContent &= "<label for=""txtVerify" & .Item("Name") & """ class=""required"">Verify " & .Item("Heading") & "</label>" & vbCrLf

                    Dim dw As New DefinitionWriter(currentControl)
                    dw.WriteControlDefinition(sContent, currentToolkitType, currentControlType, currentPrefix, currentValueAttribute, "Verify" & .Item("Name"))
                End If
            End With
        End Sub


        Private Sub AddRepeaterLayoutClosingMarkup(ByRef sContent As String)
            With currentControl
                If ControlTypeIsRepeater(.Item("ControlType")) Then
                    If .Item("LayoutType") = S_LAYOUTTYPE_VERTICAL Then
                        sContent &= vbCrLf

                        If .Item("LayoutSubtype") = S_LAYOUTSUBTYPE_ORDEREDLIST Then
                            sContent &= "</ol>"
                        ElseIf .Item("LayoutSubtype") = S_LAYOUTSUBTYPE_UNORDEREDLIST Then
                            sContent &= "</ul>"
                        End If
                    ElseIf .Item("LayoutType") = S_LAYOUTTYPE_HORIZONTAL Then
                        sContent &= vbCrLf & "</table>"
                    End If
                End If
            End With
        End Sub

        Private Sub AddControlDefinition(ByRef sContent As String)
            Dim dw As DefinitionWriter

            If childControlTypes.Rows.Count > 0 Then
                For nCounter3 As Integer = 0 To childControlTypes.Rows.Count - 1
                    dw = New DefinitionWriter(childControlTypes.Rows(nCounter3))

                    dw.WriteControlDefinition(sContent, childControlTypes.Rows(nCounter3).Item("ToolkitType"), GetDataTypeDescription(childControlTypes.Rows(nCounter3).Item("DataType")), childControlTypes.Rows(nCounter3).Item("Prefix"), childControlTypes.Rows(nCounter3).Item("ValueAttribute"))
                Next
            Else
                dw = New DefinitionWriter(currentControl)
                dw.WriteControlDefinition(sContent, currentToolkitType, currentControlType, currentPrefix, currentValueAttribute)
            End If
        End Sub

        Private Sub AddRepeaterLayoutOpeningMarkup(ByRef sContent As String)
            With currentControl
                If ControlTypeIsRepeater(.Item("ControlType")) Then
                    If .Item("LayoutType") = S_LAYOUTTYPE_VERTICAL Then
                        sContent &= vbCrLf

                        If .Item("LayoutSubtype") = S_LAYOUTSUBTYPE_ORDEREDLIST Then
                            sContent &= "<ol>" & vbCrLf
                        ElseIf .Item("LayoutSubtype") = S_LAYOUTSUBTYPE_UNORDEREDLIST Then
                            sContent &= "<ul>" & vbCrLf
                        End If
                    ElseIf .Item("LayoutType") = S_LAYOUTTYPE_HORIZONTAL Then
                        sContent &= "<table class=""tablesaw tablesaw-stack"" data-tablesaw-mode=""stack"">" & vbCrLf
                        sContent &= "<thead id=""" & .Item("Name") & "Header"" runat=""server"">" & vbCrLf
                        sContent &= vbCrLf & "<tr>" & vbCrLf

                        If .Item("LayoutSubtype") = "4" Then
                            sContent &= vbCrLf & "<th data-tablesaw-priority=""#""><label>Selected Number</label></th>" & vbCrLf
                        End If

                        If Not Repeaters.RepeaterHasColumns(.Item("RepeatColumns")) Then
                            For nChildCounter As Integer = 0 To controlsDT.Rows.Count - 1
                                If ParentIsRepeaterControl(controlsDT.Rows(nChildCounter).Item("ID"), .Item("ID")) And controlsDT.Rows(nChildCounter).Item("Visible") = "1" Then
                                    sContent &= vbCrLf & "<th data-tablesaw-priority=""#"">"
                                    sContent &= "<label for=""" & General.Controls.GetControlName(controlsDT.Rows(nChildCounter).Item("ID")) & """" & If(.Item("Required") = "1" And Not isPrintable And Not isSearch, " class=""required""", "") & ">" & controlsDT.Rows(nChildCounter).Item("Heading") & "</label>" & vbCrLf
                                    sContent &= "</th>" & vbCrLf
                                End If
                            Next
                        End If

                        sContent &= vbCrLf & "</tr>" & vbCrLf
                        sContent &= "</thead>" & vbCrLf
                        sContent &= "<tbody id=""" & .Item("Name") & "Body"" runat=""server"">" & vbCrLf
                    End If
                End If
            End With
        End Sub

        Private Sub AddHeading(ByRef sContent As String)
            With currentControl
                If repeaterLayoutType <> S_LAYOUTTYPE_HORIZONTAL Or (repeaterLayoutType = S_LAYOUTTYPE_HORIZONTAL And repeaterHasColumns) And .Item("DisplayHeading") = "1" Or (ControlTypeIsRepeater(.Item("ControlType")) And .Item("Heading") <> "") Then
                    WriteControlHeading(sContent, currentControl, If(.Item("Visible") = "0", False, True))
                ElseIf repeaterLayoutType = S_LAYOUTTYPE_VERTICAL Or repeaterLayoutType = S_LAYOUTTYPE_HORIZONTAL Then
                    Dim nRepeatColumns As Integer

                    Try
                        nRepeatColumns = .Item("RepeatColumns")
                    Catch ex As Exception

                    End Try
                End If
            End With
        End Sub

        Private Sub AddValidators(ByRef content As String)
            If Not isPrintable And Not isSearch Then
                Dim validationWriter As ValidationControlWriter

                If childControlTypes.Rows.Count > 0 Then
                    For nCounter3 As Integer = 0 To childControlTypes.Rows.Count - 1
                        validationWriter = New ValidationControlWriter(childControlTypes.Rows(nCounter3))
                        validationWriter.Write(content)
                    Next
                Else
                    validationWriter = New ValidationControlWriter(currentControl)
                    validationWriter.Write(content)
                End If
            End If
        End Sub

        Private Function GetNewLine() As String
            Return "<br />" & vbCrLf
        End Function

        Private Sub SetControlInfo()
            With currentControl
                Try
                    currentToolkitType = If(.Item("ToolkitType") <> "", .Item("ToolkitType"), "asp")
                    currentPrefix = .Item("Prefix")
                    currentControlType = GetDataTypeDescription(.Item("DataType"))
                    currentValueAttribute = .Item("ValueAttribute")
                Catch ex As Exception

                End Try

            End With
        End Sub

        Private Sub AddFileUploadAdditionalControls(ByRef sContent As String)
            With currentControl
                If IsFileUploadControl(.Item("ControlType")) Then
                    WriteFileUploadAdditionalControls(sContent)
                End If
            End With
        End Sub

        Private Sub SetRepeaterInfo(ByRef sContent As String)
            With currentControl

                If ParentIsRepeaterControl(.Item("ID"), -1, 0, parentControlID) Then
                    repeaterLayoutType = GetControlColumnValue(parentControlID, "LayoutType", controlsDT)
                    repeaterHasColumns = Repeaters.RepeaterHasColumns(GetControlColumnValue(parentControlID, "RepeatColumns", controlsDT))
                End If
            End With
        End Sub

        Private Sub AddTableColumnOpening(ByRef sContent As String)
            With currentControl
                If ParentIsRepeaterControl(.Item("ID"), -1, 0, parentControlID) Then
                    If repeaterLayoutType = S_LAYOUTTYPE_HORIZONTAL And Not ParentIsControlType(.Item("ID"), "Panel") And .Item("Visible") = "1" Then
                        Try
                            sContent &= "<td>" & vbCrLf
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End With
        End Sub

        Private Function WriteFileUploadAdditionalControls(ByRef sContent As String)
            With currentControl
                sContent &= "<asp:panel id=""pnlUploaded" & .Item("Name") & """ runat=""server"" visible=""False"">" & vbCrLf
                sContent &= "<label AssociatedControlID=""lblUploaded" & .Item("Name") & """>Currently Uploaded " & .Item("Heading") & "</label>" & vbCrLf
                sContent &= "<asp:label id=""lblUploaded" & .Item("Name") & """ runat=""server""></asp:label>" & vbCrLf
                sContent &= "<asp:label id=""lblHiddenUploaded" & .Item("Name") & """ runat=""server"" visible=""False""></asp:label>" & vbCrLf
                sContent &= "</asp:panel>" & vbCrLf
            End With
        End Function



        Sub AddFloatLayoutMarkup(ByRef sContent As String, Optional ByVal bOpen As Boolean = True)
            With currentControl
                If .Item("DisplayType") = N_DISPLAYTYPE_FLOATNEWROW Or .Item("DisplayType") = N_DISPLAYTYPE_FLOAT Then
                    sContent &= If(bOpen, "<div class=""FloatLeft"">" & vbCrLf, "</div>" & vbCrLf)
                End If

                If .Item("DisplayType") <> N_DISPLAYTYPE_STANDARD Then
                    Try
                        Dim nNextDisplayType As Integer = GetControlColumnValue(GetDataTable("Select ID FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " AND PageID = " & .Item("PageID") & " AND Position = (SELECT MIN(Position) FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " AND PageID = " & .Item("PageID") & " AND Position > " & .Item("Position") & ")", General.Variables.cnx).Rows(0).Item("ID"), "DisplayType")

                        If (nNextDisplayType = N_DISPLAYTYPE_STANDARD Or N_DISPLAYTYPE_FLOATNEWROW) And Not bOpen Then
                            sContent &= "<div style=""clear:both;""></div>" & vbCrLf
                        End If
                    Catch ex As Exception
                        'Logger.Error(ex.ToString)
                    End Try
                End If
            End With
        End Sub

        Private Sub SetupControlData(ByRef sContent As String)
            SetChildControls()
            SetControlInfo()
            SetRepeaterInfo(sContent)
        End Sub

        Private Sub AddCodeLinebreaks(ByRef sContent As String)
            sContent &= vbCrLf & vbCrLf
        End Sub

        Public Shared Sub GetChildControls(ByRef sContent As String, ByRef CurrentRow As DataRow)
            With CurrentRow
                Dim ChildControlsDT As DataTable = GetImmediateChildControls(.Item("ID"))
                Dim sNewRepeaterRow As String

                For nCounter2 = 0 To ChildControlsDT.Rows.Count - 1
                    If ControlWriteAllowed(ChildControlsDT.Rows(nCounter2)) Then
                        sNewRepeaterRow = ""

                        If CStr(.Item("RepeatColumns")) <> "" And CStr(.Item("RepeatColumns")) <> "0" Then
                            If (nCounter2 + 1) Mod CInt(.Item("RepeatColumns")) = 0 And nCounter2 <> ChildControlsDT.Rows.Count - 1 Then
                                sNewRepeaterRow = "</tr><tr>"
                            End If
                        End If

                        Dim cw As New ContentWriter(ChildControlsDT.Rows(nCounter2))
                        cw.GetControlContent(sContent, sNewRepeaterRow)
                    End If
                Next
            End With
        End Sub


    End Class
End Namespace