﻿Imports RoboAUP17

Friend Class RobotControl
    ' -----------------------------------------------------------------------------
    ' TODO
    ' -----------------------------------------------------------------------------
    ' Kinematik einbinden

    ' -----------------------------------------------------------------------------
    ' Definitions
    ' -----------------------------------------------------------------------------
    Private Const _constRefMaxAngleBack As Int32 = 10 'Bei Referenzfahrt

    ' Movement
    Private _oSyncMov As New MovementCalculations
    Private _actV As Double
    Private _actA As Double

    ' Kinematik
    Private _kin As New Kinematics
    Private _kinInit As Boolean = False

    ' Parameter
    Private WithEvents _pref As New Settings
    ' Serial Communication
    Private WithEvents _com As New SerialCommunication

    Private _refOkay(5) As Boolean
    Private _posSteps(5) As Int32
    Private _posJoint As JointAngles
    Private _posCart As CartCoords
    Private _targetJoint As JointAngles


#Region "Properties"
    Friend ReadOnly Property Pref As Settings
        Get
            Return _pref
        End Get
    End Property
    Friend ReadOnly Property RefOkay As Boolean()
        Get
            Return _refOkay
        End Get
    End Property

    Friend ReadOnly Property PosSteps As Integer()
        Get
            Return _posSteps
        End Get
    End Property

    Friend ReadOnly Property PosJoint As JointAngles
        Get
            Return _posJoint
        End Get
    End Property

    Friend ReadOnly Property PosCart As CartCoords
        Get
            Return _posCart
        End Get
    End Property

    Friend ReadOnly Property TargetJoint As JointAngles
        Get
            Return _targetJoint
        End Get
    End Property
