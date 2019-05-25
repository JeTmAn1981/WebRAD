<%@ Page Title="Whitworth University Communications - Web-RAD" Language="vb" AutoEventWireup="false" MasterPageFile="~/Template.Master" MaintainScrollPositionOnPostback="true" ValidateRequest="false" CodeBehind="frontend.aspx.vb" Inherits="stable.frontendnewpagetest" %>
<asp:Content ID="SectionName" ContentPlaceHolderID="SectionName" runat="server">Frontend</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
              <script type="text/javascript" src="//media.whitworth.edu/js/tinymce/tinymce.min.js"></script>
        <script  type="text/javascript" src="LoadRichText.js"></script>
         <script src="src/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="src/jquery.contextMenu.css" rel="stylesheet" type="text/css" />

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

                <asp:UpdatePanel ID="updMain" runat="server">
    <ContentTemplate>


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
            <asp:ListBox ID="lsbLoginColumns" SelectionMode="multiple" Height="100" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lsbLoginColumns_SelectedIndexChanged"></asp:ListBox>
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
                <asp:RequiredFieldValidator id="rfvWorkflow" csclass="error" runat="server" ControlToValidate="rblWorkflow" ErrorMessage="Please indicate whether or not this message is part of a workflow."></asp:RequiredFieldValidator>
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
<asp:Button ID="btnAddEmail" runat="server" CausesValidation="false" cssclass="button" Text="Add E-Mail"   />

            

        
        </asp:Panel>		
		
                
		<br />
	    <strong>Additional Operations</strong> 
		<br />
                <asp:CheckBoxList ID="cblAdditionalOperationTypes" runat="server" RepeatDirection="horizontal" AutoPostBack="true" OnSelectedIndexChanged="cblAdditionalOperationTypes_SelectedIndexChanged"></asp:CheckBoxList>
                
	<ajaxToolkit:TabContainer runat="server" ID="AdditionalOperationTabs">
        <ajaxToolkit:TabPanel runat="server" HeaderText="After Page Load" ID="AfterPageLoad">
            <ContentTemplate>
<asp:Repeater ID="rptAfterPageLoad" runat="server">
<ItemTemplate>
<br />
<strong>Page <%# Container.DataItem("PageNumber") %></strong> 
<br />
<asp:TextBox ID="txtOperation" runat="server" cssclass="MlText" Rows="5" TextMode="MultiLine" Columns="100"></asp:TextBox>
<asp:Label ID="lblPageID" runat="server" Visible="false" Text='<%# Container.DataItem("PageID")%>'></asp:Label>
</ItemTemplate>
</asp:Repeater>
</ContentTemplate>
</ajaxToolkit:TabPanel>
<ajaxToolkit:TabPanel runat="server" HeaderText="After Page Submit" ID="AfterPageSubmit">
            <ContentTemplate>
<asp:Repeater ID="rptAfterPageSubmit" runat="server">
<ItemTemplate>
<br />
<strong>Page <%# Container.DataItem("PageNumber") %></strong> 
<br />
<asp:TextBox ID="txtOperation" runat="server" cssclass="MlText" Rows="5" TextMode="MultiLine" Columns="100"></asp:TextBox>
<asp:Label ID="lblPageID" runat="server" Visible="false" Text='<%# Container.DataItem("PageID")%>'></asp:Label>
</ItemTemplate>
</asp:Repeater>
</ContentTemplate>
</ajaxToolkit:TabPanel>
<ajaxToolkit:TabPanel runat="server" HeaderText="Page Header" ID="PageHeader">
            <ContentTemplate>
<asp:Repeater ID="rptPageHeader" runat="server">
<ItemTemplate>
<br />
<strong>Page <%# Container.DataItem("PageNumber") %></strong> 
<br />
<asp:TextBox ID="txtOperation" runat="server" cssclass="MlText" Rows="5" TextMode="MultiLine" Columns="100"></asp:TextBox>
<asp:Label ID="lblPageID" runat="server" Visible="false" Text='<%# Container.DataItem("PageID")%>'></asp:Label>
</ItemTemplate>
</asp:Repeater>
</ContentTemplate>
</ajaxToolkit:TabPanel>
<ajaxToolkit:TabPanel runat="server" HeaderText="Before Page Load" ID="BeforePageLoad">
            <ContentTemplate>
