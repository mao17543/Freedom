using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test9_ZBuffer_ScannerEffect : MonoBehaviour
{
    public Material mat;
    public float velocity = 5;
    private bool isScanning;
    private float dis;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isScanning)
        {
            this.dis += Time.deltaTime * this.velocity;
        }

        //无人深空中按c开启扫描 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.isScanning = true;
            this.dis = 0;
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        mat.SetFloat("_ScanDistance", dis);   //ScannerEffect
        Graphics.Blit(src, dst, mat);
    }
}
