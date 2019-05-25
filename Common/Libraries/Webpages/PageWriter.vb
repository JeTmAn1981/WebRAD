Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.VisualBasic
Imports System.IO.File

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
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.utilities
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File
imports whittools.RulesAssignments

Public mustinherit Class PageWriter
        public templatePath, pageFileName, pageBody, redirectPage, additionalButtons, additionalButtonsMethods, ecommerce, pageTitle, loadFormText, mainPageName, mainPageUrl As String
    Protected pageHTMLName, pageVBName, pageDesignerName, pageType, pagePurpose, pageTitleAddition As String

    Protected sub New()
            SetTypeDependentVariables()
        End sub

        protected sub SetTypeDependentVariables
            SetEcommerceStatus()
            SetPageName()
            SetPageTitle()
            SetRedirectPage()
        End Sub

    Private Sub SetEcommerceStatus()
        If IseCommerceProject() AndAlso pageNumber = -1 Then
            ecommerce = "eCommerce"
        End If
    End Sub

    Private Sub SetPageName()
            pageFileName = If(pageNumber = -1, "index", "section" & pageNumber)
        End Sub

        Private Sub SetRedirectPage()
            If pageNumber = -1 Then
                redirectPage = "confirmation.aspx"
            Else
                If pageNumber = GetPagecount() Then
                    redirectPage = If(DefaultCertificationPage(), "certification.aspx", "confirmation.aspx")
                Else
                    redirectPage = "section" & pageNumber + 1 & ".aspx"
                End If
            End If
        End Sub

        Private Sub SetPageTitle()
        pageTitle = GetRootProjectTitle()

        If IsAncillaryProject() Then
                pageTitle &= " - " & GetAncillaryName(True)
            End If
        End Sub

   public sub WritePage()
        WriteCodebehind()
        WriteHTML()
        WriteDesigner()
    End sub

    Private sub WriteHTML()
        With projectDT.Rows(0)
            UpdateProgress($"Writing {pageType} HTML.", 2)

            AssemblePage(pageBody, projectLocation, pageHTMLName)

            pageBody = MailFieldSubstitute(pageBody, "(Responsive)", "_Responsive")
            pageBody = MailFieldSubstitute(pageBody, "(PageTitle)", pageTitle & pageTitleAddition)
            pageBody = MailFieldSubstitute(pageBody, "(PageType)", pageType)
            pageBody = MailFieldSubstitute(pageBody, "(ProjectName)", projectName)
            pageBody = MailFieldSubstitute(pageBody, "(Department)", departmentName)

            WriteSpecificHTML()

            RemoveUnusedTemplateFields(pageBody)

            WriteFile()
        End With
    End Sub

    Private Sub WriteFile(Optional ByVal extension As String = "")
        WriteAllText(baseDir & "\" & $"{pageFileName}.aspx" & extension, pageBody)
    End Sub

    Private sub WriteCodebehind()
        With projectDT.Rows(0)
            UpdateProgress($"Writing {pageType} VB.", 2)
            templatePath = GetTemplatePath() & $"\{GetFileLocation()}\{pageVBName}.eml"

            logger.Info(templatePath)

            Try
                pageBody = GetMailFile(TemplatePath)
            Catch ex As Exception
                logger.Error(ex.ToString)
            End Try

            pageBody = MailFieldSubstitute(pageBody, "(Imports)", currentImportListing)
            pageBody = MailFieldSubstitute(pageBody, "(ProjectName)", projectName)
            pageBody = MailFieldSubstitute(pageBody, "(PageType)", pageType)
            pageBody = MailFieldSubstitute(pageBody, "(PageTitle)", If(pageTitleAddition <> "", pageTitleAddition, pageTitle))
            pageBody = MailFieldSubstitute(pageBody, "(DatabaseName)", SQLDatabaseName)
            pageBody = MailFieldSubstitute(pageBody, "(SQLServerName)", GetSQLServerName(SQLServerName))

            WriteLoadForm()
            WriteSpecificCodebehind()

            RemoveUnusedTemplateFields(pageBody)

            WriteFile(".vb")
        End With
    End Sub

    Private Sub WriteLoadForm()
        If isFrontend And Not isInsert Then
            loadFormText = "ftLoader.LoadFormText("

            If IsMultipageForm() Then
                loadFormText &= "pageName"

                If pageType <> "status" Then
                    loadFormText &= ", AddressOf AddStatusBreadcrumb"
                End If
            End If

            loadFormText &= ")"
        Else
            loadFormText = ""
            AddDepartmentBreadcrumbLinks(loadFormText)
            WriteSpecificFormLoad()
            AddFormLoadHeadingCurrentPageLink(loadFormText, pageTitle, pageTitleAddition)
        End If

        pageBody = MailFieldSubstitute(pageBody, "(LoadFormText)", loadFormText)
    End Sub

    Protected Overridable Sub WriteSpecificFormLoad()

    End Sub

    Private Sub WriteDesigner()
        With projectDT.Rows(0)
            UpdateProgress($"Writing {pageType} designer.", 2)

            templatePath = GetTemplatePath() & $"\{GetFileLocation()}\{pageDesignerName}.eml"

            logger.Info(templatePath)

            Try
                pageBody = GetMailFile(templatePath)
            Catch ex As Exception
                logger.Error(ex.ToString)
            End Try

            pageBody = MailFieldSubstitute(pageBody, "(PageType)", pageType)

                WriteSpecificDesigner()

                RemoveUnusedTemplateFields(pageBody)

            WriteFile(".designer.vb")

        End With
    End sub

    protected overridable sub WriteSpecificHTML()

    End sub
    protected overridable sub WriteSpecificCodebehind()

    End sub
    protected overridable sub WriteSpecificDesigner()

    End sub
End Class
