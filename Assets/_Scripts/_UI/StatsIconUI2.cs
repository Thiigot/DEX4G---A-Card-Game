using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class StatsIconUI2 : MonoBehaviour
{
    public Image icon;
    public TMP_Text stackText;

    public void Setup(StatusEffect effect, StatusDatabase database)
    {
        // 🔥 pega sprite automaticamente
        icon.sprite = database.GetIcon(effect.GetTypeID());

        if (effect.ShowValue() && effect.value > 0)
        {
            stackText.text = effect.value.ToString();
            stackText.gameObject.SetActive(true);
        }
        else
        {
            stackText.gameObject.SetActive(false);
        }
    }
}
