﻿Shader "Particles/Standard"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		
		_MetallicGlossMap("Metallic", 2D) = "white" {}
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		_DistortionStrength("Strength", Float) = 1.0
		_DistortionBlend("Blend", Range(0.0, 1.0)) = 0.5

		_SoftParticlesNearFadeDistance("Soft Particles Near Fade", Float) = 1.0
		_SoftParticlesFarFadeDistance("Soft Particles Far Fade", Float) = 2.0
		_CameraNearFadeDistance("Camera Near Fade", Float) = 1.0
		_CameraFarFadeDistance("Camera Far Fade", Float) = 2.0

		// Hidden properties
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _FlipbookMode ("__flipbookmode", Float) = 0.0
		[HideInInspector] _LightingEnabled ("__lightingenabled", Float) = 1.0
		[HideInInspector] _DistortionEnabled ("__distortionenabled", Float) = 0.0
		[HideInInspector] _BlendOp ("__blendop", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
		[HideInInspector] _SoftParticlesEnabled ("__softparticlesenabled", Float) = 0.0
		[HideInInspector] _CameraFadingEnabled ("__camerafadingenabled", Float) = 0.0
		[HideInInspector] _SoftParticleFadeParams ("__softparticlefadeparams", Vector) = (0,0,0,0)
		[HideInInspector] _CameraFadeParams ("__camerafadeparams", Vector) = (0,0,0,0)
	}

	// SM 3.5: Supports all features
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True" "PreviewType"="Plane" }
		
		BlendOp [_BlendOp]	
		Blend [_SrcBlend] [_DstBlend]
		ZWrite [_ZWrite]
		ColorMask RGB

		GrabPass
		{
			Tags { "LightMode" = "Always" }
			"_GrabTexture"
		}

		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
            
			BlendOp Add
			Blend One Zero
			ZWrite On

			CGPROGRAM
			#pragma target 3.5

			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON _ALPHAOVERLAY_ON
			#pragma shader_feature _ _UNLIT _METALLICGLOSSMAP
			#pragma shader_feature _REQUIRE_UV2
			#pragma multi_compile_shadowcaster

			#pragma vertex vertParticleShadowCaster
			#pragma fragment fragParticleShadowCaster

			#include "UnityStandardParticleShadow.cginc"
			ENDCG
		}

		CGPROGRAM
		#pragma surface surf ParticleLighting nolightmap nometa noforwardadd keepalpha vertex:vert
		#pragma multi_compile __ SOFTPARTICLES_ON
		#pragma target 3.5

		#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON _ALPHAOVERLAY_ON
		#pragma shader_feature _ _UNLIT _METALLICGLOSSMAP
		#pragma shader_feature _NORMALMAP
		#pragma shader_feature _EMISSION
		#pragma shader_feature _FADING_ON
		#pragma shader_feature _REQUIRE_UV2
		#pragma shader_feature _DISTORTION_ON

		#include "UnityStandardParticles.cginc"
		ENDCG
	}

	// SM 2.0: Without GrabPass, Fading or Normal Maps
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True" "PreviewType"="Plane" }
		BlendOp [_BlendOp]	
		Blend [_SrcBlend] [_DstBlend]
		ZWrite [_ZWrite]
		ColorMask RGB

		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
            
			BlendOp Add
			Blend One Zero
			ZWrite On

			CGPROGRAM
			#pragma target 2.0

			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON _ALPHAOVERLAY_ON
			#pragma shader_feature _ _UNLIT _METALLICGLOSSMAP
			#pragma shader_feature _REQUIRE_UV2
			#pragma multi_compile_shadowcaster

			#pragma vertex vertParticleShadowCaster
			#pragma fragment fragParticleShadowCaster

			#include "UnityStandardParticleShadow.cginc"

			ENDCG
		}

		CGPROGRAM
		#pragma surface surf ParticleLighting nolightmap nometa noforwardadd keepalpha vertex:vert
		#pragma target 2.0

		#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON _ALPHAOVERLAY_ON
		#pragma shader_feature _ _UNLIT _METALLICGLOSSMAP
		#pragma shader_feature _REQUIRE_UV2
		#pragma shader_feature _EMISSION

		#include "UnityStandardParticles.cginc"
		ENDCG
	}

	Fallback "VertexLit"
	CustomEditor "StandardParticlesShaderGUI"
}
