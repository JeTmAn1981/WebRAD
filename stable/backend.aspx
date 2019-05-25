<%@ Page Language="vb" validaterequest="false" AutoEventWireup="false" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Template.Master" CodeBehind="backend.aspx.vb" Inherits="stable.backendnew" %>
<%@ Register TagPrefix="uc" TagName="DataSource" Src="DataSource.ascx" %>
<asp:Content ID="SectionName" ContentPlaceHolderID="SectionName" runat="server">Backend</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>

        
    <script type="text/javascript" src="/js/tinymce/tinymce.min.js"></script>
        <script language="javascript" type="text/javascript" src="LoadRichText.js"></script>
    
            <asp:RequiredFieldValidator ID="rfvIncludebackend" runat="server" ControlToValidate="rblIncludebackend" ErrorMessage="Please indicate whether or not you'd like to include a backend." />
            <asp:label cssclass="required" runat="server" AssociatedControlID="rblIncludeBackend">Include backend?</asp:label>
            <asp:RadioButtonList ID="rblIncludebackend" runat="server" RepeatDirection="Horizontal" autopostback="true">
                <asp:ListItem Value="0">No</asp:ListItem>
                <asp:ListItem Value="1" Selected="true">Yes</asp:ListItem>
            </asp:RadioButtonList>


            <asp:Panel ID="pnlBackend" runat="server">
                
<asp:Repeater ID="rptTest" runat="server">
    <ItemTemplate>
        <uc:DataSource ID="ucDataSource" runat="server" />

        <asp:Label ID="lblID" runat="server"></asp:Label>
    </ItemTemplate>
</asp:Repeater>


                  <asp:CustomValidator ID="cvBackendPath" runat="server" ErrorMessage="Please select the path for the project backend." />
            <label class="required">Backend Path</label>
           
            <asp:Panel ID="pnlBackendFolder" runat="server" Visible="false">
            <br /><Br />
            <strong>Selected Folder:</strong>
            <br />
            <asp:Label ID="lblSelectedBackendFolder" runat="server" />
                </asp:Panel>

            <br /><br />
            <asp:ListBox ID="lsbBackendFolders" Height="150" Width="300" AutoPostBack="true" runat="server" />
       
            <br />
            <asp:RequiredFieldValidator ID="rfvCreateBackendFolder" runat="server" ControlToValidate="rblCreateBackendFolder" ErrorMessage="Please indicate whether or not a new folder should be created for the backend." />
            <br />
            <label class="required">Create new folder at this location?</label>
            <br />
            <asp:RadioButtonList ID="rblCreateBackendFolder" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                <asp:ListItem Value="0">No</asp:ListItem>
                <asp:ListItem Value="1">Yes</asp:ListItem>
            </asp:RadioButtonList>

            <asp:Panel ID="pnlNewBackendFolder" runat="server" Visible="false">
                <asp:RequiredFieldValidator ID="rfvNewBackendFolderName" runat="server" ControlToValidate="txtNewBackendFolderName" ErrorMessage="Please enter the name of the new folder to be created for the backend." />
                <br />
                <label class="required">New Folder Name</label>
                <br />
                <asp:TextBox ID="txtNewBackendFolderName" runat="server" Width="300" MaxLength="100" CssClass="SlText" AutoPostBack="True" />

            </asp:Panel>


               <asp:RequiredFieldValidator ID="rfvBackendLink" runat="server" Display="dynamic" ControlToValidate="txtBackendLink"  ErrorMessage="Please enter the Backend link for this project." />
                <asp:CustomValidator ID="cvBackendLink" runat="server" OnServerValidate="cvBackendLink_ServerValidate" ErrorMessage="Temp"></asp:CustomValidator>
            <br />
            <strong>Backend Link</strong> <span class="required">required)</span>
            <br />
            <span class="smText">(ex.: Administration/BackendDirectory/)</span>
            <br />
            <asp:TextBox ID="txtBackendLink" runat="server" Width="700" MaxLength="500" CssClass="SlText" />
		


                <br />
                <Br />
                <strong>I would like to display the following columns on the main backend page:  </strong>
                <br /><br />
                <asp:Label ID="lblSelectedDisplayColumns" runat="server"></asp:Label>
                      <asp:ListBox ID="lsbDisplayColumns" SelectionMode="Multiple"  Height="100" runat="server"></asp:ListBox>
          
                <br />
                <Br />
                <strong>I would like to include the following backend options:</strong>
                <br />

                
                <asp:CheckBoxList ID="cblBackendOptions" runat="server" AutoPostBack="true"  RepeatLayout="UnorderedList" CssClass="unstyled-list-inline" OnSelectedIndexChanged="cblBackendOptions_SelectedIndexChanged"></asp:CheckBoxList>

                <asp:Panel ID="pnlShowOptionDetails" runat="server">
   
