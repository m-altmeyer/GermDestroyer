using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Logger : MonoBehaviour
{

    string logfile_path;
    string statelog_path;
    private void Awake()
    {
        createLogfile();
        createStateLog();
        //Debug.Log(logfile_path);
    }


    public void createLogfile()
    {
        //android/data/com.dfki.handwashgame/files/
        logfile_path = Path.Combine(Application.persistentDataPath, "logfile-" + System.DateTime.Now.ToString("dd-MM-yyyy") + ".csv");
        if (!File.Exists(logfile_path))
            File.Create(logfile_path);
        //Debug.Log("File created at path: " + logfile_path);
    }

    public void createStateLog()
    {
        statelog_path = Path.Combine(Application.persistentDataPath, "statelog-" + System.DateTime.Now.ToString("dd-MM-yyy") + ".csv");
        if (!File.Exists(statelog_path))
            File.Create(statelog_path);
        //Debug.Log("File created at path: " + statelog_path);
    }

    public void logEvent(string message) {
        StreamWriter writer = new StreamWriter(logfile_path, true);
                        //Example Log: timestamp|state|condition
        writer.WriteLine(System.DateTime.Now.ToString("HH:mm:ss") + ";" + message + ";" + FindObjectOfType<GameManager>().condition + ";");
        writer.Close();
        //Debug.Log("wrote in " + logfile_path);
    }

    public void logState(string state, float duration){
        //Timestamp für neuen User.
        StreamWriter writer = new StreamWriter(statelog_path, true);
                        //Example Log: timestamp|state|duration|condition
        
        writer.WriteLine(System.DateTime.Now.ToString("HH:mm:ss") + ";" + state +";" + duration + ";" + FindObjectOfType<GameManager>().condition + ";");
        writer.Close();
        //Debug.Log("wrote in " + statelog_path);
    }

    public void logState(string state)
    {
        StreamWriter writer = new StreamWriter(statelog_path, true);
        //Example Log: timestamp|state|duration|condition
        writer.WriteLine(System.DateTime.Now.ToString("HH:mm:ss") + ";" + state + ";" + FindObjectOfType<GameManager>().condition + ";");
        writer.Close();
        //Debug.Log("wrote in " + statelog_path);
    }
    

}
