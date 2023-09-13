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
        NetManager.StartLoop(1234);
    }
}


