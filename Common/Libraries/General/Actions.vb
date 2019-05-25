Imports System.Data
Imports Common.Actions.ActionWriters
Imports Common.Actions.HandlerWriter
Imports Microsoft.VisualBasic
Imports WhitTools.Utilities
Imports WhitTools.DataTables
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Repeaters
Imports Common.General.Controls
Imports Common.General.ProjectOperations
Imports Common.General.ControlTypes
Imports Common.General.Ancillary
Imports Common.General.Pages
Imports Common.General.DataTypes
Imports Common.General.DataSources
Imports Common.Webpages.BindData

Namespace General
    Public Class Actions
        Shared Function ActionRequiresListItems(ByVal nActionType As String) As Boolean
            Return If(nActionType = "5", True, False)
        End Function

        Shared Function ActionRequiresValueSelection(ByVal nActionType As String) As Boolean
            Return If(nActionType = "6", True, False)
        End Function

        Shared Sub GetPostbackActions(ByRef sPostbackActions As StringBuilder, Optional ByRef sTriggerPostbackActions As String = Nothing)
            Dim sParentControlID As String
            Dim bDisplay As Boolean = True
            Dim dt As New DataTable

            WriteLine("creating postback actions - " & pageNumber)

            dt = GetDataTable("Select C.ID, C.PageID, C.Name,C.ControlType,C.IncludePleaseSelect, T.DataType, D.Prefix, D.Description, D.ActionMethod, C.ParentControlID,C.DisplayLocation, SelectionItems, ListSelections,DataMethod,OtherDataMethod,TextField,ValueField,MinimumValue,MaximumValue, D.LabelOnPrintable From " & DT_WEBRAD_PROJECTCONTROLS & " C left outer join " & DT_WEBRAD_CONTROLTYPES & "  T on C.ControlType = T.ID left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  D on T.DataType = D.ID LEFT OUTER JOIN " & DT_WEBRAD_PROJECTDATASOURCES & " DS ON DS.ID = C.DataSourceID Where ProjectID = " & GetProjectID() & " and " & ACTION_REQUIRED_SQL_CONDITION, General.Variables.cnx)

            For nCounter = 0 To dt.Rows.Count - 1
                WriteLine("creating action #" & nCounter + 1 & " of " & dt.Rows.Count & " for control " & dt.Rows(nCounter).Item("Name") & " with ID #" & dt.Rows(nCounter).Item("ID"))
                With dt.Rows(nCounter)
                    If ParentIsRepeaterControl(.Item("ID"), -1, 0, sParentControlID) Then
                        Try
                            bDisplay = ControlDisplayAllowed(GetControlColumnValue(sParentControlID, "DisplayLocation", controlsDT))
                        Catch ex As Exception
                            'WriteLine("parnet control - " & sParentControlID)
                        End Try
                    Else
                        bDisplay = ControlDisplayAllowed(.Item("DisplayLocation"))
                    End If
                End With

                If bDisplay Then
                    CreatePostbackAction(dt.Rows(nCounter), sPostbackActions, sTriggerPostbackActions)
                End If

                bDisplay = True
            Next

            AddBackendInsertActions(sPostbackActions)
        End Sub


        Private Shared Sub AddBackendInsertActions(ByRef sPostbackActions As StringBuilder)
            If isInsert And CurrentProjectRequiresWhitworthLogin() Then
                sPostbackActions.Append(vbCrLf & vbCrLf)
                'add vbcrlfs
                sPostbackActions.Append(GetCurrentUsernameOverload())

                sPostbackActions.Append("    Protected Sub txtUser_TextChanged(sender As Object, e As EventArgs)" & vbCrLf)
                sPostbackActions.Append("       dim username as string = ExtractUsername(txtuser.text)" & vbCrLf & vbCrLf)
                sPostbackActions.Append("        If GetUserInfo(CleanSQL(username)).Rows.Count > 0 Then" & vbCrLf)
                sPostbackActions.Append($"            Redirect(""insert{GetAncillaryName()}.aspx?Username="" & username)" & vbCrLf)
                sPostbackActions.Append("        End If" & vbCrLf)
                sPostbackActions.Append("    End Sub" & vbCrLf & vbCrLf)

                sPostbackActions.Append("    Sub LoadPersonalInfo()" & vbCrLf)
                sPostbackActions.Append("        Dim dtPersonalInfo As DataTable = GetUserInfo(CleanSQL(GetQueryString(""Username"")))" & vbCrLf & vbCrLf)

                sPostbackActions.Append("        If dtPersonalInfo.Rows.Count > 0 Then" & vbCrLf)
                sPostbackActions.Append("            With dtPersonalInfo.Rows(0)" & vbCrLf)
                sPostbackActions.Append("                Try" & vbCrLf)

                Dim dtLoginColumns As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCOLUMNS & " PCL LEFT OUTER JOIN " & DT_WEBRAD_LOGINCOLUMNTYPES & " LCT ON PCL.ColumnControlID = LCT.IDNumber WHERE Type='Login' AND ProjectID = " & GetProjectID())

                For Each CurrentRow As DataRow In dtLoginColumns.Rows
                    sPostbackActions.Append("lbl" & CurrentRow.Item("ControlName") & ".Text = .Item(""" & CurrentRow.Item("ControlName") & """)" & vbCrLf)
                Next

                sPostbackActions.Append("                    txtUser.Text = getdatatable(""SELECT * FROM adTelephone.dbo.UserInfo_V WHERE Username = '"" & CleanSQL(GetQueryString(""Username"")) & ""'"", cnx).rows(0).Item(""User"")" & vbCrLf)
                sPostbackActions.Append("                Catch ex As Exception" & vbCrLf)
                sPostbackActions.Append("                End Try" & vbCrLf)
                sPostbackActions.Append("            End With" & vbCrLf)
                sPostbackActions.Append("        End If" & vbCrLf)
                sPostbackActions.Append("    End Sub" & vbCrLf & vbCrLf)
            End If
        End Sub

        Shared Sub CreatePostbackAction(ByRef CurrentRow As DataRow, ByRef sPostbackActions As StringBuilder, Optional ByRef sTriggerPostbackActions As String = Nothing)
            Dim ActionControlDT As DataTable
            Dim sControlDescription, sDataTextField, sDataValueField As String

            With CurrentRow
                If ControlDisplayAllowed(.Item("DisplayLocation")) And BelongsToPage(pageNumber, .Item("PageID")) And GetControlDataType(.Item("ControlType")) <> N_REPEATER_DATA_TYPE Then
                    sPostbackActions.Append("Public Sub " & GetRepeaterHandlerReference(.Item("ID")) & .Item("Prefix") & .Item("Name") & "_" & .Item("ActionMethod") & "(sender As Object, e As EventArgs)" & vbCrLf)

                    Dim nLayers As Integer = 0
                    sControlDescription = GetDataTypeDescription(.Item("DataType"))

                    If ParentIsRepeaterControl(.Item("ID"), "-1", nLayers) Then
                        sPostbackActions.Append("With GetParentRepeaterItem(sender)" & vbCrLf)
                    End If

                    nLayers = 0

                    ActionControlDT = GetDataTable("Select PA.ID, D.Prefix, D.Description, (Select ValueAttribute From " & DT_WEBRAD_CONTROLDATATYPES & "  Where ID = (Select DataType From " & DT_WEBRAD_CONTROLTYPES & "  Where ID = (Select ControlType From " & DT_WEBRAD_PROJECTCONTROLS & " Where ID = TriggerControl))) as ValueAttribute, C.Name,C.ControlType, c.IncludePleaseSelect, T.DataType, A.Type, TriggerControl, TargetControl,PA.UpdateRepeaterItemsSelectionType, PA.UpdateRepeaterItemsValue, PA.SetValueType, PA.ExplicitValue, PA.ControlValue, PA.DataSourceID,PA.IncludePleaseSelect,PA.MinimumValue,PA.MaximumValue,PA.DataMethod,PA.OtherDataMethod, D.LabelOnPrintable, PA.CustomActionCode From " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & "  PA left outer join " & DT_WEBRAD_CONTROLACTIONTYPES & " A on PA.Action = A.ID left outer join " & DT_WEBRAD_PROJECTCONTROLS & " C ON PA.TargetControl = C.ID left outer join " & DT_WEBRAD_CONTROLTYPES & "  T on C.ControlType = T.ID left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  D on T.DataType = D.ID Where TriggerControl = " & .Item("ID"), General.Variables.cnx)

                    For nCounter2 = 0 To ActionControlDT.Rows.Count - 1
                        CreateSpecificAction(CurrentRow, sPostbackActions, ActionControlDT.Rows(nCounter2), sControlDescription, sDataTextField, sDataValueField)

                        sPostbackActions.Append(vbCrLf)
                    Next

                    WriteListSelectionsAction(sPostbackActions, CurrentRow, sControlDescription)

                    If ParentIsRepeaterControl(.Item("ID")) Then
                        sPostbackActions.Append("End With" & vbCrLf)
                    End If


                    If NeedsTotalCalculated() Then
                        sPostbackActions.Append("CalculateTotal()" & vbCrLf)
                    End If

                    sPostbackActions.Append("End Sub" & vbCrLf & vbCrLf)

                    If Not ParentIsRepeaterControl(.Item("ID")) Then
                        sTriggerPostbackActions &= .Item("Prefix") & .Item("Name") & "_" & .Item("ActionMethod") & "(" & GetControlName(.Item("ID")) & ",Nothing)" & vbCrLf
                    End If
                End If
            End With
        End Sub

        Private Shared Sub WriteListSelectionsAction(ByRef actions As StringBuilder, CurrentRow As DataRow, sControlDescription As String)
            If CurrentRow.Item("ListSelections") = "1" Then
                With CurrentRow
                    Dim controlPrefix As String = If(.Item("LabelOnPrintable") = "1" And isPrintable, "lbl", .Item("Prefix"))
                    Dim controlReference As String = If(ParentIsRepeaterControl(.Item("ID")), "ctype(.FindControl(""" & controlPrefix & .Item("Name") & """)," & sControlDescription & ")", controlPrefix & .Item("Name"))
                    Dim selectedItemsReference As String = If(ParentIsRepeaterControl(.Item("ID")), "ctype(.FindControl(""lbl" & .Item("Name") & "SelectedItems""),Label)", "lbl" & .Item("Name") & "SelectedItems")

                    logger.Info("Writing list selections action")

                    Try
                        actions.Append(selectedItemsReference & ".Text = GetListOfSelectedValues(" & controlReference & ")" & vbCrLf)
                    Catch ex As Exception
                        logger.Error("Error writing list selections action:")
                        logger.Error(ex.ToString)
                    End Try
                End With
            End If
        End Sub

        Private Shared Sub CreateSpecificAction(ByRef CurrentRow As DataRow, ByRef sPostbackActions As StringBuilder, actionControlRow As DataRow, sControlDescription As String, sDataTextField As String, sDataValueField As String)
            Dim sTargetControlPrefix As String
            Dim sTriggerControlPrefix As String
            Dim sTargetControlReference As String
            Dim sTriggerControlReference As String

            With CurrentRow
                SetControlPrefixes(CurrentRow, actionControlRow, sTargetControlPrefix, sTriggerControlPrefix)
                SetControlReferences(CurrentRow, actionControlRow, sTargetControlPrefix, sTriggerControlPrefix, sControlDescription, sTargetControlReference, sTriggerControlReference)

                If actionControlRow.Item("Type") = "Custom" Then
                    sPostbackActions.Append(actionControlRow.Item("CustomActionCode") & vbCrLf)
                ElseIf actionControlRow.Item("Type") = "Visible" Or actionControlRow.Item("Type") = "Enabled" Then
                    If actionControlRow.Item("Type") = "Visible" Then
                        sTargetControlReference = UpdateTargetControlReference(actionControlRow)
                        sTriggerControlReference = If(ParentIsRepeaterControl(.Item("ID")), "ctype(.FindControl(""" & sTriggerControlPrefix & .Item("Name") & """)," & sControlDescription & ")", sTriggerControlPrefix & .Item("Name"))
                    End If

                    sPostbackActions.Append("Try" & vbCrLf)
                    sPostbackActions.Append(sTargetControlReference & "." & actionControlRow.Item("Type") & " = If(" & GetTriggerValues(actionControlRow.Item("ID"), sTriggerControlReference, actionControlRow.Item("ValueAttribute")) & ",True,False)" & vbCrLf)
                    sPostbackActions.Append("Catch ex As Exception" & vbCrLf)
                    sPostbackActions.Append(sTargetControlReference & "." & actionControlRow.Item("Type") & " = False" & vbCrLf)
                    sPostbackActions.Append("End Try" & vbCrLf)
                ElseIf actionControlRow.Item("Type") = "SetValue" Then
                    Dim sValueReference As String

                    If actionControlRow.Item("SetValuetype") = "Explicit" Then
                        sValueReference = """" & actionControlRow.Item("ExplicitValue") & """"
                    ElseIf actionControlRow.Item("SetValuetype") = "Database" Then
                        sValueReference = "GetDataTable(""" & GetDataSourceSelectString(actionControlRow.Item("DataSourceID")) & """,cnx).Rows(0).Item(""" & actionControlRow.Item("ExplicitValue") & """)"
                    ElseIf actionControlRow.Item("SetValuetype") = "Control" Then
                        'sValueReference = GetControlValueReference(ActionControlRow.Item("ControlValue"))
                        sValueReference = GetControlValueReference(CurrentRow)
                    End If

                    'Possible source of Bug - problem with allowing checks of SelectedValue = 1 to go through?
                    'If GetTriggerValues(ActionControlRow.Item("ID"), sTriggerControlName, ActionControlRow.Item("ValueAttribute")) <> sTriggerControlName & ".SelectedValue = ""1"" " Then
                    sPostbackActions.Append("If " & GetTriggerValues(actionControlRow.Item("ID"), sTriggerControlReference, actionControlRow.Item("ValueAttribute")) & vbCrLf)
                    'End If

                    If IsListControlType(actionControlRow.Item("ControlType")) Then
                        sPostbackActions.Append("Set" & If(IsMultiValuedListControlType(actionControlRow.Item("ControlType")), "ListControl", "") & "ItemSelected(" & sTargetControlReference & "," & sValueReference & ")" & vbCrLf)
                    Else
                        sPostbackActions.Append(sTargetControlReference & ".Text = " & sValueReference & vbCrLf)
                    End If

                    sPostbackActions.Append("End If" & vbCrLf & vbCrLf)
                Else
                    If sControlDescription = "Dropdownlist" Then
                        Select Case actionControlRow.Item("Type")
                            Case "UpdateRepeaterItems"
                                Dim sItems As String

                                If actionControlRow.Item("UpdateRepeaterItemsSelectionType") = "Explicit" Then
                                    sItems = actionControlRow.Item("UpdateRepeaterItemsValue")
                                Else
                                    sItems = sTriggerControlReference & ".selecteditem.value"
                                End If

                                GetUpdateRepeaterItems(sPostbackActions, actionControlRow, sItems, sTargetControlReference, sTriggerControlReference)
                            'Bug Alert - this might end up creating multiple dtSupplied DataTable objects in the same scope
                            Case "FillListData"
                                Dim sSelectString As String = GetDataSourceSelectString(actionControlRow.Item("DataSourceID"), sDataTextField, sDataValueField)

                                If GetTriggerValues(actionControlRow.Item("ID"), sTriggerControlReference, actionControlRow.Item("ValueAttribute")) <> sTriggerControlReference & ".SelectedValue = ""1"" " Then
                                    sPostbackActions.Append("If " & GetTriggerValues(actionControlRow.Item("ID"), sTriggerControlReference, actionControlRow.Item("ValueAttribute")) & vbCrLf)
                                End If

                                sPostbackActions.Append("FillListData(" & sTargetControlReference & ",GetDataTable(""" & sSelectString & """),""" & sDataTextField & """,""" & sDataValueField & """," & CBool(actionControlRow.Item("IncludePleaseSelect")) & If(actionControlRow.Item("IncludePleaseSelect") = "1", ",""""", "") & ")" & vbCrLf)

                                If GetTriggerValues(actionControlRow.Item("ID"), sTriggerControlReference, actionControlRow.Item("ValueAttribute")) <> sTriggerControlReference & ".SelectedValue = ""1"" " Then
                                    sPostbackActions.Append("End If" & vbCrLf & vbCrLf)
                                End If
                        End Select
                    ElseIf sControlDescription = "Radiobuttonlist" Then
                        If actionControlRow.Item("Type") = "ResetSelectedIndex" Then
                            If GetTriggerValues(actionControlRow.Item("ID"), sTriggerControlReference, actionControlRow.Item("ValueAttribute")) <> sTriggerControlReference & ".SelectedValue = ""1"" " Then
                                sPostbackActions.Append("If " & GetTriggerValues(actionControlRow.Item("ID"), sTriggerControlReference, actionControlRow.Item("ValueAttribute")) & vbCrLf)
                            End If

                            sPostbackActions.Append(sTargetControlReference & ".SelectedIndex = 0" & vbCrLf)

                            If GetTriggerValues(actionControlRow.Item("ID"), sTriggerControlReference, actionControlRow.Item("ValueAttribute")) <> sTriggerControlReference & ".SelectedValue = ""1"" " Then
                                sPostbackActions.Append("End If" & vbCrLf & vbCrLf)
                            End If
                        ElseIf actionControlRow.Item("Type") = "UpdateRepeaterItems" Then
                            GetUpdateRepeaterItems(sPostbackActions, actionControlRow, actionControlRow.Item("UpdateRepeaterItemsValue"), sTargetControlReference, sTriggerControlReference)
                        ElseIf actionControlRow.Item("Type") = "FillListData" Then
                            Dim sSelectString As String = GetDataSourceSelectString(actionControlRow.Item("DataSourceID"), sDataTextField, sDataValueField)

                            If GetTriggerValues(actionControlRow.Item("ID"), sTriggerControlReference, actionControlRow.Item("ValueAttribute")) <> sTriggerControlReference & ".SelectedValue = ""1"" " Then
                                sPostbackActions.Append("If " & GetTriggerValues(actionControlRow.Item("ID"), sTriggerControlReference, actionControlRow.Item("ValueAttribute")) & vbCrLf)
                            End If

                            sPostbackActions.Append("FillListData(" & sTargetControlReference & ",GetDataTable(""" & sSelectString & """),""" & sDataTextField & """,""" & sDataValueField & """," & CBool(actionControlRow.Item("IncludePleaseSelect")) & If(.Item("IncludePleaseSelect") = "1", ",""""", "") & ")" & vbCrLf)

                            If GetTriggerValues(actionControlRow.Item("ID"), sTriggerControlReference, actionControlRow.Item("ValueAttribute")) <> sTriggerControlReference & ".SelectedValue = ""1"" " Then
                                sPostbackActions.Append("End If" & vbCrLf & vbCrLf)
                            End If
                        End If
                    End If
                End If
            End With
        End Sub

        Private Shared Sub SetControlReferences(ByVal controlInfo As DataRow, actionControlRow As DataRow, sTargetControlPrefix As String, sTriggerControlPrefix As String, sControlDescription As String, ByRef sTargetControlReference As String, ByRef sTriggerControlReference As String)
            With controlInfo
                sTargetControlReference = If(ParentIsRepeaterControl(actionControlRow.Item("TargetControl")), "ctype(.FindControl(""" & sTargetControlPrefix & actionControlRow.Item("Name") & """)," & GetDataTypeDescription(actionControlRow.Item("DataType")) & ")", sTargetControlPrefix & actionControlRow.Item("Name"))
                sTriggerControlReference = If(ParentIsRepeaterControl(.Item("ID")), "ctype(.FindControl(""" & sTriggerControlPrefix & .Item("Name") & """)," & sControlDescription & ")", sTriggerControlPrefix & .Item("Name"))
            End With

        End Sub

        Private Shared Sub SetControlPrefixes(ByVal controlInfo As DataRow, actionControlRow As DataRow, ByRef sTargetControlPrefix As String, ByRef sTriggerControlPrefix As String)
            With controlInfo
                sTargetControlPrefix = If(actionControlRow.Item("LabelOnPrintable") = "1" And isPrintable, "lbl", actionControlRow.Item("Prefix"))
                sTriggerControlPrefix = If(.Item("LabelOnPrintable") = "1" And isPrintable, "lbl", .Item("Prefix"))
            End With

        End Sub

        Shared Function NeedsTotalCalculated() As Boolean
            Return IseCommerceProject() And isFrontend And pageNumber = -1
        End Function

        Shared Sub GetUpdateRepeaterItems(ByRef sPostbackActions As StringBuilder, ByVal rAction As DataRow, ByVal sValueReference As String, ByVal sTargetControlName As String, ByVal sTriggerControlName As String)
            Dim sDTSupplied As String

            If GetTriggerValues(rAction.Item("ID"), sTriggerControlName, rAction.Item("ValueAttribute")) <> sTriggerControlName & ".SelectedValue = ""1"" " Then
                sPostbackActions.Append("If " & GetTriggerValues(rAction.Item("ID"), sTriggerControlName, rAction.Item("ValueAttribute")) & vbCrLf)
            End If

            GetSuppliedData(sDTSupplied, sPostbackActions, rAction("TargetControl"))

            If sDTSupplied <> "" Then
                sPostbackActions.Append(sDTSupplied & vbCrLf)

                sPostbackActions.Append("UpdateRepeaterItems(" & sTargetControlName & "," & sValueReference & ",dtSupplied,""-1"",-1,cnx)")
            Else
                sPostbackActions.Append("UpdateRepeaterItems(" & sTargetControlName & "," & sValueReference & ")")
            End If

            sDTSupplied = ""

            Dim sGetBindRepeaterData As String = ""

            GetBindRepeaterData(rAction("TargetControl"), "", sGetBindRepeaterData, "", "", 1, True)

            sPostbackActions.Append(sGetBindRepeaterData)

            If GetTriggerValues(rAction.Item("ID"), sTriggerControlName, rAction.Item("ValueAttribute")) <> sTriggerControlName & ".SelectedValue = ""1"" " Then
                sPostbackActions.Append("End If" & vbCrLf & vbCrLf)
            End If
        End Sub

        Shared Function GetTriggerValues(ByVal nActionID As Integer, ByVal sControlReference As String, ByVal sValueAttribute As String) As String
            Dim sTriggerValues, sValueReference As String
            Dim dtTriggerValues As DataTable = GetDataTable("Select TV.*, (SELECT SQLDataType FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ID = (SELECT TriggerControl FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE ID = " & nActionID & ")) as TriggerControlSQLType From " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONTRIGGERVALUES & " TV Where ActionID = " & nActionID)

            If dtTriggerValues.Rows.Count = 0 Then
                sValueReference = sControlReference & "." & ProcessTriggerValue(sValueAttribute, "1")

                Return sValueReference & " = ""1"" "
            ElseIf dtTriggerValues.Rows.Count = 1 Then
                sValueReference = sControlReference & "." & ProcessTriggerValue(sValueAttribute, dtTriggerValues.Rows(0).Item("TriggerValue"))

                Return sValueReference & " " & dtTriggerValues.Rows(0).Item("TriggerOperator") & " " & GetTriggerValueQuotes(dtTriggerValues.Rows(0).Item("TriggerControlSQLType")) & If(InStr(sValueAttribute, "TriggerValue"), "True", dtTriggerValues.Rows(0).Item("TriggerValue")) & GetTriggerValueQuotes(dtTriggerValues.Rows(0).Item("TriggerControlSQLType")) & " "
            Else
                For Each CurrentRow As DataRow In dtTriggerValues.Rows
                    If sTriggerValues <> "" Then
                        sTriggerValues &= " OR "
                    End If

                    sValueReference = sControlReference & "." & ProcessTriggerValue(sValueAttribute, CurrentRow.Item("TriggerValue"))

                    sTriggerValues &= sValueReference & " " & CurrentRow.Item("TriggerOperator") & " " & GetTriggerValueQuotes(CurrentRow.Item("TriggerControlSQLType")) & If(InStr(sValueAttribute, "TriggerValue"), "True", CurrentRow.Item("TriggerValue")) & "" & GetTriggerValueQuotes(CurrentRow.Item("TriggerControlSQLType"))
                Next

                Return "(" & sTriggerValues & ")"
            End If
        End Function

        Shared Function GetTriggerValueQuotes(ByVal nSQLType As Integer) As String
            Return """"
            'This might need to use quotes for all values since comparison value from control is always string?
            'Return If(nSQLType = N_INTEGER_SQLTYPE Or nSQLType = N_FLOAT_SQLTYPE, "", """")
        End Function

        Shared Function ProcessTriggerValue(ByVal sValueAttribute As String, ByVal sTriggerValue As String) As String
            If InStr(sValueAttribute, "TriggerValue") Then
                Return Replace(sValueAttribute, "TriggerValue", sTriggerValue)
            Else
                Return sValueAttribute
            End If
        End Function

        Shared Function GetControlPostbackActionReference(ByRef CurrentRow As DataRow, ByVal sPrefix As String)
            Dim dtDataType As DataTable = GetDataTable("Select * from " & DT_WEBRAD_CONTROLDATATYPES & "  Where ID = (Select DataType From " & DT_WEBRAD_CONTROLTYPES & "  Where ID = " & CurrentRow.Item("ControlType") & ")", General.Variables.cnx)

            Return If(dtDataType.Rows(0).Item("ActionMethod") <> "", " On" & dtDataType.Rows(0).Item("ActionMethod") & "=""" & GetRepeaterHandlerReference(CurrentRow.Item("ID")) & sPrefix & CurrentRow.Item("Name") & "_" & dtDataType.Rows(0).Item("ActionMethod") & """", "")
        End Function

        ''' <summary>
        ''' Currently the only allowed repeater action is custom code.  This method will need to be updated
        ''' if other actions are allowed.
        ''' </summary>
        ''' <param name="nID"></param>
        ''' <returns></returns>
        Public shared Function GetRepeaterAction(byval nID as integer) as string
            dim sAction as string
            dim dtAction as datatable = getdatatable("SELECT * FROM " & dt_webrad_projectcontrolpostbackactions & " WHERE TriggerControl = " & nid)

            for each CurrentRow as datarow in dtaction.rows
                saction &= currentrow.item("CustomActionCode") & vbcrlf
            Next
            
            if NeedsTotalCalculated()
                sAction &= "CalculateTotal()" & vbCrLf
        End If    

            return sAction
        End Function

        Private Shared Function UpdateTargetControlReference(byval currentRow as datarow) As String
            Dim sTargetControlReference As String

            dim sTargetControlName, sTargetControlDataType as string

            sTargetControlDataType = "Panel"

            with currentRow
                if GetControlColumnValue(.Item("TargetControl"),"ControlType") = N_PANEL_CONTROL_TYPE
                    sTargetControlname = "pnl" & .Item("Name")
                else
                    sTargetControlname = .Item("Name") & "Container"
            '    sTargetControlDataType = GetDataTypeDescription(.Item("DataType"))
            End If

            sTargetControlReference = If(ParentIsRepeaterControl(.Item("TargetControl")), "ctype(.FindControl(""" & sTargetControlName & """)," & sTargetControlDataType & ")", stargetcontrolname)
            End With
            
            Return sTargetControlReference
        End Function

        End Class
End Namespace
