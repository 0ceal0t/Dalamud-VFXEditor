struct PS_IN
{
    float4 Position : SV_POSITION;
    float4 col : COLOR;
};

struct GEO_IN
{
    float4 Position : SV_POSITION;
    float4 col : COLOR;
};

cbuffer cbChangeRare
{
    float4 RenderTargetSize;
}

[maxvertexcount(6)]
void GS(line GEO_IN points[2], inout TriangleStream<PS_IN> output)
{
    float4 p0 = points[0].Position;
    float4 p1 = points[1].Position;
    float4 col = points[0].col;

    // ================
    float2 currentScreen = p0.xy / p0.w;
    float2 nextScreen = p1.xy / p1.w;

    float aspect = RenderTargetSize.x / RenderTargetSize.y;

    currentScreen.x *= aspect;
    nextScreen.x *= aspect;

    float2 dir = normalize(nextScreen - currentScreen);
    float2 normal = { -dir.y, dir.x };

    float thickness = 0.02;
    normal *= thickness / 2.0f;
    normal.x /= aspect;

    float4 offset = { normal.x, normal.y, 0, 0 };
    
    PS_IN v[4];
    v[0].col = col;
    v[1].col = col;
    v[2].col = col;
    v[3].col = col;

    v[0].Position = p0 - offset;
    v[1].Position = p0 + offset;
    v[2].Position = p1 + offset;
    v[3].Position = p1 - offset;

    output.Append(v[2]);
    output.Append(v[1]);
    output.Append(v[0]);

    output.RestartStrip();

    output.Append(v[3]);
    output.Append(v[2]);
    output.Append(v[0]);

    output.RestartStrip();
}