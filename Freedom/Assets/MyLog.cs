using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLog : MonoBehaviour
{
    const int MAX_LOG = 5;
    const float ORIGINAL_X = 50.0f;
    const float ORIGINAL_Y = 100.0f;
    const float LINESPACING = 30.0F;
    const int FONT_SIZE = 30;

    static Queue<string> _logs = new Queue<string>();

    GUIStyle style = new GUIStyle();
    void Start()
    {
        style.fontSize = FONT_SIZE;
        style.normal.textColor = Color.green;
    }

    public static void Add(string inLog)
    {
        if(_logs.Count > MAX_LOG)
        {
            _logs.Dequeue();
        }

        _logs.Enqueue(inLog);
    }

    public static void Clear()
    {
        _logs.Clear();
    }

    void OnGUI()
    {
        int index = 0;
        foreach(string log in _logs)
        {
            GUI.Label(new Rect(ORIGINAL_X, ORIGINAL_Y + LINESPACING* index, 500.0f, 100.0f), log, style);
            index++;
        }
    }
}
