Shader "Custom/StarField"
{
    Properties 
    {
        _VelX ("Velocity X", Float) = 0
        _Auto ("Auto Velocity", Float) = 0
    }

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

            float _VelX;
            float _Auto;

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

            float3 starfield(float2 samplePosition, float threshold) {
                float starValue = noise(samplePosition);
                float power = max(1 - (starValue / threshold), 0.0);
                power = power * power * power;
                return float3(power, power, power);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 finalColor;
                float2 sCoord = i.uv.xy * 4.0;

                float velocity = _VelX;
                if(_Auto) {
                    velocity = _Time.y * 0.3;
                }

                float2 pos = i.uv + float2(velocity, 0);

                finalColor = float3(0.5 - (i.uv.y * 2),0, 1 - (i.uv.y * 1.5));

                for(int i = 1; i <= 3; i++) {
                    float fi = float(i);
                    float inv = sqrt(1.0/fi);
                    finalColor += starfield((sCoord + float2(fi*100.0, -fi*50.0)) * (1.0 + fi * 0.2) + pos, 0.0005) * inv;
                }

                return fixed4(finalColor, 0.1);
            }
            ENDCG
        }
    }
}
