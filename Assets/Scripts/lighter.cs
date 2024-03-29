using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lighter : MonoBehaviour//�������� ����� ���������
{
    public float sp_rate = 0.7f;
    public bool Is_Spawner = false;
    public int ID;
    public int Max_Delay;
    public direction[] InputLines = new direction[1];
    public bool Is_Active = true;
    private int past_time = 0;
    private Dictionary<int, int> delays = new Dictionary<int, int>();
    public OutLine[] LightOuts;

    public void Add_Out(lighter light, int IdOfInput)
    {
        UnityEngine.Debug.Log(("new out on {0} on inp {1} of {2}", ID, IdOfInput, light.ID));
        OutLine[] Lines = new OutLine[LightOuts.Length + 1];
        if (LightOuts.Length == 0) { LightOuts = new OutLine[] { new OutLine(light, IdOfInput) }; return; }
        for (int i = 0; i < LightOuts.Length; i++)
        {
            Lines[i] = LightOuts[i];
        }
        Lines[LightOuts.Length] = new OutLine(light, IdOfInput);
        LightOuts = Lines;
    }

    public int Add_Direct(int Wide, int Len)
    {
        direction[] Lines = new direction[InputLines.Length + 1];
        if (InputLines.Length == 0) { InputLines = new direction[] { new direction(0, Wide, Len) }; return 0; }
        for (int i = 0; i < InputLines.Length; i++)
        {
            Lines[i] = InputLines[i];
        }
        Lines[InputLines.Length] = new direction(InputLines.Length, Wide, Len);
        InputLines = Lines;
        return InputLines.Length - 1;
    }

    public int Add_car(int directionID, int[] Cars)//���������� N ����� �� ���� �� ����������� ���������
    {
        int numOfCars = Cars.Count(i => i.Equals(1));//�������� ����� ������� ������� �� ������
        int Outs = numOfCars;
        if (Is_Active)
        {
            if (InputLines.Length - 1 >= directionID)
            {
                if (InputLines[directionID].wide >= numOfCars)
                {
                    if (InputLines[directionID].wide * InputLines[directionID].len >= InputLines[directionID].car_queue + numOfCars) { InputLines[directionID].car_queue += numOfCars; Outs = numOfCars; }
                    else Outs = InputLines[directionID].wide * InputLines[directionID].len - InputLines[directionID].car_queue;
                    InputLines[directionID].car_queue += numOfCars - Outs;
                    //UnityEngine.Debug.Log("������� ������� ��������� �� ID" + directionID.ToString());
                    return Outs;
                }
                else UnityEngine.Debug.LogError(("���� ��� ����� ������� {0} �� ����� ������� ����� ���-�� ������ {1}!", directionID, numOfCars));
            }
            else UnityEngine.Debug.LogError("����� ��� ����� ������� �� ����������!" + directionID.ToString() + ID.ToString());
        }
        else UnityEngine.Debug.LogWarning("������ �������� �� ��������");

        return 0;
    }

    public void INIT()
    {
        foreach (direction Direct in InputLines)
        {
            delays = new Dictionary<int, int>();
            delays.Add(Direct.ID, Mathf.CeilToInt(Direct.car_queue / Direct.wide) + Mathf.FloorToInt(Direct.wait >> 1));//�������� ������� � �������� �������� �� ������ ���������
            Direct.INIT(LightOuts);
        }
    }

    int index = 0;

    public void Manager_Update()//������� ����������� ������ ���������
    {
        if (!Is_Spawner)
        {
            foreach (direction Direct in InputLines)
            {
                Direct.Uptd(Is_Spawner);
                delays[Direct.ID] = Mathf.CeilToInt(Direct.car_queue / Direct.wide) + Mathf.FloorToInt(Direct.wait >> 2);
                Direct.Is_Open = false;
            }//������������� ����������� ��������� ������� ����� �� ������ �� ����������� �� ���������
            InputLines[index].Is_Open = true;//��������� ������� ����� �� ������ ������
            if (past_time == 0)
            {
                var dict = delays.OrderBy(x => x.Value).ToDictionary<KeyValuePair<int, int>, int, int>(pair => pair.Key, pair => pair.Value);
                /*string ou = "list ";
                foreach (int i in dict.Values.ToArray()) ou += i.ToString() + " "; //����� ������� ������� ��������� (�����)
                UnityEngine.Debug.Log(ou);*/

                int[] keys = dict.Keys.ToArray();

                index = keys[keys.Length - 1];
                if (index > 30) index = 30;
                past_time = delays[index];
                InputLines[index].Is_Open = true;
                UnityEngine.Debug.Log(past_time + " " + index + " " + ID);
            }//���������� ������� ������ ������� � ��������� ����������
            else past_time--;
        }
        else
        {
                InputLines[0].car_queue = 0;
                InputLines[0].Spawner(sp_rate);
            // ��������� ��������� ����� �� ������ ����������� � ����������� �� �������������
        }
    }
}

