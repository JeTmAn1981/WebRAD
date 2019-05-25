Imports Microsoft.VisualBasic
Imports System.Data
Imports WhitTools.Getter
Imports WhitTools.Setter
Imports System.Data.SqlClient
Imports System.Runtime.Remoting.Metadata.W3cXsd2001
Imports WhitTools.SQL
Imports Common
Imports Common.Actions
Imports NLog
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.Collections.Generic
Imports System
Imports System.Diagnostics

Namespace General
    Public Class Variables

#Region "Tables"
        Public Const DT_WEBRAD_CONTROLACTIONTYPES As String = "web3.WebRAD.dbo.ControlActionTypes"
        Public Const DT_WEBRAD_CONTROLDATATYPES As String = "web3.WebRAD.dbo.ControlDataTypes"
        Public Const DT_WEBRAD_CONTROLSQLTYPES As String = "web3.WebRAD.dbo.ControlSQLTypes"
        Public Const DT_WEBRAD_CONTROLDISPLAYTYPES As String = "web3.WebRAD.dbo.ControlDisplayTypes"
        Public Const DT_WEBRAD_CONTROLTYPEACTIONS As String = "web3.WebRAD.dbo.ControlTypeActions"
        Public Const DT_WEBRAD_CONTROLTYPEDETAILREQUIREMENTS As String = "web3.WebRAD.dbo.ControlTypeDetailRequirements"
        Public Const DT_WEBRAD_CONTROLTYPEDETAILS As String = "web3.WebRAD.dbo.ControlTypeDetails"
        Public Const DT_WEBRAD_CONTROLTYPEDETAILTYPES As String = "web3.WebRAD.dbo.ControlTypeDetailTypes"
        Public Const DT_WEBRAD_CONTROLTYPEDETAILCOLUMNEXCLUSIONS As String = "web3.WebRAD.dbo.ControlTypeDetailColumnExclusions"
        Public Const DT_WEBRAD_CONTROLTYPEFILETYPESALLOWED As String = "web3.WebRAD.dbo.ControlTypeFileTypesAllowed"
        Public Const DT_WEBRAD_CONTROLTYPEITEMS As String = "web3.WebRAD.dbo.ControlTypeItems"
        Public Const DT_WEBRAD_CONTROLTYPES As String = "web3.WebRAD.dbo.ControlTypes"
        Public Const DT_WEBRAD_COMPOSITECONTROLS As String = "web3.WebRAD.dbo.CompositeControls"
        Public Const DT_WEBRAD_CONTROLTYPEVALIDATORS As String = "web3.WebRAD.dbo.ControlTypeValidators"
        Public Const DT_WEBRAD_DATAMETHODTYPES As String = "web3.WebRAD.dbo.DataMethodTypes"
        Public Const DT_WEBRAD_FILETYPES As String = "web3.WebRAD.dbo.FileTypes"
        Public Const DT_WEBRAD_LAYOUTTYPES As String = "web3.WebRAD.dbo.LayoutTypes"
        Public Const DT_WEBRAD_LAYOUTSUBTYPES As String = "web3.WebRAD.dbo.LayoutSubtypes"
        Public Const DT_WEBRAD_LOGINCOLUMNTYPES As String = "web3.WebRAD.dbo.LoginColumnTypes"
        Public Const DT_WEBRAD_BACKENDACTIONTYPES As String = "web3.WebRAD.dbo.BackendActionTypes"
        Public Const DT_WEBRAD_PROJECTBACKENDOPTIONCOLUMNS As String = "web3.WebRAD.dbo.ProjectBackendOptionColumns"
        Public Const DT_WEBRAD_PROJECTBACKENDOPTIONS As String = "web3.WebRAD.dbo.ProjectBackendOptions"
        Public Const DT_WEBRAD_PROJECTBACKENDADDITIONALLINKS As String = "web3.WebRAD.dbo.ProjectBackendAdditionalLinks"
        Public Const DT_WEBRAD_BACKENDOPTIONTYPES As String = "web3.WebRAD.dbo.BackendOptionTypes"
        Public Const DT_WEBRAD_BACKENDOPTIONOPERATORTYPES As String = "web3.WebRAD.dbo.BackendOptionOperatorTypes"
        Public Const DT_WEBRAD_PROJECTBACKEND_EXPORTS As String = "web3.WebRAD.dbo.ProjectBackendExports"
        Public Const DT_WEBRAD_PROJECTBACKEND_REPORTS As String = "web3.WebRAD.dbo.ProjectBackendReports"
        Public Const DT_WEBRAD_PROJECTCONTROLFILETYPESALLOWED As String = "web3.WebRAD.dbo.ProjectControlFileTypesAllowed"
        Public Const DT_WEBRAD_PROJECTCONTROLLISTITEMS As String = "web3.WebRAD.dbo.ProjectControlListItems"
        Public Const DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONS As String = "web3.WebRAD.dbo.ProjectControlPostbackActions"
        Public Const DT_WEBRAD_PROJECTCONTROLPOSTBACKACTIONTRIGGERVALUES As String = "web3.WebRAD.dbo.ProjectControlPostbackActionTriggerValues"
        Public Const DT_WEBRAD_PROJECTCONTROLS As String = "web3.WebRAD.dbo.ProjectControls"
        Public Const DT_WEBRAD_PROJECTCOLUMNS As String = "web3.WebRAD.dbo.ProjectColumns"
        Public Const DT_WEBRAD_PROJECTPAGES As String = "web3.WebRAD.dbo.ProjectPages"
        Public Const DT_WEBRAD_PROJECTADDITIONALOPERATIONS As String = "web3.WebRAD.dbo.ProjectAdditionalOperations"
        Public Const DT_WEBRAD_ADDITIONALOPERATIONTYPES As String = "web3.WebRAD.dbo.AdditionalOperationTypes"
        Public Const AFTER_PAGE_LOAD_TYPE As Integer = 1
        Public Const AFTER_PAGE_SUBMIT_TYPE As Integer = 2
        Public Const PAGE_HEADER_TYPE As Integer = 3
        Public Const BEFORE_PAGE_LOAD_TYPE As Integer = 4
        Public Const COMMON_METHODS_TYPE As Integer = 5
        Public Const PAGE_METHODS_TYPE As Integer = 6
        Public Const IMPORTS_TYPE As Integer = 7
        Public Const BEFORE_PAGE_SUBMIT_TYPE As Integer = 8


        Public Const DT_WEBRAD_PROJECTS As String = "web3.WebRAD.dbo.Projects"
        Public Const DT_WEBRAD_PROJECTSUPERVISORS As String = "web3.WebRAD.dbo.ProjectSupervisors"
        Public Const DT_WEBRAD_PROJECTANCILLARYMAINTENANCE As String = "web3.WebRAD.dbo.ProjectAncillaryMaintenance"
        Public Const DT_WEBRAD_PROJECTBUILDS As String = "web3.WebRAD.dbo.ProjectBuilds"
        Public Const DT_WEBRAD_PROJECTBUILDPAGES As String = "web3.WebRAD.dbo.ProjectBuildPages"
        Public Const DT_WEBRAD_PROJECTBUILDBACKENDOPTIONS As String = "web3.WebRAD.dbo.ProjectBuildBackendOptions"
        Public Const DT_WEBRAD_PROJECTBUILDBACKENDANCILLARYMAINTENANCE As String = "web3.WebRAD.dbo.ProjectBuildBackendAncillaryMaintenance"
        Public Const DT_WEBRAD_SQLSERVERS As String = "web3.WebRAD.dbo.SQLServers"
        Public Const DT_WEBRAD_SQLDATABASES As String = "web3.WebRAD.dbo.SQLDatabases"
        Public Const DT_WEBRAD_VALIDATORTYPES As String = "web3.WebRAD.dbo.ValidatorTypes"
        Public Const DT_WEBRAD_PROJECTEMAILMESSAGES As String = "web3.WebRAD.dbo.ProjectEmailMessages"
        Public Const DT_WEBRAD_PROJECTEMAILMESSAGESUBMITTERCONTROLS As String = "web3.WebRAD.dbo.ProjectEmailMessageSubmitterControls"
        Public Const DT_TOPLEVELPROJECTCONTROLS_V As String = "web3.WebRAD.dbo.TopLevelProjectControls_v"
        Public Const DT_WEBRAD_PROJECTDATASOURCES As String = "web3.WebRAD.dbo.ProjectDataSources"
        Public Const DT_WEBRAD_DATASOURCEPARENTTYPES As String = "web3.WebRAD.dbo.DataSourceParentTypes"
        Public Const DT_WEBRAD_PROHIBITEDCOLUMNNAMES As String = "web3.WebRAD.dbo.ProhibitedColumnNames"

