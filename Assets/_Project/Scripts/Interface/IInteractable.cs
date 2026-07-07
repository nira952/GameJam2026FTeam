public interface IInteractable
{
    // インタラクトされたときに実行する関数
    void Interact();

    string GetInteractPrompt();
}