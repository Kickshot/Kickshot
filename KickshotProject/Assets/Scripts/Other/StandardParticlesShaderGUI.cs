#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
internal class StandardParticlesShaderGUI : ShaderGUI
{
	public enum BlendMode
	{
		Opaque,
		Cutout,
		Fade,		// Old school alpha-blending mode, fresnel does not affect amount of transparency
		Transparent, // Physically plausible transparency mode, implemented as alpha pre-multiply
		Additive,
		Subtractive,
		Modulate,
		Overlay,
		AdditiveOverlay
	}

	public enum FlipbookMode
	{
		Simple,
		Blended
	}

	private static class Styles
	{
		public static GUIContent albedoText = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A).");
		public static GUIContent alphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff.");
		public static GUIContent metallicMapText = new GUIContent("Metallic", "Metallic (R) and Smoothness (A).");
		public static GUIContent smoothnessText = new GUIContent("Smoothness", "Smoothness value.");
		public static GUIContent smoothnessScaleText = new GUIContent("Smoothness", "Smoothness scale factor.");
		public static GUIContent normalMapText = new GUIContent("Normal Map", "Normal Map.");
		public static GUIContent emissionText = new GUIContent("Color", "Emission (RGB).");

		public static GUIContent renderingMode = new GUIContent("Rendering Mode", "Determines the transparency and blending method for drawing the object to the screen.");
		public static GUIContent[] blendNames = Array.ConvertAll(Enum.GetNames(typeof(BlendMode)), item => new GUIContent(item));

		public static GUIContent flipbookMode = new GUIContent("Flip-Book Mode", "Determine the blending mode used for animated texture sheets.");
		public static GUIContent[] flipbookNames = Array.ConvertAll(Enum.GetNames(typeof(FlipbookMode)), item => new GUIContent(item));

		public static GUIContent lightingEnabled = new GUIContent("Enable Lighting", "Apply the current lighting setup to the particles.");

		public static GUIContent distortionEnabled = new GUIContent("Enable Distortion", "Use a grab pass and normal map to simulate refraction.");
		public static GUIContent distortionStrengthText = new GUIContent("Strength", "Distortion Strength.");
		public static GUIContent distortionBlendText = new GUIContent("Blend", "Weighting between albedo and grab pass.");

		public static GUIContent softParticlesEnabled = new GUIContent("Enable Soft Particles", "Fade out particle geometry when it gets close to the surface of objects written into the depth buffer.");
		public static GUIContent softParticlesNearFadeDistanceText = new GUIContent("Near fade", "Soft Particles near fade distance.");
		public static GUIContent softParticlesFarFadeDistanceText = new GUIContent("Far fade", "Soft Particles far fade distance.");

		public static GUIContent cameraFadingEnabled = new GUIContent("Enable Camera Fading", "Fade out particle geometry when it gets close to the camera.");
		public static GUIContent cameraNearFadeDistanceText = new GUIContent("Near fade", "Camera near fade distance.");
		public static GUIContent cameraFarFadeDistanceText = new GUIContent("Far fade", "Camera far fade distance.");

		public static string mainOptionsText = "Main Options";
		public static string mapsOptionsText = "Maps";
		public static string advancedOptionsText = "Advanced Options";
		public static string requiredVertexStreamsText = "Required Vertex Streams";

		public static string streamPositionText = "Position (POSITION.xyz)";
		public static string streamNormalText = "Normal (NORMAL.xyz)";
		public static string streamColorText = "Color (COLOR.xyzw)";
		public static string streamUVText = "UV (TEXCOORD0.xy)";
		public static string streamUV2Text = "UV2 (TEXCOORD0.zw)";
		public static string streamAnimBlendText = "AnimBlend (TEXCOORD1.x)";
		public static string streamTangentText = "Tangent (TANGENT.xyzw)";
		
