Shader "Unlit/Test12_FogOfWar"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Texture", 2D) = "white" {}
	_BaseTex("Base Texture", 2D) = "white" {}
	_CloudSpeed("CloudSpeed",Float) = 0.5
	}

		SubShader
	{
		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Opaque" }

		ZTest Off
		ZWrite Off Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;

		float2 baseUV : TEXCOORD1;
	};

	sampler2D _MainTex;
	sampler2D _BaseTex;
	float4 _BaseTex_ST;
	half _CloudSpeed;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		o.baseUV.xy = TRANSFORM_TEX(o.uv.xy, _BaseTex);
		o.uv = v.uv;

		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		half4 col = tex2D(_MainTex, saturate(i.uv));
		half4 baseColor = tex2D(_BaseTex, i.baseUV - _Time.x * _CloudSpeed);

		col.a = (1 - col.r) * 0.95;

		col.rgb = baseColor.rgb;

		return col;
	}
		ENDCG
	}
	}
}
