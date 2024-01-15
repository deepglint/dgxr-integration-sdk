Shader "Custom/QuadWarpURP"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white"
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D_X(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float _Intensity;
            float4x4 _Homography;
            
            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float4 uvq = mul(_Homography, float4(input.texcoord.xy,1,1));
                float4 colorT = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uvq.xy/uvq.z);
                return colorT;
            }
            ENDHLSL
        }
    }
}