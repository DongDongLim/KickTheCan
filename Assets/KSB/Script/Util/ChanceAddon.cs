using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceAddon
{
    private float chanceBase;
    public bool Chance(float percent)
    {
        // 확률이 0% 일 경우
        if (0 >= percent)
            return false;

        // 확률이 100% 일 경우
        if (100 <= percent)
            return true;

        chanceBase = Random.Range(1,1001);
        chanceBase = chanceBase * 0.1f;

        if (percent >= chanceBase)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
