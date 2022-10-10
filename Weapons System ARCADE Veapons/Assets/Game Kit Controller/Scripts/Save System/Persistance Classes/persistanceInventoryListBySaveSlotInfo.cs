using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistanceInventoryListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistanceInventoryListByPlayerInfo> playerInventoryList = new List<persistanceInventoryListByPlayerInfo> ();
}

[System.Serializable]
public class persistanceInventoryListByPlayerInfo
{
	public int playerID;
	public int inventorySlotAmount;
	public bool infiniteSlots;
	public List<persistanceInventoryObjectInfo> inventoryObjectList = new List<persistanceInventoryObjectInfo> ();
}

[System.Serializable]
public class persistanceInventoryObjectInfo
{
	public string Name;
	public int amount;
	public bool infiniteAmount;
	public string inventoryObjectName;

	public int categoryIndex;
	public int elementIndex;

	public bool isEquipped;

	public int quickAccessSlotIndex = -1;

	public float vendorPrice;
	public float sellPrice;

	public bool useMinLevelToBuy;
	public float minLevelToBuy;

	public bool spawnObject;

	public bool isWeapon;
	public bool isMeleeWeapon;
	public int projectilesInMagazine = -1;

	public persistanceInventoryObjectInfo (persistanceInventoryObjectInfo obj)
	{
		Name = obj.Name;
		amount = obj.amount;
		infiniteAmount = obj.infiniteAmount;
		inventoryObjectName = obj.inventoryObjectName;

		categoryIndex = obj.categoryIndex;
		elementIndex = obj.elementIndex;

		isEquipped = obj.isEquipped;

		quickAccessSlotIndex = obj.quickAccessSlotIndex;

		vendorPrice = obj.vendorPrice;
		sellPrice = obj.sellPrice;

		useMinLevelToBuy = obj.useMinLevelToBuy;
		minLevelToBuy = obj.minLevelToBuy;

		spawnObject = obj.spawnObject;

		isWeapon = obj.isWeapon;
		isMeleeWeapon = obj.isWeapon;
		projectilesInMagazine = obj.projectilesInMagazine; 
	}

	public persistanceInventoryObjectInfo ()
	{
		Name = "New Object";
	}
}