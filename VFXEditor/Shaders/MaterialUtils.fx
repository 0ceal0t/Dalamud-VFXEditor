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
    
    float3 EyePosition;
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

float3 toGamma(float3 v)
{
    float gamma = 1.0f / 2.2f;
    return float3(pow(v.x, gamma), pow(v.y, gamma), pow(v.z, gamma));
}

float3 computeDiffuse(LightData light, float3 worldPos, float3 N)
{
    float3 L = light.Position - worldPos;
    float distance = length(L);
    L = L / distance;
    float att = attenuation(light.Radius, light.Falloff, distance);
    
    float NdotL = max(0, dot(N, L));
    float diffuse = NdotL * att;
    if (diffuse < 0)
    {
        diffuse = 0;
    }
    return light.Color * diffuse;
}

float3 computeSpecular(LightData light, float3 worldPos, float3 N)
{
    float3 L = light.Position - worldPos;
    float distance = length(L);
    L = L / distance;
    float att = attenuation(light.Radius, light.Falloff, distance);
    
    float3 V = normalize(EyePosition - worldPos);
    float3 R = normalize(reflect(-L, N));
    float RdotV = max(0, dot(R, V));
    
    float3 H = normalize(L + V);
    float NdotH = max(0, dot(N, H));
    
    float specular = pow(RdotV, SpecularIntensity * 10) * SpecularPower * att;
    if (specular < 0)
    {
        specular = 0;
    }
    
    return light.Color * specular;
}