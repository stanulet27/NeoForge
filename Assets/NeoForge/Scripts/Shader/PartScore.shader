Shader "Universal Render Pipeline/Custom/VertexColorLitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        LOD 200
        Pass
        {
            Tags { "LightMode"="UniversalForward" "RenderPipeline" = "UniversalPipeline" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(appdata v)
            {
                Varyings o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.color = v.color;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 col = i.color;
                return col * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.vertex.xy);
            }
            ENDHLSL
        }
    }
}