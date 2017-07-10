using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TaxiCallCenter.MVP.WpfApp.Client
{
    public class SpeechKitClient
    {
        private readonly String apiKey;

        public SpeechKitClient(String apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<Byte[]> GenerateAsync(String speaker, String emotion, String text)
        {
            using (var client = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(new Dictionary<String, String>
                {
                    ["key"] = this.apiKey,
                    ["text"] = text,
                    ["format"] = "wav",
                    ["quailty"] = "hi",
                    ["lang"] = "ru-RU",
                    ["speaker"] = speaker,
                    ["speed"] = "1.0",
                    ["emotion"] = emotion
                }))
                {
                    var query = await content.ReadAsStringAsync();
                    var response = await client.GetAsync($"https://tts.voicetech.yandex.net/generate?{query}");
                    response.EnsureSuccessStatusCode();

                    var responseBytes = await response.Content.ReadAsByteArrayAsync();
                    return responseBytes;
                }
            }
        }

        public async Task<String> RecognizeAsync(Guid userId, String topic, Byte[] bytes)
        {
            //var uri = "https://asr.yandex.net/asr_xml";
            using (var client = new HttpClient())
            {
                using (var queryParams = new FormUrlEncodedContent(new Dictionary<String, String>
                {
                    ["uuid"] = userId.ToString("N"),
                    ["key"] = this.apiKey,
                    ["topic"] = topic,
                    ["lang"] = "ru-RU"
                }))
                {
                    var query = await queryParams.ReadAsStringAsync();
                    using (var byteContent = new ByteArrayContent(bytes))
                    {
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("audio/x-wav");
                        var response = await client.PostAsync($"https://asr.yandex.net/asr_xml?{query}", byteContent);
                        response.EnsureSuccessStatusCode();

                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }
    }
}
