using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WarnMe_Cherry.Extensions
{
    class Wetter
    {
        public int Forecast { get; set; } = 3;
        /// <summary>
        /// WetterCode is of type <see cref="string"/>
        /// </summary>
        public string WetterCode { get; set; } = "GMXX3719";

        readonly string websiteUrl = "http://wxdata.weather.com/wxdata/weather/local/{0}?cc=*&unit=m&dayf={1}";
        /// <summary>
        /// WebsiteUrl uses <see cref="WetterCode"/> and <see cref="Forecast"/> to create an Url
        /// </summary>
        public string WebsiteUrl => "https://www.google.com/search?q=wetter"; // { get => string.Format(websiteUrl, WetterCode, Forecast); }
        /// <summary>
        /// 
        /// </summary>
        public string WetterPattern { get; set; } = ".*?<head>.*?<ut>(.*?)</ut>.*?<ud>(.*?)</ud>.*?<us>(.*?)</us>.*?<up>(.*?)</up>.*?</head>.*?<dnam>(.*?),(.*?),(.*?)</dnam>.*?<lat>(.*?)</lat>.*?<lon>(.*?)</lon>.*?<sunr>(.*?)</sunr>.*?<suns>(.*?)</suns>.*?<zone>(.*?)</zone>.*?<tmp>(.*?)</tmp>.*?<flik>(.*?)</flik>.*?<t>(.*?)</t>.*?<icon>(.*?)</icon>.*?<r>(.*?)</r>.*?<d>(.*?)</d>.*?<s>(.*?)</s>.*?<d>(.*?)</d>.*?<t>(.*?)</t>.*?<hmid>(.*?)</hmid>.*?<vis>(.*?)</vis>.*?<day d=\"1\" t=\"(.*?)\".*?<hi>(.*?)</hi>.*?<low>(.*?)</low>.*?<icon>(.*?)</icon>.*?<t>(.*?)</t>.*?<day d=\"2\" t=\"(.*?)\".*?<hi>(.*?)</hi>.*?<low>(.*?)</low>.*?<icon>(.*?)</icon>.*?<t>(.*?)</t>.*?";

        private string GetWeatherPattern(string id) => string.Format(@"(?<=id=""{0}"")[^>]*?>(.*?)<", id);

        public System.Collections.Generic.Dictionary<string, string> WeatherData { get; set; } = new System.Collections.Generic.Dictionary<string, string>()
        {
            {"wob_loc", "" },
            {"wob_dts", "" },
            {"wob_", "" },
            {"wob_", "" },
            {"wob_", "" },
            {"wob_", "" },
            {"wob_", "" },
            {"wob_", "" },
            {"wob_", "" },
            {"wob_", "" },
            {"wob_", "" },
            {"wob_", "" },
            {"wob_", "" },
            {"wob_", "" }
        };

        /// <summary>
        /// Variablen:
        /// <para>(1-4)		Einheiten	[Temperatur, Entfernung, Geschwindigkeit, Druck]</para>
        /// <para>(5-10)	Werte		[Stadt, Region, Land, Breitengrad, Längengrad, Zeitzone]</para>
        /// <para>(11-15)				[Sonnenaufgang, Sonnenuntergang, Temperatur, Beschreibung, Bild]</para>
        /// <para>(16-23)				[Gefühlt, Wind, Richtung, Kardinal, Druck, Anstieg, Feuchtigkeit, Sichtweite]</para>
        /// <para>(24-29)	Tag 2		[Datum, Hoch, Tief, Durchschnitt, Bild, Beschreibung]</para>
        /// <para>(30-35)	Tag 3		[Datum, Hoch, Tief, Durchschnitt, Bild, Beschreibung]</para>
        /// </summary>
        public Wetter()
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(WetterPattern,
                System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            string tmp;
            try
            {
                tmp = GetWebsiteString(WebsiteUrl);
            }
            catch (WebException)
            {
                tmp = "lol";
            }
            System.Windows.MessageBox.Show(tmp);
            
            foreach (System.Text.RegularExpressions.Match match in  r.Matches(tmp))
            {
                foreach (System.Text.RegularExpressions.Group group in match.Groups)
                {
                    //System.Windows.MessageBox.Show(group.Value, group.Name);
                }
            }

        }

        private string GetWebsiteString(string url)
        {

            //System.Net.WebClient web = new System.Net.WebClient();
            //string tmp = web.DownloadString(WebsiteUrl);
            //System.Text.RegularExpressions.MatchCollection match = r.Matches(tmp);
            var task = MakeAsyncRequest(WebsiteUrl, "text/html");
            System.Windows.MessageBox.Show(task.Result);

            WebClient client = new WebClient
            {
                Proxy = WebRequest.DefaultWebProxy
            };
            client.Proxy.Credentials = CredentialCache.DefaultCredentials;
            string tmp = client.DownloadString(url);
            return tmp;
        }

        // Define other methods and classes here
        public static Task<string> MakeAsyncRequest(string url, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = contentType;
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = 5000;
            request.Proxy = WebRequest.DefaultWebProxy;
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;

            Task<WebResponse> task = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult),
                null);

            try
            {
                return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
            }
            catch (WebException)
            {
                return new Task<string>(delegate { return "nyx"; });
            }
        }

        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }

    }
}
