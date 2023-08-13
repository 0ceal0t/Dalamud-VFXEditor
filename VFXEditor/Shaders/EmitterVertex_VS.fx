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

cbuffer globalBuffer : register(b0)
{
    float4x4 worldViewProj;
    int showEdges;
}

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN)0;

    float4x4 i = float4x4(input.ir0, input.ir1, input.ir2, input.ir3);
    float4x4 m = mul(i, worldViewProj);

    output.pos = mul(input.pos, m);
    output.norm = input.norm;

    return output;
}