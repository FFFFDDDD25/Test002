using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Google.Cloud.Diagnostics.AspNetCore;
using FluentEmail.Mailgun;
using FluentEmail;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Net;
using HtmlAgilityPack;
using System.Text;

using MongoDB.Driver;
using MongoDB.Bson;

using MongoDB.Bson.Serialization.Attributes;

namespace Test002
{


    public class CrawlModel
    {
        public string name;
        public string regex;
        public int group;

        public CrawlModel(string name, string regex, int group)
        {
            this.name = name;
            this.regex = regex;
            this.group = group;
        }

    }

    public class Startup
    {

        private IConfiguration _config;
        //   Here we are using Dependency Injection to inject the Configuration object



        public void Compare_Send_WaitHour(Dictionary<string, string> dic1, ref Dictionary<string, string> dic2, string emailTitle)
        {
            if (dic1 == null || dic1.Count == 0)
            {
                return;
            }


            if (dic2 == null)
            {
                dic2 = dic1.ToDictionary(entry => entry.Key,
                                       entry => entry.Value);
                return;
            }

            var content = "";

            foreach (KeyValuePair<string, string> kv in dic1)
            {
                if (!dic2.ContainsKey(kv.Key))
                {
                    content += String.Format(
                        @"
                                target : {0}
                                insert : {2}
                                ", kv.Key, "none", kv.Value); content += "\n";

                    dic2[kv.Key] = kv.Value;
                }
                else if (kv.Value != dic2[kv.Key])
                {
                    content += String.Format(
                        @"
                                target : {0}
                                origin : {1}
                                update : {2}
                                ", kv.Key, dic2[kv.Key], kv.Value); content += "\n";

                    dic2[kv.Key] = kv.Value;
                }
            }

            List<string> temp = new List<string>();
            foreach (KeyValuePair<string, string> kv in dic2)
            {
                if (!dic1.ContainsKey(kv.Key))
                {
                    content += String.Format(
                        @"
                                target : {0}
                                origin : {1}
                                delete
                                ", kv.Key, kv.Value); content += "\n";

                    temp.Add(kv.Key);
                }
            }
            foreach (var t in temp)
            {
                dic2.Remove(t);
            }






            if (content != "")
            {
                var response = Send(emailTitle, instanceNum + ":::::::" + content);

                if (!response.Successful)
                {
                    Console.WriteLine("fail::::" + JsonConvert.SerializeObject(response.ErrorMessages));

                }
                else
                {
                }
            }
            Thread.Sleep(TimeSpan.FromHours(1));
        }

        public HtmlDocument GetPtt18(string 網址1)
        {
            var jsonResponse = "";
            try{
                
            var webRequest = System.Net.WebRequest.Create(網址1);
            if (webRequest != null)
            {
                webRequest.Headers.Add("cookie", "over18=1;");

                using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                    {
                        jsonResponse = sr.ReadToEnd();
                        //Send("sfsdffsf",jsonResponse);
                    }
                }
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(jsonResponse);

            return doc;
            
            }
            catch(Exception ex){
                log.LogError("開網址出錯 網址:"+網址1);
                log.LogError(ex.ToString());

            }
            return null;
        }
        public bool GetPtt(ref HashSet<string> hash, string 網址1, List<string> 目標們, bool AndorOr, int pages)
        {
            /*
            import requests
            r = requests.Session()
            payload ={
                "from":"/bbs/Gossiping/index.html",
                "yes":"yes"
            }
            r1 = r.post("https://www.ptt.cc/ask/over18?from=%2Fbbs%2FGossiping%2Findex.html",payload)
            r2 = r.get("https://www.ptt.cc/bbs/Gossiping/index.html")
            print(r2.text)
            */
            //btn-big
            //button
            //over18-button-container

            var hashCount = hash.Count;

            for (int i = 0; i < pages; i++)
            {
                var doc = GetPtt18(網址1);
                if(doc==null){
                    return false;
                }
                var oneNode = doc.DocumentNode.SelectSingleNode(
                    "/html/body/div[2]/div[1]/div/div[2]/a[2]"
                    .Replace("/tbody", ""));

                var testsdf = new Regex(@"href=\""(.+)\""", RegexOptions.Compiled).Matches(oneNode.OuterHtml);

                網址1 = @"https://www.ptt.cc/" + testsdf[0].Groups[1];


                //"/html/body/div[2]/div[2]/div[所有可能數字]/div[2]/a"
                var linkNodes = doc.DocumentNode.SelectNodes(
                    "/html/body/div[2]/div[2]/div/div[2]/a"
                    .Replace("/tbody", ""));

                bool hasNew = false;
                bool hasOld = false;
                foreach (var linkNode in linkNodes)
                {
                    var test = new Regex(@"a href=\""(.+)\""", RegexOptions.Compiled).Matches(linkNode.OuterHtml);

                    var 網址2 = @"https://www.ptt.cc/" + test[0].Groups[1];


                    if (hash.Contains(網址2))
                    {
                        hasOld = true;
                        continue;
                    }
                    else
                    {
                        hasNew = true;
                        hash.Add(網址2);
                    }

                    var doc2 = GetPtt18(網址2);
                    if(doc2==null){
                        continue;
                    }
                    var oneNode2 = doc2.DocumentNode.SelectSingleNode(
                        @"/html/body/div[3]/div[1]"
                        .Replace("/tbody", ""));



                    if (AndorOr)
                    {
                        bool 全部都有 = true;

                        foreach (var 目標 in 目標們)
                        {
                            if (!oneNode2.InnerText.Contains(目標))
                            {
                                全部都有 = false;
                                break;
                            }
                        }

                        if (全部都有)
                        {
                            Send(instanceNum + ":::::::" + "找到批踢踢", oneNode2.InnerText);
                        }
                    }
                    else
                    {
                        bool 全部沒有 = true;

                        foreach (var 目標 in 目標們)
                        {
                            if (oneNode2.InnerText.Contains(目標))
                            {
                                全部沒有 = false;
                                break;
                            }
                        }

                        if (!全部沒有)
                        {
                            Send(instanceNum + ":::::::" + "找到批踢踢", oneNode2.InnerText);
                        }
                    }


                }
                if (!hasNew)
                {
                    // Send(instanceNum+":::::::"+"找到批踢踢 公告","沒新的");
                    return true;
                }
                else
                {
                    
                    if(i+1==pages && hashCount != 0)
                    {
                        
                        Send(instanceNum+":::::::"+"找到批踢踢 公告",網址1+"  更新太快  可能要加大頁數或是縮短更新時間  ");
                        if(hasOld){
                            log.LogWarning(網址1+"  更新太快 但還好還有舊的    可能要加大頁數或是縮短更新時間");
                        }else{
                            log.LogWarning(網址1+"  更新太快 嚴重到沒有舊的    可能要加大頁數或是縮短更新時間");
                        }
                    }
                    //Send(instanceNum+":::::::"+"找到批踢踢 公告","有新的");
                }
            }
            return true;
        }


        public void TestLog()
        {
            var msg = "";
            msg = "~~~~Error";log.LogError(msg);Console.WriteLine(msg);Thread.Sleep(200);
            msg = "~~~~Debug";log.LogDebug(msg);Console.WriteLine(msg);Thread.Sleep(200);
            msg = "~~~~Information";log.LogInformation(msg);Console.WriteLine(msg);Thread.Sleep(200);
            msg = "~~~~Critical";log.LogCritical(msg);Console.WriteLine(msg);Thread.Sleep(200);
            msg = "~~~~Trace";log.LogTrace(msg);Console.WriteLine(msg);Thread.Sleep(200);
            msg = "~~~~Warning";log.LogWarning(msg);Console.WriteLine(msg);Thread.Sleep(200);
        }

        
        public void Comment()
        {
            var sdfsfd = @"";
            return;
            //關於 vmmem 這個程序占用太大記憶體解法:
            //建立檔案  C:\Users\User\.wslconfig
            //內容:
            //[wsl2]
            //memory=數字GB   # Limits VM memory in WSL 2 to 4 GB
            //processors=數字 # Makes the WSL 2 VM use two virtual processors
            //
            //然後重啟server  使用admin全縣的Powershell指令:  Restart-Service LxssManager
        }

//  docker pull severalnines/clustercontrol
//  docker run -d -p 5000:80 severalnines/clustercontrol
//  http://localhost:5000/   或者試試  http://localhost:5000/clustercontrol/

        private readonly ILogger<Startup> log;


        
        public class 拉拉拉{
            
            [BsonElement("name")]
            public string NNNMMM{get;set;}="ZZZ";
          
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
            public string IIIDDD{get;set;}="XXX";
        }
        public Startup(IConfiguration werwerwr)
        {










            if(false)
            {
                //建立 mongo client
                var client = new MongoClient("mongodb://abc:efg@127.0.0.1:27017");
                //取得 database
                var db = client.GetDatabase("testDB");
                //取得 user collection
                var collection = db.GetCollection<拉拉拉>("TableA");
                
                //依 name 過濾並取得一筆資料
                var docs =  collection.Find(new MongoDB.Bson.BsonDocument()).ToList();
                Console.WriteLine("111:"+docs[0].NNNMMM);
                Console.WriteLine("111:"+docs[0].IIIDDD);
                var document2 = collection.Find(a => a.NNNMMM == "DaveIsMe").FirstOrDefault();
                Console.WriteLine("222:"+document2.IIIDDD);
                Console.WriteLine("222:"+document2.NNNMMM);
                var collection2 = db.GetCollection<BsonDocument>("TableA");
                collection2.InsertOne(new BsonDocument { {"name" , "MaryIsDead"} });
            }

            var fac =  LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddDebug();         
                builder.AddConsole();
            });

            log =  fac.CreateLogger<Startup>();

            _config = werwerwr;

            Email.DefaultSender = new MailgunSender(
                               _config["Mailgun:Domain"], // Mailgun Domain
                               _config["Mailgun:APIKey"] // Mailgun API Key
                       );


            TestLog();


            if(false){
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://alive_here/Submit?checkThisUrlEveryMin=http://web_here/");
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                }
            }




            //生存確認
            new Thread(() =>
            {
                DateTime yesterday = DateTime.Now - TimeSpan.FromDays(1);
                List<DateTime> times = new List<DateTime>(){
                    new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 06, 0, 0),//台灣時間下午14點  linux時間 早上6點
                    new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 14, 0, 0),//台灣時間晚上22點  linux時間 下午14點
                    new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 22, 0, 0),//台灣時間早上6點   linux時間 晚上22點
                };

                while (true)
                {
                    if (DateTime.Now > times[0])
                    {
                        log.LogWarning("警告 這一行不應該頻繁觸發");
                        if (DateTime.Now > times[1])
                        {
                        }
                        else
                        {
                           Send(instanceNum + ":::::::" + "生存確認~~~:" + DateTime.Now, "如題");
                        }
                        times.Add(times[0].AddDays(1));
                        times.RemoveAt(0);
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                    }
                }
            }).Start();



            //八卦版
            new Thread(() =>
            {
                HashSet<string> hash = new HashSet<string>();
                while (true)
                {
                    var ori_ = hash.Count;
                    var ok = GetPtt(ref hash, "https://www.ptt.cc/bbs/Gossiping/index.html", new List<string> { "肥宅", "女" }, true,3);
                    var new_ = hash.Count;
                    log.LogInformation("八卦新增文章:"+(new_-ori_));
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
            }).Start();

            if(false){
                    //ALLPOST
                    new Thread(() =>
                    {
                        HashSet<string> hash = new HashSet<string>();
                        while (true)
                        {
                            var ori_ = hash.Count;
                            var ok = GetPtt(ref hash, "https://www.ptt.cc/bbs/ALLPOST/index.html", new List<string> { 
                                "中山國中", 
                                "中山區", 
                                "榮星", 
                                "合江", 
                                "錦州", 
                                "龍江", 
                                "行天宮", 
                                "test123456", 
                                }, false,5);
                            var new_ = hash.Count;
                            log.LogInformation("ALLPOST新增文章:"+(new_-ori_));
                            Thread.Sleep(TimeSpan.FromSeconds(30));
                        }
                    }).Start();
            }


            //套房版
            new Thread(() =>
            {
                HashSet<string> hash = new HashSet<string>();
                while (true)
                {
                    var ori_ = hash.Count;
                    var ok =  GetPtt(ref hash, "https://www.ptt.cc/bbs/Rent_tao/index.html", new List<string> { "忠孝復興", "大潤發" }, true,3);
                    var new_ = hash.Count;
                    log.LogInformation("套房新增文章:"+(new_-ori_));
                    Thread.Sleep(TimeSpan.FromMinutes(30));
                }
            }).Start();



            //河濱
            new Thread(() =>
            {
                Dictionary<string, string> dic2 = null;
                bool first = true;
                while (true)
                {
                    Func<Dictionary<string, string>> GetDic1 = () =>
                    {

                        Dictionary<string, string> dic = new Dictionary<string, string>();


                        var doc = new HtmlDocument();
                        doc.Load(new MemoryStream(new WebClient().DownloadData("http://cup2020.whfa.football.idv.tw/schedule.php?level=3")), Encoding.UTF8);

                        //<table width="100%" cellpadding="0" cellspacing="0">
                        //   <tbody>
                        //      <tr bgcolor="#000644">
                        //         <td height="20" colspan="6">
                        //            <div class="p_white">3月7日</div>
                        //         </td>
                        //      </tr>
                        //      <tr>
                        //         <td>熱血足球 (紅黑)</td>
                        //         <td>v</td>
                        //         <td>河濱FC (粉紅)</td>
                        //         <td>1400-1510</td>
                        //         <td><a href="http://cup2020.whfa.football.idv.tw/schedule.php?level=3#"><img border="0" src="./109年大臺北社會人足球聯賽官方網站_files/arrow3.gif"></a></td>
                        //         <td>
                        //            <span class="style1">0 : 0
                        //            </span>
                        //         </td>
                        //      </tr>
                        //   </tbody>
                        //</table>
                        var linkNodes = doc.DocumentNode.SelectNodes(
                            "/html/body/div[2]/table/tbody/tr[1]/td/div/div[1]/font/table/tbody/tr/td/div[2]/div/div/div/table/tbody/tr"
                            .Replace("/tbody", ""));


                        string currentDate = null;
                        foreach (var linkNode in linkNodes)
                        {
                            if (linkNode.ChildNodes.Count == 3)
                            {
                                var test = linkNode.ChildNodes[1].InnerText;
                                if (test.Contains("月") && test.Contains("日"))
                                {
                                    currentDate = test;
                                }
                                else if (test == "&nbsp;")
                                {
                                }
                                else
                                {
                                    Console.WriteLine("錯誤1");
                                    return null;
                                }

                            }
                            else if (linkNode.ChildNodes.Count == 13)
                            {
                                var test = linkNode.ChildNodes[3].InnerText;
                                if (test == "v")
                                {
                                    if (currentDate == null)
                                    {
                                        Console.WriteLine("錯誤2");
                                        return null;
                                    }
                                    else
                                    {
                                        var key = "";
                                        key = ("日期:" + currentDate + "  ");
                                        key += ("時間:" + linkNode.ChildNodes[7].InnerText + "  ");
                                        key += ("主隊:" + linkNode.ChildNodes[1].InnerText + "  ");
                                        key += ("客隊:" + linkNode.ChildNodes[5].InnerText + "  ");
                                        dic[key] = linkNode.ChildNodes[11].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("錯誤3");
                                return null;
                            }
                        }
                        return dic;


                    };

                    var dic1 = GetDic1();

                    log.LogDebug("河濱數量:"+dic1.Count);

                    if (false)//first)
                    {
                        dic1["日期:4月11日  時間:1640-1750  主隊:熱血足球 (紅黑)  客隊:挑戰者 (紅黑)  "] = "喔第一次熱血vs挑戰";
                        dic1["隨便key~"] = "隨便value~";
                        first = false;
                    }

                    Compare_Send_WaitHour(dic1, ref dic2, "社會人變化");
                }
            }).Start();


            //英超
            new Thread(() =>
            {
                Dictionary<string, string> dic2 = null;
                bool first = true;
                while (true)
                {
                    var dic1 = Sele(
                        null,
                        "https://www.bt.com/sport/football/premier-league/table",
                        20,
                        new List<CrawlModel>(){

                            new CrawlModel("team",@"href=\\""https://www.bt.com.sport.football/.{1,30}\\"">(.{1,30})</a></td>",1),//  href=\"https://www.bt.com/sport/football/leicester-city\">Leicester City</a></td> <td clas
                            
                            new CrawlModel("gd",@"<td class=\\""league-gd\\"">(.{1,4})</td>",1),//   \">4</td> <td class=\"league-gd\">+8</td> <td class=\

                            new CrawlModel("pt",@"<td class=\\""league-points\\""><span class=\\""point\\"">(.{1,5})</span></td>",1)//<td class="league-points"><span class="point">13</span></td>
                        }
                        ).Result;


                    if (false)//first)
                    {
                        dic1["Liverpool"] = "第一次利物浦";
                        dic1["隨便key"] = "隨便value";
                        dic1.Remove("Everton");
                        first = false;
                    }

                    Compare_Send_WaitHour(dic1, ref dic2, "英超變化");
                }
            }).Start();


        }

        public FluentEmail.Core.Models.SendResponse Send(string Subject, string Body)
        {
            return Email
                      .From("killuplus300@gmail.com")
                      .To("killuplus300@gmail.com")
                      .Subject(Subject)
                      .Body(Body).Send();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

        }
        //
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.


        public static int instanceNum = new Random().Next();



        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(instanceNum + ":::::::" + "Hello World! version: " + v.version + "        \n");
                });



                endpoints.MapGet("/dave", async context =>
                {
                    await Sele(
                        context,
                        "https://www.bt.com/sport/football/premier-league/table",
                        20,
                        new List<CrawlModel>(){
                            new CrawlModel("team",@"href=\\""https://www.bt.com.sport.football/.{1,30}\\"">(.{1,30})</a></td>",1),
                            new CrawlModel("pt",@"<td class=\\""league-points\\""><span class=\\""point\\"">(.{1,5})</span></td>",1),
                            new CrawlModel("gd",@"<td class=\\""league-gd\\"">(.{1,4})</td>",1)
                        }
                        );
                });

                endpoints.MapGet("/warn", async context =>
                {
                    await context.Response.WriteAsync("done\n");
                });
            });

        }


        public async Task<Dictionary<string, string>> Sele(HttpContext context, string url, int count, List<CrawlModel> craws)
        {

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");

            IWebDriver driver = null;
            try{

             driver = new ChromeDriver(
                ChromeDriverService.CreateDefaultService(),
                chromeOptions,
                TimeSpan.FromSeconds(30));
            }
            catch(Exception ex){
                log.LogError("ChromeDriver開啟失敗:"+ex.ToString());
            }

            var dic = new Dictionary<string, string>();

            try
            {
                driver.Navigate().GoToUrl(url);//開啟網頁 這行一般花兩~三秒
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);//隱式等待 - 直到畫面跑出資料才往下執行
                var msg = JsonConvert.SerializeObject(driver.PageSource);


                List<MatchCollection> mcs = new List<MatchCollection>();
                List<String> ss = new List<String>();
                foreach (var craw in craws)
                {
                    mcs.Add(new Regex(craw.regex, RegexOptions.Compiled).Matches(msg));
                    ss.Add(craw.regex);
                }



                var ok = true;
                int k =-1;
                foreach (var mc in mcs)
                {   k++;
                    if (mc.Count != count)
                    {
                        log.LogError("pattern:"+ss[k]+"  "+"期待數量:"+count+"  "+"實際數量:"+mc.Count);
                        ok = false;
                    }
                }

                if (ok)
                {
                    for (int i = 0; i < count; i++)
                    {
                        string value = "";

                        for (int j = 1; j < mcs.Count; j++) //j從1開始
                        {
                            value += " ||| ";//這邊剛改沒上傳
                            value += (craws[j].name + "=" + mcs[j][i].Groups[craws[j].group].Value);
                        }


                        dic.Add(
                            mcs[0][i].Groups[craws[0].group].Value,
                            value
                            );
                    }
                }




                //Console.Write(JsonConvert.SerializeObject(dic));


                if (context != null)
                {
                    await context.Response.WriteAsync("\n" + JsonConvert.SerializeObject(dic) + "\n");
                }
            }
            catch (Exception ex)
            {
                log.LogError("ChromeDriver爬蟲失敗:"+ex.ToString());
                if (context != null)
                {
                    await context.Response.WriteAsync("\n" + "\n" + ex.ToString() + "\n" + "\n" + "\n");
                }
            }
            finally
            {
                driver.Quit();
            }
            return dic;
        }
    }
}
