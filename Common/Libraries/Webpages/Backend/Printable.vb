Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports Common.General.Ancillary
Imports Common.General.Actions
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.Controls
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.DataTypes
Imports Common.General.Folders
Imports Common.General.DataSources
Imports Common.General.ProjectOperations
Imports Common.General.Columns
Imports Common.General.Assembly
Imports Common.General.Links
Imports Common.Webpages.Workflow
Imports Common.SQL.StoredProcedure
Imports Common.Webpages.Frontend.MultiPage
Imports Common.SQL.Main
Imports Common.SQL.Table
Imports Common.Webpages.Frontend.Email
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Export
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.Backend.ecommerce
Imports Common.Webpages.BindData
Imports Common.Webpages.ControlContent.Heading
Imports Common.Webpages.ControlContent.Main
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File

Imports System.IO.File

Imports WhitTools.Utilities
Namespace Webpages.Backend
    Public Class Printable
        Inherits Backend.Main

        Shared Sub WritePrintablePage(ByVal sPageType As String, ByVal sPagePurpose As String)
            Dim sDisplayColumns, sDataColumns, sJoins, TemplatePath, pageBody, sDepartmentLink, sDepartmentName, sSaveAncillaryCalls, sSaveAncillaryMethods, sTriggerPostbackActions, sGetBindData, sGetBindDataAdditional, sGetBindRepeaterData, sCheckControlMethods, sGetDeleteAncillaryData, sExportlink, sExportmethod, sDepartmentURL, sLoadFormText As String
            Dim postbackActionsBuilder As New StringBuilder()

            Dim sProjectName As String = RemoveNonAlphanumeric(projectDT.Rows(0).Item("PageTitle"))
            Dim strBaseDir As String = S_PROJECTFILESPATH & sProjectName & "\Frontend\"

            With projectDT.Rows(0)
                GetDepartmentInfo(sDepartmentLink, sDepartmentName, .Item("Department"), .Item("CustomDepartmentName"), sdepartmentURL)

                'Update Printable HTML Page
                TemplatePath = GetTemplatePath() & projectLocation & "\Header.eml"
                pageBody = GetMailFile(TemplatePath)

                TemplatePath = GetTemplatePath() & "Backend\UpdatePrintableHTML.eml"
                pageBody &= GetMailFile(TemplatePath)

                TemplatePath = GetTemplatePath() & projectLocation & "\Footer.eml"
                pageBody &= GetMailFile(TemplatePath)

                pageBody = MailFieldSubstitute(pageBody, "(Responsive)", "_Responsive")
                pageBody = MailFieldSubstitute(pageBody, "(PageTitle)", .Item("PageTitle"))

                pageBody = MailFieldSubstitute(pageBody, "(PageType)", sPageType)
                pageBody = MailFieldSubstitute(pageBody, "(RunAOPageHeader)", GetRunAOPageHeader)
                pageBody = MailFieldSubstitute(pageBody, "(ProjectName)", sProjectName)
                pageBody = MailFieldSubstitute(pageBody, "(Department)", sDepartmentName)
                pageBody = MailFieldSubstitute(pageBody, "(BodyContent)", GetBodyContent())
                pageBody = MailFieldSubstitute(pageBody, "(TotalOwed)", GetTotalOwedControls())
                pageBody = MailFieldSubstitute(pageBody, "(WorkflowSteps)", GetWorkflowSteps())
                pageBody = MailFieldSubstitute(pageBody, "(CustomScript)", GetCustomScript())
                pageBody = MailFieldSubstitute(pageBody, "(EcommerceControls)", GetEcommerceControls())

                RemoveUnusedTemplateFields(pageBody)

                WriteAllText(strBaseDir & sPageType & ".aspx", pageBody)

                'Update Printable VB Page
                TemplatePath = GetTemplatePath() & "\Backend\UpdatePrintableVB.eml"
                pageBody = GetMailFile(TemplatePath)
                pageBody = MailFieldSubstitute(pageBody, "(Imports)", currentImportListing)
                pageBody = MailFieldSubstitute(pageBody, "(ProjectName)", sProjectName)

                GetSaveAncillaryContent("nCurrentID", sSaveAncillaryCalls, sSaveAncillaryMethods)
                GetBindData(sGetBindData, sGetBindDataAdditional, sGetBindRepeaterData, sCheckControlMethods, "GetCurrentApplicationID()")

                pageBody = MailFieldSubstitute(pageBody, "(PageType)", sPageType)
                pageBody = MailFieldSubstitute(pageBody, "(LoadDDLsContent)", GetLoadDDLs())
                pageBody = MailFieldSubstitute(pageBody, "(DatabaseName)", SQLDatabaseName)
                pageBody = MailFieldSubstitute(pageBody, "(SQLServerName)", GetSQLServerName(SQLServerName))
                pageBody = MailFieldSubstitute(pageBody, "(MainTableName)", archiveRef & .Item("SQLMainTableName"))
                pageBody = MailFieldSubstitute(pageBody, "(TotalOwed)", If(IsEcommerceProject(), "lblTotalOwed.Text = GetInvoiceAmount(.Item(""Invoice""))", ""))
                pageBody = MailFieldSubstitute(pageBody, "(BindData)", sGetBindData)
                pageBody = MailFieldSubstitute(pageBody, "(BindDataAdditional)", sGetBindDataAdditional)
                pageBody = MailFieldSubstitute(pageBody, "(BindRepeaterData)", sGetBindRepeaterData)
                pageBody = MailFieldSubstitute(pageBody, "(BindWorkflowSteps)", GetBindWorkflowSteps())
                pageBody = MailFieldSubstitute(pageBody, "(CheckControlMethods)", sCheckControlMethods)
                pageBody = MailFieldSubstitute(pageBody, "(IDSelect)", If(General.Variables.isFrontend, "GetCurrentApplicationID()", "GetQuerystring(""ID"")"))

                GetCurrentOperations(pageBody)

                postbackActionsBuilder = New StringBuilder()
                sTriggerPostbackActions = ""
                GetPostbackActions(postbackActionsBuilder, sTriggerPostbackActions)

                If usejavascriptactions Then
                    pageBody = MailFieldSubstitute(pageBody, "(PostbackActions)", projectActionData.postback.handlers.ToString())
                    pageBody = MailFieldSubstitute(pageBody, "(TriggerPostbackActions)", projectActionData.postback.triggers)
                Else
                    pageBody = MailFieldSubstitute(pageBody, "(PostbackActions)", postbackActionsBuilder.ToString())
                    pageBody = MailFieldSubstitute(pageBody, "(TriggerPostbackActions)", sTriggerPostbackActions)
                End If


                sLoadFormText = ""

                AddFormLoadLink(sLoadFormText, sDepartmentName, sDepartmentURL)

                If sPageType.contains("ViewArchive") Then
                    AddFormLoadLink(sloadformtext, .Item("PageTitle") & " - Archive Maintenance", "archive.aspx")
                End If

                AddFormLoadHeadingCurrentPageLink(sLoadFormText, .Item("PageTitle"), " - " & sPagePurpose)

                pageBody = MailFieldSubstitute(pageBody, "(LoadFormText)", sLoadFormText)

                RemoveUnusedTemplateFields(pageBody)

                WriteAllText(strBaseDir & sPageType & ".aspx.vb", pageBody)

                'Update Printable Designer Page
                TemplatePath = GetTemplatePath() & "\Backend\UpdatePrintableDesigner.eml"
                pageBody = GetMailFile(TemplatePath)
                pageBody = MailFieldSubstitute(pageBody, "(DesignerControls)", GetDesignerControls(controlsDT))

                RemoveUnusedTemplateFields(pageBody)
                WriteAllText(strBaseDir & sPageType & ".aspx.designer.vb", pageBody)
            End With
        End Sub

    End Class
End Namespace

