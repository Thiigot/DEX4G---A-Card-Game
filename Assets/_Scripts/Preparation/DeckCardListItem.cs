using System;
using CardData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Button button;
    [SerializeField] private Image background;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.45f, 0.8f, 1f, 1f);

    private Card card;
    private Action<Card> onClick;

    public void Setup(Card cardData, bool selected, Action<Card> clickCallback)
    {
        card = cardData;
        onClick = clickCallback;

        if (label != null)
            label.text = card != null ? $"{card.cardName} ({card.cardMana})" : "Empty";

        if (background != null)
            background.color = selected ? selectedColor : normalColor;

        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(card));
        }
    }
}