<ajaxToolkit:TabContainer runat="server" ID="OptionTabs">
                 
    <ajaxToolkit:TabPanel runat="server" HeaderText="Search" ID="Search">
        <ContentTemplate>
                    <h1>Search Parameters</h1>

                <br />
                <asp:RequiredFieldValidator ID="rfvSearchColumns" runat="server" ControlToValidate="ddlSearchColumns" ErrorMessage="Please indicate how many columns should be on the search page."></asp:RequiredFieldValidator>
                <asp:Label id="lblSearchColumns" runat="server" AssociatedControlID="ddlSearchColumns" CssClass="required">How many columns should be on the search page?</asp:Label>
<asp:Dropdownlist ID="ddlSearchColumns" runat="server"></asp:Dropdownlist>

                <br />
                <Br />
                <strong>I would like to make the following controls available as search terms (ID and Date Submitted are included by default):  </strong>
                <br /><br />
                <asp:Label ID="lblSelectedSearchTermControls" runat="server" ></asp:Label>
                    <br /><br />
                      <asp:ListBox ID="lsbSearchTermControls" SelectionMode="Multiple"  Height="100" runat="server"></asp:ListBox>
          


                <br />
                <Br />
                <strong>I would like to display the following columns in the search results (ID and Date Submitted are included by default):  </strong>
                <br /><br />
                <asp:Label ID="lblSelectedSearchDisplayColumns" runat="server"></asp:Label>
                    <br /><br />
                      <asp:ListBox ID="lsbSearchDisplayColumns" SelectionMode="Multiple"  Height="100" runat="server"></asp:ListBox>
          
                    </table>
                
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" HeaderText="Actions other than delete">
        <ContentTemplate>
                    <h1>Action Parameters</h1>

                    <asp:requiredfieldvalidator ID="rfvActions" runat="server" CssClass="error" ControlToValidate="ddlActions" ErrorMessage="Please indicate how many actions you would like."></asp:requiredfieldvalidator>
                    <asp:Label id="lblActions" runat="server" AssociatedControlID="ddlActions" CssClass="required">How many actions would you like?</asp:Label>
<asp:DropDownList ID="ddlActions" AutoPostBack="true" OnSelectedIndexChanged="ddlActions_SelectedIndexChanged" runat="server"></asp:DropDownList>

                    <ol>
                    <asp:Repeater ID="rptActions" runat="server">
                        <ItemTemplate>
                            <li>
                            <asp:RequiredFieldValidator ID="rfvLabel" CssClass="error" runat="server" ControlToValidate="txtLabel" ErrorMessage="Please enter the action label."></asp:RequiredFieldValidator>
                            <asp:Label id="lblLabelHeader" runat="server" AssociatedControlID="txtLabel" CssClass="required">Action Label</asp:Label>
<asp:TextBox ID="txtLabel" runat="server" csclass="SlText" Width="300" MaxLength="500"></asp:TextBox>


                            <br />
                            <asp:RequiredFieldValidator ID="rfvActionType" CssClass="error" runat="server" ControlToValidate="ddlActionType" ErrorMessage="Please select the action type."></asp:RequiredFieldValidator>
                            <asp:Label id="lblActionTypeHeader" runat="server" AssociatedControlID="ddlActionType" CssClass="required">Action Type</asp:Label>
<asp:DropDownList ID="ddlActionType" AutoPostBack="true" OnSelectedIndexChanged="ddlActionType_SelectedIndexChanged" runat="server"></asp:DropDownList>


                                <asp:Panel ID="pnlControl" runat="server" Visible="false">
                                <br />
                            <asp:RequiredFieldValidator ID="rfvControlName" CssClass="error" runat="server" ControlToValidate="ddlControlID" ErrorMessage="Please select the control for this action."></asp:RequiredFieldValidator>
                            <asp:Label id="lblControlIDHeader" runat="server" AssociatedControlID="ddlControlID" CssClass="required">Control Name</asp:Label>
