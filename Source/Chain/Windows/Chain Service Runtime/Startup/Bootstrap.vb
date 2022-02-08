﻿Option Compare Text
Option Explicit On

Imports CHCCommonLibrary.AreaEngine.CommandLine
Imports CHCCommonLibrary.AreaEngine.Miscellaneous
Imports CHCCommonLibrary.AreaEngine.DataFileManagement.Encrypted
Imports CHCSidechainServiceLibrary.AreaCommon.Main



Namespace AreaCommon.Startup

    ''' <summary>
    ''' This static class run the application
    ''' </summary>
    Module Bootstrap

        Private Property _Bootstrap As New CHCSidechainServiceLibrary.AreaCommon.Startup.Bootstrap



        ''' <summary>
        ''' This method provide to acquire the service information
        ''' </summary>
        ''' <returns></returns>
        Private Function acquireServiceInformation() As Boolean
            environment.log.trackIntoConsole("Load service information")

            Try
                environment.log.trackEnter("startUp.acquireServiceInformation")

                With state.serviceInformation
                    .chainName = CUSTOM_ChainServiceName

                    If environment.settings.intranetMode Then
                        .addressIP = environment.ipAddress.local
                    Else
                        .addressIP = environment.ipAddress.public
                    End If

                    .intranetMode = environment.settings.intranetMode
                    .netWorkName = environment.settings.networkReferement
                    .platformHost = "Microsoft Windows Desktop Application service"
                    .softwareRelease = My.Application.Info.Version.ToString()

                    If environment.settings.secureChannel Then
                        .completeAddress = "https://"
                    Else
                        .completeAddress = "http://"
                    End If

                    .completeAddress += .addressIP & "/api/" & environment.settings.serviceID

                    .currentStatus = CHCProtocolLibrary.AreaCommon.Models.Service.InternalServiceInformation.EnumInternalServiceState.starting
                End With

                Return True
            Catch ex As Exception
                environment.log.trackException("StartUp.loadDataInformation", "Error during Load data information:" & ex.Message)

                Return False
            Finally
                environment.log.trackExit("startUp.loadDataInformation")
            End Try
        End Function



        ''' <summary>
        ''' This method provide to prepare the application to the startup
        ''' </summary>
        ''' <returns></returns>
        Public Function run() As Boolean
            Try
                Dim problemDescription As String
                Dim proceed As Boolean = True

                If proceed Then
                    If Not _Bootstrap.readParameters() Then
                        MessageBox.Show("Problem during read a parameters", "Notify problem", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        proceed = False
                    End If
                End If
                If proceed Then
                    If Not _Bootstrap.printWelcome(CUSTOM_ChainServiceName, My.Application.Info.Version.ToString()) Then
                        MessageBox.Show("Problem during print a welcome", "Notify problem", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        proceed = False
                    End If
                End If
                If proceed Then
                    If Not _Bootstrap.managePath(problemDescription) Then
                        MessageBox.Show(problemDescription, "Notify problem", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        proceed = False
                    End If
                End If
                If proceed Then
                    If Not _Bootstrap.readSettings(CUSTOM_ChainServiceName, problemDescription) Then
                        MessageBox.Show(problemDescription, "Notify problem", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        proceed = False
                    End If
                End If
                If proceed Then
                    If Not _Bootstrap.trackRuntimeStart(problemDescription) Then
                        MessageBox.Show(problemDescription, "Notify problem", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        proceed = False
                    End If
                End If
                If proceed Then
                    proceed = _Bootstrap.acquireIPAddress()
                End If
                If proceed Then
                    proceed = acquireServiceInformation()
                End If
                If proceed Then
                    proceed = _Bootstrap.readAdminKeyStore()
                End If

                Return proceed
            Catch ex As Exception
                MessageBox.Show("An error occurrent during moduleMain.bootstrap " & Err.Description, "Notify problem", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Return False
            End Try
        End Function

    End Module

End Namespace
