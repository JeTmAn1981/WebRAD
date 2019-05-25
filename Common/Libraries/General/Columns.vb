Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.Controls
Imports Common.General.Repeaters
Imports Common.General.ProjectOperations
Imports Common.General.ControlTypes
Imports Common.General.DataTypes
Imports Common.General.Folders
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum


Imports WhitTools.Utilities
Namespace General

    Public Class Columns
        Shared Sub SaveColumnsInfo(ByVal sType As String, ByRef lsbMainColumns As ListBox, ByVal optionSelected As Boolean, Optional ByRef rptTables As Repeater = Nothing, Optional ByVal nTypeID As Integer = 0)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Declare local variables
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim cmd As New SqlCommand("usp_InsertProjectColumn", cnx)

            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@ProjectID", GetProjectID())
            cmd.Parameters.AddWithValue("@TableControlID", "")
            cmd.Parameters.AddWithValue("@ColumnControlID", "")
            cmd.Parameters.AddWithValue("@Type", sType)
            cmd.Parameters.AddWithValue("@TypeID", nTypeID)

            If optionSelected Then
                cmd.Parameters("@TableControlID").Value = "0"

                SaveColumn(lsbMainColumns, cmd)

                If Not rptTables Is Nothing Then

                    For Each CurrentItem As RepeaterItem In rptTables.Items

                        cmd.Parameters("@TableControlID").Value = CType(CurrentItem.FindControl("lblID"), Label).Text

                        SaveColumn(CType(CurrentItem.FindControl("lsbColumns"), ListBox), cmd)
                    Next
                End If
            End If
        End Sub

        Shared Sub SaveColumn(ByRef lsbCurrent As ListBox, ByRef cmd As SqlCommand)
            If lsbCurrent.Items(0).Selected And lsbCurrent.Items(0).Text = "All" Then
                cmd.Parameters("@ColumnControlID").Value = "0"

                ExecuteNonQuery(cmd, "tryan", 3, False, cnx)
            Else

                For Each CurrentItem As ListItem In lsbCurrent.Items

                    If CurrentItem.Selected Then

                        cmd.Parameters("@ColumnControlID").Value = CurrentItem.Value

                        'try
                            ExecuteNonQuery(cmd, "tryan", 3, False, cnx)
                        'Catch ex As Exception
                        '    writeline(GetCmdValues(cmd))
                        'End Try
                        
                    End If
                Next
            End If
        End Sub

    End Class
End Namespace
