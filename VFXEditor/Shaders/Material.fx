#include"ModelBuffers.fx"

cbuffer PSMaterialConstants : register(b1)
{
    
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
};

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN)0;

    output.WorldPos = mul(float4(input.pos.xyz, 1.0f), World).xyz;
    output.Position = mul(float4(output.WorldPos, 1.0f), ViewProjection);
    output.Normal = normalize(mul(input.norm.xyz, (float3x3)World));
    output.TexCoords = input.uv.xy;

    return output;
}


float4 PS(PS_IN input) : SV_Target
{
    return float4( 1.0f, 0.0f, 0.0f, 1.0f );
}