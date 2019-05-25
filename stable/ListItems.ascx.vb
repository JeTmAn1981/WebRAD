Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.IO
Imports WhitTools.eCommerce
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.Filler
Imports WhitTools.DataTables
Imports WhitTools.Converter
Imports WhitTools.Validator
Imports WhitTools.Formatter
Imports WhitTools.Repeaters
Imports WhitTools.SQL
Imports WhitTools.Encryption
Imports Common.General.Main
Imports Common.General.Variables
Imports whittools.Utilities

Partial Public Class WebRADListItems
    Inherits System.Web.UI.UserControl

    Dim cnx As SqlConnection = CreateSQLConnection("WebRAD")

    Sub LoadSQLParameters(ByRef cmd As SqlCommand)
        if pnlMinimumValue.Visible
            cmd.Parameters.AddWithValue("@MinimumValue", txtMinimumValue.Text)
         end if

        If pnlMaximumValue.Visible 
            cmd.Parameters.AddWithValue("@MaximumValue", txtMaximumValue.Text)
        End If
            
        If pnlSelectionItems.Visible = True Then
            cmd.Parameters.AddWithValue("@SelectionItems", ddlSelectionItems.SelectedItem.Value)

            If ddlSelectionItems.SelectedItem.Value = "2" Then
                cmd.Parameters.AddWithValue("@DataSourceID", ucDataSource.SaveData())
            ElseIf ddlSelectionItems.SelectedItem.Value = "4" Then
                cmd.Parameters.AddWithValue("@DataMethod", ddlDataMethod.SelectedItem.Value)

                If ddlDataMethod.SelectedItem.Value = "4" Then
                    cmd.Parameters.AddWithValue("@OtherDataMethod", txtOtherDataMethod.Text)
                End If
            End If
        End If

        If pnlIncludePleaseSelect.Visible = True Then
            cmd.Parameters.AddWithValue("@IncludePleaseSelect", rblIncludePleaseSelect.SelectedItem.Value)
        End If
    End Sub

    Sub BindData(ByVal dt As DataTable)
        With dt.Rows(0)
            SetItemSelected(ddlSelectionItems, .Item("SelectionItems"))
            SetItemSelected(rblIncludePleaseSelect, .Item("IncludePleaseSelect"))
            SetItemSelected(ddlDataMethod, .Item("DataMethod"))
            txtOtherDataMethod.Text = .Item("OtherDataMethod")
            txtMinimumValue.Text = .Item("MinimumValue")
            txtMaximumValue.Text = .Item("MaximumValue")
            ucDataSource.BindData(.Item("DataSourceID"))

            If ddlSelectionItems.SelectedIndex > 0 Then
                pnlListItems.Visible = True
                pnlIncludePleaseSelect.Visible = True
            End If
        End With
    End Sub

    Sub CheckDataMethodOptions()
        If pnlDataMethod.Visible = True Then
                pnlOtherDataMethod.Visible = (ddlDataMethod.SelectedItem.Value = "4")
                pnlMinimumValue.Visible = MinimumValueUsed()
                rfvMinimumValue.Enabled = MinimumValueRequired()
                pnlMaximumValue.Visible = MaximumValueUsed()
                rfvMaximumValue.Enabled=MaximumValueRequired()
        end if
    End Sub

    Private Function MaximumValueUsed() As Boolean
        try
            Return db.DataMethodTypes.First(function(x) x.ID = ddldatamethod.selectedvalue).MaximumValueUsed
        Catch ex As Exception

        End Try
        
        return false
    End Function

    Private Function MinimumValueUsed() As Boolean
        try
            Return db.DataMethodTypes.First(function(x) x.ID = ddldatamethod.selectedvalue).MinimumValueUsed
        Catch ex As Exception

        End Try
        
        return false
    End Function

    Private Function MaximumValueRequired() As Boolean
        try
            Return db.DataMethodTypes.First(function(x) x.ID = ddldatamethod.selectedvalue).MaximumValueRequired
        Catch ex As Exception

        End Try
        
        return false
    End Function

    Private Function MinimumValueRequired() As Boolean
        try
            Return db.DataMethodTypes.First(function(x) x.ID = ddldatamethod.selectedvalue).MinimumValueRequired
        Catch ex As Exception

        End Try
        
        return false
    End Function

    Sub CheckSelectionItems()
        pnlListItems.Visible = False
        pnlDataMethod.Visible = False
        pnlOtherDataMethod.Visible = False

        CType(ucDataSource.FindControl("pnlDataSource"), Panel).Visible = False
        CType(ucDataSource.FindControl("pnlDataSourceType"), Panel).Visible = False
        CType(ucDataSource.FindControl("pnlDataSourceSpecific"), Panel).Visible = False
        CType(ucDataSource.FindControl("pnlDataTextField"), Panel).Visible = False
        CType(ucDataSource.FindControl("pnlDataValueField"), Panel).Visible = False
        pnlIncludePleaseSelect.Visible = False

        If pnlSelectionItems.Visible = True Then
            pnlIncludePleaseSelect.Visible = True

            If ddlSelectionItems.SelectedItem.Value = "1" Then
                pnlListItems.Visible = True
                pnlIncludePleaseSelect.Visible = True
            ElseIf ddlSelectionItems.SelectedItem.Value = "2" Then
                CType(ucDataSource.FindControl("pnlDataSourceType"), Panel).Visible = True
                CType(ucDataSource.FindControl("pnlDataSource"), Panel).Visible = IIf(CType(ucDataSource.FindControl("rblDataSourceType"), RadioButtonList).SelectedIndex = 1, True, False)
                CType(ucDataSource.FindControl("pnlDataSourceSpecific"), Panel).Visible = IIf(CType(ucDataSource.FindControl("rblDataSourceType"), RadioButtonList).SelectedIndex = 0, True, False)
                CType(ucDataSource.FindControl("pnlDataTextField"), Panel).Visible = True
                CType(ucDataSource.FindControl("pnlDataValueField"), Panel).Visible = True
            ElseIf ddlSelectionItems.SelectedItem.Value = "4" Then
                pnlDataMethod.Visible = True
                CheckDataMethodOptions()
            End If
        End If
    End Sub


    Protected Sub ddlSelectionItems_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSelectionItems.SelectedIndexChanged
        CheckSelectionItems()
    End Sub

    Protected Sub ddlListItems_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlListItems.SelectedIndexChanged
        UpdateRepeaterItems(rptListItems, ddlListItems.SelectedItem.Value)
    End Sub

    Sub LoadListItems(ByVal nType As Integer, ByVal nCurrentID As Integer)
        Dim dt As DataTable = GetDataTable(cnx, "Select * From " & DT_WEBRAD_PROJECTCONTROLLISTITEMS & " Where Type = " & nType & " AND ParentID = " & nCurrentID)

        SelectRepeaterData(rptListItems, dt, cnx)
        SetItemSelected(ddlListItems, dt.Rows.Count)
    End Sub

    Sub SaveListItems(ByVal nType As Integer, ByVal nCurrentID As Integer)
        Dim nCounter As Integer
        Dim cmd As New SqlCommand

        ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTCONTROLLISTITEMS & " Where Type = " & nType & " AND ParentID = " & nCurrentID, cnx)

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_InsertProjectControlListItem"

        cmd.Parameters.AddWithValue("@ParentID", nCurrentID)
        cmd.Parameters.AddWithValue("@Type", nType)
        cmd.Parameters.AddWithValue("@Text", "")
        cmd.Parameters.AddWithValue("@Value", "")

        If pnlListItems.Visible = True Then
            For nCounter = 0 To rptListItems.Items.Count - 1
                With rptListItems.Items(nCounter)
                    cmd.Parameters("@Text").Value = CType(.FindControl("txtText"), TextBox).Text
                    cmd.Parameters("@Value").Value = CType(.FindControl("txtValue"), TextBox).Text
                End With

                ExecuteNonQuery(cmd, cnx)
            Next
        End If
    End Sub

    Protected Sub ddlDataMethod_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDataMethod.SelectedIndexChanged
        CheckDataMethodOptions()
    End Sub

    Sub ClearControls()
        ddlSelectionItems.SelectedIndex = 0
        rblIncludePleaseSelect.SelectedIndex = 0
        ddlListItems.SelectedIndex = 0
        txtOtherDataMethod.Text = ""
        txtMinimumValue.Text = ""
        txtMaximumValue.Text = ""
        CType(ucDataSource.FindControl("txtDataSource"), TextBox).Text = ""
        CType(ucDataSource.FindControl("txtDataTextField"), TextBox).Text = ""
        CType(ucDataSource.FindControl("txtDataValueField"), TextBox).Text = ""
    End Sub

    Sub ShowPanels(ByVal dtRequirements As DataTable)
        For counter = 0 To Me.Controls.Count - 1
            If TypeOf Me.Controls(counter) Is Panel Then
                Me.Controls(counter).Visible = False

                For counter2 = 0 To dtRequirements.Rows.Count - 1
                    If Me.Controls(counter).ID = "pnl" & dtRequirements.Rows(counter2).Item("Name") Then
                        Me.Controls(counter).Visible = True

                        Try
                                CType(Me.Controls(counter).FindControl("rfv" & dtRequirements.Rows(counter2).Item("Name")), RequiredFieldValidator).Enabled = (dtRequirements.Rows(counter2).Item("Required") = "1")
                            Catch ex As Exception

                            End Try

                            Try
                                CType(Me.Controls(counter).FindControl("pnl" & dtRequirements.Rows(counter2).Item("Name") & "Required"), Panel).Visible = (dtRequirements.Rows(counter2).Item("Required") = "1")
                            Catch ex As Exception

                            End Try
                       End If
                Next
            End If
        Next
    End Sub

    Sub ShowDetails(ByVal dtDetails As DataTable, ByVal nControlType As Integer)
        With dtDetails.Rows(0)
            If .Item("datamethod") <> "" Then
                SetItemSelected(ddlDataMethod, .Item("DataMethod"))
            End If

            If .Item("OtherDataMethod") <> "" Then
                txtOtherDataMethod.Text = .Item("OtherDataMethod")
            End If

            If .Item("MinimumValue") <> "" Then
                txtMinimumValue.Text = .Item("MinimumValue")
            End If

            If .Item("MaximumValue") <> "" Then
                txtMaximumValue.Text = .Item("MaximumValue")
            End If

            If .Item("Datasource") <> "" Then
                CType(ucDataSource.FindControl("txtDataSource"), TextBox).Text = .Item("datasource")
            End If

            If .Item("Datatextfield") <> "" Then
                CType(ucDataSource.FindControl("txtDataTextField"), TextBox).Text = .Item("datatextfield")
            End If

            If .Item("Datavaluefield") <> "" Then
                CType(ucDataSource.FindControl("txtDataValueField"), TextBox).Text = .Item("datavaluefield")
            End If

            If .Item("SelectionItems") <> "" Then
                SetItemSelected(ddlSelectionItems, .Item("SelectionItems"))
            End If

            If .Item("IncludePleaseSelect") <> "" Then
                SetItemSelected(rblIncludePleaseSelect, .Item("IncludePleaseSelect"))
            End If


            FillListData(rptListItems, cnx, "Select * From " & DT_WEBRAD_CONTROLTYPEITEMS & " Where ControlID = '" & nControlType & "'", "", "")
            SetItemSelected(ddlListItems, rptListItems.Items.Count)

            If CheckSavedStatus(ddlListItems.SelectedItem.Value, lblListItemCount.Text) = 1 Or CheckSavedStatus(ddlListItems.SelectedItem.Value, lblListItemCount.Text) = 2 Then
                UpdateRepeaterItems(rptListItems, ddlListItems.SelectedItem.Value)
            End If

            CheckSelectionItems()
            lblListItemCount.Text = ddlListItems.SelectedItem.Value
        End With
    End Sub

    
End Class
