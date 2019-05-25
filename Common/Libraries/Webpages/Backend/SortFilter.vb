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
Imports Common.General.ProjectOperations
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
Imports Common.Webpages.ControlContent.Heading
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File


Imports WhitTools.Utilities
Namespace Webpages.Backend
    Public Class SortFilter
        Inherits Backend.Main

        Shared Sub WriteSortHandler(ByRef sSortHandler As String)
            sSortHandler &= "Sub ddlSort_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlSort.SelectedIndexChanged, ddlSortDirection.SelectedIndexChanged" & vbCrLf

            If Not isSearch Then
                sSortHandler &= " UpdateCookie(""Sort"", ddlSort.SelectedIndex)" & vbCrLf & vbCrLf
                sSortHandler &= " UpdateCookie(""SortDirection"", ddlSortDirection.SelectedIndex)" & vbCrLf & vbCrLf
                else
                sSortHandler &= "SetSessionVariable(""LastSearchQuery%(AncillaryReference)%"", """")" & vbcrlf
            End If

            sSortHandler &= If(isSearch, "RunSearch()", "BindData()") & vbCrLf
            sSortHandler &= "End Sub" & vbCrLf
        End Sub

        Shared Sub WriteSortControl(ByRef sSortList As String)
            Dim sLabel, sValue As String

            sSortList &= "<li>" & vbcrlf
            ssortlist &= "<asp:Label id=""lblSort"" runat=""server"" AssociatedControlID=""ddlSort"">Sort By:</asp:Label>" & vbcrlf
            
            sSortList &= "<asp:dropdownlist id=""ddlSort"" autopostback=""True"" runat=""server"">" & vbCrLf
            'sSortList &= "<asp:listitem selected=""true"" value=""DateSubmitted"">Date Submitted</asp:listitem>" & vbCrLf

            Dim dtSortColumns As DataTable = GetDataTable("Select OC.Label, OC.ControlID, C.Name, C.Heading, C.ControlType From " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  OC left outer join " & DT_WEBRAD_PROJECTCONTROLS & " C on OC.ControlID = C.ID Where OptionID in (Select ID From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  Where ProjectID = " & GetProjectID() & " AND Type = 1)", Common.General.Variables.cnx)
            Dim loginColumn As LoginColumnType

            For Each CurrentRow As DataRow In dtSortColumns.Rows
                loginColumn = LoginColumnTypes.Find(Function(l) l.ID = CurrentRow.Item("ControlID"))

                If loginColumn Is Nothing Then
                    sLabel = If(CurrentRow.Item("Label") <> "", CurrentRow.Item("Label"), CurrentRow.Item("Heading"))

                    'If IsControlType(CurrentRow.Item("ControlType"), "Date") Then
                    '    sValue = "cast (MT.[" & CurrentRow.Item("Name") & "] as DateTime)"
                    'Else
                    '    sValue = "MT.[" & CurrentRow.Item("Name") & "]"
                    'End If

                    If IsControlType(CurrentRow.Item("ControlType"), "Date") Then
                        sValue = "cast ([" & CurrentRow.Item("Name") & "] as DateTime)"
                    Else
                        sValue = "[" & CurrentRow.Item("Name") & "]"
                    End If
                Else
                    sLabel = loginColumn.DisplayName
                    sValue = loginColumn.ColumnName
                End If

                sSortList &= "<asp:listitem value=""" & sValue & """>" & sLabel & "</asp:listitem>" & vbCrLf
            Next

            sSortList &= "</asp:dropdownlist>" & vbCrLf
            sSortList &= "<asp:dropdownlist id=""ddlSortDirection"" autopostback=""True"" runat=""server"">" & vbCrLf

            If GetCurrentProjectDT.Rows(0).Item("DefaultSort") = "Descending" Then
                sSortList &= "<asp:listitem selected=""true"" value=""desc"">Descending</asp:listitem>" & vbCrLf
                sSortList &= "<asp:listitem value=""asc"">Ascending</asp:listitem>" & vbCrLf
            Else
                sSortList &= "<asp:listitem selected=""true"" value=""asc"">Ascending</asp:listitem>" & vbCrLf
                sSortList &= "<asp:listitem  value=""desc"">Descending</asp:listitem>" & vbCrLf
            End If

            sSortList &= "</asp:dropdownlist>" & vbCrLf
            sSortList &= "</li>"
        End Sub

        Shared Sub WriteFilterControl(ByRef sFilterList As String, ByVal sSelectStatementPart1 As String, ByVal sSelectStatementPart2 As String)
            Dim sLabel, sValue, sOperator, sSortStatement, sJoins, sAdditionalSelectColumns As String

            sfilterlist &= "<li>" & vbcrlf
            sfilterlist &= "<asp:Label id=""lblFilter"" runat=""server"" AssociatedControlID=""ddlFilter"">Show:</asp:Label>" & vbcrlf
            
            sFilterList &= "<asp:dropdownlist id=""ddlFilter"" autopostback=""True"" runat=""server"">" & vbCrLf
            'sFilterList &= "<asp:listitem selected=""true"" value=""" & sSelectStatementPart1 & sSelectStatementPart2 & """>All</asp:listitem>" & vbCrLf
            sFilterList &= "<asp:listitem selected=""true"" value=""1 = 1"">All</asp:listitem>" & vbCrLf


            Dim dtFilterOptions As DataTable = GetDataTable("Select * From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  O Where ProjectID = " & GetProjectID() & " AND Type = 2", Common.General.Variables.cnx)

            For Each CurrentRow As DataRow In dtFilterOptions.Rows
                sLabel = CurrentRow.Item("Label")

                Dim dtFilterColumns As DataTable = GetDataTable("Select OC.ControlID, OC.OperatorType, OT.Value as OperatorValue, OC.ComparisonValue, C.Name, C.Heading, C.SQLInsertItemTable, C.ForeignID From " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  OC left outer join " & DT_WEBRAD_PROJECTCONTROLS & " C on OC.ControlID = C.ID LEFT OUTER JOIN " & DT_WEBRAD_BACKENDOPTIONOPERATORTYPES & " OT ON OperatorType = OT.ID Where OptionID = " & CurrentRow.Item("ID"), Common.General.Variables.cnx)
                Dim loginColumn As LoginColumnType

                sValue = ""

                For Each CurrentRow2 As DataRow In dtFilterColumns.Rows
                    With CurrentRow2
                        If sValue <> "" Then
                            sValue &= " OR "
                        End If

                        sOperator = " " & .Item("OperatorValue")

                        loginColumn = LoginColumnTypes.Find(Function(l) l.ID = CurrentRow2.Item("ControlID"))

                        If loginColumn Is Nothing Then
                            sValue &= .Item("Name")
                        Else
                            sValue &= loginColumn.ColumnName
                        End If

                        If .Item("OperatorValue") = "IN" Or .Item("OperatorValue") = "NOT IN" Then
                            sValue &= sOperator & " (" & .Item("ComparisonValue") & ")"
                        ElseIf .Item("OperatorValue") = "LIKE" Then
                            sValue &= sOperator & " '%" & .Item("ComparisonValue") & "%'"
                        Else
                            sValue &= sOperator & " '" & .Item("ComparisonValue") & "'"
                        End If

                        If .Item("SQLInsertItemTable") <> "" Then
                            sValue = "ID IN (SELECT " & .Item("ForeignID") & " FROM " & .Item("SQLInsertItemTable") & " WHERE " & sValue & ")"
                        End If
                    End With
                Next

                'sFilterList &= "<asp:listitem value=""" & sSelectStatementPart1 & sValue & sSelectStatementPart2 & """>" & sLabel & "</asp:listitem>" & vbCrLf
                sFilterList &= "<asp:listitem value=""(" & sValue & ")"">" & sLabel & "</asp:listitem>" & vbCrLf
            Next

            sFilterList &= "</asp:dropdownlist>" & vbCrLf
            sFilterList &= "</li>" & vbcrlf
            
        End Sub

        Shared Sub WriteFilterHandler(ByRef sFilterHandler As String)
            sFilterHandler &= "Sub ddlFilter_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlFilter.SelectedIndexChanged" & vbCrLf

            If Not isSearch Then
                sFilterHandler &= "UpdateCookie(""Filter"", ddlFilter.SelectedIndex)" & vbCrLf & vbCrLf
            End If

            sFilterHandler &= If(isSearch, "RunSearch()", "BindData()") & vbCrLf
            sFilterHandler &= "End Sub" & vbCrLf
        End Sub


        Shared Sub GetSortFilterLists(ByRef sSortList As String, ByRef sSortHandler As String, ByRef sSortStatement As String, ByRef sFilterList As String, ByRef sFilterHandler As String, ByRef sFilterStatement As String, ByVal sSelectStatement As String, ByRef sSelectCookies As String, optional byref sOrderByColumn As string = "")
            With GetCurrentProjectDT.Rows(0)
                If Main.getbackendoption(S_BACKEND_OPTION_SORT) Then
                        WriteSortControl(sSortList)
                        WriteSortHandler(sSortHandler)

                        sSelectCookies &= GetSelectCookie("Sort")
                        sSelectCookies &= GetSelectCookie("SortDirection")
                End If

                Dim customSelectDataSource As DataTable = GetCustomSelectDataSource()

                If customSelectDataSource.Rows.Count > 0 AndAlso customSelectDataSource.Rows(0).Item("OrderBy") <> "" Then
                    sSortStatement = " ORDER BY " & customSelectDataSource.Rows(0).Item("OrderBy")
                    sOrderByColumn = customSelectDataSource.Rows(0).Item("OrderBy").ToString().Split(",")(0)
                Else
                    If Main.GetBackendOption(S_BACKEND_OPTION_SORT) Then
                        sSortStatement = " ORDER BY "" & ddlSort.SelectedValue & "" "" & ddlSortDirection.SelectedValue & """
                        sOrderByColumn = " "" & ddlSort.SelectedValue & "" "" & ddlSortDirection.SelectedValue & """
                    Else
                        sSortStatement &= " ORDER BY " & GetDateSubmittedColumnReference() & " desc"
                        sOrderByColumn = GetDateSubmittedColumnReference() & " desc"
                    End If
                End If

                If Main.GetBackendOption(S_BACKEND_OPTION_FILTER) Then
                    sFilterStatement = "conditionals.Add(ddlFilter.SelectedValue)"

                    Dim sSelectStatementPart1, sSelectStatementPart2 As String

                    sSelectStatementPart1 = sSelectStatement
                    sSelectStatementPart2 = sSortStatement
                    sSelectStatementPart2 &= GetWhereStatement(isSearch, "FilterStatementPresent")

                    WriteFilterControl(sFilterList, sSelectStatementPart1, sSelectStatementPart2)
                    WriteFilterHandler(sFilterHandler)

                    sSelectCookies &= GetSelectCookie("Filter")
                End If
            End With
        End Sub
    End Class
End Namespace



