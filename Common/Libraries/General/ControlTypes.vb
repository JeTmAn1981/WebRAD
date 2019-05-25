Imports Microsoft.VisualBasic
Imports WhitTools.DataTables
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Controls
Imports Common.General.DataTypes
Imports System.Data


Imports WhitTools.Utilities
Namespace General
    Public Class ControlTypes
         Shared Function IsPhoneControl(ByVal nControlType As Integer) As Boolean
            Return If(nControlType = "16", True, False)
        End Function

         Shared Function IsLiteralControlType(ByVal nControlType As Integer) As Boolean
            Return GetControlDataType(nControlType) = N_TEXTLITERAL_DATA_TYPE
        End Function

        Shared Function IsDateControl(ByVal controlType As Integer, ByVal SQLDataType As Integer) As Boolean
            Return controlType = N_DATE_CONTROL_TYPE Or SQLDataType = N_SQL_DATETIME_TYPE
        End Function

         Shared Function IsGLAccountControl(ByVal nControlType As Integer) As Boolean
            Return If(nControlType = "30", True, False)
        End Function

         Shared Function IsYesNoControl(ByVal nControlType As Integer) As Boolean
            Return If(nControlType = N_YESNO_RADIOBUTTONLIST_CONTROL_TYPE or IsDataType(ncontroltype,"Checkbox"), True, False)
         End Function

        Shared Function ControlTypeIsRepeater(ByVal nControlType As Integer) As Boolean
            Return If(nControlType = N_REPEATER_CONTROL_TYPE, True, False)
        End Function

        Shared Function IsCompositeControl(ByVal nControlType As Integer) As Boolean
            Return GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLTYPES & " WHERE ID = " & nControlType & " AND Composite = 1").Rows.Count > 0
        End Function

         Shared Function IsFileUploadControl(ByVal nControlType As Integer) As Boolean
            Return If(nControlType = N_UPLOAD_FILE_CONTROL_TYPE Or nControlType = N_UPLOAD_IMAGE_CONTROL_TYPE, True, False)
        End Function

         Shared Function IsImageUploadControl(ByVal nControlType As Integer) As Boolean
            Return nControlType = N_UPLOAD_IMAGE_CONTROL_TYPE
        End Function

        Shared function IsTimePickerControl(ByVal controlType As integer) As boolean
            return controlType = N_TIME_PICKER_CONTROL_TYPE
        End function

         Shared Function ParentIsControlType(ByVal sControlID As String, ByVal sParentControlType As String, Optional ByVal sParentControlID As String = "-1", Optional ByRef nLayers As Integer = 0, Optional ByRef sNextParentControlID As String = "", Optional ByRef sNextSQLInsertItemTable As String = "", Optional ByVal bSearchAll As Boolean = False, Optional ByVal bSearchParentRepeaters As Boolean = True) As Boolean
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Declare local variables
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim dt As DataTable = GetDataTable("Select PC.*, CT.ParentControlTypeID from " & DT_WEBRAD_PROJECTCONTROLS & " PC inner join " & DT_WEBRAD_CONTROLTYPES & " CT on PC.ControlType = CT.ID Where PC.ID = (Select ParentControlID From " & DT_WEBRAD_PROJECTCONTROLS & " Where ID = " & sControlID & ")", True)

            If dt.Rows.Count > 0 Then
                sNextParentControlID = dt.Rows(0).Item("ID")
                sNextSQLInsertItemTable = dt.Rows(0).Item("SQLInsertItemTable")

                If IsControlType(dt.Rows(0).Item("ControlType"), sParentControlType) Then
                    If (sParentControlID <> "-1" And sParentControlID = dt.Rows(0).Item("ID")) Or sParentControlID = "-1" Then
                        nLayers += 1

                        Return True
                    ElseIf sParentControlID <> "-1" And bSearchAll And bSearchParentRepeaters Then

                        nLayers += 1

                        Return ParentIsControlType(dt.Rows(0).Item("ID"), sParentControlType, sParentControlID, nLayers, sNextParentControlID, sNextSQLInsertItemTable, bSearchAll)
                    End If
                ElseIf NoParentControl(dt.Rows(0).Item("ParentControlID")) = False And bSearchAll Then

                    nLayers += 1

                    Return ParentIsControlType(dt.Rows(0).Item("ID"), sParentControlType, sParentControlID, nLayers, sNextParentControlID, sNextSQLInsertItemTable, bSearchAll)
                End If
            End If

            Return False
        End Function

         Shared Function IsControlType(ByVal nControlTypeID As Integer, ByVal sControlTypeName As String) As Boolean

            Select Case sControlTypeName
                Case "Repeater"
                    If nControlTypeID = N_REPEATER_CONTROL_TYPE Then
                        Return True
                    End If
                Case "Panel"
                    If nControlTypeID = N_PANEL_CONTROL_TYPE Then
                        Return True
                    End If
                Case "Checkboxlist"
                    If nControlTypeID = N_CHECKBOXLIST_CONTROL_TYPE Then
                        Return True
                    End If
                Case "Dropdownlist"
                    If nControlTypeID = N_DROPDOWNLIST_CONTROL_TYPE Then
                        Return True
                    End If
                Case "Radiobuttonlist"
                    If nControlTypeID = N_RADIOBUTTONLIST_CONTROL_TYPE Then
                        Return True
                    End If
                Case "Listbox"
                    If nControlTypeID = N_LISTBOX_CONTROL_TYPE Then
                        Return True
                    End If
                Case "Textbox"
                    If nControlTypeID = N_TEXTBOX_CONTROL_TYPE Then
                        Return True
                    End If
                Case "IDNumber"
                    Return If(nControlTypeID = N_IDNUMBER_CONTROL_TYPE, True, False)
                Case "Date"
                    If nControlTypeID = N_DATE_CONTROL_TYPE Then
                        Return True
                    End If
                Case "FormGroup"
                    If nControlTypeID = N_FORMGROUP_CONTROL_TYPE Then
                        Return True
                    End If
            End Select

            Return False
        End Function

         Shared Function GetControlTypesWithValues() As String
            Return "(" & N_DROPDOWNLIST_DATATYPE & "," & N_LISTBOX_DATATYPE & "," & N_CHECKBOXLIST_DATATYPE & "," & N_RADIOBUTTONLIST_DATATYPE & "," & N_TEXTBOX_DATA_TYPE & "," & N_LABEL_DATATYPE & ")"
        End Function

         Shared Function IsListControlType(ByVal ncontroltypeid As Integer) As Boolean
            Return (IsControlType(ncontroltypeid, "Checkboxlist") Or IsControlType(ncontroltypeid, "Listbox") Or IsControlType(ncontroltypeid, "Dropdownlist") Or IsControlType(ncontroltypeid, "Radiobuttonlist"))
        End Function

         Shared Function IsMultiValuedListControlType(ByVal ncontroltypeid As Integer) As Boolean
            Return (IsControlType(ncontroltypeid, "Checkboxlist") Or IsControlType(ncontroltypeid, "Listbox"))
        End Function

         Shared Function IsTextControlType(ByVal nControlTypeID As Integer) As Boolean
            Return IsDataType(nControlTypeID, "Textbox")
        End Function

         Shared Function IsMultiLineTextbox(ByRef CurrentRow As DataRow)
            Return IsTextControlType(CurrentRow.Item("ControlType")) And CurrentRow.Item("TextMode") = "MultiLine"
        End Function

         Shared Function RequiresUnorderedListLayout(ByRef CurrentRow As DataRow)
            Return IsDataType(CurrentRow.Item("ControlType"), "Radiobuttonlist") Or IsDataType(CurrentRow.Item("ControlType"), "Checkboxlist")
        End Function

        Shared Public  Function ControlTypeRequiresDesigner(ByVal nControlType As Integer) As Boolean
            Return Not (IsLiteralControlType(nControlType) Or IsDataType(nControlType, "div"))
        End Function

        Shared public  Function ControlTypeRequiresLabel(byval controlType As integer) As Boolean
            Return ControlType = N_CHECKBOX_CONTROL_TYPE
        End Function

        Shared function ControlTypeHasContainer(byval controlType as integer) As boolean
             return not (controlType = N_PANEL_CONTROL_TYPE Or controltype = N_FORMGROUP_CONTROL_TYPE)
        End function

         Shared Function ControlTypeCanUseFormGroup(ByVal nControlType As Integer)
             If IsControlType(nControlType, "FormGroup") Or IsDataType(nControlType, "Panel") or IsDataType(nControlType, "UserControl") Then'Or IsDataType(nControlType, "Literal") Then
                 Return False
             End If

             Return True
         End Function

         public shared Function ControlTypeUsesFieldset(ByVal controlType As integer) As Boolean
             Dim nDataType As Integer = GetControlDataType(controlType)
             Return (nDataType = N_CHECKBOXLIST_DATATYPE Or nDataType = N_RADIOBUTTONLIST_DATATYPE)
         End Function
    End Class
End Namespace

