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

Partial Class TopMenuNew
    Inherits System.Web.UI.UserControl

    Protected Sub libControlDetails_Click(sender As Object, e As EventArgs) Handles libControlDetails.Click
        Redirect("controls.aspx?ID=" & GetProjectID() & "&PageID=" & GetFirstPage(GetProjectID()))

    End Sub

    Protected Sub libMainDetails_Click(sender As Object, e As EventArgs) Handles libMainDetails.Click
        Redirect("index.aspx?ID=" & GetProjectID())
    End Sub

    Protected Sub libFrontendDetails_Click(sender As Object, e As EventArgs) Handles libFrontendDetails.Click
        Redirect("frontend.aspx?ID=" & GetProjectID())

    End Sub

    Protected Sub libBackendDetails_Click(sender As Object, e As EventArgs) Handles libBackendDetails.Click
        Redirect("backend.aspx?ID=" & GetProjectID())

    End Sub

    Protected Sub libFinalize_Click(sender As Object, e As EventArgs) Handles libFinalize.Click
        If AllControlsComplete() AndAlso ProjectDetailsComplete(GetProjectID())  Then
            Redirect("finalize.aspx?ID=" & Request.QueryString("ID"))
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
    End Sub
End Class
