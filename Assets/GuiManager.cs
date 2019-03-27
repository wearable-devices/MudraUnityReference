using Mudra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    public Text prop_text;
    public Text gesture_text;
    public Text IMU_text;
    public Text AirMouse_text;
    public Transform Cube;
    public Transform Cursor;
    public float AirMouseSpeedX = 6f;
    public float AirMouseSpeedY = 3f;

    float mAirMousePosX, mAirMousePosY;

    Quaternion _prevQuat;
    Quaternion _IMUquat;
    Vector3 _orientation, _orientation_prev;
    public void onIMUEvent(float[] imuData)
    {
        _IMUquat.Set(imuData[0], imuData[1], imuData[2], imuData[3]);
        _prevQuat.Set(0, 0, 0, -1);
        Cube.localRotation = _prevQuat * Quaternion.Euler(0, _IMUquat.eulerAngles.z, 0);
        //    _IMUquat = _IMUquat * Quaternion.Euler(0.0f - _IMUquat.eulerAngles.x, 0.0f - _IMUquat.eulerAngles.y, 0.0f );
             IMU_text.text = _IMUquat[0].ToString() + " " + _IMUquat[1].ToString() + " " + _IMUquat[2].ToString() + " " + _IMUquat[3].ToString();
        //  _orientation .Set(0, 1, 0);
        //     _IMUquat* _prevQuat*Quaternion.Inverse(_IMUquat);
      //  Cube.localRotation = Quaternion.Euler(0, 0, _IMUquat.eulerAngles.z);
    }

    Vector3 mousepos;
    public void onAirMouseEvent(float[] AirmouseXY)
    {
        int swidth = Camera.main.pixelWidth;
        int sheight = Camera.main.pixelHeight;
        mAirMousePosX += AirmouseXY[0] * AirMouseSpeedX * swidth;
        mAirMousePosY -= AirmouseXY[1] * AirMouseSpeedY * sheight;
        mAirMousePosX = Mathf.Clamp(mAirMousePosX, 0.0f * swidth, 1.0f * swidth);
        mAirMousePosY = Mathf.Clamp(mAirMousePosY, 0.0f * sheight, 1.0f * sheight);

        AirMouse_text.text = (mAirMousePosX).ToString() + " " + (mAirMousePosY).ToString();
        mousepos.Set(mAirMousePosX, mAirMousePosY, Camera.main.transform.position.z * -1.0f);
        Cursor.localPosition = Camera.main.ScreenToWorldPoint(mousepos);
    }

    public void onGestureEvent(UnityPlugin.GestureType gestureID)
    {
        Debug.Log("BroadcastReceiver_onMudraGestureEvent called" + gestureID.ToString());
        gesture_text.text = gestureID.ToString();
    }

    public void onFingerTipPressureEvent(float proportional)
    {
        Debug.Log(" BroadcastReceiver_onMudraProportionalEvent called" + proportional.ToString());
        prop_text.text = proportional.ToString();

    }


}
