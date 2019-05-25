Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Linq
Imports WhitTools.DataWarehouse
Imports WhitTools.eCommerce
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports WhitTools.File
Imports WhitTools.Filler

Imports WhitTools.DataTables
Imports WhitTools.Converter
Imports WhitTools.Validator
Imports WhitTools.Formatter
Imports WhitTools.GlobalEnum
Imports WhitTools.Repeaters
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.WebTeam
Imports WhitTools.SQL
Imports WhitTools.Encryption
Imports WhitTools.RulesAssignments
Imports WhitTools.Workflow
Imports Common.General.Ancillary
Imports Common.General.Assembly
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.DataSources
Imports Common.General.Repeaters
Imports Common.General.File
Imports Common.General.ProjectOperations
Imports Common.General.ControlTypes
Imports Common.General.Controls
Imports Common.General.Folders
Imports Common.General.Links
Imports Common.General.DataTypes
Imports Common.General.Actions
Imports Common.General.Pages
Imports Common.SQL.Main
Imports Common.BuildSetup
Imports Common.Webpages.BindData
Imports Common.Webpages.Frontend.Main
Imports Common.Webpages.Frontend.MultiPage
Imports Common.Webpages.Backend.Main
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.Backend.Search
Imports System.Threading
Imports System.Reflection


