<%@ Control Language="vb" AutoEventWireup="false" CodeFile="TopMenu.ascx.vb" inherits="TopMenu" %>
<h1>Web-RAD - <asp:DropDownList ID="ddlProjects" AutoPostBack="true" OnSelectedIndexChanged="ddlProjects_SelectedIndexChanged" runat="server"></asp:DropDownList></h1>
<table style="BACKGROUND-IMAGE: url(~images/bluebar_bk.jpg)" border="0" cellSpacing="0" cellPadding="0" width="100%">
			<tr>
				<td height="40" width="176" align="middle">
                         <asp:LinkButton causesvalidation="false" ID="libMainDetails" runat="server" CssClass="menuWhite" Text="Main Details"></asp:LinkButton>
                 </td>
                <td width="31"><IMG src="~images/arrow.png" width="31" height="42"></td>
                <td width="176" align="middle">           <asp:LinkButton causesvalidation="false" ID="libControlDetails"  runat="server" CssClass="menuWhite" Text="Control Details"></asp:LinkButton></td>
                <td width="31"><IMG src="~images/arrow.png" width="31" height="42"></td>
				
				<td width="176" align="middle">           <asp:LinkButton causesvalidation="false" ID="libFrontendDetails"  runat="server" CssClass="menuWhite" Text="Frontend Details"></asp:LinkButton></td>
				<td width="31"><IMG src="~images/arrow.png" width="31" height="42"></td>
				<td width="176" align="middle">           <asp:LinkButton causesvalidation="false" ID="libBackendDetails"  runat="server" CssClass="menuWhite" Text="Backend Details"></asp:LinkButton></td>
				<td width="31"><IMG src="~images/arrow.png" width="31" height="42"></td>
				<td width="176" align="middle"><asp:LinkButton causesvalidation="false" ID="libFinalize"  runat="server"  CssClass="menuWhite" Text="Finalize"></asp:LinkButton></td>
				<td bgColor="#ffffff" width="137"><IMG src="~images/arrowlong.jpg" width="89" height="42"></td>
			</tr>
		</table>
