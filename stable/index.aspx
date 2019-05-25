<%@ Page Title="Whitworth University Communications - Web-RAD" Language="vb" AutoEventWireup="false" MasterPageFile="~/Template.Master" MaintainScrollPositionOnPostback="true" ValidateRequest="false" CodeBehind="index.aspx.vb" Inherits="stable.indexnew" %>
<asp:Content ID="SectionName" ContentPlaceHolderID="SectionName" runat="server">Main Details</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
      <br style="clear:both"/>

      <div class="form-group">
      <asp:label runat=server AssociatedControlID="ddlCurrentProject">Current Project</asp:label>
      <asp:DropDownList ID="ddlCurrentProject" AutoPostBack="true" runat="server" />
          
        
     	<asp:RequiredFieldValidator ID="rfvPageTitle" runat="server" ControlToValidate="txtPageTitle" ErrorMessage="Please enter the page title." CssClass="error"></asp:RequiredFieldValidator>
		<asp:label runat=server AssociatedControlID="txtPageTitle" class="required">Page Title</asp:label>
		<asp:TextBox ID="txtPageTitle" runat="server"  CssClass="SlText" MaxLength="100"></asp:TextBox>
		

		<asp:RequiredFieldValidator CssClass="error" ID="rfvDepartmentName" runat="server" ControlToValidate="ddlDepartmentName" ErrorMessage="Please select the department name."></asp:RequiredFieldValidator>
		<asp:label runat=server AssociatedControlID="ddlDepartmentName" class="required">Department Name</asp:label>
		<asp:DropDownList ID="ddlDepartmentName"  runat="server"></asp:DropDownList>
          
        <asp:Label runat="server" ID="lblOptions" AssociatedControlID="cblOptions">Options</asp:Label>
        <fieldset>
            <asp:checkboxlist ID="cblOptions" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cblOptions_OnSelectedIndexChanged" RepeatDirection="horizontal" RepeatColumns="4">
                <asp:listitem Value="CustomDepartmentName">Custom Department Header</asp:listitem>
                <asp:listitem Value="Ecommerce">E-commerce</asp:listitem>
                <asp:listitem Value="Workflow">Workflow</asp:listitem>
        
            </asp:checkboxlist>
        </fieldset>
          
            <asp:Panel ID="pnlCustomDepartmentName" runat="server" Visible="false">
                <asp:RequiredFieldValidator CssClass="error" ID="rfvCustomDepartmentName" runat="server" ControlToValidate="txtCustomDepartmentName" ErrorMessage="Please enter the custom department name."></asp:RequiredFieldValidator>
                <asp:label runat=server AssociatedControlID="txtCustomDepartmentName" class="required">Custom Department Header</asp:label>
                <asp:TextBox ID="txtCustomDepartmentName" runat="server"  CssClass="SlText" MaxLength="100" />
            </asp:Panel>

        <asp:Panel ID="pnlEcommerce" runat="server" Visible="false">
		<asp:RequiredFieldValidator CssClass="error" ID="rfvEcommerceProduct" runat="server" ControlToValidate="ddlEcommerceProduct" ErrorMessage="Please select the e-commerce product."></asp:RequiredFieldValidator>
		<asp:label runat=server AssociatedControlID="ddlEcommerceProduct" class="required">E-Commerce Product</asp:label>
		<asp:DropDownList ID="ddlEcommerceProduct" runat="server"></asp:DropDownList>
		

        	<asp:RequiredFieldValidator ID="rfvDefaultCharge" runat="server" ControlToValidate="rblDefaultCharge" ErrorMessage="Please indicate whether this e-commerce form should include a default charge." CssClass="error"></asp:RequiredFieldValidator>
		<asp:label runat=server AssociatedControlID="rblDefaultCharge" class="required">Include a default charge?</asp:label>
            <fieldset>
		<asp:radiobuttonlist ID="rblDefaultCharge" runat="server" AutoPostBack="true"  RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" >
		    <asp:ListItem Value="0" selected="True">No</asp:ListItem>
		    <asp:ListItem Value="1">Yes</asp:ListItem>
		</asp:RadioButtonList>
                </fieldset>
		
        <asp:Panel ID="pnlDefaultCharge" runat="server" Visible="false">
        <asp:RequiredFieldValidator CssClass="error" ID="rfvDefaultChargeText" Display="Dynamic" runat="server" ControlToValidate="txtDefaultCharge" ErrorMessage="Please enter the default charge."></asp:RequiredFieldValidator>
        <asp:CompareValidator CssClass="error" ID="cmpDefaultChargeText" runat="server" ControlToValidate="txtDefaultCharge" Operator="DataTypeCheck" Type="Double" ErrorMessage="Please enter the default charge using only numbers and a decimal point."></asp:CompareValidator>
		<asp:label runat=server AssociatedControlID="txtDefaultCharge" class="required">Default Charge</asp:label>
		<asp:TextBox ID="txtDefaultCharge" runat="server" Width="50" CssClass="SlText" MaxLength="10"></asp:TextBox>
		</asp:Panel>
		</asp:Panel>
          <asp:Panel ID="pnlWorkflow" runat="server" Visible="false">
                <asp:RequiredFieldValidator CssClass="error" ID="rfvWorkflowStep" runat="server" ControlToValidate="ddlWorkflowStep" ErrorMessage="Please select the workflow step."></asp:RequiredFieldValidator>
		        <asp:label runat=server AssociatedControlID="ddlWorkflowStep" class="required">Please select the workflow step to which this form belongs:</asp:label>
                <asp:DropDownlist ID="ddlWorkflowStep" runat="server"></asp:DropDownlist>
            </asp:Panel>
		
        

        <asp:RequiredFieldValidator CssClass="error" ID="rfvSQLServer" runat="server" ControlToValidate="ddlSQLServer" ErrorMessage="Please select the SQL server."></asp:RequiredFieldValidator>
		<asp:label runat=server AssociatedControlID="ddlSQLServer" class="required">SQL Server</asp:label>
		<asp:DropDownList ID="ddlSQLServer" AutoPostBack="true" OnSelectedIndexChanged="ddlSQLServer_SelectedIndexChanged" runat="server">
            <asp:ListItem Value="web3">Web3</asp:ListItem>
            <asp:ListItem Value="emr">EMR</asp:ListItem>
		</asp:DropDownList>
		

		<asp:RequiredFieldValidator CssClass="error" ID="rfvSQLDatabase" runat="server" ControlToValidate="ddlSQLDatabase" ErrorMessage="Please select the SQL database."></asp:RequiredFieldValidator>
		<asp:label runat=server AssociatedControlID="ddlSQLDatabase" class="required">SQL Database</asp:label>
		<asp:DropDownList ID="ddlSQLDatabase" runat="server"></asp:DropDownList>
		
		<asp:RequiredFieldValidator CssClass="error" ID="rfvSQLMainTableName" runat="server" ControlToValidate="txtSQLMainTableName" ErrorMessage="Please enter the SQL main table name."></asp:RequiredFieldValidator>
		<asp:label runat=server AssociatedControlID="txtSQLMainTableName" class="required">SQL Main Table Name</asp:label>
		<asp:TextBox ID="txtSQLMainTableName" runat="server"  CssClass="SlText" MaxLength="100"></asp:TextBox>
		
	    <asp:RequiredFieldValidator CssClass="error" ID="rfvSQLInsertStoredProcedureName" runat="server" ControlToValidate="txtSQLInsertStoredProcedureName" ErrorMessage="Please enter the SQL insert stored procedure name."></asp:RequiredFieldValidator>
		<asp:label runat=server AssociatedControlID="txtSQLInsertStoredProcedureName" class="required">SQL Insert Stored Procedure Name</asp:label> (do not include prefix or action) 
		<asp:TextBox ID="txtSQLInsertStoredProcedureName" runat="server"  CssClass="SlText" MaxLength="100"></asp:TextBox>
		
          
		<asp:RequiredFieldValidator CssClass="error"  ID="rfvSQLUpdateStoredProcedureName" runat="server" ControlToValidate="txtSQLUpdateStoredProcedureName" ErrorMessage="Please enter the SQL update stored procedure name."></asp:RequiredFieldValidator>
		<asp:label runat=server class="required" AssociatedControlID="txtSQLUpdateStoredProcedureName">SQL Update Stored Procedure Name</asp:label> (do not include prefix or action)
		<asp:TextBox ID="txtSQLUpdateStoredProcedureName" runat="server"  CssClass="SlText" MaxLength="100"></asp:TextBox>
		

		<asp:label runat=server AssociatedControlID="txtSQLAdditionalCertificationStatement">SQL Additional Certification Statement</asp:label> 
		<asp:TextBox ID="txtSQLAdditionalCertificationStatement" runat="server"  CssClass="SlText" MaxLength="100"></asp:TextBox>
			</div>
		
		<div style="text-align:center">
		<asp:Button ID="btnContinue" runat="server" Text="Continue" cssclass="button" />
		</div>
		<div style="text-align:center">
		    <asp:ValidationSummary ID="vsWebRAD" runat="server" />
		</div>
            </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

