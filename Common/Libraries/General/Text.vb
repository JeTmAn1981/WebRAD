Namespace General
    Public Class Text
        Public Shared Sub UpdateMessageType(ByRef messageType As RadioButtonList, ByRef richMessageBox As TextBox, ByRef plainMessageBox As TextBox)
            If messageType.SelectedValue = "Rich" Then
                If richMessageBox.Text = "" Then
                    richMessageBox.Text = plainMessageBox.Text
                End If

                richMessageBox.Visible = True
                plainMessageBox.Visible = False
            Else
                If plainMessageBox.Text = "" Then
                    plainMessageBox.Text = richMessageBox.Text
                End If

                richMessageBox.Visible = False
                plainMessageBox.Visible = True
            End If
        End Sub
    End Class

End Namespace
