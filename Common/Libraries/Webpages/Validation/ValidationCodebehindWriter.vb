Imports Common.General
Imports Common.Webpages
Imports Common.Webpages.Validation.main
Imports Common.General.ControlTypes
Imports Common.General.Variables
Imports WhitTools

Namespace Webpages.Validation
    Public Class ValidationCodebehindWriter
        Private controlInfo as DataRow
        Private controlName As String
        private validatorName As String
        private hiddenUploaded As String
        private currentPrefix As String
        private currentControlType As String
        Private validatorContent as String

        public Sub New (ByVal controlInfo As datarow)
            me.controlInfo = controlInfo
            SetControlNames()
        End Sub

        public  function GetValidationContent() as string
             WriteStandardValidator()
             WriteCustomValidator()

            return validatorContent     
         End function

         Private  Sub WriteCustomValidator()
            with controlinfo
                If CustomValidatorRequired(DataTables.GetDataTable("Select * From " & DT_WEBRAD_CONTROLTYPES & "  Where ID = " & controlinfo.Item("ControlType"), cnx).Rows(0).Item("CustomValidatorRequired"), .Item("SQLInsertItemTable"), .Item("TextMode"), .Item("CustomValidation")) Then
                 validatorContent &= "Public Sub " & General.Repeaters.GetRepeaterHandlerReference(.Item("ID")) & "cv" & .Item("Name") & "_ServerValidate(ByVal source As System.Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) "
                 validatorContent &= vbCrLf

                 If .Item("CustomValidationCode") <> "" Then
                     validatorContent &= .Item("CustomValidationCode") & vbCrLf
                 Else
                     If ControlTypeIsRepeater(.Item("ControlType")) And .Item("Required") = "1" Then
                         validatorContent &= " args.IsValid = ValidateRepeaterItems(" & controlName & "," & validatorName & ",args," & .Item("MinimumRequired") & "," & .Item("MaximumRequired") & ",""Sorry, you must select at least " & .Item("MinimumRequired") & " " & .Item("RepeaterItemName") & "(s)."",""Sorry, you must select at most " & .Item("MaximumRequired") & " " & .Item("RepeaterItemName") & "(s)."")" & vbCrLf
                     ElseIf .Item("IsListControl") = "1" And .Item("Required") = "1" Then
                         WriteListValidator()
                     ElseIf (.Item("IsCheckControl") = "1" Or currentControlType = "Radiobutton") And .Item("Required") = "1" Then
                         validatorContent &= " args.IsValid = " & controlName & ".Checked" & vbCrLf
                     ElseIf IsPhoneControl(controlInfo.Item("ControlType")) Then
                         validatorContent &= " args.IsValid = ValidatePhone(" & controlName & ".Text," & validatorName & ","""",args," & If(.Item("Required") = "1", "False", "True") & ")" & vbCrLf
                     ElseIf IsGLAccountControl(controlInfo.Item("ControlType")) Then
                            validatorContent &= " args.IsValid = WhitTools.Validator.ValidateGLAccount(" & controlName & ".Text," & validatorName & ","""",args," & If(.Item("Required") = "1", "False", "True") & ")" & vbCrLf
                        ElseIf IsControlType(.Item("ControlType"), "Date") Then
                         validatorContent &= " args.IsValid = ValidateDate(" & controlName & ".Text," & validatorName & ","""",args," & If(.Item("Required") = "1", "False", "True") & ")" & vbCrLf
                     ElseIf IsControlType(.Item("ControlType"), "IDNumber") Then
                         WriteIDNumberValidator()
                     ElseIf currentControlType = "Textbox" Then
                         WriteTextboxValidator()
                     ElseIf IsFileUploadControl(.Item("ControlType")) Then
                         WriteFileUploadValidator()
                     End If
                 End If

                 validatorContent &= "End Sub" & vbCrLf & vbCrLf
             End If
                End With
         End Sub

        Private Sub WriteFileUploadValidator()
            with controlInfo
                Dim sFilePath As String = If(projectDT.Rows(0).Item("IncludeFrontend") = "1", projectDT.Rows(0).Item("FrontendPath") & "\UploadedFiles\"" & Request.QueryString(""ID"") & ""\", "LocalPath")

                validatorContent &= "dim params as new ValidateUploadedFileParams("""  & sFilePath & """)" & vbcrlf

                    If .Item("Required") <> "1"
                            validatorContent &= "params.fileIsRequired = false" & vbcrlf
                    End If
                

                    validatorContent &= "params.maxFileSizeInBytes = " & CInt(.Item("MaxFileSize")) * 1024 & vbcrlf
                    validatorContent &= "params.SetFileTypesAllowed(""" & Getter.GetListofValues("Select * From " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & "  FTA left outer join " & DT_WEBRAD_FILETYPES & "  FT on FTA.FileType = FT.ID Where ControlID = " & .Item("ID"), "Extension") & """)" & vbcrlf
                    validatorContent &= "params.uploadedFileName = " & hiddenUploaded & ".text" & vbcrlf & vbcrlf
                    validatorContent &= "ValidateUploadedFile(" & controlName & ", params," & validatorname & ",args)" & vbcrlf
                end with
        End Sub


        Private Sub WriteTextboxValidator()
            with controlInfo
            If .Item("TextMode") = "MultiLine" Then
                validatorContent &= "args.IsValid = ValidateTextBox(" & controlName & "," & validatorName & ", " & Controls.GetErrorMessage("Please enter/select the following: ", .Item("Heading"), True) & ", " & If(.Item("SQLDataSize") = "MAX", 0, .Item("MaxLength")) & ", """",args,"

                If .Item("Required") = "1" Then
                    validatorContent &= "False)"
                Else
                    validatorContent &= "True)"
                End If

                validatorContent &= vbCrLf
            ElseIf .Item("Required") = "1" Then
                validatorContent &= " args.IsValid = " & controlName & ".Text <> """" " & vbCrLf
            End If
                end with
        End Sub

        Private Sub WriteIDNumberValidator()
            with controlInfo
            If .Item("Required") = "1" Then
                validatorContent &= "If " & controlName & ".Text = """" Then" & vbCrLf
                validatorContent &= "args.isvalid = false" & vbCrLf
                validatorContent &= "ctype(source,customvalidator).ErrorMessage = " & Controls.GetErrorMessage("Please enter/select the following: ", .Item("Heading"), True) & vbCrLf
                validatorContent &= "Else" & vbCrLf
            End If

            validatorContent &= "ctype(source,customvalidator).ErrorMessage = ""Sorry, you did not enter a valid Whitworth ID number.  Please try again.""" & vbCrLf
            validatorContent &= "args.isvalid = ValidateStudentIDExists(" & controlName & ".Text)" & vbCrLf

            If .Item("Required") = "1" Then
                validatorContent &= "End If" & vbCrLf
            End If
                end with
        End Sub

        Private Sub WriteListValidator()
            with controlinfo
                validatorContent &= " args.IsValid = ValidateListControl(" & controlName

            If .Item("MinimumRequired") <> "" Or .Item("MaximumRequired") <> "" Then
                validatorContent &= ",source,"""",args"

                If .Item("MinimumRequired") <> "" Then
                    validatorContent &= ","

                    If .Item("MinimumRequired") = "All" Then
                        validatorContent &= controlName & ".Items.Count"
                    Else
                        validatorContent &= .Item("MinimumRequired")
                    End If
                End If

                If .Item("MaximumRequired") <> "" And .Item("MaximumRequired") <> "0" Then
                    validatorContent &= "," & .Item("MaximumRequired")
                End If
            End If

            validatorContent &= ")" & vbCrLf
                End With
            
        End Sub

        Private  Sub WriteStandardValidator()
            with controlInfo
                If .Item("RequireVerification") = "1" And General.Variables.isFrontend Then
                    validatorContent &= "Public Sub " & General.Repeaters.GetRepeaterHandlerReference(.Item("ID")) & "cvVerify" & .Item("Name") & "_ServerValidate(ByVal source As System.Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) " & vbCrLf
                    validatorContent &= "args.IsValid = CBool(" & controlName & ".Text = " & Replace(controlName, "txt", "txtVerify") & ".Text Or " & controlName & ".Text = """")" & vbCrLf
                    validatorContent &= "End Sub" & vbCrLf & vbCrLf
                End If
            End With
         End Sub

         Private  Sub SetControlNames()
            With controlInfo
                 currentPrefix = .Item("Prefix")
                 currentControlType = DataTypes.GetDataTypeDescription(.Item("DataType"))

                If General.Repeaters.ParentIsRepeaterControl(.Item("ID")) Then
                 controlName = "ctype(GetParentRepeaterItem(source).FindControl(""" & currentPrefix & .Item("Name") & """)," & currentControlType & ")"
                 hiddenUploaded = "ctype(GetParentRepeaterItem(source).FindControl(""lblHiddenUploaded" & .Item("Name") & """),Label)"
                 validatorName = "ctype(GetParentRepeaterItem(source).FindControl(""cv" & .Item("Name") & """),CustomValidator)"
             Else
                 controlName = currentPrefix & .Item("Name")
                 hiddenUploaded = "lblHiddenUploaded" & .Item("Name")
                 validatorName = "cv" & .Item("Name")
             End If
            End With
          End Sub
    End Class
End NameSpace