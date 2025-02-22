﻿using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Races;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionCrystalDefenseOn(CharacterActionParams actionParams) : CharacterAction(actionParams)
#pragma warning restore CA1050
{
    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;

        if (rulesetCharacter.HasConditionOfType(RaceWyrmkinBuilder.ConditionCrystalDefenseName))
        {
            yield break;
        }

        rulesetCharacter.InflictCondition(
            RaceWyrmkinBuilder.ConditionCrystalDefenseName,
            RuleDefinitions.DurationType.Irrelevant,
            0,
            RuleDefinitions.TurnOccurenceType.StartOfTurn,
            AttributeDefinitions.TagStatus,
            rulesetCharacter.Guid,
            rulesetCharacter.CurrentFaction.Name,
            1,
            RaceWyrmkinBuilder.ConditionCrystalDefenseName,
            0,
            0,
            0);

        if (ActingCharacter.SetProne(true))
        {
            yield return ActingCharacter.EventSystem.WaitForEvent(
                GameLocationCharacterEventSystem.Event.ProneInAnimationEnd);
        }
    }
}
