Imports System.Data
Imports Common.Common.Webpages
Imports Common.Common.Webpages.Validation
Imports Microsoft.VisualBasic
Imports WhitTools.DataTables
Imports WhitTools.Getter
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Ancillary
Imports Common.General.ControlTypes
Imports Common.General.Controls
Imports Common.Webpages.ControlContent.Attributes
Imports Common.General.ProjectOperations
Imports Common.General.Repeaters
Imports Common.General.Pages
Imports Common.General.DataTypes

Namespace Webpages.Validation
    Public Class Main
        
        Shared Function GetValidatorContent() As String
            Dim nCounter As Integer
            Dim sValidatorContent, sCurrentPrefix, sCurrentControlType, sControlName, sHiddenUploaded, sValidatorName As String

            For nCounter = 0 To controlsDT.Rows.Count - 1
                With controlsDT.Rows(nCounter)
                    If ControlDisplayAllowed(.Item("DisplayLocation")) And BelongsToPage(pageNumber, .Item("PageID")) Then
                        sValidatorContent &= New ValidationCodebehindWriter(controlsDT.Rows(nCounter)).GetValidationContent()
                    End If
                End With
            Next

            WriteInsertUserValidation(sValidatorContent)

            Return sValidatorContent
        End Function

        
        Private Shared Sub WriteInsertUserValidation(ByRef sValidatorContent As String)
            If isInsert And CurrentProjectRequiresWhitworthLogin() Then
                sValidatorContent &= "Protected Sub cvUser_ServerValidate(source As Object, args As ServerValidateEventArgs)" & vbCrLf
                sValidatorContent &= "If txtUser.Text = """" Then" & vbCrLf
                sValidatorContent &= "cvUser.ErrorMessage = ""Please enter the user for this submission.""" & vbCrLf
                sValidatorContent &= "args.IsValid = False" & vbCrLf
                sValidatorContent &= "ElseIf GetUserInfo(ExtractUsername(txtUser.Text)).Rows.Count = 0 Then" & vbCrLf
                sValidatorContent &= "cvUser.ErrorMessage = ""Sorry, that user was not found.  Please enter another user.""" & vbCrLf
                sValidatorContent &= "args.IsValid = False" & vbCrLf
                sValidatorContent &= "End If" & vbCrLf & vbCrLf
                sValidatorContent &= "End Sub" & vbCrLf & vbCrLf
            End If
        End Sub

        Shared Function CustomValidatorRequired(ByVal sCustomValidatorRequired As Integer, Optional ByVal sSQLInsertItemTable As String = "", Optional ByVal sTextMode As String = "", Optional ByVal nCustomValidation As String = "") As Boolean
            If Not isPrintable And (sSQLInsertItemTable <> "" Or sCustomValidatorRequired = 1 Or sTextMode = "MultiLine" Or nCustomValidation = "1") Then
                Return True
            End If

            Return False
        End Function

        Shared Function MinimumRequiredAboveZero(ByVal sMinimumRequired As String) As Boolean
            Try
                If sMinimumRequired = "All" Or CInt(sMinimumRequired) > 0 Then
                    Return True
                End If
            Catch ex As Exception

            End Try

            Return False
        End Function
    End Class
End Namespace

