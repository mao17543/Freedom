﻿Shader "Unlit/TestRender"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Opaque" }

		//ZTest Off
		//ZWrite Off Cull Off
		Blend Off

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "Assets/Modules/TerrainSystem/RenderResources/TerrainCommon.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

		sampler2D _MainTex;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;

		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		half4 col = tex2D(_MainTex, i.uv);
		return col;
	}
		ENDCG
	}
	}
}