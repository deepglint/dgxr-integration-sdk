using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Deepglint.XR.Toolkit.SharedComponents.GameExitButton
{
    internal class AppExitButtonPointerListener : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler
    {
        public Action OnEnter;
        public Action OnExit;
        private readonly List<string> _playerIds = new();

        public void OnPointerDown(PointerEventData eventData)
        {
            string playerId = eventData.pointerId.ToString();
            if (_playerIds.Contains(playerId)) return;
            _playerIds.Add(playerId);
            OnEnter?.Invoke();
            Debug.LogFormat(
                eventData.pointerId > 0 ? "human {0} right foot down at {1}" : "human {0} with left foot down {1}",
                eventData.pointerId, name);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            string playerId = eventData.pointerId.ToString();
            if (!_playerIds.Contains(playerId)) return;
            _playerIds.Remove(playerId);
            if (_playerIds.Count <= 0)
            {
                OnExit?.Invoke();
            }
            Debug.LogFormat(
                eventData.pointerId > 0 ? "human {0} right foot up from {1}" : "human {0} with left foot up from {1}",
                eventData.pointerId, name);
        }

        public void OnDestroy()
        {
            OnEnter = null;
            OnExit = null;
        }
    }
}