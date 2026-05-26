using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fartooth.Cards;
using Fartooth.Relics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Managers;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;

namespace Fartooth.Scripts;
// ========== 1. 瑙掕壊娉ㄥ唽 ==========
[HarmonyPatch(typeof(ModelDb), "get_AllCharacters")]
internal static class FartoothCharacterRegistrationPatch
{
    private static void Postfix(ref IEnumerable<CharacterModel> __result)
    {
        CharacterModel fartooth = ModelDb.Character<Fartooth.Characters.Fartooth>();
        if (__result.Any(existingCharacter => existingCharacter.Id == fartooth.Id))
        {
            return;
        }

        __result = __result.Concat(new[] { fartooth });
    }
}

[HarmonyPatch(typeof(UnlockState), "get_Characters")]
internal static class FartoothUnlockStateCharactersPatch
{
    private static void Postfix(ref IEnumerable<CharacterModel> __result)
    {
        CharacterModel fartooth = ModelDb.Character<Fartooth.Characters.Fartooth>();
        if (__result.Any(existingCharacter => existingCharacter.Id == fartooth.Id))
        {
            return;
        }

        __result = __result.Concat(new[] { fartooth });
    }
}
// ========== 2. 杈呭姪鍒ゆ柇 ==========





internal static class FartoothCharacterFallbacks
{
    public const string FartoothId = "FARTOOTH";
    public const string SilentVisualsPath = "res://scenes/creature_visuals/fartooth.tscn";
    public const string SilentEnergyCounterPath = "res://scenes/combat/energy_counters/ironclad_energy_counter.tscn";
    public const string SilentTrailPath = "res://scenes/vfx/card_trail_ironclad.tscn";
    public const string SilentRestSitePath = "res://scenes/rest_site/characters/fartooth_rest_site.tscn";

    public static bool IsFartoothCharacter(CharacterModel? model)
    {
        return model?.Id.Entry == FartoothId;
    }

    public static bool IsFartoothPlayer(Player? player)
    {
        return IsFartoothCharacter(player?.Character);
    }

    public static bool IsFartoothCreature(Creature? creature)
    {
        return IsFartoothPlayer(creature?.Player);
    }


}

[HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.CreateVisuals))]
internal static class FartoothCharacterCreateVisualsPatch
{
    private static bool Prefix(CharacterModel __instance, ref NCreatureVisuals __result)
    {
        if (!FartoothCharacterFallbacks.IsFartoothCharacter(__instance))
        {
            return true;
        }
        if (NCombatRoom.Instance != null)
        {
            Log.Info("[Fartooth] Combat creature visuals requested. Expecting res://scenes/creature_visuals/fartooth.tscn");
            return true;
        }
        // 闈炴垬鏂楀満鏅紙瑙掕壊閫夋嫨銆侀瑙堢瓑锛変娇鐢?Silent 璧勬簮
        Log.Info("[Fartooth] Using non-combat creature visuals fallback: " + FartoothCharacterFallbacks.SilentVisualsPath);
        __result = PreloadManager.Cache.GetScene(FartoothCharacterFallbacks.SilentVisualsPath)
            .Instantiate<NCreatureVisuals>(PackedScene.GenEditState.Disabled);
        return false;
    }
}

[HarmonyPatch(typeof(Creature), nameof(Creature.CreateVisuals))]
internal static class FartoothCreatureCreateVisualsPatch
{
    private static bool Prefix(Creature __instance, ref NCreatureVisuals? __result)
    {
        if (!FartoothCharacterFallbacks.IsFartoothCreature(__instance))
        {
            return true;
        }
        if (NCombatRoom.Instance != null)
        {
            Log.Info("[Fartooth] Combat creature visuals requested via Creature.CreateVisuals. Expecting res://scenes/creature_visuals/fartooth.tscn");
            return true;
        }
        Log.Info("[Fartooth] Using non-combat creature visuals fallback from Creature.CreateVisuals: " + FartoothCharacterFallbacks.SilentVisualsPath);
        __result = PreloadManager.Cache.GetScene(FartoothCharacterFallbacks.SilentVisualsPath)
            .Instantiate<NCreatureVisuals>(PackedScene.GenEditState.Disabled);
        return false;
    }
}

