using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;
using System.IO;
using System;

public class MarkerInteraction : MonoBehaviour
{
    public LineRenderer prefab_line;
    public List<LineRenderer> lines = new List<LineRenderer>();
    public LineRenderer line_current;
    public bool is_triggered;
    public int num_points;

    private List<float[]> data = new List<float[]>();
    private int num_trial;

    // Start is called before the first frame update
    void Start()
    {
        is_triggered = false;
        num_points = 0;
        num_trial = 0;
    }

    public void OnReset()
    {
        is_triggered = false;
        num_points = 0;

        for(int i = 0; i < lines.Count; i++)
        {
            Destroy(lines[i]);
        }
        lines.Clear();
        data.Clear();
    }

    public void OnCreate()
    {
        //Debug.Log("selected!!");
        int prev = lines.Count;
        lines.Add(Instantiate(prefab_line, gameObject.transform));
        if(prev + 1 == lines.Count){
            // success
            line_current = lines[prev];
        }

        if(num_points != 0){
            Debug.Log("the var [num_points] is not cleared!");
        }
        is_triggered = true;
    }

    public void OnCancel()
    {
        //Debug.Log("cleared!!");
        is_triggered = false;
        num_points = 0;
    }

    void FixedUpdate()
    {
        if(is_triggered)
        {
            float[] row = new float[7];
            Vector3 curPos = gameObject.transform.position;

            row[0] = (float)num_trial;
            row[1] = (float)lines.Count;
            row[2] = (float)num_points;
            row[3] = Time.time;
            row[4] = curPos.x;
            row[5] = curPos.y;
            row[6] = curPos.z;

            data.Add(row);

            line_current.positionCount += 1;
            line_current.SetPosition(num_points++, curPos);
        }
    }

    public void OnSave()
    {
        string filepath = Application.dataPath + "/Resources/" + String.Format("data{0:00}.csv", num_trial++);
        
        StreamWriter outStream = System.IO.File.CreateText(filepath);

        string index = String.Format("trial,line,points,time,x position,y position,z position,");
        outStream.WriteLine(index);

        for(int i=0; i<data.Count; i++){
            string row=String.Format("{0},{1},{2},{3},{4},{5},{6},", data[i][0], data[i][1], data[i][2], data[i][3], data[i][4], data[i][5], data[i][6]);
            outStream.WriteLine(row);
        }

        outStream.Close();

        OnReset();
    }
}
