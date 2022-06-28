using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DH
{
    public class UIMng : SingletonMini<UIMng>
    {
        public GameObject runnerUI;
        public GameObject runnerMoveJoystick;
        public GameObject runnerRotateJoystick;


        public GameObject taggerUI;
        public GameObject taggerMoveJoystick;
        public GameObject taggerRotateJoystick;

        public Button jumpBtn;
        public Button actionBtn;
        public Button testHideBtn;

        public UnityAction testHideAction;
        public UnityAction jumpAction;
        public UnityAction actionAction;


        private void Start()
        {
            
        }

        protected override void OnAwake()
        {

        }

        public void SetUI(string who) 
        {
            switch(who)
            {
                case "Runner":
                    runnerUI.SetActive(true);
                    taggerUI.SetActive(false);
                    break;
                case "Tagger":
                    runnerUI.SetActive(false);
                    taggerUI.SetActive(true);
                    break;
            }
        }

        public void SetMoveUI(PlayerMove move)
        {
            runnerMoveJoystick.GetComponent<VirtualJoyStick>().controller = move;
            runnerRotateJoystick.GetComponent<VirtualJoyStick>().controller = move;

            taggerMoveJoystick.GetComponent<VirtualJoyStick>().controller = move;
            taggerRotateJoystick.GetComponent<VirtualJoyStick>().controller = move;
        }

        public void OnTestHideButton()
        {
            testHideAction?.Invoke();
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
