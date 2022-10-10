using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class activateReactionSystem : applyEffectOnArea
{
	[Space]
	[Header ("Custom Settings")]
	[Space]

	public bool activateCustomAction;
	public string customActionName;

	public bool activateReaction;
	public string reactionName;
	public float damageToSendOnReaction;

	public bool checkToActivateReactionSystemTemporally;

	public Transform mainReactionTransform;

	public GameObject objectBlocked;

	public override void applyEffect (GameObject objectToAffect)
	{
		playerComponentsManager currentPlayerComponentsManager = objectToAffect.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {

			if (activateCustomAction) {
				playerActionSystem currentPlayerActionSystem = currentPlayerComponentsManager.getPlayerActionSystem ();

				if (currentPlayerActionSystem != null) {
					currentPlayerActionSystem.activateCustomAction (customActionName);
				}
			}

			if (activateReaction) {
				damageHitReactionSystem currentDamageHitReactionSystem = currentPlayerComponentsManager.getDamageHitReactionSystem ();

				if (currentDamageHitReactionSystem != null) {

					if (mainReactionTransform == null) {
						mainReactionTransform = transform;
					}

					bool currentDamageHitReactionSystemActiveState = currentDamageHitReactionSystem.getHitReactionActiveState ();

					if (checkToActivateReactionSystemTemporally) {
						currentDamageHitReactionSystem.setHitReactionActiveState (true);
					}

					if (objectBlocked == null) {
						objectBlocked = gameObject;
					}

					currentDamageHitReactionSystem.checkReactionToTriggerExternally (damageToSendOnReaction, mainReactionTransform.position, objectBlocked);
				
					if (checkToActivateReactionSystemTemporally) {
						currentDamageHitReactionSystem.setHitReactionActiveState (currentDamageHitReactionSystemActiveState);
					}
				}
			}
		}
	}
}