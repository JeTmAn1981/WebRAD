Imports Microsoft.VisualBasic
Imports WhitTools.File
Imports WhitTools.GlobalEnum
Imports System.IO
Imports Common.General.Variables
Imports WhitTools.Getter
Imports WhitTools.Utilities
Imports System.Web.UI.WebControls
Imports System.Collections.Generic

Namespace General
    Public Class Folders
        Public Shared defaultFolders As List(Of ListItem) = CreateDefaultFolders()

        Shared Sub LoadDefaultFolders(ByRef lsbFolders As ListBox, ByRef lblSelectedfolder As Label)
            lblSelectedfolder.Text = "None"
            lsbFolders.Items.Clear()

            For Each currentItem As ListItem In defaultFolders
                lsbFolders.Items.Add(currentItem)
            Next
        End Sub

        Shared Sub UpdateFolders(ByRef lsbFolder As ListBox, ByRef lblSelectedFolder As Label, ByRef pnlFolder As Panel, ByRef txtLink As TextBox, Optional ByVal sFolder As String = "")
            'ImpersonateAsUser()
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Declare local variables
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim dis() As DirectoryInfo

            If sFolder <> "" And sFolder <> S_NONE Then
                Try
                    dis = New DirectoryInfo(sFolder).GetDirectories
                Catch ex As Exception
                    Directory.CreateDirectory(sFolder)
                    dis = New DirectoryInfo(sFolder).GetDirectories
                End Try

                lblSelectedFolder.Text = sFolder

                Try
                    dis = New DirectoryInfo(sFolder).GetDirectories
                Catch ex As Exception
                    Try
                        Directory.CreateDirectory(sFolder)
                        dis = New DirectoryInfo(sFolder).GetDirectories
                    Catch ex2 As Exception

                    End Try

                End Try
            Else

                If lsbFolder.SelectedIndex > -1 Then
                    Try
                        If lsbFolder.SelectedItem.Value = "return" Then
                            Dim selectedFolder As String = lblSelectedFolder.Text

                            If (From item As ListItem In defaultFolders Where item.Value = selectedFolder).ToList().Count > 0 Then
                                LoadDefaultFolders(lsbFolder, lblSelectedFolder)
                                pnlFolder.Visible = lblSelectedFolder.Text <> S_NONE

                                Exit Sub
                            Else

                                dis = New DirectoryInfo(lblSelectedFolder.Text).Parent.GetDirectories

                                lblSelectedFolder.Text = New DirectoryInfo(lblSelectedFolder.Text).Parent.FullName
                            End If
                        Else
                            lblSelectedFolder.Text = lsbFolder.SelectedValue
                            dis = New DirectoryInfo(lsbFolder.SelectedValue).GetDirectories
                        End If
                    Catch ex As Exception
                        'Empty catch statement
                    End Try
                End If
            End If

            If lsbFolder.SelectedIndex > -1 Or (lblSelectedFolder.Text <> "" And lblSelectedFolder.Text <> S_NONE) Then

                lsbFolder.Items.Clear()
                lsbFolder.Items.Add(New ListItem("[return to parent]", "return"))

                Try
                    For nCounter As Integer = 0 To dis.GetUpperBound(0)
                        'Write(dis(nCounter).Name & "<br /><br />")
                        lsbFolder.Items.Add(New ListItem(dis(nCounter).Name, dis(nCounter).FullName))
                    Next
                Catch ex As Exception
                    'Empty catch statement
                End Try

                pnlFolder.Visible = lblSelectedFolder.Text <> S_NONE
            End If

            If lblSelectedFolder.Text <> "" And Not txtLink Is Nothing Then
                SetLinkBasedOnFolder(txtLink, lblSelectedFolder.Text)
            End If

            'UndoImpersonateAsUser()
        End Sub

        Shared Sub SetLinkBasedOnFolder(ByRef txtLink As TextBox, ByVal sSelectedfolder As String)
            txtLink.Text = Replace(Replace(Replace(sSelectedfolder, "\\web1\~whitworth", ""), "\\web2\~whitworth", ""), "\", "/") & "/"
            If Left(txtLink.Text, 1) = "/" Then
                txtLink.Text = Right(txtLink.Text, txtLink.Text.Length - 1)
            End If
        End Sub

        Shared Function RemoveNonAlphanumeric(ByVal sPageTitle As String) As String
            Return Regex.Replace(sPageTitle, "[^A-Za-z0-9]", "")
        End Function

        Shared Function SameServerPath(ByVal sFrontendPath As String, ByVal sBackendPath As String) As Boolean
            Return If(Left(sFrontendPath, 7) = Left(sBackendPath, 7), True, False)
        End Function

        Shared Function GetPathLink(ByVal sCurrent As String, ByVal sProjectType As String, Optional ByVal sCurrentType As String = "Link") As String
            If sProjectType = "Test" Then
                Dim sFolder As String = ""

                If Right(sCurrent, 1) = "/" Then
                    sCurrent = sCurrent.Substring(0, sCurrent.Length - 1)
                End If

                For nCounter As Integer = sCurrent.Length - 1 To 0 Step -1

                    If sCurrent.Substring(nCounter, 1) <> "/" And sCurrent.Substring(nCounter, 1) <> "\" Then
                        sFolder &= sCurrent.Substring(nCounter, 1)
                    Else
                        Exit For
                    End If
                Next

                sFolder = StrReverse(sFolder)

                If sCurrentType = "Path" Then
                    sFolder = "\\web" & If(InStr(sCurrent, "web1"), "1", "2") & "\~whitworth\~Test\Tom\" & sFolder
                Else
                    sFolder = "~Test/Tom/" & sFolder
                End If

                Return sFolder
            End If

            Return sCurrent
        End Function

        Shared Function GetTemplatePath(Optional ByVal useMachineName As Boolean = False) As String
            If GetCurrentPage().Server.MachineName.ToLower().Contains("web2") Then
                Return "\\web2\~Whitworth\Administration\InformationSystems\Forms\WebRAD\Templates\"
            End If

            Dim templatePath = GetCurrentPage().Server.MapPath("~").ToLower().Replace("\stable", "\Templates")

            ' If useMachineName Then
            templatePath = templatePath.Replace("c:\", "\\" & GetCurrentPage().Server.MachineName & "\c$\").Replace("f:\", "\\" & GetCurrentPage().Server.MachineName & "\f$\")
            ' End If

            Return templatePath & "\"
            ''Return S_WEBRAD_TEMPLATES_BASE_DIR
        End Function

        Private Shared Function CreateDefaultFolders() As List(Of ListItem)
            If defaultFolders Is Nothing Then
                defaultFolders = New List(Of ListItem)()
            End If

            If defaultFolders.Count = 0 Then
                Dim folderList As New List(Of ListItem)()

                folderList.Add(New ListItem("Web1", "\\web1\~whitworth"))
                folderList.Add(New ListItem("Web2", "\\web2\~whitworth"))
                folderList.Add(New ListItem("Web2Dev", "\\web2\~whitworthdev"))
                folderList.Add(New ListItem("Web3", "\\web3\LargePictures"))

                Return folderList
            End If

            Return defaultFolders
        End Function
    End Class
End Namespace

