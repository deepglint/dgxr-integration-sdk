using Deepglint.XR.Toolkit.DebugTool;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.SharedComponents.GameExitButton
{
    internal class AppExitButton : MonoBehaviour
    {
        private static bool _isExiting;
        private static float _duration = 5f;
        private static float _countdownTimer;

        private static RawImage _buttonImg;
        private static Text _timeText;
        private static GameObject _exitText;

        private static GameObject _bottomCanvas;
        private static GameObject _frontCanvas;
        private static GameObject _gameExitButtonPrefab;
        private static GameObject _gameExitingPrefab;
        private static Text _gameExitingText;
        private static AppExitButtonPointerListener _pointerListener;
        private static AudioSource _audio;

        public static void Create()
        {
            _bottomCanvas = GameObject.Find("ToolkitCanvas/Bottom");
            _frontCanvas = GameObject.Find("ToolkitCanvas/Front");
            _gameExitButtonPrefab =
                Instantiate(Resources.Load<GameObject>("GameExitButton"), _bottomCanvas.transform, false);
            _gameExitingPrefab =
                Instantiate(Resources.Load<GameObject>("GameExiting"), _frontCanvas.transform, false);
            _gameExitingText = _gameExitingPrefab.FindChildGameObject("GameExitingText").GetComponent<Text>();
            _pointerListener = _gameExitButtonPrefab.GetComponent<AppExitButtonPointerListener>();
            _pointerListener.OnEnter += OnEnter;
            _pointerListener.OnExit += OnExit;
            _buttonImg = _gameExitButtonPrefab.FindChildGameObject("GameExitButton_inner").GetComponent<RawImage>();
            _timeText = _gameExitButtonPrefab.FindChildGameObject("GameExitButton_inner")
                .FindChildGameObject("TimeText")
                .GetComponent<Text>();
            _exitText = _gameExitButtonPrefab.FindChildGameObject("GameExitButton_inner")
                .FindChildGameObject("ExitText");
            _audio = _gameExitButtonPrefab.GetComponent<AudioSource>();
            SetButtonRect();

            _gameExitingPrefab.SetActive(false);
            _gameExitButtonPrefab.SetActive(true);
            _countdownTimer = 0;
            SetAlpha(0.5f);
        }

        private static void SetButtonRect()
        {
            float padding = 20;
            float x;
            float y;
            float radius;

            RectTransform rectTransform = _gameExitButtonPrefab.GetComponent<RectTransform>();
            radius = rectTransform.sizeDelta.x / 2;
            x = -960 + radius + padding;

            if (DGXR.Space.Roi.x > x + radius + padding)
            {
                y = DGXR.Space.Roi.y + radius + padding;
            }
            else
            {
                y = DGXR.Space.Roi.y + radius + padding + DGXR.Space.Roi.height;
            }

            rectTransform.anchoredPosition = new(x, y);
        }

        private void Update()
        {
            if (DGXR.ApplicationSettings.toolkit.enableExitButton)
            {
                if (!_gameExitButtonPrefab.activeSelf) return;

                if (_isExiting)
                {
                    int previousSecond = Mathf.FloorToInt(_countdownTimer);
                    _countdownTimer -= Time.deltaTime;
                    int currentSecond = Mathf.FloorToInt(_countdownTimer);

                    if (currentSecond != previousSecond)
                    {
                        _gameExitingText.text = $"{currentSecond}秒后退出应用";
                        _gameExitingPrefab.SetActive(true);
                        _timeText.text = currentSecond.ToString();
                        _timeText.gameObject.SetActive(true);
                        _exitText.SetActive(false);
                        _audio.Play();
                    }

                    if (_countdownTimer <= 1)
                    {
                        _gameExitingText.text = "退出应用中";
                        _timeText.text = "";
                        _timeText.gameObject.SetActive(false);
                        _exitText.SetActive(true);
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
            else
            {
                _gameExitingPrefab.SetActive(false);
                _gameExitButtonPrefab.SetActive(false);
            }
        }


        private static void ExecuteCallback()
        {
            AppExit.Quit();
        }

        public static void OnEnter()
        {
            if (_isExiting) return;

            _countdownTimer = Time.deltaTime + _duration + 1;
            _isExiting = true;
            SetAlpha(1);
        }

        public static void OnExit()
        {
            if (!_isExiting) return;

            _countdownTimer = Time.deltaTime + _duration + 1;
            _isExiting = false;
            SetAlpha(0.5f);
        }

        private static void SetAlpha(float alpha)
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