#End Region

#Region "Strings"
        Public Const S_SUBMITTER_EMAIL_MESSAGE = "Thank you for submitting the <b>PageTitleHere</b> form.  Your submission has been received and is now being processed."
        Public Const S_PRINTABLE_LINK As String = "<a href=""updateprintable.aspx?ID=<%= Request.QueryString(""ID"")%>"" target=""_blank"">Printable Version</a>" & vbCrLf & "<br />" & vbCrLf
        Public Const S_CONFIRMATION_MESSAGE As String = "Your submission has been received and is now being processed."
        Public Const S_CLOSED_MESSAGE As String = "Sorry, the <strong>FormNameHere</strong> form is now closed."
        Public Const S_PROJECTFILESPATH As String = "\\web2\~Whitworth\Administration\InformationSystems\Forms\WebRAD\ProjectFiles\"
        Public Const S_PROJECTFILESNETWORKPATH As String = "\\web2\~Whitworth\Administration\InformationSystems\Forms\WebRAD\ProjectFiles\"
        Public Const S_WHITTOOLS_DIR As String = "\\web1\~whitworth\~WhitTools\WhitTools\bin\"
        Public Const S_WEBRAD_TEMPLATES_BASE_DIR As String = "..\Templates\WebRAD\"
        Public Const S_LAYOUTTYPE_VERTICAL As String = "1"
        Public Const S_LAYOUTTYPE_HORIZONTAL As String = "2"
        Public Const S_LAYOUTSUBTYPE_ORDEREDLIST As String = "1"
        Public Const S_LAYOUTSUBTYPE_UNORDEREDLIST As String = "2"
        Public Const S_LAYOUTSUBTYPE_NOLIST As String = "3"
        Public Const S_LAYOUTSUBTYPE_ITEMNUMBERS As String = "4"
        Public Const S_LAYOUTSUBTYPE_NOITEMNUMBERS As String = "5"
        Public Const S_ITEMS_NONESUPPLIED As String = "3"
        Public Const S_BACKEND_OPTION_SCHEDULE As String = "Schedule page"
        Public Const S_BACKEND_OPTION_SORT As String = "Sort"
        Public Const S_BACKEND_OPTION_FILTER As String = "Filter"
        Public Const S_BACKEND_OPTION_INSERT_PAGE As String = "Insert page"
        Public Const S_BACKEND_OPTION_ARCHIVE_VIEW As String = "Archive view"
        Public Const S_BACKEND_OPTION_PRINTABLE_VIEW As String = "Printable view"
        Public Const S_BACKEND_OPTION_EXPORT As String = "Export"
        Public Const S_BACKEND_OPTION_SEARCH As String = "Search"
        Public Const S_BACKEND_OPTION_PAGING As String = "Paging"
        Public Const S_BACKEND_OPTION_ANCILLARY_MAINTENANCE As String = "Ancillary maintenance"
        Public Const S_BACKEND_OPTION_REPORT As String = "Report"
        Public Const S_BACKEND_OPTION_MAINLISTING As String = "Main listing page"
        Public Const S_BACKEND_OPTION_UPDATE As String = "Update page"
        Public Const S_NAVIGATION_FILE_REFERENCE As String = "<!-- #include file=""navigation.htm"" -->"
        Public Const S_SUBMISSIONS_ID_LABEL As String = "CType(currentItem.Items(nCounter).FindControl(""lblID""), Label).Text"
        Public Const ACTION_REQUIRED_SQL_CONDITION As String = "(PerformPostbackAction = 1 Or ListSelections = 1)"
