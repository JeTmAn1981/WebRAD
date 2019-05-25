<%@ Page Language="vb" AutoEventWireup="false" ValidateRequest="false" MasterPageFile="~/Template.Master" MaintainScrollPositionOnPostback="true" CodeBehind="controls.aspx.vb" Inherits="stable.controlsnew" %>
<%@ Register TagPrefix="uc" TagName="DataSource" Src="DataSource.ascx" %>
<%@ Register TagPrefix="uc" TagName="ListItems" Src="ListItems.ascx" %>
<%@ Register TagPrefix="uc" TagName="TriggerValues" Src="TriggerValues.ascx" %>

<asp:Content ID="SectionName" ContentPlaceHolderID="SectionName" runat="server">Main Details</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        img {
            max-width:1000px !important;
        }
        
    </style>
    <link href="src/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
            <script type="text/javascript">
                function ShowControlActionOptions()
                {
                    $('#ControlActions').val() == 'Parent' ? $('#pnlSetParentControls').show() : $('#pnlSetParentControls').hide();
                    $('#ControlActions').val() == 'Delete' ? $('#pnlShowDeleteControls').show() : $('#pnlShowDeleteControls').hide();
                    $('#ControlActions').val() == 'Group' ? $('#pnlSetGroupControls').show() : $('#pnlSetGroupControls').hide();
                    $('#ControlActions').val() == 'Align' ? $('#pnlShowAlignControls').show() : $('#pnlShowAlignControls').hide();
                }
            </script>

            <div class="stack-container">
                <div class="stack">
                <label for="ControlActions">Actions:</label>
                <select id="ControlActions" onchange="ShowControlActionOptions()">
                <option Value=""></option>
                <option Value="Parent">Set Parent Control for Selected Controls</option>
                    <option Value="Group">Group Selected Controls</option>
                                        <option Value="Align">Horizontally Align Selected Controls</option>
                <option Value="Delete">Delete Selected Controls</option>
                                         </select>
                    </div>
            
                <div ID="pnlSetParentControls" class="stack" style="display:none;">
                <label for="MainContent_ddlParentControls">Set parent to:</label>
                    <asp:DropDownList ID="ddlParentControls" runat="server"></asp:DropDownList><asp:Button ID="btnSetParent" runat="server" cssclass="button" Text="Set" OnClick="btnSetParent_Click" />
            </div>

                <div ID="pnlSetGroupControls" class="stack" style="display:none;">
                <label for="MainContent_txtSetGroupName">Group Name:</label>
                    <asp:TextBox ID="txtSetGroupName" runat="server"></asp:TextBox>
                    <asp:Button ID="btnSetGroup" runat="server" cssclass="button" Text="Group" OnClick="btnSetGroup_Click" />
            </div>
            
                <div id="pnlShowDeleteControls" style="display:none" class="stack">
                <label for="MainContent_btnDeleteControls">Really delete?</label>
                    <asp:Button ID="btnDeleteControls" runat="server" cssclass="button" Text="Delete" />
            </div>

                <div id="pnlShowAlignControls" style="display:none" class="stack">
                <label for="MainContent_btnAlignControls">Really horizontally align?</label>
                    <asp:Button ID="MainContent_btnAlignControls" OnClick="MainContent_btnAlignControls_Click" runat="server" cssclass="button" Text="Align" />
            </div>

            </div>
            
    <div class="project-pages" >
    
    <asp:Repeater ID="rptPages" runat="server">
        <ItemTemplate>
        <div>
            <asp:LinkButton ID="libPage" runat="server" CausesValidation="false" ForeColor='<%# If(Container.DataItem("ID") = Request.QueryString("PageID"), System.Drawing.Color.Red, System.Drawing.Color.Black)%>' Font-Size="Large" OnClick="libPage_Click">Page <%# Container.ItemIndex + 1%></asp:LinkButton>
                <asp:Label ID="lblID" runat="server" Visible="false" Text='<%#Container.DataItem("ID") %>'></asp:label>
        </div>
        <div>
              <asp:ImageButton id="ibDelete" runat="server" OnClick="ibDelete_Click"  ImageUrl="~images/TrashCan.png" Width="50px" ToolTip="Delete Control" OnClientClick ="return confirm('Are you sure you want to delete this control?');" />
        </div>
            
        </ItemTemplate>
    </asp:Repeater>
        <div>
                      <asp:Imagebutton ID="imbAddPage" Width="50" ImageUrl="~images/Plus.png" runat="server"  />
              
        </div>
        
    </div>
            
	<table>
        <tr>
            <td width="150" height="400" valign="top">
                            <table border="0" height="400" cellpadding="0" cellspacing="0" class="LeftMenuBorder">
						<!--DWLayoutTable-->
						<tr>
							<td class="SideMenuHeader">
                                <table border="0"  cellpadding="0" cellspacing="0">
									<!--DWLayoutTable-->
									<tr>
										<td width="14">&nbsp;</td>
										<td  valign="top" class="SideMenuHeader">
                                            
                                  
        <asp:SqlDataSource ID="sds" runat="server" ConnectionString="Data Source=web3;Initial Catalog=WebRAD;Integrated Security=True"
 ProviderName="System.Data.SqlClient" OldValuesParameterFormatString="original_{0}"
 UpdateCommand="UPDATE [ProjectControls] SET position=@position WHERE [id]=@id"
 Insertcommand="Insert [ProjectControls] (position,ProjectID,PageID, DisplayLocation) values (@position,@ProjectID,@PageID, 1)"
 selectcommand="SELECT [ID], [Heading], [Name],[position],[ControlType], [ProjectID] FROM [ProjectControls] Where ProjectID = @ProjectID and PageID = @PageID ORDER BY [position] asc"
 deletecommand="Delete From web3.WebRAD.dbo.ProjectControls Where ID = @ID">
 
<UpdateParameters>
 <asp:Parameter Name="position" Type="Int32" />
 <asp:Parameter Name="id" Type="Int32" />
 <asp:Parameter Name="ProjectID" Type="Int32" />
 </UpdateParameters>
 <InsertParameters>
    <asp:Parameter Name="position" Type="String" />
    <asp:Parameter Name="ProjectID" Type="Int32" />
</InsertParameters>
<DeleteParameters>
 <asp:Parameter Name="id" Type="Int32" />
 </DeleteParameters>

</asp:SqlDataSource>

                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:CheckBox ID="chkSelectAllControls" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelectAllControls_CheckedChanged" />
                <asp:label ID="lblSelectAllControls" runat="server" style="padding-left:33px;font-weight:bolder" associatedcontrolid="chkSelectAllControls" >Select All Controls</asp:label>
                
                                                    </td>
                                                </tr>
                                                                                                <tr>
                                                    <td>
                                                        <ajaxtoolkit:reorderlist ID="rl1" runat="server" SortOrderField="position"
 AllowReorder="true" DataSourceID="sds" DataKeyField="id"   ItemInsertLocation="End" OnDeleteCommand="rlEvents_DeleteCommand"
 width="400" CssClass="controls-list"  >
 <DragHandleTemplate>
                 <img src="~Images/move-icon.png" width="30" height="30"  />
         
 </DragHandleTemplate>
        
 <ItemTemplate>
     
      <table style="list-style:none">
          <tr>
              <td>
                  <asp:CheckBox ID="chkSelectControl" runat="server" />
                <asp:label ID="lblSelectControl"  runat="server" associatedcontrolid="chkSelectControl" ></asp:label>
              </td>
              <td>
     <asp:Panel ID="pnlSelectControl" runat="server">
 <table>
    <tr>
        <td valign="middle">
            
 <asp:LinkButton causesvalidation="false" ID="libSelectControl" OnClick="libSelectControl_Click" runat="server" Text='<%# GetName(Container.DataItem("Heading"), Container.DataItem("Name"), Container.DataItem("ID"), Container.DataItem("Position")) %>'></asp:LinkButton>       
        </td>
        <td valign="middle">
              <asp:Panel ID="pnlMenuImage" runat="server">
            <img src="~Images/menu.png" width="50" height="50" />
        </asp:Panel>

              </td>
          </tr>
      </table>      
      
