Shader "Effect/JumpPadDistortion"
{
	Properties
	{
		_DispTex ("Displacement Texture", 2D) = "white" {}
        [NoScaleOffset]
        _FalloffTex ("Falloff Texture", 2D) = "white" {}
        _Magnitude ("Magnitude", Range(0,5)) = 1.0
        _Speed ("Scroll Speed", Range(0,50)) = 0.0
	}
	SubShader
	{
        ZWrite Off
        ZTest Always

        Tags 
        {
            "Queue" = "Transparent"
        }

        GrabPass {}
       
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
                float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
                float2 uv : TEXCOORD0;
				float4 grabPos : TEXCOORD1;
				float4 pos : SV_POSITION;
                float3 norm : TEXCOORD2;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.pos);
                o.uv = v.uv;
                o.norm = v.normal;
				return o;
			}
			
			sampler2D _DispTex;
            sampler2D _FalloffTex;

            float _Magnitude;
            sampler2D _GrabTexture;
            float _Speed;

			fixed4 frag (v2f i) : SV_Target
			{
                float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
                float viewMag = pow(dot(i.norm, viewDir),2);

                float2 scrollUV = i.uv;
                scrollUV.y -= _Time * _Speed;
                float2 disp = tex2D(_DispTex, scrollUV).xy * _Magnitude * viewMag * tex2D(_FalloffTex, i.uv).x;

                float4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(disp, 0,0) + i.grabPos));
				return col;
			}
			ENDCG
		}
	}
}