<asp:Repeater ID="rptBeforePageLoad" runat="server">
<ItemTemplate>
<br />
<strong>Page <%# Container.DataItem("PageNumber") %></strong> 
<br />
<asp:TextBox ID="txtOperation" runat="server" cssclass="MlText" Rows="5" TextMode="MultiLine" Columns="100"></asp:TextBox>
<asp:Label ID="lblPageID" runat="server" Visible="false" Text='<%# Container.DataItem("PageID")%>'></asp:Label>
</ItemTemplate>
</asp:Repeater>
</ContentTemplate>
</ajaxToolkit:TabPanel>
<ajaxToolkit:TabPanel runat="server" HeaderText="Common Methods" ID="CommonMethods">
            <ContentTemplate>
                <asp:TextBox ID="txtCommonMethods" TextMode="MultiLine" runat="server" Height="500" CssClass="MlText" Rows="10" columns="80"></asp:TextBox>
</ContentTemplate>
</ajaxToolkit:TabPanel>
<ajaxToolkit:TabPanel runat="server" HeaderText="Page Methods" ID="PageMethods">
            <ContentTemplate>
<asp:Repeater ID="rptPageMethods" runat="server">
<ItemTemplate>
<br />
<strong>Page <%# Container.DataItem("PageNumber") %></strong> 
<br />
<asp:TextBox ID="txtOperation" runat="server" cssclass="MlText" Rows="5" TextMode="MultiLine" Columns="100"></asp:TextBox>
<asp:Label ID="lblPageID" runat="server" Visible="false" Text='<%# Container.DataItem("PageID")%>'></asp:Label>
</ItemTemplate>
</asp:Repeater>
</ContentTemplate>
</ajaxToolkit:TabPanel>
<ajaxToolkit:TabPanel runat="server" HeaderText="Imports" ID="Imports">
            <ContentTemplate>
                <asp:TextBox ID="txtImports" TextMode="MultiLine" runat="server" CssClass="MlText" Height="500" columns="80"></asp:TextBox>
</ContentTemplate>
</ajaxToolkit:TabPanel>
<ajaxToolkit:TabPanel runat="server" HeaderText="Before Page Submit" ID="BeforePageSubmit">
            <ContentTemplate>
<asp:Repeater ID="rptBeforePageSubmit" runat="server">
<ItemTemplate>
<br />
<strong>Page <%# Container.DataItem("PageNumber") %></strong> 
<br />
<asp:TextBox ID="txtOperation" runat="server" cssclass="MlText" Rows="5" TextMode="MultiLine" Columns="100"></asp:TextBox>
<asp:Label ID="lblPageID" runat="server" Visible="false" Text='<%# Container.DataItem("PageID")%>'></asp:Label>
</ItemTemplate>
</asp:Repeater>
</ContentTemplate>
</ajaxToolkit:TabPanel>


	</ajaxToolkit:TabContainer>	


                <asp:panel id="SupervisorsContainer" runat="server" >
<asp:CustomValidator Display="Dynamic" CssClass="error" id="cvSupervisors"  Runat="server" ErrorMessage="Please enter/select the following: Supervisors." ></asp:CustomValidator>
<asp:Label AssociatedControlID="rptSupervisors" runat="server">Supervisors</asp:Label>

<div class="form-group">
<ul>
<asp:Repeater ID="rptSupervisors" Runat="server">
<ItemTemplate>
<li>
<p>
<asp:LinkButton ID="librptSupervisorsRemoveItem" runat="server" CssClass="icon-remove" data-grunticon-embed CausesValidation="false"  OnClick="librptSupervisors_RemoveItem_Click"></asp:LinkButton>
</p> 
<asp:panel id="SupervisorTypeContainer" runat="server" >
<fieldset>
<asp:requiredfieldValidator Display="Dynamic" CssClass="error" id="rfvSupervisorType" Runat="server" ErrorMessage="Please enter/select the following: Type." ControlToValidate="rblSupervisorType"></asp:RequiredFieldValidator>
<asp:Label AssociatedControlID="rblSupervisorType" class="required" runat="server">Type</asp:Label>
<asp:Radiobuttonlist ID="rblSupervisorType" Runat="server" OnSelectedIndexChanged="rptSupervisors_rblSupervisorType_SelectedIndexChanged"  Autopostback="True" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
<asp:listitem value="SingleUser" Selected="true">Single User</asp:listitem>
<asp:listitem value="EmailAddress">Email Address</asp:listitem>
</asp:Radiobuttonlist>
</fieldset>

</asp:panel>


<asp:panel id="SupervisorNameContainer" runat="server" >
<%--<asp:requiredfieldValidator Display="Dynamic" CssClass="error" id="rfvSupervisorName" Runat="server" ErrorMessage="Please enter/select the following: Supervisor's Name (enter name, username or ID)." ControlToValidate="txtSupervisorName"></asp:RequiredFieldValidator>--%>
    <asp:customvalidator ID="cvSupervisorName" runat="server" OnServerValidate="cvSupervisorName_ServerValidate" ></asp:customvalidator>
