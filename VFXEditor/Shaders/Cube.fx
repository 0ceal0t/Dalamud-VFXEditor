#include"ModelBuffers.fx"

struct VS_IN
{
    float4 pos : POSITION;
    float4 col : COLOR;
    float4 norm : NORMAL;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 norm : NORMAL;
    float4 col : COLOR;
};

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN) 0;

    float4x4 finalMatrix = mul(World, CubeMatrix);

    output.pos = mul(input.pos, finalMatrix);
    output.norm = input.norm;
    output.col = input.col;

    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    return input.col;
}