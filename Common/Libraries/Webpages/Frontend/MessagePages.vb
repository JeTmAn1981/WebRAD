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
Imports Common.General.Assembly
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Links
Imports Common.General.DataSources
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.Controls
Imports Common.General.Folders
Imports Common.General.DataTypes
Imports Common.General.Pages
Imports Common.SQL.Main
Imports Common.BuildSetup
Imports Common.Webpages.BindData
Imports Common.Webpages.Frontend.Main
Imports Common.Webpages.Frontend.MultiPage
Imports Common.Webpages.Backend.Main
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Archive
Imports System.Threading
Imports System.Reflection

Imports WhitTools.Utilities
Namespace Webpages.Frontend
    Public Class MessagePages
        Inherits Frontend.Main

        Shared Sub WriteNotFinishedPage()
            call new MessagePageWriter("notfinished", " - Application Not Finished", "notfinished",getnotfinishedtext()).writepage()
        End Sub

        Shared Sub WriteAlreadySubmittedPage()
            call new MessagePageWriter("alreadysubmitted", " - Application Already Submitted", "alreadysubmitted",GetAlreadySubmittedText()).writepage()
        End Sub

        Shared Function GetNotFinishedText() As String
            dim notFinishedText as string = "You must complete all previous sections of this form before proceeding to the "

            If DefaultCertificationPage() Then
                notfinishedtext &= "certification stage.  "
            Else
                notfinishedtext &= "final section.  " 
            End If

            notFinishedText &= "Please click the following link to return to the status page and review the previous sections:<Br /><br /><a href='status.aspx'>Status</a>" & vbCrLf

            Return notFinishedText
        End Function

        Shared Function GetAlreadySubmittedText() As String
            Return "Sorry, our records show you have already submitted this application once and may not submit it again."
        End Function

    End Class
End Namespace
