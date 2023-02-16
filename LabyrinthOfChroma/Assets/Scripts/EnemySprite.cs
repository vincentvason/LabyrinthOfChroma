using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;

    [SerializeField] private List<Color> colorList;

    // Start is called before the first frame update
    public void ChangeSpriteColor(int thisColor)
    {
        sprite.color = colorList[thisColor];
    }
}
