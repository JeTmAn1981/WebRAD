Imports System.Data
Imports System.Data.SqlClient
Imports WhitTools.Cookies
Imports WhitTools.DataTables
Imports WhitTools.Email
Imports WhitTools.Encryption
Imports WhitTools.GlobalEnum
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.Utilities
Imports WhitTools.Formatter
Imports Common
Imports WhitTools
Partial Class newuser
    Inherits System.Web.UI.Page


    Protected pageName As String = "New User/Forgot Password"
    Public authenticator As Authenticator = GetAuthenticator()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ftLoader.LoadFormText(pageName)

            pnlUser.Visible = True
            pnlConfirm.Visible = False
        End If
    End Sub


    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
        Page.Validate()

        If Page.IsValid Then
            Dim newPassword, subject, body As String

            pnlUser.Visible = False
            pnlConfirm.Visible = True

            lblEmail.Text = txtEmail.Text
            newPassword = authenticator.NewApplication(txtEmail.Text)

            subject = "" & APPLICATION_NAME & " Form - New User Password"
            body = "<p>Thanks for using the " & APPLICATION_NAME & " Form. Your password is:</p>"
            body &= "<p><strong>" & newPassword & "</strong></p>"
            body &= "<p>Click <a href='" & GetPageURL().Replace("/newuser.aspx", "") & "/login.aspx?Email=" & txtEmail.Text & "&Password=" & newPassword & "'>here</a> to log in using your new password.</p>"
            body &= "<p>You can update your password the next time you log in by clicking the 'Change Password' button at the bottom of the Application Menu.</p>"

            SendEmail(txtEmail.Text, subject, body)
        End If
    End Sub

End Class