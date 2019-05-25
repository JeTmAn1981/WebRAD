<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DataSource.ascx.vb" inherits="stable.WebRADDataSource" %>

<asp:Panel ID="pnlDataSourceType" runat="server" Visible='<%# showType %>'>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="rblDataSourceType" ErrorMessage="Please select the data source type."></asp:RequiredFieldValidator>
		                        <br />
		                        
                                <asp:label runat="server" AssociatedControlID="rblDataSourceType" CssClass="required">Data Source Type</asp:label>
                                <asp:RadioButtonList ID="rblDataSourceType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblDataSourceType_SelectedIndexChanged">
                                    <asp:ListItem Value="1">I would like to specify each part of the SQL select statement.</asp:ListItem>
                                    <asp:ListItem Value="2">I would like to write the SQL select statement as a single piece of plain text.</asp:ListItem>
		                        </asp:RadioButtonList>
                                </asp:Panel>

		                        <asp:Panel  visible="false" id="pnlDataSource" runat="server">
                                <asp:RequiredFieldValidator ID="rfvDataSource" runat="server" ControlToValidate="txtDataSource" ErrorMessage="Please enter the data source."></asp:RequiredFieldValidator>
		                        <asp:Label id="lblDataSource" runat="server" AssociatedControlID="txtDataSource" CssClass="required">Data Source</asp:Label>
<asp:TextBox ID="txtDataSource" runat="server"  CssClass="MlText" TextMode="MultiLine" Rows="5" Columns="100"></asp:TextBox>

		                            <br/>
                                    <asp:checkbox id="chkUseBackendDatasource"  runat="server" AutoPostBack="true" Text="Use separate backend data source?" OnCheckedChanged="chkUseBackendDatasource_OnCheckedChanged" />
                                <asp:panel ID="pnlBackendDAtasource" runat="server" Visible="false">
                                    <br />
                                    <asp:RequiredFieldValidator ID="rfvBackendDatasource" runat="server" ControlToValidate="txtBackendDatasource" ErrorMessage="Please enter the data source."></asp:RequiredFieldValidator>
                                    <br />
		                        <asp:Label id="lblBackendDatasource" runat="server" AssociatedControlID="txtBackendDatasource" CssClass="required">Backend Data Source</asp:Label>
                                <asp:TextBox ID="txtBackendDatasource" runat="server"  CssClass="MlText" TextMode="MultiLine" Rows="5" Columns="100"></asp:TextBox>

		                        
                                </asp:panel>
    		                        </asp:Panel>

                                <asp:Panel ID="pnlDataSourceSpecific" runat="server" Visible="<%# showSpecific %>">
                                    
                                <asp:RequiredFieldValidator ID="rfvDataSourceSelect" runat="server" CssClass="error" ControlToValidate="txtDataSourceSelect" ErrorMessage="Please enter the select portion of the data source statement."></asp:RequiredFieldValidator>
                                <asp:Label id="lblDataSourceSelectHeader" runat="server" AssociatedControlID="txtDataSourceSelect" CssClass="required">Data Source SELECT Section (do not include "SELECT")</asp:Label>
                                <asp:TextBox ID="txtDataSourceSelect" runat="server" ></asp:TextBox>
                                    
                                <br />
                                <br />
                                <asp:Label id="lblDataSourceTableHeader" runat="server" AssociatedControlID="txtDataSourceTable" CssClass="">Data Source TABLE Section (do not include "FROM")</asp:Label>
                                <asp:TextBox ID="txtDataSourceTable" runat="server" ></asp:TextBox>

                                <br />
                                <br />
                                <asp:Label id="lblDataSourceWhereHeader" runat="server" AssociatedControlID="txtDataSourceWhere" CssClass="">Data Source WHERE Section (do not include "WHERE")</asp:Label>
                                <asp:TextBox ID="txtDataSourceWhere" runat="server"  MaxLength="500"></asp:TextBox>

                                    <br />
                                <br />
                                <asp:Label id="lblDataSourceGroupByHeader" runat="server" AssociatedControlID="txtDataSourceGroupBy" CssClass="">Data Source GROUP BY Section (do not include "GROUP BY")</asp:Label>
                                <asp:TextBox ID="txtDataSourceGroupBy" runat="server"  MaxLength="500"></asp:TextBox>

                                <br />
                                <br />
                                <asp:Label id="lblDataSourceOrderByHeader" runat="server" AssociatedControlID="txtDataSourceOrderBy" CssClass="">Data Source ORDER BY Section (do not include "ORDER BY")</asp:Label>
                                <asp:TextBox ID="txtDataSourceOrderBy" runat="server"  MaxLength="500"></asp:TextBox>

                                </asp:Panel>

<asp:Panel  visible="false" id="pnlDataTextField" runat="server">
                                <asp:RequiredFieldValidator ID="rfvDataTextField" runat="server" ControlToValidate="txtDataTextField" ErrorMessage="Please enter the data text field."></asp:RequiredFieldValidator>
		                        <br />
		                        <asp:Label id="lblDataTextFieldHeader" runat="server" AssociatedControlID="txtDataTextField" CssClass="required">Data Text Field</asp:Label>
                                <asp:TextBox ID="txtDataTextField" runat="server"  MaxLength="50"></asp:TextBox>
		                        </asp:Panel>
		                        
		                        <asp:Panel  visible="false" id="pnlDataValueField" runat="server" >
                                <asp:RequiredFieldValidator ID="rfvDataValueField" runat="server" ControlToValidate="txtDataValueField" ErrorMessage="Please enter the data value field."></asp:RequiredFieldValidator>
		                        <br />
		                        <asp:Label id="lblDataValueFieldHeader" runat="server" AssociatedControlID="txtDataValueField" CssClass="required">Data Value Field</asp:Label>
                                <asp:TextBox ID="txtDataValueField" runat="server"  MaxLength="50"></asp:TextBox>
		                        </asp:Panel>