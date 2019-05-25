
Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.Controls
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.ProjectOperations
Imports Common.General.DataTypes
Imports Common.General.Folders
Imports Common.General.DataSources
Imports Common.General.Columns
Imports Common.General.Assembly
Imports Common.SQL.StoredProcedure
Imports Common.Webpages.Frontend.MultiPage
Imports Common.SQL.Main
Imports Common.Webpages.Frontend.Email
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Export
Imports Common.Webpages.Backend.Schedule
Imports Common.Webpages.BindData
Imports Common.General.File
Imports Common.General.actions
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File



Imports WhitTools.Utilities
Namespace General
    Public Class Repeaters

        Shared Function GetRepeaterHandlerReference(ByVal nID As Integer) As String
            Dim sParentControlID, sParentControlName, sParentControlType As String

            ParentIsRepeaterControl(nID, "-1", 0, sParentControlID)
            sParentControlName = GetControlColumnValue(sParentControlID, "Name", controlsDT)
            sParentControlType = GetControlColumnValue(sParentControlID, "ControlType", controlsDT)

            'Potential cause of bug.  Didn't previously need try catch around IsRepeaterControl below
            Try
                If Not ControlTypeIsRepeater(sParentControlType) Then
                    sParentControlName = ""
                End If
            Catch ex As Exception
            End Try


            Return If(sParentControlName <> "", "rpt" & sParentControlName & "_", "")
        End Function

        Shared Function GetSaveAncillaryRepeaterMethod(ByVal nID As Integer, ByVal sIdentity As String, ByVal sName As String, ByVal sStoredProcedure As String, ByVal sForeignID As String, Optional ByVal bPassRepeaterItem As Boolean = False) As String
            Dim sUploadFiles, sSaveMultiValued, sDeleteMultiValued, sControlReference As String
            Dim methodBuilder As New StringBuilder()

            sControlReference = If(bPassRepeaterItem, "ctype(rpiCurrentItem.findcontrol(""", "") & "rpt" & sName & If(bPassRepeaterItem, """),Repeater)", "")

            'Write beginning of method and initial setup of variables.  Begin loops through items in repeater.
            methodBuilder.Append("Sub Save" & sName & "Items(ByVal n" & sForeignID & " as Integer" & If(bPassRepeaterItem, ",byval rpiCurrentItem as RepeaterItem", "") & ")" & vbCrLf)
            methodBuilder.Append("dim nCounter as integer" & vbCrLf)
            methodBuilder.Append("Dim sCurrentIds as String = """"" & vbCrLf)
            methodBuilder.Append("Dim cmd as new SqlCommand" & vbCrLf & vbCrLf)
            methodBuilder.Append("cmd.connection = cnx" & vbCrLf)
            methodBuilder.Append("cmd.CommandType = CommandType.StoredProcedure" & vbCrLf)
            methodBuilder.Append("cmd.CommandText = ""usp_Insert" & sStoredProcedure & """" & vbCrLf & vbCrLf)

            methodBuilder.Append("For nCounter = 0 to " & sControlReference & ".Items.Count - 1" & vbCrLf)
            methodBuilder.Append("With " & sControlReference & ".Items(nCounter)" & vbCrLf)

            methodBuilder.Append("cmd.Parameters.Clear()" & vbCrLf & vbCrLf)
            methodBuilder.Append("cmd.Parameters.AddWithValue(""@" & sForeignID & """, n" & sForeignID & ")" & vbCrLf)
            methodBuilder.Append("cmd.Parameters.AddWithValue(""@ID"", CType(.FindControl(""lblID""),Label).Text)" & vbCrLf)

            'Find all controls included in this repeater and add them as parameters, with exception of multivalued controls.
            For nCounter As Integer = 0 To controlsDT.Rows.Count - 1
                With controlsDT.Rows(nCounter)
                    If ParentIsRepeaterControl(.Item("ID"), nID) Then
                        If IsMultiValuedListControlType(.Item("ControlType")) Then
                            sSaveMultiValued &= vbCrLf & "SaveAncillaryContent(.FindControl(""" & .Item("Prefix") & .Item("Name") & """)," & sIdentity & ",""" & .Item("SQLInsertItemStoredProcedure") & """, cnx,""" & .Item("ForeignID") & """)" & vbCrLf
                            sDeleteMultiValued &= "ExecuteNonQuery(""DELETE FROM " & .Item("SQLInsertItemTable") & " WHERE NOT " & .Item("ForeignID") & " IN (SELECT ID FROM " & GetControlColumnValue(nID, "SQLInsertItemTable") & ")"", cnx)" & vbCrLf
                        ElseIf .Item("ValueAttribute") <> "" Then
                            AddControlAsParameter(controlsDT.Rows(nCounter), methodBuilder, True)
                            'sMethod &= "cmd.Parameters.AddWithValue(""@" & .Item("Name") & """, ctype(.Findcontrol(""" & .Item("Prefix") & .Item("Name") & """)," & GetDataTypeDescription(.Item("DataType")) & ")." & .Item("ValueAttribute") & ")" & vbCrLf

                            If IsFileUploadControl(.Item("ControlType")) Then
                                SaveAncillaryUploadFiles(controlsDT.Rows(nCounter), sUploadFiles, sIdentity)
                            End If
                        End If
                    End If
                End With
            Next

            methodBuilder.Append(vbCrLf & "dim nCurrentID as integer" & vbCrLf)
            methodBuilder.Append(vbCrLf & "nCurrentID = ExecuteScalar(cmd,cnx,""tryan"")" & vbCrLf & vbCrLf)
            methodBuilder.Append("sCurrentIDs &= If(sCurrentIDs <> """", "","", """") & nCurrentID" & vbCrLf)

            If sUploadFiles <> "" Then
                methodBuilder.Append(vbCrLf & vbCrLf & sUploadFiles)
            End If

            methodBuilder.Append(sSaveMultiValued)

            'Check through all repeaters in project to see if any are children of the current repeater, then write a call to save those items.
            Dim ChildControlsDT As DataTable = GetDataTable("Select * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ControlType = " & N_REPEATER_CONTROL_TYPE & " AND ProjectID = " & GetProjectID())

            For ncounter As Integer = 0 To ChildControlsDT.Rows.Count - 1
                With ChildControlsDT.Rows(ncounter)
                    If ParentIsRepeaterControl(.Item("ID"), nID) Then
                        methodBuilder.Append(vbCrLf & "Save" & .Item("Name") & "Items(nCurrentID," & If(bPassRepeaterItem, "ctype(rpiCurrentItem.findcontrol(""", "") & "rpt" & sName & If(bPassRepeaterItem, """),Repeater)", "") & ".Items(nCounter))" & vbCrLf)
                    End If
                End With
            Next

            methodBuilder.Append("End With" & vbCrLf)
            methodBuilder.Append("Next" & vbCrLf & vbCrLf)

            Dim sNextSQLInsertItemTable, sCurrentSQLInsertItemTable, sCurrentForeignID As String

            sCurrentSQLInsertItemTable = GetControlColumnValue(nID, "SQLInsertItemTable")
            sCurrentForeignID = GetControlColumnValue(nID, "ForeignID")

            If (Not isFrontend And Not isInsert) Or (isFrontend And GetPageCount() > 1) Then
                methodBuilder.Append("ExecuteNonQuery(""DELETE FROM " & sCurrentSQLInsertItemTable & " WHERE (" & sForeignID & " = "" & n" & sCurrentForeignID & " & If(sCurrentIDs <> """", "" AND NOT ID IN ("" & sCurrentIDs & "")"", """") & "")"",cnx)" & vbCrLf)
                methodBuilder.Append(sDeleteMultiValued)

                If ParentIsRepeaterControl(nID, "-1", 0, "", sNextSQLInsertItemTable) Then
                    methodBuilder.Append("ExecuteNonQuery(""DELETE FROM " & sCurrentSQLInsertItemTable & " WHERE NOT " & sCurrentForeignID & " IN (SELECT ID FROM " & sNextSQLInsertItemTable & ")"", cnx)" & vbCrLf)
                End If
            End If

            methodBuilder.Append("End Sub" & vbCrLf & vbCrLf)

            'Write the child repeater save methods.
            For ncounter As Integer = 0 To ChildControlsDT.Rows.Count - 1
                With ChildControlsDT.Rows(ncounter)
                    If ControlDisplayAllowed(.Item("DisplayLocation")) AndAlso ParentIsRepeaterControl(.Item("ID"), nID) Then
                        methodBuilder.Append(GetSaveAncillaryRepeaterMethod(.Item("ID"), sIdentity, .Item("Name"), .Item("SQLInsertItemStoredProcedure"), .Item("ForeignID"), True))
                    End If
                End With
            Next

            Return methodBuilder.ToString()
        End Function

        Shared Function GetRepeaterAddRemoveMethods() As String
            Dim sDTSupplied, sMethods, sAction As String

            If Not Variables.isArchive Then
                For Each Currentrow As DataRow In controlsDT.Rows
                    With Currentrow
                        If ControlDisplayAllowed(.Item("DisplayLocation")) AndAlso ControlTypeIsRepeater(.Item("ControlType")) AndAlso .Item("RepeaterAddRemove") = "1" AndAlso BelongsToPage(pageNumber, .Item("PageID")) Then
                            sDTSupplied = ""
                            GetSuppliedData(sDTSupplied, New StringBuilder(), Currentrow.Item("ID"))

                            sAction = GetRepeaterAction(.Item("ID"))

                            AddRepeaterItemMethod(Currentrow, sDTSupplied, sMethods, sAction)
                            AddRemoveRepeaterItemMethod(Currentrow, sDTSupplied, sMethods, sAction)
                        End If
                    End With
                Next
            End If

            Return sMethods
        End Function

        Private Shared Sub AddRemoveRepeaterItemMethod(Currentrow As DataRow, sDTSupplied As String, ByRef sMethods As String, sAction As String)
            With Currentrow
                sMethods &= "Protected Sub librpt" & Currentrow.Item("Name") & "_RemoveItem_Click(sender As Object, e As EventArgs)" & vbCrLf
                sMethods &= "Dim ParentRepeaterItem As RepeaterItem = GetParentRepeaterItem(sender)" & vbCrLf
                sMethods &= sDTSupplied
                sMethods &= "RemoveRepeaterItem(ParentRepeaterItem.Parent, " & If(sDTSupplied <> "", "dtSupplied", "Nothing") & ", CType(GetParentRepeaterItem(sender), RepeaterItem).ItemIndex, " & Currentrow.Item("MinimumRequired") & "," & If(ParentIsRepeaterControl(.Item("ID"), "-1"), "GetParentRepeaterItem(ParentRepeaterItem.Parent).FindControl(""cv" & Currentrow.Item("Name") & """)", "cv" & Currentrow.Item("Name")) & ",""" & Currentrow.Item("RepeaterItemName") & """)" & vbCrLf
                sMethods &= sAction
                sMethods &= "End Sub" & vbCrLf & vbCrLf
            End With
        End Sub

        Private Shared Sub AddRepeaterItemMethod(Currentrow As DataRow, sDTSupplied As String, ByRef sMethods As String, sAction As String)
            With Currentrow
                If Not Variables.isArchive Then
                    sMethods &= "Protected Sub btnrpt" & Currentrow.Item("Name") & "AddItem_Click(sender As Object, e As EventArgs)" & vbCrLf
                    sMethods &= sDTSupplied
                    sMethods &= "AddNewRepeaterItem(" & If(ParentIsRepeaterControl(.Item("ID"), "-1"), "GetParentRepeaterItem(sender).FindControl(""rpt" & Currentrow.Item("Name") & """)", "rpt" & Currentrow.Item("Name")) & ", " & If(sDTSupplied <> "", "dtSupplied", "Nothing") & ", 1, " & Currentrow.Item("MaximumRequired") & "," & If(ParentIsRepeaterControl(.Item("ID"), "-1"), "GetParentRepeaterItem(sender).FindControl(""cv" & Currentrow.Item("Name") & """)", "cv" & Currentrow.Item("Name")) & ",""" & Currentrow.Item("RepeaterItemName") & """)" & vbCrLf
                    sMethods &= sAction
                    sMethods &= "End Sub" & vbCrLf & vbCrLf
                End If
            End With
        End Sub

        Shared Function GetRepeaterRemoveControl(ByRef Currentrow As DataRow) As String
            Dim sControl As String
            If Not Variables.isArchive Then
                With Currentrow
                    sControl &= "<p>" & vbCrLf
                    sControl &= "<asp:LinkButton ID=""librpt" & .Item("Name") & "RemoveItem"" runat=""server"" CssClass=""icon-remove"" data-grunticon-embed CausesValidation=""false""  OnClick=""librpt" & Currentrow.Item("Name") & "_RemoveItem_Click"">" & If(Not isFrontend, "Remove", "") & "</asp:LinkButton>" & vbCrLf
                    sControl &= "</p> " & vbCrLf
                End With

            End If

            Return sControl
        End Function

        Shared Function GetRepeaterAddControl(ByRef Currentrow As DataRow) As String
            Dim sControl As String

            If Not Variables.isArchive Then
                Try
                    With Currentrow
                        sControl = "<p>" & vbCrLf
                        sControl &= "<asp:Button ID=""btnrpt" & .Item("Name") & "AddItem"" runat=""server"" CssClass=""button"" CausesValidation=""false"" Text=""Add " & .Item("RepeaterItemName") & """ OnClick=""btnrpt" & Currentrow.Item("Name") & "AddItem_Click""></asp:Button>" & vbCrLf
                        sControl &= "</p> " & vbCrLf
                    End With

                Catch ex As Exception
                    logger.Error(ex.ToString)
                End Try

            End If

            Return sControl
        End Function

        Shared Function WrapInTablesawRow(ByVal sItem As String) As String
            Dim sRow As String

            sRow &= "<tr data-tablesaw-no-labels>" & vbCrLf
            sRow &= "<td colspan=""100"">" & vbCrLf
            sRow &= sItem & vbCrLf
            sRow &= "</td>" & vbCrLf
            sRow &= "</tr>" & vbCrLf

            Return sRow
        End Function

        Shared Function InsideHorizontalRepeater(ByVal nID As Integer) As Boolean
            Dim sParentRepeaterID As String

            If ParentIsRepeaterControl(nID, "-1", 0, sParentRepeaterID, "", "", True) Then
                If GetControlColumnValue(sParentRepeaterID, "LayoutType") = S_LAYOUTTYPE_HORIZONTAL Then
                    Return True
                End If
            End If

            Return False
        End Function


        Shared Function GetRepeaterIdentityReference(ByVal nParentControlID As Integer, ByVal sIdentity As String, ByVal sForeignID As String, ByVal sSQLMainTableName As String, Optional ByRef sGetArchiveAncillaryData As String = "") As String
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Declare local variables
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim sNextParentControlID, sNextSQLInsertItemTable, sNextForeignID As String

            sNextParentControlID = ""
            sNextSQLInsertItemTable = ""
            sNextForeignID = ""

            If ParentIsRepeaterControl(nParentControlID, "-1", 0, sNextParentControlID, sNextSQLInsertItemTable, sNextForeignID) Then
                Return sForeignID & " IN (Select ID From " & sNextSQLInsertItemTable & " Where " & GetRepeaterIdentityReference(sNextParentControlID, sIdentity, sNextForeignID, sSQLMainTableName, sGetArchiveAncillaryData) & ")"
            Else
                Return sForeignID & " in (Select ID from " & sSQLMainTableName & " Where ID = "" & " & sIdentity & " & "")"
            End If
        End Function

        Shared Function RepeaterHasColumns(ByVal sRepeaterColumns As String) As Boolean
            Return sRepeaterColumns <> "" And sRepeaterColumns <> "0" And sRepeaterColumns <> "-1"
        End Function

        Shared Function ParentIsRepeaterControl(ByVal sControlID As String, Optional ByVal sRepeaterID As String = "-1", Optional ByRef nLayers As Integer = 0, Optional ByRef sNextParentControlID As String = "", Optional ByRef sNextSQLInsertItemTable As String = "", Optional ByRef sNextForeignID As String = "", Optional ByVal bSearchAll As Boolean = False) As Boolean
            'Dim dt As New DataTable
            Dim parentControlID As Integer = 0
            Dim parentControl As ProjectControl = Nothing

            If ControlTypeHasContainer(GetControlColumnValue(sControlID, "ControlType")) Then
                nLayers += 1
            End If

            Try
                Dim currentControl = db.ProjectControls.FirstOrDefault(Function(pc) pc.ID.ToString = sControlID)

                If currentControl IsNot Nothing Then
                    parentControl = db.ProjectControls.FirstOrDefault(Function(pc) pc.ID = currentControl.ParentControlID)
                End If
            Catch ex As Exception
            End Try

            Try
                If parentControl IsNot Nothing Then
                    sNextParentControlID = parentControl.ID
                    sNextSQLInsertItemTable = parentControl.SQLInsertItemTable
                    sNextForeignID = parentControl.ForeignID

                    If ControlTypeIsRepeater(parentControl.ControlType) Then
                        If (sRepeaterID <> "-1" And sRepeaterID = parentControl.ID) Or sRepeaterID = "-1" Then
                            nLayers += 1

                            Return True
                        ElseIf sRepeaterID <> "-1" And bSearchAll Then
                            nLayers += 1

                            Return ParentIsRepeaterControl(parentControl.ID, sRepeaterID, nLayers, sNextParentControlID, sNextSQLInsertItemTable, sNextForeignID, bSearchAll)
                        End If
                    ElseIf If(parentControl.ParentControlID, -1) > -1 Then
                        nLayers += 1

                        Return ParentIsRepeaterControl(parentControl.ID, sRepeaterID, nLayers, sNextParentControlID, sNextSQLInsertItemTable, sNextForeignID, bSearchAll)
                    End If
                End If
            Catch ex As Exception

            End Try


            'dt = GetDataTable("Select PC.*, CT.ParentControlTypeID from " & DT_WEBRAD_PROJECTCONTROLS & " PC inner join " & DT_WEBRAD_CONTROLTYPES & " CT on PC.ControlType = CT.ID Where PC.ID = '" & parentControlID & "'", True)

            'If dt.Rows.Count > 0 Then

            'End If
            Return False
        End Function

        'Shared Sub AddRepeaterRemoveControl(ByRef sContent As String, ByVal currentRow As DataRow)
        '    Dim sRepeaterRemoveControl As String = GetRepeaterRemoveControl(currentRow)

        '    With currentRow
        '        If .Item("LayoutType") = S_LAYOUTTYPE_HORIZONTAL Then
        '            sContent &= WrapInTablesawRow(sRepeaterRemoveControl)
        '            'ElseIf .Item("LayoutType") = S_LAYOUTTYPE_VERTICAL And .Item("LayoutSubtype") <> S_LAYOUTSUBTYPE_NOLIST Then
        '            '    sContent &= WrapInTableRow(sRepeaterRemoveControl)
        '        Else
        '            sContent &= sRepeaterRemoveControl
        '        End If
        '    End With
        'End Sub

        Shared Sub GetNestedRepeaterInitialItems(ByRef sGetLoadDDLs As String, ByRef CurrentRow As DataRow, ByRef dtChildRepeaters As DataTable, Optional ByVal nLayers As Integer = 1)
            Dim bBeganloop As Boolean = False
            Dim sParentRepeaterName = If(nLayers > 1, "CType(CurrentItem" & nLayers - 1 & ".FindControl(""rpt" & CurrentRow.Item("Name") & """),Repeater)", "rpt" & CurrentRow.Item("Name"))

            For Each childRow As DataRow In dtChildRepeaters.Rows
                If ParentIsRepeaterControl(childRow.Item("ID"), CurrentRow.Item("ID")) Then
                    If Not bBeganloop Then
                        sGetLoadDDLs &= vbCrLf & "For Each CurrentItem" & nLayers & " As RepeaterItem In " & sParentRepeaterName & ".Items" & vbCrLf

                        GetMinimumRepeaterItems("CurrentItem" & nLayers & ".FindControl(""btnrpt" & childRow.Item("Name") & "AddItem"")", sGetLoadDDLs, childRow)

                        'If childRow.Item("MinimumRequired") > 1 Then
                        '    sGetLoadDDLs &= "For nCounter As Integer = 1 To " & childRow.Item("MinimumRequired") & vbCrLf
                        'End If

                        'sGetLoadDDLs &= "btnrpt" & childRow.Item("Name") & "AddItem_Click(CurrentItem" & nLayers & ".FindControl(""btnrpt" & childRow.Item("Name") & "AddItem""), Nothing)" & vbCrLf

                        'If childRow.Item("MinimumRequired") > 1 Then
                        '    sGetLoadDDLs &= "Next" & vbCrLf
                        'End If

                        bBeganloop = True
                    End If

                    GetNestedRepeaterInitialItems(sGetLoadDDLs, childRow, dtChildRepeaters, nLayers + 1)
                End If
            Next

            If bBeganloop Then
                sGetLoadDDLs &= "Next" & vbCrLf & vbCrLf
            End If
        End Sub

        Shared Sub GetMinimumRepeaterItems(ByVal sControlReference As String, ByRef sGetLoadDDLs As String, ByRef CurrentRow As DataRow)
            With CurrentRow
                If .Item("MinimumRequired") > 1 Then
                    sGetLoadDDLs &= "For rpt" & .Item("Name") & "MinimumCounter As Integer = 1 To " & .Item("MinimumRequired") & vbCrLf
                End If

                sGetLoadDDLs &= "btnrpt" & .Item("Name") & "AddItem_Click(" & sControlReference & ", Nothing)" & vbCrLf

                If .Item("MinimumRequired") > 1 Then
                    sGetLoadDDLs &= "Next" & vbCrLf
                End If

            End With
        End Sub

        Shared Function GetSelectRepeaterData(ByRef RepeaterRow As DataRow, ByVal sSelectRepeaterData As String, ByVal nLayers As Integer, Optional ByVal bDTSuppliedExists As Boolean = False) As String
            With RepeaterRow
                Dim sDTSupplied As String = ""

                GetSuppliedData(sDTSupplied, New StringBuilder(), .Item("ID"), bDTSuppliedExists)

                sSelectRepeaterData &= sDTSupplied

                Dim sControlReference As String = .Item("Prefix") & .Item("Name")
                Dim sSelectChildren As String

                If nLayers > 0 Then
                    sControlReference = "CType(CurrentItem" & nLayers & ".FindControl(""" & sControlReference & """),Repeater)"
                End If

                Dim sdtName As String = "dt" & RemoveNonAlphanumeric(.Item("Name"))

                sSelectRepeaterData &= "Dim " & sdtName & " As DataTable = GetDataTable(""" & GetDataSourceSelectString(.Item("DataSourceID")) & """, Common.cnx)" & vbCrLf

                sSelectRepeaterData &= "SelectRepeaterData(" & sControlReference & ", " & sdtName & ", dtSupplied, Common.cnx)" & vbCrLf

                For Each CurrentRow As DataRow In controlsDT.Rows
                    If ParentIsRepeaterControl(CurrentRow.Item("ID"), RepeaterRow.Item("ID"), 0) And ControlTypeIsRepeater(CurrentRow.Item("ControlType")) Then
                        nLayers += 1
                        sSelectChildren &= GetSelectRepeaterData(CurrentRow, "", nLayers, bDTSuppliedExists)
                    End If
                Next

                If sSelectChildren <> "" Then
                    sSelectRepeaterData &= vbCrLf & "For Each CurrentItem" & nLayers & " As RepeaterItem in " & sControlReference & ".Items" & vbCrLf
                    sSelectRepeaterData &= sSelectChildren
                    sSelectRepeaterData &= "Next" & vbCrLf
                End If

                Return sSelectRepeaterData
            End With
        End Function

        Shared Function getRepeaterUploadChildControls(ByVal nRepeaterID As Integer) As List(Of DataRow)
            Dim uploadControls As List(Of DataRow) = New List(Of DataRow)()

            For Each Currentrow As DataRow In GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & GetProjectID() & " AND NOT ParentControlID IS NULL AND (ControlType = " & N_UPLOAD_FILE_CONTROL_TYPE & " OR ControlType = " & N_UPLOAD_IMAGE_CONTROL_TYPE & ")").Rows
                If (ParentIsRepeaterControl(Currentrow.Item("ID"), nRepeaterID, 0, "", "", "", False)) Then
                    uploadControls.Add(Currentrow)
                End If
            Next

            Return uploadControls
        End Function
    End Class
End Namespace

