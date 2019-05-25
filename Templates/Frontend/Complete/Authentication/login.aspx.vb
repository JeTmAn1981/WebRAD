Imports System.Data
Imports System.Data.SqlClient
Imports WhitTools.Cookies
Imports WhitTools.DataTables
Imports WhitTools.Email
Imports WhitTools.Encryption
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.GlobalEnum
Imports WhitTools.SQL
Imports WhitTools.Utilities
Imports WhitTools.Formatter
Imports WhitTools
Imports Common
Partial Class login
    Inherits System.Web.UI.Page

    Protected pageName As String = "Login"
    Public authenticator As Authenticator = GetAuthenticator()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ftLoader.LoadFormText(pageName)

            txtEmail.Text = Trim(Request.QueryString("Email"))
            txtPassword.Attributes.Add("value", Trim(Request.QueryString("Password")))
            txtPassword.Text = Trim(Request.QueryString("Password"))

            If txtEmail.Text = "" Then
                txtEmail.Text = GetCookieValue(Common.SESSION_USER_ID)
            End If

            If Request.QueryString("Login") = "True" Then
                Login()
            End If
        End If
    End Sub

    Protected Sub imbLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        Login()
    End Sub

    Public Sub Login()
        Page.Validate()

        If Page.IsValid Then
            If authenticator.AuthenticateUser(txtEmail.Text, txtPassword.Text) Then
                SetSessionVariable(Common.SESSION_USER_ID, CleanSQL(txtEmail.Text))
                FormsAuthentication.SetAuthCookie(txtEmail.Text, False)

                Redirect("status.aspx")
                'ElseIf Request.QueryString(S_MAINTENANCE) = S_TRUE Then
                '    If AuthenticateUserAdmin(txtEmail.Text, txtPassword.Text) Then
                '        Alert("That password was accepted.")
                '    Else
                '        Alert("That password was not accepted.")
                '    End If
            Else
                lblLoginMessage.Text = "<span style='color:red'>*Invalid username or password, please try again...<span>"
            End If
        End If
    End Sub

    'Function AuthenticateUserAdmin(ByVal sLoginEmail As String, ByVal sLoginPassword As String) As Boolean

    '    Dim userRecords = authenticator.GetUserRecords(sLoginEmail, sLoginPassword)

    '    With userRecords
    '        If .unfinishedRecord IsNot Nothing Then
    '            Return True
    '        ElseIf .finishedRecord IsNot Nothing Then
    '            Return True
    '        ElseIf .archivedRecord IsNot Nothing Then
    '            Return True
    '        Else
    '            If GetDataTable("SELECT ID FROM " & MAIN_DATABASE_TABLE_NAME & " WHERE Username='" & CleanSQL(sLoginEmail) & "'").Rows.Count > 0 Then
    '                lblLoginMessage.Text = "<span style='color:black'>*Invalid username, please try again...<span>"
    '            Else
    '                lblLoginMessage.Text = "<span style='color:black'>*Invalid password, please try again...<span>"
    '            End If
    '        End If
    '    End With

    '    Return False
    'End Function

    Protected Sub btnNewUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnNewUser.Click, btnForgotPassword.Click
        Redirect("newuser.aspx")
    End Sub
End Class