#End Region

#Region "ControlTypes"
        Public Const N_REPEATER_CONTROL_TYPE As Integer = 18
        Public Const N_PANEL_CONTROL_TYPE As Integer = 19
        Public Const N_RADIOBUTTON_CONTROL_TYPE As Integer = 3
        Public Const N_CHECKBOX_CONTROL_TYPE As Integer = 1
        Public Const N_CHECKBOXLIST_CONTROL_TYPE As Integer = 2
        Public Const N_DROPDOWNLIST_CONTROL_TYPE As Integer = 5
        Public Const N_RADIOBUTTONLIST_CONTROL_TYPE As Integer = 4
        Public Const N_YESNO_RADIOBUTTONLIST_CONTROL_TYPE As Integer = 9
        Public Const N_LISTBOX_CONTROL_TYPE As Integer = 8
        Public Const N_TEXTBOX_CONTROL_TYPE As Integer = 6
        Public Const N_TEXTBOX_MULTILINE_CONTROLTYPE As Integer = 48
        Public Const N_LABEL_CONTROL_TYPE As Integer = 7
        Public Const N_DATE_CONTROL_TYPE As Integer = 20
        Public Const N_IDNUMBER_CONTROL_TYPE As Integer = 37
        Public Const N_UPLOAD_FILE_CONTROL_TYPE As Integer = 29
        Public Const N_UPLOAD_IMAGE_CONTROL_TYPE As Integer = 28
        Public Const N_TIME_PICKER_CONTROL_TYPE As Integer = 51
        Public Const N_PREFIX_CONTROL_TYPE As Integer = 46
        Public Const N_FORMGROUP_CONTROL_TYPE As Integer = 47
        Public Const N_PROSPECTGATHERING_USERCONTROL_CONTROL_TYPE As Integer = 50
