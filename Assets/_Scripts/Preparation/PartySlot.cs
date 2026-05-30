using UnityEngine;

public class PartySlot : MonoBehaviour
{
    public int index;
    public PartyCharacterView currentCharacter;

    public bool IsEmpty()
    {
        return currentCharacter == null;
    }

    public void SetCharacter(PartyCharacterView character)
    {
        currentCharacter = character;

        character.transform.position = transform.position;

        //character.SetSlot(this);
    }

    public void Clear()
    {
        currentCharacter = null;
    }
}

