struct VS_IN
{
    float4 pos : POSITION;
    float4 col : COLOR;
    float4 norm : NORMAL;
};

struct GS_IN
{
    float4 PositionCS 		    : POSITIONCS;
    float3 PositionWS 		    : POSITIONWS;
    float3 NormalWS 		    : NORMALWS;
    float DepthVS			    : DEPTHVS;
    float4 col : COLOR;
};

struct PS_IN
{
    float4 PositionCS 		    : SV_POSITION;
    float3 PositionWS 		    : POSITIONWS;
    float3 NormalWS 		    : NORMALWS;
    float DepthVS			    : DEPTHVS;
    float4 col : COLOR;
    float2 barycentricCoordinates : TEXCOORD9;
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

GS_IN VS(VS_IN input)
{
    GS_IN output = (GS_IN)0;

    output.PositionWS = mul(float4(input.pos.xyz, 1.0f), World).xyz;
    output.PositionCS = mul(float4(output.PositionWS, 1.0f), ViewProjection);
    output.DepthVS = output.PositionCS.w;
    output.NormalWS = normalize(mul(input.norm.xyz, (float3x3)World));
    output.col = input.col;

    return output;
}

[maxvertexcount(3)]
void GS( triangle GS_IN input[3], inout TriangleStream<PS_IN> OutputStream )
{   
    PS_IN g0 = (PS_IN)0;
    PS_IN g1 = (PS_IN)0;
    PS_IN g2 = (PS_IN)0;

    g0.PositionWS = input[0].PositionWS;
    g1.PositionWS = input[1].PositionWS;
    g2.PositionWS = input[2].PositionWS;

    g0.PositionCS = input[0].PositionCS;
    g1.PositionCS = input[1].PositionCS;
    g2.PositionCS = input[2].PositionCS;

    g0.DepthVS = input[0].DepthVS;
    g1.DepthVS = input[1].DepthVS;
    g2.DepthVS = input[2].DepthVS;

    g0.NormalWS = input[0].NormalWS;
    g1.NormalWS = input[1].NormalWS;
    g2.NormalWS = input[2].NormalWS;

    g0.col = input[0].col;
    g1.col = input[1].col;
    g2.col = input[2].col;

    g0.barycentricCoordinates = float2(1, 0);
    g1.barycentricCoordinates = float2(0, 1);
    g2.barycentricCoordinates = float2(0, 0);
    
    OutputStream.Append( g0 );
    OutputStream.Append( g1 );
    OutputStream.Append( g2 );

    OutputStream.RestartStrip();
}

float4 PS(PS_IN input) : SV_Target
{
    float3 LightPos = {0.0f, 1.0f, 0.0f};
    float3 Norm = normalize(input.NormalWS.xyz);
    float3 WorldPos = input.PositionWS.xyz;
    float3 LightDir = normalize(LightPos - WorldPos);

    float3 lightColor = { 1.0f, 1.0f, 1.0f};
    float3 ObjectColor = input.col.xyz;
    float3 Ambient = { 0.7f, 0.7f, 0.7f };
    float Diffuse = saturate(dot(Norm, -LightDir));
    float3 Result = saturate((Ambient + (lightColor * Diffuse * 0.6f)) * ObjectColor);

    if(ShowEdges == 1)
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