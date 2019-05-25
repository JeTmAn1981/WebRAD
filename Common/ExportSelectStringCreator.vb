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
Imports Common.General.ProjectOperations
Imports Common.General.DataTypes
Imports Common.General.Folders
Imports Common.General.DataSources
Imports Common.General.Columns
Imports Common.General.Assembly
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
Imports WhitTools.Repeaters
Imports WhitTools.Filler


Imports WhitTools.Utilities
Imports System.Web.UI.WebControls
Imports System

Public Class ExportSelectStringCreator
    Dim export As ProjectBackendExport
    Dim type As String

    Public Sub New(ByRef export As ProjectBackendExport, ByVal type As String)
        Me.export = export
        Me.type = type
    End Sub

    Function CreateSelectString(Optional ByVal sIDSelect As String = " MT.ID IN ("" & GetListOfSelectedValues(rptSubmissions) & "")") As String
        If export.DataSourceType = "1" Then
            Dim currentTableReference = "", tableReferences = "", columnReferences As String = ""
            Dim selectTables As String = "Select distinct(TableControlID), SQLInsertItemTable,ForeignID From " & DT_WEBRAD_PROJECTCOLUMNS & " PCL left outer join " & DT_WEBRAD_PROJECTCONTROLS & " PC on PCL.TableControlID = PC.ID Where PCL.Type = '" & type & "' AND TypeID = " & export.ID & " AND PCL.ProjectID = " & export.projectid & " Order by TableControlID asc"
            Dim dtTables As DataTable = GetDataTable(selectTables, Common.General.Variables.cnx)

            Dim dtTableReferences As New DataTable
            Dim dtColumns As DataTable
            dtTableReferences.Columns.Add("TableControlID")
            dtTableReferences.Columns.Add("Reference")

            For nCounter As Integer = 0 To dtTables.Rows.Count - 1
                dtColumns = GetDataTable("Select ColumnControlID, Name From " & DT_WEBRAD_PROJECTCOLUMNS & " PCL left outer join " & DT_WEBRAD_PROJECTCONTROLS & " PC on PCL.ColumnControlID = PC.ID Where TableControlID = " & dtTables.Rows(nCounter).Item("TableControlID") & " AND PCL.Type = '" & type & "' AND TypeID = " & export.ID & " AND PCL.ProjectID = " & export.ProjectID & " ORDER BY ColumnControlID asc", Common.General.Variables.cnx)

                currentTableReference = If(dtTables.Rows(nCounter).Item("TableControlID") = 0, "MT", "AT" & nCounter)
                RecordTableReference(dtTableReferences, dtTables.Rows(nCounter).Item("TableControlID"), currentTableReference)

                If dtTables.Rows(nCounter).Item("TableControlID") = 0 Then
                    tableReferences &= export.Project.GetFullDatabaseTableName() & " " & currentTableReference
                Else
                    tableReferences &= " LEFT OUTER JOIN " & archiveRef & dtTables.Rows(nCounter).Item("SQLInsertItemTable") & " " & currentTableReference & " ON " & currentTableReference & "." & dtTables.Rows(nCounter).Item("ForeignID") & " = " & GetExportJoinID(dtTableReferences, dtTables.Rows(nCounter).Item("TableControlID")) & ".ID "
                End If

                If dtColumns.Rows(0).Item("ColumnControlID") = "0" Then
                    If dtTables.Rows(nCounter).Item("TableControlID") = "0" Then
                        dtColumns = GetExportColumns()
                    Else
                        If IsListControlType(GetControlColumnValue(dtTables.Rows(nCounter).Item("TableControlID"), "ControlType", controlsDT)) Then
                            dtColumns = GetDataTable("Select " & dtTables.Rows(nCounter).Item("TableControlID") & " as ColumnControlID,'" & GetControlColumnValue(dtTables.Rows(nCounter).Item("TableControlID"), "Name", controlsDT) & "' as Name", Common.General.Variables.cnx)
                        Else
                            dtColumns = GetExportColumns(dtTables.Rows(nCounter).Item("TableControlID"))
                        End If
                    End If
                End If

                Dim sControlReference As String
                Dim nControlID As Integer
                Dim loginColumn As LoginColumnType

                For Each CurrentRow As DataRow In dtColumns.Rows
                    If columnReferences <> "" Then
                        columnReferences &= ","
                    End If

                    loginColumn = (From lc In LoginColumnTypes
                                   Where lc.ID = CurrentRow.Item("ColumnControlID")
                                   Select lc).DefaultIfEmpty(Nothing).First()

                    If loginColumn Is Nothing Then
                        sControlReference = currentTableReference & ".[" & CurrentRow.Item("Name") & "]"
                        nControlID = 0

                        Try
                            nControlID = GetControlColumnValue(CurrentRow.Item("ColumnControlID"), "ControlType")
                        Catch ex As Exception

                        End Try

                        If IsYesNoControl(nControlID) Then
                            columnReferences &= "master.dbo.uft_FormatYesNo(" & sControlReference & ") as " & CurrentRow.Item("Name")
                        Else
                            columnReferences &= sControlReference
                        End If
                    Else
                        columnReferences &= "MT." & loginColumn.ColumnName
                    End If
                Next
            Next

            Return "Select " & columnReferences & " From " & tableReferences & If(sIDSelect <> "", " WHERE " & sIDSelect, "")
        ElseIf export.DataSourceType = "2" Then
            Return GetDataSourceSelectString(export.DataSourceID, "", "", "", sIDSelect, "", True)
        End If
    End Function

    Function GetExportJoinID(ByRef dtTableReferences As DataTable, ByVal sTableControlID As String)
        Dim sNextParentControlID, sTableReference As String

        Try
            If ParentIsRepeaterControl(sTableControlID, "-1", 0, sNextParentControlID, "", "", True) Then
                For Each Currentrow As DataRow In dtTableReferences.Rows
                    If Currentrow.Item("TableControlID") = sNextParentControlID Then
                        sTableReference = Currentrow.Item("Reference")
                    End If
                Next
            Else
                sTableReference = "MT"
            End If
        Catch ex As Exception
        End Try

        Return sTableReference
    End Function

    Sub RecordTableReference(ByRef dtTableReferences As DataTable, ByVal sTableControlID As String, ByVal sReference As String)
        Dim tempRow As DataRow

        If sTableControlID <> "0" Then
            tempRow = dtTableReferences.NewRow

            tempRow.Item("TableControlID") = sTableControlID
            tempRow.Item("Reference") = sReference

            dtTableReferences.Rows.Add(tempRow)
        End If
    End Sub
End Class
