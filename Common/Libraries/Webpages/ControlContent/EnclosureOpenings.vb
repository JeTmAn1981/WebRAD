Imports General
Imports WhitTools
imports whittools.Utilities
Imports WhitTools.DataTables
Imports System.Data
Imports Common.General
Imports Common.General.main
Imports Common.General.variables
Imports Common.General.repeaters
Imports Common.Webpages.ControlContent.Attributes
Imports Common.General.DataTypes
Imports Common.General.controlTypes
Imports Common.Webpages.ControlContent
Imports Common.Webpages.ControlContent.Main
Imports Common.Webpages.ControlContent.ContentWriter
Imports Common.General.controls

Public Class EnclosureOpenings
        Public shared sub AddStackContainerOpen(ByRef sContent as string, byval controlID As integer)
            If ControlUsesStackDisplay(controlID)
                If IsFirstStackControl(controlID)
                    sContent &= "<div class=""stack-container"">" & vbcrlf
                ElseIf GetControlColumnValue(controlID,"DisplayType") = N_DISPLAYTYPE_STACK_NEWROW
                    scontent &= "</div>" & vbcrlf
                    sContent &= "<div class=""stack-container"">" & vbcrlf
                End If

                sContent &= "<div class=""stack " & GetControlDisplayTypeClassName(controlID) & """>" & vbcrlf
            End If
        End sub

         Public shared sub AddAdministrativeSectionOpen(byval controlID As integer,ByRef  sContent As String) 
             If IsFirstAdminControl(controlid) Then
            sContent &= GetAdministrativeHeader()
        End If
         End sub

            Public shared sub AddContainerOpen(ByRef sContent as string, byval controlType As integer, byval controlID as integer)
        If Not isSearch AndAlso ControlTypes.ControlTypeHasContainer(controlType) Then
            sContent &= GetContainerOpen(controlID)
        End If
    End Sub

         Public shared sub AddFieldsetOpen(ByRef sContent As String, byval controlType As integer)
             If ControlTypeUsesFieldset(controlType) Then
                 sContent &= getfieldsetopen()
             End If
         End Sub

         Public shared sub AddFormGroupOpen(ByRef sContent As String, byval controlID As integer)
             If ControlUsesFormGroup(controlID)  Then
                 sContent &= getformgroupopen
             End If
         End Sub

        Private shared Function GetContainerOpen(ByVal controlID As integer) As Object
         with getdatatable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ID = " & controlID).rows(0)
            Dim containerOpen As String = "<asp:panel id=""" & .Item("Name") & "Container"" runat=""server"" "

            If .Item("Visible") = N_VISIBLE_INVISIBLE_VALUE Then
                If UseJavascriptActions And ControlIsVisibleActionTarget(controlID) Then
                    containerOpen &= "style=""display:none;"""
                Else
                    containerOpen &= "Visible=""False"""
                End If
            ElseIf .Item("Visible") = N_VISIBLE_CUSTOM_VALUE Then
                containerOpen &= "Visible='" & .Item("CustomVisibleValue") & "'"
            ElseIf .Item("Visible") = N_VISIBLE_DEPENDENT_VALUE Then
                containerOpen &= "Visible=""False"""
            End If
            
            containeropen &=  ">" & vbcrlf

            return containerOpen
            End With
        End Function
    
        Private shared function GetAdministrativeHeader() As string
            return "<h1>Administrative Use</h1>" & vbCrLf
        End function

        Private shared function GetFormGroupOpen() as string
            return "<div class=""form-group"">" & vbCrLf
        End function

    Private shared function GetFieldsetOpen() as string
            return "<fieldset>" & vbCrLf
         End function

End Class
