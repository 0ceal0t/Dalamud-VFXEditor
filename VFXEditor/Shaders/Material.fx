#include"ModelBuffers.fx"

cbuffer VSMaterialConstants : register(b1)
{
    float3 CameraPos;
    float3 LightPos;
}

cbuffer PSMaterialConstants : register(b1)
{
    float3 DiffuseColor;
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
    float lightBoost = 5;
    float lightAmbient = 0.3f;
    
    float3 lightDir = input.LightPos - input.WorldPos; //3D position in space of the surface
    float distance = length(lightDir);
    lightDir = lightDir / distance; // = normalize(lightDir);
    distance = distance * distance; //This line may be optimised using Inverse square root

		//Intensity of the diffuse light. Saturate to keep within the 0-1 range.
    float NdotL = dot(input.Normal, lightDir);
    float intensity = saturate(NdotL);

		// Calculate the diffuse light factoring in light color, power and the attenuation
    float3 diffuse = DiffuseColor * (lightAmbient + (intensity * lightBoost) / distance);

		//Calculate the half vector between the light vector and the view vector.
		//This is typically slower than calculating the actual reflection vector
		// due to the normalize function's reciprocal square root
    float3 H = normalize(lightDir + input.CameraPos);

		//Intensity of the specular light
    float NdotH = dot(input.Normal, H);
    
    float specularStrength = SpecularPower;
    float specularExponent = SpecularIntensity * 10;
    intensity = specularStrength * pow(saturate(NdotH), 2 * specularExponent);

		//Sum up the specular light factoring
    float3 specular = SpecularColor * (lightAmbient + (intensity * lightBoost) / distance);
    
    float3 combined = EmissiveColor + specular + diffuse ;
    return float4(saturate(combined), 1);
    
    /*
    float3 dir = normalize(-input.WorldPos);
    
    float specularStrength = SpecularPower;
    float specularExponent = SpecularIntensity * 10;
    
    float3 lightDirEye = input.LightPos - input.WorldPos;
    float inverseDistance = 1 / length(lightDirEye);
    lightDirEye *= inverseDistance; //normalise
    float3 lightColor = LightColor;

    float3 iAmbient = AmbientLightColor;

    float diffuseFactor = max(0.0, dot(input.Normal, lightDirEye));
    float3 iDiffuse = diffuseFactor;

    float3 halfwayEye = normalize(dir + lightDirEye);
    float specularFactor = max(0.0, dot(halfwayEye, input.Normal));
    float3 iSpecular = specularStrength * pow(specularFactor, 2 * specularExponent);

    float3 pointLightIntensity = (iAmbient + iDiffuse + iSpecular) * lightColor * inverseDistance;
    
    float3 result = (pointLightIntensity) * DiffuseColor;

    return float4(result, 1.0);
    */
}