<%@ Page Language="vb" AutoEventWireup="false" MaintainScrollPositionOnPostback="true" ValidateRequest="false" CodeBehind="backendold.aspx.vb" Inherits="stable.backend" %>
<%@ Register TagPrefix="uc" TagName="DataSource" Src="Common/DataSource.ascx" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<%--<%--<%@ Register TagPrefix="uc" TagName="TopMenu" Src="Common/TopMenu.ascx" %>--%>--%>

<!DOCTYPE html>
<html>
	<head>
			<title>Whitworth University Communications - Web-RAD</title>
	    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<link rel="stylesheet" href="/~CSS/default.css" type="text/css" />
<link rel="stylesheet" href="/~CSS/forms.css"   type="text/css" />
<link rel="stylesheet" href="//code.jquery.com/ui/1.10.4/themes/smoothness/jquery-ui.css">
<script src="http://code.jquery.com/jquery-latest.min.js" />
<script src="//code.jquery.com/ui/1.10.4/jquery-ui.js"></script>
<script type="text/javascript" src="/js/ckeditor/ckeditor.js"></script>
<script type="text/javascript" src="/js/web2.js" language="javascript"></script>
<link rel="shortcut icon" href="/favicon/web2-favicon.ico" />
        
        <%--<script type="text/javascript" src="/js/ckeditor/ckeditor.js"></script>--%>
          
        <%--<script>
            window.onload = function () {
                CKEDITOR.replace('txtClosedMessage');

            };
</script>--%>
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

        <script type="text/javascript" src="/js/tinymce/tinymce.min.js"></script>
        <script language="javascript" type="text/javascript" src="LoadRichText.js" />
 <script src="src/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="src/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    
  
	</head>
	
	<body>
	    <!-- Header START -->
<table align="center" class="BlueTable" width="1010">
    <tr>
	    <td valign="top">
		    <img height="97" alt="Whitworth" src="/~Images/Header/General/Logo.png" width="109" usemap="#Map1" border="0" />
		    <map id="Map1" name="Map1">
			    <area shape="circle" alt="Whitworth University Home Page" coords="62, 49, 44" href="http://www.whitworth.edu/index.aspx" />
		    </map>
	    </td>
	    <td width="850"><br />
		    <span class="DeptHeader">
	    University Communications
	                </span><br />
		    <a class="TopLinks" href="http://www.whitworth.edu/index.aspx" target="_blank">Whitworth Home Page</a><span class="SmTextWhite">&nbsp;&gt;</span><br />
	    </td>
    </tr>
</table>
<!-- Header END -->

<table width="1010" border="0" align="center" cellpadding="0" cellspacing="8" bgcolor="#FFFFFF">
    <tr>
	    <td width="70" /><!-- Left Margin -->
	    <td width="870">
	        <!-- Main Table START -->
		    <span class="SmText"><br /><a href="/index.aspx">Intranet Home</a>&nbsp;&gt;</span>
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

     <%--<%--<%--<uc:TopMenu ID="topMenu" runat="server" />         --%>         --%>--%>
              <br />
            <asp:RequiredFieldValidator ID="rfvIncludebackend" runat="server" ControlToValidate="rblIncludebackend" ErrorMessage="Please indicate whether or not you'd like to include a backend." />
            <br />
            <strong>Include backend?</strong> <span class="required">(required)</span>

            <br />
            <asp:RadioButtonList ID="rblIncludebackend" runat="server" RepeatDirection="Horizontal" autopostback="true">
                <asp:ListItem Value="0">No</asp:ListItem>
                <asp:ListItem Value="1" Selected="true">Yes</asp:ListItem>
            </asp:RadioButtonList>


            <asp:Panel ID="pnlBackend" runat="server">
                <asp:DropDownList ID="ddlTest" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTest_SelectedIndexChanged">
                    <asp:ListItem Value="2"></asp:ListItem>
                    <asp:ListItem Value="3"></asp:ListItem>
                    <asp:ListItem Value="4"></asp:ListItem>
                </asp:DropDownList>
