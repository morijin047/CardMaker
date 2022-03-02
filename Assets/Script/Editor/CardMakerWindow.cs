using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;
using UnityEditor.UIElements;
using UnityEngine.UI;
using Object = System.Object;

public class CardMakerWindow : EditorWindow
{
    private Rect cardArea;
    private Rect artArea;
    private static CardScriptable cs;

    private static int indexCardType = 0;
    private static int indexSubType = 1;
    private static int indexCardAttribut = 0;
    //private static Texture2D artwork;
    private static bool newCard = true;

    [MenuItem("Custom Tool / Card Maker ")]
    public static void CreateWin()
    {
        GetWindow<CardMakerWindow>("Card Maker").Show();
    }

    private void OnEnable()
    {
        //artwork = Texture2D.whiteTexture;
        newCard = true;
    }

    public void OnGUI()
    {
        if (cs == null)
        {
            cs = CreateInstance<CardScriptable>();
        }

        cs.cardName = EditorGUI.TextField(GetRect(0f, 0.1f, 8f, 0.5f, position), "Name", cs.cardName);
        string[] cardTypes = System.Enum.GetValues(typeof(CardType)).OfType<object>().Select(o => o.ToString())
            .ToArray();
        indexCardType = EditorGUI.Popup(GetRect(0, 0.7f, 8f, 0.5f, position), "Card Type", indexCardType,
            cardTypes);
        switch (indexCardType)
        {
            case 0:
                EditorMonsterLayout();
                cs.cardType = CardType.Monster;
                break;
            case 1:
                EditorSpellLayout();
                cs.cardType = CardType.Spell;
                break;
            case 2:
                EditorTrap();
                cs.cardType = CardType.Trap;
                break;
        }

        cs.cardText = EditorGUI.TextField(GetRect(0f, 5.25f, 8f, 2.5f, position), "Card Text", cs.cardText);
        cs.artwork = (Texture2D)
            EditorGUI.ObjectField(GetRect(0f, 8f, 5f, 1f, position), "ArtWork", cs.artwork, typeof(Texture2D), true);
        CardPreview();
        //cs.cardArtWork = ScreenCapture.CaptureScreenshotAsTexture();
        if (newCard)
        {
            if (GUI.Button(GetRect(8.5f, 7f, 3f, 0.5f, position), "Save Card"))
            {
                SaveScriptable();
            }
        }
        LoadScriptables();
    }

    public void SaveScriptable()
    {
        // cs.cardArtWork.ReadPixels(GetRect(8.5f, 0.5f, 3f, 6f, position), 0,0);
        // cs.cardArtWork.Apply();
        AssetDatabase.CreateAsset(cs, Path.Combine("Assets", "Data", cs.cardName + ".asset"));
        AssetDatabase.SaveAssets();
    }

    public void LoadScriptables()
    {
        //string path = EditorUtility.OpenFilePanel("Illusion Of Chaos", "Data", "asset");
        DirectoryInfo info = new DirectoryInfo("Assets/Data");
        FileInfo[] fileInfo = info.GetFiles();
        CardScriptable temp = null;
        float start = 9f;
        for (int i = 0; i < fileInfo.Length; i++ )
        {
            if (!fileInfo[i].Name.Contains(".meta"))
            {
                start += i / 3f;
                if (GUI.Button(GetRect(0f, start, 4f, 0.3f, position),
                    Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileInfo[i].Name))))
                {
                    cs = new CardScriptable();
                    temp = (CardScriptable) AssetDatabase.LoadAssetAtPath("Assets/Data/" + fileInfo[i].Name,
                        typeof(CardScriptable));
                    newCard = false;
                }
            }
            else
            {
                start -= i / 3f;
            }

