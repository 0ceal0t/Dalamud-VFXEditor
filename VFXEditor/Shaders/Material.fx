#include"ModelBuffers.fx"

cbuffer VSMaterialConstants : register(b1)
{
    float3 CameraPos;
    float3 LightPos;
}

cbuffer PSMaterialConstants : register(b1)
{
    float3 LightDiffuseColor;
    float3 LightSpecularColor;

    float3 DiffuseColor;
    float3 AmbientLightColor;
    float3 EmissiveColor;
    float3 SpecularColor;

    float SpecularPower;
    float SpecularIntensity;
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
  float3 CameraPos: CAMERAPOS;
  float3 LightPos : LIGHTPOS;
};

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN)0;

    output.WorldPos = mul(float4(input.pos.xyz, 1.0f), World).xyz;
    output.Position = mul(float4(output.WorldPos, 1.0f), ViewProjection);
    output.Normal = normalize(mul(input.norm.xyz, (float3x3)World));
    output.TexCoords = input.uv.xy;

    output.CameraPos = mul(CameraPos, (float3x3)World);
    output.LightPos = mul(LightPos, (float3x3)World);

    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    // Get light direction for this fragment
    float3 lightDir = normalize(input.LightPos - input.WorldPos);

    float diffuseLighting = saturate(dot(input.Normal, -lightDir)); // per pixel diffuse lighting

    diffuseLighting *= ((length(lightDir) * length(lightDir)) / dot(input.LightPos - input.WorldPos, input.LightPos - input.WorldPos));

    float3 h = normalize(normalize(input.CameraPos - input.WorldPos) - lightDir);
    float specLighting = pow(saturate(dot(h, input.Normal)), 2.0f);

    return float4(saturate(AmbientLightColor + (DiffuseColor * LightDiffuseColor * 0.6f) + (specLighting * 0.5f)), 1);
}