Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.IO
Imports WhitTools.eCommerce
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.File
Imports WhitTools.Filler
Imports WhitTools.Utilities
Imports WhitTools.DataTables
Imports WhitTools.Converter
Imports WhitTools.Validator
Imports WhitTools.Formatter
Imports WhitTools.GlobalEnum
Imports WhitTools.Repeaters
Imports WhitTools.SQL
Imports WhitTools.Encryption
Imports WhitTools.Workflow
Imports Common.General.Main
Imports Common.General.Text
Imports Common.General.Variables
Imports Common.General.ControlTypes
Imports Common.General.Folders
Imports Common.Webpages.Backend.Export
Imports Common.General.Pages
Imports Common.General.Links
Imports Common.General.Columns
Imports Common.General.ProjectOperations
Imports System.Web.Script.Services
Imports System.Web.Services
Imports System.Web.UI.WebControls
Imports System.Linq
Imports System.Web.UI

Public Class ajaxtest
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub rptSupervisors_rblSupervisorType_SelectedIndexChanged(sender As Object, e As EventArgs)
        With GetParentRepeaterItem(sender)
            Try
                CType(.FindControl("SupervisorNameContainer"), Panel).Visible = If(CType(.FindControl("rblSupervisorType"), RadioButtonList).SelectedValue = "SingleUser", True, False)
            Catch ex As Exception
                CType(.FindControl("SupervisorNameContainer"), Panel).Visible = False
            End Try

            Try
                CType(.FindControl("SupervisorEmailContainer"), Panel).Visible = If(CType(.FindControl("rblSupervisorType"), RadioButtonList).SelectedValue = "EmailAddress", True, False)
            Catch ex As Exception
                CType(.FindControl("SupervisorEmailContainer"), Panel).Visible = False
            End Try

        End With
    End Sub


    Protected Sub btnrptSupervisorsAddItem_Click(sender As Object, e As EventArgs)
        AddNewRepeaterItem(rptSupervisors, Nothing, 1, , cvSupervisors, "supervisor")
    End Sub

    Protected Sub librptSupervisors_RemoveItem_Click(sender As Object, e As EventArgs)
        Dim ParentRepeaterItem As RepeaterItem = GetParentRepeaterItem(sender)
        RemoveRepeaterItem(ParentRepeaterItem.Parent, Nothing, CType(GetParentRepeaterItem(sender), RepeaterItem).ItemIndex, , cvSupervisors, "supervisor")
    End Sub

    '<ScriptMethod, WebMethod>
    'Public Function GetSupervisorNameAutocompleteData(ByVal prefixText As String, ByVal count As Integer) As IEnumerable
    '    Dim blah As List(Of String) = New List(Of String)()
    '    blah.Add("Test")
    '    Return blah
    '    'Try
    '    Return (From currentRow In GetDataTable("SELECT * FROM adTelephone.dbo.UserInfo_V WHERE IDNumber in (select plID from adTelephone.dbo.PeopleListing where PLActive='1') AND [User] like '%" & prefixText & "%' AND [User] LIKE '%" & prefixText & "%' ORDER BY PLLName, plfName").Rows
    '            Select currentRow.item("User")).ToList()
    '    'Catch ex As Exception
    '    '    WriteTextFile(ex.ToString, "autocompleteerror.txt")
    '    'End Try

    'End Function

    Public Sub RunSupervisorsControlEvents(ByRef CurrentControl As Control)
        If CurrentControl.ID = "rblSupervisorType" Then
            rptSupervisors_rblSupervisorType_SelectedIndexChanged(CurrentControl, Nothing)
        End If

        If CurrentControl.Controls.Count > 0 Then
            For Each CurrentControl2 As Control In CurrentControl.Controls
                RunSupervisorsControlEvents(CurrentControl2)
            Next
        End If
    End Sub

    Protected Sub cvSupervisorName_ServerValidate(source As Object, args As ServerValidateEventArgs)
        With GetParentRepeaterItem(source)
            If CType(.FindControl("txtSupervisorName"), TextBox).Text = "" Then
                CType(source, CustomValidator).ErrorMessage = "Please enter the username for this supervisor."
                args.IsValid = False
            ElseIf GetUserInfo(ExtractUsername(CType(.FindControl("txtSupervisorName"), TextBox).Text)).Rows.Count = 0 Then
                CType(source, CustomValidator).ErrorMessage = "Sorry, that username was not found.  Please enter another username."
                args.IsValid = False
            End If
        End With
    End Sub

End Class