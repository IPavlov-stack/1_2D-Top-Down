using System;
using System.Collections.Generic;

#nullable enable

namespace _1_2D_Top_Down
{
    public enum PlayerClass
    {
        Unassigned,
        Warrior,
        Mage,
        Hunter
    }

    // Persistent player data. Runtime-only state such as current health and
    // position deliberately stays on Player and GameplaySession.
    public sealed class PlayerProfile
    {
        private readonly HashSet<string> completedMissionIds;
        private readonly HashSet<string> unlockedAbilityIds;
        private readonly Dictionary<string, int> resources;
        private readonly Dictionary<string, int> shopPurchaseCounts;

        public Guid Id { get; }
        public string Name { get; private set; }
        public PlayerClass Class { get; private set; }
        public Experience Experience { get; }
        public DateTimeOffset CreatedUtc { get; }
        public DateTimeOffset UpdatedUtc { get; private set; }

        public IReadOnlyCollection<string> CompletedMissionIds =>
            completedMissionIds;
        public IReadOnlyCollection<string> UnlockedAbilityIds =>
            unlockedAbilityIds;
        public IReadOnlyDictionary<string, int> Resources => resources;
        public IReadOnlyDictionary<string, int> ShopPurchaseCounts =>
            shopPurchaseCounts;

        public event Action? Changed;

        public PlayerProfile()
            : this("Player", PlayerClass.Warrior)
        {
        }

        public PlayerProfile(Experience experience)
            : this(
                Guid.NewGuid(),
                "Player",
                PlayerClass.Warrior,
                experience,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                null)
        {
        }

        public PlayerProfile(string name, PlayerClass playerClass)
            : this(
                Guid.NewGuid(),
                name,
                playerClass,
                new Experience(),
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                null)
        {
        }

        internal PlayerProfile(
            Guid id,
            string name,
            PlayerClass playerClass,
            Experience experience,
            DateTimeOffset createdUtc,
            DateTimeOffset updatedUtc,
            IEnumerable<string>? completedMissions,
            IEnumerable<string>? unlockedAbilities,
            IReadOnlyDictionary<string, int>? savedResources,
            IReadOnlyDictionary<string, int>? savedShopPurchaseCounts)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Name = NormalizeName(name);
            Class = playerClass;
            Experience = experience ?? throw new ArgumentNullException(nameof(experience));
            CreatedUtc = createdUtc == default
                ? DateTimeOffset.UtcNow
                : createdUtc;
            UpdatedUtc = updatedUtc == default ? CreatedUtc : updatedUtc;

            completedMissionIds = CreateIdSet(completedMissions);
            unlockedAbilityIds = CreateIdSet(unlockedAbilities);
            resources = CreateCountDictionary(savedResources);
            shopPurchaseCounts = CreateCountDictionary(
                savedShopPurchaseCounts);

            Experience.Changed += MarkChanged;
        }

        public void Rename(string name)
        {
            string normalizedName = NormalizeName(name);
            if (Name == normalizedName)
                return;

            Name = normalizedName;
            MarkChanged();
        }

        public bool TrySelectClass(PlayerClass playerClass)
        {
            if (playerClass == PlayerClass.Unassigned ||
                (Class != PlayerClass.Unassigned && Class != playerClass))
            {
                return false;
            }

            if (Class == playerClass)
                return true;

            Class = playerClass;
            MarkChanged();
            return true;
        }

        public bool CompleteMission(string missionId)
        {
            string normalizedId = NormalizeId(missionId, nameof(missionId));
            if (!completedMissionIds.Add(normalizedId))
                return false;

            MarkChanged();
            return true;
        }

        public bool HasCompletedMission(string missionId) =>
            completedMissionIds.Contains(
                NormalizeId(missionId, nameof(missionId)));

        public bool UnlockAbility(string abilityId)
        {
            string normalizedId = NormalizeId(abilityId, nameof(abilityId));
            if (!unlockedAbilityIds.Add(normalizedId))
                return false;

            MarkChanged();
            return true;
        }

        public bool HasUnlockedAbility(string abilityId) =>
            unlockedAbilityIds.Contains(
                NormalizeId(abilityId, nameof(abilityId)));

        public int GetResourceAmount(string resourceId)
        {
            string normalizedId = NormalizeId(resourceId, nameof(resourceId));
            return resources.TryGetValue(normalizedId, out int amount)
                ? amount
                : 0;
        }

        public void AddResource(string resourceId, int amount)
        {
            if (amount <= 0)
                return;

            string normalizedId = NormalizeId(resourceId, nameof(resourceId));
            resources[normalizedId] = checked(
                GetResourceAmount(normalizedId) + amount);
            MarkChanged();
        }

        public bool TrySpendResource(string resourceId, int amount)
        {
            if (amount <= 0)
                return false;

            string normalizedId = NormalizeId(resourceId, nameof(resourceId));
            int currentAmount = GetResourceAmount(normalizedId);
            if (currentAmount < amount)
                return false;

            int remainingAmount = currentAmount - amount;
            if (remainingAmount == 0)
                resources.Remove(normalizedId);
            else
                resources[normalizedId] = remainingAmount;

            MarkChanged();
            return true;
        }

        public int GetShopPurchaseCount(string shopItemId)
        {
            string normalizedId = NormalizeId(
                shopItemId,
                nameof(shopItemId));
            return shopPurchaseCounts.TryGetValue(normalizedId, out int count)
                ? count
                : 0;
        }

        public void SetShopPurchaseCount(string shopItemId, int count)
        {
            string normalizedId = NormalizeId(
                shopItemId,
                nameof(shopItemId));
            int normalizedCount = Math.Max(0, count);

            if (GetShopPurchaseCount(normalizedId) == normalizedCount)
                return;

            if (normalizedCount == 0)
                shopPurchaseCounts.Remove(normalizedId);
            else
                shopPurchaseCounts[normalizedId] = normalizedCount;

            MarkChanged();
        }

        private void MarkChanged()
        {
            UpdatedUtc = DateTimeOffset.UtcNow;
            Changed?.Invoke();
        }

        private static string NormalizeName(string name)
        {
            string normalizedName = (name ?? string.Empty).Trim();
            if (normalizedName.Length == 0)
                throw new ArgumentException(
                    "A player profile must have a name.",
                    nameof(name));
            if (normalizedName.Length > 32)
                throw new ArgumentException(
                    "A player profile name cannot exceed 32 characters.",
                    nameof(name));

            return normalizedName;
        }

        private static string NormalizeId(string id, string parameterName)
        {
            string normalizedId = (id ?? string.Empty).Trim();
            if (normalizedId.Length == 0)
                throw new ArgumentException("An id is required.", parameterName);

            return normalizedId;
        }

        private static HashSet<string> CreateIdSet(IEnumerable<string>? ids)
        {
            HashSet<string> result = new(StringComparer.OrdinalIgnoreCase);
            if (ids == null)
                return result;

            foreach (string id in ids)
            {
                if (!string.IsNullOrWhiteSpace(id))
                    result.Add(id.Trim());
            }

            return result;
        }

        private static Dictionary<string, int> CreateCountDictionary(
            IReadOnlyDictionary<string, int>? values)
        {
            Dictionary<string, int> result =
                new(StringComparer.OrdinalIgnoreCase);
            if (values == null)
                return result;

            foreach ((string id, int count) in values)
            {
                if (!string.IsNullOrWhiteSpace(id) && count > 0)
                    result[id.Trim()] = count;
            }

            return result;
        }
    }
}
