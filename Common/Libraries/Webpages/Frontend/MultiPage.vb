Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports Common.General.Ancillary
Imports Common.General.Main
Imports Common.General.Variables
Imports Common.General.Pages
Imports Common.General.Controls
Imports Common.General.Repeaters
Imports Common.General.ControlTypes
Imports Common.General.ProjectOperations
Imports Common.General.DataTypes
Imports Common.General.Folders
Imports Common.General.DataSources
Imports Common.General.Columns
Imports Common.General.Assembly
Imports Common.SQL.Main
Imports Common.Webpages.Frontend.Email
Imports Common.Webpages.Backend.Search
Imports Common.Webpages.Backend.Export
Imports Common.Webpages.Backend.Schedule
Imports Common.General.Links
Imports Common.Webpages.BindData
Imports Common.Webpages.Backend.Archive
Imports WhitTools.DataTables
Imports WhitTools.ErrorHandler
Imports WhitTools.Email
Imports WhitTools.SQL
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.File


Imports WhitTools.Utilities
Namespace Webpages.Frontend
    Public Class MultiPage
        Shared Sub WriteStatusPage()
            If currentProjectBuild.IncludeStatusPage Then
                Call New StatusPageWriter().WritePage()
            End If
        End Sub

        Shared Sub WriteCertificationPage()
            Call New certificationpagewriter().writepage()
        End Sub

        Shared Function GetCertificationMethod() As String
            Dim sCertificationMethod As String = ""

            sCertificationMethod &= "Dim certificationcmd As New SqlCommand" & vbCrLf & vbCrLf
            sCertificationMethod &= "certificationcmd.Connection = cnx" & vbCrLf
            sCertificationMethod &= "certificationcmd.CommandType = CommandType.StoredProcedure" & vbCrLf
            sCertificationMethod &= "certificationcmd.CommandText = ""usp_Update" & projectDT.Rows(0).Item("SQLInsertStoredProcedureName") & "Certification""" & vbCrLf & vbCrLf

            sCertificationMethod &= "With certificationcmd" & vbCrLf
            sCertificationMethod &= "certificationcmd.Parameters.AddWithValue(""@Username"", Common.GetCurrentusername())" & vbCrLf
            sCertificationMethod &= "End With" & vbCrLf

            sCertificationMethod &= "dim nCurrentID as Integer = ExecuteScalar(certificationcmd,cnx)" & vbCrLf

            Return sCertificationMethod
        End Function

        Shared Function GetSectionLinks() As String
            Dim sSectionLinks As String = ""
            Dim pages = currentProject.ProjectPages.OrderBy(Function(pp) pp.ID).ToArray()

            For nCounter As Integer = 1 To pages.Count
                If pages(nCounter - 1).Dependent Then
                    sSectionLinks &= "<tr style='display:<%= IIf(sectionsAllowed(" & nCounter & "),"""",""none"") %>'>" & vbCrLf
                Else
                    sSectionLinks &= "<tr>" & vbCrLf
                End If

                sSectionLinks &= "<td><a class=""StatusSectionLink"" href='Section" & nCounter & ".aspx'>" & pages(nCounter - 1).Purpose & "</a></td>" & vbCrLf
                sSectionLinks &= "<td align=""center""><asp:label id=""lblSection" & nCounter & """ Runat=""server""></asp:label></td>" & vbCrLf
                sSectionLinks &= "</tr>" & vbCrLf
            Next

            If DefaultCertificationPage() Then
                sSectionLinks &= "<tr>"
                sSectionLinks &= "       <td  style=""padding-right:20px""><A href=""Certification.aspx"">Certification</A></td>"
                sSectionLinks &= "       <td align=""center""><asp:label id=""lblCertification"" Runat=""server""></asp:label></td>"
                sSectionLinks &= "        </tr>"
            End If

            Return sSectionLinks
        End Function

        Shared Function GetChangePasswordLink() As String
            Return "<asp:linkbutton id=""btnChangePassword"" CssClass=""button""  causesvalidation=""false"" runat=""server""><span class=""glyphicon glyphicon-user""></span> Change Password</asp:linkbutton>"
        End Function

        Shared Function GetChangePasswordMethod() As String
            Dim sChangePasswordMethod As String

            sChangePasswordMethod = "Private Sub btnChangePassword_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChangePassword.Click" & vbCrLf
            sChangePasswordMethod &= "Redirect(""changepassword.aspx"")" & vbCrLf
            sChangePasswordMethod &= "End Sub" & vbCrLf

            Return sChangePasswordMethod
        End Function


        Shared Function GetSectionTitle(ByVal nPageNumber As Integer) As String
            Dim sectionTitle As String = If(nPageNumber <> -1, GetPageInfo(nPageNumber, "Purpose"), "")

            If currentProject.IncludeSectionNumbers Then
                sectionTitle = "Section " & nPageNumber & " - " & sectionTitle
            End If

            Return sectionTitle
        End Function

        Shared Function IsFirstSection() As Boolean
            Return pageNumber = 1
        End Function

        Shared Function GetCertificationText() As String
            Return "I hereby certify that all information submitted via this application is genuine and true.  By clicking the Certify button below I am submitting my finished application."
        End Function

        Shared Function GetCheckApplicationFinishedMethod(ByVal sSQLMainTable As String) As String
            Dim checkApplicationFinished

            checkApplicationFinished = "Shared Sub CheckApplicationFinished(byval sUsername as string)" & vbCrLf

            If OptionalSections(GetProjectPages()) Then
                checkApplicationFinished = GetOptionalSectionsCheckApplicationFinishedMethod(checkApplicationFinished)
            Else
                checkApplicationFinished = GetStandardCheckApplicationFinishedmethod(checkApplicationFinished)
            End If

            checkApplicationFinished &= "End Sub" & vbCrLf & vbCrLf

            Return checkApplicationFinished
        End Function

        Private Shared Function GetStandardCheckApplicationFinishedmethod(checkApplicationFinished As Object) As Object
            Dim sectionsComplete As String = ""

            For nCounter As Integer = 1 To If(DefaultCertificationPage(), GetPageCount(), GetPageCount() - 1)
                If sectionsComplete <> "" Then
                    sectionsComplete &= " AND "
                End If

                sectionsComplete &= "Section" & nCounter & "Complete = '1'"
            Next

            sectionsComplete = "(" & sectionsComplete & ")"

            checkApplicationFinished &= "If GetDataTable(""Select * From "" & MAIN_DATABASE_TABLE_NAME & "" Where " & GetCertificationCondition() & " AND COALESCE(Deleted, 0) = 0 AND Username = '"" & sUsername & "" ' AND " & sectionsComplete & " "",cnx).Rows.Count = 0 Then" & vbCrLf
            checkApplicationFinished &= "messages.RedirectToMessage(MessageCode.NotFinished)" & vbCrLf
            checkApplicationFinished &= "End If" & vbCrLf
            Return checkApplicationFinished
        End Function

        Private Shared Function GetOptionalSectionsCheckApplicationFinishedMethod(checkApplicationFinished As String) As String
            checkApplicationFinished &= "Dim applicationData = GetDataTable(""Select * From "" & MAIN_DATABASE_TABLE_NAME & "" Where master.dbo.FieldIsPositive(Certification) = 0 AND COALESCE(Deleted, 0) = 0 AND Username = '"" & sUsername & "" ' "", cnx)" & vbCrLf & vbCrLf
            checkApplicationFinished &= "        If applicationData.Rows.Count > 0 Then" & vbCrLf
            checkApplicationFinished &= "            With applicationData.Rows(0)" & vbCrLf
            checkApplicationFinished &= "                Dim sectionsAllowed = GetSectionsAllowed().OrderBy(Function(section) section.Key).ToArray()" & vbCrLf & vbCrLf
            checkApplicationFinished &= "                For sectionNumber As Integer = 1 To sectionsAllowed.Count" & IIf(DefaultCertificationPage(), "", "-1") & vbCrLf
            checkApplicationFinished &= "                    If sectionsAllowed(sectionNumber - 1).Value And .Item(""Section"" & sectionNumber & ""Complete"") <> ""1"" Then" & vbCrLf
            checkApplicationFinished &= "                        messages.RedirectToMessage(MessageCode.NotFinished)" & vbCrLf
            checkApplicationFinished &= "                    End If" & vbCrLf
            checkApplicationFinished &= "                Next" & vbCrLf
            checkApplicationFinished &= "            End With" & vbCrLf
            checkApplicationFinished &= "        End If" & vbCrLf

            Return checkApplicationFinished
        End Function

        Private Shared Function GetSQLArchiveCondition() As String
            Return If(ProjectUsesArchive(), " AND COALESCE(Archived, 0) = 0", "")
        End Function

        Shared Function GetCheckAlreadySubmittedMethod(ByVal SQLMainTable As String, ByVal multipleSubmissions As String, ByVal SQLAdditionalCertificationStatement As String) As String
            Dim checkApplicationAlreadySubmitted As String = ""

            If General.Variables.isFrontend Then
                checkApplicationAlreadySubmitted &= "Shared Sub CheckAlreadySubmitted(ByVal sUsername As String)" & vbCrLf
                checkApplicationAlreadySubmitted &= "        Dim existingRecords = GetDataTable(""Select * FROM "" & MAIN_DATABASE_TABLE_NAME & "" WHERE COALESCE(Deleted, 0) = 0 AND Username = '"" & sUsername & ""' ORDER BY ID DESC"", cnx)" & vbCrLf
                checkApplicationAlreadySubmitted &= "        Dim finishedRecord = (From row As DataRow In existingRecords.Rows Where row.Item(""Certification"") = ""Y"" Or row.Item(""Certification"") = ""1"" Select row).FirstOrDefault" & vbCrLf
                checkApplicationAlreadySubmitted &= "        Dim unfinishedRecord = (From row As DataRow In existingRecords.Rows Where row.Item(""Certification"") <> ""Y"" And row.Item(""Certification"") <> ""1"" Select row).FirstOrDefault()" & vbCrLf & vbCrLf

                checkApplicationAlreadySubmitted &= "        If existingRecords.Rows.Count = 0 Then" & vbCrLf
                checkApplicationAlreadySubmitted &= "            ExecuteNonQuery(""Insert "" & MAIN_DATABASE_TABLE_NAME & "" (Username) VALUES ('"" & sUsername & ""')"", cnx)" & vbCrLf
                checkApplicationAlreadySubmitted &= "        ElseIf unfinishedrecord Is Nothing And finishedRecord IsNot Nothing Then" & vbCrLf

                If multipleSubmissions = "1" Then
                    checkApplicationAlreadySubmitted &= "            CopyApplication(finishedRecord.Item(""ID""))" & vbCrLf
                Else
                    checkApplicationAlreadySubmitted &= "messages.RedirectToMessage(MessageCode.AlreadySubmitted)" & vbCrLf
                End If

                checkApplicationAlreadySubmitted &= "        End If" & vbCrLf
                checkApplicationAlreadySubmitted &= "    End Sub" & vbCrLf
            End If

            Return checkApplicationAlreadySubmitted
        End Function

        Public Shared Function GetApplicationCopyMethod(ByVal SQLMainTable As String) As String
            Dim copyApplicationMethodWriter As New CopyApplicationMethodWriter(SQLMainTable)

            Return copyApplicationMethodWriter.GetMethod()
        End Function

        Shared Function GetCheckReviewInformationMethod(ByVal sSQLMainTable As String) As String
            Dim sCheckReviewInformation As String = "Shared Sub CheckReviewInformation()" & vbCrLf

            sCheckReviewInformation &= "If GetDataTable(""Select * From " & sSQLMainTable & " Where " & GetCertificationCondition() & " AND COALESCE(DELETED, 0) = 0 AND Username = '"" & Common.GetCurrentUsername() & ""'"", cnx).Rows.Count = 0 And GetDataTable(""Select * From " & sSQLMainTable & " Where Certification = '1' AND COALESCE(Deleted, 0) = 0 AND Username = '"" & Common.GetCurrentUsername() & ""'"", cnx).Rows.Count > 0 Then" & vbCrLf
            sCheckReviewInformation &= "CType(GetCurrentPage().FindControl(""pnlReviewInformation""),Panel).Visible=True" & vbCrLf
            sCheckReviewInformation &= "End If" & vbCrLf
            sCheckReviewInformation &= "End Sub" & vbCrLf & vbCrLf

            Return sCheckReviewInformation
        End Function

        Shared Function GetCertificationLoadDDLsContent(ByVal sRequireLogin As String) As String
            Dim sCertificationLoadDDLsContent As String

            sCertificationLoadDDLsContent = "CheckAlreadySubmitted(" & GetUsernameReference(sRequireLogin) & ")" & vbCrLf & vbCrLf
            sCertificationLoadDDLsContent &= "CheckApplicationFinished(" & GetUsernameReference(sRequireLogin) & ")"

            Return sCertificationLoadDDLsContent
        End Function

        Shared Sub GetAdditionalButtons(ByVal nPageNumber As Integer, ByRef sAdditionalButtons As String, ByRef sAdditionalButtonsMethods As String)
            If nPageNumber >= 1 And nPageNumber < GetPageCount() Or (nPageNumber = GetPageCount() And DefaultCertificationPage()) Then
                sAdditionalButtons = "<asp:Button ID=""btnQuit"" runat=""server"" CssClass=""button"" Text=""Save & Exit"" />"
                sAdditionalButtonsMethods &= "Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click" & vbCrLf
                sAdditionalButtonsMethods &= "Page.Validate()" & vbCrLf
                sAdditionalButtonsMethods &= "If Page.IsValid Then" & vbCrLf
                sAdditionalButtonsMethods &= "SaveData()" & vbCrLf
                sAdditionalButtonsMethods &= "Response.redirect(""status.aspx?Quit=True"")" & vbCrLf
                sAdditionalButtonsMethods &= "End If" & vbCrLf
                sAdditionalButtonsMethods &= "End Sub" & vbCrLf
            End If
        End Sub


        Shared Function GetApplicationIDMethod(ByVal sSQLMainTable As String) As String
            Dim methodText As String = GetMailFile(GetTemplatePath() & "General\GetCurrentApplicationIDMethod.eml") & vbCrLf & vbCrLf
            Dim finalReturn As String

            methodText = MailFieldSubstitute(methodText, "(SQLMainTable)", sSQLMainTable)
            methodText = MailFieldSubstitute(methodText, "(UsernameReference)", If(IsMultipageForm(), """ & Common.GetCurrentUsername() & """, "-1"))

            If IsMultipageForm() Then
                methodText = MailFieldSubstitute(methodText, "(CheckCertification)", "AND master.dbo.FieldIsPositive(Certification) = 1")

                finalReturn = "Dim dtApplication As DataTable = GetDataTable(""SELECT TOP 1 * FROM " & sSQLMainTable & " WHERE Username = '"" & Common.GetCurrentUsername() & ""' AND COALESCE(Deleted, 0) = 0 Order by Certification asc, ID desc"", cnx)" & vbCrLf
                finalReturn &= "Return If(dtApplication.Rows.Count > 0, dtApplication.Rows(0).Item(""ID""), 0)" & vbCrLf
            Else
                finalReturn = "Return 0"
            End If

            '

            methodText = MailFieldSubstitute(methodText, "(FinalReturn)", finalReturn)

            Return methodText
        End Function

        Shared Function GetCertificationCondition() As String
            Dim sAdditionalCertificationStatement As String = GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTS & " WHERE ID = " & GetProjectID()).Rows(0).Item("SQLAdditionalCertificationStatement")

            Return "master.dbo.FieldIsPositive(Certification) = 0" & sAdditionalCertificationStatement
        End Function

        Shared Function DefaultCertificationPage() As Boolean
            Return GetDataTable("SELECT * FROM " & DT_WEBRAD_PROJECTPAGES & " WHERE ProjectID = " & GetProjectID() & " AND Certification = 1").Rows.Count = 0
        End Function

        Shared Function IsMultipageForm() As Boolean
            Return GetPageCount() > 1
        End Function

        Public Shared Function GetGetAuthenticatorMethod()
            Return If(General.Variables.isFrontend AndAlso CurrentProjectRequiresNonWhitworthLogin(), CreateAuthenticatorFunction(), "")
        End Function

        Public Shared Function CreateAuthenticatorFunction() As String
            Dim authenticatorFunction As String

            authenticatorFunction = "Public Shared Function GetAuthenticator() As Authenticator" & vbCrLf
            authenticatorFunction &= "        Return New Authenticator(MAIN_DATABASE_TABLE_NAME, """ & GetSQLDatabaseName() & """, AddressOf CopyApplication)" & vbCrLf
            authenticatorFunction &= "End Function" & vbCrLf & vbCrLf

            Return authenticatorFunction
        End Function

        Public Shared Function GetSectionsAllowed() As String
            Dim pages = GetProjectPages()

            If OptionalSections(pages) Then
                Dim TemplatePath As String
                Dim pageBody As String

                TemplatePath = GetTemplatePath() & "\General\SectionsAllowed.eml"
                pageBody = GetMailFile(TemplatePath)

                Dim sectionsAllowed As String = ""

                For counter As Integer = 1 To pages.Count
                    sectionsAllowed &= "sectionsAllowed(" & counter & ") = " & GetSectionAllowedCondition(pages(counter - 1)) & vbCrLf
                Next

                pageBody = MailFieldSubstitute(pageBody, "(SectionsAllowed)", sectionsAllowed)

                RemoveUnusedTemplateFields(pageBody)

                Return pageBody
            End If

            Return ""
        End Function

        Private Shared Function OptionalSections(pages() As ProjectPage) As Boolean
            Return pages.Count > 1 And pages.Where(Function(p) If(p.Dependent, False)).Count > 0
        End Function

        Private Shared Function GetSectionAllowedCondition(ByVal page As ProjectPage) As String
            If page.Dependent Then
                Dim selectionControl = page.Project.ProjectControls.FirstOrDefault(Function(pc) pc.ID = page.SelectionControl)

                If IsMultiValuedListControlType(selectionControl.ControlType) Then
                    Return "(GetDataTable(""Select * FROM " & selectionControl.SQLInsertItemTable & " WHERE " & selectionControl.Name & " = '"" & CleanSQL(""" & page.SelectionValue & """) & ""' AND " & selectionControl.ForeignID & " = "" & currentApplicationID, cnx).Rows.Count > 0)"
                Else
                    Return "(.Item(""" & selectionControl.Name & """) = """ & page.SelectionValue & """)"
                End If
            Else
                Return "True"
            End If
        End Function

        Public Shared Function GetStatusSectionsAllowedCollection() As String
            If OptionalSections(GetProjectPages()) Then
                Return "<% Dim sectionsAllowed = Common.GetSectionsAllowed()%>" & vbCrLf
            End If

            Return ""
        End Function

        Private Shared Function GetProjectPages() As ProjectPage()
            Return currentProject.ProjectPages.OrderBy(Function(pp) pp.ID).ToArray()
        End Function

        Public Shared Function GetSectionAllowedCheck(pageNumber As Integer)
            Dim pages = GetProjectPages()

            If pages(pageNumber - 1).Dependent Then
                Return "CheckSectionAllowed(" & pageNumber & ")" & vbCrLf
            End If

            Return ""
        End Function
    End Class
End Namespace
