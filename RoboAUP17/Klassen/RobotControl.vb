﻿Imports RoboAuP17

Friend Class RobotControl
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
    Private _allRefOkay As Boolean
    Private _posSteps(5) As Int32
    Private _posJoint As JointAngles
    Private _posCart As CartCoords
    Private _posServo(2) As Int32

    Private _delayRunning As Boolean


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

    Public ReadOnly Property AllRefOkay As Boolean
        Get
            Return _allRefOkay
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
    Friend ReadOnly Property PosServo As Double()
        Get
            Dim pos(2) As Double
            For i = 0 To 2
                pos(i) = _calcServoPrc(i + 1, _posServo(i))
                If pos(i) < 0 Then pos(i) = 0
                If pos(i) > 100 Then pos(i) = 100
            Next
            Return pos
        End Get
    End Property

#End Region

    ' Events
    Friend Event SerialComPortChanged(ByVal ports As List(Of String))
    Friend Event SerialConnected()
    Friend Event SerialDisconnected()
    Friend Event Log(ByVal LogMsg As String, ByVal LogLvl As Logger.LogLevel)
    Friend Event RoboBusy(ByVal busy As Boolean, ByVal delay As Boolean)
    Friend Event RoboPositionChanged()
    Friend Event RoboServoChanged()
    Friend Event LimitSwitchStateChanged(ByVal lssState As Boolean())
    Friend Event EmergencyStopStateChanged(ByVal essState As Boolean)
    Friend Event RoboRefStateChanged()
    Friend Event RoboParameterChanged(ByVal parameterChanged As Settings.ParameterChangedParameter)

    ' -----------------------------------------------------------------------------
    ' Constructor
    ' -----------------------------------------------------------------------------
    Friend Sub New()
        _kin.SetDenavitHartenbergParameter(_pref.DenavitHartenbergParameter)
        _kin.Toolframe = _pref.Toolframe
        _kin.Workframe = _pref.Workframe
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
    Friend Function CheckCartCoords(cartCoords As CartCoords) As Boolean
        Dim jointAngles As JointAngles = _kin.InversKin(cartCoords)
        'Check inverse Kin Ergebnis
        If Double.IsNaN(jointAngles.J1) Or Double.IsNaN(jointAngles.J2) Or Double.IsNaN(jointAngles.J3) Or Double.IsNaN(jointAngles.J4) Or Double.IsNaN(jointAngles.J5) Or Double.IsNaN(jointAngles.J6) Then
            Return False
        End If
        'Check Limits
        If Not _checkJointAngleLimits(jointAngles) Then
            Return False
        End If
        Return True
    End Function
    Friend Function CheckJointAngles(jointAngles As JointAngles) As Boolean
        'Check Limits
        Return _checkJointAngleLimits(jointAngles)
    End Function
    Friend Function DoJog(nr As Int32, jogval As Double) As Boolean
        ' Jog Angle
        Dim tmpV(5) As Double
        Dim tmpA(5) As Double
        'aktuelle Geschwindigkeit und Beschleunigung berechnen
        _calcSpeedAcc(tmpV, tmpA)
        'Datensatz zusammenstellen
        _com.ClearMsgDataSend()
        Dim tmpNr As Int32 = nr - 1
        Dim target As Double
        target = Constrain(_posJoint.Items(tmpNr) + jogval, _pref.JointParameter(tmpNr).MechMinAngle, _pref.JointParameter(tmpNr).MechMaxAngle)
        _com.AddMOVDataSet(True, nr, _calcTargetToSteps(target, nr), _calcSpeedAccToSteps(_pref.JointParameter(tmpNr).ProfileMinSpeed, nr), _calcSpeedAccToSteps(tmpV(tmpNr), nr), _calcSpeedAccToSteps(tmpA(tmpNr), nr), _calcSpeedAccToSteps(_pref.JointParameter(tmpNr).ProfileStopAcc, nr))
        'Telegramm senden
        If _com.SendMOV() Then
            RaiseEvent RoboBusy(True, False)
            'Log
            RaiseEvent Log("[Robo Control] Jogbefehl ausgeführt", Logger.LogLevel.INFO)
            Return True
        Else Return False
        End If
    End Function
    ' Jog Steps
    Friend Function DoJog(nr As Int32, jogval As Int32) As Boolean
        'Jog steps
        Dim index As Integer = nr - 1
        Dim tmpJogval As Double = StepsToAngle(jogval, _pref.JointParameter(index).MotGear, _pref.JointParameter(index).MechGear, _pref.JointParameter(index).MotStepsPerRot << _pref.JointParameter(index).MotMode, 0, _pref.JointParameter(index).MechMinAngle, _pref.JointParameter(index).MechMaxAngle)
        'Datensatz zusammenstellen
        Return DoJog(nr, tmpJogval)
    End Function
    ' Jog Cart
    Friend Function DoJogCart(nr As Int32, jogval As Double) As Boolean
        Dim cartCoords As New CartCoords
        cartCoords = _posCart
        Select Case nr
            Case 1
                cartCoords.X = cartCoords.X + jogval
            Case 2
                cartCoords.Y = cartCoords.Y + jogval
            Case 3
                cartCoords.Z = cartCoords.Z + jogval
            Case 4
                Dim newYaw = cartCoords.Yaw + jogval
                cartCoords.Yaw = If(newYaw > 180, newYaw - 360, If(newYaw < -180, newYaw + 360, newYaw))
            Case 5
                Dim newPitch = cartCoords.Pitch + jogval
                cartCoords.Pitch = If(newPitch > 180, newPitch - 360, If(newPitch < -180, newPitch + 360, newPitch))
            Case 6
                Dim newRoll = cartCoords.Roll + jogval
                cartCoords.Roll = If(newRoll > 180, newRoll - 360, If(newRoll < -180, newRoll + 360, newRoll))
            Case Else
                Return False
        End Select
        Return DoTCPMov(cartCoords)
    End Function

    Friend Function DoRef(J1 As Boolean, J2 As Boolean, J3 As Boolean, J4 As Boolean, J5 As Boolean, J6 As Boolean) As Boolean
        'Datensätze sammeln
        _com.ClearMsgDataSend()
        Dim enabled() As Boolean = {J1, J2, J3, J4, J5, J6}
        For i = 0 To 5
            Dim tmpMaxStepsBack As Int32 = AngleToSteps(_constRefMaxAngleBack, _pref.JointParameter(i).MotGear, _pref.JointParameter(i).MechGear, _pref.JointParameter(i).MotStepsPerRot << _pref.JointParameter(i).MotMode, 0)
            _com.AddREFDataSet(enabled(i), i + 1, _pref.JointParameter(i).CalDir Xor _pref.JointParameter(i).MotDir, _calcSpeedAccToSteps(_pref.JointParameter(i).ProfileMinSpeed, i + 1), _calcSpeedAccToSteps(_pref.JointParameter(i).CalSpeedFast, i + 1), _calcSpeedAccToSteps(_pref.JointParameter(i).CalSpeedSlow, i + 1), _calcSpeedAccToSteps(_pref.JointParameter(i).CalAcc, i + 1), tmpMaxStepsBack, _calcSpeedAccToSteps(_pref.JointParameter(i).ProfileStopAcc, i + 1))
        Next
        'Telegram senden
        If _com.SendREF() Then
            RaiseEvent RoboBusy(True, False)
            'Log
            RaiseEvent Log("[Robo Control] Referenzfahrt gestartet", Logger.LogLevel.INFO)
            Return True
        Else Return False
        End If
    End Function
    Friend Function DoRef(joint As Int32) As Boolean
        Return DoRef(joint = 1, joint = 2, joint = 3, joint = 4, joint = 5, joint = 6)
    End Function
    Friend Function DoTCPMov(cartCoords As CartCoords) As Boolean
        Dim jointAngles As JointAngles = _kin.InversKin(cartCoords)
        'Check inverse Kin Ergebnis
        If Double.IsNaN(jointAngles.J1) Or Double.IsNaN(jointAngles.J2) Or Double.IsNaN(jointAngles.J3) Or Double.IsNaN(jointAngles.J4) Or Double.IsNaN(jointAngles.J5) Or Double.IsNaN(jointAngles.J6) Then
            RaiseEvent Log("[Robo Control] Bewegung nicht möglich, Position nicht erreichbar", Logger.LogLevel.ERR)
            RaiseEvent RoboPositionChanged()
            Return False
        End If
        Return DoJointMov(True, jointAngles)
    End Function
    Friend Function DoTCPMov(X As Double, Y As Double, Z As Double, yaw As Double, pitch As Double, roll As Double) As Boolean
        Dim cartCoords As New CartCoords(X, Y, Z, yaw, pitch, roll)
        Return DoTCPMov(cartCoords)
    End Function

    Friend Function DoJointMov(sync As Boolean, jointAngles As JointAngles) As Boolean
        'Grenzwerte prüfen
        If Not _checkJointAngleLimits(jointAngles) Then
            RaiseEvent Log("[Robo Control] Bewegung nicht möglich, Achslimit erreicht", Logger.LogLevel.ERR)
            RaiseEvent RoboPositionChanged()
            Return False
        End If
        'Daten aufbereiten
        Dim tmpVmin(5) As Double
        Dim tmpV(5) As Double
        Dim tmpA(5) As Double
        Dim atLeasOneJointMove As Boolean = False
        Dim enabled(5) As Boolean
        Dim targetJoint = jointAngles
        'Prüfen ob Ziel schon erreicht (keine Fahrt mehr notwendig)
        For i = 0 To 5
            If Math.Abs(targetJoint.Items(i) - _posJoint.Items(i)) < 0.01 Then
                enabled(i) = False
                targetJoint.SetByIndex(i, _posJoint.Items(i))
            Else
                enabled(i) = True
                atLeasOneJointMove = True
            End If
        Next
        If Not atLeasOneJointMove Then
            RaiseEvent Log("[Robo Control] Bewegung wird nicht ausgeführt, Ziel wurde bereits erreicht", Logger.LogLevel.WARN)
            Return False
        End If
        'aktuelle Geschwindigkeit und Beschleunigung berechnen
        _calcSpeedAcc(tmpV, tmpA)
        'Synchronisierte Bewegung berechnen, falls gewünscht
        If sync Then
            For i = 0 To 5
                _oSyncMov.SetValues(i, tmpV(i), _pref.JointParameter(i).ProfileMinSpeed, tmpA(i))
                _oSyncMov.SetDistance(i, If(enabled(i), Math.Abs(targetJoint.Items(i) - _posJoint.Items(i)), 0))
            Next
            If Not _oSyncMov.Calculate() Then
                RaiseEvent Log("[Robo Control] Berechnung für synchrone Bewegung fehlgeschlagen", Logger.LogLevel.ERR)
                targetJoint = CType(_posJoint.Clone(), JointAngles) ' Ziel auf aktuelle Position setzen
            Else
                'Geschwindigkeit und Beschleunigung zuweisen
                tmpVmin = _oSyncMov.GetVmin
                tmpV = _oSyncMov.GetV
                tmpA = _oSyncMov.GetA
            End If
        End If
        'Datensätze sammeln
        _com.ClearMsgDataSend()
        For i = 0 To 5
            _com.AddMOVDataSet(enabled(i), i + 1, _calcTargetToSteps(targetJoint.Items(i), i + 1), _calcSpeedAccToSteps(_pref.JointParameter(i).ProfileMinSpeed, i + 1), _calcSpeedAccToSteps(tmpV(i), i + 1), _calcSpeedAccToSteps(tmpA(i), i + 1), _calcSpeedAccToSteps(_pref.JointParameter(i).ProfileStopAcc, i + 1))
        Next
        'Telegram senden
        If _com.SendMOV() Then
            RaiseEvent RoboBusy(True, False)
            'Log
            RaiseEvent Log("[Robo Control] Bewegungsbefehl gesendet", Logger.LogLevel.INFO)
            Return True
        Else Return False
        End If
    End Function
    Friend Function DoJointMov(sync As Boolean, J1_target As Double, J2_target As Double, J3_target As Double, J4_target As Double, J5_target As Double, J6_target As Double) As Boolean
        Dim jointAngles As New JointAngles(J1_target, J2_target, J3_target, J4_target, J5_target, J6_target)
        Return DoJointMov(sync, jointAngles)
    End Function
    Friend Function DoHome(sync As Boolean) As Boolean
        Return DoJointMov(sync, _pref.JointParameter(0).MechHomePosAngle, _pref.JointParameter(1).MechHomePosAngle, _pref.JointParameter(2).MechHomePosAngle, _pref.JointParameter(3).MechHomePosAngle, _pref.JointParameter(4).MechHomePosAngle, _pref.JointParameter(5).MechHomePosAngle)
    End Function
    Friend Function DoPark(sync As Boolean) As Boolean
        Return DoJointMov(sync, _pref.JointParameter(0).MechParkPosAngle, _pref.JointParameter(1).MechParkPosAngle, _pref.JointParameter(2).MechParkPosAngle, _pref.JointParameter(3).MechParkPosAngle, _pref.JointParameter(4).MechParkPosAngle, _pref.JointParameter(5).MechParkPosAngle)
    End Function
    Friend Function DoDelay(delay As Int32) As Boolean
        RaiseEvent RoboBusy(True, True)
        _delayRunning = True
        Return _com.SendWAI(delay)
    End Function
    Friend Function MoveServoAngle(srvNr As Int32, angle As Int32, speed As Int32) As Boolean
        If speed <= 0 Or speed > 100 Then speed = 100
        If _com.SendSRV(srvNr, angle, speed) Then
            RaiseEvent RoboBusy(True, False)
            'Log
            RaiseEvent Log($"[Robo Control] Bewege Servo {srvNr}, Ziel: {angle}, Geschwindigkeit: {speed}%", Logger.LogLevel.INFO)
            Return True
        Else
            RaiseEvent Log($"[Robo Control] Servobewegung fehlgeschlagen", Logger.LogLevel.ERR)
            Return False
        End If
    End Function
    Friend Function MoveServoPrc(srvNr As Int32, prc As Double, speed As Int32) As Boolean
        Return MoveServoAngle(srvNr, _calcServoAngle(srvNr, prc), speed)
    End Function

    Friend Sub FastStop()
        If _com.SendStop() Then RaiseEvent Log("[Robo Control] Stoppbefehl gesendet", Logger.LogLevel.INFO)
    End Sub

    ' -----------------------------------------------------------------------------
    ' Private
    ' -----------------------------------------------------------------------------
    ' True = okay
    Private Function _checkJointAngleLimits(jointAngles As JointAngles) As Boolean
        Dim limit As Boolean = (jointAngles.J1 > _pref.JointParameter(0).MechMaxAngle Or
            jointAngles.J1 < _pref.JointParameter(0).MechMinAngle Or
            jointAngles.J2 > _pref.JointParameter(1).MechMaxAngle Or
            jointAngles.J2 < _pref.JointParameter(1).MechMinAngle Or
            jointAngles.J3 > _pref.JointParameter(2).MechMaxAngle Or
            jointAngles.J3 < _pref.JointParameter(2).MechMinAngle Or
            jointAngles.J4 > _pref.JointParameter(3).MechMaxAngle Or
            jointAngles.J4 < _pref.JointParameter(3).MechMinAngle Or
            jointAngles.J5 > _pref.JointParameter(4).MechMaxAngle Or
            jointAngles.J5 < _pref.JointParameter(4).MechMinAngle Or
            jointAngles.J6 > _pref.JointParameter(5).MechMaxAngle Or
            jointAngles.J6 < _pref.JointParameter(5).MechMinAngle)

        Return Not limit
    End Function
    Private Sub _checkRefState(Optional reset As Boolean = False)
        Static refOkayOld(5) As Boolean
        ' Reset Ref State
        If reset Then
            For i = 0 To 5
                refOkayOld(i) = False
            Next
            _allRefOkay = False
            Return
        End If

        ' Check all
        _allRefOkay = True
        For i = 0 To 5
            If Not _refOkay(i) Then
                _allRefOkay = False
                Exit For
            End If
        Next

        ' Check Ref State Changed
        For i = 0 To 5
            If _refOkay(i) <> refOkayOld(i) Then
                RaiseEvent RoboRefStateChanged()
                Exit For
            End If
        Next
        _refOkay.CopyTo(refOkayOld, 0)

    End Sub
    'CALCULATIONS
    Private Function _calcServoAngle(srvNr As Int32, prc As Double) As Int32
        Return CInt((_pref.ServoParameter(srvNr - 1).MaxAngle - _pref.ServoParameter(srvNr - 1).MinAngle) * (prc / 100.0) + _pref.ServoParameter(srvNr - 1).MinAngle)
    End Function
    Private Function _calcServoPrc(srvNr As Int32, angle As Int32) As Double
        Return (angle - _pref.ServoParameter(srvNr - 1).MinAngle) / (_pref.ServoParameter(srvNr - 1).MaxAngle - _pref.ServoParameter(srvNr - 1).MinAngle) * 100.0
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

    Private Function _calcStepsToJointAngle(steps As Int32, nr As Int32) As Double
        Dim tmpNr As Int32 = nr - 1
        Return StepsToAngle(If(_pref.JointParameter(tmpNr).MotDir = 1, -1, 1) * steps, _pref.JointParameter(tmpNr).MotGear, _pref.JointParameter(tmpNr).MechGear, _pref.JointParameter(tmpNr).MotStepsPerRot << _pref.JointParameter(tmpNr).MotMode, If(_pref.JointParameter(tmpNr).CalDir = 0, _pref.JointParameter(tmpNr).MechMinAngle * -1, _pref.JointParameter(tmpNr).MechMaxAngle * -1), _pref.JointParameter(tmpNr).MechMinAngle, _pref.JointParameter(tmpNr).MechMaxAngle)
    End Function

    Private Function _calcSpeedAccToSteps(speedAcc As Double, nr As Int32) As Int32
        Dim tmpNr As Int32 = nr - 1
        Return AngleToSteps(speedAcc, _pref.JointParameter(tmpNr).MotGear, _pref.JointParameter(tmpNr).MechGear, _pref.JointParameter(tmpNr).MotStepsPerRot, 0)
    End Function
    ' -----------------------------------------------------------------------------
    ' Events
    ' -----------------------------------------------------------------------------
    'Serial
    Private Sub _eSerialComPortChange(ports As List(Of String)) Handles _com.ComPortChange
        RaiseEvent SerialComPortChanged(ports)
    End Sub
    Private Sub _eSerialConnected() Handles _com.SerialConnected
        RaiseEvent SerialConnected()
    End Sub
    Private Sub _eSerialDisconnected() Handles _com.SerialDisconnected
        ' alten Ref Status reseten
        _checkRefState(True)
        RaiseEvent SerialDisconnected()
    End Sub
    Private Sub _eLog(LogMsg As String, LogLvl As Logger.LogLevel) Handles _com.Log, _pref.Log
        'Log Events durchleiten
        RaiseEvent Log(LogMsg, LogLvl)
    End Sub
    Private Sub _eFINReceived() Handles _com.FINReceived
        If Not _delayRunning Then
            RaiseEvent Log("[Robo Control] Bewegung abgeschlossen", Logger.LogLevel.INFO)
        End If
        RaiseEvent RoboBusy(False, _delayRunning)
        _delayRunning = False
    End Sub
    Private Sub _ePOSReceived(refOkay As Boolean(), posSteps As Int32()) Handles _com.POSReceived
        ' Steps speichern
        Array.Copy(posSteps, _posSteps, 6)
        ' Joint Winkel berechnen und Grenzwerte prüfen
        For i = 0 To 5
            _posJoint.SetByIndex(i, _calcStepsToJointAngle(_posSteps(i), i + 1))
            If _posJoint.Items(i) > _pref.JointParameter(i).MechMaxAngle Or _posJoint.Items(i) < _pref.JointParameter(i).MechMinAngle Then
                RaiseEvent Log($"[Robo Control] Achse {i + 1} außerhalb der Grenzwerte => Referenzfahrt erforderlich!", Logger.LogLevel.WARN)
                _posJoint.SetByIndex(i, _pref.JointParameter(i).MechMinAngle)
                refOkay(i) = False 'Parameter überschreiben!
            End If
        Next
        ' Referenzstatus aktualisieren
        refOkay.CopyTo(_refOkay, 0)
        _checkRefState()

        ' Kartesische Koordinaten berechnen (Vorwärtskinematik)
        If _kinInit And _allRefOkay Then
            _posCart = _kin.ForwardKin(_posJoint)
        Else
            _posCart.X = 0
            _posCart.Y = 0
            _posCart.Z = 0
            _posCart.Yaw = 0
            _posCart.Pitch = 0
            _posCart.Roll = 0
        End If

        RaiseEvent RoboPositionChanged()
    End Sub
    Private Sub _eSRVReceived(srvNr As Int32, srvVal As Int32) Handles _com.SRVReceived
        _posServo(srvNr - 1) = srvVal

        RaiseEvent RoboServoChanged()
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
        _checkRefState()
    End Sub
    Private Sub _eRoboParameterChanged(changedParameter As Settings.ParameterChangedParameter) Handles _pref.ParameterChanged
        Dim all As Boolean = (changedParameter = Settings.ParameterChangedParameter.All)
        If changedParameter = Settings.ParameterChangedParameter.DenavitHartenbergParameter Or all Then
            _kin.SetDenavitHartenbergParameter(_pref.DenavitHartenbergParameter)
            _kinInit = True
        End If
        If changedParameter = Settings.ParameterChangedParameter.Toolframe Or all Then
            _kin.Toolframe = _pref.Toolframe
        End If
        If changedParameter = Settings.ParameterChangedParameter.Workframe Or all Then
            _kin.Workframe = _pref.Workframe
        End If
        ' Kartesische Koordinaten berechnen (Vorwärtskinematik)
        If changedParameter = Settings.ParameterChangedParameter.DenavitHartenbergParameter Or
                changedParameter = Settings.ParameterChangedParameter.Toolframe Or
                changedParameter = Settings.ParameterChangedParameter.Workframe Or
                all Then
            _posCart = _kin.ForwardKin(_posJoint)
        End If
        RaiseEvent RoboParameterChanged(changedParameter)
    End Sub
    Private Sub _eERRReceived(err As Integer) Handles _com.ERRReceived
        If err = SerialCommunication.enumRoboError.ERR_NO_REF Then
            RaiseEvent Log("[Robo Control] Roboter nicht in Referenz", Logger.LogLevel.ERR)
            RaiseEvent RoboBusy(False, False)
        ElseIf err >= SerialCommunication.enumRoboError.ERR_REF_CANCELED Then
            Select Case err
                Case SerialCommunication.enumRoboError.ERR_REF_CANCELED
                    RaiseEvent Log("[Robo Control] Referenzfahrt vom Benutzer abgebrochen", Logger.LogLevel.ERR)
                Case SerialCommunication.enumRoboError.ERR_REF_FAILED_STEP1
                    RaiseEvent Log("[Robo Control] Referenzfahrt Schritt 1 fehlgeschlagen, Endschalter wurde nicht erreicht", Logger.LogLevel.ERR)
                Case SerialCommunication.enumRoboError.ERR_REF_FAILED_STEP2
                    RaiseEvent Log("[Robo Control] Referenzfahrt Schritt 2 fehlgeschlagen, Endschalter wurde nicht verlassen", Logger.LogLevel.ERR)
                Case SerialCommunication.enumRoboError.ERR_REF_FAILED_STEP3
                    RaiseEvent Log("[Robo Control] Referenzfahrt Schritt 3 fehlgeschlagen, Endschalter wurde nicht erreicht", Logger.LogLevel.ERR)
            End Select
            RaiseEvent RoboBusy(False, False)
        End If
        _delayRunning = False
    End Sub
End Class