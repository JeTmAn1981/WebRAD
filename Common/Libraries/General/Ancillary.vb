Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.Controls
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.ProjectOperations
Imports Common.General.DataTypes
Imports Common.General.Folders
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum


Imports WhitTools.Utilities
Namespace General

    Public Class Ancillary
        Shared Function IsAncillaryProject() As Boolean
            Return If(CStr(GetSessionVariable("ProjectID")) <> "", True, False)
        End Function

        Shared Function GetAncillaryName(Optional ByVal bIncludeSpace As Boolean = False) As String
            Return If(IsAncillaryProject(), GetFormattedProjectName(bIncludeSpace), "")
        End Function

        Public Shared Function GetFormattedProjectName(includeFormatting As Boolean) As Object
            Return GetFormattedProjectName(includeFormatting, GetCurrentPage().Session("ShortName"))
        End Function

        Public Shared Function GetFormattedProjectName(includeFormatting As Boolean, projectName As String) As Object
            If includeFormatting Then
                projectName = " " & projectName

                Return projectName
            Else
                Return Regex.Replace(If(projectName, ""), "[^A-Za-z0-9]", "")
            End If
        End Function

        Shared Function GetSaveAncillaryContent(ByVal sIdentity As String, ByRef sSaveAncillaryCalls As String, ByRef sSaveAncillaryMethods As String) As String
            Dim nCounter As Integer
            Dim sCurrentPrefix, sCurrentValue, sCurrentControlName, sCurrentControlType As String

            'Loop through top-level controls and check if they or any of their children need ancillary data saving
            For nCounter = 0 To controlsDT.Rows.Count - 1
                With controlsDT.Rows(nCounter)
                    Try
                        If NoParentControl(.Item("ParentControlID")) And ControlDisplayAllowed(.Item("DisplayLocation")) And BelongsToPage(pageNumber, .Item("PageID")) Then
                            CreateSaveAncillary(controlsDT.Rows(nCounter), sIdentity, sSaveAncillaryCalls, sSaveAncillaryMethods)
                        End If
                    Catch ex As Exception
                        logger.Error(ex.ToString)
                        'WriteLine(.Item("ParentControlID") & " - " & .Item("DisplayLocation") & " - " & .Item("PageID"))
                    End Try

                End With
            Next
        End Function

        Shared Sub CreateSaveAncillary(ByRef CurrentRow As DataRow, ByVal sIdentity As String, ByRef sSaveAncillaryCalls As String, ByRef sSaveAncillaryMethods As String)
            Dim sCurrentPrefix As String

            With CurrentRow
                sCurrentPrefix = .Item("Prefix")

                If ControlDisplayAllowed(.Item("DisplayLocation")) Then
                    If .Item("SQLInsertItemTable") <> "" Then
                        If Not ParentIsRepeaterControl(.Item("ID")) Then
                            If sCurrentPrefix = "rpt" Then
                                sSaveAncillaryCalls &= "Save" & .Item("Name") & "Items(" & sIdentity & ")" & vbCrLf
                                sSaveAncillaryMethods &= GetSaveAncillaryRepeaterMethod(.Item("ID"), sIdentity, .Item("Name"), .Item("SQLInsertItemStoredProcedure"), .Item("ForeignID"), False)
                            Else
                                sSaveAncillaryCalls &= "SaveAncillaryContent(""" & .Item("SQLInsertItemStoredProcedure") & """," & sIdentity & ", """ & sCurrentPrefix & """,""" & .Item("Name") & """,cnx,""" & .Item("ForeignID") & """)" & vbCrLf
                            End If
                        End If
                    ElseIf IsFileUploadControl(.Item("ControlType")) Then
                        If Not ParentIsRepeaterControl(.Item("ID")) Then
                            SaveAncillaryUploadFiles(CurrentRow, sSaveAncillaryCalls, sIdentity)
                        End If
                    End If

                    For Each ChildRow As DataRow In GetImmediateChildControls(.Item("ID")).Rows
                        CreateSaveAncillary(ChildRow, sIdentity, sSaveAncillaryCalls, sSaveAncillaryMethods)
                    Next
                End If
            End With
        End Sub

        Shared Sub SaveAncillaryUploadFiles(ByRef CurrentRow As DataRow, ByRef sSaveAncillary As String, ByVal sIdentity As String)
            Dim sCurrentControlName, sCurrentPrefix, sCurrentHiddenUploaded, sFrontendPath, sBackendPath As String

            With CurrentRow
                sCurrentPrefix = .Item("Prefix")
                sCurrentControlName = If(ParentIsRepeaterControl(.Item("ID")), "CType(.FindControl(""" & sCurrentPrefix & .Item("Name") & """)," & GetDataTypeDescription(.Item("DataType")) & ")", sCurrentPrefix & .Item("Name"))
                sCurrentHiddenUploaded = If(ParentIsRepeaterControl(.Item("ID")), "CType(.FindControl(""lblHiddenUploaded" & .Item("Name") & """),Label)", "lblHiddenUploaded" & .Item("Name"))

                sSaveAncillary &= "If cmd.Parameters(""@" & .Item("Name") & """).Value <> """" Then" & vbCrLf

                sFrontendPath = projectDT.Rows(0).Item("FrontendPath") & "\UploadedFiles\"" & " & sIdentity
                sBackendPath = projectDT.Rows(0).Item("BackendPath") & "\UploadedFiles\"" & " & sIdentity

                If .Item("UploadPath") <> "" And .Item("UploadPath") <> "None" Then
                    sFrontendPath = .Item("UploadPath")

                    If .Item("UploadPathNewFolder") <> "" Then
                        sFrontendPath &= "\" & Replace(.Item("UploadPathNewFolder"), "/", "\")
                    End If

                    sFrontendPath &= "\UploadedFiles\"" & " & sIdentity
                End If

                'Bug here, not correctly taking into account custom paths for uploaded files!
                WriteLine(sFrontendPath)
                WriteLine(sBackendPath)


                sSaveAncillary &= "CopyNetworkFiles(""LocalPath\UploadedFiles\~Temp\"" & Session.SessionID, """ & sFrontendPath & ", cmd.Parameters(""@" & .Item("Name") & """).Value)" & vbCrLf

                If Not isFrontend Then
                    sSaveAncillary &= "CopyNetworkFiles(""LocalPath\UploadedFiles\"" & nCurrentID, """ & sFrontendPath & ", cmd.Parameters(""@" & .Item("Name") & """).Value)" & vbCrLf
                End If

                sSaveAncillary &= "End If" & vbCrLf & vbCrLf
            End With
        End Sub

        Shared Function GetAncillaryProject(ByVal sColumnName As String) As String
            Dim currentProjectDT = GetCurrentProjectDT()

            If currentProjectDT IsNot Nothing AndAlso currentProjectDT.Rows.Count > 0 Then
                Return GetCurrentProjectDT().Rows(0).Item(sColumnName)
            End If

            Return ""
        End Function

        Shared Function GetRootProjectTitle() As String
            '''Potential bug: this code is set up under the assumption that ancillary maintenance projects will not be nested
            Try
                If IsAncillaryProject() Then
                    Dim currentProjectID = GetProjectID()

                    Dim parentProject As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTS & " WHERE ID IN (SELECT ProjectID FROM " & DT_WEBRAD_PROJECTANCILLARYMAINTENANCE & " WHERE AncillaryProjectID = " & GetProjectID() & ")", cnx)

                    Return parentProject.Rows(0).Item("PageTitle")
                    'return db.Projects.Where(Function (p) p.ID = db.ProjectAncillaryMaintenances.Where(Function (am) am.AncillaryProjectID  = currentProjectID).SingleOrDefault().ProjectID).SingleOrDefault().pagetitle
                Else
                    Return currentProject.PageTitle
                End If
            Catch ex As Exception
                'WriteLine("problem getting root project title")
                'Logger.Error(ex.ToString)
            End Try
            

            Return GetAncillaryProject("PageTitle")
        End function

        Shared function IsSingletonProject() as boolean
            return (getsessionvariable("Singleton") = "1")
        End Function

        Shared Function GetBackendUpdateReference(Optional ByVal bTemplate As Boolean = False) As String
            If IsSingletonProject() Then
                Return If(btemplate, "SingletonIndex", "Index")
            Else
                Return "Update"
            End If
        End Function

        Shared Function GetAncillaryProjects(ByVal projectID As Integer) As datatable
            Return GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTANCILLARYMAINTENANCE & " WHERE ProjectID = " & projectID & " AND ID IN (Select AncillaryMaintenanceID FROM " & DT_WEBRAD_PROJECTBUILDBACKENDANCILLARYMAINTENANCE & " WHERE BuildID = (SELECT MAX(ID) FROM " & DT_WEBRAD_PROJECTBUILDS & " WHERE ProjectID = " & projectID & "))")
        End Function
    End Class
End Namespace