<asp:DropDownList ID="ddlControlID" runat="server"></asp:DropDownList>
                                    </asp:Panel>


                            <asp:Panel ID="pnlUpdateValue" runat="server" Visible="false">
                                
                            <asp:RequiredFieldValidator ID="rfvUpdateValue" CssClass="error" runat="server" ControlToValidate="txtUpdateValue" ErrorMessage="Please enter the update value."></asp:RequiredFieldValidator>
                            <asp:Label id="lblUpdateValueHeader" runat="server" AssociatedControlID="txtUpdateValue" CssClass="required">Update Value</asp:Label>
<asp:TextBox ID="txtUpdateValue" runat="server" csclass="SlText" Width="100" MaxLength="50"></asp:TextBox>
                                </asp:Panel>

                                  <asp:Panel ID="pnlCustomActionCode" runat="server" Visible="false">
                                
                            <asp:RequiredFieldValidator ID="rfvCustomActionCode" CssClass="error" runat="server" ControlToValidate="txtCustomActionCode" ErrorMessage="Please enter the custom action code."></asp:RequiredFieldValidator>
                            <asp:Label id="lblCustomActionCodeHeader" runat="server" AssociatedControlID="txtUpdateValue" CssClass="required">Custom Action Code</asp:Label>
<asp:TextBox ID="txtCustomActionCode" runat="server" TextMode="MultiLine" Rows="5" Columns="80"></asp:TextBox>
                                </asp:Panel>

                                </li>
                        </ItemTemplate>
                    </asp:Repeater>
                        </ol>
    </td>
    </tr>
    </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" HeaderText="Sort">
        <ContentTemplate>
                   <h1>Sort Parameters</h1>

    <br />
                <br />
                <label class="required">Default sort direction?</label>
                <br />
                <asp:RadioButtonList ID="rblDefaultSort" runat="server" Repeatdirection="Horizontal">
                    <asp:ListItem Value="Descending" Selected="true"></asp:ListItem>
                    <asp:ListItem Value="Ascending"></asp:ListItem>
                </asp:RadioButtonList>


                <br />
                <asp:RequiredFieldValidator ID="rfvSortColumns" runat="server" ControlToValidate="ddlSortColumns" ErrorMessage="Please select the number of sort columns." />
                <br />
                <label class="required">How many columns would you like to be included in the Sort list?</label>
                <br />
                <asp:DropDownList ID="ddlSortColumns" AutoPostBack="true" runat="server" />

                <ol>  
                <asp:Repeater ID="rptSortColumns" runat="server">
                    <ItemTemplate>
                        <li>
                            <br />
                            <asp:RequiredFieldValidator ID="rfvControlID" runat="server" ErrorMessage="Please select the column." ControlToValidate="ddlControlID" />
                            <asp:Label id="lblControlID" runat="server" AssociatedControlID="ddlControlID" CssClass="required">Column Name</asp:Label>
<asp:DropDownList ID="ddlControlID" runat="server"></asp:DropDownList>

                          <br />
                            <br />
                        <strong>Alternate Label for Sort List (defaults to Column Name)</strong> 
                        <Br />
                        <asp:TextBox ID="txtLabel" runat="server" cssclass="SlText" maxlength="100" Width="300"> </asp:TextBox>
              
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
                </ol>
    
    </table>
            
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" HeaderText="Filter">
        <ContentTemplate>
               
        <h1>Filter Parameters</h1>

        <br />
        <asp:RequiredFieldValidator ID="rfvFilterOptions" runat="server" ControlToValidate="ddlFilterOptions" ErrorMessage="Please indicate how many filter options you'd like to offer." />
        <asp:Label id="lblFilterOptions" runat="server" AssociatedControlID="ddlFilterOptions" CssClass="required">How many filter options would you like to offer?</asp:Label>
