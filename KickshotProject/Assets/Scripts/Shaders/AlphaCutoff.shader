Shader "Custom/AlphaCutoff" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
        _AltTex ("Alternate Texture", 2D) = "white" {}
        _CutoffTex ("Cutoff Texture", 2D) = "white" {}
        _Cutoff ("Cutoff Range", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
        sampler2D _AltTex;
        sampler2D _CutoffTex;

		struct Input {
			float2 uv_MainTex;
            float2 uv_AltTex;
		};
        
		float _Cutoff;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 m = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 a = tex2D (_AltTex, IN.uv_AltTex);
            fixed c = lerp(1, tex2D (_CutoffTex, IN.uv_MainTex).x, _Cutoff);

            fixed4 ret = m * (c) + a * (1 - c);

			o.Albedo = ret.rgb;
			o.Alpha = ret.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
