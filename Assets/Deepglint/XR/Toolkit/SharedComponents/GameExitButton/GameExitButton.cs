using Deepglint.XR.Toolkit.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.SharedComponents.GameExitButton
{
    internal class GameExitButton : MonoBehaviour
    {
        private float _duration = 5f;
        private float _size = 200f;

        private bool _isIntersecting;
        private float _countdownTimer;

        private RawImage _buttonImg;
        private TextMeshProUGUI _timeText;
        private GameObject _exitText;


        private GameObject _bottomCanvas;
        private GameObject _cameraRoiPrefab;
        private GameExitButtonPointerListener _pointerListener;

        public void Start()
        {
            _bottomCanvas = GameObject.Find("ToolkitCanvas/Bottom");
            _cameraRoiPrefab =
                Instantiate(Resources.Load<GameObject>("GameExitButton"), _bottomCanvas.transform, false);
            _pointerListener = _cameraRoiPrefab.GetComponent<GameExitButtonPointerListener>();
            _pointerListener.OnEnter += OnEnter;
            _pointerListener.OnExit += OnExit;
            _buttonImg = _cameraRoiPrefab.FindChildGameObject("GameExitButton_inner").GetComponent<RawImage>();
            _timeText = _cameraRoiPrefab.FindChildGameObject("GameExitButton_inner").FindChildGameObject("TimeText")
                .GetComponent<TextMeshProUGUI>();
            _exitText = _cameraRoiPrefab.FindChildGameObject("GameExitButton_inner").FindChildGameObject("ExitText");
            RectTransform rectTransform = _cameraRoiPrefab.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new(850, DGXR.Space.Roi.y + 110);

            rectTransform.localScale = new Vector3(_size, _size, _size);

            _cameraRoiPrefab.SetActive(true);
            _countdownTimer = 0;
            SetAlpha(0.5f);
        }

        private void Update()
        {
            if (!_cameraRoiPrefab.activeSelf) return;
            if (Mathf.RoundToInt(_countdownTimer).ToString() == "0")
            {
                _timeText.text = "";
                _timeText.gameObject.SetActive(false);
                _exitText.SetActive(true);
            }
            else
            {
                _timeText.text = Mathf.RoundToInt(_countdownTimer).ToString();
                _timeText.gameObject.SetActive(true);
                _exitText.SetActive(false);
            }

            if (_isIntersecting)
            {
                _countdownTimer -= Time.deltaTime;
                if (_countdownTimer <= 0f)
                {
                    ExecuteCallback();
                }
            }
            else
            {
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
            if (_isIntersecting) return;
            _countdownTimer = Time.deltaTime + _duration;
            _isIntersecting = true;
            SetAlpha(1);
        }

        public void OnExit()
        {
            if (!_isIntersecting) return;
            _countdownTimer = Time.deltaTime + _duration;
            _isIntersecting = false;
            SetAlpha(0.5f);
        }

        private void SetAlpha(float alpha)
        {
            var color = _buttonImg.color;
            color.a = alpha;
            _buttonImg.color = color;
            _exitText.GetComponent<TextMeshProUGUI>().alpha = alpha;
        }
    }
}