<asp:Repeater ID="rptTest" runat="server">
    <ItemTemplate>
        <uc:DataSource ID="ucDataSource" runat="server" />

        <asp:Label ID="lblID" runat="server"></asp:Label>
    </ItemTemplate>
</asp:Repeater>


            <br />
                  <asp:CustomValidator ID="cvBackendPath" runat="server" ErrorMessage="Please select the path for the project backend." />
            <br />
            <strong>Backend Path</strong> <span class="required">(required)</span>
           
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
            <strong>Create new folder at this location?</strong> <span class="required">(required)</span>
            <br />
            <asp:RadioButtonList ID="rblCreateBackendFolder" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                <asp:ListItem Value="0">No</asp:ListItem>
                <asp:ListItem Value="1">Yes</asp:ListItem>
            </asp:RadioButtonList>

            <asp:Panel ID="pnlNewBackendFolder" runat="server" Visible="false">
                <asp:RequiredFieldValidator ID="rfvNewBackendFolderName" runat="server" ControlToValidate="txtNewBackendFolderName" ErrorMessage="Please enter the name of the new folder to be created for the backend." />
                <br />
                <strong>New Folder Name</strong> <span class="required">(required)</span>
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
                <asp:CheckBoxList ID="cblBackendOptions" Width="700" runat="server" AutoPostBack="true" RepeatDirection="Vertical" RepeatColumns="6" OnSelectedIndexChanged="cblBackendOptions_SelectedIndexChanged"></asp:CheckBoxList>

       
                <asp:Panel ID="pnlSearch" runat="server" Visible="false">
                    <h1>Search Parameters</h1>
<table width="100%" border="1" cellpadding="5" cellspacing="5"><tr><td>
                <br />
                <asp:RequiredFieldValidator ID="rfvSearchColumns" runat="server" ControlToValidate="ddlSearchColumns" ErrorMessage="Please indicate how many columns should be on the search page."></asp:RequiredFieldValidator>
                <br />
                <strong>How many columns should be on the search page?</strong> <span class="required">(required)</span>
                <br />
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
          
                    </td></tr></table>
                </asp:Panel>

                <asp:Panel ID="pnlActionsOtherThanDelete" runat="server" Visible="false">
                    <h1>Action Parameters</h1>
