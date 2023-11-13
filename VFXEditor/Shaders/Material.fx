#include"ModelBuffers.fx"

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

struct VS_IN
{
    float4 pos : POSITION;
    float4 uv : UV;
    float4 norm : NORMAL;
};

struct PS_IN
{
  float4 Position : SV_POSITION;
  float2 TexCoords : TEXCOORD0;
  float3 Normal : TEXCOORD1;
  float3 WorldPos : TEXCOORD2;
  float3 ViewDirection: CAMERAPOS;
  float3 LightPos : LIGHTPOS;
};

SamplerState SamplerSurface : register(s0);
Texture2D DiffuseTexture : register(t0);
Texture2D Normaltexture : register(t1);

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN)0;
    
    float4 worldPosition = mul(ModelMatrix, float4(input.pos.xyz, 1.0f));
    float4 viewModelPosition = mul(ViewMatrix, worldPosition);
    output.Position = mul(ProjectionMatrix, viewModelPosition);
    output.WorldPos = worldPosition.xyz;
    
    float3x3 normalMatrix = transpose((float3x3) NormalMatrix);
    output.Normal = normalize(mul(normalMatrix, input.norm.xyz));
    
    output.LightPos = LightPos;
    output.ViewDirection = ViewDirection;
    output.TexCoords = input.uv.xy * Repeat;

    return output;
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

float4 PS(PS_IN input) : SV_Target
{
    float3 lightDir = input.LightPos - input.WorldPos;
    float lightDistance = length(lightDir);
    
    // radius, falloff, distance
    float falloff = attenuation(Radius, Falloff, lightDistance);
    
    float3 L = normalize(lightDir);
    float3 V = normalize(input.ViewDirection - input.WorldPos);
    float3 N = normalize(input.Normal);
    
    float specularStrength = SpecularPower;
    float specularExponent = SpecularIntensity * 10;
    
    float df = dot(-lightDir, N);
    
    float3 specular = float3(0, 0, 0);
    
    if (df < 0.0f) // Idk what's happening at this point
    {
        specular += LightColor * phongSpecular(L, V, N, specularExponent) * specularStrength * falloff;
    }
    
    float3 sampledDiffuse = DiffuseTexture.Sample(SamplerSurface, input.TexCoords).xyz;
    
    // roughness, abledo
    float3 diffuse = LightColor * orenNayarDiffuse(L, V, N, Roughness, Albedo) * falloff;
    
    float3 color = float3(0, 0, 0);
    color += EmissiveColor;
    color += DiffuseColor * sampledDiffuse * (diffuse + AmbientColor);
    color += SpecularColor * (specular);
    
    color = toGamma(color);
    
    return float4(saturate(color), 1);
}