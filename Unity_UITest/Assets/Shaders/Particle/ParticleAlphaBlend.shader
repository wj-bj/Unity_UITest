Shader "UITest/Particles/Alpha_Blended" 
{
	Properties 
	{
		[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		[Toggle(_ENABLE_FOG_TRANS)] _enableFog("启用场景雾", Float) = 0

		// _StencilComp ("Stencil Comparison", Float) = 8
        // _Stencil ("Stencil ID", Float) = 0
        // _StencilOp ("Stencil Operation", Float) = 0
        // _StencilWriteMask ("Stencil Write Mask", Float) = 255
        // _StencilReadMask ("Stencil Read Mask", Float) = 255
		[HideInInspector]_LogicControlAlpha ("Logic control alpha",Range(0,1)) = 1
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane"}
		Blend SrcAlpha OneMinusSrcAlpha
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
			half _LogicControlAlpha;
			CBUFFER_END
			float4 _Area;
			struct Attributes 
			{
				float4 positionOS : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				
			};

			struct Varyings 
			{
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				FOG_COORD(2)
			};
			


			Varyings vert (Attributes input)
			{
				Varyings output;
				
				output.color = input.color * _TintColor;
				output.texcoord = TRANSFORM_TEX(input.texcoord,_MainTex);
				output.worldPos = TransformObjectToWorld(input.positionOS.xyz);
				output.vertex = TransformWorldToHClip(output.worldPos.xyz);
				TRANSFER_FOG(output.vertex.z,output.worldPos,output)
				return output;
			}

			half4 frag (Varyings input) : SV_Target
			{
				half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord) * input.color * 2.0f;
				c.a = saturate(c.a);

				#ifdef _UI_CLIP 
				if(input.worldPos.x <= _Area.x || input.worldPos.x >= _Area.y || input.worldPos.y <= _Area.z || input.worldPos.y >= _Area.w)
				{
					c.a = 0;
				}
				#endif
				MIX_FOG(c,input)
				return half4(c.rgb,c.a*_LogicControlAlpha);
			}
			ENDHLSL 
		}
	}	
}
