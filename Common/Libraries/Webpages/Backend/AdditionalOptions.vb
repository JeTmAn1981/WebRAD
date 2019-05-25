Imports Common.General
Imports WhitTools
Imports System.Data
Imports Common.General.Main
Imports Common.Webpages.Backend.Main
Imports Common.General.Variables
Imports common.general.Ancillary
Imports WhitTools.Utilities

Namespace Webpages.Backend
    Public Class AdditionalOptions
         public Shared Sub CreateAdditionalBackendOptions()
             WriteInsertPage()
             WriteArchiveView()
             WriteSchedulePage()
             WriteSearchPage()
             WriteReportPage()
             WriteAncillaryMaintenance()
         End Sub

         public Shared Sub WriteAncillaryMaintenance()
             If backendoptionbuildallowed(S_BACKEND_OPTION_ANCILLARY_MAINTENANCE) Then
                 CreateAncillaryMaintenance()
             End If
         End Sub

         Private Shared Sub WriteReportPage()
            If backendoptionbuildallowed(S_BACKEND_OPTION_REPORT) Then
                UpdateProgress("Creating backend reports", 1)
                Call New ReportPageWriter(currentProject).WritePage()
            End If
        End Sub

         Private Shared Sub WriteSearchPage()
             If backendoptionbuildallowed(S_BACKEND_OPTION_SEARCH) Then
                 UpdateProgress("Creating backend search", 1)
                 isSearch = True
                 Search.WriteSearchPage()
                 isSearch = False
             End If
         End Sub

         Private Shared Sub WriteSchedulePage()
             If backendoptionbuildallowed(S_BACKEND_OPTION_SCHEDULE) Then
                 UpdateProgress("Creating backend schedule", 1)
                 Schedule.WriteSchedulePage()
             End If
         End Sub

         Private Shared Sub WriteArchiveView()
             If backendoptionbuildallowed(S_BACKEND_OPTION_ARCHIVE_VIEW) Then
                 UpdateProgress("Creating backend archive view", 1)
                isPrintable = True
                General.Variables.isArchive = True
                'sArchiveRef = "Archive_"
                UpdateProgress("Creating backend archive index", 2)
                 WriteArchiveIndexPage()
                'UpdateProgress("Creating backend archive update", 2)
                'call new archivepagewriter().writepage()
                'Printable.WritePrintablePage("ViewArchive", "View Archived")
                'sArchiveRef = ""
                General.Variables.isArchive = False
                isPrintable = False
                 UpdateProgress("Finished with archive", 1)
             End If
         End Sub

        Private Shared Sub WriteInsertPage()
             If backendoptionbuildallowed(S_BACKEND_OPTION_INSERT_PAGE) Then
                 UpdateProgress("Creating backend insert", 1)
                 isInsert = True
                isFrontend = True
                Frontend.Main.WriteIndexPage()
                isFrontend = False
                isInsert = False
             End If
         End Sub

         Private Shared Sub WriteArchiveIndexPage()
             call New BackendIndexPageWriter("Archive", "Archive").WritePage()
         End Sub

         Shared Sub CreateAncillaryMaintenance()
            Dim dtAncillary As DataTable = GetAncillaryProjects(GetAncillaryProject("ID"))

            For Each Currentrow As DataRow In dtAncillary.Rows
                 Setter.SetSessionVariable("ProjectID",Currentrow.Item("AncillaryProjectID"))
                 Setter.SetSessionVariable("ShortName",Currentrow.Item("ShortName"))
                 Setter.SetSessionVariable("Singleton", currentrow.Item("Singleton"))

                 ancillaryProjectDT = DataTables.GetDataTable("Select P.*,  DB.Name as SQLDBName, S.Name as SQLServerName From " & DT_WEBRAD_PROJECTS & "  P left outer join " & DT_WEBRAD_SQLDATABASES & "  DB on P.SQLDatabase = DB.ID left outer join " & DT_WEBRAD_SQLSERVERS & " S on DB.ServerID = S.ID Where P.ID = " & ProjectOperations.GetProjectID(), False)
                 DataTables.EliminateNull(ancillaryProjectDT)
                 ancillaryProjectDT = DataTables.ConvertDataTableColumnTypes(ancillaryProjectDT)

                 Getter.GetCurrentPage().Session("AncillaryProjectDT") = ancillaryProjectDT

                 UpdateProgress("Creating ancillary maintenance - " & Getter.GetCurrentPage().Session("ShortName"), 2)
                 BuildSetup.GetTopLevelControls()

                 Main.WriteBackendPages()

                 SQL.Main.ProcessSQL()
             Next

             Setter.SetSessionVariable("ProjectID","")
             Setter.SetSessionVariable("ShortName","")
             Setter.SetSessionVariable("Singleton","") 

             BuildSetup.GetTopLevelControls()
         End Sub
    End Class
End NameSpace