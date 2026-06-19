using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyButton : MonoBehaviour
{
    [Tooltip("Nome exato da cena de gameplay (precisa estar no Build Settings).")]
    [SerializeField] private string gameplaySceneName = "BattleScene";

    public void OnReadyPressed()
    {
        if (string.IsNullOrEmpty(gameplaySceneName))
        {
            Debug.LogError("[ReadyButton] Nome da cena de gameplay não definido.");
            return;
        }

        Debug.Log($"[ReadyButton] Carregando cena: {gameplaySceneName}");
        SceneManager.LoadScene(gameplaySceneName);
    }
}