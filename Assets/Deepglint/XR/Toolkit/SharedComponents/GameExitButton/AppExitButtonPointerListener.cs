using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;


namespace Deepglint.XR.Toolkit.SharedComponents.GameExitButton
{
    internal class GameExitButtonPointerListener : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public Action OnEnter;
        public Action OnExit;
        private readonly List<string> _playerIds = new();

        public void OnPointerEnter(PointerEventData eventData)
        {
            string playerId = eventData.pointerId.ToString();
            if (_playerIds.Contains(playerId)) return;
            _playerIds.Add(playerId);
            OnEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
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