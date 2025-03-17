Shader "TestI/UIHighlightDiagonal"
// {
//     Properties
//     {
//         _MainTex ("Sprite Texture", 2D) = "white" {}
//         _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
//         _HighlightProgress ("Highlight Progress", Range(-1,2)) = 0
//         _HighlightWidth ("Highlight Width", Range(0.05, 0.5)) = 0.2
//     }
    
//     SubShader
//     {
//         Tags {"Queue"="Transparent" "RenderType"="Transparent"}
//         Blend SrcAlpha OneMinusSrcAlpha
//         Cull Off
//         ZWrite Off

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #include "UnityCG.cginc"

//             struct appdata_t
//             {
//                 float4 vertex : POSITION;
//                 float2 uv : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float4 pos : SV_POSITION;
//                 float2 uv : TEXCOORD0;
//             };

//             sampler2D _MainTex;
//             float4 _HighlightColor;
//             float _HighlightProgress;
//             float _HighlightWidth;

//             v2f vert (appdata_t v)
//             {
//                 v2f o;
//                 o.pos = UnityObjectToClipPos(v.vertex);
//                 o.uv = v.uv;
//                 return o;
//             }

//             fixed4 frag (v2f i) : SV_Target
//             {
//                 fixed4 col = tex2D(_MainTex, i.uv);

//                 // 计算斜线的进度（从左上到右下）
//                 float diagonal = i.uv.x - i.uv.y;  // 这条线从左上到右下
//                 float highlightStart = _HighlightProgress - _HighlightWidth * 0.5;
//                 float highlightEnd = _HighlightProgress + _HighlightWidth * 0.5;

//                 // 计算线条遮罩，形成清晰的斜线
//                 float mask = smoothstep(highlightStart, highlightEnd, diagonal) 
//                            - smoothstep(highlightEnd, highlightEnd + _HighlightWidth * 0.5, diagonal);

//                 // 计算最终颜色
//                 col.rgb = lerp(col.rgb, _HighlightColor.rgb, mask * _HighlightColor.a);
                
//                 return col;
//             }
//             ENDCG
//         }
//     }
// }

{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
        _HighlightWidth ("Highlight Width", Range(0.02, 0.5)) = 0.1
        _Speed ("Highlight Speed", Range(0.1, 5.0)) = 1.0
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _HighlightColor;
            float _HighlightWidth;
            float _Speed;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 计算当前高亮线的位置，基于时间
                float highlightProgress = frac(_Time.y * _Speed) * 2.0 - 2.0; // -1 到 1 的循环运动

                // 计算斜线的进度（从左上到右下）
                float diagonal = i.uv.x - i.uv.y;  
                float highlightStart = highlightProgress - _HighlightWidth * 0.5;
                float highlightEnd = highlightProgress + _HighlightWidth * 0.5;

                // 计算线条遮罩，形成清晰的斜线
                float mask = smoothstep(highlightStart, highlightEnd, diagonal) 
                           - smoothstep(highlightEnd, highlightEnd + _HighlightWidth * 0.5, diagonal);

                // 应用高亮颜色
                col.rgb = lerp(col.rgb, _HighlightColor.rgb, mask * _HighlightColor.a);
                
                return col;
            }
            ENDCG
        }
    }
}