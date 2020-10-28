using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Test002
{


     public class CrawlModel
     {
        public string name;
        public string regex;
        public int group;

        public CrawlModel(string name,string regex, int group)
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



        public void Compare_Send_WaitHour(ref Dictionary<string,string> dic1,ref Dictionary<string,string> dic2,string emailTitle)
        {
                    if(dic1==null || dic1.Count==0)
                    {
                        return;
                    }

                    if(dic2==null)
                    {
                        dic2 = dic1.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
                        return;
                    }
                    
                    var content ="";

                    foreach (KeyValuePair<string, string> kv in dic1)
                    {
                        if(kv.Value != dic2[kv.Key])
                        {
                            content += String.Format(
                                @"
                                target : {0}
                                origin : {1}
                                update : {2}
                                ",
                                kv.Key,
                                dic2[kv.Key],
                                kv.Value
                            );
                            content += "\n";

                            dic2[kv.Key] = kv.Value;
                        }
                    }
                    
                    if(content!="")
                    {
                        var response =  Send(emailTitle,instanceNum+":::::::"+content);
                            
                        if (!response.Successful)
                        {
                            Console.WriteLine("fail::::"+JsonConvert.SerializeObject(response.ErrorMessages));
                            
                        }
                        else
                        {
                        }
                    }
                    Thread.Sleep(TimeSpan.FromHours(1));
        }

        public static bool first = true;

        public Startup(IConfiguration config)
        {
            _config = config;

            
             Email.DefaultSender = new MailgunSender(
                                _config["Mailgun:Domain"], //   Mailgun Domain
                                _config["Mailgun:APIKey"] // Mailgun API Key
                        ); 

            Send(instanceNum+":::::::"+"程式開啟","~~~~~~~~如題");


            new Thread(() => 
            {
                Dictionary<string,string> dic2 = null;
                while(true)
                {
                    var dic1  = Sele(
                        null,
                        "https://www.bt.com/sport/football/premier-league/table",
                        20,
                        new List<CrawlModel>(){
                            new CrawlModel("team",@"href=\\""https://www.bt.com.sport.football/.{1,30}\\"">(.{1,30})</a></td>",1),//  href=\"https://www.bt.com/sport/football/leicester-city\">Leicester City</a></td> <td clas
                            new CrawlModel("pt",@"<td class=\\""league-points\\""><span class=\\""point\\"">(.{1,5})</span></td>",1),//<td class="league-points"><span class="point">13</span></td>
                            new CrawlModel("gd",@"<td class=\\""league-gd\\"">(.{1,4})</td>",1)//   \">4</td> <td class=\"league-gd\">+8</td> <td class=\
                        }
                        ).Result;

                    if(first)
                    {
                        dic1["Everton"]="第一次艾弗頓";
                        first = false;
                    }

                    Compare_Send_WaitHour(ref dic1,ref dic2,"英超變化");
                }
            }).Start();
        }

        public FluentEmail.Core.Models.SendResponse Send(string Subject, string Body)
        {
              return          Email
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
                    await context.Response.WriteAsync(instanceNum+":::::::"+"Hello World! version: "+v.version+"        \n");
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

        
        public async Task<Dictionary<string,string>> Sele(HttpContext context, string url, int count,List<CrawlModel> craws)
        {

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            IWebDriver driver = new ChromeDriver(
                ChromeDriverService.CreateDefaultService(),
                chromeOptions,
                TimeSpan.FromSeconds(30));
            var dic = new Dictionary<string,string>();
            
            try
            {
                driver.Navigate().GoToUrl(url);//開啟網頁 這行一般花兩~三秒
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);//隱式等待 - 直到畫面跑出資料才往下執行
                var msg = JsonConvert.SerializeObject(driver.PageSource);


                List<MatchCollection> mcs = new List<MatchCollection>();
                foreach(var craw in craws)
                {
                    mcs.Add(new Regex(craw.regex,RegexOptions.Compiled).Matches(msg));
                }



                var ok = true;
                foreach(var mc in mcs)
                {
                    if(mc.Count != count)
                    {
                        ok = false;
                    }
                }

                if(ok)
                {
                  for(int i=0;i<count;i++)
                  {
                    string value = "";

                    for(int j=1;j<mcs.Count;j++) //j從1開始
                    {
                        value += (craws[j].name+"="+mcs[j][i].Groups[craws[j].group].Value);
                        value += " ||| ";
                    }
                    
                    
                    dic.Add(
                        mcs[0][i].Groups[craws[0].group].Value,
                        value
                        );
                  }
                }





                Console.Write(JsonConvert.SerializeObject(dic));


                if(context!=null)
                {
                    await context.Response.WriteAsync( "\n"+JsonConvert.SerializeObject(dic)+"\n");
                }
            }
            catch (Exception ex)
            {
                if(context!=null)
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
