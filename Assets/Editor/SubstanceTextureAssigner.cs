using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SubstanceTextureAssigner
{
    private const string MaterialsFolder = "Assets/Materials/Models";
    private const string TexturesFolder = "Assets/Textures/A2_models";

    private const string ShaderName = "Autodesk Interactive";

    private class TextureSlot
    {
        public string suffix;
        public bool isNormalMap;
        public bool useSrgb;
        public bool required;
        public string[] propertyNames;

        public TextureSlot(string suffix, bool isNormalMap, bool useSrgb, bool required, params string[] propertyNames)
        {
            this.suffix = suffix;
            this.isNormalMap = isNormalMap;
            this.useSrgb = useSrgb;
            this.required = required;
            this.propertyNames = propertyNames;
        }
    }

    private static readonly List<string> MaterialNames = new()
    {
        "MAT_Tree1",
        "MAT_Tree2",
        "MAT_Tree3",
        "MAT_Tree4",
        // "MAT_Hands",
        // "MAT_Axe",
        // "MAT_Bed",
        // "MAT_Bench",
        // "MAT_ChoppingBlock",
        // "MAT_Clipboard",
        // "MAT_FallenLog",
        // "MAT_Firewood",
        // "MAT_KiwiNest",
        // "MAT_MudSlide",
        // "MAT_Rocks",
        // "MAT_Shovel",
        // "MAT_sign",
        // "MAT_Table",
        // "MAT_Trap",
        // "MAT_Trash",
        // "MAT_WoodRack"
    };

    private static readonly List<TextureSlot> TextureSlots = new()
    {
        // Substance export name suffixes must match your Output Template exactly.
        new("BaseColour", false, true,  true,  "_MainTex", "_BaseMap", "_Albedo"),
        new("Metal",      false, false, true,  "_MetallicGlossMap", "_MetallicMap", "_Metallic"),
        new("Roughness",  false, false, true,  "_SpecGlossMap", "_RoughnessMap", "_Roughness"),
        new("Normal",     true,  false, true,  "_BumpMap", "_NormalMap"),
        new("AO",         false, true,  true,  "_OcclusionMap"),

        // Keep this optional. If you are not using emission, missing emissive textures will not fail the material.
        new("Emissive",   false, true,  false, "_EmissionMap")
    };

    [MenuItem("Tools/Assign Substance Painter Textures")]
    private static void AssignTextures()
    {
        Shader shader = Shader.Find(ShaderName);

        if (shader == null)
        {
            Debug.LogError($"Could not find shader: {ShaderName}");
            return;
        }

        foreach (string materialName in MaterialNames)
        {
            string materialPath = $"{MaterialsFolder}/{materialName}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

            if (material == null)
            {
                Debug.LogError($"FAILED: Material '{materialName}' was not found at '{materialPath}'.");
                continue;
            }

            material.shader = shader;
            material.color = Color.white;

            bool allRequiredTexturesAssigned = true;

            foreach (TextureSlot slot in TextureSlots)
            {
                string textureName = $"{materialName}_{slot.suffix}";
                Texture2D texture = LoadTexture(textureName, slot.isNormalMap, slot.useSrgb);

                if (texture == null)
                {
                    if (slot.required)
                    {
                        allRequiredTexturesAssigned = false;
                        Debug.LogError($"FAILED: Texture '{slot.suffix}' for material '{materialName}' was not found. Expected file name like '{textureName}.png'.");
                    }
                    else
                    {
                        Debug.LogWarning($"OPTIONAL MISSING: Texture '{slot.suffix}' for material '{materialName}' was not found.");
                    }

                    continue;
                }

                bool assigned = TrySetTexture(material, texture, slot.propertyNames);

                if (!assigned)
                {
                    if (slot.required)
                    {
                        allRequiredTexturesAssigned = false;
                        Debug.LogError($"FAILED: Texture '{slot.suffix}' for material '{materialName}' was found but could not be applied. No matching shader property exists on '{ShaderName}'.");
                    }
                    else
                    {
                        Debug.LogWarning($"OPTIONAL FAILED: Texture '{slot.suffix}' for material '{materialName}' was found but could not be applied.");
                    }

                    continue;
                }

                if (slot.suffix == "Emissive")
                {
                    material.EnableKeyword("_EMISSION");
                }
            }

            EditorUtility.SetDirty(material);

            if (allRequiredTexturesAssigned)
            {
                Debug.Log($"SUCCESS: All textures of material '{materialName}' have been successfully assigned.");
            }
            else
            {
                Debug.LogError($"FAILED: One or more required textures of material '{materialName}' failed to assign. Check the errors above.");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Finished assigning Substance Painter textures.");
    }

    private static Texture2D LoadTexture(string textureNameWithoutExtension, bool isNormalMap, bool useSrgb)
    {
        string[] guids = AssetDatabase.FindAssets($"{textureNameWithoutExtension} t:Texture2D", new[] { TexturesFolder });

        if (guids.Length == 0)
            return null;

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer != null)
        {
            TextureImporterType targetType = isNormalMap ? TextureImporterType.NormalMap : TextureImporterType.Default;

            bool needsReimport =
                importer.textureType != targetType ||
                importer.sRGBTexture != useSrgb;

            importer.textureType = targetType;
            importer.sRGBTexture = useSrgb;

            if (needsReimport)
            {
                importer.SaveAndReimport();
            }
        }

        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static bool TrySetTexture(Material material, Texture2D texture, params string[] possiblePropertyNames)
    {
        foreach (string propertyName in possiblePropertyNames)
        {
            if (material.HasProperty(propertyName))
            {
                material.SetTexture(propertyName, texture);
                return true;
            }
        }

        return false;
    }
}