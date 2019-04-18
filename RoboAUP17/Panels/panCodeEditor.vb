﻿Imports ScintillaNET
Public Class panCodeEditor
    ' -----------------------------------------------------------------------------
    ' TODO
    ' -----------------------------------------------------------------------------
    ' Syntax Highlighting
    ' Rückgängig / Wiederholen Funktion

    Private _lastHighlightedLineIndex As Int32 = 0
    Private _maxLineNumberCharLength As Int32

    ' -----------------------------------------------------------------------------
    ' Init Panel
    ' -----------------------------------------------------------------------------
    Private Sub panCodeEditor_Load(sender As Object, e As EventArgs) Handles Me.Load
        sciCodeEditor.Margins(0).Width = sciCodeEditor.TextWidth(Style.LineNumber, "99")
        sciCodeEditor.Styles(Style.Default).Font = "Courier New"
        sciCodeEditor.Styles(Style.Default).Size = 12

        sciCodeEditor.Markers(0).Symbol = MarkerSymbol.Arrow
        sciCodeEditor.Markers(0).SetBackColor(Color.Yellow)
        sciCodeEditor.Markers(1).Symbol = MarkerSymbol.Background
        sciCodeEditor.Markers(1).SetBackColor(Color.LightGoldenrodYellow)
        sciCodeEditor.Markers(2).Symbol = MarkerSymbol.Background
        sciCodeEditor.Markers(2).SetBackColor(Color.IndianRed)

        AddHandler frmMain.ACLProgram.ProgramStarted, AddressOf _eProgramStarted
        AddHandler frmMain.ACLProgram.ProgramFinished, AddressOf _eProgramFinished
        AddHandler frmMain.ACLProgram.ProgramLineChanged, AddressOf _eProgramLineChanged
        AddHandler frmMain.ACLProgram.ErrorLine, AddressOf _eErrorLineEvent
    End Sub

    ' -----------------------------------------------------------------------------
    ' Form Control
    ' -----------------------------------------------------------------------------
    Private Sub sciCodeEditor_TextChanged(sender As Object, e As EventArgs) Handles sciCodeEditor.TextChanged
        ' Calculate Line Number Width
        Dim maxLineNumberCharLength = sciCodeEditor.Lines.Count.ToString().Length
        If (maxLineNumberCharLength = Me._maxLineNumberCharLength) Then
            Return
        End If
        sciCodeEditor.Margins(0).Width = sciCodeEditor.TextWidth(ScintillaNET.Style.LineNumber, New String("9"c, maxLineNumberCharLength + 1))
        Me._maxLineNumberCharLength = maxLineNumberCharLength

        ' Remove error marker
        _removeMarker()
    End Sub

    Private Sub sciCodeEditor_TextEdited(sender As Object, e As EventArgs) Handles sciCodeEditor.CharAdded, sciCodeEditor.Delete
        ' Remove error marker
        _removeMarker()
    End Sub

    ' -----------------------------------------------------------------------------
    ' Private
    ' -----------------------------------------------------------------------------
    Private Sub _removeMarker()
        For i = 0 To sciCodeEditor.Lines.Count - 1
            sciCodeEditor.Lines(i).MarkerDelete(2)
        Next
    End Sub

    ' -----------------------------------------------------------------------------
    ' Events
    ' -----------------------------------------------------------------------------
    Private Sub _eProgramStarted()
        If InvokeRequired Then
            Invoke(Sub() _eProgramStarted())
            Return
        End If
        sciCodeEditor.ReadOnly = True
    End Sub
    Private Sub _eProgramFinished()
        If InvokeRequired Then
            Invoke(Sub() _eProgramFinished())
            Return
        End If

        sciCodeEditor.Lines(_lastHighlightedLineIndex).MarkerDelete(0)
        sciCodeEditor.Lines(_lastHighlightedLineIndex).MarkerDelete(1)
        sciCodeEditor.ReadOnly = False
    End Sub
    Private Sub _eProgramLineChanged(line As Int32)
        If InvokeRequired Then
            Invoke(Sub() _eProgramLineChanged(line))
            Return
        End If

        sciCodeEditor.Lines(_lastHighlightedLineIndex).MarkerDelete(0)
        sciCodeEditor.Lines(_lastHighlightedLineIndex).MarkerDelete(1)
        _lastHighlightedLineIndex = line - 1
        sciCodeEditor.Lines(_lastHighlightedLineIndex).MarkerAdd(0)
        sciCodeEditor.Lines(_lastHighlightedLineIndex).MarkerAdd(1)
    End Sub

    Private Sub _eErrorLineEvent(line As Int32)
        If InvokeRequired Then
            Invoke(Sub() _eErrorLineEvent(line))
            Return
        End If

        sciCodeEditor.Lines(line - 1).MarkerAdd(2)
    End Sub
End Class