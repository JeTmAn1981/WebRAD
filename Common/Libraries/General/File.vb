Imports Microsoft.VisualBasic
Imports System.Data
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Links
Imports Common.General.pages
Imports Common.General.Controls
Imports Common.General.ControlTypes
Imports common.general.repeaters
Imports WhitTools.DataTables

Namespace General
    Public Class File
        Shared Public  Function GetUploadBindData(ByRef CurrentRow As DataRow, ByVal sIdentity As String, ByVal sReference As String) As String
            Dim sFileLink As String = GetFileLink(CurrentRow, sIdentity)
            Dim sCurrentData, sImgUploaded, sLblUploaded, sLblHiddenUploaded, sPnlUploaded As String

            With CurrentRow
                sImgUploaded = GetRepeaterControlReference("lblUploaded" & .Item("Name"), "Label", sReference)
                sLblUploaded = GetRepeaterControlReference("lblUploaded" & .Item("Name"), "Label", sReference)
                sLblHiddenUploaded = GetRepeaterControlReference("lblHiddenUploaded" & .Item("Name"), "Label", sReference)
                sPnlUploaded = GetRepeaterControlReference("pnlUploaded" & .Item("Name"), "Panel", sReference)

                If IsImageUploadControl(.Item("ControlType")) Then
                    sCurrentData &= sImgUploaded & ".Text = ""<a target='blank' href='" & sFileLink & " & ""'><img src='" & sFileLink & " & ""' class=""""FormsImage""""></a>""" & vbCrLf
                Else
                    sCurrentData &= sLblUploaded & ".Text = ""<a href='" & sFileLink & " & ""' target='_blank'>"" & .item(""" & .Item("Name") & """) & ""</a>""" & vbCrLf
                End If

                sCurrentData &= sLblHiddenUploaded & ".Text = .Item(""" & .Item("Name") & """)" & vbCrLf
                sCurrentData &= sPnlUploaded & ".Visible = CBool(.Item(""" & .Item("Name") & """) <> """")" & vbCrLf
            End With

            Return sCurrentData
        End Function

        Shared Public Function GetFileLink(ByRef CurrentRow As DataRow, ByVal sIdentity As String) As String
            Dim sFileLink As String

            With CurrentRow
                If .Item("UploadPath") <> "" And .Item("UploadPath") <> "None" Then
                    Dim sUploadPath As String = .Item("UploadPath")

                    If .Item("UploadPathNewFolder") <> "" Then
                        sUploadPath &= "\" & Replace(.Item("UploadPathNewFolder"), "/", "\")
                    End If

                    sUploadPath &= "\UploadedFiles\"" & " & sIdentity

                    sUploadPath = Replace(sUploadPath, "\\web1\~whitworth\", "")
                    sUploadPath = Replace(sUploadPath, "\\web2\~whitworth\", "")
                    sUploadPath = Replace(sUploadPath, "\", "/")

                    sFileLink = GetProjectLink(.Item("UploadPath"), sUploadPath) & " & ""/"" & .Item(""" & .Item("Name") & """)"
                ElseIf projectDT.Rows(0).Item("IncludeFrontend") = "1" Then
                    sFileLink = GetProjectLink(projectDT.Rows(0).Item("FrontendPath"), projectDT.Rows(0).Item("FrontendLink")) & "UploadedFiles/"" & " & sIdentity & " & ""/"" & .Item(""" & .Item("Name") & """)"
                Else
                    sFileLink = GetProjectLink(projectDT.Rows(0).Item("BackendPath"), projectDT.Rows(0).Item("BackendLink")) & "UploadedFiles/"" & " & sIdentity & " & ""/"" & .Item(""" & .Item("Name") & """)"
                End If
            End With

            Return sFileLink
        End Function

        public shared function GetFileUploadMethods() As String
            Dim fileUploadMethods As String

            For nCounter = 0 To controlsDT.Rows.Count - 1
                With controlsDT.Rows(nCounter)
                    If ControlDisplayAllowed(.Item("DisplayLocation")) and BelongsToPage(pageNumber, .Item("PageID")) And IsFileUploadControl(.Item("ControlType")) Then
                        fileUploadMethods &= "Protected Sub btnUpload" & .Item("Name") & "_OnClick(sender As Object, e As EventArgs)" & vbcrlf

                        If ParentIsRepeaterControl(.Item("ID"))
                            FileUploadMethods &= "With GetParentRepeaterItem(sender)" & vbcrlf
                        End If
                        
                                                fileUploadMethods &= GetSaveDocumentAndReturnFilename(controlsDT.Rows(nCounter),ParentIsRepeaterControl(.Item("ID"))) & vbcrlf

                        If ParentIsRepeaterControl(.Item("ID"))
                            FileUploadMethods &= "End With" & vbcrlf & vbcrlf
                        End If

                        fileUploadMethods &= "End Sub" & vbcrlf & vbcrlf
                    End If
                End With
            next


            Return fileUploadMethods
        End function

        public Shared Function GetSaveDocumentAndReturnFilename(CurrentRow As DataRow, ByVal inRepeater As boolean) As Object
            Dim controlReference as String
            Dim fileType as String = If(IsImageUploadControl(currentrow.Item("ControlType")),"Image","File")

            if inRepeater
                controlreference = "GetParentRepeaterItem(.FindControl(""" & CurrentRow.item("Prefix") & CurrentRow.Item("Name") & """))"
            else
                controlReference = CurrentRow.item("Prefix") & CurrentRow.Item("Name") & ".Parent"
            End If
            
            Return "SaveDocumentAndReturnFileName(New fileUploadParameters(" & controlreference & ", """ & CurrentRow.Item("Name") & """, GetLocalTempUploadPath(), fileUploadParameters.FileUploadType." & fileType & "))"
        End Function

    End Class
End Namespace
