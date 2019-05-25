''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'NAME: WhitTools Getter
'CREATED BY: Dallas Crockett
'PURPOSE: Provides a way to display the values of WhitTools.Filler functions.
'DATABASE: None
'DATE CREATED: 3/24/2010
'RELATED APPLICATIONS:
'
'MODIFIED BY:			DATE:			MODIFICATION:
'   Dallas Crockett     03/24/2010       Created the file.
'   Dallas Crockett     07/28/2010       Updated GetDataTableValues() to cutoff at a certain row for large DataTables.
'   Dallas Crockett     09/29/2010       Added the GetStudentInfo(), GetStudentIDByUsername(), GetStudentInfoByUsername() and GetDateTime() functions.
'   Dallas Crockett     05/12/2011       Updated the GetWebTeamPersonalEmails() function.
'   Tom Ryan            06/19/2012       Added GetWordCount()
'   Dallas Crockett     10/05/2012       Added GetCurrentPage(), GetCurrentUsername(), GetCurrentUserEmail(), GetCurrentStudentInfo()
'   Tom Ryan            10/09/2012       Added GetViewInvoiceLink()
'   Dallas Crockett     12/04/2012       Added GetMaintenanceSetValue()
'   Tom Ryan            01/25/2013       Added UserFileAccessRights class for checking user's permissions on files/folders
'   Dallas Crockett     04/02/2013       Added GetStack()
'   Tom Ryan            04/08/2013       Updated GetListOfSelectedValues() to handle repeaters
'   Dallas Crockett     04/25/2013       Added GetGuaranteedDate()
'   Dallas Crockett     04/26/2013       Added GetStudentIDByEmail() and GetStudentInfoByEmail()
'   Dallas Crockett     05/01/2013       Added GetUniqueInteger()
'   Dallas Crockett     11/06/2013       Added GetMasterFormsLink()
'   Dallas Crockett     04/17/2014       Added GetCurrentUserPhone() and GetCurrentUserPhoneExt()
'   Dallas Crockett     05/14/2014       Added GetGuaranteedPLID() and GetEmergencyContactPLID()
'   Tom Ryan            01/16/2015       Added GetSubDepartments()
'   Dallas Crockett     02/04/2016       Added GetWebConfigVariable()
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Security.Cryptography
Imports System.Web.Configuration
Imports System.Xml
Imports WhitTools.Converter
Imports WhitTools.Cookies
Imports WhitTools.DataTables
Imports WhitTools.DataWarehouse
Imports WhitTools.eCommerce
Imports WhitTools.Encryption
Imports WhitTools.ErrorHandler
Imports WhitTools.FacilitiesServices
Imports WhitTools.Formatter
Imports WhitTools.GlobalEnum
Imports WhitTools.Local
Imports WhitTools.RulesAssignments
Imports WhitTools.Admissions_Prospect
Imports WhitTools.Setter
Imports WhitTools.SQL
Imports WhitTools.Utilities
Imports WhitTools.Validator
Imports WhitTools.Variables
Imports WhitTools.WebTeam

