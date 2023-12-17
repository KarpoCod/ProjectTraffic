using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public light[] lights;
    //public road[] roads;
    public float t = 1;
    float timer = 0;
    public bool work = false;
    public bool pause = false;
    public GameObject pause_but;
    public GameObject[] edit_tools;

    void Update()
    {
        if (work && !pause)
        {
            if (timer < 0)
            {
                /*foreach (road line in roads)
                {
                    line.Manager_Update();
                }*/
                foreach (light blinker in lights)
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
}
