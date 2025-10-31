using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace STVoice
{
    /// <summary>
    /// ホスト(Control)の右上にマスコットを表示するヘルパー。標準PictureBoxのみ。
    /// </summary>
    public sealed class MascotBadge : IDisposable
    {
        public PictureBox Picture { get; }
        private readonly Control _host;
        private readonly ToolStrip _topBar; // MenuStrip/ToolStripなど(任意)
        private readonly int _sizePx, _marginTop, _marginRight;
        private readonly bool _dpiScale;

        private Image _orig;
        private int _minHostWidth = 0;
        private TabControl _bindTab;
        private int _bindTabIndex = -1;
        private Action _onClick;

        private float _scale = 1f;
        private int _scaledSize, _scaledTop, _scaledRight;
        private bool _allowNegativeTop = false;

        private bool _manualVisible = true;

        private MascotBadge(Control host, Image image,
            int sizePx, int marginTop, int marginRight,
            ToolStrip topBar, bool dpiScale)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _topBar = topBar;
            _sizePx = sizePx;
            _marginTop = marginTop;
            _marginRight = marginRight;
            _dpiScale = dpiScale;

            Picture = new PictureBox
            {
                Image = image ?? throw new ArgumentNullException(nameof(image)),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                TabStop = false
            };
        }

        public static MascotBadge Attach(Control host, Image image,
            int sizePx = 72, int marginTop = 12, int marginRight = 12,
            ToolStrip topBar = null, bool dpiScale = true)
        {
            var m = new MascotBadge(host, image, sizePx, marginTop, marginRight, topBar, dpiScale);
            m.AttachInternal();
            return m;
        }

        public MascotBadge WithOpacity(float opacity) // 0.0～1.0
        {
            opacity = Math.Max(0f, Math.Min(1f, opacity));
            if (_orig == null && Picture.Image != null)
                _orig = (Image)Picture.Image.Clone();

            Picture.Image?.Dispose();
            Picture.Image = ApplyOpacity(_orig ?? Properties.Resources.mascotto, opacity);
            return this;
        }

        public MascotBadge HideWhenTooNarrow(int widthPx)
        {
            _minHostWidth = widthPx;
            UpdatePosition();
            return this;
        }

        public MascotBadge VisibleOnTab(TabControl tc, int tabIndex)
        {
            _bindTab = tc;
            _bindTabIndex = tabIndex;
            if (_bindTab != null)
                _bindTab.SelectedIndexChanged += (s, e) => UpdatePosition();
            UpdatePosition();
            return this;
        }

        public MascotBadge OnClick(Action handler)
        {
            _onClick = handler;
            Picture.Cursor = Cursors.Hand;
            return this;
        }

        // ---- 位置計算（旧 Position をこの名前に統一）----
        public void UpdatePosition()
        {
            int size = Scale(_sizePx);
            Picture.Size = new Size(size, size);

            int top = Scale(_marginTop) +
                      (_topBar != null && _topBar.Visible ? _topBar.Height : 0);
            int right = Scale(_marginRight);

            var cs = _host.ClientSize;
            Picture.Location = new Point(
                Math.Max(0, cs.Width - Picture.Width - right),
                Math.Max(0, top)
            );

            // 可視の自動条件を計算
            bool autoVisible = true;
            if (_minHostWidth > 0 && _host.ClientSize.Width < Scale(_minHostWidth)) autoVisible = false;
            if (_bindTab != null && _bindTabIndex >= 0 && _bindTab.SelectedIndex != _bindTabIndex) autoVisible = false;

            // ← ★ここを唯一の可視制御に
            Picture.Visible = _manualVisible && autoVisible;
        }

        public void Dispose()
        {
            _host.Resize -= OnHostChanged;
            if (_topBar != null) _topBar.SizeChanged -= OnHostChanged;

            if (!Picture.IsDisposed)
            {
                Picture.Parent?.Controls.Remove(Picture);
                Picture.Dispose();
            }
            _orig?.Dispose();
        }

        // ---- 内部 ----
        private void AttachInternal()
        {
            _orig = (Image)Picture.Image.Clone();
            _host.Controls.Add(Picture);
            Picture.BringToFront();

            //_host.Resize += OnHostChanged;
            //if (_topBar != null) _topBar.SizeChanged += OnHostChanged;
            ComputeScaleOnce();
            Picture.Size = new Size(_scaledSize, _scaledSize);
               // レイアウト完了後に位置を確定（Resize中の中間値では動かさない）
            _host.Layout += (s, e) => _host.BeginInvoke((Action)UpdatePosition);
               if (_topBar != null)
                _topBar.Layout += (s, e) => _host.BeginInvoke((Action)UpdatePosition);

            Picture.Click += (s, e) => _onClick?.Invoke();

            UpdatePosition();
        }

        private void OnHostChanged(object s, EventArgs e) => UpdatePosition();

        private int Scale(int px)
        {
            if (!_dpiScale) return px;
            try
            {
                using (var g = _host.CreateGraphics())
                    return (int)Math.Round(px * (g.DpiX / 96f));
            }
            catch { return px; }
        }

        private static Image ApplyOpacity(Image src, float opacity)
        {
            var bmp = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            using (var ia = new ImageAttributes())
            {
                var cm = new ColorMatrix { Matrix33 = opacity };
                ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(src, new Rectangle(0, 0, bmp.Width, bmp.Height),
                    0, 0, src.Width, src.Height, GraphicsUnit.Pixel, ia);
            }
            return bmp;
        }

        private void ComputeScaleOnce()
        {
            if (!_dpiScale) { _scale = 1f; }
            else
            {
                try { _scale = _host.DeviceDpi / 96f; }
                catch
                {
                    using (var g = _host.CreateGraphics())
                        _scale = g.DpiX / 96f;
                }
            }
            _scaledSize = (int)Math.Round(_sizePx * _scale);
            _scaledTop = (int)Math.Round(_marginTop * _scale);
            _scaledRight = (int)Math.Round(_marginRight * _scale);
        }

        public void Position()
        {
            // サイズは毎回再計算せず固定（DPIが変わったら手動で呼ぶ想定）
            Picture.Size = new Size(_scaledSize, _scaledSize);

            // MenuStrip/ToolStrip がホスト(Form)直下にあるなら Bottom を起点にすると安定
            int baseTop = 0;
            if (_topBar != null && _topBar.Visible)
            {
                if (_host is Form f && _topBar.Parent == f) baseTop = _topBar.Bounds.Bottom;
                else baseTop = _topBar.Height;
            }

            int x = Math.Max(0, _host.ClientSize.Width - Picture.Width - _scaledRight);
            int y = baseTop + _scaledTop;

            if (!_allowNegativeTop) y = Math.Max(0, y);  // ← 負のマージンを許可するか選べる

            Picture.Location = new Point(x, y);

            // 可視条件を使うならここで判断。使っていないなら常に表示。
            Picture.Visible = true;
        }

        public MascotBadge AllowNegativeTop(bool allow = true)
        {
            _allowNegativeTop = allow;
            Position();
            return this;
        }

        public MascotBadge OnClickOpen(string url)
        {
            _onClick = () => OpenUrl(url);
            Picture.Cursor = Cursors.Hand;
            return this;
        }

        private static void OpenUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;

            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                // フォールバック（古い環境向け）
                try
                {
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
                    { UseShellExecute = false, CreateNoWindow = true });
                }
                catch { /* no-op */ }
            }
        }

        public MascotBadge SetManualVisible(bool visible)
        {
            _manualVisible = visible;
            UpdatePosition();                   // or UpdatePosition()
            return this;
        }
    }
}
