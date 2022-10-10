using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class showMessageOnHUDSystem : MonoBehaviour
{
	public List<messageInfo> messageInfoList = new List<messageInfo> ();

	public playerInputManager playerInput;

	public void showMessagePanel (string messageName)
	{
		for (int i = 0; i < messageInfoList.Count; i++) {
			if (messageInfoList [i].Name.Equals (messageName)) {

				if (!messageInfoList [i].showingMessage || !messageInfoList [i].dontActivateMessageIfShowing) {
					showObjectMessage (i);

					return;
				}
			}
		}
	}

	public void hideMessagePanel (string messageName)
	{
		for (int i = 0; i < messageInfoList.Count; i++) {
			if (messageInfoList [i].Name.Equals (messageName)) {

				if (messageInfoList [i].messageCoroutine != null) {
					StopCoroutine (messageInfoList [i].messageCoroutine);
				}

				messageInfoList [i].messagePanel.SetActive (false);

				messageInfoList [i].showingMessage = false;

				return;
			}
		}
	}

	public void showObjectMessage (int messageIndex)
	{
		if (messageInfoList [messageIndex].messageCoroutine != null) {
			StopCoroutine (messageInfoList [messageIndex].messageCoroutine);
		}

		messageInfoList [messageIndex].messageCoroutine = StartCoroutine (showObjectMessageCoroutine (messageIndex));
	}

	IEnumerator showObjectMessageCoroutine (int messageIndex)
	{
		messageInfo currentMessageInfo = messageInfoList [messageIndex];

		string newText = currentMessageInfo.messageContent;

		if (currentMessageInfo.checkForInputActionOnText && playerInput != null) {
			if (newText.Contains ("-ACTION NAME-")) {
				string keyAction = playerInput.getButtonKey (currentMessageInfo.includedActionNameOnText);
				newText = newText.Replace ("-ACTION NAME-", keyAction);
			}
		}

		currentMessageInfo.messageText.text = newText;

		currentMessageInfo.showingMessage = true;
		currentMessageInfo.eventOnMessage.Invoke ();

		currentMessageInfo.messagePanel.SetActive (true);

		yield return new WaitForSeconds (currentMessageInfo.messageDuration);

		if (currentMessageInfo.useMessageDuration) {
			currentMessageInfo.messagePanel.SetActive (false);

			currentMessageInfo.showingMessage = false;
		}
	}

	[System.Serializable]
	public class messageInfo
	{
		public string Name;

		[TextArea (10, 11)] public string messageContent;

		public GameObject messagePanel;

		public Text messageText;

		public bool useMessageDuration = true;
		public float messageDuration;

		public Coroutine messageCoroutine;

		public UnityEvent eventOnMessage;

		public bool showingMessage;

		public bool dontActivateMessageIfShowing;

		public bool checkForInputActionOnText;

		public string includedActionNameOnText;
	}
}
