#include"ModelUtils.fx"
#include"PhongUtils.fx"

cbuffer MaterialParams : register(b2)
{
    float TileIndex;
    float TileAlpha;
    float SphereIndex;
    float SphereMask;

    float Metalness;
    float Anisotropy;
    float SheenRate;
    float SheenTint;

    float SheenAperture;
    float3 _ParamsPad;
}

struct VS_IN
{
    float4 pos : POSITION;
    float4 tangent : TANGENT;
    float4 bitangent : BITANGENT;
    float4 uv : UV;
    float4 norm : NORMAL;
};

struct PS_IN
{
    float4 Position : SV_POSITION;
    float2 TexCoords : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 WorldPos : TEXCOORD2;
    float3 Tangent : TANGENT;
    float3 Bitangent : BITANGENT;
    float3 ViewNormal : TEXCOORD3;
    float3 ViewTangent : TEXCOORD4;
    float3 ViewBitangent : TEXCOORD5;
};

struct GBUFFER
{
    float4 Position : SV_Target0;
    float4 Normal : SV_Target1;
    float4 Color : SV_Target2;
    float4 UV : SV_Target3;
    float4 Ambient : SV_Target4;
};

SamplerState Sampler : register(s0);
Texture2DArray TileDiffuseArray : register(t0);
Texture2DArray TileNormalArray : register(t1);
Texture2DArray SphereArray : register(t2);

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN) 0;

    float4 worldPosition = mul(float4(input.pos.xyz, 1.0f), ModelMatrix);
    float4 viewModelPosition = mul(worldPosition, ViewMatrix);
    output.Position = mul(viewModelPosition, ProjectionMatrix);
    output.WorldPos = worldPosition.xyz;

    float3x3 normalMatrix = transpose((float3x3) NormalMatrix);
    output.Normal = mul(normalMatrix, normalize(input.norm.xyz));
    output.Tangent = mul(normalMatrix, normalize(input.tangent.xyz));
    output.Bitangent = mul(normalMatrix, normalize(input.bitangent.xyz));

    output.ViewNormal = mul(float4(output.Normal, 0), ViewMatrix).xyz;
    output.ViewTangent = mul(float4(output.Tangent, 0), ViewMatrix).xyz;
    output.ViewBitangent = mul(float4(output.Bitangent, 0), ViewMatrix).xyz;

    output.TexCoords = input.uv.xy * Repeat + (float2(input.uv.y, input.uv.x) * Skew);

    return output;
}

float3 sampleTileBump(float2 uv)
{
    float2 bumpXY = mad(2.0f, TileNormalArray.Sample(Sampler, float3(uv, TileIndex)).xy, -1.0f) * float2(1.0f, -1.0f);
    return float3(bumpXY, sqrt(saturate(1.0f - dot(bumpXY, bumpXY))));
}

float sampleTileOrb(float2 uv)
{
    return TileDiffuseArray.Sample(Sampler, float3(uv, TileIndex)).r;
}

float3 applyBump(float3 bump, float3 N, float3 T, float3 B)
{
    N = normalize(N);
    T = normalize(T - dot(T, N) * N);
    B = normalize(B - dot(B, N) * N);
    return normalize(bump.x * T + bump.y * B + bump.z * N);
}

GBUFFER writeGBuffer(PS_IN input, float3 N, float3 color, float3 ambient)
{
    GBUFFER result = (GBUFFER) 0;
    result.Position = float4(input.WorldPos, 1);
    result.Normal = (float4(N, 1) + 1.0f) / 2.0f;
    result.Color = float4(color, 1);
    result.UV = float4(input.TexCoords, 0, 0);
    result.Ambient = float4(ambient, 1);
    return result;
}
