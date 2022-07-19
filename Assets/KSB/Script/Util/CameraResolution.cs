using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    [SerializeField]
    bool isBlack = false;
    
    private void Awake()
    {
        Camera cam = GetComponent<Camera>();
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1080; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        float fixedAspectRatio = 1.777f;

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            cam.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            cam.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
        float currentAspectRatio = (float)Screen.width / (float)Screen.height;
        if (currentAspectRatio == fixedAspectRatio)
        {
            cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            return;
        }
        else if (currentAspectRatio > fixedAspectRatio)
        {
            float w = fixedAspectRatio / currentAspectRatio;
            float x = (1 - w) / 2;
            cam.rect = new Rect(x, 0.0f, w, 1.0f);
        }
        else if (currentAspectRatio < fixedAspectRatio)
        {
            float h = currentAspectRatio / fixedAspectRatio;
            float y = (1 - h) / 2;
            cam.rect = new Rect(0.0f, y, 1.0f, h);
        }
    }

    void OnPreCull()
    {
        if (!isBlack)
            GL.Clear(true, true, Color.black);
    }

}
