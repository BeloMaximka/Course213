using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets.Game.Global
{
    public static class GameEntities
    {
        private readonly static LinkedList<GameObject>[] entities = new LinkedList<GameObject>[Enum.GetValues(typeof(EntityType)).Cast<int>().Max() + 1];

        static GameEntities()
        {
            for (var i = 0; i < entities.Length; i++)
            {
                entities[i] = new();
            }
        }

        public static IReadOnlyCollection<GameObject> GetCollection(EntityType entityType)
        {
            return entities[(int)entityType];
        }

        public static void Add(EntityType entityType, GameObject entity)
        {
            entities[(int)entityType].AddLast(entity);
        }

        public static void Remove(EntityType entityType, GameObject entity)
        {
            entities[(int)entityType].Remove(entity);
        }
    }
}
