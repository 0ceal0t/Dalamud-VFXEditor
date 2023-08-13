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

cbuffer VSConstants : register(b0)
{
    float4x4 World;
    float4x4 ViewProjection;
}

cbuffer PSConstants : register(b0)
{
    float4 LightDirection;
    float4 LightColor;
    int ShowEdges;
    
}

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN) 0;

    float4x4 instanceMatrix = float4x4(input.ir0, input.ir1, input.ir2, input.ir3);
    float4x4 finalMatrix = mul(instanceMatrix, mul(ViewProjection, World));

    output.pos = mul(input.pos, finalMatrix);
    output.norm = input.norm;

    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    float3 LightPos = { 0.0f, 1.0f, 0.0f };
    float3 Norm = normalize(input.norm.xyz);
    float3 WorldPos = input.pos.xyz;
    float3 LightDir = normalize(LightPos - WorldPos);

    float3 lightColor = { 1.0f, 1.0f, 1.0f };
    float3 ObjectColor = { 0.0f, 1.0f, 0.0f };
    float3 Ambient = { 0.7f, 0.7f, 0.7f };
    float Diffuse = saturate(dot(Norm, -LightDir));
    float3 Result = saturate((Ambient + (lightColor * Diffuse * 0.6f)) * ObjectColor);

    float4 Out_Col = { Result.x, Result.y, Result.z, 1.0f };
    return Out_Col;
}