</div>
            
     <ajaxToolkit:HoverMenuExtender ID="hme2" runat="Server"
    TargetControlID="pnlMenuImage"
    PopupControlID="PopupMenu"
    HoverCssClass="popupHover"
    PopupPosition="right"
    OffsetX="20"
    OffsetY="0"
    PopDelay="50"   />

                                      <asp:Panel CssClass="popupMenu" ID="PopupMenu" runat="server">
                                          <div style="background-color:white;width:400px;border:1px solid black; padding:20px; display:flex; flex-direction:row; justify-content:space-around;">
                                              <asp:LinkButton ID="libInsertAbove" runat="server" 
            CommandName="Insert Above" OnClick="libInsertAbove_Click" Text="Insert Above" />
        
                                              <asp:LinkButton ID="libInsertBelow" runat="server" 
            CommandName="Insert Below" OnClick="libInsertBelow_Click" Text="Insert Below" />
        
                                          <asp:LinkButton ID="libDelete" runat="server" ToolTip="Delete Control" OnClientClick ="return confirm('Are you sure you want to delete this control?');" CommandArgument ='<%#Eval("ID") %>' CommandName="Delete" Text="Delete"></asp:LinkButton>
                                              </div>
                                          
                                              </asp:Panel>
 <asp:ImageButton id="ibDelete" runat="server" Visible="false"  CommandArgument ='<%#Eval("ID") %>' CausesValidation="false" CommandName="Delete" ImageUrl="~images/TrashCan.png" ToolTip="Delete Control" OnClientClick ="return confirm('Are you sure you want to delete this control?');" />
 <asp:Label ID="lblID" runat="server" Visible="false" Text='<%#Container.DataItem("ID") %>'></asp:label>
<asp:Label ID="lblProjectID" runat="server" Visible="false" Text='<%#Container.DataItem("ProjectID") %>'></asp:label>
<asp:Label ID="lblPosition" runat="server" Visible="false" Text='<%# Container.DataItem("Position")%>'></asp:label>
</td>
    </tr>
 </table>
         </asp:Panel>
 </ItemTemplate>
  <InsertItemTemplate>  
                  
        <div style="padding-left:25px; border-bottom:thin solid transparent;">  
                        <asp:Panel ID="panel1" runat="server" DefaultButton="Button1">  

                            <asp:Button ID="Button1" CssClass="button" runat="server" CommandName="Insert" Text="Add Control"  ValidationGroup="add" />  
                
                        </asp:Panel>  
                    </div>  
                </InsertItemTemplate>  
</ajaxtoolkit:reorderlist>

                  
                                                    </td>
                                                </tr>
                                            </table>
                                            
    

 
							            </td>			
									</tr>
								</table>							
                            </td>
						</tr>
					</table>
		
            
      
            </td>
            <td class="auto-style1">
            &nbsp;
            </td>
            <td width="100%" valign="top">
        <div style="text-align:center">
            <asp:validationsummary CssClass="error" id="vs2" runat="server" />
        </div>
		<div style="text-align:center">
		<asp:Button ID="btnContinue2" runat="server" Text="Continue" cssclass="button" />
		</div>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            
<div class="form-group">
                           

		                        <asp:requiredfieldvalidator ID="rfvControlType" runat="server" ControlToValidate="ddlControlType" ErrorMessage="Please select the control type."></asp:requiredfieldvalidator>
		                    
		                    <asp:label runat="server" associatedcontrolid="ddlControlType" id="lblControlType" CssClass="required" >Control Type</asp:label>
                            <asp:DropDownList id="ddlControlType" AutoPostBack="true" runat="server"></asp:DropDownList>
		                 


                            <asp:Panel ID="pnlParentControl" runat="server" Visible="false">
                            
		                    
		                    <asp:label runat="server" associatedcontrolid="ddlParentControl" id="lblParentControl" >Parent Control</asp:label>
<asp:DropDownList id="ddlParentControl" AutoPostBack="true" runat="server"></asp:DropDownList>
                            </asp:Panel>


                            <asp:Panel ID="pnlControlTypeDetails" runat="server" Visible="false">
		                   
		                         <asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic" ControlToValidate="txtName" ErrorMessage="Please enter the name."></asp:RequiredFieldValidator>
                                 <asp:CustomValidator ID="cvName" runat="server" display="dynamic" ErrorMessage="Sorry, another control in this project was found with that name.  Names must be unique.  Please choose another name for this control." CssClass="error"></asp:CustomValidator>
		                        
		                        <asp:label runat="server" associatedcontrolid="txtName" id="lblName"  cssclass="required">Name (do not include type prefix)</asp:Label> 
		                        
		                        <asp:TextBox ID="txtName" runat="server" Width="300" CssClass="SlText" MaxLength="100"></asp:TextBox>
		                        
                                
                                    <asp:Panel ID="pnlDisplayHeading" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvDisplayHeading" runat="server" ControlToValidate="rblDisplayHeading" ErrorMessage="Please indicate whether or not the heading should be displayed."></asp:RequiredFieldValidator>


                                 
		                         <asp:label runat="server" associatedcontrolid="rblDisplayHeading" id="lblDisplayHeading" >Display heading?</asp:label>
                                 
                                 <fieldset>
                                <asp:radiobuttonlist id="rblDisplayHeading" runat="server" AutoPostBack="true" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
                                <asp:listitem value="0">No</asp:listitem>
                                <asp:listitem value="1">Yes</asp:listitem>
                                </asp:radiobuttonlist>
                                </fieldset>


		                        </asp:Panel>
		                    

                                <asp:Panel ID="pnlHeading" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvHeading" runat="server" ControlToValidate="txtHeading" ErrorMessage="Please enter the heading."></asp:RequiredFieldValidator>

<asp:label runat="server" associatedcontrolid="txtHeading" id="lblHeading" >Heading</asp:label>
		                                
		                        <asp:TextBox ID="txtHeading" runat="server" CssClass="WebRADMultiline"  TextMode="MultiLine" rows="3" columns="80"></asp:TextBox>
		                        </asp:Panel>
                                
                                
                                <asp:Panel ID="pnlSubheading" runat="server" visible="false">
                            <asp:label runat="server" associatedcontrolid="txtSubheading" id="lblSubHeading">Subheading</asp:label>
		                        <asp:TextBox ID="txtSubHeading" runat="server" CssClass="WebRADMultiline" TextMode="MultiLine" rows="1" columns="80"></asp:TextBox>
                                    
		                        </asp:Panel>

                                <asp:Panel ID="pnlShortHeading" runat="server" Visible="false">
                                <asp:RequiredFieldValidator ID="rfvShortHeading" Enabled="false" Display="Dynamic" runat="server" ControlToValidate="txtShortHeading" ErrorMessage="Please enter a short heading."></asp:RequiredFieldValidator>
                                
		                        <asp:label runat="server" associatedcontrolid="txtShortHeading" id="lblShortHeading">Short Heading</asp:label>
		                        <asp:TextBox ID="txtShortHeading" runat="server" Width="300" CssClass="SlText" MaxLength="100"></asp:TextBox>
                                </asp:Panel>
                                
                                <asp:Panel ID="pnlRepeaterAddRemove" runat="server" Visible="false">
                                    
                                   
                                   
                                   <asp:label runat="server" associatedcontrolid="rblRepeaterAddRemove" id="lblRepeaterAddRemove" cssclass="required"  >Provide add/remove buttons?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblRepeaterAddRemove" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" AutoPostBack="true" runat="server" OnSelectedIndexChanged="rblRepeaterAddRemove_SelectedIndexChanged">
                                       <asp:ListItem Value="0">No</asp:ListItem>
                                       <asp:ListItem Value="1">Yes</asp:ListItem>
                                   </asp:radiobuttonlist>
</fieldset>
                                    
                                          <asp:Panel ID="pnlRepeaterItemName" runat="server" Visible="false">
                                    
                                   
                                              <asp:RequiredFieldValidator ID="rfvRepeaterItemName" runat="server" ControlToValidate="txtRepeaterItemName" ErrorMessage="Please enter the name of the repeater item."></asp:RequiredFieldValidator>
                                   
                                   <asp:label runat="server" associatedcontrolid="txtRepeaterItemName" id="lblRepeaterItemName" cssclass="required"  >Name of repeater item (e.g. "guest details") ?</asp:label>
