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
Imports Common.General.DataSources

Public Class WebRADDataSource
    Inherits System.Web.UI.UserControl

    Public showType As Boolean = False
    Public showSpecific As Boolean = False

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            pnlDataSourceType.Visible = showType
        End If
    End Sub

    Public Sub rblDataSourceType_SelectedIndexChanged(sender As Object, e As EventArgs)
        ShowDataSource(sender.parent.parent)
    End Sub

    Sub LoadSQLParameters(ByRef cmd As SqlCommand)
        If pnlDataSourceType.Visible Then
            cmd.Parameters.AddWithValue("@DataSourceType", rblDataSourceType.SelectedValue)
        End If

        If pnlDataSource.Visible Then
            cmd.Parameters.AddWithValue("@DataSource", Trim(txtDataSource.Text))
        End If

        If pnlDataSourceSpecific.Visible Then
            cmd.Parameters.AddWithValue("@DataSourceSelect", Trim(txtDataSourceSelect.Text))
            cmd.Parameters.AddWithValue("@DataSourceTable", Trim(txtDataSourceTable.Text))
            cmd.Parameters.AddWithValue("@DataSourceWhere", Trim(txtDataSourceWhere.Text))
            cmd.Parameters.AddWithValue("@DataSourceGroupBy", Trim(txtDataSourceGroupBy.Text))
            cmd.Parameters.AddWithValue("@DataSourceOrderBy", Trim(txtDataSourceOrderBy.Text))
        End If

        If pnlDataTextField.Visible Then
            cmd.Parameters.AddWithValue("@DataTextField", Trim(txtDataTextField.Text))
        End If

        If pnlDataValueField.Visible Then
            cmd.Parameters.AddWithValue("@DataValueField", Trim(txtDataValueField.Text))
        End If
    End Sub

    Function SaveData(Optional ByVal nParentType As Integer = N_REPORT_DATASOURCEPARENTTYPE) As Integer
        Dim cmd As New SqlCommand("usp_InsertProjectDataSource", CreateSQLConnection("WebRAD"))
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@ParentType", nParentType)

        If pnlDataSourceType.Visible Then
            cmd.Parameters.AddWithValue("@Type", rblDataSourceType.SelectedValue)
        End If

        If pnlDataSource.Visible Then
            cmd.Parameters.AddWithValue("@Source", Trim(txtDataSource.Text))
        End If

        If pnlBackendDAtasource.Visible Then
            cmd.Parameters.AddWithValue("@BackendSource", Trim(txtBackendDatasource.Text))
        End If

        If pnlDataSourceSpecific.Visible Then
            cmd.Parameters.AddWithValue("@Select", Trim(txtDataSourceSelect.Text))
            cmd.Parameters.AddWithValue("@Table", Trim(txtDataSourceTable.Text))
            cmd.Parameters.AddWithValue("@Where", Trim(txtDataSourceWhere.Text))
            cmd.Parameters.AddWithValue("@GroupBy", Trim(txtDataSourceGroupBy.Text))
            cmd.Parameters.AddWithValue("@OrderBy", Trim(txtDataSourceOrderBy.Text))
        End If

        If pnlDataTextField.Visible Then
            cmd.Parameters.AddWithValue("@TextField", Trim(txtDataTextField.Text))
        End If

        If pnlDataValueField.Visible Then
            cmd.Parameters.AddWithValue("@ValueField", Trim(txtDataValueField.Text))
        End If

        Return ExecuteScalar(cmd)
    End Function

    Sub BindData(ByVal nDataSourceID As String)
        If nDataSourceID <> "-1" Then
            Dim dataSource As Common.ProjectDataSource = db.ProjectDataSources.FirstOrDefault(Function(ds) ds.ID.ToString() = nDataSourceID)

            If dataSource IsNot Nothing Then
                With dataSource
                    SetItemSelected(rblDataSourceType, If(.Type, 0))
                    txtDataSource.Text = .Source
                    txtBackendDatasource.Text = .BackendSource
                    txtDataSourceSelect.Text = .Select
                    txtDataSourceTable.Text = .Table
                    txtDataSourceWhere.Text = .Where
                    txtDataSourceGroupBy.Text = .GroupBy
                    txtDataSourceOrderBy.Text = .OrderBy
                    txtDataTextField.Text = .Textfield
                    txtDataValueField.Text = .Valuefield

                    chkUseBackendDatasource.Checked = (.BackendSource <> "")
                    chkUseBackendDatasource_OnCheckedChanged(Nothing, Nothing)
                End With
            End If
        End If
    End Sub

    Protected Sub chkUseBackendDatasource_OnCheckedChanged(sender As Object, e As EventArgs)
    pnlBackendDAtasource.Visible=chkusebackenddatasource.checked
    End Sub


End Class