<table width="100%" border="1" cellpadding="5" cellspacing="5"><tr><td>
                    <asp:requiredfieldvalidator ID="rfvActions" runat="server" CssClass="error" ControlToValidate="ddlActions" ErrorMessage="Please indicate how many actions you would like."></asp:requiredfieldvalidator>
                    <br />
                    <strong>How many actions would you like?</strong> <span class="required">(required)</span>
                    <br />
                    <asp:DropDownList ID="ddlActions" AutoPostBack="true" OnSelectedIndexChanged="ddlActions_SelectedIndexChanged" runat="server"></asp:DropDownList>

                    <ol>
                    <asp:Repeater ID="rptActions" runat="server">
                        <ItemTemplate>
                            <li>
                            <asp:RequiredFieldValidator ID="rfvLabel" CssClass="error" runat="server" ControlToValidate="txtLabel" ErrorMessage="Please enter the action label."></asp:RequiredFieldValidator>
                            <br />
                            <strong>Action Label</strong> <span class="required">(required)</span>
                            <br />
                            <asp:TextBox ID="txtLabel" runat="server" csclass="SlText" Width="300" MaxLength="500"></asp:TextBox>

                                <br />
                            <asp:RequiredFieldValidator ID="rfvControlName" CssClass="error" runat="server" ControlToValidate="ddlControlID" ErrorMessage="Please select the control for this action."></asp:RequiredFieldValidator>
                            <br />
                            <strong>Control Name</strong> <span class="required">(required)</span>
                            <br />
                            <asp:DropDownList ID="ddlControlID" runat="server"></asp:DropDownList>

                            <br />
                            <asp:RequiredFieldValidator ID="rfvActionType" CssClass="error" runat="server" ControlToValidate="ddlActionType" ErrorMessage="Please select the action type."></asp:RequiredFieldValidator>
                            <br />
                            <strong>Action Type</strong> <span class="required">(required)</span>
                            <br />
                            <asp:DropDownList ID="ddlActionType" AutoPostBack="true" OnSelectedIndexChanged="ddlActionType_SelectedIndexChanged" runat="server"></asp:DropDownList>

                            <asp:Panel ID="pnlUpdateValue" runat="server" Visible="false">
                                
                            <asp:RequiredFieldValidator ID="rfvUpdateValue" CssClass="error" runat="server" ControlToValidate="txtUpdateValue" ErrorMessage="Please enter the update value."></asp:RequiredFieldValidator>
                            <br />
                            <strong>Update Value</strong> <span class="required">(required)</span>
                            <br />
                            <asp:TextBox ID="txtUpdateValue" runat="server" csclass="SlText" Width="100" MaxLength="50"></asp:TextBox>
                                </asp:Panel>

                                </li>
                        </ItemTemplate>
                    </asp:Repeater>
                        </ol>
    </td>
    </tr>
    </table>
                </asp:Panel>


            <asp:Panel ID="pnlSort" runat="server" Visible="false">
                <h1>Sort Parameters</h1>
<table width="100%" border="1" cellpadding="5" cellspacing="5"><tr><td>
    <br />
                <br />
                <strong>Default sort direction?</strong> <span class="required">(required)</span>
                <br />
                <asp:RadioButtonList ID="rblDefaultSort" runat="server" Repeatdirection="Horizontal">
                    <asp:ListItem Value="Descending" Selected="true"></asp:ListItem>
                    <asp:ListItem Value="Ascending"></asp:ListItem>
                </asp:RadioButtonList>


                <br />
                <asp:RequiredFieldValidator ID="rfvSortColumns" runat="server" ControlToValidate="ddlSortColumns" ErrorMessage="Please select the number of sort columns." />
                <br />
                <strong>How many columns would you like to be included in the Sort list?</strong> <span class="required">(required)</span>
                <br />
                <asp:DropDownList ID="ddlSortColumns" AutoPostBack="true" runat="server" />

                <ol>  
                <asp:Repeater ID="rptSortColumns" runat="server">
                    <ItemTemplate>
                        <li>
                            <br />
                            <asp:RequiredFieldValidator ID="rfvControlID" runat="server" ErrorMessage="Please select the column." ControlToValidate="ddlControlID" />
                            <br />
                        <strong>Column Name</strong> <span class="required">(required)</span>
                        <Br />
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
    </td></tr>
    </table>
            </asp:Panel>
  
    <asp:Panel ID="pnlFilter" runat="server" Visible="false">
        <h1>Filter Parameters</h1>
