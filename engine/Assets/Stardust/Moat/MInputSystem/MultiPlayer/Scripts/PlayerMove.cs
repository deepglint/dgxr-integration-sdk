using System.Collections;
using System.Collections.Generic;
using Moat.Model;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Moat
{
    public class PlayerMove : MonoBehaviour
    {
        public GameObject Player;
        public GameObject PlayerBody;
        public Canvas PlayerCanvas;
        public RectTransform PlayerCircleRect;
        public Text PlayerNameText;
        [SerializeField] private int userId;
        private float ratio = 1f;
        public float duration = 2f;
        private Vector2 startPosition;
        private Vector2 endPosition;
        private float elapsedTime = 0f;

        void Start()
        {
            DisplayData.ReadConfig();
            float realSpaceSize = DisplayData.configDisplay.resolution.realResolution;
            float screenResolution = DisplayData.configDisplay.resolution.systemWidth;
            ratio = screenResolution / realSpaceSize;

            userId = Player.GetComponent<PlayerInput>().playerIndex + 1;
            if (PlayerBody != null) PlayerBody.SetActive(false);
            if (PlayerCanvas != null)
            {
                // TODO 添加判断是否显示设备光圈
                PlayerCanvas.gameObject.SetActive(true);
                PlayerNameText.text = $"{char.ToUpper(Player.GetComponent<PlayerInput>().name[0])}{userId}";
            }
        }

        public void Move(Vector2 position, bool isRealSpace)
        {
            if (isRealSpace)
            {
                elapsedTime = 0;
                startPosition = PlayerCircleRect.anchoredPosition;
                endPosition = new Vector2(position.x * ratio, position.y * ratio);
                // PlayerCircleRect.anchoredPosition = new Vector2(position.x * ratio, position.y * ratio);
                StartCoroutine(MoveRawImage());
            }
            else
            {
                PlayerCircleRect.anchoredPosition = position;
            }
        }
        
        IEnumerator MoveRawImage()
        {
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                PlayerCircleRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
                yield return null;
            }

            PlayerCircleRect.anchoredPosition = endPosition;
        }
    }
}