Public Class Getter
    ''' <summary>
    ''' Used for displaying years on forms which correspond to the academic year. These displays may need to change, depending on what time of the year it is.
    ''' </summary>
    ''' <param name="nMonth">The month to check.</param>
    ''' <returns>The academic year in relation to the given month.</returns>
    ''' <remarks></remarks>
    Shared Function GetAcademicYear(ByVal nMonth As Integer) As Integer
        If Now.Month >= nMonth Then
            Return Now.Year + 1
        End If
        Return Now.Year
    End Function

    ''' <summary>
    ''' Used for displaying years on forms which correspond to the academic year. These displays may need to change, depending on what time of the year it is.
    ''' </summary>
    ''' <param name="dateTime">The date to check.</param>
    ''' <returns>The first academic year in relation to the given month. (Example: Jan 7, 2011 returns 2010 for the 2010/11 academic year)</returns>
    ''' <remarks>
    ''' Examples: 
    ''' Jan 7th, 2011 returns 2010 for the 2010/11 academic year
    ''' August 12th, 2011 returns 2011 for the 2011/12 academic year
    ''' </remarks>
    Shared Function GetAcademicYear(ByVal dateTime As DateTime) As Integer
        If dateTime.Month >= 7 Then
            Return dateTime.Year
        End If
        Return dateTime.Year - 1
    End Function

    ''' <summary>
    ''' Returns a string of all of the char codes within the provided code values.
    ''' </summary>
    ''' <remarks>If sDelimeter="table" then the results will be placed in a table.</remarks>
    Shared Function GetCharCodes(Optional ByVal nStartCode As Integer = 0, Optional ByVal nEndCode As Integer = 250, Optional ByVal sDelimeter As String = "<br />") As String
                Dim sCharList As String = ""
        Dim nCounter As Integer

        If sDelimeter = "table" Then
            sCharList = "<table>"
            For nCounter = nStartCode To nEndCode
                sCharList &= "<tr><td>" & nCounter.ToString() & "</td><td>=</td><td>" & Chr(nCounter).ToString() & "</td></tr>"
            Next
            sCharList &= "</table>"
        Else
            For nCounter = nStartCode To nEndCode
                sCharList &= nCounter & "=" & Chr(nCounter) & sDelimeter
            Next
        End If

        Return sCharList
    End Function

    ''' <summary>
    ''' Returns a string of the char codes of the provided string.
    ''' </summary>
    ''' <remarks></remarks>
    Shared Function GetCharCodes(ByVal sGetCodes As String, Optional ByVal sDelimeter As String = "<br />") As String
                Dim sCharList As String = "Char Codes:<br />"
        Dim nCounter As Integer

        If sDelimeter = "table" Then
            sCharList = "<table>"
            For nCounter = 0 To sGetCodes.Length - 1
                sCharList &= "<tr><td>" & sGetCodes.Chars(nCounter) & "</td><td>=</td><td>" & Asc(sGetCodes.Chars(nCounter)).ToString() & "</td></tr>"
            Next
            Return sCharList & "</table>"
        Else
            For nCounter = 0 To sGetCodes.Length - 1
                sCharList &= sGetCodes.Chars(nCounter) & "=" & Asc(sGetCodes.Chars(nCounter)).ToString() & sDelimeter
            Next
        End If

        Return sCharList & "<br />"
    End Function

    ''' <summary>
    ''' Returns a string of the values in a DataTable.
    ''' </summary>
    ''' <param name="dtValues">The DataTable to see the values of.</param>
    ''' <param name="sDelimeter">How to separate values.</param>
    ''' <param name="nRowCutoff">The row to stop finding values at.</param>
    ''' <param name="nDisplayType">The way the data is displayed.</param>
    ''' <param name="nColumnCutOff">The number of characters to display in the column fields.</param>
    ''' <returns>A string to display the table values.</returns>
    ''' <remarks>Display types: 1=vertical, 2=horizontal</remarks>
    Shared Function GetDataTableValues(ByVal dtValues As DataTable, Optional ByVal sDelimeter As String = "<br />", Optional ByVal nRowCutoff As Integer = -1, Optional ByVal nDisplayType As Integer = 1, Optional ByVal nColumnCutOff As Integer = -1, Optional ByVal bIncludeDataTypes As Boolean = False) As String
                Dim sValues As String = "DataTable Values:<br />"
        Dim nCounter, nCounter2, nCounterStop As Integer
                nCounterStop = dtValues.Rows.Count - 1
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check for a maximum number of rows to show
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If nRowCutoff > 0 And nRowCutoff < nCounterStop Then
            nCounterStop = nRowCutoff
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check the display type
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If nDisplayType = 1 Then
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Cycle through the rows
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            For nCounter2 = 0 To nCounterStop
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Cycle through the columns of this row
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                For nCounter = 0 To dtValues.Rows(0).ItemArray.Length - 1
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Add the row to the string
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    If IsDBNull(dtValues.Rows(nCounter2).Item(nCounter)) Then
                        sValues &= "Row " & nCounter2.ToString() & ": " & dtValues.Columns(nCounter).ToString() & "=" & S_NULL_VALUE & IIf(bIncludeDataTypes, " (" & Replace(dtValues.Columns(nCounter).DataType.ToString() & ")", "System.", ""), "") & sDelimeter
                    Else
                        sValues &= "Row " & nCounter2.ToString() & ": " & dtValues.Columns(nCounter).ToString() & "=" & dtValues.Rows(nCounter2).Item(nCounter).ToString() & IIf(bIncludeDataTypes, " (" & Replace(dtValues.Columns(nCounter).DataType.ToString() & ")", "System.", ""), "") & sDelimeter
                    End If
                Next
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Put an extra break between rows
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                sValues &= "<br />"
            Next
        ElseIf nDisplayType = 2 Then
                        Dim sCurrentRowValue As String
            Dim nLength As Integer = 1
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Start the values string
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sValues = "<table><tr>"
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Cycle through rows
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            For nCounter2 = 0 To nCounterStop 'nCounter2 = Rows
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Cycle through columns
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                For nCounter = 0 To dtValues.Columns.Count - 1
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Get the length of the item value
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    nLength = dtValues.Rows(nCounter2).Item(nCounter).ToString().Length
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Check for a null value
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    If Not IsDBNull(dtValues.Rows(nCounter2).Item(nCounter)) Then
                        If nColumnCutOff >= 0 Then
                            nLength = CInt(Math.Min(nColumnCutOff, dtValues.Rows(nCounter2).Item(nCounter).ToString().Length))
                        End If
                        sCurrentRowValue = dtValues.Rows(nCounter2).Item(nCounter).ToString().Substring(0, nLength)
                    Else
                        sCurrentRowValue = S_NULL_VALUE
                    End If
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Add the column value
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    sValues &= "<tr><td valign='top' align='left'><strong>" & dtValues.Columns(nCounter).ColumnName & ":</strong></td><td valign='top' width='25' /><td valign='top'>" & FormatPrintableText(sCurrentRowValue) & "</td><td valign='top' width='25' /><td>" & IIf(bIncludeDataTypes, Replace(dtValues.Columns(nCounter).DataType.ToString(), "System.", ""), "") & "</td></tr>"
                    sValues &= "<tr><td valign='top' colspan='5'><br /><br /></td></tr>"
                Next
            Next
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Finish the values string
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sValues &= "</table>"
        ElseIf nDisplayType = 3 Then
                        Dim sCurrentRowValue As String
            Dim nLength As Integer = 1
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Start the values string
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sValues = "<table><tr>"
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Cycle through rows
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            For nCounter2 = 0 To nCounterStop 'nCounter2 = Rows
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Cycle through columns
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                For nCounter = 0 To dtValues.Columns.Count - 1
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Get the length of the item value
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    nLength = dtValues.Rows(nCounter2).Item(nCounter).ToString().Length
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Check for a null value
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    If Not IsDBNull(dtValues.Rows(nCounter2).Item(nCounter)) Then
                        If nColumnCutOff >= 0 Then
                            nLength = CInt(Math.Min(nColumnCutOff, dtValues.Rows(nCounter2).Item(nCounter).ToString().Length))
                        End If
                        sCurrentRowValue = dtValues.Rows(nCounter2).Item(nCounter).ToString().Substring(0, nLength)
                    Else
                        sCurrentRowValue = S_NULL_VALUE
                    End If
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Add the column value
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    sValues &= "<tr><td valign='top' align='left'><strong>" & dtValues.Columns(nCounter).ColumnName & ":</strong></td><td valign='top' width='25' /><td valign='top'>" & FormatPrintableText(sCurrentRowValue) & "</td><td valign='top' width='25' /><td>" & IIf(bIncludeDataTypes, Replace(dtValues.Columns(nCounter).DataType.ToString(), "System.", ""), "") & "</td></tr>"
                Next
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Add space between rows
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                sValues &= "<tr><td><br /></td></tr>"
            Next
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Finish the values string
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sValues &= "</table>"
        Else
                        Dim nLength As Integer = 1
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Start the values string
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sValues &= "<table><tr><th>Row</th><th width='25' />"
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Cycle through the columns
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            For nCounter = 0 To dtValues.Columns.Count - 1
                sValues &= "<th align='center'>" & dtValues.Columns(nCounter).ColumnName & IIf(bIncludeDataTypes, "<br /><small>(" & Replace(dtValues.Columns(nCounter).DataType.ToString(), "System.", "") & ")</small>", "") & "</th><th width='25' />"
            Next
            sValues &= "</tr><tr><td colspan='" & ((dtValues.Columns.Count * 2) + 1).ToString() & "'><hr /></td></tr>"
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Cycle through the rows
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            For nCounter2 = 0 To nCounterStop 'nCounter2 = Rows
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Start the new table row
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                sValues &= "<tr><td>" & nCounter2.ToString() & "</td><td width='25' />"
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Cycle through the columns
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                For nCounter = 0 To dtValues.Columns.Count - 1 'nCounter = Columns
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Find the length of the column value
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    nLength = dtValues.Rows(nCounter2).Item(nCounter).ToString().Length
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Check for a null value
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    If Not IsDBNull(dtValues.Rows(nCounter2).Item(nCounter)) Then
                        If nColumnCutOff >= 0 Then
                            nLength = CInt(Math.Min(nColumnCutOff, dtValues.Rows(nCounter2).Item(nCounter).ToString().Length))
                        End If
                        sValues &= "<td>" & dtValues.Rows(nCounter2).Item(nCounter).ToString().Substring(0, nLength) & "</td><td width='25' />"
                    Else
                        sValues &= "<td>" & S_NULL_VALUE & "</td><td width='25' />"
                    End If
                Next
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Finish the table row
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                sValues &= "</tr>"
            Next
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Finish the table
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sValues &= "</table>"
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the readable DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sValues & "<br />"
    End Function

    ''' <summary>
    ''' Returns a string of the values in a DataRow.
    ''' </summary>
    ''' <param name="dr">The DataRow to see the values of.</param>
    ''' <param name="sDelimeter">How to separate values.</param>
    ''' <param name="nDisplayType">The way the data is displayed. 1=vertical, 2=horizontal</param>
    ''' <param name="nColumnCutOff">The number of characters to display in the column fields.</param>
    ''' <returns>A string to display the table values.</returns>
    ''' <remarks>Display types: 1=vertical, 2=horizontal</remarks>
    Shared Function GetDataRowValues(ByVal dr As DataRow, Optional ByVal sDelimeter As String = "<br />", Optional ByVal nDisplayType As Integer = 1, Optional ByVal nColumnCutOff As Integer = -1) As String
                Dim sValues As String = "DataRow Values:<br />"
        Dim nCounter As Integer
        Dim dtValues As DataTable = dr.Table
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check the display type
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If nDisplayType = 2 Then
                        Dim sCurrentRowValue As String
            Dim nLength As Integer = 1
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Start the values string
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sValues = "<table><tr>"
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Cycle through columns
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            For nCounter = 0 To dtValues.Columns.Count - 1
                With dtValues.Columns(nCounter)
                    If Not IsDBNull(dr.Item(.ColumnName)) Then
                        Try
                            sCurrentRowValue = dr.Item(.ColumnName).ToString()
                        Catch ex As Exception
                            sCurrentRowValue = ex.ToString()
                        End Try
                    Else
                        sCurrentRowValue = S_NULL_VALUE
                    End If
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Add the column value
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    sValues &= "<tr><td valign='top' align='left'><strong>" & dtValues.Columns(nCounter).ColumnName & ":</strong></td><td valign='top' width='25' /><td valign='top'>" & FormatPrintableText(sCurrentRowValue) & "</td><td valign='top' width='25' /><td>" & Replace(dtValues.Columns(nCounter).DataType.ToString(), "System.", "") & "</td></tr>"
                    sValues &= "<tr><td valign='top' colspan='5'><br /><br /></td></tr>"
                End With
            Next
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Finish the values string
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sValues &= "</table>"
        Else
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Cycle through the columns of this row
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            For nCounter = 0 To dtValues.Columns.Count - 1
                With dtValues.Columns(nCounter)
                    Try
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Add the columns and value to the string
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        If IsDBNull(dr.Item(.ColumnName)) Then
                            sValues &= dtValues.Columns(nCounter).ToString() & "=" & S_NULL_VALUE & " (" & Replace(dtValues.Columns(nCounter).DataType.ToString(), "System.", "") & ")" & sDelimeter
                        Else
                            sValues &= dtValues.Columns(nCounter).ToString() & "=" & dr.Item(.ColumnName).ToString() & " (" & Replace(dtValues.Columns(nCounter).DataType.ToString(), "System.", "") & ")" & sDelimeter
                        End If
                    Catch ex As Exception
                        sValues &= dtValues.Columns(nCounter).ToString() & "=" & ex.ToString() & sDelimeter
                    End Try
                End With
            Next
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the readable DataRow
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sValues & "<br />"
    End Function

    ''' <summary>
    ''' Returns the list of columns and their datatypes found in a DataTable.
    ''' </summary>
    ''' <param name="dtValues">The target DataTable.</param>
    ''' <param name="sDelimeter">The string which will be used to separate the columns displayed.</param>
    ''' <returns>A string to display the table column values.</returns>
    ''' <remarks></remarks>
    Shared Function GetDataTableColumnsHTML(ByVal dtValues As DataTable, Optional ByVal sDelimeter As String = "<br />") As String
                Dim sValues As String = "DataTable Columns:<br />"
        Dim nCounter As Integer
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Cycle through the DataTable columns
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        For nCounter = 0 To dtValues.Columns.Count - 1
            With dtValues.Columns(nCounter)
                sValues &= .ColumnName & " - " & .DataType.ToString() & sDelimeter
            End With
        Next
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the DataTable columns
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sValues & "<br />"
    End Function

    ''' <summary>
    ''' Returns a string of the values retrieved by a SQL select string in a comma-delimited list format.
    ''' </summary>
    ''' <param name="cnx">The SQL connection to be used for retrieving the data.</param>
    ''' <param name="sSelect">The SQL string to be used to specify which data is retrieved.</param>
    ''' <param name="sTarget">The single SQL column which is to be used as the basis for creating the list.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetListofValues(ByVal sSelect As String, ByVal sTarget As String, Optional ByVal sDelimiter As String = ",", Optional ByVal sAdditionalTarget As String = "", Optional ByVal cnx As SqlConnection = Nothing)
                Dim dt As DataTable = GetDataTable(sSelect, cnx)
        Dim nCounter As Integer
        Dim sList As String = ""
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check if the connection exists, if not create a generic one connected to Communications.
        'This can be used for any database, but the full path to the tables must be part of the queries.
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        CheckConnection(cnx)
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Cycle through DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        For nCounter = 0 To dt.Rows.Count - 1
            If sList <> "" Then
                sList &= sDelimiter & If(sDelimiter = ",", " ", "")
            End If
            If sAdditionalTarget <> "" Then
                If dt.Rows(nCounter).Item(sAdditionalTarget) <> "" Then
                    sList &= dt.Rows(nCounter).Item(sTarget) & " - " & dt.Rows(nCounter).Item(sAdditionalTarget)
                End If
            Else
                sList &= dt.Rows(nCounter).Item(sTarget)
            End If
        Next
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the DataTable values
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sList
    End Function

    ''' <summary>
    ''' Returns a delimited list of the values selected in a list control.
    ''' </summary>
    ''' <param name="oTarget"></param>
    ''' <param name="sSelectionType"></param>
    ''' <param name="sDelimiter"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetListOfSelectedValues(ByRef oTarget As Object, Optional ByVal sSelectionType As String = "Text", Optional ByVal sDelimiter As String = ",", Optional ByVal sCheckboxName As String = "chkAction", Optional ByVal sLabelName As String = "lblID", Optional ByVal bItemIndex As Boolean = False, Optional ByVal bRequireRepeaterItemSelected As Boolean = True)
                Dim nCounter As Integer
        Dim sSelected As String = ""
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check for the type of control
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If TypeOf oTarget Is RadioButtonList Then
            For nCounter = 0 To CType(oTarget, RadioButtonList).Items.Count - 1
                If CType(oTarget, RadioButtonList).Items(nCounter).Selected = True Then
                    If sSelected <> "" Then
                        sSelected &= sDelimiter
                    End If
                    If sSelectionType = "Text" Then
                        sSelected &= "'" & CType(oTarget, RadioButtonList).Items(nCounter).Text & "'"
                    Else
                        sSelected &= "'" & CType(oTarget, RadioButtonList).Items(nCounter).Value & "'"
                    End If
                End If
            Next
        ElseIf TypeOf oTarget Is DropDownList Then
            For nCounter = 0 To CType(oTarget, DropDownList).Items.Count - 1
                If CType(oTarget, DropDownList).Items(nCounter).Selected = True Then
                    If sSelected <> "" Then
                        sSelected &= sDelimiter
                    End If
                    If sSelectionType = "Text" Then
                        sSelected &= "'" & CType(oTarget, DropDownList).Items(nCounter).Text & "'"
                    Else
                        sSelected &= "'" & CType(oTarget, DropDownList).Items(nCounter).Value & "'"
                    End If
                End If
            Next
        ElseIf TypeOf oTarget Is ListBox Then
            For nCounter = 0 To CType(oTarget, ListBox).Items.Count - 1
                If CType(oTarget, ListBox).Items(nCounter).Selected = True Then
                    If sSelected <> "" Then
                        sSelected &= sDelimiter
                    End If

                    If sSelectionType = "Text" Then
                        sSelected &= "'" & CType(oTarget, ListBox).Items(nCounter).Text & "'"
                    Else
                        sSelected &= "'" & CType(oTarget, ListBox).Items(nCounter).Value & "'"
                    End If
                End If
            Next
        ElseIf TypeOf oTarget Is CheckBoxList Then
            For nCounter = 0 To CType(oTarget, CheckBoxList).Items.Count - 1
                If CType(oTarget, CheckBoxList).Items(nCounter).Selected = True Then
                    If sSelected <> "" Then
                        sSelected &= sDelimiter
                    End If
                    If sSelectionType = "Text" Then
                        sSelected &= "'" & CType(oTarget, CheckBoxList).Items(nCounter).Text & "'"
                    Else
                        sSelected &= "'" & CType(oTarget, CheckBoxList).Items(nCounter).Value & "'"
                    End If
                End If
            Next
        ElseIf TypeOf oTarget Is Repeater Then
            Try
                With CType(oTarget, Repeater)
                    For nCounter = 0 To .Items.Count - 1
                        If Not bRequireRepeaterItemSelected Or CType(.Items(nCounter).FindControl(sCheckboxName), CheckBox).Checked Then
                            If sSelected <> "" Then
                                sSelected &= sDelimiter
                            End If
                            If bItemIndex Then
                                sSelected &= "'" & nCounter & "'"
                            Else
                                Try
                                    sSelected &= "'" & CType(.Items(nCounter).FindControl(sLabelName), Label).Text & "'"
                                Catch ex As Exception
                                    'Empty catch statement
                                End Try
                            End If
                        End If
                    Next
                End With
            Catch ex As Exception
                'Empty catch statement
            End Try
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the selected string
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sSelected
    End Function

    ''' <summary>
    ''' Returns a DataTable filled with a student's information.
    ''' </summary>
    ''' <param name="sPLID">The PLID # to search on.</param>
    ''' <returns>A DataTable with the student's information.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentInfo(ByVal sPLID As String) As DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the student information for the provide PLID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            If sPLID <> "" Then
                Return GetDataTable("SELECT * FROM " & DT_PEOPLE_LISTING & " P LEFT OUTER JOIN " & DT_DW_CURRENT_STUDENT_INFO & " C ON P.PLID=C.STUDENT_ID WHERE PLID='" & FormatNumberLeadingZeroes(sPLID, 7) & "'")
            End If
        Catch ex As Exception
            ReportError(ex, "", "Trying to get a student's information and the attempt failed in WhitTools function Getter.GetStudentInfo(" & sPLID & ").")
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return an empty DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return New DataTable()
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sValue"></param>
    ''' <param name="sColumn"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetUserInfo(ByVal sValue As String, Optional ByVal sColumn As String = S_USERNAME) As DataTable
        Return GetDataTable("SELECT * FROM " & DV_USERINFO_V & " WHERE " & sColumn & "='" & CleanSQL(sValue) & "'")
    End Function

    Shared Function GetPrimaryDepartment(Optional ByVal sUsername As String = "", Optional ByVal nType As Integer = 1) As String
        Dim sDepartment As String = ""

        If sUsername = "" Then
            sUsername = GetCurrentUsername()
        End If

        Dim dtDepartment As DataTable = GetDataTable("SELECT * FROM " & DT_PEOPLE_LISTING & " WHERE plUsername = '" & sUsername & "'")

        If dtDepartment.Rows.Count > 0 Then
            sDepartment = dtDepartment.Rows(0).Item("plDepartment")

            If nType = 1 Then
                sDepartment = GetDepID(sDepartment)
            End If
        End If

        Return sDepartment
    End Function

    ''' <summary>
    ''' Returns the name of the student.
    ''' </summary>
    ''' <param name="sStudentID">The student id # to search on.</param>
    ''' <returns>The full name of the student.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentName(ByVal sStudentID As String) As String
        Return GetStudentInfo(sStudentID, "PLName")
    End Function

    ''' <summary>
    ''' Returns the first name of the student.
    ''' </summary>
    ''' <param name="sStudentID">The student id # to search on.</param>
    ''' <returns>The first name of the student.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentFirstName(ByVal sStudentID As String) As String
        Return GetStudentInfo(sStudentID, "PLFName")
    End Function

    ''' <summary>
    ''' Returns the last name of the student.
    ''' </summary>
    ''' <param name="sStudentID">The student id # to search on.</param>
    ''' <returns>The last name of the student.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentLastName(ByVal sStudentID As String) As String
        Return GetStudentInfo(sStudentID, "PLLName")
    End Function

    ''' <summary>
    ''' Returns the email address of the student.
    ''' </summary>
    ''' <param name="sStudentID">The student id # to search on.</param>
    ''' <returns>The email address of the student.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentEmail(ByVal sStudentID As String) As String
        Return GetStudentInfo(sStudentID, "PLEmail")
    End Function

    ''' <summary>
    ''' Gets a student's information based on their student id # and the column to pull from the DataTable.
    ''' </summary>
    ''' <param name="sStudentID">The student's ID #.</param>
    ''' <param name="sColumn">The DataTable column to pull from.</param>
    ''' <returns>The selected information for the student.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentInfo(ByVal sStudentID As String, ByVal sColumn As String) As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Validate parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sStudentID = "" Or sColumn = "" Then
            Return S_EMPTY_VALUE
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Retrieve the information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim dt As DataTable = GetStudentInfo(sStudentID)
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check if the information exists
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item(sColumn).ToString()
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return no info found
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_EMPTY_VALUE
    End Function

    ''' <summary>
    ''' Returns the student id # for the provided username.
    ''' </summary>
    ''' <param name="sUsername">The username to search on. Formatting is handled within the function.</param>
    ''' <returns>The user's student id #.</returns>
    ''' <remarks>GetCurrentUsername() can be used to get the current user's username.</remarks>
    Shared Function GetStudentIDByUsername(ByVal sUsername As String) As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Validate parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sUsername = "" Then
            Return S_UNAVAILABLE
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Retrieve the information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim dt As DataTable = GetDataTable("SELECT PLID FROM " & DT_PEOPLE_LISTING & " WHERE PLUsername='" & FormatUsername(sUsername) & "'")
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check if the information exists
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("PLID").ToString()
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return no information found
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_UNAVAILABLE
    End Function

    ''' <summary>
    ''' Gets a student's information based on their username and the column to pull from the DataTable.
    ''' </summary>
    ''' <param name="sUsername">The username to use. Formatting is taken care of within the function.</param>
    ''' <param name="sInfoToGet">The DataTable column to pull from.</param>
    ''' <returns>The selected information for the student.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentInfoByUsername(ByVal sUsername As String, ByVal sInfoToGet As String) As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Validate the parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sUsername = "" Or sInfoToGet = "" Then
            Return S_EMPTY_VALUE
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the student's information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim dt As DataTable = GetStudentInfo(GetStudentIDByUsername(sUsername))
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check that the information exists
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item(sInfoToGet).ToString()
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return an empty username
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_EMPTY_VALUE
    End Function

    ''' <summary>
    ''' Gets all of a student's information based on their username.
    ''' </summary>
    ''' <param name="sUsername">The username to use.</param>
    ''' <returns>A DataTable with the student's information.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentInfoByUsername(ByVal sUsername As String) As DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Validate the parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sUsername = "" Then
            Return New DataTable()
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the student information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return GetStudentInfo(GetStudentIDByUsername(sUsername))
    End Function

    ''' <summary>
    ''' Returns the student id # for the provided email address.
    ''' </summary>
    ''' <param name="sEmail">The email address to search on.</param>
    ''' <returns>The user's student id #.</returns>
    ''' <remarks>Request.ServerVariables("LOGON_USER").ToString() can be used to get the current user's username.</remarks>
    Shared Function GetStudentIDByEmail(ByVal sEmail As String) As String
                Dim dt As DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Trim the email
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        sEmail = Trim(sEmail)
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Validate parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sEmail = "" Then
            Return S_UNAVAILABLE
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the person information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If IsNumeric(sEmail) Then
            dt = GetDataTable("SELECT PLID FROM " & DT_PEOPLE_LISTING & " WHERE PLID='" & CleanSQL(sEmail) & "'")
        Else
            dt = GetDataTable("SELECT PLID FROM " & DT_PEOPLE_LISTING & " WHERE PLEmail='" & CleanSQL(sEmail) & "'")
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check if the PLID was found
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("PLID").ToString()
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return an unavailable email address
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_UNAVAILABLE
    End Function

    ''' <summary>
    ''' Gets a student's information based on their email address and the column to pull from the DataTable.
    ''' </summary>
    ''' <param name="sEmail">The email address to use. Formatting is taken care of within the function.</param>
    ''' <param name="sInfoToGet">The DataTable column to pull from.</param>
    ''' <returns>The selected information for the student.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentInfoByEmail(ByVal sEmail As String, ByVal sInfoToGet As String) As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Validate parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sEmail = "" Or sInfoToGet = "" Then
            Return S_EMPTY_VALUE
        End If
                Dim dt As DataTable = GetStudentInfo(GetStudentIDByEmail(sEmail))
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check if the info was found
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item(sInfoToGet).ToString()
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return unavailable information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_EMPTY_VALUE
    End Function

    ''' <summary>
    ''' Gets a student's information based on their email address.
    ''' </summary>
    ''' <param name="sEmail">The email address to use. Formatting is taken care of within the function.</param>
    ''' <returns>The selected information for the student.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentInfoByEmail(ByVal sEmail As String) As DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Validate parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sEmail = "" Then
            Return New DataTable()
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return GetStudentInfo(GetStudentIDByEmail(sEmail))
    End Function

    ''' <summary>
    ''' Gets a student's information by matching their name.
    ''' </summary>
    ''' <param name="sFirstName">The student's first name as it appears in DT_PEOPLE_LISTING.</param>
    ''' <param name="sLastName">The student's last name as it appears in DT_PEOPLE_LISTING.</param>
    ''' <returns>The student's information.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentInfoByName(ByVal sFirstName As String, ByVal sLastName As String) As DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Validate parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sFirstName = "" Or sLastName = "" Then
            Return New DataTable()
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Retrieve the information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            Return GetDataTable("SELECT * FROM " & DT_PEOPLE_LISTING & " AS P LEFT OUTER JOIN " & DT_DW_CURRENT_STUDENT_INFO & " ON PLID=STUDENT_ID WHERE PLFName='" & CleanSQL(sFirstName) & "' AND PLLName='" & CleanSQL(sLastName) & "'")
        Catch ex As Exception
            ReportError(ex, "", "Trying to get a student's information by their full name and the attempt failed in WhitTools function Getter.GetStudentInfoByName(" & sFirstName & ", " & sLastName & ").")
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return empty information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return New DataTable()
    End Function

    ''' <summary>
    ''' Gets a student's information by matching their name.
    ''' </summary>
    ''' <param name="sFirstName">The student's first name as it appears in DT_PEOPLE_LISTING.</param>
    ''' <param name="sLastName">The student's last name as it appears in DT_PEOPLE_LISTING.</param>
    ''' <param name="sColumn">The specific column to retrieve.</param>
    ''' <returns>The student's information.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentInfoByName(ByVal sFirstName As String, ByVal sLastName As String, ByVal sColumn As String) As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Validate parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sFirstName = "" Or sLastName = "" Then
            Return S_ERROR
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Retrieve the information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            Return GetDataTable("SELECT " & sColumn & " FROM " & DT_PEOPLE_LISTING & " AS P LEFT OUTER JOIN " & DT_DW_CURRENT_STUDENT_INFO & " ON PLID=STUDENT_ID WHERE PLFName='" & CleanSQL(sFirstName) & "' AND PLLName='" & CleanSQL(sLastName) & "'").Rows(0).Item(sColumn)
        Catch ex As Exception
            ReportError(ex, "", "Trying to get a student's information by their full name and the attempt failed in WhitTools function Getter.GetStudentInfoByName(" & sFirstName & ", " & sLastName & ", " & sColumn & ").")
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return empty information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_ERROR
    End Function

    ''' <summary>
    ''' Gets a student's information by matching their name.
    ''' </summary>
    ''' <param name="sName">The student's name as it appears in DT_PEOPLE_LISTING.</param>
    ''' <returns>The student's information.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentInfoByFullName(ByVal sName As String) As DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Validate parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Trim(sName) = "" Then
            Return New DataTable()
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Retrieve the information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            Return GetDataTable("SELECT * FROM " & DT_PEOPLE_LISTING & " AS P LEFT OUTER JOIN " & DT_DW_CURRENT_STUDENT_INFO & " ON PLID=STUDENT_ID WHERE PLName='" & CleanSQL(sName) & "'")
        Catch ex As Exception
            ReportError(ex, "", "Trying to get a student's information by their full name and the attempt failed in WhitTools function Getter.GetStudentInfoByName(" & sName & ").")
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return empty information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return New DataTable()
    End Function

    ''' <summary>
    ''' Gets a student's information by matching their name.
    ''' </summary>
    ''' <param name="sName">The student's name as it appears in DT_PEOPLE_LISTING.</param>
    ''' <param name="sColumn">The specific column of information to retrieve.</param>
    ''' <returns>The student's information.</returns>
    ''' <remarks></remarks>
    Shared Function GetStudentInfoByFullName(ByVal sName As String, ByVal sColumn As String) As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Validate parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Trim(sName) = "" Then
            Return S_ERROR
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Retrieve the information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            Return GetDataTable("SELECT " & sColumn & " FROM " & DT_PEOPLE_LISTING & " AS P LEFT OUTER JOIN " & DT_DW_CURRENT_STUDENT_INFO & " ON PLID=STUDENT_ID WHERE PLName='" & CleanSQL(sName) & "'").Rows(0).Item(sColumn)
        Catch ex As Exception
            ReportError(ex, "", "Trying to get a student's information by their full name and the attempt failed in WhitTools function Getter.GetStudentInfoByName(" & sName & ", " & sColumn & ").")
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return empty information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_ERROR
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sUsername"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetUserUndergraduate(ByVal sUsername As String) As Boolean

        Try

            Dim nAcademicLevel As Integer = GetStudentInfoByUsername(sUsername).Rows(0).Item("plClass").replace("0", "")

            Return CBool(nAcademicLevel >= 1 And nAcademicLevel <= 4)
        Catch ex As Exception
            'Empty catch statement
        End Try

        Return False
    End Function

    ''' <summary>
    ''' Returns the email address associated with the provided username.
    ''' </summary>
    ''' <param name="sUsername">The username of the person to retrieve the email address for.</param>
    ''' <returns>The email address associated with the provided username.</returns>
    ''' <remarks></remarks>
    Shared Function GetUserEmail(ByVal sUsername As String) As String
        Return GetStudentInfoByUsername(sUsername, "PLEmail")
    End Function

    ''' <summary>
    ''' Gets a DateTime object from the controls provided in the parameters.
    ''' </summary>
    ''' <param name="ddlMonth">The month ddl.</param>
    ''' <param name="ddlDay">The day ddl.</param>
    ''' <param name="ddlYear">The year ddl.</param>
    ''' <param name="ddlTime">The time ddl.</param>
    ''' <returns>A datetime object with the combined datetime values.</returns>
    ''' <remarks></remarks>
    Shared Function GetDateTime(ByVal ddlMonth As DropDownList, ByVal ddlDay As DropDownList, ByVal ddlYear As DropDownList, Optional ByVal ddlTime As DropDownList = Nothing) As DateTime
                Dim dateGet As DateTime

        If Not IsNothing(ddlTime) Then
            Try
                dateGet = ddlMonth.SelectedValue & "/" & ddlDay.SelectedValue & "/" & ddlYear.SelectedValue & " " & ddlTime.SelectedValue
            Catch ex As Exception
                dateGet = S_NULL_DATE
            End Try
        Else
            Try
                dateGet = ddlMonth.SelectedValue & "/" & ddlDay.SelectedValue & "/" & ddlYear.SelectedValue
            Catch ex As Exception
                dateGet = S_NULL_DATE
            End Try
        End If

        Return dateGet
    End Function

    ''' <summary>
    ''' Gets a DateTime object from the controls provided in the parameters.
    ''' </summary>
    ''' <param name="ddlMonth">The month ddl.</param>
    ''' <param name="ddlDay">The day ddl.</param>
    ''' <param name="ddlYear">The year ddl.</param>
    ''' <param name="ddlHour">The hour portion of the time stamp.</param>
    ''' <param name="ddlMinute">The minute portion of the time stamp.</param>
    ''' <param name="ddlAmPm">The am/pm portion of the time stamp.</param>
    ''' <returns>A datetime object with the combined datetime values.</returns>
    ''' <remarks></remarks>
    Shared Function GetDateTime(ByVal ddlMonth As DropDownList, ByVal ddlDay As DropDownList, ByVal ddlYear As DropDownList, ByVal ddlHour As DropDownList, ByVal ddlMinute As DropDownList, Optional ByVal ddlAmPm As DropDownList = Nothing) As DateTime
                Dim dateGet As DateTime

        If Not IsNothing(ddlHour) And Not IsNothing(ddlMinute) Then
            Try
                If Not IsNothing(ddlAmPm) Then
                    dateGet = ddlMonth.SelectedValue & "/" & ddlDay.SelectedValue & "/" & ddlYear.SelectedValue & " " & ddlHour.SelectedValue & ":" & ddlMinute.SelectedValue & ":00 " & ddlAmPm.SelectedValue
                Else
                    dateGet = ddlMonth.SelectedValue & "/" & ddlDay.SelectedValue & "/" & ddlYear.SelectedValue & " " & ddlHour.SelectedValue & ":" & ddlMinute.SelectedValue & ":00"
                End If
            Catch ex1 As Exception
                Try
                    dateGet = ddlMonth.SelectedValue & "/" & ddlDay.SelectedValue & "/" & ddlYear.SelectedValue
                Catch ex2 As Exception
                    dateGet = S_NULL_DATE
                End Try
            End Try
        Else
            Try
                dateGet = ddlMonth.SelectedValue & "/" & ddlDay.SelectedValue & "/" & ddlYear.SelectedValue
            Catch ex3 As Exception
                dateGet = S_NULL_DATE
            End Try
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the date found
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return dateGet
    End Function

    ''' <summary>
    ''' Uses a regular expression to count the number of individual words present in the target string.
    ''' </summary>
    ''' <param name="sTarget">The word string.</param>
    ''' <returns>The number of words found in the string.</returns>
    ''' <remarks></remarks>
    Shared Function GetWordCount(ByVal sTarget As String) As Integer
        Return Regex.Matches(sTarget, "[\S]+").Count
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="nTweetNumber"></param>
    ''' <param name="sTwitterName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetTwitterFeed(Optional ByVal nTweetNumber As Integer = 0, Optional ByVal sTwitterName As String = "Whitworth") As DataTable
        '        Dim nCounter As Integer
        'Dim client As New WebClient
        'Dim dateTemp As DateTime
        'Dim dt As New DataTable()
        'Dim drTemp As DataRow
        'Dim sLink As String = ""

        'Try

        '    Dim jsonDat As JArray = JArray.Parse(GetTwitterJSON(sTwitterName))

        '    dt.Columns.Add("Title")
        '    dt.Columns.Add("Date")
        '    dt.Columns.Add("Link")

        '    If nTweetNumber <= 0 Or nTweetNumber > jsonDat.Count() Then
        '        nTweetNumber = jsonDat.Count()
        '    End If

        '    If jsonDat IsNot Nothing And jsonDat.Count() > 0 Then

        '        For x As Integer = 0 To nTweetNumber - 1
        '            drTemp = dt.NewRow()
        '            drTemp("Title") = jsonDat(x)("text").ToString()
        '            drTemp("Date") = jsonDat(x)("created_at").ToString()
        '            sLink = "https://twitter.com/" & sTwitterName & "/statuses/" &
        '            drTemp("Link") = sLink & jsonDat(x)("id").ToString()
        '            dt.Rows.Add(drTemp)
        '        Next
        '    End If

        '    Dim format As New System.Globalization.CultureInfo("fr-FR", True)

        '    For nCounter = 0 To dt.Rows.Count - 1
        '        With dt.Rows(nCounter)
        '            dateTemp = DateTime.ParseExact(.Item("Date"), "ddd MMM dd HH:mm:ss zz00 yyyy", New System.Globalization.CultureInfo("en-GB"))
        '            .Item("Date") = WhitworthFormat(ConvertMonthToLong(dateTemp.Month)) & " " & dateTemp.Day
        '            .Item("Title") = Regex.Replace(.Item("Title"), "http://([A-Za-z0-9./]+)", "<a href='http://$1' target='_blank'>$1</a>")
        '            .Item("Title") = Regex.Replace(.Item("Title"), "@([A-Za-z0-9./]+)", "<a href='http://twitter.com/$1' target='_blank'>@$1</a>")
        '            .Item("Title") = Regex.Replace(.Item("Title"), "#([A-Za-z0-9./]+)", "<a href='http://twitter.com/#!/search/?q=%23$1&src=hash' target='_blank'>#$1</a>")
        '        End With
        '    Next

        '    Return dt
        'Catch ex As Exception
        '    ReportError(ex, "tryan", "Error Encountered Getting Twitter Feed", 1, False)
        'End Try

        Return New DataTable()
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sTwitterName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetTwitterJSON(ByVal sTwitterName As String) As String
                Dim oauth_token, oauth_token_secret, oauth_consumer_key, oauth_consumer_secret As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'oauth application keys
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        oauth_token = ""
        oauth_token_secret = ""
        oauth_consumer_key = ""
        oauth_consumer_secret = ""

        GetTwitterAuthentication(sTwitterName, oauth_token, oauth_token_secret, oauth_consumer_key, oauth_consumer_secret)
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'oauth implementation details
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim oauth_version = "1.0"
        Dim oauth_signature_method = "HMAC-SHA1"
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'unique request details
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim oauth_nonce = Convert.ToBase64String(New ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()))
        Dim timeSpan = DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
        Dim oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString()
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'message api details
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim resource_url = "https://api.twitter.com/1.1/statuses/user_timeline.json"
        Dim screen_name = sTwitterName
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'create oauth signature
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" & "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&screen_name={6}"
        Dim baseString = String.Format(baseFormat, oauth_consumer_key, oauth_nonce, oauth_signature_method, oauth_timestamp, oauth_token, oauth_version, Uri.EscapeDataString(screen_name))

        baseString = String.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString))

        Dim compositeKey = String.Concat(Uri.EscapeDataString(oauth_consumer_secret), "&", Uri.EscapeDataString(oauth_token_secret))
        Dim oauth_signature As String

        Using hasher As New HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey))
            oauth_signature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)))
        End Using
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'create the request header
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim headerFormat = "OAuth oauth_nonce=""{0}"", oauth_signature_method=""{1}"", " & "oauth_timestamp=""{2}"", oauth_consumer_key=""{3}"", " & "oauth_token=""{4}"", oauth_signature=""{5}"", " & "oauth_version=""{6}"""
        Dim authHeader = String.Format(headerFormat, Uri.EscapeDataString(oauth_nonce), Uri.EscapeDataString(oauth_signature_method), Uri.EscapeDataString(oauth_timestamp), Uri.EscapeDataString(oauth_consumer_key), Uri.EscapeDataString(oauth_token), Uri.EscapeDataString(oauth_signature), Uri.EscapeDataString(oauth_version))
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'make the request
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ServicePointManager.Expect100Continue = False
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim postBody = "screen_name=" & Uri.EscapeDataString(screen_name)

        resource_url += "?" & postBody

        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(resource_url), HttpWebRequest)

        request.Headers.Add("Authorization", authHeader)
        request.Method = "GET"
        request.ContentType = "application/x-www-form-urlencoded"

        Dim response As WebResponse = request.GetResponse()
        Dim responseData As String = New StreamReader(response.GetResponseStream()).ReadToEnd()

        Return responseData
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sTwitterName"></param>
    ''' <param name="oauth_token"></param>
    ''' <param name="oauth_token_secret"></param>
    ''' <param name="oauth_consumer_key"></param>
    ''' <param name="oauth_consumer_secret"></param>
    ''' <remarks></remarks>
    Shared Sub GetTwitterAuthentication(ByVal sTwitterName As String, ByRef oauth_token As String, ByRef oauth_token_secret As String, ByRef oauth_consumer_key As String, ByRef oauth_consumer_secret As String)
        Select Case sTwitterName
            Case "Whitworth"
                oauth_token = "17396360-5QNapERiKwAvv8vDsASUgyJ0SoXoVqmDXYerYhaEe"
                oauth_token_secret = "QsN3OU0vT2wsbmzMOdJ7wWHVLG8xbNt6lRu85MC4"
                oauth_consumer_key = "vWrSFm75jM4o9tCwKk76vg"
                oauth_consumer_secret = "zAw7pE0AbWctDv9phsQGFFPJTkGXbEeoge0h56dTbg"
            Case "gotowhitworth"
                oauth_token = "111108886-PpI93Wj2H8fBsqIHmp5gIOlDp56FlxlfFoTCzGIn"
                oauth_token_secret = "SgDZAbBgrzEpULvJnyoQIcV2Dq08n5V1zxJB2MaNPo"
                oauth_consumer_key = "pFM8C2O6tSXR1l7m9wNH6g"
                oauth_consumer_secret = "y46wPpLkJrcfPb1IEBcgazjzncJ3PpQpQG8YYmEI"
        End Select
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sDepartment"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetSpotlights(ByVal sDepartment As String) As DataTable
        Return GetDataTable("SELECT I.ID, Headline, Date, Link, I.Department FROM Web3.News.dbo.InTheSpotlight I WHERE (I.DepSpotlight='1' AND I.Department='" & sDepartment & "') OR ID IN (Select SpotlightID FROM Web3.News.dbo.SpotlightDepartments WHERE DepSpotlight='1' AND Department='" & sDepartment & "') GROUP BY I.ID, Headline, Date, Link, I.Department ORDER BY I.Date DESC")
    End Function

    ''' <summary>
    ''' Returns a building's information in a DataTable.
    ''' </summary>
    ''' <param name="sBuildingID">Accepts two values, 1)The building ID number or 2)The building name.</param>
    ''' <returns>The building's information in a DataTable.</returns>
    ''' <remarks></remarks>
    Shared Function GetBuildingInfo(ByVal sBuildingID As String) As DataTable
                Dim nBuildingID As Integer = GetBuildingID(sBuildingID)
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the building info
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            Return GetDataTable("SELECT * FROM " & DT_BUILDINGS & " WHERE ID='" & nBuildingID & "'")
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return New DataTable()
    End Function

    ''' <summary>
    ''' Gets a building name by the building info provided. Returns S_UNAVAILABLE if no match is found.
    ''' </summary>
    ''' <param name="sBuilding">The building to get the name for.</param>
    ''' <returns>The name of the building. Returns S_UNAVAILABLE if no match is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetBuildingName(ByVal sBuilding As String) As String
        Try
            Return GetDataTable("SELECT Name FROM " & DT_BUILDINGS & " WHERE ID='" & GetBuildingID(sBuilding) & "'").Rows(0).Item("Name").ToString()
        Catch ex As Exception
            'Empty catch statement
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return building not found
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_UNAVAILABLE
    End Function

    ''' <summary>
    ''' Gets a building name by the building id. Returns S_UNAVAILABLE if no match is found.
    ''' </summary>
    ''' <param name="nBuildingID">The building id to get the name for.</param>
    ''' <returns>The name of the building. Returns S_UNAVAILABLE if no match is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetBuildingShortName(ByVal nBuildingID As Integer) As String
        Try
            Return GetDataTable("SELECT ShortName FROM " & DT_BUILDINGS & " WHERE ID='" & GetBuildingID(nBuildingID) & "'").Rows(0).Item("ShortName").ToString()
        Catch ex As Exception
            'Empty catch statement
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return building not found
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_UNAVAILABLE
    End Function

    ''' <summary>
    ''' Gets a building id by the building ID or name. Ensures the building ID is the most up-to-date version.
    ''' </summary>
    ''' <param name="sBuilding">The building to get the id for. Accepts the building ID, name, short name or code.</param>
    ''' <returns>The building id.</returns>
    ''' <remarks></remarks>
    Shared Function GetBuildingID(ByVal sBuilding As String) As Integer
                Dim nBuildingID As Integer
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Make temporary code replacement
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        sBuilding = Replace(Replace(Replace(sBuilding, "FSRV", "PHYS"), "FINE", "LIED"), "SCHH", "SC")
        sBuilding = Replace(Replace(Replace(sBuilding, "Eileen Hendrick Hall", "Hendrick Hall"), "Facilities Services", "Physical Plant"), "Schumacher Health Center", "Schumacher")
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the Building ID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            nBuildingID = CInt(GetDataTable("SELECT ID FROM " & DT_BUILDINGS & " WHERE ID='" & CleanSQL(sBuilding) & "'", Nothing, True, -1, "", True, False, False).Rows(0).Item("ID"))
        Catch ex1 As Exception
            Try
                nBuildingID = CInt(GetDataTable("SELECT ID FROM " & DT_BUILDINGS & " WHERE Name='" & CleanSQL(sBuilding) & "'", Nothing, True, -1, "", True, False, False).Rows(0).Item("ID"))
            Catch ex2 As Exception
                Try
                    nBuildingID = CInt(GetDataTable("SELECT ID FROM " & DT_BUILDINGS & " WHERE ShortName='" & CleanSQL(sBuilding) & "'", Nothing, True, -1, "", True, False, False).Rows(0).Item("ID"))
                Catch ex3 As Exception
                    Try
                        nBuildingID = CInt(GetDataTable("SELECT ID FROM " & DT_BUILDINGS & " WHERE Code='" & CleanSQL(sBuilding) & "'", Nothing, True, -1, "", True, False, False).Rows(0).Item("ID"))
                    Catch ex4 As Exception
                        Try
                            nBuildingID = CInt(GetDataTable("SELECT ID FROM " & DT_BUILDINGS & " WHERE Code='" & CleanSQL(sBuilding.Substring(0, sBuilding.Length - 1)) & "'", Nothing, True, -1, "", True, False, False).Rows(0).Item("ID"))
                        Catch ex5 As Exception
                            nBuildingID = N_NOT_SELECTED
                        End Try
                    End Try
                End Try
            End Try
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return Building ID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return GetUpToDateBuildingID(nBuildingID)
    End Function

    ''' <summary>
    ''' Gets the most up-to-date building id by the building ID. GetBuildingID() should be called outside of 
    ''' WhitTools instead of this function.
    ''' </summary>
    ''' <param name="nBuildingID">The building to get the most up-to-date ID for.</param>
    ''' <returns>The most up-to-date building ID.</returns>
    ''' <remarks></remarks>
    Public Shared Function GetUpToDateBuildingID(ByVal nBuildingID As Integer) As Integer
                Dim nRedirectTo As Integer
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the RedirectTo ID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            nRedirectTo = CInt(GetDataTable("SELECT RedirectTo FROM " & DT_BUILDINGS & " WHERE ID='" & nBuildingID & "'").Rows(0).Item("RedirectTo"))
        Catch ex1 As Exception
            nRedirectTo = N_NOT_SELECTED
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the most up-to-date id
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If nRedirectTo <> N_NOT_SELECTED Then
            Return GetUpToDateBuildingID(nRedirectTo)
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return Building ID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return nBuildingID
    End Function

    ''' <summary>
    ''' Returns the building/location id the room/sub-location is associated with. Returns N_ERROR (-255) if no match is found.
    ''' </summary>
    ''' <param name="nRoomID">The room/sub-location to get the building/location id for.</param>
    ''' <returns>The building/location id the room/sub-location is associated with. Returns N_ERROR (-255) if no match is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetBuildingIDByRoomID(ByVal nRoomID As Integer) As Integer
        Try
            Return GetUpToDateBuildingID(GetDataTable("SELECT ID FROM " & DT_BUILDINGS & " WHERE ID=(SELECT TOP 1 BuildingID FROM " & DT_BUILDINGS_ROOMS & " WHERE ID='" & nRoomID & "')").Rows(0).Item("ID"))
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return N_ERROR
    End Function

    ''' <summary>
    ''' Gets the building/location id by comparing the DataWarehouse building name. Returns N_ERROR (-255) if no match is found.
    ''' </summary>
    ''' <param name="sDwBuilding">The building/location name as it is found in the DataWarehouse.</param>
    ''' <returns>The building/location id the DataWarehouse version is associated with. Returns N_ERROR (-255) if no match is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetBuildingIDByDwBuilding(ByVal sDwBuilding As String) As Integer
        Try
            Return GetUpToDateBuildingID(GetDataTable("SELECT ID FROM " & DT_BUILDINGS & " WHERE DataWarehouseName='" & sDwBuilding & "' AND DataWarehouseName<>'' AND DataWarehouseName IS NOT NULL").Rows(0).Item("ID"))
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return N_ERROR
    End Function

    ''' <summary>
    ''' Gets the building/location name by comparing the DataWarehouse building name. Returns S_UNAVAILABLE if no match is found.
    ''' </summary>
    ''' <param name="sDwBuilding">The building/location name as it is found in the DataWarehouse.</param>
    ''' <returns>The building/location name the DataWarehouse version is associated with. Returns S_UNAVAILABLE if no match is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetBuildingByDwBuilding(ByVal sDwBuilding As String) As String
        Return GetBuildingName(GetBuildingIDByDwBuilding(sDwBuilding))
    End Function

    ''' <summary>
    ''' Returns the name of the building/location for the provided building/location id. Returns S_UNAVAILABLE if no match is found.
    ''' </summary>
    ''' <param name="nBuildingID">The building/location to get the name of.</param>
    ''' <returns>The name of the building/location. Returns S_UNAVAILABLE if no match is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetBuildingByID(ByVal nBuildingID As Integer) As String
        Return GetBuildingName(nBuildingID)
    End Function

    ''' <summary>
    ''' Returns the building/location information for all buildings associated with the provided department.
    ''' </summary>
    ''' <param name="sDepIDCode">Accepts three values, 1)The department id number or 2)The department code or 3)The department name as it appears in the DataTable or DataWarehouse.</param>
    ''' <returns>A DataTable containing the building/location information for all buildings associated with the provided department.</returns>
    ''' <remarks></remarks>
    Shared Function GetBuildingIDsFromDepartment(ByVal sDepIDCode As String) As DataTable
        Return GetDataTable("SELECT B.* FROM " & DT_ARA_DEPARTMENTS_LOCATIONS & " L LEFT OUTER JOIN " & DT_BUILDINGS & " B ON L.BuildingID=B.ID WHERE L.DepID='" & GetDepID(sDepIDCode) & "' ORDER BY B.Name")
    End Function

    ''' <summary>
    ''' Gets all buildings in a DataTable. Active and most up-to-date buildings only. Building names are under the column "Name".
    ''' </summary>
    ''' <param name="nFilter">The way to filter the building list. 0 = Active Buildings, 1 = Full Search (everything that is not redirected, regardless of active status)</param>
    ''' <returns>A DataTable with all buildings' information.</returns>
    ''' <remarks></remarks>
    Shared Function GetBuildings(Optional ByVal nFilter As Integer = 0) As DataTable
        If nFilter = 1 Then
            Return GetDataTable("SELECT * FROM " & DT_BUILDINGS & " WHERE FullSearch='" & N_YES & "' AND RedirectTo='-1' ORDER BY Name")
        End If
        Return GetDataTable("SELECT * FROM " & DT_BUILDINGS & " WHERE Active='" & N_YES & "' AND RedirectTo='-1' ORDER BY Name")
    End Function

    ''' <summary>
    ''' Returns the name of the room/sub-location for the provided room id. Returns N_ERROR (-255) if no match is found.
    ''' </summary>
    ''' <param name="sRoomID">The room/sub-location to get the name of.</param>
    ''' <returns>The name of the room/sub-location. Returns N_ERROR (-255) if no match is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetRoomByID(ByVal sRoomID As String) As String
        Try
            Return GetDataTable("SELECT Name FROM " & DT_BUILDINGS_ROOMS & " WHERE ID='" & GetRoomID(sRoomID) & "'").Rows(0).Item("Name")
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return N_ERROR
    End Function

    ''' <summary>
    ''' Returns the name of the room/sub-location for the provided room id. Returns N_ERROR (-255) if no match is found.
    ''' </summary>
    ''' <param name="sRoomID">The room/sub-location to get the name of.</param>
    ''' <returns>The name of the room/sub-location. Returns N_ERROR (-255) if no match is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetRoomID(ByVal sRoomID As String, Optional ByVal sBuildingID As String = "") As Integer
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the room ID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            If IsNumeric(sRoomID) Then
                Return GetDataTable("SELECT ID FROM " & DT_BUILDINGS_ROOMS & " WHERE ID='" & CleanSQL(sRoomID) & "'").Rows(0).Item("ID")
            End If
            If sBuildingID <> "" Then
                Return GetDataTable("SELECT ID FROM " & DT_BUILDINGS_ROOMS & " WHERE (Name='" & CleanSQL(sRoomID) & "' OR ShortName='" & CleanSQL(sRoomID) & "') AND BuildingID='" & GetBuildingID(sBuildingID) & "'").Rows(0).Item("ID")
            End If
        Catch ex As Exception
            'Empty catch statement
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Failed to find a room
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return N_ERROR
    End Function

    ''' <summary>
    ''' Returns a string of the values in an SQLCommand.
    ''' </summary>
    ''' <param name="cmd">The SQLCommand object to see the parameters of.</param>
    ''' <param name="bReportErrors">Report any errors that occur?</param>
    ''' <returns>A string to display the SQLCommand values.</returns>
    ''' <remarks></remarks>
    Shared Function GetCmdValues(ByVal cmd As SqlCommand, Optional ByVal bReportErrors As Boolean = True) As String
                Dim sValues As String = "<table>"
        Dim nCounter As Integer
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get SqlCommand values
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Write SqlCommand values
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sValues &= "<tr><td colspan='3' align='center'><strong>SqlCommand Values</strong></td></tr>"
            sValues &= "<tr><td valign='top'><strong>CommandText:</strong></td><td width='25' /><td>" & cmd.CommandText & "</td></tr>"
            sValues &= "<tr><td valign='top'><strong>CommandTimeout:</strong></td><td width='25' /><td>" & cmd.CommandTimeout & "</td></tr>"
            sValues &= "<tr><td valign='top'><strong>CommandType:</strong></td><td width='25' /><td>" & cmd.CommandType.ToString() & "</td></tr>"
            sValues &= "<tr><td valign='top'><strong>Database:</strong></td><td width='25' /><td>" & cmd.Connection.Database & "</td></tr>"
            If cmd.Parameters.Count > 0 Then
                sValues &= "<tr><td><br /></td></tr>"
                sValues &= "<tr><td colspan='5' align='center' valign='top'><strong>Parameters</strong></td></tr>"
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Write SqlCommand parameters
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                For nCounter = 0 To cmd.Parameters.Count - 1
                    With cmd.Parameters(nCounter)
                        Try
                            sValues &= "<tr><td valign='top'><strong>" & .ParameterName & ":</strong></td><td width='25' /><td>" & .Value.ToString() & "</td><td width='25' /><td>" & Replace(.DbType.ToString(), "System.", "") & "</td></tr>"
                        Catch ex As Exception
                            'Empty catch statement
                        End Try
                    End With
                Next
            End If
        Catch ex As Exception
            If bReportErrors Then
                ReportError(ex, "", "<p>Failed to retrieve information from an SqlCommand object.</p>", N_ERROR_IMPORTANCE_LOW)
            End If
            Return S_ERROR
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the readable parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sValues & "</table>"
    End Function

    ''' <summary>
    ''' Returns a string of the values in an SQLCommand.
    ''' </summary>
    ''' <param name="cmd">The SQLCommand object to see the parameters of.</param>
    ''' <param name="bReportErrors">Report any errors that occur?</param>
    ''' <returns>A string to display the SQLCommand values.</returns>
    ''' <remarks></remarks>
    Shared Function GetSqlCmdValues(ByVal cmd As SqlCommand, Optional ByVal bReportErrors As Boolean = True) As String
        Return GetCmdValues(cmd)
    End Function

    ''' <summary>
    ''' Returns a string of the values in an sql data adapter.
    ''' </summary>
    ''' <param name="sqlDataAdapter">The SqlDataAdapter object to get values for.</param>
    ''' <param name="sDelimeter">The delimiter for the list of values.</param>
    ''' <param name="bReportErrors">Report any errors that occur?</param>
    ''' <returns>A list of SqlDataAdapter values.</returns>
    ''' <remarks></remarks>
    Shared Function GetSqlDataAdapterValues(ByVal sqlDataAdapter As SqlDataAdapter, Optional ByVal sDelimeter As String = "<br />", Optional ByVal bReportErrors As Boolean = True) As String
                Dim sValues As String = "SqlDataAdapter Values:<br />"
        Dim nCounter, nCounter2 As Integer
        Dim dtValues As DataTable = New DataTable()
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Cycle through the SqlDataAdapter values
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            sqlDataAdapter.Fill(dtValues)
            For nCounter2 = 0 To dtValues.Rows.Count - 1
                For nCounter = 0 To dtValues.Rows(0).ItemArray.Length - 1
                    With dtValues.Columns(nCounter)
                        sValues &= "Row " & nCounter2.ToString() & ": " & .ToString() & "=" & dtValues.Rows(nCounter2).Item(nCounter).ToString() & " (" & Replace(.DataType.ToString(), "System.", "") & ")" & sDelimeter
                    End With
                Next
            Next
        Catch ex As Exception
            If bReportErrors Then
                ReportError(ex, "", "<p>Failed to get SqlDataAdapter in WhitTools.Getter.GetSqlDataAdapterValues().</p>", N_ERROR_IMPORTANCE_LOW)
            End If
            Return S_ERROR
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the SqlDataAdapter parameters
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sValues & "<br />"
    End Function

    ''' <summary>
    ''' Retrieves the current html page the function is called in.
    ''' </summary>
    ''' <returns>The current html page.</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentPage() As Page
        Return CType(System.Web.HttpContext.Current.Handler, Page)
    End Function


    
    ''' <summary>
    ''' Retrieves the formatted username of the currently logged in user. Only works when ssl is required. Returns S_UNAVAILABLE if the username is "Unavailable".
    ''' </summary>
    ''' <param name="sUsername">A username to substitute. Only works if a web team member is logged in.</param>
    ''' <returns>The formatted username of the currently logged in user.</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentUsername(Optional ByVal sUsername As String = "") As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Format the current username
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            If sUsername <> "" Then
                If IsWebTeamMember() Then
                    If ValidateUsernameExists(sUsername) Then
                        Return FormatUsername(sUsername)
                    End If
                End If
            End If

            sUsername = FormatUsername(GetCurrentPage().User.Identity.Name)
        Catch ex As Exception
            sUsername = S_UNAVAILABLE
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check for an existing username
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sUsername = "" Then
            sUsername = S_UNAVAILABLE
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Save the most recently accessed username. Allows web team members to track who was logged-in more often
        'for error tracking, even if the page the error was on does not require login, but a previous one did.
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            If sUsername <> S_UNAVAILABLE Or GetSessionVariable(SESSION_MOST_RECENT_USERNAME) = S_EMPTY_VALUE Or GetCookieValue(COOKIE_MOST_RECENT_USERNAME) = S_EMPTY_VALUE Then
                SetSessionVariable(SESSION_MOST_RECENT_USERNAME, sUsername)
                UpdateCookie(COOKIE_MOST_RECENT_USERNAME, sUsername, Now.AddDays(1))
            End If
        Catch ex As Exception

        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the username
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sUsername
    End Function

    ''' <summary>
    ''' Retrieves the formatted username of the currently logged in user. Only works when ssl is required. Returns S_UNAVAILABLE if the username is "Unavailable".
    ''' </summary>
    ''' <param name="sUsername">A username to substitute. Only works if a Whitworth user is logged in.</param>
    ''' <returns>The formatted username of the currently logged in user. Returns S_UNAVAILABLE if ssl is not available.</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentUsernameFormatted(Optional ByRef sUsername As String = "") As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check if a username exists, alert web team members if it does not exist
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sUsername <> "" Then
            If Not ValidateUsernameExists(sUsername) Then
                WAlert("The username " & sUsername & " does not exist, but is attempting to be used.")
            End If
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check if a username was provided
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sUsername = "" Then
            sUsername = GetCurrentUsername()
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the username
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return FormatUsername(sUsername)
    End Function

    ''' <summary>
    ''' Retrieves the PLID of the currently logged in user. Only works when ssl is required. Returns S_STUDENT_ID_BLANK if the PLID cannot be found.
    ''' </summary>
    ''' <returns>The PLID of the currently logged in user. Returns S_STUDENT_ID_BLANK (0000000) if not found.</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentPLID() As String
                Dim sPLID As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the PLID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            sPLID = GetStudentIDByUsername(GetCurrentUsername())
            If sPLID = S_UNAVAILABLE Then
                sPLID = S_STUDENT_ID_BLANK
            End If
        Catch ex As Exception
            sPLID = S_STUDENT_ID_BLANK
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the PLID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sPLID
    End Function

    ''' <summary>
    ''' Retrieves the Faculty/Staff status of the currently logged in user. Only works when ssl is required. Returns False if the user cannot be found.
    ''' </summary>
    ''' <returns>Is the current user a faculty/staff member?</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentIsFacultyStaff() As Boolean
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the PLID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            Return IIf(GetStudentInfoByUsername(GetCurrentUsername(), "PLStaff") = 1, True, False)
        Catch ex As Exception
            'Empty catch statement
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the faculty/staff status
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return False
    End Function

    ''' <summary>
    ''' Retrieves the student status of the currently logged in user. Only works when ssl is required. Returns False if the user cannot be found.
    ''' </summary>
    ''' <returns>Is the current user a student?</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentIsStudent() As Boolean
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the PLID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            Return IIf(GetStudentInfoByUsername(GetCurrentUsername(), "PLStudent") = 1, True, False)
        Catch ex As Exception
            'Empty catch statement
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the student status
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return False
    End Function

    ''' <summary>
    ''' Retrieves the email address of the currently logged in user. Only works when ssl is required. Returns S_UNAVAILABLE if the email address cannot be found.
    ''' </summary>
    ''' <returns>The email address of the currently logged in user.</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentUserEmail() As String
        Try
            Return GetStudentInfoByUsername(GetCurrentUsername(), "PLEmail")
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return S_UNAVAILABLE
    End Function

    ''' <summary>
    ''' Retrieves the 10 digit phone number of the currently logged in user. Only works when ssl is required. Returns S_NONE if the phone number cannot be found.
    ''' </summary>
    ''' <returns>The 10 digit phone number of the currently logged in user.</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentUserPhone() As String
        Try
            Return FormatPhone(GetStudentInfoByUsername(GetCurrentUsername(), "PLPhone"))
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return S_NONE
    End Function

    ''' <summary>
    ''' Retrieves the 4 digit phone number extension of the currently logged in user. Only works when ssl is required. Returns S_NONE if the phone number cannot be found.
    ''' </summary>
    ''' <param name="bIncludeX">Include the x in front of the extension? (Ex: x4647)</param>
    ''' <returns>The 4 digit phone number extension of the currently logged in user.</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentUserPhoneExt(Optional ByVal bIncludeX As Boolean = True) As String
        Try
            Return IIf(bIncludeX, "x", "") & FormatPhone(GetStudentInfoByUsername(GetCurrentUsername(), "PLPhone")).Substring(8, 4)
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return S_NONE
    End Function

    ''' <summary>
    ''' Retrieves the PeopleListing information of the currently logged in user. Only works when ssl is required. Returns a blank DataTable if the user cannot be found.
    ''' </summary>
    ''' <returns>The PeopleListing information of the currently logged in user.</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentStudentInfo() As DataTable
        Try
            Return GetStudentInfoByUsername(GetCurrentUsername())
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return New DataTable()
    End Function

    ''' <summary>
    ''' Retrieves the PeopleListing column information of the currently logged in user. Only works when ssl is required. Returns a blank DataTable if the user cannot be found.
    ''' </summary>
    ''' <returns>The PeopleListing column information of the currently logged in user.</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentStudentInfo(ByVal sColumn As String) As String
        Try
            Return GetCurrentStudentInfo().Rows(0).Item(sColumn)
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return S_EMPTY_VALUE
    End Function

    ''' <summary>
    ''' Retrieves the name of the currently logged in user. Only works when ssl is required. Returns an empty value if the user cannot be found.
    ''' </summary>
    ''' <returns>The name of the currently logged in user.</returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentStudentName() As String
        Return GetCurrentStudentInfo("PLName")
    End Function

    ''' <summary>
    ''' Returns a link to the e-commerce invoice viewing page with the invoice number appended to the link.
    ''' </summary>
    ''' <param name="nInvoice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetViewInvoiceLink(ByVal nInvoice As Integer)
        Return S_VIEW_INVOICE_LINK & "?InvoiceNumber=" & EncryptTextInternal(nInvoice)
    End Function

    ''' <summary>
    ''' Returns the value of the provided session variable. Returns S_EMPTY_VALUE if no value is found.
    ''' </summary>
    ''' <param name="sSession">The name of the session variable to get a value for.</param>
    ''' <returns>The value of the provided session variable. Returns S_EMPTY_VALUE if no value is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetSessionVariable(ByVal sSession As String) As String
        Try
            If IsDBNull(System.Web.HttpContext.Current.Session(sSession)) Then
                Return S_EMPTY_VALUE
            ElseIf System.Web.HttpContext.Current.Session(sSession) Is Nothing Then
                Return S_EMPTY_VALUE
            Else
                Return CStr(System.Web.HttpContext.Current.Session(sSession))
            End If
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return S_EMPTY_VALUE
    End Function

    ''' <summary>
    ''' Returns the integer value of the provided session variable. Returns 0 if no value is found.
    ''' </summary>
    ''' <param name="sSession">The name of the session variable to get an integer value for.</param>
    ''' <returns>The integer value of the provided session variable. Returns 0 if no value is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetSessionVariableInt(ByVal sSession As String) As Integer
        Try
            Return IIf(IsDBNull(System.Web.HttpContext.Current.Session(sSession)), 0, IIf(System.Web.HttpContext.Current.Session(sSession) Is Nothing, 0, CInt(System.Web.HttpContext.Current.Session(sSession))))
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return 0
    End Function

    ''' <summary>
    ''' Returns the date value of the provided session variable. Returns CDate(S_NULL_DATE) if no value is found.
    ''' </summary>
    ''' <param name="sSession">The name of the session variable to get a date value for.</param>
    ''' <returns>The date value of the provided session variable. Returns CDate(S_NULL_DATE) if no value is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetSessionVariableDate(ByVal sSession As String) As DateTime
        Try
            Return IIf(IsDBNull(System.Web.HttpContext.Current.Session(sSession)), CDate(S_NULL_DATE), IIf(System.Web.HttpContext.Current.Session(sSession) Is Nothing, CDate(S_NULL_DATE), CDate(System.Web.HttpContext.Current.Session(sSession))))
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return CDate(S_NULL_DATE)
    End Function

    ''' <summary>
    ''' Returns the value of the provided QueryString variable. Returns S_EMPTY_VALUE if no value is found.
    ''' </summary>
    ''' <param name="sQueryString">The name of the QueryString variable to get a value for.</param>
    ''' <param name="bCleanSQL">Use the CleanSQL() function on the query string value?</param>
    ''' <param name="bIncludeSpecialCharacters">Convert special characters to their special form? If false, the strict form will be used.</param>
    ''' <returns>The value of the provided QueryString variable.</returns>
    ''' <remarks></remarks>
    Shared Function GetQueryString(Optional ByVal sQueryString As String = "ID", Optional ByVal bCleanSQL As Boolean = True, Optional ByVal bIncludeSpecialCharacters As Boolean = False) As String
        Try
            If IsDBNull(GetCurrentPage().Request.QueryString(sQueryString)) Then
                Return S_EMPTY_VALUE
            ElseIf GetCurrentPage().Request.QueryString(sQueryString) Is Nothing Then
                Return S_EMPTY_VALUE
            Else
                If bCleanSQL Then
                    If bIncludeSpecialCharacters Then
                        Return CleanSQL(ConvertToSpecialChars(CStr(Replace(GetCurrentPage().Request.QueryString(sQueryString), " ", "+")), False))
                    Else
                        Return CleanSQL(ConvertSpecialChars(CStr(Replace(GetCurrentPage().Request.QueryString(sQueryString), " ", "+")), False))
                    End If
                Else
                    If bIncludeSpecialCharacters Then
                        Return ConvertToSpecialChars(CStr(Replace(GetCurrentPage().Request.QueryString(sQueryString), " ", "+")), False)
                    Else
                        Return ConvertSpecialChars(CStr(Replace(GetCurrentPage().Request.QueryString(sQueryString), " ", "+")), False)
                    End If
                End If
            End If
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return S_EMPTY_VALUE
    End Function

    ''' <summary>
    ''' Creates an anchor tag for linking the user to the quick PLID search page.
    ''' </summary>
    ''' <param name="sText">The html text displayed in the hyperlink.</param>
    ''' <param name="bLinkOnlyForWebTeam">Only web team members get to access the link?</param>
    ''' <param name="sNonWebTeamText">If bLinkOnlyForWebTeam is true, then this is the text non-web team users will see.</param>
    ''' <returns>The anchor tag linking to the quick PLID search page.</returns>
    ''' <remarks></remarks>
    Shared Function GetQuickPLIDSearchLink(Optional ByVal sText As String = "<small>(Quick PLID Search)</small>", Optional ByVal bLinkOnlyForWebTeam As Boolean = False, Optional ByVal sNonWebTeamText As String = "")
        If bLinkOnlyForWebTeam Then
            If IsWebTeamMember() Then
                Return "<a href='" & HTML_PATH_WEB2 & "RulesAssignments/search_plid.aspx' target='_blank'>" & sText & "</a>"
            Else
                Return sNonWebTeamText
            End If
        End If
        Return "<a href='" & HTML_PATH_WEB2 & "RulesAssignments/search_plid.aspx' target='_blank'>" & sText & "</a>"
    End Function

    ''' <summary>
    ''' Gets the number of selected items in a control.
    ''' </summary>
    ''' <param name="cTarget">The control to check the number of selected items of.</param>
    ''' <returns>The number of selected items in the control.</returns>
    ''' <remarks></remarks>
    Shared Function GetListControlSelectedItems(ByRef cTarget As Control) As Integer
                Dim nCounter, nSelectedCount As Integer
                nSelectedCount = 0
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Cycle through list control items
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        For nCounter = 0 To CType(cTarget, ListControl).Items.Count - 1
            With CType(cTarget, ListControl).Items(nCounter)
                If .Selected Then
                    nSelectedCount += 1
                End If
            End With
        Next
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the selected count
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return nSelectedCount
    End Function

    ''' <summary>
    ''' Gets the value that bypasses the redirect to the maintenance page when a page is set as "Under Maintenance".
    ''' </summary>
    ''' <param name="bWebTeamOnly">Should only web team members be able to see this? Also displays if the maintenance QueryString is set.</param>
    ''' <returns>The Request.QueryString("Maintenance") value that should be set to bypass the maintenance page redirect.</returns>
    ''' <remarks></remarks>
    Shared Function GetMaintenanceSetValue(Optional ByVal bWebTeamOnly As Boolean = False) As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check if the current user is able to see the page
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Not bWebTeamOnly Or (IsWebTeamMember() And bWebTeamOnly) Then
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Check if the maintenance value has already been retrieved
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If GetSessionVariable(S_MAINTENANCE_SET) = "" Then
                Try
                    SetSessionVariable(S_MAINTENANCE_SET, GetVariableValue(S_MAINTENANCE_SET))
                    If GetVariableValue(S_MAINTENANCE_SET) = S_NOT_FOUND Then
                        SetSessionVariable(S_MAINTENANCE_SET, EncryptTextExternal(S_TRUE))
                    End If
                Catch ex As Exception
                    SetSessionVariable(S_MAINTENANCE_SET, EncryptTextExternal(S_TRUE))
                End Try
            End If
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Return the maintenance value
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Return GetSessionVariable(S_MAINTENANCE_SET)
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return a value that is not easily duplicated
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return EncryptTextExternal(FormatStringStripNonNumbers(FormatDateLeadingZeroes(Now, 3)))
    End Function

    ''' <summary>
    ''' Gets the current page url.
    ''' </summary>
    ''' <param name="bShortVersion">The page name of the url only. Still includes QueryString variables.</param>
    ''' <param name="bIncludeQueryStrings">Include query strings in the url?</param>
    ''' <returns>A string with the current page url.</returns>
    ''' <remarks></remarks>
    Shared Function GetPageURL(Optional ByVal bShortVersion As Boolean = False, Optional ByVal bIncludeQueryStrings As Boolean = True, Optional ByVal bIncludeHTTP As Boolean = False) As String
                Dim nCounter As Integer
        Dim sURL As String = ""
        Dim page As Page = GetCurrentPage()
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the main url
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Not IsNothing(page) Then
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the original URL
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sURL = Replace(Replace(Replace(Replace(page.Request.PhysicalPath.ToLower(), FILE_PATH_WEB1, "www.whitworth.edu/"), FILE_PATH_WEB2, "web2.whitworth.edu/"), "web2/", "web2.whitworth.edu/"), "\", "/")
            If bShortVersion Then
                sURL = Replace(page.Request.PhysicalPath.ToLower(), "\", "/")
                sURL = sURL.Substring(sURL.LastIndexOf("/") + 1, sURL.Length - (sURL.LastIndexOf("/") + 1))
            End If
            If bIncludeHTTP Then
                If HttpsIsOn() Then
                    sURL = "https://" & sURL
                Else
                    sURL = "http://" & sURL
                End If
            End If
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Check for QueryStrings
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If page.Request.QueryString.Keys.Count > 0 And bIncludeQueryStrings Then
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Find QueryString variables
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                nCounter = 0
                For Each sQueryString In page.Request.QueryString.Keys
                    If nCounter = 0 Then
                        sURL &= "?"
                    Else
                        sURL &= "&"
                    End If
                    sURL &= sQueryString & "=" & page.Request.QueryString(sQueryString)
                    nCounter = nCounter + 1
                Next
            End If
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the current page url
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sURL
    End Function

    ''' <summary>
    ''' Gets the physical path to the current page. Defaults to not include the name of the page itself, just 
    ''' the path to the current folder. Includes the "\" at the end of the path.
    ''' </summary>
    ''' <param name="bIncludeCurrentPageInPath">Include the name of the current page in the path?</param>
    ''' <returns>The physical path to the current page.</returns>
    ''' <remarks></remarks>
    Shared Function GetPagePhysicalPath(Optional ByVal bIncludeCurrentPageInPath As Boolean = False,Optional ByVal networkPath As Boolean = false) As String
                Dim sPath As String = S_ERROR
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the main url
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Not IsNothing(GetCurrentPage()) Then
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the physical path to the current page
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sPath = GetCurrentPage().Request.PhysicalPath.ToLower()

            if networkpath
                If IsMainWebsite()
                    spath = Replace(sPath,"c:\inetpub\","\\web1\")
                ElseIf IsIntranetWebsite()
                    spath = Replace(sPath,"f:\inetpub\","\\web2\")
                End If
            End If
            
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Remove the current page from the path
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If Not bIncludeCurrentPageInPath Then
                sPath = sPath.Substring(0, sPath.LastIndexOf("\") + 1)
            End If
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the physical path to the current page
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sPath
    End Function

    Private Shared Function IsIntranetWebsite() As Boolean

        Return System.environment.MachineName = "WEB2"
    End Function

    Private Shared Function IsMainWebsite() As Boolean

        Return System.environment.MachineName = "WEB1"
    End Function

    ''' <summary>
    ''' Used by the UpdateRepeaterItems method, this method parses a control object that is passed to it
    ''' and, after determining what its data type is, returns the name of the control, sans any prefixes
    ''' attached to it (e.g. "ddl" for dropdownlist).
    ''' </summary>
    ''' <param name="ctlCurrent">The control which you would like to get an ID from.</param>
    ''' <returns></returns>
    Shared Function GetControlName(ByVal ctlCurrent As Control)
        If IsValidator(ctlCurrent) Then
            Return S_EMPTY_VALUE
        ElseIf TypeOf ctlCurrent Is RadioButton Then
            Return Replace(ctlCurrent.ID, "rad", S_EMPTY_VALUE)
        ElseIf TypeOf ctlCurrent Is CheckBox Then
            Return Replace(ctlCurrent.ID, "chk", S_EMPTY_VALUE)
        ElseIf TypeOf ctlCurrent Is CheckBoxList Then
            Return Replace(ctlCurrent.ID, "cbl", S_EMPTY_VALUE)
        ElseIf TypeOf ctlCurrent Is ListBox Then
            Return Replace(ctlCurrent.ID, "lsb", S_EMPTY_VALUE)
        ElseIf TypeOf ctlCurrent Is RadioButtonList Then
            Return Replace(ctlCurrent.ID, "rbl", S_EMPTY_VALUE)
        ElseIf TypeOf ctlCurrent Is DropDownList Then
            Return Replace(ctlCurrent.ID, "ddl", S_EMPTY_VALUE)
        ElseIf TypeOf ctlCurrent Is TextBox Then
            Return Replace(ctlCurrent.ID, "txt", S_EMPTY_VALUE)
        ElseIf TypeOf ctlCurrent Is Label Then
            Return Replace(ctlCurrent.ID, "lbl", S_EMPTY_VALUE)
        ElseIf TypeOf ctlCurrent Is Repeater Then
            Return Replace(ctlCurrent.ID, "rpt", S_EMPTY_VALUE) & "dt"
        ElseIf TypeOf ctlCurrent Is UserControl Then
            Return Replace(ctlCurrent.ID, "uc", S_EMPTY_VALUE) & "dt"
        Else
            Return S_EMPTY_VALUE
        End If
    End Function

    ''' <summary>
    ''' This method will retrieve the selected value from a control that is passed to it.
    ''' The value which is selected is based on the type of control that is passed.
    ''' </summary>
    ''' <param name="ctlCurrent">The control for which you would like to retrieve the selected value.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetControlValue(ByVal ctlCurrent As Control)
        If TypeOf ctlCurrent Is RadioButton Then
            Return CType(ctlCurrent, RadioButton).Checked
        ElseIf TypeOf ctlCurrent Is CheckBox Then
            Return CType(ctlCurrent, CheckBox).Checked
        ElseIf TypeOf ctlCurrent Is CheckBoxList Then
            return getlistofselectedvalues(ctlcurrent)
        ElseIf TypeOf ctlCurrent Is RadioButtonList Then
            If CType(ctlCurrent, RadioButtonList).SelectedIndex > -1 Then
                Return CType(ctlCurrent, RadioButtonList).SelectedValue
            End If
        ElseIf TypeOf ctlCurrent Is DropDownList Then
            If CType(ctlCurrent, DropDownList).SelectedIndex > -1 Then
                Return CType(ctlCurrent, DropDownList).SelectedValue
            End If
        ElseIf TypeOf ctlCurrent Is TextBox Then
            Return CType(ctlCurrent, TextBox).Text
        ElseIf TypeOf ctlCurrent Is Label Then
            Return CType(ctlCurrent, Label).Text
        elseif typeof ctlcurrent is HtmlInputFile
            return ctype(ctlcurrent,HtmlInputFile).PostedFile.FileName
        End If

        Return S_EMPTY_VALUE
    End Function

    ''' <summary>
    ''' Gets a unique set of years found in a datetime column from the provided DataTable. Returns the columns as "Year".
    ''' </summary>
    ''' <param name="sDataTable">The DataTable to get distinct years from.</param>
    ''' <param name="sColumn">The column containing the datetime object to query.</param>
    ''' <param name="cnx">The SQLConnection object to use if the full path to the DataTable is not provided.</param>
    ''' <param name="sOrderByDirection">The direction to order the years in. DESC or ASC are the options. Defaults to DESC.</param>
    ''' <returns>A DataTable containing the distinct years found in the column.</returns>
    ''' <remarks></remarks>
    Shared Function GetDistinctYearsFromDataTable(ByVal sDataTable As String, Optional ByVal sColumn As String = "DateSubmitted", Optional ByVal cnx As SqlConnection = Nothing, Optional ByVal sOrderByDirection As String = "DESC")
        Return GetDataTable("SELECT DISTINCT(YEAR(" & sColumn & ")) AS Year FROM " & sDataTable & " ORDER BY Year " & sOrderByDirection, cnx)
    End Function

    ''' <summary>
    ''' Gets the .SelectedIndex value to assign to the ddlShow control on maintenance pages.
    ''' </summary>
    ''' <param name="sCookie">The cookie to check for a value to assign. Overrides sQueryString if provided.</param>
    ''' <param name="sQueryString">The QueryString value to check. Defaults to "Show".</param>
    ''' <returns>The index to assign to the ddlShow control on the page.</returns>
    ''' <remarks></remarks>
    Shared Function GetShowIndex(Optional ByVal sCookie As String = "", Optional ByVal sQueryString As String = S_SHOW) As Integer
        Try
            If sCookie <> "" Then
                Try
                    Return CInt(GetCookieValue(sCookie))
                Catch ex As Exception
                    'Empty catch statement
                End Try
            End If
            Return CInt(GetQueryString(sQueryString))
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return 0
    End Function

    ''' <summary>
    ''' Gets the .SelectedIndex value to assign to the ddlSortBy control on maintenance pages.
    ''' </summary>
    ''' <param name="sCookie">The cookie to check for a value to assign. Overrides sQueryString if provided.</param>
    ''' <param name="sQueryString">The QueryString value to check. Defaults to "Sort".</param>
    ''' <returns>The index to assign to the ddlSortBy control on the page.</returns>
    ''' <remarks></remarks>
    Shared Function GetSortIndex(Optional ByVal sCookie As String = "", Optional ByVal sQueryString As String = S_SORT) As Integer
        Try
            If sCookie <> "" Then
                Try
                    Return CInt(GetCookieValue(sCookie))
                Catch ex As Exception
                    'Empty catch statement
                End Try
            End If
            Return CInt(GetQueryString(sQueryString))
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return 0
    End Function

    ''' <summary>
    ''' Gets the integer value of a QueryString. Returns N_ERROR (-255) if the QueryString is not an integer value.
    ''' </summary>
    ''' <param name="sQueryString">The QueryString value to check. Defaults to "ID".</param>
    ''' <returns>The integer value of the QueryString. Returns N_ERROR (-255) if the QueryString is not an integer value.</returns>
    ''' <remarks></remarks>
    Shared Function GetQueryStringInt(Optional ByVal sQueryString As String = "ID") As Integer
        Try
            Return CInt(FormatStringStripNonNumbers(GetQueryString(sQueryString)))
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return N_ERROR
    End Function

    ''' <summary>
    ''' Gets the DateTime value of a QueryString. Returns S_NULL_DATE (1/1/1900) if the QueryString is not a DateTime value.
    ''' </summary>
    ''' <param name="sQueryString">The QueryString value to check.</param>
    ''' <returns>Tthe DateTime value of a QueryString. Returns S_NULL_DATE (1/1/1900) if the QueryString is not a DateTime value.</returns>
    ''' <remarks></remarks>
    Shared Function GetQueryStringDate(ByVal sQueryString As String) As DateTime
        Try
            Return CDate(Trim(GetQueryString(sQueryString)))
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return CDate(S_NULL_DATE)
    End Function

    ''' <summary>
    ''' Gets the current process stack in a DataTable. Table columns are FileName, LineNumber, ColumnNumber, Frame.
    ''' </summary>
    ''' <param name="ex">The optional exception to pull the stack from.</param>
    ''' <returns>A DataTable filled with the current call stack.</returns>
    ''' <remarks></remarks>
    Shared Function GetStack(Optional ByVal ex As Exception = Nothing) As DataTable
                Dim stCurrentStack As StackTrace
        Dim nCounter As Integer
        Dim drTemp As DataRow
        Dim dtStack As New DataTable()
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Setup the stack DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        dtStack.Columns.Add("FileName", GetType(String))
        dtStack.Columns.Add("LineNumber", GetType(Integer))
        dtStack.Columns.Add("ColumnNumber", GetType(Integer))
        dtStack.Columns.Add("Frame", GetType(String))
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the stack trace
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            If Not IsNothing(ex) Then
                stCurrentStack = New StackTrace(ex, True)
            Else
                stCurrentStack = New StackTrace(True)
            End If
        Catch ex2 As Exception
            Return dtStack
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Cycle through current stack frames
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        For nCounter = 0 To stCurrentStack.FrameCount - 1
            With stCurrentStack.GetFrame(nCounter)
                drTemp = dtStack.NewRow()
                drTemp.Item("FileName") = .GetFileName
                drTemp.Item("LineNumber") = .GetFileLineNumber
                drTemp.Item("ColumnNumber") = .GetFileColumnNumber
                drTemp.Item("Frame") = .ToString()
                dtStack.Rows.Add(drTemp)
            End With
        Next
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the stack DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return dtStack
    End Function

    ''' <summary>
    ''' Returns a DateTime value no matter what kind of value is passed in. If a DateTime object can be
    ''' created from the provided value then that DateTime is returned, otherwise a null DateTime value is
    ''' returned (1/1/1900 12:00:00 AM).
    ''' </summary>
    ''' <param name="sDate">The string to try to convert to a DateTime object.</param>
    ''' <returns>A DateTime value from the provided date parameter. Returns 1/1/1900 12:00:00 AM upon a failure.</returns>
    ''' <remarks></remarks>
    Shared Function GetGuaranteedDate(ByVal sDate As String) As DateTime
        Try
            Return CDate(sDate)
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return CDate(S_NULL_DATE)
    End Function

    ''' <summary>
    ''' Returns a unique number based off of the current time.
    ''' </summary>
    ''' <param name="bIncludeMilliseconds">Include milliseconds as a unique identifier.</param>
    ''' <returns>A unique number value based off of the current DateTime.</returns>
    ''' <remarks></remarks>
    Shared Function GetUniqueNumber(Optional ByVal bIncludeMilliseconds As Boolean = True) As String
        If bIncludeMilliseconds Then
            Return FormatNumberLeadingZeroes(Now.Year, 4) & FormatNumberLeadingZeroes(Now.Month, 2) & FormatNumberLeadingZeroes(Now.Day, 2) & FormatNumberLeadingZeroes(Now.Hour, 2) & FormatNumberLeadingZeroes(Now.Minute, 2) & FormatNumberLeadingZeroes(Now.Second, 2) & FormatNumberLeadingZeroes(Now.Millisecond, 3)
        End If
        Return FormatNumberLeadingZeroes(Now.Year, 4) & FormatNumberLeadingZeroes(Now.Month, 2) & FormatNumberLeadingZeroes(Now.Day, 2) & FormatNumberLeadingZeroes(Now.Hour, 2) & FormatNumberLeadingZeroes(Now.Minute, 2) & FormatNumberLeadingZeroes(Now.Second, 2)
    End Function

    ''' <summary>
    ''' Returns a DataTable filled with the English alphabet, one letter per row. Column name is "Letter".
    ''' </summary>
    ''' <returns>A DataTable filled with the English alphabet, one letter per row. Column name is "Letter".</returns>
    ''' <remarks></remarks>
    Shared Function GetAlphabet() As DataTable
                Dim dtAlphabet As New DataTable()
        Dim drTemp As DataRow
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Setup the DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        dtAlphabet.Columns.Add("Letter", GetType(String))
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Add the letters to the DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        For nCounter As Integer = 0 To 25
            drTemp = dtAlphabet.NewRow
            drTemp.Item("Letter") = ConvertAsciiNumberToString(nCounter + 65)
            dtAlphabet.Rows.Add(drTemp)
        Next
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the DataTable filled with the alphabet
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return dtAlphabet
    End Function

    ''' <summary>
    ''' Returns a description of a major value, checking up to three different tables to find a description that matches the passed value.
    ''' </summary>
    ''' <param name="sMajor"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetMajorDescription(ByVal sMajor As String) As String
        Return ConvertMajor(sMajor)
    End Function

    ''' <summary>
    ''' Gets the building/location ID for the Faculty/Staff member provided.
    ''' </summary>
    ''' <param name="sPLID">The Whitworth ID number to get the employee's office building/location id for.</param>
    ''' <returns>The building/location ID of the employee's office. Returns N_ERROR (-255) if no building match is found.</returns>
    ''' <remarks></remarks>
    Shared Function GetFacultyStaffOfficeBuilding(ByVal sPLID As String) As Integer
                Dim sBuilding As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the employee's information
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        With GetFacultyStaffInfo(sPLID)
            If .Rows.Count > 0 Then
                With .Rows(0)
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Get the building
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    If .Item("Building") <> "" Then
                        sBuilding = Trim(.Item("Building"))
                    Else
                        sBuilding = Trim(.Item("Bldg_Desc"))
                    End If
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Check for an existing building
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    If sBuilding = "" Then
                        Try
                            sBuilding = GetDepartmentBuildings(.Item("PLDepartment")).Rows(0).Item("BuildingID")
                        Catch ex As Exception
                            sBuilding = ""
                        End Try
                    End If
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Check for a matching building ID
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    Try
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Return the building ID if a match to the code is found
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        Return GetBuildingID(sBuilding)
                    Catch ex As Exception
                        Try
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            'Return the building ID if a match to the building name is found
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            Return GetBuildingID(.Item("Bldg_Desc"))
                        Catch ex2 As Exception
                            'Empty catch statement
                        End Try
                    End Try
                End With
            End If
        End With
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Failed to find an office building
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return N_ERROR
    End Function

    ''' <summary>
    ''' Returns a DataTable filled with an employee's information.
    ''' </summary>
    ''' <param name="sPLID">The PLID # to search on.</param>
    ''' <returns>A DataTable with the employee's information.</returns>
    ''' <remarks></remarks>
    Shared Function GetFacultyStaffInfo(ByVal sPLID As String) As DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the student information for the provide PLID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            If sPLID <> "" Then
                Return GetDataTable("SELECT P.*, C.*, F.Office, F.Bldg_Desc, F.Building FROM " & DT_PEOPLE_LISTING & " P LEFT OUTER JOIN " & DT_DW_CURRENT_STUDENT_INFO & " C ON P.PLID=C.STUDENT_ID LEFT OUTER JOIN " & V_FACULTY_STAFF & " F ON P.PLID=F.PLID WHERE P.PLID='" & FormatNumberLeadingZeroes(sPLID, 7) & "'")
            End If
        Catch ex As Exception
            ReportError(ex, "", "Trying to get an employee's information and the attempt failed in WhitTools function Getter.GetFacultyStaffInfo(" & sPLID & ").")
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return an empty DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return New DataTable()
    End Function

    ''' <summary>
    ''' Returns the employee's information for the given column.
    ''' </summary>
    ''' <param name="sPLID">The PLID # to search on.</param>
    ''' <param name="sColumn">The DataTable column to return the value of.</param>
    ''' <returns>The employee's information for the given column.</returns>
    ''' <remarks></remarks>
    Shared Function GetFacultyStaffInfo(ByVal sPLID As String, ByVal sColumn As String) As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the student information for the provide PLID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            If sPLID <> "" Then
                Return GetDataTable("SELECT P.*, C.*, F.Office, F.Bldg_Desc, F.Building FROM " & DT_PEOPLE_LISTING & " P LEFT OUTER JOIN " & DT_DW_CURRENT_STUDENT_INFO & " C ON P.PLID=C.STUDENT_ID LEFT OUTER JOIN " & V_FACULTY_STAFF & " F ON P.PLID=F.PLID WHERE P.PLID='" & FormatNumberLeadingZeroes(sPLID, 7) & "'").Rows(0).Item(sColumn)
            End If
        Catch ex As Exception
            ReportError(ex, "", "Trying to get an employee's information and the attempt failed in WhitTools function Getter.GetFacultyStaffInfo(" & sPLID & ", " & sColumn & ").")
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return an empty DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_ERROR
    End Function

    ''' <summary>
    ''' Get the link for the master forms list.
    ''' </summary>
    ''' <param name="nFormID">The form ID.</param>
    ''' <returns>A link to the form url.</returns>
    ''' <remarks></remarks>
    Shared Function GetMasterFormsLink(ByVal nFormID As Integer) As String
                Dim sLink As String = ""
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Get the form entry data
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            With GetDataTable("SELECT * FROM " & DT_MASTER_FORMS_LIST & " WHERE ID='" & nFormID & "'")
                If .Rows.Count > 0 Then
                    With .Rows(0)
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Add the form URL link
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        sLink = "<a href='" & .Item("Link") & "'>" & .Item("Form") & "</a>"
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Check if the link is restricted access
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        If .Item("RestrictedAccess") = N_YES Then
                            sLink &= "&nbsp;(restricted&nbsp;access)"
                        End If
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Check if the link has a maintenance page
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        If .Item("MaintenanceLink") <> "" Then
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            'Check if the current user has permission to view the page
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            If .Item("MainContact") = GetCurrentPLID() Or IsWebTeamMember() Then
                                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                                'Check if the current user is on campus
                                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                                If CheckIPOnCampus() Then
                                    sLink &= "&nbsp;-&nbsp;<a href='" & .Item("MaintenanceLink") & "'>Maintenance</a>"
                                End If
                            End If
                        End If
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Check if the link is a Sharepoint form
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        If .Item("DWID") > 0 Then
                            sLink &= "&nbsp;<img width='15' height='15' src='~Images/sharepointlogo.jpg' />"
                        End If
                    End With
                End If
            End With
        Catch ex As Exception
            ReportError(ex)
            sLink = ""
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return an error
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return sLink
    End Function

    ''' <summary>
    ''' Gets the nearest day of the week (DateTime) to the provided date.
    ''' </summary>
    ''' <param name="dateReference">The date to reference to get the nearest day of the week to.</param>
    ''' <param name="sDayOfWeek">The day of the week to find that is nearest to the reference date. Defaults to Sunday. Accepts DayOfWeek.Monday, S_TUESDAY, N_WEDNESDAY, "Thur", etc.</param>
    ''' <param name="bRoundDayDown">If true, look to the nearest date before the reference date. Otherwise, look for the nearest date after the reference date.</param>
    ''' <returns>The nearest day of the week DateTime value to the reference date.</returns>
    ''' <remarks></remarks>
    Shared Function GetNearestDayOfWeek(ByVal dateReference As DateTime, Optional ByVal sDayOfWeek As String = S_SUNDAY, Optional ByVal bRoundDayDown As Boolean = True) As DateTime
                Dim nDayOfWeek As Integer = ConvertDayOfWeekToInt(sDayOfWeek)
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Cycle through the dates until the desired day of the week is reached
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        While dateReference.DayOfWeek <> nDayOfWeek
            If bRoundDayDown Then
                dateReference = dateReference.AddDays(-1)
            Else
                dateReference = dateReference.AddDays(1)
            End If
        End While
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the date of the nearest day of the week
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return dateReference
    End Function

    ''' <summary>
    ''' Returns the current year based on a month value passed to it.  If the current month is later in the year than the month passed to it, the current year will be considered to be the following year.
    ''' </summary>
    ''' <param name="nMonth"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetCurrentYear(ByVal nMonth As Integer) As Integer
        If Now.Month >= nMonth Then
            Return Now.Year + 1
        End If
        Return Now.Year
    End Function

    ''' <summary>
    ''' Returns the end of the month DateTime value of the provided DateTime parameter. Retains the same 
    ''' timestamp as the parameter DateTime value. Adjusts for leap years.
    ''' </summary>
    ''' <param name="dateReference">The date to get the end of the month for. Defaults to the current date.</param>
    ''' <returns>The date of the last day of the month with the timestamp the same as the passed in value.</returns>
    ''' <remarks></remarks>
    Shared Function GetEndOfMonth(Optional ByVal dateReference As DateTime = Nothing) As DateTime
        If IsNothing(dateReference) Then
            dateReference = Now
        End If
        Return CDate(dateReference.Month & "/" & DateTime.DaysInMonth(dateReference.Year, dateReference.Month) & "/" & dateReference.Year & " " & dateReference.ToShortTimeString())
    End Function

    ''' <summary>
    ''' Returns the start of the month DateTime value of the provided DateTime parameter. Retains the same 
    ''' timestamp as the parameter DateTime value.
    ''' </summary>
    ''' <param name="dateReference">The date to get the start of the month for.</param>
    ''' <returns>The date of the first day of the month with the timestamp the same as the passed in value.</returns>
    ''' <remarks></remarks>
    Shared Function GetStartOfMonth(Optional ByVal dateReference As DateTime = Nothing) As DateTime
        If IsNothing(dateReference) Then
            dateReference = Now
        End If
        Return CDate(dateReference.Month & "/1/" & dateReference.Year & " " & dateReference.ToShortTimeString())
    End Function

    ''' <summary>
    ''' Returns a random number between -2147483648 (Integer.MinValue) and 2147483647 (Integer.MaxValue).
    ''' </summary>
    ''' <param name="nMin">The minimum random integer to return.</param>
    ''' <param name="nMax">The maximum random integer to return.</param>
    ''' <returns>A random number between -2147483648 and 2147483647 (or other provided parameters).</returns>
    ''' <remarks></remarks>
    Shared Function GetRandomInt(Optional ByVal nMin As Integer = Integer.MinValue, Optional ByVal nMax As Integer = Integer.MaxValue) As Integer
        Return RandomInt(nMin, nMax)
    End Function

    ''' <summary>
    ''' Returns a random number between 0 and 2147483647 (Integer.MaxValue).
    ''' </summary>
    ''' <param name="nMax">The maximum random integer to return.</param>
    ''' <param name="nMin">The minimum random integer to return.</param>
    ''' <returns>A random number between 0 and 2147483647 (or other provided parameters).</returns>
    ''' <remarks></remarks>
    Shared Function GetRandomIntPositive(Optional ByVal nMax As Integer = Integer.MaxValue, Optional ByVal nMin As Integer = 0) As Integer
        Return RandomIntPositive(nMax, nMin)
    End Function

    ''' <summary>
    ''' Returns the provide sPLID parameter, unless that PLID does not exist. Otherwise Whitworth University's 
    ''' emergency contact PLID is returned. This function is intended to always provide at least one contact 
    ''' person's PLID for a given purpose.
    ''' </summary>
    ''' <param name="sPLID">If this PLID exists, then it is returned. Otherwise the emergency contact's PLID is returned.</param>
    ''' <param name="bOnlyStaffPLIDs">Only return a Faculty/Staff person's PLID? Even if an existing PLID is provided in the sPLID parameter?</param>
    ''' <returns>If a PLID is provided in the parameters, then that PLID is returned. Otherwise the emergency contact's PLID is returned.</returns>
    ''' <remarks></remarks>
    Shared Function GetGuaranteedPLID(Optional ByVal sPLID As String = "", Optional ByVal bOnlyStaffPLIDs As Boolean = False) As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check if the passed in PLID exists
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If ValidatePLIDExists(sPLID) Then
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Check if the only faculty/staff PLIDs should be returned
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If bOnlyStaffPLIDs Then
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Check if the passed in PLID is faculty/staff
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                If IsFacultyStaff(sPLID) Then
                    Return sPLID
                End If
            Else
                Return sPLID
            End If
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the emergency contact's PLID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Check if the variable exists
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If GetVariableValue(S_EMERGENCY_CONTACT) <> S_NOT_FOUND Then
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Check if the emergency contact is still a whitworth employee
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                If Not IsFacultyStaff(GetVariableValue(S_EMERGENCY_CONTACT)) Then
                    ReportError("The emergency contact Global Enum value (S_EMERGENCY_PLID) is not an active employee any longer. The Global Enum value S_EMERGENCY_PLID needs to be updated.")
                End If
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Return the emergency contact PLID
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Return GetVariableValue(S_EMERGENCY_CONTACT)
            Else
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Update the emergency contact PLID variable
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                UpdateVariable(S_EMERGENCY_CONTACT, S_EMERGENCY_PLID)
            End If
        Catch ex As Exception
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Update the emergency contact PLID variable
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            UpdateVariable(S_EMERGENCY_CONTACT, S_EMERGENCY_PLID)
        End Try
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the emergency contact's PLID
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_EMERGENCY_PLID
    End Function

    ''' <summary>
    ''' Returns the provide sPLID parameter, unless that PLID does not exist. Otherwise Whitworth University's 
    ''' emergency contact PLID is returned. This function is intended to always provide at least one contact 
    ''' person's PLID for a given purpose.
    ''' </summary>
    ''' <param name="sPLID">If this PLID exists, then it is returned. Otherwise the emergency contact's PLID is returned.</param>
    ''' <param name="bOnlyStaffPLIDs">Only return a Faculty/Staff person's PLID? Even if an existing PLID is provided in the sPLID parameter?</param>
    ''' <returns>If a PLID is provided in the parameters, then that PLID is returned. Otherwise the emergency contact's PLID is returned.</returns>
    ''' <remarks></remarks>
    Shared Function GetEmergencyContactPLID(Optional ByVal sPLID As String = "", Optional ByVal bOnlyStaffPLIDs As Boolean = False) As String
        Return GetGuaranteedPLID(sPLID, bOnlyStaffPLIDs)
    End Function

    ''' <summary>
    ''' Returns the database the SqlConnection object is connected to.
    ''' </summary>
    ''' <param name="cnx">The SqlConnection object to check.</param>
    ''' <returns>The database the SqlConnection object is associated with.</returns>
    ''' <remarks></remarks>
    Shared Function GetConnectionDatabase(ByVal cnx As SqlConnection) As String
        CheckConnection(cnx)
        Return cnx.Database
    End Function


    ''' <summary>
    ''' Gets the column names for a table.
    ''' </summary>
    ''' <param name="sTableName">The DataTable to pull from.</param>
    ''' <param name="sDatabaseName">The Database to pull from.</param>
    ''' <param name="sDelimiter">The way to delimit columns.</param>
    ''' <param name="bIncludeID">Include ID in the list?</param>
    ''' <returns>A list of a DataTable's column names.</returns>
    ''' <remarks></remarks>
    Shared Function GetSQLTableColumnNames(ByVal sTableName As String, ByVal sDatabaseName As String, Optional ByVal sDelimiter As String = ",", Optional ByVal bIncludeID As Boolean = False) As String
        Return GetListofValues("SELECT column_name FROM information_schema.columns WHERE table_name='web_itemtype'" & If(bIncludeID, "", " AND NOT column_name='ID'") & " ORDER BY ordinal_position", "Column_Name", sDelimiter, "", CreateSQLConnection(sDatabaseName))
    End Function

    ''' <summary>
    ''' Get a link to the image for a username.
    ''' </summary>
    ''' <param name="sUsername">The username to get the image for.</param>
    ''' <returns>A link to the image for a username.</returns>
    ''' <remarks></remarks>
    Shared Function GetPictureLinkByUsername(ByVal sUsername As String) As String
        Return HTML_PATH_WEB1 & "IDCard/" & FormatUsername(sUsername) & ".jpg"
    End Function

    ''' <summary>
    ''' Get a link to the image for a PLID.
    ''' </summary>
    ''' <param name="sPLID">The PLID to get the image for.</param>
    ''' <returns>A link to the image for a PLID.</returns>
    ''' <remarks></remarks>
    Shared Function GetPictureLinkByPLID(ByVal sPLID As String) As String
        Return HTML_PATH_WEB1 & "IDCard/" & FormatUsername(GetStudentInfo(sPLID, "PLUsername")) & ".jpg"
    End Function

    ''' <summary>
    ''' Gets a list of blacklisted email addresses where Blacklisted=Yes.
    ''' </summary>
    ''' <returns>A list of blacklisted email addresses.</returns>
    ''' <remarks></remarks>
    Shared Function GetBlacklistedEmails() As DataTable
        Return GetDataTable("SELECT * FROM " & DT_EMAIL_BLACKLIST & " WHERE Blacklisted='" & N_YES & "' ORDER BY Email")
    End Function

    ''' <summary>
    ''' Gets a list of blacklisted email addresses where Blacklisted=Yes.
    ''' </summary>
    ''' <returns>A list of blacklisted email addresses.</returns>
    ''' <remarks></remarks>
    Shared Function GetEmailBlacklist() As DataTable
        Return GetBlacklistedEmails()
    End Function

    ''' <summary>
    ''' Gets a ful list of email addresses that are in the blacklist database.
    ''' </summary>
    ''' <param name="bMustBeBlacklisted">Only return email addresses that are listed as Blacklisted=Yes? If false, then all email addresses are returned.</param>
    ''' <returns>A list of blacklisted email addresses.</returns>
    ''' <remarks></remarks>
    Shared Function GetBlacklistedEmailsFullList(Optional ByVal bMustBeBlacklisted As Boolean = False) As DataTable
        If bMustBeBlacklisted Then
            Return GetDataTable("SELECT * FROM " & DT_EMAIL_BLACKLIST & " WHERE Blacklisted='" & N_YES & "' ORDER BY Email")
        End If
        Return GetDataTable("SELECT * FROM " & DT_EMAIL_BLACKLIST & " ORDER BY Email")
    End Function

    ''' <summary>
    ''' Gets the column value for a column in a DataTable.
    ''' </summary>
    ''' <param name="sDataTable">The DataTable to check the column in.</param>
    ''' <param name="sColumn">The column to get a value for.</param>
    ''' <param name="sColumnValue">The column value to get. Defaults to "MaxLength"</param>
    ''' <param name="sDatabase">The Database the DataTable is in.</param>
    ''' <returns>The column value for the column in the DataTable</returns>
    ''' <remarks></remarks>
    Shared Function GetDataTableColumnValue(ByVal sDataTable As String, ByVal sColumn As String, Optional ByVal sColumnValue As String = "MaxLength", Optional ByVal sDatabase As String = "") As String
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check column value
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sColumnValue = "COLUMN_NAME" Then
            sColumnValue = "ColumnName"
        ElseIf sColumnValue = "DATA_TYPE" Then
            sColumnValue = "DataType"
        ElseIf sColumnValue = "COLUMN_DEFAULT" Then
            sColumnValue = "DefaultValue"
        ElseIf sColumnValue = "TABLE_CATALOG" Then
            sColumnValue = "DatabaseName"
        ElseIf sColumnValue = "TABLE_SCHEMA" Then
            sColumnValue = "TableSchema"
        ElseIf sColumnValue = "TABLE_NAME" Then
            sColumnValue = "TableName"
        ElseIf sColumnValue = "IS_NULLABLE" Then
            sColumnValue = "Nullable"
        ElseIf sColumnValue = "ORDINAL_POSITION" Then
            sColumnValue = "OrdinalPosition"
        ElseIf sColumnValue = "NUMERIC_PRECISION" Then
            sColumnValue = "NumericPrecision"
        ElseIf sColumnValue = "DATETIME_PRECISION" Then
            sColumnValue = "DateTimePrecision"
        Else 'CHARACTER_MAXIMUM_LENGTH
            sColumnValue = "MaxLength"
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check if a database is provided
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sDatabase <> "" Then
            If Not DataTableInDatabase(sDataTable, sDatabase) Then
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Get the database
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                sDatabase = GetDataTableDatabase(sDataTable)
            End If
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Strip Web3. from DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        sDataTable = Replace(Replace(sDataTable, "web3.", ""), "Web3.", "")
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check the Database
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sDatabase = "" Or sDatabase = S_FAILED Or sDatabase = S_NOT_FOUND Then
            If sDataTable.Contains(".") Then
                sDatabase = sDataTable.Substring(0, sDataTable.IndexOf("."))
            Else
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'No database is provided
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ReportError("WhitTools.Getter.GetDataTableColumnValue(DT: " & sDataTable & ", Column: " & sColumn & ", DB: " & sDatabase & ") - Error 1 - No database provided.")
                Return S_ERROR
            End If
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check the DataTable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If sDataTable.Contains(".") Then
            sDataTable = sDataTable.Substring(sDataTable.LastIndexOf(".") + 1, sDataTable.Length - sDataTable.LastIndexOf(".") - 1)
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check for a valid Database
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Not IsDatabase(sDatabase) Then
            ReportError("WhitTools.Getter.GetDataTableColumnValue(DT: " & sDataTable & ", Column: " & sColumn & ", DB: " & sDatabase & ") - Error 2 - (" & sDatabase & ") is not a valid database.")
            Return S_ERROR
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Check for a valid DataTable in the Database
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Not DataTableInDatabase(sDataTable, sDatabase) Then
            ReportError("WhitTools.Getter.GetDataTableColumnValue(DT: " & sDataTable & ", Column: " & sColumn & ", DB: " & sDatabase & ") - Error 3 - (" & sDataTable & ") is not a valid DataTable in the (" & sDatabase & ") database.")
            Return S_ERROR
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return the max length value of the column
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If DataTableHasColumn(sDataTable, sColumn, sDatabase) Then
            Try
                Return CInt(GetDataTable("SELECT COLUMN_NAME AS ColumnName, DATA_TYPE AS DataType, COLUMN_DEFAULT AS DefaultValue, TABLE_CATALOG AS DatabaseName, TABLE_SCHEMA AS TableSchema, TABLE_NAME AS TableName, CHARACTER_MAXIMUM_LENGTH AS MaxLength, IS_NULLABLE AS Nullable, ORDINAL_POSITION AS OrdinalPosition, NUMERIC_PRECISION AS NumericPrecision, DATETIME_PRECISION AS DateTimePrecision FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" & CleanSQL(sDataTable) & "' AND COLUMN_NAME='" & CleanSQL(sColumn) & "' ORDER BY ORDINAL_POSITION ASC", CreateSQLConnection(sDatabase)).Rows(0).Item(sColumnValue).ToString())
            Catch ex As Exception
                'Empty catch statement
            End Try
        Else
            ReportError("WhitTools.Getter.GetDataTableColumnValue(DT: " & sDataTable & ", Column: " & sColumn & ", DB: " & sDatabase & ") - Error 4 - (" & sColumn & ") is not a valid column in the (" & sDataTable & ") DataTable.")
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Return an error
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return S_ERROR
    End Function

    ''' <summary>
    ''' Gets the max length value for a column in a DataTable.
    ''' </summary>
    ''' <param name="sDataTable">The DataTable to check the column in.</param>
    ''' <param name="sColumn">The column to get the max length value for.</param>
    ''' <param name="sDatabase">The Database the DataTable is in.</param>
    ''' <returns>The max length value for the column in the DataTable</returns>
    ''' <remarks></remarks>
    Shared Function GetDataTableColumnMaxLength(ByVal sDataTable As String, ByVal sColumn As String, Optional ByVal sDatabase As String = "") As Integer
        Try
            Return GetDataTableColumnValue(sDataTable, sColumn, "MaxLength", sDatabase)
        Catch ex As Exception
            'Empty catch statement
        End Try
        Return N_ERROR
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sDepartmentCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetSubDepartments(ByRef sDepartmentCode As String) As String
                Dim dtDepartment As DataTable = GetDataTable("SELECT * FROM " & DT_ARA_DEPARTMENTS & " WHERE Code='" & CleanSQL(sDepartmentCode) & "'")
        Dim dtPrimaryDepartment As DataTable
        Dim nDepartmentID As Integer
                If dtDepartment.Rows.Count > 0 Then
            With dtDepartment.Rows(0)
                                If .Item("RedirectTo") = "-1" Then
                    nDepartmentID = .Item("ID")
                Else
                    dtPrimaryDepartment = GetDataTable("SELECT * FROM " & DT_ARA_DEPARTMENTS & " WHERE ID='" & CleanSQL(.Item("RedirectTo")) & "'")
                    If dtPrimaryDepartment.Rows.Count > 0 Then
                        sDepartmentCode = dtPrimaryDepartment.Rows(0).Item("Code")
                        nDepartmentID = dtPrimaryDepartment.Rows(0).Item("ID")
                    End If
                End If
                                Return "'" & sDepartmentCode & "', '" & Replace(GetListofValues("SELECT * FROM " & DT_ARA_DEPARTMENTS & " WHERE RedirectTo='" & nDepartmentID & "'", "Code", "','"), " ", "") & "'"
            End With
        End If
                Return ""
    End Function

    ''' <summary>
    ''' Checks if their are any email issues currently happening.
    ''' </summary>
    ''' <returns>Are there email issues occurring?</returns>
    ''' <remarks></remarks>
    Shared Function GetEmailIssuesOccurring() As Boolean
        Return GetVariableValueBool(VARIABLE_EMAIL_ISSUES, False)
    End Function

    ''' <summary>
    ''' Checks to see if username belongs to a specified department.
    ''' </summary>
    ''' <param name="sDepartment"></param>
    ''' <param name="sUsername"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetDepartmentMembership(ByVal sDepartment As String, ByVal sUsername As String) As Boolean
        Return GetDataTable("SELECT * FROM " & DT_PEOPLE_LISTING & " WHERE PLID IN (SELECT ID FROM adTelephone.dbo.CombinedDepartments_v WHERE Department='" & CleanSQL(sDepartment) & "') AND PLActive='" & N_YES & "' AND PLUsername='" & CleanSQL(sUsername) & "'").Rows.Count > 0
    End Function

    ''' <summary>
    ''' Checks the web.config file for the given variable
    ''' </summary>
    ''' <param name="sVariable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function GetWebConfigVariable(ByVal sVariable As String) As String
        Return WebConfigurationManager.AppSettings(sVariable)
    End Function

    Shared Function GetDepartmentCode(byval username As string)
        return GetDepCode(GetPrimaryDepartment(username))
    End Function

    Shared Function GetNumberSelectedItems(byref target As listcontrol) as integer
       return (from item in target.Items where item.Selected select item).Count()
    End Function

    public shared Function GetPageControlReference(byval controlName As string) As control
        return GetControlReference(getcurrentpage(),controlname.tolower)
    End Function

    public shared Function GetControlReference(ByVal parentObject as Object, byval controlName As string) As control
        Dim foundControl As Control

        For each currentControl As Control In parentObject.Controls
            SearchForControl(currentControl,controlName,foundcontrol)
        Next

        Return foundControl
    End Function

   Private shared Function SearchForControl(byref parentControl,byval controlName,byref foundcontrol) As Control
        try
            If parentControl isnot nothing andalso parentControl.ID.ToString().ToLower() = controlName
            foundControl = parentcontrol
            Exit function
        End If

        Catch ex As Exception

        End Try
        
        For each currentControl As Control In parentControl.Controls
            SearchForControl(currentControl,controlName,foundcontrol)
        Next
    End Function

End Class