<%@ Page Language="vb" ValidateRequest="false" AutoEventWireup="false" CodeFile="login.aspx.vb"  MaintainScrollPositionOnPostBack="true" Inherits="login" %>
<!DOCTYPE HTML>
<html class="no-js" lang="en" xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=edge" />
        <title><%= Common.APPLICATION_NAME %> - Login | <%= Common.DEPARTMENT_NAME %> | Whitworth University</title>
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-head.htm" -->
	</head>
    <body class="body">
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-body-01.htm" -->
        <%= Common.DEPARTMENT_NAME %>
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-body-02.htm" -->
    <div class="form-group">
        
        <p>
            If you are a 
            <strong>new user</strong> to this application, please 
            <a href='newuser.aspx'>click here</a>. You will be able to set up a user account 
            with a primary email address and password. 
        </p>
        <p>
            If you are <strong>returning</strong> to your application, please log in below with your primary 
            email address. If you have forgotten your password, please <a href="newuser.aspx">click here</a>.
        </p>
        
        <asp:requiredfieldvalidator cssclass="error"  Runat="server" ID="reqField1" Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="Please enter an email address." />
        <asp:Label ID="lblEmail" AssociatedControlID="txtEmail" runat="server" CssClass="required">Email</asp:Label>
        <asp:TextBox Runat="server"  ID="txtEmail" />

        <!-- Username -->
        <asp:requiredfieldvalidator cssclass="error"  Runat="server" ID="reqField2" Display="Dynamic" ControlToValidate="txtPassword" ErrorMessage="Please enter a password." />
        
        <asp:Label id="lblPassword" runat="server" AssociatedControlID="txtPassword" CssClass="required">Password</asp:Label>
        <asp:TextBox  TextMode="Password" Runat="server" ID="txtPassword" />
        <asp:Label ID="lblLoginMessage" Runat="server" EnableViewState="False" CssClass="error" />
        <!-- Buttons -->
</div>  
    
<div style="TEXT-ALIGN:center" class="form-group">
                      <asp:linkbutton id="btnLogin" CssClass="button" causesvalidation="false" runat="server"><span class="glyphicon glyphicon-circle-arrow-right"></span> Login</asp:linkbutton>
              
                      <asp:linkbutton id="btnNewUser" CssClass="button" causesvalidation="false" runat="server"><span class="glyphicon glyphicon-user"></span> New User</asp:linkbutton>
              
                      <asp:linkbutton id="btnForgotPassword" CssClass="button" causesvalidation="false" runat="server"><span class="glyphicon glyphicon-question-sign"></span> Forgot Password</asp:linkbutton>
              
        </div>
    				<!-- Form End -->
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-footer.htm" -->
		<script src="//www.whitworth.edu/js/webradapps.js"></script>
		<script type="text/javascript">
		
		
		
		
		</script>
		
    </body>
</html>

