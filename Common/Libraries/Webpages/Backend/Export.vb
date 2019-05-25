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

Namespace Webpages.Backend
    Public Class Export
        Inherits Backend.Main



        Shared Function GetExportColumns(Optional ByVal nParentControlID As Integer = -1, Optional ByVal bIncludeMulti As Boolean = False) As DataTable
            Dim dtFullList As DataTable = GetDataTable("Select * FROM " & GetColumnSelectTable() & " Where ProjectID = " & GetProjectID() & If(Not bIncludeMulti, " AND (SQLInsertItemTable is null OR SQLInsertItemTable = '') ", "") & " AND IncludeDatabase = 1 Order by PageID, [Position]", cnx)

            Dim dtSelectedList As New DataTable
            Dim temprow As DataRow
            Dim sNextParentControlID As String = ""
            Dim bAddColumn As Boolean = False

            dtSelectedList.Columns.Add("ColumnControlID", GetType(String))
            dtSelectedList.Columns.Add("Name", GetType(String))

            For Each CurrentDatarow As DataRow In dtFullList.Rows
                With CurrentDatarow

                    If NoParentControl(.Item("ParentControlID")) And nParentControlID = -1 Then
                        bAddColumn = True
                    Else
                        If nParentControlID = -1 Then
                            If Not ParentIsControlType(.Item("ID"), "Repeater", -1, 0, "", "", True) Then
                                bAddColumn = True
                            End If
                        Else
                            If ParentIsControlType(.Item("ID"), "Repeater", nParentControlID, 0, sNextParentControlID, "", True, False) Then
                                bAddColumn = True
                            End If
                        End If
                    End If

                    If bAddColumn Then
                        temprow = dtSelectedList.NewRow
                        temprow.Item("ColumnControlID") = .Item("ID")
                        temprow.Item("Name") = .Item("Name")
                        dtSelectedList.Rows.Add(temprow)
                    End If

                    bAddColumn = False
                End With
            Next

            Return dtSelectedList
        End Function

        Shared Sub GetAdditionalExportColumns(ByRef rptTables As Repeater)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Declare local variables
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim dtAncillaryTables As DataTable = GetDataTable("Select ID,ControlType, Name, Heading, SQLInsertItemTable From " & DT_WEBRAD_PROJECTCONTROLS & " Where ProjectID = " & GetProjectID() & " and not SQLInsertItemTable is null and not SQLInsertItemTable = '' order by position asc", cnx)
            Dim dtChildControls As DataTable = GetDataTable("Select ID, Name, Heading, SQLInsertItemTable From " & DT_WEBRAD_PROJECTCONTROLS & " Where ProjectID = " & GetProjectID() & " And IncludeDatabase = 1", cnx)
            Dim sIDs As String = ""
            Dim dtCurrentChildren As DataTable

            SelectRepeaterData(rptTables, dtAncillaryTables, cnx)

            For nCounter = 0 To rptTables.Items.Count - 1
                For Each currentChild In dtChildControls.Rows
                    If ParentIsControlType(currentChild.Item("ID"), "Repeater", dtAncillaryTables.Rows(nCounter).Item("ID"), 0, "", "", True, False) Then
                        sIDs &= If(sIDs <> "", "," & currentChild.item("ID"), currentChild.Item("ID"))
                    End If
                Next

                If sIDs <> "" Then
                    dtCurrentChildren = GetDataTable("Select ID, Name, Heading, SQLInsertItemTable From " & DT_WEBRAD_PROJECTCONTROLS & " Where ID IN (" & sIDs & ")", cnx)
                    FillListData(CType(rptTables.Items(nCounter).FindControl("lsbColumns"), ListBox), dtCurrentChildren, "Name", "ID", False)
                Else
                    If IsListControlType(dtAncillaryTables.Rows(nCounter).Item("ControlType")) Then
                        CType(rptTables.Items(nCounter).FindControl("lsbColumns"), ListBox).Items.Insert(0, New ListItem(dtAncillaryTables.Rows(nCounter).Item("Name"), dtAncillaryTables.Rows(nCounter).Item("ID")))
                    End If
                End If

                sIDs = ""

                CType(rptTables.Items(nCounter).FindControl("lsbColumns"), ListBox).Items.Insert(0, New ListItem("All", "0"))
            Next
        End Sub

        Shared Function GetColumnSelectTable() As String
            Dim selectTable As String
            Dim projectID As Integer = GetProjectID()
            Dim project = GetDataTable("Select * From " & DT_WEBRAD_PROJECTS & " Where ID = " & projectID, CreateSQLConnection("WebRAD")).Rows(0)
            Dim prospect = ProjectIncludesProspectUserControl()

            selectTable = "("

            If project.Item("RequireLogin") = "1" Or prospect Then
                selectTable &= GetLoginColumns(prospect)
            End If

            '''Future use
            'If project.Item("Ecommerce") = "1" Then
            '    AddPaidColumn(selectTable)
            'End If

            AddProjectControlColumns(selectTable)
            selectTable &= "UNION "
            AddDateSubmittedColumn(selectTable, projectID)
            selectTable &= "AS ExportColumns "

            Return selectTable
        End Function

        Private Shared Sub AddProjectControlColumns(ByRef sSelectTable As String)
            sSelectTable &= "Select ProjectID, PageID, ID, Name, Heading, ParentControlID, SQLInsertItemTable, Position,IncludeDatabase From web3.WebRAD.dbo.ProjectControls "
        End Sub

        ''' <summary>
        ''' For future use
        ''' </summary>
        ''' <param name="sSelectTable"></param>
        ''' <param name="nProjectID"></param>
        'Private Shared Sub AddPaidColumn(ByRef sSelectTable As String)
        '    sSelectTable &= "SELECT " & nProjectID & " as ProjectID,(SELECT MIN(ID) FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & nProjectID & ") as PageID, " & N_IDNUMBER_CONTROLID & " as ID,'ID' as Name,'ID' as Heading,NULL as ParentControlID,'' as SQLInsertItemTable,-11 as Position,1 as IncludeDatabase "
        'End Sub

        Private Shared Sub AddDateSubmittedColumn(ByRef sSelectTable As String, nProjectID As Integer)
            sSelectTable &= "Select " & nProjectID & " as ProjectID,(SELECT MAX(ID) FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & nProjectID & ") as PageID, " & N_DATESUBMITTED_CONTROLID & " as ID, '" & GetDateSubmittedColumnReference() & "' as Name,'Date Submitted' as Heading,NULL as ParentControlID, '' as SQLInsertItemTable,(Select MAX(Position)+1 FROM web3.WebRAD.dbo.ProjectControls Where ProjectID = " & nProjectID & ") as Position,1 as IncludeDatabase) "
        End Sub

        Private Shared Sub AddSubmissionIDColumn(ByRef sSelectTable As String, nProjectID As Integer)
            sSelectTable &= "SELECT " & nProjectID & " as ProjectID,(SELECT MIN(ID) FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & nProjectID & ") as PageID, " & N_IDNUMBER_CONTROLID & " as ID,'ID' as Name,'ID' as Heading,NULL as ParentControlID,'' as SQLInsertItemTable,-11 as Position,1 as IncludeDatabase "
        End Sub

        Shared Function GetLoginColumns(Optional ByVal prospect As Boolean = False) As String
            Dim sLoginColumns As String = ""
            Dim nProjectID As Integer = GetProjectID()
            Dim dtProject As DataTable = GetDataTable("Select * From " & DT_WEBRAD_PROJECTS & " Where ID = " & nProjectID, CreateSQLConnection("WebRAD"))

            AddSubmissionIDColumn(sLoginColumns, nProjectID)
            sLoginColumns &= "UNION "

            Dim nPositionIndex As Integer = -10

            LoginColumnTypes.FindAll(Function(l) l.IncludeSelectStatement = True And (prospect = False Or (prospect And l.IncludeProspectUserControl))).ForEach(
                Sub(l)
                    sLoginColumns &= "Select " & nProjectID & " as ProjectID,(SELECT MIN(ID) FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & nProjectID & ") as PageID, " & l.ID & " as ID, '" & l.ColumnName & "' as Name, '" & l.DisplayName & "' as Heading, -1 as ParentControlID, '' as SQLInsertItemTable," & nPositionIndex & " as Position, 1 as IncludeDatabase UNION "
                    nPositionIndex += 1
                End Sub)

            Return sLoginColumns
        End Function
    End Class
End Namespace



