using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatedUI : MonoBehaviour
{
    private void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        transform.localScale = Vector3.one;
    }

    //캠버스가 스크린 언더바 오버레이 에서 -> 스크린 언더바 카메라로 변경이 되었다. 
    // 오버레인은 화면에 띄우는거고 -> 카메라는 카메라 기준으로 띄우는거다.
    // 기준이 되는 카메라에 맞춰서 캠버스가 변경되기 때문에 
}