<asp:TextBox id="txtRepeaterItemName" runat="server" CssClass="SlText" MaxLength="100" Width="300"></asp:TextBox>
                                    

                                </asp:Panel>

                                </asp:Panel>


                                <asp:Panel  visible="true" id="pnlRequired" runat="server">
		                            <asp:RequiredFieldValidator ID="rfvRequired" display="dynamic" runat="server" ControlToValidate="rblRequired" ErrorMessage="Please indicate whether or not this control is required."></asp:RequiredFieldValidator>
		                            
		                            <asp:label runat="server" associatedcontrolid="rblRequired" id="lblRequired" cssclass="required"  >Is this control required?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblRequired" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" AutoPostBack="true" OnSelectedIndexChanged="rblRequired_SelectedIndexChanged">
		                                <asp:ListItem Value="0">No</asp:ListItem>
                                        <asp:ListItem Value="1">Yes</asp:ListItem>
		                            
		                            </asp:radiobuttonlist>
</fieldset>
        	                   
                                
		                        <asp:Panel  visible="false" id="pnlMinimumRequired" runat="server">
		                            <asp:RequiredFieldValidator ID="rfvMinimumRequired" runat="server" ControlToValidate="ddlMinimumRequired" ErrorMessage="Please indicate the minimum selections required."></asp:RequiredFieldValidator>
		                            
		                            <asp:label runat="server" associatedcontrolid="ddlMinimumRequired" id="lblMinimumRequired" cssclass="required"  >What is the minimum number of selections required?</asp:label>
<asp:DropDownList id="ddlMinimumRequired" runat="server"></asp:DropDownList>

                                     
		                            
		                            <asp:label runat="server" associatedcontrolid="ddlMaximumRequired" id="lblMaximumRequired" >What is the maximum number of selections required?</asp:label>
<asp:DropDownList id="ddlMaximumRequired" runat="server"></asp:DropDownList>
		                       </asp:Panel>    

                                </asp:Panel>     



                                		                        <asp:Panel  visible="false" id="pnlCustomValidation" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvCustomValidation" runat="server" ControlToValidate="rblCustomValidation" ErrorMessage="Please indicate whether or not this control should use custom validation."></asp:RequiredFieldValidator>
		                        
		                        <asp:label runat="server" associatedcontrolid="rblCustomValidation" id="lblCustomValidation" cssclass="required"  >Use custom validation for this control?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblCustomValidation" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" AutoPostBack="true" OnSelectedIndexChanged="rblCustomValidation_SelectedIndexChanged">
		                            <asp:ListItem Value="0">No</asp:ListItem>
                                    <asp:ListItem Value="1">Yes</asp:ListItem>
		                            
		                        </asp:radiobuttonlist>
</fieldset>

                                                                    <asp:Panel ID="pnlCustomValidationCode" runat="server" Visible="false">
                                                                        <asp:CustomValidator ID="cvCustomValidationCode" runat="server" ErrorMessage="Please enter the custom validation code." OnServerValidate="cvCustomValidationCode_ServerValidate"></asp:CustomValidator>
		                        
		                        <asp:label runat="server" associatedcontrolid="txtCustomValidationCode" id="lblCustomValidationCode" cssclass="required"  >Custom Validation Code</asp:label>
<asp:TextBox id="txtCustomValidationCode" runat="server" TextMode="MultiLine" Rows="5" Columns="80" CssClass="MlText"></asp:TextBox>
                                                                    </asp:Panel>
		                   </asp:Panel>   

                                <asp:Panel  visible="false" id="pnlRequireVerification" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvRequireVerification" runat="server" ControlToValidate="rblRequired" ErrorMessage="Please indicate whether or not this control should require verification of text."></asp:RequiredFieldValidator>
		                        
		                        <asp:label runat="server" associatedcontrolid="rblRequireVerification" id="lblRequireVerification" cssclass="required"  >Require verification of text?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblRequireVerification" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" AutoPostBack="true">
		                            <asp:ListItem Value="0" Selected="true">No</asp:ListItem>
                                    <asp:ListItem Value="1">Yes</asp:ListItem>
		                            
		                        </asp:radiobuttonlist>
</fieldset>
		                   </asp:Panel>   
                                
                                


                                <asp:Panel ID="pnlTextPosition" runat="server" Visible="false">
                                <asp:RequiredFieldValidator ID="rfvTextPosition" Enabled="false" Display="Dynamic" runat="server" ControlToValidate="rblTextPosition" ErrorMessage="Please select the text position."></asp:RequiredFieldValidator>
                                
		                        <asp:label runat="server" associatedcontrolid="rblTextPosition" id="lblTextPosition">Text Position</asp:label>
		                                
		                        <fieldset>
<asp:radiobuttonlist ID="rblTextPosition" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
                                    <asp:ListItem Value="After" Selected="true">After Control</asp:ListItem>
                                    <asp:ListItem Value="Before">Before Control</asp:ListItem>
                                    
		                        </asp:radiobuttonlist>
</fieldset>
                                </asp:Panel>
		                        
		                        
                              <asp:Panel  visible="false" id="pnlLayoutType" runat="server">
                                <asp:RequiredFieldValidator ID="rfvLayoutType" runat="server" ControlToValidate="rblLayoutType" ErrorMessage="Please select the layout type." Enabled="false"></asp:RequiredFieldValidator>
		                        
		                         <asp:label runat="server" associatedcontrolid="rblLayoutType" id="lblLayoutType">Layout Type</asp:label>
		                                
		                        <fieldset>
<asp:radiobuttonlist ID="rblLayoutType" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" AutoPostBack="true" OnSelectedIndexChanged="rblLayoutType_SelectedIndexChanged" />

                                   
                                   <asp:RequiredFieldValidator ID="rfvLayoutSubtype" runat="server" ControlToValidate="rblLayoutSubtype" ErrorMessage="Please select the layout subtype."></asp:RequiredFieldValidator>
                                   
                                   <asp:label runat="server" associatedcontrolid="rblLayoutSubtype" id="lblLayoutSubtype" cssclass="required"  >Layout Subtype</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblLayoutSubtype" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" runat="server"></asp:radiobuttonlist>
</fieldset>
                                                                   
                                </asp:Panel>
		                     
                                
                                
                                    <asp:Panel  visible="false" id="pnlSupplyControlData" runat="server">
                                <asp:RequiredFieldValidator ID="rfvSupplyControlData" runat="server" ControlToValidate="rblSupplyControlData" ErrorMessage="Please indicate whether or not you'd like to supply this control with data."></asp:RequiredFieldValidator>
<asp:label runat="server" associatedcontrolid="rblSupplyControlData" id="lblSupplyControlData" cssclass="required"  >Supply with data?</asp:label>

		                        <fieldset>
<asp:radiobuttonlist ID="rblSupplyControlData" runat="server" AutoPostBack="true" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
                                    <asp:ListItem Value="0" Selected="true">No</asp:ListItem>
                                    <asp:ListItem Value="1">Yes</asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>
		                        </asp:Panel>
                                
                                <asp:panel id="pnlSupplyDataType" visible="false" runat="server">
                                    
		                        
		                         <asp:label runat="server" associatedcontrolid="rblSupplyDataType" id="lblSupplyDataType" cssclass="required"  >How should data be supplied?</asp:label>
<b/>
		                        <fieldset>
<asp:radiobuttonlist ID="rblSupplyDataType" runat="server" AutoPostBack="true"  RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
                                    <asp:ListItem Value="DirectAssignment" Selected="true">Direct assignment</asp:ListItem>
                                    <asp:ListItem Value="Autocomplete">Autocomplete</asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>
		                            
                                </asp:panel>
                                
                                    <asp:Panel  visible="false" id="pnlControlDataSource" runat="server">
                                
                                        
		                         <asp:label runat="server" associatedcontrolid="ucControlSupplyDataSource" id="lblontrolSupplyDataSource" cssclass="required"  >Control Data Source</asp:label>
<uc:DataSource ID="ucControlSupplyDataSource" runat="server" />
		                        </asp:Panel>

                                <asp:Panel ID="pnlControlDataSourceColumn" runat="server" visible="false">
                                    <asp:RequiredFieldValidator ID="rfvControlDataSourceColumn" runat="server" ControlToValidate="txtControlDataSourceColumn" ErrorMessage="Please enter the data column to be used for this control's data source."></asp:RequiredFieldValidator>
                                    
                                    <asp:label runat="server" associatedcontrolid="txtControlDataSourceColumn" id="lblControlDataSourceColumn" cssclass="required">Control Data Column</asp:label>
