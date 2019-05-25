Imports Microsoft.VisualBasic
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.controltypes
Imports System.Data
Imports Common.General
Imports Common.General.DataTypes
Imports  Common.General.repeaters
Imports Common.General.controls

Namespace Webpages.ControlContent
    Public Class Heading
        'Bug here, if responsive is selected and frontend is on web2, need to update web2 CSS files to 
        'have responsive class references.
        Shared Function GetHeadingStyle(Optional ByVal bStart As Boolean = True) As String
            Return If(bStart, "<span class=""InputHeader"">", "</span>")
        End Function

        Shared Sub WriteControlHeading(ByRef sContent As String, ByVal Currentrow As DataRow, ByVal bIncludeSpacing As Boolean)
            Dim sHeading As String, sRequired As String, nControlType As Integer, sName As String, sListSelections As String

            With Currentrow
                sHeading = .Item("Heading")
                sRequired = .Item("Required")
                nControlType = .Item("ControlType")
                sName = .Item("Name")
                sListSelections = .Item("ListSelections")
            End With

            If sHeading <> "" And Currentrow.Item("DisplayHeading") <> "0" Then
                AddControlLabel(Currentrow, sContent)

                If Currentrow.Item("Subheading") <> "" Then
                    sContent &= "<small>" & Currentrow.Item("Subheading") & "</small>" & vbCrLf
                End If

                If sListSelections = "1" And isSearch = False Then
                    sContent &= "<div style=""margin-bottom: 20px;"">" & vbCrLf
                    sContent &= "<asp:label id=""lbl" & sName & "SelectedItems"" runat=""server"" />" & vbCrLf
                    sContent &= "</div>" & vbCrLf
                End If
            End If
        End Sub


        Public Shared Sub AddControlLabel(currentControl As DataRow, ByRef sContent As String)
            With currentControl
                Dim controlName As String = General.Controls.GetControlName(.Item("ID"))
                Dim heading As String = If(GetControlDataType(currentControl.Item("controlType")) = N_CHECKBOX_DATATYPE, .Item("Text"), .Item("Heading"))

                sContent &= "<asp:Label AssociatedControlID=""" & controlName & """" & If(.Item("Required") = "1" And Not isPrintable And Not isSearch, " class=""required""", "") & " runat=""server"">" & If(IsInsideHorizontalRepeater(currentControl.Item("ID")), "", heading) & "</asp:Label>" & vbCrLf
            End With
        End Sub
    End Class
End Namespace

