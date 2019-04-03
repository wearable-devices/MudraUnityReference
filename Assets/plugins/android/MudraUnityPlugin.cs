//################################################
#undef UNITY_EDITOR
using UnityEngine;
using System;
using UnityEngine.Android;


namespace Mudra
{
    public enum GestureType { None, Thumb, Index, Tap };
    public delegate void OnMudraGesture(GestureType gestureID);
    public delegate void OnFingertipPressure(float proportional);
    public delegate void OnMudraIMU(float[] imuData);
    public delegate void OnAirMousePosition(float[] airmousexy);

    public sealed class UnityPlugin
    {

        static readonly UnityPlugin _instance = new UnityPlugin();
        public static UnityPlugin Instance { get { return _instance; } }
        void NOP() { }

        #region helper functions to hold callbacks results and send to callbacks each frame update
        static bool _init = false;
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
            Debug.Log("gesture was set"+ _gesture);
        }

        public void updateGestureCallbacks()
        {
            if (_gestureUpdated)
            {
                Debug.Log("gesture updated");
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

      


        #region delegate Mudra events to user callbacks

        public OnAirMousePosition aMudraAirMouseeEvent;
        public event OnAirMousePosition onMudraAirMouseEvent
        {
            add
            {
                if (aMudraAirMouseeEvent == null)
                {
#if (MUDRA_UDP)
                    NOP();
#elif (!UNITY_EDITOR && UNITY_ANDROID)
                    MudraAndroidPlugin.Instance.setOnAirMouseReady(true);
#endif
                }
                    aMudraAirMouseeEvent += value;
                

            }
            remove
            {
                aMudraAirMouseeEvent -= value;
                if (aMudraAirMouseeEvent == null)
                {
#if (MUDRA_UDP)
                    NOP();
#elif (!UNITY_EDITOR && UNITY_ANDROID)
                    MudraAndroidPlugin.Instance.setOnAirMouseReady(false);
#endif
                }
            }
        }

        public OnMudraGesture aMudraGestureEvent;
        public event OnMudraGesture onMudraGestureEvent
        {
            add
            {
                if (aMudraGestureEvent == null)
                {
#if (MUDRA_UDP)
                    NOP();
#elif (!UNITY_EDITOR && UNITY_ANDROID)
                    MudraAndroidPlugin.Instance.setOnGestureReady(true);
#endif
                }
                aMudraGestureEvent += value;
            }
            remove
            {
                aMudraGestureEvent -= value;
                if (aMudraGestureEvent == null)
                {
#if (MUDRA_UDP)
                    NOP();
#elif (!UNITY_EDITOR && UNITY_ANDROID)
                    MudraAndroidPlugin.Instance.setOnGestureReady(false);
#endif
                }
            }
        }


        public OnFingertipPressure aMudraFingertipPressureEvent;
        public event OnFingertipPressure onMudraFingertipPressureEvent
        {
            add
            {
                if (aMudraFingertipPressureEvent == null)
                {
#if (MUDRA_UDP)
                    NOP();
#elif (!UNITY_EDITOR && UNITY_ANDROID)
                    MudraAndroidPlugin.Instance.setOnFingertipPressureReady(true);
#endif
                }
                aMudraFingertipPressureEvent += value;
            }
            remove
            {
                aMudraFingertipPressureEvent -= value;
                if (aMudraFingertipPressureEvent == null)
                {
#if (MUDRA_UDP)
                   NOP();
#elif (!UNITY_EDITOR && UNITY_ANDROID)
                   MudraAndroidPlugin.Instance.setOnFingertipPressureReady(false);
#endif
                }
            }
        }


        public OnMudraIMU aMudraIMUEvent;
        public event OnMudraIMU onMudraIMUEvent
        {
            add
            {
                if (aMudraIMUEvent == null)
                { 
#if (MUDRA_UDP)
                    NOP();
#elif (!UNITY_EDITOR && UNITY_ANDROID)
                    MudraAndroidPlugin.Instance.setOnImuQuaternionReady(true);
#endif
                }
                aMudraIMUEvent += value;
            }
            remove
            {
                aMudraIMUEvent -= value;
                if (aMudraIMUEvent == null)
                {
#if (MUDRA_UDP)
                   NOP();
#elif (!UNITY_EDITOR && UNITY_ANDROID)
                    MudraAndroidPlugin.Instance.setOnImuQuaternionReady(false);
#endif
                }
            }
        }
        #endregion

        UnityPlugin()

        { }

        public void Init()
        {
            if (!_init)
            {
#if (MUDRA_UDP)
                MudraUDPPlugin.Instance.Init();
#elif (!UNITY_EDITOR && UNITY_ANDROID)
            MudraAndroidPlugin.Instance.Init();
#endif
                _init = true;
            }
            
        }


            public void Update()
        {

            if (_init)
            {

                if (aMudraIMUEvent != null)
                    updateQuaternionsCallbacks();
                if (aMudraFingertipPressureEvent != null)
                    updatePressureCallbacks();
                if (aMudraGestureEvent != null)
                    updateGestureCallbacks();
                if (aMudraAirMouseeEvent != null)
                    updateAirmouseCallbacks();


#if (MUDRA_UDP)
                MudraUDPPlugin.Instance.Update();
                IsConnected = MudraUDPPlugin.IsConnected;
#elif (!UNITY_EDITOR && UNITY_ANDROID)
                MudraAndroidPlugin.Instance.Update();
                IsConnected = MudraAndroidPlugin.Instance.IsConnected;
#endif
            }
#if (MUDRA_UDP)
            if (!_init)            
                Init();         
#endif

        }

    }

}

