i<%@ Page Language="vb" AutoEventWireup="false" MaintainScrollPositionOnPostback="true" Codebehind="interfacetest.aspx.vb" Debug="true" Inherits="stable.interfacetest" ValidateRequest="false" %>
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
            projectStyle = document.getElementById('rblProjectStyle_0').checked ? "Old" : "New";
                  
            WebRAD.WebService.SaveBuild(<%= Common.General.ProjectOperations.GetProjectID()%>,createFrontend, createBackend, projectType, formsType, projectStyle, getPages());
              }

              function getPages() {
                  var pageIndex = 0, pages = [];

                  while (document.getElementById('cblPages_' + pageIndex) != null) {
                      pages[pageIndex] = { bIncluded: document.getElementById('cblPages_' + pageIndex).checked, nID: document.getElementById('cblPages_' + pageIndex).value };
                      pageIndex++;
                  }

                  return pages;
              }

              WebRAD.WebService.Test(result);
        
              function result(blah) {
                  console.log(blah);
              }

     </script>
            <link rel="stylesheet" href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.11.4/themes/smoothness/jquery-ui.css">
<script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.11.4/jquery-ui.min.js"></script>
            <script src="https://cdnjs.cloudflare.com/ajax/libs/lodash.js/4.14.1/lodash.min.js"></script>
            <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/gridstack.js/0.2.5/gridstack.min.css" />
<script type="text/javascript" src='//cdnjs.cloudflare.com/ajax/libs/gridstack.js/0.2.5/gridstack.min.js'></script>
            
            
            <div class="grid-stack">
    <div class="grid-stack-item"
        data-gs-x="0" data-gs-y="0"
        data-gs-width="1" data-gs-height="1">
            <div class="grid-stack-item-content">
                          <div  style="height:100px; width: 100px; border: 1px solid black;">
                        <span id="FirstName">First Name</span>
                    </div>
          
            </div>
    </div>
    <div class="grid-stack-item"
        data-gs-x="4" data-gs-y="0"
        data-gs-width="1" data-gs-height="1">
            <div class="grid-stack-item-content">
                          <div  style="height:100px; width: 100px; border: 1px solid black;">
                        <span id="LastName">Last Name</span>
                    </div>
          
            </div>
    </div>
                <div class="grid-stack-item"
        data-gs-x="8" data-gs-y="0"
        data-gs-width="1" data-gs-height="1">
            <div class="grid-stack-item-content">
                          <div  style="height:100px; width: 100px; border: 1px solid black;">
                        <span id="blah">Blah</span>
                    </div>
          
            </div>
    </div>
</div>

<script type="text/javascript">
$(function () {
    var options = {
        cellHeight: "auto",
        verticalMargin: 10
        
    };
    $('.grid-stack').gridstack(options);
});
</script>
            

              <div id="DragTest" style="height:400px">
                    <div id="FirstName" style="height:200px; width: 100%; border: 1px solid black;">
                        <span>Panel</span>
                    </div>
                    <div  id="LastName" style="height:40px; width: 100%; border: 1px solid black;">
                        <span>Last Name</span>
                    </div>
                  <div  id="Address" style="height:40px; width: 100%; border: 1px solid black;">
                        <span>Address</span>
                    </div>

                </div>
            
            <div id="DragTest2">
                <div  style="height:200px; width: 200px; border: 1px solid black;">
                        <span id="FirstName2">First Name</span>
                    </div>
                    <div  style="height:40px; width: 40px; border: 1px solid black;">
                        <span id="LastName2">Last Name</span>
                    </div>
            </div>
                <script>
                    $('#DragTest').sortable({
                        containment: "parent"//,
                        //connectWith: "#DragTest div"
                    });
                    //$('#FirstName').sortable();
                    $('#FirstName').droppable({
                        drop: function( event, ui ) {
                            var droppable = $(this);
                            var draggable = ui.draggable;
                            console.log(droppable);
                            if(!$(this).has('#' + $(draggable).prop('id')).length);
                            {
                                console.log('appending');
                                // Move draggable into droppable
                                $(draggable).droppable("disable");
                                draggable.appendTo(droppable);
                                draggable.css({ top: '0px', left: '0px' });
                            }
                        }
                    });
                    //$('#DragTest div').draggable({
                    //    axis: "y",
                    //    grid: [300,20],
                    //    containment: "parent"});
                </script>

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