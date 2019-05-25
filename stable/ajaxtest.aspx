<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ajaxtest.aspx.vb" Inherits="stable.ajaxtest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="smMain" runat="server" EnablePageMethods="true" >
            <Services>
                    <asp:ServiceReference path="WebService.asmx" />
                </Services>
	    </asp:ScriptManager>
    
        <div>
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
        </div>
    </form>
</body>
</html>
