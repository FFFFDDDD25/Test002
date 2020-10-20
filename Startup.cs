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
    public class Startup
    {
        
        private IConfiguration _config;
        //   Here we are using Dependency Injection to inject the Configuration object
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
                while(true)
                {
                    var dic1  = Sele(null).Result;

                    if(dic1==null || dic1.Count==0)
                    {
                        continue;
                    }

                    if(dic2==null)
                    {
                        dic2 = dic1.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
                        continue;
                    }
                    



                    var content ="";

                    
                    foreach (KeyValuePair<string, string> kv in dic1)
                    {
                        if(kv.Value != dic2[kv.Key])
                        {
                            content += String.Format(
                                @"
                                team name:{0},
                                ori pt,gd:{1},
                                new pt,gd:{2},
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
                        var response =  Send("英超淨勝球變化",instanceNum+":::::::"+content);
                            
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


        public static Dictionary<string,string> dic2 = null;
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
                    await Sele(context);
                });

                

                endpoints.MapGet("/gan", async context =>
                {
                    await context.Response.WriteAsync(instanceNum+":::::::"+JsonConvert.SerializeObject(dic2));
                });

                
                endpoints.MapGet("/warn", async context =>
                {
                    await context.Response.WriteAsync("done\n");
                });
            });

        }

        
        public async Task<Dictionary<string,string>> Sele(HttpContext context)
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
                driver.Navigate().GoToUrl("https://www.bt.com/sport/football/premier-league/table");//開啟網頁 這行一般花兩~三秒
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);//隱式等待 - 直到畫面跑出資料才往下執行
                var msg = JsonConvert.SerializeObject(driver.PageSource);


                MatchCollection  matchesGD=null;
                MatchCollection  matchesPT=null;
                MatchCollection  matchesNAME=null;
                {
                    var 原始 = "後面註解"; //   \">4</td> <td class=\"league-gd\">+8</td> <td class=\
                    matchesGD = new Regex(@"<td class=\\""league-gd\\"">(.{1,4})</td>",RegexOptions.Compiled).Matches(msg);
                }
                
                {
                    var 原始 = "後面註解"; //<td class="league-points"><span class="point">13</span></td>
                    matchesPT = new Regex(@"<td class=\\""league-points\\""><span class=\\""point\\"">(.{1,5})</span></td>",RegexOptions.Compiled).Matches(msg);
                }

                {
                    var 原始 = "後面註解"; //  href=\"https://www.bt.com/sport/football/leicester-city\">Leicester City</a></td> <td clas
                    matchesNAME = new Regex(@"href=\\""https://www.bt.com.sport.football/.{1,30}\\"">(.{1,30})</a></td>",RegexOptions.Compiled).Matches(msg);
                }

                if(
                matchesGD.Count == matchesNAME.Count &&
                matchesGD.Count == matchesPT.Count
                )
                {
                    for(int i=0;i<matchesGD.Count;i++)
                    {
                        dic.Add(
                            matchesNAME[i].Groups[1].Value,
                            matchesPT[i].Groups[1].Value+" , "+matchesGD[i].Groups[1].Value
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
