Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.DataTypes
Imports Common.General.ProjectOperations
Imports Common.General.Folders
Imports Common.General.DataSources
Imports Common.General.Columns
Imports Common.Webpages.Validation.Main
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Export
Imports Common.General.File
Imports WhitTools.ErrorHandler
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.Utilities
Imports WhitTools.DataTables

Namespace General
    Public Class Controls
        Shared Function FormatControlHeading(ByVal sHeading As String) As String
            Return If(Not sHeading.ToLower.Contains("dataitem"), Replace(sHeading, """", "'"), sHeading)
        End Function

        Shared Function GetControlColumnValue(ByVal sControlID As String, ByVal sColumn As String, Optional ByRef ControlsDT As DataTable = Nothing, Optional ByVal sTable As String = DT_TOPLEVELPROJECTCONTROLS_V) As String
            Dim loginColumn As LoginColumnType

            If Not LoginColumnTypes Is Nothing Then
                loginColumn = (from lc in LoginColumnTypes 
                                   where lc.ID = sControlID
                select lc).DefaultIfEmpty(Nothing).First()
            End If

            If loginColumn Is Nothing Then
                If ControlsDT Is Nothing Then
                    Try
                        dim info as datatable = GetDataTable(CreateSQLConnection("WebRAD"), "Select * from " & sTable & " Where ID = " & sControlID)

                        If info.rows.count > 0
                            return info.Rows(0).Item(sColumn)
                            else
                            return -1
                        End If
                    Catch ex As Exception
                        'WriteLine("error - Select * from " & sTable & " Where ID = " & sControlID)
                        Return ""
                    End Try
                End If

                For Each CurrentRow As DataRow In ControlsDT.Rows
                    If CurrentRow.Item("ID") = sControlID Then
                        Return CurrentRow.Item(sColumn)
                    End If
                Next
            Else
                If scolumn="ControlType"
                    return logincolumn.ControlType
                    else
                    Return loginColumn.ColumnName
                End If
                End If

            Return ""
        End Function

        Shared Function GetImmediateChildControls(ByVal nControlID As Integer) As DataTable
            Return GetDataTable("Select * FROM " & DT_TOPLEVELPROJECTCONTROLS_V & " WHERE ParentControlID = " & nControlID & " Order by position asc", cnx)
            End Function

        Shared Function IsFirstStackControl(byval controlID As integer) As boolean
            if ControlUsesStackDisplay(controlID)
                Try
                Dim pageID as integer = getdatatable("SELECT PageID FROM " & dt_webrad_projectcontrols & " WHERE ID = " & controlid).Rows(0).item("PageID")
                Dim pageControls as list(of ProjectControl) = projectControls.Where(function (pc) pc.PageID = pageID).toList()
                dim controlIndex as integer = pageControls.FindIndex(function(pc) pc.ID = controlID)

                If controlIndex = 0
                    return true
                Else if not ControlUsesStackDisplay(pageControls(controlIndex - 1).ID)
                    return true
                End If
                Catch ex As Exception
                    logger.Error(ex.ToString)
                End Try

            End If

            Return False
        End Function


        Shared Function IsLastStackControl(ByVal controlID As Integer) As Boolean
            If ControlUsesStackDisplay(controlID) Then
                Dim pageControls As List(Of ProjectControl) = projectControls.Where(Function(pc) pc.PageID = GetControlColumnValue(controlID, "PageID")).ToList()
                Dim controlIndex As Integer = pageControls.FindIndex(Function(pc) pc.ID = controlID)

                If controlIndex = pageControls.Count - 1 Then
                    Return True
                ElseIf Not ControlUsesStackDisplay(pageControls(controlIndex + 1).ID) Then
                    Return True
                End If
            End If

            Return False
        End Function

        Shared Function IsFirstAdminControl(ByVal nControlID As Integer) As Boolean
            Try
                Return GetIsAdminControlBoundary(nControlID, "asc")
            Catch ex As Exception

            End Try

            Return False
        End Function

        Private Shared Function GetIsAdminControlBoundary(nControlID As Integer, ByVal direction As String) As Boolean
            Dim control As DataTable = GetDataTable("Select Top 1 ID From " & DT_WEBRAD_PROJECTCONTROLS & " Where DisplayLocation = " & N_DISPLAYLOCATION_BACKENDONLY & " and ProjectID = '" & GetProjectID() & "' ORDER BY Position " & direction, cnx)

            Return (control.Rows.Count > 0 AndAlso GetDataTable("Select Top 1 ID From " & DT_WEBRAD_PROJECTCONTROLS & " Where DisplayLocation = " & N_DISPLAYLOCATION_BACKENDONLY & " and ProjectID = '" & GetProjectID() & "' ORDER BY Position " & direction, cnx).Rows(0).Item("ID") = nControlID And Not isSearch And GetDisplayLocation(nControlID) = N_DISPLAYLOCATION_BACKENDONLY)
        End Function

        Shared Function IsLastAdminControl(ByVal nControlID As Integer) As Boolean
            Try
                Return GetIsAdminControlBoundary(nControlID, "desc")
            Catch ex As Exception

            End Try

            Return False
        End Function

        Shared Function GetDisplayLocation(ByVal nControlID As Integer) As Integer
            Return GetControlColumnValue(nControlID, "DisplayLocation")
        End Function

        Shared Function NoParentControl(ByVal sParentControlID As String) As String
            Return (sParentControlID = "0" Or sParentControlID = "" Or sParentControlID = "-1")
        End Function

        Shared Function ControlDisplayRequiresJoin(ByRef CurrentRow As DataRow) As Boolean
            Dim sDataTextField, sDataValueField, sDataSourceType As String
            With CurrentRow
                GetDataSourceSelectString(CurrentRow.Item("DataSourceID"), sDataTextField, sDataValueField, sDataSourceType)

                Return (IsDataType(.Item("ControlType"), "Dropdownlist") Or IsDataType(.Item("ControlType"), "Radiobuttonlist")) And sDataSourceType = "1" And sDataTextField <> sDataValueField
            End With
        End Function

        Shared Function GetErrorMessage(ByVal sMessage As String, ByVal sHeading As String, Optional ByVal bInMethod As Boolean = False) As String
            If sHeading.Contains("<%#") Then
                If bInMethod = False Then
                    Return "'" & Replace(FormatControlHeading(sHeading), "<%# ", "<%# """ & sMessage & """ & ") & "'"
                Else
                    sHeading = Replace(sHeading.ToLower, "<%# container.dataitem(""", """ & ctype(source.findcontrol(""lbl")
                    sHeading = Replace(sHeading, """) %>", """),Label).Text & """)

                    Return """" & sMessage & sHeading & "."""
                End If
            End If

            Return """" & sMessage & FormatControlHeading(sHeading) & "."""
        End Function

        Shared Function GetControlValueReference(ByRef CurrentRow As DataRow, Optional ByVal bInRepeater As Boolean = False, Optional ByVal bNameOnly As Boolean = False, Optional ByVal bText As Boolean = False) As String
            Dim sControlReference, sCurrentPrefix, sHiddenUploadedReference As String

            With CurrentRow
                sControlReference = If(bInRepeater, "ctype(.Findcontrol(""" & .Item("Prefix") & .Item("Name") & """)," & GetDataTypeDescription(.Item("DataType")) & ")", .Item("Prefix") & .Item("Name"))

                If bNameOnly Then
                    Return sControlReference
                End If

                sHiddenUploadedReference = If(bInRepeater, "ctype(.FindControl(""lblHiddenUploaded" & .Item("Name") & """),Label)", "lblHiddenUploaded" & .Item("Name"))
                sCurrentPrefix = .Item("Prefix")

                If IsPhoneControl(.Item("ControlType")) Then
                    Return "FormatStringStripNonNumbers(" & sControlReference & ".Text,True)"
                ElseIf IsDateControl(.Item("ControlType"), .Item("SQLDataType")) Then
                    Return "Trim(" & sControlReference & ".Text)"
                ElseIf IsFileUploadControl(.Item("ControlType")) Then
                    Return GetSaveDocumentAndReturnFilename(CurrentRow, bInRepeater)
                Else
                    If sCurrentPrefix = "txt" Or sCurrentPrefix = "lbl" Then
                        Return sControlReference & ".Text"
                    ElseIf sCurrentPrefix = "ddl" Or sCurrentPrefix = "rbl" Then
                        Return sControlReference & If(bText, ".SelectedItem.Text", ".SelectedValue")
                    ElseIf sCurrentPrefix = "chk" Or sCurrentPrefix = "rad" Then
                        Return sControlReference & ".Checked"
                    ElseIf sCurrentPrefix = "cbl" Or sCurrentPrefix = "lsb" Then
                        Return "GetListOfSelectedValues(" & sControlReference & ",""" & If(bText, "Text", "Value") & """)"
                    End If
                End If
            End With
        End Function

        Shared Function GetControlDeclaration(ByVal sToolkitType As String, ByVal sControlType As String, ByVal nControlType As Integer)
            Return sToolkitType & ":" & sControlType
        End Function

        Shared Sub GetParentControlInfo(ByRef nParentControlID As Integer, ByRef sSQLInsertItemTable As String)
            For Each CurrentRow As DataRow In controlsDT.Rows
                With CurrentRow
                    If .Item("ID") = nParentControlID Then
                        nParentControlID = .Item("ParentControlID")
                        sSQLInsertItemTable = .Item("SQLInsertItemTable")
                        Exit Sub
                    End If
                End With
            Next
        End Sub

        Shared Function ControlDisplayAllowed(ByVal nDisplayLocation As Integer) As Boolean
            Return (isFrontend = True And (nDisplayLocation = 1 Or nDisplayLocation = 2)) Or (isFrontend = False And (nDisplayLocation = 1 Or nDisplayLocation = 3))
        End Function


        Shared Function GetDesignerControls(ByVal ControlsDT As DataTable) As String
            Dim nCounter As Integer
            Dim sDesignerControls, sDesignerLibrary, sCurrentPrefix, sCurrentControlType As String

            If CurrentProjectRequiresWhitworthLogin() Then
                WriteIdentityLabelDeclarations(sDesignerControls)
            End If

            For nCounter = 0 To ControlsDT.Rows.Count - 1
                With ControlsDT.Rows(nCounter)
                    If Not ParentIsRepeaterControl(.Item("ID")) And BelongsToPage(pageNumber, .Item("PageID")) And (Not isSearch Or (isSearch And IsSearchControl(.Item("ID")))) And ControlTypeRequiresDesigner(.Item("ControlType")) Then
                        If IsFileUploadControl(.Item("ControlType")) Then
                            sDesignerControls &= "Protected WithEvents pnl" & .Item("Name") & " As Global.System.Web.UI.WebControls.Panel" & vbCrLf & vbCrLf
                            sDesignerControls &= "Protected WithEvents lblHiddenUploaded" & .Item("Name") & " As Global.System.Web.UI.WebControls.Label" & vbCrLf & vbCrLf

                            'If IsImageUploadControl(.Item("ControlType")) Then
                            '    sDesignerControls &= "Protected WithEvents lblUploaded" & .Item("Name") & " As Global.System.Web.UI.WebControls.Label" & vbCrLf & vbCrLf
                            'Else
                            sDesignerControls &= "Protected WithEvents lblUploaded" & .Item("Name") & " As Global.System.Web.UI.WebControls.Label" & vbCrLf & vbCrLf
                            'End If
                        End If

                        If .Item("ListSelections") = "1" And isSearch = False Then
                            sDesignerControls &= "Protected WithEvents lbl" & .Item("Name") & "SelectedItems As Global.System.Web.UI.WebControls.Label" & vbCrLf & vbCrLf
                        End If

                        sCurrentPrefix = .Item("Prefix")
                        sCurrentControlType = GetDataTypeDescription(.Item("DataType"))
                        sDesignerLibrary = If(.Item("DesignerLibrary") <> "", .Item("DesignerLibrary"), "WebControls")

                        sDesignerControls &= "Protected WithEvents " & sCurrentPrefix & .Item("Name") & " As Global.System.Web.UI." & sDesignerLibrary & "." & sCurrentControlType & vbCrLf & vbCrLf

                        Try
                            If CustomValidatorRequired(GetDataTable("Select * From " & DT_WEBRAD_CONTROLTYPES & "  Where ID = " & ControlsDT.Rows(nCounter).Item("ControlType"), Common.General.Variables.cnx).Rows(0).Item("CustomValidatorRequired"), .Item("SQLInsertItemTable"), .Item("TextMode"), .Item("CustomValidation")) Then
                                sDesignerControls &= "Protected WithEvents cv" & .Item("Name") & " As Global.System.Web.UI.WebControls.CustomValidator" & vbCrLf & vbCrLf
                            ElseIf .Item("Required") = "1" Then
                                sDesignerControls &= "Protected WithEvents rfv" & .Item("Name") & " As Global.System.Web.UI.WebControls.RequiredFieldValidator" & vbCrLf & vbCrLf
                            End If
                        Catch ex As Exception
                        End Try

                        If .Item("Calendar") = "1" Then
                            'sDesignerControls &= "Protected WithEvents cex" & .Item("Name") & " As AjaxControlToolkit.CalendarExtender" & vbCrLf & vbCrLf
                        End If
                    End If
                End With
            Next

            If isWorkflow And IsLastPage() Then
                sDesignerControls &= "Protected Withevents lblWorkflowStepID As Global.System.Web.UI.WebControls.Label" & vbCrLf & vbCrLf
            End If

            Return sDesignerControls
        End Function

        Shared Sub AddAvailableControls(ByRef dtSupplied As WhitTools.DataTablesSupplied)
            dtSupplied.AddRow("ddlControlID", "SQLSelect", "Select * FROM " & GetColumnSelectTable() & " Where ProjectID = " & GetProjectID() & " AND NOT ID IN (Select ID FROM web3.WebRAD.dbo.ProjectControls WHERE ControlType =" & N_REPEATER_CONTROL_TYPE & ") AND IncludeDatabase = 1 Order by [Position]", "Heading", "ID")
        End Sub

        Shared Function AllControlsComplete() As Boolean
            Return GetDataTable("Select * from " & DT_WEBRAD_PROJECTCONTROLS & " Where ID = '" & GetProjectID() & "' and ControlType IS NULL", cnx).Rows.Count = 0
        End Function

        Shared Function IsTopLevelControl(ByVal nControlID As Integer) As Boolean
            Return GetDataTable("Select * FROM " & DT_TOPLEVELPROJECTCONTROLS_V & " WHERE ID = " & nControlID).Rows.Count > 0
        End Function

        Shared Function GetRepeaterControlReference(ByVal sControlName As String, ByVal sControlTypeDescription As String, ByVal sReference As String) As String
            Return If(sReference <> "", "CType(" & sReference & ".FindControl(""" & sControlName & """)," & sControlTypeDescription & ")", sControlName)
        End Function

        Public Shared Function RepeaterRequiresPostbackTrigger(ByVal repeaterID As Integer) As Boolean
            Dim projectID As Integer = GetProjectID()

            Return _
                db.ProjectControls.Where(Function(pc) pc.ProjectID = projectID AndAlso
                        pc.ControlType IsNot Nothing).ToList().Where(
                    Function(control) _
                         GetControlDataType(control.ControlType) = N_UPLOAD_FILE_DATA_TYPE AndAlso
                                                     ParentIsRepeaterControl(control.ID, repeaterID)).Count > 0
        End Function

        Shared Function GetControlName(ByVal controlID As Integer) As String
            Dim controlData As DataTable = GetDataTable("SELECT PC.Name, D.Prefix, PC.ControlType FROM " & DT_WEBRAD_PROJECTCONTROLS & " PC LEFT OUTER JOIN " & DT_WEBRAD_CONTROLTYPES & " CT ON PC.ControlType = CT.ID LEFT OUTER JOIN " & DT_WEBRAD_CONTROLDATATYPES & " D ON CT.DataType = D.ID WHERE PC.ID = " & controlID)

            If controlData.Rows.Count > 0 Then

                With controlData.Rows(0)
                    Dim controlName As String

                    If isPrintable And DataTypeRequiresLabelOnPrintable(GetControlDataType(.Item("controlType"))) Then
                        controlName = "lbl"
                    Else
                        controlName = .Item("Prefix")
                    End If

                    controlName &= .Item("Name")

                    Return controlName
                End With
            End If

            Return ""
        End Function


        Public Shared Function ControlUsesFormGroup(ByVal controlID As Integer) As Boolean
            With db.ProjectControls.Where(Function(control) control.ID = controlID).ToList()(0)
                Try
                    Return .Visible <> "0" AndAlso Not isSearch AndAlso Not ControlTypeIsRepeater(.ControlType) AndAlso ControlTypeCanUseFormGroup(.ControlType) AndAlso Not ParentIsControlType(controlID, "FormGroup", "-1", 0, "", "", True) AndAlso Not InsideHorizontalRepeater(controlID) AndAlso Not .DisplayLocation = N_DISPLAYLOCATION_BACKENDONLY
                Catch ex As Exception
                    Return True
                End Try

            End With
            Return False
        End Function

        Public Shared Function ControlUsesStackDisplay(controlID As Integer) As Boolean
            Return InStr(GetControlDisplayTypeDescription(controlID), "Stack")
        End Function

        Public Shared Function GetControlDisplayTypeDescription(controlID As Integer) As String
            Return GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLDISPLAYTYPES & " WHERE ID = " & GetControlColumnValue(controlID, "DisplayType")).Rows(0).Item("Description").ToString()
        End Function

        Public Shared Function GetControlDisplayTypeClassName(controlID As Integer) As String
            Return GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLDISPLAYTYPES & " WHERE ID = " & GetControlColumnValue(controlID, "DisplayType")).Rows(0).Item("ClassName").ToString()
        End Function

        Public Shared Function ControlIsVisibleActionTarget(ByVal controlID As Integer) As Boolean
            Return db.ProjectControlPostbackActions.Where(Function(pa) pa.TargetControl = controlID And pa.Action = N_CONTROLACTIONTYPE_VISIBLE).Count > 0
        End Function

        Public Shared Function GetControlLifeCycleEvents() As String
            Dim lifeCycleEvents As String = ""

            AddRepeaterLifeCycleEvents(lifeCycleEvents)

            Return lifeCycleEvents
        End Function

        Private Shared Sub AddRepeaterLifeCycleEvents(ByRef lifeCycleEvents As String)
            Dim projectID As Integer = GetProjectID()
            Dim currentControls As List(Of ProjectControl) = db.ProjectControls.Where(Function(pc) pc.ProjectID = projectID AndAlso pc.ControlType IsNot Nothing).ToList()

            For Each repeaterControl In currentControls.Where(Function(pc) pc IsNot Nothing AndAlso ControlDisplayAllowed(If(pc.DisplayLocation, "1")) And (Not isFrontend Or BelongsToPage(pageNumber, pc.PageID)) And ControlTypeIsRepeater(pc.ControlType))
                Dim childUploadControls As List(Of ProjectControl) = currentControls.Where(Function(childControl) childControl IsNot Nothing AndAlso GetControlDataType(childControl.ControlType) = N_UPLOAD_FILE_DATA_TYPE AndAlso
                                                                                                                    ParentIsRepeaterControl(childControl.ID, repeaterControl.ID)).ToList()

                If childUploadControls.Count > 0 Then
                    lifeCycleEvents &= "Protected Sub rpt" & repeaterControl.Name & "_OnItemCreated(sender As Object, e As RepeaterItemEventArgs)" & vbCrLf

                    For Each childControl In childUploadControls
                        lifeCycleEvents &= "ScriptManager.GetCurrent(me).RegisterPostBackControl(e.item.FindControl(""btnUpload" & childControl.Name & """))" & vbCrLf
                    Next


                    lifeCycleEvents &= "End Sub" & vbCrLf
                End If
            Next
        End Sub

        Public Shared Sub GetPostbackControlRegistrations(ByRef registerPostbackControlsCall As String, ByRef registerPostbackControlsMethod As String)
            Dim projectID As Integer = GetProjectID()
            Dim controlRegistrations As String = ""

            For Each currentRow As DataRow In controlsDT.Rows
                With currentRow
                    Try

                        If ControlDisplayAllowed(.Item("DisplayLocation")) AndAlso BelongsToPage(pageNumber, .Item("Pageid")) And IsFileUploadControl(.Item("ControlType")) And Not ParentIsRepeaterControl(.Item("ID")) Then
                            WriteLine("ScriptManager.GetCurrent(me).RegisterPostBackControl(btnUpload" & .Item("Name") & ")" & vbCrLf)
                            controlRegistrations &= "ScriptManager.GetCurrent(me).RegisterPostBackControl(btnUpload" & .Item("Name") & ")" & vbCrLf
                        End If
                    Catch ex As Exception
                        logger.Error(ex.ToString)
                    End Try
                End With
            Next

            If controlregistrations <> "" Then
                registerpostbackcontrolscall = vbcrlf & "RegisterPostbackControls()" & vbcrlf
                registerpostbackcontrolsmethod = "Sub RegisterPostbackControls" & vbcrlf
                registerPostbackControlsMethod &= controlregistrations
                registerpostbackcontrolsmethod &= "End Sub" & vbcrlf
            End If
        End Sub

        Public Shared Function IsInsideHorizontalRepeater(controlID As String) As Boolean
            Dim parentControlID As String
            Dim parentIsRepeater As Boolean = ParentIsRepeaterControl(controlID, "-1", 0, parentControlID)

            Return parentIsRepeater AndAlso GetControlColumnValue(parentControlID, "LayoutType") = S_LAYOUTTYPE_HORIZONTAL
        End Function

        Public Shared Function AddCompositeControls(ByVal projectID As Integer, ByVal pageID As Integer, ByVal nControlType As Integer, ByVal nInsertPosition As Integer, Optional ByVal controlID As Integer = -1, Optional deleteOriginalControl As Boolean = True) As List(Of Common.ProjectControl)
            Dim individualControls = db.CompositeControls.Where(Function(cc) cc.CompositeControlType = nControlType).OrderBy(Function(cc) cc.Position).ToList()
            Dim controlsAdded As New List(Of ProjectControl)
            Dim columnExclusions = db.ControlTypeDetailColumnExclusions.ToList()
            Dim projectControlProperties = GetType(ProjectControl).GetProperties()
            Dim controlTypeDetailProperties = GetType(ControlTypeDetail).GetProperties().Where(Function(prop) Not columnExclusions.Any(Function(ce) ce.ColumnName = prop.Name))

            Dim sControlColumns As String

            db.ProjectControls.Where(Function(pc) pc.ProjectID = projectID And pc.Position >= nInsertPosition).ToList().ForEach(Sub(pc) pc.Position = pc.Position + individualControls.Count)
            db.SaveChanges()


            sControlColumns = String.Join(",", controlTypeDetailProperties.Select(Function(prop) prop.Name))

            individualControls.ForEach(Sub(control)
                                           Dim newControl As New ProjectControl()
                                           Dim newDetail = db.ControlTypeDetails.First(Function(detail) detail.ControlID = control.IndividualControlType)

                                           controlTypeDetailProperties.ToList().ForEach(Sub(detailProp)
                                                                                            Dim controlProperty = projectControlProperties.FirstOrDefault(Function(prop) prop.Name = detailProp.Name)

                                                                                            If controlProperty IsNot Nothing Then
                                                                                                controlProperty.SetValue(newControl, detailProp.GetValue(newDetail))
                                                                                            End If
                                                                                        End Sub)
                                           newControl.ID = Nothing
                                           newControl.ProjectID = projectID
                                           newControl.PageID = pageID
                                           newControl.ControlType = nControlType
                                           newControl.Position = nInsertPosition

                                           db.ProjectControls.Add(newControl)
                                           db.SaveChanges()

                                           controlsAdded.Add(newControl)

                                           nInsertPosition += 1
                                       End Sub)

            If deleteOriginalControl Then
                db.Entry(db.ProjectControls.First(Function(pc) pc.ID = controlID)).State = Entity.EntityState.Deleted
                db.SaveChanges()
            End If

            Return controlsAdded
        End Function

        Public Shared Function ControlUsesPleaseSelectItem(ByVal controlType As Integer) As Boolean
            Return Not (controlType = N_LISTBOX_CONTROL_TYPE Or controlType = N_CHECKBOXLIST_CONTROL_TYPE)
        End Function

        Public Shared Function IsRichTextboxUser(ByVal currentRow As DataRow)
            Return currentRow.Item("RichTextUser") = "1"
        End Function
    End Class
End Namespace

