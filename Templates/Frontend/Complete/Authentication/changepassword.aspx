<%@ Page Language="vb" ValidateRequest="false" AutoEventWireup="false" CodeFile="changepassword.aspx.vb"  MaintainScrollPositionOnPostBack="true" Inherits="changepassword" %>
<!DOCTYPE HTML>
<html class="no-js" lang="en" xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=edge" />
        <title><%= Common.APPLICATION_NAME %> - Change Password | <%= Common.DEPARTMENT_NAME %> | Whitworth University</title>
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-head.htm" -->
	</head>
    <body class="body">
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-body-01.htm" -->
        <%= Common.DEPARTMENT_NAME %>
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-body-02.htm" -->
    
    <div class="form-group">
		    <asp:panel id="formPanel" runat="Server">

        <asp:requiredfieldvalidator cssclass="error"  id="reqField1" ControlToValidate="txtoldPwd" ErrorMessage="Please enter your current password" Runat="server" />
        <asp:Label runat="server" AssociatedControlID="txtOldPwd" CssClass="required">Current Password</asp:Label>
        <asp:textbox id="txtoldPwd" Runat="server" TextMode="Password" Columns="45" Cssclass="SlText"></asp:textbox>
        
            <asp:requiredfieldvalidator cssclass="error"  id="reqField2" Display="Dynamic" ControlToValidate="txtnewPwd" ErrorMessage="Please enter a new password" Runat="server" />
        <asp:RegularExpressionValidator CssClass="error" id="regExValid1" Display="dynamic"  ControlToValidate="txtnewPwd" ErrorMessage="Password must be 4-20 characters with no spaces." Runat="server" ValidationExpression="\S{4,20}"></asp:RegularExpressionValidator>
        <asp:Label runat="server" AssociatedControlID="txtnewPwd" CssClass="required">New Password</asp:Label>
        <asp:textbox id="txtnewPwd" Runat="server" TextMode="Password" Columns="45" Cssclass="SlText"></asp:textbox>
        
        <asp:requiredfieldvalidator cssclass="error"  id="reqField3" Display="Dynamic" ControlToValidate="txtconfirmPwd" ErrorMessage="Please confirm your new password" Runat="server" />
        <asp:comparevalidator Display="dynamic" cssclass="error"  id="compValid1"  ControlToValidate="txtconfirmPwd" ErrorMessage="The new password and confirm new password values do not match." Runat="server" ControlToCompare="txtnewPwd"></asp:comparevalidator>
		<asp:Label runat="server" AssociatedControlID="txtConfirmPwd" CssClass="required">Confirm New Password</asp:Label>																		
        <asp:textbox id="txtconfirmPwd" Runat="server" TextMode="Password" Columns="45" Cssclass="SlText"></asp:textbox>
        
        <p>
            <asp:label id="lblLoginMessage" Runat="server" EnableViewState="False" />
            </p>
        <div style="TEXT-ALIGN: center">
        <asp:Linkbutton id="btnSubmit" runat="server" CssClass="button"><span class="glyphicon glyphicon-circle-arrow-right"></span> Submit</asp:Linkbutton>
            <asp:ValidationSummary ID="ValidationSummary" runat="server" />
            </div>
            
        </asp:panel>
        <asp:panel id="confirmPanel" runat="Server">
        <p>
        Password changed successfully! Click <A href="status.aspx">here</A> 
        to return to the application. 
            </p>
        </asp:panel>
        

    </div>


    				<!-- Form End -->
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-footer.htm" -->
		<script src="//www.whitworth.edu/js/webradapps.js"></script>
		<script type="text/javascript">
		
		
		
		
		</script>
		
    </body>
</html>

