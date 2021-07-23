using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    const bool DRAW_INSTANCE = true;
    const int NUM_NORMAL = 128;
    const int NUM_INSTANCE = 128;
    const int MAX_DRAW_COUNT = 128;

    private float _seedX, _seedZ;
    public GameObject cubePrefab;
    private MaterialPropertyBlock props;
    private MeshRenderer meshRenderer;

    float[] tempIndex1 = new float[MAX_DRAW_COUNT];
    float[] tempIndex2 = new float[MAX_DRAW_COUNT];
    float[] tempIndex3 = new float[MAX_DRAW_COUNT];
    float[] tempIndex4 = new float[MAX_DRAW_COUNT];
    Matrix4x4[] tempMatrix = new Matrix4x4[MAX_DRAW_COUNT];

    void Start()
    {
        if(DRAW_INSTANCE)
        {
            InitGPUInstance();
        }
        else
        {
            Init();
        }
    }

    void Init()
    {
        _seedX = 1f;// Random.value * 80f;
        _seedZ = 1f;// Random.value * 80f;
        props = new MaterialPropertyBlock();
        //生成80*80的方块
        for (int i = 0; i < NUM_NORMAL; i++)
        {
            for (int j = 0; j < NUM_NORMAL; j++)
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


    List<Matrix4x4> listMatrix = new List<Matrix4x4>();
    List<float> listIndex1 = new List<float>();
    List<float> listIndex2 = new List<float>();
    List<float> listIndex3 = new List<float>();
    List<float> listIndex4 = new List<float>();
    Mesh mesh;
    Material mat;

    void InitGPUInstance()
    {

        MeshFilter meshF = cubePrefab.GetComponent<MeshFilter>();
        mesh = meshF.mesh;
        mat = cubePrefab.GetComponent<MeshRenderer>().sharedMaterial;

        _seedX = 1f;// Random.value * 80f;
        _seedZ = 1f;// Random.value * 80f;
        props = new MaterialPropertyBlock();
        //生成80*80的方块
        for (int i = 0; i < NUM_INSTANCE; i++)
        {
            for (int j = 0; j < NUM_INSTANCE; j++)
            {
                CreateInstance(i,j);
            }
        }

    }

    void CreateInstance(int x, int z)
    {
        //采样的参数
        float xSample = (x + _seedX) / 15.0f;
        float zSample = (z + _seedZ) / 15.0f;

        float noise = Mathf.PerlinNoise(xSample, zSample);
        float height = 0f;
        if (noise <= 0.1)
        {
            height = 0;
            listIndex1.Add(1);
            listIndex2.Add(0);
            listIndex3.Add(0);
            listIndex4.Add(0);
        }
        else if (noise > 0.1 && noise <= 0.2)
        {
            height = 1;
            listIndex1.Add(0);
            listIndex2.Add(1);
            listIndex3.Add(0);
            listIndex4.Add(0);
        }
        else if (noise > 0.2 && noise <= 0.3)
        {
            height = 2;
            listIndex1.Add(0);
            listIndex2.Add(0);
            listIndex3.Add(1);
            listIndex4.Add(0);
        }
        else if (noise > 0.3 && noise <= 0.4)
        {
            height = 3;
            listIndex1.Add(0);
            listIndex2.Add(0);
            listIndex3.Add(0);
            listIndex4.Add(1);
        }
        else if (noise > 0.4 && noise <= 0.5)
        {
            height = 4;
            listIndex1.Add(0);
            listIndex2.Add(0);
            listIndex3.Add(0);
            listIndex4.Add(1);
        }
        else if (noise > 0.5 && noise <= 0.6)
        {
            height = 5;
            listIndex1.Add(0);
            listIndex2.Add(0);
            listIndex3.Add(0);
            listIndex4.Add(1);
        }
        else if (noise > 0.6 && noise <= 0.7)
        {
            height = 6;
            listIndex1.Add(0);
            listIndex2.Add(0);
            listIndex3.Add(0);
            listIndex4.Add(1);
        }
        else if (noise > 0.7 && noise <= 0.8)
        {
            height = 7;
            listIndex1.Add(0);
            listIndex2.Add(0);
            listIndex3.Add(0);
            listIndex4.Add(1);
        }
        else if (noise > 0.8 && noise <= 0.9)
        {
            height = 8;
            listIndex1.Add(0);
            listIndex2.Add(0);
            listIndex3.Add(0);
            listIndex4.Add(1);
        }
        else if (noise > 0.9)
        {
            height = 9;
            listIndex1.Add(0);
            listIndex2.Add(0);
            listIndex3.Add(0);
            listIndex4.Add(1);
        }

        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(new Vector3(x, height, z), Quaternion.identity, Vector3.one);
        listMatrix.Add(m);
    }

    void UpdateGPUInstance()
    {
        int index = 0;
        int count = 0;

        while (index < listMatrix.Count)
        {
            tempIndex1[count] = listIndex1[index];
            tempIndex2[count] = listIndex2[index];
            tempIndex3[count] = listIndex3[index];
            tempIndex4[count] = listIndex4[index];
            tempMatrix[count] = listMatrix[index];

            if (count == MAX_DRAW_COUNT - 1)
            {
                props.Clear();
                props.SetFloatArray("_Index1", tempIndex1);
                props.SetFloatArray("_Index2", tempIndex2);
                props.SetFloatArray("_Index3", tempIndex3);
                props.SetFloatArray("_Index4", tempIndex4);
                Graphics.DrawMeshInstanced(mesh, 0, mat, tempMatrix, MAX_DRAW_COUNT, props);
                count = 0;
            }
            else
            {
                count++;
            }

            index++;
        }
    }

    public float offset = 0.0f;
    public float distance = 0.0f;
    public float horizontalScale = 0.0f;
    public float decalHeightY = 0.0f;

    void Update()
    {
        if (DRAW_INSTANCE)
            UpdateGPUInstance();

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Shader.SetGlobalVector("_CurvedParam", new Vector4(offset / distance / distance, horizontalScale, 0, 0));
            Shader.SetGlobalFloat("_DecalHeightYParam", decalHeightY);
        }
    }
}
