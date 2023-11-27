using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class road : MonoBehaviour
{
    /*public int ID;
    public int lenght = 1;
    public int wide = 1;
    public light Output;
    public int DirectionId;
    public int[][] Road_Matrix;
    private int[] inputs;
    private int[] outputs;
    void Start()
    {
        Road_Matrix = new int[lenght][];
        for (int i = 0; i < lenght; i++)
        {
            Road_Matrix[i] = new int[wide];
        }
        inputs = new int[wide];
    }
    public void Input_Units(int[] cars)
    {
        if (cars.Length <= inputs.Length) 
        {
            inputs = cars;
        }
        else UnityEngine.Debug.LogError("на вход дороги направленной на " + Output.ID.ToString() + " подаЄтс€ больше юнитов чем максимально возможное количиство");
    }
    public void Manager_Update()
    {        ///сдвиг матрицы
        for (int i = lenght - 1; i >= 0; i--)
        {
            if (i == lenght - 1) outputs = Road_Matrix[i];
            else if (i != 0) Road_Matrix[i] = Road_Matrix[i - 1]; else Road_Matrix[i] = inputs;
        }
        for (int x = 0; x < outputs.Count(i => i.Equals(1)); x++) Output.Add_car(DirectionId);
        UnityEngine.Debug.Log("с дороги " + ID + " на направление " + DirectionId + " ушло единиц " + outputs.Count(i => i.Equals(1)));
    }*/
}
