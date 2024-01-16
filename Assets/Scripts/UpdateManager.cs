using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;

public class UpdateManager : MonoBehaviour
{
    public float globOffset = 0.5f;
    public GameObject indic;
    public lighter light_pref;
    public lighter[] lights;
    public float t = 1;
    float timer = 0;
    public bool work = false;
    public bool pause = false;
    public GameObject pause_but;
    public Slider sp_rate_sl;
    public TMP_Dropdown State_dd;
    public TMP_InputField Len;
    public TMP_InputField Lines;
    public Toggle Direct;
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

        if (!work && Input.GetMouseButtonDown(1))
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
            if (timer < 0)//обновление всех светофоров раз в такт
            {
                foreach (lighter blinker in lights)
                {
                    blinker.Manager_Update();
                }
                timer = t;
            }
            else timer -= Time.deltaTime;
        }
    }

    //работа с меню
    public void set_work(bool w)
    {
        work = !w;
        pause_but.SetActive(!w);
        pause_but.GetComponent<Toggle>().isOn = false;
        pause = false;
        edit_panel.SetActive(w);
        if (!w) foreach (lighter l in lights) l.INIT();
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

    //работа с редактированием
    private void Road_Inst()
    {
        int len = int.TryParse(Len.text, out len) ? len : 1;
        int minI = find_nearest();
        int wide = int.TryParse(Lines.text, out wide) ? wide : 1;
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
            Add_Transition(lights[LastI], lights[minI], len, wide);

            if (Direct.isOn) { Add_Transition(lights[minI], lights[LastI], len, wide); }
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
        lights[lights.Length - 1].sp_rate = sp_rate_sl.value;
        lights[lights.Length - 1].Is_Spawner = cur_st == 1;
        lights[lights.Length - 1].gameObject.GetComponent<SpriteRenderer>().color = cur_st == 1 ? Color.black : Color.green;
    }

    //вспомогательный модуль
    private Vector3 MouseCords()
    {
        Vector3 world_pos = new Vector3();
        Vector3 mPos = Input.mousePosition;
        mPos.z = Cam.nearClipPlane;
        world_pos = Cam.ScreenToWorldPoint(mPos);
        return world_pos;
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

    public void Add_Transition(lighter from, lighter to, int wide, int length)
    {
        int id = to.Add_Direct(wide, length);
        from.Add_Out(to, id);
        GameObject indicator_text_obj = Instantiate(indic);
        float midX = (from.gameObject.transform.position.x + to.gameObject.transform.position.x) / 2;
        float midY = (from.gameObject.transform.position.y + to.gameObject.transform.position.y) / 2;
        float difX = from.gameObject.transform.position.x - to.gameObject.transform.position.x;
        float difY = from.gameObject.transform.position.y - to.gameObject.transform.position.y;
        float xOffset = globOffset * Mathf.Abs(difY) / (Mathf.Abs(difY) + Mathf.Abs(difX));
        float yOffset = globOffset * Mathf.Abs(difX) / (Mathf.Abs(difY) + Mathf.Abs(difX));
        indicator_text_obj.transform.SetParent(edit_panel.transform.parent); 
        if (difY > 0) xOffset = -xOffset;
        if (difX < 0) yOffset = -yOffset;
        indicator_text_obj.transform.position = new Vector3(midX + xOffset, midY + yOffset, 100);
        to.InputLines[id].indicator = indicator_text_obj.GetComponent<Text>();
    }
}
