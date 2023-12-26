cbuffer VSMaterialConstants : register(b1)
{
    float3 ViewDirection;
    float3 LightPos;
    
    float2 Repeat;
    float2 Skew;
}

cbuffer PSMaterialConstants : register(b1)
{
    float3 DiffuseColor;
    float3 EmissiveColor;
    float3 SpecularColor;

    float SpecularPower;
    float SpecularIntensity;
    
    float3 LightColor;
    float3 AmbientColor;
    float Roughness;
    float Albedo;
    float Radius;
    float Falloff;
}

float attenuation(float r, float f, float d)
{
    float denom = d / r + 1.0f;
    float attenuation = 1.0f / (denom * denom);
    float t = (attenuation - f) / (1.0f - f);
    return max(t, 0.0f);
}

float phongSpecular(float3 lightDirection, float3 viewDirection, float3 surfaceNormal, float shininess)
{
    float3 R = -reflect(lightDirection, surfaceNormal); // Should be negative?
    return pow(max(0.0f, dot(viewDirection, R)), shininess);
}

float3 toGamma(float3 v)
{
    float gamma = 1.0f / 2.2f;
    return float3(pow(v.x, gamma), pow(v.y, gamma), pow(v.z, gamma));
}

float orenNayarDiffuse(float3 lightDirection, float3 viewDirection, float3 surfaceNormal, float roughness, float albedo)
{
    float LdotV = dot(lightDirection, viewDirection);
    float NdotL = dot(lightDirection, surfaceNormal);
    float NdotV = dot(surfaceNormal, viewDirection);

    float s = LdotV - NdotL * NdotV;
    float t = lerp(1.0f, max(NdotL, NdotV), step(0.0f, s));

    float sigma2 = roughness * roughness;
    float A = 1.0f + sigma2 * (albedo / (sigma2 + 0.13f) + 0.5f / (sigma2 + 0.33f));
    float B = 0.45f * sigma2 / (sigma2 + 0.09f);

    return albedo * max(0.0f, NdotL) * (A + B * s / t) / 3.14159265f;
}