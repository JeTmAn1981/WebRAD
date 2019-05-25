Imports System.Data
Imports Common.Actions.HandlerWriter
Imports DocumentFormat.OpenXml
Imports Microsoft.VisualBasic
Imports Common.General
Imports WhitTools.Utilities
Imports WhitTools.DataTables
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Repeaters
Imports Common.General.Controls
Imports Common.General.ControlTypes
Imports Common.General.Ancillary
Imports Common.General.Pages
Imports Common.General.DataTypes
Imports Common.General.DataSources
Imports Common.Webpages.BindData
Imports Common.General.ProjectOperations

Namespace Actions
    Public Class Main
        Shared Private  control as datarow

         Shared Function ActionRequiresListItems(ByVal nActionType As String) As Boolean
            Return If(nActionType = "5", True, False)
        End Function

         Shared Function ActionRequiresValueSelection(ByVal nActionType As String) As Boolean
            Return If(nActionType = "6", True, False)
        End Function

         Shared Sub AddActionHandlers()
            InitializeActionData()

            Dim selectstring As String = "Select C.ID, C.PageID, C.Name,C.ControlType, C.ListSelections, T.DataType, D.Prefix, D.Description, D.JSActionMethod, D.ActionMethod, C.ParentControlID,C.DisplayLocation, SelectionItems, DataMethod,OtherDataMethod,TextField,ValueField,MinimumValue,MaximumValue, D.LabelOnPrintable From " & DT_WEBRAD_PROJECTCONTROLS & " C left outer join " & DT_WEBRAD_CONTROLTYPES & "  T on C.ControlType = T.ID left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  D on T.DataType = D.ID LEFT OUTER JOIN " & DT_WEBRAD_PROJECTDATASOURCES & " DS ON DS.ID = C.DataSourceID Where ProjectID = " & GetProjectID() & " and " & ACTION_REQUIRED_SQL_CONDITION
            Dim fdjkls

            fdjkls = ""

            For Each Currentrow As DataRow In GetDataTable(selectstring, Common.General.Variables.cnx).Rows
                If BelongsToPage(pageNumber, Currentrow.Item("PageID")) Then
                    AddActionHandler(Currentrow)
                End If
            Next

            AddBackendInsertActionHandlers()
        End Sub

        Shared Sub InitializeActionData()
            Variables.projectActionData = New Actions.ActionData()
        End Sub

        Private Shared Sub AddBackendInsertActionHandlers()
            If isInsert And CurrentProjectRequiresWhitworthLogin() Then
                General.Variables.projectActionData.postback.handlers.Append(vbCrLf & vbCrLf)
                General.Variables.projectActionData.postback.handlers.Append(GetCurrentUsernameOverload())

                AddUsernameChangedMethod()
                AddLoadPersonalInfoMethod()
            End If
        End Sub

        Private Shared Sub AddLoadPersonalInfoMethod()
            With Common.General.Variables.projectActionData
                .postback.handlers.Append("    Sub LoadPersonalInfo()" & vbCrLf)
                .postback.handlers.Append("        Dim dtPersonalInfo As DataTable = GetUserInfo(CleanSQL(GetQueryString(""Username"")))" & vbCrLf & vbCrLf)

                .postback.handlers.Append("        If dtPersonalInfo.Rows.Count > 0 Then" & vbCrLf)
                .postback.handlers.Append("            With dtPersonalInfo.Rows(0)" & vbCrLf)
                .postback.handlers.Append("                Try" & vbCrLf)

                Dim dtLoginColumns As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCOLUMNS & " PCL LEFT OUTER JOIN " & DT_WEBRAD_LOGINCOLUMNTYPES & " LCT ON PCL.ColumnControlID = LCT.IDNumber WHERE Type='Login' AND ProjectID = " & GetProjectID())

                For Each CurrentRow As DataRow In dtLoginColumns.Rows
                    .postback.handlers.Append("lbl" & CurrentRow.Item("ControlName") & ".Text = .Item(""" & CurrentRow.Item("ControlName") & """)" & vbCrLf)
                Next

                .postback.handlers.Append("                    txtUser.Text = getdatatable(""SELECT * FROM adTelephone.dbo.UserInfo_V WHERE Username = '"" & CleanSQL(GetQueryString(""Username"")) & ""'"", cnx).rows(0).Item(""User"")" & vbCrLf)
                .postback.handlers.Append("                Catch ex As Exception" & vbCrLf)
                .postback.handlers.Append("                End Try" & vbCrLf)
                .postback.handlers.Append("            End With" & vbCrLf)
                .postback.handlers.Append("        End If" & vbCrLf)
                .postback.handlers.Append("    End Sub" & vbCrLf & vbCrLf)
            End With
        End Sub

        Private Shared Sub AddUsernameChangedMethod()
            With Common.General.Variables.projectActionData
                .postback.handlers.Append("    Protected Sub txtUser_TextChanged(sender As Object, e As EventArgs)" & vbCrLf)
                .postback.handlers.Append("dim username as string = ExtractUsername(txtuser.text)" & vbCrLf & vbCrLf)
                .postback.handlers.Append("        If GetUserInfo(CleanSQL(username)).Rows.Count > 0 Then" & vbCrLf)
                .postback.handlers.Append($"            Redirect(""insert{GetAncillaryName()}.aspx?Username="" & username)" & vbCrLf)
                .postback.handlers.Append("        End If" & vbCrLf)
                .postback.handlers.Append("    End Sub" & vbCrLf & vbCrLf)
            End With
        End Sub

        Private Shared Sub AddActionHandler(ByVal CurrentRow As DataRow)
            Dim sParentControlID As String
            Dim bDisplay As Boolean = True

            With CurrentRow
                Main.control = CurrentRow

                If ParentIsRepeaterControl(.Item("ID"), -1, 0, sParentControlID) Then
                    Try
                        bDisplay = ControlDisplayAllowed(GetControlColumnValue(sParentControlID, "DisplayLocation", controlsDT))
                    Catch ex As Exception
                        'WriteLine("parnet control - " & sParentControlID)
                    End Try
                End If
            End With

            If bDisplay Then
                CreateHandlers()
            End If

            bDisplay = True
        End Sub

        Private Shared Sub CreateHandlers()
            If PostbackHandlerRequired(control.Item("ID")) Then
                CreatePostbackHandler()
            End If

            If JSHandlerRequired() Then
                CreateJSHandler()
            End If
        End Sub

        Shared Sub CreatePostbackHandler()
            Dim pbwriter As ActionHandlerWriter = New PostbackActionHandlerWriter(control)
            pbwriter.WriteActionHandler()
        End Sub

        Shared Sub CreateJSHandler()
            Dim pbwriter As ActionHandlerWriter = New JSActionHandlerWriter(control)
            pbwriter.WriteActionHandler()
        End Sub

        Public Shared Function PostbackHandlerRequired(ByVal controlID As Integer) As Boolean
            WriteLine("list selections for control ID " & controlID & " - " & GetControlColumnValue(controlID, "ListSelections"))
            If GetControlColumnValue(controlID, "ListSelections") = "1" Then
                Return True
            ElseIf GetControlColumnValue(controlID, "PerformPostbackAction") = "1" Then
                Return GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLACTIONTYPES & " WHERE UseJavascript = 0 AND ID IN (SELECT Action FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE TriggerControl = " & controlID & ")").Rows.Count > 0
            End If
        End Function

        Shared Function JSHandlerRequired() As Boolean
            Return GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLACTIONTYPES & " WHERE UseJavascript = 1 AND ID IN (SELECT Action FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE TriggerControl = " & control.Item("ID") & ")").Rows.Count > 0
        End Function

        Shared Function NeedsTotalCalculated() As Boolean
            Return IseCommerceProject() And isFrontend And pageNumber = -1
        End Function


        ' Shared Function GetControlPostbackActionReference(ByRef CurrentRow As DataRow, ByVal sPrefix As String, byval sName as string) As string
        '    dim handlerReference As string = ""

        '    with currentrow
        '      If .Item("ListSelections") = "1" Then
        '                handlerreference = " OnSelectedIndexChanged=""" & sPrefix & sName & "_SelectedIndexChanged"" "
        '      elseIf PostbackHandlerRequired(currentrow.Item("ID")) andalso CurrentRow.Item("PerformPostbackAction") = "1" andalso getcontroldatatype(currentrow.item("ControlType")) <> N_REPEATER_DATA_TYPE Andalso GetDataTable("Select * From " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & "  Where TriggerControl = " & CurrentRow.Item("ID"), Common.General.Variables.cnx).Rows.Count > 0  Then
        '            Dim dtDataType As DataTable = GetDataTable("Select * from " & DT_WEBRAD_CONTROLDATATYPES & "  Where ID = (Select DataType From " & DT_WEBRAD_CONTROLTYPES & "  Where ID = " & CurrentRow.Item("ControlType") & ")", General.Variables.cnx)

        '            handlerreference = " On" & dtDataType.Rows(0).Item("ActionMethod") & "=""" & GetRepeaterHandlerReference(CurrentRow.Item("ID")) & sPrefix & CurrentRow.Item("Name") & "_" & dtDataType.Rows(0).Item("ActionMethod") & """"
        '       End If
        '    End With

        '    return handlerReference
        'End Function

        ''' <summary>
        ''' Currently the only allowed repeater action is custom code.  This method will need to be updated
        ''' if other Main are allowed.
        ''' </summary>
        ''' <param name="nID"></param>
        ''' <returns></returns>
        Shared public  Function GetRepeaterAction(byval nID as integer) as string
            dim sAction as string
            dim dtAction as datatable = getdatatable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE TriggerControl = " & nid)

            for each CurrentRow as datarow in dtaction.rows
                saction &= currentrow.item("CustomActionCode") & vbcrlf
            Next
            
            if NeedsTotalCalculated()
                sAction &= "CalculateTotal()" & vbCrLf
        End If    

            return sAction
        End Function

        Shared Public  Function ControlRequiresPostbackAction(ByVal id as integer) as boolean
                return GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLACTIONTYPES &  " WHERE UseJavascript = 0 AND ID IN (SELECT Action From " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE TriggerControl = " & id & ")").Rows.Count > 0
        End Function

        Shared Public  Function ControlRequiresJSAction(ByVal id as integer) as boolean
            return GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLACTIONTYPES &  " WHERE UseJavascript = 1 AND ID IN (SELECT Action From " & DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS & " WHERE TriggerControl = " & id & ")").Rows.Count > 0
        End Function

        Shared public  Function ActionUsesJavaScript(action as integer) As boolean
            return getdatatable("SELECT * FROM " & dt_webrad_controlactiontypes & " WHERE ID = " & action & " And UseJavascript = 1").rows.count > 0
            'return db.ControlActionTypes.Any(function(actionType) actionType.ID = action And actiontype.UseJavascript = 1)
        End Function
     End Class

End Namespace

