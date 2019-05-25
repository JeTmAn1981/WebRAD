li<%@ Page Language="vb" AutoEventWireup="false" MaintainScrollPositionOnPostback="true" Codebehind="finalizeold.aspx.vb" Debug="true" Inherits="stable.finalize" ValidateRequest="false" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<%--<%@ Register assembly="EO.Web" namespace="EO.Web" tagprefix="eo" %>--%>
<%--<%@ Register TagPrefix="uc" TagName="TopMenu" Src="Common/TopMenu.ascx" %>--%>
<!DOCTYPE html>
<html>
	<head>
			<title>Whitworth University Communications - Web-RAD</title>
        <% Response.WriteFile("\\web2\~whitworth\~Templates\ASPX_Templates\Web2_Base_1_Head.htm")%>
	    
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

 #pnlShowSQLDetails div {
     margin: 15px;
     
 }
</style>

          
           	</head>
	
	<body>

	    <% Response.WriteFile("\\web2\~whitworth\~Templates\ASPX_Templates\Web2_Base_2.htm") %>
	    University Communications
	    <% Response.WriteFile("\\web2\~whitworth\~Templates\ASPX_Templates\Web2_Base_3.htm") %>
	    <span class="SmText">
		    <a href="http://web2/Administration/InstitutionalAdvancement/UniversityCommunications/index.htm">Communications</a>&nbsp;&gt;
	    </span>
		<form id="Form1" runat="server">
		

		<!-- Page Main START -->
                <%--<uc:TopMenu ID="topMenu" runat="server" />         --%>

