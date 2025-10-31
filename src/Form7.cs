using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml.Linq;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using NLog;

namespace STVoice
{
    public partial class Form7 : Form
    {
        private List<OnseParamData> m_tempData;
        private List<TempDataRelation> m_tempDataRelation;
        private List<Speakers> m_speakers = new List<Speakers>();
        private string m_port;
        private ComboBox m_combobox1;
        private ComboBox m_combobox2;
        private Logger m_logger;

        public Form7(List<OnseParamData> tempData, List<TempDataRelation> tempDataRelation, List<Speakers> speakers, string port, ComboBox combobox2, Logger logger)
        {            InitializeComponent();
            m_tempData = tempData;
            m_tempDataRelation = tempDataRelation;
            m_speakers = speakers;
            m_port = port;
            //m_combobox1 = combobox1;
            m_combobox2 = combobox2;
            m_logger = logger;
            display();
        }

        private void display()
        {
            for (int i = 0; i< m_tempDataRelation.Count(); i++)
            {
                OnseParamData foundItem = m_tempData.Find(item => item.voice == m_tempDataRelation[i].name);
                System.Windows.Forms.ListViewItem lvi = listView1.Items.Add(m_tempDataRelation[i].name);
                lvi.SubItems.Add(m_tempDataRelation[i].voice);
                OnseParamData tempData1 = new OnseParamData();
                tempData1.wasoku = foundItem.wasoku;
                tempData1.peak = foundItem.peak;
                tempData1.yokuyou = foundItem.yokuyou;
                tempData1.wasokuat = foundItem.wasokuat;
                tempData1.onteiat = foundItem.onteiat;
                tempData1.volume = foundItem.volume;
                tempData1.voice = m_tempDataRelation[i].voice;
                tempData1.echo = foundItem.echo;
                lvi.SubItems.Add(STVoiceUtility.getSettingValue(tempData1));
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8(listView1, m_tempData, m_tempDataRelation, m_speakers, m_port, "", null, m_logger);
            form8.Show();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                System.Windows.Forms.ListViewItem list_view_item = listView1.SelectedItems[0];
                string name = list_view_item.Text;
                Form8 form8 = new Form8(listView1, m_tempData, m_tempDataRelation, m_speakers, m_port, name, list_view_item, m_logger);
                form8.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                DialogResult dr = MessageBox.Show(this, "削除してもよろしいですか？", "確認", MessageBoxButtons.YesNo);
                if (dr != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
                System.Windows.Forms.ListViewItem list_view_item = listView1.SelectedItems[0];
                string name = list_view_item.Text;
                OnseParamData foundItem = m_tempData.Find(item => item.voice == name);
                TempDataRelation foundItem2 = m_tempDataRelation.Find(item => item.name == name);
                m_tempData.Remove(foundItem);
                m_tempDataRelation.Remove(foundItem2);
                listView1.Items.Remove(listView1.SelectedItems[0]);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            STVoiceUtility.savePresetData(m_tempData, m_tempDataRelation);

            //m_combobox1.Items.Clear();
            m_combobox2.Items.Clear();
            for (int i = 0; i < m_tempDataRelation.Count(); i++)
            {
                //m_combobox1.Items.Add(m_tempDataRelation[i].name);
                m_combobox2.Items.Add(m_tempDataRelation[i].name);
            }
            bool atflag = STVoiceUtility.getAquesTalk();
            if (atflag == true)
            {
                List<ATData> atdata = STVoiceUtility.getATComboboxStr();
                for (int i = 0; i < atdata.Count(); i++)
                {
                    //m_combobox1.Items.Add(atdata[i].comboboxStr);
                    m_combobox2.Items.Add(atdata[i].comboboxStr);
                }
            }

            for (int i = 0; i < m_speakers.Count(); i++)
            {
                for (int j = 0; j < m_speakers[i].styles.Count(); j++)
                {
                    //m_combobox1.Items.Add(m_speakers[i].name + ":" + m_speakers[i].styles[j].name);
                    m_combobox2.Items.Add(m_speakers[i].name + ":" + m_speakers[i].styles[j].name);
                }
            }
            this.Close();
        }
    }
}
