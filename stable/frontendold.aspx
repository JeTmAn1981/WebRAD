<%@ Page Language="vb" AutoEventWireup="false" MaintainScrollPositionOnPostback="true" ValidateRequest="false" CodeBehind="frontendold.aspx.vb" Inherits="stable.frontend" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
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
</style>

        <%--<script type="text/javascript" src="/js/ckeditor/ckeditor.js"></script>--%>
          <script type="text/javascript" src="/js/tinymce/tinymce.min.js"></script>
        <script language="javascript" type="text/javascript" src="LoadRichText.js"></script>
         <script src="src/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="src/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    
  

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
        <dl>
          <asp:ScriptManager runat="server" ID="scriptManager">
                <Services>
                    <asp:ServiceReference path="WebService.asmx" />
                </Services>
            </asp:ScriptManager>

     
        <%--<uc:TopMenu ID="topMenu" runat="server" />         --%>
            <br />
            <asp:RequiredFieldValidator ID="rfvIncludeFrontend" runat="server" ControlToValidate="rblIncludeFrontend" ErrorMessage="Please indicate whether or not you'd like to include a frontend." />
            <br />
            <strong>Include frontend?</strong> <span class="required">(required)</span>
            <br />
            <asp:RadioButtonList ID="rblIncludeFrontend" runat="server" RepeatDirection="Horizontal" autopostback="true">
                <asp:ListItem Value="0">No</asp:ListItem>
                <asp:ListItem Value="1" Selected="true">Yes</asp:ListItem>
            </asp:RadioButtonList>

            <asp:Panel ID="pnlFrontend" runat="server" Visible="false">
            <br />
            <asp:CustomValidator ID="cvFrontendPath" runat="server" ErrorMessage="Please select the path for the project frontend." />
            <br />
            <strong>Frontend Path</strong> <span class="required">(required)</span>
           
            <asp:Panel ID="pnlFrontendFolder" runat="server" Visible="false">
            <br /><Br />
            <strong>Selected Folder:</strong>
            <br />
            <asp:Label ID="lblSelectedFrontendFolder" runat="server" />
                </asp:Panel>

            <br /><br />
            <asp:ListBox ID="lsbFrontendFolders" Height="150" Width="300" AutoPostBack="true" runat="server" />
       
            <br />
            <asp:RequiredFieldValidator ID="rfvCreateFrontendFolder" runat="server" ControlToValidate="rblCreateFrontendFolder" ErrorMessage="Please indicate whether or not a new folder should be created for the frontend." />
            <br />
            <strong>Create new folder at this location?</strong> <span class="required">(required)</span>
            <br />
            <asp:RadioButtonList ID="rblCreateFrontendFolder" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                <asp:ListItem Value="0">No</asp:ListItem>
                <asp:ListItem Value="1">Yes</asp:ListItem>
            </asp:RadioButtonList>

            <asp:Panel ID="pnlNewFrontendFolder" runat="server" Visible="false">
                <asp:RequiredFieldValidator ID="rfvNewFrontendFolderName" runat="server" ControlToValidate="txtNewFrontendFolderName" ErrorMessage="Please enter the name of the new folder to be created for the frontend." />
                <br />
                <strong>New Folder Name</strong> <span class="required">(required)</span>
                <br />
                <asp:TextBox ID="txtNewFrontendFolderName" runat="server" Width="300" MaxLength="100" CssClass="SlText" AutoPostBack="True" />

            </asp:Panel>

            
               <asp:RequiredFieldValidator ID="rfvFrontendLink" runat="server" Display="dynamic" ControlToValidate="txtFrontendLink"  ErrorMessage="Please enter the Frontend link for this project." />
                <asp:CustomValidator ID="cvFrontendLink" runat="server" OnServerValidate="cvFrontendLink_ServerValidate" ErrorMessage="Temp"></asp:CustomValidator>
            <br />
            <strong>Frontend Link</strong> <span class="required">required)</span>
            <br />
            <span class="smText">(ex.: Administration/FrontendDirectory/)</span>
            <br />
            <asp:TextBox ID="txtFrontendLink" runat="server" Width="700" MaxLength="500" CssClass="SlText" />
            
          <asp:RequiredFieldValidator ID="rfvRequireLogin" runat="server" ControlToValidate="rblRequireLogin" ErrorMessage="Please indicate whether or not you'd like to require a Whitworth network login."></asp:RequiredFieldValidator>
		<br />
	    <strong>Require a Whitworth network login?</strong> <span class="required">(required)</span>
		<br />
		<asp:RadioButtonList ID="rblRequireLogin" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" OnSelectedIndexChanged="rblRequireLogin_SelectedIndexChanged">
		    <asp:ListItem Value="0">No</asp:ListItem>
		    <asp:ListItem Value="1">Yes</asp:ListItem>
		</asp:RadioButtonList>

        <asp:Panel ID="pnlLoginColumns" runat="server" Visible="false">
            <br />
            <br />
            <strong>I'd like to display the following information about the logged-in user on the main frontend page:</strong>
            <br />
            <asp:ListBox ID="lsbLoginColumns" SelectionMode="multiple" Height="100" runat="server"></asp:ListBox>
        </asp:Panel>

                

              <asp:RequiredFieldValidator ID="rfvEmailSubmitter" runat="server" ControlToValidate="rblEmailSubmitter" ErrorMessage="Please indicate whether or not you'd like have a confirmation e-mail sent to form submitter."></asp:RequiredFieldValidator>
		<br />
	    <strong>Send a confirmation e-mail to form submitter?</strong> <span class="required">(required)</span>
		<br />
		<asp:RadioButtonList ID="rblEmailSubmitter" runat="server" AutoPostBack="true" 
                RepeatDirection="Horizontal" style="height: 27px">
		    <asp:ListItem Value="0">No</asp:ListItem>
		    <asp:ListItem Value="1">Yes</asp:ListItem>
		</asp:RadioButtonList>

        <asp:Panel ID="pnlEmailSubmitter" runat="server" Visible="false">
            <br />
            <table>
