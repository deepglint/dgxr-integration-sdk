﻿using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.SharedComponents.GameExitButton
{
    internal class GameExitButton : MonoBehaviour
    {
        private bool _isExiting;
        private float _duration = 5f;
        private float _size = 200f;
        private float _countdownTimer;

        private RawImage _buttonImg;
        private Text _timeText;
        private GameObject _exitText;


        private GameObject _bottomCanvas;
        private GameObject _frontCanvas;
        private GameObject _gameExitButtonPrefab;
        private GameObject _gameExitingPrefab;
        private Text _gameExitingText;
        private GameExitButtonPointerListener _pointerListener;

        public void Start()
        {
            _bottomCanvas = GameObject.Find("ToolkitCanvas/Bottom");
            _frontCanvas = GameObject.Find("ToolkitCanvas/Front");
            _gameExitButtonPrefab =
                Instantiate(Resources.Load<GameObject>("GameExitButton"), _bottomCanvas.transform, false);
            _gameExitingPrefab =
                Instantiate(Resources.Load<GameObject>("GameExiting"), _frontCanvas.transform, false);
            _gameExitingText = _gameExitingPrefab.FindChildGameObject("GameExitingText").GetComponent<Text>();
            _pointerListener = _gameExitButtonPrefab.GetComponent<GameExitButtonPointerListener>();
            _pointerListener.OnEnter += OnEnter;
            _pointerListener.OnExit += OnExit;
            _buttonImg = _gameExitButtonPrefab.FindChildGameObject("GameExitButton_inner").GetComponent<RawImage>();
            _timeText = _gameExitButtonPrefab.FindChildGameObject("GameExitButton_inner")
                .FindChildGameObject("TimeText")
                .GetComponent<Text>();
            _exitText = _gameExitButtonPrefab.FindChildGameObject("GameExitButton_inner")
                .FindChildGameObject("ExitText");
            RectTransform rectTransform = _gameExitButtonPrefab.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new(-850, DGXR.Space.Roi.y + 110);

            rectTransform.localScale = new Vector3(_size, _size, _size);
            _gameExitingPrefab.SetActive(false);
            _gameExitButtonPrefab.SetActive(true);
            _countdownTimer = 0;
            SetAlpha(0.5f);
        }

        private void Update()
        {
            if (!_gameExitButtonPrefab.activeSelf) return;
            if (Mathf.RoundToInt(_countdownTimer).ToString() == "0")
            {
                _gameExitingText.text = "退出游戏中";
                _timeText.text = "";
                _timeText.gameObject.SetActive(false);
                _exitText.SetActive(true);
            }
            else
            {
                _gameExitingText.text = $"{Mathf.RoundToInt(_countdownTimer).ToString()}秒后退出游戏";
                _gameExitingPrefab.SetActive(true);
                _timeText.text = Mathf.RoundToInt(_countdownTimer).ToString();
                _timeText.gameObject.SetActive(true);
                _exitText.SetActive(false);
            }

            if (_isExiting)
            {
                _countdownTimer -= Time.deltaTime;
                if (_countdownTimer <= 0f)
                {
                    ExecuteCallback();
                }
            }
            else
            {
                if (_gameExitingPrefab != null) _gameExitingPrefab.SetActive(false);
                if (_timeText != null) _timeText.gameObject.SetActive(false);
                if (_exitText != null) _exitText.SetActive(true);
            }
        }

        private void ExecuteCallback()
        {
            GameExit.Quit();
        }

        public void OnEnter()
        {
            if (_isExiting) return;
            _countdownTimer = Time.deltaTime + _duration;
            _isExiting = true;
            SetAlpha(1);
        }

        public void OnExit()
        {
            if (!_isExiting) return;
            _countdownTimer = Time.deltaTime + _duration;
            _isExiting = false;
            SetAlpha(0.5f);
        }

        private void SetAlpha(float alpha)
        {
            var imgColor = _buttonImg.color;
            imgColor.a = alpha;
            _buttonImg.color = imgColor;
            var text = _exitText.GetComponent<Text>();
            var textColor = text.color;
            textColor.a = alpha;
            text.color = textColor;
        }
    }
}