Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.IO
Imports WhitTools.eCommerce
Imports WhitTools.Cookies
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.Filler
Imports WhitTools.Utilities
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
Imports Common.General.Folders
Imports Common.General.Repeaters
Imports Common.General.Controls
Imports Common.SQL.Main
Imports Common.General.ControlTypes
Imports Common.General.Actions
Imports Common.General.Pages
Imports Common.General.ProjectOperations
Imports Common.Webpages.Frontend.MultiPage
Imports Common.General.DataTypes
Imports AjaxControlToolkit
Imports Common
Imports Microsoft.VisualBasic.CompilerServices
Imports WhitTools
Imports System.Web.UI.WebControls
Imports System.Web.UI

Partial Public Class controlsnew
    Inherits System.Web.UI.Page

    Shared cnx As SqlConnection = CreateSQLConnection("WebRAD")

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then
            Page.MaintainScrollPositionOnPostBack = True
            LoadDDLs()
        End If

        sds.SelectCommand = Replace(sds.SelectCommand, "@ProjectID", Request.QueryString("ID"))
        sds.SelectCommand = Replace(sds.SelectCommand, "@PageID", Request.QueryString("PageID"))
        sds.InsertCommand = Replace(sds.InsertCommand, "@ProjectID", Request.QueryString("ID"))
        sds.InsertCommand = Replace(sds.InsertCommand, "@PageID", Request.QueryString("PageID"))
    End Sub

    Sub FixControlPositioning()
        Dim currentPosition As Integer = 1
        Dim projectID = GetQueryString("ID")
        Dim db As New WebRADEntities()

        Dim controls = db.ProjectControls.Where(Function(control) control.ProjectID = projectID).OrderBy(Function(control) control.Position).ToList()

        For Each currentControl As ProjectControl In controls.Where(Function(control) If(control.ParentControlID, 0) = 0).ToList()
            SetControlPositions(currentControl, currentPosition)
        Next

        db.SaveChanges()
    End Sub

    Sub SetControlPositions(ByRef currentcontrol As ProjectControl, ByRef currentPosition As Integer)
        currentcontrol.Position = currentPosition
        currentPosition += 1

        For Each childControl As ProjectControl In currentcontrol.ProjectControls1
            SetControlPositions(childControl, currentPosition)
        Next
    End Sub


    Public Shared Function GetControlReference(ByVal parentObject As Object, ByVal controlName As String) As Control
        Dim foundControl As Control

        For Each currentControl As Control In parentObject.Controls
            SearchForControl(currentControl, controlName, foundControl)
        Next

        Return foundControl
    End Function

    Private Shared Function SearchForControl(ByRef parentControl, ByVal controlName, ByRef foundcontrol) As Control
        If parentControl.ID = controlName Then
            foundcontrol = parentControl
            Exit Function
        End If

        For Each currentControl As Control In parentControl.Controls
            SearchForControl(currentControl, controlName, foundcontrol)
        Next
    End Function



    Protected WithEvents ucLSI As stable.WebRADListItems
    Protected WithEvents ucControlSupplyDataSource As stable.WebRADDataSource

    Sub LoadDDLs()
        CheckCurrentPage()

        CreateLoginColumnTypes()
        FillListData(ddlControlType, cnx, "Select * from " & DT_WEBRAD_CONTROLTYPES & "  Where ParentControlTypeID IS NULL Order by Type", "Type", "ID")
        FillListData(ddlSQLDataType, cnx, "Select * from " & DT_WEBRAD_CONTROLSQLTYPES & " ", "DataType", "ID")
        FillListData(lsbFileTypesAllowed, cnx, "Select Name + ' (' +  Extension + ')' as Name, ID From " & DT_WEBRAD_FILETYPES & " ", "Name", "ID", False)

        FillListData(rblLayoutType, GetDataTable("Select * From " & DT_WEBRAD_LAYOUTTYPES & " ", cnx), "Description", "ID", False)
        FillListData(CType(ucLSI.FindControl("ddlDataMethod"), DropDownList), cnx, "Select * from " & DT_WEBRAD_DATAMETHODTYPES & " ", "Name", "ID")

        FillNumbers(ddlMaximumRequired, 0, 100, False)
        FillNumbers(CType(ucLSI.FindControl("ddlListItems"), DropDownList), 0, 100, True)
        FillNumbers(ddlRepeatColumns, 1, 10, True)
        FillNumbers(ddlActions, 1, 50, True)
        'FillListData(ddlDisplayType, db.ControlDisplayTypes.,"Description","ID",false)        
        ddlDisplayType.DataSource = db.ControlDisplayTypes.ToList()
        ddlDisplayType.DataTextField = "Description"
        ddlDisplayType.DataValueField = "ID"
        ddlDisplayType.DataBind()

        LoadPages()

        Try
            If Request.QueryString("ControlID") <> "" Then
                LoadCurrentControl(Request.QueryString("ControlID"))
            Else
                CheckControlList()
            End If
        Catch ex As Exception

        End Try

        Dim controls = db.ProjectControls.ToList().Where(Function(pc) If(pc.ProjectID, 0) = GetQueryString("ID") And If(pc.PageID, 0) = GetQueryString("PageID")).OrderBy(Function(pc) pc.Name)

        If controls.Count > 0 Then
            Try
                ddlParentControls.DataSource = controls.Select(Function(pc) New ListItem() With {.Text = pc.Name, .Value = pc.ID})
                ddlParentControls.DataTextField = "Text"
                ddlParentControls.DataValueField = "Value"
                ddlParentControls.DataBind()
            Catch ex As Exception
                WriteLine("error getting parent controls")
                WriteLine(ex.ToString())
            End Try

        End If
    End Sub



    Sub writecontrolids(ByRef control As Control)
        For Each currentcontrol As Control In control.Controls
            WriteLine(currentcontrol.ID)

            If currentcontrol.HasControls Then
                WriteLine("is parent of: ")
                writecontrolids(currentcontrol)
            End If
        Next
    End Sub


    Sub rlEvents_DeleteCommand(ByVal sender As Object, ByVal e As AjaxControlToolkit.ReorderListCommandEventArgs)
        Dim nCurrentID As Integer = e.CommandArgument.ToString()

        WhitTools.SQL.ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & "  Where TriggerControl = " & nCurrentID, cnx)
        WhitTools.SQL.ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONTRIGGERVALUES & "  Where ActionID NOT IN (Select ID FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & ")", cnx)
        WhitTools.SQL.ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTCONTROLLISTITEMS & "  Where Type = 1 and ParentID = " & nCurrentID, cnx)
        WhitTools.SQL.ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & "  Where ControlID = " & nCurrentID, cnx)
        WhitTools.SQL.ExecuteNonQuery("UPDATE " & DT_WEBRAD_PROJECTCONTROLS & " SET ParentControlID = NULL WHERE ParentControlID = " & nCurrentID, cnx)
        WhitTools.SQL.ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTCONTROLS & " Where [id] = " & nCurrentID)

        'Bug here - make sure all dependencies are deleted when control is deleted.  Having control in display list for backend
        'and then deleting it without updating the display list will cause a crash.
        rl1.DataBind()

        CheckControlList()
    End Sub

    Function GetName(ByVal sHeading, ByVal sName, ByVal nID, ByVal nPosition)
        If IsDBNull(sHeading) And IsDBNull(sName) Then
            If nID = lblCurrentControlID.Text Then
                Return "<table border='1' cellpadding='5' cellspacing='5'><tr><td>Control " & nPosition & "</td></tr></table>"
            Else
                Return "Control " & nPosition
            End If
        ElseIf sHeading = "" And sName = "" Then
            If nID = lblCurrentControlID.Text Then
                Return "<table border='1' cellpadding='5' cellspacing='5'><tr><td>Control " & nPosition & "</td></tr></table>"
            Else
                Return "Control " & nPosition
            End If
        Else
            If sHeading = "" Then
                sHeading = sName
            End If

            Dim dt As New DataTable

            dt = GetDataTable(cnx, "Select Prefix, * From " & DT_WEBRAD_PROJECTCONTROLS & " C left outer join " & DT_WEBRAD_CONTROLTYPES & "  T on C.ControlType = T.ID left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  D on T.DataType = D.ID Where c.ID = " & nID)

            With dt.Rows(0)
                If nID = lblCurrentControlID.Text Then
                    Return "<table border='1' cellpadding='5' cellspacing='5'><tr><td>" & sHeading & " (" & .Item("Prefix") & ")" & "</td></tr></table>"
                Else
                    Return sHeading & " (" & .Item("Prefix") & ")"
                End If
            End With
        End If
    End Function


    Protected Sub libSelectControl_Click(sender As Object, e As EventArgs)
        Dim libCurrent As LinkButton = sender

        Dim tclCurrent As TableCell = libCurrent.Parent.Parent

        LoadCurrentControl(CType(tclCurrent.FindControl("lblID"), Label).Text)
    End Sub

    Protected Sub libSelectPage_Click(sender As Object, e As EventArgs)
        'Dim libCurrent As LinkButton = sender

        'Dim tclCurrent As TableCell = libCurrent.Parent

        'LoadCurrentControl(CType(tclCurrent.FindControl("lblID"), Label).Text)
    End Sub

    Sub LoadCurrentControl(ByVal nControlID As Integer)
        Dim dt As New DataTable

        dt = GetDataTable(cnx, "Select * From " & DT_WEBRAD_PROJECTCONTROLS & " Where ID = " & nControlID, True, 0)

        With dt.Rows(0)
            lblCurrentControlID.Text = .Item("ID")
            lblCurrentControlPosition.Text = .Item("Position")

            ddlControlType.SelectedIndex = 0
            Clearcontrols(pnlControlTypeDetails)
            CheckParentControlAvailable()

            SetItemSelected(ddlControlType, .Item("ControlType"))
            SetItemSelected(ddlParentControl, .Item("ParentControlID"))
            CheckControlDetails()

            txtName.Text = .Item("Name")
            SetItemSelected(rblDisplayHeading, .Item("DisplayHeading"))
            txtHeading.Text = .Item("Heading")
            txtSubHeading.Text = .Item("Subheading")

            UpdateHeadingVisibility()

            SetItemSelected(rblRequired, .Item("Required"))

            FillNumbers(ddlMinimumRequired, 0, 100, False)

            If ddlControlType.SelectedIndex > 0 AndAlso ddlControlType.SelectedValue <> N_REPEATER_CONTROL_TYPE Then
                ddlMinimumRequired.Items.Insert(1, New ListItem("All"))
            End If

            SetItemSelected(ddlMinimumRequired, .Item("MinimumRequired"))
            SetItemSelected(ddlMaximumRequired, .Item("MaximumRequired"))
            ShowMinimumRequired()

            SetItemSelected(rblRequireVerification, .Item("RequireVerification"))
            SetItemSelected(rblCustomValidation, .Item("CustomValidation"))
            txtCustomValidationCode.Text = .Item("CustomValidationCode")
            pnlCustomValidationCode.Visible = If(rblCustomValidation.SelectedIndex = 1, True, False)

            txtShortHeading.Text = .Item("ShortHeading")


            SetItemSelected(rblTextPosition, .Item("TextPosition"))
            SetItemSelected(rblEnabled, .Item("Enabled"))

            LoadVisibility(.Item("Visible"))

            SetItemSelected(rblCalendar, .Item("Calendar"))
            txtCustomVisibleValue.Text = .Item("CustomVisibleValue")
            ToggleCustomVisibleValue()
            SetItemSelected(rblRichTextbox, .Item("RichValue"))
            UpdateRichTextbox()

            If pnlRichValue.Visible = True Then
                txtRichValue.Text = .Item("Value")
            Else
                txtValue.Text = .Item("Value")
            End If

            txtText.Text = .Item("Text")
            txtPlaceholder.Text = .Item("Placeholder")

            SetItemSelected(ddlTextMode, .Item("TextMode"))
            SetItemSelected(rblRichTextboxUser, .Item("RichTextUser"))
            pnlRichTextboxUser.Visible = If(ddlTextMode.SelectedValue = "MultiLine", True, False)
            txtRows.Text = .Item("Rows")
            txtColumns.Text = .Item("Columns")
            txtMaxLength.Text = .Item("MaxLength")
            SetItemSelected(rblMaxLengthType, .Item("MaxLengthType"))
            txtMaxFileSize.Text = .Item("MaxFileSize")
            txtWidth.Text = .Item("Width")
            txtCssClass.Text = .Item("CssClass")

            SetItemSelected(rblLayoutType, .Item("LayoutType"))
            ToggleLayoutType()
            SetItemSelected(rblLayoutSubtype, .Item("LayoutSubtype"))
            SetItemSelected(rblRepeaterAddRemove, .Item("RepeaterAddRemove"))
            txtRepeaterItemName.Text = .Item("RepeaterItemName")
            rblRepeaterAddRemove_SelectedIndexChanged(Nothing, Nothing)

            SetItemSelected(rblSupplyControlData, .Item("SupplyControlData"))
            rblSupplycontroldata_SelectedIndexChanged(Nothing, Nothing)

            Try
                SetItemSelected(rblSupplyDataType, .Item("SupplyDataType"))
            Catch ex As Exception

            End Try

            rblSupplyDataType_SelectedIndexChanged(Nothing, Nothing)

            ucControlSupplyDataSource.BindData(.Item("DataSourceID"))
            ShowDataSource(ucControlSupplyDataSource)
            txtControlDataSourceColumn.Text = .Item("DataSourceColumn")



            SetItemSelected(rblDisplayLocation, .Item("DisplayLocation"))
            SetItemSelected(ddlDisplayType, .Item("DisplayType"))
            ToggleSQLDefaultValue()
            SetItemSelected(rblIncludeDatabase, .Item("IncludeDatabase"))
            ToggleIncludeDatabase()

            SetItemSelected(rblAutopostback, .Item("AutoPostback"))
            SetItemSelected(rblPerformPostbackAction, .Item("PerformPostbackAction"))

            SetItemSelected(rblOnchange, .Item("Onchange"))
            txtOnchangeCall.Text = .Item("OnchangeCall")
            txtOnchangeBody.Text = .Item("OnchangeBody")
            pnlOnchangeDetail.Visible = If(rblOnchange.SelectedIndex = 1, True, False)

            If .Item("uploadPath") <> "" Then
                UpdateFolders(lsbUploadPathFolders, lblSelectedUploadPathFolder, pnlUploadPathFolder, Nothing, .Item("UploadPath"))
            End If

            txtNewUploadPathFolderName.Text = .Item("UploadPathNewFolder")
            rblCreateUploadPathFolder.SelectedIndex = If(.Item("UploadPathNewFolder") <> "", 1, 0)
            pnlNewUploadPathFolder.Visible = .Item("UploadPathNewFolder") <> ""

            SetItemSelected(rblRepeatDirection, .Item("RepeatDirection"))
            SetItemSelected(ddlRepeatColumns, .Item("RepeatColumns"))
            SetItemSelected(rblListSelections, .Item("ListSelections"))
            txtGroupName.Text = .Item("GroupName")
            SetItemSelected(rblSelectionMode, .Item("SelectionMode"))

            ucLSI.BindData(dt)

            SetItemSelected(ddlSQLDataType, .Item("SQLDataType"))
            SetItemSelected(rblAllowNegativeValue, .Item("AllowNegativeValue"))
            SetItemSelected(rblAllowNegativeValue, .Item("AllowNegativeValue"))
            txtSQLDataSize.Text = .Item("SQLDatasize")
            SetItemSelected(rblSQLDefaultValueType, .Item("SQLDefaultValueType"))
            txtSQLDefaultValue.Text = .Item("SQLDefaultValue")

            txtSQLInsertItemStoredProcedure.Text = .Item("SQLInsertItemStoredProcedure")
            txtSQLInsertItemTable.Text = .Item("SQLInsertItemTable")
            txtForeignID.Text = .Item("ForeignID")
            txtProspectCode.Text = .Item("ProspectCode")

            If txtForeignID.Text = "" Then
                txtForeignID.Text = "ForeignID"
            End If

            ddlSQLDataType_SelectedIndexChanged(Nothing, Nothing)

            CheckTextModeOptions()
        End With

        ucLSI.LoadListItems(1, lblCurrentControlID.Text)
        ucLSI.CheckSelectionItems()

        LoadPostbackActions()

        CheckPostbackStatus()
        CheckPostbackActions()
        CheckActionListItems()


        dt = New DataTable
        dt = GetDataTable(cnx, "Select * From " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & "  Where ControlID = " & lblCurrentControlID.Text)

        SetListControlItemSelected(lsbFileTypesAllowed, "", True, dt, "FileType")

        sds.SelectCommand = Replace(sds.SelectCommand, "@ProjectID", Request.QueryString("ID"))
        sds.SelectCommand = Replace(sds.SelectCommand, "@PageID", Request.QueryString("PageID"))
        sds.InsertCommand = Replace(sds.InsertCommand, "@ProjectID", Request.QueryString("ID"))
        sds.InsertCommand = Replace(sds.InsertCommand, "@PageID", Request.QueryString("PageID"))

        rl1.DataBind()
    End Sub

    Private Sub LoadVisibility(ByVal Visible As String)
        SetItemSelected(rblVisible, Visible)
        LoadVisibleDependentValues()
        rblVisible_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Sub LoadVisibleDependentValues()
        Dim Values As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE TargetControl = " & lblCurrentControlID.Text & " AND Action = " & N_CONTROLACTIONTYPE_VISIBLE)

        If Values.Rows.Count > 0 Then
            EmptyRepeater(rptVisibleDependingControls)

            For Counter As Integer = 0 To Values.Rows.Count - 1
                btnrptVisibleDependingControlsAddItem_Click(Nothing, Nothing)

                SetItemSelected(CType(rptVisibleDependingControls.Items(Counter).FindControl("ddlDependingControl"), DropDownList), Values.Rows(Counter).Item("TriggerControl"))
                CType(rptVisibleDependingControls.Items(Counter).FindControl("lblID"), Label).Text = Values.Rows(Counter).Item("ID")

                SelectRepeaterData(CType(CType(rptVisibleDependingControls.Items(Counter).FindControl("ucVisibilityTriggerValues"), UserControl).FindControl("rptTriggerValues"), Repeater), GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONTRIGGERVALUES & " WHERE ActionID = " & CType(rptVisibleDependingControls.Items(Counter).FindControl("lblID"), Label).Text))
                SetItemSelected(CType(CType(rptVisibleDependingControls.Items(Counter).FindControl("ucVisibilityTriggerValues"), UserControl).FindControl("ddlNumberTriggerValues"), DropDownList), CType(CType(rptVisibleDependingControls.Items(Counter).FindControl("ucVisibilityTriggerValues"), UserControl).FindControl("rptTriggerValues"), Repeater).Items.Count)
            Next
        End If
    End Sub

    Sub LoadPostbackActions(Optional ByVal bMore As Boolean = False)
        If ddlControlType.SelectedIndex > 0 Then
            Dim SuppliedDT As New WhitTools.DataTablesSupplied

            SuppliedDT.AddRow("ddlAction", "SQLSelect", "select * from " & DT_WEBRAD_CONTROLACTIONTYPES & " where ID = 7 OR ID in (select actiontype from " & DT_WEBRAD_CONTROLTYPEACTIONS & " where ActionControlDataType = " & GetControlDataType(ddlControlType.SelectedValue) & " and TargetControlDataType in (select datatype from " & DT_WEBRAD_CONTROLTYPES & " where ID in (select ControlType from " & DT_WEBRAD_PROJECTCONTROLS & " where ProjectID=" & GetProjectID() & " and Not ID = " & lblCurrentControlID.Text & ")))", "Type", "ID")
            SuppliedDT.AddRow("ddlUpdateRepeaterItemsValue", "FillNumbers", "", "0", "100")

            Dim PostbackActionsDT As DataTable = GetDataTable("Select * From " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & "  Where TriggerControl = " & lblCurrentControlID.Text, cnx)

            If PostbackActionsDT.Rows.Count = 0 And Not bMore Then
                EmptyRepeater(rptPostbackActions)
            Else
                Dim Temprow As DataRow

                If bMore Then
                    For nCounter = 0 To ddlActions.SelectedIndex - rptPostbackActions.Items.Count - 1
                        Temprow = PostbackActionsDT.NewRow
                        PostbackActionsDT.Rows.Add(Temprow)
                    Next
                End If
            End If

            SelectRepeaterData(rptPostbackActions, PostbackActionsDT, SuppliedDT)

            For nCounter = 0 To PostbackActionsDT.Rows.Count - 1
                Try
                    UpdateSelectedAction(CType(rptPostbackActions.Items(nCounter).FindControl("ddlAction"), DropDownList))
                    SetItemSelected(CType(rptPostbackActions.Items(nCounter).FindControl("ddlTargetControl"), DropDownList), PostbackActionsDT.Rows(nCounter).Item("TargetControl"))

                    Dim triggerValuesControl As TriggerValues = rptPostbackActions.Items(nCounter).FindControl("ucTriggerValues")

                    Dim dtTriggerValues As DataTable = GetDataTable("Select * from " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONTRIGGERVALUES & "  Where ActionID = " & PostbackActionsDT.Rows(nCounter).Item("ID"), cnx)

                    SetItemSelected(CType(triggerValuesControl.FindControl("ddlNumberTriggerValues"), DropDownList), dtTriggerValues.Rows.Count)
                    Dim currentListItems = TriggerValues.GetCurrentListItems(ucLSI)

                    Dim dtSupplied As New DataTablesSupplied()
                    dtSupplied.AddRow("ddlSelectTriggerValue", DataTablesSupplied.ActionTypes.LocalDataTable, currentListItems, "Text", "Value")

                    SelectRepeaterData(CType(CType(rptPostbackActions.Items(nCounter).FindControl("ucTriggerValues"), UserControl).FindControl("rptTriggerValues"), Repeater), dtTriggerValues, dtSupplied)
                    UpdateValueTypeSelection(rptPostbackActions.Items(nCounter).FindControl("ddlSetValueType"))

                    CType(rptPostbackActions.Items(nCounter).FindControl("ucSetValueDataSource"), WebRADDataSource).BindData(PostbackActionsDT.Rows(nCounter).Item("DataSourceID"))
                    ShowDataSource(CType(rptPostbackActions.Items(nCounter).FindControl("ucSetValueDataSource"), WebRADDataSource))
                Catch ex As Exception
                    WriteLine(ex.ToString)
                    'Logger.Error(ex.ToString)
                End Try
            Next

            SetItemSelected(ddlActions, PostbackActionsDT.Rows.Count)
        End If
    End Sub

    Sub CheckParentControlAvailable()
        Dim dt As New DataTable

        dt = GetDataTable(cnx, "Select Name + ' (' + Prefix + ')' as Name, C.ID From " & DT_WEBRAD_PROJECTCONTROLS & " C left outer join " & DT_WEBRAD_CONTROLTYPES & "  T on C.ControlType = T.ID left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  D on T.DataType = D.ID Where (PageID IS NULL OR PageID = " & GetQueryString("PageID") & ") AND ProjectID = " & Request.QueryString("ID") & " AND " & "ControlType in (Select ID From " & DT_WEBRAD_CONTROLTYPES & "  Where IsParent = 1) and not c.ID = " & lblCurrentControlID.Text)

        If dt.Rows.Count > 0 Then
            FillListData(ddlParentControl, dt, "Name", "ID")
            pnlParentControl.Visible = True
        Else
            pnlParentControl.Visible = False
        End If
    End Sub

    Sub CheckControlList()
        Dim dt As New DataTable

        CheckProjectParameters()

        dt = GetDataTable(cnx, "Select * From " & DT_WEBRAD_PROJECTCONTROLS & " Where ProjectID = '" & Request.QueryString("ID") & "' and PageID = " & Request.QueryString("PageID") & " order by Position asc")

        If dt.Rows.Count = 0 Then
            Dim cmd As New SqlCommand

            cmd.Connection = cnx
            cmd.CommandText = "usp_AddProjectControl"
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@ProjectID", Request.QueryString("ID"))
            cmd.Parameters.AddWithValue("@PageID", Request.QueryString("PageID"))

            LoadCurrentControl(WhitTools.SQL.ExecuteScalar(cmd, cnx))

            sds.SelectCommand = Replace(sds.SelectCommand, "@ProjectID", Request.QueryString("ID"))
            sds.SelectCommand = Replace(sds.SelectCommand, "@PageID", Request.QueryString("PageID"))
            sds.InsertCommand = Replace(sds.InsertCommand, "@ProjectID", Request.QueryString("ID"))
            sds.InsertCommand = Replace(sds.InsertCommand, "@PageID", Request.QueryString("PageID"))

            sds.Update()
            rl1.DataBind()
        Else
            LoadCurrentControl(dt.Rows(0).Item("ID"))
        End If
    End Sub

    Sub CheckControlDetails()
        If ddlControlType.SelectedIndex > 0 Then
            If IsCompositeControl(ddlControlType.SelectedValue) Then
                AddCompositeControls(GetProjectID(), GetCurrentPageID(), ddlControlType.SelectedValue, lblCurrentControlPosition.Text, lblCurrentControlID.Text)

                RefreshPage()
            Else
                pnlControlTypeDetails.Visible = True
                LoadControlTypeDetails(pnlControlTypeDetails, ddlControlType.SelectedItem.Value)
            End If
        Else
            pnlControlTypeDetails.Visible = False
        End If
    End Sub



    Sub LoadControlTypeDetails(ByRef rptpanel As Panel, ByVal controltype As Integer)
        Dim counter, counter2 As Integer
        Dim dt As New DataTable

        dt = GetDataTable(cnx, "Select * From " & DT_WEBRAD_CONTROLTYPEDETAILREQUIREMENTS & " R LEFT OUTER JOIN " & DT_WEBRAD_CONTROLTYPEDETAILTYPES & " T ON R.DetailTypeID = T.ID  Where ProfileID = (Select RequirementsProfile From " & DT_WEBRAD_CONTROLTYPEDETAILS & "  Where ControlID = '" & controltype & "')")

        For counter = 0 To rptpanel.Controls.Count - 1
            If TypeOf rptpanel.Controls(counter) Is Panel Then
                rptpanel.Controls(counter).Visible = False

                For counter2 = 0 To dt.Rows.Count - 1
                    If rptpanel.Controls(counter).ID = "pnl" & dt.Rows(counter2).Item("Name") Then
                        rptpanel.Controls(counter).Visible = True

                        If dt.Rows(counter2).Item("Required") = "1" Then
                            EnableControlRequired(rptpanel, counter, counter2, dt)
                        Else
                            DisableControlRequired(rptpanel, counter, counter2, dt)
                        End If
                    End If
                Next
            End If
        Next

        ucLSI.ShowPanels(dt)

        If ControlTypeIsRepeater(controltype) Then
            LoadRepeaterDetails()
        Else
            dt = New DataTable
            dt = GetDataTable(cnx, "Select D.*, NeedSize as NeedSQLSize From " & DT_WEBRAD_CONTROLTYPEDETAILS & "  D left outer join " & DT_WEBRAD_CONTROLSQLTYPES & "  S on D.SQLDataType = S.ID  Where ControlID = '" & controltype & "'")

            dt = ConvertDataTableColumnTypes(dt)

            If dt.Rows.Count > 0 Then
                With dt.Rows(0)
                    If CheckSavedStatus(controltype, CType(rptpanel.FindControl("lblCurrentControlID"), Label).Text) = 1 Or CheckSavedStatus(controltype, CType(rptpanel.FindControl("lblCurrentControlID"), Label).Text) = 2 Then
                        Clearcontrols(rptpanel)

                        If .Item("Name") <> "" Then
                            CType(rptpanel.FindControl("txtName"), TextBox).Text = .Item("Name")
                        End If

                        If .Item("value") <> "" Then
                            CType(rptpanel.FindControl("txtvalue"), TextBox).Text = .Item("value")
                        End If

                        If .Item("RichValue") <> "" Then
                            SetItemSelected(CType(rptpanel.FindControl("rblRichTextbox"), RadioButtonList), .Item("RichValue"))

                            If .Item("RichValue") = "1" Then
                                CType(rptpanel.FindControl("pnlRichValue"), Panel).Visible = True
                                CType(rptpanel.FindControl("pnlRegularValue"), Panel).Visible = False
                            End If
                        End If

                        SetItemSelected(CType(rptpanel.FindControl("rblDisplayHeading"), RadioButtonList), .Item("DisplayHeading"))

                        CType(rptpanel.FindControl("txtheading"), TextBox).Text = .Item("heading")
                        CType(rptpanel.FindControl("txtSubheading"), TextBox).Text = .Item("Subheading")

                        UpdateHeadingVisibility()

                        If .Item("maxlength") <> "" Then
                            CType(rptpanel.FindControl("txtmaxlength"), TextBox).Text = .Item("maxlength")
                        End If

                        If .Item("maxfilesize") <> "" Then
                            CType(rptpanel.FindControl("txtMaxFileSize"), TextBox).Text = .Item("MaxFileSize")
                        End If

                        If .Item("width") <> "" Then
                            CType(rptpanel.FindControl("txtwidth"), TextBox).Text = .Item("width")
                        End If

                        If .Item("cssclass") <> "" Then
                            CType(rptpanel.FindControl("txtcssclass"), TextBox).Text = .Item("cssclass")
                        End If

                        SetItemSelected(CType(rptpanel.FindControl("ddlTextMode"), DropDownList), .Item("TextMode"))
                        ddlTextMode_SelectedIndexChanged(Nothing, Nothing)

                        ucLSI.ShowDetails(dt, controltype)

                        If .Item("Required") <> "" Then
                            SetItemSelected(CType(rptpanel.FindControl("rblRequired"), RadioButtonList), .Item("Required"))
                        End If

                        If .Item("CustomValidation") <> "" Then
                            SetItemSelected(CType(rptpanel.FindControl("rblCustomValidation"), RadioButtonList), .Item("CustomValidation"))
                        End If

                        If .Item("Visible") <> "" Then
                            SetItemSelected(CType(rptpanel.FindControl("rblVisible"), RadioButtonList), .Item("Visible"))
                        End If

                        EmptyRepeater(rptVisibleDependingControls)
                        btnrptVisibleDependingControlsAddItem_Click(Nothing, Nothing)



                        If .Item("RequireVerification") <> "" Then
                            SetItemSelected(CType(rptpanel.FindControl("rblRequireVerification"), RadioButtonList), .Item("RequireVerification"))
                        End If

                        If .Item("RepeatDirection") <> "" Then
                            SetItemSelected(CType(rptpanel.FindControl("rblRepeatDirection"), RadioButtonList), .Item("RepeatDirection"))
                        End If

                        If .Item("RepeatColumns") <> "" Then
                            SetItemSelected(CType(rptpanel.FindControl("ddlRepeatColumns"), DropDownList), .Item("RepeatColumns"))
                        End If

                        If .Item("ListSelections") <> "" Then
                            SetItemSelected(CType(rptpanel.FindControl("rblListSelections"), RadioButtonList), .Item("ListSelections"))
                        End If

                        If .Item("SelectionMode") <> "" Then
                            SetItemSelected(CType(rptpanel.FindControl("rblSelectionMode"), RadioButtonList), .Item("SelectionMode"))
                        End If

                        If .Item("SQLDataType") <> "" Then
                            SetItemSelected(CType(rptpanel.FindControl("ddlSQLDataType"), DropDownList), .Item("SQLDataType"))
                        End If

                        SetListControlItemSelected(lsbFileTypesAllowed, "", True, GetDataTable("Select * From " & DT_WEBRAD_CONTROLTYPEFILETYPESALLOWED & "  Where ControlID = '" & controltype & "'", cnx), "FileType")
                        LoadDefaultFolders(lsbUploadPathFolders, lblSelectedUploadPathFolder)
                    End If

                    If NeedSQLSize(ddlSQLDataType) Then
                        CType(rptpanel.FindControl("pnlSQLDataSize"), Panel).Visible = True

                        If .Item("SQLDataSize") <> "-1" Then
                            CType(rptpanel.FindControl("txtSQLDataSize"), TextBox).Text = .Item("SQLDataSize")
                        End If
                    Else
                        CType(rptpanel.FindControl("pnlSQLDataSize"), Panel).Visible = False
                    End If

                    CType(rptpanel.FindControl("pnlAllowNegativeValue"), Panel).Visible = CType(rptpanel.FindControl("ddlSQLDataType"), DropDownList).SelectedIndex > 0 AndAlso DataTypeIsNumber(CType(rptpanel.FindControl("ddlSQLDataType"), DropDownList).SelectedValue)

                    If CType(rptpanel.FindControl("pnlSelectionMode"), Panel).Visible = True Then
                        If CType(rptpanel.FindControl("rblSelectionMode"), RadioButtonList).SelectedIndex > -1 Then
                            If CType(rptpanel.FindControl("rblSelectionMode"), RadioButtonList).SelectedItem.Value = "Multiple" Then
                                CType(rptpanel.FindControl("pnlSQLInsertItemStoredProcedure"), Panel).Visible = True
                                CType(rptpanel.FindControl("pnlSQLInsertItemTable"), Panel).Visible = True
                                CType(rptpanel.FindControl("pnlForeignID"), Panel).Visible = True
                            End If
                        End If
                    End If

                    ToggleIncludeDatabase()
                End With
            End If
        End If
    End Sub

    Private Shared Sub DisableControlRequired(rptpanel As Panel, counter As Integer, counter2 As Integer, dt As DataTable)
        Try
            CType(rptpanel.Controls(counter).FindControl("rfv" & dt.Rows(counter2).Item("Name")), RequiredFieldValidator).Enabled = False
        Catch ex As Exception

        End Try

        Try
            CType(rptpanel.Controls(counter).FindControl("pnl" & dt.Rows(counter2).Item("Name") & "Required"), Panel).Visible = False
        Catch ex As Exception

        End Try
    End Sub

    Private Shared Sub EnableControlRequired(rptpanel As Panel, counter As Integer, counter2 As Integer, dt As DataTable)
        Try
            CType(rptpanel.Controls(counter).FindControl("rfv" & dt.Rows(counter2).Item("Name")), RequiredFieldValidator).Enabled = True
        Catch ex As Exception

        End Try

        Try
            CType(rptpanel.Controls(counter).FindControl("pnl" & dt.Rows(counter2).Item("Name") & "Required"), Panel).Visible = True
        Catch ex As Exception

        End Try

        Try
            CType(rptpanel.Controls(counter).FindControl("lbl" & dt.Rows(counter2).Item("Name")), Label).CssClass = "required"
        Catch ex As Exception
            'If dt.Rows(counter2).Item("Name") = "Heading"
            '    Logger.Error(ex.ToString)
            'End If
        End Try
    End Sub

    Sub LoadRepeaterDetails()
        pnlDisplayHeading.Visible = False
        'pnlHeading.Visible = False
        pnlAutopostback.Visible = False
        pnlRequired.Visible = False
        rblVisible.SelectedIndex = 1
        SetItemSelected(rblIncludeDatabase, 1)
        ToggleIncludeDatabase()
    End Sub

    Sub SetRequired(ByRef detailpanel As Panel, ByVal detailstring As String, ByVal status As Boolean)
        CType(detailpanel.FindControl("pnl" & detailstring), Panel).Visible = status
        CType(detailpanel.FindControl("pnl" & detailstring & "Required"), Panel).Visible = status
        CType(detailpanel.FindControl("rfv" & detailstring), RequiredFieldValidator).Enabled = status
    End Sub


    Sub Clearcontrols(ByRef rptpanel As Panel)
        CType(rptpanel.FindControl("rblLayoutType"), RadioButtonList).SelectedIndex = 0
        CType(rptpanel.FindControl("rblLayoutSubtype"), RadioButtonList).SelectedIndex = 0
        CType(rptpanel.FindControl("rblDisplayLocation"), RadioButtonList).SelectedIndex = 0
        CType(rptpanel.FindControl("ddlDisplayType"), DropDownList).SelectedIndex = 0
        CType(rptpanel.FindControl("rblIncludeDatabase"), RadioButtonList).SelectedIndex = 1
        CType(rptpanel.FindControl("rblAutoPostback"), RadioButtonList).SelectedIndex = 0
        CType(rptpanel.FindControl("rblPerformPostbackAction"), RadioButtonList).SelectedIndex = 0
        CType(rptpanel.FindControl("ddlActions"), DropDownList).SelectedIndex = 0
        EmptyRepeater(rptpanel.FindControl("rptPostbackActions"))
        CType(rptpanel.FindControl("rblRequireVerification"), RadioButtonList).SelectedIndex = 0
        CType(rptpanel.FindControl("rblVisible"), RadioButtonList).SelectedIndex = 1
        CType(rptpanel.FindControl("txtCustomVisiblevalue"), TextBox).Text = ""
        CType(rptpanel.FindControl("rblCalendar"), RadioButtonList).SelectedIndex = -1
        CType(rptpanel.FindControl("rblRichTextbox"), RadioButtonList).SelectedIndex = 0
        CType(rptpanel.FindControl("txtvalue"), TextBox).Text = ""
        CType(rptpanel.FindControl("txtRichValue"), TextBox).Text = ""
        CType(rptpanel.FindControl("txtmaxlength"), TextBox).Text = ""
        CType(rptpanel.FindControl("txtMaxFileSize"), TextBox).Text = ""
        CType(rptpanel.FindControl("txtwidth"), TextBox).Text = ""
        CType(rptpanel.FindControl("txtcssclass"), TextBox).Text = ""
        CType(rptpanel.FindControl("rblRepeatDirection"), RadioButtonList).SelectedIndex = -1
        CType(rptpanel.FindControl("ddlRepeatColumns"), DropDownList).SelectedIndex = 0
        CType(rptpanel.FindControl("rblListSelections"), RadioButtonList).SelectedIndex = -1
        CType(rptpanel.FindControl("txtGroupName"), TextBox).Text = ""
        CType(rptpanel.FindControl("rblSelectionMode"), RadioButtonList).SelectedIndex = -1
        CType(rptpanel.FindControl("lsbFileTypesAllowed"), ListBox).SelectedIndex = -1
        CType(rptpanel.FindControl("lblSelectedUploadPathFolder"), Label).Text = ""

        ucLSI.ClearControls()

        Try
            CType(rptpanel.FindControl("ddlSQLDataType"), DropDownList).SelectedIndex = 0
        Catch ex As Exception

        End Try

        CType(rptpanel.FindControl("txtSQLDataSize"), TextBox).Text = ""
        CType(rptpanel.FindControl("rblSQLDefaultValueType"), RadioButtonList).SelectedIndex = 0
        CType(rptpanel.FindControl("txtSQLDefaultValue"), TextBox).Text = ""
    End Sub

    Shared dtSupplied As New DataTable

    Protected Sub ddlControlType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlControlType.SelectedIndexChanged
        CheckControlDetails()
    End Sub

    Protected Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnContinue.Click, btnContinue2.Click
        Page.Validate()

        If Page.IsValid Then
            SaveControlInfo()

            Redirect("controls.aspx?ID=" & Request.QueryString("ID") & "&ControlID=" & GetNextID() & "&PageID=" & Request.QueryString("PageID"))
        End If
    End Sub

    Sub SaveControlInfo()
        Dim cmd As New SqlCommand
        cmd.Connection = cnx
        cmd.CommandText = "usp_AddProjectControl"
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@ID", lblCurrentControlID.Text)
        cmd.Parameters.AddWithValue("@ProjectID", Request.QueryString("ID"))

        If pnlParentControl.Visible = True And ddlParentControl.SelectedIndex > 0 Then
            cmd.Parameters.AddWithValue("@ParentControlID", ddlParentControl.SelectedItem.Value)
        End If

        cmd.Parameters.AddWithValue("@ControlType", ddlControlType.SelectedItem.Value)
        cmd.Parameters.AddWithValue("@Name", Replace(Replace(txtName.Text, "'", ""), " ", ""))

        If pnlHeading.Visible = True Then
            cmd.Parameters.AddWithValue("@Heading", txtHeading.Text)
        End If

        If pnlSubheading.Visible = True Then
            cmd.Parameters.AddWithValue("@Subheading", txtSubHeading.Text)
        End If

        If pnlLayoutType.Visible = True Then
            If rblLayoutType.SelectedIndex > -1 Then
                cmd.Parameters.AddWithValue("@LayoutType", rblLayoutType.SelectedItem.Value)
                cmd.Parameters.AddWithValue("@LayoutSubtype", rblLayoutSubtype.SelectedValue)
            End If
        End If

        If pnlRepeaterAddRemove.Visible Then
            cmd.Parameters.AddWithValue("@RepeaterAddRemove", rblRepeaterAddRemove.SelectedValue)
        End If

        If pnlRepeaterItemName.Visible Then
            cmd.Parameters.AddWithValue("@RepeaterItemName", txtRepeaterItemName.Text)
        End If

        If pnlSupplyControlData.Visible = True Then
            If rblSupplyControlData.SelectedIndex > -1 Then
                cmd.Parameters.AddWithValue("@SupplyControlData", rblSupplyControlData.SelectedItem.Value)
            End If
        End If

        If pnlSupplyDataType.Visible = True Then
            cmd.Parameters.AddWithValue("@SupplyDataType", rblSupplyDataType.SelectedItem.Value)
        End If

        If pnlControlDataSource.Visible = True Then
            cmd.Parameters.AddWithValue("@DataSourceID", ucControlSupplyDataSource.SaveData())
        End If

        If pnlControlDataSourceColumn.Visible Then
            cmd.Parameters.AddWithValue("@DataSourceColumn", txtControlDataSourceColumn.Text)
        End If

        If pnlDisplayHeading.Visible = True Then
            cmd.Parameters.AddWithValue("@DisplayHeading", rblDisplayHeading.SelectedValue)
        End If

        cmd.Parameters.AddWithValue("@DisplayLocation", rblDisplayLocation.SelectedItem.Value)
        cmd.Parameters.AddWithValue("@DisplayType", ddlDisplayType.SelectedItem.Value)

        If pnlIncludeDatabase.Visible = True Then
            If rblIncludeDatabase.SelectedIndex > -1 Then
                cmd.Parameters.AddWithValue("@IncludeDatabase", rblIncludeDatabase.SelectedItem.Value)
            End If
        End If

        If pnlAutopostback.Visible = True Then
            If rblAutopostback.SelectedIndex > -1 Then
                cmd.Parameters.AddWithValue("@Autopostback", rblAutopostback.SelectedItem.Value)
            End If
        End If

        If pnlPerformPostbackAction.Visible = True Then
            If rblPerformPostbackAction.SelectedIndex > -1 Then
                cmd.Parameters.AddWithValue("@PerformPostbackAction", rblPerformPostbackAction.SelectedItem.Value)
            End If
        End If

        If pnlOnchange.Visible Then
            cmd.Parameters.AddWithValue("@Onchange", rblOnchange.SelectedValue)

            If rblOnchange.SelectedIndex = 1 Then
                cmd.Parameters.AddWithValue("@OnchangeCall", txtOnchangeCall.Text)
                cmd.Parameters.AddWithValue("@OnchangeBody", txtOnchangeBody.Text)
            End If
        End If

        If pnlEnabled.Visible = True Then
            cmd.Parameters.AddWithValue("@Enabled", rblEnabled.SelectedItem.Value)
        End If

        If pnlVisible.Visible = True Then
            cmd.Parameters.AddWithValue("@Visible", rblVisible.SelectedItem.Value)

            SaveVisibleDependentValue()
        End If

        If pnlCustomVisibleValue.Visible = True Then
            cmd.Parameters.AddWithValue("@CustomVisibleValue", txtCustomVisibleValue.Text)
        End If

        If pnlRequired.Visible = True Then
            cmd.Parameters.AddWithValue("@Required", rblRequired.SelectedItem.Value)
        End If

        If pnlMinimumRequired.Visible Then
            cmd.Parameters.AddWithValue("@MinimumRequired", ddlMinimumRequired.SelectedItem.Value)
            cmd.Parameters.AddWithValue("@MaximumRequired", ddlMaximumRequired.SelectedItem.Value)
        End If

        If pnlCustomValidation.Visible = True Then
            cmd.Parameters.AddWithValue("@CustomValidation", IIf(rblCustomValidation.SelectedIndex > 0, rblCustomValidation.SelectedValue, "0"))
        End If

        If pnlCustomValidationCode.Visible Then
            cmd.Parameters.AddWithValue("@CustomValidationCode", txtCustomValidationCode.Text)
        End If

        If pnlRequireVerification.Visible = True Then
            cmd.Parameters.AddWithValue("@RequireVerification", rblRequireVerification.SelectedItem.Value)
        End If

        If pnlShortHeading.Visible = True Then
            cmd.Parameters.AddWithValue("@ShortHeading", txtShortHeading.Text)
        End If

        If pnlTextPosition.Visible = True Then
            cmd.Parameters.AddWithValue("@TextPosition", rblTextPosition.SelectedValue)
        End If


        If pnlCalendar.Visible Then
            cmd.Parameters.AddWithValue("@Calendar", rblCalendar.SelectedValue)
        End If

        If pnlValue.Visible = True Then
            cmd.Parameters.AddWithValue("@RichValue", rblRichTextbox.SelectedValue)
            cmd.Parameters.AddWithValue("@Value", If(rblRichTextbox.SelectedIndex = 1, txtRichValue.Text, txtValue.Text))
        End If

        If pnlText.Visible = True Then
            'cmd.Parameters.AddWithValue("@Text", RemoveRichFormatting(txtText.Text))
            cmd.Parameters.AddWithValue("@Text", txtText.Text)
        End If

        If pnlPlaceholder.Visible = True Then
            cmd.Parameters.AddWithValue("@Placeholder", txtPlaceholder.Text)
        End If

        If pnlTextMode.Visible = True Then
            cmd.Parameters.AddWithValue("@TextMode", ddlTextMode.SelectedItem.Value)
        End If

        If pnlRichTextboxUser.Visible Then
            cmd.Parameters.AddWithValue("@RichTextUser", rblRichTextboxUser.SelectedValue)
        End If

        If pnlRows.Visible = True Then
            cmd.Parameters.AddWithValue("@Rows", txtRows.Text)
        End If

        If pnlColumns.Visible = True Then
            cmd.Parameters.AddWithValue("@Columns", txtColumns.Text)
        End If

        If pnlMaxLength.Visible = True Then
            cmd.Parameters.AddWithValue("@MaxLength", txtMaxLength.Text)
            cmd.Parameters.AddWithValue("@MaxLengthType", rblMaxLengthType.SelectedValue)
        End If

        If pnlMaxFileSize.Visible = True Then
            cmd.Parameters.AddWithValue("@MaxFileSize", txtMaxFileSize.Text)
        End If

        If pnlWidth.Visible = True Then
            cmd.Parameters.AddWithValue("@Width", txtWidth.Text)
        End If

        If pnlCssClass.Visible = True Then
            cmd.Parameters.AddWithValue("@CSSClass", txtCssClass.Text)
        End If

        If pnlRepeatDirection.Visible = True Then
            cmd.Parameters.AddWithValue("@RepeatDirection", rblRepeatDirection.SelectedItem.Value)
        End If

        If pnlRepeatColumns.Visible = True Then
            cmd.Parameters.AddWithValue("@RepeatColumns", ddlRepeatColumns.SelectedItem.Value)
        End If

        If pnlListSelections.Visible = True Then
            cmd.Parameters.AddWithValue("@ListSelections", rblListSelections.SelectedItem.Value)
            '      AddListSelectionsAction()
        Else
            '     RemoveListSelectionsAction()
        End If

        If pnlGroupName.Visible = True Then
            cmd.Parameters.AddWithValue("@GroupName", txtGroupName.Text)
        End If

        If pnlSelectionMode.Visible = True Then
            cmd.Parameters.AddWithValue("@SelectionMode", rblSelectionMode.SelectedItem.Value)
        End If

        cmd.Parameters.AddWithValue("@UploadPath", lblSelectedUploadPathFolder.Text)

        If pnlNewUploadPathFolder.Visible Then
            cmd.Parameters.AddWithValue("@UploadPathNewFolder", txtNewUploadPathFolderName.Text)
        End If

        ucLSI.LoadSQLParameters(cmd)

        cmd.Parameters.AddWithValue("@SQLDataType", ddlSQLDataType.SelectedItem.Value)
        cmd.Parameters.AddWithValue("@SQLDatasize", txtSQLDataSize.Text)

        If rblSQLDefaultValueType.SelectedIndex > -1 Then
            cmd.Parameters.AddWithValue("@SQLDefaultValueType", rblSQLDefaultValueType.SelectedItem.Value)
        End If

        cmd.Parameters.AddWithValue("@SQLDefaultValue", txtSQLDefaultValue.Text)
        cmd.Parameters.AddWithValue("@AllowNegativeValue", rblAllowNegativeValue.SelectedValue)

        If pnlSQLInsertItemTable.Visible = True Then
            cmd.Parameters.AddWithValue("@SQLInsertItemTable", txtSQLInsertItemTable.Text)
        End If

        If pnlForeignID.Visible Then
            cmd.Parameters.AddWithValue("@ForeignID", txtForeignID.Text)
        End If

        If pnlSQLInsertItemStoredProcedure.Visible = True Then
            cmd.Parameters.AddWithValue("@SQLInsertItemStoredProcedure", txtSQLInsertItemStoredProcedure.Text)
        End If

        If pnlProspectCode.Visible Then
            cmd.Parameters.AddWithValue("@ProspectCode", txtProspectCode.Text)
        End If

        Dim nCurrentID As Integer

        Try
            cnx.Close()
        Catch ex As Exception

        End Try

        nCurrentID = WhitTools.SQL.ExecuteScalar(cmd, cnx)

        ucLSI.SaveListItems(1, nCurrentID)
        SaveFileTypesAllowed(nCurrentID)
        SavePostbackActions()
        FixControlPositioning()
    End Sub


    Sub SavePostbackActions(Optional ByVal bFewer As Boolean = False)
        WhitTools.SQL.ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & "  Where TriggerControl = " & lblCurrentControlID.Text, cnx)

        For Each CurrentItem As RepeaterItem In rptPostbackActions.Items
            SavePostbackAction(CurrentItem, bFewer)
        Next
    End Sub

    Private Sub SavePostbackAction(ByRef ContainerItem As Object, ByVal bFewer As Boolean)
        With ContainerItem
            Dim cmd As New SqlCommand("usp_InsertProjectControlPostbackAction", cnx)
            Dim cmd2 As New SqlCommand("usp_InsertProjectControlPostbackActionTriggerValue", cnx)
            Dim nCurrentID As Integer

            cmd.CommandType = CommandType.StoredProcedure
            cmd2.CommandType = CommandType.StoredProcedure

            cmd2.Parameters.AddWithValue("@ActionID", "")
            cmd2.Parameters.AddWithValue("@TriggerValue", "")
            cmd2.Parameters.AddWithValue("@TriggerOperator", "")

            If bFewer AndAlso ContainerItem.ItemIndex > ddlActions.SelectedIndex - 1 Then
                Return
            End If

            Dim ucListItems As WebRADListItems = .FindControl("ucActionLSI")
            Dim ucDataSource As WebRADDataSource = .FindControl("ucSetValueDataSource")

            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@TriggerControl", lblCurrentControlID.Text)
            cmd.Parameters.AddWithValue("@TargetControl", "")
            cmd.Parameters.AddWithValue("@Action", "")
            cmd.Parameters.AddWithValue("@CustomActionCode", "")
            cmd.Parameters.AddWithValue("@UpdateRepeaterItemsSelectionType", "")
            cmd.Parameters.AddWithValue("@UpdateRepeaterItemsValue", "")
            cmd.Parameters.AddWithValue("@SetValueType", "")
            cmd.Parameters.AddWithValue("@ExplicitValue", "")
            cmd.Parameters.AddWithValue("@ControlValue", "")

            cmd.Parameters("@TargetControl").Value = CType(.FindControl("ddlTargetControl"), DropDownList).SelectedValue
            cmd.Parameters("@Action").Value = CType(.FindControl("ddlAction"), DropDownList).SelectedValue

            If ActionRequiresListItems(CType(.FindControl("ddlAction"), DropDownList).SelectedValue) Then
                ucListItems.LoadSQLParameters(cmd)
            ElseIf ActionRequiresValueSelection(CType(.FindControl("ddlAction"), DropDownList).SelectedValue) Then
                cmd.Parameters("@SetValueType").Value = CType(.FindControl("ddlSetValueType"), DropDownList).SelectedValue
                cmd.Parameters("@ExplicitValue").Value = CType(.FindControl("txtExplicitValue"), TextBox).Text
                cmd.Parameters("@ControlValue").Value = CType(.FindControl("ddlControlValue"), DropDownList).SelectedValue
                cmd.Parameters.AddWithValue("@DataSourceID", ucDataSource.SaveData(N_ACTION_DATASOURCEPARENTTYPE))
            End If

            If CType(.FindControl("pnlUpdateRepeaterItemsSelectionType"), Panel).Visible And CType(.FindControl("rblUpdateRepeaterItemsSelectionType"), RadioButtonList).SelectedValue <> "" Then
                cmd.Parameters("@UpdateRepeaterItemsSelectionType").Value = CType(.FindControl("rblUpdateRepeaterItemsSelectionType"), RadioButtonList).SelectedValue
            End If

            If CType(.FindControl("pnlCustomActionCode"), Panel).Visible Then
                cmd.Parameters("@CustomActionCode").Value = CType(.FindControl("txtCustomActionCode"), TextBox).Text
            End If

            If CType(.FindControl("pnlUpdateRepeaterItemsValue"), Panel).Visible And CType(.FindControl("ddlUpdateRepeaterItemsValue"), DropDownList).SelectedValue <> "" Then
                cmd.Parameters("@UpdateRepeaterItemsValue").Value = CType(.FindControl("ddlUpdateRepeaterItemsValue"), DropDownList).SelectedValue
            End If

            Try
                nCurrentID = WhitTools.SQL.ExecuteScalar(cmd)
            Catch ex As Exception
                For Each Blah As SqlParameter In cmd.Parameters
                    'WriteLine(Blah.ParameterName)
                Next
            End Try


            If ActionRequiresListItems(CType(.FindControl("ddlAction"), DropDownList).SelectedValue) Then
                ucListItems.SaveListItems(2, nCurrentID)
            End If

            cmd2.Parameters("@ActionID").Value = nCurrentID

            For Each CurrentItem2 As RepeaterItem In CType(CType(.FindControl("ucTriggerValues"), UserControl).FindControl("rptTriggerValues"), Repeater).Items
                cmd2.Parameters("@TriggerValue").Value = CType(CurrentItem2.FindControl("txtTriggerValue"), TextBox).Text
                cmd2.Parameters("@TriggerOperator").Value = CType(CurrentItem2.FindControl("ddlTriggerOperator"), DropDownList).SelectedValue

                WhitTools.SQL.ExecuteNonQuery(cmd2)
            Next
        End With
    End Sub

    Function RemoveRichFormatting(ByVal sText As String)
        Return Replace(Replace(Replace(sText, """", "'"), "<p>", ""), "</p>", "")
    End Function

    Sub SaveFileTypesAllowed(ByVal nCurrentID As Integer)
        Dim nCounter As Integer
        Dim cmd As New SqlCommand

        WhitTools.SQL.ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED & "  Where ControlID = " & nCurrentID, cnx)

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_InsertProjectControlFileTypeAllowed"
        cmd.Connection = cnx

        cmd.Parameters.AddWithValue("@ControlID", nCurrentID)
        cmd.Parameters.AddWithValue("@FileType", "")

        If pnlFileTypesAllowed.Visible = True Then
            For Each CurrentItem As ListItem In lsbFileTypesAllowed.Items
                If CurrentItem.Selected Then
                    cmd.Parameters("@FileType").Value = CurrentItem.Value

                    WhitTools.SQL.ExecuteNonQuery(cmd)
                End If
            Next
        End If
    End Sub

    Function GetNextID()
        Dim nCounter As Integer

        For nCounter = 0 To rl1.Items.Count - 1
            Try
                If CType(rl1.Items(nCounter).FindControl("lblID"), Label).Text = lblCurrentControlID.Text Then
                    If nCounter = rl1.Items.Count - 2 Then
                        If AllControlsComplete() Then
                            General.Pages.CheckLastPage(-1)
                        End If
                    Else
                        Return CType(rl1.Items(nCounter + 1).FindControl("lblID"), Label).Text
                    End If
                End If
            Catch ex As Exception
                Response.Write(nCounter)
            End Try
        Next
    End Function


    'Sub LoadParameter(ByRef cmd As SqlCommand, ByVal sTarget As String)
    '    Try
    '        If CType(Page.FindControl("pnl" & sTarget), Panel).Visible = True Then
    '            Try

    '            Catch ex As Exception

    '            End Try
    '            cmd.Parameters.AddWithValue("@Required", rblRequired.SelectedItem.Value)
    '        End If
    '    End Try
    'End Sub


    Protected Sub ddlSQLDataType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSQLDataType.SelectedIndexChanged
        pnlSQLDataSize.Visible = NeedSQLSize(ddlSQLDataType)

        If ddlSQLDataType.SelectedIndex > 0 Then
            pnlAllowNegativeValue.Visible = DataTypeIsNumber(ddlSQLDataType.SelectedValue)
        End If
    End Sub

    Protected Sub cvName_ServerValidate(source As Object, args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvName.ServerValidate
        Dim parentRepeaterControlID As String = ""
        Dim projectID As Integer = GetProjectID()

        If ddlParentControl.SelectedIndex > 0 Then
            ParentIsRepeaterControl(lblCurrentControlID.Text, "-1", 0, parentRepeaterControlID)
            WriteLine("parent repeater id - " & parentRepeaterControlID)
            WriteLine(ControlTypeIsRepeater(ddlParentControl.SelectedValue))


            If parentRepeaterControlID = "" And ControlTypeIsRepeater(GetControlColumnValue(ddlParentControl.SelectedValue, "ControlType")) Then
                parentRepeaterControlID = ddlParentControl.SelectedValue
            End If


            WriteLine("parent repeater id - " & parentRepeaterControlID)

            If parentRepeaterControlID <> "" Then
                args.IsValid = Not db.ProjectControls.First(Function(pc) pc.ID = parentRepeaterControlID).ProjectControls1.Any(Function(childPC) childPC.ID <> lblCurrentControlID.Text And childPC.Name = txtName.Text)
            Else
                CheckParentlessNameDuplicate(args)
            End If
            'AndAlso ParentIsRepeaterControl(lblCurrentControlID.Text, "-1", 0, parentRepeaterControlID) Then

        Else
            CheckParentlessNameDuplicate(args)
        End If
    End Sub

    Private Sub CheckParentlessNameDuplicate(ByRef args As ServerValidateEventArgs)
        Dim nMaxControlCount As Integer = If(GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ID = " & lblCurrentControlID.Text & " AND ControlType IS NULL").Rows.Count > 0, 0, 1)

        If GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " AND Name = '" & CleanSQL(txtName.Text) & "'").Rows.Count > nMaxControlCount Then
            args.IsValid = False
        End If
    End Sub

    Protected Sub ddlTextMode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlTextMode.SelectedIndexChanged
        CheckTextModeOptions()
    End Sub

    Sub CheckTextModeOptions()
        If ddlControlType.SelectedIndex > 0 Then
            pnlRows.Visible = If(ddlTextMode.SelectedItem.Value = "MultiLine" Or ddlControlType.SelectedValue = N_LISTBOX_CONTROL_TYPE, True, False)
            pnlColumns.Visible = pnlRows.Visible

            If pnlTextMode.Visible Then

                If ddlTextMode.SelectedValue = "SingleLine" Then
                    txtCssClass.Text = "SlText"
                    pnlRichTextboxUser.Visible = False
                ElseIf ddlTextMode.SelectedValue = "MultiLine" Then
                    txtCssClass.Text = "MlText"
                    txtRows.Text = "5"
                    txtColumns.Text = "80"
                    txtSQLDataSize.Text = "MAX"
                    pnlRichTextboxUser.Visible = True
                End If
            End If
        End If
    End Sub

    Protected Sub rblAutoPostback_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblAutopostback.SelectedIndexChanged
        CheckPostbackStatus()
    End Sub

    Sub CheckPostbackStatus()
        pnlPostbackActionOptions.Visible = rblAutopostback.SelectedItem.Value = 1 Or rblRepeaterAddRemove.SelectedIndex = 1
    End Sub

    Protected Sub rblDisplayHeading_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblDisplayHeading.SelectedIndexChanged
        UpdateHeadingVisibility()
    End Sub

    Sub UpdateHeadingVisibility()
        If pnlDisplayHeading.Visible = True Then
            pnlHeading.Visible = If(rblDisplayHeading.SelectedIndex = 1, True, False)
        End If
    End Sub

    Protected Sub ddlActions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlActions.SelectedIndexChanged
        Dim nItems As Integer = ddlActions.SelectedIndex

        If nItems < rptPostbackActions.Items.Count Then
            SavePostbackActions(True)
            LoadPostbackActions()
        Else
            SavePostbackActions()
            LoadPostbackActions(True)
        End If
    End Sub

    Sub CheckActionListItems()
        For Each CurrentItem As RepeaterItem In rptPostbackActions.Items
            With CurrentItem
                Dim listItems As WebRADListItems = .FindControl("ucActionLSI")

                Try
                    If ActionRequiresListItems(CType(.FindControl("ddlAction"), DropDownList).SelectedValue) Then
                        SetDataMethodTypes(listItems)
                        SetListItemNumbers(listItems)
                        CType(listItems.FindControl("pnlSelectionItems"), Panel).Visible = True
                    Else
                        HideListItemPanels(listItems)
                    End If
                Catch ex As Exception
                    ' 'Logger.Error(ex.ToString)
                End Try

                Try
                    Dim dt As DataTable = GetDataTable("Select * From " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & "  WHERE ID = '" & CType(.FindControl("lblID"), Label).Text & "'", cnx)
                    listItems.BindData(dt)

                    listItems.LoadListItems(2, CType(.FindControl("lblID"), Label).Text)
                    listItems.CheckSelectionItems()
                    listItems.ShowDetails(dt, 2)
                    listItems.ShowPanels(GetDataTable("Select * From " & DT_WEBRAD_CONTROLTYPEDETAILREQUIREMENTS & "  Where ProfileID = (Select RequirementsProfile From " & DT_WEBRAD_CONTROLTYPEDETAILS & "  Where ControlID = '" & ddlControlType.SelectedValue & "')", cnx))
                Catch ex As Exception
                End Try

                'blah2.CheckDataMethodOptions()
            End With
        Next
    End Sub

    Private Sub HideListItemPanels(listItems As WebRADListItems)
        For Each CurrentControl As Control In listItems.Controls
            If TypeOf CurrentControl Is Panel Then
                CurrentControl.Visible = False
            End If
        Next

        HideDataSource(listItems.FindControl("ucDataSource"))
    End Sub

    Private Sub SetListItemNumbers(listItems As WebRADListItems)

        FillNumbers(CType(listItems.FindControl("ddlListItems"), DropDownList), 0, 100)
    End Sub

    Private Sub SetDataMethodTypes(listItems As WebRADListItems)

        FillListData(CType(listItems.FindControl("ddlDataMethod"), DropDownList), GetDataTable("Select * from " & DT_WEBRAD_DATAMETHODTYPES & " ", cnx), "Name", "ID")
    End Sub


    Protected Sub rblRichTextbox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblRichTextbox.SelectedIndexChanged
        UpdateRichTextbox()
    End Sub

    Sub UpdateRichTextbox()
        If rblRichTextbox.SelectedIndex = 0 Then
            If txtValue.Text = "" And txtRichValue.Text <> "" Then
                txtValue.Text = txtRichValue.Text
            End If

            pnlRichValue.Visible = False
            pnlRegularValue.Visible = True
        Else
            If txtValue.Text <> "" And txtRichValue.Text = "" Then
                txtRichValue.Text = txtValue.Text
            End If

            pnlRichValue.Visible = True
            pnlRegularValue.Visible = False
        End If
    End Sub


    Protected Sub rblPerformPostbackAction_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblPerformPostbackAction.SelectedIndexChanged
        CheckPostbackActions()
    End Sub

    Sub CheckPostbackActions()
        pnlPostbackActions.Visible = rblPerformPostbackAction.SelectedIndex = 1
    End Sub

    Protected Sub cvFileTypesAllowed_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvFileTypesAllowed.ServerValidate
        args.IsValid = ValidateListControl(lsbFileTypesAllowed, cvFileTypesAllowed, "", args)
    End Sub

    Protected Sub cvSQLDataSize_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvSQLDataSize.ServerValidate
        If txtSQLDataSize.Text = "MAX" Then
            args.IsValid = True
        Else
            Dim nTemp As Integer

            Try
                nTemp = txtSQLDataSize.Text
                args.IsValid = True
            Catch ex As Exception
                args.IsValid = False
            End Try
        End If
    End Sub

    Protected Sub rblDisplayLocation_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblDisplayLocation.SelectedIndexChanged
        ToggleSQLDefaultValue()
    End Sub

    Sub ToggleSQLDefaultValue()
        If ddlControlType.SelectedIndex > 0 Then
            pnlSQLDefaultValue.Visible = If(rblDisplayLocation.SelectedIndex = 2 Or GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLTYPEDETAILREQUIREMENTS & " WHERE ProfileID IN (SELECT RequirementsProfile FROM " & DT_WEBRAD_CONTROLTYPEDETAILS & " WHERE ControlID = " & ddlControlType.SelectedValue & ") AND DetailTypeID = " & N_SQLDEFAULTVALUE_DETAILTYPEID).Rows.Count > 0, True, False)
        End If
    End Sub

    Protected Sub ddlAction_SelectedIndexChanged(sender As Object, e As EventArgs)
        UpdateSelectedAction(sender)
    End Sub

    Sub UpdateSelectedAction(sender As Object)
        With CType(sender.parent, RepeaterItem)
            HideAllActionOptions(sender.parent)

            If CType(.FindControl("ddlAction"), DropDownList).SelectedIndex > 0 Then

                If CType(.FindControl("ddlAction"), DropDownList).SelectedValue = "7" Then
                    CType(.FindControl("pnlCustomActionCode"), Panel).Visible = True
                Else
                    CType(.FindControl("pnlPostbackActionTargetControl"), Panel).Visible = True

                    FillListData(CType(.FindControl("ddlTargetControl"), DropDownList), GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " AND NOT ID = " & lblCurrentControlID.Text & " AND ControlType IN (SELECT ID FROM " & DT_WEBRAD_CONTROLTYPES & " WHERE DataType IN (Select TargetControlDataType FROM " & DT_WEBRAD_CONTROLTYPEACTIONS & " WHERE ActionControlDataType = " & GetControlDataType(ddlControlType.SelectedValue) & " and ActionType = " & CType(.FindControl("ddlAction"), DropDownList).SelectedValue & "))"), "Name", "ID", True)

                    If CType(.FindControl("ddlAction"), DropDownList).SelectedValue = N_UPDATEREPEATERITEMS_ACTIONTYPE Then
                        SetUpdateRepeaterActionOptions(sender.parent)
                    End If

                    If ActionRequiresListItems(CType(.FindControl("ddlAction"), DropDownList).SelectedValue) Then
                        CType(CType(.FindControl("ucActionLSI"), UserControl).FindControl("pnlSelectionItems"), Panel).Visible = True
                    ElseIf ActionRequiresValueSelection(CType(.FindControl("ddlAction"), DropDownList).SelectedValue) Then
                        ShowActionSetValueOptions(sender.parent)
                    Else
                        HideActionPanels(sender.parent)
                    End If
                End If
            End If
        End With
    End Sub

    Sub HideActionPanels(ByRef CurrentItem As RepeaterItem)
        With CurrentItem
            For Each CurrentControl As Control In CType(.FindControl("ucActionLSI"), UserControl).Controls
                If TypeOf CurrentControl Is Panel Then
                    CurrentControl.Visible = False
                End If
            Next
        End With
    End Sub

    Sub ShowActionSetValueOptions(ByRef CurrentItem As RepeaterItem)
        With CurrentItem
            CType(.FindControl("pnlSetValueOptions"), Panel).Visible = True

            ddlSetValueType_SelectedIndexChanged(CType(.FindControl("ddlSetValueType"), DropDownList), Nothing)
        End With
    End Sub
    Sub HideAllActionOptions(ByRef CurrentItem As RepeaterItem)
        With CurrentItem
            CType(.FindControl("pnlSetValueOptions"), Panel).Visible = False
            CType(.FindControl("pnlUpdateRepeaterItemsSelectionType"), Panel).Visible = False
            CType(.FindControl("pnlUpdateRepeaterItemsValue"), Panel).Visible = False
            CType(.FindControl("pnlCustomActionCode"), Panel).Visible = False
            CType(.FindControl("pnlPostbackActionTargetControl"), Panel).Visible = False
        End With
    End Sub

    Sub SetUpdateRepeaterActionOptions(ByRef CurrentItem As RepeaterItem)
        With CurrentItem
            If IsListControlType(CType(.Parent.Parent.FindControl("ddlControlType"), DropDownList).SelectedValue) Then
                CType(.FindControl("pnlUpdateRepeaterItemsSelectionType"), Panel).Visible = True

                If CType(.FindControl("rblUpdateRepeaterItemsSelectionType"), RadioButtonList).SelectedIndex = 1 Then
                    CType(.FindControl("pnlUpdateRepeaterItemsValue"), Panel).Visible = True
                End If
            Else
                CType(.FindControl("pnlUpdateRepeaterItemsValue"), Panel).Visible = True
            End If

        End With
    End Sub

    Protected Sub rblLayoutType_SelectedIndexChanged(sender As Object, e As EventArgs)
        ToggleLayoutType()
    End Sub

    Sub ToggleLayoutType()
        FillListData(rblLayoutSubtype, GetDataTable("Select * From " & DT_WEBRAD_LAYOUTSUBTYPES & " WHERE Type = " & rblLayoutType.SelectedValue & "", cnx), "Description", "ID", False)

        pnlRepeatColumns.Visible = If(rblLayoutType.SelectedIndex = 1 Or IsListControlType(If(ddlControlType.SelectedIndex > 0, ddlControlType.SelectedValue, 0)), True, False)
        'pnlRepeatColumnsRequired.Visible = False
    End Sub

    Protected Sub rblSupplycontroldata_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSupplyControlData.SelectedIndexChanged
        Try
            pnlSupplyDataType.Visible = (rblSupplyControlData.SelectedIndex = 1)
        Catch ex As Exception

        End Try

        rblSupplyDataType_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Protected Sub rblSupplyDataType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSupplyDataType.SelectedIndexChanged
        ToggleControlDataSource()
        ToggleControlDataSourceColumn()
    End Sub

    Private Sub ToggleControlDataSourceColumn()
        pnlControlDataSourceColumn.Visible = ControlDataSourceColumnRequired()
    End Sub

    Private Function ControlDataSourceColumnRequired() As Boolean
        Return pnlControlDataSource.Visible AndAlso ddlControlType.SelectedIndex > 0 AndAlso Not IsControlType(ddlControlType.SelectedValue, "Repeater")
    End Function

    Private Sub ToggleControlDataSource()
        pnlControlDataSource.Visible = rblSupplyControlData.SelectedIndex = 1

        If pnlControlDataSource.Visible Then
            If rblSupplyDataType.SelectedValue = "Autocomplete" Then
                CType(ucControlSupplyDataSource.FindControl("rblDataSourceType"), RadioButtonList).SelectedIndex = 0
                ucControlSupplyDataSource.rblDataSourceType_SelectedIndexChanged(CType(ucControlSupplyDataSource.FindControl("rblDataSourceType"), RadioButtonList), Nothing)
                CType(ucControlSupplyDataSource.FindControl("pnlDataSourceType"), Panel).Visible = False
            Else
                CType(ucControlSupplyDataSource.FindControl("pnlDataSourceType"), Panel).Visible = True
            End If
        End If
    End Sub

    Protected Sub imbAddPage_Click(sender As Object, e As ImageClickEventArgs) Handles imbAddPage.Click
        Dim cmd As New SqlCommand

        cmd.Connection = cnx
        cmd.CommandText = "Insert " & DT_WEBRAD_PROJECTPAGES & "  (ProjectID) VALUES (" & Request.QueryString("ID") & ")"

        WhitTools.SQL.ExecuteNonQuery(cmd)

        LoadPages()
    End Sub

    Protected Sub ibDelete_Click(sender As Object, e As ImageClickEventArgs)
        If CType(CType(sender.parent, RepeaterItem).FindControl("lblID"), Label).Text <> CType(rptPages.Items(0).FindControl("lblID"), Label).Text Then
            Dim nPreviousPageID As Integer = GetPreviousPage(CType(CType(sender.parent, RepeaterItem).FindControl("lblID"), Label).Text)

            WhitTools.SQL.ExecuteNonQuery("Update " & DT_WEBRAD_PROJECTCONTROLS & " SET PageID = " & nPreviousPageID & " Where PageID = " & CType(CType(sender.parent, RepeaterItem).FindControl("lblID"), Label).Text, cnx)
            WhitTools.SQL.ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTPAGES & "  Where ID = " & CType(CType(sender.parent, RepeaterItem).FindControl("lblID"), Label).Text, cnx)

            LoadPage(nPreviousPageID)
        End If
    End Sub

    Sub LoadPages()
        SelectRepeaterData(rptPages, GetDataTable("Select * From " & DT_WEBRAD_PROJECTPAGES & "  Where ProjectID = " & Request.QueryString("ID"), cnx), cnx)
    End Sub

    Protected Sub libPage_Click(sender As Object, e As EventArgs)
        SetCurrentPage(CType(CType(sender.parent, RepeaterItem).FindControl("lblID"), Label).Text)
        LoadPage(CType(CType(sender.parent, RepeaterItem).FindControl("lblID"), Label).Text)
    End Sub

    Private Sub CheckCurrentPage()
        Dim projectID As Integer

        If Integer.TryParse(GetQueryString("ID"), projectID) AndAlso projectID > 0 Then
            Dim currentProject = GetCurrentProject(projectID)
            Dim pageID As Integer

            Integer.TryParse(GetQueryString("PageID"), pageID)

            If currentProject IsNot Nothing Then
                If If(currentProject.CurrentPageID, 0) > 0 AndAlso pageID > 0 AndAlso currentProject.CurrentPageID <> pageID Then
                    Redirect("controls.aspx?ID=" & GetQueryString("ID") & "&PageID=" & currentProject.CurrentPageID)
                ElseIf If(currentProject.CurrentPageID, 0) <= 0 Then
                    Dim currentPage = currentProject.ProjectPages.OrderBy(Function(page) page.ID).FirstOrDefault
                    currentProject.CurrentPageID = currentPage.ID
                    db.SaveChanges()
                End If
            End If
        End If
    End Sub

    Sub SetCurrentPage(pageID As Integer)
        Dim projectID As Integer

        If Integer.TryParse(GetQueryString("ID"), projectID) AndAlso projectID > 0 Then
            Dim currentProject = GetCurrentProject(projectID)
            currentProject.CurrentPageID = pageID
            db.SaveChanges()
        End If
    End Sub

    Private Shared Function GetCurrentProject(projectID As Integer) As Project
        Return db.Projects.FirstOrDefault(Function(project) project.ID = projectID)
    End Function

    Sub LoadPage(ByVal nPageID As Integer)
        Redirect("controls.aspx?ID=" & Request.QueryString("ID") & "&PageID=" & nPageID)
    End Sub

    Protected Sub rblRequired_SelectedIndexChanged(sender As Object, e As EventArgs)
        rblRepeaterAddRemove_SelectedIndexChanged(Nothing, Nothing)
        ShowMinimumRequired()
    End Sub

    Protected Sub rblVisible_SelectedIndexChanged(sender As Object, e As EventArgs)
        ToggleCustomVisibleValue()
        ToggleDependentValue()
    End Sub

    Sub ToggleDependentValue()
        pnlDependentValue.Visible = (rblVisible.SelectedValue = 3)
    End Sub

    Sub ToggleCustomVisibleValue()
        pnlCustomVisibleValue.Visible = (rblVisible.SelectedValue = 2)
    End Sub

    Protected Sub rblIncludeDatabase_SelectedIndexChanged(sender As Object, e As EventArgs)
        ToggleIncludeDatabase()
    End Sub

    Sub ToggleIncludeDatabase()
        pnlSQLInsertItemTable.Visible = If(rblIncludeDatabase.SelectedIndex = 1 And (ddlControlType.SelectedValue = CStr(N_REPEATER_CONTROL_TYPE) Or ddlControlType.SelectedValue = CStr(N_CHECKBOXLIST_CONTROL_TYPE) Or (pnlSelectionMode.Visible = True And rblSelectionMode.SelectedIndex > -1 And rblSelectionMode.SelectedValue = "Multiple")), True, False)
        pnlSQLInsertItemStoredProcedure.Visible = pnlSQLInsertItemTable.Visible
        pnlForeignID.Visible = pnlSQLInsertItemTable.Visible
    End Sub

    Protected Sub libInsertAbove_Click(sender As Object, e As EventArgs)
        InsertNewControl(sender, True)
    End Sub

    Protected Sub libInsertBelow_Click(sender As Object, e As EventArgs)
        InsertNewControl(sender)
    End Sub

    Sub InsertNewControl(ByVal sender As Object, Optional ByVal bAbove As Boolean = False)
        While CType(sender, Control).ID <> "PopupMenu"
            Try
                sender = sender.parent
            Catch ex As Exception
                Exit While
            End Try
        End While

        sender = sender.parent

        Dim cmd As New SqlCommand("Insert [ProjectControls] (position,ProjectID,PageID, DisplayLocation) values (@position,@ProjectID,@PageID, '1')", cnx)
        cmd.Parameters.AddWithValue("@position", If(bAbove, CInt(CType(sender.findcontrol("lblPosition"), Label).Text) - 1, CInt(CType(sender.findcontrol("lblPosition"), Label).Text)))
        cmd.Parameters.AddWithValue("@ProjectID", GetProjectID())
        cmd.Parameters.AddWithValue("@PageID", GetQueryString("PageID"))

        WhitTools.SQL.ExecuteNonQuery(cmd)

        sds.Update()
        rl1.DataBind()
    End Sub

    Protected Sub lsbuploadpathFolders_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lsbUploadPathFolders.SelectedIndexChanged
        UpdateFolders(lsbUploadPathFolders, lblSelectedUploadPathFolder, pnlUploadPathFolder, Nothing)
    End Sub

    Protected Sub rblCreateuploadpathFolder_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblCreateUploadPathFolder.SelectedIndexChanged
        pnlNewUploadPathFolder.Visible = If(rblCreateUploadPathFolder.SelectedValue = 1, True, False)
    End Sub

    Protected Sub cvuploadpath_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvUploadPath.ServerValidate
        args.IsValid = pnlUploadPathFolder.Visible = True
    End Sub

    Protected Sub ddlSetValueType_SelectedIndexChanged(sender As Object, e As EventArgs)
        UpdateValueTypeSelection(sender)
    End Sub

    Sub UpdateValueTypeSelection(ByRef sender As Object)
        HideSetValuePanels(sender)

        Select Case CType(sender, DropDownList).SelectedValue
            Case "Explicit"
                CType(CType(sender.parent, Panel).FindControl("pnlExplicitValue"), Panel).Visible = True
            Case "Control"
                FillListData(CType(CType(sender.parent, Panel).FindControl("ddlControlValue"), DropDownList), GetDataTable("SELECT C.Name + '(' + D.Prefix + ')' as Name, C.ID FROM " & DT_WEBRAD_PROJECTCONTROLS & " C LEFT OUTER JOIN " & DT_WEBRAD_CONTROLDATATYPES & " D ON (SELECT DataType FROM " & DT_WEBRAD_CONTROLTYPES & " WHERE ID = C.ControlType)  = D.ID WHERE ProjectID = " & GetProjectID() & " AND ControlType IN (Select ID FROM " & DT_WEBRAD_CONTROLTYPES & " WHERE DataType IN (" & GetSetValueDataTypes() & "))"), "Name", "ID")
                CType(CType(sender.parent, Panel).FindControl("pnlControlValue"), Panel).Visible = True
            Case "Database"
                CType(CType(CType(sender.parent, Panel).FindControl("ucSetValueDataSource"), UserControl).FindControl("pnlDataSourceType"), Panel).Visible = True
                CType(CType(sender.parent, Panel).FindControl("pnlExplicitValue"), Panel).Visible = True
                ShowDataSource(CType(sender.parent, Panel).FindControl("ucSetValueDataSource"))
        End Select
    End Sub

    Private Sub HideSetValuePanels(sender As Object)
        For Each CurrentControl As Control In CType(CType(sender.parent, Panel).FindControl("ucSetValueDataSource"), UserControl).Controls
            If TypeOf CurrentControl Is Panel Then
                CurrentControl.Visible = False
            End If
        Next

        HideDataSource(CType(sender.parent, Panel).FindControl("ucSetValueDataSource"))

        CType(CType(sender.parent, Panel).FindControl("pnlExplicitValue"), Panel).Visible = False
        CType(CType(sender.parent, Panel).FindControl("pnlControlValue"), Panel).Visible = False
    End Sub

    Protected Sub rblUpdateRepeaterItemsSelectionType_SelectedIndexChanged(sender As Object, e As EventArgs)
        CType(CType(sender.parent.parent, RepeaterItem).FindControl("pnlUpdateRepeaterItemsValue"), Panel).Visible = If(CType(sender, RadioButtonList).SelectedValue = "Explicit", True, False)
    End Sub

    Protected Sub cvCustomValidationCode_ServerValidate(source As Object, args As ServerValidateEventArgs)
        args.IsValid = (Not (rblCustomValidation.SelectedIndex = 1 And txtCustomValidationCode.Text = ""))
    End Sub

    Protected Sub rblCustomValidation_SelectedIndexChanged(sender As Object, e As EventArgs)
        pnlCustomValidationCode.Visible = If(rblCustomValidation.SelectedIndex = 1, True, False)
    End Sub

    Protected Sub txtMaxLength_TextChanged(sender As Object, e As EventArgs)
        Try
            If pnlSQLDataType.Visible And ddlSQLDataType.SelectedValue = "1" And CInt(txtSQLDataSize.Text) < CInt(txtMaxLength.Text) Then
                txtSQLDataSize.Text = txtMaxLength.Text
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub rblOnchange_SelectedIndexChanged(sender As Object, e As EventArgs)
        pnlOnchangeDetail.Visible = If(rblOnchange.SelectedIndex = 1, True, False)
    End Sub

    Protected Sub rblRepeaterAddRemove_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlControlType.SelectedIndex > 0 AndAlso GetControlDataType(ddlControlType.SelectedValue) = N_REPEATER_DATA_TYPE Then
            Dim bUseAddRemove As Boolean = (rblRepeaterAddRemove.SelectedValue = "1")

            pnlRepeaterItemName.Visible = bUseAddRemove
            pnlRequired.Visible = bUseAddRemove
            pnlPostbackActionOptions.Visible = bUseAddRemove
            ShowMinimumRequired()
        End If
    End Sub

    Sub ShowMinimumRequired()
        pnlMinimumRequired.Visible = False

        If rblRequired.SelectedValue = "1" Then
            If ddlControlType.SelectedValue = N_CHECKBOXLIST_CONTROL_TYPE Then
                pnlMinimumRequired.Visible = True
            ElseIf pnlRepeaterAddRemove.Visible And rblRepeaterAddRemove.SelectedValue = "1" Then
                pnlMinimumRequired.Visible = True
            End If
        End If
    End Sub

    Sub SaveVisibleDependentValue()
        DeleteDependingControlActions()

        If rblVisible.SelectedValue = N_VISIBLE_DEPENDENT_VALUE Then
            For Each CurrentItem As RepeaterItem In rptVisibleDependingControls.Items
                MakeDependingControlDoPostbackAction(CurrentItem)
                SaveDependingControlAction(CurrentItem)
            Next
        End If
    End Sub

    Sub DeleteDependingControlActions()
        If rblVisible.SelectedIndex > 0 AndAlso rblVisible.SelectedValue = N_VISIBLE_DEPENDENT_VALUE Then
            WhitTools.SQL.ExecuteNonQuery("DELETE FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE TargetControl = " & lblCurrentControlID.Text & " AND Action = " & N_CONTROLACTIONTYPE_VISIBLE)
        End If
    End Sub

    Sub MakeDependingControlDoPostbackAction(ByRef CurrentItem As RepeaterItem)
        WhitTools.SQL.ExecuteNonQuery("UPDATE " & DT_WEBRAD_PROJECTCONTROLS & " SET Autopostback = 1, PerformPostbackAction = 1 WHERE ID = " & CType(CurrentItem.FindControl("ddlDependingControl"), DropDownList).SelectedValue)
    End Sub

    Sub SaveDependingControlAction(ByRef CurrentItem As RepeaterItem)
        Dim CurrentID As Integer
        Dim cmd As New SqlCommand("usp_InsertProjectControlPostbackAction", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@TriggerControl", CType(CurrentItem.FindControl("ddlDependingControl"), DropDownList).SelectedValue)
        cmd.Parameters.AddWithValue("@TargetControl", lblCurrentControlID.Text)
        cmd.Parameters.AddWithValue("@Action", N_CONTROLACTIONTYPE_VISIBLE)

        CurrentID = WhitTools.SQL.ExecuteScalar(cmd)

        cmd.CommandText = "usp_InsertProjectControlPostbackActionTriggerValue"
        cmd.Parameters.Clear()

        cmd.Parameters.AddWithValue("@ActionID", CurrentID)
        cmd.Parameters.AddWithValue("@TriggerValue", "")
        cmd.Parameters.AddWithValue("@TriggerOperator", "")

        For Each TriggerValueItem As RepeaterItem In
            CType(CType(CurrentItem.FindControl("ucVisibilityTriggerValues"), UserControl).FindControl("rptTriggerValues"), Repeater).Items

            cmd.Parameters("@TriggerValue").Value = CType(TriggerValueItem.FindControl("txtTriggerValue"), TextBox).Text
            cmd.Parameters("@TriggerOperator").Value =
                CType(TriggerValueItem.FindControl("ddlTriggerOperator"), DropDownList).SelectedValue

            WhitTools.SQL.ExecuteNonQuery(cmd)
        Next
    End Sub

    Public Sub cvVisibleDependingControls_ServerValidate(ByVal source As System.Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
        'args.IsValid = ValidateRepeaterItems(rptVisibleDependingControls,cvVisibleDependingControls,args,1,0,"Sorry, you must select at least 1 depending control details(s).","Sorry, you must select at most 0 depending control details(s).")
    End Sub




    Protected Sub btnrptVisibleDependingControlsAddItem_Click(sender As Object, e As EventArgs)
        Dim dtSupplied As New WhitTools.DataTablesSupplied()
        dtSupplied.AddRow("ddlDependingControl", "SQLSelect", "SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & If(lblCurrentControlID.Text <> "", " AND NOT ID = " & lblCurrentControlID.Text, "") & " AND ControlType IN (SELECT ID FROM " & DT_WEBRAD_CONTROLTYPES & " WHERE DataType IN (Select ActionControlDataType FROM " & DT_WEBRAD_CONTROLTYPEACTIONS & " WHERE TargetControlDataType = " & GetControlDataType(ddlControlType.SelectedValue) & " and ActionType = " & N_CONTROLACTIONTYPE_VISIBLE & ")) ORDER BY Position ASC", "Name", "ID")

        AddNewRepeaterItem(rptVisibleDependingControls, dtSupplied, 1, 0, cvVisibleDependingControls, "depending control details")
    End Sub

    Protected Sub librptVisibleDependingControls_RemoveItem_Click(sender As Object, e As EventArgs)
        Dim ParentRepeaterItem As RepeaterItem = GetParentRepeaterItem(sender)
        Dim dtSupplied As New WhitTools.DataTablesSupplied()
        dtSupplied.AddRow("ddlDependingControl", "SQLSelect", "SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & If(lblCurrentControlID.Text <> "", " AND NOT ID = " & lblCurrentControlID.Text, "") & " AND ControlType IN (SELECT ID FROM " & DT_WEBRAD_CONTROLTYPES & " WHERE DataType IN (Select ActionControlDataType FROM " & DT_WEBRAD_CONTROLTYPEACTIONS & " WHERE TargetControlDataType = " & GetControlDataType(ddlControlType.SelectedValue) & " and ActionType = " & N_CONTROLACTIONTYPE_VISIBLE & ")) ORDER BY Position ASC", "Name", "ID")

        RemoveRepeaterItem(ParentRepeaterItem.Parent, dtSupplied, CType(GetParentRepeaterItem(sender), RepeaterItem).ItemIndex, 1, cvVisibleDependingControls, "depending control details")
    End Sub

    Protected Sub btnSetParent_Click(sender As Object, e As EventArgs)
        Dim selectedIDs As String = GetSelectedControlIDs()

        If selectedIDs <> "" Then
            Dim updateCommand As String = "UPDATE " & DT_WEBRAD_PROJECTCONTROLS & " SET ParentControlID = " & ddlParentControls.SelectedValue & " WHERE ID IN (" & selectedIDs & ") AND Not ID = " & ddlParentControls.SelectedValue

            WhitTools.SQL.ExecuteNonQuery(updateCommand)
        End If

        RefreshPage()
    End Sub


    Private Function CreateGroupControl() As ProjectControl
        Dim control As New ProjectControl()

        control.ControlType = N_FORMGROUP_CONTROL_TYPE
        control.Heading = ""
        control.DisplayHeading = 1
        control.DisplayLocation = 1
        control.DisplayType = 1
        control.IncludeDatabase = 0
        control.Enabled = 1
        control.Visible = 1
        control.Required = 0
        control.CssClass = "form-group"
        control.SQLDataType = 0

        Return control
    End Function
    Private Function GetUniqueGroupName(firstControl As ProjectControl) As String
        Dim groupName As String = txtSetGroupName.Text
        Dim projectID As Integer = GetProjectID()

        If db.ProjectControls.Any(Function(pc) pc.ProjectID = projectID And pc.Name = groupName) Then
            Dim nameIndex As Integer = 2
            groupName = firstControl.Name & "Group"

            While db.ProjectControls.Any(Function(pc) pc.Name = groupName)
                groupName = firstControl.Name & nameIndex
                nameIndex += 1
            End While
        End If

        Return groupName
    End Function

    Private Function GetSelectedControlIDs() As String
        Dim selectedIDs As String = ""

        For Each currentItem As ReorderListItem In rl1.Items
            Try
                If CType(currentItem.FindControl("chkSelectControl"), CheckBox).Checked Then

                    If selectedIDs <> "" Then
                        selectedIDs &= ","
                    End If

                    selectedIDs &= CType(currentItem.FindControl("lblID"), Label).Text
                End If
            Catch ex As Exception

            End Try

        Next

        Return selectedIDs
    End Function

    Protected Sub btnDeleteControls_OnClick(sender As Object, e As EventArgs) Handles btnDeleteControls.Click
        Dim selectedIDs As String = GetSelectedControlIDs()

        If selectedIDs <> "" Then
            WhitTools.SQL.ExecuteNonQuery("DELETE FROM  " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ID IN (" & selectedIDs & ")")
        End If

        RefreshPage()
    End Sub

    Private Sub AddListSelectionsAction()
        If GetListSelectionsAction() Is Nothing Then
            Dim listSelectionsAction As ProjectControlPostbackAction = New ProjectControlPostbackAction()

            listSelectionsAction.Action = N_LISTSELECTIONS_ACTIONTYPE
            listSelectionsAction.TriggerControl = lblCurrentControlID.Text
            listSelectionsAction.TargetControl = 0
            listSelectionsAction.ControlActionType = db.ControlActionTypes.FirstOrDefault(Function(cat) cat.ID = N_LISTSELECTIONS_ACTIONTYPE)

            db.ProjectControlPostbackActions.Add(listSelectionsAction)
            db.SaveChanges()
        End If
    End Sub

    Private Sub RemoveListSelectionsAction()
        If GetListSelectionsAction() IsNot Nothing Then
            db.ProjectControlPostbackActions.Remove(GetListSelectionsAction())
            db.SaveChanges()
        End If
    End Sub

    Private Function GetListSelectionsAction() As ProjectControlPostbackAction
        Return db.ProjectControlPostbackActions.Where(Function(pcpa) pcpa.TriggerControl = lblCurrentControlID.Text).FirstOrDefault()
    End Function

    Protected Sub btnSetGroup_Click(sender As Object, e As EventArgs)
        Dim selectedIDs As String = GetSelectedControlIDs()
        Dim idArray As String() = selectedIDs.Split(",")
        Dim firstControlID As Integer = idArray(0)

        If selectedIDs <> "" Then
            Dim firstControl = db.ProjectControls.First(Function(pc) pc.ID = firstControlID)
            Dim groupControl = CreateGroupControl()

            groupControl.ProjectID = GetProjectID()
            groupControl.PageID = GetCurrentPageID()
            groupControl.Name = GetUniqueGroupName(firstControl)
            groupControl.Position = firstControl.Position - 1

            db.ProjectControls.Add(groupControl)
            db.SaveChanges()

            For Each Control In db.ProjectControls.Where(Function(pc) idArray.Contains(pc.ID))
                Control.ParentControlID = groupControl.ID
            Next

            db.SaveChanges()
        End If

        RefreshPage()
    End Sub

    Protected Sub MainContent_btnAlignControls_Click(sender As Object, e As EventArgs)
        Dim selectedIDs As String = GetSelectedControlIDs()
        Dim idArray As String() = selectedIDs.Split(",")
        Dim firstControlID As Integer = idArray(0)

        If selectedIDs <> "" Then
            Dim firstControl = db.ProjectControls.First(Function(pc) pc.ID = firstControlID)

            firstControl.DisplayType = N_DISPLAYTYPE_STACK_NEWROW

            For Each Control In db.ProjectControls.Where(Function(pc) pc.ID <> firstControlID AndAlso idArray.Contains(pc.ID))
                Control.DisplayType = N_DISPLAYTYPE_STACK
            Next

            db.SaveChanges()
        End If

        RefreshPage()
    End Sub

    Protected Sub chkSelectAllControls_CheckedChanged(sender As Object, e As EventArgs)
        For Each currentitem As ReorderListItem In rl1.Items
            If currentitem.FindControl("chkSelectControl") IsNot Nothing Then
                CType(currentitem.FindControl("chkSelectControl"), CheckBox).Checked = chkSelectAllControls.Checked

            End If

        Next
    End Sub
End Class
