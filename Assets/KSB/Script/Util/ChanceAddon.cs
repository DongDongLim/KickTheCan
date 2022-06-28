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

    
    public int ChanceThree(float first,float second,float third)
    {
        if (100 != (first+second+third))
            return 99;

        chanceBase = Random.Range(1,1001);
        chanceBase = chanceBase * 0.1f;

        // EX 확률 : first% second% third%

        // 0 보다 높고 first 와 같거나 작으면 first%
        if (0 < chanceBase && first >= chanceBase)
        {
            return 0;
        }
        // first 보다 높고 first + second 와 같거나 작으면 second%
        else if (first < chanceBase && (second + first) >= chanceBase)
        {
            return 1;
        }
        // first + second 보다 높고 100 와 같거나 작으면 third%
        else if ((second + first) < chanceBase && 100 >= chanceBase)
        {
            return 2;
        }
        return 99;  
    }
}
