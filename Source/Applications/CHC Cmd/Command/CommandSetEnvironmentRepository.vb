﻿Option Compare Text
Option Explicit On

Imports CHCCmd.AreaCommon.Models
Imports CHCCommonLibrary.AreaEngine.CommandLine




Namespace AreaCommon.Command

    ''' <summary>
    ''' This class manage the command Set Environment Repository
    ''' </summary>
    Public Class CommandSetEnvironmentRepository : Implements CommandModel

        Private Property _Command As CommandStructure


        ''' <summary>
        ''' This method provide to test write a path
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Private Function tryWritePath(ByVal path As String) As Boolean
            path = IO.Path.Combine(path, "environment.path")

            Try
                IO.File.WriteAllText(path, "Test")

                If IO.File.Exists(path) Then
                    IO.File.Delete(path)
                    Return True
                End If
            Catch ex As Exception
            End Try

            Return False
        End Function

        ''' <summary>
        ''' This method search define path
        ''' </summary>
        ''' <returns></returns>
        Public Function searchUserWritablePath() As String
            Dim found As Boolean = False
            Dim path As String = ""

            Try
                If Not found Then
#If DEBUG Then
                    path = "E:\CryptoHideCoinDTN\Binary\Applications\Console\CHC Cmd"
#Else
                path = Environment.CurrentDirectory
#End If
                    found = tryWritePath(path)
                End If
                If Not found Then
                    path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

                    found = tryWritePath(path)
                End If
                If Not found Then
                    path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)

                    found = tryWritePath(path)
                End If

                If found Then
                    Return path
                End If
            Catch ex As Exception
            End Try

            Return ""
        End Function

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
                Dim path As String = ""
                Dim parameterDataPath As String = ""

                If Not _Command.haveParameter("dataPath") Then
                    Console.WriteLine("Error: dataPath parameter missing")

                    Return False
                End If

                If Not IO.Directory.Exists(_Command.parameterValue("dataPath")) Then
                    Try
                        IO.Directory.CreateDirectory(StrConv(_Command.parameterValue("dataPath"), VbStrConv.ProperCase))
                    Catch ex As Exception
                        Console.WriteLine("Error: Problem during create a environment repository path")
                    End Try
                End If

                path = searchUserWritablePath()

                Try
                    IO.File.WriteAllText(IO.Path.Combine(path, "environment.path"), _Command.parameterValue("dataPath"))

                    Console.WriteLine("Environment repository path updated")

                    Return True
                Catch ex As Exception
                    Console.WriteLine("Error: Problem during set environment repository")
                End Try

                Return False
            Catch ex As Exception
                Return False
            End Try
        End Function

    End Class

End Namespace
