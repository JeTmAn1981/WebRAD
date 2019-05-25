Imports Microsoft.VisualBasic
Imports System.Data
Imports WhitTools.DataTables
Imports Common.General.Main
Imports Common.General.Variables

Imports WhitTools.Utilities
Namespace General
    Public Class DataTypes
         Shared Function GetSQLDataTypeName(ByVal nSQLDataType As Integer, Optional ByVal nID As Integer = 0, Optional ByVal sLocation As String = "") As String
            Dim dt As DataTable = GetDataTable("Select * from " & DT_WEBRAD_CONTROLSQLTYPES & " Where ID = " & nSQLDataType)

            Try
                Return dt.Rows(0).Item("Datatype")
            Catch ex As Exception
                'Logger.Error(ex.ToString)
            End Try

            Return ""
        End Function

         Shared Function GetSQLNullValue(ByVal nDataType As Integer) As String
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Declare local variables
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim dt As DataTable = GetDataTable(cnx, "Select * From " & DT_WEBRAD_CONTROLSQLTYPES & " Where ID = " & nDataType)

            If dt.Rows.Count > 0 Then

                Return dt.Rows(0).Item("NullValue")
            End If

            Return ""
        End Function

        Shared Function IsDataType(ByVal nControlTypeID As Integer, ByVal sDataType As String) As Boolean
            Try
                Return db.ControlTypes.First(Function(controlType) controlType.ID = nControlTypeID).ControlDataType.Description.ToLower() = sDataType.ToLower()
            Catch ex As Exception
                Return False
            End Try
        End Function

        Shared Function GetControlDataType(ByVal nControlTypeID As Integer) As Integer
            Try
                Return GetDataTable("Select DataType FROM " & DT_WEBRAD_CONTROLTYPES & " WHERE ID = " & nControlTypeID, cnx).Rows(0).Item("DataType")
            Catch ex As Exception
                logger.Error("Error getting control data type for " & nControlTypeID)
                logger.Error(ex.ToString)
            End Try

            Return 0
        End Function

        Shared  Function GetDataTypeDescription(ByVal nDataType As Integer) As String
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Declare local variables
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim dtDataType As DataTable = GetDataTable("Select * From " & DT_WEBRAD_CONTROLDATATYPES & " Where ID = '" & nDataType & "'", cnx)

            If dtDataType.Rows.Count > 0 Then
                With dtDataType.Rows(0)
                    Return If(.Item("DesignerDescription") <> "", .Item("DesignerDescription"), .Item("Description"))
                End With
            End If

            Return ""
        End Function

         Shared  Function GetSetValueDataTypes() As String
            Return "1,2,3,5,7,8"
        End Function

            Shared  Public  Function IsMultiElementDataType(ByVal datatype As integer) As Boolean
            return datatype = N_CHECKBOXLIST_DATATYPE  Or datatype = N_RADIOBUTTONLIST_DATATYPE
        End Function

        public Shared function DataTypeRequiresLabelOnPrintable(ByVal dataType As integer) As boolean
                return (GetDataTable("SELECT * FROM " & DT_WEBRAD_CONTROLDATATYPES & " WHERE ID = " & dataType).Rows(0).Item("LabelOnPrintable") = "1")
        End function
    End Class
End Namespace


