Imports Common.General
Imports WhitTools
imports whittools.Utilities
Imports System.Data
Imports Common.General.main
Imports Common.general.variables
Imports Common.general.repeaters
Imports Common.Actions.main
Imports Common.Webpages.ControlContent.Attributes
Imports Common.General.DataTypes
Imports Common.General.controlTypes
Imports Common.general.controls
Imports Common.Webpages.ControlContent
Imports Common.Webpages.ControlContent.Main
Imports Common.Webpages.ControlContent.ContentWriter
Imports Common.General.Scripts

Namespace Webpages.ControlContent
Public Class DefinitionWriter
            Private  currentControl As DataRow

            Public Sub New(ByVal control As datarow)
                currentcontrol = control
            End Sub

          Sub WriteControlDefinition(ByRef sContent As String, ByVal sToolkitType As String, ByVal sControlType As String, ByVal sPrefix As String, ByVal sValueAttribute As String, Optional ByVal sSubstituteName As String = "")
            With currentControl
                Dim sName As String = If(sSubstituteName <> "", sSubstituteName, .Item("Name"))
                Dim dt As DataTable = DataTables.GetDataTable(cnx, "Select * From " & DT_WEBRAD_CONTROLDATATYPES & " WHERE ID = (Select DataType FROM " & DT_WEBRAD_CONTROLTYPES & " WHERE ID = " & .Item("controlType") & ")")

                If IsFileUploadControl(.Item("ControlType")) Then
                    sContent &= "<div class=""stack-container"">" & vbCrLf
                    sContent &= "<div class=""stack"">" & vbCrLf
                End If

                If GetControlDataType(.Item("ControlType")) = N_DIV_DATATYPE Then
                    sContent &= "<div " & .Item("HTMLAttributes")
                    AddCSSClass(currentControl, sContent)
                    sContent &= ">" & vbCrLf
                    GetChildControls(sContent, currentControl)
                    sContent &= "</div>" & vbCrLf
                ElseIf GetControlDataType(.Item("ControlType")) = N_USERCONTROL_DATATYPE Then
                    sContent &= "<uc:" & .Item("Name") & " id=""uc" & .Item("Name") & """ runat=""server""></uc:" & .Item("Name") & ">" & vbCrLf
                ElseIf IsLiteralControlType(.Item("ControlType")) Then
                    sContent &= .Item("Value")
                Else
                    If isPrintable And DataTypeRequiresLabelOnPrintable(GetControlDataType(.Item("controlType"))) Then
                        sContent &= "<asp:label ID=""lbl" & sName & """ Runat=""server"" />"
                    Else
                        Dim sDeclaration As String

                        AddOpeningDeclaration(sContent, dt, sName, sToolkitType, sDeclaration, sPrefix, sValueAttribute)

                        If ControlTypeIsRepeater(.Item("ControlType")) Then
                            sContent &= vbCrLf & "<ItemTemplate>" & vbCrLf

                            If .Item("LayoutType") = S_LAYOUTTYPE_VERTICAL Then
                                If .Item("LayoutSubtype") <> S_LAYOUTSUBTYPE_NOLIST Then
                                    sContent &= "<li>" & vbCrLf
                                End If

                                If .Item("RepeaterAddRemove") = "1" And Not isPrintable Then
                                    sContent &= General.Repeaters.GetRepeaterRemoveControl(currentControl)
                                End If
                            ElseIf .Item("LayoutType") = S_LAYOUTTYPE_HORIZONTAL Then
                                sContent &= "<tr>" & If(.Item("LayoutSubtype") = "4", "<td><strong><%# Container.ItemIndex + 1 %></strong></td>", "") & vbCrLf
                            End If
                        End If

                        GetChildControls(sContent, currentControl)

                        Try
                            If CStr(.Item("SelectionItems")) <> "-1" And CStr(.Item("SelectionItems")) <> "" Then
                                sContent &= vbCrLf

                                If .Item("IncludePleaseSelect") = "1" Then
                                    sContent &= "<asp:listitem value=""""></asp:listitem>" & vbCrLf
                                End If

                                Dim ListItemsDT As New DataTable
                                ListItemsDT = DataTables.GetDataTable(cnx, "Select * from " & DT_WEBRAD_PROJECTCONTROLLISTITEMS & "  Where Type = 1 AND ParentID = " & .Item("ID"))

                                For nCounter2 = 0 To ListItemsDT.Rows.Count - 1
                                    sContent &= "<asp:listitem value=""" & ListItemsDT.Rows(nCounter2).Item("Value") & """" & GetListItemDefaultSelected(isInsert, .Item("Required"), ListItemsDT.Rows(nCounter2).Item("Text"), ListItemsDT.Rows(nCounter2).Item("Value")) & ">" & ListItemsDT.Rows(nCounter2).Item("Text") & "</asp:listitem>" & vbCrLf
                                Next
                            End If
                        Catch ex As Exception
                            'Logger.Error(ex.ToString)
                        End Try

                        If ControlTypeIsRepeater(.Item("ControlType")) Then
                            If .Item("LayoutType") = S_LAYOUTTYPE_HORIZONTAL Then
                                If .Item("RepeaterAddRemove") = "1" Then
                                    sContent &= "<td>" & vbCrLf
                                    sContent &= General.Repeaters.GetRepeaterRemoveControl(currentControl)
                                    sContent &= "</td>" & vbCrLf
                                End If

                                sContent &= "</tr>" & vbCrLf

                                ' sContent &= WrapInTablesawRow("<hr style=""width:100%;"" />" & vbCrLf)
                            Else
                                ' sContent &= "<hr style=""width:100%;"" />" & vbCrLf
                            End If

                            sContent &= "<asp:label id=""lblID"" runat=""server"" visible=""false"" text='<%# Container.DataItem(""ID"") %>' />" & vbCrLf

                            If .Item("LayoutType") = S_LAYOUTTYPE_VERTICAL And .Item("LayoutSubtype") <> "3" Then
                                sContent &= "</li>" & vbCrLf
                            ElseIf .Item("LayoutType") = S_LAYOUTTYPE_HORIZONTAL Then
                                sContent &= "</tr>" & vbCrLf

                            End If

                            sContent &= "</ItemTemplate>" & vbCrLf
                        End If

                        If GetDataTypeDescription(.Item("DataType")) = "Label" And Not ContainsCodeBlock(.Item("Value")) Then
                            If .Item("Value") <> "" Then
                                sContent &= .Item("Value")
                            End If
                        End If

                        sContent &= "</" & sDeclaration & ">"
                        If .Item("LayoutType") = S_LAYOUTTYPE_HORIZONTAL Then
                            sContent &= "</tbody>" & vbCrLf

                        End If
                        If .Item("RepeaterAddRemove") = "1" And Not isPrintable Then
                            Dim sRepeaterAddControl As String = GetRepeaterAddControl(currentControl)

                            If .Item("LayoutType") = S_LAYOUTTYPE_VERTICAL Then
                                sContent &= sRepeaterAddControl & vbCrLf
                            ElseIf .Item("LayoutType") = S_LAYOUTTYPE_HORIZONTAL Then
                                sContent &= General.Repeaters.WrapInTablesawRow(sRepeaterAddControl)
                            End If
                        End If
                    End If
                End If

                If IsFileUploadControl(.Item("ControlType")) Then
                    sContent &= "</div>" & vbCrLf
                End If

            End With
        End Sub

           
            Private  Sub AddOpeningDeclaration(ByRef sContent As String, dt As DataTable, sName As String, sToolkitType As String, ByRef sDeclaration As String, sPrefix As String, byval sValueAttribute As string)
            with currentcontrol
            sDeclaration = Controls.GetControlDeclaration(sToolkitType, dt.Rows(0).Item("Description"), .Item("ControlType"))

            If .Item("ControlType") = N_CHECKBOX_CONTROL_TYPE And .Item("TextPosition") = "Before" Then
                sContent &= .Item("Text")
            End If

            sContent &= "<" & sDeclaration & " ID=""" & sPrefix & sName & """ Runat=""server"""

            AddAttributes(sContent, sName, sPrefix, sValueAttribute)

            sContent = Trim(sContent) & ">"
                end with
        End Sub

        Private  Sub AddAttributes(ByRef sContent As String, sName As String, sPrefix As String, sValueAttribute As String)
            with currentControl
                AddEventAttributes(sContent, sPrefix, sName)
                AddValueAttributes(sContent,sValueAttribute)
                AddDisplayAttributes(sContent)
                AddMaxlengthAttribute(sContent)
                AddCalendarContent(sContent)
                AddTimeContent(sContent)
                CheckAttribute(currentControl, sContent, "Enabled", "", True)
                CheckAttribute(currentControl, sContent, "SelectionMode")
                CheckAttribute(currentControl, sContent, "GroupName")
                End With
         End Sub

        Private Sub AddCalendarContent(ByRef sContent As String)
            If currentControl.Item("Calendar") = "1"
                    scontent &= "onfocus=""makeDatePicker(this)"" "
                End If
         End Sub

        Private Sub AddTimeContent(ByRef sContent As String)
            If IsTimePickerControl(currentControl.item("ControlType"))
                    scontent &= "onfocus=""MakeTimePicker(this)"" "
                End If
         End Sub

        Private Sub AddDisplayAttributes(ByRef sContent As String)
            CheckAttribute(currentControl, sContent, "Placeholder")
            AddVisibleAttribute(sContent)
            CheckAttribute(currentControl, sContent, "TextMode")
            CheckAttribute(currentControl, sContent, "Rows")
            CheckAttribute(currentControl, sContent, "Columns")
            AddNewStyleAttributes(sContent)
            AddCSSClass(currentControl, sContent)
        End Sub

        Private  Sub AddRepeatDirectionAttribute(ByRef sContent As String)
            If RequiresUnorderedListLayout(currentControl) Then
                sContent &= "RepeatDirection = ""Vertical"" "
            End If
        End Sub

        Private  Sub AddNewStyleAttributes(ByRef sContent As String)
                Attributes.AddNewStyleAttributes(currentControl, sContent)
        End Sub

        Private  Sub AddMaxlengthAttribute(ByRef sContent As String)
            If currentcontrol.Item("TextMode") <> "MultiLine" Then
                CheckAttribute(currentControl, sContent, "MaxLength")
            End If
        End Sub

        
        Private  Sub AddVisibleAttribute(ByRef sContent As String)
            if IsDataType(currentControl.Item("ControlType"),"Panel") and not (UseJavascriptActions And ControlIsVisibleActionTarget(currentControl.Item("ID")))
                CheckAttribute(currentControl, sContent, "Visible", "", True)
            End If
        End Sub

        Private  Sub AddEventAttributes(ByRef sContent As String, sPrefix As String, sName As String)
            AddPostbackActionReference(sContent, sPrefix)

            sContent &= " "

            AddControlLifecycleEventReferences(sContent)
            AddAutopostback(sContent)
            AddJavascriptOnchangeReference(sContent)
        End Sub

            Private Sub AddControlLifecycleEventReferences(ByRef sContent As String)
                AddRepeaterOnItemCreatedEvent(sContent)
            End Sub

            Private Sub AddRepeaterOnItemCreatedEvent(ByRef sContent As String)
                If ControlTypeIsRepeater(currentControl.Item("ControlType")) Then

                If RepeaterRequiresPostbackTrigger(currentControl.Item("ID")) And Not isArchive Then
                    sContent &= " OnItemCreated=""rpt" & currentControl.Item("Name") & "_OnItemCreated"""
                End If
            End If
            End Sub
        
            Private  Sub AddPostbackActionReference(ByRef sContent As String, sPrefix As String)
            If Not isSearch And (PostbackHandlerRequired(currentControl.Item("ID")) Or currentControl.Item("ListSelections") = "1") Then
                sContent &= General.Actions.GetControlPostbackActionReference(currentControl, sPrefix)
            End If
        End Sub

        Private  Sub AddTextAttribute(ByRef sContent As String)
            with currentcontrol
            If Not (.Item("ControlType") = N_CHECKBOX_CONTROL_TYPE And .Item("TextPosition") = "Before") Then
                CheckAttribute(currentControl, sContent, "Text")
            End If
                end with
        End Sub

        Private  Sub AddValueAttribute(ByRef sContent As String, sValueAttribute As String)
            with currentControl
            If .Item("Value") <> "" Then
                If GetDataTypeDescription(.Item("DataType")) <> "Label" Then
                    CheckAttribute(currentcontrol, sContent, "Value", sValueAttribute)
                elseIf GetDataTypeDescription(.Item("DataType")) = "Label" and ContainsCodeBlock(.Item("Value")) Then
                    'Content inside code blocks won't make it through a postback if it's not inside
                    'single quotes rather than double quotes.  If additional concatenation with strings is needed
                    'with this content, that should all be done inside a single code block.
                    sContent &= " Text = '" & .Item("Value") & "' "
                End If
            End If
                end with
        End Sub

        Private  Sub AddJavascriptOnchangeReference(ByRef sContent As String)
            with currentcontrol
            If .Item("Onchange") = "1" Then
                sContent &= " onchange = """ & Replace(.Item("OnchangeCall"), """", "'") & """ "
            End If
                end with
        End Sub

        Private Sub AddAutopostback(ByRef sContent As String)
            With currentControl
                If (((.Item("Autopostback") = "1" Or .Item("PerformPostbackAction") = "1") And Not isSearch) Or .Item("ListSelections") = "1") And AttributeAllowed(currentControl, "Autopostback") And Not (UseJavascriptActions And Not PostbackHandlerRequired(currentControl.Item("ID"))) Then
                    sContent &= " Autopostback=""True"" "
                End If
            End With
        End Sub

        Private  Sub AddValueAttributes(ByRef sContent As String, sValueAttribute As String)
            AddValueAttribute(sContent, sValueAttribute)
            AddTextAttribute(sContent)
        End Sub
End Class
end namespace