using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Game.Global
{
    public static class GameEntities
    {
        private readonly static LinkedList<EntityData>[] entities = new LinkedList<EntityData>[Enum.GetValues(typeof(EntityType)).Cast<int>().Max() + 1];

        static GameEntities()
        {
            for (var i = 0; i < entities.Length; i++)
            {
                entities[i] = new();
            }
        }

        public static IReadOnlyCollection<EntityData> GetCollection(EntityType entityType)
        {
            return entities[(int)entityType];
        }

        public static void Add(EntityType entityType, GameObject entity)
        {
            entities[(int)entityType].AddLast(new EntityData { MainObject = entity });
        }

        public static void Destroy(EntityType entityType, GameObject entity)
        {
            var result = entities[(int)entityType].FirstOrDefault((relatetObjects) => relatetObjects.MainObject == entity);
            if (result is not null)
            {
                entities[(int)entityType].Remove(result);
                UnityEngine.Object.Destroy(result.MainObject);
                if(result.RadarPoint != null)
                {
                    UnityEngine.Object.Destroy(result.RadarPoint);
                }
            }
        }
    }
}
