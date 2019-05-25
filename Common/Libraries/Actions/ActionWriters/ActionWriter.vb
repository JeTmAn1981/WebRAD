Imports Microsoft.VisualBasic
Imports Common.General.Variables
Imports Common.Actions.Main
Imports Common.General.Repeaters
Imports Common.Webpages.BindData
Imports Common.General.DataTypes
Imports Common.General.DataSources
Imports Common.General.ControlTypes
Imports Common.General.Controls
Imports Common.Actions.TriggerValueWriter
Imports WhitTools.DataTables
Imports WhitTools.Utilities
Imports System.Data
Imports Common.General

Namespace Actions.ActionWriters
    Public mustinherit Class ActionWriter
        Public action As ProjectControlPostbackAction
        protected triggerControlName,targetControlName, targetControlContainerName as string
        protected controlDescription, actionType as string
        protected targetControl,triggerControl,targetDataType,triggerDataType As DataRow
        protected triggerValueWriter as TriggerValueWriter
        
        public Sub New(ByRef action As ProjectControlPostbackAction, byval triggerValueWriter As triggervaluewriter)
            me.action = action
            me.triggerValueWriter = triggervaluewriter
        End Sub

        Public Sub WriteAction()
            actionType = (From at As ControlActionType In db.ControlActionTypes
                                        Where at.ID = action.Action
                                        Select at.Type).First()

            SetupControlData()

            If actionType = "Custom" Then
                WriteCustom()
            ElseIf actionType = "Visible" Then
                WriteVisible()
            ElseIf actionType = "Enabled" Then
                WriteEnable()
            ElseIf actionType = "SetValue" Then
                WriteSetValue()
            ElseIf actionType = "UpdateRepeaterItems" Then
                WriteUpdateRepeaterItems()
            ElseIf actionType = "FillListData" Then
                WriteFillListData()
            ElseIf actionType = "ResetSelectedIndex" Then
                WriteResetSelectedIndex()
            End If

            projectActionData.postback.handlers.Append(vbCrLf)
        End Sub

        Protected Sub SetupControlData()
            SetupTargetControlData()
            SetupTriggerControlData()
        End Sub

        Private Sub SetupTriggerControlData()
            Dim sTriggerControlPrefix As String

            triggerControl = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ID = " & action.TriggerControl).Rows(0)
            triggerDataType = GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLDATATYPES & " WHERE ID = " & GetControlDataType(triggerControl.Item("controltype"))).Rows(0)
            sTriggerControlPrefix = If(triggerDataType.Item("LabelOnPrintable") = "1" And isPrintable, "lbl", triggerDataType.Item("Prefix"))
            triggerControlName = If(ParentIsRepeaterControl(action.TriggerControl), "ctype(.FindControl(""" & sTriggerControlPrefix & triggerControl.Item("Name") & """)," & controlDescription & ")", sTriggerControlPrefix & triggerControl.Item("Name"))
        End Sub

        Private Sub SetupTargetControlData()
            If actionType <> "Custom" And action.TargetControl <> 0 Then
                Dim sTargetControlPrefix As String

                Try
                    targetControl = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ID = " & action.TargetControl).Rows(0)
                    targetDataType = GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLDATATYPES & " WHERE ID = " & GetControlDataType(targetControl.Item("controltype"))).Rows(0)
                    sTargetControlPrefix = If(targetDataType.Item("LabelOnPrintable") = "1" And isPrintable, "lbl", targetDataType.Item("Prefix"))
                    SetTargetControlName(sTargetControlPrefix)
                    controlDescription = GetDataTypeDescription(targetDataType.Item("ID"))
                Catch ex As Exception
                    logger.Error("Error setting up target control data for action writer:")
                    logger.Error(ex.ToString)
                    logger.Error("Target control select: " & "SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ID = " & action.TargetControl)
                    logger.Error("Target data type select: " & "SELECT * FROM " & DT_WEBRAD_CONTROLDATATYPES & " WHERE ID = " & GetControlDataType(targetControl.Item("controltype")))
                End Try

            End If
        End Sub



        Protected Sub WriteUpdateRepeaterItems()
            If controlDescription = "Dropdownlist" Then
                Dim sItems As String

                If action.UpdateRepeaterItemsSelectionType = "Explicit" Then
                    sItems = action.UpdateRepeaterItemsValue
                Else
                    sItems = triggerControlName & ".selecteditem.value"
                End If

                GetUpdateRepeaterItems(sItems, targetControlName, triggerControlName)

                'Bug Alert - this might end up creating multiple dtSupplied DataTable objects in the same scope
            ElseIf controlDescription = "Radiobuttonlist" Then
                GetUpdateRepeaterItems(action.UpdateRepeaterItemsValue, targetControlName, triggerControlName)
            End If
        End Sub

        Protected Sub WriteFillListData()
            Dim sDataTextField, sDataValueField As String

            If controlDescription = "Dropdownlist" Then
                Dim sSelectString As String = GetDataSourceSelectString(action.DataSourceID, sDataTextField, sDataValueField)

                If triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) <> triggerControlName & ".SelectedValue = ""1"" " Then
                    projectActionData.postback.handlers.Append("If " & triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) & vbCrLf)
                End If

                projectActionData.postback.handlers.Append("FillListData(" & targetControlName & ",GetDataTable(""" & sSelectString & """),""" & sDataTextField & """,""" & sDataValueField & """," & CBool(action.IncludePleaseSelect) & If(action.IncludePleaseSelect = "1", ",""""", "") & ")" & vbCrLf)

                If triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) <> triggerControlName & ".SelectedValue = ""1"" " Then
                    projectActionData.postback.handlers.Append("End If" & vbCrLf & vbCrLf)
                End If
            ElseIf controlDescription = "Radiobuttonlist" Then
                Dim sSelectString As String = GetDataSourceSelectString(action.DataSourceID, sDataTextField, sDataValueField)

                If triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) <> triggerControlName & ".SelectedValue = ""1"" " Then
                    projectActionData.postback.handlers.Append("If " & triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) & vbCrLf)
                End If

                projectActionData.postback.handlers.Append("FillListData(" & targetControlName & ",GetDataTable(""" & sSelectString & """),""" & sDataTextField & """,""" & sDataValueField & """," & CBool(action.IncludePleaseSelect) & If(action.IncludePleaseSelect = "1", ",""""", "") & ")" & vbCrLf)

                If triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) <> triggerControlName & ".SelectedValue = ""1"" " Then
                    projectActionData.postback.handlers.Append("End If" & vbCrLf & vbCrLf)
                End If
            End If
        End Sub

        Protected Sub WriteResetSelectedIndex()
            If controlDescription = "Radiobuttonlist" Then
                If triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) <> triggerControlName & ".SelectedValue = ""1"" " Then
                    projectActionData.postback.handlers.Append("If " & triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) & vbCrLf)
                End If

                projectActionData.postback.handlers.Append(targetControlName & ".SelectedIndex = 0" & vbCrLf)

                If triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) <> triggerControlName & ".SelectedValue = ""1"" " Then
                    projectActionData.postback.handlers.Append("End If" & vbCrLf & vbCrLf)
                End If
            End If
        End Sub

        Protected Sub WriteSetValue()
            Dim sValueReference As String

            If action.SetValueType = "Explicit" Then
                sValueReference &= """" & action.ExplicitValue & """"
            ElseIf action.SetValueType = "Database" Then
                sValueReference &= "GetDataTable(""" & GetDataSourceSelectString(action.DataSourceID) & """,cnx).Rows(0).Item(""" & action.ExplicitValue & """)"
            ElseIf action.SetValueType = "Control" Then
                sValueReference &= GetControlValueReference(GetDataTable("Select C.ID, C.PageID, C.Name,C.ControlType, T.DataType, D.Prefix, D.Description, D.ActionMethod, C.ParentControlID,C.DisplayLocation, SelectionItems, DataMethod,OtherDataMethod,TextField,ValueField,MinimumValue,MaximumValue, D.LabelOnPrintable From " & DT_WEBRAD_PROJECTCONTROLS & " C left outer join " & DT_WEBRAD_CONTROLTYPES & "  T on C.ControlType = T.ID left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  D on T.DataType = D.ID LEFT OUTER JOIN " & DT_WEBRAD_PROJECTDATASOURCES & " DS ON DS.ID = C.DataSourceID Where C.ID = " & action.TriggerControl, Common.General.Variables.cnx).Rows(0))
            End If

            'Possible source of Bug - problem with allowing checks of SelectedValue = 1 to go through?
            'If Common.Main.triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) <> triggerControlName & ".SelectedValue = ""1"" " Then
            projectActionData.postback.handlers.Append("If " & triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) & vbCrLf)
            'End If

            If IsListControlType(targetControl.Item("ControlType")) Then
                projectActionData.postback.handlers.Append("Set" & If(IsMultiValuedListControlType(targetControl.Item("ControlType")), "ListControl", "") & "ItemSelected(" & targetControlName & "," & sValueReference & ")" & vbCrLf)
            Else
                projectActionData.postback.handlers.Append(targetControlName & ".Text = " & sValueReference & vbCrLf)
            End If

            projectActionData.postback.handlers.Append("End If" & vbCrLf & vbCrLf)
        End Sub

        Protected Sub WriteCustom()
            projectActionData.postback.handlers.Append(action.CustomActionCode & vbCrLf)
        End Sub

        Protected Sub WriteEnable()
            projectActionData.postback.handlers.Append("Try" & vbCrLf)
            projectActionData.postback.handlers.Append(targetControlName & ".Enabled = If(" & triggerValueWriter.GetTriggerValues(action.ID, triggerControlName, triggerDataType.Item("ValueAttribute")) & ",True,False)" & vbCrLf)
            projectActionData.postback.handlers.Append("Catch ex As Exception" & vbCrLf)
            projectActionData.postback.handlers.Append(targetControlName & ".Enabled = False" & vbCrLf)
            projectActionData.postback.handlers.Append("End Try" & vbCrLf)
        End Sub


        Protected Sub GetUpdateRepeaterItems(ByVal sValueReference As String, ByVal sTargetControlName As String, ByVal sTriggerControlName As String)
            Dim sDTSupplied As String

            Dim targetControl As ProjectControl = db.ProjectControls.First(Function(x) x.ID = action.TargetControl)
            Dim triggerControl As ProjectControl = db.ProjectControls.First(Function(x) x.ID = action.TriggerControl)

            Dim targetDataType As DataRow = GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLDATATYPES & " WHERE ID = " & GetControlDataType(targetControl.ControlType)).Rows(0)
            Dim triggerDataType As DataRow = GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLDATATYPES & " WHERE ID = " & GetControlDataType(triggerControl.ControlType)).Rows(0)

            If triggerValueWriter.GetTriggerValues(action.ID, sTriggerControlName, triggerDataType.Item("ValueAttribute")) <> sTriggerControlName & ".SelectedValue = ""1"" " Then
                projectActionData.postback.handlers.Append("If " & triggerValueWriter.GetTriggerValues(action.ID, sTriggerControlName, triggerDataType.Item("ValueAttribute")) & vbCrLf)
            End If

            GetSuppliedData(sDTSupplied, projectActionData.postback.handlers, action.TargetControl)

            If sDTSupplied <> "" Then
                projectActionData.postback.handlers.Append(sDTSupplied & vbCrLf)

                projectActionData.postback.handlers.Append("UpdateRepeaterItems(" & sTargetControlName & "," & sValueReference & ",dtSupplied,""-1"",-1,cnx)")
            Else
                projectActionData.postback.handlers.Append("UpdateRepeaterItems(" & sTargetControlName & "," & sValueReference & ")")
            End If

            sDTSupplied = ""

            Dim sGetBindRepeaterData As String = ""

            GetBindRepeaterData(action.TargetControl, "", sGetBindRepeaterData, "", "", 1, True)

            projectActionData.postback.handlers.Append(sGetBindRepeaterData)

            If triggerValueWriter.GetTriggerValues(action.ID, sTriggerControlName, triggerDataType.Item("ValueAttribute")) <> sTriggerControlName & ".SelectedValue = ""1"" " Then
                projectActionData.postback.handlers.Append("End If" & vbCrLf & vbCrLf)
            End If
        End Sub

        protected mustoverride Sub WriteVisible()

        protected MustOverride sub SetTargetControlName(ByVal prefix As string) 
    End Class
End Namespace



