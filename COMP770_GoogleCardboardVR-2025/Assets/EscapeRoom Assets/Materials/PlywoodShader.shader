Shader "Custom/PlywoodShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tiling ("Tiling", Float) = 1.0
        _BlendSharpness ("Blend Sharpness", Float) = 4.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        float _Tiling;
        float _BlendSharpness;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        float3 TriplanarBlendWeights(float3 normal, float sharpness)
        {
            float3 absNormal = abs(normal);
            float3 weights = pow(absNormal, sharpness);
            return weights / (weights.x + weights.y + weights.z);
        }

        float4 TriplanarTexture(float3 worldPos, float3 normal, float tiling)
        {
            float3 coords = worldPos * tiling;

            float3 weights = TriplanarBlendWeights(normal, _BlendSharpness);

            float2 xUV = coords.zy;
            float2 yUV = coords.xz;
            float2 zUV = coords.xy;

            float4 xTex = tex2D(_MainTex, xUV);
            float4 yTex = tex2D(_MainTex, yUV);
            float4 zTex = tex2D(_MainTex, zUV);

            return xTex * weights.x + yTex * weights.y + zTex * weights.z;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 texColor = TriplanarTexture(IN.worldPos, IN.worldNormal, _Tiling);
            o.Albedo = texColor.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}