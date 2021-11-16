using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class Test11_Bloom : MonoBehaviour
{
    #region Enum

    public enum CompositeType
    {
        _COMPOSITE_TYPE_ADDITIVE = 0,
        _COMPOSITE_TYPE_SCREEN = 1,
        _COMPOSITE_TYPE_COLORED_ADDITIVE = 2,
        _COMPOSITE_TYPE_COLORED_SCREEN = 3,
        _COMPOSITE_TYPE_DEBUG = 4
    }

    #endregion Enum

    #region Field

    private static Dictionary<CompositeType, string> CompositeTypes = new Dictionary<CompositeType, string>()
    {
        { CompositeType._COMPOSITE_TYPE_ADDITIVE,         CompositeType._COMPOSITE_TYPE_ADDITIVE.ToString()         },
        { CompositeType._COMPOSITE_TYPE_SCREEN,           CompositeType._COMPOSITE_TYPE_SCREEN.ToString()           },
        { CompositeType._COMPOSITE_TYPE_COLORED_ADDITIVE, CompositeType._COMPOSITE_TYPE_COLORED_ADDITIVE.ToString() },
        { CompositeType._COMPOSITE_TYPE_COLORED_SCREEN,   CompositeType._COMPOSITE_TYPE_COLORED_SCREEN.ToString()   },
        { CompositeType._COMPOSITE_TYPE_DEBUG,            CompositeType._COMPOSITE_TYPE_DEBUG.ToString()            }
    };

    public Material material;

    public CompositeType compositeType = CompositeType._COMPOSITE_TYPE_ADDITIVE;

    [Range(0, 1)]
    public float threshold = 0.8f;

    [Range(0, 10)]
    public float intensity = 10f;

    [Range(1, 10)]
    public float size = 3.5f;

    [Range(1, 10)]
    public int divide = 2;

    [Range(1, 5)]
    public int iteration = 2;

    public Color color = Color.white;

    private int idCompositeTex = 0;
    private int idCompositeColor = 0;
    private int idParameter = 0;

    #endregion Field

    #region Method

    protected void Start()
    {
        if (!SystemInfo.supportsImageEffects
         || !this.material
         || !this.material.shader.isSupported)
        {
            base.enabled = false;
        }

        this.idCompositeTex = Shader.PropertyToID("_CompositeTex");
        this.idCompositeColor = Shader.PropertyToID("_CompositeColor");
        this.idParameter = Shader.PropertyToID("_Parameter");
    }

    protected void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture resizedTex1 = RenderTexture.GetTemporary(source.width / this.divide,
                                                               source.height / this.divide,
                                                               source.depth,
                                                               source.format);
        RenderTexture resizedTex2 = RenderTexture.GetTemporary(resizedTex1.descriptor);

        //Graphics.Blit(source, resizedTex1);

        // STEP:0
        // Get resized birghtness image.

        material.SetVector(this.idParameter, new Vector3(this.threshold, this.intensity, this.size));

        Graphics.Blit(source, resizedTex1, material, 0);

        //Graphics.Blit(resizedTex1, destination);
        //return;

        // STEP:1,2
        // Get blurred brightness image.

        for (int i = 1; i <= this.iteration; i++)
        {
            Graphics.Blit(resizedTex1, resizedTex2, material, 1);
            Graphics.Blit(resizedTex2, resizedTex1, material, 2);

            material.SetVector(this.idParameter, new Vector3(this.threshold, this.intensity, this.size + i));
        }

        // STEP:3
        // Composite.

        material.EnableKeyword(CompositeTypes[this.compositeType]);
        material.SetColor(this.idCompositeColor, this.color);
        material.SetTexture(this.idCompositeTex, resizedTex1);

        Graphics.Blit(source, destination, material, 3);

        // STEP:4
        // Close.

        material.DisableKeyword(CompositeTypes[this.compositeType]);

        RenderTexture.ReleaseTemporary(resizedTex1);
        RenderTexture.ReleaseTemporary(resizedTex2);

    }

    #endregion Method
}