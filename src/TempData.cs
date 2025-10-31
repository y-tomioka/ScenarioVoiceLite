using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVoice
{
    public class OnseParameter
    {
        public OnseParameter()
        {
            //m_subtitleData = new List<Subtitle>();
            m_onseParamData = new List<OnseParamData>();
            m_accentData = new List<AccentData>();
        }
        public void clear()
        {
            //m_subtitleData = new List<Subtitle>();
            m_onseParamData = new List<OnseParamData>();
            m_accentData = new List<AccentData>();
        }
        //public List<Subtitle> m_subtitleData;
        public List<OnseParamData> m_onseParamData;
        public List<AccentData> m_accentData;
    }

    public class OnseParamData
    {
        public OnseParamData()
        {
            voice = "";
            text = "";
            wasoku = 1;
            peak = 0;
            yokuyou = 1;
            wasokuat = 100;
            onteiat = 100;
            volume = 100;
            echo = false;
            beforeSilence = 0;
            afterSilence = 0;
        }
        public string voice;
        public string text;
        public double wasoku;
        public double peak;
        public double yokuyou;
        public int wasokuat;
        public int onteiat;
        public int volume;
        public bool echo;
        public double beforeSilence;
        public double afterSilence;
    }

    public class TempDataRelation
    {
        public TempDataRelation() 
        {
            name = "";
            voice = "";
        }
        public string name;
        public string voice;
    }

    public class TempOnseKigou
    {
        public TempOnseKigou()
        {
            m_text = "";
            m_jimaku = "";
        }
        public void clear()
        {
            m_text = "";
            m_jimaku = "";
        }
        public string m_text;
        public string m_jimaku;
    }

    public class AccentData
    {
        public AccentData()
        {
            m_accent = new List<int>();
            m_pause = new List<double?>();
            m_intonation = new List<List<double>>();
            m_boin = new List<List<double?>>();
            m_siin = new List<List<double?>>();
            m_text = new AccentString();
            m_onsekigou = new TempOnseKigou();
        }
        public List<int> m_accent = new List<int>();
        public List<double?> m_pause = new List<double?>();
        public List<List<double>> m_intonation = new List<List<double>>();
        public List<List<double?>> m_boin = new List<List<double?>>();
        public List<List<double?>> m_siin = new List<List<double?>>();
        public AccentString m_text = new AccentString();
        public TempOnseKigou m_onsekigou = new TempOnseKigou();
    }
}
