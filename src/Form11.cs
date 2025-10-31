using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using NLog;

namespace STVoice
{
    public partial class Form11 : Form
    {
        private string m_jimaku;
        private TempOnseKigou m_onsekigou;
        private int m_aid;
        private int m_id;
        private string m_port;
        private OnseParamData m_onseParamData;
        private Logger m_logger;
        private OtherData m_otherData;

        public Form11(int aid, int id, string port, string str, TempOnseKigou onsekigou, OnseParamData onseParamData, Logger logger, OtherData otherData)
        {
            m_jimaku = str;
            m_onsekigou = onsekigou;
            m_aid = aid;
            m_id = id;
            m_port = port;
            m_onseParamData = onseParamData;
            m_logger = logger;
            m_otherData = otherData;
            InitializeComponent();
            textBox1.Text = (onsekigou.m_text.Length == 0) ? str : onsekigou.m_jimaku;
            textBox2.Text = (onsekigou.m_text.Length == 0) ? "" : onsekigou.m_text;
            if (onsekigou.m_text.Length == 0)
            {
                makeOnseKigou();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            makeOnseKigou();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox2.Text == "")
            {
                MessageBox.Show(this, "音声記号を入力してください");
                return;
            }

            Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\tmp");
            string wavpath = System.Environment.CurrentDirectory + "\\tmp\\" + "tmp.wav";
            File.Delete(wavpath);

            string onsegouseitext = textBox2.Text;
            int speed = m_onseParamData.wasokuat;
            int ontei = m_onseParamData.onteiat;
            bool result = STVoiceUtility.makeATWaveData(m_aid, onsegouseitext, wavpath, speed, ontei, m_otherData.boyomi, false);
            if (result == false)
            {
                MessageBox.Show(this, "音声の生成に失敗しました。");
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

        private void makeOnseKigou()
        {
            try
            {
                WebControl webControl = new WebControl(m_logger);
                string text = textBox1.Text.Replace("\r\n", " ").Trim();
                text = STVoiceUtility.beforeChangeText(text);
                if (m_aid == 0)
                {
                    text = (text.Length > 500) ? text.Substring(0, 500) : text;
                }
                else
                {
                    text = (text.Length > 1000) ? text.Substring(0, 1000) : text;
                }
                string url = "http://127.0.0.1:" + m_port + "/audio_query?text=" + text + "&speaker=" + m_id;
                string json = "";
                string getjson = webControl.post(url, json);

                JsonControl jsonControl = new JsonControl();
                AudioQuery ret = jsonControl.deserializeAudioQueryJson(getjson);
                textBox2.Text = STVoiceUtility.changeText(ret.kana);
            }
            catch(Exception exp)
            {
                m_logger.Error("音声記号生成中に例外が発生しました。VOICEVOXが起動していない可能性があります。" + exp.Message+exp.StackTrace+exp.Source);
                MessageBox.Show(this, "音声記号生成中に例外が発生しました。詳細はログをご確認ください");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = m_jimaku;
            makeOnseKigou();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox2.Text == "")
            {
                MessageBox.Show(this, "音声記号を入力してください。");
                return;
            }
            m_onsekigou.m_text = textBox2.Text;
            m_onsekigou.m_jimaku = textBox1.Text;
            m_logger.Info("アクセントを変更しました。");
            this.Close();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            textBox2.SelectedText = "'";
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            textBox2.SelectedText = "/";
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            textBox2.SelectedText = ";";
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            textBox2.SelectedText = "+";
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            textBox2.SelectedText = ",";
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            textBox2.SelectedText = "、";
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            textBox2.SelectedText = "。";
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            textBox2.SelectedText = "？";
        }
    }
}
