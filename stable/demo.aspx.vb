Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Web.Mail
Imports System.IO
Imports System.Text.RegularExpressions
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
Imports WebRAD.Common.Main
Imports WebRAD.Common.Variables
Imports WebRAD.Common.Assembly

Imports System.Threading

Public Class WebRADControl
    Public key As String
    Public item As ContextMenuItem
End Class

Public Class ContextMenuItem
    Public name As String
End Class

Partial Class demo
    Inherits System.Web.UI.Page

    <System.Web.Services.WebMethod()>
    Shared Function TestMethod() As List(Of WebRADControl)
        Dim blahList As New List(Of WebRADControl)
        Dim dt As DataTable = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = 39")

        For Each Currentrow As DataRow In dt.Rows
            Dim WRC As New WebRADControl
            Dim CMI As New ContextMenuItem

            WRC.key = Currentrow.Item("ID")
            CMI.name = Currentrow.Item("Name")
            WRC.item = CMI

            blahList.Add(WRC)
        Next

        'Dim WRC As New WebRADControl
        'Dim CMI As New ContextMenuItem

        'WRC.Key = "Test"
        'CMI.Name = "test"
        'WRC.Item = CMI

        'blahList.Add(WRC)

        'WRC = New WebRADControl
        'CMI = New ContextMenuItem

        'WRC.key = "Test2"
        'CMI.name = "test2"
        'WRC.Item = CMI

        'blahList.Add(WRC)

        Return blahList
    End Function

End Class
