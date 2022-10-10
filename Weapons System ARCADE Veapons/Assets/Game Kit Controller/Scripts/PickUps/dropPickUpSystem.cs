using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dropPickUpSystem : MonoBehaviour
{
	public bool dropPickupsEnabled = true;

	public List<dropPickUpElementInfo> dropPickUpList = new List<dropPickUpElementInfo> ();
	public List<pickUpElementInfo> managerPickUpList = new List<pickUpElementInfo> ();
	public float dropDelay;
	public bool destroyAfterDropping;
	public float pickUpScale;
	public bool setPickupScale;
	public bool randomContent;
	public float maxRadiusToInstantiate = 1;
	public Vector3 pickUpOffset;

	public float extraForceToPickup = 5;
	public float extraForceToPickupRadius = 5;
	public ForceMode forceMode = ForceMode.Impulse;

	public string mainPickupManagerName = "Pickup Manager";

	public bool showGizmo;
	GameObject newObject;
	pickUpManager mainPickupManager;

	GameObject objectToInstantiate;

	//instantiate the objects in the enemy position, setting their configuration
	public void createDropPickUpObjects ()
	{
		if (!dropPickupsEnabled) {
			return;
		}

		StartCoroutine (createDropPickUpObjectsCoroutine ());
	}

	IEnumerator createDropPickUpObjectsCoroutine ()
	{
		yield return new WaitForSeconds (dropDelay);

		Vector3 targetPosition = transform.position + getOffset ();

		Quaternion targetRotation = transform.rotation;

		int dropPickUpListCount = dropPickUpList.Count;

		int managerPickUpListCount = managerPickUpList.Count;

		for (int i = 0; i < dropPickUpListCount; i++) {
			dropPickUpElementInfo categoryList = dropPickUpList [i];

			int typeIndex = categoryList.typeIndex;

			int dropPickUpTypeListCount = categoryList.dropPickUpTypeList.Count;

			for (int k = 0; k < dropPickUpTypeListCount; k++) {
				dropPickUpTypeElementInfo pickupTypeList = categoryList.dropPickUpTypeList [k];

				int nameIndex = pickupTypeList.nameIndex;

				//of every object, create the amount set in the inspector, the ammo and the inventory objects will be added in future updates
				int maxAmount = pickupTypeList.amount;
				int quantity = pickupTypeList.quantity;

				if (randomContent) {
					maxAmount = (int)Random.Range (pickupTypeList.amountLimits.x, pickupTypeList.amountLimits.y);
				}

				if (typeIndex < managerPickUpListCount && nameIndex < managerPickUpList [typeIndex].pickUpTypeList.Count) {
					objectToInstantiate = managerPickUpList [typeIndex].pickUpTypeList [nameIndex].pickUpObject;

					bool objectToInstantiateFound = false;

					if (objectToInstantiate != null) {
						
						for (int j = 0; j < maxAmount; j++) {
							if (randomContent) {
								quantity = (int)Random.Range (pickupTypeList.quantityLimits.x, pickupTypeList.quantityLimits.y);
							}

							newObject = (GameObject)Instantiate (objectToInstantiate, targetPosition, targetRotation);

							newObject.name = objectToInstantiate.name;

							pickUpObject currentPickUpObject = newObject.GetComponent<pickUpObject> ();

							if (currentPickUpObject != null) {
								currentPickUpObject.amount = quantity;
							}

							if (setPickupScale) {
								newObject.transform.localScale = Vector3.one * pickUpScale;
							}

							//set a random position  and rotation close to the enemy position
							newObject.transform.position += Random.insideUnitSphere * maxRadiusToInstantiate;

							//apply force to the objects
							Rigidbody currentRigidbody = newObject.GetComponent<Rigidbody> ();

							if (currentRigidbody != null) {
								currentRigidbody.AddExplosionForce (extraForceToPickup, transform.position, extraForceToPickupRadius, 1, forceMode);
							}
						}

						objectToInstantiateFound = true;
					}

					if (!objectToInstantiateFound) {
						print ("Warning, the pickups haven't been configured correctly in the pickup manager inspector");
					}
				}
			}
		}

		if (destroyAfterDropping) {
			Destroy (gameObject);
		}
	}

	public void getManagerPickUpList ()
	{
		if (mainPickupManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainPickupManagerName, typeof(pickUpManager));

			mainPickupManager = FindObjectOfType<pickUpManager> ();
		} 

		if (mainPickupManager != null) {
			managerPickUpList.Clear ();

			for (int i = 0; i < mainPickupManager.mainPickUpList.Count; i++) {	
				managerPickUpList.Add (mainPickupManager.mainPickUpList [i]);
			}

			updateComponent ();

			print ("Updating Pickup Manager Info");
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update drop pickups system", gameObject);
	}

	public Vector3 getOffset ()
	{
		return (pickUpOffset.x * transform.right + pickUpOffset.y * transform.up + pickUpOffset.z * transform.forward);
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (!Application.isPlaying && showGizmo) {
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (transform.position + getOffset (), maxRadiusToInstantiate);
		}
	}

	[System.Serializable]
	public class dropPickUpElementInfo
	{
		public string pickUpType;
		public int typeIndex;
		public List<dropPickUpTypeElementInfo> dropPickUpTypeList = new List<dropPickUpTypeElementInfo> ();
	}

	[System.Serializable]
	public class dropPickUpTypeElementInfo
	{
		public string name;
		public int amount;
		public int quantity;
		public Vector2 amountLimits;
		public Vector2 quantityLimits;
		public int nameIndex;
	}
}