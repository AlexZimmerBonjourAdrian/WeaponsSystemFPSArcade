using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryBankManager : MonoBehaviour
{
	public List<inventoryListElement> inventoryListManagerList = new List<inventoryListElement> ();
	public List<inventoryInfo> bankInventoryList = new List<inventoryInfo> ();

	public string[] inventoryManagerListString;
	public List<inventoryManagerStringInfo> inventoryManagerStringInfoList = new List<inventoryManagerStringInfo> ();

	public bool loadCurrentBankInventoryFromSaveFile;
	public bool saveCurrentBankInventoryToSaveFile;

	public inventoryListManager mainInventoryManager;

	public gameManager gameSystemManager;

	public void setNewInventoryListManagerList (List<inventoryListElement> newList)
	{
		inventoryListManagerList = newList;
	}

	void Start ()
	{
		setInventoryFromInventoryListManager ();
	}

	public void setInventoryFromInventoryListManager ()
	{
		int inventoryListManagerListCount = inventoryListManagerList.Count;

		List<inventoryCategoryInfo> inventoryCategoryInfoList = mainInventoryManager.inventoryCategoryInfoList;

		for (int i = 0; i < inventoryListManagerListCount; i++) {
			inventoryListElement currentElement = inventoryListManagerList [i];

			inventoryInfo currentInventoryInfo = inventoryCategoryInfoList [currentElement.categoryIndex].inventoryList [currentElement.elementIndex];

			if (currentInventoryInfo != null) {
				inventoryInfo newInventoryInfo = new inventoryInfo (currentInventoryInfo);
				newInventoryInfo.Name = currentInventoryInfo.Name;
				newInventoryInfo.amount = currentElement.amount;

				bankInventoryList.Add (newInventoryInfo);
			}
		}
	}

	public List<inventoryInfo> getBankInventoryList ()
	{
		return bankInventoryList;
	}


	//EDITOR FUNCTIONS
	public void getInventoryListManagerList ()
	{
		inventoryManagerListString = new string[mainInventoryManager.inventoryCategoryInfoList.Count];

		for (int i = 0; i < inventoryManagerListString.Length; i++) {
			inventoryManagerListString [i] = mainInventoryManager.inventoryCategoryInfoList [i].Name;
		}

		inventoryManagerStringInfoList.Clear ();

		for (int i = 0; i < mainInventoryManager.inventoryCategoryInfoList.Count; i++) {

			inventoryManagerStringInfo newInventoryManagerStringInfoo = new inventoryManagerStringInfo ();
			newInventoryManagerStringInfoo.Name = mainInventoryManager.inventoryCategoryInfoList [i].Name;

			newInventoryManagerStringInfoo.inventoryManagerListString = new string[mainInventoryManager.inventoryCategoryInfoList [i].inventoryList.Count];

			for (int j = 0; j < mainInventoryManager.inventoryCategoryInfoList [i].inventoryList.Count; j++) {
				string newName = mainInventoryManager.inventoryCategoryInfoList [i].inventoryList [j].Name;
				newInventoryManagerStringInfoo.inventoryManagerListString [j] = newName;
			}

			inventoryManagerStringInfoList.Add (newInventoryManagerStringInfoo);
		}

		updateComponent ();
	}

	public void setInventoryObjectListNames ()
	{
		for (int i = 0; i < inventoryListManagerList.Count; i++) {
			inventoryListManagerList [i].Name = inventoryListManagerList [i].inventoryObjectName;
		}

		updateComponent ();
	}

	public void addNewInventoryObjectToInventoryListManagerList ()
	{
		inventoryListElement newInventoryListElement = new inventoryListElement ();
		newInventoryListElement.Name = "New Object";
		inventoryListManagerList.Add (newInventoryListElement);

		updateComponent ();
	}

	public void saveCurrentInventoryListToFile ()
	{
		if (gameSystemManager == null) {
			gameSystemManager = FindObjectOfType<gameManager> ();
		}

		if (gameSystemManager != null) {
			gameSystemManager.saveGameInfoFromEditor ("Inventory Bank");

			print ("Inventory Bank List saved");

			updateComponent ();
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class inventoryManagerStringInfo
	{
		public string Name;
		public string[] inventoryManagerListString;
	}
}