#End Region


#Region "Other Types And ID's"
        Public Const N_REPEATER_DATA_TYPE As Integer = 9
        Public Const N_TEXTBOX_DATA_TYPE As Integer = 1
        Public Const N_UPLOAD_FILE_DATA_TYPE As Integer = 12
        Public Const N_SQL_INT_TYPE As Integer = 2
        Public Const N_SQL_FLOAT_TYPE As Integer = 3
        Public Const N_SQL_MONEY_TYPE As Integer = 5
        Public Const N_SQL_DATETIME_TYPE As Integer = 4
        Public Const SQL_DATA_TYPE_VARCHAR As String = "nvarchar"
        Public Const N_BACKENDUPDATEACTION_TYPE As Integer = 1
        Public Const N_BACKENDCUSTOMACTION_TYPE As Integer = 2
        Public Const N_FIRSTNAME_CONTROLID As Integer = -14
        Public Const N_LASTNAME_CONTROLID As Integer = -13
        Public Const N_WHITWORTHID_CONTROLID As Integer = -11
        Public Const N_PHONE_CONTROLID As Integer = -18
        Public Const N_EMAIL_CONTROLID As Integer = -12
        Public Const N_CLASS_CONTROLID As Integer = -17
        Public Const N_DATESUBMITTED_CONTROLID As Integer = -16
        Public Const N_IDNUMBER_CONTROLID As Integer = -15
        Public Const N_UPDATEREPEATERITEMS_ACTIONTYPE As Integer = 2
        Public Const N_LISTSELECTIONS_ACTIONTYPE As Integer = 8
        Public Const N_DROPDOWNLIST_DATATYPE As Integer = 3
        Public Const N_RADIOBUTTONLIST_DATATYPE As Integer = 7
        Public Const N_LISTBOX_DATATYPE As Integer = 8
        Public Const N_LABEL_DATATYPE As Integer = 2
        Public Const N_CHECKBOXLIST_DATATYPE As Integer = 5
        Public Const N_CHECKBOX_DATATYPE As Integer = 4
        Public Const N_TEXTLITERAL_DATA_TYPE As Integer = 13
        Public Const N_DIV_DATATYPE As Integer = 14
        Public Const N_USERCONTROL_DATATYPE As Integer = 15
        Public Const N_CUSTOMACTIONTYPE_ID As Integer = 7
        Public Const N_WEBRAD_AOPAGELOADTYPE As Integer = 1
        Public Const N_WEBRAD_AOPAGESUBMITTYPE As Integer = 2
        Public Const N_WEBRAD_AOPAGEHEADERTYPE As Integer = 3
        Public Const N_ADDITIONALOPERATIONTYPE_COMMON As Integer = 5
        Public Const N_CONTROL_DATASOURCEPARENTTYPE As Integer = 1
        Public Const N_REPORT_DATASOURCEPARENTTYPE As Integer = 2
        Public Const N_ACTION_DATASOURCEPARENTTYPE As Integer = 3
        Public Const N_DISPLAY_DETAILTYPECATEGORY As Integer = 1
        Public Const N_DATA_DETAILTYPECATEGORY As Integer = 2
        Public Const N_GENERAL_DETAILTYPECATEGORY As Integer = 3
        Public Const N_ACTION_DETAILTYPECATEGORY As Integer = 4
        Public Const N_DISPLAYLOCATION_BACKENDONLY As Integer = 3
        Public Const N_DISPLAYTYPE_STANDARD As Integer = 1
        Public Const N_DISPLAYTYPE_FLOATNEWROW As Integer = 2
        Public Const N_DISPLAYTYPE_FLOAT As Integer = 3
        Public Const N_DISPLAYTYPE_STACK_NEWROW As Integer = 8
        Public Const N_DISPLAYTYPE_STACK As Integer = 4
        Public Const N_SQLDEFAULTVALUE_DETAILTYPEID As Integer = 29
        Public Const N_CONTROLACTIONTYPE_VISIBLE As Integer = 1
        Public Const N_VISIBLE_INVISIBLE_VALUE As Integer = 0
        Public Const N_VISIBLE_DEPENDENT_VALUE As Integer = 3
        Public Const N_VISIBLE_CUSTOM_VALUE As Integer = 2
        Public Const N_ADDITIONALOPERATIONTYPE_IMPORTS As Integer = 7
        Public Const N_BACKEND_OPTIONTYPE_MAINLISTING As Integer = 16
        Public Const N_BACKEND_OPTIONTYPE_UPDATE As Integer = 17
        'SQLDefaultValue'
