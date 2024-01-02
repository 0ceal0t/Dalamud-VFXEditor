cbuffer VSMaterialConstants : register(b1)
{
    float2 Repeat;
    float2 Skew;
}

struct LightData
{
    float3 Color;
    float Radius;
    float3 Position;
    float Falloff;
};

cbuffer PSMaterialConstants : register(b1)
{
    float3 DiffuseColor;
    float3 EmissiveColor;
    float3 SpecularColor;

    float SpecularPower;
    float SpecularIntensity;
    
    float3 AmbientColor;
    float Roughness;
    float Albedo;
    
    float3 ViewDirection;
    LightData Light1;
    LightData Light2;
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

float computeDiffuse(float3 viewDirection, float3 worldPos, float radius, float lightFalloff, float3 lightPos, float3 N)
{
    float3 lightDir = lightPos - worldPos;
    float lightDistance = length(lightDir);
    float falloff = attenuation(radius, lightFalloff, lightDistance);
    float3 L = normalize(lightDir);
    float3 V = normalize(viewDirection - worldPos);
    
    return orenNayarDiffuse(L, V, N, Roughness, Albedo) * falloff;
}

float3 computeDiffuse(LightData data, float3 worldPos, float3 N)
{
    return data.Color * computeDiffuse(ViewDirection, worldPos, data.Radius, data.Falloff, data.Position, N);
}

float computeSpecular(float3 viewDirection, float3 worldPos, float radius, float lightFalloff, float3 lightPos, float3 N)
{
    float3 lightDir = lightPos - worldPos;
    float lightDistance = length(lightDir);
    float falloff = attenuation(radius, lightFalloff, lightDistance);
    float3 L = normalize(lightDir);
    float3 V = normalize(viewDirection - worldPos);
    
    float df = dot(-lightDir, N);
    if (df < 0.0f)
    {
        return phongSpecular(L, V, N, SpecularIntensity * 10) * SpecularPower * falloff;
    }
    return 0.0f;
}

float3 computeSpecular(LightData data, float3 worldPos, float3 N)
{
    return data.Color * computeSpecular(ViewDirection, worldPos, data.Radius, data.Falloff, data.Position, N);
}