using System;
using System.Collections;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Interface.Animation;
using Lumina.Excel.GeneratedSheets;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Data;
using static System.Net.Mime.MediaTypeNames;
using Status = Dalamud.Game.ClientState.Statuses.Status;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class BRD
    {
        public const byte ClassID = 5;
        public const byte JobID = 23;

        public const uint
            HeavyShot = 97,
            StraightShot = 98,
            VenomousBite = 100,
            RagingStrikes = 101,
            QuickNock = 106,
            Barrage = 107,
            Bloodletter = 110,
            Windbite = 113,
            MagesBallad = 114,
            ArmysPaeon = 116,
            RainOfDeath = 117,
            BattleVoice = 118,
            EmpyrealArrow = 3558,
            WanderersMinuet = 3559,
            IronJaws = 3560,
            Sidewinder = 3562,
            PitchPerfect = 7404,
            Troubadour = 7405,
            CausticBite = 7406,
            Stormbite = 7407,
            RefulgentArrow = 7409,
            BurstShot = 16495,
            ApexArrow = 16496,
            Shadowbite = 16494,
            Ladonsbite = 25783,
            BlastArrow = 25784,
            RadiantFinale = 25785;

        public static class Buffs
        {
            public const ushort
                StraightShotReady = 122,
                RagingStrikes = 125,
                Barrage = 128,
                MagesBallad = 135,
                ArmysPaeon = 137,
                BattleVoice = 141,
                WanderersMinuet = 865,
                Troubadour = 1934,
                BlastArrowReady = 2692,
                RadiantFinale = 2722,
                ShadowbiteReady = 3002;
        }

        public static class Debuffs
        {
            public const ushort
                VenomousBite = 124,
                Windbite = 129,
                CausticBite = 1200,
                Stormbite = 1201;
        }

        public static class Config
        {
            public const string
                BRD_RagingJawsRenewTime = "ragingJawsRenewTime",
                BRD_NoWasteHPPercentage = "noWasteHpPercentage",
                BRD_STSecondWindThreshold = "BRD_STSecondWindThreshold",
                BRD_AoESecondWindThreshold = "BRD_AoESecondWindThreshold",
                BRD_VariantCure = "BRD_VariantCure";
        }

        #region Song status
        internal static bool SongIsNotNone(Song value) => value != Song.NONE;
        internal static bool SongIsNone(Song value) => value == Song.NONE;
        internal static bool SongIsWandererMinuet(Song value) => value == Song.WANDERER;
        #endregion

        // Replace HS/BS with SS/RA when procced.
        internal class BRD_StraightShotUpgrade : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_StraightShotUpgrade;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HeavyShot or BurstShot)
                {
                    if (IsEnabled(CustomComboPreset.BRD_Apex))
                    {
                        BRDGauge? gauge = GetJobGauge<BRDGauge>();

                        if (!IsEnabled(CustomComboPreset.BRD_RemoveApexArrow) && gauge.SoulVoice == 100)
                            return ApexArrow;
                        if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                            return BlastArrow;
                    }

                    if (IsEnabled(CustomComboPreset.BRD_DoTMaintainance))
                    {
                        bool venomous = TargetHasEffect(Debuffs.VenomousBite);
                        bool windbite = TargetHasEffect(Debuffs.Windbite);
                        bool caustic = TargetHasEffect(Debuffs.CausticBite);
                        bool stormbite = TargetHasEffect(Debuffs.Stormbite);
                        float venomRemaining = GetDebuffRemainingTime(Debuffs.VenomousBite);
                        float windRemaining = GetDebuffRemainingTime(Debuffs.Windbite);
                        float causticRemaining = GetDebuffRemainingTime(Debuffs.CausticBite);
                        float stormRemaining = GetDebuffRemainingTime(Debuffs.Stormbite);

                        if (InCombat())
                        {
                            if (LevelChecked(IronJaws) &&
                                ((venomous && venomRemaining < 4) || (caustic && causticRemaining < 4)) ||
                                (windbite && windRemaining < 4) || (stormbite && stormRemaining < 4))
                                return IronJaws;
                            if (!LevelChecked(IronJaws) && venomous && venomRemaining < 4)
                                return VenomousBite;
                            if (!LevelChecked(IronJaws) && windbite && windRemaining < 4)
                                return Windbite;
                        }
                    }

                    if (HasEffect(Buffs.StraightShotReady))
                        return LevelChecked(RefulgentArrow)
                            ? RefulgentArrow
                            : StraightShot;
                }

                return actionID;
            }
        }

        internal class BRD_IronJaws : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_IronJaws;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is IronJaws)
                {
                    if (IsEnabled(CustomComboPreset.BRD_IronJawsApex) && LevelChecked(ApexArrow))
                    {
                        BRDGauge? gauge = GetJobGauge<BRDGauge>();

                        if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                            return BlastArrow;
                        if (gauge.SoulVoice == 100 && IsOffCooldown(ApexArrow))
                            return ApexArrow;
                    }

                    if (!LevelChecked(IronJaws))
                    {
                        Status? venomous = FindTargetEffect(Debuffs.VenomousBite);
                        Status? windbite = FindTargetEffect(Debuffs.Windbite);
                        float venomRemaining = GetDebuffRemainingTime(Debuffs.VenomousBite);
                        float windRemaining = GetDebuffRemainingTime(Debuffs.Windbite);

                        if (venomous is not null && windbite is not null)
                        {
                            if (LevelChecked(VenomousBite) && venomRemaining < windRemaining)
                                return VenomousBite;
                            if (LevelChecked(Windbite))
                                return Windbite;
                        }

                        if (LevelChecked(VenomousBite) && (!LevelChecked(Windbite) || windbite is not null))
                            return VenomousBite;
                        if (LevelChecked(Windbite))
                            return Windbite;
                    }

                    if (!LevelChecked(Stormbite))
                    {
                        bool venomous = TargetHasEffect(Debuffs.VenomousBite);
                        bool windbite = TargetHasEffect(Debuffs.Windbite);

                        if (LevelChecked(IronJaws) && venomous && windbite)
                            return IronJaws;
                        if (LevelChecked(VenomousBite) && windbite)
                            return VenomousBite;
                        if (LevelChecked(Windbite))
                            return Windbite;
                    }

                    bool caustic = TargetHasEffect(Debuffs.CausticBite);
                    bool stormbite = TargetHasEffect(Debuffs.Stormbite);

                    if (LevelChecked(IronJaws) && caustic && stormbite)
                        return IronJaws;
                    if (LevelChecked(CausticBite) && stormbite)
                        return CausticBite;
                    if (LevelChecked(Stormbite))
                        return Stormbite;
                }

                return actionID;
            }
        }

        internal class BRD_IronJaws_Alternate : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_IronJaws_Alternate;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is IronJaws)
                {
                    if (!LevelChecked(IronJaws))
                    {
                        Status? venomous = FindTargetEffect(Debuffs.VenomousBite);
                        Status? windbite = FindTargetEffect(Debuffs.Windbite);

                        if (venomous is not null && windbite is not null)
                        {
                            float venomRemaining = GetDebuffRemainingTime(Debuffs.VenomousBite);
                            float windRemaining = GetDebuffRemainingTime(Debuffs.Windbite);

                            if (LevelChecked(VenomousBite) && venomRemaining < windRemaining)
                                return VenomousBite;
                            if (LevelChecked(Windbite))
                                return Windbite;
                        }

                        if (LevelChecked(VenomousBite) && (!LevelChecked(Windbite) || windbite is not null))
                            return VenomousBite;
                        if (LevelChecked(Windbite))
                            return Windbite;
                    }

                    if (!LevelChecked(Stormbite))
                    {
                        bool venomous = TargetHasEffect(Debuffs.VenomousBite);
                        bool windbite = TargetHasEffect(Debuffs.Windbite);
                        float venomRemaining = GetDebuffRemainingTime(Debuffs.VenomousBite);
                        float windRemaining = GetDebuffRemainingTime(Debuffs.Windbite);

                        if (LevelChecked(IronJaws) && venomous && windbite &&
                            (venomRemaining < 4 || windRemaining < 4))
                            return IronJaws;
                        if (LevelChecked(VenomousBite) && windbite)
                            return VenomousBite;
                        if (LevelChecked(Windbite))
                            return Windbite;
                    }

                    bool caustic = TargetHasEffect(Debuffs.CausticBite);
                    bool stormbite = TargetHasEffect(Debuffs.Stormbite);
                    float causticRemaining = GetDebuffRemainingTime(Debuffs.CausticBite);
                    float stormRemaining = GetDebuffRemainingTime(Debuffs.Stormbite);

                    if (LevelChecked(IronJaws) && caustic && stormbite &&
                        (causticRemaining < 4 || stormRemaining < 4))
                        return IronJaws;
                    if (LevelChecked(CausticBite) && stormbite)
                        return CausticBite;
                    if (LevelChecked(Stormbite))
                        return Stormbite;
                }

                return actionID;
            }
        }

        internal class BRD_Apex : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_Apex;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is QuickNock)
                {
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();

                    if (!IsEnabled(CustomComboPreset.BRD_RemoveApexArrow) && LevelChecked(ApexArrow) && gauge.SoulVoice == 100)
                        return ApexArrow;
                }

                return actionID;
            }
        }

        internal class BRD_AoE_oGCD : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_oGCD;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is RainOfDeath)
                {
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    bool songWanderer = gauge.Song == Song.WANDERER;
                    bool empyrealReady = LevelChecked(EmpyrealArrow) && IsOffCooldown(EmpyrealArrow);
                    bool bloodletterReady = LevelChecked(Bloodletter) && IsOffCooldown(Bloodletter);
                    bool sidewinderReady = LevelChecked(Sidewinder) && IsOffCooldown(Sidewinder);

                    if (LevelChecked(WanderersMinuet) && songWanderer && gauge.Repertoire == 3)
                        return OriginalHook(WanderersMinuet);
                    if (empyrealReady)
                        return EmpyrealArrow;
                    if (bloodletterReady)
                        return RainOfDeath;
                    if (sidewinderReady)
                        return Sidewinder;
                }

                return actionID;
            }
        }

        internal class BRD_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Ladonsbite or QuickNock)
                {
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    bool canWeave = CanWeave(actionID);

                    if (IsEnabled(CustomComboPreset.BRD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.BRD_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.BRD_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        canWeave)
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.BRD_AoE_Simple_Songs) && canWeave)
                    {
                        int songTimerInSeconds = gauge.SongTimer / 1000;
                        bool songNone = gauge.Song == Song.NONE;

                        if (songTimerInSeconds < 3 || songNone)
                        {
                            if (LevelChecked(WanderersMinuet) && IsOffCooldown(WanderersMinuet) &&
                                !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)) &&
                                !IsEnabled(CustomComboPreset.BRD_AoE_Simple_SongsExcludeWM))
                                return WanderersMinuet;

                            if (LevelChecked(MagesBallad) && IsOffCooldown(MagesBallad) &&
                                !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                                return MagesBallad;

                            if (LevelChecked(ArmysPaeon) && IsOffCooldown(ArmysPaeon) &&
                                !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                                return ArmysPaeon;
                        }
                    }

                    if (canWeave)
                    {
                        bool songWanderer = gauge.Song == Song.WANDERER;
                        bool empyrealReady = LevelChecked(EmpyrealArrow) && IsOffCooldown(EmpyrealArrow);
                        bool rainOfDeathReady = LevelChecked(RainOfDeath) && GetRemainingCharges(RainOfDeath) > 0;
                        bool sidewinderReady = LevelChecked(Sidewinder) && IsOffCooldown(Sidewinder);

                        if (LevelChecked(PitchPerfect) && songWanderer && gauge.Repertoire == 3)
                            return OriginalHook(WanderersMinuet);
                        if (empyrealReady)
                            return EmpyrealArrow;
                        if (rainOfDeathReady)
                            return RainOfDeath;
                        if (sidewinderReady)
                            return Sidewinder;

                        // healing - please move if not appropriate priority
                        if (IsEnabled(CustomComboPreset.BRD_AoE_SecondWind))
                        {
                            if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.BRD_AoESecondWindThreshold) && ActionReady(All.SecondWind))
                                return All.SecondWind;
                        }
                    }

                    bool shadowbiteReady = LevelChecked(Shadowbite) && HasEffect(Buffs.ShadowbiteReady);
                    bool blastArrowReady = LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady);

                    if (shadowbiteReady)
                        return Shadowbite;
                    if (LevelChecked(ApexArrow) && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BRD_RemoveApexArrow))
                        return ApexArrow;
                    if (blastArrowReady)
                        return BlastArrow;
                }

                return actionID;
            }
        }

        internal class BRD_ST_oGCD : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_oGCD;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Bloodletter)
                {
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    bool songArmy = gauge.Song == Song.ARMY;
                    bool songWanderer = gauge.Song == Song.WANDERER;
                    bool minuetReady = LevelChecked(WanderersMinuet) && IsOffCooldown(WanderersMinuet);
                    bool balladReady = LevelChecked(MagesBallad) && IsOffCooldown(MagesBallad);
                    bool paeonReady = LevelChecked(ArmysPaeon) && IsOffCooldown(ArmysPaeon);
                    bool empyrealReady = LevelChecked(EmpyrealArrow) && IsOffCooldown(EmpyrealArrow);
                    bool bloodletterReady = LevelChecked(Bloodletter) && IsOffCooldown(Bloodletter);
                    bool sidewinderReady = LevelChecked(Sidewinder) && IsOffCooldown(Sidewinder);

                    if (IsEnabled(CustomComboPreset.BRD_oGCDSongs) &&
                        (gauge.SongTimer < 1 || songArmy))
                    {
                        if (minuetReady)
                            return WanderersMinuet;
                        if (balladReady)
                            return MagesBallad;
                        if (paeonReady)
                            return ArmysPaeon;
                    }

                    if (songWanderer && gauge.Repertoire == 3)
                        return OriginalHook(WanderersMinuet);
                    if (empyrealReady)
                        return EmpyrealArrow;
                    if (bloodletterReady)
                        return Bloodletter;
                    if (sidewinderReady)
                        return Sidewinder;
                }

                return actionID;
            }
        }
        internal class BRD_AoE_Combo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_AoE_Combo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is QuickNock or Ladonsbite)
                {
                    if (IsEnabled(CustomComboPreset.BRD_Apex))
                    {
                        BRDGauge? gauge = GetJobGauge<BRDGauge>();
                        bool blastReady = LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady);

                        if (LevelChecked(ApexArrow) && gauge.SoulVoice == 100 && !IsEnabled(CustomComboPreset.BRD_RemoveApexArrow))
                            return ApexArrow;
                        if (blastReady)
                            return BlastArrow;
                    }

                    bool shadowbiteReady = LevelChecked(Shadowbite) && HasEffectAny(Buffs.ShadowbiteReady);

                    if (IsEnabled(CustomComboPreset.BRD_AoE_Combo) && shadowbiteReady)
                        return Shadowbite;
                }

                return actionID;
            }
        }
        internal class BRD_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_ST_SimpleMode;
            internal static bool inOpener = false;
            internal static bool openerFinished = false;
            internal static byte step = 0;
            internal static byte subStep = 0;
            internal static bool usedStraightShotReady = false;
            internal static bool usedPitchPerfect = false;
            internal delegate bool DotRecast(int value);

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HeavyShot or BurstShot)
                {
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    bool canWeave = CanWeave(actionID);
                    bool canWeaveBuffs = CanWeave(actionID, 0.6);
                    bool canWeaveDelayed = CanDelayedWeave(actionID, 0.9);
                    bool songNone = gauge.Song == Song.NONE;
                    bool songWanderer = gauge.Song == Song.WANDERER;
                    bool songMage = gauge.Song == Song.MAGE;
                    bool songArmy = gauge.Song == Song.ARMY;
                    bool canInterrupt = CanInterruptEnemy() && IsOffCooldown(All.HeadGraze);
                    int targetHPThreshold = PluginConfiguration.GetCustomIntValue(Config.BRD_NoWasteHPPercentage);
                    bool isEnemyHealthHigh = IsEnabled(CustomComboPreset.BRD_Simple_NoWaste)
                        ? GetTargetHPPercent() > targetHPThreshold
                        : true;

                    if (!InCombat() && (inOpener || openerFinished))
                    {
                        openerFinished = false;
                    }

                    if (!IsEnabled(CustomComboPreset.BRD_Simple_NoWaste))
                        openerFinished = true;

                    if (IsEnabled(CustomComboPreset.BRD_Simple_Interrupt) && canInterrupt)
                        return All.HeadGraze;

                    if (IsEnabled(CustomComboPreset.BRD_Variant_Cure) && IsEnabled(Variant.VariantCure) && PlayerHealthPercentageHp() <= GetOptionValue(Config.BRD_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.BRD_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        canWeave)
                        return Variant.VariantRampart;

                    if (IsEnabled(CustomComboPreset.BRD_Simple_Song) && isEnemyHealthHigh)
                    {
                        int songTimerInSeconds = gauge.SongTimer / 1000;

                        // Limit optimisation to when you are high enough level to benefit from it.
                        if (LevelChecked(WanderersMinuet))
                        {
                            // 43s of Wanderer's Minute, ~36s of Mage's Ballad, and ~43s of Army's Paeon    
                            bool minuetReady = IsOffCooldown(WanderersMinuet);
                            bool balladReady = IsOffCooldown(MagesBallad);
                            bool paeonReady = IsOffCooldown(ArmysPaeon);

                            if (canWeave)
                            {
                                if (songNone)
                                {
                                    // Logic to determine first song
                                    if (minuetReady && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                                        return WanderersMinuet;
                                    if (balladReady && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                                        return MagesBallad;
                                    if (paeonReady && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                                        return ArmysPaeon;
                                }

                                if (songWanderer)
                                {
                                    if (songTimerInSeconds < 3 && gauge.Repertoire > 0) // Spend any repertoire before switching to next song
                                        return OriginalHook(WanderersMinuet);
                                    if (songTimerInSeconds < 3 && balladReady)          // Move to Mage's Ballad if < 3 seconds left on song
                                        return MagesBallad;
                                }

                                if (songMage)
                                {
                                    bool empyrealReady = LevelChecked(EmpyrealArrow) && IsOffCooldown(EmpyrealArrow);

                                    // Move to Army's Paeon if < 12 seconds left on song
                                    if (songTimerInSeconds < 12 && paeonReady)
                                    {
                                        // Special case for Empyreal Arrow: it must be cast before you change to it to avoid drift!
                                        if (empyrealReady)
                                            return EmpyrealArrow;
                                        return ArmysPaeon;
                                    }
                                }
                            }

                            if (songArmy && canWeaveDelayed)
                            {
                                // Move to Wanderer's Minuet if < 3 seconds left on song or WM off CD and have 4 repertoires of AP
                                if (songTimerInSeconds < 3 || (minuetReady && gauge.Repertoire == 4))
                                    return WanderersMinuet;
                            }
                        }
                        else if (songTimerInSeconds < 3 && canWeave)
                        {
                            bool balladReady = LevelChecked(MagesBallad) && IsOffCooldown(MagesBallad);
                            bool paeonReady = LevelChecked(ArmysPaeon) && IsOffCooldown(ArmysPaeon);

                            if (balladReady)
                                return MagesBallad;
                            if (paeonReady)
                                return ArmysPaeon;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.BRD_Simple_Buffs) && (!songNone || !LevelChecked(MagesBallad)) && isEnemyHealthHigh)
                    {
                        bool radiantReady = LevelChecked(RadiantFinale) && IsOffCooldown(RadiantFinale);
                        bool ragingReady = LevelChecked(RagingStrikes) && IsOffCooldown(RagingStrikes);
                        bool battleVoiceReady = LevelChecked(BattleVoice) && IsOffCooldown(BattleVoice);
                        bool barrageReady = LevelChecked(Barrage) && IsOffCooldown(Barrage);
                        bool firstMinute = CombatEngageDuration().Minutes == 0;
                        bool restOfFight = CombatEngageDuration().Minutes > 0;

                        if (ragingReady && ((canWeaveBuffs && firstMinute) || (canWeaveDelayed && restOfFight)) &&
                            (GetCooldownRemainingTime(BattleVoice) <= 5.38 || battleVoiceReady || !LevelChecked(BattleVoice)))
                            return RagingStrikes;

                        if (canWeaveBuffs && IsEnabled(CustomComboPreset.BRD_Simple_BuffsRadiant) && radiantReady &&
                            (Array.TrueForAll(gauge.Coda, SongIsNotNone) || Array.Exists(gauge.Coda, SongIsWandererMinuet)) &&
                            (battleVoiceReady || GetCooldownRemainingTime(BattleVoice) < 0.7) &&
                            (GetBuffRemainingTime(Buffs.RagingStrikes) <= 16.5 || openerFinished) && IsOnCooldown(RagingStrikes))
                        {
                            if (!JustUsed(RagingStrikes))
                                return RadiantFinale;
                        }

                        if (canWeaveBuffs && battleVoiceReady &&
                            (GetBuffRemainingTime(Buffs.RagingStrikes) <= 16.5 || openerFinished) && IsOnCooldown(RagingStrikes))
                        {
                            if (!JustUsed(RagingStrikes))
                                return BattleVoice;
                        }

                        if (canWeaveBuffs && barrageReady && !HasEffect(Buffs.StraightShotReady) && HasEffect(Buffs.RagingStrikes))
                        {
                            if (LevelChecked(RadiantFinale) && HasEffect(Buffs.RadiantFinale))
                                return Barrage;
                            else if (LevelChecked(BattleVoice) && HasEffect(Buffs.BattleVoice))
                                return Barrage;
                            else if (!LevelChecked(BattleVoice) && HasEffect(Buffs.RagingStrikes))
                                return Barrage;
                        }
                    }

                    if (canWeave)
                    {
                        bool empyrealReady = LevelChecked(EmpyrealArrow) && IsOffCooldown(EmpyrealArrow);
                        bool sidewinderReady = LevelChecked(Sidewinder) && IsOffCooldown(Sidewinder);
                        float battleVoiceCD = GetCooldownRemainingTime(BattleVoice);
                        float empyrealCD = GetCooldownRemainingTime(EmpyrealArrow);
                        float ragingCD = GetCooldownRemainingTime(RagingStrikes);
                        float radiantCD = GetCooldownRemainingTime(RadiantFinale);

                        if (empyrealReady && ((!openerFinished && IsOnCooldown(RagingStrikes)) || (openerFinished && battleVoiceCD >= 3.5) || !IsEnabled(CustomComboPreset.BRD_Simple_Buffs)))
                            return EmpyrealArrow;

                        if (LevelChecked(PitchPerfect) && songWanderer &&
                            (gauge.Repertoire == 3 || (gauge.Repertoire == 2 && empyrealCD < 2)) &&
                            ((!openerFinished && IsOnCooldown(RagingStrikes)) || (openerFinished && battleVoiceCD >= 3.5)))
                            return OriginalHook(WanderersMinuet);

                        if (sidewinderReady && ((!openerFinished && IsOnCooldown(RagingStrikes)) || (openerFinished && battleVoiceCD >= 3.5) || !IsEnabled(CustomComboPreset.BRD_Simple_Buffs)))
                        {
                            if (IsEnabled(CustomComboPreset.BRD_Simple_Pooling))
                            {
                                if (songWanderer)
                                {
                                    if ((HasEffect(Buffs.RagingStrikes) || ragingCD > 10) &&
                                        (HasEffect(Buffs.BattleVoice) || battleVoiceCD > 10) &&
                                        (HasEffect(Buffs.RadiantFinale) || radiantCD > 10 ||
                                        !LevelChecked(RadiantFinale)))
                                        return Sidewinder;
                                }
                                else return Sidewinder;
                            }
                            else return Sidewinder;
                        }

                        if (LevelChecked(Bloodletter) && ((!openerFinished && IsOnCooldown(RagingStrikes)) || openerFinished))
                        {
                            ushort bloodletterCharges = GetRemainingCharges(Bloodletter);

                            if (IsEnabled(CustomComboPreset.BRD_Simple_Pooling) && LevelChecked(WanderersMinuet))
                            {
                                if (songWanderer)
                                {
                                    if (((HasEffect(Buffs.RagingStrikes) || ragingCD > 10) &&
                                        (HasEffect(Buffs.BattleVoice) || battleVoiceCD > 10 ||
                                        !LevelChecked(BattleVoice)) &&
                                        (HasEffect(Buffs.RadiantFinale) || radiantCD > 10 ||
                                        !LevelChecked(RadiantFinale)) &&
                                        bloodletterCharges > 0) || bloodletterCharges > 2)
                                        return Bloodletter;
                                }

                                if (songArmy && (bloodletterCharges == 3 || ((gauge.SongTimer / 1000) > 30 && bloodletterCharges > 0)))
                                    return Bloodletter;
                                if (songMage && bloodletterCharges > 0)
                                    return Bloodletter;
                                if (songNone && bloodletterCharges == 3)
                                    return Bloodletter;
                            }
                            else if (bloodletterCharges > 0)
                                return Bloodletter;
                        }

                        // healing - please move if not appropriate priority
                        if (IsEnabled(CustomComboPreset.BRD_ST_SecondWind))
                        {
                            if (PlayerHealthPercentageHp() <= PluginConfiguration.GetCustomIntValue(Config.BRD_STSecondWindThreshold) && ActionReady(All.SecondWind))
                                return All.SecondWind;
                        }
                    }

                    if (isEnemyHealthHigh)
                    {
                        bool venomous = TargetHasEffect(Debuffs.VenomousBite);
                        bool windbite = TargetHasEffect(Debuffs.Windbite);
                        bool caustic = TargetHasEffect(Debuffs.CausticBite);
                        bool stormbite = TargetHasEffect(Debuffs.Stormbite);
                        float venomRemaining = GetDebuffRemainingTime(Debuffs.VenomousBite);
                        float windRemaining = GetDebuffRemainingTime(Debuffs.Windbite);
                        float causticRemaining = GetDebuffRemainingTime(Debuffs.CausticBite);
                        float stormRemaining = GetDebuffRemainingTime(Debuffs.Stormbite);

                        DotRecast poisonRecast = delegate (int duration)
                        {
                            return (venomous && venomRemaining < duration) || (caustic && causticRemaining < duration);
                        };

                        DotRecast windRecast = delegate (int duration)
                        {
                            return (windbite && windRemaining < duration) || (stormbite && stormRemaining < duration);
                        };

                        float ragingStrikesDuration = GetBuffRemainingTime(Buffs.RagingStrikes);
                        int ragingJawsRenewTime = PluginConfiguration.GetCustomIntValue(Config.BRD_RagingJawsRenewTime);
                        bool useIronJaws = (LevelChecked(IronJaws) && poisonRecast(4)) ||
                            (LevelChecked(IronJaws) && windRecast(4)) ||
                            (LevelChecked(IronJaws) && IsEnabled(CustomComboPreset.BRD_Simple_RagingJaws) &&
                            HasEffect(Buffs.RagingStrikes) && ragingStrikesDuration < ragingJawsRenewTime &&
                            poisonRecast(40) && windRecast(40));
                        bool dotOpener = (IsEnabled(CustomComboPreset.BRD_Simple_DoTOpener) && !openerFinished) || !IsEnabled(CustomComboPreset.BRD_Simple_DoTOpener);

                        if (!LevelChecked(Stormbite))
                        {
                            if (useIronJaws)
                            {
                                openerFinished = true;
                                return IronJaws;
                            }

                            if (!LevelChecked(IronJaws))
                            {
                                if (windbite && windRemaining < 4)
                                {
                                    openerFinished = true;
                                    return Windbite;
                                }

                                if (venomous && venomRemaining < 4)
                                {
                                    openerFinished = true;
                                    return VenomousBite;
                                }
                            }

                            if (IsEnabled(CustomComboPreset.BRD_Simple_DoT))
                            {
                                if (LevelChecked(Windbite) && !windbite && dotOpener)
                                    return Windbite;
                                if (LevelChecked(VenomousBite) && !venomous && dotOpener)
                                    return VenomousBite;
                            }
                        }

                        else
                        {
                            if (useIronJaws)
                            {
                                openerFinished = true;
                                return IronJaws;
                            }

                            if (IsEnabled(CustomComboPreset.BRD_Simple_DoT))
                            {
                                if (LevelChecked(Stormbite) && !stormbite && dotOpener)
                                    return Stormbite;
                                if (LevelChecked(CausticBite) && !caustic && dotOpener)
                                    return CausticBite;
                            }
                        }
                    }

                    if (!IsEnabled(CustomComboPreset.BRD_RemoveApexArrow))
                    {
                        if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                            return BlastArrow;

                        if (LevelChecked(ApexArrow))
                        {
                            int songTimerInSeconds = gauge.SongTimer / 1000;

                            if (songMage && gauge.SoulVoice == 100)
                                return ApexArrow;
                            if (songMage && gauge.SoulVoice >= 80 &&
                                songTimerInSeconds > 18 && songTimerInSeconds < 22)
                                return ApexArrow;
                            if (songWanderer && HasEffect(Buffs.RagingStrikes) && HasEffect(Buffs.BattleVoice) &&
                                (HasEffect(Buffs.RadiantFinale) || !LevelChecked(RadiantFinale)) && gauge.SoulVoice >= 80)
                                return ApexArrow;
                        }
                    }

                    if (HasEffect(Buffs.StraightShotReady))
                        return OriginalHook(StraightShot);
                }

                return actionID;
            }
        }

        internal class BRD_Buffs : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_Buffs;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Barrage)
                {
                    bool ragingReady = LevelChecked(RagingStrikes) && IsOffCooldown(RagingStrikes);
                    bool battleVoiceReady = LevelChecked(BattleVoice) && IsOffCooldown(BattleVoice);

                    if (ragingReady)
                        return RagingStrikes;
                    if (battleVoiceReady)
                        return BattleVoice;
                }

                return actionID;
            }
        }
        internal class BRD_OneButtonSongs : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_OneButtonSongs;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is WanderersMinuet)
                {
                    // Doesn't display the lowest cooldown song if they have been used out of order and are all on cooldown.
                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    int songTimerInSeconds = gauge.SongTimer / 1000;
                    bool songWanderer = gauge.Song != Song.WANDERER;
                    bool canUse = (songWanderer || songTimerInSeconds < 3) && !JustUsed(WanderersMinuet);
                    bool wanderersMinuetReady = LevelChecked(WanderersMinuet) && IsOffCooldown(WanderersMinuet);
                    bool magesBalladReady = LevelChecked(MagesBallad) && IsOffCooldown(MagesBallad);
                    bool armysPaeonReady = LevelChecked(ArmysPaeon) && IsOffCooldown(ArmysPaeon);

                    if (wanderersMinuetReady)
                        return WanderersMinuet;

                    if (canUse)
                    {
                        if (magesBalladReady)
                        {
                            if (songWanderer && gauge.Repertoire > 0)
                                return OriginalHook(WanderersMinuet);
                            return MagesBallad;
                        }

                        if (armysPaeonReady)
                        {
                            if (songWanderer && gauge.Repertoire > 0)
                                return OriginalHook(WanderersMinuet);
                            return ArmysPaeon;
                        }
                    }
                }

                return actionID;
            }
        }

        internal class BRD_Test : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_Test;

            protected string TimePoint = "";
            protected float GCD;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HeavyShot or BurstShot)
                {
                    //Dalamud.Logging.PluginLog.Warning($"LastAction:{GetActionName(ActionWatching.LastAction)} -- LastAbility:{GetActionName(ActionWatching.LastAbility)} -- LastWeaponskill:{GetActionName(ActionWatching.LastWeaponskill)} -- LastSpell:{GetActionName(ActionWatching.LastSpell)} -- lastComboMove:{GetActionName(lastComboMove)}");
                    //Dalamud.Logging.PluginLog.Warning($"{GetCooldownElapsed(actionID)} -- {GetCooldownRemainingTime(actionID)}");

                    if (CurrentTarget == null)
                    {
                        return actionID;
                    }

                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    bool canWeave = CanWeave(actionID);
                    var gcd = GetCooldownElapsed(actionID) + GetCooldownRemainingTime(actionID);
                    if (gcd == 0)
                    {
                        gcd = GCD;
                    }
                    else
                    {
                        GCD = gcd;
                    }

                    // 判断特殊时点
                    // 风debuff
                    var windbite = OriginalHook(Windbite);
                    var windbite_buff = Debuffs.Windbite;
                    if (windbite != Windbite)
                    {
                        windbite_buff = Debuffs.Stormbite;
                    }
                    var buff1 = FindTargetEffect(windbite_buff);
                    // 毒debuff
                    var venomousBite = OriginalHook(VenomousBite);
                    var venomousBite_buff = Debuffs.VenomousBite;
                    if (venomousBite != VenomousBite)
                    {
                        venomousBite_buff = Debuffs.CausticBite;
                    }
                    var buff2 = FindTargetEffect(venomousBite_buff);

                    // 起手爆发时点
                    if (ActionReady(WanderersMinuet) && ActionReady(RagingStrikes) && (gauge.Song == Song.NONE || gauge.SongTimer < 3000 ||
                        gauge.Song == Song.MAGE && gauge.SongTimer < 10000) &&
                        !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)) && buff1 == null && buff2 == null)
                    {
                        TimePoint = "起手爆发";
                    }
                    // 起手爆发时点结束
                    if (TimePoint == "起手爆发" && !HasCharges(RagingStrikes) && !HasEffect(Buffs.RagingStrikes) && !JustUsed(RagingStrikes))
                    {
                        TimePoint = "";
                    }

                    // 根据时点进入特殊循环
                    var result = actionID;
                    switch (TimePoint)
                    {
                        case "起手爆发":
                            result = StartBurst(actionID, lastComboMove, comboTime, level);
                            if (result == 0)
                            {
                                TimePoint = "";
                                break;
                            }
                            return result;
                        case "中期爆发":
                            result = MidTermBurst(actionID, lastComboMove, comboTime, level);
                            if (result == 0)
                            {
                                TimePoint = "";
                                break;
                            }
                            return result;
                    }

                    // 通常循环
                    // 能力技能
                    if (canWeave)
                    {
                        // 完美音调
                        if (LevelChecked(PitchPerfect) && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                        {
                            return OriginalHook(WanderersMinuet);
                        }

                        // 放浪神的小步舞曲
                        if (ActionReady(WanderersMinuet) && (gauge.Song == Song.NONE || gauge.SongTimer < 3000 ||
                            gauge.Song == Song.MAGE && gauge.SongTimer < 10000) &&
                            !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                        {
                            return WanderersMinuet;
                        }

                        // 贤者的叙事谣
                        if (ActionReady(MagesBallad) && (gauge.Song == Song.NONE || gauge.SongTimer < 3000) &&
                            !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                        {
                            // 用掉完美音调先
                            if (LevelChecked(PitchPerfect) && gauge.Song == Song.WANDERER && gauge.Repertoire > 0)
                            {
                                return OriginalHook(WanderersMinuet);
                            }
                            return MagesBallad;
                        }

                        // 军神的赞美歌
                        if (ActionReady(ArmysPaeon) && (gauge.Song == Song.NONE || gauge.SongTimer < 3000 ||
                            gauge.Song == Song.MAGE && gauge.SongTimer < 10000) &&
                            !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                        {
                            // 用掉完美音调先
                            if (LevelChecked(PitchPerfect) && gauge.Song == Song.WANDERER && gauge.Repertoire > 0)
                            {
                                return OriginalHook(WanderersMinuet);
                            }
                            return ArmysPaeon;
                        }

                        // 猛者强击
                        if (ActionReady(RagingStrikes))
                        {
                            return RagingStrikes;
                        }

                        // 战斗之声
                        if (ActionReady(BattleVoice) && gauge.Song != Song.NONE)
                        {
                            return BattleVoice;
                        }

                        // 光明神的最终乐章
                        if (ActionReady(RadiantFinale) && Array.TrueForAll(gauge.Coda, SongIsNotNone))
                        {
                            return RadiantFinale;
                        }

                        // 失血箭
                        if (LevelChecked(Bloodletter) && GetRemainingCharges(Bloodletter) == GetMaxCharges(Bloodletter) && !JustUsed(Bloodletter))
                        {
                            return Bloodletter;
                        }

                        // 纷乱箭
                        if (ActionReady(Barrage) && !HasEffect(Buffs.StraightShotReady))
                        {
                            // 不需要补毒的时候才用
                            if (LevelChecked(windbite) && (!TargetHasEffect(windbite_buff) || !LevelChecked(IronJaws) && buff1.RemainingTime < 4) && !JustUsed(windbite))
                            {
                                goto GotoBarrage;
                            }
                            if (LevelChecked(venomousBite) && (!TargetHasEffect(venomousBite_buff) || !LevelChecked(IronJaws) && buff2.RemainingTime < 4) && !JustUsed(venomousBite))
                            {
                                goto GotoBarrage;
                            }
                            if (LevelChecked(IronJaws) && buff1 != null && buff2 != null && (buff1.RemainingTime < 4 || buff2.RemainingTime < 4) && !WasLastWeaponskill(IronJaws))
                            {
                                goto GotoBarrage;
                            }
                            return Barrage;
                        }
                    GotoBarrage:

                        // 九天连箭
                        if (ActionReady(EmpyrealArrow) && (gauge.Song != Song.MAGE || GetCooldownRemainingTime(RainOfDeath) > 7.5))
                        {
                            return EmpyrealArrow;
                        }

                        // 侧风诱导箭
                        if (ActionReady(Sidewinder))
                        {
                            return Sidewinder;
                        }

                        // 失血箭
                        if (ActionReady(Bloodletter) && (GetRemainingCharges(Bloodletter) > GetMaxCharges(Bloodletter) - 2 ||
                            HasEffect(Buffs.RagingStrikes) || HasEffect(Buffs.BattleVoice)) && !JustUsed(Bloodletter))
                        {
                            return Bloodletter;
                        }
                    }

                    // 战技

                    // 直线射击 / 辉煌箭
                    var straightShot = OriginalHook(StraightShot);
                    if (LevelChecked(straightShot) && HasEffect(Buffs.StraightShotReady) && HasEffect(Buffs.Barrage))
                    {
                        return straightShot;
                    }

                    // 风蚀箭 / 狂风蚀箭
                    if (LevelChecked(windbite) && (!TargetHasEffect(windbite_buff) || !LevelChecked(IronJaws) && buff1.RemainingTime < 4) && !JustUsed(windbite))
                    {
                        return windbite;
                    }

                    // 毒咬箭 / 烈毒咬箭
                    if (LevelChecked(venomousBite) && (!TargetHasEffect(venomousBite_buff) || !LevelChecked(IronJaws) && buff2.RemainingTime < 4) && !JustUsed(venomousBite))
                    {
                        return venomousBite;
                    }

                    // 伶牙俐齿
                    if (LevelChecked(IronJaws) && buff1 != null && buff2 != null && (buff1.RemainingTime < 4 || buff2.RemainingTime < 4) && !WasLastWeaponskill(IronJaws))
                    {
                        return IronJaws;
                    }

                    // 爆破箭
                    if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                    {
                        return BlastArrow;
                    }

                    // 绝峰箭
                    var buffRagingStrikes = FindEffect(Buffs.RagingStrikes);
                    var buffRadiantFinale = FindEffect(Buffs.RadiantFinale);
                    if (LevelChecked(ApexArrow))
                    {
                        // 如果猛者强击快好了，等等强者cd
                        if (gauge.SoulVoice == 100 && GetCooldownRemainingTime(RagingStrikes) <= 15)
                        {
                            goto GotoApexArrow;
                        }
                        if (LevelChecked(RadiantFinale))
                        {
                            // 90级了
                            // buff全到，直接放
                            if (gauge.SoulVoice == 100 && buffRagingStrikes != null && buffRadiantFinale != null)
                            {
                                return ApexArrow;
                            }
                            // 等等光明神
                            if (gauge.SoulVoice == 100 && buffRagingStrikes != null && buffRadiantFinale == null && GetCooldownRemainingTime(RadiantFinale) < buffRagingStrikes.RemainingTime - 10)
                            {
                                return ApexArrow;
                            }
                            // buff要结束了，高于80直接放
                            if (buffRagingStrikes != null && buffRagingStrikes.RemainingTime < 3 * gcd && gauge.SoulVoice >= 80)
                            {
                                return ApexArrow;
                            }
                        }
                        else
                        {
                            // 没有90级
                            // buff全到，直接放
                            if (gauge.SoulVoice == 100 && buffRagingStrikes != null)
                            {
                                return ApexArrow;
                            }
                            // buff要结束了，高于80直接放
                            if (buffRagingStrikes != null && buffRagingStrikes.RemainingTime < 3 * gcd && gauge.SoulVoice >= 80)
                            {
                                return ApexArrow;
                            }
                        }
                        // 猛者强击cd还早，先放了
                        if (gauge.SoulVoice == 100 && GetCooldownRemainingTime(RagingStrikes) > 15)
                        {
                            return ApexArrow;
                        }
                    }
                GotoApexArrow:

                    // 直线射击 / 辉煌箭
                    if (LevelChecked(straightShot) && HasEffect(Buffs.StraightShotReady))
                    {
                        return straightShot;
                    }

                    // 伶牙俐齿
                    if (LevelChecked(IronJaws) && buff1 != null && buff2 != null && (buff1.RemainingTime < 4 + gcd || buff2.RemainingTime < 4 + gcd) && !WasLastWeaponskill(IronJaws))
                    {
                        return IronJaws;
                    }

                    // 强力射击 / 爆发射击
                    return actionID;
                }

                return actionID;
            }

            // 起手爆发
            protected uint StartBurst(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                BRDGauge? gauge = GetJobGauge<BRDGauge>();
                bool canWeave = CanWeave(actionID);
                var gcd = GetCooldownElapsed(actionID) + GetCooldownRemainingTime(actionID);
                if (gcd == 0)
                {
                    gcd = GCD;
                }
                else
                {
                    GCD = gcd;
                }

                // 风蚀箭 / 狂风蚀箭
                var windbite = OriginalHook(Windbite);
                var windbite_buff = Debuffs.Windbite;
                if (windbite != Windbite)
                {
                    windbite_buff = Debuffs.Stormbite;
                }
                var buff1 = FindTargetEffect(windbite_buff);
                if (LevelChecked(windbite) && (!TargetHasEffect(windbite_buff) || !LevelChecked(IronJaws) && buff1.RemainingTime < 4) && !WasLastWeaponskill(windbite))
                {
                    return windbite;
                }

                // 放浪神的小步舞曲
                if (canWeave && ActionReady(WanderersMinuet) && (gauge.Song == Song.NONE || gauge.SongTimer < 3000 ||
                    gauge.Song == Song.MAGE && gauge.SongTimer < 10000) &&
                    !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                {
                    return WanderersMinuet;
                }

                // 猛者强击
                if (canWeave && ActionReady(RagingStrikes))
                {
                    return RagingStrikes;
                }

                // 毒咬箭 / 烈毒咬箭
                var venomousBite = OriginalHook(VenomousBite);
                var venomousBite_buff = Debuffs.VenomousBite;
                if (venomousBite != VenomousBite)
                {
                    venomousBite_buff = Debuffs.CausticBite;
                }
                var buff2 = FindTargetEffect(venomousBite_buff);
                if (LevelChecked(venomousBite) && (!TargetHasEffect(venomousBite_buff) || !LevelChecked(IronJaws) && buff2.RemainingTime < 4) && !JustUsed(venomousBite))
                {
                    return venomousBite;
                }

                // 完美音调
                if (canWeave && LevelChecked(PitchPerfect) && gauge.Song == Song.WANDERER && gauge.Repertoire == 3)
                {
                    return OriginalHook(WanderersMinuet);
                }

                // 九天连箭
                if (canWeave && ActionReady(EmpyrealArrow))
                {
                    return EmpyrealArrow;
                }

                // 失血箭
                if (canWeave && LevelChecked(Bloodletter) && GetRemainingCharges(Bloodletter) > 0 && WasLastAbility(EmpyrealArrow) && WasLastWeaponskill(venomousBite))
                {
                    return Bloodletter;
                }

                // 光明神的最终乐章
                if (canWeave && ActionReady(RadiantFinale) && Array.Exists(gauge.Coda, SongIsWandererMinuet))
                {
                    return RadiantFinale;
                }

                // 战斗之声
                if (canWeave && ActionReady(BattleVoice) && gauge.Song != Song.NONE)
                {
                    return BattleVoice;
                }

                // 纷乱箭
                if (canWeave && ActionReady(Barrage) && !HasEffect(Buffs.StraightShotReady))
                {
                    return Barrage;
                }

                // 侧风诱导箭
                if (canWeave && ActionReady(Sidewinder))
                {
                    return Sidewinder;
                }

                // 伶牙俐齿
                if (LevelChecked(IronJaws) && buff1 != null && buff2 != null && !WasLastWeaponskill(IronJaws) &&
                    (buff1.RemainingTime < 4 || buff2.RemainingTime < 4 || GetBuffRemainingTime(Buffs.RagingStrikes) < gcd))
                {
                    return IronJaws;
                }
                if (LevelChecked(IronJaws) && buff1 != null && buff2 != null && !WasLastWeaponskill(IronJaws) &&
                    (buff1.RemainingTime < 4 || buff2.RemainingTime < 4 || (GetBuffRemainingTime(Buffs.RagingStrikes) < 2 * gcd && !HasEffect(Buffs.StraightShotReady))))
                {
                    return IronJaws;
                }

                // 完美音调
                if (canWeave && LevelChecked(PitchPerfect) && gauge.Song == Song.WANDERER && gauge.Repertoire > 0 && WasLastWeaponskill(IronJaws))
                {
                    return OriginalHook(WanderersMinuet);
                }

                // 失血箭
                if (canWeave && LevelChecked(Bloodletter) && GetRemainingCharges(Bloodletter) > 0 && !JustUsed(Bloodletter))
                {
                    return Bloodletter;
                }

                // 爆破箭
                if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                {
                    return BlastArrow;
                }

                // 绝峰箭
                var buffRagingStrikes = FindEffect(Buffs.RagingStrikes);
                var buffRadiantFinale = FindEffect(Buffs.RadiantFinale);
                if (LevelChecked(ApexArrow))
                {
                    // 如果猛者强击快好了，等等强者cd
                    if (gauge.SoulVoice == 100 && GetCooldownRemainingTime(RagingStrikes) <= 15)
                    {
                        goto GotoApexArrow;
                    }
                    if (LevelChecked(RadiantFinale))
                    {
                        // 90级了
                        // buff全到，直接放
                        if (gauge.SoulVoice == 100 && buffRagingStrikes != null && buffRadiantFinale != null)
                        {
                            return ApexArrow;
                        }
                        // 等等光明神
                        if (gauge.SoulVoice == 100 && buffRagingStrikes != null && buffRadiantFinale == null && GetCooldownRemainingTime(RadiantFinale) < buffRagingStrikes.RemainingTime - 10)
                        {
                            goto GotoApexArrow;
                        }
                        // buff要结束了，高于80直接放
                        if (buffRagingStrikes != null && buffRagingStrikes.RemainingTime < 3 * gcd && gauge.SoulVoice >= 80)
                        {
                            return ApexArrow;
                        }
                    }
                    else
                    {
                        // 没有90级
                        // buff全到，直接放
                        if (gauge.SoulVoice == 100 && buffRagingStrikes != null)
                        {
                            return ApexArrow;
                        }
                        // buff要结束了，高于80直接放
                        if (buffRagingStrikes != null && buffRagingStrikes.RemainingTime < 3 * gcd && gauge.SoulVoice >= 80)
                        {
                            return ApexArrow;
                        }
                    }
                    // 猛者强击cd还早，先放了
                    if (gauge.SoulVoice == 100 && GetCooldownRemainingTime(RagingStrikes) > 15)
                    {
                        return ApexArrow;
                    }
                }
                GotoApexArrow:

                // 直线射击 / 辉煌箭
                var straightShot = OriginalHook(StraightShot);
                if (LevelChecked(straightShot) && HasEffect(Buffs.StraightShotReady))
                {
                    return straightShot;
                }

                // 强力射击 / 爆发射击
                return actionID;
            }

            // 中期爆发
            protected uint MidTermBurst(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                return 0;
            }

            #region 可能的功能
            //    public class GCDCalculatorData
            //    {
            //        public uint ActionID;
            //        public int GCD;
            //        public float GCDTime;
            //        public uint LastAction;
            //        public int LastActionCount;
            //        public float ComboTime;
            //    }

            //    public static class GCDCalculator
            //    {
            //        private static Hashtable datas = new();

            //        public static void UpdateWatchPoint(string name, bool reset, uint actionID, float comboTime)
            //        {
            //            var data = (GCDCalculatorData)datas[name];
            //            if (data == null || reset)
            //            {
            //                data = new GCDCalculatorData();
            //                data.ActionID = actionID;
            //                data.GCDTime = GetCooldownElapsed(actionID) + GetCooldownRemainingTime(actionID);
            //                datas[name] = data;
            //            }
            //            if (!WasLastWeaponskill(data.LastAction))
            //            {
            //                data.LastAction = ActionWatching.LastWeaponskill;
            //                data.LastActionCount = 1;
            //                data.ComboTime = comboTime;
            //                data.GCD++;
            //                return;
            //            }
            //            if (ActionWatching.LastWeaponskill == ActionWatching.LastAction && data.LastActionCount != LastActionCounter())
            //            {
            //                data.LastActionCount = LastActionCounter();
            //                data.GCD++;
            //                return;
            //            }
            //            if (WasLastWeaponskill(data.LastAction))
            //            {

            //            }
            //        }

            //        public static int GetWatchPoint(string name)
            //        {
            //            var data = (GCDCalculatorData)datas[name];
            //            return data == null ? 0 : data.GCD;
            //        }

            //    }
            //}
            #endregion

        }

        // 常用技能翻译
        // QuickNock 连珠箭 低等级AOE 18
        // Ladonsbite 百首龙牙箭 高等级AOE 82
        // WanderersMinuet 放浪神的小步舞曲 52
        // MagesBallad 贤者的叙事谣 30
        // ArmysPaeon 军神的赞美歌 40
        // PitchPerfect 完美音调 52
        // EmpyrealArrow 九天连箭 54

        internal class BRD_Test_AOE : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRD_Test_AOE;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Ladonsbite or QuickNock)
                {
                    if (CurrentTarget == null)
                    {
                        return actionID;
                    }

                    BRDGauge? gauge = GetJobGauge<BRDGauge>();
                    bool canWeave = CanWeave(actionID);

                    // 能力技
                    if (canWeave)
                    {
                        // 完美音调
                        if (LevelChecked(PitchPerfect) && gauge.Song == Song.WANDERER && (gauge.Repertoire == 3 || gauge.SongTimer < 3000))
                        {
                            return OriginalHook(WanderersMinuet);
                        }

                        // 歌曲
                        int songTimerInSeconds = gauge.SongTimer / 1000;
                        bool songNone = gauge.Song == Song.NONE;

                        if (songTimerInSeconds < 3 || songNone)
                        {
                            if (ActionReady(WanderersMinuet) && !(JustUsed(MagesBallad) || JustUsed(ArmysPaeon)))
                            {
                                return WanderersMinuet;
                            }
                            if (ActionReady(MagesBallad) && !(JustUsed(WanderersMinuet) || JustUsed(ArmysPaeon)))
                            {
                                // 用掉完美音调先
                                if (LevelChecked(PitchPerfect) && gauge.Song == Song.WANDERER && gauge.Repertoire > 0)
                                {
                                    return OriginalHook(WanderersMinuet);
                                }
                                return MagesBallad;
                            }
                            if (ActionReady(ArmysPaeon) && !(JustUsed(MagesBallad) || JustUsed(WanderersMinuet)))
                            {
                                // 用掉完美音调先
                                if (LevelChecked(PitchPerfect) && gauge.Song == Song.WANDERER && gauge.Repertoire > 0)
                                {
                                    return OriginalHook(WanderersMinuet);
                                }
                                return ArmysPaeon;
                            }
                        }

                        // 猛者强击
                        if (ActionReady(RagingStrikes))
                        {
                            return RagingStrikes;
                        }

                        // 战斗之声
                        if (ActionReady(BattleVoice) && gauge.Song != Song.NONE)
                        {
                            return BattleVoice;
                        }

                        // 死亡箭雨
                        if (LevelChecked(RainOfDeath) && GetRemainingCharges(RainOfDeath) == GetMaxCharges(RainOfDeath) && !JustUsed(RainOfDeath))
                        {
                            return RainOfDeath;
                        }

                        // 失血箭
                        if (!LevelChecked(RainOfDeath) && GetRemainingCharges(Bloodletter) == GetMaxCharges(Bloodletter) && !JustUsed(Bloodletter))
                        {
                            return Bloodletter;
                        }

                        // 九天连箭
                        if (ActionReady(EmpyrealArrow) && (gauge.Song != Song.MAGE || GetCooldownRemainingTime(RainOfDeath) > 7.5))
                        {
                            return EmpyrealArrow;
                        }

                        // 侧风诱导箭
                        if (ActionReady(Sidewinder))
                        {
                            return Sidewinder;
                        }

                        // 死亡箭雨
                        if (ActionReady(RainOfDeath) && !JustUsed(RainOfDeath))
                        {
                            return RainOfDeath;
                        }

                        // 失血箭
                        if (!LevelChecked(RainOfDeath) && ActionReady(Bloodletter) && !JustUsed(Bloodletter))
                        {
                            return Bloodletter;
                        }

                        // 光明神的最终乐章
                        if (ActionReady(RadiantFinale) && Array.TrueForAll(gauge.Coda, SongIsNotNone))
                        {
                            return RadiantFinale;
                        }
                    }

                    // 战技

                    // 爆破箭
                    if (LevelChecked(BlastArrow) && HasEffect(Buffs.BlastArrowReady))
                    {
                        return BlastArrow;
                    }

                    // 绝峰箭
                    if (LevelChecked(ApexArrow) && gauge.SoulVoice == 100)
                    {
                        return ApexArrow;
                    }

                    // 影噬箭
                    if (LevelChecked(Shadowbite) && HasEffect(Buffs.ShadowbiteReady))
                    {
                        return Shadowbite;
                    }

                    // 强力射击 / 爆发射击
                    return actionID;
                }
                return actionID;
            }
        }
    }
}
