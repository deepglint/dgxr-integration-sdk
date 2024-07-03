using Deepglint.XR.Space;
using Deepglint.XR.Toolkit.UIFrame;
using Scene.UiSample;
using UnityEngine;

namespace Samples.UI
{
    public class UiSample : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            UIModule.Create<HeaderInfo>(TargetScreen.Front);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
