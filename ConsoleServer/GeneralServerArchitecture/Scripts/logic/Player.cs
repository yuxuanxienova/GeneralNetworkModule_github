﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Player 
{
    //id
    public string id = "";

    //指向ClientState
    public ClientState state;

    //临时数据， 如：坐标
    public int x;
    public int y;
    public int z;

    //数据库数据
    public PlayerData data;

    //构造函数
    public Player(ClientState state) 
    {
        this.state = state;
    }

    //发送信息
    public void Send(MsgBase msgBase) 
    {
        NetManager.Send(state, msgBase);
    }
}
