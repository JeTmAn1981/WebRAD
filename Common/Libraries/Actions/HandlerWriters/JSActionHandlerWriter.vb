Imports System.Data
Imports Common.Actions.ActionWriters
Imports General
Imports Common.General.Repeaters
Imports Common.General.Variables
Imports Common.General.ControlTypes
Imports Common.General.DataTypes
Imports Common.General.Controls
Imports Common.Actions.Main
Imports WhitTools.DataTables

Namespace Actions.HandlerWriter
    Public Class JSActionHandlerWriter
        Inherits ActionHandlerWriter

        public Sub New(ByRef controlData As datarow)
            MyBase.New(controldata)
            controlReference = GetControlReference()
        End Sub
        

        protected overrides sub WriteActionHandlerOpening()
            projectActionData.js.handlers.Append("function " & GetMethodName() & "()" & vbCrLf & "{" & vbcrlf)
        End sub

        protected overrides sub  WriteParentItemOpening
            'projectActionData.js.handlers.Append("var targetControl = $('#" & controlData.Item("Name") & "');" & vbCrLf & vbcrlf)

            'If ParentIsRepeaterControl(controldata.Item("ID"), "-1", nLayers) Then
            '   projectActionData.js.handlers.Append("With CType(sender")

            '            For nLayersCounter = 0 To nLayers - 1
            '               projectActionData.js.handlers.Append(".parent")
            '            Next

            '           projectActionData.js.handlers.Append(", RepeaterItem)" & vbCrLf )
            'End If
               End sub

            
        Protected overrides Sub WriteActionHandlerClosing()
            writeBlur()
            writeValidatorActions()
            projectActionData.js.handlers.Append("}" & vbCrLf & vbCrLf)
        End Sub

        Private sub WriteValidatorActions()
            projectActionData.js.handlers.Append("EnableValidators();" & vbcrlf)
            projectActionData.js.handlers.Append("HideValidationSummaries();" & vbcrlf)
        End sub

        private Sub writeBlur()
            projectActionData.js.handlers.Append("$(this)")

            if IsMultiElementDataType(GetControlDataType(controldata.Item("ControlType")))
                projectActionData.js.handlers.Append(".find('input')" )
            end if

            projectActionData.js.handlers.Append(".blur();" & vbcrlf)
        End Sub

        Protected overrides Sub WriteActionTrigger()
            projectActionData.js.triggers &= GetMethodName() & ".call($('" & controlreference & "'));" & vbcrlf
        end Sub

        protected overrides Sub WriteParentItemClosing()
            
        End Sub

        protected  overrides function GetActionWriter(ByRef action As ProjectControlPostbackAction) As ActionWriter
            return new JSActionWriter(action)
        End function
        protected overrides Function GetActionMethod() As String
            return controldata.Item("JSActionMethod")
        End Function

       protected overrides Sub WriteHandlerRegistration()
            projectActionData.js.registrations &= "$('" & controlReference & "')." & controlData.Item("JSActionMethod") & "(" & controlname & "_" & controlData.Item("JSActionMethod") & ");" & vbcrlf
          End Sub

        Private Function GetControlReference() As String
            Dim controlReference,parentcontrolID As String
                
            if ParentIsRepeaterControl(controlData.Item("ID"), "-1",0,parentControlID)
                controlReference = "[id*=""" & GetControlName(parentControlID) & "_" & controlname  & """]"
            else
                controlReference =  "#" & controlname
            End If

            Return controlReference
        End Function

        protected overrides Function GetRelevantActions() As DataTable
            return GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE TriggerControl = " & controldata.Item("ID") & " And Action IN (SELECT ID FROM " & dt_webrad_controlactiontypes & " WHERE UseJavascript = 1)")
         End Function
    End Class
End Namespace