#include"ModelUtils.fx"
#include"PhongUtils.fx"

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
Texture2D DiffuseTexture : register(t0);
Texture2D NormalTexture : register(t1);

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN)0;
    
    float4 worldPosition = mul(float4(input.pos.xyz, 1.0f), ModelMatrix);
    float4 viewModelPosition = mul(worldPosition, ViewMatrix);
    output.Position = mul(viewModelPosition, ProjectionMatrix);
    output.WorldPos = worldPosition.xyz;

    float3x3 normalMatrix = transpose((float3x3) NormalMatrix);
    output.Normal = mul(normalMatrix, normalize(input.norm.xyz));
    output.Tangent = mul(normalMatrix, normalize(input.tangent.xyz));
    output.Bitangent = mul(normalMatrix, normalize(input.bitangent.xyz));
    
    output.TexCoords = input.uv.xy * Repeat + (float2(input.uv.y, input.uv.x) * Skew);

    return output;
}


GBUFFER PS(PS_IN input)
{
    float3 N = normalize(input.Normal);
    float3 geometryN = N;
    float3 tangent = normalize(input.Tangent - dot(input.Tangent, N) * N);
    float3 biTangent = normalize(input.Bitangent - dot(input.Bitangent, N) * N);

    float2 bumpXY = mad(2.0f, NormalTexture.Sample(Sampler, input.TexCoords).xy, -1.0f) * float2(1.0f, -1.0f);
    float bumpZ = sqrt(saturate(1.0f - dot(bumpXY, bumpXY)));
    N = normalize(bumpXY.x * tangent + bumpXY.y * biTangent + bumpZ * N);
    
    // Only the key light is used for diffuse, highlights come from its softbox in the environment
    float3 sampledDiffuse = DiffuseTexture.Sample(Sampler, input.TexCoords).xyz;
    float3 diffuse = computeDiffuse(Light1, input.WorldPos, N);
    
    float3 albedo = DiffuseColor * sampledDiffuse;
    
    float3 color = float3(0, 0, 0);
    color += EmissiveColor;
    color += albedo * diffuse * 0.6f;
    color += computeReflection(input.WorldPos, N, geometryN);

    float4 finalColor = float4(color, 1);
    
    GBUFFER result = (GBUFFER) 0;
    result.Position = float4(input.WorldPos, 1);
    result.Normal = ( float4(N, 1) + 1.0f ) / 2.0f;
    result.Color = finalColor;
    result.UV = float4(input.TexCoords, 0, 0);
    result.Ambient = float4(albedo * computeHemisphereAmbient(N), 1);

    return result;
}