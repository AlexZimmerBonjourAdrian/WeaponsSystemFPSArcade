using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ingameMenuPanel : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string menuPanelName;

	public bool useBlurUIPanel;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool menuPanelOpened;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject menuPanelObject;

	public menuPause pauseManager;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnOpenClostMenu;
	public UnityEvent eventOnOpenMenu;
	public UnityEvent eventOnCloseMenu;


	bool pauseManagerLocated;

	public virtual void openOrCloseMenuPanel (bool state)
	{
		menuPanelOpened = state;

		pauseManagerLocated = pauseManager != null;

		menuPanelObject.SetActive (menuPanelOpened);

		if (pauseManagerLocated) {
			pauseManager.openOrClosePlayerMenu (menuPanelOpened, menuPanelObject.transform, useBlurUIPanel);

			pauseManager.setIngameMenuOpenedState (menuPanelName, menuPanelOpened, false);

			pauseManager.enableOrDisablePlayerMenu (menuPanelOpened, true, false);

			pauseManager.checkUpdateReticleActiveState (state);
		}

		if (menuPanelOpened) {
			openMenuPanelState ();
		} else {
			closeMenuPanelState ();
		}

		checkEventsOnStateChange (menuPanelOpened);
	}

	public void openOrCloseMenuFromTouch ()
	{
		openOrCloseMenuPanel (!menuPanelOpened);

		pauseManagerLocated = pauseManager != null;

		if (pauseManagerLocated) {
			pauseManager.setIngameMenuOpenedState (menuPanelName, menuPanelOpened, true);
		}
	}

	public virtual void openMenuPanelState ()
	{

	}

	public virtual void closeMenuPanelState ()
	{

	}

	public void setPauseManager (menuPause currentMenuPause)
	{
		pauseManager = currentMenuPause;

		initializeMenuPanel ();
	}

	public virtual void initializeMenuPanel ()
	{

	}

	public void openNextPlayerOpenMenu ()
	{
		pauseManagerLocated = pauseManager != null;

		if (pauseManagerLocated) {
			pauseManager.openNextPlayerOpenMenu ();
		}
	}

	public void openPreviousPlayerOpenMenu ()
	{
		pauseManagerLocated = pauseManager != null;

		if (pauseManagerLocated) {
			pauseManager.openPreviousPlayerOpenMenu ();
		}
	}

	public void closePlayerMenuByName (string menuName)
	{
		pauseManagerLocated = pauseManager != null;

		if (pauseManagerLocated) {
			pauseManager.closePlayerMenuByName (menuName);
		}
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnOpenClostMenu) {
			if (state) {
				eventOnOpenMenu.Invoke ();
			} else {
				eventOnCloseMenu.Invoke ();
			}
		}
	}
}
