Shader "Custom/PaintShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tiling ("Tiling", Float) = 1.0
        _EmissionStrength ("Emission Strength", Float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 150

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        float _Tiling;
        float _EmissionStrength;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float3 coords = IN.worldPos * _Tiling;
            float3 n = abs(IN.worldNormal);
            float4 texColor;

            if (n.x > n.y && n.x > n.z)
                texColor = tex2D(_MainTex, coords.zy); // X projection
            else if (n.y > n.z)
                texColor = tex2D(_MainTex, coords.xz); // Y projection
            else
                texColor = tex2D(_MainTex, coords.xy); // Z projection

            o.Albedo = texColor.rgb;

            // Add slight emission for low-light visibility
            o.Emission = texColor.rgb * _EmissionStrength;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
