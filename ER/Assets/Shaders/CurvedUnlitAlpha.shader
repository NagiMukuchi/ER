Shader "Unlit/CurvedUnlitAlpha"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CurveStrength("Curve Strength", Float) = 0.002
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB

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
