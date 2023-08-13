struct VS_IN
{
    float4 pos : POSITION;
    float4 col : COLOR;
    float4 norm : NORMAL;
};

struct GS_IN
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
    float2 barycentricCoordinates : TEXCOORD9;
};

cbuffer globalBuffer : register(b0)
{
    float4x4 worldViewProj;
    int showEdges;
}

GS_IN VS(VS_IN input)
{
    GS_IN output = (GS_IN)0;

    output.pos = mul(input.pos, worldViewProj);
    output.col = input.col;
    output.norm = input.norm;

    return output;
}

[maxvertexcount(3)]
void GS( triangle GS_IN input[3], inout TriangleStream<PS_IN> OutputStream )
{   
    PS_IN g0 = (PS_IN)0;
    g0.pos = input[0].pos;
    g0.norm = input[0].norm;
    g0.col = input[0].col;
    g0.barycentricCoordinates = float2(1, 0);
    OutputStream.Append( g0 );

    PS_IN g1 = (PS_IN)0;
    g1.pos = input[1].pos;
    g1.norm = input[1].norm;
    g1.col = input[1].col;
    g1.barycentricCoordinates = float2(0, 1);
    OutputStream.Append( g1 );

    PS_IN g2 = (PS_IN)0;
    g2.pos = input[2].pos;
    g2.norm = input[2].norm;
    g2.col = input[2].col;
    g2.barycentricCoordinates = float2(0, 0);
    OutputStream.Append( g2 );
    
    OutputStream.RestartStrip();
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

    if(showEdges == 1)
    {
        float3 barys;
        barys.xy = input.barycentricCoordinates;
        barys.z = 1 - barys.x - barys.y;
        float3 deltas = fwidth(barys);
        float3 smoothing = deltas * 0.5f;
	    float3 thickness = deltas * 0.1f;
	    barys = smoothstep(thickness, thickness + smoothing, barys);
	    float minBary = min(barys.x, min(barys.y, barys.z));
        float3 wireframeColor = { 1.0f, 0.0f, 0.0f };

        Result = lerp(wireframeColor, Result, minBary);
    }

    float4 Out_Col = { Result.x, Result.y, Result.z, 1.0f };
    return Out_Col;
}