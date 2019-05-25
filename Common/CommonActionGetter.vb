Imports System.Data
Imports System.Data.SqlClient
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
Imports Common.Webpages.Backend.Actions
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

Public Class CommonActionGetter
    Dim project As Project
    Dim shortProjectName As String
    Dim shortProjectParentName As String

    Public Sub New(ByRef project As Project)
        Me.project = project
        shortProjectName = project.GetProjectNameAlphaNumericOnly()
    End Sub

    Public Sub New(ByRef project As Project, ByVal shortProjectParentName As String)
        Me.New(project)
        Me.shortProjectParentName = shortProjectParentName
    End Sub

    Public Function GetActions() As String
        Dim actions As New StringBuilder()

        actions.Append($"#Region ""{shortProjectName} Actions""" & vbCrLf)
        actions.Append(GetUpdateValuesMethod())
        actions.Append(GetDeleteItemsMethod())
        actions.Append(GetCustomActionMethods())

        Dim actionItems, archiveActionItems, actionStatements As String

        Call New MaintenanceActionsCreator(project).GetMaintenanceActions(actionItems, archiveActionItems, actionStatements, "")

        actions.Append(GetActionsHandler(actionStatements))
        WriteActionItemsFile(shortProjectName & "ActionItems.htm", actionItems)
        WriteActionItemsFile(shortProjectName & "ArchiveActionItems.htm", archiveActionItems)

        actions.Append(GetExportMethods())
        actions.Append(GetUpdateArchiveStatusMethod())
        actions.Append("#End Region" & vbCrLf & vbCrLf)

        project.ProjectAncillaryMaintenances1.ToList().ForEach(Sub(ancillary)
                                                                   actions.Append(New CommonActionGetter(ancillary.Project, shortProjectName).GetActions())
                                                               End Sub)

        Return actions.ToString()
    End Function

    Private Function GetActionsHandler(ByVal actionStatements As String) As String
        Dim handler As New StringBuilder()

        handler.Append("Public Shared Sub Handle" & shortProjectName & "Action(ByRef submissions As Repeater, ByRef delete As Panel, ByVal selectedAction As String, afterCustomAction As AfterCustomActionDelegate)" & vbCrLf)
        handler.Append("Dim pnlDelete = If(delete, New Panel())" & vbCrLf & vbCrLf)
        handler.Append("pnlDelete.Visible = False" & vbCrLf & vbCrLf)

        handler.Append("If selectedAction = ""Delete"" Then" & vbCrLf)
        handler.Append("pnlDelete.Visible = True" & vbCrLf)
        handler.Append(actionStatements)
        handler.Append("End If" & vbCrLf)
        handler.Append("End Sub" & vbCrLf & vbCrLf)

        Return handler.ToString()
    End Function

    Sub WriteActionItemsFile(ByVal fileName As String, ByVal actionItems As String)
        'logger.Info(pageType & " - actionitems " & actionItems)

        Dim basedir As String = S_PROJECTFILESPATH & If(shortProjectParentName <> "", shortProjectParentName, shortProjectName) & "\Backend"

        WriteAllText(basedir & "\" & fileName, actionItems)
    End Sub

    Private Function GetUpdateArchiveStatusMethod() As String
        Dim actions As String = ""

        If GetBackendOption(S_BACKEND_OPTION_ARCHIVE_VIEW) Then
            actions &= $"Public Shared Sub Update{shortProjectName}ArchiveStatus(ByRef rptSubmissions As Repeater, ByVal status As String)" & vbCrLf
            actions &= $"Update{shortProjectName}Values(rptSubmissions,""Archived"",status)" & vbCrLf & vbCrLf
            actions &= "If status = ""0"" Then" & vbCrLf
            actions &= $"Update{shortProjectName}Values(rptSubmissions, ""Deleted"", ""0"")" & vbCrLf
            actions &= "End If" & vbCrLf
            actions &= "End Sub" & vbCrLf & vbCrLf
        End If

        Return actions
    End Function

    Public Function GetExportMethods() As String
        Dim exportMethods As String = ""

        Dim exportSelectStatement As String = "Select * FROM " & General.Variables.DT_WEBRAD_PROJECTBACKEND_EXPORTS & " WHERE Type = 'Export' AND ProjectID = " & project.ID
        Dim sFilename As String = shortProjectName

        project.ProjectBackendExports.Where(Function(export) export.Type = "Export").ToList().ForEach(
            Sub(export)
                Dim requireSelectedValues = export.RequireSelectedValues()

                exportMethods &= "Public Shared Sub Export" & export.ID & "Items(rptSubmissions)" & vbCrLf

                If requireSelectedValues Then
                    exportMethods &= "dim sSelectedIDs as string = GetListOfSelectedValues(rptSubmissions)" & vbCrLf & vbCrLf
                    exportMethods &= "If sSelectedIDs <> """"" & vbCrLf
                End If

                If export.FileType = "Excel" Then
                    exportMethods &= "Call New ExcelExporter(GetDataTable(""" & export.CreateExportSelectString() & """, Common.cnx), """ & sFilename & export.ID & ".xlsx"").Export()" & vbCrLf
                    exportMethods &= "Redirect(""" & sFilename & export.ID & ".xlsx"")" & vbCrLf
                Else
                    exportMethods &= "RunExport(GetDataTable(""" & export.CreateExportSelectString() & """, Common.cnx), """ & sFilename & export.ID & ".csv"","""",True, Nothing, Nothing, True)" & vbCrLf
                    exportMethods &= "Redirect(""" & sFilename & export.ID & ".csv"")" & vbCrLf
                End If

                If requireSelectedValues Then
                    exportMethods &= "End If" & vbCrLf
                End If

                exportMethods &= "End Sub" & vbCrLf & vbCrLf
            End Sub)

        Return exportMethods
    End Function

    Private Function GetCustomActionMethods() As String
        Dim actions As String = ""

        Dim backendOptions = project.ProjectBackendOptions.Where(Function(opt) opt.Type = 5).ToList()

        backendOptions.ForEach(
            Sub(opt)
                opt.ProjectBackendOptionColumns.ToList().ForEach(
                    Sub(column)
                        actions &= $"Public Shared Sub CustomAction" & column.ID & "(byref rptSubmissions as repeater)" & vbCrLf

                        If column.OperatorType = N_BACKENDUPDATEACTION_TYPE Then
                            Dim targetControl = project.ProjectControls.FirstOrDefault(Function(control) control.ID = column.ControlID)

                            actions &= $"Update{shortProjectName}Values(rptSubmissions, ""{targetControl.Name}"", CleanSQL(""" & column.ComparisonValue & """))" & vbCrLf
                        ElseIf column.OperatorType = N_BACKENDCUSTOMACTION_TYPE Then
                            actions &= column.CustomActionCode & vbCrLf
                        End If

                        actions &= "End Sub" & vbCrLf & vbCrLf
                    End Sub)
            End Sub)

        Return actions
    End Function

    Private Function GetDeleteItemsMethod() As String
        Dim actions As String = ""

        actions &= $"Public Shared Sub Delete{shortProjectName}Items(byref rptSubmissions As Repeater)" & vbCrLf
        actions &= $"Update{shortProjectName}Values(rptSubmissions,""Deleted"",""1"")" & vbCrLf
        actions &= "End Sub" & vbCrLf & vbCrLf

        Return actions
    End Function

    Private Function GetUpdateValuesMethod() As String
        Dim actions As String = ""

        actions &= $"Public Shared Sub Update{shortProjectName}Values(byref rptSubmissions As Repeater, byval type as string, byval value as string)" & vbCrLf
        actions &= "Dim sSelectedItems As String = GetListOfSelectedValues(rptSubmissions)" & vbCrLf & vbCrLf
        actions &= "If sSelectedItems <> """" Then" & vbCrLf
        actions &= "ExecuteNonQuery(""Update " & project.SQLMainTableName & " SET "" & type & "" = '"" & value & ""' WHERE ID IN ("" & sSelectedItems & "")"",cnx)" & vbCrLf
        actions &= "End If" & vbCrLf
        actions &= "End Sub" & vbCrLf & vbCrLf

        Return actions
    End Function
End Class
