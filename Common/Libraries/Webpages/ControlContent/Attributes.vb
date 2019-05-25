Imports Microsoft.VisualBasic
Imports System.Data
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.DataTypes
Imports Common.General.Controls
Imports Common.General.ControlTypes

Namespace Webpages.ControlContent

    Public Class Attributes
        Shared Sub CheckAttribute(ByRef CurrentRow As DataRow, ByRef sContent As String, ByVal sCurrentAttribute As String, Optional ByVal sCurrentAttributeName As String = "", Optional ByVal bIsBool As Boolean = False)
            If AttributeAllowed(CurrentRow, sCurrentAttribute) Then
                With CurrentRow
                    Try
                        If CStr(.Item(sCurrentAttribute)) <> "" And CStr(.Item(sCurrentAttribute)) <> "-1" Then
                            If sCurrentAttributeName <> "" Then
                                sContent &= sCurrentAttributeName
                            Else
                                sContent &= sCurrentAttribute
                            End If

                            If sCurrentAttribute = "Visible" AndAlso .Item(sCurrentAttribute) = N_VISIBLE_CUSTOM_VALUE Then
                                sContent &= "=" & .Item("CustomVisibleValue") & " "
                            ElseIf sCurrentAttribute = "Visible" AndAlso (.Item(sCurrentAttribute) = N_VISIBLE_DEPENDENT_VALUE Or .Item(sCurrentAttribute) = N_VISIBLE_INVISIBLE_VALUE) Then
                                sContent &= "=""False"" "
                            ElseIf (sCurrentAttribute = "Text" Or sCurrentAttribute = "Placeholder") AndAlso .Item(sCurrentAttribute).contains("<%") Then
                                sContent &= " = '" & .Item(sCurrentAttribute) & "' "
                            Else
                                If bIsBool Then
                                    sContent &= "=""" & CBool(.Item(sCurrentAttribute)) & """ "
                                Else
                                    sContent &= "=""" & .Item(sCurrentAttribute) & """ "
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        logger.Error(.Item("Heading") & " - " & sCurrentAttribute & " - " & .Item(sCurrentAttribute) & " - " & sCurrentAttributeName & "<br /><br />" & ex.ToString & "<br /><br />")
                    End Try
                End With
            End If
        End Sub

        Shared Function AttributeAllowed(ByRef CurrentRow As DataRow, ByVal sCurrentAttribute As String) As Boolean
            If sCurrentAttribute = "Enabled" Or sCurrentAttribute = "Visible" Then
                Return CurrentRow(sCurrentAttribute) <> "1"
            ElseIf (sCurrentAttribute = "Rows" Or sCurrentAttribute = "Columns" Or sCurrentAttribute = "Autopostback") And GetControlDataType(CurrentRow.Item("ControlType")) = N_REPEATER_DATA_TYPE Then
                Return False
            ElseIf scurrentattribute = "Text" And GetControlDataType(currentrow.Item("controlType")) = N_CHECKBOX_DATATYPE And IsInsideHorizontalRepeater(CurrentRow.Item("ID")) Then
                Return False
            Else
                Return True
            End If
        End Function

        Shared Function GetErrorClass() As String
            Return "error"
        End Function

        Shared Sub AddCSSClass(ByRef CurrentRow As DataRow, ByRef content As String)
            Dim CSSClass As String = ""

            If CurrentRow.Item("CssClass") <> "" Then
                CSSClass = CurrentRow.Item("CssClass")
            End If

            If IsMultiLineTextbox(CurrentRow) Then
                CSSClass &= " multitextbox"
            End If

            If IsRichTextboxUser(CurrentRow) Then
                CSSClass &= " ckeditor"
            End If

            If RequiresUnorderedListLayout(CurrentRow) Then
                CSSClass &= " unstyled-list"

                If CurrentRow.Item("RepeatDirection") = "Horizontal" Then
                    CSSClass &= "-inline"
                End If
            End If

            If CurrentRow.Item("ControlType") = N_PREFIX_CONTROL_TYPE Then
                CSSClass &= " small"
            End If

            If GetControlDataType(CurrentRow.Item("ControlType")) = N_CHECKBOX_DATATYPE And CurrentRow.Item("Required") = "1" Then
                CSSClass &= " required"
            End If

            If CSSClass <> "" Then
                If GetControlDataType(CurrentRow.Item("ControlType")) = N_DIV_DATATYPE Then
                    content &= "class="""
                Else
                    content &= "CssClass="""
                End If

                content &= CSSClass.Trim() & """ "
            End If
         End Sub

        Shared Sub AddNewStyleAttributes(ByRef CurrentRow As DataRow, ByRef sContent As String)
            With CurrentRow
                If IsMultiLineTextbox(CurrentRow) Then
                    sContent &= "ValidationGroup=""TextBoxGroup"" "
                End If

                If RequiresUnorderedListLayout(CurrentRow) Then
                    sContent &= "RepeatLayout=""UnorderedList"" "
                End If
            End With
        End Sub
    End Class
End Namespace
