Shader "Custom/Standard_WithGlow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [ShowAsVector2] _MyVector ("Some Vector", Vector) = (0,0,0,0)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _BlinkStrength ("Strength", Range(0,2)) = 0.5
        _BlinkColor ("Blink Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType" = "TransparentCutout" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alphatest:_Cutoff BlinnPhong alpha vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #define TRANSFORM_TEX(tex,name) (tex.xy * name.xy + name.zw)
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed _BlinkStrength;
        fixed4 _Color;
        fixed4 _BlinkColor;
        float4 _MyVector;

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
            TRANSFORM_TEX(_MyVector,_MainTex);
            fixed4 baseC = c;
            c.rgb = lerp(baseC.rgb,_BlinkColor.xyz,_BlinkStrength);
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
