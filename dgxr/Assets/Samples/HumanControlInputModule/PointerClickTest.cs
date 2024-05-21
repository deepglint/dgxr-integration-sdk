using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Samples.UI
{
    public class PointerClickTest : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if (pointerEventData.button == PointerEventData.InputButton.Right)
            {
                //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
                Debug.Log(name + " Game Object Right Clicked!");
            }
            
            //Use this to tell when the user left-clicks on the Button
            if (pointerEventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log(name + " Game Object Left Clicked!");
            }
        }
    }
}