<table width="100%" border="1" cellpadding="5" cellspacing="5"><tr><td>
        <br />
        <asp:RequiredFieldValidator ID="rfvFilterOptions" runat="server" ControlToValidate="ddlFilterOptions" ErrorMessage="Please indicate how many filter options you'd like to offer." />
        <br />
        <strong>How many filter options would you like to offer?</strong> <span class="required">(required)</span>
        <br />
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
                        <strong>Label for This Filter Option (e.g. "Processed") </strong> <span class="required">(required)</span>
                        <Br />
                        <asp:TextBox ID="txtLabel" runat="server" cssclass="SlText" maxlength="100" Width="300"> </asp:TextBox>

          <br />
                <asp:RequiredFieldValidator ID="rfvOptionColumns" runat="server" ControlToValidate="ddlOptionColumns" ErrorMessage="Please select the number of columns for this filter option." />
                <br />
                <strong>How many columns would you like to be included in this Filter option?</strong> <span class="required">(required)</span>
                <br />
                <asp:DropDownList ID="ddlOptionColumns" OnSelectedIndexChanged="ddlOptionColumns_SelectedIndexChanged" AutoPostBack="true" runat="server" />

                <ul>  
                <asp:Repeater ID="rptOptionColumns" runat="server">
                    <ItemTemplate>
                        <li>
                            <br />
                            <asp:RequiredFieldValidator ID="rfvControlID" runat="server" ErrorMessage="Please select the column." ControlToValidate="ddlControlID" />
                            <br />
                        <strong>Column Name</strong> <span class="required">(required)</span>
                        <Br />
                        <asp:DropDownList ID="ddlControlID" runat="server"></asp:DropDownList>

                          <br />
                            <asp:RequiredFieldValidator ID="rfvOperatorType" runat="server" ErrorMessage="Please select the operator type." ControlToValidate="rblOperatorType" />
                            <br />
                        <strong>Operator Type</strong> <span class="required">(required)</span>
                        <Br />
                        <asp:radiobuttonList ID="rblOperatorType" runat="server" RepeatDirection="Horizontal">
                        </asp:radiobuttonList>
              
                                   <asp:RequiredFieldValidator ID="rfvComparisonValue" runat="server" ControlToValidate="txtComparisonValue" ErrorMessage="Please enter the comparison value." />
                            <br />
                        <strong>Comparison Value</strong> <span class="required">(required)</span>
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
                        </asp:Panel>


            <asp:Panel ID="pnlExport" runat="server" Visible="false">
                <h1>Export Parameters</h1>

                
                

<table width="100%" border="1" cellpadding="5" cellspacing="5">
    <tr><td>
        <asp:RequiredFieldValidator ID="rfvNumberExports" runat="server" ErrorMessage="Please select the number of exports." ControlToValidate="ddlNumberExports"></asp:RequiredFieldValidator>
        <br />
                <strong>How many exports would you like?</strong> <span class="required">(required)</span>
                <br />
        <asp:DropDownlist ID="ddlNumberExports" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlNumberExports_SelectedIndexChanged"></asp:DropDownlist>

        <ol>
                <asp:Repeater ID="rptExports" runat="server">
                    <ItemTemplate>
                        <li>
                            <br />
                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" ErrorMessage="Please enter a name for this export (e.g. &quot;Export 1&quot;)."></asp:RequiredFieldValidator>
                <br />
                <strong>Export Name</strong> <span class="required">(required)</span>
                <br />
                <asp:TextBox ID="txtName" runat="server" Width="300" CssClass="SlText" MaxLength="100"></asp:TextBox>
                
                            <br />
                <asp:RequiredFieldValidator ID="rfvType" runat="server" ControlToValidate="rblDataSourceType" ErrorMessage="Please select the export type."></asp:RequiredFieldValidator>
                <br />
                <strong>Export Type</strong> <span class="required">(required)</span>
                <br />
                <asp:RadioButtonlist ID="rblDataSourceType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblDataSourceType_SelectedIndexChanged">
                    <asp:ListItem Value="1">I will select table columns to export.</asp:ListItem>
                    <asp:ListItem Value="2">I will write a custom export SQL query.</asp:ListItem>
                </asp:RadioButtonlist>

                <br />
                <br />
                <strong>File Type</strong> <span class="required">(required)</span>
                <br />
                <asp:RadioButtonlist ID="rblFileType" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                    <asp:ListItem Value="Excel" Selected="true">Excel</asp:ListItem>
                    <asp:ListItem Value="CSV">CSV</asp:ListItem>
                </asp:RadioButtonlist>

                            
                <uc:DataSource ID="ucDataSource" runat="server" />

                <asp:Panel ID="pnlStatement" runat="server" Visible="false">
                    <br />
                <asp:RequiredFieldValidator ID="rfvSQLQuery" runat="server" ControlToValidate="txtSQLQuery" ErrorMessage="Please enter the export SQL query."></asp:RequiredFieldValidator>
                <br />
                <strong>Export SQL Query</strong> <span class="required">(required)</span>
                <br />
                    <asp:TextBox ID="txtSQLQuery" runat="server" cssclass="MlText" TextMode="MultiLine" Rows="10" Columns="120" ></asp:TextBox>
                
                </asp:Panel>

                <asp:Panel ID="pnlColumns" runat="server" Visible="false">
                <asp:CustomValidator ID="cvMainColumns" runat="server" ErrorMessage="Temp" OnServerValidate="cvMainColumns_ServerValidate"></asp:CustomValidator>
                <Br />
