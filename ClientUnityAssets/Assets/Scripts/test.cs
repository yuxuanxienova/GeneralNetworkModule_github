using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;


public class test : MonoBehaviour
{
    public TMP_InputField idInput;
    public TMP_InputField pwInput;
    public TMP_InputField textInput;
    //开始
    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);

        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);
        NetManager.AddMsgListener("MsgGetText", OnMsgGetText);
        NetManager.AddMsgListener("MsgSaveText", OnMsgSaveText);
        
        
    }
    //----------------客户端注册功能-----------
    //发送注册协议
    public void OnRegisterClick() 
    {
        MsgRegister msg = new MsgRegister();
        msg.id = idInput.text;
        msg.pw = pwInput.text;
        NetManager.Send(msg);


    }
    //收到注册协议
    public void OnMsgRegister (MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister)msgBase;
        if (msg.result == 0)
        {
            Debug.Log("注册成功");
        }
        else 
        {
            Debug.Log("注册失败");
        }
    }

    //-------------------客户端登录功能---------------------
    //发送登录协议
    public void OnLoginClick() 
    {
        MsgLogin msg = new MsgLogin();
        msg.id = idInput.text;
        msg.pw = pwInput.text;
        NetManager.Send(msg);

    }
    //收到登录协议
    public void OnMsgLogin(MsgBase msgBase) 
    {
        MsgLogin msg = (MsgLogin)msgBase;
        if (msg.result == 0)
        {
            Debug.Log("登录成功");
            //请求记事本文本
            MsgGetText msgGetText = new MsgGetText();
            NetManager.Send(msgGetText);
        }
        else 
        {
            Debug.Log("登录失败");
        }
    }
    //被踢下线
    void OnMsgKick(MsgBase msgBase) 
    {
        Debug.Log("被踢下线");
    }

    //---------------------------客户端记事本功能------------------------------
    //收到记事本文本协议
    public void OnMsgGetText(MsgBase msgBase)
    {
        MsgGetText msg = (MsgGetText)msgBase;
        textInput.text = msg.text;
    }

    //发送保存协议
    public void OnSaveClick() 
    {
        MsgSaveText msg = new MsgSaveText();
        msg.text = textInput.text;
        NetManager.Send(msg);
    }

    //收到保存协议
    public void OnMsgSaveText(MsgBase msgBase) 
    {
        MsgSaveText msg = (MsgSaveText)msgBase;
        if (msg.result == 0)
        {
            Debug.Log("保存成功");
        }
        else 
        {
            Debug.Log("保存失败");
        }
    }





    //---------------------------连接服务端----------------------------------

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