#End Region

    ' Events
    Friend Event SerialComPortChanged(ByVal ports As List(Of String))
    Friend Event SerialConnected()
    Friend Event SerialDisconnected()
    Friend Event Log(ByVal LogMsg As String, ByVal LogLvl As Logger.LogLevel)
    Friend Event RoboMoveFinished()
    Friend Event RoboMoveStarted()
    Friend Event RoboPositionChanged()
    Friend Event LimitSwitchStateChanged(ByVal lssState As Boolean())
    Friend Event EmergencyStopStateChanged(ByVal essState As Boolean)
    Friend Event RoboRefStateChanged(ByVal refState As Boolean())
    Friend Event RoboParameterChanged(ByVal joint As Boolean, ByVal servo As Boolean, ByVal dh As Boolean)

    ' -----------------------------------------------------------------------------
    ' Constructor
    ' -----------------------------------------------------------------------------
    Friend Sub New()
        _kin.setDenavitHartenbergParameter(_pref.DenavitHartenbergParameter)
        _kinInit = True
    End Sub
    ' -----------------------------------------------------------------------------
    ' Public
    ' -----------------------------------------------------------------------------
    'SERIAL
    Friend Sub SerialConnect(comPort As String)
        _com.Connect(comPort)
    End Sub
    Friend Sub SerialDisconnect()
        _com.Disconnect()
    End Sub
    'ROBOT MOVEMENTS
    Friend Sub SetSpeedAndAcc(speed As Double, acc As Double)
        _actV = speed
        _actA = acc
    End Sub
    ' Jog Angle
    Friend Function DoJog(nr As Int32, jogval As Double) As Boolean
        Dim tmpV(5) As Double
        Dim tmpA(5) As Double
        'aktuelle Geschwindigkeit und Beschleunigung berechnen
        _calcSpeedAcc(tmpV, tmpA)
        'Datensatz zusammenstellen
        _com.ClearMsgDataSend()
        Dim tmpNr As Int32 = nr - 1
        _targetJoint.SetByIndex(tmpNr, Constrain(_posJoint.Items(tmpNr) + jogval, _pref.JointParameter(tmpNr).MechMinAngle, _pref.JointParameter(tmpNr).MechMaxAngle))
        _com.AddMOVDataSet(True, nr, _calcTargetToSteps(_targetJoint.Items(tmpNr), nr), _calcSpeedAccToSteps(tmpV(tmpNr), nr), _calcSpeedAccToSteps(tmpA(tmpNr), nr), _calcSpeedAccToSteps(_pref.JointParameter(tmpNr).ProfileStopAcc, nr))
        'Telegramm senden
        If _com.SendMOV() Then
            RaiseEvent RoboMoveStarted()
            'Log
            RaiseEvent Log($"Moving Axis...", Logger.LogLevel.INFO)
            Return True
        Else Return False
        End If
    End Function
    ' Jog Steps
    Friend Function DoJog(nr As Int32, jogval As Int32) As Boolean
        'Jog steps
        Dim tmpJogval As Double = StepsToAngle(jogval, _pref.JointParameter(nr - 1).MotGear, _pref.JointParameter(nr - 1).MechGear, _pref.JointParameter(nr - 1).MotStepsPerRot << _pref.JointParameter(nr - 1).MotMode, 0)
        'Datensatz zusammenstellen
        Return DoJog(nr, tmpJogval)
    End Function

    Friend Function DoRef(J1 As Boolean, J2 As Boolean, J3 As Boolean, J4 As Boolean, J5 As Boolean, J6 As Boolean) As Boolean
        'Datensätze sammeln
        _com.ClearMsgDataSend()
        Dim enabled() As Boolean = {J1, J2, J3, J4, J5, J6}
        For i = 0 To 5
            Dim tmpMaxStepsBack As Int32 = AngleToSteps(_constRefMaxAngleBack, _pref.JointParameter(i).MotGear, _pref.JointParameter(i).MechGear, _pref.JointParameter(i).MotStepsPerRot << _pref.JointParameter(i).MotMode, 0)
            _com.AddREFDataSet(enabled(i), i + 1, _pref.JointParameter(i).CalDir Xor _pref.JointParameter(i).MotDir, _calcSpeedAccToSteps(_pref.JointParameter(i).CalSpeedFast, i + 1), _calcSpeedAccToSteps(_pref.JointParameter(i).CalSpeedSlow, i + 1), _calcSpeedAccToSteps(_pref.JointParameter(i).CalAcc, i + 1), tmpMaxStepsBack, _calcSpeedAccToSteps(_pref.JointParameter(i).ProfileStopAcc, i + 1))
        Next
        'Telegram senden
        If _com.SendREF() Then
            RaiseEvent RoboMoveStarted()
            'Log
            RaiseEvent Log($"Referenz läuft...", Logger.LogLevel.INFO)
            Return True
        Else Return False
        End If
    End Function
    Friend Function DoRef(joint As Int32) As Boolean
        Return DoRef(joint = 1, joint = 2, joint = 3, joint = 4, joint = 5, joint = 6)
    End Function

    Friend Function DoJointMov(sync As Boolean, J1_target As Double, J2_target As Double, J3_target As Double, J4_target As Double, J5_target As Double, J6_target As Double) As Boolean
        'Daten aufbereiten
        Dim tmpV(5) As Double
        Dim tmpA(5) As Double
        Dim atLeasOneJointMove As Boolean = False
        Dim enabled(5) As Boolean
        _targetJoint = New JointAngles(J1_target, J2_target, J3_target, J4_target, J5_target, J6_target)
        'Prüfen ob Ziel schon erreicht (keine Fahrt mehr notwendig)
        For i = 0 To 5
            If Math.Abs(_targetJoint.Items(i) - _posJoint.Items(i)) < 0.01 Then
                enabled(i) = False
                _targetJoint.SetByIndex(i, _posJoint.Items(i))
            Else
                enabled(i) = True
                atLeasOneJointMove = True
            End If
        Next
        If Not atLeasOneJointMove Then
            Return False
        End If
        'aktuelle Geschwindigkeit und Beschleunigung berechnen
        _calcSpeedAcc(tmpV, tmpA)
        'Synchronisierte Bewegung berechnen, falls gewünscht
        If sync Then
            For i = 0 To 5
                _oSyncMov.v_max(i) = tmpV(i)
                _oSyncMov.a_max(i) = tmpA(i)
                _oSyncMov.s(i) = If(enabled(i), Math.Abs(_targetJoint.Items(i) - _posJoint.Items(i)), 0)
            Next
            If Not _oSyncMov.calculate() Then
                RaiseEvent Log("[Robo Control] Berechnung für synchrone Bewegung fehlgeschlagen", Logger.LogLevel.ERR)
                _targetJoint = CType(_posJoint.Clone(), JointAngles) ' Ziel auf aktuelle Position setzen
            Else
                'Geschwindigkeit und Beschleunigung zuweisen
                Array.Copy(_oSyncMov.v, tmpV, _oSyncMov.v.Length)
                Array.Copy(_oSyncMov.a, tmpA, _oSyncMov.a.Length)
            End If
        End If
        'Datensätze sammeln
        _com.ClearMsgDataSend()
        For i = 0 To 5
            _com.AddMOVDataSet(enabled(i), i + 1, _calcTargetToSteps(_targetJoint.Items(i), i + 1), _calcSpeedAccToSteps(tmpV(i), i + 1), _calcSpeedAccToSteps(tmpA(i), i + 1), _calcSpeedAccToSteps(_pref.JointParameter(i).ProfileStopAcc, i + 1))
        Next
        'Telegram senden
        If _com.SendMOV() Then
            RaiseEvent RoboMoveStarted()
            'Log
            RaiseEvent Log("[Robo Control] Bewege Achsen...", Logger.LogLevel.INFO)
            Return True
        Else Return False
        End If
    End Function
    Friend Function DoHome(sync As Boolean) As Boolean
        Return DoJointMov(sync, _pref.JointParameter(0).MechHomePosAngle, _pref.JointParameter(1).MechHomePosAngle, _pref.JointParameter(2).MechHomePosAngle, _pref.JointParameter(3).MechHomePosAngle, _pref.JointParameter(4).MechHomePosAngle, _pref.JointParameter(5).MechHomePosAngle)
    End Function
    Friend Function DoPark(sync As Boolean) As Boolean
        Return DoJointMov(sync, _pref.JointParameter(0).MechParkPosAngle, _pref.JointParameter(1).MechParkPosAngle, _pref.JointParameter(2).MechParkPosAngle, _pref.JointParameter(3).MechParkPosAngle, _pref.JointParameter(4).MechParkPosAngle, _pref.JointParameter(5).MechParkPosAngle)
    End Function
    Friend Function MoveServo(srvNr As Int32, angle As Int32) As Boolean
        If _com.SendSRV(srvNr, angle) Then
            'Log
            RaiseEvent Log($"[Robo Control] Bewege Servo {srvNr}, Ziel: {angle}...", Logger.LogLevel.INFO)
            Return True
        Else Return False
        End If
    End Function

    Friend Sub FastStop()
        If _com.SendStop() Then RaiseEvent Log($"[Robo Control] Stoppbefehl gesendet", Logger.LogLevel.INFO)
    End Sub

    ' -----------------------------------------------------------------------------
    ' Private
    ' -----------------------------------------------------------------------------
    Private Sub _checkRefStateChange()
        Static refOkayOld(5) As Boolean
        For i = 0 To 5
            If _refOkay(i) <> refOkayOld(i) Then
                RaiseEvent RoboRefStateChanged(_refOkay)
                Exit For
            End If
        Next
        _refOkay.CopyTo(refOkayOld, 0)
    End Sub
    'CALCULATIONS
    Private Function _calcServoAngle(srvNr As Int32, prc As Double) As Int32
        Return CInt((_pref.ServoParameter(srvNr - 1).MaxAngle - _pref.ServoParameter(srvNr - 1).MinAngle) * (prc / 100.0) + _pref.ServoParameter(srvNr - 1).MinAngle)
    End Function
    Private Sub _calcSpeedAcc(ByRef v() As Double, ByRef a() As Double)
        'Maximale Geschwindigkeit und Beschleunigung jeder Achse ermitteln (Maxwert oder aktueller Wert, je nachdem welcher kleiner ist!)
        For i = 0 To 5
            If _pref.JointParameter(i).ProfileMaxSpeed < _actV Then
                v(i) = _pref.JointParameter(i).ProfileMaxSpeed
            Else
                v(i) = _actV
            End If

            If _pref.JointParameter(i).ProfileMaxAcc < _actA Then
                a(i) = _pref.JointParameter(i).ProfileMaxAcc
            Else
                a(i) = _actA
            End If
        Next
    End Sub

    Private Function _calcTargetToSteps(target As Double, nr As Int32) As Int32
        Dim tmpNr As Int32 = nr - 1
        Return If(_pref.JointParameter(tmpNr).MotDir = 1, -1, 1) * AngleToSteps(target, _pref.JointParameter(tmpNr).MotGear, _pref.JointParameter(tmpNr).MechGear, _pref.JointParameter(tmpNr).MotStepsPerRot << _pref.JointParameter(tmpNr).MotMode, If(_pref.JointParameter(tmpNr).CalDir = 0, _pref.JointParameter(tmpNr).MechMinAngle * -1, _pref.JointParameter(tmpNr).MechMaxAngle * -1))
    End Function

    Private Function _calcSpeedAccToSteps(speedAcc As Double, nr As Int32) As Int32
        Dim tmpNr As Int32 = nr - 1
        Return AngleToSteps(speedAcc, _pref.JointParameter(tmpNr).MotGear, _pref.JointParameter(tmpNr).MechGear, _pref.JointParameter(tmpNr).MotStepsPerRot, 0)
    End Function
    ' -----------------------------------------------------------------------------
    ' Events
    ' -----------------------------------------------------------------------------
    'Public Event ERRReceived(ByVal errnum As Int32)
    'Serial
    Private Sub _eSerialComPortChange(ports As List(Of String)) Handles _com.ComPortChange
        RaiseEvent SerialComPortChanged(ports)
    End Sub
    Private Sub _eSerialConnected() Handles _com.SerialConnected
        RaiseEvent SerialConnected()
    End Sub
    Private Sub _eSerialDisconnected() Handles _com.SerialDisconnected
        RaiseEvent SerialDisconnected()
    End Sub
    Private Sub _eLog(LogMsg As String, LogLvl As Logger.LogLevel) Handles _com.Log, _pref.Log
        'Log Events durchleiten
        RaiseEvent Log(LogMsg, LogLvl)
    End Sub
    Private Sub _eFINReceived() Handles _com.FINReceived
        RaiseEvent RoboMoveFinished()
    End Sub
    Private Sub _ePOSReceived(refOkay As Boolean(), posSteps As Int32()) Handles _com.POSReceived
        ' Steps speichern
        Array.Copy(posSteps, _posSteps, 6)
        ' Joint Winkel berechnen
        For i = 0 To 5
            _posJoint.SetByIndex(i, StepsToAngle(posSteps(i), _pref.JointParameter(i).MotGear, _pref.JointParameter(i).MechGear, _pref.JointParameter(i).MotStepsPerRot << _pref.JointParameter(i).MotMode, If(_pref.JointParameter(i).CalDir = 0, _pref.JointParameter(i).MechMinAngle * -1, _pref.JointParameter(i).MechMaxAngle * -1)))
        Next
        ' Kartesische Koordinaten berechnen (Vorwärtskinematik)
        If _kinInit Then
            _posCart = _kin.ForwardKin(_posJoint)
        End If
        ' Referenzstatus aktualisieren
        refOkay.CopyTo(_refOkay, 0)
        _checkRefStateChange()
        RaiseEvent RoboPositionChanged()
    End Sub
    Private Sub _eLSSReceived(lssState As Boolean()) Handles _com.LSSReceived
        RaiseEvent LimitSwitchStateChanged(lssState)
    End Sub
    Private Sub _eESSReceived(essState As Boolean) Handles _com.ESSReceived
        RaiseEvent EmergencyStopStateChanged(essState)
    End Sub
    Private Sub _eRESReceived() Handles _com.RESReceived
        For i = 0 To 5
            _refOkay(i) = False
        Next
        _checkRefStateChange()
    End Sub
    Private Sub _eRoboParameterChanged(joint As Boolean, servo As Boolean, dh As Boolean) Handles _pref.ParameterChanged
        If dh Then
            _kin.setDenavitHartenbergParameter(_pref.DenavitHartenbergParameter)
            _kinInit = True
        End If
        RaiseEvent RoboParameterChanged(joint, servo, dh)
    End Sub
End Class