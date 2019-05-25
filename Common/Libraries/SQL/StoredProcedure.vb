Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.ProjectOperations
Imports Common.General.Controls
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
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
'Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File



Imports WhitTools.Utilities
Namespace SQL

    Public Class StoredProcedure
        Shared Function GetMainStoredProcedureParameters() As String
            Dim nCounter As Integer
            Dim sGetMainStoredProcedureParameters As New StringBuilder()

            With projectDT.Rows(0)
                If isFrontend And CurrentProjectRequiresWhitworthLogin() Then
                    sGetMainStoredProcedureParameters.Append(".Parameters.AddWithValue(""@Username"",Common.GetCurrentUsername())" & vbCrLf)
                ElseIf isFrontend And pageNumber <> -1 And GetAncillaryProject("RequireLogin") = "0" Then
                    sGetMainStoredProcedureParameters.Append(".Parameters.AddWithValue(""@Username"",GetSessionVariable(SESSION_USER_ID))" & vbCrLf)
                ElseIf isInsert And CurrentProjectRequiresWhitworthLogin() Then
                    sGetMainStoredProcedureParameters.Append(".Parameters.AddWithValue(""@Username"",ExtractUsername(txtUser.Text))" & vbCrLf)
                End If
            End With

            For nCounter = 0 To controlsDT.Rows.Count - 1
                With controlsDT.Rows(nCounter)
                    If BelongsToPage(pageNumber, .Item("PageID")) Then
                        If ControlDisplayAllowed(.Item("DisplayLocation")) Then
                            If Not ParentIsRepeaterControl(.Item("ID")) Then
                                AddControlAsParameter(controlsDT.Rows(nCounter), sGetMainStoredProcedureParameters)
                            End If
                        End If
                    End If
                End With
            Next

            Return sGetMainStoredProcedureParameters.ToString()
        End Function

        Shared Function GetUpdateScheduleProcedure() As String
            Dim sCreateUpdateScheduleProcedure As String = "CREATE PROCEDURE usp_Update" & GetScheduleTableName() & vbCrLf
            Dim sScheduleTableName As String = GetScheduleTableName()

            sCreateUpdateScheduleProcedure &= "@OpenDate datetime = NULL," & vbCrLf
            sCreateUpdateScheduleProcedure &= "@OpenTime nvarchar(10) = ''," & vbCrLf
            sCreateUpdateScheduleProcedure &= "@CloseDate datetime = NULL," & vbCrLf
            sCreateUpdateScheduleProcedure &= "@CloseTime nvarchar(10) = ''," & vbCrLf
            sCreateUpdateScheduleProcedure &= "@Message nvarchar(MAX) = ''" & vbCrLf & vbCrLf
            sCreateUpdateScheduleProcedure &= "AS" & vbCrLf & vbCrLf
            sCreateUpdateScheduleProcedure &= "BEGIN" & vbCrLf
            'sCreateUpdateScheduleProcedure &= "DELETE FROM " & GetScheduleTableName() & vbCrLf & vbCrLf
            sCreateUpdateScheduleProcedure &= "IF (SELECT COUNT(*) FROM " & sScheduleTableName & ") > 0" & vbCrLf
            sCreateUpdateScheduleProcedure &= "UPDATE " & sScheduleTableName & " SET OpenDate = @OpenDate, OpenTime = @OpenTime, CloseDate = @CloseDate, CloseTime = @CloseTime, [Message] = @Message " & vbCrLf
            sCreateUpdateScheduleProcedure &= "ELSE" & vbCrLf
            sCreateUpdateScheduleProcedure &= "INSERT INTO " & sScheduleTableName & " (OpenDate,OpenTime,CloseDate,CloseTime,[Message]) VALUES (@OpenDate,@OpenTime,@CloseDate,@CloseTime,@Message)" & vbCrLf
            sCreateUpdateScheduleProcedure &= "END"

            Return sCreateUpdateScheduleProcedure
        End Function

        Shared Function CreateUpdateSectionProcedure() As String
            Dim sCreateUpdateSectionProcedure As String = "CREATE PROCEDURE usp_Update" & projectDT.Rows(0).Item("SQLInsertStoredProcedureName") & "Section" & pageNumber & " " & vbCrLf

            AddSQLColumns("", "", sCreateUpdateSectionProcedure, True)

            Return sCreateUpdateSectionProcedure
        End Function

        Shared Sub AddControlAsParameter(ByRef CurrentRow As DataRow, ByRef sGetMainStoredProcedureParameters As StringBuilder, Optional ByVal bInRepeater As Boolean = False)
            Dim sCurrentPrefix, sControlReference, sHiddenUploadedReference, sCmdReference As String

            With CurrentRow
                sControlReference = If(bInRepeater, "ctype(.Findcontrol(""" & .Item("Prefix") & .Item("Name") & """)," & GetDataTypeDescription(.Item("DataType")) & ")", .Item("Prefix") & .Item("Name"))
                sHiddenUploadedReference = If(bInRepeater, "ctype(.FindControl(""lblHiddenUploaded" & .Item("Name") & """),Label)", "lblHiddenUploaded" & .Item("Name"))
                sCurrentPrefix = .Item("Prefix")
                sCmdReference = If(bInRepeater, "cmd", "")

                If sControlReference = "chkVerification" Then
                    Dim fdjkl = ""
                End If
                If .Item("SQLInsertItemTable") = "" And .Item("IncludeDatabase") = "1" Then
                    If sCurrentPrefix = "rbl" Then
                        sGetMainStoredProcedureParameters.Append(vbCrLf & "If " & sControlReference & ".selectedindex > -1 Then" & vbCrLf)
                    ElseIf IsControlType(.Item("ControlType"), "Date") Then
                        sGetMainStoredProcedureParameters.Append(vbCrLf & "If " & sControlReference & ".Text <> """" Then" & vbCrLf)
                    End If

                    sGetMainStoredProcedureParameters.Append(sCmdReference & ".parameters.AddWithValue(""@" & .Item("Name") & """," & GetControlValueReference(CurrentRow, bInRepeater) & ")")

                    If sCurrentPrefix = "rbl" Or IsControlType(.Item("ControlType"), "Date") Then
                        sGetMainStoredProcedureParameters.Append(vbCrLf & "Else" & vbCrLf & sCmdReference & ".parameters.AddWithValue(""@" & .Item("Name") & """,DBNull.Value)" & vbCrLf & "End If" & vbCrLf)
                    End If

                    sGetMainStoredProcedureParameters.Append(vbCrLf)
                End If
            End With
        End Sub

        Shared Function GetMainStoredProcedure(ByRef sMainStoredProcedure As String, ByVal sMainStoredProcedureParameters As String, ByVal sSQLInsertStoredProcedureName As String, ByVal nPageNumber As Integer) As String

            'If MainStoredProcedureRequired(nPageNumber) Then
            sMainStoredProcedure = "Dim cmd As New SqlCommand" & vbCrLf
            sMainStoredProcedure &= "cmd.Connection = cnx" & vbCrLf
            sMainStoredProcedure &= "cmd.CommandType = CommandType.StoredProcedure" & vbCrLf
            sMainStoredProcedure &= "cmd.CommandText = ""usp_" & If(nPageNumber >= 1, "Update", "Insert") & sSQLInsertStoredProcedureName & If(nPageNumber <> -1, "Section" & nPageNumber, "") & vbCrLf & vbCrLf

            sMainStoredProcedure &= "With cmd" & vbCrLf
            sMainStoredProcedure &= sMainStoredProcedureParameters & vbCrLf
            sMainStoredProcedure &= "End With" & vbCrLf
            'End If

            Return ""
        End Function

        Shared Sub DropStoredProcedure(ByVal sStoredProcedureName As String)
            Dim cmd As New SqlCommand("DROP PROCEDURE " & sStoredProcedureName, sqlcnx)

            logger.Info("Dropping stored procedure " & sStoredProcedureName)

            Try
                sqlcnx.close()
            Catch ex As Exception
                logger.Error(ex.ToString)
            End Try

            Try
                sqlcnx.Open()
                cmd.ExecuteNonQuery()
                sqlcnx.Close()
            Catch ex As Exception
                '  logger.Error(ex.ToString)
            End Try
        End Sub

        Shared Sub DropMultipageStoredProcedures(ByVal sStoredProcedureName As String)
            For ncounter = 1 To GetPagecount()
                DropStoredProcedure("usp_Update" & sStoredProcedureName & "Section" & ncounter)
            Next

            DropStoredProcedure("usp_Update" & sStoredProcedureName & "Certification")
        End Sub



        Shared Function GetCertificationStoredProcedure()
            Dim sCertification As String

            sCertification &= "CREATE PROCEDURE usp_Update" & projectDT.Rows(0).Item("SQLInsertStoredProcedureName") & "Certification" & vbCrLf
            sCertification &= "@" & GetUsernameFieldReference() & vbCrLf
            sCertification &= " AS " & vbCrLf
            sCertification &= "BEGIN " & vbCrLf
            sCertification &= "Declare @ID int" & vbCrLf & vbCrLf
            sCertification &= "SET @ID = (SELECT TOP 1 ID FROM " & projectDT.Rows(0).Item("SQLMainTableName") & " WHERE Username = @Username AND (Certification IS NULL or Certification = '0' or Certification = 'N') ORDER BY ID DESC)" & vbCrLf & vbCrLf
            sCertification &= "Update " & projectDT.Rows(0).Item("SQLMainTableName") & " SET Certification = '1', CertificationDate = getdate() WHERE ID = @ID" & vbCrLf & vbCrLf
            sCertification &= "SELECT @ID" & vbCrLf
            sCertification &= "END" & vbCrLf

            Return sCertification
        End Function
    End Class
End Namespace
