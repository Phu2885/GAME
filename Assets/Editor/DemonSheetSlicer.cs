using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

/// <summary>
/// Cuts Assets/Art/Characters/Imp/demon_sheet.png (448x48) into 14 sprites demon_0 ... demon_13
/// (32x48 cells, pivot bottom-centre) using the 2D Sprite package's ISpriteEditorDataProvider.
/// Runs during import, after <see cref="PixelArtImporter"/> has set the Sprite texture type and 32 PPU.
/// Story 003.
/// </summary>
public class DemonSheetSlicer : AssetPostprocessor
{
    private const string SheetPath = "Assets/Art/Characters/Imp/demon_sheet.png";
    private const int FrameCount = 14;
    private const int FrameWidth = 32;
    private const int FrameHeight = 48;

    public override int GetPostprocessOrder() => 10;

    // Bumping this makes Unity re-import the sheet once so the slicing is applied to the existing asset.
    public override uint GetVersion() => 1;

    private void OnPreprocessTexture()
    {
        if (assetPath != SheetPath)
        {
            return;
        }

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;

        var factories = new SpriteDataProviderFactories();
        factories.Init();
        ISpriteEditorDataProvider provider = factories.GetSpriteEditorDataProviderFromObject(importer);
        provider.InitSpriteEditorDataProvider();

        // Reuse the ids of sprites that already exist so references to them survive a re-slice.
        SpriteRect[] existing = provider.GetSpriteRects();
        var rects = new SpriteRect[FrameCount];
        for (int i = 0; i < FrameCount; i++)
        {
            string spriteName = $"demon_{i}";
            GUID id = GUID.Generate();
            foreach (SpriteRect old in existing)
            {
                if (old.name == spriteName)
                {
                    id = old.spriteID;
                    break;
                }
            }

            rects[i] = new SpriteRect
            {
                name = spriteName,
                spriteID = id,
                rect = new Rect(i * FrameWidth, 0f, FrameWidth, FrameHeight),
                alignment = SpriteAlignment.BottomCenter,
                pivot = new Vector2(0.5f, 0f),
            };
        }

        provider.SetSpriteRects(rects);
        provider.Apply();
    }
}
