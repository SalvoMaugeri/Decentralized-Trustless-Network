﻿Option Compare Text
Option Explicit On

Imports CHCCommonLibrary.AreaEngine.DataFileManagement.Json
Imports CHCCommonLibrary.AreaEngine.Encryption
Imports CHCPrimaryRuntimeService.AreaCommon.Models.Network.Request




Namespace AreaProtocol

    ''' <summary>
    ''' This class contain the masternode element data
    ''' </summary>
    Public Class NodeComplete

        Inherits RequestAddNewNode

        Public Property identityPublicAddress As String = ""
        Public Property startConnectionTimeStamp As Double = 0

    End Class

    ''' <summary>
    ''' This class contain the minimal data essential A1x9 
    ''' </summary>
    Public Class EssentialA1x9

        Public Property currentMasterNodeList As New List(Of NodeComplete)

        ''' <summary>
        ''' This method provide to reorder the list of the masternode
        ''' </summary>
        ''' <returns></returns>
        Public Function reorderList() As List(Of NodeComplete)
            Try
                Dim minMasterNode As NodeComplete
                Dim singleMasterNode As NodeComplete
                Dim copyOfList As New List(Of NodeComplete)
                Dim result As New List(Of NodeComplete)

                AreaCommon.log.track("EssentialA1x9.reorderList", "Begin")

                For Each singleMasterNode In currentMasterNodeList
                    copyOfList.Add(singleMasterNode)
                Next

                Do While (copyOfList.Count > 0)
                    If (copyOfList.Count = 1) Then
                        result.Add(copyOfList(0))

                        copyOfList.RemoveAt(0)
                    Else
                        minMasterNode = copyOfList(0)

                        For i As Integer = 1 To copyOfList.Count
                            singleMasterNode = copyOfList.ElementAt(i)

                            If (singleMasterNode.startConnectionTimeStamp < minMasterNode.startConnectionTimeStamp) Then
                                minMasterNode = copyOfList.ElementAt(i)
                            End If
                        Next

                        copyOfList.Remove(minMasterNode)
                        result.Add(minMasterNode)
                    End If
                Loop

                AreaCommon.log.track("EssentialA1x9.reorderList", "Completed")

                Return result
            Catch ex As Exception
                AreaCommon.log.track("EssentialA1x9.reorderList", ex.Message, "fatal")

                Return New List(Of NodeComplete)
            End Try
        End Function

        ''' <summary>
        ''' This method provide to convert into a string the element of the object
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function toString() As String
            Dim result As String = ""

            For Each singleNode In currentMasterNodeList
                result += singleNode.toString()
            Next

            Return result
        End Function

        ''' <summary>
        ''' This methdo provide to get an hash of the object
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function getHash() As String
            Return HashSHA.generateSHA256(Me.toString())
        End Function

    End Class

    ''' <summary>
    ''' This class contain all element to manage a A1x8 command
    ''' </summary>
    Public Class A1x9

        ''' <summary>
        ''' This class contain alla member of request model
        ''' </summary>
        Public Class RequestModel : Implements IRequestModel

            Public Property common As New CommonRequest Implements IRequestModel.common
            Public Property content As New EssentialA1x9

            ''' <summary>
            ''' This method provide to convert into a string the element of the object
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function toString() As String Implements IRequestModel.toString
                Dim tmp As String = common.toString()

                tmp += content.toString()

                Return tmp
            End Function

            ''' <summary>
            ''' This methdo provide to get an hash of the object
            ''' </summary>
            ''' <returns></returns>
            Public Function getHash() As String Implements IRequestModel.getHash
                Return HashSHA.generateSHA256(Me.toString())
            End Function

        End Class

        ''' <summary>
        ''' This class contain all element of a request response
        ''' </summary>
        Public Class RequestResponseModel

            Inherits CHCCommonLibrary.AreaCommon.Models.General.RemoteResponse : Implements IRequestModel

            Private _Base As New RequestModel

            Public Property common As CommonRequest Implements IRequestModel.common
                Get
                    Return _Base.common
                End Get
                Set(value As CommonRequest)
                    _Base.common = value
                End Set
            End Property
            Public Property content As EssentialA1x9
                Get
                    Return _Base.content
                End Get
                Set(value As EssentialA1x9)
                    _Base.content = value
                End Set
            End Property
            Public Overrides Property signature As String
                Get
                    Return MyBase.signature
                End Get
                Set(value As String)
                    MyBase.signature = value
                End Set
            End Property

            ''' <summary>
            ''' This method provide to create a string of an element of this object
            ''' </summary>
            ''' <returns></returns>
            Public Overrides Function toString() As String Implements IRequestModel.toString
                Return MyBase.ToString() & _Base.toString()
            End Function

            ''' <summary>
            ''' This method provide to get the hash of this object
            ''' </summary>
            ''' <returns></returns>
            Public Function getHash() As String Implements IRequestModel.getHash
                Return _Base.getHash()
            End Function

        End Class

        ''' <summary>
        ''' This class contain all static member to recovery state 
        ''' </summary>
        Public Class RecoveryState

            ''' <summary>
            ''' This method provide to update the state from a request
            ''' </summary>
            ''' <param name="value"></param>
            ''' <param name="transactionChainRecord"></param>
            ''' <returns></returns>
            Public Shared Function fromRequest(ByRef value As RequestModel, ByVal transactionChainRecord As CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction) As Boolean
                Try
                    Dim proceed As Boolean = True
                    Dim contentPath As String = AreaCommon.paths.workData.state.contents
                    Dim hashContent As String = HashSHA.generateSHA256(value.content.toString())
                    Dim completefileName As String = IO.Path.Combine(AreaCommon.paths.workData.state.contents, hashContent) & ".Content"

                    AreaCommon.log.track("RecoveryState.fromRequest", "Begin")

                    If proceed Then
                        proceed = AreaCommon.state.runTimeState.updateChainProperty(value.common.chainReferement, AreaCommon.DAO.DBChain.DetailPropertyID.lastNodeList, value.content.currentMasterNodeList, hashContent, transactionChainRecord)
                    End If
                    If proceed Then
                        If IO.File.Exists(completefileName) Then
                            IO.File.Delete(completefileName)
                        End If
                        proceed = Not IO.File.Exists(completefileName)
                    End If
                    If proceed Then
                        proceed = IOFast(Of EssentialA1x9).save(completefileName, value.content)
                    End If

                    AreaCommon.log.track("RecoveryState.fromRequest", "Completed")

                    Return proceed
                Catch ex As Exception
                    AreaCommon.log.track("RecoveryState.fromRequest", ex.Message, "fatal")

                    Return False
                End Try
            End Function

            Public Shared Function fromTransactionLedger(ByVal statePath As String, ByRef data As TransactionChainLibrary.AreaLedger.SingleTransactionLedger) As Boolean
                ''' TODO: A1x8 RecoveryState.fromTransactionLedger
            End Function

        End Class

        ''' <summary>
        ''' This class provides the static method to validate the request
        ''' </summary>
        Public Class FormalCheck

            ''' <summary>
            ''' This method provide to verify a formal request
            ''' </summary>
            ''' <param name="requestHash"></param>
            ''' <returns></returns>
            Shared Function verify(ByVal requestHash As String) As Nullable(Of Boolean)
                Try
                    Dim proceed As Boolean = True
                    Dim request As RequestModel = AreaCommon.flow.getActiveRequest(requestHash).data

                    AreaCommon.log.track("FormalCheck.verify", "Begin")

                    If proceed Then
                        proceed = (request.common.netWorkReferement.Length > 0)
                    End If
                    If proceed Then
                        proceed = (request.common.netWorkReferement.CompareTo(AreaCommon.state.runTimeState.activeNetwork.hash) = 0)
                    End If
                    If proceed Then
                        proceed = (request.common.chainReferement.Length > 0)
                    End If
                    If proceed Then
                        proceed = request.common.chainReferement.CompareTo(AreaCommon.state.runTimeState.activeChain.hash) = 0
                    End If
                    If proceed Then
                        proceed = (request.common.requestDateTimeStamp <= CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime())
                    End If
                    If proceed Then
                        proceed = (request.content.currentMasterNodeList.Count > 0)
                    End If
                    If proceed Then
                        proceed = CHCProtocolLibrary.AreaWallet.Support.WalletAddressEngine.SingleKeyPair.checkFormatPublicAddress(request.common.publicAddressRequester)
                    End If
                    If proceed Then
                        proceed = AreaSecurity.checkSignature(request.getHash, request.common.signature, request.common.publicAddressRequester)
                    End If

                    AreaCommon.log.track("FormalCheck.verify", "Completed")

                    Return proceed
                Catch ex As Exception
                    AreaCommon.log.track("FormalCheck.verify", ex.Message, "fatal")

                    Return Nothing
                End Try
            End Function

            ''' <summary>
            ''' This method provide to evaluate a request
            ''' </summary>
            ''' <param name="value"></param>
            ''' <returns></returns>
            Shared Function evaluate(ByRef value As AreaFlow.RequestExtended) As Boolean
                Try
                    Dim request As RequestModel = value.data

                    AreaCommon.log.track("FormalCheck.evaluate", "Begin")

                    If (request.common.requestDateTimeStamp <= CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime(Now.ToUniversalTime.AddDays(-1))) Then
                        value.evaluations.rejectedNote = "Request expired"
                        value.position.verify = AreaFlow.EnumOperationPosition.completeWithNegativeResult

                        Return True
                    End If
                    If Not AreaCommon.state.runTimeState.chainByHash.ContainsKey(request.common.chainReferement) Then
                        value.evaluations.rejectedNote = "Chain not exist"
                        value.position.verify = AreaFlow.EnumOperationPosition.completeWithNegativeResult

                        Return True
                    End If

                    ''' TODO: Test the close block if the masternode list is empty

                    value.position.verify = AreaFlow.EnumOperationPosition.completeWithPositiveResult

                    AreaCommon.log.track("FormalCheck.evaluate", "Completed")

                    Return True
                Catch ex As Exception
                    AreaCommon.log.track("FormalCheck.evaluate", ex.Message, "fatal")

                    Return False
                End Try
            End Function

        End Class

        ''' <summary>
        ''' This static class provides to static method to manage a request
        ''' </summary>
        Public Class Manager

            ''' <summary>
            ''' This method provide to write request into ledger
            ''' </summary>
            ''' <returns></returns>
            Shared Function addIntoLedger(ByVal approverPublicAddress As String, ByVal consensusHash As String, ByVal registrationTimeStamp As String, ByVal value As EssentialA1x9, ByVal requesterPublicAddress As String, ByVal requestHash As String) As CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction
                Try
                    Dim contentPath As String = AreaCommon.state.currentBlockLedger.proposeNewTransaction.pathData.contents
                    Dim hash As String = value.getHash()

                    AreaCommon.log.track("A1x9.Manager.addIntoLedger", "Begin")

                    contentPath = IO.Path.Combine(contentPath, hash & ".Content")

                    If IOFast(Of EssentialA1x9).save(contentPath, value) Then

                        With AreaCommon.state.currentBlockLedger.proposeNewTransaction
                            .type = "a1x9"
                            .approverPublicAddress = approverPublicAddress
                            .consensusHash = consensusHash
                            .detailInformation = hash
                            .registrationTimeStamp = registrationTimeStamp
                            .requesterPublicAddress = requesterPublicAddress
                            .requestHash = requestHash
                            .currentHash = .getHash
                        End With

                        Return AreaCommon.state.currentBlockLedger.saveAndClean()
                    Else
                        Return New CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction
                    End If
                Catch ex As Exception
                    AreaCommon.state.currentService.currentAction.setError(Err.Number, ex.Message)

                    AreaCommon.log.track("A1x9.Manager.addIntoLedger", ex.Message, "fatal")

                    Return New CHCCommonLibrary.AreaCommon.Models.General.IdentifyLastTransaction
                Finally
                    AreaCommon.log.track("A1x9.Manager.addIntoLedger", "Completed")
                End Try
            End Function

            ''' <summary>
            ''' This method provide to save a request into temporally position
            ''' </summary>
            ''' <param name="value"></param>
            ''' <returns></returns>
            Public Shared Function saveTemporallyRequest(ByRef value As RequestModel) As Boolean
                Try
                    Return IOFast(Of RequestModel).save(IO.Path.Combine(AreaCommon.paths.workData.requestData.received, value.getHash & ".request"), value)
                Catch ex As Exception
                    Return False
                End Try
            End Function

            ''' <summary>
            ''' This method provide to load a request from a repository
            ''' </summary>
            ''' <param name="hash"></param>
            ''' <returns></returns>
            Public Shared Function loadRequest(ByVal completePath As String, ByVal hash As String) As RequestModel
                Try
                    Return IOFast(Of RequestModel).read(IO.Path.Combine(completePath, hash & ".request"))
                Catch ex As Exception
                    Return New RequestModel
                End Try
            End Function

            ''' <summary>
            ''' This method provide to save a request into temporally position from RequestResponseModel
            ''' </summary>
            ''' <param name="value"></param>
            ''' <returns></returns>
            Public Shared Function saveTemporallyRequest(ByRef value As RequestResponseModel) As Boolean
                Return saveTemporallyRequest(value)
            End Function

            ''' <summary>
            ''' This method provide to create a initial procedure A1x9
            ''' </summary>
            ''' <returns></returns>
            Shared Function createInternalRequest() As String
                Try
                    Dim data As New RequestModel

                    AreaCommon.log.track("A1x9Manager.createInternalRequest", "Begin")

                    If AreaCommon.state.currentService.requestCancelCurrentRunCommand Then Return False

                    With AreaCommon.state.keys.key(TransactionChainLibrary.AreaEngine.KeyPair.KeysEngine.KeyPair.enumWalletType.identity)
                        data.content.currentMasterNodeList = AreaCommon.state.runTimeState.activeChain.originalNodeList.Values.ToList()
                        data.content.currentMasterNodeList = data.content.reorderList()

                        data.common.netWorkReferement = AreaCommon.state.runtimeState.activeNetwork.hash
                        data.common.chainReferement = AreaCommon.state.runtimeState.activeChain.hash
                        data.common.type = "a1x9"
                        data.common.publicAddressRequester = .publicAddress
                        data.common.requestDateTimeStamp = CHCCommonLibrary.AreaEngine.Miscellaneous.timeStampFromDateTime()
                        data.common.hash = data.getHash()
                        data.common.signature = CHCProtocolLibrary.AreaWallet.Support.WalletAddressEngine.createSignature(.privateKey, data.common.hash)
                    End With

                    If saveTemporallyRequest(data) Then
                        AreaCommon.log.track("A1x9Manager.createInternalRequest", "request - Saved")

                        If AreaCommon.flow.addNewRequestDirect(data) Then
                            Return data.common.hash
                        Else
                            Return ""
                        End If
                    End If
                Catch ex As Exception
                    AreaCommon.state.currentService.currentAction.setError(Err.Number, ex.Message)

                    AreaCommon.log.track("A1x9Manager.createInternalRequest", ex.Message, "fatal")
                Finally
                    AreaCommon.log.track("A1x9Manager.createInternalRequest", "Completed")
                End Try

                Return ""
            End Function

        End Class

    End Class

End Namespace