using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int ID = -1;
    public GameObject Tooltip;

    Image img;

    Color selectedColor = new Color(0.21f, 0.47f, 0.72f);

    void Start()
    {
        img = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData data)
    {
        //Debug.Log("Pointer Enter: " + gameObject.name);
        Tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData data)
    {
        //Debug.Log("Pointer Exit: " + gameObject.name);
        Tooltip.SetActive(false);
    }

    public void OnPointerClick(PointerEventData data)
    {
        img.color = selectedColor;
        InventoryManager.SelectID(ID);
    }

    public void Unselect()
    {
        img.color = Color.white;
    }
}
