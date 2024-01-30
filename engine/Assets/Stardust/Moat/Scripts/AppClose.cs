using System;
using System.Collections;
using Moat.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Moat 
{
    public class AppClose : MonoBehaviour
    {
        private static AppClose _instance;
        public static AppClose Instance 
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AppClose>();
                    if (_instance == null)
                    {
                        Debug.LogError("AppClose instance not found!");
                    }
                }
                return _instance;
            }
        }

        public bool IsOpen { get; private set; }

        public Action OnClose;

        [SerializeField]  GameObject closeButtonObject; // Use SerializeField to expose private fields in the editor
        [SerializeField] private Text countdownText;

        private const int TimeMax = 5;
        private int _currentTime;
        private float _closeCountdown;
        private float _startTime;
        private int closeActionCount = 0;
        
        private Coroutine openCoroutine;
        private Coroutine countdownCoroutine;

        private void Awake()
        {
            EventManager.RegisterListener(ActionEvent.OnHandsCross, OnHandsCross);
            EventManager.RegisterListener(ActionEvent.OnRaiseOnHand, OnRaiseOnHand);
            _currentTime = TimeMax;
            if (_instance != null && _instance != this)
            {
                Debug.LogError("Another instance of AppClose exists!");
            }
            else
            {
                _instance = this;
            }
        }

        private void Start()
        {
            closeButtonObject.SetActive(false);
        }
        
        private void OnHandsCross(EventCallBack evt)
        {
            MDebug.Log("交互 - OnHandsCross - 双手交叉:" + DisplayData.configDisplay.allowClose);
            if (!DisplayData.configDisplay.allowClose) return;
            AppClose.Instance.OpenThrottle();
        }
        
        private void OnRaiseOnHand(EventCallBack evt)
        {
            MDebug.Log("交互 - OnRaiseOnHand - 举单手");
            if (!DisplayData.configDisplay.allowClose) return;
            AppClose.Instance.CloseApp();
        }

        public void CloseApp()
        {
            if (IsOpen)
            {
                MDebug.LogFlow("====== Closing current application =====");
                GameAppManager.Instance.CloseApp(); // Placeholder for actual close logic
                CloseDialog();
            }
        }

        private void DecrementTimer()
        {
            if (countdownText != null && _currentTime > 0)
            {
                countdownText.text = _currentTime.ToString();
                _currentTime--;
            }
        }

        public void OpenThrottle(int interval = 5)
        {
            if (IsOpen)
            {
                ResetCountdown();
                return;
            }

            if (_startTime == 0)
            {
                _startTime = Time.time;
                openCoroutine = StartCoroutine(CheckCloseCount(interval));
            }
            closeActionCount++; 
            // Debug.LogError(Mathf.Ceil(Time.time - _startTime));
        }

        private IEnumerator CheckCloseCount(int interval)
        {
            while (Time.time - _startTime < interval)
            {
                _closeCountdown = Mathf.Ceil(Time.time - _startTime);
                yield return null;
            }

            MDebug.LogFlow($"_closeCountdown >= interval: {_closeCountdown} {interval} {closeActionCount}");
            if (_closeCountdown >= interval && closeActionCount >= interval)
            {
                OpenDialog();
            }
            ResetCountdown();
        }

        public void OpenDialog()
        {
            if (IsOpen) return;

            IsOpen = true;
            _currentTime = TimeMax;
            StartCoroutine(CloseDialogAfterDelay(TimeMax));
            MDebug.LogFlow("------ Opening [Close Dialog] ------ " + _currentTime);
            closeButtonObject.SetActive(true);
            countdownCoroutine = StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            while (_currentTime > 0)
            {
                DecrementTimer();
                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator CloseDialogAfterDelay(int delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            CloseDialog();
        }

        private void CloseDialog()
        {
            if (!IsOpen) return;

            IsOpen = false;
            MDebug.LogFlow("Closing AppClose dialog " + _currentTime);
            closeButtonObject.SetActive(false);
            OnClose?.Invoke();
        }

        private void ResetCountdown()
        {
            _closeCountdown = 0;
            _startTime = 0;
            closeActionCount = 0;
            if (openCoroutine != null)
            {
                StopCoroutine(openCoroutine);
                openCoroutine = null; 
            }
        }

        private void OnDestroy()
        {
            ResetCountdown();
            EventManager.RemoveListener(ActionEvent.OnHandsCross, OnHandsCross);
            EventManager.RemoveListener(ActionEvent.OnRaiseOnHand, OnRaiseOnHand);
        }
    }
}