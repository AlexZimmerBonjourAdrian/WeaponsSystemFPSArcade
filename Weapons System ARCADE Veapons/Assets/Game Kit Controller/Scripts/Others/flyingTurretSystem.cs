using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyingTurretSystem : MonoBehaviour
{

	public bool flyingTurretEnabled = true;

	public bool flyingTurretActive;

	public GameObject flyingTurretObjectPrefab;

	public GameObject flyingTurretObject;

	public Transform objectToFollow;

	public void enableOrDisableFlyingTurret ()
	{
		setFlyingTurretActiveState (!flyingTurretActive);
	}

	public void setFlyingTurretActiveState (bool state)
	{
		flyingTurretActive = state;

		if (flyingTurretObject == null) {

			flyingTurretObject = (GameObject)Instantiate (flyingTurretObjectPrefab, objectToFollow.position, objectToFollow.rotation);

			followObjectPositionUpdateSystem currentFollowObjectPositionUpdateSystem = flyingTurretObject.GetComponentInChildren<followObjectPositionUpdateSystem> ();
		
			if (currentFollowObjectPositionUpdateSystem != null) {
				currentFollowObjectPositionUpdateSystem.setObjectToFollow (objectToFollow);
			}

			AITurret currentAITurret = flyingTurretObject.GetComponentInChildren<AITurret> ();

			if (currentAITurret != null) {
				currentAITurret.setNewTurretAttacker (objectToFollow.gameObject);
			}
		}

		if (flyingTurretObject != null) {
			flyingTurretObject.SetActive (state);
		}
	}
}
