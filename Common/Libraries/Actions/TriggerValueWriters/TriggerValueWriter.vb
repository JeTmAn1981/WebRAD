Imports WhitTools
Imports System.Data
Imports Common.General.Variables
Imports Variables = Common.General.Variables

Namespace Actions
    Public mustinherit Class TriggerValueWriter
          Function GetTriggerValues(ByVal nActionID As Integer, ByVal sControlReference As String, ByVal sValueAttribute As String) As String
             Dim sTriggerValues, sValueReference, triggerValue As String
             Dim dtTriggerValues As DataTable = DataTables.GetDataTable("Select TV.*,DT.ID as TriggerControlDataType, (SELECT SQLDataType FROM " & Common.General.Variables.DT_WEBRAD_PROJECTCONTROLS & " WHERE ID = (SELECT TriggerControl FROM " & Common.General.Variables.DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE ID = " & nActionID & ")) as TriggerControlSQLType From " & Variables.DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONTRIGGERVALUES & " TV LEFT OUTER JOIN " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS  & " PA ON TV.ActionID = PA.ID LEFT OUTER JOIN " & DT_WEBRAD_PROJECTCONTROLS & " PC ON PA.TriggerControl = PC.ID LEFT OUTER JOIN " & DT_WEBRAD_CONTROLTYPES  & " CT ON PC.ControlType = CT.ID LEFT OUTER JOIN " & DT_WEBRAD_CONTROLDATATYPES & " DT ON CT.DataType = DT.ID Where ActionID = " & nActionID)
            
             If dtTriggerValues.Rows.Count = 0 Then
                 sValueReference = GetValueReference(sControlReference, ProcessTriggerValue(sValueAttribute, "1"))

                 Return sValueReference & " " & GetCompareOperator() & " ""1"""
             ElseIf dtTriggerValues.Rows.Count = 1 Then
                sValueReference = GetValueReference(sControlReference, ProcessTriggerValue(sValueAttribute, dtTriggerValues.Rows(0).Item("TriggerValue")))
                triggerValue = GetTriggerValue(sValueAttribute, dtTriggerValues.Rows(0))

                Return sValueReference & " " & GetTriggerOperator(dtTriggerValues.Rows(0).Item("TriggerOperator"),dtTriggerValues.Rows(0).Item("TriggerControlDataType")) & " " & triggerValue 
             Else
                 For Each CurrentRow As DataRow In dtTriggerValues.Rows
                     If sTriggerValues <> "" Then
                         sTriggerValues &= GetOrOperator()
                     End If

                    sValueReference = GetValueReference(sControlReference, ProcessTriggerValue(sValueAttribute,  CurrentRow.Item("TriggerValue")))
                    triggerValue = GetTriggerValue(sValueAttribute, CurrentRow)
                    
                    sTriggerValues &= sValueReference & " " & GetTriggerOperator(currentrow.Item("TriggerOperator"),CurrentRow.Item("TriggerControlDataType")) & " " & triggerValue
                 Next

                 Return "(" & sTriggerValues & ")"
             End If
         End Function

          protected Overridable  Function GetTriggerValue(sValueAttribute As String, CurrentRow As DataRow) As Object
            Return GetTriggerValueQuotes(CurrentRow.Item("TriggerControlSQLType")) & If(InStr(sValueAttribute, "TriggerValue"), "True", CurrentRow.Item("TriggerValue")) & "" & GetTriggerValueQuotes(CurrentRow.Item("TriggerControlSQLType"))
          End Function

          protected MustOverride Function GetTriggerOperator(byval triggeroperator As string, byval triggerControlDataType as integer) As string
          
          protected  MustOverride Function GetOrOperator() As String

        protected  MustOverride Function GetCompareOperator() As String
            
         protected MustOverride  Function GetValueReference(sControlReference As String, sTriggerValue As String) As String
            
         Function GetTriggerValueQuotes(ByVal nSQLType As Integer) As String
             Return """"
             'This might need to use quotes for all values since comparison value from control is always string?
             'Return If(nSQLType = N_INTEGER_SQLTYPE Or nSQLType = N_FLOAT_SQLTYPE, "", """")
         End Function

          Function ProcessTriggerValue(ByVal sValueAttribute As String, ByVal sTriggerValue As String) As String
                 Return Replace(sValueAttribute, "TriggerValue", sTriggerValue)
          End Function

         Private  Function GetOrOperator(sTriggerValues As String) As String
            sTriggerValues &= " OR "

             Return sTriggerValues
         End Function
    End Class
End NameSpace