Shader "Unlit/CurvedUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CurveStrength("Curve Strength", Float) = 0.002
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "CurvedCode.cginc"
            ENDCG
        }
    }
}
