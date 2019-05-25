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

Imports WhitTools.DataTables
Imports WhitTools.Converter
Imports WhitTools.Validator
Imports WhitTools.Formatter
Imports WhitTools.GlobalEnum
Imports WhitTools.Repeaters
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.WebTeam
Imports WhitTools.SQL
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
Imports Common.General.Controls
Imports Common.General.Folders
Imports Common.General.DataTypes
Imports Common.General.Pages
Imports Common.SQL.Main
Imports Common.SQL.Table
Imports Common.BuildSetup
Imports Common.Webpages.BindData
Imports Common.Webpages.Frontend.Main
Imports Common.Webpages.Frontend.MultiPage
Imports Common.Webpages.Backend.Main
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.Backend.Search
Imports System.Threading
Imports System.Reflection
Imports Common.general.projectoperations

Imports WhitTools.Utilities
Namespace Webpages
    Public Class Workflow
        Inherits Backend.Main

        Shared Function GetWorkflowSteps() As String
            If isWorkflow And IsLastPage() And IsFirstWorkflowStep(GetAncillaryProject("WorkflowStep")) Then
                Dim sWorkflowSteps As String

                sWorkflowSteps &= "<br /><br />" & vbCrLf
                sWorkflowSteps &= "<div class='SubHeaders'>Workflow Steps</div>" & vbCrLf
                sWorkflowSteps &= "<br />" & vbCrLf
                sWorkflowSteps &= "<table>" & vbCrLf
                sWorkflowSteps &= "<tr>" & vbCrLf
                sWorkflowSteps &= "<td align='middle'><strong>Step</strong></td>" & vbCrLf
                sWorkflowSteps &= "<td align='middle'><strong>Submit Link</strong></td>" & vbCrLf
                sWorkflowSteps &= "<td align='middle'><strong>Submitted</strong></td>" & vbCrLf
                sWorkflowSteps &= "<td align='middle'><strong>Completed</strong></td>" & vbCrLf
                sWorkflowSteps &= "</tr>" & vbCrLf
                sWorkflowSteps &= "<asp:Repeater ID=""rptWorkflowSteps"" runat=""server"">" & vbCrLf
                sWorkflowSteps &= "<ItemTemplate>" & vbCrLf
                sWorkflowSteps &= "<tr>" & vbCrLf
                sWorkflowSteps &= "<td><a href=""<%# IIF(Container.DataItem(""RecordID"") <> ""-1"", Replace(Container.DataItem(""BackendURL""),""index.aspx"",""update.aspx""),Container.DataItem(""BackendURL"")) %>?ID=<%# Container.DataItem(""RecordID"") %>"" target=""_blank""><%# Container.DataItem(""Name"") %></a></td>" & vbCrLf
                sWorkflowSteps &= "<td align=""middle""><a href='<%# Container.DataItem(""RequestedStepPositionFrontendURL"")%>?<%# WhitTools.GlobalEnum.S_WORKFLOW_STEP_ID%>=<%# WhitTools.Encryption.EncryptExternal(Container.DataItem(""StepID""))%>'>Submit</a></td>" & vbCrLf
                sWorkflowSteps &= "<td align='middle'><%# Container.DataItem(""RequestSubmitted"") %>&nbsp;(<%# Container.DataItem(""SubmittedUser"") %>)</td>" & vbCrLf
                sWorkflowSteps &= "<td align='middle'><%# Container.DataItem(""RequestCompleted"") %>&nbsp;<%# If(Container.DataItem(""CompletedUser"") <> """",""("" & Container.DataItem(""CompletedUser"") & "")"","""") %></td>" & vbCrLf
                sWorkflowSteps &= "</tr>" & vbCrLf
                sWorkflowSteps &= "</ItemTemplate>" & vbCrLf
                sWorkflowSteps &= "</asp:Repeater>" & vbCrLf
                sWorkflowSteps &= "</table>" & vbCrLf

                Return sWorkflowSteps
            Else
                Return ""
            End If
        End Function

        Shared Function GetBindWorkflowSteps() As String
            If isWorkflow And IsFirstWorkflowStep(GetAncillaryProject("WorkflowStep")) And IsLastPage() Then
                Dim sBindWorkflowSteps As String

                sBindWorkflowSteps = "SelectRepeaterData(rptWorkflowSteps, GetDataTable(""select * from adInformationSystems.dbo.Workflowsteps_v where processid=(SELECT TOP 1 ProcessID FROM adInformationSystems.dbo.WorkflowProcessSteps WHERE RecordID = "" & GetQueryString(""ID"") & "" AND TableReference = '" & currentProject.GetFullDatabaseTableName() & "')	""))" & vbCrLf

                Return sBindWorkflowSteps
            Else
                Return ""
            End If
        End Function

        Shared Sub GetSaveWorkflowInfo(ByRef sSaveWorkflowInfoCall As String, ByRef sSaveWorkflowInfoMethod As String)
            If isWorkflow And IsLastPage() Then
                sSaveWorkflowInfoCall = "dim nWorkflowProcessID,nWorkflowStepID as Integer" & vbCrLf & vbCrLf

                sSaveWorkflowInfoCall &= "nWorkflowProcessID = GetWorkflowProcessID(lblWorkFlowStepID.Text)" & vbCrLf & vbCrLf
                sSaveWorkflowInfoCall &= "nWorkflowStepID = SaveWorkflowStep(nWorkflowProcessID,nCurrentID,lblWorkflowStepID.Text,""" & currentProject.GetFullDatabaseTableName() & """)" & vbCrLf

                If General.Variables.isFrontend Then
                    sSaveWorkflowInfoCall &= "SaveWorkflowProcessStepRequest(nWorkflowProcessID,lblWorkflowStepID.Text,Common.GetCurrentUsername())" & vbCrLf
                    sSaveWorkflowInfoCall &= "ExecuteNonQuery(""UPDATE " & currentProject.GetFullDatabaseTableName() & " SET WorkflowProcessStepID = "" & nWorkflowStepID & If(GetQueryString(""WorkflowStepID"") <> """", "",WorkflowPreviousProcessStepID = "" & GetWorkflowStepID(),"""") & "" WHERE ID = "" & nCurrentID)" & vbCrLf
                End If

                'Update workflowprocesssteprequest for this processid and step, if one exists; update date completed date, completed user
                sSaveWorkflowInfoCall &= "dim dtWorkflowTriggerControls as DataTable = GetDataTable(""SELECT *, (Select top 1 ID FROM " & DT_WORKFLOWTYPESTEPS & " WHERE Position = Destination AND TypeID = (SELECT TypeID FROM " & DT_WORKFLOWTYPESTEPS & " Where ID = "" & lblWorkflowStepID.Text & "")) as NextStepID FROM " & DT_WORKFLOWTYPESTEPTRIGGERCONTROLS & " WHERE StepID = "" & lblWorkflowStepID.Text)" & vbCrLf & vbCrLf
                sSaveWorkflowInfoCall &= "For Each CurrentTrigger as DataRow in dtWorkflowTriggerControls.Rows" & vbCrLf
                sSaveWorkflowInfoCall &= "If CType(Page.FindControl(CurrentTrigger.Item(""ControlName"")),WebControl).Enabled Then " & vbCrLf
                sSaveWorkflowInfoCall &= "SaveWorkflowProcessStepRequest(nWorkflowProcessID,lblWorkflowStepID.Text,Common.GetCurrentUsername(),CurrentTrigger.Item(""NextStepID""),GetWorkflowStepSupervisors(lblWorkflowStepID.Text))" & vbCrLf
                sSaveWorkflowInfoCall &= "End If" & vbCrLf
                sSaveWorkflowInfoCall &= "Next" & vbCrLf & vbCrLf

                'Save workflowprocesssteprequests for each enabled trigger control.  Save requesting step ID,
                'requested step id (destination), submitter username

                sSaveWorkflowInfoCall &= "CheckWorkflowFinished(nWorkflowProcessID,lblWorkflowStepID.Text)" & vbCrLf
                sSaveWorkflowInfoMethod = ""
            End If
        End Sub

        Shared Function GetWorkflowStepReference() As String
            If pageNumber = -1 Then
                Return "lblWorkflowStepID.Text"
            Else
                Return "Needtowritethisformultipage"
            End If
        End Function

    End Class
End Namespace
