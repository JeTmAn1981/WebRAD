Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.VisualBasic
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.Controls
Imports common.general.file
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.Actions.main
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
Imports Common.Webpages.Validation.main
Imports Common.Webpages.ControlContent.Main
imports common.webpages.backend.main
imports common.webpages.backend.ecommerce
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File

Public Class UpdatePageWriter
    inherits PageWriter

    Dim archive As Boolean

    Public Sub New(ByVal archive As Boolean)
        MyBase.New()

        Me.archive = archive

        pageHTMLName = $"{GetBackendUpdateReference(True)}HTML"
        pageVBName = $"{GetBackendUpdateReference(True)}VB"
        pageDesignerName = $"{GetBackendUpdateReference(True)}Designer"
        pageType = If(IsSingletonProject(), "index", "update") & GetAncillaryName()
        pageTitleAddition = " - "" & pageName & "" " & GetAncillaryName(True)
        pageFileName = If(IsSingletonProject(), "index", "update") & GetAncillaryName()

        AddActionHandlers()
    End Sub

    Protected overrides Sub WriteSpecificHTML()
            With projectDT.Rows(0)
                pageBody = MailFieldSubstitute(pageBody, "(RunAOPageHeader)", GetRunAOPageHeader)
            pageBody = MailFieldSubstitute(pageBody, "(Navigation)", S_NAVIGATION_FILE_REFERENCE)
            pageBody = MailFieldSubstitute(pageBody, "(BodyContent)", GetBodyContent())
            pageBody = MailFieldSubstitute(pageBody, "(ECommerceContent)", GetUpdateEcommerceContent())
                pageBody = MailFieldSubstitute(pageBody, "(HeadData)", GetHeadData(controlsDT))
                pageBody = MailFieldSubstitute(pageBody, "(InvoiceType)", .Item("EcommerceProduct"))
                pageBody = MailFieldSubstitute(pageBody, "(WorkflowSteps)", GetWorkflowSteps())
                pageBody = MailFieldSubstitute(pageBody, "(CustomScript)", GetCustomScript())

                If UseJavascriptActions Then
                pageBody = MailFieldSubstitute(pageBody, "(JavascriptActions)", projectActionData.js.handlers.ToString() & vbCrLf & vbCrLf & projectActionData.js.registrations)
            End If
            End with
        End Sub

    Protected Overrides Sub WriteSpecificCodebehind()
        Dim saveAncillaryCalls, saveAncillaryMethods, triggerPostbackActions, bindData, getBindDataAdditional, bindRepeaterData, checkControlMethods, getDeleteAncillaryData, editInvoiceMethod, assignInvoiceInfo As String
        Dim postbackActionsBuilder As New StringBuilder()

        GetSaveAncillaryContent("nCurrentID", saveAncillaryCalls, saveAncillaryMethods)
        GetBindData(bindData, getBindDataAdditional, bindRepeaterData, checkControlMethods)
        GetDeleteArchiveAncillaryData(getDeleteAncillaryData, "Request.Querystring(""ID"")")
        GetInvoiceMethodInfo(editInvoiceMethod, assignInvoiceInfo)

        pageBody = MailFieldSubstitute(pageBody, "(LoadDDLsContent)", GetLoadDDLs())
        pageBody = MailFieldSubstitute(pageBody, "(WebServices)", GetWebServices())
        pageBody = MailFieldSubstitute(pageBody, "(MainUpdateProcedureName)", GetAncillaryProject("SQLUpdateStoredProcedureName"))
        pageBody = MailFieldSubstitute(pageBody, "(MainUpdateProcedureParameters)", GetMainStoredProcedureParameters())
        pageBody = MailFieldSubstitute(pageBody, "(MainTableName)", GetAncillaryProject("SQLMainTableName"))
        pageBody = MailFieldSubstitute(pageBody, "(BindData)", bindData)
        pageBody = MailFieldSubstitute(pageBody, "(SetUsernameCall)", GetSetUsernameCall())
        pageBody = MailFieldSubstitute(pageBody, "(BindDataAdditional)", getBindDataAdditional)
        pageBody = MailFieldSubstitute(pageBody, "(BindRepeaterData)", bindRepeaterData)
        pageBody = MailFieldSubstitute(pageBody, "(BindWorkflowSteps)", GetBindWorkflowSteps())
        pageBody = MailFieldSubstitute(pageBody, "(CheckControlMethods)", checkControlMethods)
        pageBody = MailFieldSubstitute(pageBody, "(EditInvoiceMethod)", editInvoiceMethod)
        pageBody = MailFieldSubstitute(pageBody, "(AssignInvoiceInfo)", assignInvoiceInfo)
        pageBody = MailFieldSubstitute(pageBody, "(SaveAncillaryCalls)", saveAncillaryCalls)
        pageBody = MailFieldSubstitute(pageBody, "(SaveAncillaryMethods)", saveAncillaryMethods)
        pageBody = MailFieldSubstitute(pageBody, "(DeleteAncillaryData)", getDeleteAncillaryData)
        pageBody = MailFieldSubstitute(pageBody, "(DeleteTempFiles)", GetDeleteTempFiles())

        pageBody = MailFieldSubstitute(pageBody, "(ValidatorContent)", GetValidatorContent())
        pageBody = MailFieldSubstitute(pageBody, "(RepeaterMethods)", GetRepeaterAddRemoveMethods())
        pageBody = MailFieldSubstitute(pageBody, "(AncillaryReference)", GetAncillaryName())
        pageBody = MailFieldSubstitute(pageBody, "(HomepageName)", GetHomepagePageName())
        pageBody = MailFieldSubstitute(pageBody, "(FileUploadMethods)", GetFileUploadMethods())
        pageBody = MailFieldSubstitute(pageBody, "(ControlLifeCycleEvents)", GetControlLifeCycleEvents())

        WritePostbackControlRegistration(pageBody)

        GetCurrentOperations(pageBody)

        If UseJavascriptActions Then
            pageBody = MailFieldSubstitute(pageBody, "(PostbackActions)", projectActionData.postback.handlers.ToString())
            pageBody = MailFieldSubstitute(pageBody, "(TriggerPostbackActions)", projectActionData.postback.triggers)
        Else
            GetPostbackActions(postbackActionsBuilder, triggerPostbackActions)

            pageBody = MailFieldSubstitute(pageBody, "(PostbackActions)", postbackActionsBuilder.ToString())
            pageBody = MailFieldSubstitute(pageBody, "(TriggerPostbackActions)", triggerPostbackActions)
        End If

        If archive Then
            pageBody = MailFieldSubstitute(pageBody, "(CheckArchiveCall)", "CheckArchive()")
            pageBody = MailFieldSubstitute(pageBody, "(CheckArchiveMethod)", GetCheckArchiveMethod())
        End If
    End Sub

    Function GetCheckArchiveMethod() As String
        Dim checkArchiveMethod As New StringBuilder()

        checkArchiveMethod.Append("Sub CheckArchive()" & vbCrLf)
        checkArchiveMethod.Append("If GetQueryString(""Archive"") = ""True"" Then" & vbCrLf)
        checkArchiveMethod.Append("pageName = ""Archive""" & vbCrLf)
        checkArchiveMethod.Append("bArchive = True" & vbCrLf)
        checkArchiveMethod.Append("SetPageControlsEnabled()" & vbCrLf)
        checkArchiveMethod.Append("btnUpdate.Visible = False" & vbCrLf)
        checkArchiveMethod.Append("End If" & vbCrLf)
        checkArchiveMethod.Append("End Sub" & vbCrLf)

        Return checkArchiveMethod.ToString()
    End Function

    Private Function GetSetUsernameCall() As String
        Return IIf(CurrentProjectRequiresLogin(), "SetSessionVariable(""Username"", Get" & GetAncillaryName() & "SubmissionColumn(GetQuerystring(""ID""), ""Username""))", "") & vbCrLf
    End Function

    Private Function GetDeleteTempFiles() As String
        Return IIf(currentProject.ProjectControls.Where(Function(pc) IsFileUploadControl(If(pc.ControlType, 0))).Count > 0, "ClearTempFiles(""LocalPath\UploadedFiles"", Session.SessionID)", "")
    End Function
    Protected overrides sub WriteSpecificFormLoad()
                AddMaintenanceHomeFormLoadLink(loadFormText)
        End sub

        protected overrides Sub WriteSpecificDesigner()
            pageBody = MailFieldSubstitute(pageBody, "(DesignerControls)", GetDesignerControls(controlsDT))
        End Sub
    End Class