            if (i == fileInfo.Length - 1)
            {
                if (GUI.Button(GetRect(0f, 11.7f, 4f, 0.3f, position), "New Card"))
                {
                    temp = null;
                    cs = new CardScriptable();
                    newCard = true;
                }
            }
        }

        if (temp != null)
        {
            cs = temp;
            indexCardType = (int)cs.cardType;
            if (cs.cardType == CardType.Monster)
            {
                indexSubType = (int)cs.monsterType;
                indexCardAttribut = (int)cs.cardAttribut; 
            }
            else 
            {
                if (cs.cardType == CardType.Spell)
                {
                    indexSubType = (int)cs.spellType;
                }
                else
                {
                    indexSubType = (int) cs.trapType;
                }
            }
            
            Repaint();
        }
    }

    public static Texture2D AddWatermark(Texture2D background, Texture2D watermark, int startX, int startY)
    {
        Texture2D newTex = new Texture2D(background.width, background.height, background.format, false);

        startY = background.height - startY - watermark.height;

        for (int x = 0; x < background.width; x++)
        {
            for (int y = 0; y < background.height; y++)
            {
                // y = background.height - y;
                if ((x >= startX && x <= (watermark.width + startX)) && (y >= startY && y <= startY + watermark.height))
                {
                    Color bgColor = background.GetPixel(x, y);
                    Color wmColor = watermark.GetPixel(x - startX, y - startY);
                    Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                    newTex.SetPixel(x, y, final_color);
                }
                else
                {
                    newTex.SetPixel(x, y, background.GetPixel(x, y));
                }
            }
        }

        newTex.Apply();
        return newTex;
    }

    public void DrawOnCard(Rect positionTexture)
    {
        if (indexCardType == 0)
        {
            //Attribut
            Texture2D imgAttribut = EditorGUIUtility.whiteTexture;
            switch (indexCardAttribut)
            {
                case 0:
                    imgAttribut =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/Dark.png", typeof(Texture2D));
                    break;
                case 1:
                    imgAttribut =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/Light.png", typeof(Texture2D));
                    break;
                case 2:
                    imgAttribut =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/Fire.png", typeof(Texture2D));
                    break;
                case 3:
                    imgAttribut =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/Water.png", typeof(Texture2D));
                    break;
                case 4:
                    imgAttribut =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/Wind.png", typeof(Texture2D));
                    break;
                case 5:
                    imgAttribut =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/Earth.png", typeof(Texture2D));
                    break;
            }

            // cs.cardArtWork = AddWatermark(cs.cardArtWork, img, 345,
            //     30);
            Rect positionAttribut = GetRect(9.8f, 0.5f, 1.2f, 1, positionTexture);
            EditorGUI.DrawPreviewTexture(new Rect(positionTexture.x + positionAttribut.x,
                positionTexture.y + positionAttribut.y, positionAttribut.width, positionAttribut.height), imgAttribut);
            GUI.color = Color.black;

            //Type
            Rect positionType = GetRect(1f, 7.2f, 10, 4f, positionTexture);
            GUIStyle typeStyle = new GUIStyle(GUI.skin.label);
            typeStyle.fontStyle = FontStyle.Bold;
            string[] monsterTypes = System.Enum.GetValues(typeof(MonsterType)).OfType<object>()
                .Select(o => o.ToString())
                .ToArray();
            string typing = "[";
            if (indexSubType == 0)
            {
                typing += cs.monsterSpecie;
            }
            else if (indexSubType != 1)
            {
                typing += cs.monsterSpecie + "/" + monsterTypes[indexSubType] + "/Effect";
            }
            else
            {
                typing += cs.monsterSpecie + "/" + monsterTypes[indexSubType];
            }

            typing += "]";
            EditorGUI.LabelField(new Rect(positionTexture.x + positionType.x, positionTexture.y + positionType.y,
                positionType.width, positionType.height), typing, typeStyle);

            //atk 
            Rect positionAtk = GetRect(7.2f, 10.58f, 2, 1f, positionTexture);
            GUIStyle atkStyle = new GUIStyle(GUI.skin.label);
            atkStyle.fontStyle = FontStyle.Bold;
            EditorGUI.LabelField(new Rect(positionTexture.x + positionAtk.x, positionTexture.y + positionAtk.y,
                positionAtk.width, positionAtk.height), cs.atk.ToString(), atkStyle);
            //def 
            Rect positionDef = GetRect(9.65f, 10.58f, 2, 1f, positionTexture);
            GUIStyle defStyle = new GUIStyle(GUI.skin.label);
            defStyle.fontStyle = FontStyle.Bold;
            EditorGUI.LabelField(new Rect(positionTexture.x + positionDef.x, positionTexture.y + positionDef.y,
                positionDef.width, positionDef.height), cs.def.ToString(), defStyle);

            //level/rank
            Texture2D imgLevel;
            Rect positionLevel1;
            float spaceLevel = 0.8f;
            if (indexSubType == 5)
            {
                imgLevel = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/rank star.png",
                    typeof(Texture2D));
                for (int i = 0; i < cs.levelRank; i++)
                {
                    positionLevel1 = GetRect(1f + spaceLevel * i, 1.5f, 0.6f, 0.6f, positionTexture);
                    EditorGUI.DrawPreviewTexture(new Rect(positionTexture.x + positionLevel1.x,
                            positionTexture.y + positionLevel1.y, positionLevel1.width, positionLevel1.height),
                        imgLevel);
                }
            }
            else
            {
                imgLevel = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/level star.png",
                    typeof(Texture2D));
                for (int i = 0; i < cs.levelRank; i++)
                {
                    positionLevel1 = GetRect(10f - spaceLevel * i, 1.5f, 0.6f, 0.6f, positionTexture);
                    EditorGUI.DrawPreviewTexture(new Rect(positionTexture.x + positionLevel1.x,
                            positionTexture.y + positionLevel1.y, positionLevel1.width, positionLevel1.height),
                        imgLevel);
                }
            }
        }
        GUI.color = Color.black;
        //name
        Rect positionName = GetRect(1f, 0.5f, 8, 1f, positionTexture);
        GUIStyle nameStyle = new GUIStyle(GUI.skin.label);
        nameStyle.fontStyle = FontStyle.Bold;
        nameStyle.fontSize = 18;
        nameStyle.wordWrap = true;
        if (indexSubType == 5)
        {
            GUI.color = Color.yellow;
        }

        EditorGUI.LabelField(new Rect(positionTexture.x + positionName.x, positionTexture.y + positionName.y,
            positionName.width, positionName.height), cs.cardName, nameStyle);

        //card text
        Rect positionText = GetRect(1f, 8f, 10, 4f, positionTexture);
        GUIStyle textStyle = new GUIStyle(GUI.skin.label);
        textStyle.wordWrap = true;
        textStyle.fontSize = 9;
        EditorGUI.LabelField(new Rect(positionTexture.x + positionText.x, positionTexture.y + positionText.y,
            positionText.width, positionText.height), cs.cardText, textStyle);

        //card artwork
        // Texture2D imgArt = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/Illusion of Chaos.png",
        //     typeof(Texture2D));
        Rect positionArt = GetRect(1.4f, 2.2f, 9.1f, 6.4f, positionTexture);
        if (cs.artwork == null)
        {
            cs.artwork = Texture2D.whiteTexture;
        }

        EditorGUI.DrawPreviewTexture(new Rect(positionTexture.x + positionArt.x,
                positionTexture.y + positionArt.y, positionArt.width, positionArt.height),
            cs.artwork);

        GUI.color = Color.white;
    }

    public void CardPreview()
    {
        cs.cardIMG = EditorGUIUtility.whiteTexture;
        switch (indexCardType)
        {
            case 0:
                switch (indexSubType)
                {
                    case 0:
                        cs.cardIMG =
                            (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/normal monster.jpg",
                                typeof(Texture2D));
                        cs.monsterType = MonsterType.Normal;
                        break;
                    case 1:
                        cs.cardIMG =
                            (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/effect monster.jpg",
                                typeof(Texture2D));
                        cs.monsterType = MonsterType.Effect;
                        break;
                    case 2:
                        cs.cardIMG =
                            (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/fusion monster.jpg",
                                typeof(Texture2D));
                        cs.monsterType = MonsterType.Fusion;
                        break;
                    case 3:
                        cs.cardIMG =
                            (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/ritual monster.jpg",
                                typeof(Texture2D));
                        cs.monsterType = MonsterType.Ritual;
                        break;
                    case 4:
                        cs.cardIMG =
                            (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/synchro monster.jpg",
                                typeof(Texture2D));
                        cs.monsterType = MonsterType.Synchro;
                        break;
                    case 5:
                        cs.cardIMG = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/xyz monster.jpg",
                            typeof(Texture2D));
                        cs.monsterType = MonsterType.XYZ;
                        break;
                }

                break;
            case 1:
                if (indexSubType == 0)
                {
                    cs.cardIMG =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/spell card.jpg",
                            typeof(Texture2D));
                }
                else
                {
                    cs.cardIMG =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/spell card - icon.jpg",
                            typeof(Texture2D));
                }

                break;
            case 2:
                if (indexSubType == 0)
                {
                    cs.cardIMG =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/trap card.jpg",
                            typeof(Texture2D));
                }
                else
                {
                    cs.cardIMG =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Card Assets/trap card - icon.jpg",
                            typeof(Texture2D));
                }

                break;
        }

        Rect positionTexture = GetRect(8.5f, 0.5f, 3f, 6f, position);
        // if (GUI.changed)
        //DrawOnCard(positionTexture);
        EditorGUI.DrawPreviewTexture(positionTexture, cs.cardIMG);
        DrawOnCard(positionTexture);
    }

    public void EditorMonsterLayout()
    {
        string[] monsterTypes = System.Enum.GetValues(typeof(MonsterType)).OfType<object>().Select(o => o.ToString())
            .ToArray();
        indexSubType = EditorGUI.Popup(GetRect(0, 1.5f, 8f, 0.5f, position), "Monster Type", indexSubType,
            monsterTypes);
        cs.levelRank = EditorGUI.IntField(GetRect(0, 2f, 3f, 0.5f, position), "Level/Rank", cs.levelRank);
        string[] cardAttribut = System.Enum.GetValues(typeof(Attribut)).OfType<object>().Select(o => o.ToString())
            .ToArray();
        indexCardAttribut = EditorGUI.Popup(GetRect(4f, 2f, 3f, 0.5f, position), "Card Attribut", indexCardAttribut,
            cardAttribut);
        cs.atk = EditorGUI.IntField(GetRect(0f, 3f, 3.9f, 0.5f, position), "Atk", cs.atk);
        cs.def = EditorGUI.IntField(GetRect(4f, 3f, 4f, 0.5f, position), "Def", cs.def);
        cs.monsterSpecie =
            EditorGUI.TextField(GetRect(0, 4f, 8f, 0.5f, position), "Monster archetype", cs.monsterSpecie);
    }

    public void EditorSpellLayout()
    {
        string[] spellTypes = System.Enum.GetValues(typeof(SpellType)).OfType<object>().Select(o => o.ToString())
            .ToArray();
        indexSubType = EditorGUI.Popup(GetRect(0, 1.5f, 8f, 0.5f, position), "Spell Type", indexSubType,
            spellTypes);
    }

    public void EditorTrap()
    {
        string[] trapTypes = System.Enum.GetValues(typeof(TrapType)).OfType<object>().Select(o => o.ToString())
            .ToArray();
        indexSubType = EditorGUI.Popup(GetRect(0, 1.5f, 8f, 0.5f, position), "Trap Type", indexSubType,
            trapTypes);
    }

    public Rect GetRect(float x, float y, float width, float height, Rect position)
    {
        float widthCase = position.width / 12;
        float heightCase = position.height / 12;
        float actualWidth = width * widthCase;
        float actualHeight = height * heightCase;
        return new Rect(x * widthCase, y * heightCase, actualWidth, actualHeight);
    }
}