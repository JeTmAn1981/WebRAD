'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class ProjectColumn
    Public Property ID As Integer
    Public Property ProjectID As Nullable(Of Integer)
    Public Property TableControlID As Nullable(Of Integer)
    Public Property ColumnControlID As Nullable(Of Integer)
    Public Property Type As String
    Public Property TypeID As Nullable(Of Integer)

    Public Overridable Property Project As Project
    Public Overridable Property ProjectControl As ProjectControl

End Class