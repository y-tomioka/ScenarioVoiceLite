using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static STVoice.AudioQuery;

namespace STVoice
{
    [JsonObject("AudioQuery")]
    public class AudioQuery
    {
        public AudioQuery()
        {
            accent_phrases = new List<AccentPhrase>();
        }
        [JsonObject("Mora")]
        public class Mora
        {
            [JsonProperty("text")]
            public string? text;
            [JsonProperty("consonant")]
            public string? consonant;
            [JsonProperty("consonant_length")]
            public double? consonant_length;
            [JsonProperty("vowel")]
            public string? vowel;
            [JsonProperty("vowel_length")]
            public double? vowel_length;
            [JsonProperty("pitch")]
            public double? pitch;
        }
        [JsonObject("PauseMora")]
        public class PauseMora
        {
            [JsonProperty("text")]
            public string? text;
            [JsonProperty("consonant")]
            public string? consonant;
            [JsonProperty("consonant_length")]
            public double? consonant_length;
            [JsonProperty("vowel")]
            public string? vowel;
            [JsonProperty("vowel_length")]
            public double? vowel_length;
            [JsonProperty("pitch")]
            public double? pitch;
        }
        [JsonObject("AccentPhrase")]
        public class AccentPhrase
        {
            public AccentPhrase()
            {
                moras = new List<Mora>();
                pause_mora = new PauseMora();
            }
            [JsonProperty("moras")]
            public List<Mora> moras;
            [JsonProperty("accent")]
            public double? accent;
            [JsonProperty("pause_mora")]
            public PauseMora pause_mora;
            [JsonProperty("is_interrogative")]
            public bool is_interrogative;
        }
        [JsonProperty("accent_phrases")]
        public List<AccentPhrase> accent_phrases;
        [JsonProperty("speedScale")]
        public double? speedScale;
        [JsonProperty("pitchScale")]
        public double? pitchScale;
        [JsonProperty("intonationScale")]
        public double? intonationScale;
        [JsonProperty("volumeScale")]
        public double? volumeScale;
        [JsonProperty("prePhonemeLength")]
        public double? prePhonemeLength;
        [JsonProperty("postPhonemeLength")]
        public double? postPhonemeLength;
        [JsonProperty("outputSamplingRate")]
        public double? outputSamplingRate;
        [JsonProperty("outputStereo")]
        public bool outputStereo;
        [JsonProperty("kana")]
        public string kana;
    }

    [JsonObject("Speakers")]
    public class Speakers
    {
        [JsonObject("Styles")]
        public class Styles
        {
            [JsonProperty("name")]
            public string name;
            [JsonProperty("id")]
            public int id;

            public Styles()
            {
                name = "";
                id = 0;
            }
        }
        [JsonProperty("name")]
        public string name;
        [JsonProperty("speaker_uuid")]
        public string speaker_uuid;
        [JsonProperty("styles")]
        public List<Styles> styles;
        [JsonProperty("version")]
        public string version;

        public Speakers()
        {
            name = "";
            speaker_uuid = "";
            styles = new List<Styles>();
            version = "";
        }
    }


    internal class JsonControl
    {
        public List<Speakers> deserializeSpeakersJson(string json)
        {
            List<Speakers> speakers = JsonConvert.DeserializeObject<List<Speakers>>(json);
            return speakers;
        }

        public AudioQuery deserializeAudioQueryJson(string json)
        {
            AudioQuery query = JsonConvert.DeserializeObject<AudioQuery>(json);
            return query;
        }

        public List<AccentPhrase> deserializeAccentPhraseJson(string json)
        {
            List<AccentPhrase> query = JsonConvert.DeserializeObject<List<AccentPhrase>>(json);
            return query;
        }

        public string serializeAudioQueryJson(AudioQuery data)
        {
            string jsonStr = JsonConvert.SerializeObject(data);
            return jsonStr;
        }

        public List<AccentPhrase> getAccentData(AudioQuery query)
        {
            return query.accent_phrases;
        }

        public string serializeAccentQueryJson(List<AccentPhrase> data)
        {
            string jsonStr = JsonConvert.SerializeObject(data);
            return jsonStr;
        }

        public List<AccentPhrase> deserializeAccentQueryJson(string json)
        {
            List<AccentPhrase> query = JsonConvert.DeserializeObject<List<AccentPhrase>>(json);
            return query;
        }
    }
}
