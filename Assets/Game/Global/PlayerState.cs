using System;

namespace Assets.Game.Global
{
    public static class PlayerState
    {
        public static event Action<int> MaxHealthChanged;

        private static int maxHealth = 100;
        public static int MaxHealth
        {
            get => maxHealth;
            set
            {
                if (maxHealth == value)
                {
                    return;
                }
                maxHealth = value;
                maxHealth = Math.Max(1, value);

                MaxHealthChanged.Invoke(maxHealth);
            }
        }

        public static event Action<int> HealthChanged;

        private static int health = 100;
        public static int Health
        {
            get => health;
            set
            {
                if (health == value)
                {
                    return;
                }
                health = Math.Clamp(value, 0, maxHealth);

                HealthChanged.Invoke(health);
            }
        }

        private static int recievedDamage = 0;
        public static int RecievedDamage
        {
            get => recievedDamage;
            set
            {
                // Prevent overriding damage before it properly applied
                if (recievedDamage == value || recievedDamage != 0)
                {
                    return;
                }
                recievedDamage = value;
            }
        }

        public static void ResetRecievedDamage()
        {
            recievedDamage = 0;
        }
    }
}
