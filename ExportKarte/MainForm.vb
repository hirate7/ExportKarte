Imports System
Imports System.IO
Imports System.Text
Imports System.Data.OleDb
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports System.Collections.Generic

Public Class MainForm

    ' MDB 固定パスワード
    Private Const MDB_PASSWORD As String = "4300365"

    ' 処理中フラグ
    Private _isProcessing As Boolean = False
    Private _lastSavedCsvPath As String = String.Empty

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' デフォルトのMDB相対パス: Kdata\JSY0.MDB
        txtMdbPath.Text = Path.Combine("Kdata", "JSY0.MDB")

        ' デフォルトのCSV出力先: デスクトップ\Karte_yyyyMMdd_HHmmss.csv
        Dim desktopPath As String = GetDesktopDirectory()
        Dim defaultFileName As String = String.Format("Karte_{0:yyyyMMdd_HHmmss}.csv", DateTime.Now)
        txtCsvPath.Text = Path.Combine(desktopPath, defaultFileName)

        lblStatus.Text = "「CSV出力開始」ボタンを押してください。"
    End Sub

    ''' <summary>
    ''' デスクトップフォルダのパスを確実に取得する
    ''' </summary>
    Private Function GetDesktopDirectory() As String
        Dim desktopPath As String = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
        If String.IsNullOrEmpty(desktopPath) Then
            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        End If
        If String.IsNullOrEmpty(desktopPath) Then
            Dim userProfile As String = Environment.GetEnvironmentVariable("USERPROFILE")
            If Not String.IsNullOrEmpty(userProfile) Then
                desktopPath = Path.Combine(userProfile, "Desktop")
            End If
        End If
        If String.IsNullOrEmpty(desktopPath) OrElse Not Directory.Exists(desktopPath) Then
            desktopPath = Application.StartupPath
        End If
        Return desktopPath
    End Function

    ''' <summary>
    ''' MDBファイル参照ボタン
    ''' </summary>
    Private Sub btnBrowseMdb_Click(sender As Object, e As EventArgs) Handles btnBrowseMdb.Click
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Access Database (*.mdb)|*.mdb|すべてのファイル (*.*)|*.*"
            ofd.Title = "JSY0.MDB ファイルを選択してください"
            
            Dim currentPath As String = ResolveFullPath(txtMdbPath.Text.Trim())
            If File.Exists(currentPath) Then
                ofd.InitialDirectory = Path.GetDirectoryName(currentPath)
                ofd.FileName = Path.GetFileName(currentPath)
            Else
                ofd.InitialDirectory = Application.StartupPath
            End If

            If ofd.ShowDialog(Me) = DialogResult.OK Then
                ' アプリ起動フォルダからの相対パスにできる場合は相対パスにする
                txtMdbPath.Text = MakeRelativePath(Application.StartupPath, ofd.FileName)
            End If
        End Using
    End Sub

    ''' <summary>
    ''' CSV保存先参照ボタン
    ''' </summary>
    Private Sub btnBrowseCsv_Click(sender As Object, e As EventArgs) Handles btnBrowseCsv.Click
        Using sfd As New SaveFileDialog()
            sfd.Filter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*"
            sfd.Title = "CSVの出力先を指定してください"
            
            Dim currentPath As String = ResolveFullPath(txtCsvPath.Text.Trim())
            If Not String.IsNullOrEmpty(currentPath) AndAlso Directory.Exists(Path.GetDirectoryName(currentPath)) Then
                sfd.InitialDirectory = Path.GetDirectoryName(currentPath)
                sfd.FileName = Path.GetFileName(currentPath)
            Else
                sfd.InitialDirectory = GetDesktopDirectory()
                sfd.FileName = String.Format("Karte_{0:yyyyMMdd_HHmmss}.csv", DateTime.Now)
            End If

            If sfd.ShowDialog(Me) = DialogResult.OK Then
                txtCsvPath.Text = sfd.FileName
            End If
        End Using
    End Sub

    ''' <summary>
    ''' CSV出力実行ボタン
    ''' </summary>
    Private Async Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        If _isProcessing Then Return

        Dim rawMdbPath As String = txtMdbPath.Text.Trim()
        Dim rawCsvPath As String = txtCsvPath.Text.Trim()

        If String.IsNullOrEmpty(rawMdbPath) Then
            MessageBox.Show("MDBファイルのパスを指定してください。", "入力確認", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtMdbPath.Focus()
            Return
        End If

        If String.IsNullOrEmpty(rawCsvPath) Then
            MessageBox.Show("CSVの出力先パスを指定してください。", "入力確認", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCsvPath.Focus()
            Return
        End If

        Dim mdbFullPath As String = ResolveFullPath(rawMdbPath)
        Dim csvFullPath As String = ResolveFullPath(rawCsvPath)

        If Not File.Exists(mdbFullPath) Then
            MessageBox.Show("指定されたMDBファイルが見つかりません:" & vbCrLf & mdbFullPath, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' 出力先ディレクトリが存在しない場合は作成
        Try
            Dim csvDir As String = Path.GetDirectoryName(csvFullPath)
            If Not String.IsNullOrEmpty(csvDir) AndAlso Not Directory.Exists(csvDir) Then
                Directory.CreateDirectory(csvDir)
            End If
        Catch ex As Exception
            MessageBox.Show("出力先フォルダの準備に失敗しました:" & vbCrLf & ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        ' UIコントロールの無効化
        SetUiState(False)
        _isProcessing = True
        btnOpenFolder.Enabled = False
        progressBar.Style = ProgressBarStyle.Marquee
        lblStatus.Text = "データベースに接続中..."

        Dim totalCount As Integer = 0
        Dim isLockError As Boolean = False
        Dim errorMsg As String = String.Empty

        Try
            ' 非同期でCSV出力処理を実行
            Dim progressHandler As New Progress(Of String)(Sub(statusText)
                                                              lblStatus.Text = statusText
                                                          End Sub)

            totalCount = Await Task.Run(Function() As Integer
                                            Return ExecuteExport(mdbFullPath, csvFullPath, progressHandler)
                                        End Function)

            _lastSavedCsvPath = csvFullPath
            lblStatus.Text = String.Format("出力完了: {0:#,##0} 件出力しました。", totalCount)
            progressBar.Style = ProgressBarStyle.Blocks
            progressBar.Value = 100
            btnOpenFolder.Enabled = True

            MessageBox.Show(String.Format("CSV出力が完了しました。" & vbCrLf & _
                                          "出力件数: {0:#,##0} 件" & vbCrLf & _
                                          "保存先: {1}", totalCount, csvFullPath), _
                            "完了", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As OleDbException
            ' 排他ロックエラーの判定
            If IsExclusiveLockException(ex) Then
                isLockError = True
            Else
                errorMsg = ex.Message
            End If
        Catch ex As Exception
            If IsExclusiveLockException(ex) Then
                isLockError = True
            Else
                errorMsg = ex.Message
            End If
        Finally
            _isProcessing = False
            SetUiState(True)
            If progressBar.Style = ProgressBarStyle.Marquee Then
                progressBar.Style = ProgressBarStyle.Blocks
                progressBar.Value = 0
            End If
        End Try

        ' 排他ロックエラーの場合、指定のメッセージを表示
        If isLockError Then
            lblStatus.Text = "他のアプリがデータベースを使用中のため処理を中断しました。"
            MessageBox.Show("他のアプリを終了させてください", "排他ロックエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        ElseIf Not String.IsNullOrEmpty(errorMsg) Then
            lblStatus.Text = "エラーが発生しました。"
            MessageBox.Show("処理中にエラーが発生しました:" & vbCrLf & errorMsg, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ''' <summary>
    ''' エクスプローラーで保存先フォルダを開く
    ''' </summary>
    Private Sub btnOpenFolder_Click(sender As Object, e As EventArgs) Handles btnOpenFolder.Click
        If Not String.IsNullOrEmpty(_lastSavedCsvPath) AndAlso File.Exists(_lastSavedCsvPath) Then
            Try
                Process.Start("explorer.exe", "/select,""" & _lastSavedCsvPath & """")
            Catch ex As Exception
                MessageBox.Show("フォルダを開けませんでした: " & ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' 終了ボタン
    ''' </summary>
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' CSV出力処理の実体
    ''' </summary>
    Private Function ExecuteExport(mdbPath As String, csvPath As String, progress As IProgress(Of String)) As Integer
        progress.Report("テーブルの標題情報を取得中...")

        ' 1. 各フィールドの「標題 (Caption)」を取得
        Dim captions As Dictionary(Of String, String) = GetFieldCaptions(mdbPath)

        progress.Report("データベースに接続中...")

        Dim connStr As String = String.Format( _
            "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Database Password={1};Mode=Share Deny None;Persist Security Info=False", _
            mdbPath, MDB_PASSWORD)

        Dim exportCount As Integer = 0

        Using conn As New OleDbConnection(connStr)
            conn.Open()

            progress.Report("保険者マスタ (D_Hoken) を読み込み中...")
            Dim hokenDict As Dictionary(Of String, String) = LoadHokenDictionary(conn)

            progress.Report("D_Karte テーブルからデータを抽出中...")

            ' D_Karte のうち Data_End が Null のデータを抽出
            Dim sql As String = "SELECT * FROM D_Karte WHERE Data_End IS NULL"

            Using cmd As New OleDbCommand(sql, conn)
                Using reader As OleDbDataReader = cmd.ExecuteReader(CommandBehavior.SequentialAccess)
                    Dim fieldCount As Integer = reader.FieldCount

                    ' Shift_JIS エンコーディングで出力
                    Dim sjisEnc As Encoding = Encoding.GetEncoding(932)

                    Using sw As New StreamWriter(csvPath, False, sjisEnc)
                        ' 1. ヘッダー行出力 (Hoken_No の直後に「保険者名称」を追加、除外列はスキップ)
                        Dim headerCols As New List(Of String)()
                        For i As Integer = 0 To fieldCount - 1
                            Dim colName As String = reader.GetName(i)
                            Dim headerTitle As String = colName
                            If captions IsNot Nothing AndAlso captions.ContainsKey(colName) Then
                                headerTitle = captions(colName)
                            End If

                            ' 指定された列は除外
                            If Not IsExcludedColumn(colName, headerTitle) Then
                                headerCols.Add(CleanCsvValue(headerTitle))

                                ' Hoken_No の直後に「保険者名称」列を追加
                                If String.Equals(colName, "Hoken_No", StringComparison.OrdinalIgnoreCase) Then
                                    headerCols.Add(CleanCsvValue("保険者名称"))
                                End If
                            End If
                        Next
                        sw.WriteLine(String.Join(",", headerCols))

                        progress.Report("CSVファイルに書き込み中...")

                        ' 2. データ行出力
                        While reader.Read()
                            Dim rowCols As New List(Of String)()
                            For i As Integer = 0 To fieldCount - 1
                                Dim colName As String = reader.GetName(i)
                                Dim headerTitle As String = colName
                                If captions IsNot Nothing AndAlso captions.ContainsKey(colName) Then
                                    headerTitle = captions(colName)
                                End If

                                ' 指定された列は除外
                                If Not IsExcludedColumn(colName, headerTitle) Then
                                    If reader.IsDBNull(i) Then
                                        rowCols.Add("")
                                    Else
                                        Dim val As Object = reader.GetValue(i)
                                        ' 受診者性別(Js_Se)と被保険者性別(Hi_Se)の変換
                                        If String.Equals(colName, "Js_Se", StringComparison.OrdinalIgnoreCase) OrElse _
                                           String.Equals(colName, "Hi_Se", StringComparison.OrdinalIgnoreCase) Then
                                            rowCols.Add(CleanCsvValue(FormatGender(val)))
                                        Else
                                            rowCols.Add(CleanCsvValue(val))
                                        End If
                                    End If

                                    ' Hoken_No の直後に検索した保険者名称を追加
                                    If String.Equals(colName, "Hoken_No", StringComparison.OrdinalIgnoreCase) Then
                                        Dim hokenName As String = ""
                                        If Not reader.IsDBNull(i) Then
                                            Dim rawNo As String = reader.GetValue(i).ToString()
                                            Dim trimmedNo As String = rawNo.Trim()
                                            Dim searchKey As String = ""
                                            If trimmedNo.Length > 0 Then
                                                If trimmedNo.Length < 8 Then
                                                    searchKey = trimmedNo.PadLeft(8, " "c)
                                                Else
                                                    searchKey = trimmedNo
                                                End If
                                            End If

                                            If Not String.IsNullOrEmpty(searchKey) AndAlso hokenDict.ContainsKey(searchKey) Then
                                                hokenName = hokenDict(searchKey)
                                            End If
                                        End If
                                        ' 一致しない場合は空文字（ダブルクォーテーションなし）
                                        rowCols.Add(CleanCsvValue(hokenName))
                                    End If
                                End If
                            Next

                            sw.WriteLine(String.Join(",", rowCols))
                            exportCount += 1

                            If exportCount Mod 200 = 0 Then
                                progress.Report(String.Format("CSV書き込み中... ({0:#,##0} 件)", exportCount))
                            End If
                        End While
                    End Using
                End Using
            End Using

            conn.Close()
        End Using

        Return exportCount
    End Function

    ''' <summary>
    ''' D_Hoken テーブルから保険者番号([No])と保険者名称([Na])を読み込んでディクショナリに格納する
    ''' </summary>
    Private Function LoadHokenDictionary(conn As OleDbConnection) As Dictionary(Of String, String)
        Dim dict As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        Dim sql As String = "SELECT [No], [Na] FROM D_Hoken"

        Using cmd As New OleDbCommand(sql, conn)
            Using reader As OleDbDataReader = cmd.ExecuteReader()
                While reader.Read()
                    If Not reader.IsDBNull(0) Then
                        Dim no As String = reader.GetString(0)
                        Dim na As String = ""
                        If Not reader.IsDBNull(1) Then
                            na = reader.GetString(1)
                        End If
                        dict(no) = na
                    End If
                End While
            End Using
        End Using

        Return dict
    End Function

    ''' <summary>
    ''' DAOを使用して D_Karte テーブルの各フィールドの「標題 (Caption)」を取得する
    ''' </summary>
    Private Function GetFieldCaptions(mdbPath As String) As Dictionary(Of String, String)
        Dim dict As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        Dim daoEngine As Object = Nothing
        Dim db As Object = Nothing

        Try
            ' DAO.DBEngine.36 をレイトバインディングで生成
            Dim daoType As Type = Type.GetTypeFromProgID("DAO.DBEngine.36")
            If daoType Is Nothing Then
                Return dict
            End If

            daoEngine = Activator.CreateInstance(daoType)
            ' OpenDatabase(Name, Exclusive, ReadOnly, Connect)
            ' 共有リードオンリーでオープン
            db = daoType.InvokeMember("OpenDatabase", _
                                      Reflection.BindingFlags.InvokeMethod, _
                                      Nothing, _
                                      daoEngine, _
                                      New Object() {mdbPath, False, True, ";PWD=" & MDB_PASSWORD})

            Dim tableDefs As Object = db.GetType().InvokeMember("TableDefs", Reflection.BindingFlags.GetProperty, Nothing, db, Nothing)
            Dim tableDef As Object = tableDefs.GetType().InvokeMember("Item", Reflection.BindingFlags.GetProperty, Nothing, tableDefs, New Object() {"D_Karte"})
            Dim fields As Object = tableDef.GetType().InvokeMember("Fields", Reflection.BindingFlags.GetProperty, Nothing, tableDef, Nothing)
            Dim fieldCount As Integer = CInt(fields.GetType().InvokeMember("Count", Reflection.BindingFlags.GetProperty, Nothing, fields, Nothing))

            For i As Integer = 0 To fieldCount - 1
                Dim fld As Object = fields.GetType().InvokeMember("Item", Reflection.BindingFlags.GetProperty, Nothing, fields, New Object() {i})
                Dim fldName As String = CStr(fld.GetType().InvokeMember("Name", Reflection.BindingFlags.GetProperty, Nothing, fld, Nothing))
                Dim caption As String = fldName

                Try
                    Dim props As Object = fld.GetType().InvokeMember("Properties", Reflection.BindingFlags.GetProperty, Nothing, fld, Nothing)
                    Dim propCaption As Object = props.GetType().InvokeMember("Item", Reflection.BindingFlags.GetProperty, Nothing, props, New Object() {"Caption"})
                    Dim val As Object = propCaption.GetType().InvokeMember("Value", Reflection.BindingFlags.GetProperty, Nothing, propCaption, Nothing)
                    If val IsNot Nothing AndAlso Not String.IsNullOrEmpty(val.ToString().Trim()) Then
                        caption = val.ToString().Trim()
                    End If
                Catch
                    ' Caption プロパティが未設定の場合はフィールド名をそのまま採用
                End Try

                dict(fldName) = caption
            Next

        Catch ex As Exception
            ' 排他ロックエラーの場合は上位へスロー
            If IsExclusiveLockException(ex) Then
                Throw
            End If
            ' その他の例外はフォールバック（フィールド名で出力）
        Finally
            If db IsNot Nothing Then
                Try
                    db.GetType().InvokeMember("Close", Reflection.BindingFlags.InvokeMethod, Nothing, db, Nothing)
                Catch
                End Try
            End If
        End Try

        Return dict
    End Function

    ''' <summary>
    ''' 出力対象外の列かどうか判定する
    ''' </summary>
    Private Function IsExcludedColumn(colName As String, captionTitle As String) As Boolean
        Dim excludedNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            "D_ID",
            "Up_DT",
            "Data_End",
            "Hoken_D_ID",
            "Hoken_Code",
            "Taishoku",
            "Iryou",
            "Kyori",
            "Inin0",
            "Gisei",
            "Print_Tel",
            "Fu_No2",
            "Jk_No2",
            "Kourei"
        }
        If excludedNames.Contains(colName) Then Return True

        Dim excludedCaptions As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            "ﾃﾞ-ﾀID",
            "最終修正日時",
            "ﾃﾞ-ﾀ有効期間最後の日",
            "D_HokenのD_ID",
            "自由診療等のためのｺ-ﾄ",
            "自由診療等のためのｺ-ﾄﾞ",
            "退職者種別",
            "医療証種別",
            "往診距離ﾃﾞﾌｫﾙﾄ",
            "委任先",
            "擬制世帯主",
            "電話番号印刷許可",
            "負担者番号2",
            "受給者番号2",
            "高齢者フラグ"
        }
        If Not String.IsNullOrEmpty(captionTitle) AndAlso excludedCaptions.Contains(captionTitle) Then
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' 性別コード(0:不明, 1:男, 2:女)を文字列に変換する
    ''' </summary>
    Private Function FormatGender(val As Object) As String
        If val Is Nothing OrElse IsDBNull(val) Then
            Return ""
        End If
        Dim s As String = val.ToString().Trim()
        Select Case s
            Case "0"
                Return "不明"
            Case "1"
                Return "男"
            Case "2"
                Return "女"
            Case Else
                Return s
        End Select
    End Function

    ''' <summary>
    ''' カンマ、改行(CR, LF)、ダブルクォーテーションをすべて除去して文字列化する
    ''' </summary>
    Private Function CleanCsvValue(val As Object) As String
        If val Is Nothing OrElse IsDBNull(val) Then
            Return ""
        End If

        Dim text As String
        If TypeOf val Is DateTime Then
            Dim dt As DateTime = DirectCast(val, DateTime)
            If dt.TimeOfDay.TotalSeconds = 0 Then
                text = dt.ToString("yyyy/MM/dd")
            Else
                text = dt.ToString("yyyy/MM/dd HH:mm:ss")
            End If
        Else
            text = val.ToString()
        End If

        ' カンマ (,), ダブルクォーテーション ("), CR, LF をすべて除去
        text = text.Replace(","c, "") _
                   .Replace(""""c, "") _
                   .Replace(vbCrLf, "") _
                   .Replace(vbCr, "") _
                   .Replace(vbLf, "")

        Return text
    End Function

    ''' <summary>
    ''' 例外が排他ロックエラーかどうかを判定する
    ''' </summary>
    Private Function IsExclusiveLockException(ex As Exception) As Boolean
        If ex Is Nothing Then Return False

        Dim msg As String = ex.Message.ToLower()
        If msg.Contains("already opened exclusively") OrElse _
           msg.Contains("exclusively") OrElse _
           msg.Contains("排他") OrElse _
           msg.Contains("ロック") OrElse _
           msg.Contains("他のユーザーが排他的に開いている") OrElse _
           msg.Contains("cannot be manipulated programmatically") OrElse _
           msg.Contains("使用中") Then
            Return True
        End If

        If TypeOf ex Is OleDbException Then
            Dim oleEx As OleDbException = DirectCast(ex, OleDbException)
            For Each oleErr As OleDbError In oleEx.Errors
                Dim errText As String = oleErr.Message.ToLower()
                If errText.Contains("already opened exclusively") OrElse _
                   errText.Contains("exclusively") OrElse _
                   errText.Contains("排他") OrElse _
                   errText.Contains("cannot be manipulated programmatically") Then
                    Return True
                End If
            Next
        End If

        If ex.InnerException IsNot Nothing Then
            Return IsExclusiveLockException(ex.InnerException)
        End If

        Return False
    End Function

    ''' <summary>
    ''' パスを絶対パスに解決する
    ''' </summary>
    Private Function ResolveFullPath(pathStr As String) As String
        If String.IsNullOrEmpty(pathStr) Then Return String.Empty
        If Path.IsPathRooted(pathStr) Then
            Return Path.GetFullPath(pathStr)
        Else
            Return Path.GetFullPath(Path.Combine(Application.StartupPath, pathStr))
        End If
    End Function

    ''' <summary>
    ''' 相対パスの作成
    ''' </summary>
    Private Function MakeRelativePath(fromPath As String, toPath As String) As String
        Try
            Dim fromUri As New Uri(If(fromPath.EndsWith("\"), fromPath, fromPath & "\"))
            Dim toUri As New Uri(toPath)
            Dim relUri As Uri = fromUri.MakeRelativeUri(toUri)
            Dim relPath As String = Uri.UnescapeDataString(relUri.ToString())
            Return relPath.Replace("/"c, "\"c)
        Catch
            Return toPath
        End Try
    End Function

    ''' <summary>
    ''' UIコントロールの有効/無効切り替え
    ''' </summary>
    Private Sub SetUiState(enabled As Boolean)
        txtMdbPath.Enabled = enabled
        btnBrowseMdb.Enabled = enabled
        txtCsvPath.Enabled = enabled
        btnBrowseCsv.Enabled = enabled
        btnExport.Enabled = enabled
        btnClose.Enabled = enabled
    End Sub

End Class
