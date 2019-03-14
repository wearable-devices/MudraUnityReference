//################################################
#undef UNITY_EDITOR
using UnityEngine;
using System;
using UnityEngine.Android;


namespace Mudra
{
    public sealed class UnityPlugin
    {

        static readonly UnityPlugin _instance = new UnityPlugin();
        public static UnityPlugin Instance { get { return _instance; } }

        #region helper functions to hold callbacks results and send to callbacks each frame update
        public bool IsConnected = false;
        bool _pressureUpdated = false;
        float _pressure = 0;
        bool _gestureUpdated = false;
        GestureType _gesture = GestureType.None;
        bool _quaternionsUpdated = false;
        float[] _quaternions = { 0, 0, 0, 0 };
        bool _airmouseUpdated = false;
        float[] _airmouseXY = { 0, 0 };

        public void setAirMouseXY(float[] airmouseXY)
        {
            Debug.Log("_airmouseXY" + airmouseXY.Length.ToString()+" " + airmouseXY[0].ToString() + " " + airmouseXY[1].ToString());
            _airmouseUpdated = true;
            _airmouseXY = airmouseXY;
        }
        public void updateAirmouseCallbacks()
        {
            if (_airmouseUpdated)
            {
                
                aMudraAirMouseeEvent(_airmouseXY);
                _airmouseUpdated = false;
            }
        }

        public void setPressure(float new_pressure)
        {

            _pressureUpdated = true;
            _pressure = new_pressure;
        }
        public void updatePressureCallbacks()
        {
            if (_pressureUpdated)
            {
                aMudraFingertipPressureEvent(_pressure);
                _pressureUpdated = false;
            }
        }

        public void setGesture(GestureType new_gesture)
        {
            _gestureUpdated = true;
            _gesture = new_gesture;
        }

        public void updateGestureCallbacks()
        {
            if (_gestureUpdated)
            {
                aMudraGestureEvent(_gesture);
                _gestureUpdated = false;
            }
        }

        public void setQuaternion(float[] new_quaternion)
        {
            _quaternionsUpdated = true;
            _quaternions = new_quaternion;
        }
        public void updateQuaternionsCallbacks()
        {
            if (_quaternionsUpdated)
            {
                aMudraIMUEvent(_quaternions);
                _quaternionsUpdated = false;
            }
        }
        #endregion

        #region Android interfaces implemantation

        class OnAirMousePositionChanged : AndroidJavaProxy
        {
            UnityPlugin _unityPlugin;

            public OnAirMousePositionChanged(UnityPlugin unityplugin) : base("MudraAndroidSDK.Mudra$OnAirMousePositionChanged") { _unityPlugin = unityplugin; }

            void run(float[] var1)
            {
               _unityPlugin.setAirMouseXY(var1);
            }

        }


        class OnFingertipPressureReady : AndroidJavaProxy
        {
            UnityPlugin _unityPlugin;

            public OnFingertipPressureReady(UnityPlugin unityplugin) : base("MudraAndroidSDK.Mudra$OnFingertipPressureReady") { _unityPlugin = unityplugin; }

            void run(float var1)
            {
                _unityPlugin.setPressure(var1);
            }
        }

        class OnImuQuaternionReady : AndroidJavaProxy
        {
            UnityPlugin _broadcastReceiver;
            public OnImuQuaternionReady(UnityPlugin unityPlugin) : base("MudraAndroidSDK.Mudra$OnImuQuaternionReady") { _broadcastReceiver = unityPlugin; }

            void run(float[] retObj)
            {
               _broadcastReceiver.setQuaternion(retObj);
 
            }
        }

        class OnGestureReady : AndroidJavaProxy
        {
            UnityPlugin _broadcastReceiver;
            public OnGestureReady(UnityPlugin unityPlugin) : base("MudraAndroidSDK.Mudra$OnGestureReady") { _broadcastReceiver = unityPlugin; }

            void run(AndroidJavaObject retObj)
            {
                _broadcastReceiver.setGesture((GestureType)retObj.Call<int>("ordinal"));

            }
        }

        #endregion

        static bool _init = false;

        #region Android variables
#if (!UNITY_EDITOR && UNITY_ANDROID)

        static AndroidJavaClass _mudraClass;

        static AndroidJavaObject _mudraDevice;

#endif
        #endregion

