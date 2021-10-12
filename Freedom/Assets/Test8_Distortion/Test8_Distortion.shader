Shader "Unlit/Test8_Distortion"
{
	Properties{
		_Main_TEX("Main_TEX", 2D) = "white" {}
		_Power("Power", Float) = 1
	}

	SubShader{
		Tags{
			"IgnoreProjector" = "True"
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}

		GrabPass{ "_SceneTexture" }
		Pass{
			Name "FORWARD"
			Tags{
				"LightMode" = "ForwardBase"
			}
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#pragma multi_compile_fwdbase
			#pragma target 3.0

			uniform sampler2D _SceneTexture;
			uniform sampler2D _Main_TEX; uniform float4 _Main_TEX_ST;
			uniform float _Power;

			//fixed4 SampleScreenDynamicOff(sampler2D screen, half4 grabPos, half2 offset) {
			//	half2 uv = grabPos.xy / grabPos.w + offset;
			//	return tex2D(screen, uv);
			//}

			struct VertexInput {
				float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;
				half4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				half4 vertexColor : COLOR;
				float4 projPos : TEXCOORD1;
			};

			VertexOutput vert(VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				o.vertexColor = v.vertexColor;
				o.pos = UnityObjectToClipPos(v.vertex);
				//Unity在情况下确实不会翻转 RenderTexture，包括GrabPass
				o.projPos = ComputeGrabScreenPos(o.pos);
				COMPUTE_EYEDEPTH(o.projPos.z);
				return o;
			}
			half4 frag(VertexOutput i) : COLOR{
				half4 _Main_TEX_var = tex2D(_Main_TEX,TRANSFORM_TEX(i.uv0, _Main_TEX));
				//half2 offset = (_Main_TEX_var.rgb*_Power*_Main_TEX_var.a*i.vertexColor.a).rg;
				half2 offset = _Main_TEX_var.rr*_Power*_Main_TEX_var.a;//*i.vertexColor.a;

				//half4 sceneColor = SampleScreenDynamicOff(_SceneTexture, i.projPos, offset);
				half2 uv = i.projPos.xy / i.projPos.w + offset;
				half4 sceneColor = tex2D(_SceneTexture, uv);

				////// Lighting:
				half4 finalRGBA = half4(sceneColor.rgb, 1);
				return finalRGBA;
			}

			ENDCG
		}
	}

	FallBack "Diffuse"
}
