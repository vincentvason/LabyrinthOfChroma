using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    [SerializeField] private GameObject hatSprite;
    [SerializeField] private GameObject sprite;

    [SerializeField] private List<Color> hatColorList;
    [SerializeField] private List<Color> colorList;

    // Start is called before the first frame update
    public void ChangeSpriteColor(int thisColor)
    {
        hatSprite.SetActive(true);
        hatSprite.GetComponent<SpriteRenderer>().color = hatColorList[thisColor];
        sprite.GetComponent<SpriteRenderer>().color = colorList[thisColor];
    }
}
