﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class _PrivateKey
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla mediante l'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.userWalletKeyPrivate = New CHCSupportUIControls.WalletKeyPrivate()
        Me.confirmButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'userWalletKeyPrivate
        '
        Me.userWalletKeyPrivate.keyStoreEnabled = False
        Me.userWalletKeyPrivate.Location = New System.Drawing.Point(1, 3)
        Me.userWalletKeyPrivate.Name = "userWalletKeyPrivate"
        Me.userWalletKeyPrivate.Size = New System.Drawing.Size(509, 99)
        Me.userWalletKeyPrivate.TabIndex = 0
        Me.userWalletKeyPrivate.value = ""
        '
        'confirmButton
        '
        Me.confirmButton.Location = New System.Drawing.Point(194, 98)
        Me.confirmButton.Name = "confirmButton"
        Me.confirmButton.Size = New System.Drawing.Size(75, 38)
        Me.confirmButton.TabIndex = 1
        Me.confirmButton.Text = "Confirm"
        Me.confirmButton.UseVisualStyleBackColor = True
        '
        '_PrivateKey
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(510, 138)
        Me.Controls.Add(Me.confirmButton)
        Me.Controls.Add(Me.userWalletKeyPrivate)
        Me.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "_PrivateKey"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Request User Data"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents userWalletKeyPrivate As CHCSupportUIControls.WalletKeyPrivate
    Friend WithEvents confirmButton As Button
End Class