<asp:Repeater ID="rptEmailRecipients" runat="server">
    <ItemTemplate>
        
        <tr>
            <td>
                <table border="1" cellpadding="5" cellspacing="5"><tr><td>

                    <br />
        <asp:CustomValidator ID="cvToAddresses"  OnServerValidate="cvToAddresses_ServerValidate" runat="server" ErrorMessage="Please enter an e-mail address or select a control which contains the e-mail address(es) to which this message should be sent." />

<br />
                    <strong>I would like to this message to be sent to the following e-mail address(es):</strong>
                    <br />
                    <asp:TextBox ID="txtToAddress" runat="server" Width="500" MaxLength="500" CssClass="SlText"></asp:TextBox>
                    <br/><br/>
                    <strong>BCC:</strong>
                    <br />
                    <asp:TextBox ID="txtBCCAddress" runat="server" Width="500" MaxLength="500" CssClass="SlText"></asp:TextBox>
                    
            <br /><br />
        <strong>Please select the control(s) you would like to designate as containing the submitter's e-mail address(es):</strong> 
                    <br />
                    <asp:Label ID="lblListEmailControls" runat="server"></asp:Label>

        <br /><br />
        <asp:ListBox ID="lsbEmailControls" runat="server" SelectionMode="multiple"  Height="75" AutoPostBack="true" OnSelectedIndexChanged="lsbEmailControls_SelectedIndexChanged" />

                    <br />
                    <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject" ErrorMessage="Please enter the subject for this message."></asp:RequiredFieldValidator>
        <br />
                    <strong>Subject</strong> <span class="required">(required)</span>
                    <br />
                    <asp:TextBox ID="txtSubject" runat="server" Width="500" CssClass="SlText" MaxLength="1000"></asp:TextBox>
        
                    <br /><br />
                    <strong>Message Type</strong>
                    <br />
                    <asp:RadioButtonList ID="rblMessageType" runat="server" RepeatDirection="horizontal" AutoPostBack="true" OnSelectedIndexChanged="rblMessageType_SelectedIndexChanged">
                        <asp:ListItem Value="Rich" Selected="true">Rich</asp:ListItem>
                        <asp:ListItem Value="Plain">Plain</asp:ListItem>
                    </asp:RadioButtonList>
                                
        <br />
        <strong>Custom Confirmation E-mail Message</strong>
        <br />
        <asp:TextBox ID="txtMessage"  runat="server" CssClass="MlText RichText" TextMode="MultiLine" Rows="5" columns="100"></asp:TextBox>
        <asp:TextBox ID="txtPlainMessage" runat="server" CssClass="MlText" Visible="false" TextMode="MultiLine" Rows="8" columns="100"></asp:TextBox>
                


                <br />
                <asp:RequiredFieldValidator id="rfvWorkflow" runat="server" ControlToValidate="rblWorkflow" ErrorMessage="Please indicate whether or not this message is part of a workflow."></asp:RequiredFieldValidator>
                <br />
                <strong>Is this message part of a workflow?</strong> <span class="required">(required)</span>
                <br />
                <asp:RadioButtonList ID="rblWorkflow" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblWorkflow_SelectedIndexChanged" RepeatDirection="horizontal">
                    <asp:ListItem Value="0">No</asp:ListItem>
                    <asp:ListItem Value="1">Yes</asp:ListItem>
                </asp:RadioButtonList>
                    <asp:Panel ID="pnlWorkflowDestination" runat="server" Visible="false">
                        <br />
                        <strong>Destination</strong> <span class="required">(required)</span>
                        <br />
                        <asp:DropDownList ID="ddlWorkflowDestination" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlWorkflowDestination_SelectedIndexChanged" ></asp:DropDownList>
                        <br /><br />
                        <asp:Label ID="lblWorkflowLink" runat="server"></asp:Label>
                    </asp:Panel>

                    
                    </td></tr></table>
            </td>
            <td valign="top">
                <asp:LinkButton ID="libRemove" OnClick="libRemove_Click" Text="Remove" CausesValidation="false"  runat="server"><img style="border:0" src="~Images/cross.png" width="15" height="15" /></asp:LinkButton>
            </td>
        </tr>
        <tr><td colspan="2"><br /></td></tr>
            </div>
        </ItemTemplate>