<asp:ScriptManager runat="server" ID="scriptManager">
                <Services>
                    <asp:ServiceReference path="WebService.asmx" />
                </Services>
            </asp:ScriptManager>
            
            

    <script>
        function BeginProcess() {
            new Promise(function(resolve, reject) {
                SaveBuild();
                resolve(1);
            }).then(function(val) {
                BuildProject();
            });
        }

        function BuildProject()
        {
            var iframe = document.createElement("iframe");
            iframe.src = "buildproject.aspx?ID=" + getParameterByName('ID');
            //iframe.style.display = "none";
            document.getElementById('CopyLog').innerHTML = '<div>Copying main project data.</div>';
            document.body.appendChild(iframe);

        }
        
        function UpdateProgress(Message, spacers) {

            console.log(Message);
            console.log("<div style='margin-left:" + (parseInt(spacers) * 50) + "px'>" + "</div>");
            document.getElementById('CopyCurrentStep').innerHTML = Message;
            document.getElementById('CopyLog').innerHTML += "<div style='margin-left:" + (parseInt(spacers) * 50) + "px'>" + Message + "</div>";
        }

        function SaveBuild() {
            var pageIndex = 0;
            var createFrontend, createBackend, projectType, formsType,projectStyle;

            createFrontend = document.getElementById('chkCreateFrontend').checked;
            createBackend = document.getElementById('chkCreateBackend').checked;
            projectType = document.getElementById('rblProjectType_0').checked ? "Live" : "Test";
            formsType = document.getElementById('rblFormsType_0').checked ? "WebForms" : "MVC";
                  
            WebRAD.WebService.SaveBuild(<%= Common.General.ProjectOperations.GetProjectID()%>,createFrontend, createBackend, projectType, formsType, GetOptionsCollection('cblPages'),GetOptionsCollection('cblBackendOptions'));
        }

        function GetOptionsCollection(controlName) {
            var index = 0, collection = [];

            while (document.getElementById(controlName + '_' + index) != null) {
                collection[index] = { Included: document.getElementById(controlName + '_' + index).checked, ID: document.getElementById(controlName + '_' + index).value };
                index++;
            }

            return collection;
        }

        WebRAD.WebService.Test(result);
        
        function result(blah) {
            console.log(blah);
        }

    </script>
            
 <br /><br />
 <span class="MainHeaders">Project Details</span>
		<br/><br/>
            <div style="display: flex; justify-content: flex-start;">
 <div style="margin-right:40px;">
 	    <strong>Page Title</strong> 
		<br />
		<asp:label ID="lblPageTitle" runat="server" ></asp:label>
      </div>
     <div>
	    <strong>Department Name</strong> 
		<br />
		<asp:Label ID="lblDepartmentName" runat="server"></asp:Label>
         </div>
            </div>
 		
		
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
		
            <div>
                <br/>
            <strong>SQL</strong> <a href='javascript:void' onclick='ToggleDetail("pnlShowSQLDetails","ShowSQLDetails","HideSQLDetails")' id='ShowSQLDetails'>(show details)</a><a href='javascript:void' onclick='ToggleDetail("pnlShowSQLDetails","ShowSQLDetails","HideSQLDetails")' id='HideSQLDetails' style='display:none;'>(hide details)</a>
            </div>
            <div id="pnlShowSQLDetails" style="display: none; border-style: solid " >
                
                <div style="display: flex; justify-content: flex-start; flex-wrap:wrap">
                    <div>
        <strong>Server</strong>
		<br />
		<asp:Label ID="lblSQLServer" runat="server"></asp:Label>
		                
                    </div>
                    <div>
        <strong>Database</strong>
		<br />
		<asp:Label ID="lblSQLDatabase" runat="server"></asp:Label>
		                
                    </div>
                    <div>
        <strong>Main Table Name</strong>
		<br />
		<asp:label ID="lblSQLMainTableName" runat="server"></asp:label>
	                
                    </div>
                    <div>
    	<strong>Insert Stored Procedure Name</strong>
		<br />
		usp_Insert<asp:label ID="lblSQLInsertStoredProcedureName" runat="server"></asp:label>
	                    
                    </div>
                    <div>
    	<strong>Update Stored Procedure Name</strong>
		<br />
		usp_Update<asp:label ID="lblSQLUpdateStoredProcedureName" runat="server"></asp:label>
                        
                    </div>
                    </div>
                </div>

        <br />
        <strong>Control List</strong>  <a href='javascript:void' onclick='ToggleDetail("pnlShowControlListDetails","ShowControlListDetails","HideControlListDetails")' id='ShowControlListDetails'>(show details)</a><a href='javascript:void' onclick='ToggleDetail("pnlShowControlListDetails","ShowControlListDetails","HideControlListDetails")' id='HideControlListDetails' style='display:none;'>(hide details)</a>
           
            <div id="pnlShowControlListDetails" style="display: none;">
                <br />
        
                <asp:Label ID="lblControlList" runat="server"></asp:Label>
                </div>
            
            <br /><br />
 <strong>Frontend Link</strong> - <asp:label ID="lblFrontendLink" runat="server" Text="Frontend"></asp:label>
 <br /><br />
 <strong>Backend Link</strong> - <asp:label ID="lblBackendLink" runat="server" Text="Backend"></asp:label>


        <br /><br />
        <strong>Notes</strong>
        <br />
        <asp:TextBox ID="txtNotes" runat="server" CssClass="MlText" TextMode="MultiLine" Rows="5" Columns="100"></asp:TextBox>
        
        <br />
        <br />
            <asp:CheckBox ID="chkCreateFrontend" runat="server" Checked="true" Text="Create frontend now?" />
        
            <div style="padding-left:30px">
                <asp:CheckBoxList ID="cblPages"  runat="server"></asp:CheckBoxList>
            </div>

        <br />
        <br />
            <asp:CheckBox ID="chkCreateBackend" runat="server" Checked="true" Text="Create backend now?" />
            <div style="padding-left:33px">
                <input type="checkbox" id="chkBackendOptionsAll" onchange="ToggleOptions(this)" value="All"  />
                <label for="chkBackendOptionsAll">All</label>
                
            </div>
            <div style="padding-left:30px">
                <asp:CheckBoxList ID="cblBackendOptions" RepeatColumns="3"  runat="server"></asp:CheckBoxList>
            </div>
            <br />
        <br />
        <asp:RadioButtonList ID="rblProjectType" runat="server" RepeatDirection="horizontal" AutoPostBack="true">
            <asp:ListItem Value="Live" Selected="true">Create Live Project</asp:ListItem>
            <asp:ListItem Value="Test">Create Test Project</asp:ListItem>
        </asp:RadioButtonList>

            <br />
        <asp:RadioButtonList ID="rblFormsType" runat="server" RepeatDirection="horizontal" AutoPostBack="true">
            <asp:ListItem Value="WebForms" Selected="true">Web Forms</asp:ListItem>
            <asp:ListItem Value="MVC">MVC</asp:ListItem>
        </asp:RadioButtonList>

        
        
        <br /><br /><br />
        To finalize the creation of this project, please click the Submit button.  

		<div style="text-align:center">
		    <input type="button" value="Submit" class="Button" onclick="BeginProcess()" />
		</div>

<div id="CopyCurrentStep"></div>
<br />
    <div id="CopyLog"></div>

		
            <div style="text-align:center">
		    <asp:ValidationSummary ID="vsWebRAD" runat="server" />
		</div>		
		<!-- Page Main END -->
            </form>
	    <% Response.WriteFile("\\web2\~whitworth\~Templates\ASPX_Templates\Web2_Base_5_Footer.htm") %>
        <script>
            
            function ToggleOptions(selectAll) {
                $('#cblBackendOptions input[type="checkbox"]').prop('checked',$(selectAll).prop('checked'));
            }
            function getParameterByName(name) {
                name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
                var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                    results = regex.exec(location.search);
                return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
            }


        </script>

        		<script src="http://www.whitworth.edu/js/webradapps.js"></script>

 <% Response.WriteFile("\\web2\~whitworth\~Templates\ASPX_Templates\Web2_Base_5_Footer.htm") %>
 
    </body>
</html>