using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyButton : MonoBehaviour
{
    [Tooltip("Nome exato da cena de gameplay (precisa estar na Scene List do Build Profile).")]
    [SerializeField] private string gameplaySceneName = "BattleScene";

    public void OnReadyPressed()
    {
        if (string.IsNullOrEmpty(gameplaySceneName))
        {
            Debug.LogError("[ReadyButton] Nome da cena de gameplay não definido.");
            return;
        }

        if (PartyManager.Instance == null)
        {
            Debug.LogError("[ReadyButton] PartyManager.Instance não encontrado. Verifique se a PreparationScene está ativa.");
            return;
        }

        var compactedParty = PartyManager.Instance.GetCompactedParty();
        BattlePartyData.SetParty(compactedParty);

        Debug.Log($"[ReadyButton] Formação final salva. Carregando cena: {gameplaySceneName}");
        SceneManager.LoadScene(gameplaySceneName);
    }
}
