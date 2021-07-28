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

		// 可以使用Cull指令来控制需要剔除n哪个面的渲染图元
		// Cull Back | Front | Off
		// 若设置成Back，那么背对摄像机的面就不会被渲染
		// 若设置成Front，那么朝向摄像机的面就不会被渲染
		// 若设置成Off，就会关闭剔除功能，但是需要渲染的图元数目会成倍的增加
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