<asp:TextBox id="txtControlDataSourceColumn" runat="server" CssClass="SlText" Width="300" MaxLength="100"></asp:TextBox>
                                </asp:Panel>



                                
                                  <asp:RequiredFieldValidator ID="rfvDisplayLocation" runat="server" ControlToValidate="rblDisplayLocation" ErrorMessage="Please indicate where this control should be displayed."></asp:RequiredFieldValidator>

                                  <asp:label runat="server" associatedcontrolid="rblDisplayLocation" id="lblDisplayLocation">Where should this control be displayed?</asp:label>

                                <%-- Potential bug here, don't let controls be selected as backend only if they have a parent control --%>
		                        <fieldset>
<asp:radiobuttonlist ID="rblDisplayLocation" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" AutoPostBack="true">
		                            <asp:ListItem Value="1" Selected="true">Frontend and Backend</asp:ListItem>
		                            <asp:ListItem Value="2">Frontend only</asp:ListItem>
                                    <asp:ListItem Value="3">Backend only</asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>

                                   <asp:RequiredFieldValidator ID="rfvDisplayType" runat="server" ControlToValidate="ddlDisplayType" ErrorMessage="Please indicate how this control should be displayed."></asp:RequiredFieldValidator>

                                  
		                         <asp:label runat="server" associatedcontrolid="ddlDisplayType" id="lblDisplayType">How should this control be displayed?</asp:label>
		                                
		                        <asp:DropDownList ID="ddlDisplayType" runat="server"></asp:DropDownList>
		                        
                                <asp:Panel ID="pnlIncludeDatabase" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvIncludeDatabase" runat="server" ControlToValidate="rblIncludeDatabase" ErrorMessage="Please indicate whether or not this column should be saved to the database."></asp:RequiredFieldValidator>
                                
                                 <asp:label runat="server" associatedcontrolid="rblIncludeDatabase" id="lblIncludeDatabase">Save this column to the database?</asp:label>
		                        

		                        <fieldset>
<asp:radiobuttonlist ID="rblIncludeDatabase" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblIncludeDatabase_SelectedIndexChanged" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
		                            <asp:ListItem Value="0">No</asp:ListItem>
		                            <asp:ListItem Value="1" Selected="True">Yes</asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>
		                         </asp:Panel>

                                 <asp:Panel ID="pnlAutopostback" runat="server">
		                         <asp:RequiredFieldValidator ID="rfvAutopostback" runat="server" ControlToValidate="rblAutopostback" ErrorMessage="Please indicate whether or not autopostback should be enabled."></asp:RequiredFieldValidator>
		                        
		                        <asp:label runat="server" associatedcontrolid="rblAutopostback" id="lblAutopostback" cssclass="required"  >Enable autopostback?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblAutopostback" AutoPostBack="true" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
		                            <asp:ListItem Value="0" Selected="True">No</asp:ListItem>
		                            <asp:ListItem Value="1">Yes</asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>
                                </asp:Panel>

                                <asp:Panel ID="pnlPostbackActionOptions" runat="server" Visible="false">
                             <asp:Panel ID="pnlPerformPostbackAction" runat="server">
		                         <asp:RequiredFieldValidator ID="rfvPerformPostbackAction" runat="server" ControlToValidate="rblPerformPostbackAction" ErrorMessage="Please indicate whether or not an action should be performed on postback."></asp:RequiredFieldValidator>
		                        
		                        <asp:label runat="server" associatedcontrolid="rblPerformPostbackAction" id="lblPerformPostbackAction" cssclass="required"  >Perform action on postback?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblPerformPostbackAction" autopostback="true" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
		                            <asp:ListItem Value="0" Selected="True">No</asp:ListItem>
		                            <asp:ListItem Value="1">Yes</asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>
                                </asp:Panel>

                                <asp:Panel ID="pnlPostbackActions" runat="server" Visible="false">
                                    
                                    <asp:RequiredFieldValidator ID="rfvPostbackActions" runat="server" ControlToValidate="ddlActions" ErrorMessage="Please select the number of postback actions to perform." />
                                    
                                    <asp:label runat="server" associatedcontrolid="ddlActions" id="lblActions" cssclass="required"  >Number of Postback Actions to Perform</asp:label>
<asp:dropdownlist id="ddlActions" AutoPostBack="true" runat="server" />

                                    <ol>
                                    <asp:Repeater ID="rptPostbackActions" runat="server">
                                        <ItemTemplate>
                                            <li>
                                                                                            
                                            <asp:RequiredFieldValidator ID="rfvPostbackAction" runat="server" ControlToValidate="ddlAction" ErrorMessage="Please select the action to be performed."></asp:RequiredFieldValidator>
		                                    
		                                    <asp:label runat="server" associatedcontrolid="ddlAction" cssclass="required"  >Action to perform?</asp:label>
<asp:dropdownList id="ddlAction" AutoPostBack="true" OnSelectedIndexChanged="ddlAction_SelectedIndexChanged" runat="server"></asp:dropdownList>

                                            <asp:panel id="pnlPostbackActionTargetControl" runat="server" visible="false">
                                            <asp:RequiredFieldValidator ID="rfvPostbackActionTarget" runat="server" ControlToValidate="ddlTargetControl" ErrorMessage="Please select the target control on which the action is to be performed."></asp:RequiredFieldValidator>
		                                    
		                                    <asp:label runat="server" associatedcontrolid="ddlTargetControl" cssclass="required"  >Target control for action?</asp:label>
<asp:dropdownList id="ddlTargetControl" runat="server"></asp:dropdownList>
                                            </asp:panel>

                                            <asp:Panel ID="pnlCustomActionCode" runat="server" Visible="false">
                                                <asp:RequiredFieldValidator ID="rfvCustomActionCode" runat="server" ControlToValidate="txtCustomActionCode" ErrorMessage="Please enter the custom action code."></asp:RequiredFieldValidator>
                                                
                                                <asp:label runat="server" associatedcontrolid="txtCustomActionCode"  cssclass="required"  >Custom Action Code</asp:label>
<asp:TextBox id="txtCustomActionCode" runat="server" TextMode="MultiLine" CssClass="MlText" Rows="5" Columns="80"></asp:TextBox>
                                            </asp:Panel>

                                                <asp:Panel id="pnlUpdateRepeaterItemsSelectionType" runat="server" Visible="false">
                                                    <asp:RequiredFieldValidator ID="rfvUpdateRepeaterItemsSelectionType" ErrorMessage="Please indicate the item selection type." runat="server" ControlToValidate="rblUpdateRepeaterItemsSelectionType"></asp:RequiredFieldValidator>
                                                        
                                                    <asp:label runat="server" associatedcontrolid="rblUpdateRepeaterItemsSelectionType" id="lblUpdateRepeaterItemsSelectionType" cssclass="required"  >Number of Items - Selection Type</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblUpdateRepeaterItemsSelectionType" runat="server" RepeatDirection="vertical" AutoPostBack="true" OnSelectedIndexChanged="rblUpdateRepeaterItemsSelectionType_SelectedIndexChanged" >
                                                        <asp:ListItem Value="Control">I would like to set the number of items in the repeater to the value of this control.</asp:ListItem>
                                                        <asp:ListItem Value="Explicit">I would like to specify an explicit number of items to set to the repeater.</asp:ListItem>
                                                        </asp:radiobuttonlist>
</fieldset>
                                                    </asp:Panel>

                                                <asp:Panel ID="pnlUpdateRepeaterItemsValue" runat="server" Visible="false">
                                                    
                                                    <asp:label runat="server" associatedcontrolid="ddlUpdateRepeaterItemsValue" id="lblUpdateRepeaterItemsValue" >How many items should be selected for this repeater?</asp:label>
