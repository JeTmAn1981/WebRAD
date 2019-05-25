Imports Common.General
Imports Common.SQL
Imports WhitTools
Imports System.Data
Imports Microsoft.VisualBasic
Imports Common.General.Variables
Imports Common.General.Ancillary
Imports Common.General.ProjectOperations
Imports WhitTools.Getter
Imports WhitTools.Utilities

Public Class NavigationGenerator
    Private project As Project
    Private navigation As String
    Private ancillaryProject As ProjectAncillaryMaintenance

    Public Sub New(ByRef project As Project, Optional ancillaryProject As ProjectAncillaryMaintenance = Nothing)
        Me.project = project
        Me.ancillaryProject = ancillaryProject
    End Sub

    Public Function Generate() As String
        AddIndividualNavigationItems()

        Return navigation
    End Function

    Private Sub AddIndividualNavigationItems()
        AddProjectOptGroup()
        AddHomeNavigation()
        AddInsertNavigation()
        AddScheduleNavigation()
        AddSearchNavigation()
        AddReportNavigation()
        AddArchiveNavigation()
        AddAdditionalNavigationLinks()
        AddAncillaryNavigation()
    End Sub

    Private Sub AddProjectOptGroup()
        navigation &= $"<optgroup label=""{ GetProjectFullName() }""></optgroup>" & vbCrLf
    End Sub

    Private Sub AddHomeNavigation()
        navigation &= $"<option value=""index{project.GetNavigationName()}.aspx{GetSingletonIDReference()}"">Main Listing</option>" & vbCrLf
    End Sub

    Private Sub AddScheduleNavigation()
        If project.GetBackendOption(S_BACKEND_OPTION_SCHEDULE) Then
            navigation &= $"<option value='schedule{project.GetNavigationName()}.aspx'>Schedule</option>" & vbCrLf
        End If
    End Sub

    Private Sub AddInsertNavigation()
        If project.GetBackendOption(S_BACKEND_OPTION_INSERT_PAGE) Then
            navigation &= $"<option value='insert{project.GetNavigationName()}.aspx'>Insert New " & GetProjectFullName() & "</option>" & vbCrLf
        End If
    End Sub

    Private Sub AddSearchNavigation()
        If project.GetBackendOption(S_BACKEND_OPTION_SEARCH) Then
            navigation &= $"<option value='search{project.GetNavigationName()}.aspx'>Search</option>" & vbCrLf
        End If
    End Sub

    Private Sub AddReportNavigation()
        If project.GetBackendOption(S_BACKEND_OPTION_REPORT) Then
            navigation &= $"<option value='reports{project.GetProjectNameAlphaNumericOnly()}.aspx'>Reports</option>" & vbCrLf
        End If
    End Sub

    Private Sub AddArchiveNavigation()
        If project.GetBackendOption(S_BACKEND_OPTION_ARCHIVE_VIEW) Then
            navigation &= $"<option value='archive{project.GetNavigationName()}.aspx'>Archive View</option>" & vbCrLf
        End If
    End Sub

    Private Sub AddAncillaryNavigation()
        Dim ancillaryProjects = project.ProjectAncillaryMaintenances1.OrderBy(Function(ancillary) ancillary.ShortName).ToList()

        ancillaryProjects.ForEach(
            Sub(ancillary)
                navigation &= New NavigationGenerator(ancillary.Project, ancillary).Generate()
            End Sub)
    End Sub

    Private Sub AddAdditionalNavigationLinks()
        project.ProjectBackendAdditionalLinks.ToList().ToList().ForEach(
            Sub(link)
                navigation &= "<option value=""" & link.URL & """>" & link.Name & "</option>" & vbCrLf
            End Sub)
    End Sub

    Private Function GetSingletonIDReference() As String
        Return If(ancillaryProject IsNot Nothing AndAlso ancillaryProject.Singleton = "1", "?ID=1", "")
    End Function

    Sub AddLinkDelimiter(ByRef sLinks As String)
        sLinks &= If(sLinks <> "", "&nbsp;|&nbsp;", "")
    End Sub

    Private Function GetProjectFullName() As String
        If ancillaryProject IsNot Nothing Then
            Return ancillaryProject.ShortName
        Else
            Return project.PageTitle
        End If
    End Function


End Class
