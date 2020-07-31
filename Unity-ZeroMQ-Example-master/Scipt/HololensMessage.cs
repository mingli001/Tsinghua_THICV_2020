using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NetMQ;
using NetMQ.Sockets;
public class HololensMessage : MonoBehaviour
{
        //type of touch(one tap,double tap)
        public int Type
        {
            get;
            set;
        }
        //name of object touched
        public string Name
        {
            get;
            set;
        }
        //position of cursor
        public Vector3 Position
        {
            get;
            set;
        }
        public HololensMessage(Vector3 position)
        {
            this.Position = position;
        }
        public HololensMessage(int type, string name, Vector3 position)
        {
            this.Type = type;
            this.Name = name;
            this.Position = position;
        }
        public HololensMessage(int type, string name, float[] position)
        {
            this.Type = type;
            this.Name = name;
            this.Position = new Vector3(position[0], position[1], position[2]);
        }
        public static string GetHololensMessage(HololensMessage hololensMessage)
        {
            string message = "";
            try
            {
              //  message = JsonConvert.SerializeObject(hololensMessage);
            }
            catch (Exception e)
            {
                print("Failed to serialize object");
            }
            return message;
        }
}
