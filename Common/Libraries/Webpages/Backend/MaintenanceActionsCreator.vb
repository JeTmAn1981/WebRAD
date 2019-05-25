Imports Common.General
Imports System.Data
Imports Common.General.ProjectOperations
Imports Common.General.Variables
Imports WhitTools.DataTables
Imports WhitTools.Getter
Imports Common.General.Folders


Namespace Webpages.Backend
    Public Class MaintenanceActionsCreator
        Dim project As Project
        Dim projectName, extensionName As String
        Dim items As New StringBuilder()
        Dim archiveItems As New StringBuilder()
        Dim statements As New StringBuilder()

        Public Sub New(ByRef project As Project)
            Me.project = project
            projectName = project.GetProjectNameAlphaNumericOnly()
            extensionName = removenonalphanumeric(project.GetAncillaryShortname(GetQueryString("ID")))
        End Sub

        Public Sub GetMaintenanceActions(ByRef actionItems As String, ByRef archiveActionItems As String, ByRef actionStatements As String, ByVal pageType As String)
            AddReportsActions()
            AddCustomActions()

            If project.UsesBackendOption(S_BACKEND_OPTION_ARCHIVE_VIEW) Then
                AddArchiveActions()
                AddUnArchiveAction()
            End If

            AddExportActions(pageType)

            actionItems = items.ToString()
            archiveActionItems = archiveItems.ToString()
            actionStatements = statements.ToString()
        End Sub

        Private Sub AddExportActions(ByVal pageType As String)
            If project.UsesBackendOption(S_BACKEND_OPTION_EXPORT) Then
                CreateExportActions(pageType)
            End If
        End Sub


        Private Sub AddCustomActions()
            Dim dtActions As DataTable = GetDataTable("select OC.*, AT.Description from " & General.Variables.DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  OC left outer join " & General.Variables.DT_WEBRAD_BACKENDACTIONTYPES & "  AT on OC.OperatorType = AT.ID where optionid in (select ID from " & General.Variables.DT_WEBRAD_PROJECTBACKENDOPTIONS & "  WHERE ProjectID = " & project.ID & " and Type = 5)", Common.General.Variables.cnx)

            For Each Currentrow As DataRow In dtActions.Rows
                items.Append("<asp:listitem value=""CustomAction" & Currentrow.Item("ID") & """>" & Currentrow.Item("Label") & "</asp:listitem>" & vbCrLf)

                statements.Append("ElseIf selectedAction = ""CustomAction" & Currentrow.Item("ID") & """ Then" & vbCrLf)
                statements.Append("CustomAction" & Currentrow.Item("ID") & "(submissions)" & vbCrLf)

                statements.Append("afterCustomAction()" & vbCrLf)
            Next
        End Sub

        Private Sub AddArchiveActions()
            If project.UsesBackendOption(S_BACKEND_OPTION_ARCHIVE_VIEW) Then
                items.Append("<asp:listitem value=""Archive"">Archive Selected Items</asp:listitem>" & vbCrLf)
                items.Append("<asp:listitem value=""ArchiveDelete"">Archive and Delete Selected Items</asp:listitem>" & vbCrLf)

                statements.Append("ElseIf selectedAction = ""Archive"" Then" & vbCrLf)
                statements.Append($"Update{projectName}ArchiveStatus(submissions, 1)" & vbCrLf)
                statements.Append("Redirect(""archive" & extensionName & ".aspx"")" & vbCrLf)
                statements.Append("ElseIf selectedAction = ""ArchiveDelete"" Then" & vbCrLf)
                statements.Append($"Update{projectName}ArchiveStatus(submissions, 1)" & vbCrLf)
                statements.Append($"Delete{projectName}Items(submissions)" & vbCrLf)
                statements.Append("Redirect(""archive" & extensionName & ".aspx"")" & vbCrLf)
            End If
        End Sub

        Private Sub AddUnArchiveAction()
            archiveItems.Append("<asp:listitem value=""Unarchive"">Unarchive Selected Items</asp:listitem>" & vbCrLf)

            statements.Append("ElseIf selectedAction = ""Unarchive"" Then" & vbCrLf)
            statements.Append($"Update{projectName}ArchiveStatus(submissions, 0)" & vbCrLf)
            statements.Append("Redirect(""index" & extensionName & ".aspx"")" & vbCrLf)
        End Sub

        Private Sub AddReportsActions()
            If General.Variables.isSearch And project.UsesBackendOption(S_BACKEND_OPTION_REPORT) Then
                project.GetReports().ToList().ForEach(
                    Sub(report)
                        With report
                            items.Append("<asp:listitem value=""Report" & .ID & """>Export Selected Items to Report - " & .Name & "</asp:listitem>" & vbCrLf)

                            statements.Append("ElseIf selectedAction = ""Report" & .ID & """ Then" & vbCrLf)

                            If .DataSourceType = "1" Then
                                statements.Append("dim sSelectedIDs as string = GetListOfSelectedValues(submissions)" & vbCrLf & vbCrLf)
                                statements.Append("If sSelectedIDs <> """"" & vbCrLf)
                            End If

                            statements.Append("SetSessionVariable(""ReportSelectString"", """ & .CreateExportSelectString("Report") & """)" & vbCrLf)
                            statements.Append($"Redirect(""reports{projectName}.aspx?ID=" & .ID & """)" & vbCrLf)

                            If .DataSourceType = "1" Then
                                statements.Append("End If" & vbCrLf)
                            End If
                        End With
                    End Sub)
            End If
        End Sub

        Private Sub CreateExportActions(ByVal pageType As String)
            project.ProjectBackendExports.Where(Function(export) export.Type = "Export").ToList().ForEach(
                Sub(export)
                    With export
                        If Not (pageType.Contains("Archive") And .DataSourceType = "2") Then
                            items.Append("<asp:listitem value=""Export" & .ID & """>Export Selected Items to Excel - " & .Name & "</asp:listitem>" & vbCrLf)

                            statements.Append("ElseIf selectedAction = ""Export" & .ID & """ Then" & vbCrLf)
                            statements.Append("Export" & .ID & "Items(submissions)" & vbCrLf)
                        End If
                    End With
                End Sub)
        End Sub
    End Class
End Namespace