<asp:DropDownList ID="ddlFilterOptions" OnSelectedIndexChanged="ddlFilterOptions_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>

        <ol>
        <asp:Repeater ID="rptFilterOptions" runat="server">
            <ItemTemplate>
                <li>
                    <br />
                    <asp:RequiredFieldValidator ID="rfvSelectionType" runat="server" ControlToValidate="rblSelectionType" ErrorMessage="Please select the filter selection type."></asp:RequiredFieldValidator>
                    <br />
                    <strong>Selection Type</strong>
                    <br />
                    <asp:RadioButtonList ID="rblSelectionType" runat="server" RepeatDirection="horizontal">
                        <asp:ListItem Value="1">Single</asp:ListItem>
                        <asp:ListItem Value="2">Multiple</asp:ListItem>
                    </asp:RadioButtonList>
            
                                    <br />
                                 <asp:RequiredFieldValidator ID="rfvFilterLabel" runat="server" ControlToValidate="txtLabel" ErrorMessage="Please enter the label for this filter option." />
                            <br />
                        <label class="required">Label for This Filter Option (e.g. "Processed") </label>
                        <Br />
                        <asp:TextBox ID="txtLabel" runat="server" cssclass="SlText" maxlength="100" Width="300"> </asp:TextBox>

          <br />
                <asp:RequiredFieldValidator ID="rfvOptionColumns" runat="server" ControlToValidate="ddlOptionColumns" ErrorMessage="Please select the number of columns for this filter option." />
                <br />
                <label class="required">How many columns would you like to be included in this Filter option?</label>
                <br />
                <asp:DropDownList ID="ddlOptionColumns" OnSelectedIndexChanged="ddlOptionColumns_SelectedIndexChanged" AutoPostBack="true" runat="server" />

                <ul>  
                <asp:Repeater ID="rptOptionColumns" runat="server">
                    <ItemTemplate>
                        <li>
                            <br />
                            <asp:RequiredFieldValidator ID="rfvControlID" runat="server" ErrorMessage="Please select the column." ControlToValidate="ddlControlID" />
                            <asp:Label id="lblControlID" runat="server" AssociatedControlID="ddlControlID" CssClass="required">Column Name</asp:Label>
<asp:DropDownList ID="ddlControlID" runat="server"></asp:DropDownList>

                          <br />
                            <asp:RequiredFieldValidator ID="rfvOperatorType" runat="server" ErrorMessage="Please select the operator type." ControlToValidate="rblOperatorType" />
                            <br />
                        <label class="required">Operator Type</label>
                        <Br />
                        <asp:radiobuttonList ID="rblOperatorType" runat="server" RepeatLayout="UnorderedList" CssClass="unstyled-list-inline">
                        </asp:radiobuttonList>
              
                                   <asp:RequiredFieldValidator ID="rfvComparisonValue" runat="server" ControlToValidate="txtComparisonValue" ErrorMessage="Please enter the comparison value." />
                            <br />
                        <label class="required">Comparison Value</label>
                        <Br />
                        <asp:TextBox ID="txtComparisonValue" runat="server" cssclass="SlText" Width="300"> </asp:TextBox>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
                </ul>

                    </li>
                    </ItemTemplate>
        </asp:Repeater>
            </ol>
    </td>
    </tr>
    </table>
               
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" HeaderText="Export">
        <ContentTemplate>
                <h1>Export Parameters</h1>

                
                


    
        <asp:RequiredFieldValidator ID="rfvNumberExports" runat="server" ErrorMessage="Please select the number of exports." ControlToValidate="ddlNumberExports"></asp:RequiredFieldValidator>
        <asp:Label id="lblNumberExports" runat="server" AssociatedControlID="ddlNumberExports" CssClass="required">How many exports would you like?</asp:Label>
<asp:DropDownlist ID="ddlNumberExports" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlNumberExports_SelectedIndexChanged"></asp:DropDownlist>

        <ol>
                <asp:Repeater ID="rptExports" runat="server">
                    <ItemTemplate>
                        <li>
                            <br />
                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" ErrorMessage="Please enter a name for this export (e.g. &quot;Export 1&quot;)."></asp:RequiredFieldValidator>
                <asp:Label id="lblName" runat="server" AssociatedControlID="txtName" CssClass="required">Export Name</asp:Label>
