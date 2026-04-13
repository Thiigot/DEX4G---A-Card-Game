using UnityEngine;
using CardData;
using UnityEngine.UI;
using TMPro;
using System;
public class CardDisplay : MonoBehaviour
{
    public Card cardData;
    public Image cardImage;
    public TMP_Text nameText;
    public TMP_Text manaText;
    public TMP_Text cardText;
    [SerializeField] private Color32[] colorType =
{
    new Color32(212, 184, 40, 255),   // jackpot
    new Color32(116, 40, 40, 255),    // outlaw
    new Color32(40, 40, 116, 255),    // captain
    new Color32(40, 116, 40, 255),    // wanderer
    new Color32(99, 99, 99, 255),     // mechanic
    new Color32(203, 203, 203, 255),  // jumper
};
    public Image[] typeImage;
    public Color manaColor;
    public Color noManaColor;

    public void UpdateCardDisplay()
    {
        if (cardData == null)
        {
            Debug.LogError("cardData NULL em " + gameObject.name);
            return;
        }
        //CardColor
        Color c = colorType[(int)cardData.cardClass];
        c.a = 1f;
        cardImage.color = c;
        //CardTexts
        nameText.text = cardData.cardName;
        manaText.text = cardData.cardMana.ToString();
        cardText.text = cardData.textFront;
        //CardType
        for (int i = 0; i < typeImage.Length; i++)
        {
            typeImage[i].gameObject.SetActive(i == (int)cardData.cardType);
        }
    }

    public void UpdateManaVisual(bool canPlay)
    {
        manaText.color = canPlay ? manaColor : noManaColor;
    }
}
