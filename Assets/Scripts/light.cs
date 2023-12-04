using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class light : MonoBehaviour
{
    public bool Is_Spawner = false;
    public int ID;
    public int Max_Delay;
    public direction[] InputLines = new direction[1];
    public bool Is_Active = true;
    private int past_time = 0;
    private Dictionary<int, int> delays = new Dictionary<int, int>();

    public void Add_car(int directionID, int[] Cars)
    {
        int numOfCars = Cars.Count(i => i.Equals(1));
        if (Is_Active)
        {
            if (InputLines.Length - 1 >= directionID)
            {
                if (InputLines[directionID].wide >= numOfCars) {
                    InputLines[directionID].car_queue += numOfCars;
                    //UnityEngine.Debug.Log("единицы успешно добавлены на ID" + directionID.ToString());
                }
                else UnityEngine.Debug.LogError(("ввода под таким номером {0} не может принять такое кол-во единиц {1}!", directionID, numOfCars));
            }
            else UnityEngine.Debug.LogError("ввода под таким номером не существует!" + directionID.ToString() + ID.ToString());
        }
        else UnityEngine.Debug.LogWarning("данный светофор не работает");
    }

    void Start()
    {
        foreach (direction Direct in InputLines) delays.Add(Direct.ID, Mathf.CeilToInt(Direct.car_queue / Direct.wide) + Mathf.FloorToInt(Direct.wait >> 1));
    }

    int index = 0;

    public void Manager_Update()
    {
        
        if (!Is_Spawner)
        { 
            int[] priorities = new int[InputLines.Length];
            foreach (direction Direct in InputLines)
            {
                Direct.Uptd();
                priorities[Direct.ID] = Mathf.CeilToInt(Direct.car_queue / Direct.wide) + Mathf.FloorToInt(Direct.wait >> 2);
                Direct.Is_Open = false;
            }
            InputLines[index].Is_Open = true;
            for (int i = 0; i < priorities.Length; i++) delays[i] = priorities[i];
            if (past_time == 0)
            {
                var dict = delays.OrderBy(x => x.Value).ToDictionary<KeyValuePair<int, int>, int, int>(pair => pair.Key, pair => pair.Value);
                string ou = "list ";
                foreach (int i in dict.Values.ToArray()) {
                    ou += i.ToString() + " "; }

                UnityEngine.Debug.Log(ou);

                int[] keys = dict.Keys.ToArray();

                index = keys[keys.Length - 1];
                past_time = priorities[index];
                InputLines[index].Is_Open = true;
                UnityEngine.Debug.Log(past_time + " " + index + " " + ID);
            }
            else past_time--;
        }
        else
        {
            foreach (direction Direct in InputLines)
            {
                Direct.car_queue = 0;
                Direct.Spawner();
            }
        }
    }
}

[Serializable]
public class direction
{
    public float sp_rate = 0.7f;
    public int ID = 0;
    public int wide = 1;
    public int car_queue;
    public bool Is_Open = false;

    [Serializable]
    public class OutLine
    {
        public light light;
        public int ID_of_inp;
    }
    public OutLine[] Outs;
    public int wait = 0;
    //public int[] delays; 
    /*void Start() 
    { 
        car_queues = new int[wide]; 
        Outs = new int[wide]; 
        delays = new int[wide]; 
    }*/
    public void Uptd()
    {
        wait++;
        int OnOutput = 0;
        if (Is_Open)
        {
            wait = 0;
            if (car_queue - wide <= 0) { OnOutput = car_queue; car_queue = 0; }
            else { OnOutput = wide; car_queue -= wide; }
            while (OnOutput > 0)
            {
                int rID = UnityEngine.Random.Range(0, Outs.Length);
                int[] cars = new int[Outs[rID].light.InputLines[Outs[rID].ID_of_inp].wide];
                int rNum = UnityEngine.Random.Range(1, Outs[rID].light.InputLines[Outs[rID].ID_of_inp].wide);
                for (int x = 0; x < rNum; x++) cars[x] = 1;
                Outs[rID].light.Add_car(Outs[rID].ID_of_inp, cars);
                OnOutput -= rNum;
            }
        }
    }
    public void Spawner()
    {
        for (int rID = 0; rID < Outs.Length; rID++)
        {
            int[] cars = new int[Outs[rID].light.InputLines[Outs[rID].ID_of_inp].wide];
            int rNum = (int) Math.Round(UnityEngine.Random.Range(0, Outs[rID].light.InputLines[Outs[rID].ID_of_inp].wide) * sp_rate);
            if(rNum != 0)for (int x = 0; x <= rNum - 1; x++) cars[x] = 1;
            Outs[rID].light.Add_car(Outs[rID].ID_of_inp, cars);
        }
    }
}