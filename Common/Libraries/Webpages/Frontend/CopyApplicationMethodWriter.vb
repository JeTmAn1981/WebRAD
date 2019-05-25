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
Imports Common.SQL.Main
Imports Common.Webpages.Frontend.Email
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Export
Imports Common.Webpages.Backend.Schedule
Imports Common.General.Links
Imports Common.Webpages.BindData
Imports Common.Webpages.Backend.Archive
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File
Imports WhitTools.Utilities


Public Class CopyApplicationMethodWriter
    Dim loginColumns, loginValues, copyApplicationMethod As String
    Dim SQLMainTable As String

    Public Sub New(ByVal SQLMainTable As String)
        Me.SQLMainTable = SQLMainTable
    End Sub

    Public Function GetMethod() As String
        If isFrontend Then
            CreateLoginData()
            CreateMethod()
        End If

        Return copyApplicationMethod
    End Function

    Private Sub CreateMethod()
        copyApplicationMethod &= "Public Shared Sub CopyApplication(ByVal previousApplicationID As Integer)" & vbCrLf
        copyApplicationMethod &= GetRetainedControls()
        copyApplicationMethod &= "End Sub" & vbCrLf
    End Sub

    Private Sub CreateLoginData()
        If CurrentProjectRequiresWhitworthLogin() Then
            LoginColumnTypes.FindAll(Function(l) l.IncludeSelectStatement = True).ForEach(
                Sub(l)
                    loginColumns &= l.ColumnName & ","
                    loginValues &= "UI." & l.ColumnName & ","
                End Sub)
        End If
    End Sub

    Private Function GetRetainedControls()
        Dim tables As DataTable = GetDataTable("Select Distinct(TableControlID), SQLInsertItemTable, Name, ForeignID from " & DT_WEBRAD_PROJECTCOLUMNS & " PCL LEFT OUTER JOIN " & DT_WEBRAD_PROJECTCONTROLS & " C On PCL.TableControlID = C.ID WHERE Type='Retained' AND PCL.ProjectID=" & GetProjectID())
        Dim insertRetained As String = ""

        Dim mainTable = (From currentrow As DataRow In tables Where currentrow.Item("TableControlID") = "0" Select currentrow).FirstOrDefault
        Dim otherTables = (From currentrow As DataRow In tables Where currentrow.Item("TableControlID") <> "0" AndAlso Not ParentIsRepeaterControl(GetControlColumnValue(currentrow.Item("TableControlID"), "ParentControlID"))
                           Select currentrow).ToList

        insertRetained &= GetInsertMainTable(mainTable)

        For Each CurrentTable As DataRow In otherTables
            insertRetained &= GetInsertOtherTable(CurrentTable)
        Next

        Return insertRetained
    End Function

    Function GetInsertMainTable(ByVal mainTable As DataRow) As String
        Dim passwordColumn As String = IIf(CurrentProjectRequiresNonWhitworthLogin(), ",Password", "")
        Dim insertMainTable As String = ""

        Dim columns As String = ""
        Dim values As String = ""

        columns = loginColumns
        values = loginValues

        If mainTable IsNot Nothing Then
            columns &= GetRetainedColumns(mainTable.Item("ID")) & ","
            values &= GetRetainedColumns(mainTable.Item("ID")) & ","
        End If

        columns &= "Username"
        columns &= passwordColumn

        values &= "MT.Username"
        values &= passwordColumn

        insertMainTable &= "ExecuteNonQuery(""Insert Into " & SQLMainTable & " (" & columns & ") "
        insertMainTable &= "SELECT TOP 1 " & values & " FROM " & SQLMainTable & " MT LEFT OUTER JOIN "" & DV_USERINFO_V & "" UI ON Mt.Username = UI.Username WHERE MT.ID = "" & previousApplicationID,cnx)" & vbCrLf & vbCrLf

        insertMainTable &= "Dim currentApplicationID As Integer = ExecuteScalar(""Select IDENT_CURRENT('" & SQLMainTable & "')"", cnx)" & vbCrLf & vbCrLf

        Return insertMainTable
    End Function


    Function GetInsertOtherTable(ByVal table As DataRow, Optional ByVal counter As Integer = 1, Optional ByVal previousID As String = "previousApplicationID", Optional ByVal currentID As String = "currentApplicationID") As String
        Dim insertOtherTable As String = ""

        With table
            Dim tableControlID = .Item("TableControlID")
            Dim SQLTable = .Item("SQLInsertItemTable")
            Dim foreignID = .Item("ForeignID")

            Dim controlInfo As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ID = " & tableControlID & " AND ParentControlID IS NULL")

            insertOtherTable &= "For Each CurrentRow" & counter & " as DataRow in GetDataTable(""SELECT * FROM " & SQLTable & " WHERE " & foreignID & " = "" & " & previousID & ",cnx).Rows" & vbCrLf
            insertOtherTable &= "ExecuteNonQuery(""Insert Into " & SQLTable & " (" & foreignID & "," & GetRetainedColumns(tableControlID) & ") "
            insertOtherTable &= "SELECT "" & " & currentID & " & ""," & GetRetainedColumns(tableControlID) & " FROM " & SQLTable & " WHERE ID = "" & CurrentRow" & counter & ".Item(""ID""),cnx)" & vbCrLf

            Dim childControls = GetDataTable("select Distinct(TableControlID), SQLInsertItemTable, Name, ForeignID from " & DT_WEBRAD_PROJECTCOLUMNS & " PCL LEFT OUTER JOIN " & DT_WEBRAD_PROJECTCONTROLS & " C ON PCL.TableControlID = C.ID WHERE Type='Retained' and tableControlid in (select id from " & DT_WEBRAD_PROJECTCONTROLS & " where parentcontrolid = " & tableControlID & ")")

            If childControls.Rows.Count > 0 Then
                insertOtherTable &= "Dim nCurrentID" & counter & " As Integer = ExecuteScalar(""Select TOP 1 ID FROM " & SQLTable & " ORDER BY ID DESC"", cnx)" & vbCrLf & vbCrLf

                For Each CurrentTable As DataRow In childControls.Rows
                    insertOtherTable &= GetInsertOtherTable(CurrentTable, counter + 1, "nPreviousID", "nCurrentID" & counter)
                Next
            End If

            insertOtherTable &= "Next" & vbCrLf & vbCrLf
        End With

        Return insertOtherTable
    End Function

    Function GetRetainedColumns(ByVal tableControlID As Integer) As String
        Dim retainedColumns As DataTable = GetDataTable("SELECT Name, ColumnControlID as ID FROM " & DT_WEBRAD_PROJECTCOLUMNS & " PCL LEFT OUTER JOIN (select ID, Name FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " UNION Select IDNumber, ControlName as Name FROM " & DT_WEBRAD_LOGINCOLUMNTYPES & ") C ON C.ID = PCL.ColumnControlID WHERE Type='Retained' AND PCL.ProjectID = " & GetProjectID() & " AND TableControlID = " & tableControlID)
        Dim columns As String = ""

        If retainedColumns.Rows.Count = 0 Then
            Return ""
        ElseIf retainedColumns.Rows(0).Item("ID") = "0" Then
            If IsListControlType(GetControlColumnValue(tableControlID, "ControlType")) Then
                Return GetControlColumnValue(tableControlID, "Name")
            Else
                retainedColumns = GetDataTable("SELECT Name, ID FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE IncludeDatabase = 1 AND ProjectID = " & GetProjectID() & " AND (SQLInsertItemTable IS NULL OR SQLInsertItemTable = '')")
            End If
        End If

        For Each CurrentColumn As DataRow In retainedColumns.Rows
            Try
                If (tableControlID = 0 And Not ParentIsRepeaterControl(CurrentColumn.Item("ID"))) Or (tableControlID <> 0 And ParentIsRepeaterControl(CurrentColumn.Item("ID"), tableControlID)) Then
                    If columns <> "" Then
                        columns &= ","
                    End If

                    columns &= CurrentColumn.Item("Name")
                End If
            Catch ex As Exception
                logger.Error(ex.ToString)
                logger.Error("curentcolumnid - " & CurrentColumn.Item("ID"))
            End Try
        Next

        Return columns
    End Function
End Class
