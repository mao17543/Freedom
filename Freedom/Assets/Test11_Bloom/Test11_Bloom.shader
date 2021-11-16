Shader "Unlit/Test11_Bloom"
{
	Properties
	{
		[HideInInspector]
	_MainTex("Texture", 2D) = "white" {}

	[KeywordEnum(ADDITIVE, SCREEN, COLORED_ADDITIVE, COLORED_SCREEN, DEBUG)]
	_COMPOSITE_TYPE("Composite Type", Float) = 0

		_Parameter("(Threhold, Intensity, SamplingFrequency, -)", Vector) = (0.8, 1.0, 1.0, 0.0)
	}
		SubShader
	{
		ZTest Always Cull Off ZWrite Off

		CGINCLUDE

#include "UnityCG.cginc"
		//#include "Assets/Packages/ImageFilterLibrary/ImageFilterLibrary.cginc"

		sampler2D _MainTex;
	float4    _MainTex_ST;
	float4    _MainTex_TexelSize;
	float4    _Parameter;

#define BRIGHTNESS_THRESHOLD _Parameter.x
#define INTENSITY            _Parameter.y
#define SAMPLING_FREQUENCY   _Parameter.z

	ENDCG

		// STEP:1
		// Get resized brightness image.

		Pass
	{
		CGPROGRAM

#pragma vertex vert_img
#pragma fragment frag

		fixed4 frag(v2f_img input) : SV_Target
	{
		float4 color = tex2D(_MainTex, input.uv);
		return max(color - BRIGHTNESS_THRESHOLD, 0) * INTENSITY;
	}

		ENDCG
	}

		// STEP:2, 3
		// Get blurred brightness image.

		CGINCLUDE

		struct v2f_gaussian
	{
		float4 pos    : SV_POSITION;
		half2  uv     : TEXCOORD0;
		half2  offset : TEXCOORD1;
	};

	static const float4 GaussianFilterKernel[7] =
	{
		float4(0.0205, 0.0205, 0.0205, 0),
		float4(0.0855, 0.0855, 0.0855, 0),
		float4(0.232,  0.232,  0.232,  0),
		float4(0.324,  0.324,  0.324,  1),
		float4(0.232,  0.232,  0.232,  0),
		float4(0.0855, 0.0855, 0.0855, 0),
		float4(0.0205, 0.0205, 0.0205, 0)
	};

	float4 GaussianFilter(sampler2D tex, float4 texST, float2 texCoord, float2 offset)
	{
		float4 color = 0;

		texCoord = texCoord - offset * 3;

		for (int i = 0; i < 7; i++)
		{
			color += tex2D(tex, UnityStereoScreenSpaceUVAdjust(texCoord, texST))
				* GaussianFilterKernel[i];

			texCoord += offset;
		}

		return color;
	}

	float4 frag_gaussian(v2f_gaussian input) : SV_Target
	{
		return GaussianFilter(_MainTex, _MainTex_ST, input.uv, input.offset);
	}

		ENDCG


		Pass
	{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag_gaussian

			v2f_gaussian vert(appdata_img v)
		{
			v2f_gaussian o;

			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			o.offset = _MainTex_TexelSize.xy * float2(1, 0) * SAMPLING_FREQUENCY;

			return o;
		}

		ENDCG
	}


	Pass
	{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag_gaussian

			v2f_gaussian vert(appdata_img v)
		{
			v2f_gaussian o;

			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			o.offset = _MainTex_TexelSize.xy * float2(0, 1) * SAMPLING_FREQUENCY;

			return o;
		}

		ENDCG
	}

	// STEP:4
	// Composite to original.

	Pass
	{
		CGPROGRAM

#pragma vertex vert_img
#pragma fragment frag
#pragma multi_compile _COMPOSITE_TYPE_ADDITIVE _COMPOSITE_TYPE_SCREEN _COMPOSITE_TYPE_COLORED_ADDITIVE _COMPOSITE_TYPE_COLORED_SCREEN _COMPOSITE_TYPE_DEBUG

			sampler2D _CompositeTex;
		float4    _CompositeColor;

		fixed4 frag(v2f_img input) : SV_Target
		{
			float4 mainColor = tex2D(_MainTex,      input.uv);
			float4 compositeColor = tex2D(_CompositeTex, input.uv);

#if defined(_COMPOSITE_TYPE_COLORED_ADDITIVE) || defined(_COMPOSITE_TYPE_COLORED_SCREEN)

			//compositeColor.rgb = (compositeColor.r + compositeColor.g + compositeColor.b) * 0.3333 * _CompositeColor;

#endif

#if defined(_COMPOSITE_TYPE_SCREEN) || defined(_COMPOSITE_TYPE_COLORED_SCREEN)

			return saturate(mainColor + compositeColor - saturate(mainColor * compositeColor));
			//return fixed4(1,0,0,1);

#elif defined(_COMPOSITE_TYPE_ADDITIVE) || defined(_COMPOSITE_TYPE_COLORED_ADDITIVE)

			return saturate(mainColor + compositeColor);

#else

			return compositeColor;
			//return fixed4(0, 0, 1, 1);

#endif
		}

			ENDCG
	}


	}
}