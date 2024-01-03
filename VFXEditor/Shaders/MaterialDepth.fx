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

float4 VS(VS_IN input) : SV_POSITION
{
    float4 worldPosition = mul(ModelMatrix, float4(input.pos.xyz, 1.0f));
    float4 viewModelPosition = mul(ViewMatrix, worldPosition);
    return mul(ProjectionMatrix, viewModelPosition);
}