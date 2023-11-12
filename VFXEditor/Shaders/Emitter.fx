#include"ModelBuffers.fx"

struct VS_IN
{
    float4 pos : POSITION;
    float4 norm : NORMAL;
    float4 ir0 : INSTANCE0;
    float4 ir1 : INSTANCE1;
    float4 ir2 : INSTANCE2;
    float4 ir3 : INSTANCE3;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 norm : NORMAL;
};

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN) 0;

    float4x4 instanceMatrix = float4x4(input.ir0, input.ir1, input.ir2, input.ir3);
    float4x4 finalMatrix = mul(instanceMatrix, mul(ModelMatrix, ViewProjectionMatrix));

    output.pos = mul(input.pos, finalMatrix);
    output.norm = input.norm;

    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    float3 lightPos = { 0.0f, 1.0f, 0.0f };
    float3 norm = normalize(input.norm.xyz);
    float3 worldPos = input.pos.xyz;
    float3 lightDir = normalize(lightPos - worldPos);

    float3 lightColor = { 1.0f, 1.0f, 1.0f };
    float3 color = { 0.0f, 1.0f, 0.0f };
    float3 ambient = { 0.7f, 0.7f, 0.7f };
    float diffuse = saturate(dot(norm, -lightDir));
    float3 result = saturate((ambient + (lightColor * diffuse * 0.6f)) * color);

    return float4(result, 1);

}