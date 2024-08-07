﻿Shader "Custom/MountainRadar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OffsetX ("Offset X", float) = 0.0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "LightweightPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Cull Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

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
            //float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _OffsetX;
            
            fixed4 frag (v2f i) : Color
            {
                fixed4 col = tex2D(_MainTex, i.uv + float2(_OffsetX, 0.0));

                // if(col.a < 0.5) {
                //     col = fixed4(0,0,0,1);
                // }
                return col;
            }
            ENDCG
        }
    }
}
