Imports Common.General.Variables
Imports Common.General.Folders
Imports WhitTools.Getter
Imports WhitTools.DataTables
Imports WhitTools.Utilities
Imports Common.Webpages.Frontend.MultiPage
Imports Common.General.Ancillary
Imports System.Data
Imports System.Web.UI.WebControls.Expressions
Imports Common.Webpages.Backend.Main
Imports Common.General.Pages

Namespace General
    Public Class ProjectOperations
        Shared Sub SetCurrentProject(ByVal projectID As string)
            if projectid <> ""
                currentProject = (From p As Project In db.Projects Where p.ID = projectID Select p).ToList().DefaultIfEmpty(Nothing)(0)
            End If
            End Sub

        Shared Sub SetProjectDetails()
            Dim projectID = currentProject.ID
            projectDT = GetDataTable(cnx, "Select P.*,  DB.Name as SQLDBName, S.Name as SQLServerName From " & DT_WEBRAD_PROJECTS & "  P left outer join " & DT_WEBRAD_SQLDATABASES & "  DB on P.SQLDatabase = DB.ID left outer join " & DT_WEBRAD_SQLSERVERS & " S on DB.ServerID = S.ID Where P.ID = " & projectID)
            projectDT = ConvertDataTableColumnTypes(projectDT)
            EliminateNull(projectDT)

            projectControls = (from pc as ProjectControl in db.ProjectControls
                                  select pc
                                  where pc.ProjectID = projectID).OrderBy(function (pc) pc.PageID).OrderBy(function (pc) pc.Position).ToList()
        End Sub

        Shared function GetCurrentPageID() As integer
            return getquerystring("PageID")
        End function
        
        Shared Function ProjectDetailsComplete(ByVal sProjectID As String) As Boolean
            If sProjectID <> "" Then
                Try
                    If GetDataTable("Select * From " & DT_WEBRAD_PROJECTS & "  Where FrontendDetailsComplete = 1 and BackendDetailsComplete = 1 AND ID = '" & sProjectID & "'", cnx).Rows.Count > 0 Then
                        Return True
                    End If
                Catch ex As Exception
                    'Empty catch statement
                End Try
            End If
            Return False
        End Function

        Shared Function GetProjectID() As String
            ''Todo: Update this to check calling assembly and return currentProject object if Experimental
            '    Return If(GetSessionVariable("ProjectID") <> "", GetSessionVariable("ProjectID"), GetProjectID())
            Dim projectID As Integer

            Integer.TryParse(If(GetSessionVariable("ProjectID") <> "", GetSessionVariable("ProjectID"), If(GetQueryString("ID") <> "", GetQueryString("ID"), 0)), projectID)
            SetCurrentProject(projectID)

            If projectID = 0 Then
                Return -1
            Else
                SetProjectDetails()

                Return currentProject.ID
            End If

        End Function

        Shared Function GetCurrentProjectDT() As DataTable
            Return If(IsAncillaryProject(), CType(GetCurrentPage().Session("AncillaryProjectDT"), DataTable), projectDT)
        End Function


        Shared Function CurrentProjectRequiresLogin() As Boolean
            Return CurrentProjectRequiresWhitworthLogin() Or CurrentProjectRequiresNonWhitworthLogin()
        End Function

        Shared Function CurrentProjectRequiresWhitworthLogin() As Boolean
            Return currentProject.RequireLogin = "1"
        End Function

        Public Shared Function CurrentProjectRequiresNonWhitworthLogin() As Boolean
            Return IsMultipageForm() And Not CurrentProjectRequiresWhitworthLogin()
        End Function


        Shared Function IseCommerceProject() As Boolean
            Return GetProjectOption("Ecommerce")
        End Function

        Public Shared Function GetProjectOption(ByVal optionValue As string) As Boolean
            Dim projectID As Integer = GetProjectID()

            Return db.ProjectOptions.Where(Function (currentOption) currentOption.ProjectID = projectID And currentOption.Value = optionValue).Count > 0 or (GetAncillaryProject(optionValue) = "1")
        End Function

        Public Shared Function GetProjectPageLimit() As Object
            Return if(Webpages.Backend.Main.getbackendoption(S_BACKEND_OPTION_PAGING),GetCurrentProjectDT.Rows(0).Item("PageLimit"),0)
        End Function

        public shared Function ProjectIncludesProspectUserControl() As boolean
            dim projectID as integer = currentproject.ID

            return db.ProjectControls.Where(Function(control) control.projectid = projectid And control.ControlType = N_PROSPECTGATHERING_USERCONTROL_CONTROL_TYPE).Count > 0
        End Function

     Shared Sub CheckProjectParameters(Optional ByVal pageType As string = "")
        CheckPageID(CheckProjectID(pageType),pageType)
     End Sub

     Private shared Sub CheckPageID(projectID As Integer, byval pageType As string)
        dim pageID as Integer

         try
             pageID = getquerystring("PageID")
         Catch ex As Exception

         End Try 

         if getdatatable("SELECT * FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & projectID & " AND ID = " & pageID).Rows.Count = 0
             pageID = GetFirstPage(projectid)

             redirect("controls" & pagetype & ".aspx?ID=" & projectid & "&PageID=" & pageID)
         End If
     End Sub

     Private shared Function CheckProjectID(byval pageType As string) As Integer
        dim projectID as integer

         try
             projectID = GetProjectID()
         Catch ex As Exception

         End Try

         if getdatatable("SELECT * FROM " & dt_webrad_projects & " WHERE ID = " & projectID).Rows.Count = 0
             redirect("index" & pagetype & ".aspx")
         End If

         Return projectID
     End Function

    End Class
End Namespace