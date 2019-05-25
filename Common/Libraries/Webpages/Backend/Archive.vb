Imports Microsoft.VisualBasic
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.Pages

Namespace Webpages.Backend
    Public Class Archive
         Shared Sub GetDeleteArchiveAncillaryData(ByRef sGetDeleteAncillaryData As String, ByVal sIdentity As String, Optional ByVal nRepeaterID As Integer = 0, Optional ByRef sGetArchiveAncillaryData As String = "")
            For each currentRow As DataRow In controlsDT.Rows
                With currentRow
                    If .Item("SQLInsertItemTable") <> "" And (Not ControlTypeIsRepeater(.Item("ControlType")) Or isBackendIndex) And ((nRepeaterID = 0 And Not ParentIsRepeaterControl(.Item("ID"))) Or ParentIsRepeaterControl(.Item("ID"), nRepeaterID)) And BelongsToPage(pageNumber, .Item("PageID")) Then
                        If ControlTypeIsRepeater(.Item("ControlType")) Then
                            GetDeleteArchiveAncillaryData(sGetDeleteAncillaryData, sIdentity, .Item("ID"), sGetArchiveAncillaryData)
                        End If

                        If nRepeaterID <> 0 Then
                            sGetDeleteAncillaryData &= "ExecuteNonQuery(""DELETE FROM " & .Item("SQLInsertItemTable") & " WHERE " & GetRepeaterIdentityReference(.Item("ID"), sIdentity, .Item("ForeignID"), projectDT.Rows(0).Item("SQLMainTableName"), sGetArchiveAncillaryData) & """,cnx)" & vbCrLf
                            sGetArchiveAncillaryData &= "ExecuteNonQuery(""Insert Into Archive_" & .Item("SQLInsertItemTable") & " Select * from  " & .Item("SQLInsertItemTable") & " WHERE NOT ID IN (SELECT ID FROM Archive_" & .Item("SQLInsertItemTable") & ") AND " & GetRepeaterIdentityReference(.Item("ID"), sIdentity, .Item("ForeignID"), projectDT.Rows(0).Item("SQLMainTableName"), sGetArchiveAncillaryData) & """,cnx)" & vbCrLf
                        Else
                            sGetDeleteAncillaryData &= "ExecuteNonQuery(""DELETE FROM " & .Item("SQLInsertItemTable") & " WHERE " & .Item("ForeignID") & " = "" & " & sIdentity & ",cnx)" & vbCrLf
                            sGetArchiveAncillaryData &= "ExecuteNonQuery(""Insert Into Archive_" & .Item("SQLInsertItemTable") & " Select * from " & .Item("SQLInsertItemTable") & " WHERE NOT ID IN (SELECT ID FROM Archive_" & .Item("SQLInsertItemTable") & ") AND " & .Item("ForeignID") & " = "" & " & sIdentity & ",cnx)" & vbCrLf
                        End If
                    End If
                End With
            Next
        End Sub

    End Class
End Namespace
