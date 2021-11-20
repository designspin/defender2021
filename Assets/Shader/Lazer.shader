Shader "Custom/Lazer"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 hash(float2 p) {
                return frac(sin(float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)))) * 43758.5453);
            }

            float2 hash22(float2 p) {
                float3 p3 = frac(float3(p.xyx) * float3(.1031, .1030, .0973));
                p3 += dot(p3, p3.yzx+19.19);
                return frac((p3.xx+p3.yz) * p3.zy);
            }

            float noise(float2 p) {
                float2 n = floor(p);
                float2 f = frac(p);

                float md = 1.0;

                float2 o = hash22(n) * 0.96 + 0.02;
                float2 r = o -f;
                float2 d = dot(r, r);
                md = min(d, md);

                return md;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 col = fixed4(step(0.1, 1 + cos(noise(i.uv) * _Time.w)), noise(i.uv), noise(i.uv), 0);
                fixed4 col = fixed4(0,0,0,0);

                for(float j = 0; j < 1; j = j + 0.5)
                {
                    fixed shade = step(1 * j, i.uv.x * (1 + cos(_Time.w))) - step(((1 * j) + cos(_Time.w)), i.uv.x);
                    fixed shade1 = step(1 * j / 2, i.uv.x * (1 + cos(_Time.w))) * step(((1 * j) + cos(_Time.w)), i.uv.x);
                    fixed shade2 = step(1 * j / 3, i.uv.x * (1 + cos(_Time.w))) - step(1 - ((1 * j) + cos(_Time.w)), i.uv.x);
                    //col += fixed4(1 + tan(_Time.w * shade), tan(_Time.w * shade),cos(_Time.w * shade), 1 + tan(_Time.w * noise(i.uv)));
                    col += fixed4(noise(i.uv) * shade, noise(i.uv) * shade1, noise(i.uv) * shade2,1 + cos(_Time.w * noise(i.uv)));
                }

                return col;
            }
            ENDCG
        }
    }
}
