using System.Collections.Generic;
using System.Linq;
using Fartooth.CardPools;
using Fartooth.PotionPools;
using Fartooth.RelicPools;
using Fartooth.Relics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Fartooth.Characters;

public sealed class Fartooth : CharacterModel
{
	public override Color NameColor => new Color("beba27ff");

	public override CharacterGender Gender => CharacterGender.Feminine;

	protected override CharacterModel? UnlocksAfterRunAs => null;

	public override int StartingHp => 70;

	public override int StartingGold => 99;

	public override CardPoolModel CardPool => ModelDb.CardPool<FartoothCardPool>();

	public override RelicPoolModel RelicPool => ModelDb.RelicPool<FartoothRelicPool>();

	public override PotionPoolModel PotionPool => ModelDb.PotionPool<FartoothPotionPool>();

	public override IEnumerable<CardModel> StartingDeck => new CardModel[]
	{
		ModelDb.Card<global::Fartooth.Cards.StrikeFartooth>(),
		ModelDb.Card<global::Fartooth.Cards.StrikeFartooth>(),
		ModelDb.Card<global::Fartooth.Cards.StrikeFartooth>(),
		ModelDb.Card<global::Fartooth.Cards.StrikeFartooth>(),
		ModelDb.Card<global::Fartooth.Cards.DefendFartooth>(),
		ModelDb.Card<global::Fartooth.Cards.DefendFartooth>(),
		ModelDb.Card<global::Fartooth.Cards.DefendFartooth>(),
		ModelDb.Card<global::Fartooth.Cards.DefendFartooth>(),
	};

	public override IReadOnlyList<RelicModel> StartingRelics => new RelicModel[]
	{
		ModelDb.Relic<Sniper>()
	};

	public override float AttackAnimDelay => 0.15f;

	public override float CastAnimDelay => 0.25f;

	public override Color EnergyLabelOutlineColor => Colors.Gold;

	public override Color DialogueColor => Colors.Gold;

	public override Color MapDrawingColor => Colors.Gold;

	public override Color RemoteTargetingLineColor => Colors.Gold;

	public override Color RemoteTargetingLineOutline => Colors.Gold;

	public override List<string> GetArchitectAttackVfx()
	{
		return new List<string>();
	}
	public override string CharacterSelectSfx =>
	ModelDb.Character<Ironclad>().CharacterSelectSfx;

	public override string CharacterTransitionSfx =>
		"event:/sfx/ui/wipe_ironclad";

	[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllCharacters), MethodType.Getter)]
	public static class ModelDbAllCharactersPatch
	{
		static void Postfix(ref IEnumerable<CharacterModel> __result)
		{
			__result = __result
				.Append(ModelDb.Character<Fartooth>())
				.Distinct();
		}
	}
}
