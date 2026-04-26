using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIconUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text stackText;

    public void Setup(Sprite sprite, StatusEffect effect)
    {
        icon.sprite = sprite;

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