<asp:DropDownList id="ddlUpdateRepeaterItemsValue" runat="server" />
                                                </asp:Panel>

                                            <uc:ListItems ID="ucActionLSI" runat="server" />
                                       
                                            <asp:Panel ID="pnlSetValueOptions" runat="server" Visible="false">
                                                <asp:RequiredFieldValidator ID="rfvSetValueType" runat="server" ControlToValidate="ddlSetValueType" ErrorMessage="Please indicate to what you would like to set the value of this control."></asp:RequiredFieldValidator>
                                                
                                            <asp:label runat="server" associatedcontrolid="ddlSetValueType" id="lblSetValueType" cssclass="required"  >I would like to set the value of this control to:</asp:label>
<asp:dropdownlist id="ddlSetValueType" runat="server"  AutoPostBack="true" OnSelectedIndexChanged="ddlSetValueType_SelectedIndexChanged">
                                                    <asp:ListItem Value="">Please Select</asp:ListItem>
                                                    <asp:ListItem Value="Explicit">An explicit value.</asp:ListItem>
                                                    <asp:ListItem Value="Control">The value of another control.</asp:ListItem>
                                                    <asp:ListItem Value="Database">The result of a database query.</asp:ListItem>
                                                </asp:dropdownlist>
                                            <uc:DataSource ID="ucSetValueDataSource" runat="server" />

                                                <asp:Panel ID="pnlExplicitValue" runat="server" Visible="false">
                                                    <%--Took out the requirements for this to allow for setting to "blank" values.  Could cause a bug though.--%>
                                                    <%--<asp:RequiredFieldValidator ID="rfvExplicitValue" runat="server" ControlToValidate="txtExplicitValue" ErrorMessage="Please enter the explicit value/database column." />--%>
                                                    
                                                    <asp:label runat="server" associatedcontrolid="txtExplicitValue" id="lblExplicitValue">I would like to set this control to the following explicit value/database column:</asp:label> <%--<span class="required">(required)</span>--%>
                                                    
                                                    <asp:TextBox ID="txtExplicitValue" runat="server" Width="300" CssClass="SlText"></asp:TextBox>
                                                </asp:Panel>

                                                <asp:Panel ID="pnlControlValue" runat="server" Visible="false">
                                                    <asp:RequiredFieldValidator ID="rfvControlValue" runat="server" ControlToValidate="ddlControlValue" ErrorMessage="Please select the control the value of which will be selected." />
                                                    
                                                    <asp:label runat="server" associatedcontrolid="ddlControlValue" id="lblControlValue" cssclass="required"  >Control:</asp:label>
<asp:DropDownList id="ddlControlValue" runat="server"></asp:DropDownList>
                                                </asp:Panel>


                                                </asp:Panel>
                                                
                                                <uc:TriggerValues ID="ucTriggerValues" runat="server"></uc:TriggerValues>
                                            
                                            
<asp:Label ID="lblID" runat="server" Visible="false" Text='<%# Container.DataItem("ID")%>' />


                                                </li>
                                        </ItemTemplate>
                                   </asp:Repeater>
                                    </ol>
                                </asp:Panel>

                                 
                               </asp:Panel>
                                
                                


                                <asp:Panel ID="pnlOnchange" runat="server">
					
		                        
		                        <asp:label runat="server" associatedcontrolid="rblOnchange" id="lblOnchange" cssclass="required"  >Include Javascript onchange event?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblOnchange" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" AutoPostBack="True" OnSelectedIndexChanged="rblOnchange_SelectedIndexChanged">
		                            <asp:ListItem Value="0" Selected="True">No</asp:ListItem>
		                            <asp:ListItem Value="1">Yes</asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>

                                <asp:Panel ID="pnlOnchangeDetail" runat="server" Visible="false">
                                    
                                    <asp:RequiredFieldValidator ID="rfvOnchangeCall" runat="server" ControlToValidate="txtOnchangeCall" ErrorMessage="Please enter the onchange function call."></asp:RequiredFieldValidator>
                                    
                                    <asp:label runat="server" associatedcontrolid="txtOnchangeCall" id="lblOnchangeCall" cssclass="required"  >Onchange Function Call</asp:label>
<asp:TextBox id="txtOnchangeCall" runat="server" Width="500" MaxLength="500" CssClass="SlText" />

                                    
                                    
                                    <asp:label runat="server" associatedcontrolid="txtOnchangeBody" id="lblOnchangeBody" >Onchange Function Body</asp:label>
<asp:TextBox id="txtOnchangeBody" runat="server" CssClass="MlText" TextMode="MultiLine" Rows="5" Columns="80" />
                                </asp:Panel>
                                </asp:Panel>



                                <asp:Panel ID="pnlEnabled" runat="server">
					
		                        
		                        <asp:label runat="server" associatedcontrolid="rblEnabled" id="lblEnabled" cssclass="required"  >Enabled?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblEnabled" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
		                            <asp:ListItem Value="0">No</asp:ListItem>
		                            <asp:ListItem Value="1" Selected="True">Yes</asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>
                                </asp:Panel>

                                  <asp:Panel ID="pnlVisible" runat="server">
		                         <asp:RequiredFieldValidator ID="rfvVisible" runat="server" ControlToValidate="rblVisible" ErrorMessage="Please indicate whether or not this control should be visible."></asp:RequiredFieldValidator>
		                        
		                        <asp:label runat="server" associatedcontrolid="rblVisible" id="lblVisible" cssclass="required"  >Visible?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblVisible" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" AutoPostBack="true" OnSelectedIndexChanged="rblVisible_SelectedIndexChanged">
		                            <asp:ListItem Value="0">No</asp:ListItem>
		                            <asp:ListItem Value="1" Selected="True">Yes</asp:ListItem>
                                    <asp:ListItem Value="3">Dependent</asp:ListItem>
                                    <asp:ListItem Value="2">Custom Value</asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>

                                <asp:Panel runat="server" id="pnlDependentValue" Visible="False">
                                      <%--<asp:RequiredFieldValidator ID="rfvDependingControl" runat="server" ControlToValidate="ddlDependingControl" ErrorMessage="Please select the control on which visibility is dependent."></asp:RequiredFieldValidator>
		                                    
		                                    <asp:label runat="server" associatedcontrolid="ddlDependingControl" id="lblDependingControl" cssclass="required"  >Depending Control</asp:label>
<asp:dropdownList id="ddlDependingControl" runat="server"></asp:dropdownList>
                                                
                                            <uc:TriggerValues ID="ucVisibilityTriggerValues" runat="server" />--%>
                                    
                 
<asp:CustomValidator Display="Dynamic" CssClass="error" id="cvVisibleDependingControls" Runat="server" ErrorMessage="Please add at least 1 depending control details(s)." OnServerValidate="cvVisibleDependingControls_ServerValidate"></asp:CustomValidator>

<asp:Repeater ID="rptVisibleDependingControls" Runat="server">
<ItemTemplate>
    
<div>
    
<asp:LinkButton ID="librptVisibleDependingControlsRemoveItem" runat="server" CssClass="icon-remove" data-grunticon-embed CausesValidation="false"  OnClick="librptVisibleDependingControls_RemoveItem_Click">Remove</asp:LinkButton>
</div> 

<asp:requiredfieldValidator Display="Dynamic" CssClass="error" id="rfvDependingControl" Runat="server" ErrorMessage="Please enter/select the following: Depending Control." ControlToValidate="ddlDependingControl"></asp:RequiredFieldValidator>

<asp:label runat="server" associatedcontrolid="ddlDependingControl" id="lblDependingControlHeader" cssclass="required"  >Depending Control</asp:label>
<asp:Dropdownlist id="ddlDependingControl" Runat="server">
</asp:Dropdownlist>



<uc:TriggerValues ID="ucVisibilityTriggerValues" runat="server" />

<asp:label id="lblID" runat="server" visible="false" text='<%# Container.DataItem("ID") %>' />
</ItemTemplate>
</asp:Repeater><div>
<asp:Button ID="btnrptVisibleDependingControlsAddItem" runat="server" cssclass="button" CausesValidation="false" Text="Add depending control details" OnClick="btnrptVisibleDependingControlsAddItem_Click"></asp:Button>
</div> 






                                </asp:Panel>

                                <asp:Panel ID="pnlCustomVisibleValue" runat="server" Visible="false">
                                    
                                    <asp:RequiredFieldValidator ID="rfvCustomVisibleValue" runat="server" ControlToValidate="txtCustomVisibleValue" ErrorMessage="Please enter the custom visible value."></asp:RequiredFieldValidator>
                                    
                                    <asp:label runat="server" associatedcontrolid="txtCustomVisibleValue" id="lblCustomVisibleValue" cssclass="required"  >Custom Visible Value</asp:label>
