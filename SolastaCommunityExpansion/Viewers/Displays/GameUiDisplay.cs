﻿using System;
using System.Linq;
using System.Collections.Generic;
using ModKit;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class GameUiDisplay
    {
        private static bool DisplayBattle { get; set; }

        internal static void DisplayGameUi()
        {
            bool toggle;
            int intValue;
            float floatValue;

            UI.Label("");

            toggle = Main.Settings.AllowExtraKeyboardCharactersInNames;
            if (UI.Toggle("Allows extra keyboard characters in names", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AllowExtraKeyboardCharactersInNames = toggle;
            }

            toggle = Main.Settings.EnableCharacterExport;
            if (UI.Toggle("Enables character export from inventory screen " + "[ctrl-(E)xport]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCharacterExport = toggle;
            }

            toggle = Main.Settings.OfferAdditionalNames;
            if (UI.Toggle("Offers additional lore friendly names on character creation " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.OfferAdditionalNames = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.HideMonsterHitPoints;
            if (UI.Toggle("Displays Monsters's health in steps of 25% / 50% / 75% / 100% instead of exact hit points", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.HideMonsterHitPoints = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableInvisibleCrownOfTheMagister;
            if (UI.Toggle("Hides Crown of the Magister on game UI", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableInvisibleCrownOfTheMagister = toggle;
                ItemOptionsContext.SwitchCrownOfTheMagister();
            }

            toggle = Main.Settings.RemoveBugVisualModels;
            if (UI.Toggle("Replaces bug-like models with alternative visuals in the game " + "[must be switched on before maps are loaded] ".italic().yellow() + RequiresRestart , ref toggle, UI.AutoWidth()))
            {
                Main.Settings.RemoveBugVisualModels = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableHudToggleElementsHotkeys;
            if (UI.Toggle("Enables hotkeys to toggle HUD components visibility " + "[ctrl-(C)ontrol Panel / ctrl-(L)og / ctrl-(M)ap / ctrl-(P)arty]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHudToggleElementsHotkeys = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableInventoryFilterAndSort;
            if (UI.Toggle("Enables inventory filtering and sorting " + RequiresRestart, ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableInventoryFilterAndSort = toggle;
            }

            toggle = Main.Settings.EnableSaveByLocation;
            if (UI.Toggle("Enables save by locations / campaigns", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableSaveByLocation = toggle;
            }

            toggle = Main.Settings.InvertAltBehaviorOnTooltips;
            if (UI.Toggle("Inverts ALT key behavior on tooltips", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.InvertAltBehaviorOnTooltips = toggle;
            }

            toggle = Main.Settings.RecipeTooltipShowsRecipe;
            if (UI.Toggle("Shows crafting recipe in detailed tooltips", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.RecipeTooltipShowsRecipe = toggle;
            }

            UI.Label("");

            toggle = DisplayBattle;
            if (UI.DisclosureToggle("Battle settings:".yellow(), ref toggle, 200))
            {
                DisplayBattle = toggle;
            }

            if (DisplayBattle)
            {
                UI.Label("");

                toggle = Main.Settings.DontFollowCharacterInBattle;
                if (UI.Toggle("Battle camera doesn't follow when character is already on screen", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.DontFollowCharacterInBattle = toggle;
                }

                toggle = Main.Settings.AutoPauseOnVictory;
                if (UI.Toggle("Pauses the UI when victorious in battle", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.AutoPauseOnVictory = toggle;
                }

                toggle = Main.Settings.PermanentSpeedUp;
                if (UI.Toggle("Permanently speeds battle up", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.PermanentSpeedUp = toggle;
                }

                if (Main.Settings.DontFollowCharacterInBattle)
                {
                    intValue = Main.Settings.DontFollowMargin;
                    if (UI.Slider("Unless character is off or within % of screen edge".italic().white(), ref intValue, 0, 15, 1, "%", UI.AutoWidth()))
                    {
                        Main.Settings.DontFollowMargin = intValue;
                    }
                }

                UI.Label("");
                floatValue = Main.Settings.CustomTimeScale;
                if (UI.Slider("Battle timescale modifier".white(), ref floatValue, 1f, 50f, 1f, 1, "", UI.AutoWidth()))
                {
                    Main.Settings.CustomTimeScale = floatValue;
                }
            }

            UI.Label("");

            using (UI.HorizontalScope())
            {
                UI.Label("Empress Garb".orange() + " appearance ".white(), UI.Width(325));

                intValue = Array.IndexOf(ItemOptionsContext.EmpressGarbSkins, Main.Settings.EmpressGarbSkin);
                if (UI.SelectionGrid(ref intValue, ItemOptionsContext.EmpressGarbSkins, ItemOptionsContext.EmpressGarbSkins.Length, UI.Width(600)))
                {
                    Main.Settings.EmpressGarbSkin = ItemOptionsContext.EmpressGarbSkins[intValue];
                    ItemOptionsContext.SwitchEmpressGarb();
                }
            }

            UI.Label("");

            intValue = Main.Settings.MaxSpellLevelsPerLine;
            if (UI.Slider("Max levels per line on Spell Panel".white(), ref intValue, 3, 7, 5, "", UI.AutoWidth()))
            {
                Main.Settings.MaxSpellLevelsPerLine = intValue;
            }

            floatValue = Main.Settings.SpellPanelGapBetweenLines;
            if (UI.Slider("Gap between spell lines on Spell Panel".white(), ref floatValue, 0f, 200f, 50f, 0, "", UI.AutoWidth()))
            {
                Main.Settings.SpellPanelGapBetweenLines = floatValue;
            }
        }
    }
}
