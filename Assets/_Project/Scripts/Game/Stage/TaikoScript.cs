using Rina_Script;
using Simizu;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class TaikoScript : MonoBehaviour, IInteractable
{
    [SerializeField] private CameraPos cameraPos; 

    public string GetInteractPrompt()
    {
        return "叩く";
    }

    public PlayerState Interact()
    {
        Debug.Log("太鼓モードON！");

        //TODO : ここにカメラ移動の処理を追加する
        CameraManager.Instance.ChangeCamera(cameraPos);

        return PlayerState.Thunder;
    }
}