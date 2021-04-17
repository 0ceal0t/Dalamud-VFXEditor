struct VS_IN
{
    float4 pos : POSITION;
    float4 col : COLOR;
};

struct GEO_IN
{
    float4 Position : SV_POSITION;
    float4 col : COLOR;
};

float4x4 worldViewProj;

GEO_IN VS(VS_IN input)
{
    GEO_IN output = (GEO_IN)0;

    output.Position = mul(input.pos, worldViewProj);
    output.col = input.col;

    return output;
}