using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;
using System;

namespace Eridu.Rpg {

    [System.Serializable]
    public struct EriduRpgEntityModel {
        [Key(0)]
        public int entityId;
        [Key(1)]
        public bool isPlayer;
        public enum EntityStatGrowthSpeed { Slow, Medium, Fast };
        [Key(2)]
        public int level;
        [Key(3)]
        public int xp;
        [Key(4)]
        public EriduRpgStat strength;
        [Key(5)]
        public EriduRpgStat stamina;
        [Key(6)]
        public EriduRpgStat hp;
        [Key(7)]
        public EriduRpgStat mp;
        [Key(8)]
        public EriduRpgStat atk;
        [Key(9)]
        public EriduRpgStat armor;
        [Key(10)]
        public EriduRpgStat attackPower;
        [Key(11)]
        public EriduRpgStat critChance;

        public EriduRpgEntityModel(int level, EntityStatGrowthSpeed curveSpeed = EntityStatGrowthSpeed.Medium) {
            entityId = 0;
            isPlayer = false;

            float speedCurveMultiplier = 1f;
            if (curveSpeed == EntityStatGrowthSpeed.Slow)
                speedCurveMultiplier = .25f;
            else if (curveSpeed == EntityStatGrowthSpeed.Fast)
                speedCurveMultiplier = 1.35f;

            this.level = level;
            xp = 0;
            strength = new EriduRpgStat() { baseValue = (int)Math.Floor(level * 10 * speedCurveMultiplier), currentValue = (int)Math.Floor(level * 10 * speedCurveMultiplier) };
            stamina = new EriduRpgStat() { baseValue = (int)Math.Floor(level * 10 * speedCurveMultiplier), currentValue = (int)Math.Floor(level * 10 * speedCurveMultiplier) };
            hp = new EriduRpgStat() { baseValue = (int)Math.Floor((float)stamina.baseValue * 25), currentValue = (int)Math.Floor((float)stamina.baseValue * 25) };
            mp = new EriduRpgStat() { baseValue = (int)Math.Floor(level * 40 * speedCurveMultiplier), currentValue = (int)Math.Floor(level * 40 * speedCurveMultiplier) };
            atk = new EriduRpgStat() { baseValue = (int)Math.Floor(level * 5 * speedCurveMultiplier), currentValue = (int)Math.Floor(level * 5 * speedCurveMultiplier) };
            armor = new EriduRpgStat() { baseValue = (int)Math.Floor(level * 5 * speedCurveMultiplier), currentValue = (int)Math.Floor(level * 5 * speedCurveMultiplier) };
            attackPower = new EriduRpgStat() { baseValue = (int)Math.Floor(level * 5 * speedCurveMultiplier), currentValue = (int)Math.Floor(level * 5 * speedCurveMultiplier) };
            critChance = new EriduRpgStat() { baseValue = (int)Math.Floor(level * 5 * speedCurveMultiplier), currentValue = (int)Math.Floor(level * 5 * speedCurveMultiplier) };

        }

        public bool IsDead {
            get {
                return hp.currentValue <= 0;
            }
        }
    }

    [System.Serializable]
    public struct EriduRpgStat {
        public enum RpgStats { Strength, Stamina, HP, MP, Damage, Defense, AttackPower, CriticalChance };
        [Key(0)]
        public int baseValue;
        [Key(1)]
        public int currentValue;
    }

    public struct EriduRpgAuraEffect {
        [Key(0)]
        public bool isPermanent;
        [Key(1)]
        public float duration;
        [Key(2)]
        public System.DateTime startTime;
        [Key(3)]
        public EriduRpgStat.RpgStats statAffected;
        [Key(4)]
        public int modifier;
    }

    public struct EriduRpgAura {
        [Key(0)]
        public EriduRpgAuraEffect[] auraEffects;
    }
}