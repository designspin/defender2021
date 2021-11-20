using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RadarObject {
    public Image icon { get; set; }
    public GameObject owner;
}

public class Radar : MonoBehaviour
{
    public Transform playerPos;

    public Image playerIcon;

    private Material radarRenderer;
    private RectTransform radarRect;

    private void Awake()
    {
        radarRenderer = GetComponent<Image>().material;
        radarRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        playerIcon = Instantiate(playerIcon);
        playerIcon.gameObject.transform.SetParent(this.transform);
        playerIcon.gameObject.transform.localPosition = Vector3.zero;


    }

    public static List<RadarObject> radarObjects = new List<RadarObject>();

    public static void RegisterRadarObject(GameObject o, Image i) {
        Image image = Instantiate(i);
        radarObjects.Add(new RadarObject() { owner = o, icon = image });
    }
    
    public static void RemoveRadarObject(GameObject o)
    {
        List<RadarObject> newList = new List<RadarObject>();
        for(int i = 0; i < radarObjects.Count; i++)
        {
            if(radarObjects[i].owner == o)
            {
                Destroy(radarObjects[i].icon);
                continue;
            }
            else
                newList.Add(radarObjects[i]);
        }
        radarObjects.RemoveRange(0, radarObjects.Count);
        radarObjects.AddRange(newList);
    }

    float wrapItem(float target, float lower, float upper)
    {
        float diff = upper - lower;
        if(target > lower)
            return lower + (target - lower) % diff;
        
        target = lower + (lower - target);
        float tmp = lower + (target - lower) % diff;
        return upper - tmp;
    }
    void LateUpdate()
    {
        var playerYBounds = playerPos.gameObject.GetComponent<Player>().PlayerYBounds;
        playerIcon.transform.localPosition = new Vector3(playerIcon.transform.localPosition.x, playerPos.position.y.Remap(playerYBounds.x, playerYBounds.y, -105f, 105f), playerIcon.transform.localPosition.z);
        
        //Update mountain position on Radar;
        radarRenderer.SetFloat("_OffsetX", playerPos.position.x / 7 );

        for(int i = 0; i < radarObjects.Count; i++)
        {
            if(radarObjects[i].icon.transform.parent == null) {
                radarObjects[i].icon.transform.SetParent(this.transform);
            }

            var radarObjectX = radarObjects[i].owner.transform.position.x;
            var sum = radarObjectX - playerPos.position.x;
            
            if(sum < -3.5) {
                sum = 3.5f - Mathf.Abs(sum) % 3.5f;
            }
            

            float x = (sum).Remap(-3.5f, 3.5f, -420, 420);
            
            float y = radarObjects[i].owner.transform.position.y.Remap(playerYBounds.x, playerYBounds.y, -105f, 105f);
            radarObjects[i].icon.transform.localPosition = new Vector3(wrapItem(x, -420, 420), y, 0);
        }
    }
}
