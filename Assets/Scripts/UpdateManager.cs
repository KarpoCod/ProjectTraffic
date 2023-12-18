using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateManager : MonoBehaviour
{
    public lighter light_pref;
    public lighter[] lights;
    //public road[] roads;
    public float t = 1;
    float timer = 0;
    public bool work = false;
    public bool pause = false;
    public GameObject pause_but;
    public Toggle sp;
    public GameObject[] edit_tools;
    public Camera Cam;

    void Update()
    {
        if (!work && Input.GetMouseButtonDown(1))
        {
            Clicked();
        }
        if (work && !pause)
        {
            if (timer < 0)
            {
                /*foreach (road line in roads)
                {
                    line.Manager_Update();
                }*/
                foreach (lighter blinker in lights)
                {
                    blinker.Manager_Update();
                }
                timer = t;
            }
            else timer -= Time.deltaTime;
        }
    }
    public void set_work(bool w)
    {
        work = !w;
        pause_but.SetActive(!w);
        foreach(GameObject go in edit_tools) go.SetActive(w);
    }
    public void set_pause(bool w)
    {
        pause = w;
    }

    private void Clicked()
    {
        lighter[] li = new lighter[lights.Length + 1];
        
        Vector3 light_pos = new Vector3();
        Vector3 mPos = Input.mousePosition;
        mPos.z = Cam.nearClipPlane;
        light_pos = Cam.ScreenToWorldPoint(mPos);
        if (lights.Length == 0) 
        { 
            lights = new lighter[1];
            lighter LightObj = Instantiate(light_pref, light_pos, Quaternion.identity);
            LightObj.gameObject.name = "Traffic light " + 0;
            lights[0] = LightObj;
            lights[0].ID = lights.Length;
            lights[0].Is_Spawner = sp.isOn;
        }
        else
        {
            for (int i = 0; i < lights.Length; i++)
            {
                li[i] = lights[i];
            }
            lighter LightObj = Instantiate(light_pref, light_pos, Quaternion.identity);
            LightObj.gameObject.name = "Traffic light " + (lights.Length);
            li[lights.Length] = LightObj;
            li[lights.Length].ID = lights.Length;
            li[lights.Length].Is_Spawner = sp.isOn;
            lights = li;
           
        }
    }
}
