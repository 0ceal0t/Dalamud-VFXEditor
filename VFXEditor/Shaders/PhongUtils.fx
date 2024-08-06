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
    
    float4x4 InvViewMatrix;
    float4x4 InvProjectionMatrix;
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
    L = normalize(L);
    float NdotL = dot(N, L);
    float3 R = reflect(-L, N);

    if (NdotL > 0)
    {
        float3 V = EyePosition - worldPos;
        V = normalize(V);
        float RdotV = dot(R, V);
        return light.Color * pow(max(0.0f, RdotV), SpecularPower * 10) * SpecularIntensity;
    }
    return 0;
}