using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public int ID = -1;
    public Sprite sprite;

    private void OnMouseUp()
    {
        InventoryManager.AddItem(sprite, ID);
        Destroy(gameObject);
    }
}
