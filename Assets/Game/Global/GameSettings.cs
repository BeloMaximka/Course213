using System;

namespace Assets.Game.Global
{
    public static class GameSettings
    {
        public static event Action<float> EffectsVolumeChanged;

        private static float effectsVolume = 0.5f;
        public static float EffectsVolume
        {
            get => effectsVolume;
            set
            {
                if (effectsVolume == value)
                {
                    return;
                }
                effectsVolume = value;

                if (!isMuted && EffectsVolumeChanged != null)
                {
                    EffectsVolumeChanged.Invoke(effectsVolume);
                }
            }
        }

        public static event Action<float> MusicVolumeChanged;

        private static float musicVolume = 0.25f;
        public static float MusicVolume
        {
            get => musicVolume;
            set
            {
                if (musicVolume == value)
                {
                    return;
                }

                musicVolume = value;
                if (!isMuted && MusicVolumeChanged != null)
                {
                    MusicVolumeChanged.Invoke(musicVolume);
                }
            }
        }

        private static bool isMuted = false;
        public static bool IsMuted
        {
            get => isMuted;
            set
            {
                if (isMuted == value)
                {
                    return;
                }

                isMuted = value;
                MusicVolumeChanged.Invoke(isMuted ? 0 : musicVolume);
                EffectsVolumeChanged.Invoke(isMuted ? 0 : effectsVolume);
            }
        }

        public static event Action<DifficultyType> DifficultyChanged;

        private static DifficultyType difficulty = DifficultyType.Easy;
        public static DifficultyType Difficulty
        {
            get => difficulty;
            set
            {
                if (difficulty == value)
                {
                    return;
                }

                difficulty = value;
                DifficultyChanged.Invoke(difficulty);
            }
        }

        public static float MouseSensitivityX { get; set; } = 2f;
        public static float MouseSensitivityY { get; set; } = 2f;
        public static bool IsMouseSensitivityShared { get; set; } = true;
    }
}
