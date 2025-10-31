using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using System.Web.UI.WebControls;
using NLog;

namespace STVoice
{
    public partial class Form8 : Form
    {
        System.Windows.Forms.ListView m_list_view;
        System.Windows.Forms.ListViewItem m_list_view_item;
        private List<OnseParamData> m_tempData;
        private List<TempDataRelation> m_tempDataRelation;
        private List<Speakers> m_speakers = new List<Speakers>();
        private string m_port;
        private string m_name;
        private Logger m_logger;

        public Form8(System.Windows.Forms.ListView list_view, List<OnseParamData> tempData, List<TempDataRelation> tempDataRelation, List<Speakers> speakers, string port, string name, System.Windows.Forms.ListViewItem list_view_item, Logger logger)
        {
            InitializeComponent();
            m_list_view = list_view;
            m_tempData = tempData;
            m_tempDataRelation = tempDataRelation;
            m_speakers = speakers;
            m_port = port;
            m_name = name;
            m_list_view_item = list_view_item;
            m_logger = logger;
            display();
        }

        private void display()
        {
            bool atflag = STVoiceUtility.getAquesTalk();
            if (atflag == true)
            {
                List<ATData> atdata = STVoiceUtility.getATComboboxStr();
                for (int i = 0; i < atdata.Count(); i++)
                {
                    comboBox1.Items.Add(atdata[i].comboboxStr);
                }
            }
            for (int i = 0; i < m_speakers.Count(); i++)
            {
                for (int j = 0; j < m_speakers[i].styles.Count(); j++)
                {
                    comboBox1.Items.Add(m_speakers[i].name + ":" + m_speakers[i].styles[j].name);
                }
            }

            if (m_name != "")
            {
                OnseParamData foundItem = m_tempData.Find(item => item.voice == m_name);
                TempDataRelation foundItem2 = m_tempDataRelation.Find(item => item.name == m_name);
                textBox1.Text = m_name;

                comboBox1.SelectedItem = foundItem2.voice;
                if (STVoiceUtility.comoboboxToSpeakerIndex2((string)comboBox1.SelectedItem) > 0)
                {
                    numericUpDown1.Value = foundItem.wasokuat;
                    numericUpDown2.Value = foundItem.onteiat;
                }
                else
                {
                    numericUpDown1.Value = Convert.ToDecimal(foundItem.wasoku);
                    numericUpDown2.Value = Convert.ToDecimal(foundItem.peak);
                }
                numericUpDown3.Value = Convert.ToDecimal(foundItem.yokuyou);
                numericUpDown4.Value = foundItem.volume;
                checkBox1.Checked = foundItem.echo;
            }
            ActiveControl = textBox1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int atid = 0;
            int id = STVoiceUtility.comoboboxToSpeakerIndex(comboBox1.Text, m_speakers);
            if (id == -1)
            {
                id = STVoiceUtility.comoboboxToSpeakerIndex2(comboBox1.Text);
                if (id == -1)
                {
                    MessageBox.Show(this, "音声を選択してください。");
                    return;
                }
                atid = id;
                id = m_speakers[0].styles[0].id;
            }
            string wavpath = "";
            try
            {
                WebControl webControl = new WebControl(m_logger);
                string url = "http://127.0.0.1:" + m_port + "/audio_query?text=" + "ゆっくりしていってね。" + "&speaker=" + id;
                string json = "";
                string getjson = webControl.post(url, json);

                JsonControl jsonControl = new JsonControl();
                AudioQuery ret = jsonControl.deserializeAudioQueryJson(getjson);
                if (atid == 0)
                {
                    ret.speedScale = double.Parse(numericUpDown1.Value.ToString());
                    ret.pitchScale = double.Parse(numericUpDown2.Value.ToString());
                    ret.intonationScale = double.Parse(numericUpDown3.Value.ToString());
                }

                wavpath = System.Environment.CurrentDirectory + "\\tmp\\" + "test" + ".wav";
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
                    bool result = STVoiceUtility.makeATWaveData(atid, ret.kana, wavpath, speed, ontei, false);
                    if (result == false)
                    {
                        MessageBox.Show(this, "音声の生成に失敗しました。");
                        return;
                    }
                }
                if(checkBox1.Checked == true)
                {
                    STVoiceUtility.setEcho(wavpath);
                }
                int volume = int.Parse(numericUpDown4.Value.ToString());
                STVoiceUtility.volumeControl(wavpath, volume);
            }
            catch (Exception /*ex*/)
            {
                MessageBox.Show(this, "VOICEVOXが起動していません。作成を終了します。");
                return;
            }

