Imports System.Data

Namespace Actions
    Public Class PostbackTriggerValueWriter
        Inherits TriggerValueWriter

        Protected Overrides Function GetOrOperator() As String
            Return " OR "
        End Function

        Protected Overrides Function GetCompareOperator() As String
            Return " = "
        End Function

        Protected Overrides Function GetTriggerOperator(ByVal triggeroperator As string, byval triggerControlDataType as integer) As String
            Return triggeroperator
        End Function

        Protected Overrides Function GetValueReference(sControlReference As String, sTriggerValue As String) As String
            Return sControlReference & "." & sTriggerValue
        End Function
    End Class
End Namespace

