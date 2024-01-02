#include"ModelUtils.fx"
#include"MaterialUtils.fx"

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
    output.Tangent = normalize(mul(normalMatrix, input.tangent.xyz));
    output.Bitangent = normalize(mul(normalMatrix, input.bitangent.xyz));
    
    output.TexCoords = input.uv.xy * Repeat + (float2(input.uv.y, input.uv.x) * Skew);

    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    float3 tangent = normalize(input.Tangent);
    float3 biTangent = normalize(input.Bitangent);
    float3 bumpMap = Normaltexture.Sample(SamplerSurface, input.TexCoords).xyz;
    float3 N = normalize(input.Normal);
    bumpMap = mad(2.0f, bumpMap, -1.0f);
    N += mad(bumpMap.z, tangent, bumpMap.y * biTangent);
    N = normalize(N);
    
    float3 specular = computeSpecular(Light1, input.WorldPos, N) + computeSpecular(Light2, input.WorldPos, N);
    
    float3 sampledDiffuse = DiffuseTexture.Sample(SamplerSurface, input.TexCoords).xyz; 
    float3 diffuse = computeDiffuse(Light1, input.WorldPos, N) + computeDiffuse(Light2, input.WorldPos, N);
    
    float3 color = float3(0, 0, 0);
    color += EmissiveColor;
    color += AmbientColor;
    color += DiffuseColor * sampledDiffuse * diffuse * 0.6f;
    color += SpecularColor * specular * 0.5f;
    
    color = toGamma(color);
    
    return float4(saturate(color), 1);
}