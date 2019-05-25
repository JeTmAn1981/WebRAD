Imports System.Data
Imports General
Imports Common.Actions.ActionWriters
Imports Common.Actions.Main
Imports Common.Actions.TriggerValueWriter
Imports Common.General

Namespace Actions.ActionWriters
    Public Class PostbackActionWriter
        Inherits ActionWriter

        Public Sub New(ByRef action As ProjectControlPostbackAction)
            MyBase.New(action, New PostbackTriggerValueWriter())
        End Sub

        Protected Overrides Sub WriteVisible()
            Variables.projectActionData.postback.handlers.Append("Try" & vbCrLf)
            Variables.projectActionData.postback.handlers.Append(targetControlName & ".Visible = If(" & triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) & ",True,False)" & vbCrLf)
            Variables.projectActionData.postback.handlers.Append("Catch ex As Exception" & vbCrLf)
            Variables.projectActionData.postback.handlers.Append(targetControlName & ".Visible = False" & vbCrLf)
            Variables.projectActionData.postback.handlers.Append("End Try" & vbCrLf)
        End Sub

        protected overrides sub SetTargetControlName(ByVal prefix As string) 
                targetControlName = prefix & targetcontrol.Item("Name")
        End sub
    End Class
End Namespace