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

float3 computeHemisphereAmbient(float3 N)
{
    return AmbientColor * lerp(0.25f, 1.75f, N.y * 0.5f + 0.5f);
}

float glossToRoughness()
{
    return sqrt(2.0f / (SpecularPower * 10.0f + 2.0f));
}

float softbox(float3 dir, float3 boxDir, float size, float blur)
{
    float edge = 0.01f + blur * 0.3f;
    return smoothstep(size - edge, size + edge, dot(dir, boxDir)) * lerp(1.0f, 0.35f, blur);
}

float3 sampleStudio(float3 dir, float blur)
{
    // Wide and low contrast, a sharp horizon shows up as rings on a sphere
    float spread = blur * 0.5f;
    float3 color = lerp(float3(0.1f, 0.1f, 0.11f), float3(0.4f, 0.42f, 0.45f), smoothstep(-0.7f - spread, 0.9f + spread, dir.y));

    color += 2.0f * softbox(dir, normalize(Light1.Position), 0.94f, blur) * Light1.Color;
    color += 1.2f * softbox(dir, normalize(float3(-0.7f, 0.4f, 0.6f)), 0.95f, blur);
    color += 1.5f * softbox(dir, normalize(float3(0.3f, 0.4f, -0.9f)), 0.96f, blur);
    return color;
}

float3 computeReflection(float3 worldPos, float3 N)
{
    float3 V = normalize(EyePosition - worldPos);
    float3 R = reflect(-V, N);
    float roughness = glossToRoughness();

    // Schlick, damped for rough surfaces so they don't get a bright rim
    // Legacy specular is a highlight tint rather than a real F0, so scale it down
    float3 F0 = saturate(SpecularColor * SpecularIntensity) * 0.5f;
    float NdotV = saturate(dot(N, V));
    float3 F = F0 + (max(1.0f - roughness, F0) - F0) * pow(1.0f - NdotV, 5.0f);

    return sampleStudio(R, roughness) * F;
}
