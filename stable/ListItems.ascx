<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ListItems.ascx.vb" inherits="stable.WebRADListItems" %>
<%@ Register TagPrefix="uc" TagName="DataSource" Src="DataSource.ascx" %>

		                        
		                        <asp:Panel ID="pnlSelectionItems" runat="server" Visible="false">
		                          <asp:RequiredFieldValidator ID="rfvSelectionItems" runat="server" ControlToValidate="ddlSelectionItems" ErrorMessage="Please select the method which will be used to supply the list with selection items." Enabled="false"></asp:RequiredFieldValidator>
		                        <br />
		                         <table>
		                            <tr>
		                                <td><strong> <strong>What method will be used to supply this list with selection items?</strong></strong></td>
		                                <td><asp:Panel ID="pnlSelectionItemsRequired" runat="server" Visible="false">&nbsp;<span class="required">(required)</span></asp:Panel></td>
		                            </tr>
		                        </table>
		                        
		                        <br />
		                        <asp:dropdownList ID="ddlSelectionItems" runat="server" AutoPostBack="true">
		                            <asp:ListItem Value="">Please Select</asp:ListItem>
		                            <asp:ListItem Value="1">I will specify the list item data.</asp:ListItem>
		                            <asp:ListItem Value="2">I will specify a SQL data source for the data.</asp:ListItem>
		                            <asp:ListItem Value="4">I will specify a data method for providing the data.</asp:ListItem>
		                            <asp:ListItem Value="3">No items will be supplied.</asp:ListItem>
		                        </asp:dropdownList>
		                         </asp:Panel>
		                    
		                        <asp:Panel  visible="false" id="pnlListItems" runat="server">
		                        <asp:requiredfieldvalidator ID="rfvListItems" runat="server" ErrorMessage="Please select the number of list items to be included with this radiobuttonlist." ControlToValidate="ddlListItems"></asp:requiredfieldvalidator>
		                        <br />
		                        <table>
		                            <tr>
		                                <td><strong>List Items</strong></td>
		                                <td><asp:Panel ID="pnlListItemsRequired" runat="server" Visible="false">&nbsp;<span class="required">(required)</span></asp:Panel></td>
		                            </tr>
		                        </table>
		                        
		                        <br />
		                        I would like to specify <asp:DropDownList ID="ddlListItems" AutoPostBack="true" runat="server"></asp:DropDownList>&nbsp; list item(s).&nbsp;<span class="required">(required)</span>
		                        <br /><br />
		                        <table width="450">
		                        <asp:Repeater ID="rptListItems" runat="server">
		                            <ItemTemplate>
		                            <tr>
		                                <td align="center" valign="middle">&nbsp;</td>
		                                <td align="center" valign="middle"><strong>Text</strong></td>
		                                <td align="center" valign="middle"><strong>Value</strong></td>
		                            </tr>
		                                <tr>
		                                    <td align="center" valign="middle"><strong><%# container.itemindex + 1 %>.</strong></td>
		                                    <td align="center" valign="middle"><asp:TextBox ID="txtText" Text='<%# container.dataitem("Text") %>' runat="server" TextMode="MultiLine" rows="3" columns="50" CssClass="MlText"></asp:TextBox></td>
		                                    <td align="center" valign="middle"><asp:TextBox ID="txtValue" Text='<%# container.dataitem("Value") %>' runat="server" Width="200" CssClass="SlText" MaxLength="100"></asp:TextBox></td>
		                                </tr>
		                            </ItemTemplate>
		                        </asp:Repeater>
		                        </table>
		                        
		                        <asp:Label ID="lblListItemCount" runat="server" Visible="false"></asp:Label>
		                        </asp:Panel>
		                     
		                  
		                      <asp:Panel  visible="false" id="pnlDataMethod" runat="server">
                              <asp:RequiredFieldValidator ID="rfvDataMethod" runat="server" ControlToValidate="ddlDataMethod" ErrorMessage="Please select the data method."></asp:RequiredFieldValidator>
		                        <br />
		                        <strong>Data Method</strong>&nbsp;<span class="required">(required)</span>
                                <br />		                        
                                <asp:DropDownList ID="ddlDataMethod" AutoPostBack="true" runat="server"></asp:DropDownList>
                            </asp:Panel>

                                <asp:Panel ID="pnlOtherDataMethod" runat="server" Visible="false">
                                <asp:RequiredFieldValidator ID="rfvOtherDataMethod" runat="server" ControlToValidate="txtOtherDataMethod" ErrorMessage="Please enter the other data method."></asp:RequiredFieldValidator>
                                <br />
                                <strong>Other Data Method</strong> <span class="required">(required)</span>
                                <br />
		                        <asp:TextBox ID="txtOtherDataMethod" runat="server" Width="300" CssClass="SlText" MaxLength="500"></asp:TextBox>
                            </asp:Panel>

                                      <asp:Panel  visible="false" id="pnlIncludePleaseSelect" runat="server">
		                           <br />
		                           <asp:RequiredFieldValidator ID="rfvIncludePleaseSelect" runat="server" ControlToValidate="rblIncludePleaseSelect" ErrorMessage="Please indicate whether or not a Please Select item should be included."></asp:RequiredFieldValidator>
		                           <br />
		                           <strong>Should a "Please Select" item be included (value of "")?</strong> <span class="required">(required)</span>
		                           <br />
		                           <asp:RadioButtonList ID="rblIncludePleaseSelect" runat="server" RepeatDirection="Horizontal">
		                            <asp:ListItem Value="0" Selected="True">No</asp:ListItem>
		                            <asp:ListItem Value="1">Yes</asp:ListItem>
		                           </asp:RadioButtonList>
		                        </asp:Panel>

                                      <asp:Panel  visible="false" id="pnlMinimumValue" runat="server">
                                          
                                <asp:RequiredFieldValidator ID="rfvMinimumValue" runat="server" Display="Dynamic" ControlToValidate="txtMinimumValue" ErrorMessage="Please enter the minimum number."></asp:RequiredFieldValidator>
                                              
                                <br />
		                        <strong>Minimum Value</strong><asp:panel id="pnlMinimumValueRequired" runat="server"></asp:panel>
		                        <br />
		                        <asp:TextBox ID="txtMinimumValue" runat="server" Width="300" CssClass="SlText" MaxLength="100"></asp:TextBox>
		                        </asp:Panel>
		                     
		                     <asp:Panel  visible="false" id="pnlMaximumValue" runat="server">
		                         
                                <asp:RequiredFieldValidator ID="rfvMaximumValue" runat="server" Display="Dynamic" ControlToValidate="txtMaximumValue" ErrorMessage="Please enter the maximum number."></asp:RequiredFieldValidator>
                                     
                                <br />
		                        <strong>Maximum Value</strong><asp:panel id="pnlMaximumValueRequired" runat="server"></asp:panel>
		                        <br />
		                        <asp:TextBox ID="txtMaximumValue" runat="server" Width="300" CssClass="SlText" MaxLength="100"></asp:TextBox>
		                        </asp:Panel>
		               

<uc:DataSource ID="ucDataSource" runat="server" />
		                        
		                        
		                        