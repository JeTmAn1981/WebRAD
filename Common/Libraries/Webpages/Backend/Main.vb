Imports WhitTools.DataTables
Imports WhitTools.SQL
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Ancillary
Imports Common.General.ProjectOperations
Imports Common.General.Pages
Imports Common.Webpages.Frontend.Main
Imports ProjectFiles
imports Common.Webpages.Backend.AdditionalOptions
Imports Common.Webpages.Backend.Navigation
Imports WhitTools.Utilities

Namespace Webpages.Backend
    Public Class Main
        Inherits Webpages.Main

        Shared Sub WriteBackendPages()
            UpdateProgress("Creating Backend", 0)
            SetupIndividualProjectVariables("Backend")

            General.Variables.isFrontend = False
            pageNumber = - 1

            If Not IsSingletonProject() And BackendOptionBuildAllowed(S_BACKEND_OPTION_MAINLISTING) Then
                WriteBackendIndexPage()
            End If

            If BackendOptionBuildAllowed(S_BACKEND_OPTION_UPDATE) Then
                WriteUpdatePage()
            End If

            If Not IsAncillaryProject() Then
                WriteCommonClass()
            End If

            writewebconfig()
            CreateAdditionalBackendOptions()
        End Sub

        Private Shared Sub WriteUpdatePage()
            Call New UpdatePageWriter(GetBackendOption(S_BACKEND_OPTION_ARCHIVE_VIEW)).WritePage()
        End Sub

        Private Shared Sub WriteBackendIndexPage()
            isBackendIndex = True
            'UpdateProgress("Creating backend index", 1)
            Call new BackendIndexPageWriter("index", "Maintenance").WritePage()
            'WriteBackendIndexPage("index", "Maintenance")
            isBackendIndex = False
        End Sub

        Shared Function GetSelectCookie(ByVal sType As String) As String
            Dim sSelectCookie As String

            sSelectCookie = "If GetCookieValue(""" & sType & """) <> """" Then" & vbCrLf
            sSelectCookie &= "ddl" & sType & ".SelectedIndex = CInt(GetCookieValue(""" & sType & """))" & vbCrLf
            sSelectCookie &= "End If" & vbCrLf & vbCrLf

            Return sSelectCookie
        End Function

        Shared Function GetBackendOption(ByVal sOptionName As String) As Boolean
            If sOptionName = S_BACKEND_OPTION_MAINLISTING Or sOptionName = S_BACKEND_OPTION_UPDATE Then
                Return True
            End If

            Return GetDataTable($"Select OT.ID, OT.Name From {DT_WEBRAD_PROJECTBACKENDOPTIONS} O left outer join {DT_WEBRAD_BACKENDOPTIONTYPES}  OT on O.Type = OT.ID Where ProjectID = {GetProjectID()} AND Name = '{CleanSQL(sOptionName)}'").Rows.Count > 0
        End Function

        Shared Function ProjectUsesArchive() As Boolean
            Return GetBackendOption(S_BACKEND_OPTION_ARCHIVE_VIEW)
        End Function

        Shared Function BackendOptionBuildAllowed(ByVal sOptionName As String) As Boolean
            Return GetDataTable($"Select OT.ID, OT.Name From {DT_WEBRAD_PROJECTBUILDBACKENDOPTIONS} BO left outer join {DT_WEBRAD_BACKENDOPTIONTYPES}  OT on BO.BackendOptionTypeID = OT.ID Where Name = '{CleanSQL(sOptionName)}' AND  BuildID = " & currentProjectBuild.ID & " AND ProjectID = " & GetProjectID()).Rows.Count > 0
        End Function

        Shared Function GetWhereStatement(Optional ByVal bSearch As Boolean = False, Optional ByVal sFilterStatement As String = "") As String
            Dim sSelectString As String = ""
            Dim conditionals As New List(Of String)()

            If GetPageCount() > 1 Then
                sSelectString &= "conditionals.Add(""Certification = '1'"")" & vbCrLf
            End If

            If Not isArchive And Not bSearch And Not GetBackendOption("Custom select statement") Then
                sSelectString &= "conditionals.Add(""COALESCE(MT.DELETED,0) = 0"")" & vbCrLf
            ElseIf isArchive Then
                sSelectString &= "conditionals.add(""COALESCE(MT.ARCHIVED,0) = 1"")" & vbCrLf
            End If

            Return sSelectString
        End Function

        Shared Function BackendCreationAllowed()
            Return GetCurrentProjectDT().Rows(0).Item("IncludeBackend") = "1" And createBackend
        End Function

        Shared Function GetHomepagePageName() as string
            return if(isancillaryproject And not IsSingletonProject(), "index" & getancillaryname, "index")
        End Function
    End Class
End Namespace

