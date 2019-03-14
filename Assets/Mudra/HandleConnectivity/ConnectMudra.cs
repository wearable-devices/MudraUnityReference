using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mudra;

public class ConnectMudra : MonoBehaviour
{
    public string SceneToCallOnConnect;
    public GameObject LandscapImage;
    public GameObject PortraitImage;
    // Start is called before the first frame update
    void Start()
    {

        UnityPlugin.Instance.Init();
    }

    // Update is called once per frame
    void Update()
    {
        int swidth = Camera.main.pixelWidth;
        int sheight = Camera.main.pixelHeight;
        if (LandscapImage != null)
            LandscapImage.SetActive(swidth > sheight);
        if (PortraitImage != null)
            PortraitImage.SetActive(sheight > swidth);
        UnityPlugin.Instance.Update();
        if (UnityPlugin.Instance.IsConnected)
            SceneManager.LoadScene(SceneToCallOnConnect);
    }
}
