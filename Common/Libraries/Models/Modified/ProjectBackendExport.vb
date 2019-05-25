Imports WhitTools.Getter
Imports Common.General.Variables
Imports Common.General.Ancillary
Imports WhitTools.Formatter
Imports Common.General.Folders

Partial Public Class ProjectBackendExport
    Public Function RequireSelectedValues() As Boolean
        Try
            If DataSourceType = "1" Or (DataSourceType = 2 And ProjectDataSource.Where.Trim() = "") Then
                Return True
            End If
        Catch ex As Exception

        End Try

        Return False
    End Function

    Public Function CreateExportSelectString(Optional ByVal type As String = "Export") As String
        Return New ExportSelectStringCreator(Me, type).CreateSelectString()
    End Function
End Class
