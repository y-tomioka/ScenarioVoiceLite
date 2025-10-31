using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Security.Policy;
using NLog;
using System.Net.Http;

namespace STVoice
{
    public partial class Form1 : Form
    {
        private List<Speakers> m_speakers = new List<Speakers>();
        //private List<Subtitle> m_subtitles = new List<Subtitle>();
        private OnseParameter m_onseparam = new OnseParameter();
        private string m_port;
        private List<OnseParamData> m_tempData = new List<OnseParamData>();
        private List<TempDataRelation> m_tempDataRelation = new List<TempDataRelation>();
        private int m_fileterIndex;
        private OtherData m_otherData;
        private string m_originaltext;
        private List<int> m_accent = new List<int>();
        private List<double?> m_pause = new List<double?>();
        private List<List<double>> m_intonation = new List<List<double>>();
        private List<List<double?>> m_boin = new List<List<double?>>();
        private List<List<double?>> m_siin = new List<List<double?>>();
        private AccentString m_text = new AccentString();
        private TempOnseKigou m_onsekigou = new TempOnseKigou();
        private Logger m_logger;
        private MascotBadge m_mascot;

        public Form1(Logger logger)
        {
            InitializeComponent();
            ThemeManager.Apply(this, AppTheme.Light);
            gerPortNumber();
            displayCombobox();
            makedir();
            m_otherData = STVoiceUtility.getOtherData();
            m_logger = logger;
            m_mascot = MascotBadge.Attach(this, Properties.Resources.mascotto, sizePx: 50, marginTop: -14, marginRight: 10, topBar: this.MainMenuStrip).WithOpacity(0.82f).VisibleOnTab(tabControl1, 0).AllowNegativeTop(true).OnClickOpen("http://www.yasui-kamo.com/labo/ttsutility/");
            var tip = new ToolTip { InitialDelay = 300, ShowAlways = true };
            tip.SetToolTip(m_mascot.Picture, "公式サイトを開く");
            m_mascot.SetManualVisible(!m_otherData.mascotto);
        }

        private void makedir()
        {
            string path = System.Environment.CurrentDirectory + "\\" + "tmp" + "\\";
            Directory.CreateDirectory(path);
            string path2 = System.Environment.CurrentDirectory + "\\" + "wav" + "\\";
            Directory.CreateDirectory(path2);
            string path3 = System.Environment.CurrentDirectory + "\\" + "setting" + "\\";
            Directory.CreateDirectory(path3);
        }

        private void gerPortNumber()
        {
            m_port =  ConfigurationManager.AppSettings["port"];
        }

