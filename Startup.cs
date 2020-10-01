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
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Test002
{
    public class Startup
    {
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }
        //
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            new Thread(() => 
            {
                var apiKey = "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";
                
                Dictionary<string,int> dic2 = null;
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

                    
                    foreach (KeyValuePair<string, int> kv in dic1)
                    {
                        if(dic1[kv.Key] != dic2[kv.Key])
                        {
                            content += String.Format(
                                @"
                                team name:{0},
                                ori gd:{1},
                                new gd:{2},
                                ",
                                kv.Key,
                                dic1[kv.Key],
                                dic2[kv.Key]
                            );
                            content += "\n";
                            dic1[kv.Key] = dic2[kv.Key];
                        }
                    }
                    
                    if(content!="")
                    {
                        var response = new SendGridClient(apiKey).SendEmailAsync(MailHelper.CreateSingleEmail(
                            new EmailAddress("hyfileserver@gmail.com", "這是寄件者名稱"),
                            new EmailAddress("killuplus300@gmail.com"),
                            "GCP測試喔",
                            content,
                            content
                            )).Result;
                            
                        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                        {
                        }
                        else
                        {
                        }
                    }
                    Thread.Sleep(TimeSpan.FromHours(1));
                }
                


            }).Start();
            


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World! version: "+v.version+"        \n");
                });

                endpoints.MapGet("/dave", async context =>
                {
                    await Sele(context);
                });
            });

        }

        
        public async Task<Dictionary<string,int>> Sele(HttpContext context)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            IWebDriver driver = new ChromeDriver(
                ChromeDriverService.CreateDefaultService(),
                chromeOptions,
                TimeSpan.FromSeconds(30));
            var dic = new Dictionary<string,int>();
            
            try
            {
                driver.Navigate().GoToUrl("https://www.bt.com/sport/football/premier-league/table");//開啟網頁 這行一般花兩~三秒
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);//隱式等待 - 直到畫面跑出資料才往下執行
                var msg = JsonConvert.SerializeObject(driver.PageSource);


                MatchCollection  matchesGD=null;
                MatchCollection  matchesNAME=null;
                {
                    var 原始 = "後面註解"; //   \">4</td> <td class=\"league-gd\">+8</td> <td class=\
                    matchesGD = new Regex(@"<td class=\\""league-gd\\"">(.{1,4})</td>",RegexOptions.Compiled).Matches(msg);
                }
                

                {
                    var 原始 = "後面註解"; //  href=\"https://www.bt.com/sport/football/leicester-city\">Leicester City</a></td> <td clas
                    matchesNAME = new Regex(@"href=\\""https://www.bt.com.sport.football/.{1,30}\\"">(.{1,30})</a></td>",RegexOptions.Compiled).Matches(msg);
                }

                if(matchesGD.Count == matchesNAME.Count)
                {
                    for(int i=0;i<matchesGD.Count;i++)
                    {
                        dic.Add(
                            matchesNAME[i].Groups[1].Value,
                            Convert.ToInt32(matchesGD[i].Groups[1].Value)
                            );
                    }
                }




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
