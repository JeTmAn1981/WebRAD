Imports Microsoft.VisualBasic
Imports Common.General.Variables

Imports WhitTools.Utilities
Namespace General

    Public Class Links
        Shared Function GetProjectLink(ByVal sPath As String, ByVal sLink As String) As String
            Dim testPath = IIf(projectType = "Test", "~test/tom/", "")

            If Left(sPath, 6) = "\\web1" Then
                Return "https://www.whitworth.edu/" & testPath & sLink
            Else
                Return "http://web2/" & testPath & sLink
            End If
        End Function

        Shared Function FormatProjectLink(ByVal sLink As String) As String

            If Left(sLink, 1) = "/" Or Left(sLink, 1) = "\" Then
                sLink = Right(sLink, sLink.Length - 1)
            End If

            If Right(sLink, 1) <> "/" And Right(sLink, 1) <> "\" Then
                sLink = sLink & "/"
            End If

            Return sLink
        End Function

         Shared Function GetFormattedLink(ByVal sLink As String, ByVal sText As String) As String
            Return "<a href=""" & sLink & """>" & sText & "</a>"
        End Function

         Shared Function GetApplicationStatusLink() As String
            Return "<li><a href='status.aspx'>Status</a></li>"
        End Function

         Shared Sub ValidateLink(ByRef cvLink As CustomValidator, ByRef txtLinkText As TextBox, args As ServerValidateEventArgs)
            If txtLinkText.Text.Contains("http") Then
                cvLink.ErrorMessage = "Http/https prefixes may not be included in the link.  Please start your link as if it was beginning at the root of the site."
                args.IsValid = False
            ElseIf Left(txtLinkText.Text, 1) = "/" Or Left(txtLinkText.Text, 1) = "\" Then
                cvLink.ErrorMessage = "Links may not start with a / or \ character."
                args.IsValid = False
            End If
        End Sub
    End Class

    MustInherit public Class DirectoryLink   
    protected path, link, projectType as string
    protected  requireLogin, isExternallyAvailable, multipage as boolean
    protected Const EXTERNAL_SERVER_ADDRESS  as string = "www.whitworth.edu"
    protected Const INTERNAL_SERVER_ADDRESS  as string = "web2"

    Public Sub New(path As string, link As string, projectType As string, requireLogin As boolean, multipage as boolean)
        me.path = path
        Me.link = link
        me.projectType = projectType
        me.requireLogin = requireLogin
        me.multipage = multipage

        setExternallyAvailable()
    End Sub

    Private Sub SetExternallyAvailable()
        isexternallyavailable = Left(path, 6) = "\\web1"
    End Sub

    Public Function GenerateLink() As string
        dim generatedLink as string = GetHttpPrefix() & "://" & GetServerAddress() & "/" & General.Folders.GetPathLink(link,projectType) & GetInitialPage()

        return "<a href='" & generatedLink & "' target='_blank'>" & generatedLink & "</a>"   
    End Function
    public Function GetServerAddress() As string
        return if(isexternallyavailable, EXTERNAL_SERVER_ADDRESS,INTERNAL_SERVER_ADDRESS)
    End Function

    public MustOverride Function GetHttpPrefix() As string
    public MustOverride function GetInitialPage() As string
End Class

Public Class FrontendLink
    inherits DirectoryLink

    Public Sub New(path As string, link As string,projectType As string, requireLogin As boolean, multipage As boolean)
        MyBase.New(path,link,projectType, requireLogin, multipage)
    End Sub

    Public overrides Function GetHttpPrefix() as string
        return if(isexternallyavailable And requirelogin,"https","http")
    End Function

    public Overrides function  GetInitialPage() As string
        return If(projecttype = "Test","/","") & if(multipage,"status.aspx","index.aspx")
    End function
End Class

Public Class BackendLink
    inherits DirectoryLink

    Public Sub New(path As string, link As string,projectType As string, requireLogin As boolean, multipage As boolean)
        MyBase.New(path,link,projectType, requireLogin, multipage)
    End Sub

    Public overrides Function GetHttpPrefix() as string
        return if(isexternallyavailable,"https","http")
    End Function

    public Overrides function GetInitialPage() As string
        return If(projecttype = "Test","/","") & "index.aspx"
    End function
End Class

End Namespace
