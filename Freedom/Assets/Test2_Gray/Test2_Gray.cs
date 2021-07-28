using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2_Gray : MonoBehaviour
{
    Vector3 mousePos, hitPosition;
    public Camera mainCamera;
    Ray ray;
    RaycastHit hit;
    [Range(0.0f, 100.0f)]
    public float radius = 3;
    [Range(0.0f, 100.0f)]
    public float softness = 4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        ray = mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out hit))
        {
            Shader.SetGlobalVector("_Test2_Position", hit.point);
        }

        Shader.SetGlobalFloat("_Test2_Radius", radius);
        Shader.SetGlobalFloat("_Test2_Softness", softness);
    }
}
