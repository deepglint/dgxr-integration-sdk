using System.Collections.Generic;
using System.Threading.Tasks;
using Deepglint.XR.Space;
using UnityEngine;
using UnityEngine.Serialization;

namespace Deepglint.XR.Toolkit.RoamStick
{
    public class RoamController : MonoBehaviour 
    {
        public enum JumpStatus 
        {
            Idle,
            Charging,
            Jumping
        }
        
        private GameObject _roamCharacterObj;
        private GameObject _xrManager;
        private GameObject _roamStickObj;
        private RoamStick _roamStick;
        private RoamCharacter _roamCharacter;
        private RoamCharacterController Character;
        
        private Vector3 _currentPos = new Vector3(0, 1.6f, 0);
        [FormerlySerializedAs("_jumpStatus")] public JumpStatus jumpStatus = JumpStatus.Idle; 
        
        // SmoothingFilter
        private readonly float _alpha = 0.1f;
        private Vector3 _lastSmoothedPos = Vector3.zero;
        private readonly List<Vector3> _cacheList = new List<Vector3>();
        private Vector3 _currentMovePos = Vector3.zero;
        
        // speed
        [FormerlySerializedAs("Speed")] public float speed = 3;
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";
        public bool hasPlayer;

        private void LateUpdate()
        {
            if (_xrManager == null) _xrManager = GameObject.Find("XRManager");
            if (_roamCharacterObj == null) return;

            // _xrManager.transform.position = _roamCharacter.GetCameraTarget();
            _xrManager.transform.position = Character.CameraFollowPoint.transform.position;
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            if (hasPlayer)
            {
                characterInputs.MoveAxisForward = _roamStick.roamDirection.y;
                characterInputs.MoveAxisRight = _roamStick.roamDirection.x;
                characterInputs.JumpDown = jumpStatus == JumpStatus.Jumping;
                characterInputs.CrouchDown = jumpStatus == JumpStatus.Charging;
                characterInputs.CrouchUp = jumpStatus == JumpStatus.Jumping;
            }
            else
            {
                // Build the CharacterInputs struct
                characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
                characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
                // characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
                characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
                characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
                characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C); 
            }

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }
        
        private void SetCache(Vector3 pos)
        {
            _cacheList.Add(pos);
            _currentMovePos = Vector3.Lerp(_lastSmoothedPos, pos, _alpha);
            _lastSmoothedPos = _currentMovePos;
            if (_cacheList.Count > 10) _cacheList.RemoveAt(0);
        }
        
        public void Start()
        {
            GameObject uiRoot = GameObject.Find("UIRoot");
            _roamStickObj = Instantiate(Resources.Load<GameObject>("RoamStick"), uiRoot.transform);
            _roamStick = _roamStickObj.GetComponent<RoamStick>();
            _roamCharacterObj = Instantiate(Resources.Load<GameObject>("RoamCharacter"), transform);
            _roamCharacter = _roamCharacterObj.GetComponent<RoamCharacter>();
            Character = _roamCharacterObj.GetComponent<RoamCharacterController>();
            SetActive(false);
        }

        public void SetActive(bool status)
        {
            _roamStickObj?.SetActive(status);
            _roamCharacterObj?.SetActive(status);
        }

        public void Reset()
        { 
            _roamStick.ControlStick(Vector2.zero);
            _roamCharacterObj.transform.position = new Vector3(0, 3f, 0);
        }

        public void Move(Vector3 position3d, Vector2 position2d)
        {
            _roamStick.SetActiveJoystickTouch(false);
            SetCache(position3d);
            bool isMove = _roamStick.CheckCrossBorder(position2d);
            _roamStick.ControlStick(isMove ? new Vector2(position3d.x, position3d.z).normalized : Vector2.zero); 
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _xrManager.GetComponent<SpaceManager>().isCave = !_xrManager.GetComponent<SpaceManager>().isCave;
            }
            
            HandleCharacterInput();
        }

        private void FixedUpdateTmp()
        {
            if (_xrManager == null) _xrManager = GameObject.Find("XRManager");
            if (_roamCharacterObj == null) return;
            
            _xrManager.transform.position = _roamCharacter.GetCameraTarget() - _currentMovePos;
            SetCave();
        
            if (_roamStick.roamDirection == Vector2.zero) return;
            var position = _roamCharacterObj.transform.position;
            position += (new Vector3(_roamStick.roamDirection.x, 0, _roamStick.roamDirection.y) * Time.deltaTime * speed);
            position = new Vector3(position.x, Mathf.Max(0f, position.y), position.z);
            _roamCharacterObj.transform.position = position;
            _roamCharacter.SetHeight(_currentPos.y); 
        }

        private void SetCave()
        {
            if (_xrManager.GetComponent<SpaceManager>().isCave)
            {
                _xrManager.GetComponent<SpaceManager>().lockAll = false;
                DGXR.CavePosition = _currentMovePos;
            }
            else
            {
                DGXR.CavePosition = _xrManager.transform.position + new Vector3(0, 1.6f, 0);
            }
        }
        
        public void Charging()
        {
            if (jumpStatus != JumpStatus.Idle) return;
            jumpStatus = JumpStatus.Charging;
        }

        private void RestCharging()
        {
            jumpStatus = JumpStatus.Idle;
        }

        // 跳跃方法
        public async void Jump()
        {
            if (jumpStatus == JumpStatus.Jumping) return;
            jumpStatus = JumpStatus.Jumping;
            await Task.Delay(100);
            RestCharging();
        }
    }
}
