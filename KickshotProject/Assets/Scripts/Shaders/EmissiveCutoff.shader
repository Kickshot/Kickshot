Shader "Custom/EmissiveCutoff" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
        [HDR]
        _MainEmission ("Main Emission", 2D) = "black" {}
        _MainEmInt ("Main Emissive Intensity", Range(0,10)) = 0.0
        _AltTex ("Alternate Texture", 2D) = "white" {}
        [HDR]
        _AltEmission ("Alternate Emission", 2D) = "black" {}
        _AltEmInt ("Alternate Emisive Intensity", Range(0,10)) = 0.0
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
        sampler2D _MainEmission;
        sampler2D _AltTex;
        sampler2D _AltEmission;
        sampler2D _CutoffTex;

       
		struct Input {
			float2 uv_MainTex;
            float2 uv_AltTex;
            float2 uv_CutoffTex;
		};
        
		float _Cutoff;
        float _MainEmInt;
        float _AltEmInt;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 m = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 a = tex2D (_AltTex, IN.uv_AltTex);

            fixed4 emM = tex2D(_MainEmission, IN.uv_MainTex);
            fixed4 emA = tex2D(_AltEmission, IN.uv_AltTex);

            fixed c = clamp(tex2D (_CutoffTex, IN.uv_CutoffTex).x + _Cutoff * 2 - 1, 0, 1);
            fixed4 ret = a * (c) + m * (1 - c);
            fixed4 emRet = emA * (c) * _AltEmInt * emA.a + emM * (1-c) * _MainEmInt * emM.a;
         
            o.Emission = emRet.rgb;
			o.Albedo = ret.rgb;
			o.Alpha = ret.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