<asp:TextBox ID="txtName" runat="server" Width="300" CssClass="SlText" MaxLength="100"></asp:TextBox>
                
                            <br />
                <asp:RequiredFieldValidator ID="rfvType" runat="server" ControlToValidate="rblDataSourceType" ErrorMessage="Please select the export type."></asp:RequiredFieldValidator>
                <br />
                <label class="required">Export Type</label>
                <br />
                <asp:RadioButtonlist ID="rblDataSourceType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblDataSourceType_SelectedIndexChanged">
                    <asp:ListItem Value="1">I will select table columns to export.</asp:ListItem>
                    <asp:ListItem Value="2">I will write a custom export SQL query.</asp:ListItem>
                </asp:RadioButtonlist>

                <br />
                <br />
                <label class="required">File Type</label>
                <br />
                <asp:RadioButtonlist ID="rblFileType" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                    <asp:ListItem Value="Excel" Selected="true">Excel</asp:ListItem>
                    <asp:ListItem Value="CSV">CSV</asp:ListItem>
                </asp:RadioButtonlist>

                            
                <uc:DataSource ID="ucDataSource" runat="server" />

                <asp:Panel ID="pnlStatement" runat="server" Visible="false">
                    <br />
                <asp:RequiredFieldValidator ID="rfvSQLQuery" runat="server" ControlToValidate="txtSQLQuery" ErrorMessage="Please enter the export SQL query."></asp:RequiredFieldValidator>
                <asp:Label id="lblSQLQuery" runat="server" AssociatedControlID="txtSQLQuery" CssClass="required">Export SQL Query</asp:Label>
<asp:TextBox ID="txtSQLQuery" runat="server" cssclass="MlText" TextMode="MultiLine" Rows="10" Columns="120" ></asp:TextBox>
                
                </asp:Panel>

                <asp:Panel ID="pnlColumns" runat="server" Visible="false">
                <asp:CustomValidator ID="cvMainColumns" runat="server" ErrorMessage="Temp" OnServerValidate="cvMainColumns_ServerValidate"></asp:CustomValidator>
                <Br />
Please select the data columns you would like to include in the export.
                <Br /><br />
                <strong><asp:Label ID="lblMainTableName" runat="server"></asp:Label><%# WhitTools.DataTables.GetDataTable("Select * From " & Common.General.Variables.DT_WEBRAD_PROJECTS & "  Where ID = " & Common.General.ProjectOperations.GetProjectID()).Rows(0).Item("SQLMainTableName")%></strong>
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
    
    </li>
                        </ItemTemplate>
                </asp:Repeater>
            </ol>
        </td>
    </tr>
    </table>
        
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" HeaderText="Report">
        <ContentTemplate>
                <h1>Report Parameters</h1>

                
                


    
        <asp:RequiredFieldValidator ID="rfvNumberReports" runat="server" ErrorMessage="Please select the number of reports." ControlToValidate="ddlNumberReports"></asp:RequiredFieldValidator>
        <asp:Label id="lblNumberReports" runat="server" AssociatedControlID="ddlNumberReports" CssClass="required">How many reports would you like?</asp:Label>
<asp:DropDownlist ID="ddlNumberReports" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlNumberExports_SelectedIndexChanged"></asp:DropDownlist>

        <ol>
                <asp:Repeater ID="rptReports" runat="server">
                    <ItemTemplate>
                        <li>
                            <br />
                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" ErrorMessage="Please enter a name for this Report (e.g. &quot;Report 1&quot;)."></asp:RequiredFieldValidator>
                <asp:Label id="lblName" runat="server" AssociatedControlID="txtName" CssClass="required">Report Name</asp:Label>
<asp:TextBox ID="txtName" runat="server" Width="300" CssClass="SlText" MaxLength="100"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="rfvType" runat="server" ControlToValidate="rblDataSourceType" ErrorMessage="Please select the Report type."></asp:RequiredFieldValidator>
                <br />
                <label class="required">Report Data Source</label>
                <br />
                <asp:RadioButtonlist ID="rblDataSourceType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblDataSourceType_SelectedIndexChanged">
                    <asp:ListItem Value="1">I will select table columns.</asp:ListItem>
                    <asp:ListItem Value="2">I will write a custom SQL query.</asp:ListItem>
                </asp:RadioButtonlist>

                <uc:DataSource ID="ucDataSource" runat="server" />

                <asp:Panel ID="pnlStatement" runat="server" Visible="false">
                    <br />
                <asp:RequiredFieldValidator ID="rfvSQLQuery" runat="server" ControlToValidate="txtSQLQuery" ErrorMessage="Please enter the Report SQL query."></asp:RequiredFieldValidator>
                <asp:Label id="lblSQLQuery" runat="server" AssociatedControlID="txtSQLQuery" CssClass="required">Report SQL Query</asp:Label>
