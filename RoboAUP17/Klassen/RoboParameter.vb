﻿Imports RoboAUP17

Friend Class RoboParameter
    ' -----------------------------------------------------------------------------
    ' TODO
    ' -----------------------------------------------------------------------------
    ' ???

    ' -----------------------------------------------------------------------------
    ' Definitions
    ' -----------------------------------------------------------------------------
    Private Const cDefaultConfigFile As String = "settings\RoboParameterDefault.xml"

    Private _actFilename As String
    Private _jointParameter(5) As JointParameter
    Private _servoParameter(2) As ServoParameter
    Private _denavitHartenbergParameter(5) As DHParameter

    ' Properties
    Friend ReadOnly Property JointParameter As JointParameter()
        Get
            Return _jointParameter
        End Get
    End Property
    Friend ReadOnly Property ServoParameter As ServoParameter()
        Get
            Return _servoParameter
        End Get
    End Property

    Friend ReadOnly Property DenavitHartenbergParameter As DHParameter()
        Get
            Return _denavitHartenbergParameter
        End Get
    End Property

    ' Events
    Friend Event ParameterChanged(ByVal joint As Boolean, ByVal servo As Boolean, ByVal dh As Boolean)
    Friend Event Log(ByVal LogMsg As String, ByVal LogLvl As Logger.LogLevel)

    ' -----------------------------------------------------------------------------
    ' Constructor
    ' -----------------------------------------------------------------------------
    Friend Sub New()
        'Lade letzte geöffnete Datei, wenn vorhanden!
        If _XMLReader(My.Settings.LastConfigFile) Then
            _actFilename = My.Settings.LastConfigFile
        Else
            MessageBox.Show($"Die Parameterdatei ""{My.Settings.LastConfigFile}"" konnte nicht geladen werden. Es werden die Standardparameter geladen.", "Parameterdatei nicht gefunden!", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            If Not LoadDefaulSettings() Then
                If MessageBox.Show($"Die Parameterdatei ""{cDefaultConfigFile}"" konnte nicht gefunden werden oder ist fehlerhaft. Soll sie gesucht werden?", "Parameterdatei nicht gefunden!",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Error) = DialogResult.Yes Then
                    LoadSettings()
                End If
            End If
        End If
    End Sub

    ' -----------------------------------------------------------------------------
    ' Public
    ' -----------------------------------------------------------------------------
    Friend Sub SetJointParameter(index As Integer, jointParameter As JointParameter)
        _jointParameter(index) = jointParameter
        RaiseEvent ParameterChanged(True, False, False)
    End Sub
    Friend Sub SetServoParameter(index As Integer, servoParameter As ServoParameter)
        _servoParameter(index) = servoParameter
        RaiseEvent ParameterChanged(False, True, False)
    End Sub
    Friend Sub SetDenavitHartenbergParameter(index As Integer, dhParameter As DHParameter)
        _denavitHartenbergParameter(index) = dhParameter
        RaiseEvent ParameterChanged(False, False, True)
    End Sub
    Friend Function GetDefaulConfigFilename() As String
        Return cDefaultConfigFile
    End Function
    Friend Function LoadDefaulSettings() As Boolean
        If System.IO.File.Exists(cDefaultConfigFile) Then
            _XMLReader(cDefaultConfigFile)
            My.Settings.LastConfigFile = cDefaultConfigFile
            My.Settings.Save()
            _actFilename = cDefaultConfigFile
            RaiseEvent ParameterChanged(True, True, True)
            RaiseEvent Log($"[Parameter] Standardparameter geladen", Logger.LogLevel.ERR)
            Return True
        Else
            Return False
        End If
    End Function
    Friend Function LoadSettings() As Boolean
        Dim openFileDialog As New OpenFileDialog With {
            .Filter = "Parameter-Dateien (*.xml)|*.xml"
        }
        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Try
                If Not _XMLReader(openFileDialog.FileName) Then Return False
                My.Settings.LastConfigFile = openFileDialog.FileName
                My.Settings.Save()
                _actFilename = openFileDialog.FileName
                RaiseEvent ParameterChanged(True, True, True)
                RaiseEvent Log("[Parameter] Parameterdatei geladen", Logger.LogLevel.INFO)
                Return True
            Catch e As Exception
                RaiseEvent Log($"[Parameter] Laden fehlgeschlagen, Fehler: {e.Message}", Logger.LogLevel.ERR)
                Return False
            End Try
        End If
        Return False
    End Function

    Friend Function SaveSettings() As Boolean
        Dim saveFileDialog As New SaveFileDialog With {
            .Filter = "Parameter-Dateien (*.xml)|*.xml"
        }
        If saveFileDialog.ShowDialog() = DialogResult.OK Then
            Try
                _XMLWriter(saveFileDialog.FileName)
                My.Settings.LastConfigFile = saveFileDialog.FileName
                My.Settings.Save()
                _actFilename = saveFileDialog.FileName
                RaiseEvent Log("[Parameter] Parameterdatei gespeichert", Logger.LogLevel.INFO)
                Return True
            Catch e As Exception
                RaiseEvent Log($"[Parameter] Speichern fehlgeschlagen, Fehler: {e.Message}", Logger.LogLevel.ERR)
                Return False
            End Try
        End If
        Return False
    End Function

    Friend Function GetActFilename() As String
        If _actFilename <> Nothing Then
            Return _actFilename
        Else
            Return ""
        End If
    End Function

    ' -----------------------------------------------------------------------------
    ' Private
    ' -----------------------------------------------------------------------------
    Private Sub _XMLWriter(filename As String)
        Dim enc As New System.Text.UnicodeEncoding
        Dim XMLTextWriter As Xml.XmlTextWriter = New Xml.XmlTextWriter(filename, enc)

        With XMLTextWriter
            .Formatting = Xml.Formatting.Indented
            .Indentation = 4
            .WriteStartDocument()
            .WriteStartElement("Settings")
            For i = 0 To 5
                .WriteStartElement("J" & (i + 1).ToString())

                .WriteStartElement("mot")
                .WriteAttributeString("stepsPerRot", _jointParameter(i).MotStepsPerRot.ToString)
                .WriteAttributeString("gear", _jointParameter(i).MotGear.ToString)
                .WriteAttributeString("mode", _jointParameter(i).MotMode.ToString)
                .WriteAttributeString("dir", _jointParameter(i).MotDir.ToString)
                .WriteEndElement()

                .WriteStartElement("mech")
                .WriteAttributeString("gear", _jointParameter(i).MechGear.ToString)
                .WriteAttributeString("minAngle", _jointParameter(i).MechMinAngle.ToString)
                .WriteAttributeString("maxAngle", _jointParameter(i).MechMaxAngle.ToString)
                .WriteAttributeString("homePosAngle", _jointParameter(i).MechHomePosAngle.ToString)
                .WriteAttributeString("parkPosAngle", _jointParameter(i).MechParkPosAngle.ToString)
                .WriteEndElement()

                .WriteStartElement("cal")
                .WriteAttributeString("dir", _jointParameter(i).CalDir.ToString)
                .WriteAttributeString("speedFast", _jointParameter(i).CalSpeedFast.ToString)
                .WriteAttributeString("speedSlow", _jointParameter(i).CalSpeedSlow.ToString)
                .WriteAttributeString("acc", _jointParameter(i).CalAcc.ToString)
                .WriteEndElement()

                .WriteStartElement("profile")
                .WriteAttributeString("maxSpeed", _jointParameter(i).ProfileMaxSpeed.ToString)
                .WriteAttributeString("maxAcc", _jointParameter(i).ProfileMaxAcc.ToString)
                .WriteAttributeString("stopAcc", _jointParameter(i).ProfileStopAcc.ToString)
                .WriteEndElement()

                .WriteEndElement()
            Next
            For i = 0 To 2
                .WriteStartElement("SRV" & (i + 1).ToString())

                .WriteStartElement("angles")
                .WriteAttributeString("minAngle", _servoParameter(i).MinAngle.ToString)
                .WriteAttributeString("maxAngle", _servoParameter(i).MaxAngle.ToString)
                .WriteEndElement()

                .WriteEndElement()
            Next
            .WriteEndElement()
            .Close()
        End With
    End Sub

    Private Function _XMLReader(filename As String) As Boolean
        'Prüfe ob Datei existiert
        If Not System.IO.File.Exists(filename) Then Return False

        Dim XMLReader As Xml.XmlReader = New Xml.XmlTextReader(filename)
        Dim i As Int32 'Index
        Dim joints As Boolean = False
        Dim valid As Boolean = False

        With XMLReader
            Do While .Read ' Es sind noch Daten vorhanden 
                If .NodeType = Xml.XmlNodeType.Element Then
                    Dim e As String = .Name 'Elementname
                    If e = "Settings" And Not valid Then valid = True
                    If Not valid Then
                        .Close()
                        RaiseEvent Log($"[Parameter] Dies ist keine korrekte Parameterdatei oder die Datei ist beschädigt", Logger.LogLevel.ERR)
                        Return False
                    End If
                    If e.Substring(0, 1) = "J" Then
                        i = CInt(e.Substring(1)) - 1 'Joint Nr
                        joints = True
                    ElseIf e.Substring(0, 3) = "SRV" Then
                        i = CInt(e.Substring(3)) - 1
                        joints = False
                    End If
                    If .AttributeCount > 0 Then 'sind überhaupt Attribute vorhanden?
                        While .MoveToNextAttribute 'Attribute durchlaufen
                            If joints Then
                                '********** JOINTS **********
                                Select Case e
                                    Case "mot"
                                        Select Case .Name
                                            Case "stepsPerRot"
                                                _jointParameter(i).MotStepsPerRot = CInt(.Value)
                                            Case "gear"
                                                _jointParameter(i).MotGear = CDec(.Value)
                                            Case "mode"
                                                _jointParameter(i).MotMode = CType([Enum].Parse(GetType(MotMode), .Value), MotMode)  'CType(CInt(.Value), motMode)
                                            Case "dir"
                                                _jointParameter(i).MotDir = CType([Enum].Parse(GetType(MotDir), .Value), MotDir)
                                        End Select
                                    Case "mech"
                                        Select Case .Name
                                            Case "gear"
                                                _jointParameter(i).MechGear = CDec(.Value)
                                            Case "minAngle"
                                                _jointParameter(i).MechMinAngle = CDec(.Value)
                                            Case "maxAngle"
                                                _jointParameter(i).MechMaxAngle = CDec(.Value)
                                            Case "homePosAngle"
                                                _jointParameter(i).MechHomePosAngle = CDec(.Value)
                                            Case "parkPosAngle"
                                                _jointParameter(i).MechParkPosAngle = CDec(.Value)
                                        End Select
                                    Case "cal"
                                        Select Case .Name
                                            Case "dir"
                                                _jointParameter(i).CalDir = CType([Enum].Parse(GetType(CalDir), .Value), CalDir)
                                            Case "speedFast"
                                                _jointParameter(i).CalSpeedFast = CDec(.Value)
                                            Case "speedSlow"
                                                _jointParameter(i).CalSpeedSlow = CDec(.Value)
                                            Case "acc"
                                                _jointParameter(i).CalAcc = CDec(.Value)
                                        End Select
                                    Case "profile"
                                        Select Case .Name
                                            Case "maxSpeed"
                                                _jointParameter(i).ProfileMaxSpeed = CDec(.Value)
                                            Case "maxAcc"
                                                _jointParameter(i).ProfileMaxAcc = CDec(.Value)
                                            Case "stopAcc"
                                                _jointParameter(i).ProfileStopAcc = CDec(.Value)
                                        End Select
                                End Select
                            Else
                                '********** SERVOS **********
                                Select Case e
                                    Case "angles"
                                        Select Case .Name
                                            Case "minAngle"
                                                _servoParameter(i).MinAngle = CDec(.Value)
                                            Case "maxAngle"
                                                _servoParameter(i).MaxAngle = CDec(.Value)
                                        End Select
                                End Select
                            End If
                        End While
                    End If
                End If
            Loop
            .Close()
        End With
        Return True
    End Function
End Class