<asp:Label AssociatedControlID="txtSupervisorName" class="required" runat="server">Supervisor's Name (enter name, username or ID)</asp:Label>
<asp:Textbox ID="txtSupervisorName" Runat="server" TextMode="SingleLine" CssClass="SlText" MaxLength="50"></asp:Textbox>
    <ajaxToolkit:AutoCompleteExtender ServicePath="WebService.asmx"  ServiceMethod="GetSupervisorNameAutocompleteData"  MinimumPrefixLength="3" CompletionInterval="100" TargetControlID="txtSupervisorName" ID="SupervisorNameAutoCompleteExtender" runat="server" FirstRowSelected = "false"></ajaxToolkit:AutoCompleteExtender>
</asp:panel>


<asp:panel id="SupervisorEmailContainer" runat="server" Visible="False">
<asp:requiredfieldValidator Display="Dynamic" CssClass="error" id="rfvSupervisorEmail" Runat="server" ErrorMessage="Please enter/select the following: Supervisor Email Address." ControlToValidate="txtSupervisorEmail"></asp:RequiredFieldValidator>
<asp:Label AssociatedControlID="txtSupervisorEmail" class="required" runat="server">Supervisor Email Address</asp:Label>
<asp:Textbox ID="txtSupervisorEmail" Runat="server" TextMode="SingleLine" CssClass="SlText" MaxLength="1000"></asp:Textbox></asp:panel>


<asp:label id="lblID" runat="server" visible="false" text='<%# Container.DataItem("ID") %>' />
</li>
</ItemTemplate>
</asp:Repeater>
    <p>
<asp:Button ID="btnrptSupervisorsAddItem" runat="server" cssclass="button" CausesValidation="false" Text="Add supervisor" OnClick="btnrptSupervisorsAddItem_Click"></asp:Button>
</p> 


</ul>
    </div>

                </asp:panel>
        <asp:RequiredFieldValidator ID="rfvEmailSupervisor" runat="server" ControlToValidate="rblEmailSupervisor" ErrorMessage="Please indicate whether or not you'd like have a confirmation e-mail sent to project supervisors."></asp:RequiredFieldValidator>
		<br />
	    <strong>Send a confirmation e-mail to project supervisors?</strong> <span class="required">(required)</span>
		<br />
		<asp:RadioButtonList ID="rblEmailSupervisor" runat="server"
                RepeatDirection="Horizontal" style="height: 27px" >
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

                    
                    <asp:panel id="PagesContainer" runat="server" >
<asp:CustomValidator Display="Dynamic" CssClass="error" id="cvPages"  Runat="server" ErrorMessage="Please enter/select the following: Please enter a descriptive purpose for each page. ." OnServerValidate="cvPages_ServerValidate"></asp:CustomValidator>
<asp:Label AssociatedControlID="rptPages" runat="server">Please enter a descriptive purpose for each page. </asp:Label>

<ol>
<asp:Repeater ID="rptPages" Runat="server">
<ItemTemplate>
<li>
<p>
<asp:LinkButton ID="librptPagesRemoveItem" runat="server" CssClass="icon-remove" data-grunticon-embed CausesValidation="false"  OnClick="librptPages_RemoveItem_Click"></asp:LinkButton>
</p> 

<div class="form-group">

<asp:panel id="PurposeContainer" runat="server" >
<asp:requiredfieldValidator Display="Dynamic" CssClass="error" id="rfvPurpose" Runat="server" ErrorMessage="Please enter/select the following: Page Purpose." ControlToValidate="txtPurpose"></asp:RequiredFieldValidator>
<asp:Label AssociatedControlID="txtPurpose" class="required" runat="server">Page Purpose</asp:Label>
<asp:Textbox ID="txtPurpose" Runat="server" OnTextChanged="Page_Detail_Changed"  Autopostback="True" TextMode="SingleLine" CssClass="SlText" MaxLength="0"></asp:Textbox>
</asp:panel>

    
    <asp:panel id="CertificationContainer" runat="server" visible="false">
<asp:CustomValidator Display="Dynamic" CssClass="error" id="cvCertification"  Runat="server" ErrorMessage="Please check the Certification box." OnServerValidate="rptPages_cvCertification_ServerValidate"></asp:CustomValidator>
<asp:Checkbox ID="chkCertification" Runat="server" OnCheckedChanged="Page_Detail_Changed"  Autopostback="True" Text="Make certification page"></asp:Checkbox>
</asp:panel>

<asp:panel id="DependentContainer" runat="server" Visible = "False">
<fieldset>

<asp:checkbox ID="chkDependent" Runat="server" OnCheckedChanged="rptPages_chkDependent_SelectedIndexChanged"  Autopostback="True"></asp:checkbox>
    <asp:Label AssociatedControlID="chkDependent" runat="server">This page's inclusion is dependent on an earlier selection.</asp:Label>
