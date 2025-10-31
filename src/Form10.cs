using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static STVoice.AudioQuery;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace STVoice
{
    public partial class Form10 : Form
    {
        AudioQuery m_audioquery = new AudioQuery();
        List<List<System.Windows.Forms.RadioButton>> radiolist = new List<List<System.Windows.Forms.RadioButton>>();
        List<List<System.Windows.Forms.TextBox>> textlist = new List<List<System.Windows.Forms.TextBox>>();
        List<List<System.Windows.Forms.TextBox>> textpauselist = new List<List<System.Windows.Forms.TextBox>>();
        List<List<System.Windows.Forms.TextBox>> textboinlist = new List<List<System.Windows.Forms.TextBox>>();
        List<List<System.Windows.Forms.TextBox>> textsiinlist = new List<List<System.Windows.Forms.TextBox>>();
        int m_id;
        string m_port;
        List<int> m_accent;
        List<List<double>> m_intonation;
        List<double?> m_pause;
        List<List<double?>> m_boin;
        List<List<double?>> m_siin;
        AccentString m_text;
        OnseParamData m_onseParamData;
        string m_aquestalkkigou;
        Logger m_logger;

        public Form10(AudioQuery audioQuery, int id, string port, List<int> accent, List<List<double>> intonation, List<double?> pause, List<List<double?>> boin, List<List<double?>> siin, AccentString text, OnseParamData onseParamData, Logger logger)
        {
            m_audioquery = audioQuery;
            m_id = id;
            m_port = port;
            m_accent = accent;
            m_intonation = intonation;
            m_pause = pause;
            m_boin = boin;
            m_siin = siin;
            m_text = text;
            m_onseParamData = onseParamData;
            m_logger = logger;
            InitializeComponent();
            textBox1.Text = m_text.text;
            m_aquestalkkigou = m_text.aquestalkkigou;
            displayCheckBox();
        }

        private void displayCheckBox()
        {
            int rety = 0;

            for(int i=0; i<textlist.Count(); i++)
            {
                for (int j = 0; j < textlist[i].Count(); j++)
                {
                    textlist[i][j].MouseWheel -= new System.Windows.Forms.MouseEventHandler(TextBox_MouseWheel);
                }
            }
            for (int i = 0; i < textpauselist.Count(); i++)
            {
                for (int j = 0; j < textpauselist[i].Count(); j++)
                {
                    textpauselist[i][j].MouseWheel -= new System.Windows.Forms.MouseEventHandler(TextBox_MouseWheel);
                }
            }
            for (int i = 0; i < textboinlist.Count(); i++)
            {
                for (int j = 0; j < textboinlist[i].Count(); j++)
                {
                    textboinlist[i][j].MouseWheel -= new System.Windows.Forms.MouseEventHandler(TextBox_MouseWheel);
                }
            }
            for (int i = 0; i < textsiinlist.Count(); i++)
            {
                for (int j = 0; j < textsiinlist[i].Count(); j++)
                {
                    textsiinlist[i][j].MouseWheel -= new System.Windows.Forms.MouseEventHandler(TextBox_MouseWheel);
                }
            }

            radiolist.Clear();
            textlist.Clear();
            textpauselist.Clear();
            textboinlist.Clear();
            textsiinlist.Clear();
            panel1.Controls.Clear();
            panel2.Controls.Clear();
            panel3.Controls.Clear();

            for (int i = 0; i < m_audioquery.accent_phrases.Count(); i++)
            {
                /* アクセント初期設定 */
                if (m_accent.Count > 0)
                {
                    m_audioquery.accent_phrases[i].accent = m_accent[i];
                }
                //if (i == 0)
                {
                    List<System.Windows.Forms.RadioButton> curradiolist = new List<System.Windows.Forms.RadioButton>();
                    System.Windows.Forms.GroupBox groupBox = addControl(m_audioquery.accent_phrases[i], int.Parse(m_audioquery.accent_phrases[i].accent.ToString()) - 1, ref rety, curradiolist);
                    //groupBox.Text = i.ToString();
                    int point = rety - groupBox.Height;
                    groupBox.Location = new Point(30, point);
                    //rety= rety + 10;
                    panel1.Controls.Add(groupBox);
                    radiolist.Add(curradiolist);
                }
            }
            rety = 0;
            for (int i = 0; i < m_audioquery.accent_phrases.Count(); i++)
            {
                /* イントネーション初期設定 */
                if (m_intonation.Count > 0)
                {
                    for (int j = 0; j < m_audioquery.accent_phrases[i].moras.Count(); j++)
                    {
                        m_audioquery.accent_phrases[i].moras[j].pitch = m_intonation[i][j];
                    }
                }
                //if (i == 0)
                {
                    List<System.Windows.Forms.TextBox> curtextlist = new List<System.Windows.Forms.TextBox>();
                    addControlPitch(m_audioquery.accent_phrases[i], ref rety, curtextlist);
                    textlist.Add(curtextlist);
                }
            }
            rety = 0;
            for (int i = 0; i < m_audioquery.accent_phrases.Count(); i++)
            {
                /* Pause・母音・子音 初期設定 */
                if (m_pause.Count > 0)
                {
                    if (m_audioquery.accent_phrases[i].pause_mora != null)
                    {
                        m_audioquery.accent_phrases[i].pause_mora.vowel_length = m_pause[i];
                    }
                }
                if (m_boin.Count > 0)
                {
                    for (int j = 0; j < m_audioquery.accent_phrases[i].moras.Count(); j++)
                    {
                        m_audioquery.accent_phrases[i].moras[j].vowel_length = m_boin[i][j];
                    }
                }
                if (m_siin.Count > 0)
                {
                    for (int j = 0; j < m_audioquery.accent_phrases[i].moras.Count(); j++)
                    {
                        m_audioquery.accent_phrases[i].moras[j].consonant_length = m_siin[i][j];
                    }
                }
                //if (i == 0)
                {
                    List<System.Windows.Forms.TextBox> curtextlist = new List<System.Windows.Forms.TextBox>();
                    List<System.Windows.Forms.TextBox> curboinlist = new List<System.Windows.Forms.TextBox>();
                    List<System.Windows.Forms.TextBox> cursiinlist = new List<System.Windows.Forms.TextBox>();
                    addControlPause(m_audioquery.accent_phrases[i], ref rety, curtextlist, curboinlist, cursiinlist);
                    textpauselist.Add(curtextlist);
                    textboinlist.Add(curboinlist);
                    textsiinlist.Add(cursiinlist);
                }
            }
        }

        private System.Windows.Forms.GroupBox addControl(AccentPhrase moji, int accent, ref int rety, List<System.Windows.Forms.RadioButton> list)
        {
            int point = 0;
            System.Windows.Forms.GroupBox groupBox = new System.Windows.Forms.GroupBox();
            groupBox.AutoSize = true;
            for (int j = 0; j < moji.moras.Count(); j++)
            {
                System.Windows.Forms.RadioButton radioButton = new System.Windows.Forms.RadioButton();
                radioButton.Text = moji.moras[j].text;
                if (j == accent)
                {
                    radioButton.Checked = true;
                }
                else
                {
                    radioButton.Checked = false;
                }
                point = 20 + 20 * j;
                radioButton.Location = new Point(10, point);
                groupBox.Height = point;
                groupBox.Controls.Add(radioButton);
                list.Add(radioButton);
            }
            groupBox.Height = groupBox.Height + 20;
            rety = groupBox.Height + rety;
            return groupBox;
        }

        private void addControlPitch(AccentPhrase moji, ref int rety, List<System.Windows.Forms.TextBox> list)
        {
            int point = rety;
            for (int j = 0; j < moji.moras.Count(); j++)
            {
                System.Windows.Forms.TextBox textButton = new System.Windows.Forms.TextBox();
                System.Windows.Forms.Label label = new System.Windows.Forms.Label();

                label.Text = moji.moras[j].text.ToString();
                label.Height = 20;
                textButton.Text = (Math.Truncate((decimal)moji.moras[j].pitch * 100) / 100).ToString();
                textButton.Width = 150;
                textButton.TextAlign = HorizontalAlignment.Right;

                point = rety + 20 + 20 * j;
                label.Location = new Point(10, point);
                textButton.Location = new Point(50, point);
                panel2.Controls.Add(label);
                panel2.Controls.Add(textButton);
                list.Add(textButton);
                textButton.MouseWheel += new MouseEventHandler(this.TextBox_MouseWheel);
            }
            rety = point + 20;
            return;
        }

        private void addControlPause(AccentPhrase moji, ref int rety, List<System.Windows.Forms.TextBox> list, List<System.Windows.Forms.TextBox> boinlist, List<System.Windows.Forms.TextBox> siinlist)
        {
            int point = rety;
            for (int j = 0; j < moji.moras.Count(); j++)
            {
                //System.Windows.Forms.TextBox textButton = new System.Windows.Forms.TextBox();
                System.Windows.Forms.Label label = new System.Windows.Forms.Label();

                label.Text = moji.moras[j].text.ToString();
                label.Height = 20;
                //textButton.Text = (Math.Truncate((decimal)moji.moras[j].pitch * 100) / 100).ToString();
                //textButton.Width = 150;
                //textButton.TextAlign = HorizontalAlignment.Right;

                point = rety + 20 + 20 * j;
                label.Location = new Point(10, point);
                //textButton.Location = new Point(50, point);
                panel3.Controls.Add(label);
                //panel2.Controls.Add(textButton);
                //list.Add(textButton);
                /* 子音 */
                {
                    System.Windows.Forms.TextBox textButton = new System.Windows.Forms.TextBox();
                    if (moji.moras[j].consonant_length == null)
                    {
                        textButton.Text = "0";
                    }
                    else
                    {
                        textButton.Text = (Math.Truncate((decimal)moji.moras[j].consonant_length * 1000) / 1000).ToString();
                    }
                    textButton.Width = 150;
                    textButton.TextAlign = HorizontalAlignment.Right;
                    textButton.Location = new Point(50, point);
                    panel3.Controls.Add(textButton);
                    siinlist.Add(textButton);
                    textButton.MouseWheel += new MouseEventHandler(this.TextBox_MouseWheel);
                }
                /* 母音 */
                {
                    System.Windows.Forms.TextBox textButton = new System.Windows.Forms.TextBox();
                    if (moji.moras[j].vowel_length == null)
                    {
                        textButton.Text = "0";
                    }
                    else
                    {
                        textButton.Text = (Math.Truncate((decimal)moji.moras[j].vowel_length * 1000) / 1000).ToString();
                    }
                    textButton.Width = 90;
                    textButton.TextAlign = HorizontalAlignment.Right;
                    textButton.Location = new Point(220, point);
                    panel3.Controls.Add(textButton);
                    boinlist.Add(textButton);
                    textButton.MouseWheel += new MouseEventHandler(this.TextBox_MouseWheel);
                }
            }
            if (moji.pause_mora != null)
            {
                System.Windows.Forms.TextBox textButton = new System.Windows.Forms.TextBox();
                System.Windows.Forms.Label label = new System.Windows.Forms.Label();

                label.Text = "、";
                label.Height = 20;
                textButton.Text = (Math.Truncate((decimal)moji.pause_mora.vowel_length * 100) / 100).ToString();
                textButton.Width = 150;
                textButton.TextAlign = HorizontalAlignment.Right;

                point = point + 20;
                label.Location = new Point(10, point);
                textButton.Location = new Point(50, point);
                panel3.Controls.Add(label);
                panel3.Controls.Add(textButton);
                list.Add(textButton);
                textButton.MouseWheel += new MouseEventHandler(this.TextBox_MouseWheel);
            }
            rety = point + 20;
            return;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\tmp");
            string wavpath = System.Environment.CurrentDirectory + "\\tmp\\" + "tmp.wav";
            File.Delete(wavpath);

            /* イントネーション変更 */
            JsonControl jsonControl = new JsonControl();
            WebControl webControl = new WebControl(m_logger);
            List<AudioQuery.AccentPhrase> accent = jsonControl.getAccentData(m_audioquery);
            for (int i = 0; i < textlist.Count(); i++)
            {
                for (int j = 0; j < textlist[i].Count; j++)
                {
                    bool retval = validateText(textlist[i][j]);
                    if (retval == false)
                    {
                        return;
                    }
                    accent[i].moras[j].pitch = double.Parse(textlist[i][j].Text);
                }
            }
            /* Pause変更 */
            for (int i = 0; i < textpauselist.Count(); i++)
            {
                for (int j = 0; j < textpauselist[i].Count; j++)
                {
                    bool retval = validatePauseText(textpauselist[i][j]);
                    if (retval == false)
                    {
                        return;
                    }
                    accent[i].pause_mora.vowel_length = double.Parse(textpauselist[i][j].Text);
                }
            }
            /* 母音変更 */
            for (int i = 0; i < textboinlist.Count(); i++)
            {
                for (int j = 0; j < textboinlist[i].Count; j++)
                {
                    bool retval = validateBoinSiin(textboinlist[i][j]);
                    if (retval == false)
                    {
                        return;
                    }
                    accent[i].moras[j].vowel_length = double.Parse(textboinlist[i][j].Text);
                }
            }
            /* 子音変更 */
            for (int i = 0; i < textsiinlist.Count(); i++)
            {
                for (int j = 0; j < textsiinlist[i].Count; j++)
                {
                    bool retval = validateBoinSiin(textsiinlist[i][j]);
                    if (retval == false)
                    {
                        return;
                    }
                    accent[i].moras[j].consonant_length = double.Parse(textsiinlist[i][j].Text);
                }
            }
            m_audioquery.speedScale = m_onseParamData.wasoku;
            m_audioquery.pitchScale = m_onseParamData.peak;
            m_audioquery.intonationScale = m_onseParamData.yokuyou;

            try
            {
                string jsonStr = jsonControl.serializeAudioQueryJson(m_audioquery);
                string url2 = "http://127.0.0.1:" + m_port + "/synthesis?speaker=" + m_id;
                webControl.postAndSave(url2, jsonStr, wavpath);
            }
            catch(Exception exp)
            {
                m_logger.Error("音声生成中に例外が発生しました。VOICEVOXが起動していない可能性があります。" + exp.Message+exp.StackTrace+exp.Source);
                MessageBox.Show(this, "音声生成中に例外が発生しました。詳細はログをご確認ください");
                return;
            }
            if (m_onseParamData.echo == true)
            {
                STVoiceUtility.setEcho(wavpath);
            }
            int volume = m_onseParamData.volume;
            STVoiceUtility.volumeControl(wavpath, volume);

            System.Media.SoundPlayer player = new System.Media.SoundPlayer(wavpath);
            player.Play();
        }

        private bool validateText(System.Windows.Forms.TextBox text)
        {
            double data = 0.0;
            try
            {
                data = double.Parse(text.Text);
            }
            catch
            {
                text.Focus();
                MessageBox.Show(this, "扱える値ではありません。-1.0～7.0の範囲で入力してください");
                return false;
            }
            /*
            if (data <= 0)
            {
                return true;
            }
            */
            if (data < -1.0 || data > 7.0)
            {
                text.Focus();
                MessageBox.Show(this, "扱える値ではありません。-1.0～7.0の範囲で入力してください");
                return false;
            }
            return true;
        }

        private bool validatePauseText(System.Windows.Forms.TextBox text)
        {
            double data = 0.0;
            try
            {
                data = double.Parse(text.Text);
            }
            catch
            {
                text.Focus();
                MessageBox.Show(this, "扱える値ではありません。0.00～1.00の範囲で入力してください");
                return false;
            }
            if (data < 0.0 || data > 1.0)
            {
                text.Focus();
                MessageBox.Show(this, "扱える値ではありません。0.00～1.00の範囲で入力してください");
                return false;
            }
            return true;
        }

        private bool validateBoinSiin(System.Windows.Forms.TextBox text)
        {
            double data = 0.0;
            try
            {
                data = double.Parse(text.Text);
            }
            catch
            {
                text.Focus();
                MessageBox.Show(this, "扱える値ではありません。0.0～0.5の範囲で入力してください");
                return false;
            }
            if (data == 0)
            {
                return true;
            }
            if (data < 0.0 || data > 0.500)
            {
                text.Focus();
                MessageBox.Show(this, "扱える値ではありません。0.0～0.5の範囲で入力してください");
                return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < textlist.Count(); i++)
            {
                for (int j = 0; j < textlist[i].Count; j++)
                {
                    bool retval = validateText(textlist[i][j]);
                    if (retval == false)
                    {
                        return;
                    }
                }
            }

            for (int i = 0; i < textpauselist.Count(); i++)
            {
                for (int j = 0; j < textpauselist[i].Count; j++)
                {
                    bool retval = validatePauseText(textpauselist[i][j]);
                    if (retval == false)
                    {
                        return;
                    }
                }
            }

            for (int i = 0; i < textboinlist.Count(); i++)
            {
                for (int j = 0; j < textboinlist[i].Count; j++)
                {
                    bool retval = validateBoinSiin(textboinlist[i][j]);
                    if (retval == false)
                    {
                        return;
                    }
                }
            }
            for (int i = 0; i < textsiinlist.Count(); i++)
            {
                for (int j = 0; j < textsiinlist[i].Count; j++)
                {
                    bool retval = validateBoinSiin(textsiinlist[i][j]);
                    if (retval == false)
                    {
                        return;
                    }
                }
            }

            for (int i = 0; i < textlist.Count(); i++)
            {
                for (int j = 0; j < textlist[i].Count(); j++)
                {
                    textlist[i][j].MouseWheel -= new System.Windows.Forms.MouseEventHandler(TextBox_MouseWheel);
                }
            }
            for (int i = 0; i < textpauselist.Count(); i++)
            {
                for (int j = 0; j < textpauselist[i].Count(); j++)
                {
                    textpauselist[i][j].MouseWheel -= new System.Windows.Forms.MouseEventHandler(TextBox_MouseWheel);
                }
            }
            for (int i = 0; i < textboinlist.Count(); i++)
            {
                for (int j = 0; j < textboinlist[i].Count(); j++)
                {
                    textboinlist[i][j].MouseWheel -= new System.Windows.Forms.MouseEventHandler(TextBox_MouseWheel);
                }
            }
            for (int i = 0; i < textboinlist.Count(); i++)
            {
                for (int j = 0; j < textsiinlist[i].Count(); j++)
                {
                    textsiinlist[i][j].MouseWheel -= new System.Windows.Forms.MouseEventHandler(TextBox_MouseWheel);
                }
            }

            m_accent.Clear();
            for (int i = 0; i < radiolist.Count(); i++)
            {
                for (int j = 0; j < radiolist[i].Count; j++)
                {
                    if (radiolist[i][j].Checked)
                    {
                        m_accent.Add(j + 1);
                        break;
                    }
                }
            }
            m_intonation.Clear();
            for(int i=0; i<textlist.Count(); i++)
            {
                List<double> list = new List<double>();
                for (int j = 0; j < textlist[i].Count; j++)
                {
                    list.Add(double.Parse(textlist[i][j].Text));
                }
                m_intonation.Add(list);
            }
            m_pause.Clear();
            for (int i = 0; i < textpauselist.Count(); i++)
            {
                if(textpauselist[i].Count == 0)
                {
                    m_pause.Add(null);
                    continue;
                }
                for (int j = 0; j < textpauselist[i].Count; j++)
                {
                    m_pause.Add(double.Parse(textpauselist[i][j].Text));
                }
            }
            m_boin.Clear();
            for (int i = 0; i < textboinlist.Count(); i++)
            {
                List<double?> list = new List<double?>();
                for (int j = 0; j < textboinlist[i].Count; j++)
                {
                    list.Add(double.Parse(textboinlist[i][j].Text));
                }
                m_boin.Add(list);
            }
            m_siin.Clear();
            for (int i = 0; i < textsiinlist.Count(); i++)
            {
                List<double?> list = new List<double?>();
                for (int j = 0; j < textsiinlist[i].Count; j++)
                {
                    list.Add(double.Parse(textsiinlist[i][j].Text));
                }
                m_siin.Add(list);
            }
            m_text.text = textBox1.Text;
            m_text.aquestalkkigou = m_aquestalkkigou;
            m_logger.Info("アクセントを変更しました。");
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\wav");
                string wavpath = System.Environment.CurrentDirectory + "\\tmp\\" + "tmp.wav";
                File.Delete(wavpath);

                /* アクセント変更 */
                JsonControl jsonControl = new JsonControl();
                WebControl webControl = new WebControl(m_logger);
                string url3 = "http://127.0.0.1:" + m_port + "/mora_pitch/?speaker=" + m_id;
                List<AudioQuery.AccentPhrase> accent = jsonControl.getAccentData(m_audioquery);

                for (int i = 0; i < radiolist.Count(); i++)
                {
                    for (int j = 0; j < radiolist[i].Count; j++)
                    {
                        if (radiolist[i][j].Checked)
                        {
                            accent[i].accent = j + 1;
                            break;
                        }
                    }
                }
                string jsonStr = jsonControl.serializeAccentQueryJson(accent);
                string getjson = webControl.post(url3, jsonStr);
                List<AudioQuery.AccentPhrase> getaccent = jsonControl.deserializeAccentQueryJson(getjson);
                for (int i = 0; i < getaccent.Count(); i++)
                {
                    for (int j = 0; j < getaccent[i].moras.Count(); j++)
                    {
                        textlist[i][j].Text = (Math.Truncate((decimal)getaccent[i].moras[j].pitch * 100) / 100).ToString();
                        if (getaccent[i].moras[j].vowel_length == null)
                        {
                            textboinlist[i][j].Text = "0";
                        }
                        else
                        {
                            textboinlist[i][j].Text = (Math.Truncate((decimal)getaccent[i].moras[j].vowel_length * 1000) / 1000).ToString();
                        }
                        if (getaccent[i].moras[j].consonant_length == null)
                        {
                            textsiinlist[i][j].Text = "0";
                        }
                        else
                        {
                            textsiinlist[i][j].Text = (Math.Truncate((decimal)getaccent[i].moras[j].consonant_length * 1000) / 1000).ToString();
                        }
                    }
                }
            }
            catch(Exception exp)
            {
                m_logger.Error("アクセント適用中に例外が発生しました。VOICEVOXが起動していない可能性があります。" + exp.Message+exp.StackTrace+exp.Source);
                MessageBox.Show(this, "アクセント適用中に例外が発生しました。詳細はログをご確認ください");
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = m_text.text;

            string text = textBox1.Text.Replace("\r\n", " ").Trim();
            text = STVoiceUtility.beforeChangeText(text);
            text = (text.Length > 500) ? text.Substring(0, 500) : text;

            AudioQuery ret;
            try
            {
                WebControl webControl = new WebControl(m_logger);
                JsonControl jsonCtrl = new JsonControl();
                string url = "http://127.0.0.1:" + m_port + "/audio_query?text=" + text + "&speaker=" + m_id;
                string json = "";
                string getjson = webControl.post(url, json);
                ret = jsonCtrl.deserializeAudioQueryJson(getjson);
            }
            catch(Exception exp)
            {
                m_logger.Error("リセット中に例外が発生しました。VOICEVOXが起動していない可能性があります。" + exp.Message + exp.StackTrace + exp.Source);
                MessageBox.Show(this, "リセット中に例外が発生しました。詳細は、ログをご確認ください");
                return;
            }
            m_audioquery = ret;
            m_accent.Clear();
            m_intonation.Clear();
            m_pause.Clear();
            m_boin.Clear();
            m_siin.Clear();
            m_aquestalkkigou = ret.kana;

            displayCheckBox();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text.Replace("\r\n", " ").Trim();
            text = STVoiceUtility.beforeChangeText(text);
            text = (text.Length > 500) ? text.Substring(0, 500) : text;
            //m_text.text = text;

            /* アクセント情報取得 */
            AudioQuery ret;
            try
            {
                WebControl webControl = new WebControl(m_logger);
                JsonControl jsonCtrl = new JsonControl();
                string url = "http://127.0.0.1:" + m_port + "/audio_query?text=" + text + "&speaker=" + m_id;
                string json = "";
                string getjson = webControl.post(url, json);
                ret = jsonCtrl.deserializeAudioQueryJson(getjson);
            }
            catch(Exception exp)
            {
                m_logger.Error("テキストからアクセント変換中に例外が発生しました。VOICEVOXが起動していない可能性があります。" + exp.Message + exp.StackTrace + exp.Source);
                MessageBox.Show(this, "テキストからアクセント変換中に例外が発生しました。詳細は、ログをご確認ください");
                return;
            }
            m_audioquery = ret;
            m_accent.Clear();
            m_intonation.Clear();
            m_pause.Clear();
            m_boin.Clear();
            m_siin.Clear();
            m_aquestalkkigou = ret.kana;

            displayCheckBox();
        }

        private void TextBox_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
            System.Windows.Forms.TextBox textBox = sender as System.Windows.Forms.TextBox;
            if (textBox != null)
            {
                if (e.Delta > 0)
                {
                    textBox.Text = (double.Parse(textBox.Text) + 0.01).ToString();
                }
                else
                {
                    if (double.Parse(textBox.Text) > 0)
                    {
                        textBox.Text = (double.Parse(textBox.Text) - 0.01).ToString();
                    }
                }
            }
        }
    }
}
