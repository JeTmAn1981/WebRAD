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
Imports WhitTools.EcommerceConstants
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.ProjectOperations
Imports Common.General.ControlTypes
Imports Common.General.Folders
Imports Common.Webpages.Backend.Export
Imports Common.General.Pages
Imports Common.General.Links
Imports Common.General.Columns
Imports System.Web.UI.WebControls

Partial Public Class frontend
    Inherits System.Web.UI.Page

    Shared cnx As SqlConnection = CreateSQLConnection("WebRAD")
    Shared ProjectDT As New DataTable
    Public dtEmailSupplied As New WhitTools.DataTablesSupplied

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Session("ProjectID") = ""

        If Not Page.IsPostBack Then
            LoadDDLs()
            BindData()
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Function TestMethod(ByVal nProjectID As Integer) As List(Of Common.WebRADControl)

        Dim controlList As New List(Of Common.WebRADControl)

        Dim WRC As New Common.WebRADControl
        Dim CMI As New Common.ContextMenuItem

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
        Dim dtControls As DataTable = GetDataTable("Select * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & nProjectID & " AND ControlType IN (SELECT ID FROM " & DT_WEBRAD_CONTROLTYPES & " WHERE DataType IN " & GetControlTypesWithValues() & ") ORDER BY Position asc")

        For Each CurrentRow As DataRow In dtControls.Rows
            With CurrentRow
                WRC = New Common.WebRADControl
                CMI = New Common.ContextMenuItem

                WRC.key = .Item("Name")
                CMI.Name = .Item("Name")
                WRC.item = CMI

                controlList.Add(WRC)
            End With
        Next


        Return controlList
    End Function

    Sub LoadDDLs()
        ProjectDT = GetDataTable(cnx, "Select P.*,  DB.Name as SQLDBName, S.Name as SQLServerName From " & DT_WEBRAD_PROJECTS & "  P left outer join " & DT_WEBRAD_SQLDATABASES & "  DB on P.SQLDatabase = DB.ID left outer join " & DT_WEBRAD_SQLSERVERS & " S on DB.ServerID = S.ID Where P.ID = " & Request.QueryString("ID"))
        LoadDefaultFolders(lsbFrontendFolders, lblSelectedFrontendFolder)

        CreateLoginColumnTypes()
        '        FillListData(lsbEmailControls, GetDataTable("Select ID, Heading From " & DT_WEBRAD_PROJECTCONTROLS & " Where ParentControlID IS NULL AND ControlType IN (Select ID FROM " & DT_WEBRAD_CONTROLTYPES & "  Where DataType IN (1,2,3)) AND ProjectID = " & Request.QueryString("ID"), cnx), "Heading", "ID", False)
        FillListData(lsbLoginColumns, GetDataTable("Select * FROM " & DT_WEBRAD_LOGINCOLUMNTYPES & " WHERE Frontend = 1 ORDER BY [Order] asc"), "Heading", "IDNumber")
        FillListData(cblAdditionalOperationTypes, GetDataTable("SELECT * FROM " & DT_WEBRAD_ADDITIONALOPERATIONTYPES), "Name", "ID", False)
        '"Select ID, Heading From " & DT_WEBRAD_PROJECTCONTROLS & " Where ParentControlID IS NULL AND ControlType IN (Select ID FROM " & DT_WEBRAD_CONTROLTYPES & "  Where DataType IN (1,2,3)) AND ProjectID = " & Request.QueryString("ID")

        LoadEmailSupplied()
        UpdateRepeaterItems(rptEmailRecipients, 1, dtEmailSupplied)

        CType(rptEmailRecipients.Items(0).FindControl("txtSubject"), TextBox).Text = ProjectDT.Rows(0).Item("PageTitle") & " Submission Received"
        UpdateEmailSettings()

        Try
            FillListData(lsbMainColumns, GetExportColumns(-1, False, If(ProjectDT.Rows(0).Item("RequireLogin") = 1, True, False)), "Name", "ColumnControlID", False)
        Catch ex As Exception

        End Try

        lsbMainColumns.Items.Insert(0, New ListItem("All", "0"))
        GetAdditionalExportColumns(rptTables)

        lblMainTableName.Text = GetDataTable(cnx, "Select * From " & DT_WEBRAD_PROJECTS & "  Where ID = " & Request.QueryString("ID")).Rows(0).Item("SQLMainTableName")
    End Sub

    Sub LoadEmailSupplied()
        dtEmailSupplied.Rows.Clear()
        dtEmailSupplied.AddRow("lsbEmailControls", "SQLSelect", "Select ID, Name From " & DT_WEBRAD_PROJECTCONTROLS & " Where ControlType IN (Select ID FROM " & DT_WEBRAD_CONTROLTYPES & "  Where DataType IN (1,2,3)) AND ProjectID = " & GetProjectID() & " ORDER BY PageID asc, Position asc ", "Name", "ID")
        dtEmailSupplied.AddRow("ddlWorkflowDestination", "SQLSelect", "Select distinct(ID), Name FROM " & DT_WORKFLOWPOSITIONS & " WHERE ID IN (SELECT Destination FROM " & DT_WORKFLOWTYPESTEPTRIGGERCONTROLS & " TC Where StepID=" & ProjectDT.Rows(0).Item("WorkflowStep") & ")", "Name", "ID")
    End Sub

    Sub BindData()
        If Request.QueryString("ID") <> "" Then
            'Dim dt As New DataTable

            'dt = GetDataTable(cnx, "Select * From " & DT_WEBRAD_PROJECTS & "  Where ID = " & Request.QueryString("ID"))

            If ProjectDT.Rows.Count > 0 Then
                With ProjectDT.Rows(0)
                    SetItemSelected(rblIncludeFrontend, .Item("IncludeFrontend"))
                    showFrontend()

                    If pnlFrontend.Visible = True Then
                        SetItemSelected(rblRequireLogin, .Item("RequireLogin"))
                        SetListControlItemSelected(lsbLoginColumns, "", True, GetDataTable("Select * FROM " & DT_WEBRAD_PROJECTCOLUMNS & " WHERE Type='Login' AND ProjectID = " & GetProjectID(), cnx), "ColumnControlID")

                        ToggleLoginColumns()
                        SetItemSelected(rblEmailSupervisor, .Item("EmailSupervisor"))

                        UpdateFolders(lsbFrontendFolders, lblSelectedFrontendFolder, pnlFrontendFolder, txtFrontendLink, .Item("frontendPath"))
                        txtFrontendLink.Text = .Item("frontendLink")
                        txtNewFrontendFolderName.Text = .Item("frontendNewFolder")
                        rblCreateFrontendFolder.SelectedIndex = If(.Item("frontendNewFolder") <> "", 1, 0)
                        pnlNewFrontendFolder.Visible = .Item("frontendNewFolder") <> ""

                        SelectRepeaterData(rptSupervisors, GetDataTable("Select plname as Name, plemail as Email, SupervisorID  From " & DT_WEBRAD_PROJECTSUPERVISORS & " S left outer join adTelephone.dbo.PeopleListing P on S.SupervisorID = P.PLID Where ProjectID = " & Request.QueryString("ID"), cnx), cnx)

                        SetItemSelected(rblEmailSubmitter, .Item("SubmitterEmail"))

                        Dim dtEmailRecipients As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTEMAILMESSAGES & " WHERE ProjectID = " & GetProjectID())

                        If dtEmailRecipients.Rows.Count > 0 Then
                            SelectRepeaterData(rptEmailRecipients, dtEmailRecipients, dtEmailSupplied)

                            For Each CurrentItem As RepeaterItem In rptEmailRecipients.Items
                                rblMessageType_SelectedIndexChanged(CurrentItem.FindControl("rblMessageType"), Nothing)
                            Next
                        End If

                        For nCounter As Integer = 0 To dtEmailRecipients.Rows.Count - 1
                            SetListControlItemSelected(CType(rptEmailRecipients.Items(nCounter).FindControl("lsbEmailControls"), ListBox), "", True, GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTEMAILMESSAGESUBMITTERCONTROLS & " WHERE MessageID = " & dtEmailRecipients.Rows(nCounter).Item("ID")), "ControlID")
                            CType(rptEmailRecipients.Items(nCounter).FindControl("lblListEmailControls"), Label).Text = GetListOfSelectedValues(rptEmailRecipients.Items(nCounter).FindControl("lsbEmailControls"))
                            rblWorkflow_SelectedIndexChanged(rptEmailRecipients.Items(nCounter).FindControl("rblWorkflow"), Nothing)
                            ddlWorkflowDestination_SelectedIndexChanged(rptEmailRecipients.Items(nCounter).FindControl("ddlWorkflowDestination"), Nothing)
                        Next


                        pnlEmailSubmitter.Visible = If(rblEmailSubmitter.SelectedIndex = 1, True, False)

                        SetItemSelected(rblConfirmationPageMessageType, .Item("ConfirmationMessageType"))
                        txtConfirmationPageMessageRich.Text = .Item("ConfirmationMessage")
                        txtConfirmationPageMessagePlain.Text = .Item("ConfirmationMessage")
                        rblConfirmationPageMessageType_SelectedIndexChanged(Nothing, Nothing)
                    End If
                End With
            End If

            Dim dtPages As DataTable = GetDataTable("Select * From " & DT_WEBRAD_PROJECTPAGES & "  Where ProjectID = " & GetProjectID())

            If dtPages.Rows.Count > 1 Then
                SelectRepeaterData(rptPages, dtPages, cnx)


                For Each currentitem As RepeaterItem In rptPages.Items
                    If currentitem.ItemIndex = rptPages.Items.Count - 1 Then
                        CType(currentitem.FindControl("pnlMakeCertificationPage"), Panel).Visible = True
                    Else
                        CType(currentitem.FindControl("chkCertification"), CheckBox).Checked = False
                    End If
                Next


                SetItemSelected(rblMultipleSubmissions, ProjectDT.Rows(0).Item("MultipleSubmissions"))

                SetListControlItemSelected(lsbMainColumns, "", True, GetDataTable("Select * From " & DT_WEBRAD_PROJECTCOLUMNS & "  Where Type = 'Retained' AND ProjectID = " & Request.QueryString("ID") & " and TableControlID = 0", cnx), "ColumnControlID")

                For Each CurrentItem As RepeaterItem In rptTables.Items
                    SetListControlItemSelected(CType(CurrentItem.FindControl("lsbColumns"), ListBox), "", True, GetDataTable("Select * From " & DT_WEBRAD_PROJECTCOLUMNS & "  Where Type='Retained' AND ProjectID = " & Request.QueryString("ID") & " and TableControlID = " & CType(CurrentItem.FindControl("lblID"), Label).Text, cnx), "ColumnControlID")
                Next

                ToggleRetainedColumns()
                pnlPages.Visible = True
            End If

            SetListControlItemSelected(cblAdditionalOperationTypes, "", True, GetDataTable("SELECT Distinct(TYPE) FROM " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " WHERE PageID IN (SELECT ID FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & GetProjectID() & ")"), "Type")

            LoadAdditionalOperations()
        End If
    End Sub


    Sub LoadAdditionalOperations()
        If GetListOfSelectedValues(cblAdditionalOperationTypes, "Value") <> "" Then
            SelectRepeaterData(rptAdditionalOperationPages, GetDataTable("select P.ID as PageID, Name, CASE WHEN O.AdditionalOperations IS NULL THEN '' ELSE O.AdditionalOperations END Operation, (select PageNumber from (select ID, rank() OVER(PARTITION BY ProjectID ORDER BY ID) as PageNumber from " & DT_WEBRAD_PROJECTPAGES & " where projectID = " & GetProjectID() & ") as PageRank WHERE ID = P.ID) as PageNumber, T.ID as OperationTypeID from " & DT_WEBRAD_PROJECTPAGES & " P join " & DT_WEBRAD_ADDITIONALOPERATIONTYPES & " T ON T.ID IN (" & GetListOfSelectedValues(cblAdditionalOperationTypes, "Value") & ") LEFT OUTER JOIN " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " O ON (O.Type = T.ID AND O.PageID = P.ID) where ProjectID=" & GetProjectID() & " ORDER BY P.Id, T.ID"))
        Else
            EmptyRepeater(rptAdditionalOperationPages)
        End If
    End Sub

    Function CheckControlsComplete()
        Dim ncounter As Integer
        Dim dt As New DataTable

        If Request.QueryString("ID") <> "" Then
            dt = GetDataTable(cnx, "Select * From " & DT_WEBRAD_PROJECTCONTROLS & " Where ProjectID = " & Request.QueryString("ID"))

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

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Declare local variables
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim dt As DataTable
        Dim dr As DataRow
        Dim nCounter As Integer
        Dim sPLID As String = txtSupervisorPLID.Text
        Dim sName, sEmail As String
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check for a valid id
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'If ValidateStudentIDExists(sPLID) Then
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the supervisor's name and e-mail
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        sName = GetStudentInfo(sPLID, "PLName")
        sEmail = GetStudentInfo(sPLID, "PLEmail")
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check that the supervisor does not already exist
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If GetDataTable("SELECT ID FROM " & DT_WEB_INVOICE_SUPERVISORS & " WHERE InvoiceType='" & Request.QueryString("ID") & "' AND Name='" & CleanSQL(sName) & "' AND Email='" & CleanSQL(sEmail) & "'").Rows.Count > 0 Then
            WriteAlert("That supervisor is already in the list. You may have deleted them from the list, but if you have not committed the update yet then they are still in the database. Refresh the page to return them to the list.")
            txtSupervisorPLID.Text = ""
            Exit Sub
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Create the name datatable
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        dt = New DataTable()
        dt.Columns.Add("SupervisorID", GetType(String))
        dt.Columns.Add("Name", GetType(String))
        dt.Columns.Add("Email", GetType(String))
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the current supervisor list
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If rptSupervisors.Items.Count > 0 Then
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Cycle through the supervisor list
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            For nCounter = 0 To rptSupervisors.Items.Count - 1
                With rptSupervisors.Items(nCounter)
                    dr = dt.NewRow
                    dr.Item("SupervisorID") = CType(.FindControl("lblSupervisorID"), Label).Text
                    dr.Item("Name") = CType(.FindControl("lblName"), Label).Text
                    dr.Item("Email") = CType(.FindControl("lblEmail"), Label).Text
                    dt.Rows.Add(dr)
                End With
            Next
        End If

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Add the new supervisor
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        dr = dt.NewRow
        dr.Item("SupervisorID") = sPLID
        dr.Item("Name") = sName
        dr.Item("Email") = sEmail
        dt.Rows.Add(dr)

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Assign the new supervisor list
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        rptSupervisors.DataSource = dt
        rptSupervisors.DataBind()
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Deselect name
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        txtSupervisorPLID.Text = ""
        'End If
    End Sub

    Sub rptSupervisors_ItemCommand(ByVal source As System.Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Declare local variables
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim nCounter As Integer
        Dim dt As DataTable
        Dim dr As DataRow
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Find the button command name
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If e.CommandName = S_DELETE Then
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Create the datatable
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            dt = New DataTable()
            dt.Columns.Add("SupervisorID", GetType(String))
            dt.Columns.Add("Name", GetType(String))
            dt.Columns.Add("Email", GetType(String))
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the supervisor names, excluding the one to delete
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            For nCounter = 0 To rptSupervisors.Items.Count - 1
                With rptSupervisors.Items(nCounter)
                    If CType(.FindControl("lblEmail"), Label).Text <> CType(e.Item.FindControl("lblEmail"), Label).Text Then
                        dr = dt.NewRow
                        dr.Item("SupervisorID") = CType(.FindControl("lblSupervisorID"), Label).Text
                        dr.Item("Name") = CType(.FindControl("lblName"), Label).Text
                        dr.Item("Email") = CType(.FindControl("lblEmail"), Label).Text
                        dt.Rows.Add(dr)
                    End If
                End With
            Next
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Recreate the supervisor list without the deleted name
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            rptSupervisors.DataSource = dt
            rptSupervisors.DataBind()
        End If
    End Sub

    Protected Sub lsbFrontendFolders_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lsbFrontendFolders.SelectedIndexChanged
        UpdateFolders(lsbFrontendFolders, lblSelectedFrontendFolder, pnlFrontendFolder, txtFrontendLink)
    End Sub

    Protected Sub rblCreateFrontendFolder_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblCreateFrontendFolder.SelectedIndexChanged
        pnlNewFrontendFolder.Visible = If(rblCreateFrontendFolder.SelectedValue = 1, True, False)
    End Sub

    Protected Sub cvFrontendPath_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvFrontendPath.ServerValidate
        args.IsValid = pnlFrontendFolder.Visible = True
    End Sub

    Protected Sub rblEmailSubmitter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblEmailSubmitter.SelectedIndexChanged
        pnlEmailSubmitter.Visible = If(rblEmailSubmitter.SelectedValue = 1, True, False)
    End Sub

    Protected Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnContinue.Click
        Page.Validate()

        If Page.IsValid Then
            SaveMainFrontendInfo()
            SaveAncillaryFrontendInfo()

            Redirect("backend.aspx?ID=" & Request.QueryString("ID"))
        End If
    End Sub

    Private Sub SaveAncillaryFrontendInfo()
        SaveEmailMessages()
        SaveSupervisors()
        SavePages()
        SaveAdditionalOperations()
        SaveColumns()
    End Sub

    Private Sub SaveMainFrontendInfo()
        Dim cmd As New SqlCommand
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_UpdateProjectFrontend"
        cmd.Connection = cnx

        cmd.Parameters.AddWithValue("@ID", Request.QueryString("ID"))
        cmd.Parameters.AddWithValue("@IncludeFrontend", rblIncludeFrontend.SelectedValue)
        cmd.Parameters.AddWithValue("@RequireLogin", rblRequireLogin.SelectedValue)
        cmd.Parameters.AddWithValue("@EmailSupervisor", rblEmailSupervisor.SelectedValue)
        cmd.Parameters.AddWithValue("@FrontendLink", FormatProjectLink(txtFrontendLink.Text))
        cmd.Parameters.AddWithValue("@FrontendPath", lblSelectedFrontendFolder.Text)
        cmd.Parameters.AddWithValue("@ConfirmationMessageType", rblConfirmationPageMessageType.SelectedValue)
        cmd.Parameters.AddWithValue("@ConfirmationMessage", GetConfirmationPageMessage())

        If pnlPages.Visible Then
            cmd.Parameters.AddWithValue("@MultipleSubmissions", rblMultipleSubmissions.SelectedValue)

            ExecuteNonQuery("Delete FROM " & DT_WEBRAD_PROJECTCOLUMNS & " WHERE Type='Retained' AND ProjectID = " & GetProjectID())

            SaveColumnsInfo("Retained", lsbMainColumns, pnlRetainedColumns.Visible, rptTables)
        End If

        If pnlNewFrontendFolder.Visible = True Then
            cmd.Parameters.AddWithValue("@FrontendNewFolder", txtNewFrontendFolderName.Text)
        End If

        cmd.Parameters.AddWithValue("@SubmitterEmail", rblEmailSubmitter.SelectedValue)

        ExecuteNonQuery("Update " & DT_WEBRAD_PROJECTCONTROLS & " Set SubmitterEmail = 0 Where ProjectID = " & Request.QueryString("ID"), cnx)

        ExecuteNonQuery(cmd, cnx)
    End Sub

    Private Sub SaveColumns()

        ExecuteNonQuery("Delete FROM " & DT_WEBRAD_PROJECTCOLUMNS & " WHERE Type='Login' AND ProjectID = " & GetProjectID())
        SaveColumnsInfo("Login", lsbLoginColumns, pnlLoginColumns.Visible)
    End Sub

    Private Sub SavePages()
        If pnlPages.Visible Then
            Dim cmd As New SqlCommand
            cmd.Parameters.Clear()
            cmd.CommandText = "usp_UpdateProjectPage"
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@PageID", "")
            cmd.Parameters.AddWithValue("@Purpose", "")
            cmd.Parameters.AddWithValue("@Certification", "")

            For Each CurrentItem As RepeaterItem In rptPages.Items
                cmd.Parameters("@PageID").Value = CType(CurrentItem.FindControl("lblID"), Label).Text
                cmd.Parameters("@Purpose").Value = CType(CurrentItem.FindControl("txtPurpose"), TextBox).Text
                cmd.Parameters("@Certification").Value = CType(CurrentItem.FindControl("chkCertification"), CheckBox).Checked

                ExecuteNonQuery(cmd, cnx)
            Next
        End If
    End Sub

    Private Function SaveSupervisors() As SqlCommand
        Dim cmd As SqlCommand
        Dim nCounter As Integer

        ExecuteNonQuery("Delete From " & DT_WEBRAD_PROJECTSUPERVISORS & " Where ProjectID = " & Request.QueryString("ID"))

        cmd = New SqlCommand
        cmd.Connection = cnx
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_InsertProjectSupervisor"

        cmd.Parameters.AddWithValue("@ProjectID", Request.QueryString("ID"))
        cmd.Parameters.AddWithValue("@SupervisorID", "")

        For nCounter = 0 To rptSupervisors.Items.Count - 1
            cmd.Parameters("@SupervisorID").Value = CType(rptSupervisors.Items(nCounter).FindControl("lblSupervisorID"), Label).Text
            ExecuteNonQuery(cmd)
        Next
        Return cmd
    End Function

    Private Sub SaveEmailMessages()
        DeleteCurrentEmailMessageInfo()

        If pnlEmailSubmitter.Visible = True Then
            Dim nEmailMessageID As Integer
            Dim cmd As New SqlCommand

            SetupEmailMessageCommand(cmd)

            For Each CurrentItem As RepeaterItem In rptEmailRecipients.Items
                With CurrentItem
                    SaveEmailMessage(cmd, CurrentItem)
                End With
            Next
        End If
    End Sub

    Private Sub SaveEmailMessage(cmd As SqlCommand, CurrentItem As RepeaterItem)
        Dim nEmailMessageID As Integer

        cmd.Parameters("@Workflow").Value = CType(CurrentItem.FindControl("rblWorkflow"), RadioButtonList).SelectedValue
        cmd.Parameters("@WorkflowDestination").Value = If(CType(CurrentItem.FindControl("rblWorkflow"), RadioButtonList).SelectedIndex > 0, CType(CurrentItem.FindControl("ddlWorkflowDestination"), DropDownList).SelectedValue, "")
        cmd.Parameters("@ToAddress").Value = CType(CurrentItem.FindControl("txtToAddress"), TextBox).Text
        cmd.Parameters("@BCCAddress").Value = CType(CurrentItem.FindControl("txtBCCAddress"), TextBox).Text
        cmd.Parameters("@Subject").Value = CType(CurrentItem.FindControl("txtSubject"), TextBox).Text
        cmd.Parameters("@MessageType").Value = CType(CurrentItem.FindControl("rblMessageType"), RadioButtonList).SelectedValue
        cmd.Parameters("@Message").Value = If(cmd.Parameters("@MessageType").Value = "Rich", CType(CurrentItem.FindControl("txtMessage"), TextBox).Text, CType(CurrentItem.FindControl("txtPlainMessage"), TextBox).Text)

        nEmailMessageID = ExecuteScalar(cmd)

        Dim cmd2 As New SqlCommand
        cmd2.Connection = cnx
        cmd2.CommandType = CommandType.StoredProcedure
        cmd2.CommandText = "usp_InsertProjectEmailMessageSubmitterControl"

        cmd2.Parameters.AddWithValue("@MessageID", nEmailMessageID)
        cmd2.Parameters.AddWithValue("@ControlID", "")

        For Each CurrentListItem As ListItem In CType(CurrentItem.FindControl("lsbEmailControls"), ListBox).Items
            If CurrentListItem.Selected Then
                cmd2.Parameters("@ControlID").Value = CurrentListItem.Value
                ExecuteNonQuery(cmd2)
            End If
        Next
    End Sub

    Private Sub SetupEmailMessageCommand(cmd As SqlCommand)

        cmd.Connection = cnx
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_InsertProjectEmailMessage"

        cmd.Parameters.AddWithValue("@ProjectID", Request.QueryString("ID"))
        cmd.Parameters.AddWithValue("@TypeID", "1")
        cmd.Parameters.AddWithValue("@ToAddress", "")
        cmd.Parameters.AddWithValue("@BCCAddress", "")
        cmd.Parameters.AddWithValue("@Subject", "")
        cmd.Parameters.AddWithValue("@MessageType", "")
        cmd.Parameters.AddWithValue("@Message", "")
        cmd.Parameters.AddWithValue("@Workflow", "")
        cmd.Parameters.AddWithValue("@WorkflowDestination", "")
    End Sub

    Private Sub DeleteCurrentEmailMessageInfo()

        ExecuteNonQuery("DELETE FROM " & DT_WEBRAD_PROJECTEMAILMESSAGES & " WHERE ProjectID = " & GetProjectID())
        ExecuteNonQuery("DELETE FROM " & DT_WEBRAD_PROJECTEMAILMESSAGESUBMITTERCONTROLS & " WHERE MessageID IN (Select ID FROM " & DT_WEBRAD_PROJECTEMAILMESSAGES & " WHERE ProjectID = " & GetProjectID() & ")")
    End Sub

    Sub SaveAdditionalOperations()
        ExecuteNonQuery("DELETE FROM " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " WHERE PageID IN (Select ID FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & GetProjectID() & ")")

        Dim cmd As New SqlCommand("usp_InsertProjectAdditionalOperation", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@PageID", "")
        cmd.Parameters.AddWithValue("@Type", "")
        cmd.Parameters.AddWithValue("@AdditionalOperations", "")

        For Each CurrentItem As RepeaterItem In rptAdditionalOperationPages.Items
            With CurrentItem
                cmd.Parameters("@Type").Value = CType(.FindControl("lblOperationTypeID"), Label).Text
                cmd.Parameters("@PageID").Value = CType(.FindControl("lblPageID"), Label).Text
                cmd.Parameters("@AdditionalOperations").Value = CType(.FindControl("txtOperation"), TextBox).Text

                ExecuteNonQuery(cmd)
            End With
        Next
    End Sub

    Protected Sub rblIncludeFrontend_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblIncludeFrontend.SelectedIndexChanged
        showFrontend()
    End Sub

    Sub showFrontend()
        pnlFrontend.Visible = If(rblIncludeFrontend.SelectedValue = 1, True, False)
    End Sub

    Protected Sub cvFrontendLink_ServerValidate(source As Object, args As ServerValidateEventArgs)
        ValidateLink(cvFrontendLink, txtFrontendLink, args)
    End Sub


    Protected Sub txtNewFrontendFolderName_TextChanged(sender As Object, e As EventArgs) Handles txtNewFrontendFolderName.TextChanged
        txtFrontendLink.Text = txtFrontendLink.Text & txtNewFrontendFolderName.Text

        If Right(txtFrontendLink.Text, 1) <> "/" Then
            txtFrontendLink.Text &= "/"
        End If
    End Sub

    Protected Sub rblRequireLogin_SelectedIndexChanged(sender As Object, e As EventArgs)
        ToggleLoginColumns()
    End Sub

    Sub ToggleLoginColumns()
        pnlLoginColumns.Visible = If(rblRequireLogin.SelectedIndex = 1, True, False)
    End Sub

    Protected Sub libRemove_Click(sender As Object, e As EventArgs)
        If rptEmailRecipients.Items.Count > 1 Then
            UpdateRepeaterItems(rptEmailRecipients, rptEmailRecipients.Items.Count, dtEmailSupplied, CType(sender.parent, RepeaterItem).ItemIndex)
            UpdateEmailSettings()
        End If
    End Sub

    Protected Sub btnAddEmail_Click(sender As Object, e As EventArgs) Handles btnAddEmail.Click
        LoadEmailSupplied()

        UpdateRepeaterItems(rptEmailRecipients, rptEmailRecipients.Items.Count + 1, dtEmailSupplied)

        CType(rptEmailRecipients.Items(rptEmailRecipients.Items.Count - 1).FindControl("txtSubject"), TextBox).Text = ProjectDT.Rows(0).Item("PageTitle") & " Submission Received"

        UpdateEmailSettings()
    End Sub

    Private Sub UpdateEmailSettings()
        For Each CurrentItem As RepeaterItem In rptEmailRecipients.Items
            UpdateDefaultMessage(CurrentItem)
            rblMessageType_SelectedIndexChanged(CurrentItem.FindControl("rblMessageType"), Nothing)
            rblWorkflow_SelectedIndexChanged(CurrentItem.FindControl("rblWorkflow"), Nothing)
        Next

    End Sub

    Private Sub UpdateDefaultMessage(CurrentItem As RepeaterItem)

        CType(CurrentItem.FindControl("txtMessage"), TextBox).Text = If(CType(CurrentItem.FindControl("txtMessage"), TextBox).Text = "", "Thank you for submitting the <strong>" & ProjectDT.Rows(0).Item("PageTitle") & "</strong> form.  Your submission has been received and is now being processed.", CType(CurrentItem.FindControl("txtMessage"), TextBox).Text)
    End Sub

    Protected Sub cvEmail_ServerValidate(source As Object, args As ServerValidateEventArgs)

    End Sub


    Protected Sub lsbEmailControls_SelectedIndexChanged(sender As Object, e As EventArgs)
        CType(CType(sender.parent, RepeaterItem).FindControl("lblListEmailControls"), Label).Text = GetListOfSelectedValues(sender)
    End Sub

    Protected Sub cvToAddresses_ServerValidate(source As Object, args As ServerValidateEventArgs)
        With CType(source.parent, RepeaterItem)
            If CType(.FindControl("txtToAddress"), TextBox).Text = "" And ValidateListControl(CType(.FindControl("lsbEmailControls"), ListBox), Nothing) = False Then
                args.IsValid = False
            End If
        End With
    End Sub

    Protected Sub rblWorkflow_SelectedIndexChanged(sender As Object, e As EventArgs)
        'With CType(sender.parent, RepeaterItem)
        '    If CType(sender, RadioButtonList).SelectedValue = "1" Then
        '        CType(.FindControl("lblWorkflowLink"), Label).Text = "<br /><br />The following will be appended to the e-mail message:" & GetWorkflowLink(GetDataTable("Select WorkflowStep FROM " & DT_WEBRAD_PROJECTS & " WHERE ID = " & GetProjectID()).Rows(0).Item("WorkflowStep"))
        '    Else
        '        CType(.FindControl("lblWorkflowLink"), Label).Text = ""
        '    End If
        'End With

        With CType(sender.parent, RepeaterItem)
            CType(.FindControl("pnlWorkflowDestination"), Panel).Visible = If(CType(sender, RadioButtonList).SelectedValue = "1", True, False)
        End With
    End Sub

    Protected Sub rblMultipleSubmissions_SelectedIndexChanged(sender As Object, e As EventArgs)
        ToggleRetainedColumns()
    End Sub

    Sub ToggleRetainedColumns()
        pnlRetainedColumns.Visible = If(rblMultipleSubmissions.SelectedIndex = 1, True, False)
    End Sub


    Protected Sub ddlWorkflowDestination_SelectedIndexChanged(sender As Object, e As EventArgs)
        With CType(CType(sender.parent, Panel).FindControl("lblWorkflowLink"), Label)
            .Text = ""

            If CType(sender, DropDownList).SelectedIndex > 0 Then
                .Text = "<br /><br />The following will be appended to the e-mail message:" & GetWorkflowLink(CType(sender, DropDownList).SelectedValue)
            End If
        End With
    End Sub

    Protected Sub cblAdditionalOperationTypes_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadAdditionalOperations()
    End Sub

    Sub UpdateFolders(ByRef lsbFolder As ListBox, ByRef lblSelectedFolder As Label, ByRef pnlFolder As Panel, ByRef txtLink As TextBox, Optional ByVal sFolder As String = "")
        ImpersonateAsUser()
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Declare local variables
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim dis() As DirectoryInfo

        If sFolder <> "" And sFolder <> S_NONE Then

            Try
                dis = New DirectoryInfo(sFolder).GetDirectories
            Catch ex As Exception
                Directory.CreateDirectory(sFolder)
                dis = New DirectoryInfo(sFolder).GetDirectories
            End Try


            lblSelectedFolder.Text = sFolder

            Try
                dis = New DirectoryInfo(sFolder).GetDirectories
            Catch ex As Exception
                Try
                    Directory.CreateDirectory(sFolder)
                    dis = New DirectoryInfo(sFolder).GetDirectories
                Catch ex2 As Exception

                End Try

            End Try
        Else

            If lsbFolder.SelectedIndex > -1 Then

                Try

                    If lsbFolder.SelectedItem.Value = "return" Then

                        If lblSelectedFolder.Text = "\\web1\~whitworth" Or lblSelectedFolder.Text = "\\web2\~whitworth" Or lblSelectedFolder.Text = "\\web2\~whitworthdev" Then

                            LoadDefaultFolders(lsbFolder, lblSelectedFolder)
                            pnlFolder.Visible = lblSelectedFolder.Text <> S_NONE

                            Exit Sub
                        Else

                            dis = New DirectoryInfo(lblSelectedFolder.Text).Parent.GetDirectories

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

        If lsbFolder.SelectedIndex > -1 Or (lblSelectedFolder.Text <> "" And lblSelectedFolder.Text <> S_NONE) Then

            lsbFolder.Items.Clear()
            lsbFolder.Items.Add(New ListItem("[return to parent]", "return"))

            Try
                For nCounter As Integer = 0 To dis.GetUpperBound(0)
                    'Write(dis(nCounter).Name & "<br /><br />")
                    lsbFolder.Items.Add(New ListItem(dis(nCounter).Name, dis(nCounter).FullName))
                Next
            Catch ex As Exception
                'Empty catch statement
            End Try

            pnlFolder.Visible = lblSelectedFolder.Text <> S_NONE
        End If

        If lblSelectedFolder.Text <> "" And Not txtLink Is Nothing Then
            SetLinkBasedOnFolder(txtLink, lblSelectedFolder.Text)
        End If

        UndoImpersonateAsUser()
    End Sub

    Protected Sub rblMessageType_SelectedIndexChanged(sender As Object, e As EventArgs)
        With CType(sender.parent, RepeaterItem)
            If CType(sender, RadioButtonList).SelectedValue = "Rich" Then
                If CType(.FindControl("txtMessage"), TextBox).Text = "" Then
                    CType(.FindControl("txtMessage"), TextBox).Text = CType(.FindControl("txtPlainMessage"), TextBox).Text
                End If

                CType(.FindControl("txtMessage"), TextBox).Visible = True
                CType(.FindControl("txtPlainMessage"), TextBox).Visible = False
            Else
                If CType(.FindControl("txtPlainMessage"), TextBox).Text = "" Then
                    CType(.FindControl("txtPlainMessage"), TextBox).Text = CType(.FindControl("txtMessage"), TextBox).Text
                End If

                CType(.FindControl("txtMessage"), TextBox).Visible = False
                CType(.FindControl("txtPlainMessage"), TextBox).Visible = True
            End If
        End With
    End Sub

    Protected Sub rblConfirmationPageMessageType_SelectedIndexChanged(sender As Object, e As EventArgs)
        If rblConfirmationPageMessageType.SelectedValue = "Rich" Then
            If txtConfirmationPageMessageRich.Text = "" And txtConfirmationPageMessagePlain.Text <> "" Then
                txtConfirmationPageMessageRich.Text = txtConfirmationPageMessagePlain.Text
            End If

            pnlConfirmationMessageRich.Visible = True
            pnlConfirmationMessagePlain.Visible = False
        Else
            If txtConfirmationPageMessagePlain.Text = "" And txtConfirmationPageMessageRich.Text <> "" Then
                txtConfirmationPageMessagePlain.Text = txtConfirmationPageMessageRich.Text
            End If

            pnlConfirmationMessageRich.Visible = False
            pnlConfirmationMessagePlain.Visible = True
        End If
    End Sub

    Function GetConfirmationPageMessage() As String
        If rblConfirmationPageMessageType.SelectedValue = "Rich" Then
            Return txtConfirmationPageMessageRich.Text
        Else
            Return txtConfirmationPageMessagePlain.Text
        End If
    End Function
End Class