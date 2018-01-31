#ifndef UNITY_STANDARD_PARTICLES_INCLUDED
#define UNITY_STANDARD_PARTICLES_INCLUDED

#include "UnityPBSLighting.cginc"

#if _REQUIRE_UV2
#define _FLIPBOOK_BLENDING 1
#endif

struct appdata_particles
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 color : COLOR;
	#if defined(_FLIPBOOK_BLENDING)
	float4 texcoords : TEXCOORD0;
	float texcoordBlend : TEXCOORD1;
	#else
	float2 texcoords : TEXCOORD0;
	#endif
	#if defined(_NORMALMAP)
	float4 tangent : TANGENT;
	#endif
};
	  
struct Input
{
	float4 color : COLOR;
	float2 texcoord;
	#if defined(_FLIPBOOK_BLENDING)
	float3 texcoord2AndBlend;
	#endif
	#if defined(SOFTPARTICLES_ON) || defined(_FADING_ON)
	float4 projectedPosition;
	#endif
	#if _DISTORTION_ON
	float4 grabPassPosition;
	#endif
};

fixed4 readTexture(sampler2D tex, Input IN)
{
	fixed4 color = tex2D (tex, IN.texcoord);
	#ifdef _FLIPBOOK_BLENDING
	fixed4 color2 = tex2D(tex, IN.texcoord2AndBlend.xy);
	color = lerp(color, color2, IN.texcoord2AndBlend.z);
	#endif
	return color;
}

sampler2D _MainTex;
float4 _MainTex_ST;
half4 _Color;
sampler2D _BumpMap;
half _BumpScale;
sampler2D _EmissionMap;
half3 _EmissionColor;
sampler2D _MetallicGlossMap;
half _Metallic;
half _Glossiness;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
float4 _SoftParticleFadeParams;
float4 _CameraFadeParams;
half _Cutoff;

#define SOFT_PARTICLE_NEAR_FADE _SoftParticleFadeParams.x
#define SOFT_PARTICLE_INV_FADE_DISTANCE _SoftParticleFadeParams.y

#define CAMERA_NEAR_FADE _CameraFadeParams.x
#define CAMERA_INV_FADE_DISTANCE _CameraFadeParams.y

#if _DISTORTION_ON
sampler2D _GrabTexture;
half _DistortionStrength;
half _DistortionBlend;
#endif

void vert (inout appdata_particles v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
			
	#if defined(_FLIPBOOK_BLENDING)
	o.texcoord = v.texcoords.xy;
	o.texcoord2AndBlend.xy = v.texcoords.zw;
	o.texcoord2AndBlend.z = v.texcoordBlend;
	#else
	o.texcoord = TRANSFORM_TEX(v.texcoords.xy, _MainTex);
	#endif

	float4 clipPosition = UnityObjectToClipPos(v.vertex);

	#if defined(SOFTPARTICLES_ON) || defined(_FADING_ON)
	o.projectedPosition = ComputeScreenPos (clipPosition);
	COMPUTE_EYEDEPTH(o.projectedPosition.z);
	#endif

	#if _DISTORTION_ON
	o.grabPassPosition = ComputeGrabScreenPos (clipPosition);
	#endif
}
      
void surf (Input IN, inout SurfaceOutputStandard o)
{
	half4 albedo = readTexture (_MainTex, IN);
	albedo *= _Color;

	#if defined(_ALPHAOVERLAY_ON)
	albedo.rgb = lerp(1 - 2 * (1 - albedo.rgb) * (1 - IN.color.rgb), 2 * albedo.rgb * IN.color.rgb, step(albedo.rgb, 0.5));
    albedo.a *= IN.color.a;
	#else
	albedo *= IN.color;
	#endif

	#if defined(SOFTPARTICLES_ON) && defined(_FADING_ON)
	if (SOFT_PARTICLE_NEAR_FADE > 0.0 || SOFT_PARTICLE_INV_FADE_DISTANCE > 0.0)
	{
		float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.projectedPosition)));
		float fade = saturate (SOFT_PARTICLE_INV_FADE_DISTANCE * ((sceneZ - SOFT_PARTICLE_NEAR_FADE) - IN.projectedPosition.z));
		#if defined(_ALPHAPREMULTIPLY_ON)
		albedo *= fade;
		#else
		albedo.a *= (fade * fade);
		#endif
	}
	#endif

	#if defined(_FADING_ON)
	float cameraFade = saturate((IN.projectedPosition.z - CAMERA_NEAR_FADE) * CAMERA_INV_FADE_DISTANCE);
	#if defined(_ALPHAPREMULTIPLY_ON)
	albedo *= cameraFade;
	#else
	albedo.a *= cameraFade;
	#endif
	#endif
			
	#if defined(_METALLICGLOSSMAP)
	fixed2 metallicGloss = readTexture (_MetallicGlossMap, IN).ra * fixed2(1.0, _Glossiness);
	#else
	fixed2 metallicGloss = fixed2(_Metallic, _Glossiness);
	#endif

	#if defined(_NORMALMAP)
	float3 normal = normalize (UnpackScaleNormal (readTexture (_BumpMap, IN), _BumpScale));
	#else
	float3 normal = float3(0,0,1);
	#endif

	#if defined(_EMISSION)
	half3 emission = readTexture (_EmissionMap, IN).rgb;
	#else
	half3 emission = 0;
	#endif

	#if _DISTORTION_ON
	float4 grabPosUV = UNITY_PROJ_COORD(IN.grabPassPosition);
	grabPosUV.xy += normal.xy * _DistortionStrength * albedo.a;
	half3 grabPass = tex2Dproj(_GrabTexture, grabPosUV).rgb;
	albedo.rgb = lerp(grabPass, albedo.rgb, saturate(albedo.a - _DistortionBlend));
	#endif

	o.Albedo = albedo.rgb;
	#if defined(_NORMALMAP)
	o.Normal = normal;
	#endif
	o.Emission = emission * _EmissionColor;
	o.Metallic = metallicGloss.r;
	o.Smoothness = metallicGloss.g;

	#if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON) || defined(_ALPHAOVERLAY_ON)
	o.Alpha = albedo.a;
	#else
	o.Alpha = 1;
	#endif

	#if defined(_ALPHAMODULATE_ON)
	o.Albedo = lerp(1.0, albedo.rgb, albedo.a);
	#endif

	#if defined(_ALPHATEST_ON)
	clip (albedo.a - _Cutoff);
	#endif
}

inline half4 LightingParticleLighting(SurfaceOutputStandard s, half3 viewDir, UnityGI gi)
{
#if defined(_UNLIT)
	return half4(s.Albedo, s.Alpha);
#else
	return LightingStandard(s, viewDir, gi);
#endif
}

inline void LightingParticleLighting_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
{
#if !defined(_UNLIT)
	LightingStandard_GI(s, data, gi);
#endif
}

#endif // UNITY_STANDARD_PARTICLES_INCLUDED
