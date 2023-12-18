using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public light light_pref;
    public light[] lights;
    //public road[] roads;
    public float t = 1;
    float timer = 0;
    public bool work = false;
    public bool pause = false;
    public GameObject pause_but;
    public GameObject[] edit_tools;
    public Camera Cam;

    void Update()
    {
        if (!work && Input.GetMouseButtonDown(0))
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

    private void Clicked()
    {
        light[] li = new light[lights.Length + 1];
        Debug.Log(Input.mousePosition);
        Vector3 light_pos = new Vector3();
        Vector3 mPos = Input.mousePosition; 
        Debug.Log(Cam.ScreenToWorldPoint(mPos));
        Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            light_pos = hit.point;
        }
        if (lights.Length == 0) { lights = new light[1]; lights[0] = Instantiate(light_pref, light_pos, Quaternion.identity); }
        else
        {
            for (int i = 0; i < lights.Length; i++)
            {
                li[i] = lights[i];
            }
            lights[lights.Length - 1] = Instantiate(light_pref, light_pos, Quaternion.identity);
        }
    }
}