<asp:TextBox ID="txtSQLQuery" runat="server" cssclass="MlText" TextMode="MultiLine" Rows="10" Columns="120" ></asp:TextBox>
                
                </asp:Panel>

                <asp:Panel ID="pnlColumns" runat="server" Visible="false">
                <asp:CustomValidator ID="cvMainColumns" runat="server" ErrorMessage="Temp" OnServerValidate="cvMainColumns_ServerValidate"></asp:CustomValidator>
                <Br />
Please select the data columns you would like to include in the Report.
                <Br /><br />
                <strong><asp:Label ID="lblMainTableName" runat="server"></asp:Label><%# WhitTools.DataTables.GetDataTable("Select * From " & Common.General.Variables.DT_WEBRAD_PROJECTS & "  Where ID = " & Common.General.ProjectOperations.GetProjectID()).Rows(0).Item("SQLMainTableName")%></strong>
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
        <strong>Message Type</strong>
        <br />
        <asp:RadioButtonList ID="rblTemplateMessageType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rblTemplateMessageType_SelectedIndexChanged">
            <asp:ListItem Value="Rich" Selected="true">Rich</asp:ListItem>
            <asp:ListItem Value="Plain">Plain</asp:ListItem>
        </asp:RadioButtonList>

                            <br /><br />
        <strong>Report Template</strong>
        <br />
        <asp:TextBox ID="txtTemplate"  runat="server" CssClass="MlText RichText" TextMode="MultiLine" Rows="5" columns="100"></asp:TextBox>
        <asp:TextBox ID="txtTemplatePlain"  runat="server" CssClass="MlText" TextMode="MultiLine" Rows="5" columns="100"></asp:TextBox>
        </asp:Panel>		
		



        <br />
        
    
    </li>
                        </ItemTemplate>
                </asp:Repeater>
            </ol>
        </td>
    </tr>
    </table>
            
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" HeaderText="Schedule page">
        <ContentTemplate>
                         <%--Bug here, the closed message isn't adequately recorded the first time through.  You have to save the page
                    then come back and save it again to get it to show the message correctly on the frontend.--%>
                
                    <br /><br />
                    Please enter the message you'd like to be shown on the closed page for this form.  If you enter nothing, the default message, "Sorry, the <strong><asp:label id="lblFormName" runat="server"></asp:label></strong> form is now closed.", will be displayed.
                    <br /><br />
                    <strong>Closed Message</strong>
                    <br />
                    <asp:TextBox ID="txtClosedMessage" runat="server" CssClass="MlText RichText" TextMode="MultiLine" rows="5" Columns="80"></asp:TextBox>


        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" HeaderText="Custom select statement">
        <ContentTemplate>
                    <Br /><br />
                    Please enter your custom select statement for the index page below:
                    
                    <uc:DataSource ID="ucCustomSelectDataSource" showType="false" showSpecific="true" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" HeaderText="Paging">
        <ContentTemplate>
                    <br />
                    <asp:CompareValidator ID="cmpPageLimit" runat="server" ControlTovalidate="txtPageLimit" Display="dynamic" ErrorMessage="Please enter the page limit using numbers only." Operator="DataTypeCheck" Type="Integer"></asp:CompareValidator>
                      <asp:RequiredFieldValidator ID="rfvPageLimit" Display="dynamic" runat="server" ControlToValidate="txtPageLimit" ErrorMessage="Please enter the page limit."></asp:RequiredFieldValidator>
                      <asp:RangeValidator ID="rvPageLimit" runat="server" ControlToValidate="txtPageLimit" MinimumValue="1" MaximumValue="1000" type="Integer"  errormessage="Please enter a page limit that is greater than 0 and less than 1000."></asp:RangeValidator>
                    <asp:Label id="lblPageLimit" runat="server" AssociatedControlID="txtPageLimit" CssClass="required">How many items should be listed on each page?</asp:Label>
<asp:TextBox ID="txtPageLimit" runat="server" Width="50" MaxLength="10" CssClass="SlText"></asp:TextBox>

        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" HeaderText="Ancillary maintenance">
        <ContentTemplate>
                    <asp:RequiredFieldValidator ID="rfvNumberAncillaryMaintenance" runat="server" ControlToValidate="ddlNumberAncillaryMaintenance" ErrorMessage="Please indicate how many ancillary maintenance pages you'd like."></asp:RequiredFieldValidator>
                    <asp:Label id="lblNumberAncillaryMaintenance" runat="server" AssociatedControlID="ddlNumberAncillaryMaintenance" CssClass="required">How many ancillary maintenance pages?</asp:Label>
