// This shader visuzlizes the normal vector values on the mesh.
Shader "Example/URPUnlitShaderNormal"
{    
    Properties
    { }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {            
            HLSLPROGRAM            
            #pragma vertex vert            
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"       
            struct particle
{
    float3 position;
    float3 velocity;
};     
            StructuredBuffer<particle> PointBuffer;
            StructuredBuffer<float4> ColorBuffer;
            struct Attributes
            {
                float4 positionOS   : POSITION;
                // Declaring the variable containing the normal vector for each
                // vertex.
                half3 normal        : NORMAL;
                uint vertexId : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                half3 normal        : TEXCOORD0;
                float3 color        : TEXCOORD1;
            };                                   

            Varyings vert(Attributes IN)
            {                
                Varyings OUT;
                IN.positionOS = float4(PointBuffer[IN.vertexId].position,1);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);       
                // Use the TransformObjectToWorldNormal function to transform the
                // normals from object to world space. This function is from the 
                // SpaceTransforms.hlsl file, which is referenced in Core.hlsl.
                OUT.normal = TransformObjectToWorldNormal(IN.normal);      
                OUT.color = ColorBuffer[IN.vertexId].xyz;          
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {                
                half4 color = 0;
                // IN.normal is a 3D vector. Each vector component has the range
                // -1..1. To show all vector elements as color, including the
                // negative values, compress each value into the range 0..1.
                color.rgb = IN.color;         
                return color;
            }
            ENDHLSL
        }
    }
}