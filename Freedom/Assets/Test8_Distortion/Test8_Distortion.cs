using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test8_Distortion : MonoBehaviour
{
    float time = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            time = 10.0f;
        }

        if(time > float.Epsilon)
        {
            time -= Time.deltaTime;
            float scale = 10 - time;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }
}
