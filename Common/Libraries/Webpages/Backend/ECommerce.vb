Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.Controls
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.ProjectOperations
Imports Common.General.DataTypes
Imports Common.General.Folders
Imports Common.General.DataSources
Imports Common.General.Columns
Imports Common.General.Assembly
Imports Common.SQL.StoredProcedure
Imports Common.Webpages.Frontend.MultiPage
Imports Common.SQL.Main
Imports Common.SQL.Table
Imports Common.Webpages.Frontend.Email
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Export
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.BindData
Imports Common.Webpages.ControlContent.Heading
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File


Imports WhitTools.Utilities
Namespace Webpages.Backend

    Public Class ECommerce

        Shared Function GetUpdateEcommerceContent() As String
            Dim sEcommerceContent As String = ""

            If IsEcommerceProject() Then
                sEcommerceContent &= GetHeadingStyle() & "Total Owed:" & GetHeadingStyle(False) & "&nbsp;<asp:label id=""lblTotalOwed"" runat=""server"" />" & vbCrLf
                sEcommerceContent &= "<br /><br />" & vbCrLf
                sEcommerceContent &= "<asp:Button id=""btnEditInvoice"" runat=""server"" cssclass=""button"" text=""Edit Invoice"" />" & vbCrLf
                sEcommerceContent &= "<asp:Label id=""lblInvoiceNumber"" runat=""server"" visible=""False"" />" & vbCrLf
                sEcommerceContent &= "<asp:label id=""lblInvoicetype"" runat=""server"" visible=""False"">" & projectDT.Rows(0).Item("EcommerceProduct") & "</asp:label>" & vbCrLf
                sEcommerceContent &= "<asp:Label id=""lblUSBAmountDue"" runat=""server"" visible=""False"" />" & vbCrLf
                sEcommerceContent &= "<asp:label id=""lblUSBtransactionkey"" runat=""server"" visible=""False""></asp:label>"
            End If

            Return sEcommerceContent
        End Function

        Shared Sub GetInvoiceMethodInfo(ByRef sEditInvoiceMethod As String, ByRef sAssignInvoiceInfo As String)
            If IseCommerceProject() Then
                sEditInvoiceMethod &= "Private Sub btnEditInvoice_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditInvoice.Click" & vbCrLf
                sEditInvoiceMethod &= "redirect(""http://web2/eCommerce/Invoices/Update/update.aspx?InvoiceNumber="" & EncryptText(lblInvoiceNumber.Text))" & vbCrLf
                sEditInvoiceMethod &= "End Sub" & vbCrLf & vbCrLf

                sAssignInvoiceInfo &= "lblInvoiceNumber.text = .item(""Invoice"")" & vbCrLf
                sAssignInvoiceInfo &= "lblTotalOwed.Text = GetInvoiceAmount(.Item(""Invoice""))" & vbCrLf
            End If
        End Sub

        Shared Function GetEcommerceControls() As String
            Dim controlText As String

            If IseCommerceProject() Then
                controlText = GetMailFile(GetTemplatePath() & "\General\EcommerceControls.eml")

                controlText = MailFieldSubstitute(controlText, "(InvoiceType)", projectDT.Rows(0).Item("EcommerceProduct"))
            End If

            return controlText
        End Function

        shared Function GetTotalOwedControls() As string
            dim controlText as String

            if IsEcommerceProject()
                controltext = "<asp:Panel id=""pnlTotalOwed"" runat=""server"">" & vbcrlf
                controltext &= "<br /><br />" & GetHeadingStyle() & "Total Owed:" & GetHeadingStyle(False) & "&nbsp;<asp:label id=""lblTotalOwed"" runat=""server"" />" & vbcrlf
                controltext &= "</asp:Panel>" & vbcrlf
            End If

            return controlText
        End Function
    End Class
End Namespace

