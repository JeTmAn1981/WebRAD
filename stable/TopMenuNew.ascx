<%@ Control Language="vb" AutoEventWireup="false" CodeFile="TopMenuNew.ascx.vb" inherits="TopMenuNew" %>
<style>
    .TopMenuItem {
        float: left;
        /*background-image: url(~images/bluebar_bk.jpg), url(~images/arrow.png), url(~images/arrow.png);*/
        margin: 20px;
        height: 50px;
    }
</style>

<div class="TopMenuItem ">
                       <asp:LinkButton causesvalidation="false" ID="libMainDetails" runat="server" cssclass="button"  Text="Main Details"></asp:LinkButton>
</div>
<div class="TopMenuItem">
   <asp:LinkButton causesvalidation="false" ID="libControlDetails"  runat="server" cssclass="button" Text="Control Details"></asp:LinkButton>    
</div>
<div class="TopMenuItem">
   <asp:LinkButton causesvalidation="false" ID="libFrontendDetails"  runat="server" cssclass="button" Text="Frontend Details"></asp:LinkButton>    
</div>
<div class="TopMenuItem">
<asp:LinkButton causesvalidation="false" ID="libBackendDetails"  runat="server" cssclass="button" Text="Backend Details"></asp:LinkButton>    
</div>
<div class="TopMenuItem">
<asp:LinkButton causesvalidation="false" ID="libFinalize"  runat="server"  cssclass="button" Text="Finalize"></asp:LinkButton>    
</div>
<br style="clear:both;" />