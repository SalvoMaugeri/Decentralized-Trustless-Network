﻿Option Compare Text
Option Explicit On

Imports System.Web.Http
Imports CHCModels.AreaModel.Network.Response





Namespace Controllers

    ' GET: api/{GUID service}/service/testController
    <RoutePrefix("LWMServiceApi")>
    Public Class testController

        Inherits ApiController



        ''' <summary>
        ''' This method provide to return a current time of server (test service)
        ''' </summary>
        ''' <returns></returns>
        Public Function GetValue() As RemoteResponse
            Dim result As New RemoteResponse
            Dim enter As Boolean = False
            Try
                enter = True

                result.responseTime = CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime()
            Catch ex As Exception
                result.responseStatus = RemoteResponse.EnumResponseStatus.inError
                result.errorDescription = "503 - Generic Error"
            End Try

            Return result
        End Function

    End Class

End Namespace