<asp:TextBox id="txtCustomVisibleValue" runat="server" Width="300" CssClass="SlText" ></asp:TextBox>
                                </asp:Panel>
                                </asp:Panel>
		                        
		                        
                                <asp:Panel  visible="false" id="pnlCalendar" runat="server">
                                           <asp:RequiredFieldValidator ID="rfvCalendar" runat="server" ControlToValidate="rblCalendar" Display="dynamic" ErrorMessage="Please indicate whether or not you'd like to use a calendar for date selection."></asp:RequiredFieldValidator> 
                                    
                                <asp:label runat="server" associatedcontrolid="rblCalendar" id="lblCalendar" cssclass="required"  >Use calendar for date selection?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblCalendar" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" AutoPostBack="true">
                                        <asp:ListItem Value="0">No</asp:ListItem>
                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                    </asp:radiobuttonlist>
</fieldset>
</asp:Panel>
		                        
		                        <asp:Panel  visible="false" id="pnlValue" runat="server">
                                           <asp:RequiredFieldValidator ID="rfvRichtextbox" runat="server" ControlToValidate="rblRichTextbox" Display="dynamic" ErrorMessage="Please indicate whether or not you'd like to use a rich textbox for this value."></asp:RequiredFieldValidator> 
                                    
                                <asp:label runat="server" associatedcontrolid="rblRichTextbox" id="lblRichTextbox" cssclass="required"  >Use rich textbox for value?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblRichTextbox" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" AutoPostBack="true" OnSelectedIndexChanged="rblRichTextbox_SelectedIndexChanged">
                                        <asp:ListItem Value="0" Selected="true">No</asp:ListItem>
                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                    </asp:radiobuttonlist>
</fieldset>


                                    
		                        <asp:label runat="server" AssociatedControlID="">Value</asp:label>
		                                
                                <asp:Panel ID="pnlRegularValue" runat="server" >
                                    <asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="txtValue" ErrorMessage="Please enter the value." Enabled="false"></asp:RequiredFieldValidator>
		                        
		                        
                                    <asp:TextBox ID="txtValue" runat="server" CssClass="MlText" textmode="MultiLine" rows="3" Columns="100"></asp:TextBox>
                                </asp:Panel>

                                    <asp:Panel ID="pnlRichValue" runat="server" Visible="false">
                                        <asp:RequiredFieldValidator ID="rfvRichValue" runat="server" ControlToValidate="txtRichValue" ErrorMessage="Please enter the value." Enabled="false"></asp:RequiredFieldValidator>
		                        
		                        
                                        <asp:TextBox ID="txtRichValue" runat="server" TextMode="MultiLine" Rows="15" columns="80"  CssClass="MlText"></asp:TextBox>
                                    </asp:Panel>
		                         </asp:Panel>

                                   <asp:Panel  visible="false" id="pnlText" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvText" Display="Dynamic" runat="server" ControlToValidate="txtText" ErrorMessage="Please enter the text." Enabled="false"></asp:RequiredFieldValidator>
                                
		                        <asp:label runat="server" associatedcontrolid="txtText" id="lblText">Text</asp:label>
		                        
		                        
		                        <asp:TextBox ID="txtText" runat="server" TextMode="MultiLine" Width="300" CssClass="MlText" Rows="5" columns="100"></asp:TextBox>
		                         </asp:Panel>
		                        

                                <asp:Panel  visible="false" id="pnlPlaceholder" runat="server">
		                        <asp:label runat="server" associatedcontrolid="txtPlaceholder" id="lblPlaceholder">Placeholder</asp:label>
		                        
		                        
		                        <asp:TextBox ID="txtPlaceholder" runat="server" Width="500" MaxLength="500" CssClass="SlText"></asp:TextBox>
		                         </asp:Panel>


                                <asp:Panel  visible="false" id="pnlTextMode" runat="server">
					            
		                        
		                        <asp:label runat="server" associatedcontrolid="ddlTextMode" id="lblTextMode" >Text Mode</asp:label>
<asp:DropDownList id="ddlTextMode" AutoPostBack="true" runat="server">
		                            <asp:ListItem Value="SingleLine"></asp:ListItem>
		                            <asp:ListItem Value="MultiLine"></asp:ListItem>
		                            <asp:ListItem Value="Password"></asp:ListItem>
		                        </asp:DropDownList>

                                    <asp:Panel ID="pnlRichTextboxUser" runat="server" Visible="false">
                                               <asp:RequiredFieldValidator ID="rfvRichTextboxUser" runat="server" ControlToValidate="rblRichTextboxUser" Display="dynamic" ErrorMessage="Please indicate whether or not you'd like to take user input via rich textbox."></asp:RequiredFieldValidator> 
                                    
                                <asp:label runat="server" associatedcontrolid="rblRichTextboxUser" id="lblRichTextboxUser" cssclass="required"  >Use rich textbox for user input?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblRichTextboxUser" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" AutoPostBack="true">
                                        <asp:ListItem Value="0" Selected="true">No</asp:ListItem>
                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                    </asp:radiobuttonlist>
</fieldset>
                                        </asp:Panel>
		                        </asp:Panel>


                                <asp:Panel  visible="false" id="pnlRows" runat="server">
                                <asp:RequiredFieldValidator ID="rfvRows" runat="server" Display="Dynamic" ControlToValidate="txtRows" ErrorMessage="Please enter the number of rows."></asp:RequiredFieldValidator>
		                        <asp:CompareValidator ID="cmpRows" runat="server" ControlToValidate="txtRows" Operator="DataTypeCheck" Type="Integer" Errormessage="Please enter the number of rows using integers only."></asp:CompareValidator>
		                        
		                        <asp:label runat="server" associatedcontrolid="txtRows" id="lblRows" cssclass="required"  >Rows</asp:label>
<asp:TextBox id="txtRows" runat="server" CssClass="SlText" Width="50" MaxLength="5"></asp:TextBox>
		                        </asp:Panel>

                                <asp:Panel  visible="false" id="pnlColumns" runat="server">
                                <asp:RequiredFieldValidator ID="rfvColumns" runat="server" Display="Dynamic" ControlToValidate="txtColumns" ErrorMessage="Please enter the number of columns."></asp:RequiredFieldValidator>
		                        <asp:CompareValidator ID="cmpColumns" runat="server" ControlToValidate="txtColumns" Operator="DataTypeCheck" Type="Integer" Errormessage="Please enter the number of columns using integers only."></asp:CompareValidator>
		                        
		                        <asp:label runat="server" associatedcontrolid="txtColumns" id="lblColumns" cssclass="required"  >Columns</asp:label>
<asp:TextBox id="txtColumns" runat="server" CssClass="SlText" Width="50" MaxLength="5"></asp:TextBox>
		                        </asp:Panel>

		                             
		                        <asp:Panel  visible="false" id="pnlMaxLength" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvMaxLength" runat="server" Display="dynamic" ControlToValidate="txtMaxLength" ErrorMessage="Please enter the max length." Enabled="false"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="cmpMaxLength" runat="server" ControlToValidate="txtMaxFileSize" Operator="DataTypeCheck" Type="Integer" errormessage="Please enter the max length using numbers only."></asp:CompareValidator>
		                        
		                         <asp:label runat="server" associatedcontrolid="txtMaxLength" id="lblMaxLength">Max Length</asp:label>
		                        
		                        <asp:TextBox ID="txtMaxLength" runat="server" Width="40" CssClass="SlText" OnTextChanged="txtMaxLength_TextChanged" AutoPostBack="true" MaxLength="6"></asp:TextBox>

		                        
		                         <asp:label runat="server" associatedcontrolid="rblMaxLengthType" id="lblMaxLengthType">Max Length Type</asp:label>
		                        
		                        <fieldset>
<asp:radiobuttonlist ID="rblMaxLengthType" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" runat="server" >
		                            <asp:listitem Value="Characters" Selected ="true"></asp:listitem>
                                    <asp:listitem Value="Words"></asp:listitem>
		                        </asp:radiobuttonlist>
