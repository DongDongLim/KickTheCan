using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSetting : MonoBehaviour
{
    [SerializeField]
    private Transform[] objectSpawnPosList;
    private int value;
    private int count;

    private void Awake() {
        value = 0;
        for (int i = 0; i < objectSpawnPosList.Length; i++)
        {
            value += objectSpawnPosList[i].childCount;
        }
        DH.MapSettingMng.instance.objectSpawnPos = new Transform[value];
        
        count = 0;

        for (int i = 0; i < objectSpawnPosList.Length; i++)
        {
            for (int j = 0; j < objectSpawnPosList[i].childCount; j++)
            {
                DH.MapSettingMng.instance.objectSpawnPos[count] = objectSpawnPosList[i].GetChild(j);
                count++;
            }
        }
    }
}
