Shader "Custom/GPUInstanceDiffuse"
{
	Properties
	{
		//四种贴图
		_Tex1("Stone",2D) = "white"{}
		_Tex2("Water",2D) = "white"{}
		_Tex3("Soil",2D) = "white"{}
		_Tex4("Grass",2D) = "white"{}
		//贴图索引
		_Index1("Index1",float) = 1
		_Index2("Index2",float) = 1
		_Index3("Index3",float) = 1
		_Index4("Index4",float) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
					//gpu instance
			#pragma multi_compile_instancing
					//阴影
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _Tex1;
			sampler2D _Tex2;
			sampler2D _Tex3;
			sampler2D _Tex4;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
					float4 pos : SV_POSITION;
				float3 worldNormal:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
				SHADOW_COORDS(3)
			};

			//不同的属性
			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(float, _Index1)
			UNITY_DEFINE_INSTANCED_PROP(float, _Index2)
			UNITY_DEFINE_INSTANCED_PROP(float, _Index3)
			UNITY_DEFINE_INSTANCED_PROP(float, _Index4)
			UNITY_INSTANCING_BUFFER_END(Props)

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				TRANSFER_SHADOW(o);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float3 worldNormal = i.worldNormal;
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuse = _LightColor0.rgb*saturate(0.5*dot(worldNormal, lightDir) + 0.5);
				float shadow = SHADOW_ATTENUATION(i);
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos.xyz);
				float3 mainTex1,mainTex2,mainTex3,mainTex4 = float3(0,0,0);
				UNITY_SETUP_INSTANCE_ID(i);

				mainTex1 = tex2D(_Tex1, i.uv).xyz;
				mainTex2 = tex2D(_Tex2, i.uv).xyz;
				mainTex3 = tex2D(_Tex3, i.uv).xyz;
				mainTex4 = tex2D(_Tex4, i.uv).xyz;

				//根据传来的0和1选择贴图
				return float4(((mainTex1 * UNITY_ACCESS_INSTANCED_PROP(Props, _Index1) +
					mainTex2 * UNITY_ACCESS_INSTANCED_PROP(Props, _Index2) +
					mainTex3 * UNITY_ACCESS_INSTANCED_PROP(Props, _Index3) +
					mainTex4 * UNITY_ACCESS_INSTANCED_PROP(Props, _Index4)
					)*diffuse + ambient)*shadow,1);
			}
			ENDCG
		}
	}

	fallback "Diffuse"
}

