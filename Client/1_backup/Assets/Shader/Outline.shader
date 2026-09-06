// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Outline2"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Outline("Outline", Range(0.002, 3)) = 0.005
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
			pass {
				Cull front
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				float _Outline;
				uniform float4 _OutlineColor;
				struct v2f {
				float4 pos : SV_POSITION;
				float3 color : COLOR0;
				};
				float4 CreateOutline(float4 vertPos, float Outline)
				{
					
					float4x4 scaleMat;
					scaleMat[0][0] = 1.0f + Outline;
					scaleMat[0][1] = 0.0f;
					scaleMat[0][2] = 0.0f;
					scaleMat[0][3] = 0.0f;
					scaleMat[1][0] = 0.0f;
					scaleMat[1][1] = 1.0f + Outline;
					scaleMat[1][2] = 0.0f;
					scaleMat[1][3] = 0.0f;
					scaleMat[2][0] = 0.0f;
					scaleMat[2][1] = 0.0f;
					scaleMat[2][2] = 1.0f + Outline;
					scaleMat[2][3] = 0.0f;
					scaleMat[3][0] = 0.0f;
					scaleMat[3][1] = 0.0f;
					scaleMat[3][2] = 0.0f;
					scaleMat[3][3] = 1.0f;

					return mul(scaleMat, vertPos);
				}
				v2f vert(appdata_base v) 
				{
					v2f o;
					o.pos = UnityObjectToClipPos(CreateOutline(v.vertex, _Outline));
				
					o.color = _OutlineColor;
					UNITY_TRANSFER_FOG(o, o.pos);
					return o;
				}
				half4 frag(v2f i) :COLOR{
				return half4(_OutlineColor);
				}
				ENDCG
			}
		}
    FallBack "Diffuse"
}
