using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using NetMQ;
using NetMQ.Sockets;
using UnityStandardAssets.Vehicles.Car;

public class test : MonoBehaviour
{
    Thread thread;
    static float steering=0,accelaration=0,handbrake=0,footbrake=0,speed=0;
    PublisherSocket pub;
    CarController carController;
    Rigidbody rigidbody;
    Vector3 position;
    NetMQMessage vehiclemessage;
    bool burdenbool = false;
    
    // Start is called before the first frame update
    void Start()
    {
        position = Vector3.zero;
        thread = new Thread(new ThreadStart(Test));
        thread.Start();
        carController = GetComponent<CarController>();
        rigidbody = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.localPosition;
        carController.Move(steering,accelaration,handbrake,footbrake);
        speed = rigidbody.velocity.magnitude;
    }
    public void Test() {
        AsyncIO.ForceDotNet.Force();
        pub = new PublisherSocket();
        pub.Bind("tcp://*:1000");
        
        try
        {
            while (true)
            {
                vehiclemessage = new NetMQMessage();
                vehiclemessage.Append(new NetMQFrame("Speed is "));
                vehiclemessage.Append(new NetMQFrame(speed.ToString()));
                vehiclemessage.Append(new NetMQFrame("Position is:"));
                vehiclemessage.Append(new NetMQFrame(Vector2String(position)));
                if (burdenbool) {
                    vehiclemessage.Append(new NetMQFrame("Something dropped"));
                }
                pub.SendMultipartMessage(vehiclemessage);
                Thread.Sleep(1000);
                vehiclemessage.Clear();
            }
        } 
        catch (NetMQException error) 
        {
            print(error.Data+"something");
        }
    }
    public string Vector2String(Vector3 position) {
        string _position = position.x + "   " + position.y + "   " + position.z;
        return _position;
    }
    public void SpeedUp() {
        accelaration = accelaration + 0.1f;
    }
    public void SpeedDown() {
        accelaration = accelaration - 0.1f;

    }
    public void TurnLeft() {
        steering = steering - 0.1f;
    }
    public void TurnRight() {
        steering = steering + 0.1f;
    }
    public void Reset()
    {
        rigidbody.velocity = Vector3.zero;
        steering = 0;
        accelaration = 0;
        transform.localPosition = new Vector3(0,-2,10);
        transform.localEulerAngles = Vector3.zero;

    }
    public void DropSomething() {
        Vector3 upperposition = transform.localPosition+new Vector3(5,10,0);
        // GameObject burden = (GameObject)Instantiate(Resources.Load("D:/UnityFile2019/BuaaTest/Assets/Prefab/burden.prefab"));
        GameObject burden = GameObject.CreatePrimitive(PrimitiveType.Cube);
        burden.transform.localPosition = upperposition;
        burden.AddComponent<Rigidbody>();
        burdenbool = true;
    }
    private void OnDestroy()
    {
        try
        {
            thread.Abort();
            pub.Close();
            pub.Dispose();
            NetMQConfig.Cleanup();
            
        }
        catch (Exception ex)
        {
            print("Failed to close resources");
        }
        finally
        {
            thread.Abort();
        }
    }
}