</fieldset>

    
<asp:panel id="SelectionControlContainer" runat="server" Visible="false">
<asp:requiredfieldValidator Display="Dynamic" CssClass="error" id="rfvSelectionControl" Runat="server" ErrorMessage="Please enter/select the following: Please select the control which will provide the value:." ControlToValidate="ddlSelectionControl"></asp:RequiredFieldValidator>
<asp:Label AssociatedControlID="ddlSelectionControl" class="required" runat="server">Please select the control which will provide the value:</asp:Label>
<asp:Dropdownlist ID="ddlSelectionControl" Runat="server" OnSelectedIndexChanged="rptPages_ddlSelectionControl_SelectedIndexChanged"  Autopostback="True">
</asp:Dropdownlist></asp:panel>


<asp:panel id="SelectionValueContainer" runat="server" Visible="False">
<asp:requiredfieldValidator Display="Dynamic" CssClass="error" id="rfvSelectionValue" Runat="server" ErrorMessage="Please enter/select the following: Please enter the selection value:." ControlToValidate="txtSelectionValue"></asp:RequiredFieldValidator>
<asp:Label AssociatedControlID="txtSelectionValue" class="required" runat="server">Please enter the selection value:</asp:Label>
<asp:Textbox ID="txtSelectionValue" Runat="server" TextMode="SingleLine" CssClass="SlText" MaxLength="500" AutoPostBack="true" OnTextChanged="Page_Detail_Changed"></asp:Textbox></asp:panel>

</asp:panel>




</div>

<asp:label id="lblID" runat="server" visible="false" text='<%# Container.DataItem("ID") %>' />
</li>
</ItemTemplate>
</asp:Repeater><p>
<asp:Button ID="btnrptPagesAddItem" runat="server" cssclass="button" CausesValidation="false" Text="Add page" OnClick="btnrptPagesAddItem_Click"></asp:Button>
</p> 


</ol></asp:panel>


                </asp:Panel>

        
    </ContentTemplate>
</asp:UpdatePanel>

                <br /><br />
        The default message displayed on the confirmation page is "Your submission has been received and is now being processed."  If you would like to display a custom message, please indicate below.

                            <br /><br />
<asp:CheckBox ID="chkCustomConfirmationPageMessage" runat="server" Text="Custom confirmation page message?" AutoPostBack="true" OnCheckedChanged="chkCustomConfirmationPageMessage_CheckedChanged" />                
        
                <asp:Panel ID="pnlConfirmationPageMessage" runat="server" Visible="false">
        
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
                    
        <asp:TextBox ID="txtConfirmationPageMessageRich" runat="server" CssClass="MlText RichText" TextMode="MultiLine" Rows="5" columns="100"></asp:TextBox>
                        <asp:TextBox ID="txtConfirmationPageMessagePlain" runat="server" CssClass="MlText" TextMode="MultiLine" Rows="5" columns="100"></asp:TextBox>
                    
        </asp:Panel>		
		
                <asp:Panel ID="pnlShowCustomStatusPageMessage" runat="server" Visible="false">
                    <br /><br />
<asp:CheckBox ID="chkCustomStatusPagemessage" runat="server" Text="Custom status page message?" AutoPostBack="true" OnCheckedChanged="chkCustomStatusPagemessage_CheckedChanged" />                
                        
                <asp:Label runat="server" AssociatedControlID="chkCustomStatusPagemessage"></asp:Label>

		        <asp:Panel ID="pnlStatusPageMessage" runat="server" Visible="false">
                    <br /><br />
        <strong>Message Type</strong>
        <br />
        <asp:RadioButtonList ID="rblStatusPageMessageType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rblStatusPageMessageType_SelectedIndexChanged">
            <asp:ListItem Value="Rich" Selected="true">Rich</asp:ListItem>
            <asp:ListItem Value="Plain">Plain</asp:ListItem>
        </asp:RadioButtonList>

        <br /><br />
        <strong>Custom Status Page Message</strong>
        <br />
                    
        <asp:TextBox ID="txtStatusPageMessageRich" runat="server" CssClass="MlText RichText" TextMode="MultiLine" Rows="5" columns="100"></asp:TextBox>
                        <asp:TextBox ID="txtStatusPageMessagePlain" runat="server" CssClass="MlText" TextMode="MultiLine" Rows="5" columns="100"></asp:TextBox>
                    
        </asp:Panel>		
		</asp:Panel>
	</asp:Panel>

		<div style="text-align:center">
		<asp:Button ID="btnContinue" runat="server" Text="Continue" cssclass="button" />
		</div>
		<div style="text-align:center">
		    <asp:ValidationSummary ID="vsWebRAD" runat="server" />
		</div>

</asp:Content>


