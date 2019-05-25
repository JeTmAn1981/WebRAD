Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.ProjectOperations
Imports Common.General.Pages
Imports Common.General.Controls
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.DataTypes
Imports Common.General.Folders
Imports Common.General.DataSources
Imports Common.General.Columns
Imports Common.General.Assembly
Imports Common.Webpages.Frontend.MultiPage
Imports Common.SQL.Main
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
    Public Class Table
        Shared Sub CreateSQLTable(ByVal sCreateTable As String, ByVal sTableName As String)
            Dim cmd As New SqlCommand
            cmd.Connection = sqlcnx

            If CheckSQLObjectExists(sTableName, sqlcnx) Then
                logger.Info("Aligning SQL table - " & sTableName)

                Try
                    If CheckSQLObjectExists(sTableName & "_Temp", sqlcnx) Then
                        DropTempTable(sTableName, cmd)
                    End If
                Catch ex As Exception
                    logger.Error(ex.ToString)
                    sqlcnx.Close()
                End Try

                CreateTempTable(sCreateTable, sTableName, cmd)
                AlignTables(sTableName & "_Temp", sTableName, GetSQLDatabaseName(projectDT.Rows(0).Item("SQLDatabase")), sqlcnx)
                DropTempTable(sTableName, cmd)
            Else
                CreateTable(sCreateTable, sTableName, cmd)
            End If
        End Sub

        Shared Function CheckSQLObjectExists(ByVal sObjectName As String, Optional ByVal cnx As SqlConnection = Nothing) As Boolean
            Try
                WhitTools.SQL.CheckConnection(cnx)
                Dim cmd As New SqlCommand

                If sObjectName = S_EMPTY_VALUE Then
                    Return False
                End If

                cmd.Connection = cnx
                cmd.CommandText = "IF object_id('" & sObjectName & "') IS NOT NULL SELECT 'True' ELSE SELECT 'False'"

                If cmd.Connection.State = ConnectionState.Closed Then
                    cmd.Connection.Open()
                End If

                Dim bObjectExists = cmd.ExecuteScalar

                cnx.Close()

                Return CBool(bObjectExists)
            Catch ex As Exception

            End Try

            Return False
        End Function



        Private Shared Sub DropTempTable(sTableName As String, ByRef cmd As SqlCommand)
            Try
                cmd.CommandText = "DROP TABLE " & sTableName & "_Temp"

                logger.Info(cmd.CommandText)

                sqlcnx.Open()
                cmd.ExecuteNonQuery()
                sqlcnx.Close()
            Catch ex As Exception
                sqlcnx.Close()
            End Try
        End Sub

        Private Shared Sub CreateTable(sCreateTable As String, tableName As String, ByRef cmd As SqlCommand)
            logger.Info("Creating new SQL table - " & tableName)

            Try
                cmd.CommandText = sCreateTable

                logger.Info(cmd.CommandText)

                sqlcnx.Open()
                cmd.ExecuteNonQuery()
                sqlcnx.Close()

                logger.Info("Created SQL table with the following command:")
                logger.Info(sCreateTable)
            Catch ex As Exception
                logger.Error("Error creating SQL table - " & ex.ToString)
                sqlcnx.Close()
            End Try

        End Sub

        Private Shared Sub CreateTempTable(sCreateTable As String, sTableName As String, ByRef cmd As SqlCommand)
            Try

                cmd.CommandText = Replace(sCreateTable, sTableName, sTableName & "_Temp")

                logger.Info(cmd.CommandText)

                sqlcnx.Open()
                cmd.ExecuteNonQuery()
                sqlcnx.Close()
            Catch ex As Exception
                logger.Error(ex.ToString)
                sqlcnx.Close()
            End Try
        End Sub

        Shared Sub AlignTables(ByVal sCopyFromTableName As String, ByVal sCopyToTableName As String, ByVal sDatabase As String, ByVal cnx As SqlConnection)
            Dim dtSchema As DataTable = GetDataTable("SELECT column_name, data_type, character_maximum_length, COLUMN_DEFAULT FROM " & sDatabase & ".INFORMATION_SCHEMA.columns where TABLE_NAME='" & sCopyFromTableName & "'", cnx)
            Dim sDataType, sCharacterLength, sdefaultValue As String
            Dim cmd As New SqlCommand
            cmd.Connection = cnx

            For Each CurrentRow As DataRow In dtSchema.Rows
                Dim selectString As String = "SELECT * FROM INFORMATION_SCHEMA.columns where TABLE_NAME='" & sCopyToTableName & "' and COLUMN_NAME = '" & CurrentRow.Item("column_name") & "'"

                Try
                    Dim dtDataType As DataTable = GetDataTable(selectString, cnx, False)

                    If dtDataType.Rows.Count > 0 Then
                        sDataType = IIf(IsDBNull(dtDataType.Rows(0).Item("data_type")), "", dtDataType.Rows(0).Item("data_type"))
                        sCharacterLength = IIf(IsDBNull(dtDataType.Rows(0).Item("character_maximum_length")), "", dtDataType.Rows(0).Item("character_maximum_length"))
                        sdefaultValue = IIf(IsDBNull(dtDataType.Rows(0).Item("COLUMN_DEFAULT")), "-1", dtDataType.Rows(0).Item("COLUMN_DEFAULT"))
                    Else
                        sDataType = ""
                        sCharacterLength = ""
                        sdefaultValue = "-1"
                    End If
                Catch ex As Exception
                    logger.Info(selectString)
                    logger.Error(ex.ToString)
                End Try

                If sDataType <> "" Then
                    AlterExistingColumn(sCopyToTableName, sDatabase, sDataType, sCharacterLength, sdefaultValue, cmd, CurrentRow)
                Else
                    AddNewColumn(sCopyToTableName, sDatabase, cmd, CurrentRow)
                End If
            Next
        End Sub

        Private Shared Sub AddNewColumn(sCopyToTableName As String, sDatabase As String, ByRef cmd As SqlCommand, CurrentRow As DataRow)
            Dim sAlterString As String = "ALTER TABLE " & sDatabase & ".dbo." & sCopyToTableName & " ADD [" & CurrentRow.Item("column_name") & "] " & CurrentRow.Item("data_type")

            Try
                If CurrentRow.Item("character_maximum_length").ToString = "-1" And CurrentRow.Item("data_type").ToString = SQL_DATA_TYPE_VARCHAR Then
                    sAlterString &= "(MAX)"
                ElseIf (CurrentRow.Item("character_maximum_length").ToString <> "-1" And CurrentRow.Item("character_maximum_length").ToString <> "") Or CurrentRow.Item("data_type").ToString = SQL_DATA_TYPE_VARCHAR Then
                    sAlterString &= "(" & CurrentRow.Item("character_maximum_length") & ")"
                End If
            Catch ex As Exception
                logger.Error("first exception: " & ex.ToString) '
                sAlterString &= "(" & CurrentRow.Item("character_maximum_length") & ")"
            End Try

            If CurrentRow.Item("COLUMN_DEFAULT") <> "-1" And CurrentRow.Item("COLUMN_DEFAULT") <> "" Then
                sAlterString &= " DEFAULT " & CurrentRow.Item("COLUMN_DEFAULT")
            End If

            Try
                cmd.CommandText = sAlterString

                Try
                    cmd.Connection.Close()
                Catch ex As Exception

                End Try

                logger.Info(sAlterString)

                cmd.Connection.Open()
                cmd.ExecuteNonQuery()
                cmd.Connection.Close()
            Catch ex As Exception
                logger.Error("Error adding new column with following SQL: " & sAlterString)
                logger.Info("Error message: " & ex.ToString)
            End Try
        End Sub

        Private Shared Sub AlterExistingColumn(sCopyToTableName As String, sDatabase As String, sDataType As String, sCharacterLength As String, ByVal sDefaultValue As String, ByRef cmd As SqlCommand, CurrentRow As DataRow)
            Dim sAlterString As String = "ALTER TABLE " & sDatabase & ".dbo." & sCopyToTableName & " ALTER COLUMN [" & CurrentRow.Item("column_name") & "] " & CurrentRow.Item("data_type")

            If sDataType <> CurrentRow.Item("data_type").ToString() Or sCharacterLength <> CurrentRow.Item("character_maximum_length").ToString() Or sDefaultValue <> CurrentRow.Item("COLUMN_DEFAULT").ToString() Then
                Try
                    If CurrentRow.Item("character_maximum_length") <> "-1" Or CurrentRow.Item("data_type") = SQL_DATA_TYPE_VARCHAR Then
                        sAlterString &= "(" & If(CurrentRow.Item("Data_Type") = SQL_DATA_TYPE_VARCHAR And CurrentRow.Item("Character_maximum_length") = "-1", "MAX", CurrentRow.Item("character_maximum_length")) & ")"
                    End If
                Catch ex As Exception
                    sAlterString &= "(" & If(CurrentRow.Item("Data_Type") = SQL_DATA_TYPE_VARCHAR And CurrentRow.Item("Character_maximum_length") = "-1", "MAX", CurrentRow.Item("character_maximum_length")) & ")"
                End Try

                'This doesn't work, need better way to switch default if different.
                'If sDefaultValue <> "-1" Then
                '    sAlterString &= " DEFAULT " & CurrentRow.Item("COLUMN_DEFAULT")
                'End If

                logger.Info(sAlterString)

                Try
                    cmd.CommandText = sAlterString

                    Try
                        cmd.Connection.Close()
                    Catch ex As Exception

                    End Try

                    cmd.Connection.Open()
                    cmd.ExecuteNonQuery()
                    cmd.Connection.Close()
                Catch ex As Exception
                    cmd.Connection.Close()
                    logger.Error(ex.ToString)
                End Try

            End If
        End Sub

        Shared Sub AddSQLColumns(ByRef createTable As String, ByRef createInsertProcedure As String, ByRef createUpdateProcedure As String, Optional ByVal bSection As Boolean = False)
            Dim insertProcedureParameters, insertProcedureBody, updateProcedureParameters, updateProcedureBody, updateCertificationDate, sDeclarations As String
            Dim addToCreateTable As String = ""

            modelProperties = ""
            conceptualProperties = ""
            storageProperties = ""
            scalarProperties = ""

            updateProcedureParameters = If(pageNumber = -1, " @ID int", "@" & GetUsernameFieldReference())

            If CurrentProjectRequiresWhitworthLogin() Then
                If IsMultipageForm() Then
                    updateCertificationDate &= "SET @CertificationDate = getdate()" & vbCrLf & vbCrLf
                End If

                LoginColumnTypes.FindAll(Function(l) l.IncludeSelectStatement = True).ForEach(Sub(l)
                                                                                                  addToCreateTable &= "," & l.ColumnName & " " & l.SQLType & " "
                                                                                              End Sub)

                createTable &= addToCreateTable
            End If

            addToCreateTable = ""

            currentProject.ProjectControls.ToList().Where(Function(pc) (pageNumber = -1 Or pc.PageID = GetPageInfo(pageNumber, "ID")) And If(pc.SQLInsertItemTable, "") = "").ToList().ForEach(
                Sub(pc)
                    With pc
                        If Not ParentIsRepeaterControl(.ID) And .IncludeDatabase = "1" Then
                            Dim SQLType As String

                            SQLType = GetSQLDataTypeName(.SQLDataType, .ID, "AddSQLColumns")

                            addToCreateTable &= ", "
                            addToCreateTable &= "[" & .Name & "] "
                            addToCreateTable &= SQLType

                            Try
                                Dim sqlDataType = db.ControlSQLTypes.FirstOrDefault(Function(st) st.ID = .SQLDataType)

                                If sqlDataType Is Nothing Then
                                    Throw New Exception("Invalid SQL data Type encountered when selecting for entity framework properties")
                                End If

                                modelProperties &= "public " & If(.Required <> "1" And sqlDataType.ModelType <> "string", "Nullable<", "") & sqlDataType.ModelType & If(.Required <> "1" And sqlDataType.ModelType <> "string", ">", "") & " " & .Name & " { get; set; }" & vbCrLf
                                conceptualProperties &= "<Property Name=""" & .Name & """ Type=""" & sqlDataType.EntityType & """"
                                storageProperties &= "<Property Name=""" & .Name & """"
                                scalarProperties &= "<ScalarProperty Name=""" & .Name & """ ColumnName=""" & .Name & """ />" & vbCrLf
                            Catch ex As Exception

                            End Try

                            If SQLType = "Varchar" Then
                                conceptualProperties &= " FixedLength=""false"" Unicode=""false"""

                                storageProperties &= " Type=""varchar"""

                                If .SQLDataSize.ToString.ToLower <> "max" Then
                                    storageProperties &= " MaxLength=""" & .SQLDataSize & """"
                                    conceptualProperties &= " MaxLength=""" & .SQLDataSize.ToString().ToLower() & """"
                                End If
                            Else
                                storageProperties &= " Type=""" & SQLType.ToLower & """"
                            End If

                            storageProperties &= " />" & vbCrLf
                            conceptualProperties &= " />" & vbCrLf

                            If insertProcedureParameters <> "" Then
                                insertProcedureParameters &= "," & vbCrLf
                            End If

                            If insertProcedureBody = "" Then
                                insertProcedureBody = "("
                            Else
                                insertProcedureBody &= "," & vbCrLf
                            End If

                            insertProcedureParameters &= "@" & .Name & " " & GetSQLDataTypeName(.SQLDataType, .ID, "AddSQLColumns2")
                            insertProcedureBody &= "[@" & .Name & "]"

                            If .SQLDataType = 1 Then
                                insertProcedureParameters &= "(" & .SQLDataSize & ")"
                            End If

                            If .DisplayLocation = "3" Then
                                insertProcedureParameters &= " = " & .GetSQLDefaultValue()
                            End If
                            'End If

                            If ((.DisplayLocation = "1" Or .DisplayLocation = "3") And Not bSection) Or (bSection And .DisplayLocation = "1") Then
                                'Commented out this condition - put back in later.  Right now not sure how to handle the case when a control is 
                                'saved to the database but included on the frontend only.  Any point to allowing that?
                                'If bFrontend Or (Not bFrontend And (.DisplayLocation = "1" Or .DisplayLocation = "3")) Then
                                updateProcedureParameters &= "," & vbCrLf

                                If updateProcedureBody <> "" Then
                                    updateProcedureBody &= "," & vbCrLf
                                End If

                                updateProcedureParameters &= "@" & .Name & " " & GetSQLDataTypeName(.SQLDataType, .ID, "AddSQLColumns3")
                                updateProcedureBody &= "[" & .Name & "]" & " = @" & .Name

                                If .SQLDataType = 1 Then
                                    updateProcedureParameters &= "(" & .SQLDataSize & ")"
                                End If
                            End If

                            If .SQLDataType = 1 Then
                                addToCreateTable &= "(" & .SQLDataSize & ")"
                            End If

                            addToCreateTable &= IIf(If(.SQLDefaultValue, "") <> "", " DEFAULT " & .GetSQLDefaultValue(), "")
                        End If
                    End With
                End Sub)

            createTable &= addToCreateTable

            GetPasswordFieldReference()

            With GetCurrentProjectDT.Rows(0)
                If IsMultipageForm() Then
                    AddCertificationField(insertProcedureParameters, insertProcedureBody)
                End If

                If CurrentProjectRequiresLogin() Then
                    AddUsernameField(createTable, insertProcedureParameters, insertProcedureBody, updateProcedureBody, sDeclarations, bSection)
                ElseIf CurrentProjectRequiresNonWhitworthLogin() Then
                    createTable &= "," & GetPasswordFieldReference() & " "
                End If

                If IseCommerceProject() Then
                    AddInvoiceField(createTable, insertProcedureParameters, insertProcedureBody)
                End If

                If isWorkflow Then
                    createTable = AddWorkflowFields(createTable)
                End If

                If pageNumber <> -1 Then
                    If updateProcedureBody <> "" Then
                        updateProcedureBody &= "," & vbCrLf
                    End If

                    updateProcedureBody &= "Section" & pageNumber & "Complete" & " = 1"
                End If

                insertProcedureBody &= ")"
                insertProcedureBody = Replace(insertProcedureBody, "@", "") & vbCrLf & vbCrLf & "VALUES" & vbCrLf & vbCrLf & Replace(Replace(insertProcedureBody, "[", ""), "]", "") & vbCrLf

                createInsertProcedure &= insertProcedureParameters & vbCrLf & vbCrLf & "AS" & vbCrLf & vbCrLf & "BEGIN" & vbCrLf & vbCrLf & updateCertificationDate & sDeclarations & "INSERT " & .Item("SQLMainTableName") & vbCrLf & insertProcedureBody & vbCrLf & "select IDENT_CURRENT('" & .Item("SQLMainTableName") & "')" & vbCrLf & vbCrLf & "END"

                createUpdateProcedure &= updateProcedureParameters & vbCrLf & vbCrLf & "AS" & vbCrLf & vbCrLf & "BEGIN" & vbCrLf & vbCrLf & If(bSection And IsFirstSection(), sDeclarations, "") & "Update " & .Item("SQLMainTableName") & " SET " & vbCrLf & vbCrLf & updateProcedureBody & vbCrLf & vbCrLf & "WHERE " & If(pageNumber = -1, "ID = @ID", "(Username = @Username AND " & GetCertificationCondition() & ")") & vbCrLf & vbCrLf & "" & vbCrLf & vbCrLf & If(pageNumber = -1, "select @ID", "Select ID From " & .Item("SQLMainTableName") & " Where (Username = @Username AND " & GetCertificationCondition() & ")") & vbCrLf & vbCrLf & "END"
            End With
        End Sub

        Private Shared Function AddWorkflowFields(createTable As String) As String
            createTable &= ",WorkflowProcessStepID int "
            createTable &= ",WorkflowPreviousProcessStepID int "
            Return createTable
        End Function

        Private Shared Sub AddInvoiceField(ByRef createTable As String, ByRef insertProcedureParameters As String, ByRef insertProcedureBody As String)
            createTable &= ",Invoice int "

            If insertProcedureParameters <> "" Then
                insertProcedureParameters &= "," & vbCrLf
            End If

            If insertProcedureBody = "" Then
                insertProcedureBody = "("
            Else
                insertProcedureBody &= "," & vbCrLf
            End If

            insertProcedureParameters &= "@Invoice int"
            insertProcedureBody &= "@Invoice"
        End Sub

        Private Shared Sub AddCertificationField(ByRef insertProcedureParameters As String, ByRef insertProcedureBody As String)
            If insertProcedureParameters <> "" Then
                insertProcedureParameters &= "," & vbCrLf
            End If

            If insertProcedureBody = "" Then
                insertProcedureBody = "("
            Else
                insertProcedureBody &= "," & vbCrLf
            End If

            insertProcedureParameters &= "@CertificationDate datetime = null"
            insertProcedureBody &= "@CertificationDate"

            If insertProcedureParameters <> "" Then
                insertProcedureParameters &= "," & vbCrLf
            End If

            If insertProcedureBody = "" Then
                insertProcedureBody = "("
            Else
                insertProcedureBody &= "," & vbCrLf
            End If

            insertProcedureParameters &= "@Certification nvarchar(1) = '1'"
            insertProcedureBody &= "@Certification"
        End Sub

        Private Shared Sub AddUsernameField(ByRef createTable As String, ByRef insertProcedureParameters As String, ByRef insertProcedureBody As String, ByRef updateProcedureBody As String, ByRef declarations As String, ByVal bSection As Boolean)
            Dim includedLCT = LoginColumnTypes.Where(Function(lct) lct.IncludeSelectStatement).ToList()

            createTable &= "," & GetUsernameFieldReference() & " "

            If insertProcedureParameters <> "" Then
                insertProcedureParameters &= "," & vbCrLf
            End If

            If insertProcedureBody = "" Then
                insertProcedureBody = "("
            Else
                insertProcedureBody &= "," & vbCrLf
            End If

            insertProcedureParameters &= "@" & GetUsernameFieldReference()


            For Each l As LoginColumnType In includedLCT
                If l.IncludeSelectStatement Then
                    insertProcedureBody &= "@" & l.ColumnName & ","

                    declarations &= "DECLARE @" & l.ColumnName & " " & l.SQLType & vbCrLf

                End If
            Next

            insertProcedureBody &= "@Username"

            For Each l As LoginColumnType In includedLCT
                Dim columnReference As String = If(l.ColumnName = "Phone", "master.dbo.ufn_RemoveNonNumeric(" & l.ColumnName & ")", l.ColumnName)

                declarations &= "SET @" & l.ColumnName & " = (SELECT TOP 1 CASE WHEN " & columnReference & " IS NULL THEN 'Unknown - Username ' + @Username ELSE " & columnReference & " END  FROM " & DV_USERINFO_V & " WHERE Username = @Username)" & vbCrLf

                If IsFirstSection() Then
                    updateProcedureBody &= "," & vbCrLf & l.ColumnName & " = @" & l.ColumnName
                End If
            Next

            WriteLine("declarations - " & declarations)
        End Sub


        Shared Sub CreateSingletonRecord(ByVal sTableName As String)
            Dim dtCurrent As DataTable = GetDataTable("SELECT * FROM " & sTableName, sqlcnx)

            If dtCurrent.Rows.Count = 0 Then
                ExecuteNonQuery("INSERT INTO " & sTableName & " (DateSubmitted) VALUES (GetDate())", sqlcnx)
            End If
        End Sub

        Shared Function GetSingletonID(ByVal sTableName As String) As Integer
            Try
                Return ExecuteScalar("SELECT ID FROM " & sTableName, sqlcnx)
            Catch ex As Exception

                logger.Error(ex.ToString)

                Return 1
            End Try
        End Function
    End Class
End Namespace
