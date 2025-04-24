using Assets.Game.Global;
using UnityEngine;

public class RadarScript : MonoBehaviour
{
    public float activeScreenRadiusRatio = 0.63f;
    public float maxRadarDistance = 30f;

    public RectTransform radar;
    public Transform pointCollection;

    public GameObject enemyPoint;
    public GameObject healthPoint;


    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        UpdateEntities(EntityType.Enemy, enemyPoint);
        UpdateEntities(EntityType.FirstAid, healthPoint);
    }

    private void UpdateEntities(EntityType entityType, GameObject prefab)
    {
        foreach (var entity in GameEntities.GetCollection(entityType))
        {
            if(entity.RadarPoint == null)
            {
                entity.RadarPoint = Instantiate(prefab, pointCollection);
            }
            UpdateEntity(entity);
        }
        
    }

    private void UpdateEntity(EntityData entity)
    {
        float screenRadius = radar.rect.width * activeScreenRadiusRatio * 0.75f;
        Vector3 d = entity.MainObject.transform.position - player.position;
        Vector3 camFwd = Camera.main.transform.forward;
        d.y = 0;
        camFwd.y = 0;
        float angle = Vector3.SignedAngle(camFwd, d, Vector3.down);
        float r = d.magnitude / maxRadarDistance * screenRadius;
        
        if(r > screenRadius)
        {
            entity.RadarPoint.SetActive(false);
            return;
        }

        if (!entity.RadarPoint.activeInHierarchy)
        {
            entity.RadarPoint.SetActive(true);
        }

        entity.RadarPoint.transform.localPosition = new Vector3(
            -r * Mathf.Sin(angle * Mathf.Deg2Rad), 
            r * Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
