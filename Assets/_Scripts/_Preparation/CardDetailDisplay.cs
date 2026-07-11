using CardData;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDetailDisplay : MonoBehaviour
{
    [Header("Identidade")]
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text cardManaText;
    [SerializeField] private Image[] cardTypeIcon;

    [Header("Texto da Carta")]
    [SerializeField] private TMP_Text sideLabel;
    [SerializeField] private TMP_Text textEffect;

    [Header("Arte")]
    [SerializeField] private Image cardArtImage;

    [Header("Flip")]
    [SerializeField] private Button flipButton;
    [SerializeField] private TMP_Text flipButtonLabel;
    [SerializeField] private float flipDuration = 0.35f;


    [Header("Cor por classe")]
    [SerializeField] private Image cardBackground;
    [SerializeField]
    private Color32[] classColors =
    {
        new Color32(212, 184,  40, 255), // Jackpot
        new Color32(116,  40,  40, 255), // Outlaw
        new Color32( 40,  40, 116, 255), // Captain
        new Color32( 40, 116,  40, 255), // Wanderer
        new Color32( 99,  99,  99, 255), // Mechanic
        new Color32(203, 203, 203, 255), // Jumper
    };


    private Card currentCard;
    private bool showingFront = true;
    private bool isFlipping = false;

    private RectTransform cardRect;


    // ─── Inicialização ────────────────────────────────────────────────────────
    void Awake()
    {
        cardRect = GetComponent<RectTransform>();

        if (flipButton != null)
            flipButton.onClick.AddListener(OnFlipClicked);
    }
    public void Populate(Card card)
    {
        if (card == null) return;
        currentCard = card;
        showingFront = true;

        ApplyStaticData(card);
        ApplySideData(showingFront);
        UpdateFlipButton();
    }


    // ─── Dados estáticos (não mudam no flip) ──────────────────────────────────

    void ApplyStaticData(Card card)
    {
        //Name and Mana
        if (cardNameText != null) cardNameText.text = card.cardName;
        if (cardManaText != null) cardManaText.text = card.isXCost ? "X" : card.cardMana.ToString();

        if (card.cardClass == CardClass.Mechanic || card.cardClass == CardClass.Jumper)
        {
            cardNameText.color = Color.black;
        }
        else
        {
            cardNameText.color = Color.white;
        }
        //CardType
        for (int i = 0; i < cardTypeIcon.Length; i++)
        {
            cardTypeIcon[i].gameObject.SetActive(i == (int)card.cardType);
        }

        // Cor de fundo pela classe
        if (cardBackground != null && classColors.Length > (int)card.cardClass)
        {
            Color c = classColors[(int)card.cardClass];
            c.a = 1f;
            cardBackground.color = c;
        }

        // Arte
        if (cardArtImage != null)
        {
            if (card.cardArt != null)
            {
                cardArtImage.sprite = card.cardArt;
                cardArtImage.enabled = true;
            }
            else
            {
                cardArtImage.enabled = false;
            }
        }

        if (flipButton != null)
            flipButton.gameObject.SetActive(true);
    }

    // ─── Dados do lado (mudam no flip) ────────────────────────────────────────

    void ApplySideData(bool front)
    {
        if (currentCard == null) return;

        string label = front ? "Frontline Effect" : "Backline Effect";
        string text = front ? currentCard.textInFront : currentCard.textInBack;

        if (sideLabel != null) sideLabel.text = label;
        if (textEffect != null) textEffect.text = string.IsNullOrWhiteSpace(text) ? "—" : text;
    }

    void UpdateFlipButton()
    {
        if (flipButtonLabel == null) return;
        flipButtonLabel.text = showingFront ? "Flip" : "Flip";
    }
    // ─── Flip ─────────────────────────────────────────────────────────────────
    void OnFlipClicked()
    {
        if (isFlipping) return;
        StartCoroutine(FlipRoutine());
    }

    IEnumerator FlipRoutine()
    {
        isFlipping = true;

        if (flipButton != null) flipButton.interactable = false;

        Vector3 originalScale = cardRect != null ? cardRect.localScale : Vector3.one;
        Transform target = cardRect != null ? cardRect : transform;

        float half = flipDuration / 2f;

        // Primeira metade: achata em X até 0 (carta "sumindo" de lado)
        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            float sx = Mathf.Lerp(originalScale.x, 0f, EaseInQuad(t));
            target.localScale = new Vector3(sx, originalScale.y, originalScale.z);
            yield return null;
        }

        target.localScale = new Vector3(0f, originalScale.y, originalScale.z);

        // ── Ponto de virada: troca os dados ──
        showingFront = !showingFront;
        ApplySideData(showingFront);
        UpdateFlipButton();

        // Segunda metade: reabre de X 0 → 1 (carta "aparecendo" do outro lado)
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            float sx = Mathf.Lerp(0f, originalScale.x, EaseOutQuad(t));
            target.localScale = new Vector3(sx, originalScale.y, originalScale.z);
            yield return null;
        }

        target.localScale = originalScale;

        if (flipButton != null) flipButton.interactable = true;
        isFlipping = false;
    }

    // ─── Easing ───────────────────────────────────────────────────────────────

    static float EaseInQuad(float t) => t * t;
    static float EaseOutQuad(float t) => t * (2f - t);
}