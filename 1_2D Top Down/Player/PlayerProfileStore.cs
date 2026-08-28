using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Stores one versioned JSON document per profile and remembers which
    /// profile was active. A custom directory can be supplied by tests/tools.
    /// </summary>
    public sealed class PlayerProfileStore
    {
        private const int CurrentSaveVersion = 1;
        private const string ActiveProfileFileName = "active-profile.txt";
        private readonly JsonSerializerOptions jsonOptions;

        public string ProfilesDirectory { get; }

        public PlayerProfileStore(string? profilesDirectory = null)
        {
            ProfilesDirectory = profilesDirectory ?? Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "A guy called Pesho",
                "Profiles");

            jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
            jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public PlayerProfile? LoadActive()
        {
            string activeProfilePath = Path.Combine(
                ProfilesDirectory,
                ActiveProfileFileName);
            if (!File.Exists(activeProfilePath))
                return null;

            string idText = File.ReadAllText(activeProfilePath).Trim();
            if (!Guid.TryParse(idText, out Guid profileId))
                return null;

            return Load(profileId);
        }

        public PlayerProfile? Load(Guid profileId)
        {
            string profilePath = GetProfilePath(profileId);
            if (!File.Exists(profilePath))
                return null;

            string json = File.ReadAllText(profilePath);
            PlayerProfileSaveDocument? document =
                JsonSerializer.Deserialize<PlayerProfileSaveDocument>(
                    json,
                    jsonOptions);

            if (document == null || document.Version != CurrentSaveVersion)
            {
                throw new InvalidDataException(
                    $"Unsupported player profile save version: " +
                    $"{document?.Version.ToString() ?? "missing"}.");
            }

            return FromSaveData(document.Profile);
        }

        public IReadOnlyList<PlayerProfile> LoadAll()
        {
            if (!Directory.Exists(ProfilesDirectory))
                return Array.Empty<PlayerProfile>();

            List<PlayerProfile> profiles = new();
            foreach (string path in Directory.EnumerateFiles(
                         ProfilesDirectory,
                         "*.json"))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    PlayerProfileSaveDocument? document =
                        JsonSerializer.Deserialize<PlayerProfileSaveDocument>(
                            json,
                            jsonOptions);
                    if (document?.Version == CurrentSaveVersion)
                        profiles.Add(FromSaveData(document.Profile));
                }
                catch (Exception exception) when (
                    exception is IOException or
                    UnauthorizedAccessException or
                    JsonException or
                    InvalidDataException or
                    ArgumentException)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Skipping invalid profile '{path}': {exception.Message}");
                }
            }

            return profiles
                .OrderByDescending(profile => profile.UpdatedUtc)
                .ToArray();
        }

        public void Save(PlayerProfile profile, bool makeActive = true)
        {
            ArgumentNullException.ThrowIfNull(profile);
            Directory.CreateDirectory(ProfilesDirectory);

            PlayerProfileSaveDocument document = new()
            {
                Version = CurrentSaveVersion,
                Profile = ToSaveData(profile)
            };
            string json = JsonSerializer.Serialize(document, jsonOptions);

            WriteAtomically(GetProfilePath(profile.Id), json);
            if (makeActive)
            {
                WriteAtomically(
                    Path.Combine(
                        ProfilesDirectory,
                        ActiveProfileFileName),
                    profile.Id.ToString("D"));
            }
        }

        public void Delete(Guid profileId)
        {
            string profilePath = GetProfilePath(profileId);
            if (File.Exists(profilePath))
                File.Delete(profilePath);

            string activeProfilePath = Path.Combine(
                ProfilesDirectory,
                ActiveProfileFileName);
            if (!File.Exists(activeProfilePath))
                return;

            string activeIdText = File.ReadAllText(activeProfilePath).Trim();
            if (Guid.TryParse(activeIdText, out Guid activeProfileId) &&
                activeProfileId == profileId)
            {
                File.Delete(activeProfilePath);
            }
        }

        private string GetProfilePath(Guid profileId)
        {
            if (profileId == Guid.Empty)
                throw new ArgumentException(
                    "A profile id is required.",
                    nameof(profileId));

            return Path.Combine(
                ProfilesDirectory,
                $"{profileId:N}.json");
        }

        private static void WriteAtomically(string targetPath, string contents)
        {
            string temporaryPath = targetPath + ".tmp";

            try
            {
                File.WriteAllText(temporaryPath, contents);
                File.Move(temporaryPath, targetPath, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
        }

        private static PlayerProfileSaveData ToSaveData(PlayerProfile profile)
        {
            return new PlayerProfileSaveData
            {
                Id = profile.Id,
                Name = profile.Name,
                Class = profile.Class,
                Level = profile.Experience.Level,
                CurrentExperience = profile.Experience.CurrentExperience,
                TotalExperience = profile.Experience.TotalExperience,
                CreatedUtc = profile.CreatedUtc,
                UpdatedUtc = profile.UpdatedUtc,
                CompletedMissionIds = profile.CompletedMissionIds.ToList(),
                UnlockedAbilityIds = profile.UnlockedAbilityIds.ToList(),
                Resources = new Dictionary<string, int>(profile.Resources),
                ShopPurchaseCounts = new Dictionary<string, int>(
                    profile.ShopPurchaseCounts)
            };
        }

        private static PlayerProfile FromSaveData(PlayerProfileSaveData data)
        {
            if (data == null)
                throw new InvalidDataException("The profile data is missing.");

            Experience experience = new();
            experience.Restore(
                data.Level,
                data.CurrentExperience,
                data.TotalExperience);

            return new PlayerProfile(
                data.Id,
                data.Name,
                data.Class,
                experience,
                data.CreatedUtc,
                data.UpdatedUtc,
                data.CompletedMissionIds,
                data.UnlockedAbilityIds,
                data.Resources,
                data.ShopPurchaseCounts);
        }

        private sealed class PlayerProfileSaveDocument
        {
            public PlayerProfileSaveDocument()
            {
            }

            public int Version { get; set; }
            public PlayerProfileSaveData Profile { get; set; } = new();
        }

        private sealed class PlayerProfileSaveData
        {
            public PlayerProfileSaveData()
            {
            }

            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public PlayerClass Class { get; set; }
            public int Level { get; set; } = 1;
            public int CurrentExperience { get; set; }
            public long TotalExperience { get; set; }
            public DateTimeOffset CreatedUtc { get; set; }
            public DateTimeOffset UpdatedUtc { get; set; }
            public List<string> CompletedMissionIds { get; set; } = new();
            public List<string> UnlockedAbilityIds { get; set; } = new();
            public Dictionary<string, int> Resources { get; set; } = new();
            public Dictionary<string, int> ShopPurchaseCounts { get; set; } =
                new();
        }
    }
}
