#include"ModelUtils.fx"
#include"MaterialUtils.fx"

struct VS_IN
{
    float4 pos : POSITION;
    float4 tangent : TANGENT;
    float4 uv : UV;
    float4 norm : NORMAL;
    float4 color : COLOR;
};

struct PS_IN
{
    float4 Position : SV_POSITION;
    float2 TexCoords : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 WorldPos : TEXCOORD2;
    float3 ViewDirection: CAMERAPOS;
    float3 LightPos : LIGHTPOS;
    float3 Tangent : TANGENT;
    float3 Bitangent : BITANGENT;
    float3 Color : COLOR;
};

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN)0;
    
    float4 worldPosition = mul(ModelMatrix, float4(input.pos.xyz, 1.0f));
    float4 viewModelPosition = mul(ViewMatrix, worldPosition);
    output.Position = mul(ProjectionMatrix, viewModelPosition);
    output.WorldPos = worldPosition.xyz;
    
    float3x3 normalMatrix = transpose((float3x3) NormalMatrix);
    output.Normal = normalize(mul(normalMatrix, input.norm.xyz));
    output.Tangent = normalize(mul(normalMatrix, input.tangent.xyz));
    output.Bitangent = normalize(cross(output.Normal, output.Tangent)); // Bitangent calculated here <---
    
    output.LightPos = LightPos;
    output.ViewDirection = ViewDirection;
    
    output.TexCoords = input.uv.xy;
    output.Color = input.color.xyz;

    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    float3 lightDir = input.LightPos - input.WorldPos;
    float lightDistance = length(lightDir);
    
    float falloff = attenuation(Radius, Falloff, lightDistance);
    
    float3 L = normalize(lightDir);
    float3 V = normalize(input.ViewDirection - input.WorldPos);
    
    float3 tangent = normalize(input.Tangent);
    float3 biTangent = normalize(input.Bitangent);

    float3 N = normalize(input.Normal);
    
    float specularStrength = SpecularPower;
    float specularExponent = SpecularIntensity * 10;
    float df = dot(-lightDir, N);
    float3 specular = float3(0, 0, 0);
    if (df < 0.0f) // Idk what's happening at this point
    {
        specular += LightColor * phongSpecular(L, V, N, specularExponent) * specularStrength * falloff;
    }
    
    float3 diffuse = LightColor * orenNayarDiffuse(L, V, N, Roughness, Albedo) * falloff;
    
    float3 color = float3(0, 0, 0);
    color += EmissiveColor;
    color += DiffuseColor * input.Color * (diffuse + AmbientColor); // <--- TODO: change to UV if necessary
    color += SpecularColor * (specular);
    
    color = toGamma(color);
    
    return float4(saturate(color), 1);
}