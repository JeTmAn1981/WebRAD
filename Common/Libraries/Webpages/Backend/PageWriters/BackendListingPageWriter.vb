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
Imports Common.Webpages.Backend.Navigation
Imports Common.Webpages.Backend.actions
Imports Common.Webpages.Backend.SortFilter
Imports Common.Webpages.Backend.Main
Imports Common.General.DataSources

Imports WhitTools.Email
Imports WhitTools.Utilities
Imports WhitTools.DataTables
Imports System.IO.File

Public mustinherit Class BackendListingPageWriter
    inherits PageWriter

    Private displayColumns, dataColumns, joins, additionalSelectColumns, sortControl, sortHandler, sortStatement, filterControl, filterHandler, filterStatement, selectCookies, orderByColumn, selectRepeaterStatement, getDeleteAncillaryData As String
    Protected selectStatement, whereStatement, mainTableReference As String
    'Dim actionItems, actionMethods, actionStatements As String

    Public Sub New()
        MyBase.New()
    End Sub

    protected overrides Sub WriteSpecificHTML()
        With projectDT.Rows(0)
            WriteListingSpecificHTML()

            pageBody = MailFieldSubstitute(pageBody, "(ActionItems)", "<!-- #include file=""" & currentProject.GetProjectNameAlphaNumericOnly() & IIf(pagePurpose = "Archive", "Archive", "") & "ActionItems.htm"" -->")
            pageBody = MailFieldSubstitute(pageBody, "(Navigation)", S_NAVIGATION_FILE_REFERENCE)
            pageBody = MailFieldSubstitute(pageBody, "(InsertLink)", currentProject.GetInsertLink())
            pageBody = MailFieldSubstitute(pageBody, "(SortControl)", sortControl)
            pageBody = MailFieldSubstitute(pageBody, "(FilterControl)", filterControl)
            pageBody = MailFieldSubstitute(pageBody, "(DisplayColumns)", displayColumns)
            pageBody = MailFieldSubstitute(pageBody, "(DataColumns)", dataColumns)
            pageBody = MailFieldSubstitute(pageBody, "(AncillaryReference)", GetAncillaryName)
        end with
    End Sub

    protected overridable sub WriteListingSpecificHTML()

    End Sub

    Protected Sub SetPageVariables()
        'mainTableReference = sArchiveRef & GetAncillaryProject("SQLMainTableName")
        mainTableReference = GetAncillaryProject("SQLMainTableName")

        GetBackendColumns(displayColumns, dataColumns, joins, additionalSelectColumns)

        SetSelectStatement()
        SetWhereStatement()

        GetSortFilterLists(sortControl, sortHandler, sortStatement, filterControl, filterHandler, filterStatement, selectStatement, selectCookies, orderByColumn)
        GetDeleteArchiveAncillaryData(getDeleteAncillaryData, S_SUBMISSIONS_ID_LABEL, 0)

    End Sub

    Private Sub SetWhereStatement()
        If ProjectUsesCustomSelectStatement() AndAlso GetCustomSelectDataSource().Rows(0).Item("Where") <> "" Then
            whereStatement = "conditionals.Add(""" & GetCustomSelectDataSource().Rows(0).Item("Where") & """)"
        Else
            whereStatement = GetWhereStatement(isSearch, filterStatement)
        End If
    End Sub

    Private Sub SetSelectStatement()
        Dim customSelectDataSource As DataTable = GetCustomSelectDataSource()

        If customSelectDataSource.Rows.Count > 0 Then
            mainTableReference = customSelectDataSource.Rows(0).Item("Table").replace(GetAncillaryProject("SQLMainTableName"), mainTableReference)

            selectStatement = "SELECT " & customSelectDataSource.Rows(0).Item("Select") & " FROM " & mainTableReference
        Else
            selectStatement = "Select MT.*" & additionalSelectColumns & " FROM " & mainTableReference & " MT" & joins
        End If
    End Sub

    Protected overrides Sub WriteSpecificCodebehind()
        'Bug here, paging and filtering do not play nicely together.  The filtering process is set up to only put the actual filter
        'part of the statement in the values of the ddlFilter listitems, but the WhitTools ShowFilteredPageResults
        'method is expecting the entire select statement to be there.  Might need to rewrite one of these methods to make it work.

        If pagetype <> "Archive"Then
            If Main.getbackendoption(S_BACKEND_OPTION_PAGING) Then
                If Main.getbackendoption(S_BACKEND_OPTION_FILTER) Then
                    selectRepeaterStatement = "WhitTools.Maintenance.ShowFilteredPageResults(sSelectQuery, rptSubmissions, ddlFilter, lblPageNumbers, """",GetQueryString(S_PAGE), ""index.aspx"", " & GetCurrentProjectDT.Rows(0).Item("PageLimit") & ", True, Common.cnx)" & vbcrlf
                Else
                    selectRepeaterStatement = "WhitTools.Maintenance.ShowFilteredPageResults(sSelectQuery, rptSubmissions, Nothing, lblPageNumbers, """", GetQueryString(S_PAGE), ""index.aspx"", " & GetCurrentProjectDT.Rows(0).Item("PageLimit") & ", True, Common.cnx)" & vbcrlf
                End If

                selectRepeaterStatement &= "lblRecordsTotal.Text = lblPageNumbers.Text"
            Else
                selectRepeaterStatement = "SelectRepeaterData(rptSubmissions,getdatatable(Common.cnx, sSelectQuery),cnx)" & vbcrlf
                selectRepeaterStatement &= "lblRecordsTotal.Text = rptSubmissions.Items.Count & "" record(s) total."""
            End If
        End If

        pageBody = MailFieldSubstitute(pageBody, "(ProjectShortName)", currentProject.GetProjectNameAlphaNumericOnly())
        pageBody = MailFieldSubstitute(pageBody, "(ActionHandlerCall)", "Handle" & currentProject.GetProjectNameAlphaNumericOnly() & "Action(rptSubmissions, " & IIf(pagePurpose = "Archive", "Nothing", "pnlDelete") & ", ddlActions.SelectedValue," & IIf(pagePurpose = "Search", "AddressOf RunSearch", "AddressOf RedirectToIndex") & ")")
        pageBody = MailFieldSubstitute(pageBody, "(SortHandler)", sortHandler)
        pageBody = MailFieldSubstitute(pageBody, "(FilterHandler)", filterHandler)
        pageBody = MailFieldSubstitute(pageBody, "(SelectStatement)", selectStatement)
        pageBody = MailFieldSubstitute(pageBody, "(FilterStatement)", filterStatement)
        pageBody = MailFieldSubstitute(pageBody, "(SortStatement)", sortStatement)
        pageBody = MailFieldSubstitute(pageBody, "(OrderByColumn)", orderByColumn)
        pageBody = MailFieldSubstitute(pageBody, "(PageLimit)", GetProjectPageLimit())
        pageBody = MailFieldSubstitute(pageBody, "(SetShowControl)", if(filterstatement <> "", "frs.SetShowControl(ddlFilter)", ""))
        pageBody = MailFieldSubstitute(pageBody, "(WhereStatement)", whereStatement)
        pageBody = MailFieldSubstitute(pageBody, "(MainTableName)", mainTableReference)
        pageBody = MailFieldSubstitute(pageBody, "(DeleteAncillaryData)", getDeleteAncillaryData)
        pageBody = MailFieldSubstitute(pageBody, "(AncillaryReference)", GetAncillaryName)
        pageBody = MailFieldSubstitute(pageBody, "(SelectCookies)", selectCookies)
        pageBody = MailFieldSubstitute(pageBody, "(SelectRepeater)", If(pageType.contains("index"), selectRepeaterStatement, ""))
        pageBody = MailFieldSubstitute(pageBody, "(PreviousPage)", pageType & GetAncillaryName() & ".aspx")

        WriteListingSpecificCodebehind()
    End Sub

    protected overrides Sub WriteSpecificFormLoad()
        WriteListingSpecificFormLoadText()
    End Sub

    protected overridable Sub WriteListingSpecificCodebehind()

    End Sub

    Protected Overridable Sub WriteListingSpecificFormLoadText()

    End Sub


End Class
