Imports System.Data
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Controls
Imports Common.General.ControlTypes
Imports Common.General.DataTypes
Imports Common.General.ProjectOperations
Imports Common.General.Folders
Imports Common.General.Links
Imports Common.SQL.Main
Imports Common.Webpages.Backend.Archive
Imports Common.Webpages.Backend.SortFilter
Imports Common.Webpages.BindData
Imports Common.Webpages.ControlContent
Imports Common.Webpages.ControlContent.Heading
Imports Common.Webpages.Backend.Actions
Imports Common.Webpages.Backend.Columns
Imports Common.General.DataSources
Imports WhitTools.DataTables
Imports WhitTools.Utilities
Imports WhitTools.Email
Imports WhitTools.File

Namespace Webpages.Backend
    Public Class Search
        Inherits Backend.Main

        Shared Sub WriteSearchPage()
            call new searchpagewriter().writepage()
        End Sub

        Public Shared Function GetSearchTerms() As String
            Dim sCurrentPrefix, sSearchTerms As String

            For Each CurrentRow As DataRow In searchControls.Rows
                With CurrentRow
                    sCurrentPrefix = .Item("Prefix")

                    If .Item("SQLInsertItemTable") <> "" Then
                        sSearchTerms &= GetTermOptional(GetControlValueReference(CurrentRow), " ID In (Select " & CurrentRow.Item("ForeignID") & " FROM " & CurrentRow.Item("SQLInsertItemTable") & " WHERE " & CurrentRow.Item("Name") & " In ("" & " & GetControlValueReference(CurrentRow) & " & ""))")
                    Else
                        If IsDateControl(.Item("ControlType"), .Item("SQLDataType")) Then
                            Dim controlReference As String = If(.Item("Name") = "DateSubmitted", GetDateSubmittedColumnReference(), .Item("Name"))

                            sSearchTerms &= "AddDateRangeSearchValuesToQuery(conditionals,txt" & controlReference & ".Text,""" & controlReference & """)" & vbCrLf
                        ElseIf sCurrentPrefix = "ddl" Or sCurrentPrefix = "rbl" Or sCurrentPrefix = "rad" Then
                            sSearchTerms &= GetTermOptional(GetControlValueReference(CurrentRow), " " & CurrentRow.Item("Name") & " = '"" & CleanSQL(" & GetControlValueReference(CurrentRow) & ") & ""'")
                        ElseIf sCurrentPrefix = "chk" Then
                            sSearchTerms &= "If rbl" & .Item("Name") & ".SelectedIndex > 0 Then " & vbCrLf
                            sSearchTerms &= "conditionals.Add("" " & .Item("Name") & " = '"" & rbl" & .Item("Name") & ".SelectedItem.Value & ""'"")" & vbCrLf
                            sSearchTerms &= "End If" & vbCrLf & vbCrLf
                        ElseIf sCurrentPrefix = "txt" Or sCurrentPrefix = "lbl" Then
                            sSearchTerms &= GetTermOptional(GetControlValueReference(CurrentRow), " " & CurrentRow.Item("Name") & " like '%"" & CleanSQL(" & GetControlValueReference(CurrentRow) & ") & ""%'")
                        End If
                    End If
                End With
            Next

            Return sSearchTerms
        End Function

        Shared Function GetTermOptional(ByVal sCurrentReference As String, ByVal sSelectStatement As String) As String
            Dim sTermOptional As String
            sTermOptional = "If " & sCurrentReference & " <> """" Then " & vbCrLf
            sTermOptional &= "conditionals.Add(""" & sSelectStatement & """)" & vbCrLf
            sTermOptional &= "End If" & vbCrLf & vbCrLf

            Return sTermOptional
        End Function

        'Bug here?  Allowing repeater controls in selection list for search controls, but method doesn't know how to handle them.
        Shared Function GetSearchControls(ByVal sSearchControlLoginContent As String, ByVal nColumnCount As Integer) As String
            Dim sContent, sCurrentPrefix, sCurrentControlType, sCurrentValueAttribute As String
            Dim dt As New DataTable

            Dim nSearchControlColumns As Integer

            nSearchControlColumns = GetSearchControlColumns()

            sContent &= "<div class=""stack-container"" id=""searchElements"">" & vbCrLf

            If Main.GetBackendOption(S_BACKEND_OPTION_ARCHIVE_VIEW) Then
                sContent &= "<div class=""stack"">" & GetArchiveControl() & "</div>"
            End If

            sContent &= sSearchControlLoginContent


            For nCounter As Integer = 0 To searchControls.Rows.Count - 1
                With searchControls.Rows(nCounter)
                    'Potential bug here, need better separation between login controls (which include date submitted)
                    'and date submitted control outside of login projects
                    If .Item("ID") > 0 Or (Not CurrentProjectRequiresWhitworthLogin() And .Item("ID") = N_DATESUBMITTED_CONTROLID) Then
                        sContent &= "<div class=""stack"">" & vbCrLf

                        If GetControlDataType(.Item("ControlType")) = N_CHECKBOX_DATATYPE Then
                            sContent &= "<label for=""rbl" & .Item("Name") & """>" & .Item("ShortHeading") & "</label>"

                            sContent &= "<asp:RadiobuttonList ID=""rbl" & .Item("Name") & """ runat=""server"" RepeatDirection=""Horizontal"">" & vbCrLf
                            sContent &= "<asp:ListItem value="""" Selected=""True"">N/A</asp:ListItem>" & vbCrLf
                            sContent &= "<asp:ListItem value=""1"">True</asp:ListItem>" & vbCrLf
                            sContent &= "<asp:ListItem value=""0"">False</asp:ListItem>" & vbCrLf
                            sContent &= "</asp:RadioButtonList>" & vbCrLf
                        ElseIf IsDateControl(.Item("controlType"), .Item("SQLDataType")) Then
                            Dim controlName As String = If(.Item("Name") = "DateSubmitted", GetDateSubmittedColumnReference(), .Item("Name"))

                            sContent &= "<asp:Label AssociatedControlID=""txt" & controlName & """ runat=""server"">" & .Item("Heading") & "</asp:Label>" & vbCrLf
                            sContent &= "<asp:Textbox ID=""txt" & controlName & """ Runat=""server""></asp:Textbox>" & vbCrLf
                            sContent &= "<script>" & vbCrLf
                            sContent &= "$(""#txt" & controlName & """).daterangepicker(WebRADApps.dateRangePickerOptions);" & vbCrLf
                            sContent &= "</script>" & vbCrLf
                        Else
                            For Each currentRow As DataRow In controlsDT.Rows
                                If currentRow.Item("ID") = searchControls.Rows(nCounter).Item("ID") Then
                                    Call New ContentWriter(currentRow).GetControlContent(sContent)
                                    Exit For
                                End If
                            Next
                        End If

                        sContent &= "</div>" & vbCrLf
                    End If

                End With
            Next

            sContent &= "</div>" & vbCrLf

            Return sContent
        End Function


        Shared Sub GetLoginSearchControlData(ByRef controlContent As String, ByRef columnCount As Integer)
            If CurrentProjectRequiresWhitworthLogin() Then
                Dim searchControlColumns, controlMaxLength, controlWidth, controlType As Integer
                Dim controlHeading, controlReference, controlColumnName As String
                Dim loginColumn As LoginColumnType

                searchControlColumns = GetSearchControlColumns()

                For Each CurrentRow As DataRow In GetLoginSearchControls().Rows
                    With CurrentRow
                        controlContent &= "<div class=""stack"">" & vbCrLf

                        controlHeading = ""
                        controlReference = ""
                        controlColumnName = ""
                        controlType = 0
                        controlMaxLength = 0
                        controlWidth = 0

                        loginColumn = LoginColumnTypes.Find(Function(l) l.ID = CurrentRow.Item("ControlID"))

                        If Not loginColumn Is Nothing Then
                            controlType = loginColumn.ControlType
                            controlHeading = loginColumn.DisplayName
                            controlReference = loginColumn.ControlReference
                            controlColumnName = loginColumn.ColumnName
                            controlMaxLength = loginColumn.ControlMaxLength
                            controlWidth = loginColumn.ControlWidth
                        End If

                        If IsDateControl(.Item("controlType"), .Item("SQLDataType")) Then
                            Dim controlName As String = If(.Item("Name") = "DateSubmitted", GetDateSubmittedColumnReference(), .Item("Name"))

                            controlContent &= "<asp:Label AssociatedControlID=""txt" & controlName & """ runat=""server"">" & .Item("Heading") & "</asp:Label>" & vbCrLf
                            controlContent &= "<asp:Textbox ID=""txt" & controlName & """ Runat=""server""></asp:Textbox>" & vbCrLf
                            controlContent &= "<script>" & vbCrLf
                            controlContent &= "$(""#txt" & controlName & """).daterangepicker(WebRADApps.dateRangePickerOptions);" & vbCrLf
                            controlContent &= "</script>" & vbCrLf
                        ElseIf controlType = N_TEXTBOX_CONTROL_TYPE Then
                            controlContent &= $"<label for=""{controlReference}"">{controlHeading}</label>" & vbCrLf
                            controlContent &= "<asp:textbox id=""" & controlReference & """ runat=""server"" Width=""" & controlWidth & """ MaxLength=""" & controlMaxLength & """ cssclass=""SlText"" />" & vbCrLf
                        ElseIf controlType = N_DROPDOWNLIST_CONTROL_TYPE Then
                            controlContent &= $"<label for=""{controlReference}"">{controlHeading}</label>" & vbCrLf
                            controlContent &= "<asp:dropdownlist id=""" & controlReference & """ runat=""server"">" & vbCrLf

                            If controlColumnName = "Class" Then
                                controlContent &= "<asp:listitem value="""">Please Select</asp:listitem>" & vbCrLf
                                controlContent &= "<asp:listitem value=""Freshman"" />" & vbCrLf
                                controlContent &= "<asp:listitem value=""Sophomore"" />" & vbCrLf
                                controlContent &= "<asp:listitem value=""Junior"" />" & vbCrLf
                                controlContent &= "<asp:listitem value=""Senior"" />" & vbCrLf
                                controlContent &= "<asp:listitem value=""Graduate"" />" & vbCrLf
                            End If

                            controlContent &= "</asp:dropdownlist>" & vbCrLf
                        End If

                        controlContent &= "</div>" & vbCrLf
                    End With
                Next
            End If
        End Sub

        Shared Function GetSearchControlColumns() As Integer
            Return getcurrentprojectdt().Rows(0).Item("SearchColumns")
        End Function

                Shared Function GetLoginSearchControls() As DataTable
            Dim dtSearch As DataTable = GetDataTable("Select * From " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  OC left outer join " & DT_WEBRAD_PROJECTCONTROLS & " PC on PC.ID = OC.ControlID Where ControlID < 1 AND OptionID in (Select Min(ID) From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  WHERE Type = 10 AND ProjectID = " & GetProjectID() & ")", Common.General.Variables.cnx)

            Return dtSearch
        End Function

        Shared Function GetSearchQuery(ByVal sSelectStatement As String) As String
            Dim sSearchQuery As String = ""

            Dim searchSelect As String = sSelectStatement

            If Main.GetBackendOption(S_BACKEND_OPTION_ARCHIVE_VIEW) Then
                sSearchQuery &= "If rblArchive.SelectedValue = ""Archive"" Then" & vbCrLf
                sSearchQuery &= "sSearchQuery &= ""Select * FROM (Select *, 'ViewArchive' As UpdateLocation FROM (" & sSelectStatement & ") AS Archive  WHERE COALESCE(Archived,0) = 1) AS MT""" & vbCrLf
                sSearchQuery &= "ElseIf rblArchive.SelectedValue = ""Main"" Then" & vbCrLf
                sSearchQuery &= "sSearchQuery &= ""SELECT * FROM (SELECT *, 'Update' As UpdateLocation FROM (" & sSelectStatement & ") AS Main WHERE COALESCE(Deleted,0) = 0) AS MT""" & vbCrLf
                sSearchQuery &= "Else" & vbCrLf
                sSearchQuery &= "sSearchQuery &= ""SELECT * FROM (SELECT *, 'Update' As UpdateLocation FROM (" & sSelectStatement & ") AS Main WHERE COALESCE(Deleted,0) = 0 UNION SELECT *, 'ViewArchive' As UpdateLocation FROM (" & sSelectStatement & ") AS Archive  WHERE COALESCE(Archived,0) = 1) AS MT""" & vbCrLf
                sSearchQuery &= "End If" & vbCrLf
            Else
                sSearchQuery &= "sSearchQuery &= ""SELECT * FROM (SELECT *, 'Update' As UpdateLocation FROM (" & sSelectStatement & ") AS Main WHERE COALESCE(Deleted,0) = 0) AS MT""" & vbCrLf
            End If

            Return sSearchQuery
        End Function

        Shared Function IsSearchControl(ByVal nID As Integer) As Boolean
            Dim dtSearch As DataTable = GetDataTable("Select * From " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & "  OC left outer join " & DT_WEBRAD_PROJECTCONTROLS & " PC on PC.ID = OC.ControlID Where ControlID = " & nID & " AND  OptionID in (Select Min(ID) From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & "  WHERE Type = 10 AND ProjectID = " & GetProjectID() & ")", Common.General.Variables.cnx)

            Return If(dtSearch.Rows.Count > 0, True, False)
        End Function

        Shared Function GetArchiveControl() As String
            Dim sControl As String = ""

            sControl &= "<br />" & vbCrLf
            sControl &= "<br />" & vbCrLf
            sControl &= "<strong>List</strong>" & vbCrLf
            sControl &= "<br />" & vbCrLf
            sControl &= "<asp:RadiobuttonList ID=""rblArchive"" runat=""server"" RepeatDirection=""Vertical"">" & vbCrLf
            sControl &= "<asp:ListItem value=""Main"" Selected=""True"">Main Only</asp:ListItem>" & vbCrLf
            sControl &= "<asp:ListItem value=""Archive"">Archive Only</asp:ListItem>" & vbCrLf
            sControl &= "<asp:ListItem value=""Both"">Both</asp:ListItem>" & vbCrLf
            sControl &= "</asp:RadioButtonList>" & vbCrLf

            Return sControl
        End Function

        Public Shared Sub SetSearchControls(projectID As Integer)
            searchControls = GetDataTable("SELECT * FROM  (Select ID, ProjectID, PageID, Position, ParentControlID, ControlType, SQLDataType, Prefix,Name, Heading, ShortHeading, SQLInsertItemTable, ForeignID FROM " & DT_TOPLEVELPROJECTCONTROLS_V & " UNION SELECT IDNumber as ID, " & projectID & " as ProjectID,(SELECT MAX(ID) + 1 FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & projectID & ") As PageId, [Order] AS Position, NULL as ParentControlID, ControlType,0 as SQLDataType, Prefix,ControlName as Name, Heading, Heading as Shortheading,'' as SQLInsertItemTable, NULL as ForeignID FROM " & DT_WEBRAD_LOGINCOLUMNTYPES & ") As SearchControls WHERE ProjectID = " & projectID & " AND ID IN (Select ControlID From " & DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS & " OC Where OptionID in (Select Min(ID) From " & DT_WEBRAD_PROJECTBACKENDOPTIONS & " WHERE Type = 10 AND ID > 0 AND ProjectID = " & projectID & ")) Order by PageID asc, Position asc")
        End Sub

    End Class
End Namespace



