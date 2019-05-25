Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.IO.File
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
Imports Common.General.ProjectOperations
Imports Common.General.DataSources
Imports Common.General.Actions

Imports Common.General.Columns
Imports Common.General.Assembly
Imports Common.General.Links
Imports Common.Webpages.Frontend.MultiPage
Imports Common.Webpages.Main
Imports Common.Webpages.Frontend.MessagePages
Imports Common.SQL.StoredProcedure
Imports Common.SQL.Main
Imports Common.SQL.Table
Imports Common.Webpages.Backend
Imports Common.Webpages.Frontend.Email
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Printable
Imports Common.Webpages.Backend.Export
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.Backend.Archive
Imports Common.Webpages.BindData
Imports Common.Webpages.Workflow
Imports Common.Webpages.Validation
Imports Common.Webpages.ControlContent.Main
Imports Common.ProjectFiles
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File


Imports WhitTools.Utilities
Imports WhitTools
Imports Common.General
Imports System.Text.RegularExpressions
Imports System
Imports System.Linq
Imports System.Web.UI.WebControls

Namespace Webpages.Frontend
    Public Class Main
        Inherits Webpages.Main

        Shared Sub WriteFrontendPages()
            If FrontendCreationAllowed() Then
                SetupIndividualProjectVariables("Frontend")

                UpdateProgress("Creating Frontend", 0)

                With projectDT.Rows(0)
                    If IsMultipageForm() Then
                        pageNumber = 0
                        UpdateProgress("Writing multipage pages.", 1)
                        WriteMultipageSpecificPages()
                    Else
                        pageNumber = -1
                        currentPage = currentProject.ProjectPages.First()
                        UpdateProgress("Writing index page.", 1)
                        WriteIndexPage()
                    End If

                    WriteCommonClass()
                    WriteMultipageSections()

                    WriteWebConfig()
                End With

                WritePrintablePage()

                UpdateProgress("Creating and copying frontend project files", 0)
                CreateAndCopyFrontendProjectFiles()
            End If
        End Sub


        Public Shared Sub WriteIndexPage()
            Call New IndexPageWriter().WritePage()
        End Sub


        Shared Function FrontendCreationAllowed()
            Return projectDT.Rows(0).Item("IncludeFrontend") = "1" And createFrontend
        End Function

        Private Shared Sub WritePrintablePage()
            pageNumber = -1
            isPrintable = True
            Printable.WritePrintablePage("Printable", "Printable View")
            isPrintable = False
        End Sub


        Private Shared Sub WriteMultipageSections()
            If GetPageCount() > 1 Then
                pageNumber = 1

                For Each page As ListItem In Common.General.Variables.pages.Where(Function(selectedPage) selectedPage.Value > 0)
                    If page.Selected Then
                        currentPage = currentProject.ProjectPages.FirstOrDefault(Function(pp) pp.ID = page.Value)
                        WriteIndexPage()
                    End If

                    pageNumber += 1
                Next
            End If
        End Sub

        Public Shared Sub WriteCommonClass()
            Dim TemplatePath As String
            Dim pageBody As String

            TemplatePath = GetTemplatePath() & "\General\CommonVB.eml"
            pageBody = GetMailFile(TemplatePath)
            pageBody = MailFieldSubstitute(pageBody, "(Imports)", currentImportListing)
            pageBody = MailFieldSubstitute(pageBody, "(ProjectName)", projectName)
            pageBody = MailFieldSubstitute(pageBody, "(DatabaseName)", SQLDatabaseName)
            pageBody = MailFieldSubstitute(pageBody, "(DepartmentName)", departmentName)
            pageBody = MailFieldSubstitute(pageBody, "(IsFrontend)", General.Variables.isFrontend)
            pageBody = MailFieldSubstitute(pageBody, "(SupervisorsList)", GetSupervisorList())
            pageBody = MailFieldSubstitute(pageBody, "(SQLServerName)", GetSQLServerName(SQLServerName))
            pageBody = MailFieldSubstitute(pageBody, "(SQLMainTableName)", currentProject.GetFullDatabaseTableNameWithServer())
            pageBody = MailFieldSubstitute(pageBody, "(CommonCode)", GetCommonFunctions(projectDT.Rows(0).Item("SQLMainTableName"), GetAncillaryProject("RequireLogin"), projectDT.Rows(0).Item("MultipleSubmissions"), projectDT.Rows(0).Item("SQLAdditionalCertificationStatement"), GetCheckClosedMethod()))
            pageBody = MailFieldSubstitute(pageBody, "(SectionsAllowed)", GetSectionsAllowed())
            pageBody = MailFieldSubstitute(pageBody, "(ProjectTitle)", projectTitle)

            If GetCurrentProjectDT().Rows(0).Item("ConfirmationMessage") <> "" Then
                pageBody = MailFieldSubstitute(pageBody, "(ConfirmationMessage)", GetFormattedConfirmationMessage())
            Else
                pageBody = MailFieldSubstitute(pageBody, "(ConfirmationMessage)", S_CONFIRMATION_MESSAGE)
            End If

            pageBody = MailFieldSubstitute(pageBody, "(ClosedMessage)", projectDT.Rows(0).Item("ClosedMessage").ToString().Replace(vbCrLf, "").Replace("""", """"""))

            If Not isFrontend Then
                pageBody = MailFieldSubstitute(pageBody, "(Actions)", currentProject.GetCommonActions())
            End If

            Directory.CreateDirectory(baseDir & "\App_Code\")

            RemoveUnusedTemplateFields(pageBody)
            WriteAllText(baseDir & "\" & "\App_Code\Common.vb", pageBody)
        End Sub

        Public Shared Function GetFormattedConfirmationMessage() As String
            Return New Regex("[\r\n]").Replace(GetCurrentProjectDT().Rows(0).Item("ConfirmationMessage").ToString().Replace("""", """""").Replace(Environment.NewLine, ""), "")
        End Function



        Private Shared Sub WriteMultipageSpecificPages()
            WriteStatusPage()

            If DefaultCertificationPage() Then
                WriteCertificationPage()
            End If
        End Sub
    End Class
End Namespace
