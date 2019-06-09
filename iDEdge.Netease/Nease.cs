using System;
using System.IO;
using System.Net;
using System.Text;

namespace iDEdge.Netease
{
    class Nease
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"iDEdge {Core.ver}");
            if (args.Length == 0)
            {
                Console.WriteLine("用法:\n" +
                    "iDEdge.exe [网易云链接|歌名]");
                Environment.Exit(2);
            }
            string id = args[0];
            Environment.Exit(Nease.Make(id));

        }
        public static int Make(string id)
        {
            if (id == null || id == "")
                return 1;
            string dir = Environment.GetEnvironmentVariable("temp") + "\\" + DateTime.Now.ToBinary().ToString() + "\\";
            Directory.CreateDirectory(dir);
            if (id.IndexOf("http") > -1)
            {
                Console.WriteLine("检测到分享链接");
                id = Url2Id(id);
            }
            else
            {
                Console.WriteLine("可能是歌名搜索");
                id = Name2Id(id);
            }
            string name = Id2Name(id);
            Console.WriteLine(name);
            IdDownMp3(id, dir + "mp3");
            string lrc = Id2Lrc(id);
            lrc = Core.Lrc2Ass(lrc, $"iDEdge {Core.ver} 生成的室内操");

            File.WriteAllText(dir + "lrc", lrc, Encoding.UTF8);
            string output = Core.Merge(dir, name);
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
        public static string Id2Lrc(string id)
        {
            string lrcaddr = $"https://v1.itooi.cn/netease/lrc?id={id}";
            return Core.GetWebText(lrcaddr);
        }

        public static void IdDownMp3(string id, string path)
        {
            string mp3 = $"https://v1.itooi.cn/netease/url?id={id}&quality=flac";
            WebClient webClient = new WebClient();
            webClient.DownloadFile(mp3, path);
        }

        public static string Id2Name(string id)
        {
            string nameaddr = $"https://v1.itooi.cn/netease/song?id={id}";
            string name = Core.GetWebText(nameaddr);
            return name.LastBetween("\"name\":\"", "\"");
        }

        public static string Name2Id(string id)
        {
            string url = $"https://v1.itooi.cn/netease/search?keyword={id}&type=song&pageSize=1";
            string r = Core.GetWebText(url);
            return r.Between("\"id\":", ",");
        }

        public static string Url2Id(string id)
        {
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
            return id;
        }

    }
}
