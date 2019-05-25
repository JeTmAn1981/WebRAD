<%@ Page Language="vb" ValidateRequest="false" AutoEventWireup="false" CodeFile="message.aspx.vb"  MaintainScrollPositionOnPostBack="true" Inherits="message" %>
<!DOCTYPE HTML>
<html class="no-js" lang="en" xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=edge" />
        <meta name="robots" content="noindex">
        <title><%= Common.APPLICATION_NAME %> | <%= Common.DEPARTMENT_NAME %> | Whitworth University</title>
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-head.htm" -->
		
		
    </head>
    <body class="body">
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-body-01.htm" -->
        <%= Common.DEPARTMENT_NAME %>
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-body-02.htm" -->
	    <!-- Form Start -->
		<!-- Page Main START -->
		<div>
        <div ID="DefaultMessage" style="display:none;">
		<p class='bordered'>
        Sorry, a problem with the application was encountered.
        </p>
        <p>
            For assistance with this error, please contact the Whitworth web team by emailing <a href='mailto:tryan@whitworth.edu'>tryan@whitworth.edu</a> or calling 509.777.4695.
        </p>

		</div>

        <div id="Message<%= WhitTools.messages.MessageCode.AlreadySubmitted %>" style="display:none;">
            <p>
            Sorry, our records show you have already submitted this application once and may not submit it again.
        </p>
        </div>

                <div id="Message<%= WhitTools.messages.MessageCode.Closed %>" style="display:none;">
            <p>
                <%= Common.closedMessage %>
            </p>
        </div>

                <div id="Message<%= WhitTools.messages.MessageCode.NotFinished %>" style="display:none;">
            <p>According to our records you have not completed all sections of the <%= Common.APPLICATION_NAME %> form. Please <a href="status.aspx">click here</a> to return to the application menu and review your status.</p>
        </div>

                <div id="Message<%= WhitTools.messages.MessageCode.SessionExpired %>" style="display:none;">
    <p>
    You have taken more than 24 hours To fill out the <%= Common.APPLICATION_NAME %> form. The current session state 
            has expired, so your previously entered information Is no longer available. Please 
            <a href = "login.aspx" > click here</a> 
            to log in And resume filling out the application.
        </p>
        </div>
    <div id="Message<%= WhitTools.messages.MessageCode.ApplicationComplete %>" style="display:none;">
            <%= Common.CONFIRMATION_MESSAGE %>
        </div>
        <script>
            var currentMessage = document.getElementById('Message<%= Session("CurrentMessageCode") %>');
            console.log('<%= Session("CurrentMessageCode") %>');
            if (currentMessage)
                currentMessage.style.display = 'block';
            else
                document.getElementById('DefaultMessage').style.display = 'block';
        </script>
	</div>	


				<!-- Form End -->
        <!-- #include file="\\web1\~whitworth\forms\forms-templates\template-footer.htm" -->
		<script src="//www.whitworth.edu/js/webradapps.js"></script>
		<script type="text/javascript">
		
		
		
		
		</script>
		
    </body>
</html>
