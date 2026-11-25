<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    Friend WithEvents lblMdb As System.Windows.Forms.Label
    Friend WithEvents txtMdbPath As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowseMdb As System.Windows.Forms.Button
    Friend WithEvents lblCsv As System.Windows.Forms.Label
    Friend WithEvents txtCsvPath As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowseCsv As System.Windows.Forms.Button
    Friend WithEvents grpStatus As System.Windows.Forms.GroupBox
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents progressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents btnExport As System.Windows.Forms.Button
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnOpenFolder As System.Windows.Forms.Button

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblMdb = New System.Windows.Forms.Label()
        Me.txtMdbPath = New System.Windows.Forms.TextBox()
        Me.btnBrowseMdb = New System.Windows.Forms.Button()
        Me.lblCsv = New System.Windows.Forms.Label()
        Me.txtCsvPath = New System.Windows.Forms.TextBox()
        Me.btnBrowseCsv = New System.Windows.Forms.Button()
        Me.grpStatus = New System.Windows.Forms.GroupBox()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.progressBar = New System.Windows.Forms.ProgressBar()
        Me.btnExport = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnOpenFolder = New System.Windows.Forms.Button()
        Me.grpStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblMdb
        '
        Me.lblMdb.AutoSize = True
        Me.lblMdb.Font = New System.Drawing.Font("メイリオ", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMdb.Location = New System.Drawing.Point(16, 20)
        Me.lblMdb.Name = "lblMdb"
        Me.lblMdb.Size = New System.Drawing.Size(95, 18)
        Me.lblMdb.TabIndex = 0
        Me.lblMdb.Text = "データベース(&D):"
        '
        'txtMdbPath
        '
        Me.txtMdbPath.Font = New System.Drawing.Font("メイリオ", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtMdbPath.Location = New System.Drawing.Point(120, 17)
        Me.txtMdbPath.Name = "txtMdbPath"
        Me.txtMdbPath.Size = New System.Drawing.Size(370, 25)
        Me.txtMdbPath.TabIndex = 1
        '
        'btnBrowseMdb
        '
        Me.btnBrowseMdb.Font = New System.Drawing.Font("メイリオ", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnBrowseMdb.Location = New System.Drawing.Point(496, 15)
        Me.btnBrowseMdb.Name = "btnBrowseMdb"
        Me.btnBrowseMdb.Size = New System.Drawing.Size(75, 28)
        Me.btnBrowseMdb.TabIndex = 2
        Me.btnBrowseMdb.Text = "参照..."
        Me.btnBrowseMdb.UseVisualStyleBackColor = True
        '
        'lblCsv
        '
        Me.lblCsv.AutoSize = True
        Me.lblCsv.Font = New System.Drawing.Font("メイリオ", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblCsv.Location = New System.Drawing.Point(16, 60)
        Me.lblCsv.Name = "lblCsv"
        Me.lblCsv.Size = New System.Drawing.Size(94, 18)
        Me.lblCsv.TabIndex = 3
        Me.lblCsv.Text = "出力先CSV(&C):"
        '
        'txtCsvPath
        '
        Me.txtCsvPath.Font = New System.Drawing.Font("メイリオ", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtCsvPath.Location = New System.Drawing.Point(120, 57)
        Me.txtCsvPath.Name = "txtCsvPath"
        Me.txtCsvPath.Size = New System.Drawing.Size(370, 25)
        Me.txtCsvPath.TabIndex = 4
        '
        'btnBrowseCsv
        '
        Me.btnBrowseCsv.Font = New System.Drawing.Font("メイリオ", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnBrowseCsv.Location = New System.Drawing.Point(496, 55)
        Me.btnBrowseCsv.Name = "btnBrowseCsv"
        Me.btnBrowseCsv.Size = New System.Drawing.Size(75, 28)
        Me.btnBrowseCsv.TabIndex = 5
        Me.btnBrowseCsv.Text = "参照..."
        Me.btnBrowseCsv.UseVisualStyleBackColor = True
        '
        'grpStatus
        '
        Me.grpStatus.Controls.Add(Me.lblStatus)
        Me.grpStatus.Controls.Add(Me.progressBar)
        Me.grpStatus.Font = New System.Drawing.Font("メイリオ", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpStatus.Location = New System.Drawing.Point(19, 100)
        Me.grpStatus.Name = "grpStatus"
        Me.grpStatus.Size = New System.Drawing.Size(552, 95)
        Me.grpStatus.TabIndex = 6
        Me.grpStatus.TabStop = False
        Me.grpStatus.Text = "処理状態"
        '
        'lblStatus
        '
        Me.lblStatus.AutoEllipsis = True
        Me.lblStatus.Location = New System.Drawing.Point(15, 23)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(522, 23)
        Me.lblStatus.TabIndex = 0
        Me.lblStatus.Text = "待機中..."
        '
        'progressBar
        '
        Me.progressBar.Location = New System.Drawing.Point(18, 52)
        Me.progressBar.Name = "progressBar"
        Me.progressBar.Size = New System.Drawing.Size(519, 25)
        Me.progressBar.TabIndex = 1
        '
        'btnExport
        '
        Me.btnExport.Font = New System.Drawing.Font("メイリオ", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnExport.Location = New System.Drawing.Point(160, 215)
        Me.btnExport.Name = "btnExport"
        Me.btnExport.Size = New System.Drawing.Size(140, 38)
        Me.btnExport.TabIndex = 7
        Me.btnExport.Text = "CSV出力開始(&E)"
        Me.btnExport.UseVisualStyleBackColor = True
        '
        'btnOpenFolder
        '
        Me.btnOpenFolder.Enabled = False
        Me.btnOpenFolder.Font = New System.Drawing.Font("メイリオ", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnOpenFolder.Location = New System.Drawing.Point(310, 215)
        Me.btnOpenFolder.Name = "btnOpenFolder"
        Me.btnOpenFolder.Size = New System.Drawing.Size(130, 38)
        Me.btnOpenFolder.TabIndex = 8
        Me.btnOpenFolder.Text = "保存先フォルダ開く"
        Me.btnOpenFolder.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Font = New System.Drawing.Font("メイリオ", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnClose.Location = New System.Drawing.Point(460, 215)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(100, 38)
        Me.btnClose.TabIndex = 9
        Me.btnClose.Text = "閉じる(&X)"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(588, 275)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnOpenFolder)
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.grpStatus)
        Me.Controls.Add(Me.btnBrowseCsv)
        Me.Controls.Add(Me.txtCsvPath)
        Me.Controls.Add(Me.lblCsv)
        Me.Controls.Add(Me.btnBrowseMdb)
        Me.Controls.Add(Me.txtMdbPath)
        Me.Controls.Add(Me.lblMdb)
        Me.Font = New System.Drawing.Font("メイリオ", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "カルテデータ CSV出力 (D_Karte Data_End Null)"
        Me.grpStatus.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
End Class
