<%@ Page Language="vb" AutoEventWireup="false" ValidateRequest="false" CodeBehind="buildproject.aspx.vb" Inherits="stable.buildproject" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<%--<%@ Register assembly="EO.Web" namespace="EO.Web" tagprefix="eo" %>--%>
<!DOCTYPE html>
<html>
	<head>
           	</head>
	
	<body>
<form id="Form1" runat="server">
		<!-- Page Main START -->
        <dl>
       
          <table style="BACKGROUND-IMAGE: url(~images/bluebar_bk.jpg)" border="0" cellSpacing="0" cellPadding="0" width="100%">
			<tr>
				<td height="40" width="176" align="middle">
                         <asp:LinkButton causesvalidation="false" ID="libMainDetails" runat="server" CssClass="menuWhite" Text="Main Details"></asp:LinkButton>
                 </td>
                <td width="31"><IMG src="~images/arrow.png" width="31" height="42"></td>
                <td width="176" align="middle">           <asp:LinkButton causesvalidation="false" ID="libControlDetails"  runat="server" CssClass="menuWhite" Text="Control Details"></asp:LinkButton></td>
                <td width="31"><IMG src="~images/arrow.png" width="31" height="42"></td>
				
				<td width="176" align="middle">           <asp:LinkButton causesvalidation="false" ID="libFrontendDetails"  runat="server" CssClass="menuWhite" Text="Frontend Details"></asp:LinkButton></td>
				<td width="31"><IMG src="~images/arrow.png" width="31" height="42"></td>
				<td width="176" align="middle">           <asp:LinkButton causesvalidation="false" ID="libBackendDetails"  runat="server" CssClass="menuWhite" Text="Backend Details"></asp:LinkButton></td>
				<td width="31"><IMG src="~images/arrow.png" width="31" height="42"></td>
				<td width="176" align="middle"><asp:LinkButton causesvalidation="false" ID="libFinalize"  runat="server"  ForeColor="Red" CssClass="menuWhite" Text="Finalize"></asp:LinkButton></td>
				<td bgColor="#ffffff" width="137"><IMG src="~images/arrowlong.jpg" width="89" height="42"></td>
			</tr>
		</table>

            <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
<%--            <asp:UpdatePanel ID="pnlProgressArea" runat="server" UpdateMode="Always" visible="false">
                <ContentTemplate>
                    <asp:Label ID="lblProgressArea" runat="server"></asp:Label>
                      <Br />
            <eo:ProgressBar ID="ProgressBar1" ShowPercentage="true" runat="server" BorderColor="Black" BorderWidth="1px"
            Height="16px" IndicatorColor="LightBlue" Value="0" ControlSkinID="None" BorderStyle="Solid"
            Width="300px" StartTaskButton="btnTest" Maximum="10" OnRunTask="ProgressBar1_RunTask"></eo:ProgressBar>
            <asp:Button ID="btnTest" runat="server" Text="Start Test" />
                </ContentTemplate>
                 <Triggers>
                   <asp:AsyncPostBackTrigger ControlID="btnTest" />
                 </Triggers>
            </asp:UpdatePanel>--%>
            
          

<%--  <asp:ScriptManager runat="server" ID="pageScriptManager" />
              <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnDoTask" Text="Do Task" OnClick="Button1_Click" />
            <hr />
            <asp:Panel runat="server" ID="pnlProgressWrapper" CssClass="ProgressWrapper">
                <asp:Panel runat="server" ID="pnlProgress" CssClass="Progress" Visible="false" />
            </asp:Panel>
            <hr />
            <asp:Label runat="server" ID="lblMessage" />
        </ContentTemplate>
    </asp:UpdatePanel>
--%>


            
<input type="button" value="Copy" onclick="BeginProcess();" />
<div id="CopyCurrentStep"></div>
<br />
    <div id="CopyLog"></div>

    <script>
        function BeginProcess() {
                        var iframe = document.createElement("iframe");
            iframe.src = "http://www.whitworth.edu/Administration/InformationSystems/Forms/WebRADMVCVB/Home/CopyProject?nProjectID=27";
            iframe.style.display = "none";
            document.getElementById('CopyLog').innerHTML = '<div>Copying main project data.</div>';
            document.body.appendChild(iframe);

            return false;
        }
        
        function UpdateProgress(Message, spacers) {

            console.log(Message);
            console.log("<div style='margin-left:" + (parseInt(spacers) * 50) + "px'>" + "</div>");
            document.getElementById('CopyCurrentStep').innerHTML = Message;
            document.getElementById('CopyLog').innerHTML += "<div style='margin-left:" + (parseInt(spacers) * 50) + "px'>" + Message + "</div>";
        }
     </script>




 <br /><br />
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
		<strong>SQL Server</strong>
		<br />
		<asp:Label ID="lblSQLServer" runat="server"></asp:Label>
			
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

        <br /><br />
        <strong>Notes</strong>
        <br />
        <asp:TextBox ID="txtNotes" runat="server" CssClass="MlText" TextMode="MultiLine" Rows="5" Columns="100"></asp:TextBox>
        
        <br />
        <br />
            <asp:CheckBox ID="chkCreateFrontend" runat="server" Checked="true" Text="Create frontend now?" />
        
            <div style="padding-left:30px">
                <asp:CheckBoxList ID="cblPages" runat="server"></asp:CheckBoxList>
            </div>

        <br />
        <br />
            <asp:CheckBox ID="chkCreateBackend" runat="server" Checked="true" Text="Create backend now?" />
        <br /><br />
        <asp:RadioButtonList ID="rblProjectType" runat="server" RepeatDirection="horizontal">
            <asp:ListItem Value="Live" Selected="true">Create Live Project</asp:ListItem>
            <asp:ListItem Value="Test">Create Test Project</asp:ListItem>
        </asp:RadioButtonList>

                <br /><br />
        <asp:RadioButtonList ID="rblProjectStyle" runat="server" RepeatDirection="horizontal">
            <asp:ListItem Value="Old" Selected="true">Old Style</asp:ListItem>
            <asp:ListItem Value="New">New Style</asp:ListItem >
        </asp:RadioButtonList>
        
        
        <br /><br /><br />
        To finalize the creation of this project, please click the Submit button.  Once all project files and data are created, you will be linked to the locations of your new project pages.

		<div style="text-align:center">
		<asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="Button" />
		</div>
		<div style="text-align:center">
		    <asp:ValidationSummary ID="vsWebRAD" runat="server" />
		</div>		</dl>
        </form>
    </body>
</html>


