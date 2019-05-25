Imports System
Imports System.Security.Cryptography
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.Web
Imports System.Net
Imports System.IO
Imports Common
Imports WhitTools.eCommerce
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.File
Imports WhitTools.Filler
Imports WhitTools.Utilities
Imports WhitTools.Email
Imports WhitTools.DataTables
Imports WhitTools.Converter
Imports WhitTools.Validator
Imports WhitTools.Formatter
Imports WhitTools.GlobalEnum
Imports WhitTools.Repeaters
Imports WhitTools.SQL
Imports WhitTools.Encryption
Imports WhitTools.Workflow
Imports WhitTools.eCommerceStatusChecks
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.ProjectOperations

Public Class Template
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack
            LoadFormText()
        End If
        
    End Sub
    
    Sub LoadFormText()
        Dim projectOptions as String
        Dim projectID As Integer = 0 

        Integer.TryParse(GetProjectID,projectid)

        projectOptions &= "<datalist id=""projectList"">"

        For Each currentRow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTS & " WHERE ID > 0 ORDER BY PageTitle ASC").Rows
            projectOptions &= "<option " & If(projectID = currentRow.Item("ID"), "selected", "") & " value=""" & currentRow.Item("ID") & """>" & currentRow.Item("PageTitle") & "</option>"
        Next

        projectOptions &= "</datalist>"

        'lblWebRADPageTitle.Text = "<h2>WebRAD - Main Details - <select onchange=""window.location = window.location.href.replace(/ID=(\d+)/i,'ID=' + this.value);"">" & projectOptions & "</select></h2>"
        projectOptions &= "<input onchange=""ChangeToProject(this.value)"" list='projectList'>"
        lblProjectSelection.Text = projectOptions

    End Sub



End Class