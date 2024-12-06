﻿using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    #region Barbarian

    private const string BrutalStrike = "BarbarianBrutalStrike";
    private static ConditionDefinition _conditionBrutalStrike;
    private static ConditionDefinition _conditionHamstringBlow;
    private static ConditionDefinition _conditionStaggeringBlow;
    private static ConditionDefinition _conditionStaggeringBlowAoO;
    private static ConditionDefinition _conditionSunderingBlow;
    private static FeatureDefinitionFeatureSet _featureSetBarbarianBrutalStrike;
    private static FeatureDefinitionFeatureSet _featureSetBarbarianBrutalStrikeImprovement13;
    private static FeatureDefinitionFeatureSet _featureSetBarbarianBrutalStrikeImprovement17;

    private static void BuildBarbarianBrutalStrike()
    {
        const string BrutalStrikeImprovement13 = "BarbarianBrutalStrikeImprovement13";
        const string BrutalStrikeImprovement17 = "BarbarianBrutalStrikeImprovement17";

        var additionalDamageBrutalStrike =
            FeatureDefinitionAdditionalDamageBuilder
                .Create("AdditionalDamageBrutalStrike")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag("BrutalStrike")
                .SetDamageDice(DieType.D10, 1)
                .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 9)
                .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                .AddCustomSubFeatures(
                    ClassHolder.Barbarian,
                    new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, _, _) => (OperationType.Set,
                        character.IsToggleEnabled((Id)ExtraActionId.BrutalStrikeToggle))))
                .AddToDB();

        _conditionBrutalStrike = ConditionDefinitionBuilder
            .Create($"Condition{BrutalStrike}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageBrutalStrike)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var powerPool = FeatureDefinitionPowerBuilder
            .Create($"Power{BrutalStrike}")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .Build())
            .AddToDB();

        powerPool.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden, new CustomBehaviorBrutalStrike(powerPool));

        // Forceful Blow

        var powerForcefulBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}ForcefulBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Hamstring Blow

        var powerHamstringBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}HamstringBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        _conditionHamstringBlow = ConditionDefinitionBuilder
            .Create("ConditionHamstringBlow")
            .SetGuiPresentation($"Power{BrutalStrike}HamstringBlow", Category.Feature,
                ConditionHindered)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityHamstringBlow")
                    .SetGuiPresentation($"Power{BrutalStrike}HamstringBlow", Category.Feature, Gui.NoLocalization)
                    .SetBaseSpeedAdditiveModifier(-3)
                    .AddToDB())
            .CopyParticleReferences(ConditionSlowed)
            .AddToDB();

        _conditionHamstringBlow.GuiPresentation.description = Gui.EmptyContent;

        // Staggering Blow

        var powerStaggeringBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}StaggeringBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        _conditionStaggeringBlow = ConditionDefinitionBuilder
            .Create("ConditionStaggeringBlow")
            .SetGuiPresentation($"Power{BrutalStrike}StaggeringBlow", Category.Feature,
                ConditionDazzled)
            .SetSilent(Silent.WhenRemoved)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create("SavingThrowAffinityStaggeringBlow")
                    .SetGuiPresentation($"Power{BrutalStrike}StaggeringBlow", Category.Feature, Gui.NoLocalization)
                    .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                        AttributeDefinitions.AbilityScoreNames)
                    .AddToDB())
            .AddSpecialInterruptions(ConditionInterruption.SavingThrow)
            .CopyParticleReferences(ConditionDazzled)
            .AddToDB();

        _conditionStaggeringBlow.GuiPresentation.description = Gui.EmptyContent;

        _conditionStaggeringBlowAoO = ConditionDefinitionBuilder
            .Create("ConditionStaggeringBlowAoO")
            .SetGuiPresentation(Category.Condition, ConditionDazzled)
            .SetSilent(Silent.WhenAdded)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(Tabletop2014Context.ActionAffinityConditionBlind)
            .AddToDB();

        // Sundering Blow

        var additionalDamageSunderingBlow = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{BrutalStrike}SunderingBlow")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("SunderingBlow")
            .SetDamageDice(DieType.D10, 1)
            .AddToDB();

        var conditionSunderingBlowAlly = ConditionDefinitionBuilder
            .Create("ConditionSunderingBlowAlly")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageSunderingBlow)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var powerSunderingBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}SunderingBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        _conditionSunderingBlow = ConditionDefinitionBuilder
            .Create("ConditionSunderingBlow")
            .SetGuiPresentation($"Power{BrutalStrike}SunderingBlow", Category.Feature,
                ConditionTargetedByGuidingBolt)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .AddCustomSubFeatures(new CustomBehaviorSunderingBlow(powerSunderingBlow, conditionSunderingBlowAlly))
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttackedNotBySource)
            .CopyParticleReferences(ConditionLeadByExampleMarked)
            .AddToDB();

        // MAIN

        PowerBundle.RegisterPowerBundle(powerPool, true,
            powerForcefulBlow, powerHamstringBlow, powerStaggeringBlow, powerSunderingBlow);

        var actionAffinityToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityBrutalStrikeToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.BrutalStrikeToggle)
            .AddToDB();

        _featureSetBarbarianBrutalStrike = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{BrutalStrike}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerPool, actionAffinityToggle, powerForcefulBlow, powerHamstringBlow)
            .AddToDB();

        _featureSetBarbarianBrutalStrikeImprovement13 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{BrutalStrikeImprovement13}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerStaggeringBlow, powerSunderingBlow)
            .AddToDB();

        _featureSetBarbarianBrutalStrikeImprovement17 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{BrutalStrikeImprovement17}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
    }

    private sealed class CustomBehaviorBrutalStrike(FeatureDefinitionPower powerBarbarianBrutalStrike)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private static readonly EffectForm ForcefulBlowForm = EffectFormBuilder
            .Create()
            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
            .Build();

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!attacker.OnceInMyTurnIsValid(BrutalStrike) ||
                !rulesetAttacker.IsToggleEnabled((Id)ExtraActionId.BrutalStrikeToggle) ||
                !rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, ConditionDefinitions.ConditionReckless.Name))
            {
                yield break;
            }

            rulesetAttacker.InflictCondition(
                _conditionBrutalStrike.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                _conditionBrutalStrike.Name,
                0,
                0,
                0);

            var aborted = false;
            var attempts = rulesetAttacker.GetClassLevel(Barbarian) >= 17 ? 2 : 1;
            var usablePower = PowerProvider.Get(powerBarbarianBrutalStrike, rulesetAttacker);
            List<FeatureDefinitionPower> selectedPowers = [];
            RulesetUsablePower savedUsablePower = null;

            for (var i = 0; i < attempts; i++)
            {
                yield return attacker.MyReactToSpendPowerBundle(
                    usablePower,
                    [defender],
                    attacker,
                    powerBarbarianBrutalStrike.Name,
                    reactionValidated: ReactionValidated,
                    reactionNotValidated: ReactionNotValidated,
                    battleManager: battleManager);

                if (aborted)
                {
                    break;
                }

                if (selectedPowers.Count > 1)
                {
                    continue;
                }

                // don't offer 1st selected effect again
                savedUsablePower = PowerProvider.Get(selectedPowers[0], rulesetAttacker);
                rulesetAttacker.UsablePowers.Remove(PowerProvider.Get(selectedPowers[0], rulesetAttacker));
            }

            // recover first selected usable power
            if (savedUsablePower != null)
            {
                rulesetAttacker.UsablePowers.Add(savedUsablePower);
            }

            yield break;

            void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                // determine selected power to collect cost
                var option = reactionRequest.SelectedSubOption;
                var subPowers = powerBarbarianBrutalStrike.GetBundle()?.SubPowers;

                if (subPowers == null)
                {
                    return;
                }

                var selectedPower = subPowers[option];

                selectedPowers.Add(selectedPower);

                switch (selectedPower.Name)
                {
                    case $"Power{BrutalStrike}ForcefulBlow":
                        actualEffectForms.Add(ForcefulBlowForm);
                        break;
                    case $"Power{BrutalStrike}HamstringBlow":
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionHamstringBlow.Name);
                        break;
                    case $"Power{BrutalStrike}StaggeringBlow":
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionStaggeringBlow.Name);
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionStaggeringBlowAoO.Name);
                        break;
                    case $"Power{BrutalStrike}SunderingBlow":
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionSunderingBlow.Name);
                        break;
                }
            }

            void ReactionNotValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                aborted = true;
            }
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled((Id)ExtraActionId.BrutalStrikeToggle) ||
                !rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, ConditionDefinitions.ConditionReckless.Name))
            {
                yield break;
            }

            attacker.SetSpecialFeatureUses(BrutalStrike, 0);
        }

        private static void InflictCondition(
            RulesetCharacter rulesetAttacker, RulesetCharacter rulesetDefender, string conditionName)
        {
            rulesetDefender.InflictCondition(
                conditionName,
                DurationType.Round,
                1,
                (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionName,
                0,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorSunderingBlow(
        FeatureDefinitionPower powerSunderingBlow,
        ConditionDefinition conditionSunderingBlowAlly) : IPhysicalAttackInitiatedOnMe, IMagicEffectAttackInitiatedOnMe
    {
        public IEnumerator OnMagicEffectAttackInitiatedOnMe(
            CharacterActionMagicEffect action,
            RulesetEffect activeEffect,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            bool firstTarget,
            bool checkMagicalAttackDamage)
        {
            var damageType = activeEffect.EffectDescription.FindFirstDamageForm()?.DamageType;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (damageType == null ||
                rulesetAttacker == null ||
                rulesetAttacker is RulesetCharacterEffectProxy)
            {
                yield break;
            }

            AddBonusAttackAndDamageRoll(attacker.RulesetCharacter, defender.RulesetActor, attackModifier);
        }

        public IEnumerator OnPhysicalAttackInitiatedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var damageType = attackMode.EffectDescription.FindFirstDamageForm()?.DamageType;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (damageType == null ||
                rulesetAttacker == null ||
                rulesetAttacker is RulesetCharacterEffectProxy)
            {
                yield break;
            }

            AddBonusAttackAndDamageRoll(attacker.RulesetCharacter, defender.RulesetActor, attackModifier);
        }

        private void AddBonusAttackAndDamageRoll(
            RulesetCharacter rulesetAttacker,
            RulesetActor rulesetDefender,
            ActionModifier actionModifier)
        {
            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, _conditionSunderingBlow.Name, out var activeCondition))
            {
                return;
            }

            var rulesetSource = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

            if (rulesetAttacker == rulesetSource)
            {
                return;
            }

            rulesetDefender.RemoveCondition(activeCondition);

            var bonusAttackRoll =
                rulesetAttacker.RollDie(DieType.D10, RollContext.None, false, AdvantageType.None, out _, out _);

            actionModifier.AttackRollModifier += bonusAttackRoll;
            actionModifier.AttacktoHitTrends.Add(new TrendInfo(
                bonusAttackRoll, FeatureSourceType.CharacterFeature, powerSunderingBlow.Name, powerSunderingBlow));

            rulesetAttacker.InflictCondition(
                conditionSunderingBlowAlly.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                FactionDefinitions.Party.Name,
                1,
                conditionSunderingBlowAlly.Name,
                0,
                0,
                0);
        }
    }

    internal static void SwitchBarbarianBrutalStrike()
    {
        Barbarian.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == _featureSetBarbarianBrutalStrike ||
            x.FeatureDefinition == _featureSetBarbarianBrutalStrikeImprovement13 ||
            x.FeatureDefinition == _featureSetBarbarianBrutalStrikeImprovement17 ||
            x.FeatureDefinition == FeatureSetBarbarianBrutalCritical ||
            x.FeatureDefinition == AttributeModifierBarbarianBrutalCriticalAdd);

        if (Main.Settings.EnableBarbarianBrutalStrike)
        {
            Barbarian.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrike, 9),
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrikeImprovement13, 13),
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrikeImprovement17, 17));
        }
        else
        {
            Barbarian.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(FeatureSetBarbarianBrutalCritical, 9),
                new FeatureUnlockByLevel(AttributeModifierBarbarianBrutalCriticalAdd, 13),
                new FeatureUnlockByLevel(AttributeModifierBarbarianBrutalCriticalAdd, 17));
        }

        Barbarian.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private static readonly FeatureDefinition FeatureBarbarianInstinctivePounce = FeatureDefinitionBuilder
        .Create("FeatureBarbarianInstinctivePounce")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static void LoadBarbarianInstinctivePounce()
    {
        var powerBarbarianInstinctivePounceTargeting = FeatureDefinitionPowerBuilder
            .Create(PowerBarbarianRageStart, "PowerBarbarianInstinctivePounceTargeting")
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, new CustomBehaviorFilterTargetingPositionHalfMove())
            .AddToDB();

        PowerBarbarianRageStart.AddCustomSubFeatures(
            new PowerOrSpellFinishedByMePowerBarbarianRageStart(powerBarbarianInstinctivePounceTargeting));
        PowerBarbarianPersistentRageStart.AddCustomSubFeatures(
            new PowerOrSpellFinishedByMePowerBarbarianRageStart(powerBarbarianInstinctivePounceTargeting));
        PathOfTheSavagery.PowerPrimalInstinct.AddCustomSubFeatures(
            new PowerOrSpellFinishedByMePowerBarbarianRageStart(powerBarbarianInstinctivePounceTargeting));
    }

    private sealed class PowerOrSpellFinishedByMePowerBarbarianRageStart(FeatureDefinitionPower powerDummyTargeting)
        : IPowerOrSpellInitiatedByMe
    {
        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (!Main.Settings.EnableBarbarianInstinctivePounce)
            {
                yield break;
            }

            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker.GetClassLevel(Barbarian) < 7)
            {
                yield break;
            }

            yield return CampaignsContext.SelectPosition(action, powerDummyTargeting);

            var position = action.ActionParams.Positions[0];

            if (attacker.LocationPosition == position)
            {
                yield break;
            }

            var distance = (int)int3.Distance(attacker.LocationPosition, position);

            attacker.UsedTacticalMoves -= distance;

            if (attacker.UsedTacticalMoves < 0)
            {
                attacker.UsedTacticalMoves = 0;
            }

            attacker.UsedTacticalMovesChanged?.Invoke(attacker);
            attacker.MyExecuteActionTacticalMove(position);
        }
    }

    internal static void SwitchBarbarianInstinctivePounce()
    {
        Barbarian.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureBarbarianInstinctivePounce);

        if (Main.Settings.EnableBarbarianInstinctivePounce)
        {
            Barbarian.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureBarbarianInstinctivePounce, 7));
        }

        Barbarian.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchBarbarianRecklessSameBuffDebuffDuration()
    {
        RecklessAttack.GuiPresentation.description = Main.Settings.EnableBarbarianRecklessSameBuffDebuffDuration
            ? "Action/&RecklessAttackExtendedDescription"
            : "Action/&RecklessAttackDescription";
    }

    internal static void SwitchBarbarianRegainOneRageAtShortRest()
    {
        FeatureSetBarbarianRage.GuiPresentation.description = Main.Settings.EnableBarbarianRegainOneRageAtShortRest
            ? "Feature/&FeatureSetRageExtendedDescription"
            : "Feature/&FeatureSetRageDescription";
    }

    internal static void SwitchBarbarianRelentlessRage()
    {
        DamageAffinityBarbarianRelentlessRage.GuiPresentation.description = Main.Settings.EnableBarbarianRelentlessRage
            ? "Feature/&RelentlessRageExtendedDescription"
            : "Feature/&RelentlessRageDescription";
    }

    private static void LoadBarbarianPersistentRage()
    {
        var powerBarbarianPersistentRegainRagePoints = FeatureDefinitionPowerBuilder
            .Create("PowerBarbarianPersistentRegainRagePoints")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
            .SetShowCasting(false)
            .AddToDB();

        PowerBarbarianPersistentRageStart.AddCustomSubFeatures(
            new CustomBehaviorPowerBarbarianPersistentRageStart(powerBarbarianPersistentRegainRagePoints));
    }

    internal static void SwitchBarbarianPersistentRage()
    {
        if (Main.Settings.EnableBarbarianPersistentRage)
        {
            ConditionRagingNormal.SpecialInterruptions.SetRange(
                ConditionInterruption.NoAttackOrDamagedInTurn);
            ConditionRagingPersistent.durationParameter = 10;
            ConditionRagingPersistent.SpecialInterruptions.Clear();
            ConditionRagingPersistent.GuiPresentation.description = "Action/&PersistentRageStartExtendedDescription";
            PowerBarbarianPersistentRageStart.GuiPresentation.description =
                "Action/&PersistentRageStartExtendedDescription";
        }
        else
        {
            ConditionRagingNormal.SpecialInterruptions.SetRange(
                ConditionInterruption.NoAttackOrDamagedInTurn, ConditionInterruption.BattleEnd);
            ConditionRagingPersistent.durationParameter = 1;
            ConditionRagingPersistent.SpecialInterruptions.SetRange(ConditionInterruption.BattleEnd);
            ConditionRagingPersistent.GuiPresentation.description = "Action/&PersistentRageStartDescription";
            PowerBarbarianPersistentRageStart.GuiPresentation.description = "Action/&PersistentRageStartDescription";
        }
    }

    private sealed class CustomBehaviorPowerBarbarianPersistentRageStart(
        FeatureDefinitionPower powerBarbarianPersistentRegainRagePoints) : IInitiativeEndListener, IOnItemEquipped
    {
        public IEnumerator OnInitiativeEnded(GameLocationCharacter character)
        {
            if (!Main.Settings.EnableBarbarianPersistentRage)
            {
                yield break;
            }

            var rulesetCharacter = character.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerBarbarianPersistentRegainRagePoints, rulesetCharacter);

            if (rulesetCharacter.UsedRagePoints == 0 ||
                rulesetCharacter.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            yield return character.MyReactToDoNothing(
                ExtraActionId.DoNothingFree,
                character,
                "PersistentRegainRagePoints",
                "CustomReactionPersistentRegainRagePointsDescription"
                    .Formatted(Category.Reaction, rulesetCharacter.UsedRagePoints),
                ReactionValidated);

            yield break;

            void ReactionValidated()
            {
                // be silent on combat log
                usablePower.remainingUses--;
                rulesetCharacter.UsedRagePoints = 0;
                EffectHelpers.StartVisualEffect(
                    character, character, PowerDefilerEatFriends, EffectHelpers.EffectType.Caster);
            }
        }

        public void OnItemEquipped(RulesetCharacterHero hero)
        {
            if (hero.IsWearingHeavyArmor() &&
                hero.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionRagingPersistent.Name, out var activeCondition))
            {
                hero.RemoveCondition(activeCondition);
            }
        }
    }

    #endregion
}
