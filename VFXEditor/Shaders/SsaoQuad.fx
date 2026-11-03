#include"ModelUtils.fx"
#include"PhongUtils.fx"

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

Texture2D<float4> DepthTexture : register(t0);
Texture2D<float4> PositionTexture : register(t1);
Texture2D<float4> NormalTexture : register(t2);
Texture2D<float4> ColorTexture : register(t3);
Texture2D<float4> UVTexture : register(t4);
Texture2D<float4> AmbientTexture : register(t5);

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN) 0;
    output.Position = float4(input.pos.xyz, 1);
    output.UV.x = output.Position.x;
    output.UV.y = output.Position.y;

    return output;
}

// ===================

#define DISK_SAMPLE_COUNT (16)
static float2 poissonDisk[16] =
{
    float2(0.2770745f, 0.6951455f),
    float2(0.1874257f, -0.02561589f),
    float2(-0.3381929f, 0.8713168f),
    float2(0.5867746f, 0.1087471f),
    float2(-0.3078699f, 0.188545f),
    float2(0.7993396f, 0.4595091f),
    float2(-0.09242552f, 0.5260149f),
    float2(0.3657553f, -0.5329605f),
    float2(-0.3829718f, -0.2476171f),
    float2(-0.01085108f, -0.6966301f),
    float2(0.8404155f, -0.3543923f),
    float2(-0.5186161f, -0.7624033f),
    float2(-0.8135794f, 0.2328489f),
    float2(-0.784665f, -0.2434929f),
    float2(0.9920505f, 0.0855163f),
    float2(-0.687256f, 0.6711345f)
};

float doAmbientOcclusion(int2 pixel, float3 pos, float3 norm)
{
    float4 samplePos = PositionTexture.Load(int3(pixel, 0));
    if (samplePos.w == 0.0f) return 0.0f; // Background

    float3 diff = samplePos.xyz - pos;
    const float3 v = normalize(diff);
    const float d = length(diff) * 2.0f;
    return max(0.0, dot(norm, v) - 0.2f) * (1.0f / (1.0f + d)) * 1.5f;
}

float4 PS(PS_IN input) : SV_Target
{
    int2 pixel = int2(input.Position.xy);
    float4 normal = NormalTexture.Load(int3(pixel, 0));
    float3 color = ColorTexture.Load(int3(pixel, 0)).xyz;
    
    if (normal.w > 0.0f)
    {
        float3 position = PositionTexture.Load(int3(pixel, 0)).xyz;
        float3 N = normalize(mad(2.0f, normal.xyz, -1.0f));

        float ao = 0.0f;
        
        int sampleCount = DISK_SAMPLE_COUNT;
        float sampleRadius = 8.0f;
        int2 maxPixel = int2(Size) - 1;
        
        for (int i = 0; i < DISK_SAMPLE_COUNT; i++)
        {
            int2 samplePixel = clamp(pixel + int2(poissonDisk[i] * sampleRadius), int2(0, 0), maxPixel);
            ao = ao + doAmbientOcclusion(samplePixel, position, N);
        }

        ao = 1.0f - abs(ao / float(sampleCount));
        ao = saturate(pow(ao, 6.0f));
        
        color += AmbientTexture.Load(int3(pixel, 0)).xyz * ao;
    }
    
    color = toGamma(color);
    return float4(saturate(color), 1);
}
