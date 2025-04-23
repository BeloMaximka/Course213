using System;

namespace Assets.Game
{
    public static class GameEvents
    {
        public static event Action NightBegin;
        public static event Action DayBegin;

        public static void OnNightBegin()
        {
            NightBegin?.Invoke();
        }

        public static void OnDayBegin()
        {
            DayBegin?.Invoke();
        }
    }
}
