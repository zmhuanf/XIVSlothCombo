﻿using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class DNC
    {
        public const byte JobID = 38;

        public const uint
            // Single Target
            Cascade = 15989,
            Fountain = 15990,
            ReverseCascade = 15991,
            Fountainfall = 15992,
            StarfallDance = 25792,
            // AoE
            Windmill = 15993,
            Bladeshower = 15994,
            RisingWindmill = 15995,
            Bloodshower = 15996,
            Tillana = 25790,
            // Dancing
            StandardStep = 15997,
            TechnicalStep = 15998,
            StandardFinish0 = 16003,
            StandardFinish1 = 16191,
            StandardFinish2 = 16192,
            TechnicalFinish0 = 16004,
            TechnicalFinish1 = 16193,
            TechnicalFinish2 = 16194,
            TechnicalFinish3 = 16195,
            TechnicalFinish4 = 16196,
            // Fan Dances
            FanDance1 = 16007,
            FanDance2 = 16008,
            FanDance3 = 16009,
            FanDance4 = 25791,
            // Other
            Peloton = 7557,
            SaberDance = 16005,
            EnAvant = 16010,
            Devilment = 16011,
            ShieldSamba = 16012,
            Flourish = 16013,
            Improvisation = 16014,
            CuringWaltz = 16015;

        public static class Buffs
        {
            public const ushort
                // Flourishing & Silken (Procs)
                FlourishingCascade = 1814,
                FlourishingFountain = 1815,
                FlourishingWindmill = 1816,
                FlourishingShower = 1817,
                FlourishingFanDance = 2021,
                SilkenSymmetry = 2693,
                SilkenFlow = 2694,
                FlourishingFinish = 2698,
                FlourishingStarfall = 2700,
                FlourishingSymmetry = 3017,
                FlourishingFlow = 3018,
                // Dances
                StandardStep = 1818,
                TechnicalStep = 1819,
                StandardFinish = 1821,
                TechnicalFinish = 1822,
                // Fan Dances
                ThreeFoldFanDance = 1820,
                FourFoldFanDance = 2699,
                // Other
                Peloton = 1199,
                ShieldSamba = 1826;
        }

        public static class Config
        {
            public const string
                DNCEspritThreshold_ST = "DNCEspritThreshold_ST";                            // Single target Esprit threshold
            public const string
                DNCEspritThreshold_AoE = "DNCEspritThreshold_AoE";                          // AoE Esprit threshold

            #region Simple ST Sliders
            public const string
                DNCSimpleSSBurstPercent = "DNCSimpleSSBurstPercent";                        // Standard Step    target HP% threshold
            public const string
                DNCSimpleTSBurstPercent = "DNCSimpleTSBurstPercent";                        // Technical Step   target HP% threshold
            public const string
                DNCSimpleFeatherBurstPercent = "DNCSimpleFeatherBurstPercent";              // Feather burst    target HP% threshold
            public const string
                DNCSimpleSaberThreshold = "DNCSimpleSaberThreshold";                        // Saber Dance      Esprit threshold
            public const string
                DNCSimplePanicHealWaltzPercent = "DNCSimplePanicHealWaltzPercent";          // Curing Waltz     player HP% threshold
            public const string
                DNCSimplePanicHealWindPercent = "DNCSimplePanicHealWindPercent";            // Second Wind      player HP% threshold
            #endregion

            #region Simple AoE Sliders
            public const string
                DNCSimpleSSAoEBurstPercent = "DNCSimpleSSAoEBurstPercent";                  // Standard Step    target HP% threshold
            public const string
                DNCSimpleTSAoEBurstPercent = "DNCSimpleTSAoEBurstPercent";                  // Technical Step   target HP% threshold
            public const string
                DNCSimpleAoESaberThreshold = "DNCSimpleAoESaberThreshold";                  // Saber Dance      Esprit threshold
            public const string
                DNCSimpleAoEPanicHealWaltzPercent = "DNCSimpleAoEPanicHealWaltzPercent";    // Curing Waltz     player HP% threshold 
            public const string
                DNCSimpleAoEPanicHealWindPercent = "DNCSimpleAoEPanicHealWindPercent";      // Second Wind      player HP% threshold
            #endregion

            public const string
                DNCVariantCurePercent = "DNCVariantCurePercent";                            // Variant Cure     player HP% threshold
        }

        internal class DNC_DanceComboReplacer : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_DanceComboReplacer;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (GetJobGauge<DNCGauge>().IsDancing)
                {
                    uint[]? actionIDs = Service.Configuration.DancerDanceCompatActionIDs;

                    if (actionID == actionIDs[0] || (actionIDs[0] == 0 && actionID == Cascade))     // Cascade replacement
                        return OriginalHook(Cascade);
                    if (actionID == actionIDs[1] || (actionIDs[1] == 0 && actionID == Flourish))    // Fountain replacement
                        return OriginalHook(Fountain);
                    if (actionID == actionIDs[2] || (actionIDs[2] == 0 && actionID == FanDance1))   // Reverse Cascade replacement
                        return OriginalHook(ReverseCascade);
                    if (actionID == actionIDs[3] || (actionIDs[3] == 0 && actionID == FanDance2))   // Fountainfall replacement
                        return OriginalHook(Fountainfall);
                }

                return actionID;
            }
        }

        internal class DNC_FanDanceCombos : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_FanDanceCombos;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // FD 1 --> 3, FD 1 --> 4
                if (actionID is FanDance1)
                {
                    if (IsEnabled(CustomComboPreset.DNC_FanDance_1to3_Combo) &&
                        HasEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;
                    if (IsEnabled(CustomComboPreset.DNC_FanDance_1to4_Combo) &&
                        HasEffect(Buffs.FourFoldFanDance))
                        return FanDance4;
                }

                // FD 2 --> 3, FD 2 --> 4
                if (actionID is FanDance2)
                {
                    if (IsEnabled(CustomComboPreset.DNC_FanDance_2to3_Combo) &&
                        HasEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;
                    if (IsEnabled(CustomComboPreset.DNC_FanDance_2to4_Combo) &&
                        HasEffect(Buffs.FourFoldFanDance))
                        return FanDance4;
                }

                return actionID;
            }
        }

        internal class DNC_DanceStepCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_DanceStepCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DNCGauge? gauge = GetJobGauge<DNCGauge>();

                // Standard Step
                if (actionID is StandardStep && gauge.IsDancing && HasEffect(Buffs.StandardStep))
                    return gauge.CompletedSteps < 2
                        ? gauge.NextStep
                        : StandardFinish2;

                // Technical Step
                if ((actionID is TechnicalStep) && gauge.IsDancing && HasEffect(Buffs.TechnicalStep))
                    return gauge.CompletedSteps < 4
                        ? gauge.NextStep
                        : TechnicalFinish4;

                return actionID;
            }
        }

        internal class DNC_FlourishingFanDances : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_FlourishingFanDances;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // Fan Dance 3 & 4 on Flourish
                if (actionID is Flourish && CanWeave(actionID))
                {
                    if (HasEffect(Buffs.ThreeFoldFanDance))
                        return FanDance3;
                    if (HasEffect(Buffs.FourFoldFanDance))
                        return FanDance4;
                }

                return actionID;
            }
        }

        internal class DNC_ST_MultiButton : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_ST_MultiButton;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Cascade)
                {
                    #region Types
                    DNCGauge? gauge = GetJobGauge<DNCGauge>();
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    // ST Esprit overcap protection
                    if (IsEnabled(CustomComboPreset.DNC_ST_EspritOvercap) && LevelChecked(SaberDance) &&
                        gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCEspritThreshold_ST))
                        return SaberDance;

                    if (CanWeave(actionID))
                    {
                        // ST Fan Dance overcap protection
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDanceOvercap) &&
                            LevelChecked(FanDance1) && gauge.Feathers is 4)
                            return FanDance1;

                        // ST Fan Dance 3/4 on combo
                        if (IsEnabled(CustomComboPreset.DNC_ST_FanDance34))
                        {
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;
                            if (HasEffect(Buffs.FourFoldFanDance))
                                return FanDance4;
                        }
                    }

                    // ST base combos
                    if (LevelChecked(Fountainfall) && flow)
                        return Fountainfall;
                    if (LevelChecked(ReverseCascade) && symmetry)
                        return ReverseCascade;
                    if (LevelChecked(Fountain) && lastComboMove is Cascade)
                        return Fountain;
                }

                return actionID;
            }
        }

        internal class DNC_AoE_MultiButton : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_AoE_MultiButton;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Windmill)
                {
                    #region Types
                    DNCGauge? gauge = GetJobGauge<DNCGauge>();
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    // AoE Esprit overcap protection
                    if (IsEnabled(CustomComboPreset.DNC_AoE_EspritOvercap) && LevelChecked(SaberDance) &&
                        gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCEspritThreshold_AoE))
                        return SaberDance;

                    if (CanWeave(actionID))
                    {
                        // AoE Fan Dance overcap protection
                        if (IsEnabled(CustomComboPreset.DNC_AoE_FanDanceOvercap) &&
                            LevelChecked(FanDance2) && gauge.Feathers is 4)
                            return FanDance2;

                        // AoE Fan Dance 3/4 on combo
                        if (IsEnabled(CustomComboPreset.DNC_AoE_FanDance34))
                        {
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;
                            if (HasEffect(Buffs.FourFoldFanDance))
                                return FanDance4;
                        }
                    }

                    // AoE base combos
                    if (LevelChecked(Bloodshower) && flow)
                        return Bloodshower;
                    if (LevelChecked(RisingWindmill) && symmetry)
                        return RisingWindmill;
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill)
                        return Bladeshower;
                }

                return actionID;
            }
        }

        internal class DNC_Starfall_Devilment : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_Starfall_Devilment;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Devilment && HasEffect(Buffs.FlourishingStarfall)
                    ? StarfallDance
                    : actionID;
        }

        internal class DNC_CombinedDances : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_CombinedDances;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // One-button mode for both dances (SS/TS). SS takes priority.
                if (actionID is StandardStep)
                {
                    DNCGauge? gauge = GetJobGauge<DNCGauge>();

                    // Devilment
                    if (IsEnabled(CustomComboPreset.DNC_CombinedDances_Devilment) && IsOnCooldown(StandardStep) && IsOffCooldown(Devilment) && !gauge.IsDancing)
                    {
                        if ((LevelChecked(Devilment) && !LevelChecked(TechnicalStep)) ||    // Lv. 62 - 69
                            (LevelChecked(TechnicalStep) && IsOnCooldown(TechnicalStep)))   // Lv. 70+ during Tech
                            return Devilment;
                    }

                    // Flourish
                    if (IsEnabled(CustomComboPreset.DNC_CombinedDances_Flourish) &&
                        InCombat() && !gauge.IsDancing &&
                        ActionReady(Flourish) &&
                        IsOnCooldown(StandardStep))
                        return Flourish;

                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // Tech Step
                    if (IsOnCooldown(StandardStep) && IsOffCooldown(TechnicalStep) &&
                        !gauge.IsDancing && !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // Dance steps
                    if (gauge.IsDancing)
                    {
                        if (HasEffect(Buffs.StandardStep))
                        {
                            return gauge.CompletedSteps < 2
                                ? gauge.NextStep
                                : StandardFinish2;
                        }

                        if (HasEffect(Buffs.TechnicalStep))
                        {
                            return gauge.CompletedSteps < 4
                                ? gauge.NextStep
                                : TechnicalFinish4;
                        }
                    }
                }

                return actionID;
            }
        }

        internal class DNC_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_ST_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Cascade)
                {
                    if (CurrentTarget == null)
                    {
                        return actionID;
                    }

                    #region Types
                    DNCGauge? gauge = GetJobGauge<DNCGauge>();
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    #region Pre-pull
                    // ST Peloton
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Peloton) &&
                        !InCombat() && !HasEffectAny(Buffs.Peloton) && GetBuffRemainingTime(Buffs.StandardStep) > 5)
                        return Peloton;
                    #endregion

                    #region Dance Fills
                    // ST Standard (Dance) Steps & Fill
                    if ((IsEnabled(CustomComboPreset.DNC_ST_Simple_SS) || IsEnabled(CustomComboPreset.DNC_ST_Simple_StandardFill)) &&
                        HasEffect(Buffs.StandardStep))
                        return gauge.CompletedSteps < 2
                            ? gauge.NextStep
                            : StandardFinish2;

                    // ST Technical (Dance) Steps & Fill
                    if ((IsEnabled(CustomComboPreset.DNC_ST_Simple_TS) || IsEnabled(CustomComboPreset.DNC_ST_Simple_TechFill)) &&
                        HasEffect(Buffs.TechnicalStep))
                        return gauge.CompletedSteps < 4
                            ? gauge.NextStep
                            : TechnicalFinish4;
                    #endregion

                    #region Weaves
                    // ST Devilment
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Devilment) &&
                        CanWeave(actionID) && ActionReady(Devilment) &&
                        (HasEffect(Buffs.TechnicalFinish) || !LevelChecked(TechnicalStep)))
                        return Devilment;

                    // ST Flourish
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Flourish) &&
                        CanDelayedWeave(actionID, 1.25, 0.5) && ActionReady(Flourish) &&
                        !HasEffect(Buffs.ThreeFoldFanDance) && !HasEffect(Buffs.FourFoldFanDance) &&
                        !HasEffect(Buffs.FlourishingSymmetry) && !HasEffect(Buffs.FlourishingFlow))
                        return Flourish;

                    // ST Interrupt
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Interrupt) &&
                        CanInterruptEnemy() && ActionReady(All.HeadGraze) &&
                        !HasEffect(Buffs.TechnicalFinish))
                        return All.HeadGraze;

                    if (IsEnabled(CustomComboPreset.DNC_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.DNCVariantCurePercent))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DNC_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (CanWeave(actionID))
                    {
                        // ST Feathers & Fans
                        if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Feathers) && LevelChecked(FanDance1))
                        {
                            // FD3
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            // FD1 HP% Dump
                            if (GetTargetHPPercent() <= PluginConfiguration.GetCustomIntValue(Config.DNCSimpleFeatherBurstPercent) && gauge.Feathers > 0)
                                return FanDance1;

                            if (LevelChecked(TechnicalStep))
                            {
                                // Burst FD1
                                if (HasEffect(Buffs.TechnicalFinish) && gauge.Feathers > 0)
                                    return FanDance1;

                                // FD1 Pooling
                                if (gauge.Feathers > 3 &&
                                    (GetCooldownRemainingTime(TechnicalStep) > 2.5f || IsOffCooldown(TechnicalStep)))
                                    return FanDance1;
                            }

                            // FD1 Non-pooling & under burst level
                            if (!LevelChecked(TechnicalStep) && gauge.Feathers > 0)
                                return FanDance1;

                        }

                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;

                        // ST Panic Heals
                        if (IsEnabled(CustomComboPreset.DNC_ST_Simple_PanicHeals))
                        {
                            if (ActionReady(CuringWaltz) &&
                                PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNCSimplePanicHealWaltzPercent))
                                return CuringWaltz;

                            if (ActionReady(All.SecondWind) &&
                                PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNCSimplePanicHealWindPercent))
                                return All.SecondWind;
                        }

                        // ST Improvisation
                        if (IsEnabled(CustomComboPreset.DNC_ST_Simple_Improvisation) &&
                            ActionReady(Improvisation))
                            return Improvisation;
                    }
                    #endregion

                    #region GCD
                    // ST Standard Step (outside of burst)
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_SS) && ActionReady(StandardStep) && !HasEffect(Buffs.TechnicalFinish))
                    {
                        if (((!HasTarget() || GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSSBurstPercent)) &&
                            ((IsOffCooldown(TechnicalStep) && !InCombat()) || GetCooldownRemainingTime(TechnicalStep) > 5) &&
                            (IsOffCooldown(Flourish) || (GetCooldownRemainingTime(Flourish) > 5))) ||
                            IsOffCooldown(StandardStep))
                            return StandardStep;
                    }

                    // ST Technical Step
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_TS) && ActionReady(TechnicalStep) &&
                        (!HasTarget() || GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNCSimpleTSBurstPercent)) &&
                        !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // ST Saber Dance
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_SaberDance) && LevelChecked(SaberDance) &&
                        (GetCooldownRemainingTime(TechnicalStep) > 5 || IsOffCooldown(TechnicalStep)))
                    {
                        if (gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSaberThreshold) ||
                            (HasEffect(Buffs.TechnicalFinish) && gauge.Esprit >= 50))
                            return SaberDance;
                    }

                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // ST combos and burst attacks
                    if (LevelChecked(Fountain) && lastComboMove is Cascade && comboTime is < 2 and > 0)
                        return Fountain;

                    // ST Standard Step (inside of burst)
                    if (IsEnabled(CustomComboPreset.DNC_ST_Simple_SS) && IsOffCooldown(StandardStep) && HasEffect(Buffs.TechnicalFinish))
                    {
                        if (GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSSBurstPercent) &&
                            (GetBuffRemainingTime(Buffs.TechnicalFinish) > 5))
                            return StandardStep;
                    }

                    if (LevelChecked(Fountainfall) && flow)
                        return Fountainfall;
                    if (LevelChecked(ReverseCascade) && symmetry)
                        return ReverseCascade;
                    if (LevelChecked(Fountain) && lastComboMove is Cascade && comboTime > 0)
                        return Fountain;
                    #endregion
                }

                return actionID;
            }
        }

        internal class DNC_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNC_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Windmill)
                {
                    if (CurrentTarget == null)
                    {
                        return actionID;
                    }

                    #region Types
                    DNCGauge? gauge = GetJobGauge<DNCGauge>();
                    bool flow = HasEffect(Buffs.SilkenFlow) || HasEffect(Buffs.FlourishingFlow);
                    bool symmetry = HasEffect(Buffs.SilkenSymmetry) || HasEffect(Buffs.FlourishingSymmetry);
                    #endregion

                    #region Dance Fills
                    // AoE Standard (Dance) Steps & Fill
                    if ((IsEnabled(CustomComboPreset.DNC_AoE_Simple_SS) || IsEnabled(CustomComboPreset.DNC_AoE_Simple_StandardFill)) &&
                        HasEffect(Buffs.StandardStep))
                        return gauge.CompletedSteps < 2
                            ? gauge.NextStep
                            : StandardFinish2;

                    // AoE Technical (Dance) Steps & Fill
                    if ((IsEnabled(CustomComboPreset.DNC_AoE_Simple_TS) || IsEnabled(CustomComboPreset.DNC_AoE_Simple_TechFill)) &&
                        HasEffect(Buffs.TechnicalStep))
                        return gauge.CompletedSteps < 4
                            ? gauge.NextStep
                            : TechnicalFinish4;
                    #endregion

                    #region Weaves
                    // AoE Devilment
                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Devilment) &&
                        CanWeave(actionID) && ActionReady(Devilment) &&
                        (HasEffect(Buffs.TechnicalFinish) || !LevelChecked(TechnicalStep)))
                        return Devilment;

                    // AoE Flourish
                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Flourish) &&
                        CanDelayedWeave(actionID, 1.25, 0.5) && ActionReady(Flourish)
                        && !HasEffect(Buffs.ThreeFoldFanDance) && !HasEffect(Buffs.FourFoldFanDance) &&
                        !HasEffect(Buffs.FlourishingSymmetry) && !HasEffect(Buffs.FlourishingFlow))
                        return Flourish;

                    // AoE Interrupt
                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Interrupt) &&
                        CanInterruptEnemy() && ActionReady(All.HeadGraze) &&
                        !HasEffect(Buffs.TechnicalFinish))
                        return All.HeadGraze;

                    if (IsEnabled(CustomComboPreset.DNC_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.DNCVariantCurePercent))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DNC_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    if (CanWeave(actionID))
                    {
                        /*
                        // AoE Feathers & Fans
                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Feathers) && LevelChecked(FanDance1))
                        {
                            int minFeathers = IsEnabled(CustomComboPreset.DNC_AoE_Simple_FeatherPooling) && LevelChecked(TechnicalStep)
                                ? (GetCooldownRemainingTime(TechnicalStep) < 2.5f ? 4 : 3)
                                : 0;

                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            if ((gauge.Feathers > minFeathers ||
                                (HasEffect(Buffs.TechnicalFinish) && gauge.Feathers > 0)) &&
                                LevelChecked(FanDance2))
                                return FanDance2;
                        }
                        */

                        // AoE Feathers & Fans
                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Feathers) && LevelChecked(FanDance1))
                        {
                            // FD3
                            if (HasEffect(Buffs.ThreeFoldFanDance))
                                return FanDance3;

                            if (LevelChecked(FanDance2))
                            {
                                if (LevelChecked(TechnicalStep))
                                {
                                    // Burst FD2
                                    if (HasEffect(Buffs.TechnicalFinish) && gauge.Feathers > 0)
                                        return FanDance2;

                                    // FD2 Pooling
                                    if (gauge.Feathers > 3 &&
                                        (GetCooldownRemainingTime(TechnicalStep) > 2.5f || IsOffCooldown(TechnicalStep)))
                                        return FanDance2;
                                }

                                // FD2 Non-pooling & under burst level
                                if (!LevelChecked(TechnicalStep) && gauge.Feathers > 0)
                                    return FanDance2;
                            }

                            // FD1 Replacement for Lv.30-49
                            if (!LevelChecked(FanDance2) && gauge.Feathers > 0)
                                return FanDance1;
                        }

                        if (HasEffect(Buffs.FourFoldFanDance))
                            return FanDance4;

                        // AoE Panic Heals
                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_PanicHeals))
                        {
                            if (ActionReady(CuringWaltz) &&
                                PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNCSimpleAoEPanicHealWaltzPercent))
                                return CuringWaltz;

                            if (ActionReady(All.SecondWind) &&
                                PlayerHealthPercentageHp() < PluginConfiguration.GetCustomIntValue(Config.DNCSimpleAoEPanicHealWindPercent))
                                return All.SecondWind;
                        }

                        // AoE Improvisation
                        if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_Improvisation) &&
                            ActionReady(Improvisation))
                            return Improvisation;
                    }
                    #endregion

                    #region GCD
                    // AoE Standard Step (outside of burst)
                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_SS) && ActionReady(StandardStep) && !HasEffect(Buffs.TechnicalFinish))
                    {
                        if (((!HasTarget() || GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSSAoEBurstPercent)) &&
                            ((IsOffCooldown(TechnicalStep) && !InCombat()) || GetCooldownRemainingTime(TechnicalStep) > 5) &&
                            (IsOffCooldown(Flourish) || (GetCooldownRemainingTime(Flourish) > 5))) ||
                            IsOffCooldown(StandardStep))
                            return StandardStep;
                    }

                    // AoE Technical Step
                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_TS) && ActionReady(TechnicalStep) &&
                        (!HasTarget() || GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNCSimpleTSAoEBurstPercent)) &&
                        !HasEffect(Buffs.StandardStep))
                        return TechnicalStep;

                    // AoE Saber Dance
                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_SaberDance) && LevelChecked(SaberDance) &&
                        (GetCooldownRemainingTime(TechnicalStep) > 5 || IsOffCooldown(TechnicalStep)))
                    {
                        if (gauge.Esprit >= PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSaberThreshold) ||
                            (HasEffect(Buffs.TechnicalFinish) && gauge.Esprit >= 50))
                            return SaberDance;
                    }

                    if (HasEffect(Buffs.FlourishingStarfall))
                        return StarfallDance;
                    if (HasEffect(Buffs.FlourishingFinish))
                        return Tillana;

                    // AoE combos and burst attacks
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill && comboTime is < 2 and > 0)
                        return Bladeshower;

                    // AoE Standard Step (inside of burst)
                    if (IsEnabled(CustomComboPreset.DNC_AoE_Simple_SS) && IsOffCooldown(StandardStep) && HasEffect(Buffs.TechnicalFinish))
                    {
                        if (GetTargetHPPercent() > PluginConfiguration.GetCustomIntValue(Config.DNCSimpleSSAoEBurstPercent) &&
                            (GetBuffRemainingTime(Buffs.TechnicalFinish) > 5))
                            return StandardStep;
                    }

                    if (LevelChecked(Bloodshower) && flow)
                        return Bloodshower;
                    if (LevelChecked(RisingWindmill) && symmetry)
                        return RisingWindmill;
                    if (LevelChecked(Bladeshower) && lastComboMove is Windmill && comboTime > 0)
                        return Bladeshower;
                    #endregion
                }

                return actionID;
            }
        }
    }
}
