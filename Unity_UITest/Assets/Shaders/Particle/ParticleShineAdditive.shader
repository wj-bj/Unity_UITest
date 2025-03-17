
Shader "UITest/Particles/Shine_Additive" 
{
	Properties 
	{
		[HDR]_MainColor ("Main Color", Color) = (1, 1, 1, 1)
		// _Intensity("Intensity", Range(0,2)) = 1.0
		_MainTex ("Main Texture", 2D) = "white" {}
		[Enum(CullMode)]_CullMode("Cull Mode", Float) = 0
		[HideInInspector]_LogicControlAlpha ("Logic control alpha",Range(0,1)) = 1
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		Cull [_CullMode] 
		Lighting Off 
		ZWrite Off 
		ZTest Off
		
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			TEXTURE2D(_MainTex);	SAMPLER(sampler_MainTex);
			CBUFFER_START(UnityPerMaterial)
			half4 _MainTex_ST;
			float4 _MainColor;
			half _LogicControlAlpha;
			CBUFFER_END
			float _Intensity;
			
			struct Attributes 
			{
				float4 pos : POSITION;
				half4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			struct Varyings 
			{
				half4 color : COLOR0;
				half2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Varyings vert (Attributes input) 
			{
				Varyings output;
				output.color = input.color;
				output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
				output.pos = TransformObjectToHClip(input.pos);
				return output;
			}

			half4 frag (Varyings input) : SV_TARGET 
			{
				half4 c = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, input.uv)* input.color * _MainColor;// * _Intensity;
				c.a *= _LogicControlAlpha;
				return c;
			}
			ENDHLSL
		}
	}

}