using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class test : MonoBehaviour
{
    //��ʼ
    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
        
    }
    //�յ�MsgMoveЭ��
    public void OnMsgMove(MsgBase msgBase) 
    {
        MsgMove msg = (MsgMove)msgBase;

        //��Ϣ����
        Debug.Log("OnMsgMove msg.x = " + msg.x);
        Debug.Log("OnMsgMove msg.y = " + msg.y);
        Debug.Log("OnMsgMove msg.z = " + msg.z);
    }

    //���ӳɹ��ص�
    public void OnConnectSucc(string err) 
    {
        Debug.Log("OnConnectSucc");
        //TODO:������Ϸ
    }

    //����ʧ�ܻص�
    public void OnConnectFail(string err) 
    {
        Debug.Log("OnConnectFail" + err);
        //TODO:������ʾ������ʧ�ܣ������ԣ�

    }

    //�ر�����
    public void OnConnectClose(string err) 
    {
        Debug.Log("OnConnectClose");
        //TODO:������ʾ������Ͽ���
        //TODO:������ʾ���������ӣ�

    }

    //��ҵ�����Ӱ�ť
    public void OnConnectClick()
    {
        NetManager.Connect("127.0.0.1", 1234);
        //TODO:��ʼתȦ����ʾ�������С�
    }

    //�����ر�
    public void OnCloseCLick() 
    {
        NetManager.Close();
    }

    //��ҵ�����Ͱ�ť
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
