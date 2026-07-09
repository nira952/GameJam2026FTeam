using Rina_Script;
using Simizu;
using UnityEngine;

public class WorpPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private TaikoScript taikoScript;

    [SerializeField] private CameraPos cameraPos;

    public PlayerState CurrentState => PlayerState.Thunder;

    public string GetInteractPrompt()
    {
        return "ワープ";
    }

    public PlayerState Interact(Transform targetTransform)
    {
        // ワープ処理を実行
        if (taikoScript != null)
        {
            Debug.Log("ワープ処理を実行");

            targetTransform.position = taikoScript.transform.position;

            CameraManager.Instance.ChangeCamera(cameraPos);
        }

        return CurrentState;
    }
}