#End Region

        Public Shared Property cnx() As SqlConnection
            Get
                If HttpContext.Current.Session("WebRADSQLConnection") Is Nothing Then
                    HttpContext.Current.Session("WebRADSQLConnection") = CreateSQLConnection("WebRAD")
                End If

                Return HttpContext.Current.Session("WebRADSQLConnection")
            End Get

            Set(ByVal value As SqlConnection)
                HttpContext.Current.Session("WebRADSQLConnection") = value
            End Set
        End Property

        Public Shared Property pageNumber() As Integer
            Get
                If HttpContext.Current.Session("pageNumber") Is Nothing Then
                    HttpContext.Current.Session("pageNumber") = -1
                End If

                Return HttpContext.Current.Session("pageNumber")
            End Get

            Set(ByVal value As Integer)
                HttpContext.Current.Session("pageNumber") = value
            End Set
        End Property
        Public Shared Property isFrontend() As Boolean
            Get
                If HttpContext.Current.Session("isFrontend") Is Nothing Then
                    HttpContext.Current.Session("isFrontend") = True
                End If

                Return HttpContext.Current.Session("isFrontend")
            End Get

            Set(ByVal value As Boolean)
                HttpContext.Current.Session("isFrontend") = value
            End Set
        End Property
        Public Shared Property createFrontend() As Boolean
            Get
                If HttpContext.Current.Session("createFrontend") Is Nothing Then
                    HttpContext.Current.Session("createFrontend") = False
                End If

                Return HttpContext.Current.Session("createFrontend")
            End Get

            Set(ByVal value As Boolean)
                HttpContext.Current.Session("createFrontend") = value
            End Set
        End Property
        Public Shared Property createBackend() As Boolean
            Get
                If HttpContext.Current.Session("createBackend") Is Nothing Then
                    HttpContext.Current.Session("createBackend") = False
                End If

                Return HttpContext.Current.Session("createBackend")
            End Get

            Set(ByVal value As Boolean)
                HttpContext.Current.Session("createBackend") = value
            End Set
        End Property
        Public Shared Property isPrintable() As Boolean
            Get
                If HttpContext.Current.Session("isPrintable") Is Nothing Then
                    HttpContext.Current.Session("isPrintable") = False
                End If

                Return HttpContext.Current.Session("isPrintable")
            End Get

            Set(ByVal value As Boolean)
                HttpContext.Current.Session("isPrintable") = value
            End Set
        End Property
        Public Shared Property isSearch() As Boolean
            Get
                If HttpContext.Current.Session("isSearch") Is Nothing Then
                    HttpContext.Current.Session("isSearch") = False
                End If

                Return HttpContext.Current.Session("isSearch")
            End Get

            Set(ByVal value As Boolean)
                HttpContext.Current.Session("isSearch") = value
            End Set
        End Property
        Public Shared Property isInsert() As Boolean
            Get
                If HttpContext.Current.Session("isInsert") Is Nothing Then
                    HttpContext.Current.Session("isInsert") = False
                End If

                Return HttpContext.Current.Session("isInsert")
            End Get

            Set(ByVal value As Boolean)
                HttpContext.Current.Session("isInsert") = value
            End Set
        End Property
        Public Shared Property isWorkflow() As Boolean
            Get
                If HttpContext.Current.Session("isWorkflow") Is Nothing Then
                    HttpContext.Current.Session("isWorkflow") = False
                End If

                Return HttpContext.Current.Session("isWorkflow")
            End Get

            Set(ByVal value As Boolean)
                HttpContext.Current.Session("isWorkflow") = value
            End Set
        End Property
        Public Shared Property isMVC() As Boolean
            Get
                If HttpContext.Current.Session("isMVC") Is Nothing Then
                    HttpContext.Current.Session("isMVC") = False
                End If

                Return HttpContext.Current.Session("isMVC")
            End Get

            Set(ByVal value As Boolean)
                HttpContext.Current.Session("isMVC") = value
            End Set
        End Property
        Public Shared Property isBackendIndex() As Boolean
            Get
                If HttpContext.Current.Session("isBackendIndex") Is Nothing Then
                    HttpContext.Current.Session("isBackendIndex") = False
                End If

                Return HttpContext.Current.Session("isBackendIndex")
            End Get

            Set(ByVal value As Boolean)
                HttpContext.Current.Session("isBackendIndex") = value
            End Set
        End Property
        Public Shared Property UseJavascriptActions() As Boolean
            Get
                If HttpContext.Current.Session("UseJavascriptActions") Is Nothing Then
                    HttpContext.Current.Session("UseJavascriptActions") = False
                End If

                Return HttpContext.Current.Session("UseJavascriptActions")
            End Get

            Set(ByVal value As Boolean)
                HttpContext.Current.Session("UseJavascriptActions") = value
            End Set
        End Property
        Public Shared Property isArchive() As Boolean
            Get
                If HttpContext.Current.Session("isArchive") Is Nothing Then
                    HttpContext.Current.Session("isArchive") = False
                End If

                Return HttpContext.Current.Session("isArchive")
            End Get

            Set(ByVal value As Boolean)
                HttpContext.Current.Session("isArchive") = value
            End Set
        End Property

        Public Shared Property ancillaryProjectDT() As DataTable
            Get
                If HttpContext.Current.Session("ancillaryProjectDT") Is Nothing Then
                    HttpContext.Current.Session("ancillaryProjectDT") = New DataTable()
                End If

                Return HttpContext.Current.Session("ancillaryProjectDT")
            End Get

            Set(ByVal value As DataTable)
                HttpContext.Current.Session("ancillaryProjectDT") = value
            End Set
        End Property
        Public Shared Property controlsDT() As DataTable
            Get
                If HttpContext.Current.Session("controlsDT") Is Nothing Then
                    HttpContext.Current.Session("controlsDT") = New DataTable()
                End If

                Return HttpContext.Current.Session("controlsDT")
            End Get

            Set(ByVal value As DataTable)
                HttpContext.Current.Session("controlsDT") = value
            End Set
        End Property

        Public Shared Property searchControls() As DataTable
            Get
                If HttpContext.Current.Session("searchControls") Is Nothing Then
                    HttpContext.Current.Session("searchControls") = New DataTable()
                End If

                Return HttpContext.Current.Session("searchControls")
            End Get

            Set(ByVal value As DataTable)
                HttpContext.Current.Session("searchControls") = value
            End Set
        End Property


        Public Shared Property db() As WebRADEntities
            Get
                If HttpContext.Current.Session("db") Is Nothing Then
                    HttpContext.Current.Session("db") = New WebRADEntities()

                End If

                Return HttpContext.Current.Session("db")
            End Get

            Set(ByVal value As WebRADEntities)
                HttpContext.Current.Session("db") = value
            End Set
        End Property

        Public Shared Property projectActionData() As ActionData
            Get
                If HttpContext.Current.Session("projectActionData") Is Nothing Then
                    HttpContext.Current.Session("projectActionData") = New ActionData()
                End If

                Return HttpContext.Current.Session("projectActionData")
            End Get

            Set(ByVal value As ActionData)
                HttpContext.Current.Session("projectActionData") = value
            End Set
        End Property


        Public Shared Property sqlcnx() As SqlConnection
            Get
                Return HttpContext.Current.Session("sqlcnx")
            End Get

            Set(ByVal value As SqlConnection)
                HttpContext.Current.Session("sqlcnx") = value
            End Set
        End Property

        Public Shared Property projectDT() As DataTable
            Get
                Return HttpContext.Current.Session("projectDT")
            End Get

            Set(ByVal value As DataTable)
                HttpContext.Current.Session("projectDT") = value
            End Set
        End Property

        Public Shared Property pages() As List(Of ListItem)
            Get
                Return HttpContext.Current.Session("pages")
            End Get

            Set(ByVal value As List(Of ListItem))
                HttpContext.Current.Session("pages") = value
            End Set
        End Property

        Public Shared Property currentProjectBuild() As ProjectBuild
            Get
                Return HttpContext.Current.Session("currentProjectBuild")
            End Get

            Set(ByVal value As ProjectBuild)
                HttpContext.Current.Session("currentProjectBuild") = value
            End Set
        End Property
        Public Shared Property currentProject() As Project
            Get
                Return HttpContext.Current.Session("currentProject")
            End Get

            Set(ByVal value As Project)
                HttpContext.Current.Session("currentProject") = value
            End Set
        End Property

        Public Shared Property currentPage() As ProjectPage
            Get
                Return HttpContext.Current.Session("currentProjectPage")
            End Get

            Set(ByVal value As ProjectPage)
                HttpContext.Current.Session("currentProjectPage") = value
            End Set
        End Property
        Public Shared Property projectControls() As List(Of ProjectControl)
            Get
                Return HttpContext.Current.Session("projectControls")
            End Get

            Set(ByVal value As List(Of ProjectControl))
                HttpContext.Current.Session("projectControls") = value
            End Set
        End Property

        Public Shared Property logger() As NLog.ILogger
            Get
                Return HttpContext.Current.Session("logger")
            End Get

            Set(ByVal value As NLog.ILogger)
                HttpContext.Current.Session("logger") = value
            End Set
        End Property

        Public Shared Property projectName() As String
            Get
                Return HttpContext.Current.Session("projectName")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("projectName") = value
            End Set
        End Property
        Public Shared Property projectTitle() As String
            Get
                Return HttpContext.Current.Session("projectTitle")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("projectTitle") = value
            End Set
        End Property
        Public Shared Property SQLDatabaseName() As String
            Get
                Return HttpContext.Current.Session("SQLDatabaseName")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("SQLDatabaseName") = value
            End Set
        End Property
        Public Shared Property SQLServerName() As String
            Get
                Return HttpContext.Current.Session("SQLServerName")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("SQLServerName") = value
            End Set
        End Property
        Public Shared Property SQLMainTableName() As String
            Get
                Return HttpContext.Current.Session("SQLMainTableName")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("SQLMainTableName") = value
            End Set
        End Property

        Public Shared Property currentImportListing As String
            Get
                Return HttpContext.Current.Session("currentImportListing")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("currentImportListing") = value
            End Set
        End Property


        Public Shared Property storageProperties() As String
            Get
                Return HttpContext.Current.Session("storageProperties")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("storageProperties") = value
            End Set
        End Property
        Public Shared Property conceptualProperties() As String
            Get
                Return HttpContext.Current.Session("conceptualProperties")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("conceptualProperties") = value
            End Set
        End Property
        Public Shared Property scalarProperties() As String
            Get
                Return HttpContext.Current.Session("scalarProperties")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("scalarProperties") = value
            End Set
        End Property
        Public Shared Property modelProperties() As String
            Get
                Return HttpContext.Current.Session("modelProperties")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("modelProperties") = value
            End Set
        End Property
        Public Shared Property contentIncludes() As String
            Get
                Return HttpContext.Current.Session("contentIncludes")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("contentIncludes") = value
            End Set
        End Property
        Public Shared Property compileIncludes() As String
            Get
                Return HttpContext.Current.Session("compileIncludes")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("compileIncludes") = value
            End Set
        End Property
        Public Shared Property baseDir() As String
            Get
                Return HttpContext.Current.Session("baseDir")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("baseDir") = value
            End Set
        End Property
        Public Shared Property departmentLink() As String
            Get
                Return HttpContext.Current.Session("departmentLink")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("departmentLink") = value
            End Set
        End Property
        Public Shared Property departmentName() As String
            Get
                Return HttpContext.Current.Session("departmentName")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("departmentName") = value
            End Set
        End Property
        Public Shared Property departmentUrl() As String
            Get
                Return HttpContext.Current.Session("departmentUrl")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("departmentUrl") = value
            End Set
        End Property
        Public Shared Property projectLocation() As String
            Get
                Return HttpContext.Current.Session("projectLocation")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("projectLocation") = value
            End Set
        End Property
        Public Shared Property archiveRef() As String
            Get
                Return HttpContext.Current.Session("archiveRef")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("archiveRef") = value
            End Set
        End Property

        Public Shared Property projectType() As String
            Get
                If HttpContext.Current.Session("projectType") Is Nothing Then
                    HttpContext.Current.Session("projectType") = "Live"
                End If

                Return HttpContext.Current.Session("projectType")
            End Get

            Set(ByVal value As String)
                HttpContext.Current.Session("projectType") = value
            End Set
        End Property

        Public Shared Property stopwatch As Stopwatch
            Get
                Return _stopwatch
            End Get
            Set(value As Stopwatch)
                _stopwatch = value
            End Set
        End Property

        Shared _stopwatch As Stopwatch

        Public Shared Property lastProgressMark As TimeSpan
            Get
                Return _lastProgressMark
            End Get
            Set(value As TimeSpan)
                _lastProgressMark = value
            End Set
        End Property

        Shared _lastProgressMark As TimeSpan
    End Class
End Namespace
