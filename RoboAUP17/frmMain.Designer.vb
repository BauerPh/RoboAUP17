﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.MenuStrip = New System.Windows.Forms.MenuStrip()
        Me.DateiToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.msNew = New System.Windows.Forms.ToolStripMenuItem()
        Me.msOpen = New System.Windows.Forms.ToolStripMenuItem()
        Me.msSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.msSaveAs = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.msExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.BearbeitenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.msUndo = New System.Windows.Forms.ToolStripMenuItem()
        Me.msRedo = New System.Windows.Forms.ToolStripMenuItem()
        Me.AnsichtToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AnsichtSpeichernToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AnsichtLadenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.VariablenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TeachpunkteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ACLEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TeachboxToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EinstellungenToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.DenavitHartenbergParameterToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.TCPServerToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.HilfeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Aup17RoboAufGitHubToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.InfoÜberAup17RoboToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStrip = New System.Windows.Forms.ToolStrip()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsCbComPort = New System.Windows.Forms.ToolStripComboBox()
        Me.tsBtnDisconnect = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsBtnConnect = New System.Windows.Forms.ToolStripButton()
        Me.tsBtnSave = New System.Windows.Forms.ToolStripButton()
        Me.tsBtnOpen = New System.Windows.Forms.ToolStripButton()
        Me.RoboterStatusToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip.SuspendLayout()
        Me.MenuStrip.SuspendLayout()
        Me.ToolStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 647)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(1113, 22)
        Me.StatusStrip.TabIndex = 0
        Me.StatusStrip.Text = "StatusStrip"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(37, 17)
        Me.ToolStripStatusLabel1.Text = "Info..."
        '
        'MenuStrip
        '
        Me.MenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DateiToolStripMenuItem, Me.BearbeitenToolStripMenuItem, Me.AnsichtToolStripMenuItem, Me.EinstellungenToolStripMenuItem1, Me.HilfeToolStripMenuItem})
        Me.MenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip.Name = "MenuStrip"
        Me.MenuStrip.Size = New System.Drawing.Size(1113, 24)
        Me.MenuStrip.TabIndex = 1
        Me.MenuStrip.Text = "MenuStrip"
        '
        'DateiToolStripMenuItem
        '
        Me.DateiToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.msNew, Me.msOpen, Me.msSave, Me.msSaveAs, Me.ToolStripSeparator4, Me.msExit})
        Me.DateiToolStripMenuItem.Name = "DateiToolStripMenuItem"
        Me.DateiToolStripMenuItem.Size = New System.Drawing.Size(46, 20)
        Me.DateiToolStripMenuItem.Text = "Datei"
        '
        'msNew
        '
        Me.msNew.Image = CType(resources.GetObject("msNew.Image"), System.Drawing.Image)
        Me.msNew.Name = "msNew"
        Me.msNew.Size = New System.Drawing.Size(180, 22)
        Me.msNew.Text = "Neu"
        '
        'msOpen
        '
        Me.msOpen.Image = CType(resources.GetObject("msOpen.Image"), System.Drawing.Image)
        Me.msOpen.Name = "msOpen"
        Me.msOpen.Size = New System.Drawing.Size(180, 22)
        Me.msOpen.Text = "Öffnen"
        '
        'msSave
        '
        Me.msSave.Image = CType(resources.GetObject("msSave.Image"), System.Drawing.Image)
        Me.msSave.Name = "msSave"
        Me.msSave.Size = New System.Drawing.Size(180, 22)
        Me.msSave.Text = "Speichern"
        '
        'msSaveAs
        '
        Me.msSaveAs.Image = CType(resources.GetObject("msSaveAs.Image"), System.Drawing.Image)
        Me.msSaveAs.Name = "msSaveAs"
        Me.msSaveAs.Size = New System.Drawing.Size(180, 22)
        Me.msSaveAs.Text = "Speichern unter..."
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(177, 6)
        '
        'msExit
        '
        Me.msExit.Image = CType(resources.GetObject("msExit.Image"), System.Drawing.Image)
        Me.msExit.Name = "msExit"
        Me.msExit.Size = New System.Drawing.Size(180, 22)
        Me.msExit.Text = "Beenden"
        '
        'BearbeitenToolStripMenuItem
        '
        Me.BearbeitenToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.msUndo, Me.msRedo})
        Me.BearbeitenToolStripMenuItem.Name = "BearbeitenToolStripMenuItem"
        Me.BearbeitenToolStripMenuItem.Size = New System.Drawing.Size(75, 20)
        Me.BearbeitenToolStripMenuItem.Text = "Bearbeiten"
        '
        'msUndo
        '
        Me.msUndo.Image = CType(resources.GetObject("msUndo.Image"), System.Drawing.Image)
        Me.msUndo.Name = "msUndo"
        Me.msUndo.Size = New System.Drawing.Size(180, 22)
        Me.msUndo.Text = "Rückgängig"
        '
        'msRedo
        '
        Me.msRedo.Image = CType(resources.GetObject("msRedo.Image"), System.Drawing.Image)
        Me.msRedo.Name = "msRedo"
        Me.msRedo.Size = New System.Drawing.Size(180, 22)
        Me.msRedo.Text = "Wiederholen"
        '
        'AnsichtToolStripMenuItem
        '
        Me.AnsichtToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AnsichtSpeichernToolStripMenuItem, Me.AnsichtLadenToolStripMenuItem, Me.ToolStripSeparator3, Me.VariablenToolStripMenuItem, Me.TeachpunkteToolStripMenuItem, Me.ACLEditorToolStripMenuItem, Me.TeachboxToolStripMenuItem, Me.RoboterStatusToolStripMenuItem, Me.ToolsToolStripMenuItem})
        Me.AnsichtToolStripMenuItem.Name = "AnsichtToolStripMenuItem"
        Me.AnsichtToolStripMenuItem.Size = New System.Drawing.Size(59, 20)
        Me.AnsichtToolStripMenuItem.Text = "Ansicht"
        '
        'AnsichtSpeichernToolStripMenuItem
        '
        Me.AnsichtSpeichernToolStripMenuItem.Name = "AnsichtSpeichernToolStripMenuItem"
        Me.AnsichtSpeichernToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.AnsichtSpeichernToolStripMenuItem.Text = "Ansicht speichern..."
        '
        'AnsichtLadenToolStripMenuItem
        '
        Me.AnsichtLadenToolStripMenuItem.Name = "AnsichtLadenToolStripMenuItem"
        Me.AnsichtLadenToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.AnsichtLadenToolStripMenuItem.Text = "Ansicht laden..."
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(174, 6)
        '
        'VariablenToolStripMenuItem
        '
        Me.VariablenToolStripMenuItem.Name = "VariablenToolStripMenuItem"
        Me.VariablenToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.VariablenToolStripMenuItem.Text = "Variablen"
        '
        'TeachpunkteToolStripMenuItem
        '
        Me.TeachpunkteToolStripMenuItem.Name = "TeachpunkteToolStripMenuItem"
        Me.TeachpunkteToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.TeachpunkteToolStripMenuItem.Text = "Teachpunkte"
        '
        'ACLEditorToolStripMenuItem
        '
        Me.ACLEditorToolStripMenuItem.Name = "ACLEditorToolStripMenuItem"
        Me.ACLEditorToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.ACLEditorToolStripMenuItem.Text = "ACL-Editor"
        '
        'TeachboxToolStripMenuItem
        '
        Me.TeachboxToolStripMenuItem.Name = "TeachboxToolStripMenuItem"
        Me.TeachboxToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.TeachboxToolStripMenuItem.Text = "Teachbox"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.ToolsToolStripMenuItem.Text = "Tools"
        '
        'EinstellungenToolStripMenuItem1
        '
        Me.EinstellungenToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DenavitHartenbergParameterToolStripMenuItem, Me.ToolStripSeparator5, Me.TCPServerToolStripMenuItem1})
        Me.EinstellungenToolStripMenuItem1.Name = "EinstellungenToolStripMenuItem1"
        Me.EinstellungenToolStripMenuItem1.Size = New System.Drawing.Size(90, 20)
        Me.EinstellungenToolStripMenuItem1.Text = "Einstellungen"
        '
        'DenavitHartenbergParameterToolStripMenuItem
        '
        Me.DenavitHartenbergParameterToolStripMenuItem.Name = "DenavitHartenbergParameterToolStripMenuItem"
        Me.DenavitHartenbergParameterToolStripMenuItem.Size = New System.Drawing.Size(247, 22)
        Me.DenavitHartenbergParameterToolStripMenuItem.Text = "Denavit-Hartenberg-Parameter..."
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(244, 6)
        '
        'TCPServerToolStripMenuItem1
        '
        Me.TCPServerToolStripMenuItem1.Name = "TCPServerToolStripMenuItem1"
        Me.TCPServerToolStripMenuItem1.Size = New System.Drawing.Size(247, 22)
        Me.TCPServerToolStripMenuItem1.Text = "TCP-Server..."
        '
        'HilfeToolStripMenuItem
        '
        Me.HilfeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Aup17RoboAufGitHubToolStripMenuItem, Me.ToolStripSeparator6, Me.InfoÜberAup17RoboToolStripMenuItem})
        Me.HilfeToolStripMenuItem.Name = "HilfeToolStripMenuItem"
        Me.HilfeToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HilfeToolStripMenuItem.Text = "Hilfe"
        '
        'Aup17RoboAufGitHubToolStripMenuItem
        '
        Me.Aup17RoboAufGitHubToolStripMenuItem.Name = "Aup17RoboAufGitHubToolStripMenuItem"
        Me.Aup17RoboAufGitHubToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.Aup17RoboAufGitHubToolStripMenuItem.Text = "Aup17 Robo auf GitHub"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(197, 6)
        '
        'InfoÜberAup17RoboToolStripMenuItem
        '
        Me.InfoÜberAup17RoboToolStripMenuItem.Name = "InfoÜberAup17RoboToolStripMenuItem"
        Me.InfoÜberAup17RoboToolStripMenuItem.Size = New System.Drawing.Size(200, 22)
        Me.InfoÜberAup17RoboToolStripMenuItem.Text = "Informationen"
        '
        'ToolStrip
        '
        Me.ToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsBtnOpen, Me.tsBtnSave, Me.ToolStripSeparator1, Me.tsCbComPort, Me.tsBtnConnect, Me.tsBtnDisconnect, Me.ToolStripSeparator2})
        Me.ToolStrip.Location = New System.Drawing.Point(0, 24)
        Me.ToolStrip.Name = "ToolStrip"
        Me.ToolStrip.Size = New System.Drawing.Size(1113, 25)
        Me.ToolStrip.TabIndex = 2
        Me.ToolStrip.Text = "ToolStrip"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'tsCbComPort
        '
        Me.tsCbComPort.Name = "tsCbComPort"
        Me.tsCbComPort.Size = New System.Drawing.Size(121, 25)
        Me.tsCbComPort.Text = "COM-Port"
        '
        'tsBtnDisconnect
        '
        Me.tsBtnDisconnect.Image = CType(resources.GetObject("tsBtnDisconnect.Image"), System.Drawing.Image)
        Me.tsBtnDisconnect.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsBtnDisconnect.Name = "tsBtnDisconnect"
        Me.tsBtnDisconnect.Size = New System.Drawing.Size(70, 22)
        Me.tsBtnDisconnect.Text = "Trennen"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'tsBtnConnect
        '
        Me.tsBtnConnect.Image = CType(resources.GetObject("tsBtnConnect.Image"), System.Drawing.Image)
        Me.tsBtnConnect.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsBtnConnect.Name = "tsBtnConnect"
        Me.tsBtnConnect.Size = New System.Drawing.Size(80, 22)
        Me.tsBtnConnect.Text = "Verbinden"
        '
        'tsBtnSave
        '
        Me.tsBtnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsBtnSave.Image = CType(resources.GetObject("tsBtnSave.Image"), System.Drawing.Image)
        Me.tsBtnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsBtnSave.Name = "tsBtnSave"
        Me.tsBtnSave.Size = New System.Drawing.Size(23, 22)
        Me.tsBtnSave.Text = "Speichern"
        '
        'tsBtnOpen
        '
        Me.tsBtnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsBtnOpen.Image = CType(resources.GetObject("tsBtnOpen.Image"), System.Drawing.Image)
        Me.tsBtnOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsBtnOpen.Name = "tsBtnOpen"
        Me.tsBtnOpen.Size = New System.Drawing.Size(23, 22)
        Me.tsBtnOpen.Text = "Öffnen"
        '
        'RoboterStatusToolStripMenuItem
        '
        Me.RoboterStatusToolStripMenuItem.Name = "RoboterStatusToolStripMenuItem"
        Me.RoboterStatusToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.RoboterStatusToolStripMenuItem.Text = "Roboter Status"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1113, 669)
        Me.Controls.Add(Me.ToolStrip)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.MenuStrip)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.MenuStrip
        Me.Name = "frmMain"
        Me.Text = "Aup17 Robo v1.0"
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.MenuStrip.ResumeLayout(False)
        Me.MenuStrip.PerformLayout()
        Me.ToolStrip.ResumeLayout(False)
        Me.ToolStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents StatusStrip As StatusStrip
    Friend WithEvents MenuStrip As MenuStrip
    Friend WithEvents DateiToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BearbeitenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStrip As ToolStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents HilfeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents InfoÜberAup17RoboToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents tsCbComPort As ToolStripComboBox
    Friend WithEvents tsBtnDisconnect As ToolStripButton
    Friend WithEvents msOpen As ToolStripMenuItem
    Friend WithEvents msSave As ToolStripMenuItem
    Friend WithEvents msSaveAs As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents msNew As ToolStripMenuItem
    Friend WithEvents AnsichtToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AnsichtSpeichernToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AnsichtLadenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents VariablenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TeachpunkteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ACLEditorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EinstellungenToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents DenavitHartenbergParameterToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents msExit As ToolStripMenuItem
    Friend WithEvents msUndo As ToolStripMenuItem
    Friend WithEvents msRedo As ToolStripMenuItem
    Friend WithEvents TeachboxToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents TCPServerToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents Aup17RoboAufGitHubToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator6 As ToolStripSeparator
    Friend WithEvents tsBtnOpen As ToolStripButton
    Friend WithEvents tsBtnSave As ToolStripButton
    Friend WithEvents tsBtnConnect As ToolStripButton
    Friend WithEvents RoboterStatusToolStripMenuItem As ToolStripMenuItem
End Class
