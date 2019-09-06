using NeteaseCloudMusicApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace iDEdge.Netease
{
    public class Nease
    {
        public static void Main(string[] args)
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
            CloudMusicApi api = new CloudMusicApi();
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
            var jobj = Id2JObj(id);
            var name = JObj2Name(jobj);
            Console.WriteLine(name);
            Console.WriteLine(JObj2Singer(jobj));
            Console.WriteLine(JObj2Album(jobj));
            IdDownMp3(id, dir + "mp3");
            string lrc = Id2Lrc(id);
            lrc = Core.Lrc2Ass(lrc, $"iDEdge {Core.ver} 生成的室内操");

            File.WriteAllText(dir + "lrc", lrc, Encoding.UTF8);
            name = name.Replace(":", " ").Replace("?", " ").Replace('"', ' ').Replace("/", " ").Replace("\\", " ").Replace("*", " ").Replace("<", " ").Replace(">", " ").Replace("|", " ");
            string output = Core.Merge(dir, name + ".mkv");
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
        public static int Make(string id, string name)
        {
            string dir = Environment.GetEnvironmentVariable("temp") + "\\" + DateTime.Now.ToBinary().ToString() + "\\";
            Directory.CreateDirectory(dir);
            IdDownMp3(id, dir + "mp3");
            string lrc = Id2Lrc(id);
            lrc = Core.Lrc2Ass(lrc, $"iDEdge {Core.ver} 生成的室内操");

            File.WriteAllText(dir + "lrc", lrc, Encoding.UTF8);
            string output = Core.Merge(dir, name);
            if (File.Exists($"{name}"))
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
            CloudMusicApi api = new CloudMusicApi();
            api.Request(CloudMusicApiProviders.Lyric,
                new Dictionary<string, string>()
                {
                    ["id"] = id
                },
                out JObject obj);
            var res = obj["lrc"].Value<string>("lyric");
            api.Dispose();
            return res;
        }
        public static void IdDownMp3(string id, string path)
        {
            CloudMusicApi api = new CloudMusicApi();
            api.Request(CloudMusicApiProviders.SongUrl,
                new Dictionary<string, string>()
                {
                    ["id"] = id
                },
                out JObject obj);
            var mp3 = obj["data"][0].Value<string>("url");
            api.Dispose();
            WebClient webClient = new WebClient();
            webClient.DownloadFile(mp3, path);
        }
        public static JObject Id2JObj(string id)
        {
            CloudMusicApi api = new CloudMusicApi();
            api.Request(CloudMusicApiProviders.SongDetail,
                new Dictionary<string, string>()
                {
                    ["ids"] = id
                },
                out JObject obj);
            api.Dispose();
            return obj;
        }
        public static async Task<JObject> Id2JObjAsync(string id)
        {
            CloudMusicApi api = new CloudMusicApi();
            (_, var obj) = await api.RequestAsync(CloudMusicApiProviders.SongDetail,
                new Dictionary<string, string>()
                {
                    ["ids"] = id
                });
            api.Dispose();
            return obj;
        }
        public static string JObj2Name(JObject obj) => obj["songs"][0].Value<string>("name");
        public static string JObj2Singer(JObject obj)
        {
            string res = "";
            foreach (var i in obj["songs"][0]["ar"])
            { //适配多个歌手
                res += i.Value<string>("name") + "/";
            }
            res = res.TrimEnd('/');
            return res;
        }
        public static string JObj2Album(JObject obj) => obj["songs"][0]["al"].Value<string>("name");
        public static string JObj2Pic(JObject obj) => obj["songs"][0]["al"].Value<string>("picUrl");

        public static string Name2Id(string id)
        {
            CloudMusicApi api = new CloudMusicApi();
            api.Request(CloudMusicApiProviders.Search,
                new Dictionary<string, string>()
                {
                    ["keywords"] = id,
                    ["limit"] = "1"
                },
                out JObject obj);
            id = obj["result"]["songs"][0].Value<int>("id").ToString();
            api.Dispose();
            return id;
        }
        public static async Task<string> Name2IdAsync(string id)
        {
            CloudMusicApi api = new CloudMusicApi();
            (_, var obj) = await api.RequestAsync(CloudMusicApiProviders.Search,
                new Dictionary<string, string>()
                {
                    ["keywords"] = id,
                    ["limit"] = "1"
                });
            id = obj["result"]["songs"][0].Value<int>("id").ToString();
            api.Dispose();
            return id;
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
