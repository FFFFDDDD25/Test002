using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Test002
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                ;
    }
}

//Is there a way to make apt-get install answer "yes" to the "Do you want to continue [y/N]?"?
//apt-get -y install [packagename]

//筆者個人認為，dockerfile 最重要的 FORM、COPY、RUN、CMD 正好對應 docker image 從建立到執行的四階段。
//1.1 初始設定
//  FROM 指定 docker image 建立時，使用的底層 (Base Image)。
//      FROM <image> [AS <name>]
//  ENV  The ENV instruction sets the environment variable <key> to the value <value>
//1.2 配置
//  COPY 複制檔案或資料夾到 container 的檔案系統內。
//      FROM <image> [AS <name>]
//  ADD 除了 copy 的功能外，額外支援 URL 的資料來源。
//1.3 建立 image
//  RUN builds your application with make.
//      # 1. shell
//          RUN <command> 
//      # 2. exec
//          RUN ["executable", "param1", "param2"]
//1.4 啟動 container
//  CMD specifies what command to run within the container.
//      # 1. exec form, this is the preferred form
//          CMD ["executable","param1","param2"] 
//      # 2. as default parameters to ENTRYPOINT
//          CMD ["param1","param2"]
//      # 3. shell form
//          CMD command param1 param2
//  ENTRYPOINT  An ENTRYPOINT allows you to configure a container that will run as an executable.
//      ENTRYPOINT has two forms:
//          ENTRYPOINT ["executable", "param1", "param2"] (exec form, preferred)
//          ENTRYPOINT command param1 param2 (shell form)


/*  
docker exec -it mongo7788 service mongod stop  或者進去db使用  db.shutdownServer();
docker exec -it mongo7788 rm -rf ./data/db

docker volume  rm 7f591350a7c37603754c06425e8814a5d9a01dacf4ac8555de2d0503b76bea34
docker volume  rm 125b892f2e747b5f3faeab2eeaec51dedef9da45c76ee2602906b938b86d1a5e
docker volume  rm 644eac4062bb1a3ae4e46e999b79d5c86d64d680b78ead248f21022df7c52fc0
docker volume  rm 46968ffbc59d6d083427fa57d687fd8d5f90fa33942204d1d88c9b3315d97cee
docker volume  rm e50305cf9ab2d80017ae264603e7a6798949582701a0c5905628d234ae1fa3ca


docker pull mongo:4.2  4.2是比較多gui支援的版本..嗎?
docker run --name mongo7788 -p 27017:27017 -v /data/db:/data/db -d mongo:4.2 --auth
docker exec -it mongo7788 mongo admin  或拿掉admin然後進去後  use admin
db.createUser( { user: 'abc', pwd: 'efg', roles: [ { role: 'root', db: 'admin' } ] } );
docker restart mongo7788
docker exec -it mongo7788 mongo -u abc -p efg --authenticationDatabase admin
use testDB
db.TableA.save({name:'DaveIsMe'})
show dbs
show collections
db.TableA.find();



PS C:\Users\User>
PS C:\Users\User>
PS C:\Users\User> docker run --name mongo7788 -p 27017:27017 -v /data/db:/data/db -d mongo --auth
96beda992a38ec0d1ae722b2656d532140022664ece3262f1bcedff708c3f191
PS C:\Users\User> docker volume ls
DRIVER              VOLUME NAME
local               8c9c9858da97f3bcdf0679f5ebc9a5a43b0f411d7e4092352a01625a0750fc0f
PS C:\Users\User>
PS C:\Users\User>
PS C:\Users\User>
PS C:\Users\User>



ssh <username>@<host> How do I find the user name and host name of my local machine?
To find the username, type whoami

Enter MongoDB node hostname
cmd: mongo getHostName()                        b72f9d434701
cmd: docker exec -it mongo7788 hostname         b72f9d434701
cmd: 進去container的bash 什麼都不用輸入就可以看到  root@b72f9d434701:/#

*/