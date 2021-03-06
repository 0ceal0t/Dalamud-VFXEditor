struct VS_IN
{
    float4 pos : POSITION;
    float4 col : COLOR;
    float4 norm : NORMAL;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 col : COLOR;
    float4 norm : NORMAL;
};

float4x4 worldViewProj;

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN)0;

    output.pos = mul(input.pos, worldViewProj);
    output.col = input.col;
    output.norm = input.norm;

    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    float3 LightPos = {0.0f, 1.0f, 0.0f};
    float3 Norm = normalize(input.norm.xyz);
    float3 WorldPos = input.pos.xyz;
    float3 LightDir = normalize(LightPos - WorldPos);

    float3 LightColor = { 1.0f, 1.0f, 1.0f};
    float3 ObjectColor = input.col.xyz;
    float3 Ambient = { 0.7f, 0.7f, 0.7f };
    float Diffuse = saturate(dot(Norm, -LightDir));
    float3 Result = saturate((Ambient + (LightColor * Diffuse * 0.6f)) * ObjectColor);

    float4 Out_Col = { Result.x, Result.y, Result.z, 1.0f };
    return Out_Col;
}