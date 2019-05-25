
Imports WhitTools

Namespace General
    Public Class DataSourceParameters
         Shared Function GetDataSource(ByVal nDataSourceID As Integer, Optional ByRef sDataTextField As String = "", Optional ByRef sDataValueField As String = "", Optional ByRef sDataSourceType As String = "", Optional ByVal sIDSelect As String = "") As String
             Dim dtDataSource As DataTable = DataTables.GetDataTable("SELECT * FROM " & General.Variables.DT_WEBRAD_PROJECTDATASOURCES & " WHERE ID = " & nDataSourceID)

             If dtDataSource.Rows.Count > 0 Then
                 With dtDataSource.Rows(0)
                     sDataTextField = .Item("TextField")
                     sDataValueField = .Item("ValueField")
                     sDataSourceType = .Item("Type")
                     ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                     'Declare local variables
                     ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                     Dim sDataSource As String

                     If .Item("Type") = "1" Then
                         If .Item("Source") = "" Then
                             sDataSource = "SELECT " & .Item("Select")

                             If .Item("Table") <> "" Then
                                 sDataSource &= " FROM " & .Item("Table")
                             End If

                             If .Item("Where") <> "" Then
                                 sDataSource &= " WHERE " & .Item("Where")
                             ElseIf sIDSelect <> "" Then
                                 sDataSource &= " WHERE MT.ID IN ("" & GetListOfSelectedValues(rptSubmissions) & "")"
                             End If
                            
                             If .Item("GroupBy") <> "" Then
                                 sDataSource &= " GROUP BY " & .Item("GroupBy")
                             End If

                             If .Item("OrderBy") <> "" Then
                                 sDataSource &= " ORDER BY " & .Item("OrderBy")
                             End If
                         Else
                             sDataSource = .Item("Source")
                         End If

                         Return sDataSource
                     Else
                         Return .Item("Source")
                     End If
                 End With
             End If

             Return ""
         End Function
    End Class
End NameSpace