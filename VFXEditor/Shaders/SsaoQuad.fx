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

Texture2D<float4> DepthTexture : register(t0);
Texture2D<float4> PositionTexture : register(t1);
Texture2D<float4> NormalTexture : register(t2);
Texture2D<float4> ColorTexture : register(t3);
Texture2D<float4> UVTexture : register(t4);

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN) 0;
    output.Position = float4(input.pos.xyz, 1);
    output.UV.x = output.Position.x;
    output.UV.y = output.Position.y;

    return output;
}

float doAmbientOcclusion(in float2 tcoord, in float2 uv, in float3 p, in float3 cnorm)
{
    float g_scale = 1.0f; // TODO
    float g_bias = 0.0f; // TODO
    float g_intensity = 10.0f; // TODO
    
    float3 diff = PositionTexture.Sample(Sampler, tcoord + uv).xyz - p;
    const float3 v = normalize(diff);
    const float d = length(diff) * g_scale;
    return max(0.0, dot(cnorm, v) - g_bias) * (1.0 / (1.0 + d)) * g_intensity;
}

float4 PS(PS_IN input) : SV_Target
{
    float2 coords = input.Position.xy / Size;
    float4 N = NormalTexture.Sample(Sampler, coords);
    
    float3 color = ColorTexture.Sample(Sampler, coords).xyz;
    
    if (N.w > 0.0f)
    {
        float3 position = PositionTexture.Sample(Sampler, coords);
        float3 normal = N.xyz * 2.0f - 1.0f;
        
        float2 rand = normalize(float2(0.5, 0.5)); // TODO
        float radius = 5.0f / position.z; // TODO
        
        const float2 vec[4] = { float2(1, 0), float2(-1, 0), float2(0, 1), float2(0, -1) };
        
        float ao = 0.0f;
        
        int iterations = 4;
        for (int j = 0; j < iterations; ++j)
        {
            float2 coord1 = reflect(vec[j], rand) * radius;
            float2 coord2 = float2(coord1.x * 0.707 - coord1.y * 0.707, coord1.x * 0.707 + coord1.y * 0.707);
            
            ao += doAmbientOcclusion(coords, coord1 * 0.25, position, normal);
            ao += doAmbientOcclusion(coords, coord2 * 0.5, position, normal);
            ao += doAmbientOcclusion(coords, coord1 * 0.75, position, normal);
            ao += doAmbientOcclusion(coords, coord2, position, normal);
        }
        
        ao /= (float) iterations * 4.0;
        
        color += AmbientColor * ao;

    }
    
    color = toGamma(color);
    return float4(saturate(color), 1);
}