using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // 키보드 마우스 터치를 이벤트로 오브젝트에 보낼 수 있는 기능을 지원

public class VirtualJoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform lever;
    private RectTransform rectTransform;

    [SerializeField,Range(10, 150)]
    private float leverRange;

    private Vector2 inputDirection;
    private bool isInput;

    [SerializeField]
    private SeekerController controller;

    public enum JoyStickType { Move, Rotate }
    public JoyStickType joyStickType;

    public float rotateSensitivity = 1f;  // 카메라 회전 속도
    public float moveSensitivity = 1f;  // 캐릭터 회전 속도

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ControlJoyStickLever(eventData.position);
        isInput = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        ControlJoyStickLever(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lever.anchoredPosition = Vector3.zero;
        isInput = false;

        switch(joyStickType)
        {
            case JoyStickType.Move:
                controller.Move(Vector2.zero);
                break;
            case JoyStickType.Rotate:
                break;
        }
    }

    private void ControlJoyStickLever(Vector2 vecTouch)
    {
        // Vector2 vec = new Vector2(vecTouch - rectTransform.position);
        // vec = Vector2.ClampMagnitude(vec, )
        
        var inputPos = new Vector2(vecTouch.x - rectTransform.position.x, vecTouch.y - rectTransform.position.y);
        var inputVector = inputPos.magnitude < leverRange ? inputPos : inputPos.normalized * leverRange;
        lever.localPosition = inputVector;
        inputDirection = inputVector / leverRange;
    }

    private void InputControlVector()
    {
        // 캐릭터에게 입력 벡터를 전달
        switch(joyStickType)
        {
            case JoyStickType.Move:
                controller.Move(inputDirection * moveSensitivity);
                break;
            case JoyStickType.Rotate:
                controller.LookAround(inputDirection * rotateSensitivity);
                break;
        }
    }

    private void Update() {
        if (isInput)
        {
            InputControlVector();
        }
    }
}
