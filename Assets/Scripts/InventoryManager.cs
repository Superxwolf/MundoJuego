using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public GameObject InventoryHolder;

    public int selectedID = -1;
    public Transform TooltipPrefab;
    public Sprite Bubble;

    public Dictionary<int, GameObject> inventory = new Dictionary<int, GameObject>();

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public static void AddItem(Texture2D tex, int ID)
    {
        Sprite sprite = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f));
        sprite.name = tex.name;
        AddItem(sprite, ID);
    }

    public static void AddItem(Sprite sprite, int ID)
    {
        GameObject img = new GameObject(sprite.name);
        img.transform.SetParent(instance.InventoryHolder.transform);
        img.AddComponent<Image>().sprite = instance.Bubble;
        img.AddComponent<CircleCollider2D>().isTrigger = true;
        
        img.transform.localScale = Vector3.one;
        //img.AddComponent<Button>().onClick.AddListener(() => SelectID(ID));

        GameObject newTooltip = Instantiate(instance.TooltipPrefab, img.transform).gameObject;
        newTooltip.GetComponent<Tooltip>().SetText(sprite.name);

        InventoryItem item = img.AddComponent<InventoryItem>();
        item.Tooltip = newTooltip;
        item.ID = ID;
        newTooltip.SetActive(false);

        instance.inventory.Add(ID, img);

        GameObject subImg = new GameObject("Item");
        subImg.transform.SetParent(img.transform);
        subImg.AddComponent<Image>().sprite = sprite;

        SoundManager.Play("Click");
    }

    public static void SelectID(int ID)
    {
        instance._SelectID(ID);
    }

    void _SelectID(int ID)
    {
        if (ID == selectedID) return;
        if (selectedID > 0)
        {
            Debug.Log("Unselecting: " + selectedID);
            inventory[selectedID].GetComponent<InventoryItem>().Unselect();
        }
        selectedID = ID;
        Debug.Log("Selected ID: " + ID);

        SoundManager.Play("Select");
    }

    public static int GetSelectedID()
    {
        return instance.selectedID;
    }

    public static void DeleteID(int ID)
    {
        instance._DeleteID(ID);
    }

    void _DeleteID(int ID)
    {
        Destroy(inventory[ID]);
        inventory.Remove(ID);
        if (selectedID == ID) selectedID = 0;
    }
}
