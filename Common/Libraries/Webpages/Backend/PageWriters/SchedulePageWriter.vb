Imports Common.General.Ancillary
Imports Common.General.Variables
Imports Common.General.ProjectOperations
Imports Common.Webpages.Backend
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.Backend.Printable
Imports Common.Webpages.Backend.Export
Imports Common.Webpages.Backend.Archive
Imports Common.Webpages.Workflow
Imports Common.Webpages.Backend.Columns
Imports Common.Webpages.Backend.actions
Imports Common.Webpages.Backend.SortFilter
Imports Common.Webpages.Backend.Main
Imports WhitTools.Email

Public Class SchedulePageWriter
    inherits BackendListingPageWriter
        private ScheduleControlLoginContent, ScheduleTerms As string
    
    Public Sub New()
        MyBase.New()

        pageHTMLName = $"ScheduleHTML"
        pageVBName = $"ScheduleVB"
        pageDesignerName = $"ScheduleDesigner"
        me.pageType = "Schedule" & GetAncillaryName()
        pageTitleAddition = " - " & "Schedule"
        pageFileName = pageType
        
        SetPageVariables()
    End Sub

    protected overrides Sub WriteListingSpecificHTML()
        dim loginContent, terms As string

        Scheduleterms = terms

        pageBody = MailFieldSubstitute(pageBody, "(ClosedMessage)", FormatMessageForString(projectDT.Rows(0).Item("ClosedMessage")))

    End Sub
    
    protected overrides Sub WriteListingSpecificCodebehind()
        pageBody = MailFieldSubstitute(pageBody, "(LoadDDLsContent)", GetLoadDDLs())
        pageBody = MailFieldSubstitute(pageBody, "(ScheduleTableName)", GetScheduleTableName())
        pageBody = MailFieldSubstitute(pageBody, "(AncillaryReference)", GetAncillaryName())
        
    End Sub

    protected overrides sub WriteSpecificFormLoad()
                AddMaintenanceHomeFormLoadLink(loadFormText)
    End sub

End Class
