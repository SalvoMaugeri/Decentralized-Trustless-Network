﻿Option Compare Text
Option Explicit On

Imports CHCCommonLibrary.Support
Imports CHCCommonLibrary.AreaEngine.DataFileManagement.XML
Imports CHCCommonLibrary.AreaEngine.Encryption




Namespace AreaProtocol

    Public Class A0x6

        Public Class RequestModel
            Public Property requestCode As String = "A0x6"

            Public Property requestDateTimeStamp As Double = 0
            Public Property publicWalletAddressRequester As String = ""
            Public Property requestHash As String = ""
            Public Property signature As String = ""

            Public Property generalCondition As String = ""

            Public Overrides Function toString() As String
                Dim tmp As String = ""

                tmp += MyBase.ToString()
                tmp += generalCondition

                Return tmp
            End Function

            Public Function getHash() As String
                Return HashSHA.generateSHA256(Me.toString())
            End Function

        End Class

        Public Class FileEngine

            Inherits BaseFile(Of RequestModel)

        End Class

        Public Class RecoveryState

            Public Shared Function fromRequest(ByRef value As RequestModel, ByRef transactionChainRecord As CHCCommonLibrary.AreaCommon.Models.General.IdentifyRecordLedger, ByVal hashContent As String) As Boolean
                Dim proceed As Boolean = True

                If proceed Then
                    proceed = AreaCommon.state.runtimeState.addProperty(AreaState.ChainStateEngine.PropertyID.generalCondition, value.generalCondition, transactionChainRecord.recordCoordinate, transactionChainRecord.recordHash, hashContent, False)
                End If

                Return proceed
            End Function

            Public Shared Function fromTransactionLedger(ByVal statePath As String, ByRef data As TransactionChainLibrary.AreaLedger.LedgerEngine.SingleRecordLedger) As Boolean
                Try
                    AreaCommon.state.runtimeState.activeNetwork.generalCondition.value = TransactionChainLibrary.AreaEngine.Ledger.State.StateEngine.readContentFromFile(statePath, data.detailInformation)

                    Return True
                Catch ex As Exception
                    Return False
                End Try
            End Function

        End Class

        Public Class Manager

            Private data As New RequestModel

            Public Property log As LogEngine
            Public Property currentService As CHCProtocolLibrary.AreaCommon.Models.Administration.ServiceStateResponse




            Private Function writeDataIntoLedger(ByVal contentStatePath As String, ByRef hashContent As String) As CHCCommonLibrary.AreaCommon.Models.General.IdentifyRecordLedger
                Try
                    With AreaCommon.state.currentBlockLedger.currentRecord
                        .actionCode = "a0x6"
                        .registrationDate = CHCCommonLibrary.AreaEngine.Miscellaneous.timestampFromDateTime()
                        .detailInformation = HashSHA.generateSHA256(data.generalCondition)
                        .requesterPublicAddress = data.publicWalletAddressRequester
                        .requestHash = data.requestHash
                    End With

                    hashContent = AreaCommon.state.currentBlockLedger.currentRecord.detailInformation

                    TransactionChainLibrary.AreaEngine.Ledger.State.StateEngine.writeDataContent(contentStatePath, data.generalCondition, hashContent)

                    If AreaCommon.state.currentBlockLedger.BlockComplete() Then
                        Return AreaCommon.state.currentBlockLedger.saveAndClean()
                    End If
                Catch ex As Exception
                    currentService.currentAction.setError(Err.Number, ex.Message)

                    log.track("A0x6Manager.init", ex.Message, "fatal")
                End Try

                Return New CHCCommonLibrary.AreaCommon.Models.General.IdentifyRecordLedger
            End Function


            Public Function init(ByRef paths As CHCProtocolLibrary.AreaSystem.VirtualPathEngine, ByVal generalCondition As String, ByVal publicWalletIdAddress As String, ByVal privateKeyRAW As String) As Boolean
                Try
                    Dim requestFileEngine As New FileEngine
                    Dim ledgerCoordinate As CHCCommonLibrary.AreaCommon.Models.General.IdentifyRecordLedger
                    Dim hashContent As String

                    log.track("A0x6Manager.init", "Begin")

                    currentService.currentAction.setAction("1x0007", "BuildManager - A0x6 - A0x6Manager")

                    If currentService.requestCancelCurrentRunCommand Then Return False

                    data.generalCondition = generalCondition
                    data.publicWalletAddressRequester = publicWalletIdAddress
                    data.requestDateTimeStamp = CHCCommonLibrary.AreaEngine.Miscellaneous.timestampFromDateTime()
                    data.requestHash = data.getHash
                    data.signature = CHCProtocolLibrary.AreaWallet.Support.WalletAddressEngine.createSignature(privateKeyRAW, data.requestHash)

                    requestFileEngine.data = data

                    requestFileEngine.fileName = IO.Path.Combine(AreaCommon.paths.workData.currentVolume.requests, data.requestHash & ".request")

                    If requestFileEngine.save() Then
                        log.track("A0x6Manager.init", "request - Saved")

                        ledgerCoordinate = writeDataIntoLedger(paths.workData.state.contents, hashContent)

                        If (ledgerCoordinate.recordCoordinate.Length = 0) Then
                            currentService.currentAction.setError("-1", "Error during update ledger")
                            currentService.currentAction.reset()

                            log.track("A0x6Manager.init", "Error: Error during update ledger", "fatal")

                            Return False
                        End If

                        log.track("A0x6Manager.init", "Ledger updated")

                        If Not RecoveryState.fromRequest(data, ledgerCoordinate, hashContent) Then
                            currentService.currentAction.setError("-1", "Error during update State")
                            currentService.currentAction.reset()

                            log.track("A0x6Manager.init", "Error: Error during update State", "fatal")

                            Return False
                        End If

                        log.track("A0x6Manager.init", "State updated")

                        Return True
                    End If
                Catch ex As Exception
                    currentService.currentAction.setError(Err.Number, ex.Message)

                    log.track("A0x6Manager.init", ex.Message, "fatal")
                End Try

                Return False
            End Function

        End Class

    End Class

End Namespace