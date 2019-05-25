Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.IO
Imports WhitTools.eCommerce
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.Filler

Imports WhitTools.DataTables
Imports WhitTools.Converter
Imports WhitTools.Validator
Imports WhitTools.Formatter
Imports WhitTools.SQL
Imports WhitTools.Encryption
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.ProjectOperations

Partial Public Class confirmation
    Inherits System.Web.UI.Page

     Shared cnx As SqlConnection = CreateSQLConnection("WebRAD")
     Shared ProjectDT As New DataTable
     Shared ControlsDT As New DataTable
    Public Const S_CONFIRMATION_MESSAGe As String = "Your submission has been received and is now being processed."

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then
            BindData()
        End If
    End Sub

    Sub BindData()
        Dim sDepartmentLink, sDepartmentname As String

        ProjectDT = GetDataTable(cnx, "Select P.*,  D.Department as DepartmentName, DB.Name as SQLDBName From " & DT_WEBRAD_PROJECTS & "  P left outer join Communications.dbo.ARA_Departments D on P.Department = D.ID left outer join " & DT_WEBRAD_SQLDATABASES & "  DB on P.SQLDatabase = DB.ID  Where P.ID = " & Request.QueryString("ID"), False)
        ProjectDT = ConvertDataTableColumnTypes(ProjectDT)

        With ProjectDT.Rows(0)
            Dim sFrontendLocation, sBackendLocation As String

            sFrontendLocation = If(Left(.Item("FrontendPath"), 6) = "\\web1", "http" & If(.Item("RequireLogin") = "1", "s", "") & "://www.whitworth.edu/", "http://web2/")
            sBackendLocation = If(Left(.Item("BackendPath"), 6) = "\\web1", "http" & If(.Item("RequireLogin") = "1", "s", "") & "://www.whitworth.edu/", "http://web2/")

            lblFrontendLink.Text = "<a href='" & sFrontendLocation & .Item("FrontendLink") & "' target='_blank'>" & sFrontendLocation & .Item("FrontendLink") & "</a>"
            lblBackendLink.Text = "<a href='" & sBackendLocation & .Item("BackendLink") & "/index.aspx' target='_blank'>" & sBackendLocation & .Item("BackendLink") & "/index.aspx</a>"
            lblPageTitle.Text = .Item("PageTitle")
            lblEcommerce.Text = If(IsECommerceProject(), "Y", "N")
            lblSQLDatabase.Text = .Item("SQLDBName")
            lblDepartmentName.Text = .Item("DepartmentName")
            lblSQLMainTableName.Text = .Item("SQLMainTableName")
            lblSQLInsertStoredProcedureName.Text = .Item("SQLInsertStoredProcedureName")
            lblSQLUpdateStoredProcedureName.Text = .Item("SQLUpdateStoredProcedureName")
            lblControlList.Text = GetListofValues("select case when Heading  = '' then Name + ' (' + Prefix + ')' else Heading + ' (' + Prefix + ')' end ControlDetail from " & DT_WEBRAD_PROJECTCONTROLS & " C left outer join " & DT_WEBRAD_CONTROLTYPES & "  T on C.ControlType = T.ID  left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  D on T.DataType = D.ID Where C.ProjectID = " & Request.QueryString("ID") & " Order by position asc", "ControlDetail", ",", "", cnx)
        End With
    End Sub

End Class