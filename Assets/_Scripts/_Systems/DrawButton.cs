using UnityEngine;

public class DrawButton : MonoBehaviour
{
    public DeckManager deckManager;

    public void Draw()
    {
        deckManager.DrawCard();
    }
}