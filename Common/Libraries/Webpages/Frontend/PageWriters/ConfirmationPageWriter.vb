Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.VisualBasic
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.Controls
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.DataTypes
Imports Common.General.Folders
Imports Common.General.ProjectOperations
Imports Common.General.DataSources
Imports Common.General.Actions
Imports Common.General.Columns
Imports Common.General.Assembly
Imports Common.General.Links
Imports Common.Webpages.Frontend.MultiPage
Imports Common.Webpages.Frontend.MessagePages
Imports Common.SQL.StoredProcedure
Imports Common.SQL.Main
Imports Common.SQL.Table
Imports Common.Webpages.Backend
Imports Common.Webpages.Frontend.Email
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Printable
Imports Common.Webpages.Backend.Export
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.Backend.Archive
Imports Common.Webpages.BindData
Imports Common.Webpages.Workflow
Imports Common.Webpages.Validation
Imports Common.Webpages.ControlContent.Main
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File

Public Class ConfirmationPageWriter
    Inherits PageWriter

    Public Sub New()
        MyBase.New()

        pageHTMLName = "ConfirmationHTML"
        pageVBName = "ConfirmationVB"
        pageDesignerName = "ConfirmationDesigner"
        pageType = "Confirmation"
        pageTitleAddition = " - Confirmation"
        pageFileName = "confirmation"
    End Sub

    Protected Overrides Sub WriteSpecificHTML()
        pageBody = MailFieldSubstitute(pageBody, "(HeadData)", GetHeadData(Nothing))
        pageBody = MailFieldSubstitute(pageBody, "(ConfirmationMessage)", GetCustomMessage())
    End Sub

    Function GetCustomMessage() As String
        If currentProject.CustomConfirmationPageMessage Then
            Return currentProject.ConfirmationMessage
        End If

        Return ""
    End Function

    Sub WriteSpecificFormLoad()
         AddFormLoadLink(loadformtext, pagetitle)
    End Sub
 End Class
