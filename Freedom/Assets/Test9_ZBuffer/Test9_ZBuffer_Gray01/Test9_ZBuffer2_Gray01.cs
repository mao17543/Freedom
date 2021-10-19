using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test9_ZBuffer2_Gray01 : MonoBehaviour
{
    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
