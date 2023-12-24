using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public Slider sp_rate_sl;
    public TMP_Dropdown State_dd;
    public GameObject edit_panel;
    public Camera Cam;
    public int cur_st;
    public GameObject Road_Panel;
    private bool road_ed = false;
    Vector3[] positions = new Vector3[2];
    public int LastI = -1;
    public LineRenderer LinePref;
    public LineRenderer Line;

    void Update()
    {
        cur_st = State_dd.value;
        
        if (road_ed)
        {
            Line.SetPosition(1, MouseCords());
        }

        if (!work &&  Input.GetMouseButtonDown(1))
        {
            switch (cur_st)
            {
                case 0:
                    Light_Inst();
                    break;
                case 1:
                    Light_Inst();
                    break;
                case 2:
                    Road_Inst();
                    break;
            }
            
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
        pause_but.GetComponent<Toggle>().isOn = false;
        pause = false;
        edit_panel.SetActive(w);
    }

    public void set_pause(bool w)
    {
        pause = w;
    }

    public void Change_editor_state(int state)
    {
        cur_st = state;
        sp_rate_sl.gameObject.SetActive(state == 1);
        Road_Panel.SetActive(state == 2);
    }

    private int find_nearest()
    {
        Vector3 mouse = MouseCords();
        float minDist = 1000f;
        int minI = 0;
        for (int i = 0; i < lights.Length; i++)
        {
            lighter nLight = lights[i];
            float curDist = Vector3.Distance(nLight.gameObject.GetComponent<Transform>().position, mouse);
            if (curDist < minDist)
            {
                minI = i;
                minDist = curDist;
            }
        }
        return minDist < 9f ? minI : -1;
    }

    private void Road_Inst()
    {  
        int minI = find_nearest();
        if (minI > -1 && !road_ed)
        {
            Line = Instantiate(LinePref);
            Line.positionCount = 2;
            road_ed = true;
            positions[0] = lights[minI].gameObject.transform.position;
            Line.SetPosition(0, positions[0]);
        }
        else if (minI > -1 && road_ed)
        {
            road_ed = false;
            positions[1] = lights[minI].gameObject.transform.position;
            Line.SetPosition(1, positions[1]);
        }
        LastI = minI;
    }

    private void Light_Inst()
    {
        lighter[] li = new lighter[lights.Length + 1];
        
        
        Vector3 light_pos = MouseCords();
        if (lights.Length == 0) 
        { 
            lights = new lighter[1];
            lighter LightObj = Instantiate(light_pref, light_pos, Quaternion.identity);
            LightObj.gameObject.name = "Traffic light " + 0;
            lights[0] = LightObj;
            lights[0].ID = lights.Length;
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
            lights = li;
           
        }
        lights[lights.Length-1].sp_rate = sp_rate_sl.value;
        lights[lights.Length-1].Is_Spawner = cur_st == 1;
        lights[lights.Length - 1].gameObject.GetComponent<SpriteRenderer>().color = cur_st == 1 ? Color.black : Color.green;
    }

    private Vector3 MouseCords()
    {
        Vector3 light_pos = new Vector3();
        Vector3 mPos = Input.mousePosition;
        mPos.z = Cam.nearClipPlane;
        light_pos = Cam.ScreenToWorldPoint(mPos);
        return light_pos;
    }
}
