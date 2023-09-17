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
    //��ʼ
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
    //----------------�ͻ���ע�Ṧ��-----------
    //����ע��Э��
    public void OnRegisterClick() 
    {
        MsgRegister msg = new MsgRegister();
        msg.id = idInput.text;
        msg.pw = pwInput.text;
        NetManager.Send(msg);


    }
    //�յ�ע��Э��
    public void OnMsgRegister (MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister)msgBase;
        if (msg.result == 0)
        {
            Debug.Log("ע��ɹ�");
        }
        else 
        {
            Debug.Log("ע��ʧ��");
        }
    }

    //-------------------�ͻ��˵�¼����---------------------
    //���͵�¼Э��
    public void OnLoginClick() 
    {
        MsgLogin msg = new MsgLogin();
        msg.id = idInput.text;
        msg.pw = pwInput.text;
        NetManager.Send(msg);

    }
    //�յ���¼Э��
    public void OnMsgLogin(MsgBase msgBase) 
    {
        MsgLogin msg = (MsgLogin)msgBase;
        if (msg.result == 0)
        {
            Debug.Log("��¼�ɹ�");
            //������±��ı�
            MsgGetText msgGetText = new MsgGetText();
            NetManager.Send(msgGetText);
        }
        else 
        {
            Debug.Log("��¼ʧ��");
        }
    }
    //��������
    void OnMsgKick(MsgBase msgBase) 
    {
        Debug.Log("��������");
    }

    //---------------------------�ͻ��˼��±�����------------------------------
    //�յ����±��ı�Э��
    public void OnMsgGetText(MsgBase msgBase)
    {
        MsgGetText msg = (MsgGetText)msgBase;
        textInput.text = msg.text;
    }

    //���ͱ���Э��
    public void OnSaveClick() 
    {
        MsgSaveText msg = new MsgSaveText();
        msg.text = textInput.text;
        NetManager.Send(msg);
    }

    //�յ�����Э��
    public void OnMsgSaveText(MsgBase msgBase) 
    {
        MsgSaveText msg = (MsgSaveText)msgBase;
        if (msg.result == 0)
        {
            Debug.Log("����ɹ�");
        }
        else 
        {
            Debug.Log("����ʧ��");
        }
    }





    //---------------------------���ӷ����----------------------------------

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
