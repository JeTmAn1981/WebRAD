Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Common.Actions
Imports System.IO.File
Imports Microsoft.VisualBasic
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.Controls
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.File
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
Imports Common.Webpages.Validation.Main
Imports Common.Webpages.ControlContent.Main
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File
Imports System.Linq

Public Class IndexPageWriter
    Inherits PageWriter

    Public Sub New()
        MyBase.New()

        pageHTMLName = $"Index{ecommerce}HTML"
        pageVBName = $"Index{ecommerce}VB"
        pageDesignerName = $"Index{ecommerce}Designer"
        pageType = If(isInsert, "insert" & GetAncillaryName(), pageFileName)
        pageTitleAddition = If(isInsert, " - Insert", "") & GetSectionTitle(pageNumber)
        pageFileName = If(isInsert, "insert" & GetAncillaryName(), pageFileName)

        Common.Actions.Main.AddActionHandlers()
    End Sub

    Protected Overrides Sub WriteSpecificHTML()
        With projectDT.Rows(0)
            pageBody = MailFieldSubstitute(pageBody, "(RunAOPageHeader)", GetRunAOPageHeader())

            If isInsert Then
                mainPageName = "Main Listing"
                mainPageUrl = "index" & GetAncillaryName() & ".aspx"
            Else
                If GetPageCount() > 1 Then
                    mainPageName = "Status"
                    mainPageUrl = "status.aspx"
                End If

                pageBody = MailFieldSubstitute(pageBody, "(FormName)", pageTitle)
            End If

            If pageNumber = -1 Or (pageNumber = GetPageCount() And Not DefaultCertificationPage()) Then
                pageBody = MailFieldSubstitute(pageBody, "(SubmitType)", "Submit")
                pageBody = MailFieldSubstitute(pageBody, "(SubmitText)", "Submit")
            Else
                pageBody = MailFieldSubstitute(pageBody, "(SubmitType)", "Continue")
                pageBody = MailFieldSubstitute(pageBody, "(SubmitText)", "Save & Continue")
            End If

            pageBody = MailFieldSubstitute(pageBody, "(BodyContent)", GetBodyContent())
            pageBody = MailFieldSubstitute(pageBody, "(InvoiceType)", .Item("EcommerceProduct"))
            pageBody = MailFieldSubstitute(pageBody, "(AdditionalButtons)", additionalButtons)
            pageBody = MailFieldSubstitute(pageBody, "(CustomScript)", GetCustomScript())

            If isInsert Then
                pageBody = MailFieldSubstitute(pageBody, "(Navigation)", "<ul class=""unstyled-list-inline maintenance-items"">" & S_NAVIGATION_FILE_REFERENCE & "</ul>")
            End If

            If UseJavascriptActions Then
                pageBody = MailFieldSubstitute(pageBody, "(JavascriptActions)", projectActionData.js.handlers.ToString() & vbCrLf & vbCrLf & projectActionData.js.registrations)
            End If

            Try
                pageBody = MailFieldSubstitute(pageBody, "(HeadData)", GetHeadData(controlsDT))
            Catch ex As Exception
                logger.Error(ex.ToString)

            End Try
        End With
    End Sub

    Protected Overrides Sub WriteSpecificCodebehind()
        Dim sGetBindData, sGetbindDataAdditional, sGetBindRepeaterData, sGetDeleteAncillaryData, sTriggerPostbackActions As String
        Dim sendEmailCall, sendEmailMethod, saveAncillaryCalls, saveAncillaryMethods, checkControlMethods, saveWorkflowInfoCall, saveWorkflowInfoMethod As String
        Dim postbackActionsBuilder As New StringBuilder()

        GetBindData(sGetBindData, sGetbindDataAdditional, sGetBindRepeaterData, checkControlMethods)

        If pageNumber <> -1 Then
            GetDeleteArchiveAncillaryData(sGetDeleteAncillaryData, "nCurrentID")
            pageBody = MailFieldSubstitute(pageBody, "(CheckSectionAllowed)", GetSectionAllowedCheck(pageNumber))
        Else
            sGetBindData = ""
            sGetBindRepeaterData = ""
        End If

        pageBody = MailFieldSubstitute(pageBody, "(LoadDDLsContent)", GetLoadDDLs())
        pageBody = MailFieldSubstitute(pageBody, "(BindData)", sGetBindData)
        pageBody = MailFieldSubstitute(pageBody, "(WebServices)", GetWebServices())
        pageBody = MailFieldSubstitute(pageBody, "(BindDataAdditional)", sGetbindDataAdditional)
        pageBody = MailFieldSubstitute(pageBody, "(CheckScheduleCall)", GetCheckScheduleCall())
        pageBody = MailFieldSubstitute(pageBody, "(CallBindData)", If(sGetBindData <> "", "BindData()", ""))
        pageBody = MailFieldSubstitute(pageBody, "(CheckControlMethods)", checkControlMethods)
        pageBody = MailFieldSubstitute(pageBody, "(DeleteAncillaryData)", sGetDeleteAncillaryData)
        pageBody = MailFieldSubstitute(pageBody, "(DeleteTempFiles)", GetDeleteTempFiles())
        pageBody = MailFieldSubstitute(pageBody, "(MainStoredProcedureType)", If(pageNumber <> -1, "Update", "Insert"))
        pageBody = MailFieldSubstitute(pageBody, "(MainStoredProcedureName)", GetAncillaryProject("SQLInsertStoredProcedureName") & If(pageNumber <> -1, "Section" & pageNumber, ""))
        pageBody = MailFieldSubstitute(pageBody, "(MainStoredProcedureParameters)", GetMainStoredProcedureParameters())

        If pageNumber = -1 Or (pageNumber = GetPageCount() And Not DefaultCertificationPage()) Then
            pageBody = MailFieldSubstitute(pageBody, "(SubmitType)", "Submit")
        Else
            pageBody = MailFieldSubstitute(pageBody, "(SubmitType)", "Continue")
        End If

        pageBody = MailFieldSubstitute(pageBody, "(AdditionalButtonsMethods)", additionalButtonsMethods)
        pageBody = MailFieldSubstitute(pageBody, "(Redirect)", GetRedirectCommand(If(isInsert, "index" & GetAncillaryName() & ".aspx", redirectPage)))
        pageBody = MailFieldSubstitute(pageBody, "(ValidatorContent)", GetValidatorContent())
        pageBody = MailFieldSubstitute(pageBody, "(RepeaterMethods)", GetRepeaterAddRemoveMethods())

        GetCurrentOperations(pageBody)

        GetSaveAncillaryContent("nCurrentID", saveAncillaryCalls, saveAncillaryMethods)

        pageBody = MailFieldSubstitute(pageBody, "(SaveAncillaryCalls)", saveAncillaryCalls)
        pageBody = MailFieldSubstitute(pageBody, "(SaveAncillaryMethods)", saveAncillaryMethods)

        GetSaveWorkflowInfo(saveWorkflowInfoCall, saveWorkflowInfoMethod)

        pageBody = MailFieldSubstitute(pageBody, "(SaveWorkflowInfoCall)", saveWorkflowInfoCall)
        pageBody = MailFieldSubstitute(pageBody, "(SaveWorkflowInfoMethod)", saveWorkflowInfoMethod)

        GetEmailContent(sendEmailCall, sendEmailMethod)

        pageBody = MailFieldSubstitute(pageBody, "(SendEmailCall)", sendEmailCall)
        pageBody = MailFieldSubstitute(pageBody, "(SendEmailMethod)", sendEmailMethod)

        pageBody = MailFieldSubstitute(pageBody, "(FileUploadMethods)", GetFileUploadMethods())
        pageBody = MailFieldSubstitute(pageBody, "(ControlLifeCycleEvents)", GetControlLifeCycleEvents())

        WritePostbackControlRegistration(pageBody)

        If UseJavascriptActions Then
            pageBody = MailFieldSubstitute(pageBody, "(PostbackActions)", projectActionData.postback.handlers.ToString())
        Else
            GetPostbackActions(postbackActionsBuilder, sTriggerPostbackActions)
            pageBody = MailFieldSubstitute(pageBody, "(PostbackActions)", postbackActionsBuilder.ToString())
        End If

        If isInsert Then

            If UseJavascriptActions Then
                pageBody = MailFieldSubstitute(pageBody, "(TriggerPostbackActions)", projectActionData.postback.triggers)
            Else
                pageBody = MailFieldSubstitute(pageBody, "(TriggerPostbackActions)", sTriggerPostbackActions)
            End If
        End If
    End Sub

    Private Function GetDeleteTempFiles() As String

        Return IIf(db.ProjectControls.ToList().Where(Function(pc) IsFileUploadControl(If(pc.ControlType, 0))).Count > 0, "ClearTempFiles(""LocalPath\UploadedFiles"", Session.SessionID)", "")
    End Function

    Protected Overrides Sub WriteSpecificFormLoad()
        If isInsert Then
            AddMaintenanceHomeFormLoadLink(loadFormText)
        Else
            AddFormLoadLink(loadFormText, pageTitle)
            AddFormLoadLink(loadFormText, mainPageName, mainPageUrl)
        End If
    End Sub

    Protected Overrides Sub WriteSpecificDesigner()
        pageBody = MailFieldSubstitute(pageBody, "(DesignerControls)", GetDesignerControls(controlsDT))
    End Sub

    Private Function GetRedirectCommand(ByVal page As String) As String
        If page = "confirmation.aspx" Then
            Return "messages.RedirectToMessage(MessageCode.ApplicationComplete)"
        Else
            Return "Redirect(""" & page & """)"
        End If
    End Function
End Class
