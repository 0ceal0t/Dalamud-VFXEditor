#include"MaterialUtils.fx"

GBUFFER PS(PS_IN input)
{
    float3 geometryN = normalize(input.Normal);
    float3 N = applyBump(sampleTileBump(input.TexCoords), input.Normal, input.Tangent, input.Bitangent);

    // Only the key light is used for diffuse, highlights come from its softbox in the environment
    float3 diffuse = computeDiffuse(Light1, input.WorldPos, N);

    float3 albedo = DiffuseColor * sampleTileOrb(input.TexCoords);

    float3 color = float3(0, 0, 0);
    color += EmissiveColor;
    color += albedo * diffuse * 0.6f;
    color += computeReflection(input.WorldPos, N, geometryN);

    return writeGBuffer(input, N, color, albedo * computeHemisphereAmbient(N));
}