        private void displayCombobox()
        {
            STVoiceUtility.readPresetData(m_tempData, m_tempDataRelation);
            WebControl webControl = new WebControl(m_logger);
            string url = "http://127.0.0.1:" + m_port + "/speakers";
            string json = webControl.get(url);
            JsonControl jsonCtrl = new JsonControl();
            m_speakers = jsonCtrl.deserializeSpeakersJson(json);

            for (int i = 0; i < m_tempDataRelation.Count(); i++)
            {
                //comboBox1.Items.Add(m_tempDataRelation[i].name);
                comboBox2.Items.Add(m_tempDataRelation[i].name);
            }

            bool atflag = STVoiceUtility.getAquesTalk();
            if (atflag == true)
            {
                List<ATData> atdata = STVoiceUtility.getATComboboxStr();
                for (int i = 0; i < atdata.Count(); i++)
                {
                    //comboBox1.Items.Add(atdata[i].comboboxStr);
                    comboBox2.Items.Add(atdata[i].comboboxStr);
                    OnseParamData tempData = new OnseParamData();
                    tempData.voice = atdata[i].comboboxStr;
                    m_tempData.Add(tempData);
                }
            }

            for (int i = 0; i < m_speakers.Count(); i++)
            {

                for (int j = 0; j < m_speakers[i].styles.Count(); j++)
                {
                    //comboBox1.Items.Add(m_speakers[i].name + ":" + m_speakers[i].styles[j].name);
                    comboBox2.Items.Add(m_speakers[i].name + ":" + m_speakers[i].styles[j].name);
                    OnseParamData tempData = new OnseParamData();
                    tempData.voice = m_speakers[i].name + ":" + m_speakers[i].styles[j].name;
                    m_tempData.Add(tempData);
                }
            }

            //comboBox3.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnseParamData foundItem = m_tempData.Find(item => item.voice == (string)comboBox2.SelectedItem);
            TempDataRelation foundItem2 = m_tempDataRelation.Find(item => item.name == (string)comboBox2.SelectedItem);

            string voice = (string)comboBox2.SelectedItem;
            if(foundItem2 != null)
            {
                voice = foundItem2.voice;
            }
            if (STVoiceUtility.comoboboxToSpeakerIndex2(voice) > 0)
            {
                //numericUpDown2.Enabled = false;
                numericUpDown3.Enabled = false;
                numericUpDown1.DecimalPlaces = 0;
                numericUpDown1.Increment = 10;
                numericUpDown1.Maximum = 300;
                numericUpDown1.Minimum = 50;

                label9.Text = "音程";
                numericUpDown2.DecimalPlaces = 0;
                numericUpDown2.Increment = 10;
                numericUpDown2.Maximum = 200;
                numericUpDown2.Minimum = 50;

                if (foundItem2 == null)
                {
                    numericUpDown1.Value = 100;
                    numericUpDown2.Value = 100;
                    numericUpDown4.Value = 100;
                    checkBox2.Checked = false;
                }
                else
                {
                    numericUpDown1.Value = foundItem.wasokuat;
                    numericUpDown2.Value = foundItem.onteiat;
                    numericUpDown4.Value = foundItem.volume;
                    checkBox2.Checked = foundItem.echo;
                }
            }
            else
            {
                //numericUpDown2.Enabled = true;
                numericUpDown3.Enabled = true;
                numericUpDown1.DecimalPlaces = 2;
                numericUpDown1.Increment = Convert.ToDecimal(0.1);
                numericUpDown1.Maximum = Convert.ToDecimal(2.0);
                numericUpDown1.Minimum = Convert.ToDecimal(0.5);

                label9.Text = "音高";
                numericUpDown2.DecimalPlaces = 2;
                numericUpDown2.Increment = Convert.ToDecimal(0.01);
                numericUpDown2.Maximum = Convert.ToDecimal(0.15);
                numericUpDown2.Minimum = Convert.ToDecimal(-0.15);

                if (foundItem2 == null)
                {
                    numericUpDown1.Value = Convert.ToDecimal(1.0);
                    numericUpDown2.Value = Convert.ToDecimal(0.0);
                    numericUpDown3.Value = Convert.ToDecimal(1.0);
                    numericUpDown4.Value = 100;
                    checkBox2.Checked = false;
                }
                else
                {
                    numericUpDown1.Value = Convert.ToDecimal(foundItem.wasoku);
                    numericUpDown2.Value = Convert.ToDecimal(foundItem.peak);
                    numericUpDown3.Value = Convert.ToDecimal(foundItem.yokuyou);
                    numericUpDown4.Value = foundItem.volume;
                    checkBox2.Checked = foundItem.echo;
                }
                m_onsekigou.clear();
            }
            numericUpDown5.Value = 0;
            numericUpDown6.Value = 0;
            m_accent.Clear();
            m_intonation.Clear();
            m_boin.Clear();
            m_siin.Clear();
            m_pause.Clear();
            m_text.clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(textBox3.Text.Length == 0)
            {
                MessageBox.Show(this, "字幕を入力してください。");
                return;
            }

            int atid = 0;
            string voicedata = STVoiceUtility.getVoice(comboBox2.Text, m_tempDataRelation);
            int id = STVoiceUtility.comoboboxToSpeakerIndex(voicedata, m_speakers);
            if (id == -1)
            {
                id = STVoiceUtility.comoboboxToSpeakerIndex2(voicedata);
                if (id == -1)
                {
                    MessageBox.Show(this, "音声を選択してください。");
                    return;
                }
                atid = id;
                id = m_speakers[0].styles[0].id;
            }
            m_logger.Info("音声を作成します。");
            string usetext = textBox3.Text;
            string wavpath = "";
            try
            {
                m_otherData.no++;
                WebControl webControl = new WebControl(m_logger);
                JsonControl jsonControl = new JsonControl();
                AudioQuery ret = null;
                {
                    string text = textBox3.Text.Replace("\r\n", " ").Trim();
                    text = STVoiceUtility.beforeChangeText(text);
                    if (atid == 0)
                    {
                        text = (text.Length > 500) ? text.Substring(0, 500) : text;
                    }
                    else
                    {
                        text = (text.Length > 1000) ? text.Substring(0, 1000) : text;
                    }
                    if(m_text.text.Length > 0)
                    {
                        text = m_text.text;
                    }
                    string url = "http://127.0.0.1:" + m_port + "/audio_query?text=" + text + "&speaker=" + id;
                    string json = "";
                    string getjson = webControl.post(url, json);

                    ret = jsonControl.deserializeAudioQueryJson(getjson);
                    if (atid == 0)
                    {
                        if (m_text.aquestalkkigou.Length > 0)
                        {
                            string url2 = "http://127.0.0.1:" + m_port + "/accent_phrases?text=" + m_text.aquestalkkigou + "&speaker=" + id + "&is_kana=true";
                            string json2 = "";
                            string getjson2 = webControl.post(url2, json2);
                            List<AudioQuery.AccentPhrase> ret2 = jsonControl.deserializeAccentPhraseJson(getjson2);
                            ret.accent_phrases = ret2;
                        }
                        if (m_accent.Count > 0)
                        {
                            /* イントネーション変更 */
                            for (int i = 0; i < ret.accent_phrases.Count(); i++)
                            {
                                for (int j = 0; j < ret.accent_phrases[i].moras.Count(); j++)
                                {
                                    ret.accent_phrases[i].moras[j].pitch = m_intonation[i][j];
                                }
                            }
                        }
                        if (m_pause.Count > 0)
                        {
                            /* Pause変更 */
                            for (int i = 0; i < ret.accent_phrases.Count(); i++)
                            {
                                if (ret.accent_phrases[i].pause_mora != null)
                                {
                                    ret.accent_phrases[i].pause_mora.vowel_length= m_pause[i];
                                }
                            }
                        }
                        if (m_boin.Count > 0)
                        {
                            /* 母音変更 */
                            for (int i = 0; i < ret.accent_phrases.Count(); i++)
                            {
                                for (int j = 0; j < ret.accent_phrases[i].moras.Count(); j++)
                                {
                                    ret.accent_phrases[i].moras[j].vowel_length = m_boin[i][j];
                                }
                            }
                        }
                        if (m_siin.Count > 0)
                        {
                            /* 子音変更 */
                            for (int i = 0; i < ret.accent_phrases.Count(); i++)
                            {
                                for (int j = 0; j < ret.accent_phrases[i].moras.Count(); j++)
                                {
                                    ret.accent_phrases[i].moras[j].consonant_length = m_siin[i][j];
                                }
                            }
                        }

                        ret.speedScale = double.Parse(numericUpDown1.Value.ToString());
                        ret.pitchScale = double.Parse(numericUpDown2.Value.ToString());
                        ret.intonationScale = double.Parse(numericUpDown3.Value.ToString());
                    }
                }

                wavpath = System.Environment.CurrentDirectory + "\\tmp\\" + STVoiceUtility.getFileName(m_otherData.no, usetext, comboBox2.Text) + ".wav";
                if (atid == 0)
                {
                    string jsonStr = jsonControl.serializeAudioQueryJson(ret);
                    string url2 = "http://127.0.0.1:" + m_port + "/synthesis?speaker=" + id;
                    webControl.postAndSave(url2, jsonStr, wavpath);
                }
                else
                {
                    int speed = int.Parse(numericUpDown1.Value.ToString());
                    int ontei = int.Parse(numericUpDown2.Value.ToString());
                    string onsegouseitext = "";
                    if (m_onsekigou.m_text.Length > 0)
                    {
                        onsegouseitext = m_onsekigou.m_text;
                    }
                    else
                    {
                        onsegouseitext = ret.kana;
                    }
                    bool result = STVoiceUtility.makeATWaveData(atid, onsegouseitext, wavpath, speed, ontei, m_otherData.boyomi);
                    if (result == false)
                    {
                        MessageBox.Show(this, "音声の生成に失敗しました。");
                        return;
                    }
                }

                if(checkBox2.Checked == true)
                {
                    STVoiceUtility.setEcho(wavpath);
                }

                int before = (int)(double.Parse(numericUpDown5.Value.ToString()) * 1000);
                int after = (int)(double.Parse(numericUpDown6.Value.ToString()) * 1000);
                STVoiceUtility.addSilent(wavpath, before, after);

                int volume = int.Parse(numericUpDown4.Value.ToString());
                STVoiceUtility.volumeControl(wavpath, volume);
            }
            catch (Exception exp)
            {
                m_logger.Error("音声作成中に例外が発生しました。VOICEVOXが起動していない可能性があります。" + exp.Message+exp.StackTrace+exp.Source);
                MessageBox.Show(this, "音声作成中に例外が発生しました。詳細はログをご確認ください");
                return;
            }

            try
            {
                string filepath = System.Environment.CurrentDirectory + "\\tmp\\" + STVoiceUtility.getFileName(m_otherData.no, usetext, comboBox2.Text) + ".txt";
                if (m_otherData.jimaku == true)
                {
                    //string filepath = m_path + "\\" + STVoiceUtility.getFileName(m_subtitles[i].number, listView1.Items[i].SubItems[2].Text) + ".txt";
                    STVoiceUtility.createJimakuFile(filepath, usetext);
                }

                string destFilePath = m_otherData.savepath + "\\" + STVoiceUtility.getFileName(m_otherData.no, usetext, comboBox2.Text) + ".txt";
                string destwavpath = m_otherData.savepath + "\\" + STVoiceUtility.getFileName(m_otherData.no, usetext, comboBox2.Text) + ".wav";
                File.Delete(destwavpath);
                if (m_otherData.jimaku == true)
                {
                    File.Delete(destFilePath);
                    File.Move(filepath, destFilePath);
                }
                File.Move(wavpath, destwavpath);
            }
            catch (Exception exp)
            {
                m_logger.Error("ファイル移動中に例外が発生しました。" + exp.Message + exp.StackTrace + exp.Source);
                MessageBox.Show(this, "ファイルを移動できませんでした。一度、アプリを終了してください。");
                return;
            }
            m_logger.Info("音声を生成しました。");
            MessageBox.Show(this, "音声を生成しました。");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string usetext = textBox3.Text;
            string path = m_otherData.savepath + "\\" + STVoiceUtility.getFileName(m_otherData.no, usetext, comboBox2.Text) + ".wav";
            if (File.Exists(path))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
                player.Play();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string usetext = textBox3.Text;
            string path = m_otherData.savepath + "\\" + STVoiceUtility.getFileName(m_otherData.no, usetext, comboBox2.Text) + ".wav";
            if (File.Exists(path))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
                player.Stop();
            }
        }

