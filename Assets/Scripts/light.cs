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
    public direction[] ways = new direction[1];
    public bool Is_Active = true;
    private int past_time = 0;
    private Dictionary<int, int> delays = new Dictionary<int, int>();

    [System.Serializable]    
    public class direction
    {
        public int ID = 0;
        public int wide = 1;
        public int car_queue;
        public bool Is_Open = false;
        public road[] possible_ways;
        public int wait = 0;
        public int OnOutput = 0;
        //public int[] delays; 
        /*void Start() 
        { 
            car_queues = new int[wide]; 
            possible_ways = new int[wide]; 
            delays = new int[wide]; 
        }*/
        public void Uptd()
        {
            wait++;
            OnOutput = 0;
            if (Is_Open)
            {
                if (car_queue - wide < 0) { OnOutput = car_queue; car_queue = 0; }
                else { OnOutput = wide; car_queue -= wide; }
                while(OnOutput > 0)
                {
                    int rID = Random.Range(0, possible_ways.Length);
                    int[] cars = new int[possible_ways[rID].wide];
                    int rNum = Random.Range(0, possible_ways[rID].wide);
                    for (int x = 0; x <= rNum; x++)
                    {
                        cars[x] = 1;
                    }
                    possible_ways[rID].Input_Units(cars);
                    OnOutput -= rNum;
                }
            }
        }
        public void Spawner()
        {
            for (int rID = 0; rID < possible_ways.Length; rID++)
            {
                int[] cars = new int[possible_ways[rID].wide];
                int rNum = Random.Range(0, possible_ways[rID].wide);
                for (int x = 0; x <= rNum; x++)
                {
                    cars[x] = 1;
                }
                possible_ways[rID].Input_Units(cars);
            }
            
        }
    }


    public void Add_car(int directionID)
    {
        if (Is_Active)
        {
            if (ways.Length - 1 <= directionID)
            {
                ways[directionID].car_queue++;
                UnityEngine.Debug.Log("единица успешно добавлена на ID" + directionID.ToString());
            }
            else UnityEngine.Debug.LogError("ввода под таким номером не существует!" + directionID.ToString());
        }
        else UnityEngine.Debug.LogWarning("данный светофор не работает");
    }

    void Start()
    {
        foreach (direction Direct in ways)
        {
            delays.Add(Direct.ID, Mathf.CeilToInt(Direct.car_queue / Direct.wide) + Mathf.FloorToInt(Direct.wait >> 1));
        }
    }

    public void Manager_Update()
    {
        if (!Is_Spawner)
        {
            int[] priorities = new int[ways.Length];
            foreach (direction Direct in ways)
            {
                Direct.Uptd();
                priorities[Direct.ID] = Mathf.CeilToInt(Direct.car_queue / Direct.wide) + Mathf.FloorToInt(Direct.wait >> 2);
            }
            for (int i = 0; i < priorities.Length; i++)
            {
                delays[i] = priorities[i];
            }

            if (past_time == 0)
            {
                var dict = delays.OrderBy(x => x.Value).ToDictionary<KeyValuePair<int, int>, int, int>(pair => pair.Key, pair => pair.Value);

                int index = dict.Keys.ToArray()[0];
                past_time = priorities[index];
                UnityEngine.Debug.Log(past_time);

            }
            else past_time--;
        }
        else
        {
            foreach (direction Direct in ways)
            {
                Direct.Spawner();
            }
        }
    }
}
