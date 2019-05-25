Imports System.Data
Imports System.Data.SqlClient
Imports WhitTools.Cookies
Imports WhitTools.DataTables
Imports WhitTools.Email
Imports WhitTools.Encryption
Imports WhitTools.GlobalEnum
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.SQL
Imports WhitTools.Utilities
Imports WhitTools.Formatter
Imports WhitTools
Imports Common
Partial Class changepassword
    Inherits System.Web.UI.Page


    Protected pageName As String = "Change Password"
    Public authenticator As Authenticator = GetAuthenticator()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ftLoader.LoadFormText(pageName, AddressOf AddStatusBreadcrumb)

            formPanel.Visible = True
            confirmPanel.Visible = False
        End If
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
        If Page.IsPostBack Then
            Page.Validate()

            If Page.IsValid Then
                If authenticator.AuthenticateUser(GetSessionVariable(Common.SESSION_USER_ID), txtoldPwd.Text) Then
                    authenticator.ChangePassword(GetSessionVariable(Common.SESSION_USER_ID), txtnewPwd.Text)

                    formPanel.Visible = False
                    confirmPanel.Visible = True
                Else
                    lblLoginMessage.Text = "<span style='color:red'>*Invalid username and password combination, please try again...<span>"
                End If
            End If
        End If
    End Sub
End Class