</asp:Repeater>
                    </table>
<asp:Button ID="btnAddEmail" runat="server" CausesValidation="false" CssClass="Button" Text="Add E-Mail"   />

            

        
        </asp:Panel>		
		
                
		<br />
	    <strong>Additional Operations</strong> 
		<br />
                <asp:CheckBoxList ID="cblAdditionalOperationTypes" runat="server" RepeatDirection="horizontal" AutoPostBack="true" OnSelectedIndexChanged="cblAdditionalOperationTypes_SelectedIndexChanged"></asp:CheckBoxList>
		
                
                            <asp:Repeater ID="rptAdditionalOperationPages" runat="server">
                                <ItemTemplate>
                                        <br />
                                        <strong><%# Container.DataItem("Name")%> - Page <%# Container.DataItem("PageNumber") %></strong> 
                                        <br />
                                        <asp:TextBox ID="txtOperation" runat="server" cssclass="MlText" Rows="5" TextMode="MultiLine" Columns="100"></asp:TextBox>
                                        <asp:Label ID="lblPageID" runat="server" Visible="false" Text='<%# Container.DataItem("PageID")%>'></asp:Label>
                                   <asp:Label ID="lblOperationTypeID" runat="server" Visible="false" Text='<%# Container.DataItem("OperationTypeID")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:Repeater>
                    
        
                
        <br /><br />
		<!-- Supervisors -->
		<strong>List of Supervisors</strong>
             <asp:customvalidator id="cvSupervisor" runat="server" errormessage="Temp" />
		<table>
		    <asp:repeater id="rptSupervisors" onitemcommand="rptSupervisors_ItemCommand" runat="server">
		        <itemtemplate>
		            <tr>
		                <td>
		                    <asp:label id="lblName" text='<%# Container.DataItem("Name") %>' runat="server" />&nbsp;-&nbsp;<asp:label id="lblSupervisorID"  text='<%# Container.DataItem("SupervisorID") %>' runat="server" />
                            <asp:label id="lblEmail" visible="false" text='<%# Container.DataItem("Email") %>' runat="server" />
		                    
                        </td>
		                <td>
		                    <asp:button id="btnDelete" cssclass="Button" commandname="Delete" text="Delete" causesvalidation="False" runat="server" />
		                </td>
		            </tr>
		        </itemtemplate>
		    </asp:repeater>
		    <tr>
		        <td><asp:TextBox ID="txtSupervisorPLID" runat="server" cssclass="TextField" Width="300" MaxLength="250" /></td>
		        <td valign="top">
                    <asp:button id="btnAdd" causesvalidation="False" cssclass="Button" text="Add" runat="server" />
                    <small>(use supervisor's PLID number) <a href="http://web2/RulesAssignments/search_plid.aspx" target="_blank">Quick PLID Search</a></small>
                </td>
		    </tr>
		</table>
       

        <asp:RequiredFieldValidator ID="rfvEmailSupervisor" runat="server" ControlToValidate="rblEmailSupervisor" ErrorMessage="Please indicate whether or not you'd like have a confirmation e-mail sent to project supervisors."></asp:RequiredFieldValidator>
		<br />
	    <strong>Send a confirmation e-mail to project supervisors?</strong> <span class="required">(required)</span>
		<br />
		<asp:RadioButtonList ID="rblEmailSupervisor" runat="server" AutoPostBack="true" 
                RepeatDirection="Horizontal" style="height: 27px">
		    <asp:ListItem Value="0">No</asp:ListItem>
		    <asp:ListItem Value="1">Yes</asp:ListItem>
		</asp:RadioButtonList>



                <asp:Panel ID="pnlPages" runat="server" Visible="false">
                    <asp:RequiredFieldValidator ID="rfvMultipleSubmissions" runat="server" ControlToValidate="rblMultipleSubmissions" ErrorMessage="Please indicate whether or not you'd like to allow multiple submissions."></asp:RequiredFieldValidator>
                    <br />
                    <strong>Will multiple submissions from the same person be allowed?</strong> <span class="required">(required)</span>
                    <br />
                    <asp:RadioButtonList ID="rblMultipleSubmissions" AutoPostBack="true" OnSelectedIndexChanged="rblMultipleSubmissions_SelectedIndexChanged" runat="server" RepeatDirection="horizontal">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>

                    <asp:Panel ID="pnlRetainedColumns" runat="server" Visible="false">
                        <br /><br />
                        Please select the data you'd like to be carried over from any previous application into a new one.

                        <Br /><br />
                <strong><asp:Label ID="lblMainTableName" runat="server"></asp:Label></strong>
                <br /><br />
                <asp:ListBox ID="lsbMainColumns" SelectionMode="Multiple"  Height="150" runat="server"></asp:ListBox>
                            
        <asp:Repeater ID="rptTables" runat="server">
            <ItemTemplate>
                <Br /><Br />
                <strong><%# Container.DataItem("SQLInsertItemTable")%></strong>
                <br /><br />
                <asp:ListBox ID="lsbColumns" SelectionMode="Multiple"  Height="150" runat="server"></asp:ListBox>
                <asp:Label ID="lblID" runat="server" Visible="false" Text='<%# Container.DataItem("ID")%>' />
            </ItemTemplate>
        </asp:Repeater>
                    </asp:Panel>

                    <br /><br />
                    Please enter a descriptive purpose for each page.
                    <br /><br />
                    <table width="600">
                        <tr>
                            <td align="center"><strong>Page</strong></td>
                            <td><strong>Purpose</strong> <span class="required">(required)</span></td>
                            
                        </tr>
                    <asp:Repeater ID="rptPages" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td colspan="2">
                                    <asp:Requiredfieldvalidator ID="rfvPurpose" runat="server" ControlToValidate="txtPurpose" Display="dynamic" ErrorMessage="Please enter this page's purpose."></asp:Requiredfieldvalidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="center"><%# Container.ItemIndex + 1%></td>
                                <td><asp:TextBox ID="txtPurpose" runat="server" Width="300" MaxLength="100" CssClass="slText"></asp:TextBox>
                                    <asp:Label ID="lblID" runat="server" Visible="false" Text='<%# Container.DataItem("ID")%>'></asp:Label>
                                </td>
                                <td>
                                <asp:Panel ID="pnlMakeCertificationPage" runat="server" visible="false">
                                
                                        <asp:CheckBox ID="chkCertification" runat="server" Text="Make certification page" />
                                    </asp:Panel>
                                    </td>
                                
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    </table>
                </asp:Panel>

                <asp:Panel ID="pnlConfirmationPageMessage" runat="server" Visible="true">
        <br /><br />
        The default message displayed on the confirmation page is "Your submission has been received and is now being processed."  If you would like to display a custom message, please enter it below.

                    <br /><br />
        <strong>Message Type</strong>
        <br />
        <asp:RadioButtonList ID="rblConfirmationPageMessageType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rblConfirmationPageMessageType_SelectedIndexChanged">
            <asp:ListItem Value="Rich" Selected="true">Rich</asp:ListItem>
            <asp:ListItem Value="Plain">Plain</asp:ListItem>
        </asp:RadioButtonList>

        <br /><br />
        <strong>Custom Confirmation Message</strong>
        <br />
                    <asp:Panel ID="pnlConfirmationMessageRich" runat="server">
        <asp:TextBox ID="txtConfirmationPageMessageRich" runat="server" CssClass="MlText RichText" TextMode="MultiLine" Rows="5" columns="100"></asp:TextBox>
                        </asp:Panel>
                    <asp:Panel ID="pnlConfirmationMessagePlain" runat="server" Visible="false">
                        <asp:TextBox ID="txtConfirmationPageMessagePlain" runat="server" CssClass="MlText" TextMode="MultiLine" Rows="5" columns="100"></asp:TextBox>
                    </asp:Panel>
        </asp:Panel>		
		
	</asp:Panel>
				
		
		<div style="text-align:center">
		<asp:Button ID="btnContinue" runat="server" Text="Continue" CssClass="Button" />
		</div>
		<div style="text-align:center">
		    <asp:ValidationSummary ID="vsWebRAD" runat="server" />
		</div>
		</dl>
		<!-- Page Main END -->
            </form>
	    <% Response.WriteFile("\\web2\~whitworth\~Templates\ASPX_Templates\Web2_Base_5_Footer.htm") %>


    </body>
</html>


