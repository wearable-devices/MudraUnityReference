﻿#undef UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mudra;
using UnityEngine.Events;

[System.Serializable]
public class MudraIMUEvent : UnityEvent<float[]>
{
}

[System.Serializable]
public class MudraAirMouseEvent : UnityEvent<float[]>
{
}

[System.Serializable]
public class MudraGestureEvent : UnityEvent<UnityPlugin.GestureType >
{
}

[System.Serializable]
public class MudraFingertipPressureEvent : UnityEvent<float>
{
}

public class MudraManager : MonoBehaviour
{
  //  static readonly MudraManagerEngine _instance = new MudraManagerEngine();
  //  public static MudraManagerEngine Enable { get { return _instance; } }
    public  MudraIMUEvent IMUEvent;
    public  MudraAirMouseEvent AirMouseEvent;
    public  MudraGestureEvent GestureEvent;
    public  MudraFingertipPressureEvent FingerTipPressureEvent;



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UnityPlugin.Instance.Update();
    }

    public void initMudra()
    {
        UnityPlugin.Instance.Init();
    }

    private void BroadcastReceiver_onIMUEvent(float[] imuData)
    {
        if (IMUEvent != null)
            IMUEvent.Invoke(imuData);
    }

    private void Mudra_onAirMouseevent(float[] AirmouseXY)
    {
        if (AirMouseEvent != null)
            AirMouseEvent.Invoke(AirmouseXY);
    }

    private void BroadcastReceiver_onMudraGestureEvent(UnityPlugin.GestureType gestureID)
    {
        if (GestureEvent != null)
            GestureEvent.Invoke(gestureID);
    }

    private void BroadcastReceiver_onFingerTipPressureEvent(float pressure)
    {
        if (FingerTipPressureEvent != null)
            FingerTipPressureEvent.Invoke(pressure);
    }



        public void Enable_IMU(bool _imu)
        {     
            if (_imu)
                UnityPlugin.Instance.onMudraIMUEvent += BroadcastReceiver_onIMUEvent;
            else
                UnityPlugin.Instance.onMudraIMUEvent -= BroadcastReceiver_onIMUEvent;
        }

        public void Enable_AirMouse(bool _airmouse)
        {
            if (_airmouse)
                UnityPlugin.Instance.onMudraAirMouseEvent += Mudra_onAirMouseevent;
            else
                UnityPlugin.Instance.onMudraAirMouseEvent -= Mudra_onAirMouseevent;
        }

        public void Enable_Pressure(bool _pressure)
        {
            if (_pressure)
                UnityPlugin.Instance.onMudraFingertipPressureEvent += BroadcastReceiver_onFingerTipPressureEvent;
            else
                UnityPlugin.Instance.onMudraFingertipPressureEvent -= BroadcastReceiver_onFingerTipPressureEvent;
        }

        public void Enable_Gestures(bool _gesture)
        {
            if (_gesture)
                UnityPlugin.Instance.onMudraGestureEvent += BroadcastReceiver_onMudraGestureEvent;
            else
                UnityPlugin.Instance.onMudraGestureEvent -= BroadcastReceiver_onMudraGestureEvent;
        }

    }

