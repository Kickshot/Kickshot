Shader "Effects/TornadoTest" {
	Properties {
		_MainTex ("MainTex", 2D) = "black" {}
		_U_Speed ("U_Speed", Float) = 0.0
		_V_Speed ("V_Speed", Float) = 0.0
		_Color ("Color", Color) = (1,1,1,1)
		_NumberOfWaves ("Waves", FLoat) = 0.0
		_SizeOfWaves ("Wave Size", Float) = 0.0
		_DirectionalWaveSpeed ("WaveDirectionSpeed", Float) = 0.0
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.0
		_Twist("Reel vector", Vector) = (1, 0.1, 5, 0)
	}
	SubShader {
		Tags {
			"RenderType"="Opaque" 
		}
	    Pass {
		    Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
		
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#define UNITY_PASS_FORWARDBASE
			#pragma target 3.0
			#pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
			uniform float4 _TimeEditor;
			uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			uniform float _U_Speed;
			uniform float _V_Speed;
			uniform float _NumberOfWaves
			uniform float _SizeOfWaves;
			uniform float _DirectionalWaveSpeed;
			half _Cutoff;
			float4 _Twist;
			#include "UnityCG.cginc"

			struct VertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 textcoord0 : TEXCOORD0;
			};

			struct VertexOutput {
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					float4 posWorld : TEXCOORD1;
					float3 normalDir : TEXCOORD2;
					float height : TEXCOORD3;
					UNITY_FOG_COORDS(3)
			};

			VertexOutput vert (VertexInput vInput) {
				VertexOutput vOutput = v.textcoord0;
				vOutput.uv0 = vInput.textcoord0;
				vOutput.normalDir = UnityObjectToWorldNormal(vInput.normal);
				float4 time = _Time + _TimeEditor;
				float h = vOutput.uv0.g;
				vInput.vertex.xyz += ((sin(_NumberOfWaves*(mul(unity_ObjectToWorld, vInput.vertex).g.r+(time.g*_DirectionalWaveSpeed))*3.14))*h.r)*vInput.normal*_SizeOfWaves);
				vOutput.posWorld = mul(unity_ObjectToWorld, vInput.vertex);
				float3 worldpos = mul(unity_ObjectToWorld, vInput.vertex).xyz;
				float4 pivot = mul(unity_ObjectToWorld, float4(0,0,0,1));
				float height = (worldpos.y - pivot.y + _Twist.w) * _Twist.y;
				vInput.vertex.x += sin(_Time.y*_Twist.z + worldpos.y * _Twist.x) * height;
				vInput.vertex.z += sin(_Time.y*_Twist.z + worldpos.y * _Twist.x + 3.1415/2) * height;
				vOutput.height = height;
				vOutput.pos = UnityObjectToClipPos(vInput.vertex);
				return vOutput;
			}
			float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 node_4620 = _Time + _TimeEditor;
                float2 node_5411 = ((i.uv0+(node_4620.g*_U_Speed)*float2(1,0))+(node_4620.g*_V_Speed)*float2(0,1));
                float4 node_9700 = tex2D(_MainTex,TRANSFORM_TEX(node_5411, _MainTex));
                float3 node_4951 = (_TintColor.rgb*node_9700.rgb*_EmmisionStrench);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
				clip(node_9700.r - _Cutoff);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
