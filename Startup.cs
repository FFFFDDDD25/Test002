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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World! ASP.NET\n");
                });

                endpoints.MapGet("/dave", async context =>
                {
                    await Sele(context);
                });

                
                endpoints.MapGet("/gan", async context =>
                {
                    await Print(context);
                });
            });

        }

        
        public async Task Print(HttpContext context)
        {
            DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles(); //Getting Text files
            string str = "";


            
            str = "";
            foreach(FileInfo file in Files )
            {
                str = str + "\n" + file.Name;
            }
            await context.Response.WriteAsync("ffffffffffffffffffffffff_11" + str+"\n");
            str = "";
            foreach(DirectoryInfo dir in d.GetDirectories() )
            {
                str = str + "\n" + dir.Name;
                if(dir.Name=="files"){
                    foreach(var f in dir.GetFiles() )
                    {
                        str = str + "\n" + "--------"+f.Name;
                    }
                }
            }
            await context.Response.WriteAsync("dddddddddddddddddddddddd" + str+"\n");

        }

        public async Task Sele(HttpContext context)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            IWebDriver driver = new ChromeDriver("files",chromeOptions);

            try
            {
                driver.Navigate().GoToUrl("https://www.bt.com/sport/football/premier-league/table");//開啟網頁
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3000);//隱式等待 - 直到畫面跑出資料才往下執行


                var FALSE = false;
                if (FALSE)
                {
                    //輸入帳號
                    IWebElement inputAccount = driver.FindElement(By.Name("email"));
                    //清除按鈕
                    inputAccount.Clear();
                    Thread.Sleep(500);
                    inputAccount.SendKeys("LiveChat_Account");
                    Thread.Sleep(500);

                    IWebElement inputPassword = driver.FindElement(By.Name("password"));
                    inputPassword.Clear();
                    Thread.Sleep(500);
                    inputPassword.SendKeys("LiveChat_Password");
                    Thread.Sleep(500);

                    //點擊執行
                    IWebElement submitButton = driver.FindElement(By.XPath("/html/body/div[2]/div/div[2]/div[2]/div/div/form/div[3]/button/span"));
                    Thread.Sleep(500);
                    submitButton.Click();
                    Thread.Sleep(2000);
                }


                var msg = JsonConvert.SerializeObject(driver.PageSource);
                var listPoint = new List<string>();
                var listName = new List<string>();


                {
                    var 原始 = "後面註解"; //   <td class="league-gd">+8</td>
                    MatchCollection  matches = new Regex(@"league-gd........",RegexOptions.Compiled).Matches(msg);
                    foreach (Match match in matches)
                    {
                        var m = new Regex(">([+-]{0,1}[0-9]{1,4})<").Match(match.Value);
                        if (m.Success)
                        {
                            listPoint.Add(m.Groups[1].Value);
                        }
                    }
                }
                

                {
                    var 原始 = "後面註解"; //  <a href="https://www.bt.com/sport/football/liverpool">LIV</a>
                    MatchCollection  matches = new Regex(@"www.bt.com.sport.football.([a-z\-]{0,30})..>(.{0,30})<",RegexOptions.Compiled).Matches(msg);
                   // MatchCollection  matches = new Regex(@"www.bt.com.sport.football.........................)",RegexOptions.Compiled).Matches(msg);
                   foreach (Match match in matches)
                    {
                        listName.Add(match.Groups[1].Value);
                    }
                }

//
                await context.Response.WriteAsync("listPoint "+listPoint.Count+"\n");
                await context.Response.WriteAsync("listName "+listName.Count+"\n");

                if(listPoint.Count==listName.Count)
                {
                    for(int i=0;i<listName.Count;i++)
                    {
                        await context.Response.WriteAsync( listPoint[i] +": " +  listName[i]+"\n");
                    }
                }
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync("\n" + "\n" + ex.ToString() + "\n" + "\n" + "\n");
                return;
            }
            finally
            {
                await context.Response.WriteAsync(driver.Title + "\n");
                driver.Quit();
            }


        }
    }
}
