using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;

public class lighter : MonoBehaviour//основной класс светофора
{
    public bool Is_Spawner = false;
    public int ID;
    public int Max_Delay;
    public direction[] InputLines = new direction[1];
    public bool Is_Active = true;
    private int past_time = 0;
    private Dictionary<int, int> delays = new Dictionary<int, int>();

    public int Add_car(int directionID, int[] Cars)//добавление N машин на одно из направлений светофора
    {
        int numOfCars = Cars.Count(i => i.Equals(1));//возможно потом добавлю деление на полосы
        int Outs = numOfCars;
        if (Is_Active)
        {
            if (InputLines.Length - 1 >= directionID)
            {
                if (InputLines[directionID].wide >= numOfCars)
                {
                    if (InputLines[directionID].wide * InputLines[directionID].len >= InputLines[directionID].car_queue + numOfCars) { InputLines[directionID].car_queue += numOfCars; Outs = 0; }
                    else Outs = numOfCars - InputLines[directionID].wide * InputLines[directionID].len + InputLines[directionID].car_queue;
                    InputLines[directionID].car_queue += numOfCars - Outs;
                    //UnityEngine.Debug.Log("единицы успешно добавлены на ID" + directionID.ToString());
                }
                else UnityEngine.Debug.LogError(("ввода под таким номером {0} не может принять такое кол-во единиц {1}!", directionID, numOfCars));
            }
            else UnityEngine.Debug.LogError("ввода под таким номером не существует!" + directionID.ToString() + ID.ToString());
        }
        else UnityEngine.Debug.LogWarning("данный светофор не работает");
        return Outs;
    }

    void Start()
    {
        foreach (direction Direct in InputLines) delays.Add(Direct.ID, Mathf.CeilToInt(Direct.car_queue / Direct.wide) + Mathf.FloorToInt(Direct.wait >> 1));//создание словаря с временем ожидания на каждом светофоре
    }

    int index = 0;

    public void Manager_Update()//главный управляющий скрипт светофора
    {
        
        if (!Is_Spawner)
        { 
            foreach (direction Direct in InputLines)//распределение приоритетов включения зелёного света на каждом из направлений на светофоре
            {
                Direct.Uptd();
                delays[Direct.ID] = Mathf.CeilToInt(Direct.car_queue / Direct.wide) + Mathf.FloorToInt(Direct.wait >> 2);
                Direct.Is_Open = false;
            }
            InputLines[index].Is_Open = true;//включение зелёного света на нужном канале
            if (past_time == 0)//обновление времени работы каналов и включение следующего
            {
                var dict = delays.OrderBy(x => x.Value).ToDictionary<KeyValuePair<int, int>, int, int>(pair => pair.Key, pair => pair.Value);
                //string ou = "list ";
                //foreach (int i in dict.Values.ToArray()) ou += i.ToString() + " "; //вывод текущей очереди включения (дебаг)
                //UnityEngine.Debug.Log(ou);

                int[] keys = dict.Keys.ToArray();

                index = keys[keys.Length - 1];
                past_time = delays[index];
                InputLines[index].Is_Open = true;
                UnityEngine.Debug.Log(past_time + " " + index + " " + ID);
            }
            else past_time--;
        }
        else
        {
            foreach (direction Direct in InputLines)// случайный спавн машин на каждом направлении
            {
                Direct.car_queue = 0;
                Direct.Spawner();
            }
        }
    }
}

[Serializable]
public class direction//класс отдельного ВХОДА светофора
{
    public float sp_rate = 0.7f;
    public int ID = 0;
    public int wide = 1;
    public int len = 1;
    public int car_queue;
    public bool Is_Open = false;

    [Serializable]
    public class OutLine //доступные направления на выход с узла от этого ВХОДА
    {
        public lighter light;
        public int ID_of_inp;
        public bool pos = true;
        public OutLine(lighter l, int ID)
        {
            pos = true;
            light = l;
            ID_of_inp = ID;
        }
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
    public void Uptd()//обновление 
    {
        wait++;
        int OnOutput = 0;
        if (Is_Open)
        {
            OutLine[] Pos_Outs = new OutLine[Outs.Length];
            for(int i = 0; i < Outs.Length; i++) Pos_Outs[i] = new OutLine(Outs[i].light, Outs[i].ID_of_inp);
            //Pos_Outs = Outs;

            wait = 0;
            if (car_queue - wide <= 0) { OnOutput = car_queue; car_queue = 0; }
            else { OnOutput = wide; car_queue -= wide; }
            while (OnOutput > 0 && Pos_Outs.Count(i => i.pos.Equals(false)) < Pos_Outs.Length)
            {
                int rID = UnityEngine.Random.Range(0, Pos_Outs.Length);
                while (Pos_Outs[rID].pos == false) 
                { 
                    if (rID < Pos_Outs.Length - 1) rID++;
                    else rID = 0;
                }
                int[] cars = new int[Pos_Outs[rID].light.InputLines[Pos_Outs[rID].ID_of_inp].wide];
                int rNum = UnityEngine.Random.Range(1, Pos_Outs[rID].light.InputLines[Pos_Outs[rID].ID_of_inp].wide);
                for (int x = 0; x < rNum; x++) cars[x] = 1;
                int m = Pos_Outs[rID].light.Add_car(Pos_Outs[rID].ID_of_inp, cars);
                if (m == 0) Pos_Outs[rID].pos = false;
                OnOutput -= m;
            }
            car_queue += OnOutput;
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