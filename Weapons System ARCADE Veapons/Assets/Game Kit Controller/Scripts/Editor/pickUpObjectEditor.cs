using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(pickUpObject))]
public class pickUpObjectEditor : Editor
{
	SerializedProperty amount;
	SerializedProperty useAmountPerUnit;
	SerializedProperty amountPerUnit;
	SerializedProperty pickUpSound;
	SerializedProperty staticPickUp;
	SerializedProperty moveToPlayerOnTrigger;
	SerializedProperty pickUpOption;
	SerializedProperty mainPickupType;
	SerializedProperty canBeExamined;
	SerializedProperty examiningObject;
	SerializedProperty ignoreExamineObjectBeforeStoreEnabled;
	SerializedProperty showPickupInfoOnTaken;
	SerializedProperty usePickupIconOnScreen;
	SerializedProperty pickupIconGeneralName;
	SerializedProperty pickupIconName;
	SerializedProperty usePickupIconOnTaken;
	SerializedProperty pickupIcon;
	SerializedProperty usableByAnything;
	SerializedProperty usableByPlayer;
	SerializedProperty usableByVehicles;
	SerializedProperty usableByCharacters;
	SerializedProperty useEventOnTaken;
	SerializedProperty eventOnTaken;
	SerializedProperty useEventOnRemainingAmount;
	SerializedProperty eventOnRemainingAmount;
	SerializedProperty sendPickupFinder;
	SerializedProperty sendPickupFinderEvent;

	SerializedProperty inventoryObjectManager;

	SerializedProperty deviceStringActionManager;

	SerializedProperty mainSphereCollider;
	SerializedProperty mainCollider;

	SerializedProperty mainRigidbody;

	SerializedProperty mainPickupManagerName;

	pickUpObject manager;

	void OnEnable ()
	{
		amount = serializedObject.FindProperty ("amount");
		useAmountPerUnit = serializedObject.FindProperty ("useAmountPerUnit");
		amountPerUnit = serializedObject.FindProperty ("amountPerUnit");
		pickUpSound = serializedObject.FindProperty ("pickUpSound");
		staticPickUp = serializedObject.FindProperty ("staticPickUp");
		moveToPlayerOnTrigger = serializedObject.FindProperty ("moveToPlayerOnTrigger");
		pickUpOption = serializedObject.FindProperty ("pickUpOption");
		mainPickupType = serializedObject.FindProperty ("mainPickupType");
		canBeExamined = serializedObject.FindProperty ("canBeExamined");
		examiningObject = serializedObject.FindProperty ("examiningObject");
		ignoreExamineObjectBeforeStoreEnabled = serializedObject.FindProperty ("ignoreExamineObjectBeforeStoreEnabled");
		showPickupInfoOnTaken = serializedObject.FindProperty ("showPickupInfoOnTaken");
		usePickupIconOnScreen = serializedObject.FindProperty ("usePickupIconOnScreen");
		pickupIconGeneralName = serializedObject.FindProperty ("pickupIconGeneralName");
		pickupIconName = serializedObject.FindProperty ("pickupIconName");
		usePickupIconOnTaken = serializedObject.FindProperty ("usePickupIconOnTaken");
		pickupIcon = serializedObject.FindProperty ("pickupIcon");
		usableByAnything = serializedObject.FindProperty ("usableByAnything");
		usableByPlayer = serializedObject.FindProperty ("usableByPlayer");
		usableByVehicles = serializedObject.FindProperty ("usableByVehicles");
		usableByCharacters = serializedObject.FindProperty ("usableByCharacters");
		useEventOnTaken = serializedObject.FindProperty ("useEventOnTaken");
		eventOnTaken = serializedObject.FindProperty ("eventOnTaken");
		useEventOnRemainingAmount = serializedObject.FindProperty ("useEventOnRemainingAmount");
		eventOnRemainingAmount = serializedObject.FindProperty ("eventOnRemainingAmount");
		sendPickupFinder = serializedObject.FindProperty ("sendPickupFinder");
		sendPickupFinderEvent = serializedObject.FindProperty ("sendPickupFinderEvent");

		inventoryObjectManager = serializedObject.FindProperty ("inventoryObjectManager");

		deviceStringActionManager = serializedObject.FindProperty ("deviceStringActionManager");

		mainSphereCollider = serializedObject.FindProperty ("mainSphereCollider");
		mainCollider = serializedObject.FindProperty ("mainCollider");

		mainRigidbody = serializedObject.FindProperty ("mainRigidbody");

		mainPickupManagerName = serializedObject.FindProperty ("mainPickupManagerName");

		manager = (pickUpObject)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (amount);
		EditorGUILayout.PropertyField (useAmountPerUnit);
		if (useAmountPerUnit.boolValue) {
			EditorGUILayout.PropertyField (amountPerUnit);
		}
	
		EditorGUILayout.PropertyField (pickUpSound);
		EditorGUILayout.PropertyField (staticPickUp);
		EditorGUILayout.PropertyField (moveToPlayerOnTrigger);
		EditorGUILayout.PropertyField (pickUpOption);

		EditorGUILayout.PropertyField (mainPickupType);
		EditorGUILayout.PropertyField (mainPickupManagerName);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Examine Settings", "window");
		EditorGUILayout.PropertyField (canBeExamined);
		if (canBeExamined.boolValue) {
			GUILayout.Label ("Examining Object\t" + examiningObject.boolValue);
		}

		EditorGUILayout.PropertyField (ignoreExamineObjectBeforeStoreEnabled);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Icon Settings", "window");
		EditorGUILayout.PropertyField (showPickupInfoOnTaken);

		EditorGUILayout.PropertyField (usePickupIconOnScreen);
		if (usePickupIconOnScreen.boolValue) {
			EditorGUILayout.PropertyField (pickupIconGeneralName);
			EditorGUILayout.PropertyField (pickupIconName);
		}
		EditorGUILayout.PropertyField (usePickupIconOnTaken);
		if (usePickupIconOnTaken.boolValue) {
			EditorGUILayout.PropertyField (pickupIcon);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Pick Up Used By", "window");
		EditorGUILayout.PropertyField (usableByAnything);
		if (!usableByAnything.boolValue) {
			EditorGUILayout.PropertyField (usableByPlayer);
			EditorGUILayout.PropertyField (usableByVehicles);
			EditorGUILayout.PropertyField (usableByCharacters);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Pick Up Taken Event Settings", "window");
		EditorGUILayout.PropertyField (useEventOnTaken);
		if (useEventOnTaken.boolValue) {
			EditorGUILayout.PropertyField (eventOnTaken);

			EditorGUILayout.PropertyField (useEventOnRemainingAmount);
			if (useEventOnRemainingAmount.boolValue) {
				EditorGUILayout.PropertyField (eventOnRemainingAmount);
			}
		}
		EditorGUILayout.PropertyField (sendPickupFinder);
		if (sendPickupFinder.boolValue) {
			EditorGUILayout.PropertyField (sendPickupFinderEvent);
		}

		EditorGUILayout.Space ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Pickup Components")) {
			manager.assignPickupElementsOnEditor ();
		}
			
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object Components", "window");
		EditorGUILayout.PropertyField (inventoryObjectManager);
		EditorGUILayout.PropertyField (deviceStringActionManager);
		EditorGUILayout.PropertyField (mainSphereCollider);
		EditorGUILayout.PropertyField (mainCollider);
		EditorGUILayout.PropertyField (mainRigidbody);
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
#endif