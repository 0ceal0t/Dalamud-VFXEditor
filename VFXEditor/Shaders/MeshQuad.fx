#include"ModelUtils.fx"
#include"MaterialUtils.fx"

struct VS_IN
{
    float4 pos : POSITION;
};

struct PS_IN
{
    float4 Position : SV_POSITION;
    float4 UV : TEXCOORD0;
};

SamplerState Sampler : register(s0);

Texture2D DepthTexture : register(t0);
Texture2D PositionTexture : register(t1);
Texture2D NormalTexture : register(t2);
Texture2D ColorTexture : register(t3);
Texture2D UVTexture : register(t4);

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN) 0;
    output.Position = float4(input.pos.xyz, 1);
    output.UV.x = output.Position.x;
    output.UV.y = output.Position.y;

    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    float2 pos = input.Position.xy / Size;
    return float4(ColorTexture.Sample(Sampler, pos).xyz, 1);
}