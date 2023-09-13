using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class test : MonoBehaviour
{
    //开始
    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
        
    }
    //收到MsgMove协议
    public void OnMsgMove(MsgBase msgBase) 
    {
        MsgMove msg = (MsgMove)msgBase;

        //消息处理
        Debug.Log("OnMsgMove msg.x = " + msg.x);
        Debug.Log("OnMsgMove msg.y = " + msg.y);
        Debug.Log("OnMsgMove msg.z = " + msg.z);
    }

    //连接成功回调
    public void OnConnectSucc(string err) 
    {
        Debug.Log("OnConnectSucc");
        //TODO:进入游戏
    }

    //连接失败回调
    public void OnConnectFail(string err) 
    {
        Debug.Log("OnConnectFail" + err);
        //TODO:弹出提示框（连接失败，请重试）

    }

    //关闭连接
    public void OnConnectClose(string err) 
    {
        Debug.Log("OnConnectClose");
        //TODO:弹出提示框（网络断开）
        //TODO:弹出提示框（重新连接）

    }

    //玩家点击连接按钮
    public void OnConnectClick()
    {
        NetManager.Connect("127.0.0.1", 1234);
        //TODO:开始转圈，提示“连接中”
    }

    //主动关闭
    public void OnCloseCLick() 
    {
        NetManager.Close();
    }

    //玩家点击发送按钮
    public void OnMoveClick() 
    {
        MsgMove msg = new MsgMove();
        msg.x = 120;
        msg.y = 123;
        msg.z = -6;
        NetManager.Send(msg);

    }

    public void Update()
    {
        NetManager.Update();


        if (Input.GetKeyDown(KeyCode.T))
        {

            TestMain();

        }
    }

    public void TestMain()
    {
        //---------------test------------------ 

        //MsgMove msgMove = new MsgMove();
        //byte[] bs = MsgBase.EncodeName(msgMove);

        //int count;
        //string name = MsgBase.DecodeName(bs, 0, out count);

        //Debug.Log(name); //MsgMove
        //Debug.Log(count); //2+7=9

    }

}
