using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class objectExperienceSystem : MonoBehaviour
{
	[Header ("Experience Settings")]
	[Space]

	public bool useExperienceRandomRange;
	public Vector2 experienceRandomRange;
	public int experienceAmount;

	public bool useTransformAsExpTextPosition = true;
	public Transform objectTransform;

	public string extraExperienceText;

	[Space]
	[Header ("Money Settings")]
	[Space]

	public bool useMoneyRandomRange;
	public Vector2 moneyRandomRange;
	public int moneyAmount;

	[Space]
	[Header ("Inventory Object Settings")]
	[Space]

	public bool giveInventoryObject;
	public string inventoryObjectName;
	public int inventoryObjectAmount;
	public Transform positionToInstantiateInventoryObjectPickup;
	public float maxRadiusToInstantiate;
	public float forceAmount;
	public float forceRadius;
	public ForceMode inventoryObjectForceMode;

	public bool giveInventoryObjectList;
	public List<inventoryElementInfo> inventoryElementInfoList = new List<inventoryElementInfo> ();

	[Space]
	[Header ("Inventory Extra Slots Settings")]
	[Space]

	public bool increaseInventorySlotsAmount;
	public int extraInventorySlotsAmount;

	[Space]
	[Header ("Inventory Extra Weight Settings")]
	[Space]

	public bool increaseInventoryWeightLimit;
	public float extraWeightLimit;
			
	[Space]
	[Header ("Skill Points Settings")]
	[Space]

	public bool getSkillPoints;
	public bool useSkillPointsRandomRange;
	public Vector2 skillPointsRandomRange;
	public int skillPointsAmount;

	[Space]
	[Header ("Skill Settings")]
	[Space]

	public bool getSkill;
	public string skillToGetName;
	public bool showMessageOnGetSkill;
	[Tooltip ("Write -SKILLNAME- in the string to replace that part of the message with the name of the skill.")]
	public string messageOnGetSkill;

	[Space]
	[Header ("Abilities Settings")]
	[Space]

	public bool getAbility;
	public string abilityToGetName;
	public bool showMessageOnGetAbility;
	[Tooltip ("Write -ABILITYNAME- in the string to replace that part of the message with the name of the skill.")]
	public string messageOnGetAbility;

	[Space]
	[Header ("Player Manual Settings")]
	[Space]

	public bool setPlayerManually;
	public bool sendAlwaysToMainPlayer;
	public bool searchPlayerByTagIfNotAssigned = true;
	public GameObject playerToConfigure;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventOnReward;
	public UnityEvent eventtOnReward;

	public void sendExperienceToPlayerManually ()
	{
		if (setPlayerManually) {
			sendExperienceToAttacker (playerToConfigure);
		}
	}

	public void sendExperienceToPlayer (GameObject playerToUse)
	{
		if (setPlayerManually) {
			playerToUse = playerToConfigure;
		}
			
		sendExperienceToAttacker (playerToUse);
	}

	public void sendExperienceToAttacker (GameObject attackerGameObject)
	{
		if (sendAlwaysToMainPlayer) {
			attackerGameObject = GKC_Utils.findMainPlayerOnScene ();
		}

		if (attackerGameObject == null) {
			if (setPlayerManually && searchPlayerByTagIfNotAssigned) {
				
				attackerGameObject = GKC_Utils.findMainPlayerOnScene ();

				if (attackerGameObject == null) {
					return;
				}
			} else {
				return;
			}
		}

		if (objectTransform == null) {
			objectTransform = transform;
		}

		if (applyDamage.isVehicle (attackerGameObject)) {
			GameObject vehicleDriver = applyDamage.getVehicleDriver (attackerGameObject);

			if (vehicleDriver != null) {
				attackerGameObject = vehicleDriver;
			}
		}

		playerComponentsManager currentPlayerComponentsManager = attackerGameObject.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			if (experienceAmount != 0 || (useExperienceRandomRange && (experienceRandomRange.x != 0 || experienceRandomRange.y != 0))) {
				playerExperienceSystem currentPlayerExperienceSystem = currentPlayerComponentsManager.getPlayerExperienceSystem ();

				if (currentPlayerExperienceSystem != null) {

					float newAmount = experienceAmount;

					if (useExperienceRandomRange) {
						newAmount = Random.Range (experienceRandomRange.x, experienceRandomRange.y);

						newAmount = Mathf.RoundToInt (newAmount);
					}

					currentPlayerExperienceSystem.getExperienceAmount ((int)newAmount, objectTransform, useTransformAsExpTextPosition, extraExperienceText);
				}
			}

			if (moneyAmount != 0) {
				currencySystem currentCurrencySystem = currentPlayerComponentsManager.getCurrencySystem ();

				if (currentCurrencySystem != null) {

					float newAmount = moneyAmount;

					if (useMoneyRandomRange && moneyRandomRange != Vector2.zero) {
						newAmount = Random.Range (moneyRandomRange.x, moneyRandomRange.y);

						newAmount = Mathf.RoundToInt (newAmount);
					}

					currentCurrencySystem.increaseTotalMoneyAmount (newAmount);
				}
			}

			if (giveInventoryObject) {
				if (positionToInstantiateInventoryObjectPickup == null) {
					positionToInstantiateInventoryObjectPickup = transform;
				}

				if (giveInventoryObjectList) {
					for (int i = 0; i < inventoryElementInfoList.Count; i++) {
						if (inventoryElementInfoList [i].inventoryObjectAmount == 0) {
							inventoryElementInfoList [i].inventoryObjectAmount = 1;
						}

						applyDamage.giveInventoryObjectToCharacter (attackerGameObject, inventoryElementInfoList [i].Name, inventoryElementInfoList [i].inventoryObjectAmount, 
							positionToInstantiateInventoryObjectPickup, forceAmount, maxRadiusToInstantiate, inventoryObjectForceMode, forceRadius);
					}
				} else {
					if (inventoryObjectAmount == 0) {
						inventoryObjectAmount = 1;
					}

					applyDamage.giveInventoryObjectToCharacter (attackerGameObject, inventoryObjectName, inventoryObjectAmount, 
						positionToInstantiateInventoryObjectPickup, forceAmount, maxRadiusToInstantiate, inventoryObjectForceMode, forceRadius);
				}
			}

			if (increaseInventorySlotsAmount) {
				applyDamage.addInventoryExtraSpace (attackerGameObject, extraInventorySlotsAmount);
			}

			if (increaseInventoryWeightLimit) {
				applyDamage.increaseInventoryBagWeight (extraWeightLimit, attackerGameObject);
			}

			if (getSkillPoints) {
				playerExperienceSystem currentPlayerExperienceSystem = currentPlayerComponentsManager.getPlayerExperienceSystem ();

				if (currentPlayerExperienceSystem != null) {
					
					float newAmount = skillPointsAmount;

					if (useSkillPointsRandomRange) {
						newAmount = Random.Range (skillPointsRandomRange.x, skillPointsRandomRange.y);

						newAmount = Mathf.RoundToInt (newAmount);
					}

					currentPlayerExperienceSystem.getSkillPoints ((int)newAmount);
				}
			}

			if (getSkill) {
				playerSkillsSystem currentPlayerSkillsSystem = currentPlayerComponentsManager.getPlayerSkillsSystem ();

				if (currentPlayerSkillsSystem != null) {
					currentPlayerSkillsSystem.getSkillByName (skillToGetName);

					if (showMessageOnGetSkill) {
						pickUpsScreenInfo currentPickUpsScreenInfo = currentPlayerComponentsManager.getPickUpsScreenInfo ();

						if (currentPickUpsScreenInfo != null) {
							messageOnGetSkill = messageOnGetSkill.Replace ("-SKILLNAME-", skillToGetName);

							currentPickUpsScreenInfo.recieveInfo (messageOnGetSkill);
						}
					}
				}
			}

			if (getAbility) {
				playerAbilitiesSystem currentPlayerAbilitiesSystem = currentPlayerComponentsManager.getPlayerAbilitiesSystem ();

				if (currentPlayerAbilitiesSystem != null) {
					currentPlayerAbilitiesSystem.enableAbilityByName (abilityToGetName);

					if (showMessageOnGetAbility) {
						pickUpsScreenInfo currentPickUpsScreenInfo = currentPlayerComponentsManager.getPickUpsScreenInfo ();

						if (currentPickUpsScreenInfo != null) {
							messageOnGetSkill = messageOnGetSkill.Replace ("-ABILITYNAME-", abilityToGetName);

							currentPickUpsScreenInfo.recieveInfo (messageOnGetAbility);
						}
					}
				}
			}

			if (useEventOnReward) {
				eventtOnReward.Invoke ();
			}
		}
	}

	[System.Serializable]
	public class inventoryElementInfo
	{
		public string Name;
		public int inventoryObjectAmount;
	}
}