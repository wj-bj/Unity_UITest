Shader "UITest/DissolveEffect"
{
    Properties
    {

       _MainTex ("MainTex", 2D) = "white" {} // 边缘颜色
        _MaskTex ("Dissolve Mask", 2D) = "white" {} // 控制消散的灰度图
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0 // 消散强度
        [HideInInspector]_EdgeWidth ("Edge Width", Range(0, 0.2)) = 0.05 // 边缘宽度
        _EdgeColor ("Edge Color", Color) = (1,1,1,1) // 边缘颜色
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha // 透明混合
        Cull Off // 让 UI 贴图双面可见
        ZWrite Off // 关闭深度写入
        
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
            sampler2D _MaskTex;
            float4 _MainTex_ST;
            float _DissolveAmount;
            float _EdgeWidth;
            float4 _EdgeColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 读取 UI 贴图颜色
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 读取 Mask 灰度值
                float mask = tex2D(_MaskTex, i.uv).r;

                // 计算消散效果
                float dissolve = smoothstep(mask - _EdgeWidth, mask, _DissolveAmount);
                dissolve = _DissolveAmount==0?0:dissolve;
                float edgewith = lerp(0, 0.1, _DissolveAmount);
                // 边缘发光效果
                float edge = smoothstep(mask, mask + edgewith, _DissolveAmount);
                edge = _DissolveAmount==1?1:edge;
                col.rgb = lerp(col.rgb, _EdgeColor.rgb, edge);
                
                // 应用透明度
                col.a *= dissolve;

                return col;
            }
            ENDCG
        }
    }
}