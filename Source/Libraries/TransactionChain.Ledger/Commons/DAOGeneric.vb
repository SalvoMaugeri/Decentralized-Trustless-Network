﻿Option Compare Text
Option Explicit On

Imports System.Data.SQLite





Namespace AreaCommon.DAO

    ''' <summary>
    ''' This class contain the generic method to manage a db
    ''' </summary>
    Public Class DBGeneric

        ''' <summary>
        ''' This is the enumeration relative of a type of db
        ''' </summary>
        Public Enum DBPropertyID

            undefined
            typeOfDB
            dateCreation
            name

        End Enum

        Public Shared Property logIstance As CHCCommonLibrary.Support.LogEngine


        ''' <summary>
        ''' This method provide to insert a SQL Property Identity DB
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Private Shared Function insertSQLPropertyIdentityDB(ByVal id As DBPropertyID, ByVal value As String, ByVal connectionString As String) As Boolean
            Try
                Dim sql As String = ""

                logIstance.track("DBGeneric.insertSQLPropertyIdentityDB", "Begin")

                sql += "INSERT INTO dbIdentity "
                sql += " (property_id, value) "
                sql += "VALUES "
                sql += " (" & id & ", '" & value & "')"

                Return executeDataTable(sql, connectionString)
            Catch ex As Exception
                logIstance.track("DBGeneric.insertSQLPropertyIdentityDB", "Failed = " & ex.Message, "fatal", True)

                Return False
            Finally
                logIstance.track("DBGeneric.insertSQLPropertyIdentityDB", "Complete")
            End Try
        End Function


        ''' <summary>
        ''' This method provide to Create DB Table
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <param name="connectionString"></param>
        ''' <returns></returns>
        Public Shared Function executeDataTable(ByVal sql As String, Optional ByVal connectionString As String = "", Optional ByVal closeDB As Boolean = True, Optional ByRef connectionDB As SQLiteConnection = Nothing) As Boolean
            Try
                Dim sqlCommand As SQLiteCommand

                logIstance.track("DBGeneric.createDBTable", "Begin")

                If (connectionString.Length > 0) Then
                    connectionDB = New SQLiteConnection(connectionString)

                    connectionDB.Open()
                End If

                logIstance.track("DBGeneric.createDBTable", "Connection Open")

                sqlCommand = New SQLiteCommand(connectionDB)

                sqlCommand.CommandText = sql

                sqlCommand.ExecuteScalar()

                logIstance.track("DBGeneric.createDBTable", "Command execute")

                If closeDB Then
                    connectionDB.Close()

                    logIstance.track("DBGeneric.createDBTable", "Connection close")
                End If

                Return True
            Catch ex As Exception
                logIstance.track("DBGeneric.createDBTable", ex.Message, "fatal")

                Return False
            Finally
                logIstance.track("DBGeneric.createDBTable", "Complete")
            End Try
        End Function

        ''' <summary>
        ''' This method provide to insert SQL
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <param name="connectionString"></param>
        ''' <returns></returns>
        Public Shared Function executeDataTable(ByVal sql As String, ByRef connectionString As String) As Boolean
            Try
                Dim connectionDB As SQLiteConnection
                Dim sqlCommand As SQLiteCommand

                logIstance.track("DBGeneric.executeDataTable", "Begin")

                connectionDB = New SQLiteConnection(connectionString)

                connectionDB.Open()

                logIstance.track("DBGeneric.executeDataTable", "Connection Open")

                sqlCommand = New SQLiteCommand(connectionDB)

                sqlCommand.CommandText = sql

                sqlCommand.ExecuteScalar()

                logIstance.track("DBGeneric.executeDataTable", "Command executed")

                connectionDB.Close()

                logIstance.track("DBGeneric.executeDataTable", "Connection Closed")

                Return True
            Catch ex As Exception
                logIstance.track("DBGeneric.executeDataTable", ex.Message, "fatal")

                Return False
            End Try
        End Function

        ''' <summary>
        ''' This method provide to insert data table with result ID
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <param name="connectionString"></param>
        ''' <returns></returns>
        Public Shared Function insertDataTableWithResult(ByVal sql As String, ByRef connectionString As String) As Integer
            Try
                Dim connectionDB As SQLiteConnection
                Dim sqlCommand As SQLiteCommand
                Dim response As Integer

                logIstance.track("ChainStateEngine.insertSQLNewChain", "Begin")

                connectionDB = New SQLiteConnection(connectionString)

                connectionDB.Open()

                logIstance.track("ChainStateEngine.insertSQLNewChain", "DB Open")

                sqlCommand = New SQLiteCommand(connectionDB)

                sqlCommand.CommandText = sql

                response = sqlCommand.ExecuteScalar()

                logIstance.track("ChainStateEngine.insertSQLNewChain", "Execute scalar")

                connectionDB.Close()

                logIstance.track("ChainStateEngine.insertSQLNewChain", "DB Close")

                Return response
            Catch ex As Exception
                logIstance.track("ChainStateEngine.insertSQLNewChain", "Failed = " & ex.Message, "fatal", True)

                Return False
            Finally
                logIstance.track("ChainStateEngine.insertSQLNewChain", "Complete")
            End Try
        End Function

        ''' <summary>
        ''' This method provide to select only result from a select SQL
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <param name="connectionString"></param>
        ''' <param name="closeDB"></param>
        ''' <returns></returns>
        Public Shared Function selectResultDataTable(ByVal sql As String, Optional ByVal connectionString As String = "", Optional closeDB As Boolean = True, Optional ByRef connectionDB As SQLiteConnection = Nothing) As Object
            Dim result As Object

            Try
                Dim sqlCommand As SQLiteCommand

                logIstance.track("DBGeneric.selectResultDataTable", "Begin")

                If (connectionString.Length > 0) Then
                    connectionDB = New SQLiteConnection(connectionString)

                    connectionDB.Open()

                    logIstance.track("DBGeneric.selectResultDataTable", "DB Open")
                End If

                sqlCommand = New SQLiteCommand(connectionDB)

                sqlCommand.CommandText = sql

                result = sqlCommand.ExecuteScalar()

                logIstance.track("DBGeneric.selectResultDataTable", "Execute scalar")

                If closeDB Then
                    connectionDB.Close()

                    logIstance.track("DBGeneric.selectResultDataTable", "DB Close")
                End If

                Return result
            Catch ex As Exception
                logIstance.track("DBGeneric.selectResultDataTable", "Failed = " & ex.Message, "fatal", True)

                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' This method provide to create an identity db table
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function createIdentityDBTable(ByVal connectionString As String) As Boolean
            Try
                Dim sql As String = ""

                logIstance.track("DBGeneric.createIdentityDBTable", "Begin")

                sql += "CREATE TABLE dbIdentity "
                sql += " (property_id INTEGER PRIMARY KEY, "
                sql += "  value NVARCHAR(1024) NOT NULL "
                sql += ");"

                Return executeDataTable(sql, connectionString)
            Catch ex As Exception
                logIstance.track("DBGeneric.selectResultDataTable", "Failed = " & ex.Message, "fatal")

                Return False
            Finally
                logIstance.track("DBGeneric.createIdentityDBTable", "Complete")
            End Try
        End Function

        ''' <summary>
        ''' This method provide to write an Identity on a DB
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function writeIdentityDB(ByVal connectionString As String, ByVal nameDB As String, ByVal typeDB As String) As Boolean
            Try
                logIstance.track("DBGeneric.writeIdentityDB", "Begin")

                insertSQLPropertyIdentityDB(DBPropertyID.dateCreation, CHCCommonLibrary.AreaEngine.Miscellaneous.atMomentGMT(), connectionString)
                insertSQLPropertyIdentityDB(DBPropertyID.name, nameDB, connectionString)
                insertSQLPropertyIdentityDB(DBPropertyID.typeOfDB, typeDB, connectionString)

                logIstance.track("DBGeneric.writeIdentityDB", "Complete")

                Return True
            Catch ex As Exception
                logIstance.track("DBGeneric.writeIdentityDB", ex.Message, "fatal")

                Return False
            End Try
        End Function

    End Class

End Namespace