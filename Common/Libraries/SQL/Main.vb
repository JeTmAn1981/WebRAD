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
Imports Common.Webpages.Backend.Main
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
'Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File
Imports WhitTools.Utilities

Namespace SQL
    Public Class Main
        Shared Sub ProcessSQL()
            DropMainStoredProcedures()
            CreateSQL()
            MigrateArchiveTables()
        End Sub

        Public Shared migrationSQL As String

        Shared Sub MigrateArchiveTables()

            Try
                If GetBackendOption(S_BACKEND_OPTION_ARCHIVE_VIEW) AndAlso GetDataTable("SELECT * FROM WebRAD.dbo.ProjectArchiveMigrations WHERE ProjectID = " & GetProjectID() & " And Type='" & projectType & "'").Rows.Count = 0 Then
                    migrationSQL = ""

                    MigrateArchiveTable(GetCurrentProjectDT().Rows(0).Item("SQLMainTableName"), True)

                    For Each currentRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " And SQLInsertItemTable <> ''").Rows
                        MigrateArchiveTable(currentRow.Item("SQLInsertItemTable"))
                    Next

                    logger.Info("Migrating archival data for new process")
                    logger.Info(migrationSQL)

                    ExecuteNonQuery(migrationSQL, sqlcnx)
                    ExecuteNonQuery("INSERT INTO WebRAD.dbo.ProjectArchiveMigrations (ProjectID, Type) VALUES (" & GetProjectID() & ", '" & projectType & "')", cnx)
                End If
            Catch ex As Exception
                logger.Error("Error migrating SQL with the following code:")
                logger.Error(migrationSQL)
                logger.Error("Error encountered: " & ex.ToString)
            End Try
        End Sub

        Shared Sub MigrateArchiveTable(ByVal tableName As String, Optional ByVal main As Boolean = False)
            Dim selectColumns As String = GetListofValues("select * from INFORMATION_SCHEMA.columns where table_name='Archive_" & tableName & "' and not column_name in ('Deleted','Archived') and column_name in (select column_name from INFORMATION_SCHEMA.columns where table_name='" & tableName & "')", "column_Name", ",", "", sqlcnx)

            migrationSQL &= "Set IDENTITY_INSERT dbo." & tableName & " On;" & Environment.NewLine

            If main Then
                migrationSQL &= "UPDATE " & tableName & " Set Archived=1 WHERE ID In (Select ID FROM Archive_" & tableName & ");" & Environment.NewLine
            End If

            migrationSQL &= "INSERT INTO " & tableName & " (" & selectColumns & If(main, ", Deleted, Archived", "") & ") Select " & selectColumns & If(main, ", 1, 1", "") & " FROM Archive_" & tableName & " WHERE ID Not In (Select ID FROM " & tableName & ");" & Environment.NewLine
            migrationSQL &= "Set IDENTITY_INSERT dbo." & tableName & " OFF;" & Environment.NewLine & Environment.NewLine
        End Sub

        Shared Sub DropMainStoredProcedures()
            With GetCurrentProjectDT.Rows(0)
                DropStoredProcedure("usp_Insert" & .Item("SQLInsertStoredProcedureName"))

                If GetPageCount() > 1 Then
                    DropMultipageStoredProcedures(.Item("SQLUpdateStoredProcedureName"))
                    DropStoredProcedure("usp_Update" & projectDT.Rows(0).Item("SQLInsertStoredProcedureName") & "Certification")
                End If

                DropStoredProcedure("usp_Update" & .Item("SQLUpdateStoredProcedureName"))
            End With
        End Sub

        Public Shared Function SQLTableExists(ByVal tableName As String, ByVal dbcnx As SqlConnection) As Boolean
            Return (WhitTools.SQL.ExecuteScalar("SELECT Count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" & WhitTools.SQL.CleanSQL(tableName) & "')", dbcnx) > 0)
        End Function

        Shared Sub CreateSQL()
            Dim nCounter, nPageCount As Integer
            Dim sCreateTable, sCreateInsertProcedure, sCreateUpdateProcedure As String

            With GetCurrentProjectDT.Rows(0)
                nPageCount = GetPageCount()

                sCreateTable = "CREATE TABLE " & GetSQLDatabaseName(.Item("SQLDatabase")) & ".dbo." & .Item("SQLMainTableName")
                sCreateTable &= " ( "
                sCreateTable &= "ID int Not NULL IDENTITY (1, 1)"

                sCreateInsertProcedure = " CREATE PROCEDURE usp_Insert" & .Item("SQLInsertStoredProcedureName") & " " & vbCrLf
                sCreateUpdateProcedure = " CREATE PROCEDURE usp_Update" & .Item("SQLUpdateStoredProcedureName") & " " & vbCrLf

                pageNumber = -1

                logger.Info("Adding SQL Columns")
                AddSQLColumns(sCreateTable, sCreateInsertProcedure, sCreateUpdateProcedure)

                If nPageCount > 1 Then
                    For nPageCounter As Integer = 1 To nPageCount
                        sCreateTable &= ", Section" & nPageCounter & "Complete int Default 0"
                    Next

                    sCreateTable &= ",Certification nvarchar(1) Default 'N'"
                    sCreateTable &= ",CertificationDate datetime DEFAULT NULL"

                    modelProperties &= "public Nullable<string> Certification { get; set; }" & vbCrLf
                    modelProperties &= "public Nullable<System.DateTime> CertificationDate { get; set; }" & vbCrLf
                End If

                If ProjectIncludesProspectUserControl() Then
                    sCreateTable &= ",FirstName nvarchar(50) DEFAULT ''"
                    sCreateTable &= ",LastName nvarchar(50) DEFAULT ''"
                    sCreateTable &= ",Email nvarchar(50) DEFAULT ''"
                    sCreateTable &= ",OAID int DEFAULT NULL"
                End If

                sCreateTable &= ",DateSubmitted datetime DEFAULT getdate()"
                sCreateTable &= ",Deleted bit DEFAULT 0"

                If GetBackendOption(S_BACKEND_OPTION_ARCHIVE_VIEW) Then
                    sCreateTable &= ",Archived bit DEFAULT 0"
                End If

                sCreateTable &= ", PRIMARY KEY (ID)"
                sCreateTable &= " ) "

                CreateSQLTable(sCreateTable, .Item("SQLMainTableName"))

                currentProject.ProjectControls.ToList().Where(Function(pc) (pageNumber = -1 Or pc.PageID = GetPageInfo(pageNumber, "ID")) And pc.SQLInsertItemTable <> "").ToList().ForEach(
                    Sub(pc)
                        CreateAncillarySQL(pc, ControlTypeIsRepeater(pc.ControlType))
                    End Sub)

                If IsSingletonProject() Then
                    CreateSingletonRecord(.Item("SQLMainTableName"))
                End If

                CreateStoredProcedure(sCreateInsertProcedure)

                If nPageCount > 1 Then
                    For nCounter = 1 To nPageCount
                        pageNumber = nCounter

                        CreateStoredProcedure(CreateUpdateSectionProcedure())
                    Next

                    CreateStoredProcedure(GetCertificationStoredProcedure())
                End If

                CreateStoredProcedure(sCreateUpdateProcedure)
                CreateScheduleSQL()
                'CreateSupervisorSQL()
            End With
        End Sub

        Private Shared Sub CreateStoredProcedure(procedureBody As String)
            Dim cmd As New SqlCommand()
            cmd.Connection = sqlcnx

            Try
                sqlcnx.Close()
            Catch ex As Exception

            End Try

            Try
                logger.Info("Creating stored procedure:")
                logger.Info(procedureBody)
                cmd.CommandText = procedureBody
                sqlcnx.Open()
                cmd.ExecuteNonQuery()
                sqlcnx.Close()
            Catch ex As Exception
                sqlcnx.Close()

                logger.Error(sqlcnx.ConnectionString)
                logger.Error(ex.ToString)
            End Try
        End Sub

        Public Shared Sub CreateScheduleSQL()
            'If GetBackendOption(S_BACKEND_OPTION_SCHEDULE) Then
            CreateScheduleSQLTable()
            CreateScheduleSQLSP()
            'End If
        End Sub

        Public Shared Sub CreateScheduleSQLSP()
            DropStoredProcedure("usp_Update" & GetScheduleTableName())
            CreateStoredProcedure(GetUpdateScheduleProcedure())
        End Sub

        Private Shared Sub CreateScheduleSQLTable()
            Dim sCreateTable As String

            With GetCurrentProjectDT.Rows(0)
                sCreateTable = "CREATE TABLE " & GetSQLDatabaseName(.Item("SQLDatabase")) & ".dbo." & GetScheduleTableName()
                sCreateTable &= " ( "
                sCreateTable &= "OpenDate datetime"
                sCreateTable &= ",OpenTime nvarchar(10)"
                sCreateTable &= ",CloseDate datetime"
                sCreateTable &= ",CloseTime nvarchar(10)"
                sCreateTable &= ",Message nvarchar(MAX)"
                sCreateTable &= " ) "

                CreateSQLTable(sCreateTable, GetScheduleTableName())
            End With
        End Sub

        Shared Sub CreateAncillarySQL(ByRef control As ProjectControl, Optional ByVal bIsRepeater As Boolean = False)
            Dim sCreateTable, sCreateInsertProcedure, sInsertParameters, sInsertColumns, sInsertValues, sUpdateColumns As New StringBuilder()
            Dim foreignKey As String
            Dim RepeaterControlsDT As New DataTable
            Dim nCounter As Integer

            With control
                foreignKey = .ForeignID

                Try
                    ExecuteNonQuery("DROP PROCEDURE  usp_Insert" & .SQLInsertItemStoredProcedure, sqlcnx)
                Catch ex As Exception
                End Try

                Dim tableName As String = GetSQLDatabaseName(currentProject.SQLDatabase) & ".dbo." & .SQLInsertItemTable

                sCreateTable.Append("CREATE TABLE " & tableName)
                sCreateTable.Append(" ( ")
                sCreateTable.Append("ID int NOT NULL IDENTITY (1, 1)")
                sCreateTable.Append("," & foreignKey & " int NOT NULL")

                If bIsRepeater Then
                    currentProject.ProjectControls.ToList().Where(Function(pc) ParentIsRepeaterControl(pc.ID, .ID)).ToList().ForEach(
                        Sub(pc)
                            With pc
                                If .IncludeDatabase = "1" And .SQLInsertItemTable = "" Then
                                    sCreateTable.Append(",[" & .Name & "] ")
                                    sCreateTable.Append(GetSQLDataTypeName(.SQLDataType, .ID, "CreateRepeaterAncillarySQL"))

                                    sInsertParameters.Append("," & vbCrLf & "@" & .Name & " " & GetSQLDataTypeName(.SQLDataType, .ID, "CreateRepeaterAncillarySQL2"))
                                    sInsertColumns.Append("," & vbCrLf & "[" & .Name & "]")
                                    sInsertValues.Append("," & vbCrLf & "@" & .Name)
                                    sUpdateColumns.Append("," & vbCrLf & "[" & .Name & "] = @" & .Name)

                                    If .SQLDataType = "1" Then
                                        sCreateTable.Append("(" & .SQLDataSize & ")")

                                        sInsertParameters.Append("(" & .SQLDataSize & ")")
                                    End If
                                End If
                            End With
                        End Sub)
                Else
                    sCreateTable.Append(",[" & .Name & "] ")
                    sCreateTable.Append(GetSQLDataTypeName(.SQLDataType, .ID, "CreateAncillarySQL"))

                    sInsertParameters.Append("," & vbCrLf & "@" & .Name & " " & GetSQLDataTypeName(.SQLDataType, .ID, "CreateAncillarySQL2"))
                    sInsertColumns.Append("," & vbCrLf & "[" & .Name & "]")
                    sInsertValues.Append("," & vbCrLf & "@" & .Name)
                    sUpdateColumns.Append("," & vbCrLf & "[" & .Name & "] = @" & .Name)

                    If .SQLDataType = "1" Then
                        sCreateTable.Append("(" & .SQLDataSize & ")")
                        sInsertParameters.Append("(" & .SQLDataSize & ")")
                    End If
                End If

                sCreateInsertProcedure.Append(" CREATE PROCEDURE usp_Insert" & .SQLInsertItemStoredProcedure & " " & vbCrLf)
                sCreateInsertProcedure.Append("@ID int = 0," & vbCrLf)
                sCreateInsertProcedure.Append("@" & foreignKey & " int")
                sCreateInsertProcedure.Append(sInsertParameters)
                sCreateInsertProcedure.Append(vbCrLf & vbCrLf & "AS" & vbCrLf & vbCrLf & "BEGIN" & vbCrLf & vbCrLf)
                sCreateInsertProcedure.Append("IF @ID = 0" & vbCrLf)
                sCreateInsertProcedure.Append("BEGIN" & vbCrLf)
                sCreateInsertProcedure.Append("INSERT " & .SQLInsertItemTable & vbCrLf)
                sCreateInsertProcedure.Append("(" & foreignKey & "")
                sCreateInsertProcedure.Append(sInsertColumns)
                sCreateInsertProcedure.Append(")" & vbCrLf & vbCrLf)
                sCreateInsertProcedure.Append("VALUES" & vbCrLf & vbCrLf)
                sCreateInsertProcedure.Append("(@" & foreignKey & "")
                sCreateInsertProcedure.Append(sInsertValues)
                sCreateInsertProcedure.Append(")" & vbCrLf & vbCrLf)
                sCreateInsertProcedure.Append("SET @ID = (SELECT IDENT_CURRENT('" & .SQLInsertItemTable & "'))" & vbCrLf)
                sCreateInsertProcedure.Append("END" & vbCrLf)
                sCreateInsertProcedure.Append("ELSE" & vbCrLf)
                sCreateInsertProcedure.Append("BEGIN" & vbCrLf)
                sCreateInsertProcedure.Append("UPDATE " & .SQLInsertItemTable & " SET " & vbCrLf)
                sCreateInsertProcedure.Append(foreignKey & " = @" & foreignKey)
                sCreateInsertProcedure.Append(sUpdateColumns.ToString() & vbCrLf)
                sCreateInsertProcedure.Append("WHERE ID = @ID" & vbCrLf)
                sCreateInsertProcedure.Append("END" & vbCrLf)

                sCreateInsertProcedure.Append("SELECT @ID" & vbCrLf & vbCrLf)
                sCreateInsertProcedure.Append("END")

                sCreateTable.Append(", PRIMARY KEY (ID)")
                sCreateTable.Append(" ) ")

                CreateSQLTable(sCreateTable.ToString(), .SQLInsertItemTable)

                AddForeignKey(control, foreignKey, tableName)

                If GetBackendOption(S_BACKEND_OPTION_ARCHIVE_VIEW) Then
                    sCreateTable.Append(Replace(sCreateTable.ToString(), .SQLInsertItemTable, "Archive_" & .SQLInsertItemTable))
                    sCreateTable.Append(Replace(sCreateTable.ToString(), " IDENTITY (1, 1)", ""))

                    CreateSQLTable(sCreateTable.ToString(), "Archive_" & .SQLInsertItemTable)
                End If


                Try
                    ExecuteNonQuery(sCreateInsertProcedure.ToString(), sqlcnx)
                Catch ex As Exception
                    cnx.Close()
                    logger.Error(ex.ToString & "<br /><br />")
                    logger.Error(sCreateInsertProcedure.ToString())
                End Try
            End With
        End Sub

        Private Shared Sub AddForeignKey(control As ProjectControl, foreignKey As String, tableName As String)
            Dim parentTableName As String
            Dim parentControlID As String = ""

            With control
                If ParentIsRepeaterControl(.ID,,, parentControlID,,, True) Then
                    parentTableName = currentProject.ProjectControls.FirstOrDefault(Function(pc) pc.ID.ToString() = parentControlID).SQLInsertItemTable
                Else
                    parentTableName = currentProject.SQLMainTableName
                End If

                Dim constraintName As String = "FK_" & .SQLInsertItemTable & "_" & parentTableName
                Dim addForeignKey As String = "IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = '" & constraintName & "')"

                addForeignKey &= "ALTER TABLE " & tableName & " ADD CONSTRAINT " & constraintName & " FOREIGN KEY (" & foreignKey & ") REFERENCES " & parentTableName & "(ID) ON DELETE CASCADE"

                logger.Info(addForeignKey)
                ExecuteNonQuery(addForeignKey, sqlcnx)
            End With
        End Sub

        Shared Function GetSQLDatabaseName(ByVal nSQLDatabase As Integer) As String
            Return If(projectType = "Test", "Test2", GetDataTable(cnx, "Select * From " & DT_WEBRAD_SQLDATABASES & " Where ID = " & nSQLDatabase).Rows(0).Item("Name"))
        End Function

        Shared Function GetSQLDatabaseName() As String
            Return GetSQLDatabaseName(currentProject.SQLDatabase)
        End Function

        Shared Function GetSQLServerName(ByVal sSQLServerName As String) As String
            Return If(sSQLServerName <> "web3", ",""" & sSQLServerName & """", "")
        End Function

        Shared Function GetUsernameFieldReference() As String
            Return "Username nvarchar(100)"
        End Function

        Shared Function GetPasswordFieldReference() As String
            Return "Password nvarchar(MAX)"
        End Function

        Public shared    function DataTypeIsNumber(ByVal nSQLDataType As Integer) As Boolean
        return (nSQLDataType = N_SQL_INT_TYPE or nSQLDataType = N_SQL_FLOAT_TYPE)
    End function

    public shared Function NeedSQLSize(byref dataType As DropDownList) As Boolean
        Try
                
            if datatype.selectedindex > 0 then
                   Dim dt As New DataTable

                    dt = GetDataTable(cnx, "Select * From " & DT_WEBRAD_CONTROLSQLTYPES & "  Where ID = " & datatype.selectedvalue)

                    With dt.Rows(0)
                        Return CBool(.Item("NeedSize"))
                    End With
            end if
          Catch ex As Exception
                
            End Try

            Return false
    End Function

    Shared public Function GetProjectSQLConnection() As SqlConnection
            Return WhitTools.SQL.CreateSQLConnection(GetSQLDatabaseName(GetCurrentProjectDT().Rows(0).Item("SQLDatabase")))
        End Function

        Public Shared Sub ExecuteNonQuery(ByVal SQL As String, Optional ByVal cnx As SqlConnection = Nothing)
            If cnx Is Nothing Then
                cnx = WhitTools.SQL.CreateSQLConnection()
            End If

            Dim cmd As New SqlCommand

            cmd.Connection = cnx
            cmd.CommandText = SQL

            cmd.Connection.Open()
            cmd.ExecuteNonQuery()
            cmd.Connection.Close()
        End Sub


        Public Shared Function Executescalar(ByVal SQL As String, Optional ByVal cnx As SqlConnection = Nothing) As String
            If cnx Is Nothing Then
                cnx = WhitTools.SQL.CreateSQLConnection()
            End If

            Dim cmd As New SqlCommand
            Dim returnValue As String

            cmd.Connection = cnx
            cmd.CommandText = SQL

            cmd.Connection.Open()
            returnValue = cmd.ExecuteScalar()
            cmd.Connection.Close()

            Return returnValue
        End Function
    End Class
End Namespace
