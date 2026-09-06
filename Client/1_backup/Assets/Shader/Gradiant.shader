Shader "Custom/Gradiant"
{
    Properties
    {
        _Color1 ("Color", Color) = (1,1,1,1)
        _Color2 ("Color", Color) = (1,1,1,1)
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Scale ("Scale", float) = 1

        _StencilComp ("Stencil Comparison", float) = 8
        _Stencil ("Stencil ID", float) = 1
        _StencilOp ("Stencil Operation", float) = 1
        _StencilWriteMask ("Stencil Write Mask", float) = 255
        _StencilReadMask ("Stencil Read Mask", float) = 255
        _ColorMask ("Color Mask", float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Background" "Ignoreprojector"="true" }
        LOD 100

        ZWrite On

        pass
        {
        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Scale;

struct v2f
{
    float4 pos : SV_Position;
    fixed4 col : COLOR;
};
v2f vert(appdata_full v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.col = lerp(_Color1, _Color2, v.texcoord.x);
    return o;
}
float4 frag(v2f i) : COLOR
{
    float4 c = i.col;
    c.a = 1;
    return c;
}
ENDCG
}
}
}
