Shader "Unlit/VHSUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseIntensity ("Noise Intensity", Range(0,1)) = 0.12
        _NoiseScale ("Noise Scale", Float) = 200.0
        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 0.25
        _ScanlineCount ("Scanline Count", Float) = 400.0
        _ScanlineSpeed ("Scanline Speed", Float) = 1.0
        _Chromatic ("Chromatic Aberration", Range(0,0.02)) = 0.003
        _WobbleAmp ("Wobble Amp", Range(0,0.05)) = 0.01
        _WobbleFreq ("Wobble Freq", Float) = 8.0
        _Vignette ("Vignette", Range(0,1)) = 0.25
        _Desaturation ("Desaturation", Range(0,1)) = 0.0
        _Contrast ("Contrast", Range(0.5,2)) = 1.1
        _Jitter ("Jitter", Vector) = (0,0,0,0)
        _TimeGlobal ("TimeGlobal", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _NoiseIntensity;
            float _NoiseScale;
            float _ScanlineIntensity;
            float _ScanlineCount;
            float _ScanlineSpeed;
            float _Chromatic;
            float _WobbleAmp;
            float _WobbleFreq;
            float _Vignette;
            float _Desaturation;
            float _Contrast;
            float4 _Jitter;
            float _TimeGlobal;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            // simple pseudo-random
            float rand(float2 co) {
                return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453);
            }

            float3 ApplyContrast(float3 col, float contrast)
            {
                return ((col - 0.5) * contrast) + 0.5;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = TRANSFORM_TEX(i.uv, _MainTex);

                // Vertical wobble (simulate VHS tape warp)
                float wob = sin((uv.x * _WobbleFreq) + (_TimeGlobal * 2.0)) * _WobbleAmp;
                uv.y += wob * (1.0 - uv.y); // stronger near bottom slightly

                // line jitter (per-line horizontal offset)
                float lineOffset = sin((uv.y * _ScanlineCount * 0.1) + _TimeGlobal * 30.0) * (_Jitter.x * 0.004);
                uv.x += lineOffset;

                // chromatic aberration: sample channels with small offsets influenced by jitter
                float2 chroma = float2(_Chromatic, 0) + (_Jitter.xy * 0.002);

                float3 colR = tex2D(_MainTex, uv + chroma).rgb;
                float3 colG = tex2D(_MainTex, uv).rgb;
                float3 colB = tex2D(_MainTex, uv - chroma).rgb;
                float3 col = float3(colR.r, colG.g, colB.b);

                // Scanlines
                float scan = sin((uv.y * _ScanlineCount * 3.14159) + (_TimeGlobal * _ScanlineSpeed));
                scan = (scan * 0.5) + 0.5; // 0..1
                float scanMod = lerp(1.0, scan, _ScanlineIntensity);
                col *= scanMod;

                // Noise
                float n = rand(uv * _NoiseScale + _TimeGlobal * 37.0);
                col += (n - 0.5) * _NoiseIntensity;

                // Ghost smear: sample a slightly offset uv and blend lightly
                float2 ghostOff = _Jitter.xy * 0.006;
                float3 ghost = tex2D(_MainTex, uv + ghostOff).rgb;
                col = lerp(col, ghost, 0.06);

                // Vignette
                float2 centre = float2(0.5, 0.5);
                float dist = distance(i.uv, centre);
                float vig = smoothstep(0.8, 0.3, dist); // 1 at center -> 0 at edges
                col *= lerp(1.0, 1.0 - _Vignette, 1.0 - vig);

                // Desaturate a bit (optional)
                float lum = dot(col, float3(0.299, 0.587, 0.114));
                col = lerp(col, float3(lum, lum, lum), _Desaturation);

                // Contrast
                col = ApplyContrast(col, _Contrast);

                col = saturate(col);

                return float4(col, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Texture"
}