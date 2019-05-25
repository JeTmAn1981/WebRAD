<%@ Page Language="vb" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" MasterPageFile="~/Template.Master" CodeBehind="finalize.aspx.vb" Inherits="stable.finalizenewer" %>
<asp:Content ID="SectionName" ContentPlaceHolderID="SectionName" runat="server">Finalize</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
<asp:UpdatePanel ID="MainUpdatePanel" runat="server">
    <ContentTemplate>
        
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
            iframe.style.display = "none";
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
            let projectID = <%= Common.General.ProjectOperations.GetProjectID() %>;

            createFrontend = $("input[id*='chkCreateFrontend']")[0].checked;
            createBackend = $("input[id*='chkCreateBackend']")[0].checked;
            includeStatusPage = $("input[id*='chkIncludeStatusPage']")[0] ? $("input[id*='chkIncludeStatusPage']")[0].checked : 'false';
            projectType = $("input[id*='rblProjectType_0']")[0].checked ? "Live" : "Test";
            formsType = $("input[id*='rblFormsType_0']")[0].checked ? "WebForms" : "MVC";

            let username = '<%= WhitTools.Getter.GetCurrentUsername() %>';
            let pages = GetOptionsCollection(window.document, 'cblPages')
            let optionsAndAncillaryMaintenance = GetBackendOptions(projectID);
                        
            console.log(pages);

            WebRAD.WebService.SaveBuild(projectID,createFrontend, createBackend, includeStatusPage,projectType, formsType, username, pages,optionsAndAncillaryMaintenance.backendOptions,optionsAndAncillaryMaintenance.ancillaryMaintenanceSelected);
        }

        function GetBackendOptions(projectID) {
            let backendOptions = GetOptionsCollection(window.document, 'MainContent_cblBackendOptions')

               backendOptions.forEach(function(part, index, options) {
                    options[index].ProjectID = projectID;
                    options[index].BackendOptionTypeID = options[index].ID;
            });

            let optionsAndAncillary = GetAncillaryMaintenanceOptions(backendOptions);

            let backendOptionsRemovedProperties = [];
            optionsAndAncillary.backendOptions.forEach(function (part, index, options) {
                backendOptionsRemovedProperties.push({ ProjectID: options[index].ProjectID, BackendOptionTypeID: options[index].BackendOptionTypeID });
            });

            return { backendOptions: backendOptionsRemovedProperties, ancillaryMaintenanceSelected: optionsAndAncillary.ancillaryMaintenanceSelected };
        }

        function GetAncillaryMaintenanceOptions(backendOptions) {
            let ancillaryMaintenanceSelected = [];
            let rptrAncillaryMaintenance = $("div[id*='MainContent_rptAncillaryMaintenance_AncillaryMaintenanceContainer']");
            
            $(rptrAncillaryMaintenance).map((index, ancillaryMaintenance) => {
                if ($(ancillaryMaintenance).find('input[id*="chkSelect"]')[0].checked) {
                    ancillaryMaintenanceSelected.push({ ID: $(ancillaryMaintenance).find('input[id*="AncillaryProjectMaintenanceID"]')[0].value });

                    let ancillaryOptions = GetOptionsCollection(ancillaryMaintenance, 'cblBackendOptions');
                    let projectID = $(ancillaryMaintenance).find('[id*="AncillaryProjectID"]')[0].value;
                
                    ancillaryOptions.forEach(function(part, index, options) {
                    options[index].ProjectID = projectID;
                    options[index].BackendOptionTypeID = options[index].ID;
                    
                    backendOptions.push(options[index]);
                });
                }
            });

            console.log(ancillaryMaintenanceSelected);
            console.log(backendOptions);

            return { backendOptions: backendOptions, ancillaryMaintenanceSelected: ancillaryMaintenanceSelected };
        }


        function GetOptionsCollection(parent, controlName) {
            var index = 0, collection = [];

            $(parent).find("input[id*='" + controlName + "']").map((index, checkbox) =>
            {
                if (checkbox.checked) {
                    collection.push({ ID: checkbox.value, Included: true });
                }
            });

            return collection;

            //var currentOption = $(parent).find("input[id*='" + controlName + '_' + index + "']")[0];
            //console.log(parent);
            //console.log("input[id*='" + controlName + '_' + index + "']");
            //console.log('current option', currentOption);
            //while (currentOption != null)
            //{
            //    collection[index] = { Included: currentOption.checked, ID: currentOption.value };
            //    index++;

            //    currentOption = $(parent).find("input[id*='" + controlName + '_' + index + "']")[0];
            //}
            
            //return collection;
        }

        WebRAD.WebService.Test(result);
        
        function result(blah) {
            console.log(blah);
        }

        //$(document).ready(() => SaveBuild());
        
    </script>
   
 <h3><asp:label ID="lblPageTitle" runat="server" ></asp:label>