		public static GUIContent streamApplyToAllSystemsText = new GUIContent("Apply to Systems", "Apply the vertex stream layout to all Particle Systems using this material");
	}

	MaterialProperty blendMode = null;
	MaterialProperty flipbookMode = null;
	MaterialProperty lightingEnabled = null;
	MaterialProperty distortionEnabled = null;
	MaterialProperty distortionStrength = null;
	MaterialProperty distortionBlend = null;
	MaterialProperty albedoMap = null;
	MaterialProperty albedoColor = null;
	MaterialProperty alphaCutoff = null;
	MaterialProperty metallicMap = null;
	MaterialProperty metallic = null;
	MaterialProperty smoothness = null;
	MaterialProperty bumpScale = null;
	MaterialProperty bumpMap = null;
	MaterialProperty emissionColorForRendering = null;
	MaterialProperty emissionMap = null;
	MaterialProperty softParticlesEnabled = null;
	MaterialProperty cameraFadingEnabled = null;
	MaterialProperty softParticlesNearFadeDistance = null;
	MaterialProperty softParticlesFarFadeDistance = null;
	MaterialProperty cameraNearFadeDistance = null;
	MaterialProperty cameraFarFadeDistance = null;

	MaterialEditor m_MaterialEditor;
	ColorPickerHDRConfig m_ColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 1/99f, 3f);

	bool m_FirstTimeApply = true;

	public void FindProperties (MaterialProperty[] props)
	{
		blendMode = FindProperty("_Mode", props);
		flipbookMode = FindProperty("_FlipbookMode", props);
		lightingEnabled = FindProperty("_LightingEnabled", props);
		distortionEnabled = FindProperty("_DistortionEnabled", props);
		distortionStrength = FindProperty("_DistortionStrength", props);
		distortionBlend = FindProperty("_DistortionBlend", props);
		albedoMap = FindProperty("_MainTex", props);
		albedoColor = FindProperty("_Color", props);
		alphaCutoff = FindProperty ("_Cutoff", props);
		metallicMap = FindProperty("_MetallicGlossMap", props);
		metallic = FindProperty("_Metallic", props);
		smoothness = FindProperty("_Glossiness", props);
		bumpScale = FindProperty ("_BumpScale", props);
		bumpMap = FindProperty ("_BumpMap", props);
		emissionColorForRendering = FindProperty ("_EmissionColor", props);
		emissionMap = FindProperty("_EmissionMap", props);
		softParticlesEnabled = FindProperty("_SoftParticlesEnabled", props);
		cameraFadingEnabled = FindProperty("_CameraFadingEnabled", props);
		softParticlesNearFadeDistance = FindProperty("_SoftParticlesNearFadeDistance", props);
		softParticlesFarFadeDistance = FindProperty("_SoftParticlesFarFadeDistance", props);
		cameraNearFadeDistance = FindProperty("_CameraNearFadeDistance", props);
		cameraFarFadeDistance = FindProperty("_CameraFarFadeDistance", props);
	}

	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] props)
	{
		FindProperties (props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
		m_MaterialEditor = materialEditor;
		Material material = materialEditor.target as Material;

		// Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
		// material to a standard shader.
		// Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
		if (m_FirstTimeApply)
		{
			MaterialChanged(material);
			m_FirstTimeApply = false;
		}

		ShaderPropertiesGUI (material);
	}

	public void ShaderPropertiesGUI (Material material)
	{
		// Use default labelWidth
		EditorGUIUtility.labelWidth = 0f;

		// Detect any changes to the material
		EditorGUI.BeginChangeCheck();
		{
			BlendModePopup();

			EditorGUILayout.Space();
			GUILayout.Label(Styles.mainOptionsText, EditorStyles.boldLabel);

			FlipbookModePopup();
			LightingPopup(material);
			FadingPopup(material);
			DistortionPopup(material);

			EditorGUILayout.Space();
			GUILayout.Label(Styles.mapsOptionsText, EditorStyles.boldLabel);

			DoAlbedoArea(material);
			DoSpecularMetallicArea(material);
			DoNormalMapArea(material);
			DoEmissionArea(material);
			EditorGUI.BeginChangeCheck();
			m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);
			if (EditorGUI.EndChangeCheck())
				emissionMap.textureScaleAndOffset = albedoMap.textureScaleAndOffset; // Apply the main texture scale and offset to the emission texture as well, for Enlighten's sake			
		}
		if (EditorGUI.EndChangeCheck())
		{
			foreach (var obj in blendMode.targets)
				MaterialChanged((Material)obj);
		}

		EditorGUILayout.Space();

		GUILayout.Label(Styles.advancedOptionsText, EditorStyles.boldLabel);
		m_MaterialEditor.RenderQueueField();

		EditorGUILayout.Space();

		GUILayout.Label(Styles.requiredVertexStreamsText, EditorStyles.boldLabel);
		DoVertexStreamsArea(material);
	}

	public override void AssignNewShaderToMaterial (Material material, Shader oldShader, Shader newShader)
	{
		// _Emission property is lost after assigning Standard shader to the material
		// thus transfer it before assigning the new shader
		if (material.HasProperty("_Emission"))
		{
			material.SetColor("_EmissionColor", material.GetColor("_Emission"));
		}

		base.AssignNewShaderToMaterial(material, oldShader, newShader);

		if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
		{
			SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
			return;
		}

		BlendMode blendMode = BlendMode.Opaque;
		if (oldShader.name.Contains("/Transparent/Cutout/"))
		{
			blendMode = BlendMode.Cutout;
		}
		else if (oldShader.name.Contains("/Transparent/"))
		{
			// NOTE: legacy shaders did not provide physically based transparency
			// therefore Fade mode
			blendMode = BlendMode.Fade;
		}
		material.SetFloat("_Mode", (float)blendMode);

		MaterialChanged(material);
	}

	void BlendModePopup()
	{
		EditorGUI.showMixedValue = blendMode.hasMixedValue;
		var mode = (BlendMode)blendMode.floatValue;

		EditorGUI.BeginChangeCheck();
		mode = (BlendMode)EditorGUILayout.Popup(Styles.renderingMode, (int)mode, Styles.blendNames);
		if (EditorGUI.EndChangeCheck())
		{
			m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
			blendMode.floatValue = (float)mode;
		}

		EditorGUI.showMixedValue = false;
	}

	void FlipbookModePopup()
	{
		EditorGUI.showMixedValue = flipbookMode.hasMixedValue;
		var mode = (FlipbookMode)flipbookMode.floatValue;

		EditorGUI.BeginChangeCheck();
		mode = (FlipbookMode)EditorGUILayout.Popup(Styles.flipbookMode, (int)mode, Styles.flipbookNames);
		if (EditorGUI.EndChangeCheck())
		{
			m_MaterialEditor.RegisterPropertyChangeUndo("Flip-Book Mode");
			flipbookMode.floatValue = (float)mode;
		}

		EditorGUI.showMixedValue = false;
	}

	void LightingPopup(Material material)
	{
		// Some blend modes don't require lighting
		BlendMode blendMode = (BlendMode)material.GetFloat("_Mode");
		bool hasLighting = BlendModeSupportsLighting(blendMode);

		if (hasLighting)
		{
			EditorGUI.showMixedValue = lightingEnabled.hasMixedValue;
			var enabled = lightingEnabled.floatValue;

			if (blendMode == BlendMode.Transparent && !EditorGUI.showMixedValue)
			{
				using (new EditorGUI.DisabledScope(true))
				{
					EditorGUILayout.Toggle(Styles.lightingEnabled, true);
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				enabled = EditorGUILayout.Toggle(Styles.lightingEnabled, enabled != 0.0f) ? 1.0f : 0.0f;
				if (EditorGUI.EndChangeCheck())
				{
					m_MaterialEditor.RegisterPropertyChangeUndo("Lighting Enabled");
					lightingEnabled.floatValue = enabled;
				}
			}

			EditorGUI.showMixedValue = false;
		}
	}

	void FadingPopup(Material material)
	{
		// Z write doesn't work with fading
		bool hasZWrite = (material.GetInt("_ZWrite") != 0);
		if (!hasZWrite)
		{
			// Soft Particles
			{
				EditorGUI.showMixedValue = softParticlesEnabled.hasMixedValue;
				var enabled = softParticlesEnabled.floatValue;

				EditorGUI.BeginChangeCheck();
				enabled = EditorGUILayout.Toggle(Styles.softParticlesEnabled, enabled != 0.0f) ? 1.0f : 0.0f;
				if (EditorGUI.EndChangeCheck())
				{
					m_MaterialEditor.RegisterPropertyChangeUndo("Soft Particles Enabled");
					softParticlesEnabled.floatValue = enabled;
				}

				if (enabled != 0.0f)
				{
					int indentation = 2;
					m_MaterialEditor.ShaderProperty(softParticlesNearFadeDistance, Styles.softParticlesNearFadeDistanceText, indentation);
					m_MaterialEditor.ShaderProperty(softParticlesFarFadeDistance, Styles.softParticlesFarFadeDistanceText, indentation);
				}
			}

			// Camera Fading
			{
				EditorGUI.showMixedValue = cameraFadingEnabled.hasMixedValue;
				var enabled = cameraFadingEnabled.floatValue;

				EditorGUI.BeginChangeCheck();
				enabled = EditorGUILayout.Toggle(Styles.cameraFadingEnabled, enabled != 0.0f) ? 1.0f : 0.0f;
				if (EditorGUI.EndChangeCheck())
				{
					m_MaterialEditor.RegisterPropertyChangeUndo("Camera Fading Enabled");
					cameraFadingEnabled.floatValue = enabled;
				}

				if (enabled != 0.0f)
				{
					int indentation = 2;
					m_MaterialEditor.ShaderProperty(cameraNearFadeDistance, Styles.cameraNearFadeDistanceText, indentation);
					m_MaterialEditor.ShaderProperty(cameraFarFadeDistance, Styles.cameraFarFadeDistanceText, indentation);
				}
			}

			EditorGUI.showMixedValue = false;
		}
	}

	void DistortionPopup(Material material)
	{
		// Z write doesn't work with distortion
		bool hasZWrite = (material.GetInt("_ZWrite") != 0);
		if (!hasZWrite)
		{
			EditorGUI.showMixedValue = distortionEnabled.hasMixedValue;
			var enabled = distortionEnabled.floatValue;

			EditorGUI.BeginChangeCheck();
			enabled = EditorGUILayout.Toggle(Styles.distortionEnabled, enabled != 0.0f) ? 1.0f : 0.0f;
			if (EditorGUI.EndChangeCheck())
			{
				m_MaterialEditor.RegisterPropertyChangeUndo("Distortion Enabled");
				distortionEnabled.floatValue = enabled;
			}

			if (enabled != 0.0f)
			{
				int indentation = 2;
				m_MaterialEditor.ShaderProperty(distortionStrength, Styles.distortionStrengthText, indentation);
				m_MaterialEditor.ShaderProperty(distortionBlend, Styles.distortionBlendText, indentation);
			}

			EditorGUI.showMixedValue = false;
		}
	}

	void DoAlbedoArea(Material material)
	{
		m_MaterialEditor.TexturePropertyWithHDRColor(Styles.albedoText, albedoMap, albedoColor, m_ColorPickerHDRConfig, true);
		if (((BlendMode)material.GetFloat("_Mode") == BlendMode.Cutout))
		{
			m_MaterialEditor.ShaderProperty(alphaCutoff, Styles.alphaCutoffText, MaterialEditor.kMiniTextureFieldLabelIndentLevel);
		}
	}

	void DoEmissionArea(Material material)
	{	
		// Emission for GI?
		if (m_MaterialEditor.EmissionEnabledProperty())
		{
			bool hadEmissionTexture = emissionMap.textureValue != null;

			// Texture and HDR color controls
			m_MaterialEditor.TexturePropertyWithHDRColor(Styles.emissionText, emissionMap, emissionColorForRendering, m_ColorPickerHDRConfig, false);

			// If texture was assigned and color was black set color to white
			float brightness = emissionColorForRendering.colorValue.maxColorComponent;
			if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
				emissionColorForRendering.colorValue = Color.white;

			// change the GI flag and fix it up with emissive as black if necessary
			m_MaterialEditor.LightmapEmissionFlagsProperty( MaterialEditor.kMiniTextureFieldLabelIndentLevel, true );
		}
	}

	void DoSpecularMetallicArea(Material material)
	{
		bool useLighting = (material.GetFloat("_LightingEnabled") > 0.0f);
		if (useLighting)
		{
			bool hasGlossMap = metallicMap.textureValue != null;
			m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapText, metallicMap, hasGlossMap ? null : metallic);

			int indentation = 2; // align with labels of texture properties
			bool showSmoothnessScale = hasGlossMap;
			m_MaterialEditor.ShaderProperty(smoothness, showSmoothnessScale ? Styles.smoothnessScaleText : Styles.smoothnessText, indentation);
		}
	}

	void DoNormalMapArea(Material material)
	{
		bool useLighting = (material.GetFloat("_LightingEnabled") > 0.0f);
		bool useDistortion = (material.GetFloat("_DistortionEnabled") > 0.0f);
		if (useLighting || useDistortion)
		{
			m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap, bumpMap.textureValue != null ? bumpScale : null);
		}
	}

	void DoVertexStreamsArea(Material material)
	{
		// Display list of streams required to make this shader work
		bool useLighting = (material.GetFloat("_LightingEnabled") > 0.0f);
		bool useFlipbookBlending = (material.GetFloat("_FlipbookMode") > 0.0f);
		bool useTangents = material.GetTexture("_BumpMap") && useLighting;

		GUILayout.Label(Styles.streamPositionText, EditorStyles.label);

		if (useLighting)
			GUILayout.Label(Styles.streamNormalText, EditorStyles.label);

		GUILayout.Label(Styles.streamColorText, EditorStyles.label);
		GUILayout.Label(Styles.streamUVText, EditorStyles.label);

		if (useFlipbookBlending)
		{
			GUILayout.Label(Styles.streamUV2Text, EditorStyles.label);
			GUILayout.Label(Styles.streamAnimBlendText, EditorStyles.label);
		}

		if (useTangents)
			GUILayout.Label(Styles.streamTangentText, EditorStyles.label);

		// Set the streams on all systems using this material
		if (GUILayout.Button(Styles.streamApplyToAllSystemsText, EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
		{
			List<ParticleSystemVertexStream> streams = new List<ParticleSystemVertexStream>();
			streams.Add(ParticleSystemVertexStream.Position);
			
			if (useLighting)
				streams.Add(ParticleSystemVertexStream.Normal);

			streams.Add(ParticleSystemVertexStream.Color);
			streams.Add(ParticleSystemVertexStream.UV);

			if (useFlipbookBlending)
			{
				streams.Add(ParticleSystemVertexStream.UV2);
				streams.Add(ParticleSystemVertexStream.AnimBlend);
			}

			if (useTangents)
				streams.Add(ParticleSystemVertexStream.Tangent);

			ParticleSystemRenderer[] renderers = UnityEngine.Object.FindObjectsOfType(typeof(ParticleSystemRenderer)) as ParticleSystemRenderer[];
			foreach (ParticleSystemRenderer renderer in renderers)
			{
				if (renderer.sharedMaterial == material)
					renderer.SetActiveVertexStreams(streams);
			}
		}

		EditorGUILayout.Space();
	}

	public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
	{
		switch (blendMode)
		{
			case BlendMode.Opaque:
				material.SetOverrideTag("RenderType", "");
				material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.DisableKeyword("_ALPHAMODULATE_ON");
				material.DisableKeyword("_ALPHAOVERLAY_ON");
				material.renderQueue = -1;
				break;
			case BlendMode.Cutout:
				material.SetOverrideTag("RenderType", "TransparentCutout");
				material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.EnableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.DisableKeyword("_ALPHAMODULATE_ON");
				material.DisableKeyword("_ALPHAOVERLAY_ON");
				material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
				break;
			case BlendMode.Fade:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.DisableKeyword("_ALPHAMODULATE_ON");
				material.DisableKeyword("_ALPHAOVERLAY_ON");
				material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
				break;
			case BlendMode.Transparent:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				material.DisableKeyword("_ALPHAMODULATE_ON");
				material.DisableKeyword("_ALPHAOVERLAY_ON");
				material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
				break;
			case BlendMode.Additive:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.DisableKeyword("_ALPHAMODULATE_ON");
				material.DisableKeyword("_ALPHAOVERLAY_ON");
				material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
				break;
			case BlendMode.Subtractive:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.ReverseSubtract);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.DisableKeyword("_ALPHAMODULATE_ON");
				material.DisableKeyword("_ALPHAOVERLAY_ON");
				material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
				break;
			case BlendMode.Modulate:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Multiply);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.EnableKeyword("_ALPHAMODULATE_ON");
				material.DisableKeyword("_ALPHAOVERLAY_ON");
				material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
				break;
			case BlendMode.Overlay:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.DisableKeyword("_ALPHAMODULATE_ON");
				material.EnableKeyword("_ALPHAOVERLAY_ON");
				material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
				break;
			case BlendMode.AdditiveOverlay:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.DisableKeyword("_ALPHAMODULATE_ON");
				material.EnableKeyword("_ALPHAOVERLAY_ON");
				material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
				break;
		}
	}

	static void SetMaterialKeywords(Material material)
	{
		// Z write doesn't work with distortion/fading
		bool hasZWrite = (material.GetInt("_ZWrite") != 0);

		// Set the define for lighting
		BlendMode blendMode = (BlendMode)material.GetFloat("_Mode");
		bool useLighting = (material.GetFloat("_LightingEnabled") > 0.0f) && BlendModeSupportsLighting(blendMode);
		if (blendMode == BlendMode.Transparent)
			useLighting = true;
		SetKeyword(material, "_UNLIT", !useLighting);
		
		// Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
		// (MaterialProperty value might come from renderer material property block)
		bool useDistortion = (material.GetFloat("_DistortionEnabled") > 0.0f) && !hasZWrite;
		SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") && (useLighting || useDistortion));
		SetKeyword(material, "_METALLICGLOSSMAP", (material.GetTexture("_MetallicGlossMap") != null) && useLighting);

		// A material's GI flag internally keeps track of whether emission is enabled at all, it's enabled but has no effect
		// or is enabled and may be modified at runtime. This state depends on the values of the current flag and emissive color.
		// The fixup routine makes sure that the material is in the correct state if/when changes are made to the mode or color.
		MaterialEditor.FixupEmissiveFlag( material );
		bool shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
		SetKeyword (material, "_EMISSION", shouldEmissionBeEnabled);

		// Set the define for flipbook blending
		bool useFlipbookBlending = (material.GetFloat("_FlipbookMode") > 0.0f);
		SetKeyword(material, "_REQUIRE_UV2", useFlipbookBlending);

		// Clamp fade distances
		bool useSoftParticles = (material.GetFloat("_SoftParticlesEnabled") > 0.0f);
		bool useCameraFading = (material.GetFloat("_CameraFadingEnabled") > 0.0f);
		float softParticlesNearFadeDistance = material.GetFloat("_SoftParticlesNearFadeDistance");
		float softParticlesFarFadeDistance = material.GetFloat("_SoftParticlesFarFadeDistance");
		float cameraNearFadeDistance = material.GetFloat("_CameraNearFadeDistance");
		float cameraFarFadeDistance = material.GetFloat("_CameraFarFadeDistance");

		if (softParticlesNearFadeDistance < 0.0f)
		{
			softParticlesNearFadeDistance = 0.0f;
			material.SetFloat("_SoftParticlesNearFadeDistance", 0.0f);
		}
		if (softParticlesFarFadeDistance < 0.0f)
		{
			softParticlesFarFadeDistance = 0.0f;
			material.SetFloat("_SoftParticlesFarFadeDistance", 0.0f);
		}
		if (cameraNearFadeDistance < 0.0f)
		{
			cameraNearFadeDistance = 0.0f;
			material.SetFloat("_CameraNearFadeDistance", 0.0f);
		}
		if (cameraFarFadeDistance < 0.0f)
		{
			cameraFarFadeDistance = 0.0f;
			material.SetFloat("_CameraFarFadeDistance", 0.0f);
		}

		// Set the define for fading
		bool useFading = (useSoftParticles || useCameraFading) && !hasZWrite;
		SetKeyword(material, "_FADING_ON", useFading);
		if (useSoftParticles)
			material.SetVector("_SoftParticleFadeParams", new Vector4(softParticlesNearFadeDistance, 1.0f / (softParticlesFarFadeDistance - softParticlesNearFadeDistance), 0.0f, 0.0f));
		else
			material.SetVector("_SoftParticleFadeParams", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
		if (useCameraFading)
			material.SetVector("_CameraFadeParams", new Vector4(cameraNearFadeDistance, 1.0f / (cameraFarFadeDistance - cameraNearFadeDistance), 0.0f, 0.0f));
		else
			material.SetVector("_CameraFadeParams", new Vector4(0.0f, Mathf.Infinity, 0.0f, 0.0f));

		// Set the define for distortion + grabpass
		SetKeyword(material, "_DISTORTION_ON", useDistortion);
		material.SetShaderPassEnabled("Always", useDistortion);
	}

	static void MaterialChanged(Material material)
	{
		SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));

		SetMaterialKeywords(material);
	}

	static void SetKeyword(Material m, string keyword, bool state)
	{
		if (state)
			m.EnableKeyword (keyword);
		else
			m.DisableKeyword (keyword);
	}

	static bool BlendModeSupportsLighting(BlendMode blendMode)
	{
		return (blendMode != BlendMode.Additive && blendMode != BlendMode.Subtractive && blendMode != BlendMode.Modulate && blendMode != BlendMode.Overlay);
	}
}

} // namespace UnityEditor
#endif