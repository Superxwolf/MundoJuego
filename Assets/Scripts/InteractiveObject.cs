using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    Color baseColor;
    SpriteRenderer rend;
    public GameObject InfoPanel;

    public int ID = -1;
    public bool placed = false;

    public static Color darkColor = new Color(0.23f, 0.23f, 0.23f, 0.4f);
    public static Color selectedDarkColor = new Color(0.35f, 0.35f, 0.35f, 0.65f);

    public GameObject Highlight;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        baseColor = rend.color;
        if (!placed) rend.color = darkColor;
    }

    private void OnMouseEnter()
    {
        if (placed)
        {
            Debug.Log("Mouse entered: " + gameObject.name);
            //rend.color = new Color(1, 0.33f, 0.22f);
            Highlight.SetActive(true);
            InfoPanel.SetActive(true);
        }
        else
        {
            rend.color = selectedDarkColor;
        }
    }

    private void OnMouseExit()
    {
        if (placed)
        {
            Debug.Log("Mouse Exit: " + gameObject.name);
            rend.color = baseColor;
            InfoPanel.SetActive(false);
            Highlight.SetActive(false);
        }
        else
        {
            rend.color = darkColor;
        }
    }

    private void OnMouseUp()
    {
        if (placed) return;
        if(InventoryManager.GetSelectedID() == ID)
        {
            placed = true;
            rend.color = baseColor;

            InventoryManager.DeleteID(ID);

            rend.color = Vector4.one;
            OnMouseEnter();

            SoundManager.Play("BubblePop");
        }
        else
        {
            Debug.Log("Trying to place wrong object");
        }
    }

    public void PlaceObject(int ID)
    {

    }
}
