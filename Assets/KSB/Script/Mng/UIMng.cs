using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace DH
{
    public class UIMng : SingletonMini<UIMng>
    {
        public GameObject observerUI;
        public GameObject observerMoveJoystick;
        public GameObject observerRotateJoystick;

        public GameObject runnerUI;
        public GameObject runnerMoveJoystick;
        public GameObject runnerRotateJoystick;

        public GameObject taggerUI;
        public GameObject taggerMoveJoystick;
        public GameObject taggerRotateJoystick;
        public GameObject[] attackCount;

        public Button jumpBtn;
        public Button actionBtn;
        public Button testHideBtn;

        public UnityAction testHideAction;
        public UnityAction jumpAction;
        public UnityAction actionAction;

        public GameObject loadingScreen;
        public GameObject taggerCaption;
        public GameObject runnerCaption;
        public GameObject startCount;

        public TextMeshProUGUI runnerCount;
        public TextMeshProUGUI taggerCount;


        private void Start()
        {
            loadingScreen.SetActive(true);
        }

        protected override void OnAwake()
        {

        }

        public void SetAttackCount(int count)
        {
            for (int i = 1; i < attackCount.Length+1; i++)
            {
                if (count >= i)
                {
                    attackCount[i-1].SetActive(true);
                }
                else
                {
                    attackCount[i-1].SetActive(false);
                }
            }
        }

        public void SetUI(string who) 
        {
            switch(who)
            {
                case "Observer":
                    observerUI.SetActive(true);
                    runnerUI.SetActive(false);
                    taggerUI.SetActive(false);
                    break;
                case "Runner":
                    observerUI.SetActive(false);
                    runnerUI.SetActive(true);
                    taggerUI.SetActive(false);
                    break;
                case "Tagger":
                    observerUI.SetActive(false);
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

            observerMoveJoystick.GetComponent<VirtualJoyStick>().controller = move;
            observerRotateJoystick.GetComponent<VirtualJoyStick>().controller = move;
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
