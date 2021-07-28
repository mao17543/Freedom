Shader "Unlit/Test3_Discard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		// ����ʹ��Cullָ����������Ҫ�޳�n�ĸ������ȾͼԪ
		// Cull Back | Front | Off
		// �����ó�Back����ô�������������Ͳ��ᱻ��Ⱦ
		// �����ó�Front����ô�������������Ͳ��ᱻ��Ⱦ
		// �����ó�Off���ͻ�ر��޳����ܣ�������Ҫ��Ⱦ��ͼԪ��Ŀ��ɱ�������
		Cull front

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD1;
				float3 normal : TEXCOORD2;
			};

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.normal = normalize(UnityObjectToWorldNormal(v.normal));

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed diffuse = saturate(dot(normalize(UnityWorldSpaceLightDir(i.worldPos.xyz)), i.normal));
                // sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * diffuse;
				// apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

				clip(-i.worldPos.y);

                return col;
            }
            ENDCG
        }
    }
}
