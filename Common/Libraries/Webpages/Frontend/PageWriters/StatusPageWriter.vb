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

Public Class StatusPageWriter
    Inherits PageWriter

    Public Sub New()
        MyBase.New()

        pageHTMLName = "StatusHTML"
        pageVBName = "StatusVB"
        pageDesignerName = "StatusDesigner"
        pagetype = "status"
        pageFileName = "status"
    End Sub


    Protected Overrides Sub WriteSpecificHTML()
        With projectDT.Rows(0)
            pageBody = MailFieldSubstitute(pageBody, "(NumberSections)", GetWordsPagecount())
            pageBody = MailFieldSubstitute(pageBody, "(SectionLinks)", GetSectionLinks())
            pageBody = MailFieldSubstitute(pageBody, "(SectionsAllowedCollection)", GetStatusSectionsAllowedCollection())
            pageBody = MailFieldSubstitute(pageBody, "(ChangePasswordLink)", If(GetAncillaryProject("RequireLogin") = "0", GetChangePasswordLink(), ""))
            pageBody = MailFieldSubstitute(pageBody, "(CustomMessage)", GetCustomMessage())
        End With
    End Sub

    Function GetCustomMessage() As String
        If currentProject.CustomStatusPageMessage Then
            Return currentProject.StatusMessage
        End If

        Return ""
    End Function

    Protected overrides Sub WriteSpecificCodebehind()
        pageBody = MailFieldSubstitute(pageBody, "(CheckReviewCall)", If(GetAncillaryProject("MultipleSubmissions") = "1", "CheckReviewInformation()", ""))
        pageBody = MailFieldSubstitute(pageBody, "(CheckScheduleCall)", GetCheckScheduleCall())
                pageBody = MailFieldSubstitute(pageBody, "(BindDataContent)", GetBindData("", "", "", ""))
                pageBody = MailFieldSubstitute(pageBody, "(ChangePasswordMethod)", If(GetAncillaryProject("RequireLogin") = "0", GetChangePasswordMethod(), ""))
    End Sub

    protected overrides Sub WriteSpecificFormLoad()
         AddFormLoadLink(loadformtext, pagetitle)
    End Sub
 End Class
