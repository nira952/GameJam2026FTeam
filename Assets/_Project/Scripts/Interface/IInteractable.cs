using Rina_Script;

public interface IInteractable
{
    // インタラクトされたときに実行する関数
    PlayerState Interact();

    string GetInteractPrompt();
}