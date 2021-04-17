struct VS_IN
{
    float4 pos : POSITION;
    float4 uv : UV;
    float4 norm : NORMAL;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 uv : UV;
    float4 norm : NORMAL;
};

float4x4 worldViewProj;

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN)0;

    output.pos = mul(input.pos, worldViewProj);
    output.uv = input.uv;
    output.norm = input.norm;

    return output;
}