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

Partial Public Class ARA_Departments
    Public Property ID As Integer
    Public Property Department As String
    Public Property Code As String
    Public Property Type As Nullable(Of Integer)
    Public Property Location As String
    Public Property MailStop As String
    Public Property Phone As String
    Public Property Fax As String
    Public Property HomePageURL As String
    Public Property HomePageURL2 As String
    Public Property Active As Nullable(Of Integer)
    Public Property DepSearch As Nullable(Of Integer)
    Public Property RedirectTo As Nullable(Of Integer)
    Public Property PortalDepartment As String
    Public Property CopyJobTicket As Nullable(Of Integer)

    Public Overridable Property ARA_Assignments As ICollection(Of ARA_Assignments) = New HashSet(Of ARA_Assignments)
    Public Overridable Property ARA_Departments_Locations As ICollection(Of ARA_Departments_Locations) = New HashSet(Of ARA_Departments_Locations)

End Class