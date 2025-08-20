Shader "Curved/WorldCurver_Builtin"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _BendXZ("Bend Strength (x=X^2, y=Z^2)", Vector) = (0.0, 0.00015, 0, 0)
        _MaxHeight("Clamp Height", Float) = 100.0
        _PivotWS("Pivot (world XZ)", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "DisableBatching"="True" }
        LOD 100
        Cull Back
        ZWrite On
        CGPROGRAM
        #pragma surface surf Unlit addshadow vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _BendXZ;
        float _MaxHeight;
        float4 _PivotWS;

        struct Input { float2 uv_MainTex; };

        void Bend(inout float4 pos)
        {
            float3 world = mul(unity_ObjectToWorld, pos).xyz;
            float2 d = world.xz - _PivotWS.xz;
            float yOff = _BendXZ.x * d.x * d.x + _BendXZ.y * d.y * d.y;
            yOff = min(yOff, _MaxHeight);
            world.y += yOff;
            pos = mul(unity_WorldToObject, float4(world,1));
        }

        void vert(inout appdata_full v)
        {
            Bend(v.vertex);
        }

        void surf(Input IN, inout UnityIndirect o) { }
        fixed4 LightingUnlit(UnityIndirect s, fixed3 lightDir, fixed atten) { return 1; }
        ENDCG
    }
    Fallback "Diffuse"
}
