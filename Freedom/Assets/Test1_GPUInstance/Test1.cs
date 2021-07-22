using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    private float _seedX, _seedZ;
    public GameObject cubePrefab;
    private MaterialPropertyBlock props;
    private MeshRenderer meshRenderer;

    void Start()
    {

        _seedX = Random.value * 80f;
        _seedZ = Random.value * 80f;
        props = new MaterialPropertyBlock();
        //生成80*80的方块
        for (int i = 0; i < 80; i++)
        {
            for (int j = 0; j < 80; j++)
            {
                GameObject cube = Instantiate(cubePrefab);
                cube.transform.localPosition = new Vector3(i, 0, j);
                cube.transform.SetParent(transform);
                //根据perlin噪声赋值高度
                SetY(cube);
            }
        }
    }

    void SetY(GameObject cube)
    {
        float height = 0;
        //采样的参数
        float xSample = (cube.transform.localPosition.x + _seedX) / 15.0f;
        float zSample = (cube.transform.localPosition.z + _seedZ) / 15.0f;
        //生成perlin噪声图
        float noise = Mathf.PerlinNoise(xSample, zSample);
        //根据不同的值设定高度以及贴图
        meshRenderer = cube.GetComponent<MeshRenderer>();
        if (noise <= 0.1)
        {
            height = 0;
            props.SetFloat("_Index1", 1);
            props.SetFloat("_Index2", 0);
            props.SetFloat("_Index3", 0);
            props.SetFloat("_Index4", 0);
            meshRenderer.SetPropertyBlock(props);
        }
        else if (noise > 0.1 && noise <= 0.2)
        {
            height = 1;
            props.SetFloat("_Index1", 0);
            props.SetFloat("_Index2", 1);
            props.SetFloat("_Index3", 0);
            props.SetFloat("_Index4", 0);
            meshRenderer.SetPropertyBlock(props);
        }
        else if (noise > 0.2 && noise <= 0.3)
        {
            height = 2;
            props.SetFloat("_Index1", 0);
            props.SetFloat("_Index2", 0);
            props.SetFloat("_Index3", 1);
            props.SetFloat("_Index4", 0);
            meshRenderer.SetPropertyBlock(props);
        }
        else if (noise > 0.3 && noise <= 0.4)
        {
            height = 3;
            props.SetFloat("_Index1", 0);
            props.SetFloat("_Index2", 0);
            props.SetFloat("_Index3", 0);
            props.SetFloat("_Index4", 1);
            meshRenderer.SetPropertyBlock(props);
        }
        else if (noise > 0.4 && noise <= 0.5)
        {
            height = 4;
            props.SetFloat("_Index1", 0);
            props.SetFloat("_Index2", 0);
            props.SetFloat("_Index3", 0);
            props.SetFloat("_Index4", 1);
            meshRenderer.SetPropertyBlock(props);
        }
        else if (noise > 0.5 && noise <= 0.6)
        {
            height = 5;
            props.SetFloat("_Index1", 0);
            props.SetFloat("_Index2", 0);
            props.SetFloat("_Index3", 0);
            props.SetFloat("_Index4", 1);
            meshRenderer.SetPropertyBlock(props);
        }
        else if (noise > 0.6 && noise <= 0.7)
        {
            height = 6;
            props.SetFloat("_Index1", 0);
            props.SetFloat("_Index2", 0);
            props.SetFloat("_Index3", 0);
            props.SetFloat("_Index4", 1);
            meshRenderer.SetPropertyBlock(props);
        }
        else if (noise > 0.7 && noise <= 0.8)
        {
            height = 7;
            props.SetFloat("_Index1", 0);
            props.SetFloat("_Index2", 0);
            props.SetFloat("_Index3", 0);
            props.SetFloat("_Index4", 1);
            meshRenderer.SetPropertyBlock(props);
        }
        else if (noise > 0.8 && noise <= 0.9)
        {
            height = 8;
            props.SetFloat("_Index1", 0);
            props.SetFloat("_Index2", 0);
            props.SetFloat("_Index3", 0);
            props.SetFloat("_Index4", 1);
            meshRenderer.SetPropertyBlock(props);
        }
        else if (noise > 0.9)
        {
            height = 9;
            props.SetFloat("_Index1", 0);
            props.SetFloat("_Index2", 0);
            props.SetFloat("_Index3", 0);
            props.SetFloat("_Index4", 1);
            meshRenderer.SetPropertyBlock(props);
        }
        cube.transform.localPosition = new Vector3(cube.transform.localPosition.x, height, cube.transform.localPosition.z);
    }
}