<asp:DropDownList ID="ddlNumberAncillaryMaintenance" AutoPostBack="true" runat="server"></asp:DropDownList>

                    <asp:Repeater ID="rptAncillaryMaintenance" runat="server">
                        <ItemTemplate>
                            <br />
                            <asp:RequiredFieldValidator ID="rfvProject" runat="server" ControlToValidate="ddlProject" ErrorMessage="Please select the project for this ancillary maintenance."></asp:RequiredFieldValidator>
                    <asp:Label id="lblProject" runat="server" AssociatedControlID="ddlProject" CssClass="required">Project</asp:Label>
<asp:DropDownList ID="ddlProject" runat="server"></asp:DropDownList>

                            <br />
                            <asp:RequiredFieldValidator ID="rfvShortName" runat="server" ControlToValidate="txtShortName" ErrorMessage="Please enter the short name for this ancillary maintenance."></asp:RequiredFieldValidator>
                    <asp:Label id="lblShortName" runat="server" AssociatedControlID="txtShortName" CssClass="required">Short Name</asp:Label>
<asp:TextBox ID="txtShortName" runat="server" CssClass="SlText" Width="300" MaxLength="100"></asp:TextBox>
                            
                    <br />
                    <br />
                    <asp:checkbox id="chkSingleton" runat="server" text="Singleton" />
                            
                        </ItemTemplate>
                    </asp:Repeater>
      
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
     <ajaxToolkit:TabPanel runat="server" HeaderText="Additional links">
        <ContentTemplate>
                                        <br />
<asp:CustomValidator Display="Dynamic" CssClass="error" id="cvAdditionalLinks" Runat="server" ErrorMessage="Please enter/select the following: Additional Links." OnServerValidate="cvAdditionalLinks_ServerValidate"></asp:CustomValidator>
<br />
<h3>Additional Links</h3>
<br /><br />
<asp:Repeater ID="rptAdditionalLinks" Runat="server">
<ItemTemplate>
                <div class="stack-container">
                <div class="stack">
                    <asp:requiredfieldValidator CssClass="error" id="rfvName" Runat="server" ErrorMessage="Please enter/select the following: Link Name." ControlToValidate="txtName"></asp:RequiredFieldValidator>
                    <strong>Link Name</strong>
<asp:Textbox ID="txtName" Runat="server" TextMode="SingleLine" MaxLength="50" Width="300" CSSClass="SlText"></asp:Textbox></td>

                </div>
                <div class="stack">
<asp:requiredfieldValidator CssClass="error" id="rfvURL" Runat="server" ErrorMessage="Please enter/select the following: Link URL." ControlToValidate="txtURL"></asp:RequiredFieldValidator>
                    <strong>Link URL</strong>
                    
<asp:Textbox ID="txtURL" Runat="server" TextMode="SingleLine" MaxLength="0" Width="300" CSSClass="SlText"></asp:Textbox></td>

                </div>
                <div class="stack">
                    <asp:LinkButton ID="librptAdditionalLinksRemoveItem" runat="server" CssClass="icon-remove" data-grunticon-embed CausesValidation="false"  OnClick="librptAdditionalLinks_RemoveItem_Click">Remove</asp:LinkButton>
                </div> 
                    
            </div>
    <br /><br />
<asp:label id="lblID" runat="server" visible="false" text='<%# Container.DataItem("ID") %>' />
</tr>
</ItemTemplate>
</asp:Repeater>
            
<div>
<asp:Button ID="btnrptAdditionalLinksAddItem" runat="server" cssclass="button" CausesValidation="false" Text="Add additional link" OnClick="btnrptAdditionalLinksAddItem_Click"></asp:Button>
</div> 


        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
                
              </asp:Panel>  
                </asp:Panel>

            
        
		<br /><Br />
		<div style="text-align:center">
		<asp:Button ID="btnContinue" runat="server" Text="Continue" cssclass="button" />
		</div>
		<div style="text-align:center">
		    <asp:ValidationSummary ID="vsWebRAD" runat="server" />
		</div>
    <script>
        $('#BackendSectionTabs').tabs({
            hide: {
                effect: "slide",
                duration: 250
            },
            activate: function (event, ui) {
                $(event.currentTarget).blur();
         
            }
        });
    </script>
            </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>