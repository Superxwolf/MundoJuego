using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public Text TextField;
    public string[] QuestTexts;

    int currentQuestCounter = 0;

	// Use this for initialization
	IEnumerator Start()
    {
        TextField.text = QuestTexts[0];
        yield return new WaitUntil(() => InventoryManager.instance.inventory.Count > 0);
        TextField.text = QuestTexts[++currentQuestCounter];
        yield return new WaitUntil(() => InventoryManager.instance.selectedID != 0);
        TextField.text = QuestTexts[++currentQuestCounter];
        yield return new WaitUntil(() => InventoryManager.instance.selectedID == 0);
        TextField.text = QuestTexts[++currentQuestCounter];
    }
}
