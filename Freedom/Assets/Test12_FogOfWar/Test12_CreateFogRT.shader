Shader "Unlit/Test12_CreateFogRT"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Texture", 2D) = "white" {}
	_ViewBounds("View Bounds", Vector) = (0,0,0,0)
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		ZTest Off
		ZWrite Off Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_instancing

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 wpos : TEXCOORD1;
	};

	UNITY_INSTANCING_BUFFER_START(MyProperties)
		UNITY_DEFINE_INSTANCED_PROP(float4, _Trans)
		UNITY_INSTANCING_BUFFER_END(MyProperties)

		sampler2D _MainTex;
	float4 _FowSize;
	float4 _ViewBounds;

	float3 CustomTransformToWorldScale(float4 vertex, float4 trans, float2 scale) {
		float3 wpos = (float3)0;
		vertex.xz *= scale;
		float s, c;
		sincos(trans.z * 0.02463994238109642, s, c);
		wpos.x = vertex.x * c + vertex.z * s;
		wpos.z = -vertex.x * s + vertex.z * c;
		wpos.xz += trans.yx;
		return wpos;
	}

	v2f vert(appdata v)
	{
		UNITY_SETUP_INSTANCE_ID(v)
			v2f o;
		float4 trans = UNITY_ACCESS_INSTANCED_PROP(MyProperties, _Trans);
		float3 wp = CustomTransformToWorldScale(v.vertex, trans, 4);
		//o.wpos = wp;
		//wp.xz = (wp.xz - _ViewBounds.xy + 0.5) * _ViewBounds.zw;
		//wp.xz = (wp.xz + 0.5) * float2(0.1, 0.1);
		wp.xz = (wp.xz + _ViewBounds.yx + 0.5) * _ViewBounds.wz;
		float2 p = wp.xz * 2.0 - 1.0;
		p.y = -p.y;
		o.vertex = float4(p, 0.001, 1.0);
		o.uv = v.uv;
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		half4 col = tex2D(_MainTex, saturate(i.uv));
		col.a = col.r;
		col.rgb = 1.0f;
		return col;
	}
		ENDCG
	}
	}
}
