// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'TransformObjectToHClip(*)'

Shader "UITest/Particles/Additive" 
{
	Properties 
	{
		[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_Lightness ("Lightness", Range(1.0, 3.0)) = 2.0
		[Gamma]_MainTex ("Particle Texture", 2D) = "white" {}
		[Toggle(_ENABLE_FOG_TRANS)] _enableFog("启用场景雾", Float) = 0
		[HideInInspector]_LogicControlAlpha ("Logic control alpha",Range(0,1)) = 1
		 [Toggle(_ENABLE_FOG_TRANS)] _enableFog("启用场景雾", Float) = 0

		// _StencilComp ("Stencil Comparison", Float) = 8
        // _Stencil ("Stencil ID", Float) = 0
        // _StencilOp ("Stencil Operation", Float) = 0
        // _StencilWriteMask ("Stencil Write Mask", Float) = 255
        // _StencilReadMask ("Stencil Read Mask", Float) = 255
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		//ColorMask RGB
		Cull Off 
		Lighting Off 
		ZWrite Off
		
		// Stencil
		// {
		// 	Ref [_Stencil]
		// 	Comp [_StencilComp]
		// 	Pass [_StencilOp]
		// 	ReadMask [_StencilReadMask]
		// 	WriteMask [_StencilWriteMask]
		// }

		Pass 
		{
			HLSLPROGRAM
			#pragma multi_compile _ _UI_CLIP
			#pragma multi_compile _ _ENABLE_FOG
            #pragma multi_compile _ _ENABLE_FOG_TRANS           
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Assets/Shaders/Particle/ParticlesCommon.hlsl"
			TEXTURE2D(_MainTex);  SAMPLER(sampler_MainTex);
			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			half4 _TintColor;
			half _Lightness;
			half _LogicControlAlpha;
			CBUFFER_END
			float4 _Area;

			struct Attributes 
			{
				float4 positionOS : POSITION;
				half4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct Varyings 
			{
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				FOG_COORD(2)
			};
			
			Varyings vert (Attributes input)
			{
				Varyings output = (Varyings)0;
				
				output.color = input.color;
				output.uv = TRANSFORM_TEX(input.uv,_MainTex);
				output.worldPos =TransformObjectToWorld(input.positionOS.xyz);  //mul(unity_ObjectToWorld, input.positionOS);
				output.vertex = TransformWorldToHClip(output.worldPos.xyz);
				TRANSFER_FOG(output.vertex.z,output.worldPos,output)
				return output;
			}

			half4 frag (Varyings input) : SV_Target
			{
				half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
				c = _Lightness * input.color * _TintColor * c;

				#ifdef _UI_CLIP 
				if(input.worldPos.x <= _Area.x || input.worldPos.x >= _Area.y || input.worldPos.y <= _Area.z || input.worldPos.y >= _Area.w)
				{
					c.a = 0;
				}
				#endif
				// c.rgb = ParticlesOutColor(c.rgb) * 0.5;
				MIX_FOG_ADD(c,input)
				return half4(c.rgb,c.a*_LogicControlAlpha);
			}
			ENDHLSL 
		}
	}	
}