[HarmonyPatch(typeof(NEnergyCounter), nameof(NEnergyCounter.Create))]
internal static class FartoothEnergyCounterCreatePatch
{
    private static readonly FieldInfo PlayerField = AccessTools.Field(typeof(NEnergyCounter), "_player");

    private static bool Prefix(Player player, ref NEnergyCounter? __result)
    {
        if (!FartoothCharacterFallbacks.IsFartoothPlayer(player))
        {
            return true;
        }

        if (TestMode.IsOn)
        {
            __result = null;
            return false;
        }

        Log.Info("[Fartooth] Using energy counter scene: " + FartoothCharacterFallbacks.SilentEnergyCounterPath);
        NEnergyCounter energyCounter = PreloadManager.Cache.GetScene(FartoothCharacterFallbacks.SilentEnergyCounterPath)
            .Instantiate<NEnergyCounter>(PackedScene.GenEditState.Disabled);
        PlayerField.SetValue(energyCounter, player);
        __result = energyCounter;
        return false;
    }
}

[HarmonyPatch(typeof(NCardTrailVfx), nameof(NCardTrailVfx.Create))]
internal static class FartoothCardTrailPatch
{
    private static readonly FieldInfo NodeToFollowField = AccessTools.Field(typeof(NCardTrailVfx), "_nodeToFollow");

    private static bool Prefix(Control card, string characterTrailPath, ref NCardTrailVfx? __result)
    {
        if (characterTrailPath != "res://scenes/vfx/card_trail_fartooth.tscn")
        {
            return true;
        }

        if (TestMode.IsOn)
        {
            __result = null;
            return false;
        }

        Log.Info("[Fartooth] Using card trail scene: " + FartoothCharacterFallbacks.SilentTrailPath + " (requested " + characterTrailPath + ")");
        NCardTrailVfx cardTrail = PreloadManager.Cache.GetScene(FartoothCharacterFallbacks.SilentTrailPath)
            .Instantiate<NCardTrailVfx>(PackedScene.GenEditState.Disabled);
        NodeToFollowField.SetValue(cardTrail, card);
        __result = cardTrail;
        return false;
    }
}

[HarmonyPatch(typeof(NRestSiteCharacter), nameof(NRestSiteCharacter.Create))]
internal static class FartoothRestSiteCharacterPatch
{
    private static readonly FieldInfo RestSitePlayerBackingField =
        AccessTools.Field(typeof(NRestSiteCharacter), "<Player>k__BackingField");

    private static readonly FieldInfo RestSiteCharacterIndexField =
        AccessTools.Field(typeof(NRestSiteCharacter), "_characterIndex");

    private static bool Prefix(Player player, int characterIndex, ref NRestSiteCharacter __result)
    {
        if (!FartoothCharacterFallbacks.IsFartoothPlayer(player))
        {
            return true;
        }

        Log.Info("[Fartooth] Using rest site scene: " + FartoothCharacterFallbacks.SilentRestSitePath);
        NRestSiteCharacter restSiteCharacter = PreloadManager.Cache.GetScene(FartoothCharacterFallbacks.SilentRestSitePath)
            .Instantiate<NRestSiteCharacter>(PackedScene.GenEditState.Disabled);
        RestSitePlayerBackingField.SetValue(restSiteCharacter, player);
        RestSiteCharacterIndexField.SetValue(restSiteCharacter, characterIndex);
        __result = restSiteCharacter;
        return false;
    }
}


