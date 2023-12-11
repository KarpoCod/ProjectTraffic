using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public light[] lights;
    public road[] roads;
    public float t = 1;
    float timer = 0;
    public bool work = false;

    void Update()
    {
        if (work)
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
        work = w;
    }
}
