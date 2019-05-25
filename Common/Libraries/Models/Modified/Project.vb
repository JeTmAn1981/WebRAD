Imports WhitTools.Getter
Imports Common.General.Variables
Imports Common.General.Ancillary
Imports WhitTools.Formatter
Imports Common.General.Folders
Imports Common.SQL.Main

Partial Public Class Project
    Public Function GetNavigation(Optional ancillaryProject As ProjectAncillaryMaintenance = Nothing) As String
        Return New NavigationGenerator(Me, ancillaryProject).Generate()
    End Function

    Public Function IsAncillaryProject() As Boolean
        Return ID <> GetQueryString("ID")
    End Function

    Public Function GetAncillaryShortname(ByVal parentProjectID As Integer) As String
        If IsAncillaryProject() Then
            Dim ancillaryMaintenance = db.ProjectAncillaryMaintenances.FirstOrDefault(Function(ancillary) ancillary.AncillaryProjectID = ID And ancillary.ProjectID = parentProjectID)

            If ancillaryMaintenance IsNot Nothing Then
                Return ancillaryMaintenance.ShortName
            End If
        End If

        Return ""
    End Function



    Public Function GetBackendOption(ByVal sOptionName As String) As Boolean
        If sOptionName = S_BACKEND_OPTION_MAINLISTING Or sOptionName = S_BACKEND_OPTION_UPDATE Then
            Return True
        End If

        Return ProjectBackendOptions.Any(Function(backendOption) backendOption.BackendOptionType.Name = sOptionName)
    End Function


    Public Function GetNavigationName() As String
        If Not IsAncillaryProject() Then
            Return ""
        Else
            Return GetFormattedProjectName(False, GetProjectFullName())
        End If
    End Function

    'Potential bug here: when getting the short name, we're grabbing the short name for the first
    'ancillary maintenance record in which this project appears.  So it's possible there could
    'be multiple short names to choose from if the project is included as an ancillary project
    'in more than one main project.  Pretty unlikely this will be an issue.  It would be 
    'better to have the short name only assigned once at the project level but I don't want to
    'mandate that if it's not needed for most projects.
    Private Function GetProjectFullName() As String
        If Not IsAncillaryProject() Then
            Return PageTitle
        Else
            Return GetShortName()
        End If
    End Function

    Private Function GetShortName() As String
        Dim ancillaryProject = New WebRADEntities().ProjectAncillaryMaintenances.FirstOrDefault(Function(ancillary) ancillary.AncillaryProjectID = ID)

        If ancillaryProject IsNot Nothing Then
            Return ancillaryProject.ShortName
        Else
            Return PageTitle
        End If
    End Function

    Public Function GetInsertLink() As String
        If GetBackendOption(S_BACKEND_OPTION_INSERT_PAGE) Then
            Return $"<li><a class=""button"" href=""insert{GetNavigationName()}.aspx"">Insert New " & GetProjectFullName() & "</a></li>" & vbCrLf
        End If

        Return ""
    End Function

    Public Function GetCommonActions() As String
        Return New CommonActionGetter(Me).GetActions()
    End Function

    Public Function GetProjectNameAlphaNumericOnly() As String
        Return RemoveNonAlphanumeric(PageTitle)
    End Function

    Public Function GetReports() As IEnumerable(Of ProjectBackendExport)
        Return ProjectBackendExports.Where(Function(export) export.Type = "Report")
    End Function

    Function GetFullDatabaseTableName() As String
        Return GetSQLDatabaseName(SQLDatabase) & ".dbo." & archiveRef & SQLMainTableName
    End Function

    Function GetFullDatabaseTableNameWithServer() As String
        Return SQLServerName & "." & GetFullDatabaseTableName()
    End Function

    Public Function UsesBackendOption(ByVal optionName As String) As Boolean
        Return ProjectBackendOptions.Any(Function(backendOption) backendOption.BackendOptionType.Name = optionName)
    End Function
End Class