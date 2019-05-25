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

Public Class BackendIndexPageWriter
    inherits BackendListingPageWriter

    'private displayColumns, dataColumns, joins, additionalSelectColumns, navigation, actionMethods, actionStatements, sortControl, sortHandler, sortStatement, filterControl, filterHandler, filterStatement, selectCookies, orderByColumn, selectRepeaterStatement, getDeleteAncillaryData As String
    protected selectStatement as string

    Public Sub New(ByVal pageType As String, ByVal pagePurpose As String)
        MyBase.New()

        pageHTMLName = $"{pageType}HTML"
        pageVBName = $"{pageType}VB"
        pageDesignerName = $"{pageType}Designer"
        Me.pageType = pageType & GetAncillaryName()
        Me.pagePurpose = pagePurpose
        pageTitleAddition = " - " & pagePurpose
        pageFileName = Me.pageType

        SetPageVariables()
    End Sub

    protected overrides Sub WriteListingSpecificHTML()
            if pageType.contains("Archive")
                AddArchiveActions()
            End If
    End Sub

    Private Sub AddArchiveActions()
        'If actionItems <> "" Then
        '    Dim sArchiveActions As String = "<asp:Label id=""lblActions"" runat=""server"" AssociatedControlID=""ddlActions"">Action:</asp:Label>" & vbcrlf

        '    sarchiveActions &= "<asp:dropdownlist id=""ddlActions""  autopostback=""True"" runat=""server"">" & vbcrlf
        '    sarchiveactions &= "<asp:listitem navigation=""Please Select"">Please Select</asp:listitem>" & vbcrlf
        '    sarchiveActions &= actionItems
        '    sarchiveactions &= "</asp:dropdownlist>" & vbcrlf

        '    actionItems = sArchiveActions

        'End If
    End Sub

    protected overrides Sub WriteListingSpecificFormLoadText()
        AddFormLoadLink(loadformtext, GetAncillaryProject("PageTitle"))
    End Sub

    protected overrides Sub WriteListingSpecificCodebehind()
        'if pagetype.contains("Archive") And actionitems <> ""
        '            pageBody = MailFieldSubstitute(pageBody, "(ActionHandles)", "Handles ddlActions.SelectedIndexChanged")

        'End If
    End Sub
    End Class
