using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using System.Collections;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Configuration;

namespace STVoice
{
    public class AccentString
    {
        public AccentString()
        {
            text = "";
            aquestalkkigou = "";
        }
        public void clear()
        {
            text = "";
            aquestalkkigou = "";
        }
        public string text;
        public string aquestalkkigou;
    }

    public class ATData
    {
        public string comboboxStr;
        public string first;
        public string second;
    }

    public class StrCmpLogical : IComparer, IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);
        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
        public int Compare(object x, object y)
        {
            return this.Compare(x.ToString(), y.ToString());
        }
    }

    [XmlRoot("OtherData")]
    public class OtherData
    {
        public OtherData()
        {
            singleplay = true;
            savepath = System.Environment.CurrentDirectory + "\\wav";
            no = 0;
            jimaku = false;
            mascotto = false;
            delay = 400;
            boyomi = false;
        }

        [XmlElement("singleplay")]
        public bool singleplay;
        [XmlElement("savepath")]
        public string savepath;
        [System.Xml.Serialization.XmlIgnore]
        public int no;
        [XmlElement("jimaku")]
        public bool jimaku;
        [XmlElement("mascotto")]
        public bool mascotto;
        [XmlElement("delay")]
        public int delay;
        [XmlElement("boyomi")]
        public bool boyomi;
    }

    internal class STVoiceUtility
    {
        public static int comoboboxToSpeakerIndex(string comboBoxText, List<Speakers> speakers)
        {
            string[] voice = comboBoxText.Split(':');

            if(voice.Length != 2)
            {
                return -1;
            }

            int id = -1;
            for (int i = 0; i < speakers.Count(); i++)
            {
                if (voice[0] == speakers[i].name)
                {
                    for (int j = 0; j < speakers[i].styles.Count(); j++)
                    {
                        if (voice[1] == speakers[i].styles[j].name)
                        {
                            id = speakers[i].styles[j].id;
                            return id;
                        }
                    }
                }
            }
            return id;
        }

        public static int comoboboxToSpeakerIndex2(string comboBoxText)
        {
            List<ATData> atdata = getATComboboxStr();
            for(int i=0; i<atdata.Count(); i++)
            {
                if(atdata[i].comboboxStr == comboBoxText)
                {
                    return 10000 + i;
                }
            }
            return -1;
        }

        public static string getFileName(int number, string text, string voice)
        {
            string[] voices = voice.Split(':');
            string voicetmp = (voices.Length >= 2) ? changeEnableFileName(voices[0]) + "(" + changeEnableFileName(voices[1]) + ")" : changeEnableFileName(voice);
            text = changeEnableFileName(text.Replace("\r\n", " ").Trim());
            string data;
            if(text.Length <= 20)
            {
                data = text;
            }
            else
            {
                data = text.Substring(0, 20);
            }
            string filename = number.ToString() + "_" + voicetmp + "_" + data;
            return filename;
        }

        private static string changeEnableFileName(string str)
        {
            return str.Replace("\\", "").Replace("/", "").Replace(":", "")
                      .Replace("*", "").Replace("?", "").Replace("\"", "")
                      .Replace("<", "").Replace(">", "").Replace("|", "");
        }

        public static string changeText(string str)
        {
            //str = str.Replace("_", "").Replace("ッ、", "ッ").Replace("/パ", "パ").Replace("ワ'ッ", "ワ。").Replace("'ッ/ツ", "ッツ").Replace("ッ'、", "、").Replace("ッ、", "、").Replace("'ッ？", "？").Replace("ッ'？", "？").Replace("ヴ", "ブ"); 
            str = str.Replace("_", "").Replace("ッ、", "ッ").Replace("/パ", "パ").Replace("'ッ/ツ", "ッツ").Replace("ッ'、", "、").Replace("ッ、", "、").Replace("'ッ？", "？").Replace("ッ'？", "？").Replace("ヴィ", "ビ").Replace("ヴァ", "バ").Replace("ヴェ", "ベ").Replace("ヴゥ", "ブ").Replace("ヴォ", "ボ").Replace("ヴ", "ブ"); 
            if (str.Substring(str.Length - 1) == "ッ")
            {
                str = str.TrimEnd('ッ') + "。";
            }
            else if(str.Length > 2 && str.Substring(str.Length - 2) == "ッ'")
            {
                str = str.TrimEnd('\'').TrimEnd('ッ') + "。";
            }
            return str;
        }

        public static string beforeChangeText(string str)
        {
            str = str.Replace("+", "ぷらす").Replace("#", "しゃーぷ");
            return str;
        }

        private static string ConvertStr(string s)
        {
            StringBuilder sb = new StringBuilder();
            char[] target = s.ToCharArray();
            char c;
            for (int i = 0; i < target.Length; i++)
            {
                c = target[i];
                if (c >= 'ァ' && c <= 'ヴ')
                { //-> カタカナの範囲
                    c = (char)(c - 0x0060);  //-> 変換
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static void createJimakuFile(string path, string text)
        {
            StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
            sw.WriteLine(text);
            sw.Close();
        }

        public static List<ATData> getATComboboxStr()
        {
            List<ATData> atcombo = new List<ATData>();
            {
                ATData data1 = new ATData();
                data1.comboboxStr = "AquesTalk:女性1";
                data1.first = "AquesTalk";
                data1.second = "女性1";
                atcombo.Add(data1);
            }
            {
                ATData data1 = new ATData();
                data1.comboboxStr = "AquesTalk:女性2";
                data1.first = "AquesTalk";
                data1.second = "女性2";
                atcombo.Add(data1);
            }
            {
                ATData data1 = new ATData();
                data1.comboboxStr = "AquesTalk:男性1";
                data1.first = "AquesTalk";
                data1.second = "男性1";
                atcombo.Add(data1);
            }
            {
                ATData data1 = new ATData();
                data1.comboboxStr = "AquesTalk:男性2";
                data1.first = "AquesTalk";
                data1.second = "男性2";
                atcombo.Add(data1);
            }
            {
                ATData data1 = new ATData();
                data1.comboboxStr = "AquesTalk:ロボット";
                data1.first = "AquesTalk";
                data1.second = "ロボット";
                atcombo.Add(data1);
            }
            {
                ATData data1 = new ATData();
                data1.comboboxStr = "AquesTalk:中性";
                data1.first = "AquesTalk";
                data1.second = "中性";
                atcombo.Add(data1);
            }
            {
                ATData data1 = new ATData();
                data1.comboboxStr = "AquesTalk:機械";
                data1.first = "AquesTalk";
                data1.second = "機械";
                atcombo.Add(data1);
            }
            {
                ATData data1 = new ATData();
                data1.comboboxStr = "AquesTalk:特殊";
                data1.first = "AquesTalk";
                data1.second = "特殊";
                atcombo.Add(data1);
            }
            return atcombo;
        }

        public static string getSettingValue(OnseParamData tempData)
        {
            if(comoboboxToSpeakerIndex2(tempData.voice) > 0)
            {
                return "[音量]" + tempData.volume.ToString() + ", [話速]" + tempData.wasokuat.ToString() + ", [音程]" + tempData.onteiat.ToString() + ", [エコー]" + tempData.echo.ToString();
            }
            else
            {
                return "[音量]" + tempData.volume.ToString() + ", [話速]" + tempData.wasoku.ToString() + ", [音高]" + tempData.peak.ToString() + ", [抑揚]" + tempData.yokuyou.ToString() + ", [エコー]" + tempData.echo.ToString();
            }
        }

        public static void setSettingValue(OnseParamData tempData, List<OnseParamData> initialData)
        {
            OnseParamData foundItem = initialData.Find(item => item.voice == tempData.voice);
            if (foundItem != null)
            {
                tempData.wasoku = foundItem.wasoku;
                tempData.peak = foundItem.peak;
                tempData.yokuyou = foundItem.yokuyou;
                tempData.wasokuat = foundItem.wasokuat;
                tempData.onteiat = foundItem.onteiat;
                tempData.volume = foundItem.volume;
                tempData.echo = foundItem.echo;
                tempData.beforeSilence = foundItem.beforeSilence;
                tempData.afterSilence = foundItem.afterSilence;
            }
        }

        public static bool makeATWaveData(int atid, string kana, string path, int speed, int ontei, bool boyomi, bool change = true)
        {
            string text = kana;
            if (change == true)
            {
                text = STVoiceUtility.changeText(kana);
            }
            if(boyomi == true)
            {
                text = text.Replace("'", "");
            }
            try
            {
                if (atid == 10000)
                {
                    return ATF1.start(path, text, speed, ontei);
                }
                else if (atid == 10001)
                {
                    return ATF2.start(path, text, speed, ontei);
                }
                else if (atid == 10002)
                {
                    return ATM1.start(path, text, speed, ontei);
                }
                else if (atid == 10003)
                {
                    return ATM2.start(path, text, speed, ontei);
                }
                else if (atid == 10004)
                {
                    return ATDVD.start(path, text, speed, ontei);
                }
                else if (atid == 10005)
                {
                    return ATIMD1.start(path, text, speed, ontei);
                }
                else if (atid == 10006)
                {
                    return ATJGR.start(path, text, speed, ontei);
                }
                else
                {
                    return ATR1.start(path, text, speed, ontei);
                }
            }
            catch (Exception /*exp*/)
            {
                return false;
            }
        }

        public static void volumeControl(string path, int volume)
        {
            if(volume == 100)
            {
                return;
            }
            AudioFileReader audioReader = new AudioFileReader(path);
            audioReader.Volume = 0.01f * (float)volume;
            WaveOut waveOut = new WaveOut();
            waveOut.Init(audioReader);
            byte[] samples = new byte[audioReader.Length];
            audioReader.Read(samples, 0, samples.Length);
            audioReader.Dispose();
            WaveFileWriter waveFileWriter = new WaveFileWriter(path, waveOut.OutputWaveFormat);
            waveFileWriter.Write(samples, 0, samples.Length);
            waveFileWriter.Flush();
            waveOut.Dispose();
            waveFileWriter.Dispose();
        }

        public static void savePresetData(List<OnseParamData> tempData, List<TempDataRelation> tempDataRelation)
        {
            string path = System.Environment.CurrentDirectory + "\\setting";
            string filepath = path + "\\setting.csv";
            if (tempDataRelation.Count() == 0)
            {
                File.Delete(filepath);
                return;
            }
            Directory.CreateDirectory(path);
            StreamWriter writer = new StreamWriter(filepath, false, Encoding.UTF8);
            for (int i=0; i< tempDataRelation.Count(); i++)
            {
                OnseParamData foundItem = tempData.Find(item => item.voice == tempDataRelation[i].name);
                writer.WriteLine(tempDataRelation[i].name + "," + tempDataRelation[i].voice + "," + foundItem.volume + "," + foundItem.wasokuat + "," + foundItem.onteiat + "," + foundItem.wasoku + "," + foundItem.peak +"," + foundItem.yokuyou + "," + foundItem.echo);
            }
            writer.Close();
        }

        public static void readPresetData(List<OnseParamData> tempData, List<TempDataRelation> tempDataRelation)
        {
            string path = System.Environment.CurrentDirectory + "\\setting";
            string filepath = path + "\\setting.csv";
            if (!File.Exists(filepath))
            {
                return;
            }
            StreamReader sr = new StreamReader(filepath, Encoding.UTF8);
            while (sr.Peek() != -1)
            {
                string[] array = sr.ReadLine().Split(',');
                if (array.Length < 8)
                {
                    continue;
                }
                OnseParamData tempData1 = new OnseParamData();
                tempData1.voice = array[0];
                tempData1.volume = int.Parse(array[2]);
                tempData1.wasokuat= int.Parse(array[3]);
                tempData1.onteiat= int.Parse(array[4]);
                tempData1.wasoku = double.Parse(array[5]);
                tempData1.peak = double.Parse(array[6]);
                tempData1.yokuyou = double.Parse(array[7]);
                if (array.Length == 9)
                {
                    tempData1.echo = bool.Parse(array[8]);
                }

                TempDataRelation tempDataRelation1 = new TempDataRelation();
                tempDataRelation1.voice = array[1];
                tempDataRelation1.name= array[0];

                tempData.Add(tempData1);
                tempDataRelation.Add(tempDataRelation1);
            }
            sr.Close();
        }

        public static string getVoice(string combobox, List<TempDataRelation> tempDataRelation)
        {
            TempDataRelation foundItem2 = tempDataRelation.Find(item => item.name == combobox);
            if(foundItem2 != null)
            {
                return foundItem2.voice;
            }
            else
            {
                return combobox;
            }
        }


        public static void setOtherData(OtherData data)
        {
            var xs = new XmlSerializer(typeof(OtherData));
            string path = System.Environment.CurrentDirectory + "\\setting\\othersetting.xml";
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                xs.Serialize(sw, data);
            }
        }

        public static OtherData getOtherData()
        {
            string path = System.Environment.CurrentDirectory + "\\setting\\othersetting.xml";
            if (!File.Exists(path))
            {
                return new OtherData();
            }
            var xs = new XmlSerializer(typeof(OtherData));
            OtherData otherData;
            using (var sr = new StreamReader(path, Encoding.UTF8))
            using (var xr = System.Xml.XmlReader.Create(sr))
            {
                otherData = (OtherData)xs.Deserialize(xr);
            }
            return otherData;
        }

        public static void setEcho(string inputPath, int delayMs = 100, float feedback = 0.4f, float wet = 0.4f)
        {
            if (!File.Exists(inputPath)) throw new FileNotFoundException(inputPath);
            if (delayMs < 1) delayMs = 1;
            feedback = Math.Max(0f, Math.Min(0.95f, feedback));
            wet = Math.Max(0f, Math.Min(1f, wet));
            float dry = 1f - wet;

            int sampleRate, channels;
            float[] srcAll;

            // 1) 全サンプルを float[] に読み込んでから reader を破棄（ロック解除）
            using (var reader = new AudioFileReader(inputPath)) // 32-bit float へ展開される
            {
                sampleRate = reader.WaveFormat.SampleRate;
                channels = reader.WaveFormat.Channels;

                var buf = new float[sampleRate * channels]; // チャンク読み込み用
                var list = new List<float>((int)(reader.Length / 4));
                int read;
                while ((read = reader.Read(buf, 0, buf.Length)) > 0)
                    list.AddRange(new ArraySegment<float>(buf, 0, read));

                srcAll = list.ToArray();
            } // ここでファイルロック解除

            // 2) オフラインでエコー処理（原音 + 短いテイル分のゼロ入力）
            int delaySamplesPerCh = sampleRate * delayMs / 1000;
            int delayLineLen = Math.Max(1, delaySamplesPerCh) * channels;
            var delayLine = new float[delayLineLen];
            int w = 0;

            int tailMs = delayMs * 4; // ←短めテイル
            int tailSamples = sampleRate * channels * tailMs / 1000;

            var output = new float[srcAll.Length + tailSamples];

            // 元データ
            for (int i = 0; i < srcAll.Length; i++)
            {
                float input = srcAll[i];
                float echo = delayLine[w];
                float y = dry * input + wet * echo;
                // クリップ簡易防止
                if (y > 1f) y = 1f; else if (y < -1f) y = -1f;
                output[i] = y;

                float fb = input + echo * feedback;
                if (fb > 1f) fb = 1f; else if (fb < -1f) fb = -1f;
                delayLine[w] = fb;

                if (++w >= delayLine.Length) w = 0;
            }
            // テイル（ゼロ入力を流して残響を吐き切る）
            for (int i = 0; i < tailSamples; i++)
            {
                float echo = delayLine[w];
                float y = wet * echo; // input=0 なので dry*0 は省略
                if (y > 1f) y = 1f; else if (y < -1f) y = -1f;
                output[srcAll.Length + i] = y;

                float fb = echo * feedback; // input=0
                if (fb > 1f) fb = 1f; else if (fb < -1f) fb = -1f;
                delayLine[w] = fb;

                if (++w >= delayLine.Length) w = 0;
            }

            // 3) 同じパスに上書き保存（ここで初めて書く）
            //    まず元ファイルを削除してから、同じファイル名で保存します。
            //    ※下は IEEE float WAV で保存（簡単＆劣化なし）。16bit が良ければ後述。
            File.Delete(inputPath);
            using (var writer = new WaveFileWriter(inputPath, WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels)))
            {
                writer.WriteSamples(output, 0, output.Length);
            }

        }

        public static void addSilent(string wavPath, int beforeMs, int afterMs)
        {
            string tmp = Path.Combine(Path.GetDirectoryName(wavPath)!, Guid.NewGuid() + ".wav");

            using (var r = new WaveFileReader(wavPath))
            {
                var wf = r.WaveFormat;
                // ★ PCM16 以外なら何もしない（AquesTalkは通常ここを通る＝PCM16）
                //if (wf.Encoding != WaveFormatEncoding.Pcm || wf.BitsPerSample != 16)
                //    return;

                using (var w = new WaveFileWriter(tmp, wf))
                {
                    int sr = wf.SampleRate;           // ★ ヘッダ書換え後のSR
                    int block = wf.BlockAlign;

                    long beforeSamples = (long)Math.Round(beforeMs * sr / 1000.0);
                    long afterSamples = (long)Math.Round(afterMs * sr / 1000.0);

                    // 前無音（フレーム単位でゼロ書き）
                    WriteZeros(w, beforeSamples, block);

                    // 元音声コピー（bytesRead厳守）
                    byte[] buf = new byte[wf.AverageBytesPerSecond];
                    int bytesRead;
                    while ((bytesRead = r.Read(buf, 0, buf.Length)) > 0)
                        w.Write(buf, 0, bytesRead);

                    // 後無音
                    WriteZeros(w, afterSamples, block);
                }
            }

            File.Delete(wavPath);
            File.Move(tmp, wavPath);
        }

        private static void WriteZeros(WaveFileWriter w, long frames, int blockAlign)
        {
            if (frames <= 0) return;
            byte[] zeros = new byte[blockAlign * 4096];
            long remain = frames;
            while (remain > 0)
            {
                int f = (int)Math.Min(4096, remain);
                w.Write(zeros, 0, f * blockAlign);
                remain -= f;
            }
        }

        public static bool getAquesTalk()
        {
            return (ConfigurationManager.AppSettings["aquestalk"] == "true") ? true : false;
        }

    }
}
