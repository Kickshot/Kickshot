Shader "Effects/WaterfallFoam" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Alpha Cutoff", 2D) = "white" {}
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.0
        _USpeed ("U Speed", Range(0,20)) = 0.0
        _VSpeed ("V Speed", Range(0,20)) = 0.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Standard alpha:fade
		#pragma target 3.0

        sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        half _USpeed;
        half _VSpeed;
        float _AlphaCutoff;

		void surf (Input IN, inout SurfaceOutputStandard o) {
            float2 dispUV = IN.uv_MainTex;
            dispUV.x += _Time.r * _USpeed;
            dispUV.y += _Time.r * _VSpeed;
			float4 c = tex2D (_MainTex, dispUV);
            half ret = clamp((c.r - _AlphaCutoff) * 100.0, 0, 1);
			o.Albedo = _Color.rgb;
			o.Alpha = ret;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
