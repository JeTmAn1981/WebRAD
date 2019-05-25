<%@ Page Language="vb" AutoEventWireup="false" Codebehind="confirmation.aspx.vb" Inherits="stable.confirmation" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<!DOCTYPE html>
<html>
	<head>
			<title>Whitworth University Communications - Web-RAD</title>
	    <!-- #include virtual="/~Templates/ASPX_Templates/Web2_Base_1_Head.htm" -->
	        <style type="text/css">
                .Button
                {
                    height: 26px;
                }
            </style>
            <style type="text/css">
 .DragHandleClass
 {
 width: 12px;
 height: 12px;
 background-color: red;
 cursor:move;
 }
</style>

	</head>
	
	<body>
	    <!-- #include virtual="/~Templates/ASPX_Templates/Web2_Base_2.htm" -->
	    University Communications
	    <!-- #include virtual="/~Templates/ASPX_Templates/Web2_Base_3.htm" -->
	    <span class="SmText">
		    <a href="http://web2/Administration/InstitutionalAdvancement/UniversityCommunications/index.htm">Communications</a>&nbsp;&gt;
	    </span>
		<h1>Web-RAD</h1>
	    <form id="Form1" runat="server">
		<!-- Page Main START -->
        <dl>
        
 
 <br /><br />
 Your project has now been created.  Listed below are links to the frontend and backend in their live locations:
 <br /><br />
 Frontend - <asp:label ID="lblFrontendLink" runat="server" Text="Frontend"></asp:label>
 <br /><br />
 Backend - <asp:label ID="lblBackendLink" runat="server" Text="Backend"></asp:label>

 <br /><br /><br />
 <span class="MainHeaders">Project Details</span>
 <br /><br />
 <br />
	    <strong>Page Title</strong> 
		<br />
		<asp:label ID="lblPageTitle" runat="server" ></asp:label>
		
		<br />
		<br />
	    <strong>Department Name</strong> 
		<br />
		<asp:Label ID="lblDepartmentName" runat="server"></asp:Label>
		
		<br />
		<br />
	    <strong>Is this an e-commerce form?</strong>
		<br />
        <asp:Label ID="lblEcommerce" runat="server"></asp:Label>
		
		<asp:Panel ID="pnlEcommerce" runat="server" Visible="false">
		<br />
		<br />
	    <strong>E-Commerce Product</strong>
		<br />
		<asp:Label ID="lblEcommerceProduct" runat="server"></asp:Label>
		
		</asp:Panel>
		
		<br />
		<br />
		<strong>SQL Database</strong>
		<br />
		<asp:Label ID="lblSQLDatabase" runat="server"></asp:Label>
			
		<br />
		<br />
		<strong>SQL Main Table Name</strong>
		<br />
		<asp:label ID="lblSQLMainTableName" runat="server"></asp:label>
	
	    <br />
		<br />
		<strong>SQL Insert Stored Procedure Name</strong>
		<br />
		usp_Insert<asp:label ID="lblSQLInsertStoredProcedureName" runat="server"></asp:label>
		
		<br />
		<br />
		<strong>SQL Update Stored Procedure Name</strong>
		<br />
		usp_Update<asp:label ID="lblSQLUpdateStoredProcedureName" runat="server"></asp:label>

        <br /><br />
        <strong>Control List</strong>
        <br />
        <asp:Label ID="lblControlList" runat="server"></asp:Label>
		
		
  		</dl>
		<!-- Page Main END -->
	    <!-- #include virtual="/~Templates/ASPX_Templates/Web2_Base_5_Footer.htm" -->
    </body>
</html>

