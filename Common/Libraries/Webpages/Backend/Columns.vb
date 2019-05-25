Imports System.Data
Imports Common.General
Imports Common.SQL
Imports Common.Webpages.ControlContent
Imports WhitTools
Imports Common.General.Main
Imports Common.General.controltypes
Imports Common.General.Variables
Imports Common.general.ProjectOperations
Imports Common.General.Pages

Namespace Webpages.Backend
    Public Class Columns
         Shared Sub GetBackendColumns(ByRef sDisplayColumns As String, ByRef sDataColumns As String, ByRef sJoins As String, ByRef sAdditionalSelectColumns As String)
             Dim sSearchQuery, sHeading, sValue, columnType As String

             columnType = "th"

            sSearchQuery = "Select OC.ControlID, PC.ControlType, PC.DataSourceID,CASE WHEN PC.ShortHeading <> '' THEN PC.ShortHeading ELSE PC.Heading END Heading, PC.Name, [Type], [Table], ValueField, TextField, PC.SQLDataType,PC.SQLInsertItemTable, PC.ForeignID From " & General.Variables.DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  OC left outer join " & General.Variables.DT_WEBRAD_PROJECTCONTROLS & " PC on OC.ControlID = PC.ID LEFT OUTER JOIN " & General.Variables.DT_WEBRAD_PROJECTDATASOURCES & " DS ON DS.ID = PC.DataSourceID Where OptionID in "

            If General.Variables.isSearch Then
                 sSearchQuery &= " (Select Max(ID) From " & General.Variables.DT_WEBRAD_PROJECTBACKENDOPTIONS & "  Where ProjectID = " & ProjectOperations.GetProjectID() & " and Type = 10) order by OC.ID asc"
             Else
                 sSearchQuery &= " (Select ID From " & General.Variables.DT_WEBRAD_PROJECTBACKENDOPTIONS & "  Where ProjectID = " & ProjectOperations.GetProjectID() & " and Type = 8) order by OC.ID asc"
             End If

             Dim dtDisplayControls As DataTable = DataTables.GetDataTable(sSearchQuery, General.Variables.cnx)
             Dim loginColumn As LoginColumnType

             For nCounter As Integer = 0 To dtDisplayControls.Rows.Count - 1
                 With dtDisplayControls.Rows(nCounter)
                    loginColumn =  (from lc As LoginColumnType in General.Main.LoginColumnTypes
                         where lc.ID = .Item("ControlID")
                         select lc).DefaultIfEmpty(Nothing).First()

                     If loginColumn Is Nothing Then
                         sHeading = .Item("Heading")

                        If .Item("Name") = "DateSubmitted" Then
                            sValue = "<%# Container.DataItem(""" & GetDateSubmittedColumnReference() & """) %>"
                        ElseIf IsYesNoControl(.Item("ControlType")) Then
                            sValue = "<%# WhitTools.Formatter.FormatYesNo(Container.Dataitem(""" & .Item("Name") & """)) %>"
                        ElseIf IsDateControl(.Item("ControlType"), .Item("SQLDataType")) Then
                            sValue = "<%# WhitTools.Formatter.FormatShortDate(Container.Dataitem(""" & .Item("Name") & """)) %>"
                        ElseIf .Item("SQLInsertItemTable") <> "" Then
                            sValue = "<%# WhitTools.Getter.GetListofValues(""Select * FROM " & .Item("SQLInsertItemTable") & " WHERE " & .Item("ForeignID") & " = "" & Container.Dataitem(""ID""), """ & .Item("Name") & """, "","", """", Common.cnx) %>"
                        ElseIf Controls.ControlDisplayRequiresJoin(dtDisplayControls.Rows(nCounter)) Then
                            sValue = "<%# Container.Dataitem(""" & .Item("Name") & "Text"") %>"
                            sAdditionalSelectColumns &= "," & .Item("Table") & ".[" & .Item("TextField") & "] AS " & .Item("Name") & "Text"
                            sJoins &= " LEFT OUTER JOIN " & .Item("Table") & " ON " & .Item("Table") & ".[" & .Item("ValueField") & "] = MT.[" & .Item("Name") & "]"
                        Else
                            sValue = "<%# Container.Dataitem(""" & .Item("Name") & """) %>"
                         End If
                     Else
                         sHeading = loginColumn.DisplayName
                        sValue = If(loginColumn.BackendDisplayValue = "", "<%# Container.Dataitem(""" & If(loginColumn.ColumnName = "DateSubmitted", GetDateSubmittedColumnReference(), loginColumn.ColumnName) & """) %>", loginColumn.BackendDisplayValue)
                    End If

                        sDisplayColumns &= "<" & columnType & "  data-tablesaw-priority=""#"" align=""center"" valign=""top"">" & sHeading & "</" & columnType & ">" & vbCrLf
                        sDataColumns &= "<td align=""center"" valign=""top"">" & sValue & "</td>" & vbCrLf
                 End With
             Next

             AddEcommerceColumns(sDisplayColumns, columnType, sDataColumns)
             AddWorkflowColumns(sDisplayColumns, columnType, sDataColumns)
         End Sub

         Private Shared Sub AddWorkflowColumns(ByRef sDisplayColumns As String, columnType As String, ByRef sDataColumns As String)
            If isWorkflow And WhitTools.Workflow.IsFirstWorkflowStep(Ancillary.GetAncillaryProject("WorkflowStep")) And islastpage() Then
                AddDisplayHeading(sDisplayColumns, columnType, "Workflow Finished?")
                sDataColumns &= "<td align=""center"" valign=""top""><%# WhitTools.Datatables.GetDataTable(""SELECT CASE WHEN "" & Container.DataItem(""ID"") & "" IN (Select RecordID FROM adInformationSystems.dbo.WorkflowProcessSteps Where TableReference = '" & currentProject.GetFullDatabaseTableName() & "' AND ProcessID IN (SELECT ProcessID From adInformationSystems.dbo.WorkflowProcessStepRequests WHERE DateCompleted IS NULL)) THEN 'N' else 'Y' END WorkflowComplete"").Rows(0).Item(""WorkflowComplete"") %></td>" & vbCrLf
            End If
        End Sub

         Private Shared Sub AddDisplayHeading(ByRef displayColumns As String, columnType As String, byval heading As string)
            displayColumns &= "<" & columnType & "  data-tablesaw-priority=""#"" align=""center"" valign=""top"">"
            displayColumns &= heading
             displayColumns &= "</" & columnType & ">" & vbCrLf
         End Sub

         Private Shared Sub AddEcommerceColumns(ByRef sDisplayColumns As String, columnType As String, ByRef sDataColumns As String)
            If ProjectOperations.IseCommerceProject() Then
                 AddDisplayHeading(sDisplayColumns, columnType,"Amount Owed")
                 sDataColumns &= "<td align=""center"" valign=""top""><%# WhitTools.EcommerceStatusChecks.GetInvoiceAmountAndPaymentStatus(container.dataitem(""Invoice"")) %></td>" & vbCrLf
             End If
         End Sub
    End Class
End NameSpace