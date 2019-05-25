Imports Microsoft.VisualBasic
Imports System.Data
Imports WhitTools.DataTables
Imports Common.General.Main
Imports Common.General.Variables
Imports WhitTools.Utilities
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace General

    Public Class DataSources
        Shared Function GetDataSourceSelectString(ByVal nDataSourceID As Integer, Optional ByRef sDataTextField As String = "", Optional ByRef sDataValueField As String = "", Optional ByRef sDataSourceType As String = "", Optional ByVal sIDSelect As String = "", Optional ByVal additionalWhere As String = "", Optional includeIDSelectInCustomWhere As Boolean = False) As String
            Dim dtDataSource As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTDATASOURCES & " WHERE ID = " & nDataSourceID)

            Dim where As String

            If dtDataSource.Rows.Count > 0 Then
                With dtDataSource.Rows(0)
                    sDataTextField = .Item("TextField")
                    sDataValueField = .Item("ValueField")
                    sDataSourceType = .Item("Type")

                    Dim sDataSource As String

                    If .Item("Type") = "1" Then
                        If .Item("Source") = "" Then
                            sDataSource = "SELECT " & .Item("Select")

                            If .Item("Table") <> "" Then
                                sDataSource &= " FROM " & .Item("Table")
                            End If

                            If .Item("Where") <> "" Then
                                where = " WHERE " & .Item("Where")

                                If includeIDSelectInCustomWhere And sIDSelect <> "" Then
                                    where &= " AND " & sIDSelect
                                End If
                            ElseIf sIDSelect <> "" Then
                                where = " WHERE " & sIDSelect
                            End If

                            If additionalWhere <> "" Then
                                where &= If(where <> "", " AND ", " WHERE ")
                                where &= additionalWhere
                            End If

                            sDataSource &= where

                            If .Item("GroupBy") <> "" Then
                                sDataSource &= " GROUP BY " & .Item("GroupBy")
                            End If

                            If .Item("OrderBy") <> "" Then
                                sDataSource &= " ORDER BY " & .Item("OrderBy")
                            End If
                        Else
                            If (Not isFrontend) And .Item("BackendSource") <> "" Then
                                sDataSource = .Item("BackendSource")
                            Else
                                sDataSource = .Item("Source")
                            End If

                        End If

                        Return sDataSource
                    Else
                        If (Not isFrontend) And .Item("BackendSource") <> "" Then
                            sDataSource = .Item("BackendSource")
                        Else
                            sDataSource = .Item("Source")
                        End If

                        Return sDataSource
                    End If
                End With
            End If

            Return ""
        End Function

        Shared Sub ShowDataSource(ByRef ucDataSource As UserControl)
            With ucDataSource
                CType(.FindControl("pnlDataSource"), Panel).Visible = IIf(CType(.FindControl("rblDataSourceType"), RadioButtonList).SelectedIndex = 1, True, False)
                CType(.FindControl("pnlDataSourceSpecific"), Panel).Visible = IIf(CType(.FindControl("rblDataSourceType"), RadioButtonList).SelectedIndex = 0, True, False)
            End With
        End Sub

        Shared Sub HideDataSource(ByRef ucDataSource As UserControl)
            With ucDataSource
                CType(.FindControl("pnlDataSource"), Panel).Visible = False
                CType(.FindControl("pnlDataSourceSpecific"), Panel).Visible = False
            End With
        End Sub


        Public Shared Function ProjectUsesCustomSelectStatement() As Boolean
            Return GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTDATASOURCES & " WHERE ID = " & GetCustomSelectDataSourceID()).Rows.Count > 0
        End Function

        Private Shared Function GetCustomSelectDataSourceID() As String
            Return If(currentProject.CustomSelectDataSourceID, 0)
        End Function

        Public Shared Function GetCustomSelectDataSource() As DataTable
            Return GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTDATASOURCES & " WHERE ID = " & GetCustomSelectDataSourceID())
        End Function

    End Class
End Namespace
