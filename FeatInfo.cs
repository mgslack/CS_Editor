/*
 * Class holding the known feats of Colony Ship: asset name, game display name, category,
 * requirement and effect.  Names, requirements and effects are the game's own texts (from
 * the feat assets, game version 1.0.x), categories are the ones used by the Colony Ship
 * wiki (https://colonyship.fandom.com/wiki/Feats).  Regular and heroic feats come first,
 * followed by the special feats that are gained by other means than leveling up.
 *
 * ----------------------------------------------------------------------------
 *
 * Author: Mads Petersen
 * Written: 2026-10-27
 *
 * ----------------------------------------------------------------------------
 */
namespace CS_Editor
{
    public class FeatInfo
    {
        #region Properties
        public string Asset { get; }
        public string Name { get; }
        public string Category { get; }
        public string Requirement { get; }
        public string Effect { get; }
        #endregion

        #region Known Feats
        public static readonly FeatInfo[] KnownFeats = {
            // Heroic (character creation)
            new FeatInfo("F_Gifted", "Gifted", "Heroic (character creation)", "", "+3 Stat Point, +4 Skill Points"),
            new FeatInfo("F_H_CultLeader", "Cult Leader", "Heroic (character creation)", "CHA 10", "Party: +20% Extra Experience. Companions: Easier bonding, can become devoted (+1 Natural DR)"),
            new FeatInfo("F_H_DodgeThis", "Dodge This!", "Heroic (character creation)", "PER 10", "20% of accuracy is converted to critical chance, Accuracy +12, Critical Strike Skill Gain +100%."),
            new FeatInfo("F_H_FastRunner", "Fast Runner", "Heroic (character creation)", "DEX 10", "Disables enemy reaction, +6AP to movement, Initiative +24, Evasion Skill Gain +100%."),
            new FeatInfo("F_H_HealingFactor", "Healing Factor", "Heroic (character creation)", "CON 10", "CON +2, Regeneration +3, Stat Healing +1, Max Implants +2. Biotech Skill Gain +100%."),
            new FeatInfo("F_H_Juggernaut", "Juggernaut", "Heroic (character creation)", "STR 10", "Knockdowns become Stuns, Natural DR +1 (+3 when 13HP or less), Melee Damage +2, Armor Skill Gain +100%."),
            new FeatInfo("F_H_Mastermind", "Mastermind", "Heroic (character creation)", "INT 10", "+25% experience bonus, free feats at levels 2, 6 and 10."),
            // Combat
            new FeatInfo("F_Charger", "Charger", "Combat", "", "+4AP to movement, Initiative +4"),
            new FeatInfo("F_CriticalThinker", "Critical Thinker", "Combat", "PER 6", "For each skill level: Critical Chance +1%, Critical Damage +4% (+0.04), Takedown +2"),
            new FeatInfo("F_FastDraw", "Fast Draw", "Combat", "", "Adds 12 Initiative, +4 action points in the first turn"),
            new FeatInfo("F_Opportunist", "Opportunist", "Combat", "PER 6", "Reaction +30, Reaction Attack: 20% of reaction chance added to accuracy, 50% to grazes."),
            new FeatInfo("F_QuickFingers", "Quick Fingers", "Combat", "", "Halves AP Cost of Reload, Lockpick, Belt and Inventory Actions, Initiative +4"),
            new FeatInfo("F_SlowMetabolism", "Slow Metabolism", "Combat", "", "Stim duration +1 turn, Death Timer +1"),
            // Party
            new FeatInfo("F_Captain", "Captain", "Party", "CHA 9", "Party members only: Accuracy +8, Reaction +20, Critical Chance +6%"),
            new FeatInfo("F_LoneWolf", "Lone Wolf", "Party", "", "AP +2, Evasion +12, Initiative +16, Critical Chance +6% when working alone. Harder to bond with companions."),
            new FeatInfo("F_PersonalMagnetism", "Personal Magnetism", "Party", "CHA <8", "Max Followers +1"),
            new FeatInfo("F_Sarge", "Sarge", "Party", "CHA 7-8", "Party members only: Accuracy +4, Critical Chance +6%"),
            // Defense
            new FeatInfo("F_ArmoredWarfare", "Armored Warfare", "Defense", "STR 6", "Lowers AP penalty in any equipment by 1, Armor Handling +8."),
            new FeatInfo("F_ArtfulDodger", "Artful Dodger", "Defense", "DEX 6", "Evasion +12. 30% of hits are converted to grazes, 70% to misses (from 50/50)"),
            new FeatInfo("F_ToughBastard", "Tough Bastard", "Defense", "", "HP +10, Enemy's Critical Damage is halved"),
            new FeatInfo("F_TrueGrit", "True Grit", "Defense", "", "Ballistic Resistance +1. When not in cover: Evasion +6, Reaction +20"),
            // On Kill
            new FeatInfo("F_Bloodlust", "Bloodlust", "On Kill", "", "On Kill: 50% Critical Chance for the next attack"),
            new FeatInfo("F_SecondWind", "Second Wind", "On Kill", "", "On Kill: +6 Action Points"),
            // General
            new FeatInfo("F_Bionic", "Bionic", "General", "No Healing Factor", "Max Implants +1"),
            new FeatInfo("F_Educated", "Educated", "General", "", "+1 Tagged Skill, +25% Extra Experience (works retroactively)"),
            new FeatInfo("F_MasterTrader", "Master Trader", "General", "CHA 7", "Auto-Success on haggling checks, Unique Deals, 25% discount when buying, +1 initial disposition"),
            new FeatInfo("F_SkillMonkey", "Skill Monkey", "General", "INT 8", "Skill Gain on Science and Stealth skills increased by 35% (works retroactively)"),
            // Melee
            new FeatInfo("F_Berserker", "Berserker", "Melee", "STR 8", "Melee: Attack Cost -1, aimed attacks are permanently disabled. When 13HP or less: +2 to damage."),
            new FeatInfo("F_Gladiator", "Gladiator", "Melee", "DEX 8", "Melee: Reaction +30, Graze +10, Graze Damage +25%"),
            new FeatInfo("F_HeavyHitter", "Heavy Hitter", "Melee", "STR 8", "Melee: Stagger +20, Penetration +10, Critical Damage +15% (+ 0.15)"),
            new FeatInfo("F_PreciseStrike", "Precise Strike", "Melee", "PER 8", "Melee: Aimed Attacks -1 AP Cost, Critical Chance +6%, Non-Regular Attacks Accuracy +12"),
            new FeatInfo("F_Warrior", "Warrior", "Melee", "", "Melee: 25% Skill Gain (Retroactive). Accuracy +6. Evasion and Accuracy +1 for every skill level."),
            // Melee Weapon Specialization
            new FeatInfo("F_Basher", "Basher", "Melee Weapon Specialization", "", "Blunt: Accuracy +8, Knockdown Critical Chance +20. Penetration +2 for each skill level"),
            new FeatInfo("F_Butcher", "Butcher", "Melee Weapon Specialization", "", "Bladed: Accuracy +8, Carve Critical Chance +20. Aimed +2 for each skill level"),
            // Ranged
            new FeatInfo("F_BigGameHunter", "Big Game Hunter", "Ranged", "PER 8", "Ranged: Penetration + 10, Critical Damage +15% (0.15)"),
            new FeatInfo("F_ControlledBurst", "Controlled Burst", "Ranged", "STR 8", "Ranged: Multishots cost 1 AP less to use, Recoil Control +10, 20% of recoil control added to ranged accuracy."),
            new FeatInfo("F_Gunfighter", "Gunfighter", "Ranged", "DEX 8", "Ranged: Reaction +20, Graze +10, Graze Damage +25%"),
            new FeatInfo("F_MasterBlaster", "Master Blaster", "Ranged", "", "Energy Weapons: Accuracy +20, Critical Chance +12%, Penetration +10"),
            new FeatInfo("F_Sharpshooter", "Sharpshooter", "Ranged", "PER 8", "Ranged: Aimed +6, Ignores 15 points of cover, Range +2"),
            new FeatInfo("F_LotsOfGuns", "Guns, Lots of Guns", "Ranged", "", "Ranged: 25% Skill Gain (Retroactive), Reaction +20, Accuracy +1 for every other weapon skill level."),
            // Ranged Weapon Specialization
            new FeatInfo("F_Gunner", "Sprayer", "Ranged Weapon Specialization", "", "SMG: Accuracy +8. Graze Damage +3% and penetration +1 for each skill level"),
            new FeatInfo("F_Pistolero", "Pistolero", "Ranged Weapon Specialization", "", "Pistol: Accuracy +8. Reaction +4 and critical chance +1% for each skill level"),
            new FeatInfo("F_Rifleman", "Rifleman", "Ranged Weapon Specialization", "", "Rifle: Accuracy +8. Critical damage +3% and Aimed +1 for each skill level"),
            new FeatInfo("F_TooClose", "Shotgunner", "Ranged Weapon Specialization", "", "Shotgun: Accuracy +8. Stagger +6 and accuracy +1 for each skill level"),
            // Stealth
            new FeatInfo("F_Ghost", "Ghost", "Stealth", "", "-1 to noise generation per tile, actions generate half noise (except Takedown)."),
            new FeatInfo("F_Prowler", "Prowler", "Stealth", "", "Sneaking increased by 5, yellow tiles range increased to 30 (from 20)"),
            new FeatInfo("F_ThroatSlitter", "Throat Slitter", "Stealth", "", "Takedown AP Cost reduced to 3AP and noise cut by half. Takedown +10."),
            new FeatInfo("F_TunnelRunner", "Tunnel Runner", "Stealth", "", "Moving costs 1 AP, can use movement AP in stealth"),
            // Special: Hunter (terminal bonus)
            new FeatInfo("F_G_C_Frog_1", "Frog Hunter", "Special: Hunter (terminal bonus)", "", "Party Bonus Against Frogs: +12 Accuracy, +12 Evasion, +6 Critical Strike"),
            new FeatInfo("F_G_C_Creeper_1", "Creeper Hunter", "Special: Hunter (terminal bonus)", "", "Party Bonus Against Creepers: +12 Accuracy, +6 Critical Strike, +10 Penetration"),
            new FeatInfo("F_G_C_Robot", "Machine Hunter", "Special: Hunter (terminal bonus)", "", "Party Bonus Against Robots and Turrets: +12 Critical Strike, +10 Penetration"),
            // Special: Mutation (Head Hunter quest)
            new FeatInfo("F_G_Super_1", "Accelerated Reaction I", "Special: Mutation (Head Hunter quest)", "", "Permanent: Reaction +20"),
            new FeatInfo("F_G_Super_2", "Accelerated Reaction II", "Special: Mutation (Head Hunter quest)", "", "Permanent: Reaction +20; Aggro Stim: +1 AP"),
            new FeatInfo("F_G_Super_3", "Accelerated Reaction III", "Special: Mutation (Head Hunter quest)", "", "Permanent: Reaction +20; Aggro Stim: +2 AP, Evasion +20"),
            new FeatInfo("F_G_Regen_1", "Cellular Regeneration I", "Special: Mutation (Head Hunter quest)", "", "Permanent: Regeneration +1"),
            new FeatInfo("F_G_Regen_2", "Cellular Regeneration II", "Special: Mutation (Head Hunter quest)", "", "Permanent: Regeneration +1 ; Regen Stim (On Use): Regen +3"),
            new FeatInfo("F_G_Regen_3", "Cellular Regeneration III", "Special: Mutation (Head Hunter quest)", "", "Permanent: Regeneration +1 ; Regen Stim (On Use): Regen +5, Natural DR +1"),
            new FeatInfo("F_G_Neural_1", "Mental Fortitude I", "Special: Mutation (Head Hunter quest)", "", "Permanent: Neural +5, Skill Gain +15%"),
            new FeatInfo("F_G_Neural_2", "Mental Fortitude II", "Special: Mutation (Head Hunter quest)", "", "Permanent: Neural +5, Skill Gain +15%; Zen Stim: Neural +15"),
            new FeatInfo("F_G_Neural_3", "Mental Fortitude III", "Special: Mutation (Head Hunter quest)", "", "Permanent: Neural +5, Skill Gain +15%; Zen Stim: Neural +30, No Perception Loss"),
            // Special: Ask Questions Later
            new FeatInfo("F_G_AskLater_1", "Ask Questions Later I", "Special: Ask Questions Later", "[Initiative Bonus] x5", "+2 Party Initiative, Initial Disposition -1."),
            new FeatInfo("F_G_AskLater_2", "Ask Questions Later II", "Special: Ask Questions Later", "[Initiative Bonus] x10", "+4 Party Initiative, +10% Skill Gain on Combat Skills, Initial Disposition -2."),
            new FeatInfo("F_G_AskLater_3", "Ask Questions Later III", "Special: Ask Questions Later", "[Initiative Bonus] x15", "+6 Party Initiative, +20% Skill Gain on Combat Skills, Initial Disposition -3."),
            // Special: Death Timer
            new FeatInfo("F_G_Revive_1", "Still Alive", "Special: Death Timer", "Fell in combat 1 time", "HP -1, Evasion +1, Death Timer +1"),
            new FeatInfo("F_G_Revive_2", "Not My Time Yet", "Special: Death Timer", "Fell in combat 3 times", "HP -2, Evasion +2, Death Timer +1"),
            new FeatInfo("F_G_Revive_3", "Hard To Kill", "Special: Death Timer", "Fell in combat 5 times", "HP -3, Evasion +3, Death Timer +1"),
            new FeatInfo("F_G_Revive_4", "Back From The Dead", "Special: Death Timer", "Fell in combat 7 times", "HP -4, Evasion +4, Death Timer +1"),
            new FeatInfo("F_G_Revive_5", "Revenant", "Special: Death Timer", "Fell in combat 10 times or more", "HP -5, Evasion +5, Death Timer +2"),
            // Special: Overclocking
            new FeatInfo("F_G_Implant_1", "Overclocked", "Special: Overclocking", "One Implant Overclocked", "-1 HP"),
            new FeatInfo("F_G_Implant_2", "One More Won't Hurt", "Special: Overclocking", "Two Implants Overclocked", "-2 HP"),
            new FeatInfo("F_G_Implant_3", "Small Price To Pay", "Special: Overclocking", "Three Implants Overclocked", "-4 HP"),
            new FeatInfo("F_G_Implant_4", "Worth The Price", "Special: Overclocking", "Four Implants Overclocked", "-6 HP, -1 Energy DR"),
            new FeatInfo("F_G_Implant_5", "No Gain Without Pain", "Special: Overclocking", "Five Implants Overclocked", "-8 HP, -1 Energy DR"),
            new FeatInfo("F_G_Implant_6", "Power Through Sacrifice", "Special: Overclocking", "Six Implants Overclocked", "-10 HP, -1 Energy DR"),
            new FeatInfo("F_G_Implant_7", "No Longer Human", "Special: Overclocking", "Seven Implants Overclocked", "-10 HP, -2 Energy DR"),
            // Special: Other
            new FeatInfo("F_G_Mutant", "Mutant", "Special: Other", "", "Regen +1, Stat Healing +1, Toxic Resistance +75"),
            new FeatInfo("F_F_Regulator", "Regulator", "Special: Other", "", "You're a Regulator and have a badge to prove it"),
            // Companion
            new FeatInfo("F_C_Cobra", "Spec Ops", "Companion", "", "Evasion +18, Reaction +20, Penetration +10"),
            new FeatInfo("F_C_Evans_Defender", "Sniper", "Companion", "", "When in cover: Ballistic Resistance +2, Ranged Aimed Attacks -2 AP Cost and +6 Accuracy."),
            new FeatInfo("F_C_Evans_Desperado", "Desperado", "Companion", "", "During placement phase or when not in cover: +2 AP, +4 Initiative, +25 reaction."),
            new FeatInfo("F_C_Faythe_Assassin_1", "Assassin", "Companion", "", "Melee Damage +2, Critical Chance +8, Penetration +10, Takedown +15."),
            new FeatInfo("F_C_Faythe_Thief_1", "Thief", "Companion", "", "-1 to noise generation per tile. +25% skill gains in sneak, lockpick and steal. Evasion +6 when in cover."),
            new FeatInfo("F_C_Garret", "Enforcer", "Companion", "", "Critical Chance +4, Critical Damage +15% (+0.15), Stagger +20"),
            new FeatInfo("F_C_Harbinger", "The Third Daughter", "Companion", "", "Experience Bonus +20%, +30 Neural Resistance, Party Initiative +4"),
            new FeatInfo("F_C_Jed", "Punisher", "Companion", "", "Initiative +4, Natural DR +1, Recoil Control +15, Melee Damage +2"),
            new FeatInfo("F_C_Knurl", "Outcast", "Companion", "", "Critical Damage +15% (+0.15), Evasion +18, Regen +1"),
            new FeatInfo("F_C_Mathias", "Unwavering Faith", "Companion", "", "All Resistances +30, +1 Ballistic and Melee DR, +1 regen") };
        #endregion

        // --------------------------------------------------------------------

        #region Constructor
        public FeatInfo(string asset, string name, string category, string requirement, string effect)
        {
            Asset = asset; Name = name; Category = category; Requirement = requirement; Effect = effect;
        }
        #endregion

        // --------------------------------------------------------------------

        #region Public Methods
        // Known feat for asset name, null if not known.
        public static FeatInfo? Find(string asset)
        {
            return Array.Find(KnownFeats, f => f.Asset == asset);
        }
        #endregion
    }
}
