using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class drainStatSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool drainEnabled;
	public LayerMask layerToCheck;

	public bool activateDrainCoroutineDirectly;

	public float drainStatRate = 0.3f;
	public float maxDrainDistance = 20;

	public bool moveObjectCloserToPlayer;
	public float moveObjectSpeed = 5;
	public float lookObjectSpeed = 5;
	public float minDistanceToMove = 3;

	public bool useSphereCastAll;
	public float sphereCastAllRadius;

	public float startDrainDelay;

	public bool stopDrainDirectly = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool drainActive;
	public bool drainActionPaused;
	public GameObject currentObjectDetected;
	public bool moveObjectActive;

	[Space]
	[Header ("Stats Settings")]
	[Space]

	public List<statsInfo> statsInfoList = new List<statsInfo> ();

	[Space]
	[Header ("Remote Event Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameListOnStart = new List<string> ();
	public List<string> remoteEventNameListOnEnd = new List<string> ();

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnStartDrain;
	public UnityEvent envetOnEndDrain;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform mainCameraTransform;
	public Transform playerTransform;

	public playerStatsSystem mainPlayerStatsSystem;

	playerStatsSystem statsSystemDetected;

	List<playerStatsSystem> playerStatsSystemList = new List<playerStatsSystem> ();

	RaycastHit hit;

	Coroutine drainCoroutine;

	remoteEventSystem currentRemoteEventSystem;

	RaycastHit[] hitsList;

	List<GameObject> detectedGameObjectList = new List<GameObject> ();

	public void checkTargetToDrain ()
	{
		if (!drainEnabled) {
			return;
		}

		if (drainActionPaused) {
			return;
		}

		if (drainActive) {
			if (stopDrainDirectly) {
				stopDrain ();
			} else {
				envetOnEndDrain.Invoke ();
			}

			return;
		}

		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, maxDrainDistance, layerToCheck)) {
			bool startDrain = false;

			if (useSphereCastAll) {
				Ray newRay = new Ray (hit.point, mainCameraTransform.forward);

				hitsList = Physics.SphereCastAll (newRay, sphereCastAllRadius, maxDrainDistance, layerToCheck);

				detectedGameObjectList.Clear ();

				playerStatsSystemList.Clear ();

				List<GameObject> temporalGameObjectList = new List<GameObject> ();
			
				for (int i = 0; i < hitsList.Length; i++) {
					temporalGameObjectList.Add (hitsList [i].collider.gameObject);
				}

				for (int i = 0; i < temporalGameObjectList.Count; i++) {
					playerComponentsManager currentPlayerComponentsManager = temporalGameObjectList [i].GetComponent<playerComponentsManager> ();

					if (currentPlayerComponentsManager) {
						statsSystemDetected = currentPlayerComponentsManager.getPlayerStatsSystem ();

						if (statsSystemDetected) {
							playerStatsSystemList.Add (statsSystemDetected);

							detectedGameObjectList.Add (temporalGameObjectList [i]);

							startDrain = true;
						}
					}
				}
			} else {
				playerComponentsManager currentPlayerComponentsManager = hit.collider.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager) {
					statsSystemDetected = currentPlayerComponentsManager.getPlayerStatsSystem ();

					if (statsSystemDetected) {
						currentObjectDetected = hit.collider.gameObject;

						startDrain = true;
					}
				}
			}

			if (startDrain) {
				print ("start drain");

				eventOnStartDrain.Invoke ();

				drainActive = true;

				if (activateDrainCoroutineDirectly) {
					startDrainCoroutine ();
				}
			}
		}
	}

	public void startDrainCoroutine ()
	{
		stopActivateDrainStatCoroutine ();

		drainCoroutine = StartCoroutine (activateDrainStatCoroutine ());
	}

	public void stopActivateDrainStatCoroutine ()
	{
		if (drainCoroutine != null) {
			StopCoroutine (drainCoroutine);
		}
	}

	IEnumerator activateDrainStatCoroutine ()
	{
		yield return new WaitForSeconds (startDrainDelay);

		bool statTotallyDrained = false;

		int statsDrainedAmount = 0;

		while (!statTotallyDrained) {
			yield return new WaitForSeconds (drainStatRate);

			for (int i = 0; i < statsInfoList.Count; i++) {
				
				mainPlayerStatsSystem.addOrRemovePlayerStatAmount (statsInfoList [i].statToIncrease, statsInfoList [i].increaseStatAmount);

				statsDrainedAmount = 0;

				for (int j = 0; j < statsInfoList [i].statToDrainList.Count; j++) {
					if (useSphereCastAll) {

						for (int k = 0; k < playerStatsSystemList.Count; k++) {
							playerStatsSystemList [k].addOrRemovePlayerStatAmount (statsInfoList [i].statToDrainList [j], statsInfoList [i].drainStatAmount);

							if (playerStatsSystemList [k].getStatValue (statsInfoList [i].statToDrainList [j]) <= 0) {
								statsDrainedAmount++;
							}
						}
					} else {
						statsSystemDetected.addOrRemovePlayerStatAmount (statsInfoList [i].statToDrainList [j], statsInfoList [i].drainStatAmount);
				
						if (statsSystemDetected.getStatValue (statsInfoList [i].statToDrainList [j]) <= 0) {
							statsDrainedAmount++;
						}
					}
				}

				if (useSphereCastAll) {
					if (statsDrainedAmount >= statsInfoList [i].statToDrainList.Count * playerStatsSystemList.Count) {
						statTotallyDrained = true;
					}
				} else {
					if (statsDrainedAmount >= statsInfoList [i].statToDrainList.Count) {
						statTotallyDrained = true;
					}
				}
			}
				
			yield return null;
		}

		stopDrain ();

		if (!stopDrainDirectly) {
			envetOnEndDrain.Invoke ();
		}
	}

	public void stopDrain ()
	{
		if (!drainEnabled) {
			return;
		}

		if (drainActive) {
			print ("stop drain");

			stopActivateDrainStatCoroutine ();

			if (stopDrainDirectly) {
				envetOnEndDrain.Invoke ();
			}

			drainActive = false;

			checkRemoteEventOnEnd ();

			currentObjectDetected = null;

			moveObjectActive = false;

			stopMoveObjectCoroutine ();

			drainActionPaused = false;
		}
	}

	public void checkRemoteEventOnStart ()
	{
		if (useRemoteEventOnObjectsFound) {
			if (useSphereCastAll) {
				for (int i = 0; i < detectedGameObjectList.Count; i++) {
					currentRemoteEventSystem = detectedGameObjectList [i].GetComponent<remoteEventSystem> ();

					if (currentRemoteEventSystem != null) {
						for (int j = 0; j < remoteEventNameListOnStart.Count; j++) {

							currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnStart [j]);
						}
					}
				}
			} else {
				if (currentObjectDetected) {
					currentRemoteEventSystem = currentObjectDetected.GetComponent<remoteEventSystem> ();

					if (currentRemoteEventSystem != null) {
						for (int i = 0; i < remoteEventNameListOnStart.Count; i++) {

							currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnStart [i]);
						}
					}
				}
			}
		}
	}

	public void checkRemoteEventOnEnd ()
	{
		if (useRemoteEventOnObjectsFound) {
			if (useSphereCastAll) {
				for (int i = 0; i < detectedGameObjectList.Count; i++) {
					currentRemoteEventSystem = detectedGameObjectList [i].GetComponent<remoteEventSystem> ();

					if (currentRemoteEventSystem != null) {
						for (int j = 0; j < remoteEventNameListOnEnd.Count; j++) {

							currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnEnd [j]);
						}
					}
				}
			} else {
				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameListOnEnd.Count; i++) {

						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnEnd [i]);
					}

					currentRemoteEventSystem = null;
				}
			}
		}
	}

	Coroutine moveCoroutine;

	public void startMoveObjectCoroutine ()
	{
		stopMoveObjectCoroutine ();

		moveCoroutine = StartCoroutine (moveObjectCoroutine ());
	}

	public void stopMoveObjectCoroutine ()
	{
		if (moveCoroutine != null) {
			StopCoroutine (moveCoroutine);
		}
	}

	IEnumerator moveObjectCoroutine ()
	{
		while (drainActive) {
			if (moveObjectCloserToPlayer && moveObjectActive) {
				if (useSphereCastAll) {
					for (int i = 0; i < detectedGameObjectList.Count; i++) {
						moveObject (detectedGameObjectList [i].transform);
					}
				} else {
					moveObject (currentObjectDetected.transform);
				}
			}

			yield return null;
		}
	}

	void moveObject (Transform objectToMove)
	{
		float currentDistance = GKC_Utils.distance (playerTransform.position, objectToMove.position);

		if (currentDistance > minDistanceToMove) {
			objectToMove.position = Vector3.Lerp (objectToMove.position, playerTransform.position, Time.deltaTime * moveObjectSpeed);
		}

		Vector3 lookDirection = playerTransform.position - objectToMove.position;
		lookDirection = lookDirection / lookDirection.magnitude;

		Quaternion targetRotation = Quaternion.LookRotation (lookDirection);

		objectToMove.rotation = Quaternion.Lerp (objectToMove.rotation, targetRotation, Time.deltaTime * lookObjectSpeed);
	}

	public void setDrainActionPausedState (bool state)
	{
		drainActionPaused = state;
	}

	public void setMoveObjectActiveState (bool state)
	{
		moveObjectActive = state;

		if (moveObjectActive) {
			startMoveObjectCoroutine ();
		}
	}

	[System.Serializable]
	public class statsInfo
	{
		public string statToIncrease;

		public float increaseStatAmount = 5;
		public float drainStatAmount = 5;

		public List<string> statToDrainList = new List<string> ();
	}
}
