Imports System.Data
Imports Microsoft.VisualBasic
Imports WhitTools.DataTables
Imports WhitTools.Getter
Imports Common.General.Main
Imports Common.General.Variables
Imports WhitTools.Utilities
Imports Common.General.Ancillary
Imports Common.General.ControlTypes
Imports Common.General.Controls
Imports Common.Webpages.ControlContent.Attributes
Imports Common.Webpages
Imports Common.General.ProjectOperations
Imports Common.General.Repeaters
Imports Common.General.Pages
Imports Common.General.DataTypes
Imports Common.Webpages.Validation.main

Namespace Webpages.Validation
    Public Class ValidationControlWriter
    Private controlInfo as datarow
    private validatorDisabled as string 

    Sub New( ByRef CurrentRow As DataRow)
        controlinfo = currentrow
        SetValidatorDisabled()
    End Sub

        Sub Write(ByRef content As String)
            With controlInfo
                If CustomValidatorRequired(GetDataTable("Select * From " & DT_WEBRAD_CONTROLTYPES & "  Where ID = " & .Item("ControlType"), General.Variables.cnx).Rows(0).Item("CustomValidatorRequired"), .Item("SQLInsertItemTable"), .Item("TextMode"), .Item("CustomValidation")) Then
                    WriteCustomValidator(content)
                Else
                    WriteRequiredFieldValidator(content)
                    WriteCompareValidator(content)
                    WriteRegularExpressionValidator(content)
                End If

                WriteAdditionalValidators(controlInfo, content)
            End With
        End Sub

         Private sub SetValidatorDisabled() 
            dim controlID as Integer = controlinfo.item("ID")
            dim currentControl As projectcontrol = db.ProjectControls.Where(function (control) control.id = controlid).First()
            dim disabledAttribute = " Enabled=""False"" "

            If UseJavascriptActions
                while db.ProjectControls.Where(function(control) control.ID = currentcontrol.ParentControlID).count > 0
                        if currentControl.Visible = 0
                            validatorDisabled =  disabledAttribute
                            exit sub
                        else
                            currentControl = db.ProjectControls.Where(function(control) control.ID = currentcontrol.ParentControlID).First()
                        End If

                End While

                if currentControl.Visible = 0
                    validatorDisabled =  disabledAttribute
                End If
            End If
        End Sub

        Private Sub WriteAdditionalValidators(controlinfo As DataRow, ByRef content As String)
            With controlinfo
                Dim ValidatorsDT As New DataTable
                Dim nValidatorsCount As Integer
                Dim sErrorMessage, sValidationExpression As String

                ValidatorsDT = GetDataTable(General.Variables.cnx, "Select Name, Prefix, ErrorMessage, ValidationExpression From " & DT_WEBRAD_CONTROLTYPEVALIDATORS & "  V left outer join " & DT_WEBRAD_VALIDATORTYPES & "  T on V.Type = T.ID Where ControlID = " & .Item("ControlType"))

                For nValidatorsCount = 0 To ValidatorsDT.Rows.Count - 1
                    If controlinfo.Item("ControlType") <> N_TEXTBOX_MULTILINE_CONTROLTYPE Or (controlinfo.Item("ControlType") = N_TEXTBOX_MULTILINE_CONTROLTYPE And controlinfo.Item("MaxLengthType") = "Words" And controlinfo.Item("MaxLength") <> "0") Then
                        content &= "<asp:" & ValidatorsDT.Rows(nValidatorsCount).Item("Name") & "" & validatorDisabled & " Display=""Dynamic"" CssClass=""" & GetErrorClass() & """ id=""" & ValidatorsDT.Rows(nValidatorsCount).Item("Prefix") & .Item("Name") & """ Runat=""server"""

                        sErrorMessage = ValidatorsDT.Rows(nValidatorsCount).Item("ErrorMessage")
                        sErrorMessage = Replace(sErrorMessage, "WordCount", controlinfo.Item("MaxLength"))

                        sValidationExpression = ValidatorsDT.Rows(nValidatorsCount).Item("ValidationExpression")
                        sValidationExpression = Replace(sValidationExpression, "WordCount", controlinfo.Item("MaxLength"))

                        If ValidatorsDT.Rows(nValidatorsCount).Item("Errormessage") <> "" Then
                            content &= " ErrorMessage='" & sErrorMessage & "'"
                        Else
                            content &= " ErrorMessage=" & GetErrorMessage("Please enter/select the following: ", .Item("Heading"))
                        End If

                        If ValidatorsDT.Rows(nValidatorsCount).Item("ValidationExpression") <> "" Then
                            content &= " ValidationExpression=""" & sValidationExpression & """"
                        End If

                        content &= " ControlToValidate=""" & .Item("Prefix") & .Item("Name") & """></asp:" & ValidatorsDT.Rows(nValidatorsCount).Item("Name") & ">" & vbCrLf
                    End If
                Next
            End With
        End Sub

        Private Sub WriteRequiredFieldValidator(ByRef content As String)
            With controlInfo
                If .Item("Required") = "1" Then
                    content &= "<asp:requiredfieldValidator Display=""Dynamic""" & validatorDisabled & " CssClass=""" & GetErrorClass() & """ id=""rfv" & .Item("Name") & """"
                    content &= " Runat=""server"" ErrorMessage=" & GetErrorMessage("Please enter/select the following: ", .Item("Heading")) & " ControlToValidate=""" & .Item("Prefix") & .Item("Name") & """></asp:RequiredFieldValidator>" & vbCrLf
                End If
            End With
        End Sub

        Private Sub WriteCompareValidator(ByRef content As String)
            With controlInfo
                If .Item("DataType") = N_TEXTBOX_DATA_TYPE And (.Item("SQLDataType") = N_SQL_INT_TYPE Or .Item("SQLDataType") = N_SQL_FLOAT_TYPE) Then
                    Dim sValidationDataType As String = If(.Item("SQLDataType") = N_SQL_INT_TYPE, "Integer", "Double")

                    content &= "<asp:CompareValidator id=""cmp" & .Item("Name") & """" & validatorDisabled & " runat=""server"" CssClass=""" & GetErrorClass() & """ Display=""Dynamic"" ControlToValidate=""" & .Item("Prefix") & .Item("Name") & """ Operator=""DataTypeCheck"" Type=""" & sValidationDataType & """ ErrorMessage=" & GetErrorMessage("Please enter/select the following using only numbers" & If(.Item("SQLDataType") = N_SQL_FLOAT_TYPE, " and at most one decimal point", "") & ": ", .Item("Heading")) & "></asp:CompareValidator>" & vbCrLf

                    Dim minValue, maxValue As Decimal

                    If Decimal.TryParse(.Item("MinimumValue"), minValue) Then
                        content &= "<asp:CompareValidator id=""cmp" & .Item("Name") & "Minimum""" & validatorDisabled & " runat=""server"" CssClass=""" & GetErrorClass() & """ Display=""Dynamic"" ControlToValidate=""" & .Item("Prefix") & .Item("Name") & """ Operator=""GreaterThanEqual"" ValueToCompare=""" & minValue & """ Type=""" & sValidationDataType & """ ErrorMessage=" & GetErrorMessage("Please enter/select the following using a number that is greater than or equal to " & minValue & ": ", .Item("Heading")) & "></asp:CompareValidator>" & vbCrLf
                    End If

                    If Decimal.TryParse(.Item("MaximumValue"), maxValue) Then
                        content &= "<asp:CompareValidator id=""cmp" & .Item("Name") & "Maximum""" & validatorDisabled & " runat=""server"" CssClass=""" & GetErrorClass() & """ Display=""Dynamic"" ControlToValidate=""" & .Item("Prefix") & .Item("Name") & """ Operator=""LessThanEqual"" ValueToCompare=""" & maxValue & """ Type=""" & sValidationDataType & """ ErrorMessage=" & GetErrorMessage("Please enter/select the following using a number that is less than or equal to " & maxValue & ": ", .Item("Heading")) & "></asp:CompareValidator>" & vbCrLf
                    End If
                End If

            End With
        End Sub

        Private Sub WriteRegularExpressionValidator(ByRef content As String)
            With controlInfo
                If .Item("DataType") = N_TEXTBOX_DATA_TYPE AndAlso .Item("SQLDataType") = N_SQL_MONEY_TYPE Then
                    content &= "<asp:RegularExpressionValidator id=""rev" & .Item("Name") & "Currency"" CssClass=""" & GetErrorClass() & """  ValidationExpression=""\$?(\d+)?\.?(\d+)?"" ControlToValidate=""txt" & .Item("Name") & """ ErrorMessage=" & GetErrorMessage("Please enter a valid currency amount:", .Item("Heading")) & " runat=""server""></asp:RegularExpressionValidator>" & vbCrLf
                End If
            End With
        End Sub

        Private Sub WriteCustomValidator(ByRef content As String)
            With controlInfo
                Dim sName, sHeading As String

                sName = .Item("Name")
                sHeading = FormatControlHeading(.Item("Heading"))

                If .Item("Heading") = "" Then
                    sHeading = FormatControlHeading(.Item("Name"))
                End If

                content &= "<asp:CustomValidator Display=""Dynamic"" CssClass=""" & GetErrorClass() & """ id=""cv" & .Item("Name") & """ " & validatorDisabled & " Runat=""server"" ErrorMessage="

                If .Item("ValidatorMessage") <> "" Then
                    If .Item("ShortHeading").contains("<%#") Then
                        content &= "'" & Replace(.Item("ValidatorMessage"), "ShortHeadingHere", .Item("ShortHeading")) & "'"
                    Else
                        content &= """" & Replace(.Item("ValidatorMessage"), "ShortHeadingHere", .Item("ShortHeading")) & """"
                    End If

                ElseIf ControlTypeIsRepeater(.Item("ControlType")) And .Item("RepeaterAddRemove") = "1" And .Item("Required") = "1" Then
                    content &= """Please add at least " & .Item("MinimumRequired") & " " & .Item("RepeaterItemName") & "(s)."""
                Else
                    content &= GetErrorMessage("Please enter/select the following: ", sHeading)
                End If

                content &= " OnServerValidate=""" & GetRepeaterHandlerReference(.Item("ID")) & "cv" & .Item("Name") & "_ServerValidate"""

                content &= "></asp:CustomValidator>" & vbCrLf
            End With
        End Sub

End Class
End Namespace