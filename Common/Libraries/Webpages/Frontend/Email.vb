Imports System.Data
Imports System.Data.SqlClient
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
Imports Common.General.DataSources
Imports Common.General.Columns
Imports Common.General.Assembly

Imports Common.General.Links
Imports Common.SQL.StoredProcedure
Imports Common.Webpages.Frontend.MultiPage
Imports Common.SQL.Main
Imports Common.SQL.Table
Imports Common.Webpages.Frontend.Email
Imports Common.General.ProjectOperations
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Export
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.BindData
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File
Imports WhitTools.Formatter
Imports WhitTools.Workflow
Imports WhitTools.RulesAssignments


Imports WhitTools.Utilities
Namespace Webpages.Frontend


    Public Class Email
        Inherits Webpages.Main

        Shared Sub GetEmailContent(ByRef sSendEmailCall As String, ByRef sSendEmailMethod As String)
            GetSupervisorEmail(sSendEmailMethod, sSendEmailCall)
            GetSubmitterEmail(sSendEmailMethod, sSendEmailCall)
        End Sub

        Private Shared Sub GetSubmitterEmail(ByRef sSendEmailMethod As String, ByRef sSendEmailCall As String)
            Dim sBody As String
            Dim sSubject As String
            Dim bControlInRepeater As Boolean

            If GetAncillaryProject("SubmitterEmail") = "1" And islastpage() Then
                sSendEmailCall &= "SendSubmitterEmail(" & If(isWorkflow, "nWorkflowStepID", "") & ")" & vbCrLf

                Dim dtEmailMessages As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTEMAILMESSAGES & " WHERE ProjectID = " & GetProjectID())

                sSendEmailMethod &= "Sub SendSubmitterEmail(Optional byVal nWorkflowStepID as Integer = 0)" & vbCrLf
                sSendEmailMethod &= "dim sTo as string = """"" & vbCrLf & vbCrLf
                sSendEmailMethod &= "dim sBCC as string = """"" & vbCrLf & vbCrLf

                For Each CurrentMessage As DataRow In dtEmailMessages.Rows
                    With CurrentMessage
                        .Item("Message") = FormatStringStripCarriageReturn(.Item("Message"))
                        sBody = If(.Item("MessageType") = "Rich", Replace(.Item("Message"), """", """"""), .Item("Message"))
                        'sBody = If(.Item("MessageType") = "Rich", FormatStringStripCarriageReturn(Replace(.Item("Message"), """", """""")), .Item("Message"))

                        sSubject = .Item("Subject")

                        If CurrentMessage.Item("Workflow") = "1" Then
                            sBody &= GetWorkflowLink(CurrentMessage.Item("WorkflowDestination"))
                        End If

                        If .Item("ToAddress") <> "" Then
                            sSendEmailMethod &= "sTo = """ & .Item("ToAddress") & ";""" & vbCrLf
                        End If

                        If .Item("BCCAddress") <> "" Then
                            sSendEmailMethod &= "sBCC = """ & .Item("BCCAddress") & ";""" & vbCrLf
                        End If

                        Dim EmailControlsDT As DataTable = GetDataTable("SELECT C.*, D.Prefix, CT.DataType FROM " & DT_WEBRAD_PROJECTCONTROLS & " C left outer join " & DT_WEBRAD_CONTROLTYPES & " CT on C.ControlType = CT.ID LEFT OUTER JOIN " & DT_WEBRAD_CONTROLDATATYPES & " D on CT.DataType = D.ID WHERE C.ID IN (Select ControlID FROM " & DT_WEBRAD_PROJECTEMAILMESSAGESUBMITTERCONTROLS & " WHERE MessageID = " & CurrentMessage.Item("ID") & ")", General.Variables.cnx)

                        For Each SubmitterControl As DataRow In EmailControlsDT.Rows
                            Dim nLayers As Integer = 0
                            Dim sNextParentControlID As String

                            bControlInRepeater = ParentIsRepeaterControl(SubmitterControl.Item("ID"), "-1", nLayers, sNextParentControlID)

                            If bControlInRepeater Then
                                'Must update this to support infinitely nested repeaters
                                sSendEmailMethod &= "For Each CurrentItem as RepeaterItem in rpt" & GetControlColumnValue(sNextParentControlID, "Name") & ".Items" & vbCrLf
                                sSendEmailMethod &= "With CurrentItem" & vbCrLf
                            End If

                            If IsMultipageForm() Then
                                sSendEmailMethod &= "sTo &= GetDataTable(""Select * FROM "" & MAIN_DATABASE_TABLE_NAME & "" WHERE ID = "" & GetCurrentApplicationID()).Rows(0).Item(""" & SubmitterControl.Item("Name") & """)" & vbCrLf
                            Else
                                sSendEmailMethod &= "If " & GetControlValueReference(SubmitterControl, bControlInRepeater) & " <> """" And " & GetControlValueReference(SubmitterControl, bControlInRepeater, True) & ".Enabled Then" & vbCrLf

                                sSendEmailMethod &= "If sTo <> """" And sTo <> "";"" Then" & vbCrLf
                                sSendEmailMethod &= "sTo &= "";""" & vbCrLf
                                sSendEmailMethod &= "End If" & vbCrLf

                                sSendEmailMethod &= "sTo &= " & GetControlValueReference(SubmitterControl, bControlInRepeater) & vbCrLf
                                sSendEmailMethod &= "End If" & vbCrLf
                            End If

                            If bControlInRepeater Then
                                sSendEmailMethod &= "End With" & vbCrLf
                                sSendEmailMethod &= "Next" & vbCrLf & vbCrLf
                            End If
                        Next

                        sSendEmailMethod &= vbCrLf
                        UpdateEmailControlReferences(sSubject)
                        UpdateEmailControlReferences(sBody)

                        sSendEmailMethod &= "If sTo <> """" And sTo <> "";"" Then" & vbCrLf
                        sSendEmailMethod &= "WhitTools.Email.SendEmail(sTo, """ & sSubject & """, """ & sBody & """,""webteam@whitworth.edu"","""",sBCC)" & vbCrLf
                        sSendEmailMethod &= "End If" & vbCrLf & vbCrLf
                        sSendEmailMethod &= "sTo = """"" & vbCrLf
                        sSendEmailMethod &= "sBCC = """"" & vbCrLf

                        If bControlInRepeater Then
                            sSendEmailMethod &= "End With" & vbCrLf
                            sSendEmailMethod &= "Next" & vbCrLf
                        End If
                    End With
                Next

                sSendEmailMethod &= "End Sub" & vbCrLf & vbCrLf
            End If
        End Sub

        Private Shared Sub GetSupervisorEmail(ByRef sSendEmailMethod As String, ByRef sSendEmailCall As String)
            Dim sBody As String
            Dim sSubject As String

            If GetAncillaryProject("EmailSupervisor") = "1" And IsLastPage() Then
                sSendEmailCall = vbCrLf & vbCrLf & "SendSupervisorEmail(nCurrentID)" & vbCrLf

                sSubject = projectDT.Rows(0).Item("PageTitle") & " Submission Received"
                sBody = "A new submission from the <b>" & projectDT.Rows(0).Item("PageTitle") & "</b> form has been received.  Please click the following link to view the details:<br /><br /><a href='" & GetProjectLink(projectDT.Rows(0).Item("BackendPath"), projectDT.Rows(0).Item("BackendLink")) & "update.aspx?ID="" & nCurrentID & ""'>" & projectDT.Rows(0).Item("PageTitle") & " - Maintenance</a>"

                sSendEmailMethod = "Sub SendSupervisorEmail(byVal nCurrentID as Integer)" & vbCrLf
                sSendEmailMethod &= "SendRuleEmail(" & GetEmailRuleID() & ", " & projectDT.Rows(0).Item("Department") & ", ""noreply@whitworth.edu"", """ & sBody & """, """ & sSubject & """)" & vbCrLf
                sSendEmailMethod &= "End Sub" & vbCrLf & vbCrLf
            End If
        End Sub

        Shared Sub UpdateEmailControlReferences(ByRef sBody As String)
            Dim dtControlData As DataTable
            Dim sControlReference As String
            Dim ControlPattern As New Regex("\{\{([A-Za-z0-9-]+)\}\}")

            Dim FoundControls As MatchCollection = ControlPattern.Matches(sBody)

            For Each CurrentMatch As Match In FoundControls
                For Each CurrentControl As DataRow In controlsDT.Rows
                    If CurrentControl.Item("Name") = Replace(Replace(CurrentMatch.Value, "{{", ""), "}}", "") Then
                        sControlReference = """ & " & GetControlValueReference(CurrentControl, False, False, True) & " & """

                        sBody = Replace(sBody, CurrentMatch.Value, sControlReference)
                    End If
                Next
            Next


            sControlReference = """ & lblFirstName.Text & """
            sBody = Replace(sBody, "{{FirstName}}", sControlReference)

            sControlReference = """ & lblLastName.Text & """
            sBody = Replace(sBody, "{{LastName}}", sControlReference)
        End Sub

        Shared Function GetEmailRuleID() As Integer
            Dim communicationsDB As New communicationsentities()
            Dim projectName As String = currentProject.GetProjectNameAlphaNumericOnly
            Dim username = GetCurrentUsername()
            Dim currentRule = communicationsDB.ARA_Rules.FirstOrDefault(Function(rule) rule.Description = projectName)

            If currentRule Is Nothing Then
                currentRule = New ARA_Rules()

                currentRule.Type = "2"
                currentRule.ShortDescription = projectName
                currentRule.Description = projectName
                currentRule.LastModifiedBy = username
                currentRule.LastModified = Now
                currentRule.SubmittedBy = username
                currentRule.DateSubmitted = Now

                communicationsDB.ARA_Rules.Add(currentRule)
            End If

            AddCurrentSupervisorAssignments(currentRule)
            communicationsDB.SaveChanges()

            Return currentRule.ID
        End Function

        Private Shared Sub AddCurrentSupervisorAssignments(ByRef currentRule As ARA_Rules)
            Dim currentAssignments = currentRule.ARA_Assignments.ToList()

            Dim newAssignments = TransformSupervisorsToAssignments(currentProject.ProjectSupervisors.ToList()).Where(
                Function(newAssignment)
                    Return Not currentAssignments.Any(
                    Function(currentassignment)
                        Return currentassignment.PLID = newAssignment.PLID And currentassignment.DepID = newAssignment.DepID
                    End Function)
                End Function).ToList()

            For Each newAssignment As ARA_Assignments In newAssignments
                currentRule.ARA_Assignments.Add(newAssignment)
            Next
        End Sub

        Private Shared Function TransformSupervisorsToAssignments(ByVal supervisors As List(Of ProjectSupervisor)) As List(Of ARA_Assignments)
            Return supervisors.Select(
                            Function(supervisor)
                                Dim assignment = New ARA_Assignments()

                                assignment.DepID = currentProject.Department

                                If supervisor.SupervisorType = "SingleUser" Then
                                    assignment.PLID = ExtractIDNumber(supervisor.SupervisorName)
                                Else
                                    assignment.PLID = supervisor.SupervisorEmail
                                End If

                                Return assignment
                            End Function).ToList()
        End Function
    End Class
End Namespace

