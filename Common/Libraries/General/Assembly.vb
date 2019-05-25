Imports Microsoft.VisualBasic
Imports WhitTools.Email
Imports Common.General.Folders
Imports WhitTools.Utilities
Imports Common.General.Variables
Imports Common.General.Ancillary
Imports System.IO
Imports System.IO.File
Imports WhitTools.Getter

Namespace General
    Public Class Assembly
        Shared Sub AssemblePage(ByRef sPageBody As String, ByVal sProjectLocation As String, ByVal sFileName As String)
            Dim sTemplatePath As String = GetTemplatePath()

            Dim pageBody As New StringBuilder()
            Try
                pageBody.Append(GetMailFile(sTemplatePath & sProjectLocation & "\Header.eml"))
                pageBody.Append(GetMailFile(sTemplatePath & GetFileLocation() & sFileName & ".eml"))
                pageBody.Append(GetMailFile(sTemplatePath & sProjectLocation & "\Footer.eml"))

                sPageBody = pageBody.ToString()
            Catch ex As Exception
                logger.Info(sTemplatePath)
                logger.Info(sProjectLocation)
                logger.Info(GetFileLocation())
                logger.Info(sFileName)
                logger.Info(sTemplatePath & sProjectLocation & sFileName & ".eml")
            End Try
        End Sub

        Public Shared Function GetFileLocation() As String
            Return If(isFrontend,"Frontend\","Backend\")
         End Function
    End Class
End Namespace