Imports WhitTools.Utilities
Namespace Webpages
    Public Class BindData
        Inherits Webpages.Main

        Shared Function GetBindData(ByRef sGetBindData As String, ByRef sGetBindDataAdditional As String, ByRef sGetBindRepeaterData As String, ByRef sCheckControlMethods As String,optional byval sIdentity as string = "Request.Querystring(""ID"")") As String
            Dim nCounter As Integer
            Dim bFoundRepeater As Boolean = False
            Dim sTriggerPostbackActions As String
            Dim getBindDataBuilder As New StringBuilder()

            sGetBindData = ""
            sGetBindDataAdditional = ""
            sGetBindRepeaterData = ""
            sCheckControlMethods = ""

            If pageNumber = -1 Then
                GetBindDataAssignments(getBindDataBuilder, sGetBindDataAdditional, sGetBindRepeaterData, sCheckControlMethods, sIdentity)
            ElseIf pageNumber = 0 Then
                getBindDataBuilder.Append("CheckAlreadySubmitted(" & GetUsernameReference(GetAncillaryProject("RequireLogin")) & ")" & vbCrLf & vbCrLf)
                getBindDataBuilder.Append("Dim dtMain As DataTable = GetDataTable(""Select * FROM " & projectDT.Rows(0).Item("SQLMainTableName") & " WHERE " & GetCertificationCondition() & " AND Username = '"" & " & GetUsernameReference(GetAncillaryProject("RequireLogin")) & " & ""'"", Common.cnx)" & vbCrLf & vbCrLf)
                getBindDataBuilder.Append("With dtMain.Rows(0)" & vbCrLf)
                For nCounter = 1 To GetPageCount()
                    getBindDataBuilder.Append("lblSection" & nCounter & ".Text = FormatYesNo(.Item(""Section" & nCounter & "Complete""))" & vbCrLf)
                Next

                If DefaultCertificationPage() Then
                    getBindDataBuilder.Append("lblCertification.Text = FormatYesNo(.Item(""Certification""))" & vbCrLf)
                End If

                getBindDataBuilder.Append("End With" & vbCrLf)
            Else
                getBindDataBuilder.Append("Sub BindData()" & vbCrLf)
                getBindDataBuilder.Append("Dim dt As New DataTable" & vbCrLf)
                getBindDataBuilder.Append("dt = getdatatable(""Select * From " & projectDT.Rows(0).Item("SQLMainTableName") & " Where " & GetCertificationCondition() & " AND Username = '"" & " & GetUsernameReference(GetAncillaryProject("RequireLogin")) & " & ""'"",Common.cnx, True, """",,,,,,, True)" & vbCrLf)
                getBindDataBuilder.Append("If dt.rows.count > 0 Then " & vbCrLf)
                getBindDataBuilder.Append("With dt.rows(0)" & vbCrLf)

                GetBindDataAssignments(getBindDataBuilder, sGetBindDataAdditional, sGetBindRepeaterData, sCheckControlMethods, ".item(""ID"")")

                getBindDataBuilder.Append(sGetBindRepeaterData & vbCrLf)

                GetPostbackActions(New StringBuilder(), sTriggerPostbackActions)

                getBindDataBuilder.Append(sGetBindDataAdditional)
                getBindDataBuilder.Append(sTriggerPostbackActions & vbCrLf)
                getBindDataBuilder.Append("End With" & vbCrLf)
                getBindDataBuilder.Append("Else" & vbCrLf)
                getBindDataBuilder.Append("redirect(""status.aspx"")" & vbCrLf)
                getBindDataBuilder.Append("End If" & vbCrLf)
                getBindDataBuilder.Append("End Sub" & vbCrLf)
            End If

            sGetBindData = getBindDataBuilder.ToString()

            Return sGetBindData
        End Function

        Shared Sub GetBindDataAssignments(ByRef sGetBindData As StringBuilder, ByRef sGetBindDataAdditional As String, ByRef sGetBindRepeaterData As String, ByRef sCheckControlMethods As String, Optional ByVal sIdentity As String = "Request.QueryString(""ID"")")
            Dim bFoundRepeater As Boolean = False
            Dim sCurrentPrefix As String

            GetPersonalInfoBindData(sGetBindData)

            For nCounter = 0 To controlsDT.Rows.Count - 1
                With controlsDT.Rows(nCounter)
                    If ControlDisplayAllowed(.Item("DisplayLocation")) And Not ParentIsRepeaterControl(.Item("ID"), "-1", 0, "", "", "", True) And BelongsToPage(pageNumber, .Item("PageID")) Then
                        If .Item("Prefix") = "rpt" And .Item("IncludeDatabase") = "1" Then
                            If Not bFoundRepeater Then
                                sGetBindRepeaterData &= "dim dtSupplied as WhitTools.DataTablesSupplied" & vbCrLf
                            End If

                            bFoundRepeater = True

                            GetBindRepeaterData(.Item("ID"), .Item("ForeignID"), sGetBindRepeaterData, sCheckControlMethods, sIdentity)
                        ElseIf .Item("IncludeDatabase") = "1" Then
                            sCurrentPrefix = .Item("Prefix")

                            Dim sCurrentData As New StringBuilder()

                            If .Item("SQLInsertItemTable") = "" Then
                                If sCurrentPrefix = "txt" Or sCurrentPrefix = "lbl" Then
                                    If IsControlType(.Item("ControlType"), "Date") Then
                                        sCurrentData.Append("Try" & vbCrLf)
                                        sCurrentData.Append(If(isPrintable = True And .Item("LabelOnPrintable") = "1", "lbl", sCurrentPrefix) & .Item("Name") & ".Text = CType(.Item(""" & .Item("Name") & """), DateTime).ToShortDateString" & vbCrLf)
                                        sCurrentData.Append("Catch ex as Exception" & vbCrLf)
                                        sCurrentData.Append("End Try" & vbCrLf)
                                    Else
                                        sCurrentData.Append(If(isPrintable = True And .Item("LabelOnPrintable") = "1", "lbl", sCurrentPrefix) & .Item("Name") & ".Text = " & If(isPrintable, "FormatPrintableText(", "") & ".item(""" & .Item("Name") & """)" & If(isPrintable, ")", "") & vbCrLf)
                                    End If
                                ElseIf sCurrentPrefix = "ddl" Or sCurrentPrefix = "rbl" Then
                                    sCurrentData.Append("SetItemSelected(" & sCurrentPrefix & .Item("Name") & ", .item(""" & .Item("Name") & """))")
                                ElseIf sCurrentPrefix = "chk" Or sCurrentPrefix = "rad" Then
                                    sCurrentData.Append(sCurrentPrefix & .Item("Name") & ".Checked = GetBoolean(.item(""" & .Item("Name") & """))")
                                ElseIf IsFileUploadControl(.Item("ControlType")) Then
                                    sCurrentData.Append(GetUploadBindData(controlsDT.Rows(nCounter), sIdentity, ""))
                                End If
                            Else
                                sCurrentData.Append("SetListControlItemSelected(" & sCurrentPrefix & .Item("Name") & ", """",True, GetDataTable(""Select * From " & archiveRef & .Item("SQLInsertItemTable") & " Where " & .Item("ForeignID") & " = '"" & " & sIdentity & " & ""'"",cnx),""" & .Item("Name") & """)" & vbCrLf)
                                ''Bug here, it's possible to select options which will cause multiple duplicate event handlers for this control to appear.
                                ''For instance, selecting list selections but also creating an action for the control will do this.
                                'If .Item("ListSelections") = "1" And bSearch = False Then
                                '    sCurrentData.Append(sCurrentPrefix & .Item("Name") & "_SelectedIndexChanged(Nothing,Nothing)" & vbCrLf)                                'End If
                            End If

                            sCurrentData.Append(vbCrLf)
                            'Binding controlData for list controls with no items supplied comes later than it does for other controls.
                            'The reasoning is that actions triggered by other controls may supply items to the control
                            'in question and so other controls need a chance to have their actions triggered first.

                            If BindDataAdditionalRequired(controlsDT.Rows(nCounter)) Then
                                sGetBindDataAdditional &= sCurrentData.ToString()

                                If .Item("PerformPostbackAction") = "1" Then
                                    Dim controlActions As String
                                    CreatePostbackAction(GetDataTable("Select C.ID, C.PageID, C.Name,C.ControlType, T.DataType, D.Prefix, D.Description, D.ActionMethod, C.ParentControlID,C.DisplayLocation, C.IncludePleaseSelect,SelectionItems,listSelections, DataMethod,OtherDataMethod,TextField,ValueField,MinimumValue,MaximumValue, D.LabelOnPrintable From " & DT_WEBRAD_PROJECTCONTROLS & " C left outer join " & DT_WEBRAD_CONTROLTYPES & "  T on C.ControlType = T.ID left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  D on T.DataType = D.ID LEFT OUTER JOIN " & DT_WEBRAD_PROJECTDATASOURCES & " DS ON DS.ID = C.DataSourceID Where C.ID = " & .Item("ID")).Rows(0), New StringBuilder(), controlActions)
                                    sGetBindDataAdditional &= controlActions
                                End If
                            Else
                                sGetBindData.Append(sCurrentData.ToString())
                            End If

                            sCurrentData = New StringBuilder()
                        End If
                    End If
                End With
            Next
        End Sub

        Private Shared Sub GetPersonalInfoBindData(ByRef getBindDataBuilder As StringBuilder)
            If CurrentProjectRequiresWhitworthLogin() And pageNumber <= 1 Then
                getBindDataBuilder.Append("Try" & vbCrLf)

                Dim loginColumns As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCOLUMNS & " PCL LEFT OUTER JOIN " & DT_WEBRAD_LOGINCOLUMNTYPES & " LCT ON PCL.ColumnControlID = LCT.IDNumber WHERE Type='Login' AND ProjectID = " & GetProjectID())

                If loginColumns.Rows.Count > 0 Then
                    getBindDataBuilder.Append("If lbl" & loginColumns.Rows(0).Item("ControlName") & ".Text = """"" & vbCrLf)

                    For Each CurrentRow As DataRow In loginColumns.Rows
                        getBindDataBuilder.Append("lbl" & CurrentRow.Item("ControlName") & ".Text = .Item(""" & CurrentRow.Item("ControlName") & """)" & vbCrLf)
                    Next

                    getBindDataBuilder.Append("End If" & vbCrLf)
                End If

                getBindDataBuilder.Append("Catch ex as Exception" & vbCrLf)
                getBindDataBuilder.Append("End Try" & vbCrLf)
            End If
        End Sub

        Private Shared Function BindDataAdditionalRequired(ByVal controlData As datarow ) As Boolean
            With controlData
                Return IsListControlType(.Item("ControlType")) And .Item("SelectionItems") = S_ITEMS_NONESUPPLIED
            End With
        End Function

        Shared Sub GetBindRepeaterData(ByVal nRepeaterID As Integer, ByVal sForeignID As String, ByRef sGetBindRepeaterData As String, ByRef sCheckControlMethods As String, ByVal sIdentity As String, Optional ByRef nCounterIndex As Integer = 1, Optional ByVal bEventHandler As Boolean = False)
            Dim bChildControlsFireEventHandlers, bChildControlsIncludesMultivalued As Boolean
            Dim sRepeaterReference As String
            Dim dtControl As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ID = " & nRepeaterID)
            Dim sRepeaterName, sSQLInsertItemTable, sDataSourceID As String

            With dtControl.Rows(0)
                sRepeaterName = .Item("Name")
                sSQLInsertItemTable = .Item("SQLInsertItemTable")
                sDataSourceID = .Item("DataSourceID")
            End With

            If nCounterIndex > 1 Then
                sIdentity = "CType(CurrentItem" & nCounterIndex - 1 & ".FindControl(""lblID""),Label).Text"
            End If

            If nCounterIndex > 1 Then
                sRepeaterReference = "CType(CurrentItem" & nCounterIndex - 1 & ".FindControl(""rpt" & sRepeaterName & """),Repeater)"
            Else
                If ParentIsRepeaterControl(nRepeaterID) Then
                    sRepeaterReference = "CType(.FindControl(""rpt" & sRepeaterName & """),Repeater)"
                Else
                    sRepeaterReference = "rpt" & sRepeaterName
                End If
            End If

            CheckChildControls(nRepeaterID, bChildControlsFireEventHandlers, bChildControlsIncludesMultivalued)

            'Possible bug: what kind of behavior should occur for upload controls during an update repeater items action event?
            'Currently this will not assign uploaded file controlData to the controls in the event of an action update.
            If Not bEventHandler Then
                Dim sDTSupplied As String = ""
                Dim sdtName As String = "dt" & RemoveNonAlphanumeric(sRepeaterName)

                sGetBindRepeaterData &= "Dim " & sdtName & " As DataTable = GetDataTable(""Select * From " & archiveRef & sSQLInsertItemTable & " Where " & sForeignID & " = '"" & " & sIdentity & " & ""'"",Common.cnx)" & vbCrLf & vbCrLf

                GetSuppliedData(sDTSupplied, New StringBuilder(), nRepeaterID, True)

                sGetBindRepeaterData &= sDTSupplied

                sGetBindRepeaterData &= "If " & sdtName & ".Rows.Count > 0 Then" & vbCrLf

                sGetBindRepeaterData &= "SelectRepeaterData(" & sRepeaterReference & ", GetDataTable(""Select * From " & archiveRef & sSQLInsertItemTable & " Where " & sForeignID & " = '"" & " & sIdentity & " & ""'"",Common.cnx)" & If(sDTSupplied <> "", ",dtSupplied", "") & ", Common.cnx)" & vbCrLf & vbCrLf
                sGetBindRepeaterData &= "End If" & vbCrLf

                Dim uploadChildren As List(Of DataRow) = getRepeaterUploadChildControls(nRepeaterID)

                If uploadChildren.Count > 0 Then
                    sGetBindRepeaterData &= "For " & RemoveNonAlphanumeric(sSQLInsertItemTable) & "Counter As Integer = 0 To " & sdtName & ".Rows.Count - 1" & vbCrLf
                    sGetBindRepeaterData &= "With " & sdtName & ".Rows(" & RemoveNonAlphanumeric(sSQLInsertItemTable) & "Counter)" & vbCrLf

                    For Each currentRow As DataRow In uploadChildren
                        sGetBindRepeaterData &= GetUploadBindData(currentRow, ".Item(""ID"")", sRepeaterReference & ".Items(" & RemoveNonAlphanumeric(sSQLInsertItemTable) & "Counter)")
                    Next

                    sGetBindRepeaterData &= "End With" & vbCrLf
                    sGetBindRepeaterData &= "Next" & vbCrLf
                End If
            End If

            If (bChildControlsFireEventHandlers Or bChildControlsIncludesMultivalued) Then
                AddEventHandlerTriggers(nRepeaterID, nCounterIndex, sRepeaterName, bChildControlsFireEventHandlers, bChildControlsIncludesMultivalued, bEventHandler, sGetBindRepeaterData, sIdentity, sRepeaterReference, sCheckControlMethods)
            End If
        End Sub

        Private Shared Sub AddEventHandlerTriggers(nRepeaterID As Integer, nCounterIndex As Integer, sRepeaterName As String, bChildControlsFireEventHandlers As Boolean, bChildControlsIncludesMultivalued As Boolean, bEventHandler As Boolean, ByRef sGetBindRepeaterData As String, sIdentity As String, sRepeaterReference As String, ByRef sCheckControlMethods As String)
            sGetBindRepeaterData &= vbCrLf
            sGetBindRepeaterData &= "For Each CurrentItem" & nCounterIndex & " as RepeaterItem in " & sRepeaterReference & ".Items" & vbCrLf

            If bChildControlsFireEventHandlers Then
                AddRunControlEvents(sGetBindRepeaterData, nCounterIndex, sRepeaterName, sCheckControlMethods, nRepeaterID)
            End If

            If bChildControlsIncludesMultivalued Then
                AddBindRepeaters(sGetBindRepeaterData, nCounterIndex, sCheckControlMethods, nRepeaterID, sIdentity, bEventHandler)
            End If

            sGetBindRepeaterData &= "Next" & vbCrLf
        End Sub

        Private Shared Sub AddBindRepeaters(ByRef sGetBindRepeaterData As String, nCounterIndex As Integer, ByRef sCheckControlMethods As String, nRepeaterID As Integer, sIdentity As String, bEventHandler As Boolean)

            For Each CurrentRow As DataRow In controlsDT.Rows
                With CurrentRow
                    If ControlTypeIsRepeater(.Item("ControlType")) And ParentIsRepeaterControl(.Item("ID"), nRepeaterID) Then
                        sGetBindRepeaterData &= vbCrLf

                        GetBindRepeaterData(.Item("ID"), .Item("ForeignID"), sGetBindRepeaterData, sCheckControlMethods, sIdentity, nCounterIndex + 1, bEventHandler)
                    ElseIf IsMultiValuedListControlType(.Item("ControlType")) And ParentIsRepeaterControl(.Item("ID"), nRepeaterID) Then
                        sGetBindRepeaterData &= vbCrLf

                        sGetBindRepeaterData &= "SetListControlItemSelected(CType(CurrentItem" & nCounterIndex & ".FindControl(""" & .Item("Prefix") & .Item("Name") & """), ListControl), """", True, GetDataTable(""SELECT * FROM " & .Item("SQLInsertItemTable") & " WHERE " & .Item("ForeignID") & " = "" & CType(CurrentItem" & nCounterIndex & ".FindControl(""lblID""),Label).Text, cnx), """ & .Item("Name") & """)" & vbCrLf
                    End If
                End With
            Next
        End Sub

        Private Shared Sub AddRunControlEvents(ByRef sGetBindRepeaterData As String, nCounterIndex As Integer, sRepeaterName As String, ByRef sCheckControlMethods As String, nRepeaterID As Integer)
            sGetBindRepeaterData &= "For Each CurrentControl" & nCounterIndex & " as Control in CurrentItem" & nCounterIndex & ".Controls" & vbCrLf
            sGetBindRepeaterData &= "Run" & sRepeaterName & "ControlEvents(CurrentControl" & nCounterIndex & ")" & vbCrLf

            CreateRunControlEventsMethod(sCheckControlMethods, nRepeaterID, sRepeaterName)

            sGetBindRepeaterData &= "Next" & vbCrLf
        End Sub

        Shared Sub CheckChildControls(ByVal nRepeaterID As Integer, ByRef bFireEventHandlers As Boolean, ByRef bIncludesMultivalued As Boolean)
            bFireEventHandlers = False
            bIncludesMultivalued = False

            For Each CurrentRow As DataRow In controlsDT.Rows
                With CurrentRow
                    If ParentIsRepeaterControl(.Item("ID"), nRepeaterID) Then
                        If ControlTypeIsRepeater(.Item("ControlType")) Or IsMultiValuedListControlType(.Item("ControlType")) Then
                            bIncludesMultivalued = True
                        End If

                        If .Item("PerformPostbackAction") = "1" Then
                            bFireEventHandlers = True
                        End If
                    End If
                End With
            Next
        End Sub

        Shared Sub CreateRunControlEventsMethod(ByRef sCheckControlMethods As String, ByVal nRepeaterID As Integer, ByVal sRepeaterName As String)
            Dim sControlList As String = ""
            Dim dtActions As DataTable

            sCheckControlMethods &= "Public Sub Run" & sRepeaterName & "ControlEvents(ByRef CurrentControl As Control)" & vbCrLf
            
            For nCounter As Integer = 0 To controlsDT.Rows.Count - 1
                If ParentIsRepeaterControl(controlsDT.Rows(nCounter).Item("ID"), nRepeaterID) And CStr(controlsDT.Rows(nCounter).Item("PerformPostbackAction")) = "1" Then
                    dtActions = GetDataTable("Select C.ID, C.Name, D.Prefix, D.Description, D.ActionMethod, C.ParentControlID,SelectionItems, DataMethod,OtherDataMethod,TextField,ValueField,MinimumValue,MaximumValue From " & DT_WEBRAD_PROJECTCONTROLS & " C left outer join " & DT_WEBRAD_CONTROLTYPES & "  T on C.ControlType = T.ID left outer join " & DT_WEBRAD_CONTROLDATATYPES & "  D on T.DataType = D.ID LEFT OUTER JOIN " & DT_WEBRAD_PROJECTDATASOURCES & " DS ON DS.ID = C.DataSourceID Where C.ID = " & controlsDT.Rows(nCounter).Item("ID"), General.Variables.cnx)

                    sControlList &= If(sControlList <> "", "Else", "") & "If CurrentControl.ID = """ & controlsDT.Rows(nCounter).Item("Prefix") & controlsDT.Rows(nCounter).Item("Name") & """ Then" & vbCrLf

                    For ncounter2 As Integer = 0 To dtActions.Rows.Count - 1
                        sControlList &= GetRepeaterHandlerReference(controlsDT.Rows(nCounter).Item("ID")) & dtActions.Rows(ncounter2).Item("Prefix") & dtActions.Rows(ncounter2).Item("Name") & "_" & dtActions.Rows(ncounter2).Item("ActionMethod") & "(CurrentControl,Nothing)" & vbCrLf
                    Next
                End If
            Next

            sCheckControlMethods &= sControlList & "End If" & vbCrLf & vbcrlf
            
            sCheckControlMethods &= "If CurrentControl.Controls.Count > 0 Then" & vbCrLf
            sCheckControlMethods &= "For Each CurrentControl2 As Control In CurrentControl.Controls" & vbCrLf
            sCheckControlMethods &= "Run" & sRepeaterName & "ControlEvents(CurrentControl2)" & vbCrLf
            sCheckControlMethods &= "Next" & vbCrLf
            sCheckControlMethods &= "End If" & vbCrLf

            sCheckControlMethods &= "End Sub" & vbCrLf & vbCrLf
        End Sub


    End Class
End Namespace
