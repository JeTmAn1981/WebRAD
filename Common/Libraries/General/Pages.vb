Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports WhitTools.DataTables
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.Converter
Imports Common.General.Main
Imports Common.General.ProjectOperations
Imports Common.General.Variables
Imports Common.General.ControlTypes
Imports Common.General.DataTypes
Imports Common.Webpages.Frontend.MultiPage
Imports WhitTools.Utilities
Imports System.Linq

Namespace General

    Public Class Pages

        Shared Function GetFirstPage() As Integer
            Return GetFirstPage(GetProjectID())
        End Function
        ''' <summary>
        ''' Checks to see if a page has already been created for this project.  If no pages exist, creates a page.  Then returns the first page's ID.
        ''' </summary>
        ''' <param name="nProjectID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Shared Function GetFirstPage(ByVal nProjectID As String) As Integer
            Dim projectID As Integer
            Dim nPageID As Integer

            If Integer.TryParse(nProjectID, projectID) AndAlso db.Projects.Any(Function(p) p.ID = projectID) Then
                Dim dtPages As DataTable

                dtPages = GetDataTable("Select * From " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & nProjectID & " Order By ID asc")

                If dtPages.Rows.Count = 0 Then

                    Dim cmd As New SqlCommand

                    cmd.Connection = cnx
                    cmd.CommandText = "Insert " & DT_WEBRAD_PROJECTPAGES & " (ProjectID) VALUES (" & nProjectID & ")"

                    ExecuteNonQuery(cmd, cnx)

                    cmd.CommandText = "Select ident_current('ProjectPages')"

                    cnx.Open()
                    nPageID = cmd.ExecuteScalar
                    cnx.Close()
                Else
                    nPageID = dtPages.Rows(0).Item("ID")
                End If

            End If

            Return nPageID

        End Function

        Shared Sub CheckLastPage(Optional ByVal nPageID As Integer = -1, Optional ByVal isNew As Boolean = False)
            If nPageID = -1 Then
                nPageID = GetQueryString("PageID")
            End If

            Dim dtPages As DataTable = GetDataTable("Select * From " & DT_WEBRAD_PROJECTPAGES & " Where ProjectID = " & GetProjectID() & " Order by ID desc", cnx)

            If dtPages.Rows(0).Item("ID") = nPageID Then
                Redirect("frontend" & If(isNew, "new", "") & ".aspx?ID=" & GetProjectID())
            Else
                For nCounter As Integer = 0 To dtPages.Rows.Count - 1
                    If dtPages.Rows(nCounter).Item("ID") = nPageID Then
                        Redirect("controls" & If(isNew, "new", "") & ".aspx?ID=" & GetProjectID() & "&PageID=" & dtPages.Rows(nCounter - 1).Item("ID"))
                    End If
                Next
            End If
        End Sub

        Shared Function IsLastPage() As Boolean
            Return pageNumber = -1 Or (Not DefaultCertificationPage() And pageNumber = GetPageCount())
        End Function



        Shared Function GetPreviousPage(Optional ByVal nPageID As Integer = -1) As Integer

            If nPageID = -1 Then
                nPageID = GetQueryString("PageID")
            End If

            Return GetDataTable("Select top 2 * from " & DT_WEBRAD_PROJECTPAGES & " Where ProjectID = " & GetProjectID() & " and ID <= " & nPageID & " Order by ID desc").Rows(1).Item("ID")
        End Function

        Shared Function GetPageCount() As Integer
            Return GetDataTable("Select * From " & DT_WEBRAD_PROJECTPAGES & " Where ProjectID = " & GetProjectID(), cnx).Rows.Count
        End Function

        Shared Function GetWordsPageCount() As String
            Return ConvertNumberToWords(GetPageCount()).ToLower().Trim()
        End Function

        Shared Function GetPageInfo(ByVal nPageNumber As Integer, ByVal sColumn As String) As String
            Dim dtPages As DataTable = GetDataTable("Select * from " & DT_WEBRAD_PROJECTPAGES & " Where ProjectID = " & GetProjectID() & " order by ID asc")

            Try
                Return If(nPageNumber = -1, dtPages.Rows(0).Item(sColumn), dtPages.Rows(nPageNumber - 1).Item(sColumn))
            Catch ex As Exception
                Return dtPages.Rows(0).Item(sColumn)
            End Try
        End Function

        Shared Function BelongsToPage(ByVal nPageNumber As Integer, ByVal nPageID As Integer) As Boolean
            If nPageNumber = -1 Then
                Return True
            ElseIf nPageID = GetPageInfo(nPageNumber, "ID") Then
                Return True
            End If

            Return False
        End Function
    End Class
End Namespace
