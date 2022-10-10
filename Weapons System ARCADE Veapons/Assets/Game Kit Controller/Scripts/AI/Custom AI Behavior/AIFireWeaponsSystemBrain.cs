using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIFireWeaponsSystemBrain : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool weaponsSystemEnabled = true;

	public bool drawWeaponsWhenResumingAI;

	public bool drawWeaponWhenAttackModeSelected;

	public bool keepWeaponsIfNotTargetsToAttack;

	public bool canDropWeaponExternallyEnabled = true;

	[Space]
	[Header ("Attack Settings")]
	[Space]

	public bool attackEnabled;

	public float fireWeaponAttackRate = 0.17f;

	[Space]
	[Header ("Weapons Settings")]
	[Space]

	public bool changeWeaponAfterTimeEnabled;

	public bool changeWeaponRandomly;

	public Vector2 randomChangeWeaponRate;

	public float minTimeToUseNewWeaponAfterChange = 3;

	[Space]
	[Header ("Weapon List")]
	[Space]

	public int currentWeaponIndex;

	[Space]

	public List<AIWeaponInfo> AIWeaponInfoList = new List<AIWeaponInfo> ();

	[Space]
	[Space]
	[Space]
	[Header ("Roll/Dodge Settings")]
	[Space]

	public bool rollEnabled;

	public Vector2 randomRollWaitTime;

	public float minWaitTimeAfterRollActive = 1.3f;

	public List<Vector2> rollMovementDirectionList = new List<Vector2> ();

	[Space]
	[Header ("Random Walk Settings")]
	[Space]

	public bool randomWalkEnabled;

	public Vector2 randomWalkWaitTime;
	public Vector2 randomWalkDuration;
	public Vector2 randomWalkRadius;

	[Space]
	[Header ("Search Weapons On Scene Settings")]
	[Space]

	public bool searchWeaponsPickupsOnLevelIfNoWeaponsAvailable;

	public bool useEventOnNoWeaponToPickFromScene;
	public UnityEvent eventOnNoWeaponToPickFromScene;

	public bool useEventsOnNoWeaponsAvailable;
	public UnityEvent eventOnNoWeaponsAvailable;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool weaponsSystemActive;

	public bool weaponEquiped;

	public bool aimingWeapon;

	public bool waitingForAttackActive;
	float currentRandomTimeToAttack;

	public bool walkActive;
	public bool waitingWalkActive;

	public bool waitingRollActive;

	public bool canUseAttackActive;

	public bool attackStatePaused;

	public bool insideMinDistanceToAttack;

	public bool searchingWeapon;
	public bool characterHasWeapons;

	public bool behaviorStatesPaused;

	public bool shootingWeapon;

	public bool checkingToChangeWeaponActive;

	public bool onSpotted;

	public bool changeWeaponCheckActive;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnCombatActive;
	public UnityEvent eventOnCombatActive;
	public UnityEvent eventOnCombatDeactivate;

	[Space]
	[Header ("Components")]
	[Space]

	public playerWeaponsManager mainPlayerWeaponsManager;
	public dashSystem mainDashSystem;
	public findObjectivesSystem mainFindObjectivesSystem;
	public AINavMesh mainAINavmeshManager;

	float lastTimeAttack;

	int currentAttackTypeIndex;

	bool weaponInfoStored;

	int currentAttackIndex;

	int currentAttackTypeToAlternateIndex;


	float lastTimeRollActive;

	float lastTimeWaitRollActive;

	float currentRollWaitTime;


	float lastTimeWaitWalkActive;
	float currentWalkTime;
	float lastTimeWalkActive;

	float currentWalkDuration;
	float currentWalkRadius;

	bool rollCoolDownActive;

	float currentPauseAttackStateDuration;
	float lastTimeAttackPauseWithDuration;

	bool checkIfDrawWeaponActive;

	float randomWaitTime;

	float lastTimeFireWeaponAttackAtDistance;

	bool checkIfAICarryingWeapon;

	float currentPathDistanceToTarget;
	float minDistanceToAim;
	float minDistanceToDraw;
	float minDistanceToShoot;

	bool checkIfSearchWeapon;

	bool AIPaused;

	bool cancelCheckAttackState;

	GameObject currentWeaponToGet;

	float lastTimeChangeWeapon;

	float timeToChangeWeapon;

	AIWeaponInfo currentAIWeaponInfo;


	public void updateAI ()
	{
		if (weaponsSystemActive) {
			AIPaused = mainFindObjectivesSystem.isAIPaused ();

			if (!AIPaused) {
				if (!checkIfAICarryingWeapon) {
					if (mainPlayerWeaponsManager.isPlayerCarringWeapon ()) {
						weaponEquiped = true;
					}

					checkIfAICarryingWeapon = true;
				}

				if (walkActive) {
					if (Time.time > lastTimeWalkActive + currentWalkDuration || mainFindObjectivesSystem.getRemainingDistanceToTarget () < 0.5f) {
						resetRandomWalkState ();
					}
				}

				if (searchingWeapon) {
					if (currentWeaponToGet != null) {
						return;
					}

					if (currentWeaponToGet == null) {
						characterHasWeapons = mainPlayerWeaponsManager.checkIfWeaponsAvailable () ||
						mainPlayerWeaponsManager.checkIfUsableWeaponsPrefabListActive ();

						mainFindObjectivesSystem.setSearchigWeaponState (false);

						searchingWeapon = false;

						checkIfSearchWeapon = false;
					}
				}
			}
		}
	}

	public void resetRandomWalkState ()
	{
		mainFindObjectivesSystem.setRandomWalkPositionState (0);

		waitingWalkActive = false;

		walkActive = false;

		lastTimeWalkActive = Time.time;
	}

	public void resetRollState ()
	{
		waitingRollActive = false;

		lastTimeRollActive = Time.time;
	}

	public void resetStates ()
	{
		resetRandomWalkState ();

		resetRollState ();
	}

	public void checkIfResetStatsOnRandomWalk ()
	{
		if (walkActive) {
			resetStates ();
		}
	}

	public void checkRollState ()
	{
		if (rollEnabled) {

			if (walkActive) {
				return;
			}

			if (!insideMinDistanceToAttack) {
				resetRollState ();

				lastTimeRollActive = 0;

				return;
			}

			if (waitingRollActive) {
				if (Time.time > lastTimeWaitRollActive + currentRollWaitTime) {

					int randomRollMovementDirection = Random.Range (0, rollMovementDirectionList.Count - 1);

					mainDashSystem.activateDashStateWithCustomDirection (rollMovementDirectionList [randomRollMovementDirection]);

					resetRollState ();
				}
			} else {
				if (Time.time > lastTimeRollActive + randomWaitTime) {
					currentRollWaitTime = Random.Range (randomRollWaitTime.x, randomRollWaitTime.y);

					lastTimeWaitRollActive = Time.time;

					waitingRollActive = true;

					randomWaitTime = Random.Range (0.1f, 0.5f);
				}
			}
		}
	}

	public void checkWalkState ()
	{
		if (randomWalkEnabled) {

			rollCoolDownActive = Time.time < lastTimeRollActive + 0.7f;

			if (rollCoolDownActive) {
				return;
			}

			if (waitingWalkActive) {
				if (!walkActive) {

					if (Time.time > lastTimeWaitWalkActive + currentWalkTime) {
						mainFindObjectivesSystem.setRandomWalkPositionState (currentWalkRadius);

						lastTimeWalkActive = Time.time;

						walkActive = true;
					}
				}
			} else {
				currentWalkTime = Random.Range (randomWalkWaitTime.x, randomWalkWaitTime.y);

				lastTimeWaitWalkActive = Time.time;

				waitingWalkActive = true;

				currentWalkDuration = Random.Range (randomWalkDuration.x, randomWalkDuration.y);

				currentWalkRadius = Random.Range (randomWalkRadius.x, randomWalkRadius.y);

				walkActive = false;
			}
		}
	}

	public void updateInsideMinDistance (bool newInsideMinDistanceToAttack)
	{
		insideMinDistanceToAttack = newInsideMinDistanceToAttack;

		if (mainFindObjectivesSystem.attackAlwaysOnPlace) {
			insideMinDistanceToAttack = true;
		}

		if (insideMinDistanceToAttack) {
			if (checkIfDrawWeaponActive) {
				if (!mainPlayerWeaponsManager.isPlayerCarringWeapon ()) {
					setDrawOrHolsterWeaponState (true);
				}

				checkIfDrawWeaponActive = false;
			}
		} else {
			if (aimingWeapon) {
				setAimWeaponState (false);
			}
		}
	}

	void updateLookAtTargetIfBehaviorPaused ()
	{
		cancelCheckAttackState = false;

		if (mainFindObjectivesSystem.attackTargetDirectly) {
			mainFindObjectivesSystem.lookingAtTargetPosition = true;

			mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();
		} else {
			currentPathDistanceToTarget = mainFindObjectivesSystem.currentPathDistanceToTarget;
			minDistanceToAim = mainFindObjectivesSystem.minDistanceToAim;
			minDistanceToDraw = mainFindObjectivesSystem.minDistanceToDraw;
			minDistanceToShoot = mainFindObjectivesSystem.minDistanceToShoot; 

			bool useHalfMinDistance = mainAINavmeshManager.useHalfMinDistance;

			if (useHalfMinDistance) {

				mainFindObjectivesSystem.lookingAtTargetPosition = false;

				cancelCheckAttackState = true;
			} else {

				if (currentPathDistanceToTarget <= minDistanceToAim) {
					mainFindObjectivesSystem.lookingAtTargetPosition = true;

					mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();
				} else {
					if (currentPathDistanceToTarget >= minDistanceToAim + 1.5f) {
						mainFindObjectivesSystem.lookingAtTargetPosition = false;

						cancelCheckAttackState = true;
					} else {
						if (mainFindObjectivesSystem.lookingAtTargetPosition) {
							mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();
						}
					}
				}
			}
		}
	}

	public void updateMainFireWeaponsBehavior ()
	{
		if (!weaponsSystemActive) {
			return;
		}

		if (AIPaused) {
			return;
		}

		if (behaviorStatesPaused) {
			updateLookAtTargetIfBehaviorPaused ();

			return;
		}

		checkWalkState ();

		if (walkActive) {
			return;
		}
			
		checkRollState ();

		if (rollEnabled) {
			if (Time.time < lastTimeRollActive + minWaitTimeAfterRollActive) {
				return;
			}
		}

		if (characterHasWeapons) {
			cancelCheckAttackState = false;

			if (mainFindObjectivesSystem.attackTargetDirectly) {
				mainFindObjectivesSystem.lookingAtTargetPosition = true;

				mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();

				if (changeWeaponCheckActive) {
					if (Time.time < lastTimeChangeWeapon + minTimeToUseNewWeaponAfterChange) {
						return;
					} else {
						changeWeaponCheckActive = false;
					}
				}

				if (!weaponEquiped) {
					setDrawOrHolsterWeaponState (true);
				} else {
					if (!aimingWeapon) {
						if (mainPlayerWeaponsManager.currentWeaponWithHandsInPosition () && mainPlayerWeaponsManager.isPlayerCarringWeapon () && !mainPlayerWeaponsManager.currentWeaponIsMoving ()) {
							setAimWeaponState (true);
						}
					}

					if (aimingWeapon) {
						if (!mainPlayerWeaponsManager.currentWeaponIsMoving () &&
						    mainPlayerWeaponsManager.reloadingActionNotActive () &&
						    mainPlayerWeaponsManager.isPlayerCarringWeapon () &&
						    mainFindObjectivesSystem.checkIfMinimumAngleToAttack () &&
						    !mainPlayerWeaponsManager.isActionActiveInPlayer () &&
						    mainPlayerWeaponsManager.canPlayerMove ()) {

							shootTarget ();
						}
					}
				}
			} else {
//				if (showDebugPrint) {
//					print ("looking at target");
//				}
				if (changeWeaponCheckActive) {
					if (Time.time < lastTimeChangeWeapon + minTimeToUseNewWeaponAfterChange) {
						return;
					} else {
						changeWeaponCheckActive = false;
					}
				}

				currentPathDistanceToTarget = mainFindObjectivesSystem.currentPathDistanceToTarget;
				minDistanceToAim = mainFindObjectivesSystem.minDistanceToAim;
				minDistanceToDraw = mainFindObjectivesSystem.minDistanceToDraw;
				minDistanceToShoot = mainFindObjectivesSystem.minDistanceToShoot; 

				bool useHalfMinDistance = mainAINavmeshManager.useHalfMinDistance;

				if (useHalfMinDistance) {
					if (aimingWeapon) {
						setAimWeaponState (false);
					}

					mainFindObjectivesSystem.lookingAtTargetPosition = false;

					cancelCheckAttackState = true;
				} else {

					if (currentPathDistanceToTarget <= minDistanceToAim) {
						if (!aimingWeapon) {
							if (mainPlayerWeaponsManager.currentWeaponWithHandsInPosition () && mainPlayerWeaponsManager.isPlayerCarringWeapon () && !mainPlayerWeaponsManager.currentWeaponIsMoving ()) {
								setAimWeaponState (true);
							}
						}

						mainFindObjectivesSystem.lookingAtTargetPosition = true;

						mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();
					} else {
						if (currentPathDistanceToTarget >= minDistanceToAim + 1.5f) {
							if (aimingWeapon) {
								setAimWeaponState (false);
							}

							mainFindObjectivesSystem.lookingAtTargetPosition = false;

							cancelCheckAttackState = true;
						} else {
							if (mainFindObjectivesSystem.lookingAtTargetPosition) {
								mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();
							}
						}
					}

					if (!weaponEquiped && currentPathDistanceToTarget <= minDistanceToDraw) {
						setDrawOrHolsterWeaponState (true);
					}
				}
			}
				
			checkAttackState ();
		} else {
			checkNoFireWeaponsAvailableState ();
		}
	}

	public void checkNoFireWeaponsAvailableState ()
	{
		if (!weaponsSystemActive) {
			return;
		}

		if (!searchingWeapon && !checkIfSearchWeapon) {

			characterHasWeapons = mainPlayerWeaponsManager.checkIfWeaponsAvailable () ||
			mainPlayerWeaponsManager.checkIfUsableWeaponsPrefabListActive ();

			//seach for the closest weapon
			if (!characterHasWeapons) {

				if (useEventsOnNoWeaponsAvailable) {
					eventOnNoWeaponsAvailable.Invoke ();
				}

				if (searchWeaponsPickupsOnLevelIfNoWeaponsAvailable) {
					searchingWeapon = true;

					mainFindObjectivesSystem.setSearchigWeaponState (true);

					bool weaponFound = false;

					pickUpObject[] pickupList = FindObjectsOfType (typeof(pickUpObject)) as pickUpObject[];

					for (int i = 0; i < pickupList.Length; i++) {
						if (!weaponFound) {

							weaponPickup currentWeaponPickup = pickupList [i].gameObject.GetComponent<weaponPickup> ();

							if (currentWeaponPickup != null) {
								if (mainPlayerWeaponsManager.checkIfWeaponCanBePicked (currentWeaponPickup.weaponName)) {
									currentWeaponToGet = pickupList [i].getPickupTrigger ().gameObject;

									mainAINavmeshManager.setTarget (pickupList [i].transform);

									mainAINavmeshManager.setTargetType (false, true);

									weaponFound = true;

									mainAINavmeshManager.lookAtTaget (false);
									//print (pickupList [i].secondaryString);
								}
							}
						}
					}

					if (!weaponFound) {
						if (useEventOnNoWeaponToPickFromScene) {
							eventOnNoWeaponToPickFromScene.Invoke ();
						}
					}
				} else {
					checkIfSearchWeapon = true;
				}

				//it will need to check if the weapon can be seen by the character and if it is can be reached by the navmesh
			}

			mainFindObjectivesSystem.lookingAtTargetPosition = false;
		}
	}

	public void checkAttackState ()
	{
		if (!attackEnabled) {
			return;
		}

		if (!insideMinDistanceToAttack) {
			return;
		}

		if (currentPauseAttackStateDuration > 0) {
			if (Time.time > currentPauseAttackStateDuration + lastTimeAttackPauseWithDuration) {

				attackStatePaused = false;

				currentPauseAttackStateDuration = 0;
			} else {
				return;
			}
		}


		if (!canUseAttackActive) {
			return;
		}
			
		if (!aimingWeapon && !cancelCheckAttackState) {
			setAimWeaponState (true);
		}

//		if (showDebugPrint) {
//			print ("check to fire");
//		}

		if (mainFindObjectivesSystem.isOnSpotted ()) {
			if (!onSpotted) {
				onSpotted = true;
			}
		} else {
			if (onSpotted) {

				onSpotted = false;

				checkingToChangeWeaponActive = false;
			}
		}

		if (onSpotted) {
			if (changeWeaponAfterTimeEnabled) {
				if (!checkingToChangeWeaponActive) {
					if (!changeWeaponCheckActive) {
						lastTimeChangeWeapon = Time.time;

						checkingToChangeWeaponActive = true;

						timeToChangeWeapon = Random.Range (randomChangeWeaponRate.x, randomChangeWeaponRate.y);
					}
				} else {
					if (Time.time > lastTimeChangeWeapon + timeToChangeWeapon) {
						if (!mainPlayerWeaponsManager.currentWeaponIsMoving () &&
						    mainPlayerWeaponsManager.reloadingActionNotActive () &&
						    mainPlayerWeaponsManager.isPlayerCarringWeapon () &&
						    mainFindObjectivesSystem.checkIfMinimumAngleToAttack () &&
						    !mainPlayerWeaponsManager.isActionActiveInPlayer () &&
						    mainPlayerWeaponsManager.canPlayerMove ()) {

							string previousWeaponName = "";

							if (currentAIWeaponInfo != null) {
								previousWeaponName = currentAIWeaponInfo.Name;
							}

							setNextWeapon ();

							if (previousWeaponName != currentAIWeaponInfo.Name) {
								setShootWeaponState (false);

								aimingWeapon = false;

								mainPlayerWeaponsManager.changeCurrentWeaponByName (currentAIWeaponInfo.Name);

								changeWeaponCheckActive = true;

								lastTimeChangeWeapon = Time.time;
							}
						}
					}
				}
			}
		}

		if (changeWeaponCheckActive) {
			if (Time.time < lastTimeChangeWeapon + minTimeToUseNewWeaponAfterChange) {
				return;
			} else {
				changeWeaponCheckActive = false;
			}
		}

		if (Time.time > fireWeaponAttackRate + lastTimeFireWeaponAttackAtDistance) {
			if (weaponEquiped &&
			    aimingWeapon &&
			    currentPathDistanceToTarget <= minDistanceToShoot &&
			    !mainPlayerWeaponsManager.currentWeaponIsMoving () &&
			    mainPlayerWeaponsManager.reloadingActionNotActive () &&
			    mainPlayerWeaponsManager.isPlayerCarringWeapon () &&
			    mainFindObjectivesSystem.checkIfMinimumAngleToAttack () &&
			    !mainPlayerWeaponsManager.isActionActiveInPlayer () &&
			    mainPlayerWeaponsManager.canPlayerMove ()) {

				shootTarget ();
			
			}

			lastTimeFireWeaponAttackAtDistance = Time.time;
		}
	}

	public void updateMainFireWeaponsAttack (bool newCanUseAttackActiveState)
	{
		canUseAttackActive = newCanUseAttackActiveState;
	}

	public void setNextWeapon ()
	{
		bool newWeaponIndexFound = false;

		int newWeaponIndex = -1;

		if (changeWeaponRandomly) {
			while (!newWeaponIndexFound) {

				newWeaponIndex = Random.Range (0, AIWeaponInfoList.Count);

				if (newWeaponIndex != currentWeaponIndex) {
					if (AIWeaponInfoList [newWeaponIndex].weaponEnabled) {
						newWeaponIndexFound = true;
					}
				}
			}
		} else {
			newWeaponIndex = currentWeaponIndex;

			while (!newWeaponIndexFound) {
				newWeaponIndex++;

				if (newWeaponIndex >= AIWeaponInfoList.Count) {
					newWeaponIndex = 0;
				}

				if (AIWeaponInfoList [newWeaponIndex].weaponEnabled) {
					newWeaponIndexFound = true;
				}
			}
		}

		if (newWeaponIndex > -1) {
			setNewWeaponByName (AIWeaponInfoList [newWeaponIndex].Name);

			if (showDebugPrint) {
				print ("changing weapon to " + AIWeaponInfoList [newWeaponIndex].Name);
			}
		}

		checkingToChangeWeaponActive = false;
	}

	public void setNewWeaponByName (string weaponName)
	{
		if (!weaponsSystemEnabled) {
			return;
		}

		int newWeaponIndex = AIWeaponInfoList.FindIndex (s => s.Name.Equals (weaponName));

		if (newWeaponIndex > -1) {
			if (currentAIWeaponInfo != null) {
				currentAIWeaponInfo.isCurrentWeapon = false;
			}

			currentAIWeaponInfo = AIWeaponInfoList [newWeaponIndex];

			currentAIWeaponInfo.isCurrentWeapon = true;

			currentWeaponIndex = newWeaponIndex;
		}
	}

	public void setWeaponsSystemActiveState (bool state)
	{
		if (!weaponsSystemEnabled) {
			return;
		}

		weaponsSystemActive = state;

		checkEventsOnCombatStateChange (weaponsSystemActive);

		if (weaponsSystemActive && drawWeaponWhenAttackModeSelected && !mainPlayerWeaponsManager.isPlayerCarringWeapon ()) {
			setDrawOrHolsterWeaponState (true);
		}

		onSpotted = false;

		changeWeaponCheckActive = false;
	}

	void checkEventsOnCombatStateChange (bool state)
	{
		if (useEventsOnCombatActive) {
			if (state) {
				eventOnCombatActive.Invoke ();
			} else {
				eventOnCombatDeactivate.Invoke ();
			}
		}
	}

	public void pauseAttackDuringXTime (float newDuration)
	{
		currentPauseAttackStateDuration = newDuration;

		lastTimeAttackPauseWithDuration = Time.time;

		attackStatePaused = true;
	}

	public void resetBehaviorStates ()
	{
		resetStates ();

		waitingForAttackActive = false;

		checkIfDrawWeaponActive = true;

		if (keepWeaponsIfNotTargetsToAttack) {
			setDrawOrHolsterWeaponState (false);
		} else {
			setAimWeaponState (false);
		}

		insideMinDistanceToAttack = false;
	}

	public void setDrawOrHolsterWeaponState (bool state)
	{
		if (state) {
			mainPlayerWeaponsManager.drawCurrentWeaponWhenItIsReady (true);
		} else {
			mainPlayerWeaponsManager.drawOrKeepWeapon (false);
		}

		weaponEquiped = state;

		if (!weaponEquiped) {
			aimingWeapon = false;
		}

		if (showDebugPrint) {
			print ("setting draw weapon state " + state);
		}
	}

	public void setAimWeaponState (bool state)
	{
		if (showDebugPrint) {
			print ("setting aim active state " + state);
		}

		if (state) {
			if (!aimingWeapon) {
				mainPlayerWeaponsManager.aimCurrentWeaponWhenItIsReady (true);
				lastTimeFireWeaponAttackAtDistance = Time.time;
			}
		} else {
			if (aimingWeapon) {
				mainPlayerWeaponsManager.stopAimCurrentWeaponWhenItIsReady (true);
			}
		}

		aimingWeapon = state;
	}

	public void setShootWeaponState (bool state)
	{
		mainPlayerWeaponsManager.shootWeapon (state);

		shootingWeapon = state;
	}

	public void dropWeapon ()
	{
		if (!weaponsSystemActive) {
			return;
		}

		if (AIPaused) {
			return;
		}

		if (behaviorStatesPaused) {
			return;
		}

		if (!canDropWeaponExternallyEnabled) {
			return;
		}

		if (shootingWeapon) {
			setShootWeaponState (false);
		}

		if (mainPlayerWeaponsManager.dropWeaponCheckingMinDelay ()) {
			aimingWeapon = false;

			resetAttackState ();

			updateWeaponsAvailableState ();

			if (characterHasWeapons) {
				setDrawOrHolsterWeaponState (true);
			}
		}
	}

	public void shootTarget ()
	{
		setShootWeaponState (true);
	}

	public void resetAttackState ()
	{
		weaponEquiped = false;
		aimingWeapon = false;
	}

	public void checkIfDrawWeaponsWhenResumingAI ()
	{
		if (drawWeaponsWhenResumingAI && !weaponEquiped) {
			setDrawOrHolsterWeaponState (true);
		}
	}

	public void stopAim ()
	{
		if (aimingWeapon) {
			setAimWeaponState (false);
		}
	}

	public void disableOnSpottedState ()
	{

	}

	public void updateWeaponsAvailableState ()
	{
		characterHasWeapons = mainPlayerWeaponsManager.checkIfWeaponsAvailable ();
	}

	public void setBehaviorStatesPausedState (bool state)
	{
		behaviorStatesPaused = state;

		if (behaviorStatesPaused) {
			resetAttackState ();
		}
	}

	public bool isWeaponEquiped ()
	{
		return weaponEquiped;
	}

	public void inputWeaponMeleeAttack ()
	{
		if (weaponsSystemActive) {
			setShootWeaponState (false);

			mainPlayerWeaponsManager.inputWeaponMeleeAttack ();
		}
	}

	[System.Serializable]
	public class AIWeaponInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;

		public bool weaponEnabled;

		public bool isCurrentWeapon;
	}
}