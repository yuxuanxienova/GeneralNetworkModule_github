﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class MsgHandler 
{
    public static void MsgPing(ClientState c, MsgBase msgBase) 
    {
        Console.WriteLine("MsgPing");
        c.lastPingTime = NetManager.GetTimeStamp();
        MsgPong msgPong = new MsgPong();
        NetManager.Send(c, msgPong);

    }
}
