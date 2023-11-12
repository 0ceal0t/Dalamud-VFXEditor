#include"ModelBuffers.fx"

cbuffer VSMaterialConstants : register(b1)
{
    float3 CameraPos;
    float3 LightPos;
}

cbuffer PSMaterialConstants : register(b1)
{
    float3 LightDiffuseColor;
    float3 LightSpecularColor;

    float3 DiffuseColor;
    float3 AmbientLightColor;
    float3 EmissiveColor;
    float3 SpecularColor;

    float SpecularPower;
    float SpecularIntensity;
}

struct VS_IN
{
    float4 pos : POSITION;
    float4 uv : UV;
    float4 norm : NORMAL;
};

struct PS_IN
{
  float4 Position : SV_POSITION;
  float2 TexCoords : TEXCOORD0;
  float3 Normal : TEXCOORD1;
  float3 WorldPos : TEXCOORD2;
  float3 CameraPos: CAMERAPOS;
  float3 LightPos : LIGHTPOS;
};

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN)0;

    output.WorldPos = mul(float4(input.pos.xyz, 1.0f), World).xyz;
    output.Position = mul(float4(output.WorldPos, 1.0f), ViewProjection);
    output.TexCoords = input.uv.xy;
    
    output.Normal = normalize(input.norm.xyz);
    output.CameraPos = CameraPos;
    output.LightPos = LightPos;

    return output;
}


float4 PS(PS_IN input) : SV_Target
{
    float3 cameraDir = normalize(-input.CameraPos);
    
    float ambientStrength = 0.1;
    float specularStrength = 0.9;
    float specularExponent = 100;
    
    float3 lightDirEye = input.LightPos - input.CameraPos;
    float inverseDistance = 1 / length(lightDirEye);
    lightDirEye *= inverseDistance; //normalise
    float3 lightColor = LightColor;

    float3 iAmbient = ambientStrength;

    float diffuseFactor = max(0.0, dot(input.Normal, lightDirEye));
    float3 iDiffuse = diffuseFactor;

    float3 halfwayEye = normalize(cameraDir + lightDirEye);
    float specularFactor = max(0.0, dot(halfwayEye, input.Normal));
    float3 iSpecular = specularStrength * pow(specularFactor, 2 * specularExponent);

    float3 pointLightIntensity = (iAmbient + iDiffuse + iSpecular) * lightColor * inverseDistance;
    
    float3 result = (pointLightIntensity) * DiffuseColor;

    return float4(result, 1.0);
}