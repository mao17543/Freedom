using UnityEngine;
using System.Collections;

public class FPSDisplayScript : MonoBehaviour
{
    public float updateInterval = 0.5F;
    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private string fpsText;
    private GameObject[] AllObjects;
    private int DrawCalls;
    void CalculateFPS()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            fpsText = format;
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }

    GUIStyle style = new GUIStyle();

    void Start()
    {
        Application.targetFrameRate = 200;
        timeleft = updateInterval;
        style.fontSize = 60;
        style.normal.textColor = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFPS();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(50.0f, 50.0f, 100.0f, 100.0f), fpsText, style);
    }
}