using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class MainClass 
{
    static JavaScriptSerializer Js = new JavaScriptSerializer();
    public static void Main(string[] args) 
    {
        //连接数据库
        if (!DbManager.Connect("game", "127.0.0.1", 3306, "root", "")) 
        {
            return;
        }

        ////测试
        //DbManager.CreatePlayer("aglab");
        //PlayerData pd = DbManager.GetPlayerData("aglab");
        //pd.coin = 256;
        //DbManager.UpdatePlayerData("aglab", pd);

        //开启网络监听
        NetManager.StartLoop(1234);
    }
}


