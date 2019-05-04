using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace iDEdge
{
    class Program
    {
        public const string ver = "1.0.2";

        static void Main(string[] args)
        {
            Console.WriteLine($"iDEdge {ver}");
            if (args.Length == 0)
            {
                Console.WriteLine("用法:\n" +
                    "iDEdge.exe [网易云链接|歌名]");
                Environment.Exit(2);
            }
            string id = args[0];
            if (id == "server")
            {
                string ip = "localhost";
                if (args.Length == 2)
                    ip = args[1];
                Microsoft.Owin.Hosting.WebApp.Start<Startup>($"http://{ip}:9000");
                Console.WriteLine($"API 服务已在 http://{ip}:9000 启动");
                while (true)
                    Console.ReadLine();
            }
            else
                Environment.Exit(Make(id));
        }

        public static int Make(string id)
        {
            if (id == null || id == "")
                return 1;
            string dir = Environment.GetEnvironmentVariable("temp") + "\\" + DateTime.Now.ToBinary().ToString() + "\\";
            string lrc = "";
            string name = "";
            Directory.CreateDirectory(dir);
            if (id.IndexOf("http") > -1)
            {
                Console.WriteLine("检测到分享链接");
                if (id.IndexOf('?') > -1)
                {
                    id = id.GetRight("id=");
                    if (id.IndexOf("&") > -1)
                        id = id.GetLeft("&");
                }
                else if (id.IndexOf("/song/") > -1)
                {
                    id = id.GetRight("/song/");
                    id = id.Trim('/');
                }
            }
            else
            {
                Console.WriteLine("可能是歌名搜索");
                string url = $"https://api.bzqll.com/music/netease/search?key=579621905&s={id}&type=song&limit=1&offset=0";
                string r = GetWebText(url);
                id = r.Between("\"id\":\"", "\"");
            }



            string lrcaddr = $"https://api.bzqll.com/music/netease/lrc?key=579621905&id={id}";
            string mp3 = $"https://api.bzqll.com/music/netease/url?key=579621905&id={id}&br=999000";
            string nameaddr = $"https://api.bzqll.com/music/netease/song?key=579621905&id={id}";
            WebClient webClient = new WebClient();
            name = GetWebText(nameaddr);
            name = name.Between("\"name\":\"", "\"");
            Console.WriteLine(name);
            webClient.DownloadFile(mp3, dir + "mp3");
            lrc = GetWebText(lrcaddr);


            Regex timeReg = new Regex(@"(?<=^\[)(\d|\:|\.)+(?=])");
            Regex strReg = new Regex(@"(?<=]).+", RegexOptions.RightToLeft);
            string[] lrcLines = lrc.Split('\n');
            lrc = "[Script Info]\nTitle: Convented Sub\nScriptType: v4.00+\nWrapStyle: 0\nScaledBorderAndShadow: yes\nYCbCr Matrix: None\n\n[V4 + Styles]\nFormat: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding\nStyle: Default,微软雅黑,30,&H00FFFFFF,&H000000FF,&H00000000,&H00000000,0,0,0,0,100,100,0,0,1,2,2,8,10,10,10,1\n\n[Events]\nFormat: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text\n";
            TimeSpan last = TimeSpan.Zero;
            string lastlrc = $"iDEdge {ver} 生成的室内操";
            foreach (string line in lrcLines)
            {
                if (line.Trim() == "")
                    continue;
                Match timeMatch = timeReg.Match(line);
                if (!timeMatch.Success)
                    continue;
                Match strMatch = strReg.Match(line);
                TimeSpan time = TimeSpan.Parse("00:" + timeMatch.Value);
                if (lastlrc.Trim() != "")
                    lrc += "Dialogue: 0," + string.Format("{0:d2}:{1:d2}:{2:d2}.{3:d2}", last.Hours, last.Minutes, last.Seconds, last.Milliseconds / 10) +
                        "," + string.Format("{0:d2}:{1:d2}:{2:d2}.{3:d2}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds / 10) +
                        ",Default,,0,0,0,," + lastlrc.Trim() + "\n";
                last = time;
                lastlrc = strMatch.Success ? strMatch.Value : "";
            }
            TimeSpan timelast = last.Add(TimeSpan.FromSeconds(10));
            if (lastlrc.Trim() != "")
                lrc += "Dialogue: 0," + string.Format("{0:d2}:{1:d2}:{2:d2}.{3:d2}", last.Hours, last.Minutes, last.Seconds, last.Milliseconds / 10) +
                    "," + string.Format("{0:d2}:{1:d2}:{2:d2}.{3:d2}", timelast.Hours, timelast.Minutes, timelast.Seconds, timelast.Milliseconds / 10) +
                    ",Default,,0,0,0,," + lastlrc.Trim() + "\n";

            File.WriteAllText(dir + "lrc", lrc, Encoding.UTF8);
            Process merge = new Process();
            merge.StartInfo.CreateNoWindow = false;
            merge.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\MkvMerge.exe";
            merge.StartInfo.UseShellExecute = false;
            merge.StartInfo.Arguments = "--ui-language zh_CN " +
                $"--output \"{Environment.CurrentDirectory}\\{name}.mkv\" " +
                $"--language 0:eng ( \"{AppDomain.CurrentDomain.BaseDirectory}\\res.pak\" ) " +
                $"--language 0:und ( \"{dir}mp3\" ) " +
                $"--language 0:und ( \"{dir}lrc\" ) --track-order 0:0,1:0,2:0";
            merge.StartInfo.RedirectStandardOutput = true;
            merge.Start();
            merge.WaitForExit();
            string output = merge.StandardOutput.ReadToEnd();
            merge.Close();
            if (File.Exists($"{Environment.CurrentDirectory}\\{name}.mkv"))
                Console.WriteLine("成功");
            else
            {
                Console.WriteLine("失败");
                File.WriteAllText(dir + "log", output);
                Console.WriteLine($"日志已保存到 {dir}");
                return 4;
            }
            return 0;
        }

        static string GetWebText(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream datastream = response.GetResponseStream();
            StreamReader reader = new StreamReader(datastream, Encoding.UTF8);
            string result = reader.ReadToEnd();
            reader.Close();
            datastream.Close();
            response.Close();
            return result;
        }
    }
    public static class StringHelper
    {
        /// <summary>
        /// 取文本左边内容
        /// </summary>
        /// <param name="str">文本</param>
        /// <param name="s">标识符</param>
        /// <returns>左边内容</returns>
        public static string GetLeft(this string str, string s)
        {
            string temp = str.Substring(0, str.IndexOf(s));
            return temp;
        }


        /// <summary>
        /// 取文本右边内容
        /// </summary>
        /// <param name="str">文本</param>
        /// <param name="s">标识符</param>
        /// <returns>右边内容</returns>
        public static string GetRight(this string str, string s)
        {
            string temp = str.Substring(str.IndexOf(s) + s.Length);
            return temp;
        }

        /// <summary>
        /// 取文本中间内容
        /// </summary>
        /// <param name="str">原文本</param>
        /// <param name="leftstr">左边文本</param>
        /// <param name="rightstr">右边文本</param>
        /// <returns>返回中间文本内容</returns>
        public static string Between(this string str, string leftstr, string rightstr)
        {
            int i = str.IndexOf(leftstr) + leftstr.Length;
            string temp = str.Substring(i, str.IndexOf(rightstr, i) - i);
            return temp;
        }
    }
}