Please select the data columns you would like to include in the export.
                <Br /><br />
                <strong><asp:Label ID="lblMainTableName" runat="server"></asp:Label>
                    <%# WhitTools.DataTables.GetDataTable("Select * From " & Common.General.variables.DT_WEBRAD_PROJECTS & "  Where ID = " & common.General.ProjectOperations.GetProjectID()).Rows(0).Item("SQLMainTableName")%></strong>
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
                </asp:Panel>
        
                
            <asp:Panel ID="pnlReport" runat="server" Visible="false">
                <h1>Report Parameters</h1>

                
                

<table width="100%" border="1" cellpadding="5" cellspacing="5">
    <tr><td>
        <asp:RequiredFieldValidator ID="rfvNumberReports" runat="server" ErrorMessage="Please select the number of reports." ControlToValidate="ddlNumberReports"></asp:RequiredFieldValidator>
        <br />
                <strong>How many reports would you like?</strong> <span class="required">(required)</span>
                <br />
        <asp:DropDownlist ID="ddlNumberReports" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlNumberExports_SelectedIndexChanged"></asp:DropDownlist>

        <ol>
                <asp:Repeater ID="rptReports" runat="server">
                    <ItemTemplate>
                        <li>
                            <br />
                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" ErrorMessage="Please enter a name for this Report (e.g. &quot;Report 1&quot;)."></asp:RequiredFieldValidator>
                <br />
                <strong>Report Name</strong> <span class="required">(required)</span>
                <br />
                <asp:TextBox ID="txtName" runat="server" Width="300" CssClass="SlText" MaxLength="100"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="rfvType" runat="server" ControlToValidate="rblDataSourceType" ErrorMessage="Please select the Report type."></asp:RequiredFieldValidator>
                <br />
                <strong>Report Data Source</strong> <span class="required">(required)</span>
                <br />
                <asp:RadioButtonlist ID="rblDataSourceType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblDataSourceType_SelectedIndexChanged">
                    <asp:ListItem Value="1">I will select table columns.</asp:ListItem>
                    <asp:ListItem Value="2">I will write a custom SQL query.</asp:ListItem>
                </asp:RadioButtonlist>

                <uc:DataSource ID="ucDataSource" runat="server" />

                <asp:Panel ID="pnlStatement" runat="server" Visible="false">
                    <br />
                <asp:RequiredFieldValidator ID="rfvSQLQuery" runat="server" ControlToValidate="txtSQLQuery" ErrorMessage="Please enter the Report SQL query."></asp:RequiredFieldValidator>
                <br />
                <strong>Report SQL Query</strong> <span class="required">(required)</span>
                <br />
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
        <strong>Report Template</strong>
        <br />
        <asp:TextBox ID="txtTemplate"  runat="server" CssClass="MlText RichText" TextMode="MultiLine" Rows="5" columns="100"></asp:TextBox>
        
    
    </li>
                        </ItemTemplate>
                </asp:Repeater>
            </ol>
        </td>
    </tr>
    </table>
                </asp:Panel>
        

                <%--Bug here, the closed message isn't adequately recorded the first time through.  You have to save the page
                    then come back and save it again to get it to show the message correctly on the frontend.--%>
                <asp:Panel ID="pnlSchedulePage" runat="server" Visible="false">

                    <br /><br />
                    Please enter the message you'd like to be shown on the closed page for this form.  If you enter nothing, the default message, "Sorry, the <strong><asp:label id="lblFormName" runat="server"></asp:label></strong> form is now closed.", will be displayed.
                    <br /><br />
                    <strong>Closed Message</strong>
                    <br />
                    <asp:TextBox ID="txtClosedMessage" runat="server" CssClass="MlText RichText" TextMode="MultiLine" rows="5" Columns="80"></asp:TextBox>

                </asp:Panel>


                <asp:Panel ID="pnlCustomselectstatement" runat="server" Visible="false">
                    <Br /><br />
                    Please enter your custom select statement for the index page below:
                    <Br />
                    <asp:RequiredFieldValidator ID="rfvCustomSelectStatement" runat="server" ControlToValidate="txtcustomSelectStatement" ErrorMessage="Please enter your custom select statement."></asp:RequiredFieldValidator>
                    <br />
                    <strong>Custom Select Statement</strong> <span class="required">(required)</span>
                    <br />
                    <asp:TextBox ID="txtCustomSelectStatement" runat="server" CssClass="MlText" TextMode="MultiLine" rows="5" Columns="80"></asp:TextBox>
                </asp:Panel>

                  <asp:Panel ID="pnlPaging" runat="server" Visible="false">
                    <br />
                    <asp:CompareValidator ID="cmpPageLimit" runat="server" ControlTovalidate="txtPageLimit" Display="dynamic" ErrorMessage="Please enter the page limit using numbers only." Operator="DataTypeCheck" Type="Integer"></asp:CompareValidator>
                      <asp:RequiredFieldValidator ID="rfvPageLimit" Display="dynamic" runat="server" ControlToValidate="txtPageLimit" ErrorMessage="Please enter the page limit."></asp:RequiredFieldValidator>
                      <asp:RangeValidator ID="rvPageLimit" runat="server" ControlToValidate="txtPageLimit" MinimumValue="1" MaximumValue="1000" type="Integer"  errormessage="Please enter a page limit that is greater than 0 and less than 1000."></asp:RangeValidator>
                    <br />
                    <strong>How many items should be listed on each page?</strong> <span class="required">(required)</span>
                    <br />
                    <asp:TextBox ID="txtPageLimit" runat="server" Width="50" MaxLength="10" CssClass="SlText"></asp:TextBox>

                </asp:Panel>

                <asp:Panel ID="pnlAncillarymaintenance" runat="server" Visible="false">
                    <asp:RequiredFieldValidator ID="rfvNumberAncillaryMaintenance" runat="server" ControlToValidate="ddlNumberAncillaryMaintenance" ErrorMessage="Please indicate how many ancillary maintenance pages you'd like."></asp:RequiredFieldValidator>
                    <br />
                    <strong>How many ancillary maintenance pages?</strong> <span class="required">(required)</span>
                    <br />
                    <asp:DropDownList ID="ddlNumberAncillaryMaintenance" AutoPostBack="true" runat="server"></asp:DropDownList>

                    <asp:Repeater ID="rptAncillaryMaintenance" runat="server">
                        <ItemTemplate>
                            <br />
                            <asp:RequiredFieldValidator ID="rfvProject" runat="server" ControlToValidate="ddlProject" ErrorMessage="Please select the project for this ancillary maintenance."></asp:RequiredFieldValidator>
                    <br />
                    <strong>Project</strong> <span class="required">(required)</span>
                    <br />
                    <asp:DropDownList ID="ddlProject" runat="server"></asp:DropDownList>

                            <br />
                            <asp:RequiredFieldValidator ID="rfvShortName" runat="server" ControlToValidate="txtShortName" ErrorMessage="Please enter the short name for this ancillary maintenance."></asp:RequiredFieldValidator>
                    <br />
                    <strong>Short Name</strong> <span class="required">(required)</span>
                    <br />
                    <asp:TextBox ID="txtShortName" runat="server" CssClass="SlText" Width="300" MaxLength="100"></asp:TextBox>
                            
                    <br />
                    <br />
                    <asp:checkbox id="chkSingleton" runat="server" text="Singleton" />
                            
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:Panel>


                <asp:Panel ID="pnlAdditionalLinks" runat="server" Visible="false">
                                        <br />
