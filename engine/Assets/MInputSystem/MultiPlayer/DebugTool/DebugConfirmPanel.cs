using System;
using UnityEngine;
using UnityEngine.UI;

public class DebugConfirmPanel : MonoBehaviour
{
    public Action OnLeave;
    public Action OnControl;

    private Button _leaveButton;
    private Button _controlButton;
    
    public void SetOnLeave(Action callback)
    {
        OnLeave = callback;
    }

    public void SetOnControl(Action callback)
    {
        OnControl = callback;
    }

    public void SetButtons(Button leaveButton, Button controlButton)
    {
        _leaveButton = leaveButton;
        _controlButton = controlButton;
    }

    private void Start()
    {
        _leaveButton.onClick.AddListener(() =>
        {
            OnLeave?.Invoke();
            gameObject.SetActive(false);
        });
        _controlButton.onClick.AddListener(() =>
        {
            OnControl?.Invoke();
            gameObject.SetActive(false);
        });
    }
}
