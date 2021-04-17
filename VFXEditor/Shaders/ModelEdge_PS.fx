struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 col : COLOR;
};

float4 PS(PS_IN input) : SV_Target
{
    float4 Out_Col = { input.col.xyz, 1.0f };
    return Out_Col;
}