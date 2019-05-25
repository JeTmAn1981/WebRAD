Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.Mail
Imports System
Imports System.Text.RegularExpressions
Imports System.Linq
Imports Common
Imports WhitTools
Imports WhitTools.ActiveDirectory
Imports WhitTools.Address
Imports WhitTools.Admissions
Imports WhitTools.Admissions_Graduate
Imports WhitTools.Admissions_Password
Imports WhitTools.Admissions_Prospect
Imports WhitTools.Alumni
Imports WhitTools.Audio
Imports WhitTools.Authenticator
Imports WhitTools.BillingInformation
Imports WhitTools.Comparer
Imports WhitTools.Converter
Imports WhitTools.Cookies
Imports WhitTools.DataTables
Imports WhitTools.DataTablesSupplied
Imports WhitTools.DataWarehouse
Imports WhitTools.eCommerce
Imports WhitTools.InvoiceVariable
Imports WhitTools.eCommerceCoupons
Imports WhitTools.eCommerceStatusChecks
Imports WhitTools.Email
Imports WhitTools.EmailTemplate
Imports WhitTools.EmploymentApplication
Imports WhitTools.Encryption
Imports WhitTools.ErrorHandler
Imports WhitTools.FacilitiesServices
Imports WhitTools.File
Imports WhitTools.fileUploadParameters
Imports WhitTools.FileCompare
Imports WhitTools.Filler
Imports WhitTools.FilteredRepeaterSelector
Imports WhitTools.Formatter
Imports WhitTools.GetDatatableParams
Imports WhitTools.Getter
Imports WhitTools.GlobalEnum
Imports WhitTools.GSEScholarshipApplication
Imports WhitTools.Local
Imports WhitTools.Maintenance
Imports WhitTools.ProspectInfoLoader
Imports WhitTools.RAD
Imports WhitTools.Repeaters
Imports WhitTools.RSS
Imports WhitTools.RulesAssignments
Imports WhitTools.Setter
Imports WhitTools.Sorter
Imports WhitTools.SQL
Imports WhitTools.StudentApplications
Imports WhitTools.ReferenceRequestSubmitter
Imports WhitTools.StudentEmployment
Imports WhitTools.TextBoxes
Imports WhitTools.TravelRequest
Imports WhitTools.UserFileAccessRights
Imports WhitTools.Utilities
Imports WhitTools.SetValidParams
Imports WhitTools.ValidateDateParams
Imports WhitTools.Validator
Imports WhitTools.Variables
Imports WhitTools.VehicleRegistrationDetail
Imports WhitTools.VehicleRequest
Imports WhitTools.Video
Imports WhitTools.WebTeam
Imports WhitTools.Workflow
Imports WhitTools.XML
Imports WhitTools.messages

Public Class message
    Inherits System.Web.UI.Page

    Dim pageName As String
#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then
            CheckMessageCode()
            LoadFormText()
            LoadDDLs()
        End If
    End Sub

    Function CheckMessageCode() As Boolean
        Dim currentMessage = (From m As messages.Message In messages.messageTypes Where m.code = Session("CurrentMessageCode") Select m).FirstOrDefault()

        If currentMessage IsNot Nothing Then
            pageName = currentMessage.pageName

            If currentMessage.code = MessageCode.ApplicationComplete Then
                FormsAuthentication.SignOut()
                SetSessionVariable(SESSION_USER_ID, "")
            End If
        End If
    End Function

    Sub LoadFormText()
        ftLoader.LoadFormText(pageName)
    End Sub

    Sub LoadDDLs()

    End Sub


End Class

