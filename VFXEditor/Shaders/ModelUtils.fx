cbuffer VSConstants : register(b0)
{
    float4x4 ModelMatrix;
    float4x4 ViewMatrix;
    float4x4 ProjectionMatrix;
    float4x4 ViewProjectionMatrix;
    float4x4 NormalMatrix;
    float4x4 CubeMatrix;
}

cbuffer PSConstants : register(b0)
{
    int ShowEdges;
    float2 Size;
    float3 CameraPosition;
}