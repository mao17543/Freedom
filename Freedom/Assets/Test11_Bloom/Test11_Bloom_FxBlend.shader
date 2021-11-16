Shader "Unlit/Test11_Bloom_Fx"
{
	Properties{
		[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_ColorPow("Color Power",Range(0,5)) = 1
		_MainTex("Particle Texture", 2D) = "white" {}
	_ScrollSpeedX("Scroll Speed X",Range(-10,10)) = 0
		_ScrollSpeedY("Scroll Speed Y",Range(-10,10)) = 0

		// _InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		[HideInInspector]_MainTex_ST_Proxy("MainTex_ST Proxy", Vector) = (1, 1, 0, 0)
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest",Float) = 8
	}

		Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off
		ZTest[_ZTest]

		SubShader{
		LOD 100
		Pass{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#pragma multi_compile_instancing
#include "UnityCG.cginc"

		sampler2D _MainTex;
	float4 _MainTex_ST;
	half _ScrollSpeedX;
	half _ScrollSpeedY;


	UNITY_INSTANCING_BUFFER_START(MyProperties)
		UNITY_DEFINE_INSTANCED_PROP(half4, _MainTex_ST_Proxy)
		UNITY_DEFINE_INSTANCED_PROP(half4, _TintColor)
		UNITY_DEFINE_INSTANCED_PROP(half, _ColorPow)
		UNITY_INSTANCING_BUFFER_END(MyProperties)

		struct appdata_t {
		float4 vertex : POSITION;
		half4 color : COLOR;
		float4 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		half4 color : COLOR;
		float4 texcoord : TEXCOORD0;
	};

	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		half4 tintColor = UNITY_ACCESS_INSTANCED_PROP(MyProperties, _TintColor);
		half colPow = UNITY_ACCESS_INSTANCED_PROP(MyProperties, _ColorPow);
		half4 MainTex_ST_Proxy = UNITY_ACCESS_INSTANCED_PROP(MyProperties, _MainTex_ST_Proxy);
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.color = v.color * tintColor * 2.0;
		o.color.rgb = o.color.rgb * colPow;
		o.texcoord.xy = (v.texcoord.xy * _MainTex_ST.xy * MainTex_ST_Proxy.xy + (_MainTex_ST.zw + MainTex_ST_Proxy.zw)) + frac(float2(0,_ScrollSpeedY) * _Time.y) + frac(float2(_ScrollSpeedX,0) * _Time.y);
		o.texcoord.z = v.texcoord.z + 1;
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		half4 col = i.color * tex2D(_MainTex, i.texcoord.xy);
		col.rgb = saturate(col.rgb) * i.texcoord.z;
		return col;
	}
		ENDCG
	}
	}
		//FallBack "Mobile/Particles/Alpha Blended"
	}
}
