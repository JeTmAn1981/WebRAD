Imports System.Data
Namespace  General
    Public Class Scripts
    Public Shared Function GetAddRequiredClassScript(ByRef data As datarow) As string
        with data
            Return "<script>$(""label[for='" & .Item("Prefix") & .Item("Name") & "']"").addClass(""required"");</script>" & vbCrLf
        End With
    End Function

    Public Shared Function GetDatepickerScript(ByRef controlName As string) As string
        Dim script As String

        script = "<script type=""text/javascript"">" & vbCrLf
        script &= "$(""id*=['" & controlName & "']"").datepicker();" & vbcrlf
        script &= "</script>" & vbcrlf

        return script
    End Function
End Class
End Namespace