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
Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports System.Security.Principal

Partial Public Class frontendnewpagetest
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

        'WriteTabStuff()
    End Sub

    Private Shared Sub WriteTabStuff()
        Dim tabs As String = ""
        Dim declarations As String = ""

        For Each aot In db.AdditionalOperationTypes
            declarations &= "Public Const " & String.Join("_", aot.Name.ToUpper().Split(" ")) & "_TYPE As Integer = " & aot.ID & vbCrLf

            tabs &= "<ajaxToolkit:TabPanel runat=""server"" HeaderText=""" & aot.Name & """ ID=""" & RemoveNonAlphanumeric(aot.Name) & """>" & vbCrLf
            tabs &= "            <ContentTemplate>" & vbCrLf
            tabs &= "<asp:Repeater ID=""rpt" & RemoveNonAlphanumeric(aot.Name) & """ runat=""server"">" & vbCrLf
            tabs &= "<ItemTemplate>" & vbCrLf
            tabs &= "<br />" & vbCrLf
            tabs &= "<strong>Page <%# Container.DataItem(""PageNumber"") %></strong> " & vbCrLf
            tabs &= "<br />" & vbCrLf
            tabs &= "<asp:TextBox ID=""txtOperation"" runat=""server"" cssclass=""MlText"" Rows=""5"" TextMode=""MultiLine"" Columns=""100""></asp:TextBox>" & vbCrLf
            tabs &= "<asp:Label ID=""lblPageID"" runat=""server"" Visible=""false"" Text='<%# Container.DataItem(""PageID"")%>'></asp:Label>" & vbCrLf
            tabs &= "</ItemTemplate>" & vbCrLf
            tabs &= "</asp:Repeater>" & vbCrLf
            tabs &= "</ContentTemplate>" & vbCrLf
            tabs &= "</ajaxToolkit:TabPanel>" & vbCrLf
        Next

        WriteTextFile(tabs, "tabs.txt")
        WriteTextFile(declarations, "declarations.txt")
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

        'hitTools.Email.SendEmail("tryan@whitworth.edu", "blah", HttpContext.Current.GetProjectID())
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
        Dim projectselectstring = "Select P.*,  DB.Name as SQLDBName, S.Name as SQLServerName From " & DT_WEBRAD_PROJECTS & "  P left outer join " & DT_WEBRAD_SQLDATABASES & "  DB on P.SQLDatabase = DB.ID left outer join " & DT_WEBRAD_SQLSERVERS & " S on DB.ServerID = S.ID Where P.ID = " & GetProjectID()

        ProjectDT = GetDataTable(projectselectstring, cnx)
        LoadDefaultFolders(lsbFrontendFolders, lblSelectedFrontendFolder)

        CreateLoginColumnTypes()
        FillListData(lsbLoginColumns, GetDataTable("Select * FROM " & DT_WEBRAD_LOGINCOLUMNTYPES & " WHERE Frontend = 1 ORDER BY [Order] asc"), "Heading", "IDNumber")
        FillListData(cblAdditionalOperationTypes, GetDataTable("SELECT * FROM " & DT_WEBRAD_ADDITIONALOPERATIONTYPES & " ORDER BY PerPage desc, [Order]"), "Name", "ID", False)

        LoadEmailSupplied()

        UpdateRepeaterItems(rptEmailRecipients, 1, dtEmailSupplied)
        CType(rptEmailRecipients.Items(0).FindControl("txtSubject"), TextBox).Text = ProjectDT.Rows(0).Item("PageTitle") & " Submission Received"
        UpdateEmailSettings()

        Try
            FillListData(lsbMainColumns, GetExportColumns(-1, False), "Name", "ColumnControlID", False)
        Catch ex As Exception

        End Try

        lsbMainColumns.Items.Insert(0, New ListItem("All", "0"))
        GetAdditionalExportColumns(rptTables)

        lblMainTableName.Text = GetDataTable(cnx, "Select * From " & DT_WEBRAD_PROJECTS & "  Where ID = " & GetProjectID()).Rows(0).Item("SQLMainTableName")
    End Sub

    Sub LoadEmailSupplied()
        Dim emailControlSelect As String = "Select ID, Name, Pageid, Position From " & DT_WEBRAD_PROJECTCONTROLS & " Where ControlType IN (Select ID FROM " & DT_WEBRAD_CONTROLTYPES & "  Where DataType IN (1,2,3)) AND ProjectID = " & GetProjectID()

        For counter As Integer = 0 To lsbLoginColumns.Items.Count - 1
            If lsbLoginColumns.Items(counter).Text = "Email" Then
                emailControlSelect &= " UNION select " & lsbLoginColumns.Items(counter).Value & " as id, '" & CleanSQL(lsbLoginColumns.Items(counter).Text) & "' as Name, 0 as pageid, " & counter & " as position "
            End If
        Next

        emailControlSelect &= " ORDER BY PageID asc, Position asc "



        dtEmailSupplied.Rows.Clear()
        dtEmailSupplied.AddRow("lsbEmailControls", "SQLSelect", emailControlSelect, "Name", "ID")
        dtEmailSupplied.AddRow("ddlWorkflowDestination", "SQLSelect", "Select distinct(ID), Name FROM " & DT_WORKFLOWPOSITIONS & " WHERE ID IN (SELECT Destination FROM " & DT_WORKFLOWTYPESTEPTRIGGERCONTROLS & " TC Where StepID=" & ProjectDT.Rows(0).Item("WorkflowStep") & ")", "Name", "ID")
    End Sub

    Sub BindData()
        If GetProjectID() <> "0" Then
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

                        SetItemSelected(rblEmailSubmitter, .Item("SubmitterEmail"))

                        BindEmailOptions()

                        chkCustomConfirmationPageMessage.Checked = .Item("CustomConfirmationPageMessage")
                        chkCustomConfirmationPageMessage_CheckedChanged(chkCustomConfirmationPageMessage, Nothing)
                        SetItemSelected(rblConfirmationPageMessageType, .Item("ConfirmationMessageType"))
                        txtConfirmationPageMessageRich.Text = .Item("ConfirmationMessage")
                        txtConfirmationPageMessagePlain.Text = .Item("ConfirmationMessage")
                        rblConfirmationPageMessageType_SelectedIndexChanged(Nothing, Nothing)

                        chkCustomStatusPagemessage.Checked = .Item("CustomStatusPageMessage")
                        chkCustomStatusPagemessage_CheckedChanged(chkCustomStatusPagemessage, Nothing)
                        SetItemSelected(rblStatusPageMessageType, .Item("StatusMessageType"))
                        txtStatusPageMessageRich.Text = .Item("StatusMessage")
                        txtStatusPageMessagePlain.Text = .Item("StatusMessage")
                        rblStatusPageMessageType_SelectedIndexChanged(Nothing, Nothing)
                    End If
                End With
            End If

            BindPages()

            SetListControlItemSelected(cblAdditionalOperationTypes, "", True, GetDataTable("SELECT Distinct(TYPE) FROM " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " WHERE len(AdditionalOperations) > 0 AND PageID IN (SELECT ID FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & GetProjectID() & ")"), "Type")

            cblAdditionalOperationTypes_SelectedIndexChanged(cblAdditionalOperationTypes, Nothing)

            Dim dtSupervisors As DataTable = GetDataTable("Select * From ProjectSupervisors Where ProjectID = '" & GetQueryString("ID") & "'", cnx)

            If dtSupervisors.Rows.Count > 0 Then
                SelectRepeaterData(rptSupervisors, GetDataTable("Select * From ProjectSupervisors Where ProjectID = '" & GetQueryString("ID") & "'", cnx), cnx)
            End If

            For Each CurrentItem1 As RepeaterItem In rptSupervisors.Items
                For Each CurrentControl1 As Control In CurrentItem1.Controls
                    RunSupervisorsControlEvents(CurrentControl1)
                Next
            Next
        End If
    End Sub

    Private Sub BindEmailOptions()
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
    End Sub

    Private Sub BindPages()
        Dim dtPages As DataTable = GetDataTable("Select * From " & DT_WEBRAD_PROJECTPAGES & "  Where ProjectID = " & GetProjectID())

        If dtPages.Rows.Count > 1 Then
            Dim dtSupplied As New WhitTools.DataTablesSupplied()

            SelectRepeaterData(rptPages, dtPages, dtSupplied, cnx)
            SetPageSelectionControls(dtPages)

            UpdatePageControls()

            SetItemSelected(rblMultipleSubmissions, ProjectDT.Rows(0).Item("MultipleSubmissions"))
            SetListControlItemSelected(lsbMainColumns, "", True, GetDataTable("Select * From " & DT_WEBRAD_PROJECTCOLUMNS & "  Where Type = 'Retained' AND ProjectID = " & GetProjectID() & " and TableControlID = 0", cnx), "ColumnControlID")

            For Each CurrentItem As RepeaterItem In rptTables.Items
                SetListControlItemSelected(CType(CurrentItem.FindControl("lsbColumns"), ListBox), "", True, GetDataTable("Select * From " & DT_WEBRAD_PROJECTCOLUMNS & "  Where Type='Retained' AND ProjectID = " & GetProjectID() & " and TableControlID = " & CType(CurrentItem.FindControl("lblID"), Label).Text, cnx), "ColumnControlID")
            Next

            ToggleRetainedColumns()
            pnlPages.Visible = True
            pnlShowCustomStatusPageMessage.Visible = True
        End If
    End Sub

    Private Sub SetPageSelectionControls(ByVal pages As DataTable)
        For counter As Integer = 0 To rptPages.Items.Count - 1
            FillListData(CType(rptPages.Items(counter).FindControl("ddlSelectionControl"), DropDownList), GetDataTable(cnx, "SELECT (CASE WHEN COALESCE(Purpose,'') <> '' THEN Purpose + ' - ' ELSE '' end) + Name as Name, PC.ID FROM " & DT_WEBRAD_PROJECTPAGES & " pp join " & DT_WEBRAD_PROJECTCONTROLS & " pc on pp.ID = pc.pageid WHERE pp.ProjectID=" & GetProjectID() & " AND PP.ID < " & CType(rptPages.Items(counter).FindControl("lblID"), Label).Text & " AND ControlType in (Select ID from ControlTypes where DataType in (Select ID from ControlDataTypes where ProvidesValue = 1)) and not ParentControlID IN (Select ID FROM ProjectControls PC2 Where PC2.PageID = PC.ID AND ControlType IN (Select ID FROM ControlTypes Where DataType = 9)) ORDER BY pageID, position"), "Name", "ID")
            SetItemSelected(CType(rptPages.Items(counter).FindControl("ddlSelectionControl"), DropDownList), pages.Rows(counter).Item("selectionControl"))
        Next
    End Sub

    Sub LoadAdditionalOperations()
        For Each operationType In db.AdditionalOperationTypes
            LoadAdditionalOperationType(operationType)
        Next
    End Sub

    Private Function GetTab(typeName As String) As AjaxControlToolkit.TabPanel
        Return CType(AdditionalOperationTabs.FindControl(typeName), AjaxControlToolkit.TabPanel)
    End Function

    Private Function GetAdditionalOperationRepeater(typeName As String) As Repeater
        Return CType(GetTab(typeName).FindControl("rpt" & typeName), Repeater)
    End Function
    Private Sub LoadAdditionalOperationType(ByVal type As Common.AdditionalOperationType)
        Dim typeName As String = RemoveNonAlphanumeric(type.Name)
        Dim currentRPT As Repeater = GetAdditionalOperationRepeater(typeName)

        If type.PerPage Then
            If currentRPT IsNot Nothing Then
                SelectRepeaterData(currentRPT, GetDataTable("select P.ID as PageID, CASE WHEN O.AdditionalOperations IS NULL THEN '' ELSE O.AdditionalOperations END Operation, (select PageNumber from (select ID, rank() OVER(PARTITION BY ProjectID ORDER BY ID) as PageNumber from " & DT_WEBRAD_PROJECTPAGES & " where projectID = " & GetProjectID() & ") as PageRank WHERE ID = P.ID) as PageNumber from " & DT_WEBRAD_PROJECTPAGES & " P LEFT OUTER JOIN " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " O ON (O.Type = " & type.ID & " AND O.PageID = P.ID) where ProjectID=" & GetProjectID() & " ORDER BY P.Id"))
            End If
        Else
            Dim operationText As String = ""

            For Each currentrow As DataRow In GetDataTable("select P.ID as PageID, CASE WHEN O.AdditionalOperations IS NULL THEN '' ELSE O.AdditionalOperations END Operation, (select PageNumber from (select ID, rank() OVER(PARTITION BY ProjectID ORDER BY ID) as PageNumber from " & DT_WEBRAD_PROJECTPAGES & " where projectID = " & GetProjectID() & ") as PageRank WHERE ID = P.ID) as PageNumber from " & DT_WEBRAD_PROJECTPAGES & " P LEFT OUTER JOIN " & DT_WEBRAD_PROJECTADDITIONALOPERATIONS & " O ON (O.Type = " & type.ID & " AND O.PageID = P.ID) where ProjectID=" & GetProjectID() & " ORDER BY P.Id").Rows
                operationText &= currentrow.Item("Operation")
            Next

            Dim textboxName = "txt" & New Regex("[^\w]").Replace(type.Name, "")

            Try
                CType(FindControlOnPage(GetCurrentPage(), textboxName), TextBox).Text = operationText
            Catch ex As Exception
                WriteLine(ex.ToString)
                WriteLine(type.Name)
                WriteLine(textboxName)
            End Try

        End If
    End Sub

    Function FindControlOnPage(ByRef parent As Control, ByVal controlID As String) As Control
        If parent.ID = controlID Then
            Return parent
        Else
            For Each childControl As Control In parent.Controls
                Dim returnedControl = FindControlOnPage(childControl, controlID)

                If returnedControl IsNot Nothing Then
                    Return returnedControl
                End If
            Next
        End If

        Return Nothing
    End Function

    Function CheckControlsComplete()
        Dim ncounter As Integer
        Dim dt As New DataTable

        If GetProjectID() <> "" Then
            dt = GetDataTable(cnx, "Select * From " & DT_WEBRAD_PROJECTCONTROLS & " Where ProjectID = " & GetProjectID())

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

            Redirect("backend.aspx?ID=" & GetProjectID())
        End If
    End Sub

    Private Sub SaveAncillaryFrontendInfo()
        SaveEmailMessages()
        SaveSupervisorsItems()
        SavePages()
        SaveAdditionalOperations()
        SaveColumns()
    End Sub

    Private Sub SaveMainFrontendInfo()
        Dim cmd As New SqlCommand
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_UpdateProjectFrontend"
        cmd.Connection = cnx

        cmd.Parameters.AddWithValue("@ID", GetProjectID())
        cmd.Parameters.AddWithValue("@IncludeFrontend", rblIncludeFrontend.SelectedValue)
        cmd.Parameters.AddWithValue("@RequireLogin", rblRequireLogin.SelectedValue)
        cmd.Parameters.AddWithValue("@EmailSupervisor", rblEmailSupervisor.SelectedValue)
        cmd.Parameters.AddWithValue("@FrontendLink", FormatProjectLink(txtFrontendLink.Text))
        cmd.Parameters.AddWithValue("@FrontendPath", lblSelectedFrontendFolder.Text)
        cmd.Parameters.AddWithValue("@CustomConfirmationPageMessage", chkCustomConfirmationPageMessage.Checked)
        cmd.Parameters.AddWithValue("@ConfirmationMessageType", rblConfirmationPageMessageType.SelectedValue)
        cmd.Parameters.AddWithValue("@ConfirmationMessage", GetConfirmationPageMessage())
        cmd.Parameters.AddWithValue("@CustomStatusPageMessage", chkCustomStatusPagemessage.Checked)
        cmd.Parameters.AddWithValue("@StatusMessageType", rblStatusPageMessageType.SelectedValue)
        cmd.Parameters.AddWithValue("@StatusMessage", GetStatusPageMessage())

        If pnlPages.Visible Then
            cmd.Parameters.AddWithValue("@MultipleSubmissions", rblMultipleSubmissions.SelectedValue)

            ExecuteNonQuery("Delete FROM " & DT_WEBRAD_PROJECTCOLUMNS & " WHERE Type='Retained' AND ProjectID = " & GetProjectID())

            SaveColumnsInfo("Retained", lsbMainColumns, pnlRetainedColumns.Visible, rptTables)
        End If

        If pnlNewFrontendFolder.Visible = True Then
            cmd.Parameters.AddWithValue("@FrontendNewFolder", txtNewFrontendFolderName.Text)
        End If

        cmd.Parameters.AddWithValue("@SubmitterEmail", rblEmailSubmitter.SelectedValue)

        ExecuteNonQuery("Update " & DT_WEBRAD_PROJECTCONTROLS & " Set SubmitterEmail = 0 Where ProjectID = " & GetProjectID(), cnx)

        ExecuteNonQuery(cmd, cnx)
    End Sub

    Private Sub SaveColumns()
        ExecuteNonQuery("Delete FROM " & DT_WEBRAD_PROJECTCOLUMNS & " WHERE Type='Login' AND ProjectID = " & GetProjectID())
        SaveColumnsInfo("Login", lsbLoginColumns, pnlLoginColumns.Visible)
    End Sub

    Private Sub SavePages()
        If pnlPages.Visible Then
            For Each CurrentItem As RepeaterItem In rptPages.Items
                UpdatePage(CurrentItem)
            Next
        End If
    End Sub


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

        cmd.Parameters.AddWithValue("@ProjectID", GetProjectID())
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

        For Each aot As Common.AdditionalOperationType In db.AdditionalOperationTypes.ToList().Where(Function(operationType) cblAdditionalOperationTypes.Items.FindByValue(operationType.ID).Selected)
            cmd.Parameters("@Type").Value = aot.ID

            If aot.PerPage Then
                For Each CurrentItem As RepeaterItem In GetAdditionalOperationRepeater(RemoveNonAlphanumeric(aot.Name)).Items
                    With CurrentItem
                        cmd.Parameters("@PageID").Value = CType(.FindControl("lblPageID"), Label).Text
                        cmd.Parameters("@AdditionalOperations").Value = CType(.FindControl("txtOperation"), TextBox).Text

                        ExecuteNonQuery(cmd)
                    End With
                Next
            Else
                cmd.Parameters("@PageID").Value = GetFirstPage()

                Dim textboxName = "txt" & New Regex("[^\w]").Replace(aot.Name, "")

                cmd.Parameters("@AdditionalOperations").Value = CType(FindControlOnPage(GetCurrentPage(), textboxName), TextBox).Text

                ExecuteNonQuery(cmd)
            End If
        Next
    End Sub

    Protected Sub rblIncludeFrontend_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblIncludeFrontend.SelectedIndexChanged
        showFrontend()
    End Sub

    Sub showFrontend()
        pnlFrontend.Visible = (rblIncludeFrontend.SelectedValue = 1)
        pnlConfirmationPageMessage.Visible = (rblIncludeFrontend.SelectedValue = 1)
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

        Dim detailOptionsSelected As Integer

        For Each currentItem As ListItem In cblAdditionalOperationTypes.Items
            Dim currentTab = (From tab As AjaxControlToolkit.TabPanel In AdditionalOperationTabs.Tabs
                              Where tab.HeaderText = currentItem.Text
                              Select tab).FirstOrDefault()

            If currentTab IsNot Nothing Then
                currentTab.Visible = currentItem.Selected
                detailOptionsSelected += 1
            End If
        Next
    End Sub

    Sub UpdateFolders(ByRef lsbFolder As ListBox, ByRef lblSelectedFolder As Label, ByRef pnlFolder As Panel, ByRef txtLink As TextBox, Optional ByVal sFolder As String = "")
        'Dim wIdCon As WindowsImpersonationContext = DirectCast(HttpContext.Current.User.Identity, WindowsIdentity).Impersonate()

        'Try
        '    ' ImpersonateAsUser()


        'Catch ex As Exception

        'End Try

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

        'Try
        '    '            UndoImpersonateAsUser()
        '    wIdCon.Undo()
        'Catch ex As Exception

        'End Try

    End Sub

    Protected Sub rblMessageType_SelectedIndexChanged(sender As Object, e As EventArgs)
        With CType(sender.parent, RepeaterItem)
            UpdateMessageType(CType(sender, RadioButtonList), CType(.FindControl("txtMessage"), TextBox), CType(.FindControl("txtPlainMessage"), TextBox))
        End With
    End Sub


    Protected Sub rblConfirmationPageMessageType_SelectedIndexChanged(sender As Object, e As EventArgs)
        UpdateMessageType(rblConfirmationPageMessageType, txtConfirmationPageMessageRich, txtConfirmationPageMessagePlain)
    End Sub

    Function GetConfirmationPageMessage() As String
        Return IIf(rblConfirmationPageMessageType.SelectedValue = "Rich", txtConfirmationPageMessageRich.Text, txtConfirmationPageMessagePlain.Text)
    End Function

    Function GetStatusPageMessage() As String
        Return IIf(rblStatusPageMessageType.SelectedValue = "Rich", txtStatusPageMessageRich.Text, txtStatusPageMessagePlain.Text)
    End Function

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

    <ScriptMethod, WebMethod>
    Public Shared Function GetSupervisorNameAutocompleteData(ByVal prefixText As String, ByVal count As Integer) As IEnumerable
        'Dim blah As List(Of String) = New List(Of String)()
        'blah.Add("Test")
        'Return blah
        'Try
        Return (From currentRow In GetDataTable("SELECT * FROM adTelephone.dbo.UserInfo_V WHERE IDNumber in (select plID from adTelephone.dbo.PeopleListing where PLActive='1') AND [User] like '%" & prefixText & "%' AND [User] LIKE '%" & prefixText & "%' ORDER BY PLLName, plfName").Rows
                Select currentRow.item("User")).ToList()
        'Catch ex As Exception
        '    WriteTextFile(ex.ToString, "autocompleteerror.txt")
        'End Try

    End Function

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

    Sub SaveSupervisorsItems()
        Dim nCounter As Integer
        Dim sCurrentIds As String = ""
        Dim cmd As New SqlCommand

        cmd.Connection = cnx
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_InsertProjectSupervisor"

        For nCounter = 0 To rptSupervisors.Items.Count - 1
            With rptSupervisors.Items(nCounter)
                cmd.Parameters.Clear()

                cmd.Parameters.AddWithValue("@ProjectID", GetQueryString("ID"))
                cmd.Parameters.AddWithValue("@ID", CType(.FindControl("lblID"), Label).Text)

                If CType(.FindControl("rblSupervisorType"), RadioButtonList).SelectedIndex > -1 Then
                    cmd.Parameters.AddWithValue("@SupervisorType", CType(.FindControl("rblSupervisorType"), RadioButtonList).SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@SupervisorType", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("@SupervisorName", CType(.FindControl("txtSupervisorName"), TextBox).Text)
                cmd.Parameters.AddWithValue("@SupervisorEmail", CType(.FindControl("txtSupervisorEmail"), TextBox).Text)

                Dim nCurrentID As Integer

                nCurrentID = ExecuteScalar(cmd, cnx, "tryan")

                sCurrentIds &= If(sCurrentIds <> "", ",", "") & nCurrentID
            End With
        Next

        ExecuteNonQuery("DELETE FROM ProjectSupervisors WHERE (ProjectID = " & GetQueryString("ID") & If(sCurrentIds <> "", " AND NOT ID IN (" & sCurrentIds & ")", "") & ")", cnx)
    End Sub

    Protected Sub cvSupervisorName_ServerValidate(source As Object, args As ServerValidateEventArgs)
        With GetParentRepeaterItem(source)
            If CType(.FindControl("txtSupervisorName"), TextBox).Text = "" Then
                CType(source, CustomValidator).ErrorMessage = "Please enter the username for this supervisor."
                args.IsValid = False
            ElseIf CType(.FindControl("rblSupervisorType"), RadioButtonList).SelectedValue = "SingleUser" AndAlso GetUserInfo(ExtractUsername(CType(.FindControl("txtSupervisorName"), TextBox).Text)).Rows.Count = 0 Then
                CType(source, CustomValidator).ErrorMessage = "Sorry, that username was not found.  Please enter another username."
                args.IsValid = False
            End If
        End With
    End Sub

    Public Sub cvPages_ServerValidate(ByVal source As System.Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
    End Sub

    Public Sub rptPages_cvCertification_ServerValidate(ByVal source As System.Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
    End Sub


    Public Sub Page_Detail_Changed(sender As Object, e As EventArgs)
        UpdatePage(GetParentRepeaterItem(sender))
    End Sub

    Public Sub rptPages_chkDependent_SelectedIndexChanged(sender As Object, e As EventArgs)
        ShowSelectionControl(sender)

        UpdatePage(GetParentRepeaterItem(sender))
    End Sub

    Private Shared Sub ShowSelectionControl(sender As Object)
        Dim parentRepeateritem = GetParentRepeaterItem(sender)

        With parentRepeateritem
            Try
                CType(.FindControl("SelectionControlContainer"), Panel).Visible = CType(.FindControl("chkDependent"), CheckBox).Checked
            Catch ex As Exception
                CType(.FindControl("SelectionControlContainer"), Panel).Visible = False
            End Try
        End With
    End Sub

    Public Sub rptPages_ddlSelectionControl_SelectedIndexChanged(sender As Object, e As EventArgs)
        ShowSelectionValue(sender)

        UpdatePage(GetParentRepeaterItem(sender))
    End Sub

    Private Sub ShowSelectionValue(sender As Object)
        Dim parentRepeateritem = GetParentRepeaterItem(sender)

        With parentRepeateritem
            Try
                CType(.FindControl("SelectionValueContainer"), Panel).Visible = CType(.FindControl("ddlSelectionControl"), DropDownList).SelectedIndex > 0
            Catch ex As Exception
                CType(.FindControl("SelectionValueContainer"), Panel).Visible = False
            End Try
        End With
    End Sub

    Protected Sub btnrptPagesAddItem_Click(sender As Object, e As EventArgs)
        Dim dtSupplied As New WhitTools.DataTablesSupplied

        dtSupplied.AddRow("ddlSelectionControl", "SQLSelect", "SELECT (CASE WHEN COALESCE(Purpose,'') <> '' THEN Purpose + ' - ' ELSE '' end) + Name as Name, PC.ID FROM projectpages pp join ProjectControls pc on pp.ID = pc.pageid WHERE pp.ProjectID=" & GetProjectID() & " AND ControlType in (Select ID from ControlTypes where DataType in (Select ID from ControlDataTypes where ProvidesValue = 1)) and not ParentControlID IN (Select ID FROM ProjectControls PC2 Where PC2.PageID = PC.ID AND ControlType IN (Select ID FROM ControlTypes Where DataType = 9)) ORDER BY pageID, position", "Name", "ID")
        AddNewRepeaterItem(rptPages, dtSupplied, 1, 0, cvPages, "page")

        PageItemsChanged()
    End Sub

    Protected Sub librptPages_RemoveItem_Click(sender As Object, e As EventArgs)
        Dim ParentRepeaterItem As RepeaterItem = GetParentRepeaterItem(sender)
        Dim dtSupplied As New WhitTools.DataTablesSupplied

        dtSupplied.AddRow("ddlSelectionControl", "SQLSelect", "SELECT (CASE WHEN COALESCE(Purpose,'') <> '' THEN Purpose + ' - ' ELSE '' end) + Name as Name, PC.ID FROM projectpages pp join ProjectControls pc on pp.ID = pc.pageid WHERE pp.ProjectID=" & GetProjectID() & " AND ControlType in (Select ID from ControlTypes where DataType in (Select ID from ControlDataTypes where ProvidesValue = 1)) and not ParentControlID IN (Select ID FROM ProjectControls PC2 Where PC2.PageID = PC.ID AND ControlType IN (Select ID FROM ControlTypes Where DataType = 9)) ORDER BY pageID, position", "Name", "ID")
        RemoveRepeaterItem(ParentRepeaterItem.Parent, dtSupplied, CType(ParentRepeaterItem, RepeaterItem).ItemIndex, , cvPages, "page")

        ExecuteNonQuery("DELETE FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ID = " & CType(ParentRepeaterItem.FindControl("lblID"), Label).Text)

        PageItemsChanged()
    End Sub

    Private Sub PageItemsChanged()
        SavePages()
        BindPages()
        UpdatePageControls()
    End Sub

    Private Sub UpdatePageControls()
        For Each currentItem As RepeaterItem In rptPages.Items
            If currentItem.ItemIndex = rptPages.Items.Count - 1 Then
                CType(currentItem.FindControl("CertificationContainer"), Panel).Visible = True
            Else
                CType(currentItem.FindControl("chkCertification"), CheckBox).Checked = False
            End If

            CType(currentItem.FindControl("DependentContainer"), Panel).Visible = DependentAllowed(currentItem, currentItem.ItemIndex)

            For Each CurrentControl1 As Control In currentItem.Controls
                RunPagesControlEvents(CurrentControl1)
            Next
        Next
    End Sub

    Public Sub RunPagesControlEvents(ByRef CurrentControl As Control)
        If CurrentControl.ID = "chkDependent" Then
            ShowSelectionControl(CurrentControl)
        ElseIf CurrentControl.ID = "ddlSelectionControl" Then
            ShowSelectionValue(CurrentControl)
        End If

        If CurrentControl.Controls.Count > 0 Then
            For Each CurrentControl2 As Control In CurrentControl.Controls
                RunPagesControlEvents(CurrentControl2)
            Next
        End If
    End Sub

    Private Sub UpdatePage(pageItem As RepeaterItem)
        Dim showAllowed As Boolean = DependentAllowed(pageItem, pageItem.ItemIndex)

        CType(pageItem.FindControl("DependentContainer"), Panel).Visible = showAllowed

        If Not showAllowed Then
            CType(pageItem.FindControl("chkDependent"), CheckBox).Checked = False
            CType(pageItem.FindControl("SelectionControlContainer"), Panel).Visible = False
        End If

        Try
            Dim currentPage As Common.ProjectPage

            Try
                currentPage = db.ProjectPages.ToList().FirstOrDefault(Function(pp) pp.ID = CType(pageItem.FindControl("lblID"), Label).Text)

            Catch ex As Exception

            End Try

            If currentPage Is Nothing Then
                currentPage = New Common.ProjectPage()
                currentPage.ProjectID = GetQueryString("ID")

                db.ProjectPages.Add(currentPage)
            End If

            currentPage.Purpose = CType(pageItem.FindControl("txtPurpose"), TextBox).Text
            currentPage.Certification = If(CType(pageItem.FindControl("chkCertification"), CheckBox).Checked, 1, 0)
            currentPage.Dependent = CType(pageItem.FindControl("chkDependent"), CheckBox).Checked

            Dim selectionControl As DropDownList = CType(pageItem.FindControl("ddlSelectionControl"), DropDownList)

            currentPage.SelectionControl = CInt(If(selectionControl.SelectedIndex > 0, CInt(selectionControl.SelectedValue), -1))
            currentPage.SelectionValue = CType(pageItem.FindControl("txtSelectionValue"), TextBox).Text

            db.SaveChanges()
        Catch ex As Exception
            WriteLine(ex.ToString)
        End Try

    End Sub

    Protected Sub chkCustomStatusPagemessage_CheckedChanged(sender As Object, e As EventArgs)
        pnlStatusPageMessage.Visible = chkCustomStatusPagemessage.Checked
    End Sub

    Protected Sub rblStatusPageMessageType_SelectedIndexChanged(sender As Object, e As EventArgs)
        txtStatusPageMessageRich.Visible = rblStatusPageMessageType.SelectedValue = "Rich"
        txtStatusPageMessagePlain.Visible = Not rblStatusPageMessageType.SelectedValue = "Rich"
    End Sub

    Protected Sub chkCustomConfirmationPageMessage_CheckedChanged(sender As Object, e As EventArgs)
        pnlConfirmationPageMessage.Visible = chkCustomConfirmationPageMessage.Checked
    End Sub

    Protected Sub lsbLoginColumns_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadEmailSupplied()
        UpdateRepeaterItems(rptEmailRecipients, rptEmailRecipients.Items.Count, dtEmailSupplied)
    End Sub

    Public Function DependentAllowed(ByVal dependentContainer As Object, ByVal index As Integer) As Boolean
        Return index <> 0 And Not CType(GetParentRepeaterItem(dependentContainer).FindControl("chkCertification"), CheckBox).Checked
    End Function
End Class