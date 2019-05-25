Imports System.Linq
Imports System.Data
Imports System.Data.SqlClient
Imports Common.General
Imports Microsoft.VisualBasic
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages

Imports Common.General.Controls
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.DataTypes
Imports Common.General.Folders
Imports Common.Webpages.Backend
Imports Common.General.ProjectOperations
Imports Common.General.DataSources
Imports Common.General.Columns
Imports Common.General.Links
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
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File

Public Class ReportPageWriter
    Inherits BackendListingPageWriter

    Dim mainContent, code, listing As String
    Dim panelDetails, panelDesigners, showSub, runReport As String
    Dim clickHandlers As New StringBuilder()
    Dim project As Project
    Dim reports As List(Of ProjectBackendExport)

    Public Sub New(ByRef project As Project)
        MyBase.New()

        Me.project = project
        Me.reports = project.GetReports().ToList()

        pageHTMLName = $"ReportsHTML"
        pageVBName = $"ReportsVB"
        pageDesignerName = $"ReportsDesigner"
        pageType = "reports" & GetAncillaryName()
        pageTitleAddition = " - Reports " & GetAncillaryName(True)
        pageFileName = "reports" & project.GetProjectNameAlphaNumericOnly()
    End Sub

    Protected Overrides Sub WriteSpecificHTML()
        CreateMainContent()

        pageBody = MailFieldSubstitute(pageBody, "(MainContent)", mainContent)
    End Sub

    Protected Overrides Sub WriteSpecificCodebehind()
        CreateCode()
        CreateRunReport()

        pageBody = MailFieldSubstitute(pageBody, "(RunReport)", runReport)
        pageBody = MailFieldSubstitute(pageBody, "(ReportCode)", code)
    End Sub

    Private Sub CreateCode()
        CreateClickHandlers()
        CreateShowReportSub()

        code = clickHandlers.ToString()
        code &= showSub
    End Sub

    Private Sub CreateMainContent()
        CreateLinks()
        CreatePanelDetails()

        mainContent = listing
        mainContent &= panelDetails
    End Sub

    Private Sub CreateRunReport()
        reports.ForEach(Sub(report)
                            runReport &= "If GetQueryString(""ID"") = """ & report.ID & """ Then" & vbCrLf
                            runReport &= "libReport" & report.ID & "_Click(libReport" & report.ID & ", Nothing)" & vbCrLf
                            runReport &= "End If" & vbCrLf & vbCrLf
                        End Sub)
    End Sub

    Private Sub CreateShowReportSub()
        showSub = "Sub ShowReport(ByRef pnlCurrentReport As Panel)" & vbCrLf

        reports.ForEach(Sub(report)
                            showSub &= "pnlReport" & report.ID & ".Visible = False" & vbCrLf
                        End Sub)

        showSub &= vbCrLf & "pnlCurrentReport.Visible = True" & vbCrLf
        showSub &= "End Sub" & vbCrLf
    End Sub

    Private Sub CreateClickHandlers()
        reports.ForEach(Sub(report)
                            clickhandlers.Append("Protected Sub libReport" & report.ID & "_Click(sender As Object, e As EventArgs)" & vbCrLf)
                            clickhandlers.Append("Dim sSelectString As String = If(GetSessionVariable(""ReportSelectString"") <> """", GetSessionVariable(""ReportSelectString""), """ & report.CreateExportSelectString("Report") & """)" & vbCrLf)
                            clickhandlers.Append("SelectRepeaterData(rptReport" & report.ID & ", GetDataTable(sSelectString,cnx))" & vbCrLf)
                            clickhandlers.Append("ShowReport(pnlReport" & report.ID & ")" & vbCrLf)
                            clickhandlers.Append("SetSessionVariable(""ReportSelectString"", """")" & vbCrLf)
                            clickhandlers.Append("End Sub" & vbCrLf & vbCrLf)
                        End Sub)

    End Sub

    Private Sub CreatePanelDetails()
        reports.ForEach(Sub(report)
                            panelDetails &= "<asp:panel id=""pnlReport" & report.ID & """ runat=""server"" Visible=""false"">" & vbCrLf
                            panelDetails &= "<br /><br />" & vbCrLf
                            panelDetails &= "<h3>" & report.Name & "</h3>" & vbCrLf
                            panelDetails &= "<asp:Repeater id=""rptReport" & report.ID & """ runat=""server"">" & vbCrLf
                            panelDetails &= "<ItemTemplate>" & vbCrLf
                            panelDetails &= Regex.Replace(report.Template, "\{\{(.*?)\}\}", "<%# Container.Dataitem(""$1"") %>") & vbCrLf
                            panelDetails &= "</ItemTemplate>" & vbCrLf
                            panelDetails &= "</asp:Repeater>" & vbCrLf
                            panelDetails &= "</asp:panel>" & vbCrLf
                        End Sub)

    End Sub

    Private Sub CreateLinks()
        listing = "<ul id=""ReportLinks"">" & vbCrLf

        reports.ForEach(Sub(report)
                            listing &= "<li><asp:LinkButton ID=""libReport" & report.ID & """ runat=""server"" text=""" & report.Name & """ onclick=""libReport" & report.ID & "_Click""></asp:LinkButton></li>" & vbCrLf
                        End Sub)

        listing &= "</ul>" & vbCrLf
    End Sub

    Protected Overrides Sub WriteSpecificFormLoad()
        AddMaintenanceHomeFormLoadLink(loadFormText)
    End Sub

    Protected Overrides Sub WriteSpecificDesigner()
        pageBody = MailFieldSubstitute(pageBody, "(DesignerControls)", GetDesignerControls(controlsDT))
    End Sub
End Class