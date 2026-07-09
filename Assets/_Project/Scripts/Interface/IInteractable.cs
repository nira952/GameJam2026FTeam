using Rina_Script;
using UnityEngine;

public interface IInteractable
{
    public PlayerState CurrentState { get; }

    // インタラクトされたときに実行する関数
    PlayerState Interact(Transform targetTransform);

    string GetInteractPrompt();
}