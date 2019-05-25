Imports General
Imports WhitTools
imports whittools.Utilities
Imports System.Data
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
Imports Common.EnclosureOpenings
Imports Common.EnclosureClosings

Public Class EnclosuresWriter
    Private currentControl as datarow

    public sub New(byval control as datarow)
        currentcontrol = control
    end sub

        public Sub AddEnclosureOpenings(ByRef sContent As String)
            with currentcontrol
                 AddAdministrativeSectionOpen(.Item("ID"),scontent)

                 AddStackContainerOpen(sContent,.Item("ID"))
                 AddContainerOpen(sContent,.Item("controlType"),.Item("ID"))
                 AddFormGroupOpen(sContent,.Item("ID"))
                 AddFieldsetOpen(sContent,.Item("ControlType"))
            End With
        End Sub

        public  Sub AddEnclosureClosings(ByRef sContent As String)
            with currentControl
                AddFieldsetClose(sContent,.item("controlType"))
                AddFormgroupClose(sContent,.Item("ID"))
                AddContainerClose(sContent,.Item("ControlType"))
                AddStackContainerClose(sContent,.Item("ID"))

                AddAdministrativeSectionClose(currentcontrol.Item("ID"),scontent)
            End With
        End Sub
 End Class
