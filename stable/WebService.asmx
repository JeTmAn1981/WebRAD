<%@ WebService Language="VB" Class="WebRAD.WebService" %>
Imports Common.General.Main
Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Xml
Imports System.Web.Services.Protocols
Imports System.Web.Script.Services
Imports Common
Imports WhitTools.DataTables
Imports WhitTools.Getter
imports whittools.utilities
Imports Common.ProjectFiles
imports common.general.variables
imports common.general.controltypes

Namespace WebRAD
    <WebService(Namespace:="http://tempuri.org/")> _
    <WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ScriptService()> _
    Public Class WebService
        Inherits System.Web.Services.WebService

        <WebMethod()>
        <ScriptMethod(UseHttpGet:=True)>
        Public Sub SaveBuild(ByVal nProjectID As Integer, ByVal bCreateFrontend As Boolean, ByVal bCreateBackend As Boolean, ByVal includeStatusPage As Boolean, ByVal sProjectType As String, ByVal sFormsType As String, ByVal username As String, ByVal pages() As ProjectPage, ByVal backendOptions() As ProjectBuildBackendOption, ByVal ancillaryMaintenance() As ProjectBuildBackendAncillaryMaintenance)
            SaveProjectBuild(nProjectID, bCreateFrontend, bCreateBackend, includeStatusPage, sProjectType, sFormsType, username, pages, backendOptions, ancillaryMaintenance)
        End Sub


        <ScriptMethod, WebMethod>
        Public Function GetSupervisorNameAutocompleteData(ByVal prefixText As String, ByVal count As Integer) As IEnumerable
            Return (From currentRow In GetDataTable("SELECT * FROM adTelephone.dbo.UserInfo_V WHERE IDNumber in (select plID from adTelephone.dbo.PeopleListing where PLActive='1') AND [User] like '%" & prefixText & "%' AND [User] LIKE '%" & prefixText & "%' ORDER BY PLLName, plfName").Rows
                    Select currentRow.item("User")).ToList()
            'Catch ex As Exception
            '    WriteTextFile(ex.ToString, "autocompleteerror.txt")
            'End Try

        End Function


        <WebMethod()> _
        <ScriptMethod(UseHttpGet:=True)> _
        Function GetProjectControls(ByVal nProjectID As Integer) As List(Of Common.WebRADControl)
            Dim controlList As New List(Of Common.WebRADControl)

            Dim WRC As New Common.WebRADControl
            Dim CMI As New Common.ContextMenuItem

            LoginColumnTypes.ForEach(Sub(lct)
                                         WRC = New Common.WebRADControl
                                         CMI = New Common.ContextMenuItem

                                         WRC.key = lct.ColumnName
                                         CMI.Name = lct.ColumnName
                                         WRC.item = CMI

                                         controlList.Add(WRC)
                                     End Sub)


            Dim dtControls As DataTable = GetDataTable("Select * FROM " & DT_WEBRAD_PROJECTCONTROLS & " WHERE ProjectID = " & nProjectID & " AND ControlType IN (SELECT ID FROM " & DT_WEBRAD_CONTROLTYPES & " WHERE DataType IN " & GetControlTypesWithValues() & ") ORDER BY Position asc")

            For Each CurrentRow As DataRow In dtControls.Rows
                With CurrentRow
                    WRC = New Common.WebRADControl
                    CMI = New Common.ContextMenuItem

                    WRC.key = .Item("Name")
                    CMI.name = .Item("Name")
                    WRC.item = CMI

                    controlList.Add(WRC)
                End With
            Next



            Return controlList
        End Function


        <WebMethod()> _
        <ScriptMethod(UseHttpGet:=True)> _
        Public function Test()
            return "test"
        End function
    End Class
End Namespace