<asp:CustomValidator Display="Dynamic" CssClass="error" id="cvAdditionalLinks" Runat="server" ErrorMessage="Please enter/select the following: Additional Links." OnServerValidate="cvAdditionalLinks_ServerValidate"></asp:CustomValidator>
<br />
<h3>Additional Links</h3>
<br /><br />
<table width='800'>

<tr>

<td><strong>Link Name</strong> <span class="required">(required)</span></td>

<td><strong>Link URL</strong> <span class="required">(required)</span></td>

</tr>
<asp:Repeater ID="rptAdditionalLinks" Runat="server">
<ItemTemplate>
<tr>
<td>
<br />
<asp:requiredfieldValidator Display="Dynamic" CssClass="error" id="rfvName" Runat="server" ErrorMessage="Please enter/select the following: Link Name." ControlToValidate="txtName"></asp:RequiredFieldValidator>
<asp:Textbox ID="txtName" Runat="server" TextMode="SingleLine" MaxLength="50" Width="300" CSSClass="SlText"></asp:Textbox></td>


<td>
<br />
<asp:requiredfieldValidator Display="Dynamic" CssClass="error" id="rfvURL" Runat="server" ErrorMessage="Please enter/select the following: Link URL." ControlToValidate="txtURL"></asp:RequiredFieldValidator>
<asp:Textbox ID="txtURL" Runat="server" TextMode="SingleLine" MaxLength="0" Width="300" CSSClass="SlText"></asp:Textbox></td>


