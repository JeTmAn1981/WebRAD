Imports WhitTools.Utilities
Imports WhitTools.Getter
Imports WhitTools.Filler
Imports WhitTools.DataTables
Imports WhitTools.Setter
Imports System.Text.RegularExpressions
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.ProjectOperations
Imports Common.General.Pages
Imports Common.General.Controls

Partial Class TopMenu
    Inherits System.Web.UI.UserControl

    Protected Sub libControlDetails_Click(sender As Object, e As EventArgs) Handles libControlDetails.Click
        If GetProjectID() <> "" Then
            Redirect("controls.aspx?ID=" & GetProjectID() & "&PageID=" & GetFirstPage(GetProjectID()))
        End If
    End Sub

    Protected Sub libMainDetails_Click(sender As Object, e As EventArgs) Handles libMainDetails.Click
         Redirect("index.aspx?ID=" & GetProjectID())
    End Sub

    Protected Sub libFrontendDetails_Click(sender As Object, e As EventArgs) Handles libFrontendDetails.Click
        If GetProjectID() <> "" Then
            Redirect("frontend.aspx?ID=" & GetProjectID())
        End If
    End Sub

    Protected Sub libBackendDetails_Click(sender As Object, e As EventArgs) Handles libBackendDetails.Click
        If GetProjectID() <> "" Then
            Redirect("backend.aspx?ID=" & GetProjectID())
        End If
    End Sub

    Protected Sub libFinalize_Click(sender As Object, e As EventArgs) Handles libFinalize.Click
        If AllControlsComplete() And ProjectDetailsComplete(GetProjectID()) And GetProjectID() <> "" Then
            Redirect("finalize.aspx?ID=" & Request.QueryString("ID"))
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            FillListData(ddlProjects, GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTS & " ORDER BY PageTitle ASC"), "PageTitle", "ID")
            SetItemSelected(ddlProjects, GetProjectID())
        End If
    End Sub

    Protected Sub ddlProjects_SelectedIndexChanged(sender As Object, e As EventArgs)
        'New Regex("/([A-Za-z0-9]+?)\.aspx").Match(Request.Url.ToString).Groups(1).ToString)
        Redirect(Request.Url.Segments(Request.Url.Segments.Length - 1) & "?ID=" & ddlProjects.SelectedValue)
    End Sub
End Class
