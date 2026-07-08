using Rina_Script;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class TaikoScript : MonoBehaviour, IInteractable
{
    public string GetInteractPrompt()
    {
        return "叩く";
    }

    public PlayerState Interact()
    {
        Debug.Log("太鼓モードON！");

        //TODO : ここにカメラ移動の処理を追加する

        return PlayerState.Thunder;
    }
}