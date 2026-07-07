using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class TaikoScript : MonoBehaviour, IInteractable
{
    public string GetInteractPrompt()
    {
        return "叩く";
    }

    public void Interact()
    {
        Debug.Log("太鼓を叩いた！");

        //TODO : ここにカメラ移動の処理を追加する
    }
}