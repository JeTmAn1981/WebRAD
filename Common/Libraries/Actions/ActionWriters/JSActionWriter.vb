Imports System.Data
Imports General
Imports Common.Actions.Main
Imports Common.Actions.TriggerValueWriter
Imports Common.General.variables
Imports Common.General.ControlTypes
Imports Common.General.Controls
Imports Common.General.DataTypes
Imports Common.General.Repeaters
Namespace Actions.ActionWriters


    Public Class JSActionWriter
        Inherits ActionWriter
        Public Sub New(ByRef action As ProjectControlPostbackAction)
            MyBase.New(action, New JSTriggerValueWriter())
        End Sub

        Protected Overrides Sub WriteVisible()
            Dim controlDataType as integer = GetControlDataType(getcontrolcolumnvalue(action.TriggerControl,"ControlType"))
            Dim valueCondition as String = GetValueCondition(controlDataType)

            If IsMultiElementDataType(controlDataType)
                valueCondition &= " && $(this).is(':checked')"
            
                projectActionData.js.handlers.Append("var visibilityConditionCheck = function () { return " & valueCondition & "; };"  & vbcrlf & vbcrlf)
                projectActionData.js.handlers.Append("WebRADApps.UpdateMultielementVisibility.call(this, visibilityConditionCheck, " & GetTargetControlReference() & ");" & vbcrlf)
            else
                projectActionData.js.handlers.Append("WebRADApps.ToggleVisibility(" & valueCondition & ", " & GetTargetControlReference & ");" & vbCrLf)
            End If
        End Sub

        Private Function GetValueCondition(controlDataType As Integer) As String
            dim valueCondition As String

            if controlDataType = N_LISTBOX_DATATYPE 
                valueCondition &= "$(this).val() != null && "
            End If

            valueCondition &= triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("JSValueAttribute"))

            Return valueCondition
        End Function

        private function GetTargetControlReference() As string
            Dim parentcontrolID as Integer

            if ParentIsRepeaterControl(targetControl.Item("ID"),"-1",0,parentControlID)
                return "GetRptContainerReference(this, '" & targetControlName & "')"
            Else 
                return "$('#" & targetControlName  & "')"
            End If
        End function

        protected overrides sub SetTargetControlName(ByVal prefix As string) 
            if GetControlColumnValue(action.TargetControl,"controlType") <> N_PANEL_CONTROL_TYPE
                targetControlName = targetcontrol.Item("Name") & "Container"
            else
                targetControlName = prefix & targetcontrol.Item("Name")
            End If
        End sub
    End Class
    End Namespace