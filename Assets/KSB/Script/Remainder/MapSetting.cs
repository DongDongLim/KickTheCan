using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSetting : MonoBehaviour
{
    [SerializeField]
    private Transform[] objectSpawnPosList;
    private int value;
    private int count;


    public void SetObjectSpawnPosList(ref Transform[] objectSpawnPos)
    {
        value = 0;
        for (int i = 0; i < objectSpawnPosList.Length; i++)
        {
            value += objectSpawnPosList[i].childCount;
        }
        objectSpawnPos = new Transform[value];

        count = 0;

        for (int i = 0; i < objectSpawnPosList.Length; i++)
        {
            for (int j = 0; j < objectSpawnPosList[i].childCount; j++)
            {
                objectSpawnPos[count] = objectSpawnPosList[i].GetChild(j);
                count++;
            }
        }
    }
}