[HarmonyPatch(typeof(CharacterModel), "get_MerchantAnimPath")]
internal static class FartoothMerchantAnimPathPatch
{
    private const string SilentMerchantPath = "res://scenes/merchant/characters/fartooth_merchant.tscn";

    private static bool Prefix(CharacterModel __instance, ref string __result)
    {
        if (!FartoothCharacterFallbacks.IsFartoothCharacter(__instance))
        {
            return true;
        }

        __result = SilentMerchantPath;
        Log.Info("[Fartooth] Using merchant scene fallback: " + __result);
        return false;
    }
}
[HarmonyPatch(typeof(ProgressSaveManager), "ObtainCharUnlockEpoch")]
internal static class FartoothObtainCharUnlockEpochPatch
{
    private static bool Prefix(Player localPlayer)
    {
        return !FartoothCharacterFallbacks.IsFartoothPlayer(localPlayer);
    }
}

[HarmonyPatch(typeof(Neow), "DefineDialogues")]
internal static class FartoothNeowDialoguePatch
{
    private static void Postfix(ref AncientDialogueSet __result)
    {
        AncientDialogue fartoothDialogue = new AncientDialogue(
            "event:/sfx/npcs/neow/neow_welcome",
            "event:/sfx/npcs/neow/neow_sleepy",
            "",
            "event:/sfx/npcs/neow/neow_welcome")
        {
            VisitIndex = 0
        };

        Dictionary<string, IReadOnlyList<AncientDialogue>> characterDialogues =
            new Dictionary<string, IReadOnlyList<AncientDialogue>>(__result.CharacterDialogues);

        characterDialogues[FartoothCharacterFallbacks.FartoothId] = new AncientDialogue[1]
        {
            fartoothDialogue
        };

        __result = new AncientDialogueSet
        {
            FirstVisitEverDialogue = __result.FirstVisitEverDialogue,
            CharacterDialogues = characterDialogues,
            AgnosticDialogues = __result.AgnosticDialogues
        };
    }
}

[HarmonyPatch(typeof(Orobas), "DefineDialogues")]
internal static class FartoothOrobasDialoguePatch
{
    private static void Postfix(ref AncientDialogueSet __result)
    {
        Dictionary<string, IReadOnlyList<AncientDialogue>> characterDialogues =
            new Dictionary<string, IReadOnlyList<AncientDialogue>>(__result.CharacterDialogues);

        characterDialogues[FartoothCharacterFallbacks.FartoothId] = new AncientDialogue[2]
        {
            new AncientDialogue("", "", "", "")
            {
                VisitIndex = 0
            },
            new AncientDialogue("", "", "")
            {
                VisitIndex = 1
            }
        };

        __result = new AncientDialogueSet
        {
            FirstVisitEverDialogue = __result.FirstVisitEverDialogue,
            CharacterDialogues = characterDialogues,
            AgnosticDialogues = __result.AgnosticDialogues
        };
    }
}

[HarmonyPatch(typeof(Darv), "DefineDialogues")]
internal static class FartoothDarvDialoguePatch
{
    private static void Postfix(ref AncientDialogueSet __result)
    {
        Dictionary<string, IReadOnlyList<AncientDialogue>> characterDialogues =
            new Dictionary<string, IReadOnlyList<AncientDialogue>>(__result.CharacterDialogues);

        characterDialogues[FartoothCharacterFallbacks.FartoothId] = new AncientDialogue[2]
        {
            new AncientDialogue(
                "event:/sfx/npcs/darv/darv_introduction",
                "",
                "event:/sfx/npcs/darv/darv_excited")
            {
                VisitIndex = 0
            },
            new AncientDialogue("event:/sfx/npcs/darv/darv_endeared")
            {
                VisitIndex = 1
            }
        };

        __result = new AncientDialogueSet
        {
            FirstVisitEverDialogue = __result.FirstVisitEverDialogue,
            CharacterDialogues = characterDialogues,
            AgnosticDialogues = __result.AgnosticDialogues
        };
    }
}