        public delegate void OnAirMousePosition(float[] airmousexy);
        public OnAirMousePosition aMudraAirMouseeEvent;
        public event OnAirMousePosition onMudraAirMouseEvent
        {
            add
            {

#if (!UNITY_EDITOR && UNITY_ANDROID)
                if (aMudraAirMouseeEvent == null)
                    _mudraDevice.Call("setOnAirMousePositionChanged", new OnAirMousePositionChanged(this));
#endif
                aMudraAirMouseeEvent += value;

            }
            remove
            {
                aMudraAirMouseeEvent -= value;
#if (!UNITY_EDITOR && UNITY_ANDROID)
                if (aMudraAirMouseeEvent == null)
                    _mudraDevice.Call("setOnAirMousePositionChanged", null);
#endif
            }
        }



        public enum GestureType { None, Thumb, Index, Tap };

        #region delegate Mudra events to user callbacks

        public delegate void OnMudraGesture(GestureType gestureID);
        public OnMudraGesture aMudraGestureEvent;
        public event OnMudraGesture onMudraGestureEvent
        {
            add
            {

#if (!UNITY_EDITOR && UNITY_ANDROID)
                if (aMudraGestureEvent == null)
                    _mudraDevice.Call("setOnGestureReady", new OnGestureReady(this));
#endif
                aMudraGestureEvent += value;

            }
            remove
            {
                aMudraGestureEvent -= value;
#if (!UNITY_EDITOR && UNITY_ANDROID)
                if (aMudraGestureEvent == null)
                    _mudraDevice.Call("setOnGestureReady", null);
#endif
            }
        }

        public delegate void OnFingertipPressure(float proportional);
        public OnFingertipPressure aMudraFingertipPressureEvent;
        public event OnFingertipPressure onMudraFingertipPressureEvent
        {
            add
            {
#if (!UNITY_EDITOR && UNITY_ANDROID)
                if (aMudraFingertipPressureEvent == null)
                    _mudraDevice.Call("setOnFingertipPressureReady", new OnFingertipPressureReady(this));
#endif
                aMudraFingertipPressureEvent += value;
            }
            remove
            {
                aMudraFingertipPressureEvent -= value;
#if (!UNITY_EDITOR && UNITY_ANDROID)
                if (aMudraFingertipPressureEvent == null)
                    _mudraDevice.Call("setOnFingertipPressureReady", null);
#endif
            }
        }

        public delegate void OnMudraIMU(float[] imuData);
        public OnMudraIMU aMudraIMUEvent;
        public event OnMudraIMU onMudraIMUEvent
        {
            add
            {
#if (!UNITY_EDITOR && UNITY_ANDROID)
                if (aMudraIMUEvent == null)
                    _mudraDevice.Call("setOnImuQuaternionReady", new OnImuQuaternionReady(this));
#endif
                aMudraIMUEvent += value;
            }
            remove
            {
                aMudraIMUEvent -= value;
#if (!UNITY_EDITOR && UNITY_ANDROID)
                if (aMudraIMUEvent == null)
                    _mudraDevice.Call("setOnImuQuaternionReady", null);
#endif
            }
        }
        #endregion

        UnityPlugin()

        { }

        public void Init()
        {

#if (!UNITY_EDITOR && UNITY_ANDROID)

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
            Debug.Log("this works");

     _mudraClass = new AndroidJavaClass("MudraAndroidSDK.Mudra");
            Debug.Log("this doesnt");

           AndroidJavaClass feature = new AndroidJavaClass("MudraAndroidSDK.Feature");
            AndroidJavaClass jcu = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jcu.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = jo.Call<AndroidJavaObject>("getApplicationContext");
            _mudraDevice = _mudraClass.CallStatic<AndroidJavaObject>("autoConnectPaired", context);
            String LICENSE = "Feature::RawData";
            _mudraDevice.Call("setLicense", 0, LICENSE);
            _init = true;

#endif
        }

        public void setAirMouseScreenSize(int width, int height) {
            if (aMudraGestureEvent == null)
            {
                _mudraDevice.Call("setOnAirMouseScreenSize", width, height);
            }
            else
                Debug.Log("setAirMouseScreenSize Exception - Airmouse callback must be set prior to calling setAirMouseScreenSize");
        }

            public void Update()
        {
            
            if (_init)
            {
#if (!UNITY_EDITOR && UNITY_ANDROID)
                IsConnected = _mudraDevice.Call<bool>("isConnected");
                Debug.Log("device connected?" + IsConnected.ToString());
#endif
                if (aMudraIMUEvent != null)
                    updateQuaternionsCallbacks();
                if (aMudraFingertipPressureEvent != null)
                    updatePressureCallbacks();
                if (aMudraGestureEvent != null)
                    updateGestureCallbacks();
                if (aMudraAirMouseeEvent != null)
                    updateAirmouseCallbacks();


            }
        }
    }
}

