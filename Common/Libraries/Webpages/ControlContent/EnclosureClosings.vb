Imports General
Imports WhitTools
imports whittools.Utilities
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

Public Class EnclosureClosings
        public shared  sub AddAdministrativeSectionClose(byval controlID As integer,ByRef  sContent As String) 
             If IsLastAdminControl(controlid) Then
                 scontent &= "</div>" & vbcrlf
             End If
         End sub

        public shared sub AddStackContainerClose(ByRef sContent as string, byval controlID As integer)
            If ControlUsesStackDisplay(controlID)
                sContent &= "</div>" & vbcrlf

                If IsLastStackControl(controlID)
                    sContent &= "</div>" & vbcrlf
                End If
            End If
        End sub

        public shared Sub AddFieldsetClose(ByRef sContent As String, byval controlType As integer)
            If ControlTypeUsesFieldset(controlType) Then
                 sContent &= GetFieldsetClose()
             End If
         End Sub

         
         public shared  Sub AddFormgroupClose(ByRef sContent As String, byval controlID As integer)
             If not isSearch andalso ControlUsesFormGroup(controlID) Then
                 sContent &= GetFormGroupClose()
             End If
         End Sub

        public shared Sub AddContainerClose(ByRef sContent as string, byval controlType As integer)
             if not isSearch andalso ControlTypes.ControlTypeHasContainer(controlType)
                 sContent &= GetContainerClose()
             End If
         End Sub

         Private shared Function GetContainerClose() As String
            Return "</asp:panel>" & vbcrlf
         End Function

        
         Private shared Function GetFormGroupClose() As String
            Return vbCrLf & "</div>" & vbCrLf
         End Function

        Private shared  Function GetFieldsetClose() As String
            Return vbCrLf & "</fieldset>" & vbCrLf
         End Function

End Class
