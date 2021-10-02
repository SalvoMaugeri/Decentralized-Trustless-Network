﻿Option Compare Text
Option Explicit On

Imports CHCCommonLibrary.Support
Imports CHCCommonLibrary.AreaEngine.DataFileManagement.XML
Imports CHCCommonLibrary.AreaEngine.Encryption




Namespace AreaProtocol

    Public Class A0x3

        Public Class PrimaryAssetFile

            Inherits BaseFile(Of CHCProtocolLibrary.AreaCommon.Models.Network.AssetModel)

        End Class

        Public Class RequestModel
            Public Property requestCode As String = "A0x3"

            Public Property requestDateTimeStamp As Double = 0
            Public Property publicWalletAddressRequester As String = ""
            Public Property requestHash As String = ""
            Public Property signature As String = ""

            Public Property primaryAsset As New CHCProtocolLibrary.AreaCommon.Models.Network.AssetModel

            Public Overrides Function toString() As String
                Dim tmp As String = ""

                tmp += MyBase.toString()
                tmp += primaryAsset.toString()

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
                    proceed = AreaCommon.state.runtimeState.addProperty(AreaState.ChainStateEngine.PropertyID.assetData, "", transactionChainRecord.recordCoordinate, transactionChainRecord.recordHash, hashContent, False)
                End If

                If proceed Then
                    With AreaCommon.state.runtimeState.activeNetwork.primaryAssetData.value
                        .burnable = value.primaryAsset.burnable
                        .digit = value.primaryAsset.digit
                        .stakeable = value.primaryAsset.stakeable
                        .name = value.primaryAsset.name
                        .nameUnit = value.primaryAsset.nameUnit
                        .prestake = value.primaryAsset.prestake
                        .qtaInitialStake = value.primaryAsset.qtaInitialStake
                        .qtaTotal = value.primaryAsset.qtaTotal
                        .shortName = value.primaryAsset.shortName
                        .stakeable = value.primaryAsset.stakeable
                        .symbol = value.primaryAsset.symbol
                    End With
                End If

                Return proceed
            End Function

            Public Shared Function fromTransactionLedger(ByVal statePath As String, ByRef data As TransactionChainLibrary.AreaLedger.LedgerEngine.SingleRecordLedger) As Boolean
                Try
                    Dim engine As New PrimaryAssetFile

                    engine.fileName = IO.Path.Combine(statePath, "Contents", data.detailInformation & ".content")

                    If engine.read() Then
                        AreaCommon.state.runtimeState.activeNetwork.primaryAssetData.value = engine.data
                    End If

                    engine.data = Nothing

                    engine = Nothing

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


            Private Function writeDataContent(ByVal contentStatePath As String, ByRef primaryAsset As CHCProtocolLibrary.AreaCommon.Models.Network.AssetModel, ByVal primaryAssetHash As String) As Boolean
                Try
                    Dim engine As New PrimaryAssetFile

                    engine.data = primaryAsset

                    engine.fileName = IO.Path.Combine(contentStatePath, primaryAssetHash & ".content")

                    Return engine.save()
                Catch ex As Exception
                    currentService.currentAction.setError(Err.Number, ex.Message)

                    log.track("A0x3Manager.init", ex.Message, "fatal")

                    Return False
                End Try
            End Function

            Private Function writeDataIntoLedger(ByVal contentStatePath As String, ByRef hashContent As String) As CHCCommonLibrary.AreaCommon.Models.General.IdentifyRecordLedger
                Try
                    With AreaCommon.state.currentBlockLedger.currentRecord
                        .actionCode = "a0x3"
                        .registrationDate = CHCCommonLibrary.AreaEngine.Miscellaneous.timestampFromDateTime()
                        .detailInformation = HashSHA.generateSHA256(data.primaryAsset.getHash())
                        .requesterPublicAddress = data.publicWalletAddressRequester
                        .requestHash = data.requestHash
                    End With

                    hashContent = AreaCommon.state.currentBlockLedger.currentRecord.detailInformation

                    writeDataContent(contentStatePath, data.primaryAsset, hashContent)

                    If AreaCommon.state.currentBlockLedger.BlockComplete() Then
                        Return AreaCommon.state.currentBlockLedger.saveAndClean()
                    End If
                Catch ex As Exception
                    currentService.currentAction.setError(Err.Number, ex.Message)

                    log.track("A0x3Manager.init", ex.Message, "fatal")
                End Try

                Return New CHCCommonLibrary.AreaCommon.Models.General.IdentifyRecordLedger
            End Function


            Public Function init(ByRef paths As CHCProtocolLibrary.AreaSystem.VirtualPathEngine, ByVal primaryAsset As CHCProtocolLibrary.AreaCommon.Models.Network.AssetModel, ByVal publicWalletIdAddress As String, ByVal privateKeyRAW As String) As Boolean
                Try
                    Dim requestFileEngine As New FileEngine
                    Dim ledgerCoordinate As CHCCommonLibrary.AreaCommon.Models.General.IdentifyRecordLedger
                    Dim hashContent As String

                    log.track("A0x3Manager.init", "Begin")

                    currentService.currentAction.setAction("1x0004", "BuildManager - A0x3 - A0x3Manager")

                    If currentService.requestCancelCurrentRunCommand Then Return False

                    data.primaryAsset = primaryAsset
                    data.publicWalletAddressRequester = publicWalletIdAddress
                    data.requestDateTimeStamp = CHCCommonLibrary.AreaEngine.Miscellaneous.timestampFromDateTime()
                    data.requestHash = data.getHash
                    data.signature = CHCProtocolLibrary.AreaWallet.Support.WalletAddressEngine.createSignature(privateKeyRAW, data.requestHash)

                    requestFileEngine.data = data

                    requestFileEngine.fileName = IO.Path.Combine(AreaCommon.paths.workData.currentVolume.requests, data.requestHash & ".request")

                    If requestFileEngine.save() Then
                        log.track("A0x3Manager.init", "request - Saved")

                        ledgerCoordinate = writeDataIntoLedger(paths.workData.state.contents, hashContent)

                        If (ledgerCoordinate.recordCoordinate.Length = 0) Then
                            currentService.currentAction.setError("-1", "Error during update ledger")
                            currentService.currentAction.reset()

                            log.track("A0x3Manager.init", "Error: Error during update ledger", "fatal")

                            Return False
                        End If

                        log.track("A0x3Manager.init", "Ledger updated")

                        If Not RecoveryState.fromRequest(data, ledgerCoordinate, hashContent) Then
                            currentService.currentAction.setError("-1", "Error during update State")
                            currentService.currentAction.reset()

                            log.track("A0x3Manager.init", "Error: Error during update State", "fatal")

                            Return False
                        End If

                        log.track("A0x3Manager.init", "State updated")

                        Return True
                    End If
                Catch ex As Exception
                    currentService.currentAction.setError(Err.Number, ex.Message)

                    log.track("A0x3Manager.init", ex.Message, "fatal")
                End Try

                Return False
            End Function

        End Class

    End Class

End Namespace