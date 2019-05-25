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

Public Class CertificationPageWriter
    Inherits PageWriter

    Public Sub New()
        MyBase.New()

        pageHTMLName = $"Index{eCommerce}HTML"
        pageVBName = $"Index{eCommerce}VB"
        pageDesignerName = $"Index{eCommerce}Designer"
        pagetype = "certification"
        pageTitleAddition = "Certification"
        pageFileName = "certification"
    End Sub


    protected overrides Sub WriteSpecificHTML()
        If DefaultCertificationPage() Then
                With projectDT.Rows(0)
                    pageBody = MailFieldSubstitute(pageBody, "(SubmitType)", "Certify")
                    pageBody = MailFieldSubstitute(pageBody, "(SubmitText)", "Certify")
                    pageBody = MailFieldSubstitute(pageBody, "(BodyContent)", GetCertificationText())
                    pageBody = MailFieldSubstitute(pageBody, "(InvoiceType)", .Item("EcommerceProduct"))
                End With
            End If
    End Sub

    protected overrides Sub WriteSpecificCodebehind()
        Dim sGetBindData, sGetBindDataAdditional, sGetBindRepeaterData, sGetDeleteAncillaryData, sCheckControlMethods, sendEmailCall, sendEmailMethod As String

        With projectDT.Rows(0)
            GetBindData(sGetBindData, sGetBindDataAdditional, sGetBindRepeaterData, sCheckControlMethods)

            If pageNumber <> -1 Then
                GetDeleteArchiveAncillaryData(sGetDeleteAncillaryData, "nCurrentID")
            End If

            pageBody = MailFieldSubstitute(pageBody, "(LoadDDLsContent)", GetCertificationLoadDDLsContent(GetAncillaryProject("RequireLogin")))
            pageBody = MailFieldSubstitute(pageBody, "(MainStoredProcedureType)", "Update")
            pageBody = MailFieldSubstitute(pageBody, "(MainStoredProcedureName)", .Item("SQLInsertStoredProcedureName") & "Certification")
            pageBody = MailFieldSubstitute(pageBody, "(MainStoredProcedureParameters)", "cmd.Parameters.AddWithValue(""@Username"", " & GetUsernameReference(GetAncillaryProject("RequireLogin")) & ")")
            pageBody = MailFieldSubstitute(pageBody, "(SubmitType)", "Certify")
            pageBody = MailFieldSubstitute(pageBody, "(CheckScheduleCall)", GetCheckScheduleCall())
            pageBody = MailFieldSubstitute(pageBody, "(Redirect)", "messages.RedirectToMessage(MessageCode.ApplicationComplete)")

            'Does this actually send e-mail?  Looks like conditions exclude multipage forms.
            'Don't think I have solved the problem of grabbing the e-mail address yet.
            GetEmailContent(sendEmailCall, sendEmailMethod)

            pageBody = MailFieldSubstitute(pageBody, "(SendEmailCall)", sendEmailCall)
            pageBody = MailFieldSubstitute(pageBody, "(SendEmailMethod)", sendEmailMethod)
        End With
     End Sub

    protected overrides Sub WriteSpecificFormLoad()
        AddFormLoadLink(loadformtext, pagetitle)
        AddFormLoadLink(loadFormText, "Status", "status.aspx")

    End Sub
 End Class
