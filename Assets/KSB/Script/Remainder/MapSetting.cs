using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSetting : MonoBehaviour
{
    [SerializeField]
    private Transform taggerSpawnPos;
    [SerializeField]
    private Transform runnerSpawnPos;
    [SerializeField]
    private Transform[] objectSpawnPosList;
    // CanLink / DH
    [SerializeField]
    private Transform[] canSpqwnPosList;

    private int value;
    private int count;

    private void Awake() {
        SetPlayerSpawnPos();
    }

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

    public void SetPlayerSpawnPos()
    {
        DH.MapSettingMng.instance.taggerSpawnPos = taggerSpawnPos.position;
        DH.MapSettingMng.instance.runnerSpawnPos = runnerSpawnPos.position;
    }


    // CanLink / DH
    public Vector3 CanSpqwnPos()
    {
        int rand = Random.Range(0, canSpqwnPosList.Length);
        DH.MapSettingMng.instance.canTransform = canSpqwnPosList[rand].position;
        return canSpqwnPosList[rand].position;
    }

}
