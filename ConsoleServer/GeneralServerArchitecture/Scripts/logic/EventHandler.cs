using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class EventHandler 
{
    public static void OnDisConnect(ClientState c) 
    {
        Console.WriteLine("Close");
    }

    public static void OnTimer()
    {
    }


}
