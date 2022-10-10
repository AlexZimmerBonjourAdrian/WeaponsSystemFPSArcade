using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Presets;
using UnityEditor;
#endif

public class applyPresetSystem : MonoBehaviour
{
	public static void GKCapplyProjectSettings ()
	{
		#if UNITY_EDITOR

		print ("Apply tag manager and input manager presets");

		Preset mainInputPreset = AssetDatabase.LoadAssetAtPath<Preset> ("Assets/Game Kit Controller/Presets/GKC InputManager.preset");

		SerializedObject mainInputManager = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset") [0]);

		if (mainInputManager != null) {
			Object inputManagerTargetObject = mainInputManager.targetObject;

			mainInputPreset.ApplyTo (inputManagerTargetObject);
			
			print ("Input manager presets found and applied");
		}


		Preset tagManagerPreset = AssetDatabase.LoadAssetAtPath<Preset> ("Assets/Game Kit Controller/Presets/GKC TagManager.preset");

		SerializedObject tagManager = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset") [0]);

		if (tagManager != null) {
			Object tagManagerTargetObject = tagManager.targetObject;

			tagManagerPreset.ApplyTo (tagManagerTargetObject);
			
			print ("Tag manager presets found and applied");
		}

		#endif
	}
}
