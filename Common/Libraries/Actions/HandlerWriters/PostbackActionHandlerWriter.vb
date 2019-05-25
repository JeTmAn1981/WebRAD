Imports System.data
Imports Common.Actions.ActionWriters
Imports Common.General.Controls
Imports Common.Actions.main
Imports Common.General
Imports Common.General.Repeaters
Imports Common.General.Variables
Imports WhitTools.DataTables

Namespace Actions.HandlerWriter
Public Class PostbackActionHandlerWriter
        Inherits ActionHandlerWriter

        public Sub New(ByRef controlData As DataRow)
            MyBase.New(controldata)
        End Sub

        protected overrides sub WriteActionHandlerOpening()
            projectActionData.postback.handlers.Append("Public Sub " & GetMethodName() & "(sender As Object, e As EventArgs)" & vbCrLf)
        End sub
        
        protected overrides sub  WriteParentItemOpening
            If ParentIsRepeaterControl(controldata.Item("ID"), "-1", nLayers) Then
               variables.projectActionData.postback.handlers.Append("With CType(sender")

                        For nLayersCounter = 0 To nLayers - 1
                           variables.projectActionData.postback.handlers.Append(".parent")
                        Next

                       variables.projectActionData.postback.handlers.Append(", RepeaterItem)" & vbCrLf )
            End If
              End sub

        Protected overrides Sub WriteActionHandlerClosing()
            variables.projectActionData.postback.handlers.Append("End Sub" & vbCrLf & vbCrLf)
        End Sub

        Protected overrides Sub WriteActionTrigger()
            If Not ParentIsRepeaterControl(controldata.Item("ID")) Then
                Variables.projectActionData.postback.triggers &= GetMethodName() & "(" & GetControlName(controlData.Item("ID")) & ",Nothing)" & vbCrLf
            End If
        End Sub

        protected overrides Sub WriteParentItemClosing()
            If ParentIsRepeaterControl(controldata.Item("ID")) Then
                       variables.projectActionData.postback.handlers.Append("End With" & vbCrLf)
            End If
        End Sub

        protected  overrides function GetActionWriter(ByRef action As ProjectControlPostbackAction) As ActionWriter
            return new PostbackActionWriter(action)
        End function

        protected overrides Function GetActionMethod() As String
            return controldata.Item("ActionMethod")
        End Function

        protected overrides Sub WriteHandlerRegistration()

        End Sub

        protected overrides Function GetRelevantActions() As DataTable
            return GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE TriggerControl = " & controldata.Item("ID") & " And Action IN (SELECT ID FROM " & dt_webrad_controlactiontypes & " WHERE UseJavascript = 0)")
         End Function
    End Class
End Namespace