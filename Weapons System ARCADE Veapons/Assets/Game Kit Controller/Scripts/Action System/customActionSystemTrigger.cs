using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customActionSystemTrigger : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool activateActionOnEnter;
	public bool activateActionOnExit;

	public string actionToActivateName;

	public bool stopActionActive;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool setCustomActionSystemTransform;
	public Transform customActionSystemTransform;

	public void setPlayerOnEnter (GameObject newPlayer)
	{
		if (activateActionOnEnter) {
			activateCustomAction (newPlayer);
		}
	}

	public void setPlayerOnExit (GameObject newPlayer)
	{
		if (activateActionOnExit) {
			activateCustomAction (newPlayer);
		}
	}

	public void activateCustomAction (GameObject newPlayer)
	{
		playerComponentsManager currentPlayerComponentsManager = newPlayer.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			playerActionSystem currentPlayerActionSystem = currentPlayerComponentsManager.getPlayerActionSystem ();

			if (currentPlayerActionSystem != null) {
				if (stopActionActive) {
					currentPlayerActionSystem.stopCustomAction (actionToActivateName);
				} else {
					if (setCustomActionSystemTransform) {
						currentPlayerActionSystem.setCustomActionTransform (actionToActivateName, customActionSystemTransform);
					}

					currentPlayerActionSystem.activateCustomAction (actionToActivateName);
				}
			}
		}
	}
}
