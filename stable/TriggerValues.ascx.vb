Imports WhitTools
Imports WhitTools.Repeaters
Imports WhitTools.DataTables
Imports WhitTools.getter
Imports WhitTools.utilities
Imports WhitTools.Filler
Public Class TriggerValues
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        End Sub

    Protected Sub ddlNumberTriggerValues_SelectedIndexChanged(sender As Object, e As EventArgs)
          Dim dtSupplied as new DataTablesSupplied()
        Dim listItems = GetCurrentListItems(Page.FindControl("ucLSI"))
        dtSupplied.AddRow("ddlSelectTriggerValue", DataTablesSupplied.ActionTypes.LocalDataTable, listItems, "Text", "Value")

        UpdateRepeaterItems(rptTriggerValues, ddlNumberTriggerValues.SelectedValue, dtSupplied)

        'For Each currentItem As RepeaterItem In rptTriggerValues.Items
        '    FillListData(CType(currentItem.FindControl("ddlSelectTriggerValue"), DropDownList), listItems, "Text", "Value")
        'Next
    End Sub

    public shared Function GetCurrentListItems(byref listItemsControl As webradlistitems) as datatable
        dim dt as new DataTable
        dim temprow as DataRow

        dt.Columns.Add("Text")
        dt.Columns.Add("Value")
        Return dt

        For Each currentItem As RepeaterItem In CType(GetPageControlReference("rptListItems"), Repeater).Items
            temprow = dt.NewRow()
            temprow.Item("Text") = CType(currentItem.FindControl("txtText"), TextBox).Text
            temprow.Item("Value") = CType(currentItem.FindControl("txtValue"), TextBox).Text

            dt.Rows.Add(temprow)
        Next

        temprow = dt.NewRow
        temprow.Item("Text") = "Other"
            temprow.Item("Value") = "Other"
        dt.Rows.InsertAt(temprow, 0)

        Return dt
    End Function

    Protected Sub ddlSelectTriggerValue_SelectedIndexChanged(sender As Object, e As EventArgs)

    End Sub
End Class