using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScrappyChests;

public static class DifficultyIconHooks
{
    private static Texture2D _whiteScrapTexture;
    private static Texture2D _yellowScrapTexture;

    public static void Init()
    {
        _whiteScrapTexture = CreateReadableTexture(Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Scrap/texScrapWhiteIcon.png").WaitForCompletion());
        _yellowScrapTexture = CreateReadableTexture(Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Scrap/texScrapYellowIcon.png").WaitForCompletion());

        On.EclipseDifficultyMedalDisplay.Refresh += EclipseDifficultyMedalDisplay_Refresh;
        On.RoR2.DifficultyDef.GetIconSprite += DifficultyDef_GetIconSprite;
        On.RoR2.UI.RuleChoiceController.UpdateChoiceDisplay += RuleChoiceController_UpdateChoiceDisplay;
        On.RoR2.EclipseRun.OnClientGameOver += EclipseRun_OnClientGameOver;
    }

    //https://forum.unity.com/threads/easy-way-to-make-texture-isreadable-true-by-script.1141915/
    private static Texture2D CreateReadableTexture(Texture2D texture)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(texture, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(renderTex.width, renderTex.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);

        Texture2D result = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false, true);
        Color[] pixels = readableText.GetPixels(
            0,
            0,
            texture.width,
            texture.height
        );
        result.SetPixels(pixels);
        result.Apply();

        return result;
    }

    private static void EclipseDifficultyMedalDisplay_Refresh(On.EclipseDifficultyMedalDisplay.orig_Refresh orig, EclipseDifficultyMedalDisplay self)
    {
        orig(self);
        if (Configuration.Instance.ModEnabled.Value)
        {
            LocalUser localUser = LocalUserManager.GetFirstLocalUser();
            SurvivorDef survivorPreference = localUser?.userProfile.GetSurvivorPreference();
            if (!survivorPreference)
            {
                return;
            }

            var maxEclipseLevels = Configuration.Instance.MaxCompletedEclipseLevels;

            if (maxEclipseLevels.TryGetValue(survivorPreference.cachedName, out int maxLevel) && self.eclipseLevel <= maxLevel)
            {
                bool allCompleted = true;
                foreach (SurvivorDef orderedSurvivorDef in SurvivorCatalog.orderedSurvivorDefs)
                {
                    if (self.ShouldDisplaySurvivor(orderedSurvivorDef, localUser)
                        && maxEclipseLevels.TryGetValue(orderedSurvivorDef.cachedName, out int maxLevel2)
                        && maxLevel2 < self.eclipseLevel)
                    {
                        allCompleted = false;
                        break;
                    }
                }

                self.iconImage.sprite = AddScapToSprite(self.iconImage.sprite, allCompleted ? _yellowScrapTexture : _whiteScrapTexture);
            }
        }
    }

    private static Sprite DifficultyDef_GetIconSprite(On.RoR2.DifficultyDef.orig_GetIconSprite orig, DifficultyDef self)
    {
        orig(self);
        if (Configuration.Instance.ModEnabled.Value && self.foundIconSprite)
        {
            return AddScapToSprite(self.iconSprite, _whiteScrapTexture);
        }

        return self.iconSprite;
    }

    private static void RuleChoiceController_UpdateChoiceDisplay(On.RoR2.UI.RuleChoiceController.orig_UpdateChoiceDisplay orig, RoR2.UI.RuleChoiceController self, RuleChoiceDef displayChoiceDef)
    {
        orig(self, displayChoiceDef);
        if (Configuration.Instance.ModEnabled.Value && displayChoiceDef.globalName.StartsWith("Difficulty."))
        {
            self.image.sprite = AddScapToSprite(self.image.sprite, _whiteScrapTexture);
        }
    }

    private static Sprite AddScapToSprite(Sprite sprite, Texture2D scrapTexture)
    {
        var newSprite = Sprite.Create(CreateReadableTexture(sprite.texture), sprite.rect, sprite.pivot, sprite.pixelsPerUnit);
        AddScapTexture(newSprite.texture, scrapTexture);
        newSprite.texture.Apply();
        return newSprite;
    }

    private static void AddScapTexture(Texture2D texture, Texture2D scrapTexture)
    {
        int width = texture.width / 2;
        int height = texture.height / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color c = scrapTexture.GetPixelBilinear((float)x / width, (float)y / height);
                if (c.a > 0)
                {
                    texture.SetPixel(x + width, y, c);
                }
            }
        }
    }

    private static void EclipseRun_OnClientGameOver(On.RoR2.EclipseRun.orig_OnClientGameOver orig, EclipseRun self, RunReport runReport)
    {
        int currentEclipseLevel = EclipseRun.GetEclipseLevelFromRuleBook(self.ruleBook);

        orig(self, runReport);

        if (Configuration.Instance.ModEnabled.Value && runReport.gameEnding.isWin)
        {
            for (int index = 0; index < PlayerCharacterMasterController.instances.Count; ++index)
            {
                NetworkUser networkUser = PlayerCharacterMasterController.instances[index].networkUser;
                if (networkUser)
                {
                    LocalUser localUser = networkUser.localUser;
                    if (localUser != null)
                    {
                        SurvivorDef survivor = networkUser.GetSurvivorPreference();
                        if (survivor)
                        {
                            var levels = Configuration.Instance.MaxCompletedEclipseLevels;

                            int newEclipseLevel = Math.Min(8, currentEclipseLevel);
                            int currentLevel = levels.TryGetValue(survivor.cachedName, out int current) ? current : 0;

                            if (newEclipseLevel > currentLevel)
                            {
                                levels[survivor.cachedName] = newEclipseLevel;
                                Configuration.Instance.MaxCompletedEclipseLevels = levels;
                            }
                        }
                    }
                }
            }
        }
    }
}
