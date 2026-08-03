 using System;

namespace _1_2D_Top_Down
{
    public enum PlayerStatType
    {
        MaxHealth,
        MaxMana,
        HealthRegen,
        ManaRegen,
        Damage,
        Knockback,
        MoveSpeed,
        ProjectileSpeed,
        ProjectileCount,
        AttackSpeed,
        CritRate,
        CritDamage,
        Armor,
        FireResistance,
        FrostResistance,
        LightningResistance,
        PoisonResistance
    }

    public sealed class PlayerStats
    {
        public int MaxHealth { get; private set; } = 100;
        public float MaxMana { get; private set; } = 100f;
        public float HealthRegen { get; private set; } = 0.75f;
        public float ManaRegen { get; private set; } = 12f;

        public int Damage { get; private set; } = 1;
        public float Knockback { get; private set; } = 360f;
        public float MoveSpeed { get; private set; } = 400f;
        public float ProjectileSpeed { get; private set; } = 600f;
        public int ProjectileCount { get; private set; } = 1;

        // Разстоянието в градуси
        public float ProjectileSpreadAngleDegrees { get; } = 20f;
        // Ще се използват по-късно
        public float AttackSpeed { get; private set; } = 1f;
        public float CritRate { get; private set; } = 0f;
        public float CritDamage { get; private set; } = 1.5f;
        public float Armor { get; private set; } = 0f;
        public float FireResistance { get; private set; } = 0f;
        public float FrostResistance { get; private set; } = 0f;
        public float LightningResistance { get; private set; } = 0f;
        public float PoisonResistance { get; private set; } = 0f;

        public void Add(PlayerStatType stat, float amount)
        {
            switch (stat)
            {
                case PlayerStatType.MaxHealth:
                    MaxHealth = Math.Max(1, MaxHealth + (int)MathF.Round(amount));
                    break;

                case PlayerStatType.MaxMana:
                    MaxMana = Math.Max(1f, MaxMana + amount);
                    break;

                case PlayerStatType.HealthRegen:
                    HealthRegen = Math.Max(0f, HealthRegen + amount);
                    break;

                case PlayerStatType.ManaRegen:
                    ManaRegen = Math.Max(0f, ManaRegen + amount);
                    break;

                case PlayerStatType.Damage:
                    Damage = Math.Max(0, Damage + (int)MathF.Round(amount));
                    break;

                case PlayerStatType.Knockback:
                    Knockback = Math.Max(0f, Knockback + amount);
                    break;

                case PlayerStatType.MoveSpeed:
                    MoveSpeed = Math.Max(0f, MoveSpeed + amount);
                    break;

                case PlayerStatType.ProjectileSpeed:
                    ProjectileSpeed = Math.Max(0f, ProjectileSpeed + amount);
                    break;
                case PlayerStatType.ProjectileCount:
                    ProjectileCount = Math.Max( 1, ProjectileCount + (int)MathF.Round(amount));
                    break;

                case PlayerStatType.AttackSpeed:
                    AttackSpeed = Math.Max(0.01f, AttackSpeed + amount);
                    break;

                case PlayerStatType.CritRate:
                    CritRate = Math.Clamp(CritRate + amount, 0f, 100f);
                    break;

                case PlayerStatType.CritDamage:
                    CritDamage = Math.Max(1f, CritDamage + amount);
                    break;

                case PlayerStatType.Armor:
                    Armor = Math.Max(0f, Armor + amount);
                    break;

                case PlayerStatType.FireResistance:
                    FireResistance = Math.Clamp(FireResistance + amount, 0f, 100f);
                    break;

                case PlayerStatType.FrostResistance:
                    FrostResistance = Math.Clamp(FrostResistance + amount, 0f, 100f);
                    break;

                case PlayerStatType.LightningResistance:
                    LightningResistance = Math.Clamp(LightningResistance + amount, 0f, 100f);
                    break;

                case PlayerStatType.PoisonResistance:
                    PoisonResistance = Math.Clamp(PoisonResistance + amount, 0f, 100f);
                    break;
            }
        }
    }
}