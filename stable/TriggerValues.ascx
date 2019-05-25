<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="TriggerValues.ascx.vb" inherits="stable.TriggerValues" %>
<br />
                                                <asp:RequiredFieldValidator ID="rfvNumberTriggerValues" runat="server" ControlToValidate="ddlNumberTriggerValues" ErrorMessage="Please indicate the number of trigger values." />
                                            <br />
                                            <strong>How many values will trigger this action (0 defaults to "")?</strong> <span class="required">(required)</span>
                                            <br />
                                            <asp:DropDownList ID="ddlNumberTriggerValues" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlNumberTriggerValues_SelectedIndexChanged">
                                                <asp:ListItem value="0"></asp:ListItem>
                                                <asp:ListItem value="1"></asp:ListItem>
                                                <asp:ListItem value="2"></asp:ListItem>
                                                <asp:ListItem value="3"></asp:ListItem>
                                                <asp:ListItem value="4"></asp:ListItem>
                                                <asp:ListItem value="5"></asp:ListItem>
                                                <asp:ListItem value="6"></asp:ListItem>
                                                <asp:ListItem value="7"></asp:ListItem>
                                                <asp:ListItem value="8"></asp:ListItem>
                                                <asp:ListItem value="9"></asp:ListItem>
                                                <asp:ListItem value="10"></asp:ListItem>
                                            </asp:DropDownList>

                                            <asp:Repeater ID="rptTriggerValues" runat="server">
                                                <ItemTemplate>
                                            <br />
                                            <br />
                                                    <div class="stack-container">
                                                        <div class="stack" style="float:left;">
                                                            <strong>Trigger Operator #<%# Container.ItemIndex + 1%></strong>
                                            <br />
                                            <asp:DropDownList ID="ddlTriggerOperator" runat="server">
                                                <asp:ListItem Value="="></asp:ListItem>
                                                <asp:ListItem Value="<>"></asp:ListItem>
                                                <asp:ListItem Value=">"></asp:ListItem>
                                                <asp:ListItem Value=">="></asp:ListItem>
                                                <asp:ListItem Value="<"></asp:ListItem>
                                                <asp:ListItem Value="<="></asp:ListItem>
                                            </asp:DropDownList>

                                                        </div>
                                                        <div class="stack" style="float:left;">
                                                                                                        <strong>Trigger Value #<%# Container.ItemIndex + 1%></strong>
                                                    <br />
                                                    <asp:panel ID="pnlShowListTriggerValue" runat="server" visible="false">
                                                        <asp:dropdownlist ID="ddlSelectTriggerValue" OnSelectedIndexChanged="ddlSelectTriggerValue_SelectedIndexChanged" runat="server"></asp:dropdownlist>
                                                    </asp:panel>
                                                    
                                                    <asp:panel ID="pnlShowTextTriggerValue" runat="server">
                                                    <asp:TextBox ID="txtTriggerValue" runat="server" Width="300" cssclass="SlText" />
                                                        </asp:panel>

                                                        </div>
                                                    </div>
                                            
                                                    <br />
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            