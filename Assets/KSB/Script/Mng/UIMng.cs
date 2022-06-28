using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DH
{
    public class UIMng : SingletonMini<UIMng>
    {
        public GameObject moveJoystick;
        public GameObject rotateJoystick;
        public Button jumpBtn;
        public Button actionBtn;

        public UnityAction jumpAction;
        public UnityAction actionAction;

        private void Start()
        {
            
        }

        protected override void OnAwake()
        {

        }

        public void SetMoveUI(PlayerMove move)
        {
            moveJoystick.GetComponent<VirtualJoyStick>().controller = move;
            rotateJoystick.GetComponent<VirtualJoyStick>().controller = move;
            
        }


        public void OnJumpButton()
        {
            jumpAction?.Invoke();
        }
        public void OnActionButton()
        {
            actionAction?.Invoke();
        }

        public void OnLobbyButton()
        {

        }

        public void OnExitButton()
        {

        }

        IEnumerator CheckEscapeButton()
        {
            yield return new WaitForSeconds(0.1f);                    
        }

    }
}