<td>
<div>
<asp:LinkButton ID="librptAdditionalLinksRemoveItem" runat="server" CssClass="icon-remove" data-grunticon-embed CausesValidation="false"  OnClick="librptAdditionalLinks_RemoveItem_Click">Remove</asp:LinkButton>
</div> 
</td>
</tr>
<asp:label id="lblID" runat="server" visible="false" text='<%# Container.DataItem("ID") %>' />
</tr>
</ItemTemplate>
</asp:Repeater><tr data-tablesaw-no-labels>
<td colspan="3">
<div>
<asp:Button ID="btnrptAdditionalLinksAddItem" runat="server" CssClass="button" CausesValidation="false" Text="Add additional link" OnClick="btnrptAdditionalLinksAddItem_Click"></asp:Button>
</div> 

</td>
</tr>

</table>


                </asp:Panel>

                </asp:Panel>

            
        
		<br /><Br />
		<div style="text-align:center">
		<asp:Button ID="btnContinue" runat="server" Text="Continue" CssClass="Button" />
		</div>
		<div style="text-align:center">
		    <asp:ValidationSummary ID="vsWebRAD" runat="server" />
		</div>
		</dl>
		<!-- Page Main END -->
	                        </form>
				</div>
		    <!-- Main Table END -->
	    </td>
	    <td width="70" /> <!-- Right Margin -->
    </tr>
</table>
<!-- Footer START -->
<table width="1010" border="0" align="center" cellpadding="0" cellspacing="4" bordercolor="#FFFFFF" bgcolor="#FFFFFF">
	<tr><td height="20" bgcolor="#3A5773" /></tr>
</table>
<!-- Footer END -->
<script type="text/javascript">
    ShowRepeaterHeader('AdditionalLinks');

		</script>

    </body>
</html>

