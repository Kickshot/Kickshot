Shader "Transparent/CartoonWaterShader" {

 Properties {
     _Color ("Main Color", Color) = (1,1,1,1)
     _MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
     _MainTex2 ("Base (RGB) TransGloss (A)", 2D) = "white" {}
 }
 SubShader {
     Tags {"Queue"="Transparent-150" "IgnoreProjector"="True" "RenderType"="Transparent"}
     LOD 300
     Cull Off
     CGPROGRAM
     
         #pragma surface surf NoLighting alpha vertex:vert
         sampler2D _MainTex;
                  sampler2D _MainTex2;
         float4 _Color;

         struct Input 
         {
             float2 uv_MainTex;
             float2 uv_MainTex2;
         };

         fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	     {
	         fixed4 c;
	         c.rgb = s.Albedo;
	         c.a = s.Alpha;
	         return c;
	     }
         void vert (inout appdata_full v){

         }
         void surf (Input IN, inout SurfaceOutput o) {
             half4 tex = tex2D(_MainTex, IN.uv_MainTex);
             half4 tex2 = tex2D(_MainTex2, IN.uv_MainTex2);
             o.Albedo = tex.rgb + tex2.rgb *_Color.rgb;
             o.Gloss = tex.a+tex2.a;
             o.Alpha = (tex.a+tex2.a) * _Color.a;
  
         }
         
     ENDCG
 }
}