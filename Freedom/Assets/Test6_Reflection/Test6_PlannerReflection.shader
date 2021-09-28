Shader "Reflection/PlanarReflection"
{
	Properties
	{
		_reflectionColorLerpA("ReflectionColorLerpA",Color) = (0,0,0,0)
		_reflectionColorLerpB("ReflectionColorLerpB",Color) = (0,0,0,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
		//Tags{ "LightMode" = "ForwardBase" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "UnityLightingCommon.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				half3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half4 diffuse : COLOR0;
				float4 screenPos : TEXCOORD0;
				float2 uv : TEXCOORD1;
				half4 ambient : TEXCOORD2;
				SHADOW_COORDS(3)
				//float4 _ShadowCoord : TEXCOORD2;
			};

			sampler2D _ReflectionTex;
			float4 _reflectionColorLerpA;
			float4 _reflectionColorLerpB;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenPos = ComputeScreenPos(o.vertex);
				//o._ShadowCoord = ComputeScreenPos(o.vertex);

				half3 wNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(wNormal, _WorldSpaceLightPos0.xyz));
				o.diffuse = nl * _LightColor0;
				o.ambient = half4(ShadeSH9(half4(wNormal, 1)), 0.0f);


				TRANSFER_SHADOW(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_ReflectionTex, i.screenPos.xy / i.screenPos.w);
				fixed4 finalColor = lerp(_reflectionColorLerpA, _reflectionColorLerpB, col);
				half shadow = SHADOW_ATTENUATION(i);

				half4 diffuse = i.diffuse * shadow + i.ambient;

				//fixed4 shadowColor = lerp(float4(0.8, 0.8, 0.8, 1), float4(1, 1, 1, 1), step(0.5, SHADOW_ATTENUATION(i)));

				return finalColor * diffuse;
			}
			ENDCG
		}

		UsePass "VertexLit/SHADOWCASTER"
		//Pass
		//{
		//	Tags{ "LightMode" = "ShadowCaster" }
		//	CGPROGRAM
		//	#pragma vertex VSMain
		//	#pragma fragment PSMain

		//	float4 VSMain(float4 vertex:POSITION) : SV_POSITION
		//	{
		//		return UnityObjectToClipPos(vertex);
		//	}

		//	float4 PSMain(float4 vertex:SV_POSITION) : SV_TARGET
		//	{
		//		return 0;
		//	}

		//	ENDCG
		//}
	}
}