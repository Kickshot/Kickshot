Shader "Custom/SimpleEmissiveCutoff" {
	Properties {
        [HDR]
        _MainColor ("Main Color", Color) = (1,1,1,1)
        [HDR]
        _AltColor ("Alternate Color", Color) = (1,1,1,1)
        _CutoffTex ("Cutoff Texture", 2D) = "white" {}
        _Cutoff ("Cutoff Range", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

        sampler2D _CutoffTex;
       
		struct Input {
            float2 uv_CutoffTex;
		};

        fixed4 _MainColor;
        fixed4 _AltColor;
		float _Cutoff;

		void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed c = floor(tex2D (_CutoffTex, IN.uv_CutoffTex).x + _Cutoff);
            fixed4 ret = _AltColor * (c) + _MainColor * (1 - c);

            o.Emission = ret.rgb;
			o.Albedo = ret.rgb;
			o.Alpha = ret.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
