Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity.Spatial

Partial Public Class LoginColumnType


    Public Sub New(ByVal nID As Integer)
            ID = nID
        End Sub

        Public BackendDisplayValue As String
        Public ColumnName As String
        Public ControlMaxLength As Integer
        Public ControlWidth As Integer
        Public DisplayName As String
    Public SQLType As String = "nvarchar(50)"
    Public ControlReference As String
        Public IncludeSelectStatement As Boolean = True
        Public IncludeProspectUserControl As Boolean = False
End Class