            if (File.Exists(wavpath))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(wavpath);
                player.Play();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (STVoiceUtility.comoboboxToSpeakerIndex2((string)comboBox1.SelectedItem) > 0)
            {
                //numericUpDown2.Enabled = false;
                numericUpDown3.Enabled = false;
                numericUpDown1.DecimalPlaces = 0;
                numericUpDown1.Increment = 10;
                numericUpDown1.Maximum = 300;
                numericUpDown1.Minimum = 50;

                label3.Text = "音程";
                numericUpDown2.DecimalPlaces = 0;
                numericUpDown2.Increment = 10;
                numericUpDown2.Maximum = 200;
                numericUpDown2.Minimum = 50;

                numericUpDown1.Value = 100;
                numericUpDown2.Value = 100;
                numericUpDown4.Value = 100;
            }
            else
            {
                //numericUpDown2.Enabled = true;
                numericUpDown3.Enabled = true;
                numericUpDown1.DecimalPlaces = 2;
                numericUpDown1.Increment = Convert.ToDecimal(0.1);
                numericUpDown1.Maximum = Convert.ToDecimal(2.0);
                numericUpDown1.Minimum = Convert.ToDecimal(0.5);

                label3.Text = "音高";
                numericUpDown2.DecimalPlaces = 2;
                numericUpDown2.Increment = Convert.ToDecimal(0.01);
                numericUpDown2.Maximum = Convert.ToDecimal(0.15);
                numericUpDown2.Minimum = Convert.ToDecimal(-0.15);

                numericUpDown1.Value = Convert.ToDecimal(1.0);
                numericUpDown2.Value = Convert.ToDecimal(0.0);
                numericUpDown3.Value = Convert.ToDecimal(1.0);
                numericUpDown4.Value = 100;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show(this, "名前を入力してください。");
                return;
            }
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show(this, "音声を入力してください。");
                return;
            }
            if (textBox1.Text.IndexOf("(") > 0)
            {
                MessageBox.Show(this, "名前に()は利用できません。");
                return;
            }
            OnseParamData foundItem = m_tempData.Find(item => item.voice == textBox1.Text);
            if (foundItem != null && m_name == "")
            {
                MessageBox.Show(this, "名前が重複しています。");
                return;
            }

            OnseParamData tempData;
            if (m_name == "" || foundItem == null)
            {
                tempData = new OnseParamData();
            }
            else
            {
                tempData = foundItem;
            }
            if (STVoiceUtility.comoboboxToSpeakerIndex2((string)comboBox1.SelectedItem) > 0)
            {
                int.TryParse(numericUpDown1.Value.ToString(), out tempData.wasokuat);
                int.TryParse(numericUpDown2.Value.ToString(), out tempData.onteiat);
            }
            else
            {
                double.TryParse(numericUpDown1.Value.ToString(), out tempData.wasoku);
                double.TryParse(numericUpDown2.Value.ToString(), out tempData.peak);
            }
            double.TryParse(numericUpDown3.Value.ToString(), out tempData.yokuyou);
            int.TryParse(numericUpDown4.Value.ToString(), out tempData.volume);
            tempData.voice = textBox1.Text;
            tempData.echo = checkBox1.Checked;

            TempDataRelation tempDataRelation = new TempDataRelation();
            if (m_name == "" || foundItem == null)
            {
                tempDataRelation = new TempDataRelation();
            }
            else
            {
                tempDataRelation = m_tempDataRelation.Find(item => item.name == m_name);
            }
            tempDataRelation.voice = comboBox1.SelectedItem.ToString();
            tempDataRelation.name = textBox1.Text;

            if (m_name == "" || foundItem == null)
            {
                m_tempData.Add(tempData);
                m_tempDataRelation.Add(tempDataRelation);

                OnseParamData foundItem2 = m_tempData.Find(item => item.voice == m_name);
                if (foundItem2 != null)
                {
                    m_tempData.Remove(foundItem2);
                }
                TempDataRelation foundItem3 = m_tempDataRelation.Find(item => item.name == m_name);
                if (foundItem3 != null)
                {
                    m_tempDataRelation.Remove(foundItem3);
                }
            }

            OnseParamData tempData1 = new OnseParamData();
            tempData1.wasoku = tempData.wasoku;
            tempData1.peak = tempData.peak;
            tempData1.yokuyou = tempData.yokuyou;
            tempData1.wasokuat = tempData.wasokuat;
            tempData1.onteiat = tempData.onteiat;
            tempData1.volume = tempData.volume;
            tempData1.voice = tempDataRelation.voice;
            if (m_name == "")
            {
                System.Windows.Forms.ListViewItem lvi = m_list_view.Items.Add(tempDataRelation.name);
                lvi.SubItems.Add(tempDataRelation.voice);
                lvi.SubItems.Add(STVoiceUtility.getSettingValue(tempData1));
            }
            else
            {
                m_list_view_item.SubItems[0].Text = tempDataRelation.name;
                m_list_view_item.SubItems[1].Text = tempDataRelation.voice;
                m_list_view_item.SubItems[2].Text = STVoiceUtility.getSettingValue(tempData1);
            }

            m_list_view.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.Close();
        }
    }
}