</fieldset>
                                    


		                         </asp:Panel>

                                   <asp:Panel  visible="false" id="pnlMaxFileSize" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvMaxFileSize" Display="dynamic" runat="server" ControlToValidate="txtMaxFileSize" ErrorMessage="Please enter the max file size in kilobytes." Enabled="false"></asp:RequiredFieldValidator>
                                       <asp:CompareValidator ID="cmpMaxFileSize" runat="server" ControlToValidate="txtMaxFileSize" Operator="DataTypeCheck" Type="Integer" errormessage="Please enter the max file size using numbers only."></asp:CompareValidator>
		                        
		                         <asp:label runat="server" associatedcontrolid="txtMaxFileSize" id="lblMaxFileSize">Max File Size (KB)</asp:label>
		                        
		                        <asp:TextBox ID="txtMaxFileSize" runat="server" Width="40" CssClass="SlText" MaxLength="6"></asp:TextBox>
		                         </asp:Panel>

                                      <asp:Panel  visible="false" id="pnlFileTypesAllowed" runat="server">
		                        <asp:customValidator ID="cvFileTypesAllowed" runat="server" ErrorMessage="Please select at least one allowed file type."></asp:customvalidator>
		                        
		                         <asp:label runat="server" associatedcontrolid="lsbFileTypesAllowed" id="lblFileTypesAllowed">File Types Allowed</asp:label>
		                        
						<asp:listbox id="lsbFileTypesAllowed" SelectionMode="Multiple"  Height="100" Width="200" runat="server" />
		                         </asp:Panel>

                                <asp:Panel ID="pnlUploadPath" runat="server" Visible="false">
                                         
                  <asp:CustomValidator ID="cvUploadPath" runat="server" ErrorMessage="Please select the upload path." />
            
            <asp:label runat="server" associatedcontrolid="pnlUploadPathFolder" id="lblUploadPathFolder" cssclass="required"  >Upload Path</asp:label>
<asp:Panel id="pnlUploadPathFolder" runat="server" Visible="false">
            
            <asp:label runat="server" associatedcontrolid="lblSelectedUploadPathFolder" >Selected Folder:</asp:label>
<asp:Label id="lblSelectedUploadPathFolder" runat="server" />
                </asp:Panel>

            
            <asp:ListBox ID="lsbUploadPathFolders" Height="150" Width="300" AutoPostBack="true" runat="server" />
       
            
            <asp:RequiredFieldValidator ID="rfvCreateUploadPathFolder" runat="server" ControlToValidate="rblCreateUploadPathFolder" ErrorMessage="Please indicate whether or not a new folder should be created for the upload path." />
            
            <asp:label runat="server" associatedcontrolid="rblCreateUploadPathFolder" id="lblCreateUploadPathFolder" cssclass="required"  >Create new folder at this location?</asp:label>
<fieldset>
<asp:radiobuttonlist ID="rblCreateUploadPathFolder" runat="server" AutoPostBack="true" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
                <asp:ListItem Value="0">No</asp:ListItem>
                <asp:ListItem Value="1">Yes</asp:ListItem>
            </asp:radiobuttonlist>
</fieldset>

            <asp:Panel ID="pnlNewUploadPathFolder" runat="server" Visible="false">
                <asp:RequiredFieldValidator ID="rfvNewUploadPathFolderName" runat="server" ControlToValidate="txtNewUploadPathFolderName" ErrorMessage="Please enter the name of the new folder to be created for the upload path." />
                
                <asp:label runat="server" associatedcontrolid="txtNewUploadPathFolderName" id="lblNewUploadPathFolderName" cssclass="required"  >New Folder Name</asp:label>
<asp:TextBox id="txtNewUploadPathFolderName" runat="server" Width="300" MaxLength="100" CssClass="SlText" AutoPostBack="True" />

            </asp:Panel>
                                </asp:Panel>
		                       
		                        <asp:Panel  visible="false" id="pnlWidth" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvWidth" runat="server" ControlToValidate="txtWidth" ErrorMessage="Please enter the width." Enabled="false"></asp:RequiredFieldValidator>
		                        <asp:label runat="server" associatedcontrolid="txtWidth" id="lblWidth">Width</asp:label>
		                        
		                        <asp:TextBox ID="txtWidth" runat="server" Width="40" CssClass="SlText" MaxLength="6"></asp:TextBox>
		                         </asp:Panel>
		                       
		                        <asp:Panel  visible="false" id="pnlCssClass" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvCssClass" runat="server" ControlToValidate="txtCssClass" ErrorMessage="Please enter the CssClass." Enabled="false"></asp:RequiredFieldValidator>
		                        
		                         <asp:label runat="server" associatedcontrolid="txtCssClass" id="lblCssClass">CSS Class</asp:label>
		                        
		                        <asp:TextBox ID="txtCssClass"  runat="server" Width="100" CssClass="SlText" MaxLength="50"></asp:TextBox>
		                        </asp:Panel>
		                        
		                        <asp:Panel  visible="false" id="pnlRepeatDirection" runat="server">
                                <asp:RequiredFieldValidator ID="rfvRepeatDirection" runat="server" ControlToValidate="rblRepeatDirection" ErrorMessage="Please select the repeat direction." Enabled="false"></asp:RequiredFieldValidator>
		                        
		                         <asp:label runat="server" associatedcontrolid="rblRepeatDirection" id="lblRepeatDirection">Repeat Direction</asp:label>
		                        
		                        <fieldset>
<asp:radiobuttonlist ID="rblRepeatDirection" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
		                            <asp:ListItem Value="Vertical"></asp:ListItem>
		                            <asp:ListItem Value="Horizontal"></asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>
		                        </asp:Panel>
		                        
		                        
		                            <asp:Panel  visible="false" id="pnlRepeatColumns" runat="server">
                                <asp:RequiredFieldValidator ID="rfvRepeatColumns" runat="server" ControlToValidate="ddlRepeatColumns" ErrorMessage="Please select the number of repeat columns." Enabled="false"></asp:RequiredFieldValidator>
		                        <asp:label runat="server" associatedcontrolid="ddlRepeatColumns" id="lblRepeatColumns">Repeat Columns</asp:label>
		                        
		                        <asp:dropdownList ID="ddlRepeatColumns" runat="server"></asp:dropdownList>
		                         </asp:Panel>

                                       <asp:Panel  visible="false" id="pnlListSelections" runat="server">
                                <asp:RequiredFieldValidator ID="rfvListSelections" runat="server" ControlToValidate="rblListSelections" ErrorMessage="Please indicate whether or not selections should be listed above the control." Enabled="false"></asp:RequiredFieldValidator>
		                        
		                         <asp:label runat="server" associatedcontrolid="rblListSelections" id="lblListSelections">List selections above?</asp:label>
		                        
		                        <fieldset>
<asp:radiobuttonlist ID="rblListSelections" runat="server" AutoPostBack="true" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
                <asp:ListItem Value="0">No</asp:ListItem>
                <asp:ListItem Value="1">Yes</asp:ListItem>
            </asp:radiobuttonlist>
</fieldset>
		                         </asp:Panel>
		                        
		                           <asp:Panel  visible="false" id="pnlGroupName" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvGroupName" runat="server" ControlToValidate="txtGroupName" ErrorMessage="Please enter the group name." Enabled="false"></asp:RequiredFieldValidator>
<asp:label runat="server" associatedcontrolid="txtGroupName" id="lblGroupName">Group Name</asp:label>     
		                        <asp:TextBox ID="txtGroupName" runat="server" Width="300" CssClass="SlText" MaxLength="50"></asp:TextBox>
		                        </asp:Panel>
		                        
		                        
		                        <asp:Panel  visible="false" id="pnlSelectionMode" runat="server">
                                <asp:RequiredFieldValidator ID="rfvSelectionMode" runat="server" ControlToValidate="rblSelectionMode" ErrorMessage="Please select the repeat direction." Enabled="false"></asp:RequiredFieldValidator>
		                        
		                         <asp:label runat="server" associatedcontrolid="rblSelectionMode" id="lblSelectionMode">Selection Mode</asp:label>
		                        
		                        <fieldset>
