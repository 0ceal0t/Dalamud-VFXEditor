#include"ModelUtils.fx"
#include"MaterialUtils.fx"

struct VS_IN
{
    float4 pos : POSITION;
    float4 tangent : TANGENT;
    float4 uv : UV;
    float4 norm : NORMAL;
    float4 color : COLOR;
};

struct PS_IN
{
    float4 Position : SV_POSITION;
    float2 TexCoords : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 WorldPos : TEXCOORD2;
    float3 Tangent : TANGENT;
    float3 Bitangent : BITANGENT;
    float3 Color : COLOR;
};

struct GBUFFER
{
    float4 Position : SV_Target0;
    float4 Normal : SV_Target1;
    float4 Color : SV_Target2;
    float4 UV : SV_Target3;
};

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
    output.Bitangent = normalize(cross(output.Normal, output.Tangent)); // Bitangent calculated here <---
    
    output.TexCoords = input.uv.xy;
    output.Color = input.color.xyz;

    return output;
}


GBUFFER PS(PS_IN input)
{
    float3 tangent = normalize(input.Tangent);
    float3 biTangent = normalize(input.Bitangent);
    float3 N = normalize(input.Normal);
    
    float3 specular = computeSpecular(Light1, input.WorldPos, N) + computeSpecular(Light2, input.WorldPos, N);
    
    float3 diffuse = computeDiffuse(Light1, input.WorldPos, N) + computeDiffuse(Light2, input.WorldPos, N);
    
    float3 color = float3(0, 0, 0);
    color += EmissiveColor;
    color += AmbientColor;
    color += DiffuseColor * input.Color * diffuse * 0.6f;
    color += SpecularColor * input.Color * specular * 0.5f;
    
    color = toGamma(color);
    
    float4 finalColor = float4(saturate(color), 1);
    
    GBUFFER result = (GBUFFER) 0;
    result.Position = input.Position;
    result.Normal = float4(N, 1);
    result.Color = finalColor;
    result.UV = float4(input.TexCoords, 0, 0);

    return result;
}