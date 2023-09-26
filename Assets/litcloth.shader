Shader "Custom/LitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                 uint vertexId : SV_VertexID;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
            };

            sampler2D _MainTex;
            StructuredBuffer<float3> PointBuffer;
            v2f vert (appdata_t v)
            {
                v2f o;
                v.vertex.xyz = PointBuffer[v.vertexId];
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = normalize(UnityObjectToWorldNormal(v.normal));
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.uv);
                half3 lighting = max(dot(normalize(i.normal), float3(0, 1, 0)), 0); // Simple Lambertian lighting
                return texColor * half4(lighting,1);
            }
            ENDCG
        }
    }
}
