﻿Public Class TCPVariables
    Private WithEvents _tcpCom As New TCPCommunication
    Private _variables As New Dictionary(Of String, Integer)

    Friend Event Connected()
    Friend Event Disconnected()
    Friend Event ConnectError()
    Friend Event VariableChanged(ByVal name As String, ByVal val As Integer)

    Friend Function Listen(port As Integer) As Boolean
        Return _tcpCom.Listen(port)
    End Function
    Friend Function Connect(ip As Net.IPAddress, port As Integer) As Boolean
        Return _tcpCom.Connect(ip, port)
    End Function
    Friend Function Connect(host As String, port As Integer) As Boolean
        Return _tcpCom.Connect(host, port)
    End Function
    Friend Sub TerminateConnection()
        _tcpCom.Terminate()
    End Sub

    Friend Function AddVariable(name As String) As Boolean
        If _variables.ContainsKey(name) Then Return False
        _variables.Add(name, 0)
        Return True
    End Function
    Friend Function RemoveVariable(name As String) As Boolean
        If Not _variables.ContainsKey(name) Then Return False
        _variables.Remove(name)
        Return True
    End Function
    Friend Function SetVariable(name As String, val As Integer) As Boolean
        If Not _variables.ContainsKey(name) Then Return False
        _variables(name) = val
        _tcpCom.Send($"{name};{val}")
        Return True
    End Function
    Friend Function GetVariable(name As String, ByRef val As Integer) As Boolean
        If Not _variables.ContainsKey(name) Then Return False
        val = _variables(name)
        Return True
    End Function

    Friend Function GetGridViewDataSource() As Object
        Dim _varDataArray = From row In _variables Select New With {.Variable = row.Key, .Wert = row.Value}
        Return _varDataArray.ToArray
    End Function


    Private Sub _eMsgReceived(msg As String) Handles _tcpCom.MessageReceived
        Dim tmpSplit() As String = msg.Split(";"c)
        If tmpSplit.Length = 2 Then
            Dim name As String = tmpSplit(0)
            If _variables.ContainsKey(name) Then
                Dim val As Integer
                If Integer.TryParse(tmpSplit(1), val) Then
                    ' Set Variable
                    _variables(name) = val
                    ' Raise Event
                    RaiseEvent VariableChanged(name, val)
                End If
            End If
        End If
    End Sub
    Private Sub _eConnected() Handles _tcpCom.Connected, _tcpCom.ListenerClientConnected
        RaiseEvent Connected()
    End Sub
    Private Sub _eDisconnected() Handles _tcpCom.Disconnected, _tcpCom.ListenerClientDisconnected
        RaiseEvent Disconnected()
    End Sub
    Private Sub _eConnectError() Handles _tcpCom.ConnectError
        RaiseEvent ConnectError()
    End Sub
End Class