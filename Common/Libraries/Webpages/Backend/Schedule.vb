Imports System.Data
Imports System.Data.SqlClient
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
Imports Common.General.DataSources
Imports Common.General.Columns
Imports Common.General.Assembly
Imports Common.General.Links
Imports Common.SQL.StoredProcedure
Imports Common.Webpages.Frontend.MultiPage
Imports Common.SQL.Main
Imports Common.SQL.Table
Imports Common.Webpages.Frontend.Email
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Export
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.BindData
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File
Imports Common.Webpages.Backend.Navigation


Imports WhitTools.Utilities
Namespace Webpages.Backend
    Public Class Schedule
        Inherits Backend.Main

        Shared Sub WriteSchedulePage()
            call new schedulepagewriter().writepage()

            'Dim sDisplayColumns, sDataColumns, sJoins, TemplatePath, pageBody, sDepartmentLink, sDepartmentName, sSaveAncillaryCalls, sSaveAncillaryMethods, sPostbackActions, sTriggerPostbackActions, sGetBindData, sGetBindDataAdditional, sGetBindRepeaterData, sCheckControlMethods, sGetDeleteAncillaryData, sExportlink, sExportmethod, sDepartmentURL, sLoadFormText,sNavigation As String
            'Dim sProjectName As String = RemoveNonAlphanumeric(ProjectDT.Rows(0).Item("PageTitle"))
            'Dim strBaseDir As String = S_PROJECTFILESPATH & sProjectName & "\Backend\"

            'With ProjectDT.Rows(0)
            '    GetDepartmentInfo(sDepartmentLink, sDepartmentName, .Item("Department"), .Item("CustomDepartmentName"), sDepartmentURL)
            '    GetBackendNavigation(sNavigation, "schedule")

            '    'Schedule HTML Page
            '    TemplatePath = GetTemplatePath() & sProjectLocation & "\Header.eml"
            '    pageBody = GetMailFile(TemplatePath)

            '    TemplatePath = GetTemplatePath() & "\Backend\ScheduleHTML.eml"
            '    pageBody &= GetMailFile(TemplatePath)

            '    TemplatePath = GetTemplatePath() & sProjectLocation & "\Footer.eml"
            '    pageBody &= GetMailFile(TemplatePath)

            '    pageBody = MailFieldSubstitute(pageBody, "(Responsive)", "_Responsive")
            '    pageBody = MailFieldSubstitute(pageBody, "(PageTitle)", .Item("PageTitle") & " - Schedule")
            '    pageBody = MailFieldSubstitute(pageBody, "(PageType)", "schedule")
            '    pageBody = MailFieldSubstitute(pageBody, "(Navigation)", S_NAVIGATION_FILE_REFERENCE)
            '    pageBody = MailFieldSubstitute(pageBody, "(ProjectName)", .Item("PageTitle"))
            '    pageBody = MailFieldSubstitute(pageBody, "(Department)", sDepartmentName)
            '    pageBody = MailFieldSubstitute(pageBody, "(ClosedMessage)", FormatClosedMessage(ProjectDT.Rows(0).Item("ClosedMessage")))

            '    RemoveUnusedTemplateFields(pageBody)
            '    WriteAllText(strBaseDir & "\" & "schedule.aspx", pageBody)

            '    'Schedule VB Page
            '    TemplatePath = GetTemplatePath() & "\Backend\ScheduleVB.eml"
            '    pageBody = GetMailFile(TemplatePath)
            '    pageBody = MailFieldSubstitute(pageBody, "(Imports)", BuildSetup.GetImports())
            '    pageBody = MailFieldSubstitute(pageBody, "(ProjectName)", sProjectName)

            '    GetSaveAncillaryContent("nCurrentID", sSaveAncillaryCalls, sSaveAncillaryMethods)
            '    GetBindData(sGetBindData, sGetBindDataAdditional, sGetBindRepeaterData, sCheckControlMethods)

            '    pageBody = MailFieldSubstitute(pageBody, "(LoadDDLsContent)", GetLoadDDLs())
            '    pageBody = MailFieldSubstitute(pageBody, "(DatabaseName)", sSQLDatabaseName)
            '    pageBody = MailFieldSubstitute(pageBody, "(SQLServerName)", GetSQLServerName(sSQLServerName))
            '    pageBody = MailFieldSubstitute(pageBody, "(SQLMainTableName)", .Item("SQLMainTableName"))
            '    pageBody = MailFieldSubstitute(pageBody, "(ScheduleTableName)", GetScheduleTableName())
            '    pageBody = MailFieldSubstitute(pageBody, "(BindData)", sGetBindData)
            '    pageBody = MailFieldSubstitute(pageBody, "(BindDataAdditional)", sGetBindDataAdditional)

            '    sLoadFormText = ""

            '    AddFormLoadLink(sLoadFormText, sDepartmentName, sDepartmentURL)
            '    AddFormLoadLink(sloadformtext, .Item("PageTitle") & " - Maintenance", "index.aspx")
            '    AddFormLoadHeadingCurrentPageLink(sLoadFormText, .Item("PageTitle") , " - Schedule")
              
            '    pageBody = MailFieldSubstitute(pageBody, "(LoadFormText)", sLoadFormText)
                
            '    RemoveUnusedTemplateFields(pageBody)
            '    WriteAllText(strBaseDir & "\" & "schedule.aspx.vb", pageBody)

            '    'Schedule Designer Page
            '    TemplatePath = GetTemplatePath() & "\Backend\ScheduleDesigner.eml"
            '    pageBody = GetMailFile(TemplatePath)

            '    RemoveUnusedTemplateFields(pageBody)
            '    WriteAllText(strBaseDir & "\" & "schedule.aspx.designer.vb", pageBody)
            'End With
        End Sub

        Public Shared Function FormatMessageForString(ByVal message As String) As String
            message = Replace(message, """", """""")
            message = Replace(message, vbCrLf, "")
            'message = Replace(message, "<p>", "")
            'message = Replace(message, "</p>", "")

            Return message
        End Function

        Shared Function GetCheckScheduleCall() As String
            Dim sCheckClosedCall As String = ""

            If Main.GetBackendOption(S_BACKEND_OPTION_SCHEDULE) And General.Variables.isFrontend And Not isInsert Then
                sCheckClosedCall = "If GetQueryString(""Maintenance"") <> S_TRUE Then" & vbCrLf
                sCheckClosedCall &= "CheckSchedule()" & vbCrLf
                sCheckClosedCall &= "End If" & vbCrLf
            End If

            Return sCheckClosedCall
        End Function

        Shared Function GetScheduleTableName() As String
            Return Replace(RemoveNonAlphanumeric(GetAncillaryProject("PageTitle")), "-", "") & "Schedule"
        End Function

    End Class
End Namespace
