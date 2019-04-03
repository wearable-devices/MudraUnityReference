
using UnityEngine;
using System;

namespace Mudra
{
     sealed class MudraAndroidPlugin
    {
        static readonly MudraAndroidPlugin _instance = new MudraAndroidPlugin();
        public static MudraAndroidPlugin Instance { get { return _instance; } }

        #region Android variables
        static AndroidJavaClass _mudraClass;
        static AndroidJavaObject _mudraDevice;
        public bool IsConnected = false;
        #endregion



        #region Android interfaces implemantation

        class OnAirMousePositionChanged : AndroidJavaProxy
        {
            MudraAndroidPlugin _unityPlugin;

            public OnAirMousePositionChanged(MudraAndroidPlugin unityplugin) : base("MudraAndroidSDK.Mudra$OnAirMousePositionChanged") { _unityPlugin = unityplugin; }

            void run(float[] var1)
            {
                UnityPlugin.Instance.setAirMouseXY(var1);                
            }

        }

        class OnFingertipPressureReady : AndroidJavaProxy
        {
            MudraAndroidPlugin _unityPlugin;

            public OnFingertipPressureReady(MudraAndroidPlugin unityplugin) : base("MudraAndroidSDK.Mudra$OnFingertipPressureReady") { _unityPlugin = unityplugin; }

            void run(float var1)
            {
                UnityPlugin.Instance.setPressure(var1);
            }
        }

        class OnImuQuaternionReady : AndroidJavaProxy
        {
            MudraAndroidPlugin _broadcastReceiver;
            public OnImuQuaternionReady(MudraAndroidPlugin unityPlugin) : base("MudraAndroidSDK.Mudra$OnImuQuaternionReady") { _broadcastReceiver = unityPlugin; }

            void run(float[] retObj)
            {
                try
                {
                    UnityPlugin.Instance.setQuaternion(retObj);
                    Debug.Log("q" + retObj[0]);
                }
                catch (Exception e)
                {
                    Debug.Log(" error with quaternions "+e);
                }
            }
        }

        class OnGestureReady : AndroidJavaProxy
        {
            MudraAndroidPlugin _broadcastReceiver;
            public OnGestureReady(MudraAndroidPlugin unityPlugin) : base("MudraAndroidSDK.Mudra$OnGestureReady") { _broadcastReceiver = unityPlugin; }

            void run(AndroidJavaObject retObj)
            {
                UnityPlugin.Instance.setGesture((GestureType)retObj.Call<int>("ordinal"));
            }
        }

        public void setOnAirMouseReady(bool ena)
        {
            if (ena)
                _mudraDevice.Call("setOnAirMousePositionChanged", new OnAirMousePositionChanged(this));
            else
                _mudraDevice.Call("setOnAirMousePositionChanged", null);
        }

        public void setOnGestureReady(bool ena)
        {
            if (ena)
                _mudraDevice.Call("setOnGestureReady", new OnGestureReady(this));
            else
                _mudraDevice.Call("setOnGestureReady", null);
        }

        public void setOnFingertipPressureReady(bool ena)
        {
            if (ena)
                _mudraDevice.Call("setOnFingertipPressureReady", new OnFingertipPressureReady(this));
            else
                _mudraDevice.Call("setOnFingertipPressureReady", null);
        }

        public void setOnImuQuaternionReady(bool ena)
        {
            if (ena)
                _mudraDevice.Call("setOnImuQuaternionReady", new OnImuQuaternionReady(this));
            else
                _mudraDevice.Call("setOnImuQuaternionReady", null);
        }



        #endregion
        public void Init()
        {


            /*       check that external permission is granted
                  if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
                      Debug.Log("ExternalStorageRead permission has been granted.");
                  else
                      Permission.RequestUserPermission(Permission.ExternalStorageRead);

                  if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                      Debug.Log("ExternalStorageRead permission has been granted.");
                  else
                      Permission.RequestUserPermission(Permission.ExternalStorageWrite);

                  */

            _mudraClass = new AndroidJavaClass("MudraAndroidSDK.Mudra");
            AndroidJavaClass feature = new AndroidJavaClass("MudraAndroidSDK.Feature");
            AndroidJavaClass jcu = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jcu.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = jo.Call<AndroidJavaObject>("getApplicationContext");
            _mudraDevice = _mudraClass.CallStatic<AndroidJavaObject>("autoConnectPaired", context);
            String LICENSE = "Feature::RawData";
            _mudraDevice.Call("setLicense", 0, LICENSE);

            Debug.Log("Android Init Done");

        }

        public void Update()
        {
            
                IsConnected = _mudraDevice.Call<bool>("isConnected");

            Debug.Log("Android Update Done");
        }
    }
}
