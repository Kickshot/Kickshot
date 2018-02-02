Shader "Effects/TornadoShell" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
        _USpeed ("U Speed", Range(0,20)) = 0.0
        _VSpeed ("V Speed", Range(0,20)) = 0.0
        _UStretch ("U Stretch", Range(1,20)) = 1.0
        _VStretch ("V Stretch", Range(1,20)) = 1.0
        _NoiseOctave ("Noise Octave", Range(1,50)) = 1.0
        _NoiseCutoff ("Noise Cutoff", Range(0,1)) = 0.0
        _DispRad ("Dispersion Radius", Range(0,20)) = 0.0
        _DispLoc ("Dispersion Location", Vector) = (0,0,0)
	}
	SubShader {
        Pass {
    		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
    		LOD 200

            ZWrite On
            ZTest On
            Blend SrcAlpha OneMinusSrcAlpha

    		CGPROGRAM
    		#pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "ClassicNoise2D.hlsl"
                  
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

    		half _USpeed;
            half _VSpeed;
            half _UStretch;
            half _VStretch;
            float _NoiseCutoff;
    		fixed4 _Color;
            half _NoiseOctave;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_TARGET {
                float2 dispUV = i.uv;
                dispUV.x /= _UStretch;
                dispUV.y /= _VStretch;
                dispUV.x += _Time * _USpeed;
                dispUV.y += _Time * _VSpeed;
                fixed4 col = cnoise(dispUV * _NoiseOctave);//tex2D(_NoiseTex, dispUV);
                clip(col.r - _NoiseCutoff);
                col = _Color;
                col.a = 1.0;
                return col;
            }
    		ENDCG
        }
	}
	FallBack "Diffuse"
}
