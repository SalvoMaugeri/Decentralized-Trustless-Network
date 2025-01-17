﻿Option Compare Text
Option Explicit On

Imports CHCCmd.AreaCommon.Models
Imports CHCCommonLibrary.AreaEngine.CommandLine




Namespace AreaCommon.Command

    ''' <summary>
    ''' This class manage the command help 
    ''' </summary>
    Public Class CommandHelp : Implements CommandModel

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
            Console.WriteLine("Help list command")
            Console.WriteLine("=================")
            Console.WriteLine()
            Console.WriteLine("-help                        Show this list")
            Console.WriteLine("-info                        Show an info of this application")
            Console.WriteLine("-release                     Show a relase of this application")
            Console.WriteLine("-updateSystemDate            Update the system date")
            Console.WriteLine("-currentTime                 Show the current time (current/GMT/Timestamp)")
            Console.WriteLine("-ipAddress                   Get the IP Address (public and private)")
            Console.WriteLine("-sideChainServiceSettings    Open a Chain Settings Editor")
            Console.WriteLine("   --service:                Set a service name")
            Console.WriteLine("   --dataPath:               Set a data path")
            Console.WriteLine("   --showAsFile              Show the content in a notepad")
            Console.WriteLine("   --password:               Set a password to decode")
            Console.WriteLine("-showLog                     Show a log of a service")
            Console.WriteLine("   --service:                Set a service name")
            Console.WriteLine("   --dataPath:               Set a data path")
            Console.WriteLine("   --password:               Set a password to decode settings file")
            Console.WriteLine("   --securityKey:            Set a password of a security key")
            Console.WriteLine("-startServe                  Start a service")
            Console.WriteLine("   --service:                Set a service name")
            Console.WriteLine("   --dataPath:               Set a data path")
            Console.WriteLine("   --password:               Set a password to decode settings file")
            Console.WriteLine("-stopServe                   Stop a service")
            Console.WriteLine("   --service:                Set a service name")
            Console.WriteLine("   --dataPath:               Set a data path")
            Console.WriteLine("   --password:               Set a password to decode settings file")
            Console.WriteLine("   --securityKey:            Set a password of a security key")
            Console.WriteLine("-testServe                   Test a service")
            Console.WriteLine("   --service:                Set a service name")
            Console.WriteLine("   --dataPath:               Set a data path")
            Console.WriteLine("   --password:               Set a password to decode settings file")
            Console.WriteLine("   --securityKey:            Set a password of a security key")
            Console.WriteLine("-setEnvironmentRepository    Set the repository of all environment")
            Console.WriteLine("   --dataPath:               Set a data path of repository")
            Console.WriteLine("-getEnvironmentRepository    Get the repository of all environment")

            Return True
        End Function

    End Class

End Namespace