</h3>
<br /><br />
<div class="stack-container">
    <div class="stack">
        <strong>Department Name</strong> 
		<br />
		<asp:Label ID="lblDepartmentName" runat="server"></asp:Label>
        
    </div>
    <div class="stack">
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
		
 		
    </div>
  
</div>
  <div class="stack-container">
          <div class="stack">
              <br /><br />
            <strong>SQL</strong> <a href='javascript:void' onclick='ToggleDetail("pnlShowSQLDetails","ShowSQLDetails","HideSQLDetails")' id='ShowSQLDetails'>(show details)</a><a href='javascript:void' onclick='ToggleDetail("pnlShowSQLDetails","ShowSQLDetails","HideSQLDetails")' id='HideSQLDetails' style='display:none;'>(hide details)</a>
   
             <div id="pnlShowSQLDetails" style="display: none; border-style: solid " >
                
                <div style="margin:20; display: flex; justify-content: space-between; flex-wrap:wrap">
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
         </div>
   
        <div class="stack">
        <br /><br />
        <strong>Control List</strong>  <a href='javascript:void' onclick='ToggleDetail("pnlShowControlListDetails","ShowControlListDetails","HideControlListDetails")' id='ShowControlListDetails'>(show details)</a><a href='javascript:void' onclick='ToggleDetail("pnlShowControlListDetails","ShowControlListDetails","HideControlListDetails")' id='HideControlListDetails' style='display:none;'>(hide details)</a>
           
            <div id="pnlShowControlListDetails" style="display: none;">
                <br />
        
                <asp:Label ID="lblControlList" runat="server"></asp:Label>
                </div>
     
        </div>
      </div>
            
            <br /><br />
        <div class="stack-container">
            <div class="stack">
                <strong>Frontend Link</strong><br /><asp:label ID="lblFrontendLink" runat="server" Text="Frontend"></asp:label>
            </div>
            <div class="stack">
                <strong>Backend Link</strong><br /><asp:label ID="lblBackendLink" runat="server" Text="Backend"></asp:label>
            </div>
        </div>
 
 
        <br />
        <br />
        <h3>Build Options</h3>
        <br /><br />
        <div class="stack-container">
            <div class="stack">
                <strong>Frontend</strong>
                <br />
                    <asp:CheckBox ID="chkCreateFrontend" runat="server" Checked="true" Text="Create frontend now?" />
        
            <div style="padding-left:30px">
                <asp:CheckBox ID="chkIncludeStatusPage" runat="server" Text="Status Page" />
                <asp:CheckBoxList ID="cblPages"  runat="server"></asp:CheckBoxList>
            </div>

        
            </div>
            <div class="stack">
                <strong>Backend</strong>
                <br />
                    <asp:CheckBox ID="chkCreateBackend" runat="server" Checked="true" Text="Create backend now?" />
            <div style="padding-left:33px">
                <%--<input type="checkbox" id="chkBackendOptionsAll" onchange="ToggleOptions('cblBackendOptions', this)" value="All"  />--%>
                <%--<label for="chkBackendOptionsAll">All</label>--%>
                <asp:CheckBox ID="chkBackendOptionsAll" runat="server" OnCheckedChanged="chkBackendOptionsAll_CheckedChanged" AutoPostBack="true" />
                <asp:Label runat="server" AssociatedControlID="chkBackendOptionsAll">All</asp:Label>
                
                
            </div>
            <div style="padding-left:30px">
                <asp:CheckBoxList ID="cblBackendOptions" RepeatColumns="3"  runat="server" AutoPostBack="true"></asp:CheckBoxList>
            </div>
            <asp:Panel ID="pnlAncillaryMaintenance" runat="server" Visible="false">
                <br />
                <div  style="padding-left:30px">
                <strong>Ancillary Maintenance</strong>
                    <br /><br />
                    <div style="padding-left:3px;">
                
                        <asp:CheckBox ID="chkAncillaryMaintenanceAll" runat="server" OnCheckedChanged="chkAncillaryMaintenanceAll_CheckedChanged"  AutoPostBack="true"/>
                <asp:Label runat="server" AssociatedControlID="chkAncillaryMaintenanceAll">All</asp:Label>
                </div>
                                       
                    
                    <asp:Repeater ID="rptAncillaryMaintenance" runat="server" ItemType="Common.ProjectAncillaryMaintenance">
                        <ItemTemplate>
                            <div runat="server" id="AncillaryMaintenanceContainer">
                            <asp:CheckBox ID="chkSelect"  AutoPostBack="true" OnCheckedChanged="chkSelect_CheckedChanged" runat="server" />
                            <asp:Label runat="server" AssociatedControlID="chkSelect"><%# Item.ShortName %></asp:Label>
                          
                            <asp:Panel ID="pnlShowBackendOptions" runat="server" Visible="false">
                            <div style="padding-left:33px">
       
                                     <br />
               <asp:CheckBox ID="chkBOAll" runat="server" OnCheckedChanged="chkBOAll_CheckedChanged" AutoPostBack="true" />
                <asp:Label runat="server" AssociatedControlID="chkBOAll">All</asp:Label>
                
                
            </div>
            <div style="padding-left:30px">
                <asp:CheckBoxList ID="cblBackendOptions" ItemType="Common.ProjectBackendOption"  RepeatColumns="3"  runat="server" AutoPostBack="true"></asp:CheckBoxList>
            </div>
                            <hr />
                                </asp:Panel>
                                <input type="hidden" value="<%# Item.AncillaryProjectID %>" id="AncillaryProjectID" runat="server" />
                                <input type="hidden" value="<%# Item.ID %>" id="AncillaryProjectMaintenanceID" runat="server" />
                                <asp:Label ID="lblAncillaryProjectID" runat="server" Visible="false" Text="<%# Item.AncillaryProjectID %>"></asp:Label>
                                <asp:Label ID="lblAncillaryProjectMaintenanceID" runat="server" Visible="false" Text="<%# Item.ID %>"></asp:Label>
                                
                                </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    
                    
                </div>
            </asp:Panel>
        
            </div>
        </div>
            <br />
        <br />
        <div class="stack-container">
            <div class="stack">
                <strong>Project Type</strong>
                <br />
                <asp:RadioButtonList ID="rblProjectType" runat="server" RepeatDirection="horizontal" AutoPostBack="true">
            <asp:ListItem Value="Live" Selected="true">Live</asp:ListItem>
            <asp:ListItem Value="Test">Test</asp:ListItem>
        </asp:RadioButtonList>

            </div>
            <div class="stack" style="display:none;">
                <strong>Form Type</strong>
                <br />
        <asp:RadioButtonList ID="rblFormsType" runat="server" RepeatDirection="horizontal" AutoPostBack="true">
            <asp:ListItem Value="WebForms" Selected="true">Web Forms</asp:ListItem>
            <asp:ListItem Value="MVC">MVC</asp:ListItem>
        </asp:RadioButtonList>

            </div>
        </div>

        
        
        <br /><br /><br />
        To finalize the creation of this project, please click the Submit button.  

		<div style="text-align:center">
		    <input type="button" value="Submit" class="button" onclick="BeginProcess()" />
            		    
        
		</div>

<div id="CopyCurrentStep"></div>
<br />
    <div id="CopyLog"></div>
    <div style="text-align:center">
		    <asp:ValidationSummary ID="vsWebRAD" runat="server" />
		</div>		

        <script>
            function ToggleOptions(targetID, selectAllValue) {
                $('#' + targetID + ' input[type="checkbox"]').prop('checked',$(selectAllValue).prop('checked'));
            }

            function getParameterByName(name) {
                name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
                var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                    results = regex.exec(location.search);
                return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
            }


        </script>

        		<script src="http://www.whitworth.edu/js/webradapps.js"></script>
        
	</ContentTemplate>
</asp:UpdatePanel>            	
        
</asp:Content>