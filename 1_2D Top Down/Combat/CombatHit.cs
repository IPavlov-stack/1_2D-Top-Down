using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public readonly struct CombatHit
    {
        public int Damage { get; }
        public DamageType DamageType { get; }
        public CombatFaction SourceFaction { get; }
        public string SourceId { get; }
        public Vector2 HitPosition { get; }
        public float Knockback { get; }
        public bool CanBeReflected { get; }

        public CombatHit(
            int damage,
            DamageType damageType,
            CombatFaction sourceFaction,
            string sourceId,
            Vector2 hitPosition,
            float knockback = 0f,
            bool canBeReflected = false)
        {
            Damage = damage;
            DamageType = damageType;
            SourceFaction = sourceFaction;
            SourceId = sourceId ?? string.Empty;
            HitPosition = hitPosition;
            Knockback = knockback;
            CanBeReflected = canBeReflected;
        }
    }
}
