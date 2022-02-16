#if UNITY_EDITOR
using System;
using ThunderRoad;
using UnityEditor;
using UnityEngine;

public static class MaterialExtension
{
    private static readonly int Layer0 = Shader.PropertyToID("_Layer0");
    private static readonly int Layer1 = Shader.PropertyToID("_Layer1");
    private static readonly int Layer1NormalMap = Shader.PropertyToID("_Layer1NormalMap");
    private static readonly int UseReveal = Shader.PropertyToID("_UseReveal");

    public static MoesTexture CreateMoeTexture(this Material material)
    {
        try
        {
            var path = AssetDatabase.GetAssetPath(material);

            var moesTexture = new MoesTexture()
            {
                path = path.Substring(0, path.Length - 4),
                material = material,
                metallic = (Texture2D)material.GetTexture("_MetallicGlossMap"),
                occlusion = (Texture2D)material.GetTexture("_OcclusionMap"),
                emission = (Texture2D)material.GetTexture("_EmissionMap"),
                smoothness = (Texture2D)material.GetTexture("_SpecGlossMap"),
                isAlphaClip = material.GetInt("_AlphaClip") == 1,
                isAutodesk = material.shader.name.Contains("Autodesk"),
                moesExists = AssetDatabase.LoadAssetAtPath<Material>(path.Replace(".mat", "_MOES.mat")) != null
            };

            if (!moesTexture.smoothness)
            {
                moesTexture.smoothness = moesTexture.material.GetInt("_SmoothnessTextureChannel") == 0
                    ? (Texture2D)material.GetTexture("_MetallicGlossMap")
                    : (Texture2D)material.GetTexture("_BaseMap");
                moesTexture.smoothnessIsAlpha = true;
            }

            if (!moesTexture.metallic && !moesTexture.occlusion && !moesTexture.emission && !moesTexture.smoothness)
                return null;

            return moesTexture;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static void SetBloodTextures(this Material material, Material bloodDecalMaterial)
    {
        //only for MOES
        if (material.shader.name != "ThunderRoad/Lit")
            return;

        material.SetTexture(Layer0, bloodDecalMaterial.GetTexture(Layer0));
        material.SetTexture(Layer1, bloodDecalMaterial.GetTexture(Layer1));
        material.SetTexture(Layer1NormalMap, bloodDecalMaterial.GetTexture(Layer1NormalMap));
        material.SetFloat(UseReveal, 1);

        AssetDatabase.SaveAssets();
    }
}

#endif