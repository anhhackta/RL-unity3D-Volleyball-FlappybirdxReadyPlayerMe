using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public float tileSize;

    private Sprite tileSprite;
    private void Start()
    {
        if (transform.childCount > 0)
        {
            tileSprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            tileSize = tileSprite.rect.width / 100;
        }
        else
        {
            Debug.LogError("No tile exists!");
        }
            
    }
    private void LateUpdate()
    {
        transform.Translate(Vector3.left * Time.deltaTime);
        if (transform.localPosition.x < -tileSize)
        {
            transform.Translate(Vector3.right * tileSize);
        }
    }
}
