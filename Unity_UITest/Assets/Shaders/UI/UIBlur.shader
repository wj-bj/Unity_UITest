Shader "UITest/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurDirection ("Blur Direction", Vector) = (1, 0, 0, 0)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
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
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _BlurDirection;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Gaussian weights for 5 samples
                float weights[5] = { 0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216 };
                
                // Center sample
                fixed4 col = tex2D(_MainTex, i.uv) * weights[0];
                
                // Offset samples
                for (int j = 1; j < 5; j++)
                {
                    float2 offset = _BlurDirection.xy * _MainTex_TexelSize.xy * j;
                    col += tex2D(_MainTex, i.uv + offset) * weights[j];
                    col += tex2D(_MainTex, i.uv - offset) * weights[j];
                }
                
                return col;
            }
            ENDCG
        }
    }
} 