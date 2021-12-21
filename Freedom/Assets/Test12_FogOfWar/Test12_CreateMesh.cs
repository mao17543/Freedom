using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Test12_CreateMesh : MonoBehaviour
{
    int maxX = 0;
    int maxY = 0;
    BitArray bits = null;
    public float scaleUp = 2f;

    const int MAX_INSTANCING_COUNT = 128;
    Matrix4x4[] IdentityMatrices = new Matrix4x4[MAX_INSTANCING_COUNT];
    Vector4[] trans = new Vector4[MAX_INSTANCING_COUNT];

    //[System.NonSerialized]
    public RenderTexture fowRT = null;
    public CommandBuffer fowCB = null;

    public MeshRenderer fogRenderer;
    public Material matFogMainBaseCreateRt = null;

    static MaterialPropertyBlock _fowProp = null;
    static MaterialPropertyBlock fowProp
    {
        get
        {
            if (_fowProp == null)
            {
                _fowProp = new MaterialPropertyBlock();
            }

            return _fowProp;
        }
    }

    static Mesh _fowMesh = null;
    static Mesh fowMesh
    {
        get
        {
            if (_fowMesh == null)
            {
                _fowMesh = CreatePlane(0.5f, 0.5f, 0.0f, 0.0f, 1, PlaneType.XZ, VertexLayout.Position | VertexLayout.Texcoord);
            }

            return _fowMesh;
        }
    }

    #region plane
    public enum PlaneType
    {
        XY,
        XZ,
        YZ
    }

    public enum VertexLayout
    {
        Position = 0x1,
        Normal = 0x2,
        Texcoord = 0x4,
        All = Position | Normal | Texcoord,
    }

    public static List<Vector3> vertexCache = new List<Vector3>(128);
    public static List<Vector2> uvCache = new List<Vector2>(128);
    public static List<Color> colorCache = new List<Color>(128);
    public static List<int> indexCache = new List<int>(128);

    public static Mesh CreatePlane(float width, float height, float pivotW = 0.0f, float pivotH = 0.0f, int sub = 1, PlaneType planeType = PlaneType.XZ, VertexLayout vlayout = VertexLayout.Position)
    {
        vertexCache.Clear();
        colorCache.Clear();
        uvCache.Clear();
        indexCache.Clear();

        var halfW = width * 0.5f;
        var halfH = height * 0.5f;

        var vertW = sub + 1;
        var vertH = sub + 1;

        var stepW = width / sub;
        var stepH = height / sub;
        var stepU = 1.0f / sub;
        var stepV = 1.0f / sub;

        var startW = -halfW - pivotW;
        var startH = -halfH - pivotH;

        var hasUV = (vlayout & VertexLayout.Texcoord) != 0;
        var wp = 0.0f;
        var hp = 0.0f;
        var uv = Vector2.zero;
        var vertex = Vector3.zero;
        for (int j = 0; j < vertH; j++)
        {
            hp = startH + j * stepH;
            uv.y = j * stepV;
            for (int i = 0; i < vertW; i++)
            {
                wp = startW + i * stepW;
                uv.x = i * stepU;
                switch (planeType)
                {
                    case PlaneType.XY:
                        vertex.Set(wp, hp, 0);
                        break;
                    case PlaneType.XZ:
                        vertex.Set(wp, 0, hp);
                        break;
                    case PlaneType.YZ:
                        vertex.Set(0, wp, hp);
                        break;
                }
                vertexCache.Add(vertex);

                if (hasUV)
                    uvCache.Add(uv);
            }
        }

        for (int j = 0; j < sub; j++)
        {
            for (int i = 0; i < sub; i++)
            {
                var ridx = j * vertW;
                var nridx = (j + 1) * vertW;
                indexCache.Add(ridx + i);
                indexCache.Add(nridx + i);
                indexCache.Add(ridx + i + 1);

                indexCache.Add(ridx + i + 1);
                indexCache.Add(nridx + i);
                indexCache.Add(nridx + i + 1);
            }
        }

        var mesh = new Mesh();
        mesh.SetVertices(vertexCache);
        if (hasUV)
            mesh.SetUVs(0, uvCache);
        mesh.SetTriangles(indexCache, 0);

        if ((vlayout & VertexLayout.Normal) != 0)
        {
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }
#if !UNITY_EDITOR
            mesh.UploadMeshData(true);
#endif
        return mesh;
    }
    #endregion

    //test
    public int unlockX = 0;
    public int unlockY = 0;

    // Use this for initialization
    void Awake()
    {
        InitCommandBuffer();
        SetMapSize(36, 44);
        Draw();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            bits.SetAll(false);
            ResetDraw();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            SetBits(unlockX, unlockY);
            Draw();
        }
    }

    #region Init
    void InitCommandBuffer()
    {
        for (int i = 0; i < MAX_INSTANCING_COUNT; i++)
            IdentityMatrices[i] = Matrix4x4.identity;

        fowRT = new RenderTexture(1024, 1024, 0, RenderTextureFormat.R8);
        fowRT.name = "[FogOfMainBase - RT]";
        fowRT.filterMode = FilterMode.Bilinear;
        fowRT.wrapMode = TextureWrapMode.Mirror;
        //fowRT.autoGenerateMips = true;
        //fowRT.useMipMap = true;
        fowRT.Create();

        fowCB = new CommandBuffer();
        fowCB.name = "[FogOfMainBase-CB]";

        if (fogRenderer != null && fogRenderer.sharedMaterial != null)
        {
            fogRenderer.sharedMaterial.mainTexture = fowRT;
        }
    }

    #endregion

    int CoordToIndex(int bx, int by)
    {
        if (bx < 0 || bx >= maxX || by < 0 || by >= maxY)
            return -1;
        return by * maxX + bx;
    }

    Vector2Int IndexToCoord(int bidx)
    {
        return new Vector2Int(bidx % maxX, bidx / maxX);
    }

    public void Init()
    {
        InitCommandBuffer();
        SetMapSize(36, 44);
        Draw();

        if (bits != null)
        {
            bits.SetAll(false);
        }
    }

    public void SetBits(int x, int y)
    {
        int index = CoordToIndex(x, y);
        if (bits != null && index >= 0 && index < bits.Length)
        {
            bits[index] = true;
        }
    }

    bool GetBits(int x, int y)
    {
        int index = CoordToIndex(x, y);
        if (bits != null && index >= 0 && index < bits.Length)
        {
            return bits[index];
        }

        return false;
    }

    public void Draw()
    {
        fowProp.Clear();
        fowCB.Clear();
        fowCB.SetRenderTarget(fowRT);
        fowCB.ClearRenderTarget(true, true, Color.black);

        int count = 0;
        for (int x = 0; x < maxX; ++x)
        {
            for (int y = 0; y < maxY; ++y)
            {
                if (!GetBits(x, y))
                    continue;

                trans[count++].Set(x, y, 0f, 1.0f);

                if (count >= 128)
                {
                    fowProp.SetVectorArray("_Trans", trans);
                    fowCB.DrawMeshInstanced(fowMesh, 0, matFogMainBaseCreateRt, 0, IdentityMatrices, count, fowProp);
                    count = 0;
                }
            }
        }

        if (count > 0)
        {
            fowProp.SetVectorArray("_Trans", trans);
            fowCB.DrawMeshInstanced(fowMesh, 0, matFogMainBaseCreateRt, 0, IdentityMatrices, count, fowProp);
        }

        Graphics.ExecuteCommandBuffer(fowCB);
    }

    public void SetMapSize(float row, float col)
    {
        if (matFogMainBaseCreateRt != null)
        {
            //matFogMainBaseCreateRt.SetVector("_ViewBounds", new Vector4(0f, 0f, 0.1f, 0.1f));
            float offset = (scaleUp - 1) / 2;
            matFogMainBaseCreateRt.SetVector("_ViewBounds", new Vector4(row * offset, col * offset, 1f / row / scaleUp, 1f / col / scaleUp));
        }

        maxX = (int)row;
        maxY = (int)col;

        bits = new BitArray(maxX * maxY, false);

        if (fogRenderer != null)
        {
            float s = 0.0331f * scaleUp;
            fogRenderer.transform.localPosition = Vector3.zero;
            fogRenderer.transform.localScale = new Vector3(s * col, 1, s * row);
        }
    }

    public void ResetDraw()
    {
        fowCB.Clear();
        fowCB.SetRenderTarget(fowRT);
        fowCB.ClearRenderTarget(true, true, Color.black);

        Graphics.ExecuteCommandBuffer(fowCB);
    }

    public void SetVisible(bool inIsVisible)
    {
        if (gameObject != null)
        {
            gameObject.SetActive(inIsVisible);
        }
    }

    public void UnlockFogs(List<int> listIndex)
    {
        if (listIndex != null && listIndex.Count > 0)
        {
            for (int i = 0; i < listIndex.Count; i++)
            {
                Vector2Int vec2 = IndexToCoord(listIndex[i]);
                SetBits(vec2.x, vec2.y);
            }

            Draw();
        }
    }
}