        private void 音声プリセットToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7(m_tempData, m_tempDataRelation, m_speakers, m_port, comboBox2, m_logger);
            form7.Show();
        }

        private void バージョン情報ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "ScenarioVoice Lite ver：1.0");
        }

        private void 使い方ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://yasui-kamo.com/labo/ttsutility/");
        }

        private void その他ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form9 form9 = new Form9(m_otherData, m_mascot);
            form9.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Length == 0)
            {
                MessageBox.Show(this, "字幕を入力してください。");
                return;
            }

            int atid = 0;
            string voicedata = STVoiceUtility.getVoice(comboBox2.Text, m_tempDataRelation);
            int id = STVoiceUtility.comoboboxToSpeakerIndex(voicedata, m_speakers);
            if (id == -1)
            {
                id = STVoiceUtility.comoboboxToSpeakerIndex2(voicedata);
                if (id == -1)
                {
                    MessageBox.Show(this, "音声を選択してください。");
                    return;
                }
                atid = id;
                id = m_speakers[0].styles[0].id;
            }
            OnseParamData onseParamData = getOnseParamData();
            m_logger.Info("アクセントを変更します");
            if (atid != 0)
            {
                //MessageBox.Show("VOICEVOX以外はこの機能を利用できません。");
                Form11 form11 = new Form11(atid, id, m_port, textBox3.Text, m_onsekigou, onseParamData, m_logger, m_otherData);
                form11.Show();
                return;
            }
            string text = "";
            if (m_text.text.Length <= 0)
            {
                text = textBox3.Text.Replace("\r\n", " ").Trim();
                text = STVoiceUtility.beforeChangeText(text);
                text = (text.Length > 500) ? text.Substring(0, 500) : text;
                m_text.text = textBox3.Text;
            }
            else
            {
                text = m_text.text;
            }

            /* アクセント情報取得 */
            WebControl webControl = new WebControl(m_logger);
            JsonControl jsonCtrl = new JsonControl();
            string url = "http://127.0.0.1:" + m_port + "/audio_query?text=" + text + "&speaker=" + id;
            string json = "";
            string getjson = webControl.post(url, json);
            AudioQuery ret = jsonCtrl.deserializeAudioQueryJson(getjson);
            if (m_text.aquestalkkigou.Length > 0)
            {
                string url2 = "http://127.0.0.1:" + m_port + "/accent_phrases?text=" + m_text.aquestalkkigou + "&speaker=" + id + "&is_kana=true";
                string json2 = "";
                string getjson2 = webControl.post(url2, json2);
                List<AudioQuery.AccentPhrase> ret2 = jsonCtrl.deserializeAccentPhraseJson(getjson2);
                ret.accent_phrases = ret2;
            }
            if (m_text.aquestalkkigou.Length == 0)
            {
                m_text.aquestalkkigou = ret.kana;
            }

            Form10 form10 = new Form10(ret, id, m_port, m_accent, m_intonation, m_pause, m_boin, m_siin, m_text, onseParamData, m_logger);
            form10.Show();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            m_accent.Clear();
            m_intonation.Clear();
            m_boin.Clear();
            m_siin.Clear();
            m_pause.Clear();
            m_text.clear();
            m_onsekigou.clear();
        }

        private OnseParamData getOnseParamData()
        {
            OnseParamData onseData = new OnseParamData();
            string voicedata = STVoiceUtility.getVoice((string)comboBox2.SelectedItem, m_tempDataRelation);
            if (STVoiceUtility.comoboboxToSpeakerIndex2(voicedata) > 0)
            {
                int.TryParse(numericUpDown1.Value.ToString(), out onseData.wasokuat);
                int.TryParse(numericUpDown2.Value.ToString(), out onseData.onteiat);
            }
            else
            {
                double.TryParse(numericUpDown1.Value.ToString(), out onseData.wasoku);
                double.TryParse(numericUpDown2.Value.ToString(), out onseData.peak);
            }
            double.TryParse(numericUpDown3.Value.ToString(), out onseData.yokuyou);
            int.TryParse(numericUpDown4.Value.ToString(), out onseData.volume);
            onseData.echo = checkBox2.Checked;
            onseData.voice = comboBox2.SelectedItem.ToString();
            onseData.text = textBox3.Text;

            return onseData;
        }

        private void ご意見ご要望などToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.google.com/forms/d/e/1FAIpQLSdGUA3nb-FPraIMdF_P8NsS1rKMWjhEsMY0tK8vHHm4zgMVOw/viewform");
        }
    }
}
