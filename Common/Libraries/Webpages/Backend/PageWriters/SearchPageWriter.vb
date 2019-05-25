Imports Common.General.Ancillary
Imports Common.General.Variables
Imports Common.General.ProjectOperations
Imports Common.Webpages.Backend
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Printable
Imports Common.Webpages.Backend.Export
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.Backend.Archive
Imports Common.Webpages.Workflow
Imports Common.Webpages.Backend.Columns
Imports Common.Webpages.Backend.actions
Imports Common.Webpages.Backend.SortFilter
Imports Common.Webpages.Backend.Main
Imports WhitTools.Email
Imports WhitTools.Utilities
Imports WhitTools.File

Public Class SearchPageWriter
    inherits BackendListingPageWriter
        private searchControlLoginContent, searchTerms As string
    
    Public Sub New()
        MyBase.New()

        pageHTMLName = $"SearchHTML"
        pageVBName = $"SearchVB"
        pageDesignerName = $"SearchDesigner"
        me.pageType = "search" & GetAncillaryName()
        pageTitleAddition = " - " & "Search"
        pagePurpose = "Search"
        pageFileName = pageType

        SetPageVariables()
        'GetLoginSearchControlData(searchControlLoginContent, searchTerms, GetSearchControlColumns())
    End Sub

    protected overrides Sub WriteListingSpecificHTML()
        dim loginContent, terms As String

        GetLoginSearchControlData(loginContent, 2)

        pageBody = MailFieldSubstitute(pageBody, "(BodyContent)", GetSearchControls(loginContent, GetSearchControlColumns()))
        pageBody = MailFieldSubstitute(pageBody, "(AncillaryReference)",GetAncillaryName())
    End Sub
    
    protected overrides Sub WriteListingSpecificCodebehind()
        pageBody = MailFieldSubstitute(pageBody, "(SearchQuery)", GetSearchQuery(selectStatement))
        pageBody = MailFieldSubstitute(pageBody, "(SearchTerms)", GetSearchTerms())
        pageBody = MailFieldSubstitute(pageBody, "(LoadDDLsContent)", GetLoadDDLs())
        pageBody = MailFieldSubstitute(pageBody, "(ProjectShortName)", currentProject.GetProjectNameAlphaNumericOnly())
    End Sub

    protected overrides sub WriteSpecificFormLoad()
                AddMaintenanceHomeFormLoadLink(loadFormText)
        End sub

End Class