<asp:radiobuttonlist ID="rblSelectionMode" runat="server" AutoPostBack="true" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
		                            <asp:ListItem Value="Multiple"></asp:ListItem>
		                            <asp:ListItem Value="Single"></asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>
		                        </asp:Panel>
		                        
		                          <uc:ListItems ID="ucLSI" runat="server" />
		                         
		                        <asp:Panel ID="pnlSQLDataType" Visible="false" runat="server">
		                        <asp:RequiredFieldValidator ID="rfvSQLDataType" runat="server" ControlToValidate="ddlSQLDataType" ErrorMessage="Please select this control's SQL data type."></asp:RequiredFieldValidator>
		                        
		                        <asp:label runat="server" associatedcontrolid="ddlSQLDataType" id="lblSQLDataType" cssclass="required"  >SQL Data Type</asp:label>
<asp:dropdownList id="ddlSQLDataType" runat="server" AutoPostBack="true" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline"></asp:dropdownList>
		                        </asp:Panel>
                                
                                

		                         <asp:Panel  visible="false" id="pnlSQLDataSize" runat="server">
		                         <asp:RequiredFieldValidator ID="rfvSQLDataSize" runat="server" Display="Dynamic" ControlToValidate="txtSQLDataSize" ErrorMessage="Please enter the SQL data size."></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="cvSQLDataSize" runat="server" ErrorMessage="Please enter the SQL data size using only numbers or MAX."></asp:CustomValidator>
		                         
		                         
                                <asp:label runat="server" associatedcontrolid="txtSQLDataSize" id="lblSQLDataSize" cssclass="required"  >SQL Data Size</asp:label>
<asp:TextBox id="txtSQLDataSize" runat="server" Width="45" CssClass="SlText" MaxLength="5"></asp:TextBox>
		                        </asp:Panel>

                                <asp:Panel  visible="false" id="pnlSQLDefaultValue" runat="server">
                                <asp:RequiredFieldValidator  CssClass="error" ID="rfvSQLDefaultValueType" runat="server" ControlToValidate="rblSQLDefaultValueType" ErrorMessage="Please select the SQL default value type."></asp:RequiredFieldValidator>
                                <asp:Label runat="server" CssClass="required" AssociatedControlID="rblSQLDefaultValueType">Default Value Type</asp:Label>
                                <asp:RadioButtonList runat="server" ID="rblSQLDefaultValueType" RepeatDirection="horizontal">
                                    <asp:ListItem Value="Text" Selected="true">Text</asp:ListItem>
                                    <asp:ListItem Value="Function">Function</asp:ListItem>
                                </asp:RadioButtonList>

		                         <asp:RequiredFieldValidator ID="rfvSQLDefaultValue" runat="server" Display="Dynamic" ControlToValidate="txtSQLDefaultValue" ErrorMessage="Please enter the SQL default value."></asp:RequiredFieldValidator>
                                 
                                <asp:label runat="server" associatedcontrolid="txtSQLDefaultValue" id="lblSQLDefaultValue" cssclass="required"  >SQL Default Value</asp:label>
<asp:TextBox id="txtSQLDefaultValue" runat="server" Width="300" CssClass="SlText" MaxLength="50"></asp:TextBox>
		                        </asp:Panel>
		                        
                                <asp:Panel  visible="false" id="pnlAllowNegativeValue" runat="server">
                                 
		                         
                                <asp:label runat="server" associatedcontrolid="rblAllowNegativeValue" id="lblAllowNegativeValue">Allow negative value?</asp:label>
		                        
		                        <fieldset>
<asp:radiobuttonlist runat="server" id="rblAllowNegativeValue" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
		                            <asp:ListItem value="0" Selected="true">No</asp:ListItem>
                                    <asp:ListItem value="1">Yes</asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>
		                        </asp:Panel>
                                
                                <asp:Panel  visible="false" id="pnlAllowZeroValue" runat="server">
                                 
		                         
                                <asp:label runat="server" associatedcontrolid="rblAllowZeroValue" id="lblAllowZeroValue">Allow zero value?</asp:label>
		                        
		                        <fieldset>
<asp:radiobuttonlist runat="server" id="rblAllowZeroValue" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
		                            <asp:ListItem value="0" Selected="true">No</asp:ListItem>
                                    <asp:ListItem value="1">Yes</asp:ListItem>
		                        </asp:radiobuttonlist>
</fieldset>
		                        </asp:Panel>

		                              <asp:Panel  visible="false" id="pnlSQLInsertItemTable" runat="server">
                              <asp:RequiredFieldValidator ID="rfvSQLInsertItemTable" runat="server" ControlToValidate="txtSQLInsertItemTable" ErrorMessage="Please enter the SQL inserted items table name."></asp:RequiredFieldValidator>
		                        
		                         <asp:label runat="server" associatedcontrolid="txtSQLInsertItemTable" id="lblSQLInsertItemTable">SQL Table For Inserted Items</asp:label> (do not include prefix or action)&nbsp;<span class="required">(required)</span>
		                        
		                        <asp:TextBox ID="txtSQLInsertItemTable" runat="server" Width="300" CssClass="SlText" MaxLength="100"></asp:TextBox>
		                         </asp:Panel>

                                      
		                             <asp:Panel  visible="false" id="pnlSQLInsertItemStoredProcedure" runat="server">
                              <asp:RequiredFieldValidator ID="rfvSQLInsertItemStoredProcedure" runat="server" ControlToValidate="txtSQLInsertItemStoredProcedure" ErrorMessage="Please enter SQL insert stored procedure name."></asp:RequiredFieldValidator>
		                        
		                         <asp:label runat="server" associatedcontrolid="txtSQLInsertItemStoredProcedure" id="lblSQLInsertItemStoredProcedure">SQL Stored Procedure For Inserting Items</asp:label> (do not include prefix or action)&nbsp;<span class="required">(required)</span>
		                        
		                        <asp:TextBox ID="txtSQLInsertItemStoredProcedure" runat="server" Width="300" CssClass="SlText" MaxLength="100"></asp:TextBox>
		                         </asp:Panel>

                                                   <asp:Panel  visible="false" id="pnlForeignID" runat="server">
                              <asp:RequiredFieldValidator ID="rfvForeignID" runat="server" ControlToValidate="txtForeignID" ErrorMessage="Please enter the foreign id colum name."></asp:RequiredFieldValidator>
		                        
		                         <asp:label runat="server" associatedcontrolid="txtForeignID" id="lblForeignID">Foreign ID Column Name</asp:label> &nbsp;<span class="required">(required)</span>
		                        
		                        <asp:TextBox ID="txtForeignID" runat="server" Width="300" CssClass="SlText" MaxLength="50"></asp:TextBox>
		                         </asp:Panel>
		            
<asp:panel ID="pnlProspectCode" visible="false" runat="server">
    <asp:requiredfieldvalidator ID="rfvProspectCode" runat="server" controltovalidate="txtProspectCode" Display="dynamic" ErrorMessage="Please enter the prospect code"></asp:requiredfieldvalidator>
    <asp:label id="lblProspectCode" runat="server" AssociatedControlID="txtProspectCode" cssclass="required">Prospect Code</asp:label>
    <asp:TextBox runat="server" id="txtProspectCode" maxlength="10"></asp:TextBox>
</asp:panel>
		        </asp:Panel>
                                       <asp:Label ID="lblCurrentControlID" runat="server" Visible="true"></asp:label>
<asp:Label ID="lblCurrentControlPosition" runat="server" Visible="true"></asp:label>

    </div>


                                			
		</ContentTemplate>
    </asp:UpdatePanel>    
		<div style="text-align:center">
		<asp:Button ID="btnContinue" runat="server" Text="Continue" cssclass="button" 
                Height="26px" />
		</div>
		<div style="text-align:center">
		    <asp:ValidationSummary CssClass="error" ID="vsWebRAD" runat="server" />
		</div>

      </td>
            </tr>
        </table>

       <script type="text/javascript" src="https://media.whitworth.edu/js/ckeditor/ckeditor.js"></script>

        <script type="text/javascript" >
            $(document).ready(function () {
                CKEDITOR.replace('MainContent_txtRichValue');
            });
        </script>
                <script type="text/javascript" src="https://media.whitworth.edu/js/tinymce/tinymce.min.js"></script>
        <script language="javascript" type="text/javascript" src="LoadRichText.js"></script>
         <script src="src/jquery.contextMenu.js" type="text/javascript"></script>
 
</asp:Content>

