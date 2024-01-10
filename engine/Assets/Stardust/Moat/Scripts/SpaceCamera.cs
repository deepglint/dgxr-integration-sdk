using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moat
{
    public enum SpaceCameraPosition : int
    {
        Front = 0,
        Right = 1,
        Back = 2,
        Left = 3,
    }

    public class SpaceCamera : MonoBehaviour
    {
        public SpaceCameraPosition SpaceCameraPos;
        public GameObject FrontWall;
        public GameObject BackWall;
        public GameObject LeftWall;
        public GameObject RightWall;

        private SpaceCameraPosition _spaceCameraPos;

        // Start is called before the first frame update
        void Start()
        {
            
            SpaceCameraPos = SpaceCameraPosition.Front;
            _spaceCameraPos = SpaceCameraPosition.Front;
            SetSpaceCamera();
        }

        void SetSpaceCamera()
        {
            BackWall.SetActive(true);
            FrontWall.SetActive(true);
            LeftWall.SetActive(true);
            RightWall.SetActive(true);
            switch (_spaceCameraPos)
            {
                case SpaceCameraPosition.Front:
                    FrontWall.SetActive(false);
                    transform.position = new Vector3(0, 2, 4);
                    transform.rotation = Quaternion.Euler(new Vector3(21, 180, 0));
                    break;
                case SpaceCameraPosition.Right:
                    BackWall.SetActive(false);
                    RightWall.SetActive(false);
                    transform.position = new Vector3(3, 2, -2);
                    transform.rotation = Quaternion.Euler(new Vector3(20, -52, 0));
                    break;
                case SpaceCameraPosition.Back:
                    BackWall.SetActive(false);
                    transform.position = new Vector3(0, 2, -4);
                    transform.rotation = Quaternion.Euler(new Vector3(20, 0, 0));
                    break;
                case SpaceCameraPosition.Left:
                    FrontWall.SetActive(false);
                    LeftWall.SetActive(false);
                    transform.position = new Vector3(-3, 2, 2);
                    transform.rotation = Quaternion.Euler(new Vector3(20, 130, 0));
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_spaceCameraPos != SpaceCameraPos)
            {
                _spaceCameraPos = SpaceCameraPos;
                SetSpaceCamera();
            }
        }
    }
}