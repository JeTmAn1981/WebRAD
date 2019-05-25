Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Linq
Imports WhitTools.DataWarehouse
Imports WhitTools.eCommerce
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.File
Imports WhitTools.Filler
Imports System.IO.File
Imports WhitTools.DataTables
Imports WhitTools.Converter
Imports WhitTools.Validator
Imports WhitTools.Formatter
Imports WhitTools.GlobalEnum
Imports WhitTools.Repeaters
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.WebTeam
'Imports WhitTools.SQL
Imports WhitTools.Encryption
Imports WhitTools.RulesAssignments
Imports WhitTools.Workflow
Imports Common.General.Ancillary
Imports Common.General.Assembly
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.DataSources
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.ProjectOperations
Imports Common.General.Controls
Imports Common.General.Folders
Imports Common.General.DataTypes
Imports Common.General.Pages
Imports Common.SQL.Main
Imports Common.BuildSetup
Imports Common.Webpages.BindData
Imports Common.Webpages.Frontend.Main
Imports Common.Webpages.Frontend.MultiPage
Imports Common.Webpages.Backend.Main
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.Backend.Search
Imports System.Threading
Imports System.Reflection
Imports Common

Imports WhitTools.Utilities
Imports Common.Webpages.Backend

Public Class ProjectFiles

    Public Shared Sub CreateAndCopyBackendProjectFiles()
        Dim sDirectory As String
        Dim sBackendPath As String
        Dim sBackendLink As String

        sDirectory = S_PROJECTFILESNETWORKPATH & projectName & "\Backend"

        If Not Directory.Exists(sDirectory) Then
            Directory.CreateDirectory(sDirectory.Replace("\\web2\~Whitworth\", "f:\inetpub\~whitworth"))
        End If

        Dim cmdShellCommand = "exec xp_cmdshell 'robocopy """ & GetTemplatePath() & "Visual Studio Files\Standard"" """ & sDirectory & """ /E'"

        Dim cmd As New SqlCommand("", cnx)
        cmd.CommandTimeout = 300

        cmd.CommandText = cmdShellCommand
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()

        WriteProjectFile("ProjectName.sln", "Backend")

        cmdShellCommand = "exec xp_cmdshell 'robocopy """ & GetTemplatePath() & "Visual Studio Files\Standard"" """ & sDirectory & """ /E'"

        cmd.CommandText = cmdShellCommand
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()


        WriteProjectFile("ProjectName.sln", "Backend")
        WriteProjectFile("ProjectName.vbproj", "Backend")
        WriteProjectFile("ProjectName.vbproj.user", "Backend")
        WriteProjectFile("Global.asax", "Backend")
        WriteProjectFile("AssemblyInfo.vb", "Backend", "My Project\")
        WriteProjectFile("Resources.Designer.vb", "Backend", "My Project\")
        WriteProjectFile("Settings.Designer.vb", "Backend", "My Project\")
        WriteSharedContent()

        sBackendPath = GetPathLink(projectDT.Rows(0).Item("BackendPath"), projectType, "Path")
        sBackendLink = GetPathLink(projectDT.Rows(0).Item("BackendLink"), projectType)

        If Not Directory.Exists(sBackendPath) Then
            cmdShellCommand = "exec xp_cmdshell 'MD " & sBackendPath & "'"
            cmd.CommandText = cmdShellCommand

            cmd.CommandText = cmdShellCommand
            cmd.Connection.Open()
            cmd.ExecuteNonQuery()
            cmd.Connection.Close()
        End If

        logger.Info("Copying backend files to live location")

        Try
            cmdShellCommand = "exec xp_cmdshell 'robocopy """ & sDirectory & """ """ & sBackendPath & """ /E'"
            cmd.CommandText = cmdShellCommand

            cmd.Connection.Open()
            cmd.ExecuteNonQuery()
            cmd.Connection.Close()
        Catch ex As Exception
            logger.Error("Error copying files:")
            logger.Error(ex.ToString)
        End Try


        CreateVirtualDirectory("Backend", projectName, sBackendLink, sBackendPath)

        If ProjectIncludesProspectUserControl() Then
            CreateVirtualDirectory("Backend", projectName, sBackendLink & "ProspectGathering/", "\\web2\~whitworth\Administration\Admissions\ProspectGathering")
        End If
    End Sub

    Private Shared Sub WriteSharedContent()
        WriteNavigationFile()
    End Sub

    Public Shared Sub CreateAndCopyFrontendProjectFiles()
        Dim sFrontendPath As String
        Dim sFrontendLink As String
        Dim sDirectory As String = S_PROJECTFILESNETWORKPATH & projectName & "\Frontend"

        If isMVC Then
            WriteMVCProjectFiles(sDirectory)
        Else
            WriteWebformsProjectFiles(sDirectory)
        End If

        sFrontendPath = GetPathLink(projectDT.Rows(0).Item("FrontendPath"), projectType, "Path")
        sFrontendLink = GetPathLink(projectDT.Rows(0).Item("FrontendLink"), projectType)

        If Not Directory.Exists(sFrontendPath) Then
            logger.Info("exec xp_cmdshell 'MD " & sFrontendPath & "'")
            WhitTools.SQL.ExecuteNonQuery("exec xp_cmdshell 'MD " & sFrontendPath & "'", "", 3, True, Nothing, False, True, False)
        End If

        logger.Info("exec xp_cmdshell 'robocopy """ & sDirectory & """ """ & sFrontendPath & """ /E'")
        Dim cmd As SqlCommand = New SqlCommand("exec xp_cmdshell 'robocopy """ & sDirectory & """ """ & sFrontendPath & """ /E'", WhitTools.SQL.CreateSQLConnection())

        Try
            cmd.Connection.Open()
            cmd.ExecuteNonQuery()
            cmd.Connection.Close()

        Catch ex As Exception
            logger.Error("Error copying project files: ")
            logger.Error(ex.ToString)
        End Try

        CreateVirtualDirectory("Frontend", projectName, sFrontendLink, sFrontendPath)

        If ProjectIncludesProspectUserControl() Then
            CreateVirtualDirectory("Frontend", projectName, sFrontendLink & "ProspectGathering/", "\\web1\~whitworth\Administration\Admissions\Undergraduate\Forms\ProspectGathering")
        End If
    End Sub

    Private Shared Sub WriteWebformsProjectFiles(sDirectory As String)
        UpdateProgress("Writing WebForms project files", 1)

        If Not Directory.Exists(sDirectory) Then
            logger.Info("creating - " & sDirectory.Replace("\\web2\~Whitworth\", "f:\inetpub\~whitworth"))
            Directory.CreateDirectory(sDirectory.Replace("\\web2\~Whitworth\", "f:\inetpub\~whitworth"))
        End If

        CopyCompleteFiles(sDirectory)

        WriteProjectFile("ProjectName.sln", "Frontend")
        WriteProjectFile("ProjectName.vbproj", "Frontend")
        WriteProjectFile("ProjectName.vbproj.user", "Frontend")
        WriteProjectFile("Global.asax", "Frontend")
        WriteProjectFile("AssemblyInfo.vb", "Frontend", "My Project\")
        WriteProjectFile("Resources.Designer.vb", "Frontend", "My Project\")
        WriteProjectFile("Settings.Designer.vb", "Frontend", "My Project\")
    End Sub

    Private Shared Sub CopyCompleteFiles(directory As String)
        CopyStandardFiles(directory)
        CopyMessagePage(directory)

        If CurrentProjectRequiresNonWhitworthLogin() Then
            CopyAuthenticationPages(directory)
        End If
    End Sub

    Private Shared Sub CopyStandardFiles(directory As String)
        ExecuteViaCommandShell("robocopy """ & GetTemplatePath() & "Visual Studio Files\Standard"" """ & directory & """ /E")
    End Sub

    Private Shared Sub CopyMessagePage(directory As String)
        ExecuteViaCommandShell("robocopy """ & GetTemplatePath() & "Frontend\Complete"" """ & directory & """ ""message*.*"" /E")
    End Sub

    Private Shared Sub CopyAuthenticationPages(ByVal directory As String)
        ExecuteViaCommandShell("robocopy """ & GetTemplatePath() & "Frontend\Complete\Authentication"" """ & directory & """ /E")
    End Sub

    Private Shared Sub ExecuteViaCommandShell(ByVal command As String)
        logger.Info("exec xp_cmdshell '" & command & "'")
        WhitTools.SQL.ExecuteNonQuery("exec xp_cmdshell '" & command & "'", "", 3, True, Nothing, False, True, False)
    End Sub


    Private Shared Sub WriteMVCProjectFiles(sDirectory As String)
        WhitTools.SQL.ExecuteNonQuery("exec xp_cmdshell 'copy """ & S_WHITTOOLS_DIR & "WhitTools*.*"" """ & GetTemplatePath() & "MVC Files\Standard\bin"" /Y'", "", 3, True, Nothing, False, True, False)

        If Not Directory.Exists(sDirectory) Then
            Directory.CreateDirectory(sDirectory.Replace("\\web2\~Whitworth\", "f:\inetpub\~whitworth"))
        End If

        UpdateProgress("Writing standard MVC files", 1)
        WhitTools.SQL.ExecuteNonQuery("exec xp_cmdshell 'robocopy """ & GetTemplatePath() & "MVC Files\Standard"" """ & sDirectory & """ /E'", "", 3, True, Nothing, False, True, False)

        UpdateProgress("Writing bundles", 1)

        WriteAppStartFiles()

        UpdateProgress("Writing MVC models", 1)

        WriteModels()

        UpdateProgress("Writing MVC project files", 1)

        WriteProjectFile("Global.asax", "Frontend")
        WriteProjectFile("Global.asax.cs", "Frontend")
        WriteProjectFile("ProjectName.sln", "Frontend")
        WriteProjectFile("ProjectName.csproj", "Frontend")
        WriteProjectFile("ProjectName.csproj.user", "Frontend")
        WriteProjectFile("Web.config", "Frontend")

        UpdateProgress("Writing MVC views", 1)
        WriteProjectFile("Web.config", "Frontend", "Views\")
        WriteProjectFile("_ViewStart.cshtml", "Frontend", "Views\")
        WriteProjectFile("_Layout.cshtml", "Frontend", "Views\Shared\")
        WriteProjectFile("Error.cshtml", "Frontend", "Views\Shared\")
        WriteProjectFile("Index.cshtml", "Frontend", "Views\Home\")
        WriteProjectFile("Confirmation.cshtml", "Frontend", "Views\Home\")

        WriteProjectFile("AssemblyInfo.cs", "Frontend", "Properties\")

        WriteProjectFile("HomeController.cs", "Frontend", "Controllers\")
    End Sub

    Shared Sub WriteAppStartFiles()
        WriteProjectFile("BundleConfig.cs", "Frontend", "App_Start\")
        WriteProjectFile("FilterConfig.cs", "Frontend", "App_Start\")
        WriteProjectFile("RouteConfig.cs", "Frontend", "App_Start\")
    End Sub

    Shared Sub WriteModels()
        WriteProjectFile("ProjectName.Context.cs", "Frontend", "Models\")
        WriteProjectFile("ProjectName.Context.tt", "Frontend", "Models\")
        WriteProjectFile("ProjectName.cs", "Frontend", "Models\")
        WriteProjectFile("ProjectName.Designer.cs", "Frontend", "Models\")
        WriteProjectFile("ProjectName.tt", "Frontend", "Models\")
        WriteProjectFile("ProjectName.edmx.diagram", "Frontend", "Models\")
        WriteProjectFile("ProjectName.edmx", "Frontend", "Models\")
    End Sub

    Shared Sub WriteNavigationFile()
        Dim basedir As String = S_PROJECTFILESPATH & projectName & "\Backend"
        Dim navigation As String = ""

        Webpages.Backend.Navigation.GetBackendNavigation(navigation, GetBackendUpdateReference() & GetAncillaryName())

        WriteAllText(basedir & "\" & "navigation.htm", navigation)
    End Sub

    Shared Sub WriteProjectFile(ByVal sFileName As String, ByVal sProjectType As String, Optional ByVal sLocation As String = "")
        Dim TemplatePath, pageBody As String
        Dim strBaseDir As String = S_PROJECTFILESPATH & projectName & "\" & sProjectType & "\" & sLocation

        If Not Directory.Exists(strBaseDir) Then
            Directory.CreateDirectory(strBaseDir)
        End If

        With projectDT.Rows(0)
            If isMVC Then
                TemplatePath = GetTemplatePath() & "MVC Files\Templates\" & sLocation & sFileName
            Else
                TemplatePath = GetTemplatePath() & "Visual Studio Files\Templates\" & sLocation & sFileName
            End If

            Try
                pageBody = System.IO.File.ReadAllText(TemplatePath)
            Catch ex As Exception
                logger.Error("Error getting file - " & TemplatePath)
            End Try

            pageBody = MailFieldSubstitute(pageBody, "(ProjectName)", projectName)
            pageBody = MailFieldSubstitute(pageBody, "(ProjectTitle)", projectTitle)
            pageBody = MailFieldSubstitute(pageBody, "(SQLServerName)", SQLServerName)
            pageBody = MailFieldSubstitute(pageBody, "(SQLDatabaseName)", SQLDatabaseName)
            pageBody = MailFieldSubstitute(pageBody, "(SQLMainTableName)", SQLMainTableName)

            If sFileName = "ProjectName.vbproj" Then
                SetProjectCompileContentIncludes()

                pageBody = MailFieldSubstitute(pageBody, "(ContentIncludes)", contentIncludes)
                pageBody = MailFieldSubstitute(pageBody, "(CompileIncludes)", compileIncludes)
            End If

            If sFileName = "ProjectName.edmx" Then
                pageBody = MailFieldSubstitute(pageBody, "(StorageProperties)", storageProperties)
                pageBody = MailFieldSubstitute(pageBody, "(ConceptualProperties)", conceptualProperties)
                pageBody = MailFieldSubstitute(pageBody, "(ScalarProperties)", scalarProperties)
            End If

            If sFileName = "ProjectName.cs" Then
                pageBody = MailFieldSubstitute(pageBody, "(ModelProperties)", modelProperties)
            End If

            If sFileName = "HomeController.cs" Then
                pageBody = MailFieldSubstitute(pageBody, "(Page1Columns)", GetListofValues("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE PageID = (SELECT MIN(ID) FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & GetProjectID() & ") AND IncludeDatabase=1 ORDER BY Position ASC", "Name"))
            End If

            Try
                RemoveUnusedTemplateFields(pageBody)

                WriteAllText(strBaseDir & "\" & sFileName.Replace("ProjectName", projectName), pageBody)
            Catch ex As Exception
                logger.Error(ex.ToString)
                Exit Sub
            End Try
        End With
    End Sub

    Shared Sub CreateVirtualDirectory(ByVal sLocation As String, ByVal sProjectName As String, ByVal sVirtualPath As String, ByVal sProjectPath As String)
        Dim templatePath, pageBody, siteName, server, rootPath As String
        Dim strBaseDir As String = S_PROJECTFILESPATH & sProjectName & "\"

        With projectDT.Rows(0)
            templatePath = GetTemplatePath() & "SetUpVirtualDirectory.ps1"

            If Right(sVirtualPath, 1) = "/" Then
                sVirtualPath = Left(sVirtualPath, sVirtualPath.Length - 1)
            End If

            If Left(sProjectPath, 6) = "\\web1" Then
                sProjectPath = sProjectPath.Replace("\\web1\~whitworth\", "").Replace("/", "\")
                siteName = "whitworth.edu"
                server = "web1"
                rootPath = "c:\inetpub\~whitworth"
            Else
                sProjectPath = sProjectPath.Replace("\\web2\~whitworth\", "").Replace("/", "\")
                siteName = "Intranet"
                server = "web2"
                rootPath = "f:\inetpub\~whitworth"
            End If

            pageBody = GetMailFile(templatePath)
            pageBody = MailFieldSubstitute(pageBody, "(SiteName)", siteName)
            pageBody = MailFieldSubstitute(pageBody, "(VirtualPath)", sVirtualPath)
            pageBody = MailFieldSubstitute(pageBody, "(RootPath)", rootPath)
            pageBody = MailFieldSubstitute(pageBody, "(ProjectPath)", sProjectPath)
            pageBody = MailFieldSubstitute(pageBody, "(AnonymousAuthentication)", GetAnonymousAuthenticationValue(sLocation, server))
            pageBody = MailFieldSubstitute(pageBody, "(SSL)", GetSSLValue(sLocation, server))

            Try
                RemoveUnusedTemplateFields(pageBody)
                WriteAllText(strBaseDir & "\" & "SetUp" & sLocation & "VirtualDirectory.ps1", pageBody)
            Catch ex As Exception
            End Try
        End With


        Dim cmdShellCommand As String
        Dim cmd As New SqlCommand("", cnx)
        cmd.CommandTimeout = 300

        cmdShellCommand = "exec xp_cmdshell 'copy /Y """ & strBaseDir.Replace("f:\inetpub\", "\\web2\") & "SetUp" & sLocation & "VirtualDirectory.ps1"" ""c:\SetUp" & sLocation & "VirtualDirectory.ps1""'"
        logger.Info(cmdShellCommand)
        cmd.CommandText = cmdShellCommand

        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()


        Try
            cmdShellCommand = "exec xp_cmdshell 'c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe invoke-command -computer:" & server & " -filepath:""c:\SetUp" & sLocation & "VirtualDirectory.ps1" & """'"
            logger.Info(cmdShellCommand)
            cmd.CommandText = cmdShellCommand

            cmd.Connection.Open()
            cmd.ExecuteNonQuery()
            cmd.Connection.Close()
        Catch ex As Exception
            'Response.Write(ex.ToString)
        End Try
    End Sub

    Shared Function GetAnonymousAuthenticationValue(ByVal location As String, ByVal server As String) As String
        Return If(location = "Frontend" And server = "web1" And Not CurrentProjectRequiresWhitworthLogin(), "True", "False")
    End Function

    Shared Function GetSSLValue(ByVal location As String, ByVal server As String) As String
        If (location = "Backend" And server = "web1") Or (projectType <> "Test" And location = "Frontend" And server = "web1" AndAlso CurrentProjectRequiresWhitworthLogin()) Then
            Return "Ssl"
        End If

        Return "None"
    End Function

    Shared Sub SaveProjectBuild(ByVal nProjectID As Integer, ByVal bCreateFrontend As Boolean, ByVal bCreateBackend As Boolean, ByVal includeStatusPage As Boolean, ByVal sProjectType As String, ByVal sFormsType As String, ByVal username As String, ByVal pages() As ProjectPage, ByVal backendOptions() As ProjectBuildBackendOption, ByVal ancillaryMaintenance() As ProjectBuildBackendAncillaryMaintenance)
        Dim db = New WebRADEntities()
        Dim build As New ProjectBuild()


        build.ProjectID = nProjectID
        build.Frontend = bCreateFrontend
        build.Backend = bCreateBackend
        build.IncludeStatusPage = includeStatusPage
        build.Type = sProjectType
        build.FormsType = sFormsType
        build.Username = username
        build.DateSubmitted = Now
        build.ProjectBuildPages = New List(Of ProjectBuildPage)

        pages.Where(Function([option]) [option].Included).ToList().ForEach(Sub(page)
                                                                               build.ProjectBuildPages.Add(New ProjectBuildPage() With {.PageID = page.ID})
                                                                           End Sub)

        build.ProjectBuildBackendOptions = New List(Of ProjectBuildBackendOption)
        backendOptions.ToList().ForEach(Sub([option])
                                            build.ProjectBuildBackendOptions.Add([option])
                                        End Sub)

        build.ProjectBuildBackendAncillaryMaintenances = New List(Of ProjectBuildBackendAncillaryMaintenance)
        ancillaryMaintenance.ToList().ForEach(Sub(ancillary)
                                                  build.ProjectBuildBackendAncillaryMaintenances.Add(New ProjectBuildBackendAncillaryMaintenance() With {.AncillaryMaintenanceID = ancillary.ID})
                                              End Sub)

        db.ProjectBuilds.Add(build)
        db.SaveChanges()
    End Sub

    Shared Sub CorrectProjectFilePaths()
        Dim sFrontendPath, sBackendPath As String

        If CStr(projectDT.Rows(0).Item("FrontendNewFolder")) <> "" Then
            sFrontendPath = projectDT.Rows(0).Item("FrontendPath")
            sFrontendPath &= "\" & projectDT.Rows(0).Item("FrontendNewFolder")

            ExecuteNonQuery("Update " & DT_WEBRAD_PROJECTS & "  Set FrontendPath = '" & WhitTools.SQL.CleanSQL(sFrontendPath) & "', FrontendNewFolder='' Where ID = " & projectDT.Rows(0).Item("ID"), Common.General.Variables.cnx)
        End If

        If projectDT.Rows(0).Item("BackendNewFolder") <> "" Then
            sBackendPath = projectDT.Rows(0).Item("BackendPath")
            sBackendPath &= "\" & projectDT.Rows(0).Item("BackendNewFolder")

            ExecuteNonQuery("Update " & DT_WEBRAD_PROJECTS & "  Set BackendPath = '" & WhitTools.SQL.CleanSQL(sBackendPath) & "', BackendNewFolder='' Where ID = " & projectDT.Rows(0).Item("ID"), Common.General.Variables.cnx)
        End If

        projectDT = GetDataTable(cnx, "Select P.*,  DB.Name as SQLDBName, S.Name as SQLServerName From " & DT_WEBRAD_PROJECTS & "  P left outer join " & DT_WEBRAD_SQLDATABASES & "  DB on P.SQLDatabase = DB.ID left outer join " & DT_WEBRAD_SQLSERVERS & " S on DB.ServerID = S.ID Where P.ID = " & GetProjectID(), False)
        projectDT = ConvertDataTableColumnTypes(projectDT)
        EliminateNull(projectDT)
    End Sub

    Shared Sub SetProjectCompileContentIncludes()
        SetCompileContentInclude("confirmation.aspx")

        If pages.Count > 1 Then
            SetMultipageCompileContentIncludes()
        End If
    End Sub

    Private Shared Sub SetMultipageCompileContentIncludes()
        SetCompileContentInclude("status.aspx")
        'Potential bug, need to check to make sure standalone certification is used with project
        SetCompileContentInclude("certification.aspx")

        For pageCounter As Integer = 1 To pages.Count
            SetCompileContentInclude("section" & pageCounter & ".aspx")
        Next
    End Sub


    Shared Sub SetCompileContentInclude(ByVal fileName As String)
        SetContentInclude(fileName)
        SetCompileInclude(fileName)
    End Sub

    Shared Sub SetContentInclude(ByVal fileName As String)
        contentIncludes &= "<Content Include=""" & fileName & """ />"
    End Sub

    Shared Sub SetCompileInclude(ByVal fileName As String)
        compileIncludes = "<Compile Include=""" & fileName & ".vb"">" & vbCrLf
        compileIncludes &= "<DependentUpon>" & fileName & "</DependentUpon>" & vbCrLf
        compileIncludes &= "<SubType>ASPXCodeBehind</SubType>" & vbCrLf
        compileIncludes &= "</Compile>" & vbCrLf
    End Sub
End Class
