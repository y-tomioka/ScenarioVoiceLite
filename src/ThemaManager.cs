using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace STVoice
{
    public enum AppTheme { Light, Dark }

    public static class ThemeManager
    {
        // ---- パレット（必要に応じて後で微調整します） ----
        public static class Light
        {
            public static readonly Color Window = Color.FromArgb(245, 247, 250);
            public static readonly Color Surface = Color.White;
            public static readonly Color Border = Color.FromArgb(204, 210, 217);
            public static readonly Color Text = Color.FromArgb(33, 37, 41);
            public static readonly Color Subtext = Color.FromArgb(90, 96, 104);
            public static readonly Color Input = Color.White;
            public static readonly Color Accent = Color.FromArgb(59, 130, 246);
            public static readonly Color ButtonNeutral = Color.FromArgb(248, 249, 250);
        }

        public static class Dark
        {
            public static readonly Color Window = Color.FromArgb(24, 26, 28);
            public static readonly Color Surface = Color.FromArgb(34, 36, 38);
            public static readonly Color Border = Color.FromArgb(60, 64, 67);
            public static readonly Color Text = Color.FromArgb(235, 236, 240);
            public static readonly Color Subtext = Color.FromArgb(180, 184, 190);
            public static readonly Color Input = Color.FromArgb(30, 32, 34);
            public static readonly Color Accent = Color.FromArgb(37, 99, 235);
            public static readonly Color ButtonNeutral = Color.FromArgb(41, 43, 45);
        }

        public static void Apply(Form form, AppTheme theme)
        {
            var p = theme == AppTheme.Light ? typeof(Light) : typeof(Dark);
            Color win = (Color)p.GetField("Window").GetValue(null);
            Color surface = (Color)p.GetField("Surface").GetValue(null);
            Color border = (Color)p.GetField("Border").GetValue(null);
            Color text = (Color)p.GetField("Text").GetValue(null);
            Color subtext = (Color)p.GetField("Subtext").GetValue(null);
            Color input = (Color)p.GetField("Input").GetValue(null);
            Color accent = (Color)p.GetField("Accent").GetValue(null);
            Color btnNeutral = (Color)p.GetField("ButtonNeutral").GetValue(null);

            form.BackColor = win;
            form.ForeColor = text;

            // フォーム直下の「カード」っぽいコンテナ（Panel/GroupBox等）にSurface色
            ApplyRecursive(form, control =>
            {
                switch (control)
                {
                    case GroupBox g:
                        g.BackColor = surface;
                        g.ForeColor = text;
                        // 既定のGroupBox境界線を薄く見せたい場合はPaddingで中身に余白を確保
                        //g.Padding = new Padding(16);
                        break;

                    case Panel pnl:
                        if (pnl.BorderStyle == BorderStyle.None)
                            pnl.BackColor = surface;
                        else
                            pnl.BackColor = surface;
                        break;

                    case LinkLabel ll: // ←順序は Label より先
                        ll.LinkColor = accent;
                        ll.ActiveLinkColor = ControlPaint.Light(accent);
                        ll.VisitedLinkColor = accent;
                        ll.ForeColor = text;
                        ll.UseMnemonic = false;
                        break;

                    case Label lb when !(control is LinkLabel):
                        lb.ForeColor = lb.Enabled ? text : subtext;
                        // 見切れ防止：明示的に固定サイズにしていないラベルは AutoSize
                        if (!"noautosize".Equals(lb.Tag as string, StringComparison.OrdinalIgnoreCase))
                        {
                            lb.AutoSize = true;
                            lb.UseMnemonic = false; // 日本語で'_'誤解釈を避ける
                        }
                        break;

                     case TextBox tb:
                        tb.BackColor = input;
                        tb.ForeColor = text;
                        tb.BorderStyle = BorderStyle.FixedSingle;
                        break;

                    case ComboBox cb:
                        cb.BackColor = input;
                        cb.ForeColor = text;
                        cb.FlatStyle = FlatStyle.Standard; // 標準描画
                        break;

                    case CheckBox ck:
                        ck.ForeColor = text;
                        break;

                    case Button btn:
                        // “参照”のようなニュートラルボタン
                        bool isAccent = IsAccentButton(btn);
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderColor = border;
                        btn.FlatAppearance.BorderSize = 1;
                        btn.BackColor = isAccent ? accent : btnNeutral;
                        btn.ForeColor = isAccent ? Color.White : text;
                        //btn.Padding = new Padding(4, 3, 4, 3);
                        break;

                    case TabControl tabs:
                        tabs.DrawMode = TabDrawMode.Normal; // 標準のまま
                        tabs.Appearance = TabAppearance.Normal;
                        tabs.BackColor = surface;
                        tabs.ForeColor = text;
                        SetDoubleBuffered(tabs);
                        foreach (TabPage page in tabs.TabPages)
                        {
                            page.BackColor = surface;
                            page.ForeColor = text;
                            //page.Padding = new Padding(16);
                        }
                        break;

                    case ListView lv:
                        lv.BackColor = surface;
                        lv.ForeColor = text;
                        lv.BorderStyle = BorderStyle.FixedSingle;
                        SetDoubleBuffered(lv);
                        break;

                    case StatusStrip ss:
                        ss.BackColor = surface;
                        ss.ForeColor = text;
                        break;

                    case MenuStrip ms:
                        ms.BackColor = surface;
                        ms.ForeColor = text;
                        foreach (ToolStripMenuItem it in ms.Items)
                        {
                            it.ForeColor = text;
                        }
                        break;

                    default:
                        // コンテナ類
                        if (control is ContainerControl cc)
                        {
                            control.BackColor = surface;
                            control.ForeColor = text;
                        }
                        break;
                }
            });
        }

        // 判定：アクセント色にしたいボタン（名前・テキスト・Tagで切替可能）
        public static bool IsAccentButton(Button btn)
        {
            // 例: 次へ / OK / 実行 ボタンを強調
            string t = (btn.Text ?? "").Trim();
            if (t == "次へ" || t.Equals("OK", StringComparison.OrdinalIgnoreCase) || t.Contains("実行") || t.Contains("音声合成"))
                return true;
            if ((btn.Tag as string)?.Contains("accent") == true) return true;
            if (btn.Name?.EndsWith("Primary", StringComparison.OrdinalIgnoreCase) == true) return true;
            return false;
        }

        private static void ApplyRecursive(Control root, Action<Control> apply)
        {
            apply(root);
            foreach (Control c in root.Controls)
                ApplyRecursive(c, apply);
        }

        private static void SetDoubleBuffered(Control c)
        {
            try
            {
                typeof(Control).InvokeMember("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                    null, c, new object[] { true });
            }
            catch { /* ignore */ }
        }
    }
}
