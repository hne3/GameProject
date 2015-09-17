﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

	private static GameManager _instance;
	
	public static GameManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<GameManager> ();
				
				//Tell unity not to destroy this object when loading a new scene!
				DontDestroyOnLoad (_instance.gameObject);
			}
			
			return _instance;
		}
	}
	
	void Awake ()
	{
		if (_instance == null) {
			//If I am the first instance, make me the Singleton
			_instance = this;
			DontDestroyOnLoad (this);
		} else {
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if (this != _instance) {
				Destroy (this.gameObject);
			}
		}
	}

	public enum GAME_STATE
	{
		MAIN_MENU,
		SETTINGS,
		ABOUT,
		GAME_MENU,
		FIND_GAME,
		HOST_GAME,
		LOBBY_WAITING,
		IN_GAME,
		GAME_RESULTS
	}

	private GameObject Player;
	private GameObject Lobby;
	private GAME_STATE currentGameState;

	private IDictionary<GAME_STATE,int> stateToSceneMap;

	void Start ()
	{
		//Setup the mapping of game state to level number
		stateToSceneMap = new Dictionary<GAME_STATE, int>();
		stateToSceneMap.Add (GAME_STATE.MAIN_MENU, 0);

		//Set default game state
		currentGameState = GAME_STATE.MAIN_MENU;

		//Create a new player object

	}

	public void transitionGameState (GAME_STATE state)
	{
		checkForLevelConflicts (state);

		//Here is where we can do other things such as doing the animation swaps for the menu system, etc.
	}

	private void checkForLevelConflicts(GAME_STATE newState)
	{
		int currentStateLevel = stateToSceneMap [currentGameState];
		int newStateLevel = stateToSceneMap [newState];
		if (newStateLevel != currentStateLevel) {
			Application.LoadLevel(newStateLevel);
		}
	}

}
