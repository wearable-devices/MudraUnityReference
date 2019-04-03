
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
namespace Mudra
{
    sealed class MudraUDPPlugin
    {
        static readonly MudraUDPPlugin _instance = new MudraUDPPlugin();
        public static MudraUDPPlugin Instance { get { return _instance; } }

        byte[] data;
        IPEndPoint ipep;
        UdpClient newsock;
        IPEndPoint sender;
        static Boolean messageReceived = false;
        public static IPEndPoint e;
        public static UdpClient u;
        public static string receiveString = "";
        public static bool IsConnected = true;

        public void Init()
        {
            e = new IPEndPoint(IPAddress.Any, 5000);
            u = new UdpClient(e);
            ReceiveMessages(e, u);
        }

        public class UdpState
        {
            public UdpClient client;
            public IPEndPoint endpoint;
            public UdpState(UdpClient c, IPEndPoint iep)
            {
                this.client = c;
                this.endpoint = iep;
            }
        }


        public static void ReceiveMessages(IPEndPoint e, UdpClient u)
        {
            UdpState s = new UdpState(u, e);
            u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).client;
            IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).endpoint;

            Byte[] receiveBytes = u.EndReceive(ar, ref e);
            receiveString = Encoding.ASCII.GetString(receiveBytes);

            messageReceived = true;
            UdpState s = new UdpState(u, e);
            u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
        }

        private static void parse_data(string in_msg)

        {
            string[] parsed1 = { "" }, parsed2 = { "" }, parsed3 = { "" };
            parsed1 = in_msg.Split(':');
            if (parsed1.Length > 1)
            {
                parsed2 = parsed1[1].Split(' ');
                parsed3 = parsed1[2].Split(',');
            }
            switch (parsed1[1])
            {
                case ("gesture"):
                    switch (parsed1[2])
                    {
                        case ("thumb"):
                            if (UnityPlugin.Instance.aMudraGestureEvent!=null)
                                UnityPlugin.Instance.aMudraGestureEvent(GestureType.Thumb);                            
                            break;
                        case ("tap"):
                            if (UnityPlugin.Instance.aMudraGestureEvent != null)
                                UnityPlugin.Instance.aMudraGestureEvent(GestureType.Tap);                            
                            break;
                        case ("index"):
                            if (UnityPlugin.Instance.aMudraGestureEvent != null)
                                UnityPlugin.Instance.aMudraGestureEvent(GestureType.Index);                            
                            break;
                        default:
                            break;
                    }
                    break;
                case ("proportional"):
                    if (UnityPlugin.Instance.aMudraFingertipPressureEvent!=null)
                        UnityPlugin.Instance.aMudraFingertipPressureEvent(float.Parse(parsed1[2]));
                    break;
                case "quaternions":
                    float[] imuData = { float.Parse(parsed3[0]), float.Parse(parsed3[1]), float.Parse(parsed3[2]), float.Parse(parsed3[3]) };
                    if (UnityPlugin.Instance.aMudraIMUEvent!=null)
                        UnityPlugin.Instance.aMudraIMUEvent(imuData);
                    break;
                case "airmouse":
                    float[] airmouseData = { float.Parse(parsed3[0]), float.Parse(parsed3[1])};     
                    if (UnityPlugin.Instance.aMudraAirMouseeEvent!=null)
                        UnityPlugin.Instance.aMudraAirMouseeEvent(airmouseData);
                    break;
                case ("connect"):
                    IsConnected = true;                 
                                       
                    break;

                default:
                    break;


            }
        }

        public void Update()
        {
            if (messageReceived == true)
            {

                messageReceived = false;
                parse_data(receiveString);
            }
        }
    }
}
