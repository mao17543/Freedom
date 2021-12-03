using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Test9_FogOfWar : MonoBehaviour {
    //[System.NonSerialized]
    public RenderTexture fowRT = null;
    public Mesh fowMesh = null;
    public CommandBuffer fowCB = null;
    public Material MatRender;

    public Material MatFogOfWar;
    public Material matCreateFogRT = null;

    MaterialPropertyBlock _fowProp = null;
    public MaterialPropertyBlock fowProp {
        get
        {
            if(_fowProp == null)
            {
                _fowProp = new MaterialPropertyBlock();
            }

            return _fowProp;
        }
    }

    // Use this for initialization
    void Start () {
        InitMatCreateFogRT();
        InitMesh();
        InitCommandBuffer();
        InitMap();
    }

    bool a = true;
    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            TestDraw();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
        }
	}

    #region Init
    void InitMatCreateFogRT()
    {
        if(matCreateFogRT != null)
        {
            matCreateFogRT.SetVector("_ViewBounds", new Vector4(0f, 0f, 0.1f, 0.1f));
        }
    }

    void InitCommandBuffer()
    {
        for (int i = 0; i < MAX_INSTANCING_COUNT; i++)
            IdentityMatrices[i] = Matrix4x4.identity;
        fowRT = new RenderTexture(1024, 1024, 0, RenderTextureFormat.R8);
        fowRT.name = "[FogOfWar - RT]";
        fowRT.filterMode = FilterMode.Bilinear;
        fowRT.wrapMode = TextureWrapMode.Mirror;
        //fowRT.autoGenerateMips = true;
        //fowRT.useMipMap = true;
        fowRT.Create();

        MatRender.mainTexture = fowRT;
        MatFogOfWar.mainTexture = fowRT;

        fowCB = new CommandBuffer();
        fowCB.name = "[FogOfWar-CB]";
    }

    void InitMesh()
    {
        fowMesh = Hexagons.MiscUtil.CreatePlane(1.0f, 1.0f, 0.0f, 0.0f, 1, Hexagons.MiscUtil.PlaneType.XZ, Hexagons.MiscUtil.VertexLayout.Position | Hexagons.MiscUtil.VertexLayout.Texcoord);
    }
    #endregion

    int[][] map = new int[10][];
    const int MAX_INSTANCING_COUNT = 128;
    Vector4[] trans = new Vector4[MAX_INSTANCING_COUNT];
    Matrix4x4[] IdentityMatrices = new Matrix4x4[MAX_INSTANCING_COUNT];

    void InitMap()
    {
        map[0] = new int[10] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        map[1] = new int[10] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 };
        map[2] = new int[10] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
        map[3] = new int[10] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 };
        map[4] = new int[10] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 };
        map[5] = new int[10] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 };
        map[6] = new int[10] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 };
        map[7] = new int[10] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 };
        map[8] = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 };
        map[9] = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
    }

    void TestDraw()
    {
        fowProp.Clear();
        int count = 0;
        fowCB.Clear();
        fowCB.SetRenderTarget(fowRT);
        fowCB.ClearRenderTarget(true, true, Color.black);

        for (int x = 0; x < 10; ++x)
        {
            for (int y = 0; y < 10; ++y)
            {
                if (map[x][y] == 0)
                    continue;

                trans[count++].Set(x, y, 0f, 1.0f);

                if (count >= 128)
                {
                    fowProp.SetVectorArray("_Trans", trans);
                    fowCB.DrawMeshInstanced(fowMesh, 0, matCreateFogRT, 0, IdentityMatrices, count, fowProp);
                    count = 0;
                }
            }
        }

        if (count > 0)
        {
            fowProp.SetVectorArray("_Trans", trans);
            fowCB.DrawMeshInstanced(fowMesh, 0, matCreateFogRT, 0, IdentityMatrices, count, fowProp);
        }

        Graphics.ExecuteCommandBuffer(fowCB);
        //fowCB.Clear();
    }
}
