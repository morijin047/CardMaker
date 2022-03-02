using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScriptable : ScriptableObject
{
    public CardType cardType;
    public string cardName;
    public SpellType spellType;
    public MonsterType monsterType;
    public TrapType trapType;
    public Attribut cardAttribut;
    public string monsterSpecie;
    public string cardText;
    public int atk;
    public int def;
    public int levelRank;
    public Texture2D cardIMG;
    public Texture2D artwork;

    public void NewCard()
    {
        atk = 2500;
        Debug.Log("Test");
    }
}