[HarmonyPatch(typeof(Nonupeipe), "DefineDialogues")]
internal static class FartoothNonupeipeDialoguePatch
{
    private static void Postfix(ref AncientDialogueSet __result)
    {
        Dictionary<string, IReadOnlyList<AncientDialogue>> characterDialogues =
            new Dictionary<string, IReadOnlyList<AncientDialogue>>(__result.CharacterDialogues);

        characterDialogues[FartoothCharacterFallbacks.FartoothId] = new AncientDialogue[2]
        {
            new AncientDialogue(
                "event:/sfx/npcs/nonupeipe/nonupeipe_welcome",
                "",
                "event:/sfx/npcs/nonupeipe/nonupeipe_welcome",
                "",
                "event:/sfx/npcs/nonupeipe/nonupeipe_giggle")
            {
                VisitIndex = 0
            },
            new AncientDialogue(
                "event:/sfx/npcs/nonupeipe/nonupeipe_welcome",
                "")
            {
                VisitIndex = 1
            }
        };

        __result = new AncientDialogueSet
        {
            FirstVisitEverDialogue = __result.FirstVisitEverDialogue,
            CharacterDialogues = characterDialogues,
            AgnosticDialogues = __result.AgnosticDialogues
        };
    }
}

[HarmonyPatch(typeof(Tanx), "DefineDialogues")]
internal static class FartoothTanxDialoguePatch
{
    private static void Postfix(ref AncientDialogueSet __result)
    {
        Dictionary<string, IReadOnlyList<AncientDialogue>> characterDialogues =
            new Dictionary<string, IReadOnlyList<AncientDialogue>>(__result.CharacterDialogues);

        characterDialogues[FartoothCharacterFallbacks.FartoothId] = new AncientDialogue[2]
        {
            new AncientDialogue(
                "event:/sfx/npcs/tanx/tanx_curiosity",
                "",
                "event:/sfx/npcs/tanx/tanx_laugh")
            {
                VisitIndex = 0
            },
            new AncientDialogue(
                "event:/sfx/npcs/tanx/tanx_roar",
                "")
            {
                VisitIndex = 1
            }
        };

        __result = new AncientDialogueSet
        {
            FirstVisitEverDialogue = __result.FirstVisitEverDialogue,
            CharacterDialogues = characterDialogues,
            AgnosticDialogues = __result.AgnosticDialogues
        };
    }
}

[HarmonyPatch(typeof(Tezcatara), "DefineDialogues")]
internal static class FartoothTezcataraDialoguePatch
{
    private static void Postfix(ref AncientDialogueSet __result)
    {
        Dictionary<string, IReadOnlyList<AncientDialogue>> characterDialogues =
            new Dictionary<string, IReadOnlyList<AncientDialogue>>(__result.CharacterDialogues);

        characterDialogues[FartoothCharacterFallbacks.FartoothId] = new AncientDialogue[2]
        {
            new AncientDialogue(
                "",
                "",
                "",
                "")
            {
                VisitIndex = 0
            },
            new AncientDialogue("")
            {
                VisitIndex = 1
            }
        };

        __result = new AncientDialogueSet
        {
            FirstVisitEverDialogue = __result.FirstVisitEverDialogue,
            CharacterDialogues = characterDialogues,
            AgnosticDialogues = __result.AgnosticDialogues
        };
    }
}

[HarmonyPatch(typeof(Vakuu), "DefineDialogues")]
internal static class FartoothVakuuDialoguePatch
{
    private static void Postfix(ref AncientDialogueSet __result)
    {
        Dictionary<string, IReadOnlyList<AncientDialogue>> characterDialogues =
            new Dictionary<string, IReadOnlyList<AncientDialogue>>(__result.CharacterDialogues);

        characterDialogues[FartoothCharacterFallbacks.FartoothId] = new AncientDialogue[2]
        {
            new AncientDialogue(
                "",
                "",
                "",
                "",
                "")
            {
                VisitIndex = 0
            },
            new AncientDialogue("")
            {
                VisitIndex = 1
            }
        };

        __result = new AncientDialogueSet
        {
            FirstVisitEverDialogue = __result.FirstVisitEverDialogue,
            CharacterDialogues = characterDialogues,
            AgnosticDialogues = __result.AgnosticDialogues
        };
    }
}

[HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenBossesDefeatedEpoch")]
internal static class FartoothCheckFifteenBossesDefeatedEpochPatch
{
    private static bool Prefix(Player localPlayer)
    {
        return !FartoothCharacterFallbacks.IsFartoothPlayer(localPlayer);
    }
}

[HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenElitesDefeatedEpoch")]
internal static class FartoothCheckFifteenElitesDefeatedEpochPatch
{
    private static bool Prefix(Player localPlayer)
    {
        return !FartoothCharacterFallbacks.IsFartoothPlayer(localPlayer);
    }
}

[HarmonyPatch(typeof(TheArchitect), "WinRun")]
internal static class FartoothArchitectWinRunPatch
{
    private static bool Prefix(TheArchitect __instance, ref Task __result)
    {
        if (!FartoothCharacterFallbacks.IsFartoothPlayer(__instance.Owner))
        {
            return true;
        }

        __result = SafeWinRun(__instance);
        return false;
    }

    private static Task SafeWinRun(TheArchitect architectEvent)
    {
        if (!LocalContext.IsMe(architectEvent.Owner))
        {
            return Task.CompletedTask;
        }

        if (architectEvent.Owner?.RunState?.Players.Count > 1)
        {
            NCombatRoom.Instance?.SetWaitingForOtherPlayersOverlayVisible(visible: true);
        }

        RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
        return Task.CompletedTask;
    }
}

[HarmonyPatch(typeof(AromaOfChaos), "MaintainControl")]
internal static class FartoothAromaOfChaosMaintainControlPatch
{
    private static readonly MethodInfo SetEventFinishedMethod =
        AccessTools.Method(typeof(EventModel), "SetEventFinished");

    private static bool Prefix(AromaOfChaos __instance, ref Task __result)
    {
        if (!FartoothCharacterFallbacks.IsFartoothPlayer(__instance.Owner))
        {
            return true;
        }

        __result = SafeMaintainControl(__instance);
        return false;
    }

    private static Task SafeMaintainControl(AromaOfChaos aromaOfChaos)
    {
        return SafeMaintainControlAsync(aromaOfChaos);
    }

    private static async Task SafeMaintainControlAsync(AromaOfChaos aromaOfChaos)
    {
        CardModel? cardToUpgrade = (await CardSelectCmd.FromDeckForUpgrade(
            aromaOfChaos.Owner,
            new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1))).FirstOrDefault();

        if (cardToUpgrade != null)
        {
            CardCmd.Upgrade(cardToUpgrade);
        }

        LocString description = new("events", "AROMA_OF_CHAOS.pages.MAINTAIN_CONTROL.description");
        string aromaPrincipleKey = aromaOfChaos.Owner.Character.Id.Entry + ".aromaPrinciple";
        if (LocString.Exists("characters", aromaPrincipleKey))
        {
            description.Add("AromaPrinciple", new LocString("characters", aromaPrincipleKey));
        }
        else
        {
            description.Add("AromaPrinciple", aromaOfChaos.Owner.Character.Title);
        }

        SetEventFinishedMethod.Invoke(aromaOfChaos, new object[] { description });
    }
}



[HarmonyPatch(typeof(NCardLibrary), nameof(NCardLibrary._Ready))]
internal static class FartoothCardLibraryPatch
{
    private static readonly FieldInfo CardPoolFiltersField =
        AccessTools.Field(typeof(NCardLibrary), "_cardPoolFilters");

    private static readonly FieldInfo PoolFiltersField =
        AccessTools.Field(typeof(NCardLibrary), "_poolFilters");

    private static readonly FieldInfo LastHoveredControlField =
        AccessTools.Field(typeof(NCardLibrary), "_lastHoveredControl");

    private static readonly MethodInfo UpdateCardPoolFilterMethod =
        AccessTools.Method(typeof(NCardLibrary), "UpdateCardPoolFilter");

    private const string FartoothCardIconPath = "res://images/ui/top_panel/character_icon_fartooth.png";

    private static void Postfix(NCardLibrary __instance)
    {
        CharacterModel fartooth = ModelDb.Character<Fartooth.Characters.Fartooth>();
        Dictionary<CharacterModel, NCardPoolFilter> cardPoolFilters =
            (Dictionary<CharacterModel, NCardPoolFilter>)CardPoolFiltersField.GetValue(__instance);

        if (cardPoolFilters.ContainsKey(fartooth))
        {
            return;
        }

        GridContainer poolFiltersContainer =
            __instance.GetNode<GridContainer>("Sidebar/MarginContainer/TopVBox/PoolFilters");
        NCardPoolFilter silentFilter = __instance.GetNode<NCardPoolFilter>("%SilentPool");
        NCardPoolFilter fartoothFilter = silentFilter.Duplicate() as NCardPoolFilter;
        if (fartoothFilter == null)
        {
            return;
        }

        fartoothFilter.Name = "FartoothPool";
        fartoothFilter.Visible = true;
        poolFiltersContainer.AddChild(fartoothFilter);
        poolFiltersContainer.MoveChild(fartoothFilter, 2);
        fartoothFilter.Owner = __instance;

        Texture2D texture = GD.Load<Texture2D>(FartoothCardIconPath);
        if (texture != null)
        {
            TextureRect image = fartoothFilter.GetNode<TextureRect>("Image");
            image.Texture = texture;

            TextureRect shadow = image.GetNode<TextureRect>("Shadow");
            shadow.Texture = texture;
        }

        fartoothFilter.Loc = new LocString("characters", "FARTOOTH.title");
        fartoothFilter.Connect(
            NCardPoolFilter.SignalName.Toggled,
            Callable.From<NCardPoolFilter>(filter => UpdateCardPoolFilterMethod.Invoke(__instance, new object[] { filter })));
        fartoothFilter.Connect(
            Control.SignalName.FocusEntered,
            Callable.From(() => LastHoveredControlField.SetValue(__instance, fartoothFilter)));

        Dictionary<NCardPoolFilter, Func<CardModel, bool>> poolFilters =
            (Dictionary<NCardPoolFilter, Func<CardModel, bool>>)PoolFiltersField.GetValue(__instance);
        poolFilters.Add(fartoothFilter, static c => c.Pool is Fartooth.CardPools.FartoothCardPool);
        cardPoolFilters.Add(fartooth, fartoothFilter);

        UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
        fartoothFilter.Visible = unlockState.Characters.Contains(fartooth);
    }
}



//[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
//internal static class FartoothTouchOfOrobasUpgradePatch
//{
//    private static bool Prefix(RelicModel starterRelic, ref RelicModel __result)
//    {
 //       if (starterRelic.Id != ModelDb.Relic<Sniper>().Id)
//        {
//            return true;
//        }

//        __result = ModelDb.Relic<SecretCrimsonScepter>();
//        return false;
//    }
//}

[HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.SetupForPlayer))]
internal static class FartoothArchaicToothSetupPatch
{
    private static readonly MethodInfo StarterCardSetter =
        AccessTools.PropertySetter(typeof(ArchaicTooth), nameof(ArchaicTooth.StarterCard));

    private static readonly MethodInfo AncientCardSetter =
        AccessTools.PropertySetter(typeof(ArchaicTooth), nameof(ArchaicTooth.AncientCard));

    

    

    
}





