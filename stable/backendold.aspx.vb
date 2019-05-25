Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.IO
Imports Common
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
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.DataSources
Imports Common.General.Folders
Imports Common.Webpages.Backend.Export
Imports Common.General.Controls
Imports Common.General.Links
Imports Common.General.Columns
Imports Common.General.ProjectOperations
Imports Common.General.Pages
Imports Common.General.ControlTypes

Partial Public Class Backend
    Inherits System.Web.UI.Page

     Shared _cnx As SqlConnection = CreateSQLConnection("WebRAD")

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then
            LoadDdLs()
            BindData()
        End If
    End Sub

    Sub FixDataSource(ByVal sTable As String, ByVal nParentType As Integer)
        Dim dtData As DataTable = GetDataTable("select * from " & sTable & " where not DataSourceType is null and datasourceid is null", _cnx)
        Dim cmd As New SqlCommand("usp_InsertProjectDataSource", CreateSQLConnection("WebRAD"))
        cmd.CommandType = CommandType.StoredProcedure

        For Each currentrow As DataRow In dtData.Rows
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@ParentType", nParentType)

            With currentrow
                cmd.Parameters.AddWithValue("@Type", .Item("DataSourceType"))
                cmd.Parameters.AddWithValue("@Source", .Item("DataSource"))
                cmd.Parameters.AddWithValue("@Select", .Item("DataSourceSelect"))
                cmd.Parameters.AddWithValue("@Table", .Item("DataSourceTable"))
                cmd.Parameters.AddWithValue("@Where", .Item("DataSourceWhere"))
                cmd.Parameters.AddWithValue("@GroupBy", .Item("DataSourceGroupBy"))
                cmd.Parameters.AddWithValue("@OrderBy", .Item("DataSourceOrderBy"))
                cmd.Parameters.AddWithValue("@TextField", .Item("DataTextField"))
                cmd.Parameters.AddWithValue("@ValueField", .Item("DataValueField"))
            End With

            ExecuteNonQuery("UPDATE " & sTable & " SET DataSourceID  = " & ExecuteScalar(cmd) & " WHERE ID = " & currentrow.Item("ID"), _cnx)
        Next
    End Sub

    'Sub UserControlTest()
    '    SelectRepeaterData(rptTest, GetDataTable("SELECT 0 AS ID"))

    '    For Each currentitem As RepeaterItem In rptTest.Items
    '        With CType(currentitem.FindControl("ucDataSource"), WebRADDataSource)
    '            .BindData(11)
    '            CType(.FindControl("pnlDataSourceType"), Panel).Visible = True
    '            ShowDataSource(currentitem.FindControl("ucDataSource"))
    '        End With
    '    Next

    '    Dim dtCreate As DataTable = CreateRepeaterData(rptTest, rptTest.Items.Count, cnx, -1, -1)
    '    Dim dtusercontrol As DataTable = dtCreate.Rows(0).Item("DataSourceDT")

    '    'WriteDataTableValues(dtCreate)

    '    'WriteDataTableValues(dtusercontrol)

    '    UpdateRepeaterItems(rptTest, 2)

    '    For Each currentitem As RepeaterItem In rptTest.Items
    '        With CType(currentitem.FindControl("ucDataSource"), WebRADDataSource)
    '            '                .BindData(11)
    '            CType(.FindControl("pnlDataSourceType"), Panel).Visible = True
    '            ShowDataSource(currentitem.FindControl("ucDataSource"))
    '        End With
    '    Next



    '    dtCreate = CreateRepeaterData(rptTest, rptTest.Items.Count, cnx, -1, -1)

    '    dtusercontrol = dtCreate.Rows(0).Item("DataSourceDT")

    '    WriteDataTableValues(dtusercontrol)

    '    Exit Sub

    '    For Each currentitem As RepeaterItem In rptTest.Items
    '        With CType(currentitem.FindControl("ucDataSource"), WebRADDataSource)
    '            '    .BindData(11)
    '            CType(.FindControl("pnlDataSourceType"), Panel).Visible = True
    '            ShowDataSource(currentitem.FindControl("ucDataSource"))
    '        End With
    '    Next


    '    Exit Sub

    'End Sub

    Sub LoadDdLs()
        CreateLoginColumnTypes()
        LoadDefaultFolders(lsbBackendFolders, lblSelectedBackendFolder)
        FillListData(lsbDisplayColumns, GetExportColumns(-1, True), "Name", "ColumnControlID", False)
        FillListData(lsbSearchTermControls, GetExportColumns(-1, True), "Name", "ColumnControlID", False)
        FillListData(lsbSearchDisplayColumns, GetExportColumns(-1, True), "Name", "ColumnControlID", False)
        FillListData(cblBackendOptions, GetDataTable("Select * From " & DT_WEBRAD_BACKENDOPTIONTYPES & "  Where Display=1", _cnx), "Name", "ID", False)
        FillNumbers(ddlSortColumns, 1, 10)
        FillNumbers(ddlFilterOptions, 1, 10)
        FillNumbers(ddlActions, 1, 10)
        FillNumbers(ddlSearchColumns, 1, 5)
        FillNumbers(ddlNumberAncillaryMaintenance, 1, 10)
        FillNumbers(ddlNumberExports, 1, 10)
        FillNumbers(ddlNumberReports, 1, 10)
        'Dim dtSupplied As New WhitTools.DataTablesSupplied

        'Dim dtExportColumns As DataTable = GetExportColumns()

        'Dim AllRow As DataRow = dtExportColumns.NewRow
        'AllRow.Item("Name") = "All"
        'AllRow.Item("ColumnControlID") = "0"
        'dtExportColumns.Rows.InsertAt(AllRow, 0)

        'dtSupplied.AddRow("lsbMainColumns", "FillListData", GetExportColumns(), "Name", "ColumnControlID")

        'UpdateRepeaterItems(rptExports, 2)

        'For Each CurrentItem As RepeaterItem In rptExports.Items
        '    GetAdditionalExportColumns(CurrentItem.FindControl("rptTables"))
        'Next

        'FillListData(lsbMainColumns, GetExportColumns(), "Name", "ColumnControlID", False)
        'lsbMainColumns.Items.Insert(0, New ListItem("All", "0"))
        'lblMainTableName.Text = GetDataTable(cnx, "Select * From " & DT_WEBRAD_PROJECTS & "  Where ID = " & Request.QueryString("ID")).Rows(0).Item("SQLMainTableName")


    End Sub

    Sub BindData()
        If Request.QueryString("ID") <> "" Then
            Dim dt As New DataTable

            dt = GetDataTable(_cnx, "Select * From " & DT_WEBRAD_PROJECTS & "  Where ID = " & Request.QueryString("ID"))

            If dt.Rows.Count > 0 Then
                With dt.Rows(0)
                    SetItemSelected(rblIncludebackend, .Item("Includebackend"))
                    lblFormName.Text = .Item("PageTitle")
                    Showbackend()

                    If pnlBackend.Visible = True Then
                        SetListControlItemSelected(cblBackendOptions, "", True, GetDataTable("Select * From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  Where ProjectID = " & Request.QueryString("ID"), _cnx), "Type")

                        SetListControlItemSelected(lsbDisplayColumns, "", True, GetDataTable("Select * From " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  OC left outer join " & DT_WEBRAD_PROJECTCONTROLS & " PC on PC.ID = OC.ControlID Where OptionID in (Select ID From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  WHERE Type = 8 AND ProjectID = " & GetProjectID() & ")", _cnx), "ControlID")
                        lblSelectedDisplayColumns.Text = Replace(GetListOfSelectedValues(lsbDisplayColumns), "'", "")

                        If lblSelectedDisplayColumns.Text <> "" Then
                            lblSelectedDisplayColumns.Text &= "<br /><Br />"
                        End If

                        ShowBackendOptions()

                        SetItemSelected(rblDefaultSort, .Item("DefaultSort"))
                        LoadSortOptions()
                        LoadFilterOptions()
                        LoadActionOptions()
                        LoadSearchOptions()
                        LoadAncillaryMaintenance()

                        LoadExports("Export")
                        LoadExports("Report")

                        If pnlSchedulePage.Visible Then
                            txtClosedMessage.Text = .Item("ClosedMessage")

                            If txtClosedMessage.Text = "" Then
                                txtClosedMessage.Text = "Sorry, the <strong>" & .Item("PageTitle") & "</strong> form is now closed."
                            End If
                        End If

                        UpdateFolders(lsbBackendFolders, lblSelectedBackendFolder, pnlBackendFolder, txtBackendLink, .Item("backendPath"))
                        txtPageLimit.Text = .Item("PageLimit")
                        txtBackendLink.Text = .Item("BackendLink")
                        txtNewBackendFolderName.Text = .Item("backendNewFolder")
                        txtCustomSelectStatement.Text = .Item("CustomSelectStatement")

                        rblCreateBackendFolder.SelectedIndex = If(.Item("backendNewFolder") <> "", 1, 0)
                        pnlNewBackendFolder.Visible = .Item("backendNewFolder") <> ""

                        Dim dtProjectBackendAdditionalLinks As DataTable = GetDataTable("Select * From " & DT_WEBRAD_PROJECTBACKENDADDITIONALLINKS & " Where ProjectID = '" & Request.QueryString("ID") & "'", _cnx)

                        SelectRepeaterData(rptAdditionalLinks, GetDataTable("Select * From " & DT_WEBRAD_PROJECTBACKENDADDITIONALLINKS & " Where ProjectID = '" & Request.QueryString("ID") & "'", _cnx), _cnx)
                    End If
                End With
            End If
        End If
    End Sub

    Sub LoadExports(ByVal sType As String)
        If CType(Page.FindControl("pnl" & sType), Panel).Visible Then
            Dim dtExports As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTBACKEND_EXPORTS & " WHERE Type = '" & sType & "' AND ProjectID = " & GetProjectID())
            Dim dtSupplied As New WhitTools.DataTablesSupplied
            Dim dtExportColumns As DataTable = GetExportColumns()
            Dim rptCurrent As Repeater = CType(Page.FindControl("rpt" & sType & "s"), Repeater)

            Dim allRow As DataRow = dtExportColumns.NewRow
            allRow.Item("Name") = "All"
            allRow.Item("ColumnControlID") = "0"
            dtExportColumns.Rows.InsertAt(allRow, 0)

            dtSupplied.AddRow("lsbMainColumns", "FillListData", dtExportColumns, "Name", "ColumnControlID")

            SelectRepeaterData(rptCurrent, dtExports, dtSupplied)
            'WriteLine("SELECT * FROM " & DT_WEBRAD_PROJECTBACKEND_EXPORTS & " WHERE Type = '" & sType & "' AND ProjectID = " & GetProjectID())
            For nCounter As Integer = 0 To rptCurrent.Items.Count - 1
                GetAdditionalExportColumns(rptCurrent.Items(nCounter).FindControl("rptTables"))

                rblDataSourceType_SelectedIndexChanged(rptCurrent.Items(nCounter).FindControl("rblDataSourceType"), Nothing)
                SetListControlItemSelected(CType(rptCurrent.Items(nCounter).FindControl("lsbMainColumns"), ListBox), "", True, GetDataTable("Select * From " & DT_WEBRAD_PROJECTCOLUMNS & "  Where Type='" & sType & "' AND ProjectID = " & Request.QueryString("ID") & " and TableControlID = 0 AND TypeID = " & dtExports.Rows(nCounter).Item("ID"), _cnx), "ColumnControlID")

                'If sType = "Report" Then
                CType(rptCurrent.Items(nCounter).FindControl("ucDataSource"), WebRADDataSource).BindData(dtExports.Rows(nCounter).Item("DataSourceID"))
                'End If

                For Each currentItem As RepeaterItem In CType(rptCurrent.Items(nCounter).FindControl("rptTables"), Repeater).Items
                    Try
                        'WriteLine("Select * From " & DT_WEBRAD_PROJECTCOLUMNS & "  Where Type='" & sType & "' AND ProjectID = " & Request.QueryString("ID") & " and TableControlID = " & CType(CurrentItem.FindControl("lblID"), Label).Text & " AND TypeID = " & dtExports.Rows(nCounter).Item("ID"))
                    Catch ex As Exception
                        'WriteLine(nCounter)
                    End Try
                    'WriteLine("Select * From " & DT_WEBRAD_PROJECTCOLUMNS & "  Where Type='" & sType & "' AND ProjectID = " & Request.QueryString("ID") & " and TableControlID = " & CType(CurrentItem.FindControl("lblID"), Label).Text & " AND TypeID = " & dtExports.Rows(nCounter).Item("ID"))
                    SetListControlItemSelected(CType(currentItem.FindControl("lsbColumns"), ListBox), "", True, GetDataTable("Select * From " & DT_WEBRAD_PROJECTCOLUMNS & "  Where Type='" & sType & "' AND ProjectID = " & Request.QueryString("ID") & " and TableControlID = " & CType(currentItem.FindControl("lblID"), Label).Text & " AND TypeID = " & dtExports.Rows(nCounter).Item("ID"), _cnx), "ColumnControlID")
                Next
            Next

            SetItemSelected(CType(Page.FindControl("ddlNumber" & sType & "s"), DropDownList), dtExports.Rows.Count)
        End If
    End Sub

    Protected Sub cvMainColumns_ServerValidate(source As Object, args As ServerValidateEventArgs)
        args.IsValid = ValidateListControl(CType(CType(source.parent, Panel).FindControl("lsbMainColumns"), ListBox), source)
    End Sub

    Protected Sub rblDataSourceType_SelectedIndexChanged(sender As Object, e As EventArgs)
        With CType(sender.parent, RepeaterItem)
            'If Left(sender.parent.clientid, 10) = "rptReports" Then
            CType(CType(.FindControl("ucDataSource"), UserControl).FindControl("pnlDataSourceSpecific"), Panel).Visible = If(CType(.FindControl("rblDataSourceType"), RadioButtonList).SelectedValue = "2", True, False)
            CType(.FindControl("pnlColumns"), Panel).Visible = If(CType(.FindControl("rblDataSourceType"), RadioButtonList).SelectedValue = "2", False, True)
            'Else
            'If CType(.FindControl("rblDataSourceType"), RadioButtonList).SelectedIndex = 0 Then
            '    CType(.FindControl("pnlColumns"), Panel).Visible = True
            '    CType(.FindControl("pnlStatement"), Panel).Visible = False
            'ElseIf CType(.FindControl("rblDataSourceType"), RadioButtonList).SelectedIndex = 1 Then
            '    CType(.FindControl("pnlStatement"), Panel).Visible = True
            '    CType(.FindControl("pnlColumns"), Panel).Visible = False
            'End If
            'End If
        End With
    End Sub

    Protected Sub cblBackendOptions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cblBackendOptions.SelectedIndexChanged
        ShowBackendOptions()
    End Sub

    Sub ShowBackendOptions()
        For nCounter As Integer = 0 To cblBackendOptions.Items.Count - 1
            Try
                CType(Page.FindControl("pnl" & Replace(cblBackendOptions.Items(nCounter).Text, " ", "")), Panel).Visible = cblBackendOptions.Items(nCounter).Selected
            Catch ex As Exception

            End Try
        Next
    End Sub


    Sub LoadSortOptions()
        Dim dtOptions As DataTable = GetDataTable("Select * from " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  Where OptionID = (Select ID From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  Where Type = 1 and ProjectID = " & Request.QueryString("ID") & ")", _cnx)

        SetItemSelected(ddlSortColumns, dtOptions.Rows.Count)

        Dim dtSupplied As New WhitTools.DataTablesSupplied

        AddAvailableControls(dtSupplied)

        SelectRepeaterData(rptSortColumns, dtOptions, dtSupplied)
    End Sub

    Sub LoadActionOptions()
        Dim dtOptions As DataTable = GetDataTable("Select OptionID, ControlID, Label, OperatorType as ActionType, ComparisonValue as UpdateValue from " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  Where OptionID = (Select ID From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  Where Type = 5 and ProjectID = " & Request.QueryString("ID") & ")", _cnx)

        SetItemSelected(ddlActions, dtOptions.Rows.Count)

        Dim dtSupplied As New WhitTools.DataTablesSupplied

        AddAvailableControls(dtSupplied)
        dtSupplied.AddRow("ddlActionType", "SQLSelect", "Select ID, Description FROM " & DT_WEBRAD_BACKENDACTIONTYPES & " ", "Description", "ID")

        SelectRepeaterData(rptActions, dtOptions, dtSupplied)

        For Each currentItem As RepeaterItem In rptActions.Items
            ddlActionType_SelectedIndexChanged(currentItem.FindControl("ddlActionType"), Nothing)
        Next
    End Sub

    Sub LoadFilterOptions()
        Dim dtSupplied As New WhitTools.DataTablesSupplied

        Dim dtOptions As DataTable = GetDataTable("Select * from " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  Where Type = 2 and ProjectID = " & Request.QueryString("ID"), _cnx)

        SetItemSelected(ddlFilterOptions, dtOptions.Rows.Count)

        dtSupplied.AddRow("ddlOptionColumns", "FillNumbers", "", "1", "10")
        dtSupplied.AddRow("rblOperatorType", "SQLSelect", "Select * FROM " & DT_WEBRAD_BACKENDOPTIONOPERATORTYPES, "Description", "ID")

        SelectRepeaterData(rptFilterOptions, dtOptions, dtSupplied)

        For nCounter As Integer = 0 To rptFilterOptions.Items.Count - 1
            Dim dtColumns As DataTable = GetDataTable("Select * From " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  Where OptionID = " & dtOptions.Rows(nCounter).Item("ID"), _cnx)

            SetItemSelected(CType(rptFilterOptions.Items(nCounter).FindControl("ddlOptionColumns"), DropDownList), dtColumns.Rows.Count)

            dtSupplied = New WhitTools.DataTablesSupplied
            AddAvailableControls(dtSupplied)
            dtSupplied.AddRow("rblOperatorType", "SQLSelect", "Select * FROM " & DT_WEBRAD_BACKENDOPTIONOPERATORTYPES, "Description", "ID")

            SelectRepeaterData(CType(rptFilterOptions.Items(nCounter).FindControl("rptOptionColumns"), Repeater), dtColumns, dtSupplied)
        Next
    End Sub

    Sub LoadAncillaryMaintenance()
        Dim dtSupplied As New WhitTools.DataTablesSupplied

        Dim dtOptions As DataTable = GetDataTable("Select AncillaryProjectID as Project, ShortName, Singleton from " & DT_WEBRAD_PROJECTANCILLARYMAINTENANCE & "  Where ProjectID = " & GetProjectID(), _cnx)

        SetItemSelected(ddlNumberAncillaryMaintenance, dtOptions.Rows.Count)

        dtSupplied.AddRow("ddlProject", "SQLSelect", "Select * FROM web3.WebRAD.dbo.Projects WHERE NOT ID = " & GetProjectID() & " ORDER By PageTitle", "PageTitle", "ID")

        SelectRepeaterData(rptAncillaryMaintenance, dtOptions, dtSupplied)
    End Sub

    Sub LoadSearchOptions()
        SetListControlItemSelected(lsbSearchTermControls, "ControlID", True, GetDataTable("Select * From " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  OC left outer join " & DT_WEBRAD_PROJECTCONTROLS & " PC on PC.ID = OC.ControlID Where OptionID in (Select Min(ID) From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  WHERE Type = 10 AND ProjectID = " & GetProjectID() & ")", _cnx), "ControlID")
        lblSelectedSearchTermControls.Text = Replace(GetListOfSelectedValues(lsbSearchTermControls), "'", "")

        SetListControlItemSelected(lsbSearchDisplayColumns, "ControlID", True, GetDataTable("Select * From " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  OC left outer join " & DT_WEBRAD_PROJECTCONTROLS & " PC on PC.ID = OC.ControlID Where OptionID in (Select Max(ID) From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  WHERE Type = 10 AND ProjectID = " & GetProjectID() & ")", _cnx), "ControlID")
        lblSelectedSearchDisplayColumns.Text = Replace(GetListOfSelectedValues(lsbSearchDisplayColumns), "'", "")

        SetItemSelected(ddlSearchColumns, GetDataTable("Select SearchColumns From Projects Where ID = " & GetProjectID(), _cnx).Rows(0).Item("SearchColumns"))
    End Sub

    Function CheckControlsComplete()
        Dim ncounter As Integer
        Dim dt As New DataTable

        If Request.QueryString("ID") <> "" Then
            dt = GetDataTable(_cnx, "Select * From " & DT_WEBRAD_PROJECTCONTROLS & " Where ProjectID = " & Request.QueryString("ID"))

            If dt.Rows.Count > 0 Then
                For ncounter = 0 To dt.Rows.Count - 1
                    If dt.Rows(ncounter).Item("ControlType") = "-1" Then
                        Return 0
                    End If
                Next
            Else
                Return 0
            End If
        Else
            Return 0
        End If

        Return 1
    End Function



    Protected Sub lsbBackendFolders_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lsbBackendFolders.SelectedIndexChanged
        If lblSelectedBackendFolder.Text <> "" Then
            UpdateFolders(lsbBackendFolders, lblSelectedBackendFolder, pnlBackendFolder, txtBackendLink)
        End If
    End Sub

    Sub UpdateFolders(ByRef lsbFolder As ListBox, ByRef lblSelectedFolder As Label, ByRef pnlFolder As Panel, ByRef txtLink As TextBox, Optional ByVal sFolder As String = "")
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ImpersonateAsUser()
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Declare local variables
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim dis() As DirectoryInfo
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sFolder <> "" And sFolder <> S_NONE Then
            Try
                dis = New DirectoryInfo(sFolder).GetDirectories
            Catch ex As Exception
                Directory.CreateDirectory(sFolder)
                dis = New DirectoryInfo(sFolder).GetDirectories
            End Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            lblSelectedFolder.Text = sFolder
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Try
                dis = New DirectoryInfo(sFolder).GetDirectories
            Catch ex As Exception
                Directory.CreateDirectory(sFolder)
                dis = New DirectoryInfo(sFolder).GetDirectories
            End Try
        Else
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If lsbFolder.SelectedIndex > -1 Then
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                '
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Try
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    '
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    If lsbFolder.SelectedItem.Value = "return" Then
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        '
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        If lblSelectedFolder.Text = "\\web1\~whitworth" Or lblSelectedFolder.Text = "\\web2\~whitworth" Or lblSelectedFolder.Text = "\\web2\~whitworthdev" Then
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            '
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            LoadDefaultFolders(lsbFolder, lblSelectedFolder)
                            pnlFolder.Visible = lblSelectedFolder.Text <> S_NONE
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            '
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            Exit Sub
                        Else
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            '
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            dis = New DirectoryInfo(lblSelectedFolder.Text).Parent.GetDirectories
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            '
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
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
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If lsbFolder.SelectedIndex > -1 Or (lblSelectedFolder.Text <> "" And lblSelectedFolder.Text <> S_NONE) Then
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            lsbFolder.Items.Clear()
            lsbFolder.Items.Add(New ListItem("[return to parent]", "return"))
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Try
                For nCounter As Integer = 0 To dis.GetUpperBound(0)
                    'Write(dis(nCounter).Name & "<br /><br />")
                    lsbFolder.Items.Add(New ListItem(dis(nCounter).Name, dis(nCounter).FullName))
                Next
            Catch ex As Exception
                'Empty catch statement
            End Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            pnlFolder.Visible = lblSelectedFolder.Text <> S_NONE
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If lblSelectedFolder.Text <> "" And Not txtLink Is Nothing Then
            SetLinkBasedOnFolder(txtLink, lblSelectedFolder.Text)
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        UndoImpersonateAsUser()
    End Sub


    Protected Sub rblCreateBackendFolder_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblCreateBackendFolder.SelectedIndexChanged
        pnlNewBackendFolder.Visible = If(rblCreateBackendFolder.SelectedValue = 1, True, False)
    End Sub

    Protected Sub cvBackendPath_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvBackendPath.ServerValidate
        args.IsValid = pnlBackendFolder.Visible = True
    End Sub

    Protected Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnContinue.Click
        Page.Validate()

        If Page.IsValid Then
            Dim cmd As New SqlCommand

            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "usp_UpdateProjectBackend"
            cmd.Connection = _cnx

            cmd.Parameters.AddWithValue("@ID", Request.QueryString("ID"))
            cmd.Parameters.AddWithValue("@IncludeBackend", rblIncludebackend.SelectedValue)
            cmd.Parameters.AddWithValue("@DefaultSort", rblDefaultSort.SelectedValue)

            If pnlSchedulePage.Visible Then
                'Bug here, the first time you select schedule and save the backend options, it doesn't seem to save the closed message

                cmd.Parameters.AddWithValue("@ClosedMessage", If(txtClosedMessage.Text <> "", txtClosedMessage.Text, "Sorry, the <strong>" & lblFormName.Text & "</strong> form is now closed."))
            End If

            cmd.Parameters.AddWithValue("@BackendLink", FormatProjectLink(txtBackendLink.Text))
            cmd.Parameters.AddWithValue("@BackendPath", lblSelectedBackendFolder.Text)

            If pnlNewBackendFolder.Visible Then
                cmd.Parameters.AddWithValue("@BackendNewFolder", txtNewBackendFolderName.Text)
            End If

            If pnlCustomselectstatement.Visible Then
                cmd.Parameters.AddWithValue("@CustomSelectStatement", txtCustomSelectStatement.Text)
            End If

            cmd.Parameters.AddWithValue("@SearchColumns", ddlSearchColumns.SelectedValue)
            cmd.Parameters.AddWithValue("@PageLimit", txtPageLimit.Text)

            ExecuteNonQuery(cmd, _cnx)

            ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTCOLUMNS & "  Where (Type='Export' OR Type='Report') AND ProjectID = " & GetProjectID(), "tryan", 3, False, _cnx)
            ExecuteNonQuery("DELETE FROM " & DT_WEBRAD_PROJECTBACKEND_EXPORTS & " WHERE ProjectID = " & GetProjectID())

            SaveBackendOptions()
            SaveAdditionalLinksItems(GetProjectID())

            Redirect("finalize.aspx?ID=" & Request.QueryString("ID"))
        End If
    End Sub

    Sub SaveBackendOptions()
        ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  Where ProjectID = " & Request.QueryString("ID"), _cnx)
        ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  WHERE Not OptionID in (Select ID From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & " )", _cnx)

        Dim cmd As New SqlCommand
        cmd.Connection = _cnx
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_InsertProjectBackendOption"

        For nCounter As Integer = 0 To cblBackendOptions.Items.Count - 1
            If cblBackendOptions.Items(nCounter).Selected Then
                If cblBackendOptions.Items(nCounter).Text = "Filter" Then
                    SaveFilterInfo()
                ElseIf cblBackendOptions.Items(nCounter).Text = "Sort" Then
                    SaveSortInfo()
                ElseIf cblBackendOptions.Items(nCounter).Text = "Actions other than delete" Then
                    SaveActions()
                ElseIf cblBackendOptions.Items(nCounter).Text = "Search" Then
                    SaveSearchColumns()
                Else
                    InsertWebRadProjectBackendOption(cblBackendOptions.Items(nCounter).Value)
                End If

                If cblBackendOptions.Items(nCounter).Text = "Export" Or cblBackendOptions.Items(nCounter).Text = "Report" Then
                    SaveExports(cblBackendOptions.Items(nCounter).Text, CType(Page.FindControl("rpt" & cblBackendOptions.Items(nCounter).Text & "s"), Repeater))
                End If
            End If
        Next

        SaveAncillaryMaintenance()
        SaveDisplayColumns()
    End Sub

    'Sub SaveExports()
    '    Dim nExportID As Integer
    '    Dim cmd As New SqlCommand("usp_InsertProjectBackendExport", cnx)
    '    cmd.CommandType = CommandType.StoredProcedure

    '    cmd.Parameters.AddWithValue("@ProjectID", GetProjectID())
    '    cmd.Parameters.AddWithValue("@Name", "")
    '    cmd.Parameters.AddWithValue("@Type", "")
    '    cmd.Parameters.AddWithValue("@SQLQuery", "")

    '    For Each CurrentItem As RepeaterItem In rptExports.Items
    '        With CurrentItem
    '            cmd.Parameters("@Name").Value = CType(.FindControl("txtName"), TextBox).Text
    '            cmd.Parameters("@Type").Value = CType(.FindControl("rblDataSourceType"), RadioButtonList).SelectedValue
    '            cmd.Parameters("@SQLQuery").Value = If(CType(.FindControl("pnlStatement"), Panel).Visible, CType(.FindControl("txtSQLQuery"), TextBox).Text, "")

    '            nExportID = ExecuteScalar(cmd)

    '            SaveColumnsInfo("Export", CurrentItem.FindControl("lsbMainColumns"), pnlExport, CurrentItem.FindControl("rptTables"), nExportID)
    '        End With
    '    Next
    'End Sub

    Sub SaveExports(ByVal sType As String, ByVal rptCurrent As Repeater)
        Dim nExportId As Integer
        Dim cmd As New SqlCommand("usp_InsertProjectBackendExport", _cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@ProjectID", GetProjectID())
        cmd.Parameters.AddWithValue("@Name", "")
        cmd.Parameters.AddWithValue("@Type", sType)
        cmd.Parameters.AddWithValue("@FileType", "")
        cmd.Parameters.AddWithValue("@DataSourceType", "")
        cmd.Parameters.AddWithValue("@SQLQuery", "")
        cmd.Parameters.AddWithValue("@Template", "")
        cmd.Parameters.AddWithValue("@DataSourceID", "")

        For Each currentItem As RepeaterItem In rptCurrent.Items
            With currentItem
                cmd.Parameters("@Name").Value = CType(.FindControl("txtName"), TextBox).Text

                If sType = "Export" Then
                    cmd.Parameters("@FileType").Value = CType(.FindControl("rblFileType"), RadioButtonList).SelectedValue
                Else
                    cmd.Parameters("@FileType").Value = ""
                End If

                cmd.Parameters("@DataSourceType").Value = CType(.FindControl("rblDataSourceType"), RadioButtonList).SelectedValue

                If CType(.FindControl("rblDataSourceType"), RadioButtonList).SelectedValue = "2" Then
                    cmd.Parameters("@DataSourceID").Value = CType(currentItem.FindControl("ucDataSource"), WebRADDataSource).SaveData(N_REPORT_DATASOURCEPARENTTYPE)
                Else
                    cmd.Parameters("@DataSourceID").Value = DBNull.Value
                End If

                cmd.Parameters("@Template").Value = If(Not CType(.FindControl("txtTemplate"), TextBox) Is Nothing, CType(.FindControl("txtTemplate"), TextBox).Text, "")

                nExportId = ExecuteScalar(cmd)
                SaveColumnsInfo(sType, currentItem.FindControl("lsbMainColumns"), BackendOptionSelected(sType), currentItem.FindControl("rptTables"), nExportId)
            End With
        Next
    End Sub

    Sub SaveAncillaryMaintenance()
        ExecuteNonQuery("DELETE FROM " & DT_WEBRAD_PROJECTANCILLARYMAINTENANCE & " WHERE ProjectID = " & GetProjectID())

        If pnlAncillarymaintenance.Visible Then
            Dim cmd As New SqlCommand
            cmd.Connection = _cnx
            cmd.CommandText = "usp_InsertProjectAncillaryMaintenance"
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@ProjectID", GetProjectID)
            cmd.Parameters.AddWithValue("@AncillaryProjectID", "")
            cmd.Parameters.AddWithValue("@ShortName", "")
            cmd.Parameters.AddWithValue("@Singleton", "")

            For Each currentItem As RepeaterItem In rptAncillaryMaintenance.Items
                With currentItem
                    cmd.Parameters("@AncillaryProjectID").Value = CType(.FindControl("ddlProject"), DropDownList).SelectedValue
                    cmd.Parameters("@ShortName").Value = CType(.FindControl("txtShortName"), TextBox).Text
                    cmd.Parameters("@Singleton").Value = CType(.FindControl("chkSingleton"), checkbox).checked
                End With

                ExecuteNonQuery(cmd)
            Next
        End If
    End Sub

    Sub SaveActions()
        If pnlActionsOtherThanDelete.Visible Then
            Dim cmd As New SqlCommand
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Connection = _cnx

            Dim nCurrentId As Integer

            nCurrentId = InsertWebRadProjectBackendOption(5)

            cmd.CommandText = "usp_InsertProjectBackendOptionColumn"
            cmd.Parameters.AddWithValue("@OptionID", nCurrentId)
            cmd.Parameters.AddWithValue("@ControlID", "")
            cmd.Parameters.AddWithValue("@Label", "")
            cmd.Parameters.AddWithValue("@OperatorType", "")
            cmd.Parameters.AddWithValue("@ComparisonValue", "")
            cmd.Parameters.AddWithValue("@CustomActionCode", "")

            For Each currentItem As RepeaterItem In rptActions.Items
                cmd.Parameters("@ControlID").Value = CType(currentItem.FindControl("ddlControlID"), DropDownList).SelectedValue
                cmd.Parameters("@Label").Value = CType(currentItem.FindControl("txtLabel"), TextBox).Text
                cmd.Parameters("@OperatorType").Value = CType(currentItem.FindControl("ddlActionType"), DropDownList).SelectedValue
                cmd.Parameters("@ComparisonValue").Value = If(CType(currentItem.FindControl("pnlUpdateValue"), Panel).Visible, CType(currentItem.FindControl("txtUpdateValue"), TextBox).Text, "")
                cmd.Parameters("@CustomActionCode").Value = If(CType(currentItem.FindControl("pnlCustomActionCode"), Panel).Visible, CType(currentItem.FindControl("txtCustomActionCode"), TextBox).Text, "")

                ExecuteScalar(cmd)
            Next
        End If
    End Sub

    Sub SaveDisplayColumns()
        Dim cmd As New SqlCommand
        cmd.Connection = _cnx
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_InsertProjectBackendOption"

        cmd.Parameters.AddWithValue("@ProjectID", Request.QueryString("ID"))
        cmd.Parameters.AddWithValue("@Type", 8)

        Dim nCurrentId As Integer = ExecuteScalar(cmd, _cnx)

        cmd.Parameters.Clear()
        cmd.CommandText = "usp_InsertProjectBackendOptionColumn"

        cmd.Parameters.AddWithValue("@OptionID", nCurrentId)
        cmd.Parameters.AddWithValue("@ControlID", "")

        For Each currentItem As ListItem In lsbDisplayColumns.Items
            If currentItem.Selected Then
                cmd.Parameters("@ControlID").Value = currentItem.Value

                ExecuteNonQuery(cmd, "tryan", 3, False, _cnx)
            End If
        Next
    End Sub

    Sub SaveSearchColumns()
        Dim cmd As New SqlCommand
        cmd.Connection = _cnx
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_InsertProjectBackendOption"

        cmd.Parameters.AddWithValue("@ProjectID", Request.QueryString("ID"))
        cmd.Parameters.AddWithValue("@Type", 10)

        Dim nSearchTermsId As Integer = ExecuteScalar(cmd, _cnx)
        Dim nSearchColumnsId As Integer = ExecuteScalar(cmd, _cnx)

        cmd.Parameters.Clear()
        cmd.CommandText = "usp_InsertProjectBackendOptionColumn"

        cmd.Parameters.AddWithValue("@OptionID", nSearchTermsId)
        cmd.Parameters.AddWithValue("@ControlID", "")

        For Each currentItem As ListItem In lsbSearchTermControls.Items
            If currentItem.Selected Then
                cmd.Parameters("@ControlID").Value = currentItem.Value

                ExecuteNonQuery(cmd, "tryan", 3, False, _cnx)
            End If
        Next

        cmd.Parameters.Clear()
        cmd.CommandText = "usp_InsertProjectBackendOptionColumn"

        cmd.Parameters.AddWithValue("@OptionID", nSearchColumnsId)
        cmd.Parameters.AddWithValue("@ControlID", "")

        For Each currentItem As ListItem In lsbSearchDisplayColumns.Items
            If currentItem.Selected Then
                cmd.Parameters("@ControlID").Value = currentItem.Value

                ExecuteNonQuery(cmd, "tryan", 3, False, _cnx)
            End If
        Next

    End Sub

    Sub SaveSortInfo()
        If pnlSort.Visible Then
            Dim cmd As New SqlCommand
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Connection = _cnx

            Dim nCurrentId As Integer

            nCurrentId = InsertWebRadProjectBackendOption(1)

            cmd.CommandText = "usp_InsertProjectBackendOptionColumn"
            cmd.Parameters.AddWithValue("@OptionID", nCurrentId)
            cmd.Parameters.AddWithValue("@ControlID", "")
            cmd.Parameters.AddWithValue("@Label", "")

            For Each currentItem As RepeaterItem In rptSortColumns.Items
                cmd.Parameters("@ControlID").Value = CType(currentItem.FindControl("ddlControlID"), DropDownList).SelectedValue
                cmd.Parameters("@Label").Value = CType(currentItem.FindControl("txtLabel"), TextBox).Text

                ExecuteScalar(cmd)
            Next
        End If
    End Sub

    Function InsertWebRadProjectBackendOption(ByVal nType As Integer, Optional ByVal sLabel As String = "", Optional ByVal nSelectionType As Integer = Nothing) As Integer
        Dim cmd As New SqlCommand
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Connection = _cnx
        cmd.CommandText = "usp_InsertProjectBackendOption"

        cmd.Parameters.AddWithValue("@ProjectID", Request.QueryString("ID"))
        cmd.Parameters.AddWithValue("@Type", nType)
        cmd.Parameters.AddWithValue("@Label", sLabel)
        cmd.Parameters.AddWithValue("@SelectionType", nSelectionType)

        Dim nCurrentId As Integer

        nCurrentId = ExecuteScalar(cmd)

        Return nCurrentId
    End Function

    Sub SaveFilterInfo()
        If pnlFilter.Visible Then
            For Each currentItem As RepeaterItem In rptFilterOptions.Items
                Dim cmd As New SqlCommand
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Connection = _cnx

                Dim nCurrentId As Integer

                nCurrentId = InsertWebRadProjectBackendOption(2, CType(currentItem.FindControl("txtLabel"), TextBox).Text, CType(currentItem.FindControl("rblSelectionType"), RadioButtonList).SelectedValue)

                cmd.CommandText = "usp_InsertProjectBackendOptionColumn"
                cmd.Parameters.AddWithValue("@OptionID", nCurrentId)
                cmd.Parameters.AddWithValue("@ControlID", "")
                cmd.Parameters.AddWithValue("@OperatorType", "")
                cmd.Parameters.AddWithValue("@ComparisonValue", "")

                For Each currentItem2 As RepeaterItem In CType(currentItem.FindControl("rptOptionColumns"), Repeater).Items
                    cmd.Parameters("@ControlID").Value = CType(currentItem2.FindControl("ddlControlID"), DropDownList).SelectedValue
                    cmd.Parameters("@OperatorType").Value = CType(currentItem2.FindControl("rblOperatorType"), RadioButtonList).SelectedValue
                    cmd.Parameters("@ComparisonValue").Value = CType(currentItem2.FindControl("txtComparisonValue"), TextBox).Text

                    ExecuteScalar(cmd)
                Next
            Next
        End If
    End Sub


    'Sub SaveExportInfo()
    '    Dim cmd As New SqlCommand("usp_InsertProjectColumn", cnx)

    '    cmd.CommandType = CommandType.StoredProcedure
    '    cmd.Parameters.AddWithValue("@ProjectID", Request.QueryString("ID"))
    '    cmd.Parameters.AddWithValue("@TableControlID", "")
    '    cmd.Parameters.AddWithValue("@ColumnControlID", "")
    '    cmd.Parameters.AddWithValue("@Type", "Export")

    '    If pnlExport.Visible Then
    '        cmd.Parameters("@TableControlID").Value = "0"

    '        SaveExportColumn(lsbMainColumns, cmd)

    '        For Each CurrentItem As RepeaterItem In rptTables.Items
    '            cmd.Parameters("@TableControlID").Value = CType(CurrentItem.FindControl("lblID"), Label).Text

    '            SaveExportColumn(CType(CurrentItem.FindControl("lsbColumns"), ListBox), cmd)
    '        Next
    '    End If
    'End Sub

    Sub SaveExportColumn(ByRef lsbCurrent As ListBox, ByRef cmd As SqlCommand)
        If lsbCurrent.Items(0).Selected Then
            cmd.Parameters("@ColumnControlID").Value = "0"

            ExecuteNonQuery(cmd, "tryan", 3, False, _cnx)
        Else
            For Each currentItem As ListItem In lsbCurrent.Items
                If currentItem.Selected Then
                    cmd.Parameters("@ColumnControlID").Value = currentItem.Value

                    ExecuteNonQuery(cmd, "tryan", 3, False, _cnx)
                End If
            Next
        End If
    End Sub


    Protected Sub ddlSortColumns_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSortColumns.SelectedIndexChanged
        Dim dtSupplied As New WhitTools.DataTablesSupplied

        AddAvailableControls(dtSupplied)
        WriteDataTableValues(dtSupplied)
        UpdateRepeaterItems(rptSortColumns, ddlSortColumns.SelectedValue, _cnx, dtSupplied)
    End Sub

    Protected Sub ddlFilterOptions_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim dtSupplied As New WhitTools.DataTablesSupplied

        dtSupplied.AddRow("ddlOptionColumns", "FillNumbers", "", "1", "10")
        dtSupplied.AddRow("rblOperatorType", "SQLSelect", "Select * FROM " & DT_WEBRAD_BACKENDOPTIONOPERATORTYPES, "Description", "ID")
        AddAvailableControls(dtSupplied)


        UpdateRepeaterItems(rptFilterOptions, ddlFilterOptions.SelectedValue, _cnx, dtSupplied)
    End Sub

    Protected Sub ddlOptionColumns_SelectedIndexChanged(sender As Object, e As EventArgs)
        With CType(sender.parent, RepeaterItem)
            Dim dtSupplied As New WhitTools.DataTablesSupplied

            AddAvailableControls(dtSupplied)

            dtSupplied.AddRow("rblOperatorType", "SQLSelect", "Select * FROM " & DT_WEBRAD_BACKENDOPTIONOPERATORTYPES, "Description", "ID")

            UpdateRepeaterItems(CType(.FindControl("rptOptionColumns"), Repeater), CType(.FindControl("ddlOptionColumns"), DropDownList).SelectedValue, _cnx, dtSupplied)
        End With
    End Sub



    Protected Sub rblIncludebackend_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblIncludebackend.SelectedIndexChanged
        Showbackend()
    End Sub

    Sub Showbackend()
        pnlBackend.Visible = If(rblIncludebackend.SelectedValue = 1, True, False)
    End Sub



    Protected Sub ddlActions_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim dtSupplied As New WhitTools.DataTablesSupplied

        AddAvailableControls(dtSupplied)

        dtSupplied.AddRow("ddlActionType", "SQLSelect", "Select ID, Description FROM " & DT_WEBRAD_BACKENDACTIONTYPES & " ", "Description", "ID")

        UpdateRepeaterItems(rptActions, ddlActions.SelectedValue, _cnx, dtSupplied)

        For Each currentItem As RepeaterItem In rptActions.Items
            ddlActionType_SelectedIndexChanged(currentItem.FindControl("ddlActionType"), Nothing)
        Next
    End Sub

    Protected Sub ddlActionType_SelectedIndexChanged(sender As Object, e As EventArgs)
        With CType(sender.parent, RepeaterItem)
            If CType(sender, DropDownList).SelectedIndex > 0 Then
                CType(.FindControl("pnlUpdateValue"), Panel).Visible = CType(sender, DropDownList).SelectedValue = N_BACKENDUPDATEACTION_TYPE
                CType(.FindControl("pnlCustomActionCode"), Panel).Visible = CType(sender, DropDownList).SelectedValue = N_BACKENDCUSTOMACTION_TYPE
            End If
        End With
    End Sub

    Protected Sub cvBackendLink_ServerValidate(source As Object, args As ServerValidateEventArgs)
        ValidateLink(cvBackendLink, txtBackendLink, args)
    End Sub

    Protected Sub txtNewbackendFolderName_TextChanged(sender As Object, e As EventArgs) Handles txtNewBackendFolderName.TextChanged
        SetLinkBasedOnFolder(txtBackendLink, lblSelectedBackendFolder.Text)
        txtBackendLink.Text = txtBackendLink.Text & txtNewBackendFolderName.Text

        If Right(txtBackendLink.Text, 1) <> "/" Then
            txtBackendLink.Text &= "/"
        End If
    End Sub

    Protected Sub ddlNumberAncillaryMaintenance_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlNumberAncillaryMaintenance.SelectedIndexChanged
        Dim dtSupplied As New WhitTools.DataTablesSupplied

        dtSupplied.AddRow("ddlProject", "SQLSelect", "Select * FROM web3.WebRAD.dbo.Projects WHERE NOT ID = " & GetProjectID() & " AND NOT ID IN (SELECT ProjectID FROM " & DT_WEBRAD_PROJECTANCILLARYMAINTENANCE & " WHERE AncillaryProjectID = " & GetProjectID() & ") ORDER By PageTitle", "PageTitle", "ID")
        UpdateRepeaterItems(rptAncillaryMaintenance, ddlNumberAncillaryMaintenance.SelectedValue, dtSupplied)
    End Sub



    Protected Sub ddlNumberExports_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim dtSupplied As New WhitTools.DataTablesSupplied
        Dim dtExportColumns As DataTable = GetExportColumns()
        Dim rptCurrent As Repeater = CType(Page.FindControl("rpt" & Replace(sender.id, "ddlNumber", "")), Repeater)
        Dim allRow As DataRow = dtExportColumns.NewRow

        allRow.Item("Name") = "All"
        allRow.Item("ColumnControlID") = "0"
        dtExportColumns.Rows.InsertAt(allRow, 0)

        dtSupplied.AddRow("lsbMainColumns", "FillListData", dtExportColumns, "Name", "ColumnControlID")
        UpdateRepeaterItems(rptCurrent, CType(sender, DropDownList).SelectedValue, dtSupplied)

        For Each currentItem As RepeaterItem In rptCurrent.Items
            GetAdditionalExportColumns(currentItem.FindControl("rptTables"))

            rblDataSourceType_SelectedIndexChanged(currentItem.FindControl("rblDataSourceType"), Nothing)
        Next
    End Sub

    <System.Web.Services.WebMethod()>
     Function TestMethod(ByVal nProjectId As Integer) As List(Of WebRADControl)

        Dim controlList As New List(Of WebRADControl)

        Dim wrc As New WebRADControl
        Dim cmi As New Common.ContextMenuItem

        'WRC.key = "Test"
        'CMI.name = "test"
        'WRC.item = CMI

        'blahList.Add(WRC)

        'WRC = New WebRADControl
        'CMI = New ContextMenuItem

        'WRC.key = "Test2"
        'CMI.name = "test2"
        'WRC.item = CMI

        'blahList.Add(WRC)

        'hitTools.Email.SendEmail("tryan@whitworth.edu", "blah", HttpContext.Current.Request.QueryString("ID"))
        ' Console.Write("fasdjkl")
        Dim dtControls As DataTable = GetDataTable("Select * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & nProjectId & " AND ControlType IN (SELECT ID FROM " & DT_WEBRAD_CONTROLTYPES & " WHERE DataType IN " & GetControlTypesWithValues() & ") ORDER BY Position asc")


        For Each currentRow As DataRow In dtControls.Rows
            With currentRow
                wrc = New Common.WebRADControl
                cmi = New Common.ContextMenuItem

                wrc.key = .Item("Name")
                cmi.Name = .Item("Name")
                wrc.item = cmi

                controlList.Add(wrc)
            End With
        Next


        Return controlList
    End Function

    Protected Sub ddlTest_SelectedIndexChanged(sender As Object, e As EventArgs)
        UpdateRepeaterItems(rptTest, ddlTest.SelectedValue)

        For Each currentitem As RepeaterItem In rptTest.Items
            With CType(currentitem.FindControl("ucDataSource"), WebRADDataSource)
                '                .BindData(11)
                CType(.FindControl("pnlDataSourceType"), Panel).Visible = True
                ShowDataSource(currentitem.FindControl("ucDataSource"))
            End With
        Next

    End Sub

    Protected Sub btnrptAdditionalLinksAddItem_Click(sender As Object, e As EventArgs)
        AddNewRepeaterItem(rptAdditionalLinks, Nothing, 1, , cvAdditionalLinks, "additional link")
    End Sub

    Protected Sub librptAdditionalLinks_RemoveItem_Click(sender As Object, e As EventArgs)
        Dim parentRepeaterItem As RepeaterItem = GetParentRepeaterItem(sender)
        RemoveRepeaterItem(parentRepeaterItem.Parent, Nothing, CType(GetParentRepeaterItem(sender), RepeaterItem).ItemIndex, , cvAdditionalLinks, "additional link")
    End Sub

    Sub SaveAdditionalLinksItems(ByVal nProjectId As Integer)
        Dim nCounter As Integer
        Dim sCurrentIds As String = ""
        Dim cmd As New SqlCommand

        cmd.Connection = _cnx
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_InsertProjectBackendAdditionalLink"

        For nCounter = 0 To rptAdditionalLinks.Items.Count - 1
            With rptAdditionalLinks.Items(nCounter)
                cmd.Parameters.Clear()

                cmd.Parameters.AddWithValue("@ProjectID", nProjectId)
                cmd.Parameters.AddWithValue("@ID", CType(.FindControl("lblID"), Label).Text)
                cmd.Parameters.AddWithValue("@Name", CType(.FindControl("txtName"), TextBox).Text)
                cmd.Parameters.AddWithValue("@URL", CType(.FindControl("txtURL"), TextBox).Text)

                Dim nCurrentId As Integer

                nCurrentId = ExecuteScalar(cmd, _cnx, "tryan")

                sCurrentIds &= If(sCurrentIds <> "", ",", "") & nCurrentId
            End With
        Next

        ExecuteNonQuery("DELETE FROM " & DT_WEBRAD_PROJECTBACKENDADDITIONALLINKS & " WHERE (ProjectID = " & nProjectId & If(sCurrentIds <> "", " AND NOT ID IN (" & sCurrentIds & ")", "") & ")", _cnx)
    End Sub

    Public Sub cvAdditionalLinks_ServerValidate(ByVal source As System.Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
    End Sub

    Function BackendOptionSelected(optionText As String) As Boolean
        Return cblBackendOptions.Items.FindByText(optionText).Selected
    End Function

End Class
