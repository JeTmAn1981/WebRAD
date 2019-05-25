Imports System.Data
Imports ClosedXML.Excel
Imports Common.Actions.ActionWriters
Imports Common.General
Imports Common
Imports Common.General.DataTypes
Imports Common.General.Variables
Imports Common.General.Controls
Imports Common.General.ControlTypes
Imports Common.General.Repeaters
Imports Enumerable = System.Linq.Enumerable

Namespace Actions.HandlerWriter
public mustinherit Class ActionHandlerWriter
        protected controlData as datarow
        protected controlName As string
        protected controlReference as string
        protected nLayers As Integer = 0
        
        public sub New(ByRef controlData as datarow)
            me.controlData = controlData
            controlName = GetControlName(controlData.Item("ID"))
       End sub

        protected MustOverride Sub WriteParentItemOpening()
        protected MustOverride sub WriteActionHandlerOpening()
        Protected mustoverride Sub WriteActionHandlerClosing()
        Protected mustoverride Sub WriteActionTrigger()
        protected mustoverride Sub WriteParentItemClosing()

        protected MustOverride function  GetActionWriter(ByRef action As ProjectControlPostbackAction) As ActionWriter

        protected Function GetMethodName() As string
                Return controlName & "_" & GetActionMethod()
        End Function

        Protected mustoverride Function GetActionMethod()  As String

        Public Sub WriteActionHandler()
            WriteActionHandlerOpening()

            WriteParentItemOpening()

            nLayers = 0

            WriteActions()

            WriteParentItemClosing()

            WriteCalculateTotalCall()

            WriteActionHandlerClosing()

            WriteActionTrigger()

            WriteHandlerRegistration()
        End Sub

        Protected MustOverride Sub WriteHandlerRegistration()

        Private Sub WriteCalculateTotalCall()
            If Main.NeedsTotalCalculated() Then
                projectActionData.postback.handlers.Append("CalculateTotal()" & vbCrLf)
            End If
        End Sub

        Protected Sub WriteActions()
            Dim actionID As Integer
            Dim relevantActions = GetRelevantActions()

            For Each currentAction As DataRow In relevantActions.Rows
                actionID = currentAction.Item("ID")
                GetActionWriter(db.ProjectControlPostbackActions.First(Function(x) x.ID = actionID)).WriteAction()
            Next

            logger.Info("Control ID #" & controlData.Item("ID") & " list selections - " & controlData.Item("ListSelections") & " search - " & isSearch)

            If controlData.Item("ListSelections") = "1" And Not isSearch Then
                WriteListSelections()
            End If
        End Sub

        Protected Sub WriteListSelections()
            logger.Info("Writing list selections action")

            Try
                projectActionData.postback.handlers.Append("lbl" & controlData.Item("Name") & "SelectedItems.Text = GetListOfSelectedValues(" & controlName & ")" & vbCrLf)
            Catch ex As Exception
                logger.Error("Error writing list selections action:")
                logger.Error(ex.ToString)
            End Try

        End Sub

        Protected MustOverride Function GetRelevantActions() As DataTable
 End Class
 End Namespace
    
    
        