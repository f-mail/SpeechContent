using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpeechContent
{
    public static class SpeechContentOps
    {
        private static string BaseFolder = @"E:\Google Drive\+SpeechContent";
        private static int MaxItems = 50;
        private static readonly char PrefixURL = '>';

        public static void Init()
        {
            //not implemented
        }

        public static void Init(string baseFolder, int maxItems)
        {
            BaseFolder = baseFolder;
            MaxItems = maxItems;
        }

        public static async Task<SpeechContentData> GetContentData()
        {
            List<SpeechContentItem> items = new List<SpeechContentItem>();
            for (int i = 1; i <= MaxItems; i++)
            {
                if (!File.Exists(GetFileName(i))) break;

                items.Add(await GetContentItem(i));
            }

            //check fragmentation
            var dir = new DirectoryInfo(Path.Combine(BaseFolder, "content"));
            int countFiles = dir.GetFiles(@"sc_*.txt").Length;
            if (items.Count != countFiles)
            {
                CompressData();
                return await GetContentData();
            }

            return new SpeechContentData(items, GetPosition());            
        }

        public static string AddContent(string txt)
        {
            if (String.IsNullOrEmpty(txt))
                throw new ArgumentException();
            if (txt.Length < 5)
                throw new ArgumentException();

            //primitive return 
            string returnTxt = txt;
            if (returnTxt.Length > 150)
                returnTxt = returnTxt.Substring(0, 147) + "...";

            //check url set
            if (txt.Length < 5000)
            {
                string possibleUrls = txt;
                Regex.Replace(txt, @"\s+", "\n");
                string[] arrUrls = possibleUrls.Split('\n');
                if(arrUrls.Length > 0)
                {
                    bool isAllUrl = true;
                    foreach(string url in arrUrls)
                    {
                        if (!IsCheckURL(url.Trim()))
                            isAllUrl = false;
                    }
                    if(isAllUrl)
                    {
                        foreach(string url in arrUrls)
                        {
                            AddContentFile(url, SpeechContentType.URL);
                        }
                        return returnTxt;
                    }
                }
            }

            AddContentFile(txt.Trim(), SpeechContentType.Text);
            return returnTxt;
        }

        public static string AddContentFromClipboard()
        {
            return AddContent(GetTextFromClipboard());
        }

        public static void MoveContentItem(int index, int newIndex)
        {
            MoveContentFile(index, newIndex);
        }

        public static void UpdateContentItem(string txt, int index)
        {
            if (String.IsNullOrEmpty(txt) || txt.Length < 5)
                throw new ArgumentException();
            
            SpeechContentType scType = SpeechContentType.Text;
            if (IsCheckURL(txt.Trim())) scType = SpeechContentType.URL;
            UpdateContentFile(txt.Trim(), scType, index);
        }

        public static void DeleteContentItem(int index)
        {
            DeleteContentFile(index);
        }

        public static int GetPosition()
        {
            StreamReader sr = new StreamReader(Path.Combine(BaseFolder, "content", "position.txt"));
            string sPos = sr.ReadToEnd();
            int pos = Convert.ToInt32(sPos);
            sr.Close();
            return pos;
        }

        public static void SetPosition(int pos)
        {
            var sw = new StreamWriter(Path.Combine(BaseFolder, "content", "position.txt"), false, Encoding.UTF8);
            sw.Write(pos);
            sw.Close();
        }

        public static void DoCompressData()
        {
            CompressData();
        }



        //-------------------------------------------------------------------

        // NON-PUBLIC MEMBERS

        //-------------------------------------------------------------------



        private static int CompressData()
        {
            int pos = GetPosition();
            int countDeleted = 0;
            for(int i = 1; i < pos; i++)
            {
                if (File.Exists(GetFileName(i)))
                {
                    File.Delete(GetFileName(i));
                    countDeleted++;
                }
                    
            }

            int curIndex = 1;
            for(int i = pos; i <= MaxItems; i++)
            {
                if (File.Exists(GetFileName(i)))
                    File.Move(GetFileName(i), GetFileName(curIndex++));
            }

            SetPosition(1);

            return countDeleted;
        }

        private static async Task<SpeechContentItem> GetContentItem(int index)
        {
            if (!File.Exists(GetFileName(index))) throw new ArgumentOutOfRangeException();

            string line = "";
            using (var sr = new StreamReader(GetFileName(index), Encoding.UTF8))
            {
                line = sr.ReadLine();
            }
            
            if (IsCheckURLLine(line))
            {
                
                string url = line.Remove(0, 1);
                string desc = await GetTitleFromURL(url);
                return new SpeechContentItem(index, SpeechContentType.URL, GetFileName(index), desc, url);
            }
            else
            {
                return new SpeechContentItem(index, SpeechContentType.Text, GetFileName(index), line);
            }            
        }

        private static bool IsCheckURL(string txt)
        {
            return Regex.IsMatch(txt, @"(www|http:|https:)+[^\s]+[\w]");
        }

        private static bool IsCheckURLLine(string line)
        {
            if (String.IsNullOrEmpty(line)) return false;
            if (line.Length < 5) return false;
            if (line[0] != PrefixURL) return false;            
            line = line.Remove(0, 1).Trim();
            return IsCheckURL(line);
        }

        private static Dictionary<string, string> CacheTitles = new Dictionary<string, string>();
        private static async Task<string> GetTitleFromURL(string url)
        {
            if (CacheTitles.ContainsKey(url.Trim())) return CacheTitles[url.Trim()];
            string title = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest.Create(url) as HttpWebRequest);
                HttpWebResponse response = (await request.GetResponseAsync() as HttpWebResponse);

                using (Stream stream = response.GetResponseStream())
                {
                    // compiled regex to check for <title></title> block
                    Regex titleCheck = new Regex(@"<title>\s*(.+?)\s*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    int bytesToRead = 8092;
                    byte[] buffer = new byte[bytesToRead];
                    string contents = "";
                    int length = 0;
                    while ((length = stream.Read(buffer, 0, bytesToRead)) > 0)
                    {
                        // convert the byte-array to a string and add it to the rest of the
                        // contents that have been downloaded so far
                        contents += Encoding.UTF8.GetString(buffer, 0, length);

                        Match m = titleCheck.Match(contents);
                        if (m.Success)
                        {
                            // we found a <title></title> match =]
                            title = m.Groups[1].Value.ToString();
                            break;
                        }
                        else if (contents.Contains("</head>"))
                        {
                            // reached end of head-block; no title found =[
                            break;
                        }
                    }
                }
            }
            catch
            {
                title = "Connection Error";
            }

            CacheTitles[url.Trim()] = title;
            return title;
        }

        private static int GetFirstAvailaibleIndex()
        {
            for (int i = 1; i <= MaxItems; i++)
            {
                if (!File.Exists(GetFileName(i)))
                {
                    return i;
                }
            }

            if (CompressData() == 0) throw new OverflowException();
            return GetFirstAvailaibleIndex();            
        }

        private static void AddContentFile(string txt, SpeechContentType scType)
        {
            txt = txt.Trim();
            if (scType == SpeechContentType.URL)
                txt = PrefixURL + txt;

            using (var sw = new StreamWriter(GetFileName(GetFirstAvailaibleIndex()), false, Encoding.UTF8))
            {
                sw.Write(txt);
            }
        }

        private static void UpdateContentFile(string txt, SpeechContentType scType, int index)
        {
            if (scType == SpeechContentType.URL)
                txt = PrefixURL + txt;

            using (var sw = new StreamWriter(Path.Combine(BaseFolder,
                "content", $"sc_{index}.txt"), false, Encoding.UTF8))
            {
                sw.Write(txt);
            }
        }

        private static void DeleteContentFile(int index)
        {            
            if (!File.Exists(GetFileName(index)))
                throw new ArgumentOutOfRangeException();

            File.Delete(GetFileName(index));

            //compress
            for(int i = index+1; i < MaxItems; i++)
            {
                if (!File.Exists(GetFileName(i))) break;
                File.Move(GetFileName(i), GetFileName(i - 1));
            }
        }   
        
        private static void MoveContentFile(int index, int newIndex)
        {
            if (!(File.Exists(GetFileName(index)) &&
                  File.Exists(GetFileName(newIndex))))
                throw new ArgumentOutOfRangeException();

            File.Move(GetFileName(newIndex), GetFileName(10000));
            File.Move(GetFileName(index), GetFileName(newIndex));
            File.Move(GetFileName(10000), GetFileName(index));

        }

        private static string GetFileName(int index)
        {
            return Path.Combine(BaseFolder, "content", $"sc_{index}.txt");
        }

        [STAThread]
        private static string GetTextFromClipboard()
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                string clipboardText = Clipboard.GetText(TextDataFormat.UnicodeText);
                return clipboardText;
            }
            return null;
        }
    }
    

    // ---------------------------------------------------------------

    // CLASSES

    // ---------------------------------------------------------------          
    
    public enum SpeechContentType
    {
        URL,
        Text
    }
    
    public class SpeechContentItem
    {
        public int Index { get; set; }         
        public string Desc { get; set; }
        public SpeechContentType Type { get; set; }

        public string URL { get; set; }

        public string FileName { get; set; }


        public SpeechContentItem(int index, SpeechContentType type, string fileName, string desc = "", string url = "")
        {
            Index = index;
            Type = type;
            Desc = desc;
            URL = url;
            FileName = fileName;
        }
    }

    public class SpeechContentData
    {
        public IEnumerable<SpeechContentItem> Items { get; set; }
        public int Position { get; set; }

        public SpeechContentData(IEnumerable<SpeechContentItem> items, int position)
        {
            Items = items;
            Position = position;
        }
    }
}
