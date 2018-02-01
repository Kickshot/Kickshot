Shader "Effects/Waterfall" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _AlphaTex ("Alpha Cutoff", 2D) = "white" {}
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

        sampler2D _AlphaTex;
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        half _USpeed;
        half _VSpeed;

		void surf (Input IN, inout SurfaceOutputStandard o) {
            float2 dispUV = IN.uv_MainTex;
            dispUV.x += _Time * _USpeed;
            dispUV.y += _Time * _VSpeed;
			fixed4 c = tex2D (_MainTex, dispUV) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = tex2D(_AlphaTex, dispUV).r;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
