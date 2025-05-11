Shader "Custom/FogOfWarShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlobalDarkness ("Global Darkness", Range(0, 1)) = 0.9
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        
        // Отключаем запись в z-буфер
        ZWrite Off
        // Включаем смешивание
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
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _PlayerPosition;
            float3 _PlayerForward;
            float _LightAngle;
            float _LightDistance;
            float _GlobalDarkness;
            float _LightIntensity;
            float4 _LightColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Получаем базовый цвет из текстуры
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Рассчитываем направление от игрока к текущему пикселю
                float3 dirToPixel = normalize(i.worldPos - _PlayerPosition);
                
                // Рассчитываем угол между направлением игрока и направлением к пикселю
                float angleToPixel = degrees(acos(dot(_PlayerForward, dirToPixel)));
                
                // Рассчитываем расстояние от игрока до пикселя
                float distToPixel = distance(_PlayerPosition, i.worldPos);
                
                // Проверяем, находится ли пиксель в конусе света фонарика
                float lightFactor = 0;
                if (angleToPixel <= _LightAngle / 2 && distToPixel <= _LightDistance)
                {
                    // Расчет интенсивности света в зависимости от расстояния и угла
                    float distFactor = 1 - saturate(distToPixel / _LightDistance);
                    float angleFactor = 1 - saturate(angleToPixel / (_LightAngle / 2));
                    
                    lightFactor = distFactor * angleFactor * _LightIntensity;
                }
                
                // Результирующая прозрачность пикселя (темнота)
                float darkness = _GlobalDarkness * (1 - lightFactor);
                
                // Применяем туман войны как полупрозрачный черный цвет
                return fixed4(0, 0, 0, darkness);
            }
            ENDCG
        }
    }
} 