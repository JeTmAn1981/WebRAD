<%@ Page Language="vb" ValidateRequest="false" AutoEventWireup="false" CodeFile="newuser.aspx.vb"  MaintainScrollPositionOnPostBack="true" Inherits="newuser" %>
<!DOCTYPE HTML>
<html class="no-js" lang="en" xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=edge" />
        <title><%= Common.APPLICATION_NAME %> - New User | <%= Common.DEPARTMENT_NAME %> | Whitworth University</title>
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-head.htm" -->
	</head>
    <body class="body">
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-body-01.htm" -->
        <%= Common.DEPARTMENT_NAME %>
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-body-02.htm" -->
    
    <div class="form-group">
<asp:panel id="pnlUser" runat="Server" DefaultButton="btnSubmit">
            <p>Please enter your email address below and a temporary password will be emailed to you.</p>

            <asp:requiredfieldvalidator cssclass="error"  id="reqField1" Display="Dynamic" ErrorMessage="Please enter an email address." ControlToValidate="txtEmail" Runat="server" />
            <asp:RegularExpressionValidator id="revEmail" ErrorMessage="Please enter the email address in the form of me@you.com" ControlToValidate="txtEmail" Runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
            <asp:label runat="server" AssociatedControlID="txtEmail" CssClass="required">Email</asp:label>
            <asp:textbox id="txtEmail" Runat="server" />

            <asp:RequiredFieldValidator ID="rfvConfirmEmail" cssclass="error" runat="server" Display="Dynamic" ControlToValidate="txtConfirmEmail" ErrorMessage="Please confirm your email address."></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="cmpConfirmEmail" cssclass="error" runat="server" Operator="Equal" ControlToValidate="txtConfirmEmail" ControlToCompare="txtEmail"  ErrorMessage="Please enter the same email address you entered above."></asp:CompareValidator>
            <asp:label runat="server" AssociatedControlID="txtConfirmEmail" CssClass="required">Confirm Email</asp:label>
            <asp:textbox id="txtConfirmEmail" Runat="server" />

		    <asp:label id="lblLoginMessage" Runat="server" EnableViewState="False" />
            
            <div style="text-align:center;">
                <asp:Linkbutton id="btnSubmit" runat="server" CssClass="button"><span class="glyphicon glyphicon-circle-arrow-right"></span> Submit</asp:Linkbutton>
                <asp:ValidationSummary ID="vsNewUser" runat="server" />
            </div>
        </asp:panel>
        <asp:panel id="pnlConfirm" runat="Server">
            <p>
                Your password has been sent to <asp:Label id="lblEmail" runat="Server" />.   
                If this address is not correct or if you do not receive the password containing 
                your email within 24 hours, please contact the Whitworth web team at
                <a href='mailto:<%= WhitTools.GlobalEnum.EMAIL_WEB_TEAM %>'><%= WhitTools.GlobalEnum.EMAIL_WEB_TEAM %></a>. 
                
                <p class="bordered">
                Please add <a href='mailto:<%= WhitTools.GlobalEnum.EMAIL_WEB_TEAM %>'><%= WhitTools.GlobalEnum.EMAIL_WEB_TEAM %></a> to your email whitelist to make sure your password 
                email is not accidentally flagged as spam.
                    </p>
            </p>
            <p>Click the following link to return to the login page: <a href="login.aspx">Log In</a></p>
            <asp:Label ID="lblNewPassword" runat="server" />
        </asp:panel>    </div>

    				<!-- Form End -->
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-footer.htm" -->
		<script src="//www.whitworth.edu/js/webradapps.js"></script>
		<script type="text/javascript">
		
		
		
		
		</script>
		
    </body>
</html>

