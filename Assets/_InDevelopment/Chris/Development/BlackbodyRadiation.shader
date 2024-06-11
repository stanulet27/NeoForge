Shader "Unlit/BlackbodyRadiation"
{
    Properties
    {
        _Temperature ("Temperature", Float) = 6500
        _Metallic ("Metallic", Range(0, 1)) = 1.0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.8
        _EmissionIntensity ("Emission Intensity", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows
            #include "UnityCG.cginc"
            
            struct Input
            {
                float2 uv_MainTex;
            };
            

            float _Temperature;
            half _Metallic;
            half _Smoothness;
            half _EmissionIntensity;


            
            float3 TemperatureToRGB(float kelvin)
            {
                float3 color;
                kelvin = kelvin / 100.0;
                if (kelvin <= 66)
                {
                    color.r = 255.0;
                    color.g = kelvin;
                    color.g = 99.4708025861 * log(color.g) - 161.1195681661;

                    if (kelvin <= 19)
                    {
                        color.b = 0.0;
                    }
                    else
                    {
                        color.b = kelvin - 10;
                        color.b = 138.5177312231 * log(color.b) - 305.0447927307;
                    }
                }
                else
                {
                    color.r = kelvin - 60.0;
                    color.r = 329.698727446 * pow(color.r, -0.1332047592);
                    color.g = kelvin - 60.0;
                    color.g = 288.1221695283 * pow(color.g, -0.0755148492);
                    color.b = 255.0;
                }
                color = saturate(color / 255.0);
                return color;
            }

            float3 BlackTo550(float tempterature)
            {
                float3 color;
                float3 color550 = TemperatureToRGB(550);

                color.r = (color550.r) * (tempterature - 500.0) / (550.0 - 500.0);
                color.g = (color550.g) * (tempterature - 500.0) / (550.0 - 500.0);
                color.b = (color550.b) * (tempterature - 500.0) / (550.0 - 500.0);
                //color = saturate(color / 255.0);
                return color;
            }
            float3 SteelToBlack(float kelvin)
            {
                //lerp between steel and black (221,221,221) (0,0,0)
                float3 color;
                
                color.r = 221 - 221.0 * ((kelvin-450.0) / (500.0 - 450.0));
                color.g = 221 - 221.0 * ((kelvin-450.0) / (500.0 - 450.0));
                color.b = 221 - 221.0 * ((kelvin-450.0) / (500.0 - 450.0));
                
                color = saturate(color / 255.0);
                return color;
            }
            
            void surf (Input IN, inout SurfaceOutputStandard o)
            {
                float3 color;
                if(_Temperature >= 550)
                {
                    color = TemperatureToRGB(_Temperature);
                    _Metallic = 0;
                }
                else if (_Temperature >= 500)
                {
                    color = BlackTo550(_Temperature);
                    _Metallic = 0;
                }
                else if(_Temperature >= 450)  {
                    color = SteelToBlack(_Temperature);
                    _Smoothness = _Temperature / 500.0;
                    _Metallic = 1 - _Temperature / 500.0;
                    _EmissionIntensity = 0;
                }
                else
                {
                    float grey = 200.0/255.0;
                    color = float3(grey,grey,grey);
                    _EmissionIntensity = 0;
                }
                o.Albedo = color;
                o.Metallic = _Metallic;
                o.Smoothness = _Smoothness;
                o.Emission = color * _EmissionIntensity;
            }
            ENDCG
        
    }
    FallBack "Diffuse"
}