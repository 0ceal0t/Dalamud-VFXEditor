struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 uv : UV;
    float4 norm : NORMAL;
};

float4x4 animData;

Texture2D ShaderTexture : register(t0);
SamplerState Sampler : register(s0);

float2 rotateUV(float2 uv, float rotation) {
    float mid = 0.5;
    float2 value = {
        cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
        cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
    };
    return value;
}

float2 moveUV(float2 uv, float2 scale, float2 scroll, float rot) {
    float2 mid = { 0.5, 0.5 };
    return mid + scale * (rotateUV(uv, rot) - mid) + scroll;
}

float4 PS(PS_IN input) : SV_Target
{
    float3 LightPos = {0.0f, 1.0f, 0.0f};
    float3 Norm = normalize(input.norm.xyz);
    float3 WorldPos = input.pos.xyz;
    float3 LightDir = normalize(LightPos - WorldPos);

    float3 LightColor = { 1.0f, 1.0f, 1.0f};

    float2 scale = {animData[0][0], animData[1][0]};
    float2 scroll = { animData[2][0], animData[3][0] };
    float rotation = animData[0][1];
    float2 uv = moveUV(input.uv.xy, scale, scroll, rotation);

    float3 ObjectColor = ShaderTexture.Sample(Sampler, uv).xyz;

    float3 Ambient = { 0.7f, 0.7f, 0.7f };
    float Diffuse = saturate(dot(Norm, -LightDir));
    float3 Result = saturate((Ambient + (LightColor * Diffuse * 0.6f)) * ObjectColor);

    float4 Out_Col = { Result.x, Result.y, Result.z, 1.0f };
    return Out_Col;
}