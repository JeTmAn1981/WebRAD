Imports Common.General
Imports Common.SQL
Imports WhitTools
Imports System.Data
Imports Microsoft.VisualBasic
Imports Common.General.Variables
Imports Common.General.Ancillary
Imports Common.General.ProjectOperations
Imports WhitTools.Getter
Imports WhitTools.Utilities

Namespace Webpages.Backend
    Public Class Navigation
        public Shared mainNavigation as String

        Shared Sub CreateMainNavigation()
            mainNavigation = currentProject.GetNavigation()
        End Sub

        Shared Sub GetBackendNavigation(ByRef navigation As String, Optional ByVal pageType As String = "")
            If mainNavigation <> ""
                dim sNavigationItem as string = "<li>" & vbcrlf
                snavigationitem &= "<label id=""lblNavigateTo"" for=""NavigateTo"">Navigate To:</label>" & vbcrlf
                snavigationitem &= "<select id=""NavigateTo"" onchange=""window.location = this.value;"">" & vbcrlf
                snavigationitem &= "<option></option>" & vbcrlf
                sNavigationItem &= mainnavigation
                snavigationitem &= "</select>" & vbcrlf
                snavigationitem &= "</li>" & vbcrlf

                navigation = snavigationitem
            End If

            'AddInsertNavigation(navigation)
        End Sub

        Private Shared Function NavigationItemsRequired(sNavigation As String) As Boolean
            Return (sNavigation <> "" or Main.getbackendoption(S_BACKEND_OPTION_INSERT_PAGE))
        End Function

    End Class
End NameSpace