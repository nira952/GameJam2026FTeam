using Rina_Script;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class TaikoScript : MonoBehaviour, IInteractable
{
    [SerializeField] private WorpPoint worpPoint;

    public PlayerState CurrentState => PlayerState.Move;

    public string GetInteractPrompt()
    {
        return "戻る";
    }

    public PlayerState Interact(Transform targetTransform)
    {
        Debug.Log("元の位置に戻る！");

        if (worpPoint != null)
        {
            targetTransform.position = worpPoint.transform.position;
        }



        return CurrentState;
    }
}