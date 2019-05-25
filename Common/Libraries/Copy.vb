Imports System.Data
Imports Microsoft.VisualBasic
Imports WhitTools.Getter
Imports WhitTools.DataTables
Imports WhitTools.SQL
Imports Common.General.Main
Imports Common.General.Variables


Imports WhitTools.Utilities
Imports System.Collections

Public Class Copy

    Sub CopyProject(ByVal nProjectID As Integer)
        Dim nNewProjectID, nNewPageID As Integer
        Dim sProjectsColumns As String = GetTableColumns("Projects", "'ID','WorkflowStep','EcommerceProduct','NumberExports','SubmitterEmailMessage'")
        Dim sProjectPagesColumns As String = GetTableColumns("ProjectPages", "'ID'")
        Dim sProjectAdditionalOperationsColumns As String = GetTableColumns("ProjectAdditionalOperations", "'ID'")
        Dim sProjectControlPostbackActionsColumns As String = GetTableColumns("ProjectControlPostbackActions", "'ID'")

        ExecuteNonQuery("INSERT INTO " & DT_WEBRAD_PROJECTS & " (" & sProjectsColumns & ") SELECT " & sProjectsColumns & " FROM " & DT_WEBRAD_PROJECTS & " WHERE ID  = " & nProjectID)
        nNewProjectID = ExecuteScalar("Select ident_current('Projects')", Common.General.Variables.cnx)
        'WriteLine("INSERT INTO " & DT_WEBRAD_PROJECTS & " (" & sProjectsColumns & ") SELECT " & sProjectsColumns & " FROM " & DT_WEBRAD_PROJECTS & " WHERE ID  = " & nProjectID)
        'WriteLine("Select ident_current('Projects')")

        Dim ControlIDs As Hashtable = New Hashtable()

        For Each PageRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & nProjectID).Rows
            ExecuteNonQuery("INSERT INTO " & DT_WEBRAD_PROJECTPAGES & " (" & sProjectPagesColumns & ") SELECT " & Replace(sProjectPagesColumns, "[ProjectID]", nNewProjectID & " AS ProjectID") & " FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ID  = " & PageRow.Item("ID"))
            nNewPageID = ExecuteScalar("Select ident_current('ProjectPages')", Common.General.Variables.cnx)
            'WriteLine("INSERT INTO " & DT_WEBRAD_PROJECTPAGES & " (" & sProjectPagesColumns & ") SELECT " & Replace(sProjectPagesColumns, "[ProjectID]", nNewProjectID & " AS ProjectID") & " FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ID  = " & PageRow.Item("ID"))
            'WriteLine("Select ident_current('ProjectPages')")

            For Each ControlRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE PageID = " & PageRow.Item("ID") & " AND ParentControlID IS NULL").Rows
                CopyControl(ControlRow, nNewProjectID, nNewPageID, ControlIDs)
            Next

            For Each AORow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " WHERE PageID = " & nNewPageID).Rows
                ExecuteNonQuery("INSERT INTO " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " (" & sProjectAdditionalOperationsColumns & ") SELECT " & Replace(Replace(sProjectAdditionalOperationsColumns, "[ProjectID]", nNewProjectID & " AS ProjectID"), "[PageID]", nNewPageID & " AS PageID") & " FROM " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " WHERE ID  = " & AORow.Item("ID"))
                nNewPageID = ExecuteScalar("Select ident_current('ProjectAdditionalOperations')", Common.General.Variables.cnx)
                'WriteLine("INSERT INTO " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " (" & sProjectAdditionalOperationsColumns & ") SELECT " & Replace(Replace(sProjectAdditionalOperationsColumns, "[ProjectID]", nNewProjectID & " AS ProjectID"), "[PageID]", nNewPageID & " AS PageID") & " FROM " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " WHERE ID  = " & AORow.Item("ID"))
                'WriteLine("Select ident_current('ProjectAdditionalOperations')")
            Next
        Next

        For Each ActionRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE Triggercontrol in (Select ID FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & nProjectID & ")", Common.General.Variables.cnx).Rows
            Dim nNewDataSourceID As Integer = GetNewDataSource(ActionRow.Item("DataSourceID"))
            Dim nNewPostbackActionID As Integer
            Dim sProjectControlPostbackActionTriggerValuesColumns As String = GetTableColumns("ProjectControlPostbackActionTriggerValues", "'ID'")

            'WriteLine("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " (" & sProjectControlPostbackActionsColumns & ") SELECT " & Replace(Replace(Replace(sProjectControlPostbackActionsColumns, "[TriggerControl]", ControlIDs(ActionRow.Item("TriggerControl")) & " AS TriggerControl"), "[TargetControl]", ControlIDs(ActionRow.Item("TargetControl")) & " AS TargetControl"), "[DataSourceID]", If(nNewDataSourceID <> 0, nNewDataSourceID & " AS DataSourceID", "DataSourceID")) & " FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE ID  = " & ActionRow.Item("ID"))
            'WriteLine("Select ident_current('ProjectControlPostbackActions')")

            Try
                ExecuteNonQuery("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " (" & sProjectControlPostbackActionsColumns & ") SELECT " & Replace(Replace(Replace(sProjectControlPostbackActionsColumns, "[TriggerControl]", ControlIDs(ActionRow.Item("TriggerControl")) & " AS TriggerControl"), "[TargetControl]", ControlIDs(ActionRow.Item("TargetControl")) & " AS TargetControl"), "[DataSourceID]", If(nNewDataSourceID <> 0, nNewDataSourceID & " AS DataSourceID", "DataSourceID")) & " FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE ID  = " & ActionRow.Item("ID"))
                nNewPostbackActionID = ExecuteScalar("Select ident_current('ProjectControlPostbackActions')", Common.General.Variables.cnx)
            Catch ex As Exception
                'Logger.Error(ex.ToString)
                Exit Sub
            End Try

            'WriteLine("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONTRIGGERVALUES & " (" & sProjectControlPostbackActionTriggerValuesColumns & ") SELECT " & Replace(sProjectControlPostbackActionTriggerValuesColumns, "[ActionID]", nNewPostbackActionID & " AS ActionID") & " FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONTRIGGERVALUES & " WHERE ActionID  = " & ActionRow.Item("ID"))
            ExecuteNonQuery("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONTRIGGERVALUES & " (" & sProjectControlPostbackActionTriggerValuesColumns & ") SELECT " & Replace(sProjectControlPostbackActionTriggerValuesColumns, "[ActionID]", nNewPostbackActionID & " AS ActionID") & " FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONTRIGGERVALUES & " WHERE ActionID  = " & ActionRow.Item("ID"))

        Next

        For Each ColumnRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCOLUMNS & " WHERE ProjectID = " & nProjectID, Common.General.Variables.cnx).Rows
            Dim nNewPostbackActionID As Integer
            Dim sProjectColumnsColumns As String = GetTableColumns("ProjectColumns", "'ID'")

            'WriteLine("INSERT INTO " & DT_WEBRAD_PROJECTCOLUMNS & " (" & sProjectColumnsColumns & ") SELECT " & Replace(Replace(Replace(sProjectColumnsColumns, "[ProjectID]", nNewProjectID & " AS ProjectID"), "TableControlID", ControlIDs(ColumnRow.Item("TableControlID")) & " AS TableControlID"), "ColumnControlID", ControlIDs(ColumnRow.Item("ColumnControlID")) & " AS ColumnControlID") & " FROM " & DT_WEBRAD_PROJECTCOLUMNS & " WHERE ID  = " & ColumnRow.Item("ID"))
            'WriteLine("Select ident_current('ProjectColumns')")

            Try
                ExecuteNonQuery("INSERT INTO " & DT_WEBRAD_PROJECTCOLUMNS & " (" & sProjectColumnsColumns & ") SELECT " & Replace(Replace(Replace(sProjectColumnsColumns, "[ProjectID]", nNewProjectID & " AS ProjectID"), "[TableControlID]", IIf(ControlIDs(ColumnRow.Item("TableControlID")) <> "", ControlIDs(ColumnRow.Item("TableControlID")), "0") & " AS TableControlID"), "[ColumnControlID]", If(ControlIDs(ColumnRow.Item("ColumnControlID")) <> "", ControlIDs(ColumnRow.Item("ColumnControlID")), "0") & " AS ColumnControlID") & " FROM " & DT_WEBRAD_PROJECTCOLUMNS & " WHERE ID  = " & ColumnRow.Item("ID"))
                nNewPostbackActionID = ExecuteScalar("Select ident_current('ProjectColumns')", Common.General.Variables.cnx)
            Catch ex As Exception
                'Logger.Error(ex.ToString)
                Exit Sub
            End Try
        Next

    End Sub

    Function GetTableColumns(ByVal sTableName As String, ByVal sExceptions As String) As String
        Return "[" & GetListofValues("select * from information_schema.columns where table_name = '" & sTableName & "'" & IIf(sExceptions <> "", "AND NOT COLUMN_NAME IN (" & sExceptions & ")", ""), "COLUMN_NAME", "],[", "", Common.General.Variables.cnx) & "]"
    End Function

    Public Shared Sub CopyControl(ByRef CurrentRow As DataRow, ByVal nNewProjectID As Integer, ByVal nNewPageID As Integer, ByRef ControlIds As Hashtable, Optional ByVal nParentControlID As Integer = 0)

        Dim sProjectControlsColumns As String = "[" & GetListofValues("select * from information_schema.columns where table_name = 'ProjectControls' AND NOT COLUMN_NAME IN ('ID')", "COLUMN_NAME", "],[", "", Common.General.Variables.cnx) & "]"
        Dim sProjectControlFileTypesAllowedColumns As String = "[" & GetListofValues("select * from information_schema.columns where table_name = 'ProjectControlFileTypesAllowed' AND NOT COLUMN_NAME IN ('ID')", "COLUMN_NAME", "],[", "", Common.General.Variables.cnx) & "]"
        Dim sProjectControlListItemsColumns As String = "[" & GetListofValues("select * from information_schema.columns where table_name = 'ProjectControlListItems' AND NOT COLUMN_NAME IN ('ID')", "COLUMN_NAME", "],[", "", Common.General.Variables.cnx) & "]"

        Dim nNewControlID, nNewDataSourceID As Integer

        Try
            nNewDataSourceID = GetNewDataSource(CurrentRow.Item("DataSourceID"))
        Catch ex As Exception
            'Logger.Error(ex.ToString)
            Exit Sub
        End Try

        'WriteLine("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLS & " (" & sProjectControlsColumns & ") SELECT " & Replace(Replace(Replace(Replace(sProjectControlsColumns, "[ProjectID]", nNewProjectID & " AS ProjectID"), "[PageID]", nNewPageID & " AS PageID"), "[DataSourceID]", If(nNewDataSourceID <> 0, nNewDataSourceID & " AS DataSourceID", "DataSourceID")), "[ParentControlID]", IIf(nParentControlID <> 0, nParentControlID & " AS ParentControlID", "ParentControlID")) & " FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ID  = " & CurrentRow.Item("ID"))
        'WriteLine("Select ident_current('ProjectControls')")

        ExecuteNonQuery("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLS & " (" & sProjectControlsColumns & ") SELECT " & Replace(Replace(Replace(Replace(sProjectControlsColumns, "[ProjectID]", nNewProjectID & " AS ProjectID"), "[PageID]", nNewPageID & " AS PageID"), "[DataSourceID]", If(nNewDataSourceID <> 0, nNewDataSourceID & " AS DataSourceID", "DataSourceID")), "[ParentControlID]", IIf(nParentControlID <> 0, nParentControlID & " AS ParentControlID", "ParentControlID")) & " FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ID  = " & CurrentRow.Item("ID"))
        nNewControlID = ExecuteScalar("Select ident_current('ProjectControls')", Common.General.Variables.cnx)

        ControlIds.Add(CurrentRow.Item("ID"), nNewControlID)


        ExecuteNonQuery("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & " (" & sProjectControlFileTypesAllowedColumns & ") SELECT " & Replace(sProjectControlFileTypesAllowedColumns, "[ControlID]", nNewControlID & " AS ControlID") & " FROM " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & " WHERE ControlID  = " & CurrentRow.Item("ID"))
        ExecuteNonQuery("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLLISTITEMS & " (" & sProjectControlListItemsColumns & ") SELECT " & Replace(sProjectControlListItemsColumns, "[ParentID]", nNewControlID & " AS ParentID") & " FROM " & DT_WEBRAD_PROJECTCONTROLLISTITEMS & " WHERE ParentID  = " & CurrentRow.Item("ID"))
        ExecuteNonQuery("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & " (" & sProjectControlFileTypesAllowedColumns & ") SELECT " & Replace(sProjectControlFileTypesAllowedColumns, "[ControlID]", nNewControlID & " AS ControlID") & " FROM " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & " WHERE ControlID  = " & CurrentRow.Item("ID"))

        'WriteLine("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & " (" & sProjectControlFileTypesAllowedColumns & ") SELECT " & Replace(sProjectControlFileTypesAllowedColumns, "[ControlID]", nNewControlID & " AS ControlID") & " FROM " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & " WHERE ControlID  = " & CurrentRow.Item("ID"))
        'WriteLine("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLLISTITEMS & " (" & sProjectControlListItemsColumns & ") SELECT " & Replace(sProjectControlListItemsColumns, "[ParentID]", nNewControlID & " AS ParentID") & " FROM " & DT_WEBRAD_PROJECTCONTROLLISTITEMS & " WHERE ParentID  = " & CurrentRow.Item("ID"))
        'WriteLine("INSERT INTO " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & " (" & sProjectControlFileTypesAllowedColumns & ") SELECT " & Replace(sProjectControlFileTypesAllowedColumns, "[ControlID]", nNewControlID & " AS ControlID") & " FROM " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & " WHERE ControlID  = " & CurrentRow.Item("ID"))


        For Each ChildControlRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ParentControlID = " & CurrentRow.Item("ID")).Rows
            'WriteLine("select children - SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ParentControlID = " & CurrentRow.Item("ID"))

            CopyControl(ChildControlRow, nNewProjectID, nNewPageID, ControlIds, nNewControlID)
            'Exit Sub
        Next
    End Sub

    Public Shared Function GetNewDataSource(ByVal nDataSourceID As Integer) As Integer
        Dim sProjectDataSourcesColumns As String = "[" & GetListofValues("select * from information_schema.columns where table_name = 'ProjectDataSources' AND NOT COLUMN_NAME IN ('ID')", "COLUMN_NAME", "],[", "", Common.General.Variables.cnx) & "]"

        If nDataSourceID <> 0 Then
            'WriteLine("INSERT INTO " & DT_WEBRAD_PROJECTDATASOURCES & " (" & sProjectDataSourcesColumns & ") SELECT " & sProjectDataSourcesColumns & " FROM " & DT_WEBRAD_PROJECTDATASOURCES & " WHERE ID  = " & nDataSourceID)
            'WriteLine("Select ident_current('ProjectDataSources')")

            Try
                ExecuteNonQuery("INSERT INTO " & DT_WEBRAD_PROJECTDATASOURCES & " (" & sProjectDataSourcesColumns & ") SELECT " & sProjectDataSourcesColumns & " FROM " & DT_WEBRAD_PROJECTDATASOURCES & " WHERE ID  = " & nDataSourceID)
                Return ExecuteScalar("Select ident_current('ProjectDataSources')", Common.General.Variables.cnx)

            Catch ex As Exception
                'Logger.Error(ex.ToString)
                Exit Function
            End Try

        End If

        Return 0
    End Function
End Class
