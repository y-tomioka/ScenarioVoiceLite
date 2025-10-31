using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STVoice
{
    public partial class Form9 : Form
    {
        private OtherData m_otherData;
        private MascotBadge m_mascot;

        public Form9(OtherData otherData, MascotBadge mascot)
        {
            InitializeComponent();
            m_otherData = otherData;
            initializeData();
            m_mascot = mascot;
        }

        private void initializeData()
        {
            //checkBox1.Checked = m_otherData.singleplay;
            textBox1.Text = m_otherData.savepath;
            textBox2.Text = m_otherData.no.ToString();
            checkBox2.Checked = m_otherData.jimaku;
            checkBox3.Checked = m_otherData.mascotto;
            checkBox4.Checked = m_otherData.boyomi;
            //umericUpDown1.Value = m_otherData.delay;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "音声保存フォルダを指定してください。";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.SelectedPath = (textBox1.Text.Length > 0) ? textBox1.Text : System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            fbd.ShowNewFolderButton = true;

            //ダイアログを表示する
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                textBox1.Text = fbd.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "0";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //m_otherData.singleplay = checkBox1.Checked;
            m_otherData.savepath = textBox1.Text;
            m_otherData.no = int.Parse(textBox2.Text);
            m_otherData.jimaku = checkBox2.Checked;
            m_otherData.mascotto = checkBox3.Checked;
            //m_otherData.delay = int.Parse(numericUpDown1.Value.ToString());
            m_otherData.boyomi = checkBox4.Checked;
            STVoiceUtility.setOtherData(m_otherData);
            m_mascot.SetManualVisible(!m_otherData.mascotto);
            this.Close();
        }
    }
}
