using UnityEditor;
using UnityEngine;

/// <summary>
/// Imports every texture under Assets/Art as crisp pixel art:
/// point filtering, 32 pixels per unit, no compression, no mipmaps.
/// </summary>
public class PixelArtImporter : AssetPostprocessor
{
    private const string ArtRoot = "Assets/Art/";
    private const float PixelsPerUnit = 32f;

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(ArtRoot))
        {
            return;
        }

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
    }
}
