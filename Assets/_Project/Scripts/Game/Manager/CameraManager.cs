using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using Unity.Cinemachine.Editor;
using Unity.Cinemachine.TargetTracking;
using Rina_Script;

namespace Simizu
{
    public enum CameraPos
    {
        LeftUpper,      // 左上
        LeftLower,      // 左下
        RightUpper,     // 右上
        RightLower,     // 右下
        Center,         // 中央
        Player,         // プレイヤー
    }


    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        // プレイヤーを追従するカメラ
        [SerializeField] public CinemachineCamera pCamera;

        // 他のカメラ
        [SerializeField] public CinemachineCamera[] nCameras;

        private CinemachineCamera currentCamera;

        // 非選択時のバーチャルカメラの優先度
        [SerializeField] private int unselectedPriority = 0;

        // 選択時のバーチャルカメラの優先度
        [SerializeField] private int selectedPriority = 10;

        private void Start()
        {
            currentCamera = pCamera;
            ResetCamera();
        }

        public void ChangeCamera(CameraPos cameraPos)
        {
            currentCamera.Priority = unselectedPriority;
            nCameras[(int)cameraPos].Priority = selectedPriority;
        }

        public void ResetCamera()
        {
            for(int i = 0;  i < nCameras.Length; i++)
            {
                nCameras[i].Priority = unselectedPriority;
            }
            currentCamera.Priority = selectedPriority;
        }
    }
}