[Serializable]
public class OutLine //��������� ����������� �� ����� � ���� �� ����� �����
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

[Serializable]
public class direction//����� ���������� ����� ���������
{
    public Text indicator;
    public int ID = 0;
    public int wide = 1;
    public int len = 1;
    public int car_queue;
    public bool Is_Open = false;

    public direction(int id, int wi, int l)
    {
        ID = id;
        wide = wi;
        len = l;
    }

    public OutLine[] Outs;
    public int wait;
    /*public int[] delays; 
    void Start() 
    { 
        car_queues = new int[wide]; 
        Outs = new int[wide]; 
        delays = new int[wide]; 
    }*/



    public void INIT(OutLine[] LightOuts)
    {
        indicator.text = "0%";
        Outs = LightOuts;
        car_queue = 0;
        wait = 0;
    }


    public void Uptd(bool Is_Spawner)//���������� 
    {
        if (Is_Spawner) { car_queue = 0; }
        indicator.text = (car_queue * 100 / (wide * len)).ToString() + "%";
        if(car_queue > 0) wait++;
        int OnOutput = 0;
        if (Is_Open)
        {
            OutLine[] Pos_Outs = new OutLine[Outs.Length];
            for (int i = 0; i < Outs.Length; i++) Pos_Outs[i] = new OutLine(Outs[i].light, Outs[i].ID_of_inp);
            //Pos_Outs = Outs;

            wait = 0;
            if (car_queue - wide <= 0) { OnOutput = car_queue; car_queue = 0; }
            else { OnOutput = wide; car_queue -= wide; }
            while (OnOutput > 0 && Pos_Outs.Count(i => i.pos.Equals(true)) != 0)
            {
                int rID = UnityEngine.Random.Range(0, Pos_Outs.Length);
                while (Pos_Outs[rID].pos == false)
                {
                    if (rID < Pos_Outs.Length - 1) rID++;
                    else rID = 0;
                }
                int[] cars = new int[Pos_Outs[rID].light.InputLines[Pos_Outs[rID].ID_of_inp].wide];
                int p = Pos_Outs[rID].light.InputLines[Pos_Outs[rID].ID_of_inp].wide;
                int rNum = UnityEngine.Random.Range(1, p > OnOutput ? OnOutput : p);
                for (int x = 0; x < rNum; x++) cars[x] = 1;
                int m = Pos_Outs[rID].light.Add_car(Pos_Outs[rID].ID_of_inp, cars);
                if (m == 0) Pos_Outs[rID].pos = false;
                OnOutput -= m;
            }
            car_queue += OnOutput;
        }
    }
    public void Spawner(float sp_rate)
    {
        for (int rID = 0; rID < Outs.Length; rID++)
        {
            int[] cars = new int[Outs[rID].light.InputLines[Outs[rID].ID_of_inp].wide];
            int rNum = (int)Math.Round(UnityEngine.Random.Range(0, Outs[rID].light.InputLines[Outs[rID].ID_of_inp].wide * 100) * sp_rate / 100);
            if (rNum < 0 ) rNum = 0;
            if (rNum != 0) for (int x = 0; x <= rNum - 1; x++) cars[x] = 1;
            Outs[rID].light.Add_car(Outs[rID].ID_of_inp, cars);
        }
    }
}
