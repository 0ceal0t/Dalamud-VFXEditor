#include"MaterialUtils.fx"

GBUFFER PS(PS_IN input)
{
    float3 geometryN = normalize(input.Normal);
    float3 bump = sampleTileBump(input.TexCoords);

    float3 N = normalize(lerp(geometryN, applyBump(bump, input.Normal, input.Tangent, input.Bitangent), TileAlpha));
    float3 viewN = normalize(lerp(normalize(input.ViewNormal), applyBump(bump, input.ViewNormal, input.ViewTangent, input.ViewBitangent), TileAlpha));

    float3 albedo = DiffuseColor * lerp(1.0f, sampleTileOrb(input.TexCoords), TileAlpha);

    float3 V = normalize(EyePosition - input.WorldPos);
    float NdotV = saturate(dot(N, V));
    float roughness = clamp(Roughness, 0.03f, 1.0f);
    float metalness = saturate(Metalness);

    float3 F0 = lerp(float3(0.04f, 0.04f, 0.04f), albedo, metalness) * SpecularColor;
    float3 diffuseAlbedo = albedo * (1.0f - metalness);

    float3 T = normalize(input.Tangent - dot(input.Tangent, N) * N);
    float3 anisoTangent = cross(T, V);
    float3 anisoNormal = cross(anisoTangent, T);
    float3 reflectionN = normalize(lerp(N, anisoNormal, clamp(Anisotropy, -1.0f, 1.0f) * (1.0f - roughness) * 0.8f));

    float3 color = float3(0, 0, 0);
    color += EmissiveColor;
    color += diffuseAlbedo * computeDiffuse(Light1, input.WorldPos, N) * 0.6f;

    float2 sphereUV = viewN.xy * float2(0.5f, -0.5f) + 0.5f;
    float3 sphere = SphereArray.Sample(Sampler, float3(sphereUV, SphereIndex)).xyz;
    float3 reflection = computeReflectionF0(input.WorldPos, reflectionN, geometryN, F0, roughness);
    color += reflection * lerp(float3(1, 1, 1), sphere * 2.0f, saturate(SphereMask));

    float3 sheenColor = SheenRate * lerp(float3(1, 1, 1), albedo, saturate(SheenTint));
    color += sheenColor * pow(1.0f - NdotV, max(SheenAperture, 0.1f)) * sampleStudio(N, 1.0f);

    return writeGBuffer(input, N, color, diffuseAlbedo * computeHemisphereAmbient(N));
}
