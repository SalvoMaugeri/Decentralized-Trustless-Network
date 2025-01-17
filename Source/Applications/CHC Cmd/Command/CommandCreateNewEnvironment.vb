﻿Option Compare Text
Option Explicit On

Imports CHCCmd.AreaCommon.Models
Imports CHCCommonLibrary.AreaEngine.CommandLine




Namespace AreaCommon.Command

    ''' <summary>
    ''' This class manage the command a create new environment
    ''' </summary>
    Public Class CommandCreateNewEnvironment : Implements CommandModel

        Private Property _Command As CommandStructure

        Private Property CommandModel_command As CommandStructure Implements CommandModel.command
            Get
                Return _Command
            End Get
            Set(value As CommandStructure)
                _Command = value
            End Set
        End Property

        Private Function CommandModel_run() As Boolean Implements CommandModel.run
            Try
                If Not _Command.haveParameter("name") Then
                    Console.WriteLine("Error: name missing")

                    Return False
                End If

                If Not _Command.haveParameter("dataPath") Then
                    Console.WriteLine("Error: dataPath parameter missing")

                    Return False
                End If

                Dim path As String = AreaEngine.EnvironmentRepositoryEngine.searchUserEnvironmentPath()
                Dim environmentRepositoryPath As String = IO.Path.Combine(path, "environment.path")
                Dim environmentPath As String = ""

                If IO.File.Exists(environmentRepositoryPath) Then
                    environmentPath = IO.File.ReadAllText(environmentRepositoryPath)

                    If Not AreaEngine.EnvironmentsEngine.createNewEnvironment(environmentPath, _Command.parameterValue("name"), _Command.parameterValue("dataPath")) Then
                        Console.WriteLine("Error: Problem during create a new environment")
                    End If
                End If

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

    End Class

End Namespace
