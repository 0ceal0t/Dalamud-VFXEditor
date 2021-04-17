struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 norm : NORMAL;
};

float4 PS(PS_IN input) : SV_Target
{
    float3 LightPos = {0.0f, 1.0f, 0.0f};
    float3 Norm = normalize(input.norm.xyz);
    float3 WorldPos = input.pos.xyz;
    float3 LightDir = normalize(LightPos - WorldPos);

    float3 LightColor = { 1.0f, 1.0f, 1.0f};
    float3 ObjectColor = { 0.0f, 1.0f, 0.0f };
    float3 Ambient = { 0.7f, 0.7f, 0.7f };
    float Diffuse = saturate(dot(Norm, -LightDir));
    float3 Result = saturate((Ambient + (LightColor * Diffuse * 0.6f)) * ObjectColor);

    float4 Out_Col = { Result.x, Result.y, Result.z, 1.0f };
    return Out_Col;
}