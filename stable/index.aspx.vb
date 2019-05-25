Imports System
Imports System.Security.Cryptography
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.Web
Imports System.Net
Imports System.IO
Imports Common
Imports WhitTools.RulesAssignments
Imports WhitTools.eCommerce
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.File
Imports WhitTools.Filler
Imports WhitTools.Utilities
Imports WhitTools.Email
Imports WhitTools.DataTables
Imports WhitTools.Converter
Imports WhitTools.Validator
Imports WhitTools.Formatter
Imports WhitTools.GlobalEnum
Imports WhitTools.Repeaters
Imports WhitTools.SQL
Imports WhitTools.Encryption
Imports WhitTools.Workflow
Imports WhitTools.eCommerceStatusChecks
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.ProjectOperations

Imports CsQuery.ExtensionMethods
Imports System.Web.UI.WebControls


'Public Class WebRADControl
'    Public key As String
'    Public item As ContextMenuItem
'End Class

'Public Class ContextMenuItem
'    Public name As String
'End Class

Partial Public Class indexnew
    Inherits System.Web.UI.Page

    Shared cnx As SqlConnection = CreateSQLConnection("WebRAD")


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then
            LoadDDLs()
            BindData()
        End If


        'Dim ecommercecnx As SqlConnection = CreateSQLConnection("eCommerce")
        'Dim sBody, sEmail As String
        'Dim dt As DataTable = GetDataTable("select distinct(email_addr) from web_invoices i left outer join web_customer c on i.inv_customer_id = c.customer_id where not email_Addr='' and not email_Addr='tryan@whitworth.edu' and inv_date >= '2013-11-05 10:37:44.490' and inv_date <= '11/5/2013 3:00:00 PM'", ecommercecnx)
        'Dim dtCustomer As DataTable

        'For Each Currentrow As DataRow In dt.Rows
        '    sEmail = Currentrow.Item("email_addr")

        '    dtCustomer = GetDataTable("Select top 1 * FROM Web_Invoices i left outer join web_customer c on i.inv_customer_id = c.customer_id left outer join WEB_Payments p on i.Inv_No = p.Invoice left outer join WEB_InvoiceType IT on i.Inv_Type = IT.ID WHERE email_addr = '" & sEmail & "' and inv_date >= '2013-11-05 10:37:44.490' and inv_date <= '11/5/2013 3:00:00 PM'", ecommercecnx)

        '    With dtCustomer.Rows(0)
        '        sBody = "Dear <b>" & .Item("FIRST_NAME") & " " & .Item("LAST_NAME") & "</b>:<br /><br />It has come our attention that you may have experienced an error while attempting to make an e-commerce payment via a Whitworth webpage on <b>11/5/2013</b>.  This was due to a failure on the part of our e-commerce provider U.S. Bank's online e-payment page.  This error has now been resolved.  Previously we were not able to take your online payment due to this outage, but if you would like to make your payment for <b>" & .Item("Description") & "</b> in the amount of <b>" & String.Format("{0:c}", .Item("Total")) & "</b> now you may do so by clicking the link below, which will take you to the U.S. Bank e-payment page.  Thank you for your patience during this outage. If you have any questions about this process, please e-mail Senior Web Programmer Tom Ryan at <a href='mailto:tryan@whitworth.edu'>tryan@whitworth.edu</a><br /><br />"
        '        sBody &= "<a href='" & GetRedirectURL(.Item("Inv_No")) & "'>Make Online Payment</a>"

        '    End With

        '    SendEmail(sEmail, "Whitworth E-Payment Troubleshooting", sBody, "tryan@whitworth.edu", "", "tryan@whitworth.edu")
        'Next

        'Dim dt As DataTable = GetDataTable("Select * FROM web3.adHumanResources.dbo.EDPBusinesses", cnx)

        'For Each CurrentRow As DataRow In dt.Rows
        '    'WriteLine(EncryptTextExternal(CurrentRow.Item("ID")))
        'Next

        'CopyProject(280)
    End Sub

    Sub LoadDDLs()
        FillInvoiceTypes(ddlEcommerceProduct, True)
        FillDepartments(ddlDepartmentName)
        FillListData(ddlSQLServer, cnx, "Select * From " & DT_WEBRAD_SQLSERVERS, "Name", "ID", False)
        FillListData(ddlSQLDatabase, cnx, "Select * From " & DT_WEBRAD_SQLDATABASES & " Where ServerID = " & ddlSQLServer.SelectedValue & " order by Name", "Name", "ID")

        FillListData(ddlCurrentProject, GetDataTable("Select * from " & DT_WEBRAD_PROJECTS & " WHERE ID > 0 Order By PageTitle asc", cnx), "PageTitle", "ID", True, "New Project")
        FillListData(ddlWorkflowStep, GetDataTable("Select S.ID, P.Name FROM " & DT_WORKFLOWTYPESTEPS & " S left outer join " & DT_WORKFLOWPOSITIONS & " P on S.Position = P.ID Order by TypeID asc, s.ID asc"), "Name", "ID")
    End Sub

    Sub BindData()
        Dim dt As New DataTable

        dt = GetDataTable(cnx, "Select * From " & DT_WEBRAD_PROJECTS & "  Where ID = " & GetProjectID())

        If dt.Rows.Count > 0 Then
            With dt.Rows(0)
                SetItemSelected(ddlCurrentProject, GetProjectID())
                txtPageTitle.Text = .Item("PageTitle")

                SetItemSelected(ddlDepartmentName, .Item("Department"))
                SelectProjectOptions()
                txtCustomDepartmentName.Text = .Item("CustomDepartmentName")
                SetItemSelected(ddlEcommerceProduct, .Item("EcommerceProduct"))


                If pnlEcommerce.Visible Then
                    If .Item("DefaultCharge") <> "" Then
                        SetItemSelected(rblDefaultCharge, 1)
                        txtDefaultCharge.Text = .Item("DefaultCharge")
                        pnlDefaultCharge.Visible = True
                    Else
                        SetItemSelected(rblDefaultCharge, 0)
                    End If
                End If

                SetItemSelected(ddlWorkflowStep, .Item("WorkflowStep"))

                Try
                    SetItemSelected(ddlSQLServer, GetDataTable("Select ServerID From " & DT_WEBRAD_SQLDATABASES & " WHERE ID = " & .Item("SQLDatabase"), cnx).Rows(0).Item("ServerID"))
                    UpdateSQLDatabases()
                    SetItemSelected(ddlSQLDatabase, .Item("SQLDatabase"))

                Catch ex As Exception

                End Try

                txtSQLMainTableName.Text = .Item("SQLMainTableName")
                txtSQLInsertStoredProcedureName.Text = .Item("SQLInsertStoredProcedureName")
                txtSQLUpdateStoredProcedureName.Text = .Item("SQLUpdateStoredProcedureName")
                txtSQLAdditionalCertificationStatement.Text = .Item("SQLAdditionalCertificationStatement")
            End With
        End If

    End Sub

    Function SaveProjectDetails() As Integer
        Dim cmd As New SqlCommand

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "usp_CreateProject"
        cmd.Connection = cnx

        If GetProjectID() <> -1 Then
            cmd.Parameters.AddWithValue("@ID", GetProjectID())
        End If

        cmd.Parameters.AddWithValue("@PageTitle", txtPageTitle.Text)
        cmd.Parameters.AddWithValue("@Department", ddlDepartmentName.SelectedItem.Value)

        If pnlCustomDepartmentName.Visible = True Then
            cmd.Parameters.AddWithValue("@CustomDepartmentName", txtCustomDepartmentName.Text)
        End If

        cmd.Parameters.AddWithValue("@EcommerceProduct", ddlEcommerceProduct.SelectedItem.Value)

        If pnlDefaultCharge.Visible = True Then
            cmd.Parameters.AddWithValue("@DefaultCharge", txtDefaultCharge.Text)
        End If

        cmd.Parameters.AddWithValue("@WorkflowStep", ddlWorkflowStep.SelectedValue)
        cmd.Parameters.AddWithValue("@SQLDatabase", ddlSQLDatabase.SelectedItem.Value)
        cmd.Parameters.AddWithValue("@SQLMainTableName", txtSQLMainTableName.Text)
        cmd.Parameters.AddWithValue("@SQLInsertStoredProcedureName", txtSQLInsertStoredProcedureName.Text)
        cmd.Parameters.AddWithValue("@SQLUpdateStoredProcedureName", txtSQLUpdateStoredProcedureName.Text)
        cmd.Parameters.AddWithValue("@SQLAdditionalCertificationStatement", txtSQLAdditionalCertificationStatement.Text)

        Dim nCurrentID As Integer

        nCurrentID = ExecuteScalar(cmd, cnx)

        UpdateProjectOptions(nCurrentID)

        Return nCurrentID
    End Function

    Sub UpdateProjectOptions(ByVal projectID As Integer)
        db.Projects.Where(Function(project) project.ID = projectID).First().ProjectOptions =
                (From currentItem As ListItem In cblOptions.Items Where currentItem.Selected
                 Select New ProjectOption With {.ProjectID = projectID, .Value = currentItem.Value}).tolist()
        db.SaveChanges()
    End Sub
    Protected Sub rblDefaultCharge_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblDefaultCharge.SelectedIndexChanged
        pnlDefaultCharge.Visible = If(rblDefaultCharge.SelectedItem.Value = "1", True, False)
    End Sub

    Protected Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnContinue.Click
        Page.Validate()

        If Page.IsValid Then
            Dim nProjectID As Integer = SaveProjectDetails()


            Redirect("controls.aspx?ID=" & nProjectID & "&PageID=" & GetFirstPage(nProjectID))
        End If
    End Sub

    Protected Sub ddlCurrentProject_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlCurrentProject.SelectedIndexChanged
        Redirect("index.aspx" & If(ddlCurrentProject.SelectedIndex > 0, "?ID=" & ddlCurrentProject.SelectedValue, ""))
    End Sub

    Protected Sub ddlSQLServer_SelectedIndexChanged(sender As Object, e As EventArgs)
        UpdateSQLDatabases()
    End Sub

    Sub UpdateSQLDatabases()
        FillListData(ddlSQLDatabase, cnx, "Select * From " & DT_WEBRAD_SQLDATABASES & " Where ServerID = " & ddlSQLServer.SelectedValue & " order by Name", "Name", "ID")
    End Sub

    Sub CopyProject(ByVal projectID As Integer)
        Dim nNewProjectID, nNewBackendOptionID, nNewBackendOptionColumnID, nNewProjectPageID As Integer



        'dim source = db.Projects.first(function (project) project.id = projectID)

        ''Create and add clone object to context before setting its values
        'dim clone = new Project()
        'db.Projects.Add(clone)

        ''Copy values from source to clone
        'dim sourceValues = db.Entry(source).CurrentValues
        'db.Entry(clone).CurrentValues.SetValues(sourceValues)

        ''Change values of the copied entity
        'clone.PageTitle &= " - Copy"

        ''Insert clone with changes into database
        'db.SaveChanges()


        '    dim newProject as project = db.Projects.asnotracking.first(function (project) project.id = projectID)
        'newproject.PageTitle = newproject.PageTitle & " - Copy"
        'db.Projects.add(newProject)
        'db.SaveChanges()
        'exit sub

        nNewProjectID = CopyTable("Projects", projectID)

        ExecuteNonQuery("Update Projects Set PageTitle = PageTitle + ' - Copy' WHERE ID = " & nNewProjectID, cnx)

        Dim dtProjectBackendOptions As DataTable = GetDataTable("Select * From ProjectBackendOptions Where ProjectID = " & projectID, cnx)

        For Each CurrentRow As DataRow In dtProjectBackendOptions.Rows
            nNewBackendOptionID = CopyTable("ProjectBackendOptions", CurrentRow.Item("ID"))

            ExecuteNonQuery("Update ProjectBackendOptions SET ProjectID = " & nNewProjectID & " WHERE ID = " & nNewBackendOptionID, cnx)

            Dim dtProjectBackendOptionColumns As DataTable = GetDataTable("Select * FROM ProjectBackendOptionColumns Where OptionID = " & CurrentRow.Item("ID"), cnx)

            For Each CurrentRow2 As DataRow In dtProjectBackendOptionColumns.Rows
                nNewBackendOptionColumnID = CopyTable("ProjectBackendOptionColumns", CurrentRow2.Item("ID"))

                ExecuteNonQuery("Update ProjectBackendOptionColumns SET OptionID = " & nNewBackendOptionID & " Where ID =" & nNewBackendOptionColumnID, cnx)
            Next
        Next

        'ExecuteNonQuery("Insert INTO ProjectExportColumns (ProjectID, TableControlID, ColumnControlID) Select " & nNewProjectID & ",TableControlID,ColumnControlID FROM ProjectExportColumns Where ProjectID = " & projectID, cnx)
        ExecuteNonQuery("INSERT INTO ProjectSupervisors (ProjectID, SupervisorID) Select " & nNewProjectID & ",SupervisorID FROM ProjectSupervisors Where ProjectID = " & projectID, cnx)

        Dim dtProjectPages As DataTable = GetDataTable("Select * FROM ProjectPages Where ProjectID = " & projectID, cnx)

        For Each CurrentRow As DataRow In dtProjectPages.Rows
            nNewProjectPageID = CopyTable("ProjectPages", CurrentRow.Item("ID"))

            ExecuteNonQuery("Update ProjectPages SET ProjectID = " & nNewProjectID & " WHERE ID = " & nNewProjectPageID, cnx)

            CopyControls(projectID, nNewProjectID, CurrentRow.Item("ID"), nNewProjectPageID)
        Next

        If dtProjectPages.Rows.Count = 0 Then
            CopyControls(projectID, nNewProjectID)
        End If
    End Sub

    Sub CopyControls(ByVal nOldProjectID As Integer, ByVal nNewProjectID As Integer, Optional ByVal nOldProjectPageID As Integer = 0, Optional ByVal nNewProjectPageID As Integer = 0)
        Dim dtProjectControls As DataTable = GetDataTable("Select * FROM ProjectControls WHERE ProjectID = " & nOldProjectID & If(nOldProjectPageID > 0, " AND PageID =  " & nOldProjectPageID, ""), cnx)
        Dim nNewProjectControlID, nNewFileTypeAllowedID, nNewListItemID, nNewPostbackActionID, nNewActionTriggerValueID As Integer
        Dim dtControlIDs As New DataTable
        Dim tempRow As DataRow

        dtControlIDs.Columns.Add("OldID")
        dtControlIDs.Columns.Add("NewID")

        For Each CurrentRow In dtProjectControls.Rows
            nNewProjectControlID = CopyTable("ProjectControls", CurrentRow.item("ID"))

            ExecuteNonQuery("Update ProjectControls SET ProjectID = " & nNewProjectID & " WHERE ID = " & nNewProjectControlID, cnx)

            If nOldProjectPageID > 0 Then
                ExecuteNonQuery("Update ProjectControls SET PageID = " & nNewProjectPageID & " WHERE ID = " & nNewProjectControlID, cnx)
            End If

            tempRow = dtControlIDs.NewRow
            tempRow.Item("OldID") = CurrentRow.item("ID")
            tempRow.Item("NewID") = nNewProjectControlID
            dtControlIDs.Rows.Add(tempRow)
        Next

        For Each CurrentRow In dtControlIDs.Rows
            ExecuteNonQuery("UPDATE ProjectControls Set ParentControlID = " & CurrentRow.Item("NewID") & " WHERE ProjectID = " & nNewProjectID & " AND ParentControlID = " & CurrentRow.Item("OldID"), cnx)

            Dim dtFileTypesAllowed As DataTable = GetDataTable("Select * FROM ProjectControlFileTypesAllowed Where ControlID = " & CurrentRow.Item("OldID"), cnx)

            For Each FileTypeRow As DataRow In dtFileTypesAllowed.Rows
                nNewFileTypeAllowedID = CopyTable("ProjectControlFileTypesAllowed", FileTypeRow.Item("ID"))

                ExecuteNonQuery("Update ProjectControlFileTypesAllowed SET ControlID = " & CurrentRow.Item("NewID") & " Where ID = " & nNewFileTypeAllowedID, cnx)
            Next

            Dim dtListItems As DataTable = GetDataTable("Select * FROM ProjectControlListItems Where ParentID = " & CurrentRow.Item("OldID"), cnx)

            For Each ListItemRow As DataRow In dtListItems.Rows
                nNewListItemID = CopyTable("ProjectControlListItems", ListItemRow.Item("ID"))

                ExecuteNonQuery("Update ProjectControlListItems SET ParentID = " & CurrentRow.Item("NewID") & " Where ID = " & nNewListItemID, cnx)
            Next

            '        ExecuteNonQuery("Update ProjectExportColumns SET TableControlID = " & CurrentRow.Item("NewID") & " WHERE ProjectID = " & nNewProjectID & " and TableControlID = " & CurrentRow.Item("OldID"), cnx)
            '        ExecuteNonQuery("Update ProjectExportColumns SET ColumnControlID = " & CurrentRow.Item("NewID") & " WHERE ProjectID = " & nNewProjectID & " and ColumnControlID = " & CurrentRow.Item("OldID"), cnx)
        Next

        Dim dtPostbackActions As DataTable = GetDataTable("Select * FROM ProjectControlPostbackActions Where TriggerControl IN (Select ID From ProjectControls Where ProjectID = " & nOldProjectID & ")", cnx)

        For Each PostbackActionRow As DataRow In dtPostbackActions.Rows
            nNewPostbackActionID = CopyTable("ProjectControlPostbackActions", PostbackActionRow.Item("ID"))

            Dim nNewTriggerControlID, nNewTargetControlID As Integer

            Try
                nNewTriggerControlID = CType(dtControlIDs.Select("OldID = " & PostbackActionRow.Item("TriggerControl")), DataRow())(0).Item("NewID")
                nNewTargetControlID = CType(dtControlIDs.Select("OldID = " & PostbackActionRow.Item("TargetControl")), DataRow())(0).Item("NewID")

            Catch ex As Exception

            End Try

            ExecuteNonQuery("Update ProjectControlPostbackActions SET TriggerControl = " & nNewTriggerControlID & ", TargetControl = " & nNewTargetControlID & " WHERE ID = " & nNewPostbackActionID, cnx)

            Dim dtActionTriggerValues As DataTable = GetDataTable("Select * FROM ProjectControlPostbackActionTriggerValues Where ActionID = " & PostbackActionRow.Item("ID"), cnx)

            For Each ActionTriggerValueRow As DataRow In dtActionTriggerValues.Rows
                nNewActionTriggerValueID = CopyTable("ProjectControlPostbackActionTriggerValues", ActionTriggerValueRow.Item("ID"))

                ExecuteNonQuery("Update ProjectControlPostbackActionTriggerValues SET ActionID = " & nNewPostbackActionID & " WHERE ID = " & nNewActionTriggerValueID, cnx)
            Next
        Next
    End Sub

    Function CopyTable(ByVal sTableName As String, ByVal nOldID As Integer) As Integer
        Dim sColumnList As String = GetListofValues("SELECT '[' + COLUMN_NAME + ']' as COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & sTableName & "' and not COLUMN_NAME = 'ID'", "COLUMN_NAME", ",", "", cnx)

        ExecuteNonQuery("INSERT INTO " & sTableName & " (" & sColumnList & ") SELECT " & sColumnList & " FROM " & sTableName & " Where ID = " & nOldID, cnx)

        Return ExecuteScalar("Select ident_current('" & sTableName & "')", cnx)
    End Function

    Protected Sub cblOptions_OnSelectedIndexChanged(sender As Object, e As EventArgs)
        For Each currentItem As ListItem In cblOptions.Items
            CType(GetPageControlReference("pnl" & currentItem.Value), Panel).Visible = currentItem.Selected
        Next
    End Sub

    Sub SelectProjectOptions()
        Dim projectID As Integer = GetProjectID()

        For Each currentOption As ProjectOption In db.ProjectOptions.Where(Function(projectOption) projectOption.ProjectID = projectID)
            SetListItemSelected(cblOptions, currentOption.Value)
        Next

        cblOptions_OnSelectedIndexChanged(Nothing, Nothing)
    End Sub
End Class