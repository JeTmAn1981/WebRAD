Imports System.Data
Imports Common.Actions
Imports Common.General.Variables

Namespace Actions
    Public Class JSTriggerValueWriter
        Inherits TriggerValueWriter

        Protected Overrides Function GetOrOperator() As String
            Return " || "
        End Function

        Protected Overrides Function GetCompareOperator() As String
            Return " == "
        End Function

        Protected overrides Function GetTriggerOperator(byval triggerOperator As string, byval triggerControlDataType as integer) As string
            if triggerControlDataType = N_LISTBOX_DATATYPE
                return "!="
                else
                Return TransformTriggerOperator(triggeroperator)
            End If
            
        End Function

       Private Function TransformTriggerOperator(ByVal triggeroperator As String) As string
            if triggeroperator = "=" 
                triggeroperator = "=="
            ElseIf triggeroperator = "<>"
                triggeroperator = "!="
            End If

            return triggeroperator
        End Function

        Protected Overrides Function GetValueReference(sControlReference As String, sTriggerValue As String) As String
            Return "$(this)." & sTriggerValue
        End Function

        protected Overrides Function GetTriggerValue(sValueAttribute As String, CurrentRow As DataRow) As Object
            with currentrow
                If .Item("TriggerControlDataType") = N_listbox_datatype
                    Return "-1"
                    Else 
                    Return GetTriggerValueQuotes(CurrentRow.Item("TriggerControlSQLType")) & If(InStr(sValueAttribute, "TriggerValue"), "True", CurrentRow.Item("TriggerValue")) & "" & GetTriggerValueQuotes(CurrentRow.Item("TriggerControlSQLType"))
            End If
            
            End With
        End Function
    End Class

End Namespace
