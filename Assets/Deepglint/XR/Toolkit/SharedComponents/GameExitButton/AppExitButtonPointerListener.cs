using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Deepglint.XR.Toolkit.SharedComponents.GameExitButton
{
    internal class AppExitButtonPointerListener : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public Action OnEnter;
        public Action OnExit;
        private readonly List<string> _playerIds = new();

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerId > 0)
            {
                Debug.Log($"human {eventData.pointerId} right foot with position: {eventData.position}, world position: {eventData.worldPosition} entered into exit-button");
            }
            else
            {
                Debug.Log($"human {eventData.pointerId} left foot with position: {eventData.position}, world position: {eventData.worldPosition} entered into exit-button");
            }
            string playerId = eventData.pointerId.ToString();
            if (_playerIds.Contains(playerId)) return;
            _playerIds.Add(playerId);
            OnEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerId > 0)
            {
                Debug.Log($"human {eventData.pointerId} right foot with position: {eventData.position}, world position: {eventData.worldPosition} exited from exit-button");
            }
            else
            {
                Debug.Log($"human {eventData.pointerId} left foot with position: {eventData.position}, world position: {eventData.worldPosition} exited from exit-button");
            }
            string playerId = eventData.pointerId.ToString();
            if (!_playerIds.Contains(playerId)) return;
            _playerIds.Remove(playerId);
            if (_playerIds.Count <= 0)
            {
                OnExit?.Invoke();
            }
        }

        public void OnDestroy()
        {
            OnEnter = null;
            OnExit = null;
        }
    }
}