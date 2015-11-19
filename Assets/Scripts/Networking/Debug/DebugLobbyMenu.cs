﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DebugLobbyMenu : Bolt.GlobalEventListener {

    private string tempPassword = ServerSideData.Password;

    private float width = 300f;

    private const int SERVER_MENU_LINE_COUNT = 8;
    private const int CLIENT_MENU_LINE_COUNT = 5;

    private const float LINE_HEIGHT = 27.3f;

    private int selectedGameMode = 0;
    private int selectedMap = 0;

    private string[] mapList;
    private string[] humanReadableMapList;

    private string ipAddress = "Unknown";

    public Texture buttonBG;
    void Update() {
        DebugHUD.setValue("IsSever", BoltNetwork.isServer);
        if (BoltNetwork.isClient) {
            DebugHUD.setValue("ping", BoltNetwork.server.PingNetwork);
        }
        DebugHUD.setValue("username", GameManager.instance.CurrentUserName);
    }

    void Start() {
        mapList = new string[0];
        humanReadableMapList = new string[0];
        updateMapList(GameModeManager.GameModeNames[0]);
        if (BoltNetwork.isServer) {
            try {
                ipAddress = ""+System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())[0];
            } catch { }
        }
    }

    private void updateMapList(string gameModeName) {
        mapList = GameModeManager.GetSupportedMapsForGameMode(gameModeName).ToArray();
        humanReadableMapList = mapList.Select(s => GameModeManager.GetHumanReadableNameForMap(s)).ToArray();
        selectedMap = 0;
        if (humanReadableMapList.Length > 0) {
            Lobby.MapName = humanReadableMapList[selectedMap];
        } else {
            Lobby.MapName = "Not Selected";
        }
    }

    void OnGUI() {
        if(!BoltNetwork.isRunning){
            Application.LoadLevel(1);
            return;
        }

        
        if (BoltNetwork.isClient) {
            StartBox(CLIENT_MENU_LINE_COUNT * LINE_HEIGHT);
            DrawClientMenu();
        } else {
            StartBox((SERVER_MENU_LINE_COUNT + GameModeManager.GameModes.Length + mapList.Length) * LINE_HEIGHT);
            DrawServerMenu();
        }
        EndBox();
    }

    private void DrawServerMenu() {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("IP Address: " + ipAddress);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false));
        GUILayout.Label("Lobby Password:", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        tempPassword = GUILayout.TextField(tempPassword, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUI.enabled = tempPassword != ServerSideData.Password;
        if (GUILayout.Button("Apply new Password")) {
            ServerSideData.Password = tempPassword;
            ServerSideData.UpdateZeusData();
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Game mode:");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        int newGameModeIndex = GUILayout.SelectionGrid(selectedGameMode, GameModeManager.GameModeNames, 1);
        if (newGameModeIndex != selectedGameMode) {
            selectedGameMode = newGameModeIndex;
            IGameMode newGameMode = GameModeManager.GameModes[selectedGameMode];
            GameManager.instance.GameMode = newGameMode;
            updateMapList(GameModeManager.GameModeNames[selectedGameMode]);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Map:");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        int newSelectedMap = GUILayout.SelectionGrid(selectedMap, humanReadableMapList, 1);
        if (newSelectedMap != selectedMap) {
            selectedMap = newSelectedMap;
            Lobby.MapName = humanReadableMapList[selectedMap];
        }
        GUILayout.EndHorizontal();

        DrawTeamChangeButtons();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start Game")) {
            string sceneName = "ingame_" + mapList[selectedMap] + "_" + GameManager.instance.GameMode.GetType().Name;
            Debug.Log("loading scene \"" + sceneName + "\"");
            BoltNetwork.LoadScene(sceneName);
        }
        GUILayout.EndHorizontal();
    }

    private void DrawClientMenu() {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        IGameMode mode = GameManager.instance.GameMode;
        GUILayout.Label("Mode: " + (mode != null ? mode.GameModeName : "-"));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Map: " + Lobby.MapName);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        DrawTeamChangeButtons();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Disconnect")) {
            BoltNetwork.server.Disconnect();
        }
        GUILayout.EndHorizontal();
    }

    private void StartBox(float height) {
        GUILayout.BeginArea(new Rect(Screen.width - width - 10, (Screen.height - height)/2f, width, height), GUI.skin.box);
        GUILayout.BeginVertical();
    }
    private void EndBox() {
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void DrawTeamChangeButtons() {
        bool teams = GameManager.instance.GameMode.UsesTeams;
        GUILayout.BeginHorizontal();
        if (teams) {
            GUILayout.Label("Set team:");
        } else {
            GUILayout.Label("Set color:");
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        for (int i = 0; i <= 7; i++) {
            Color bgRestore = GUI.backgroundColor;
            Color restore = GUI.contentColor;
            GUIContent content;
            if (teams) {
                content = new GUIContent(i + "");
            } else {
                GUI.contentColor = Teams.Colors[i];
                content = new GUIContent(buttonBG);
            }
            GUI.backgroundColor = Teams.Colors[i];
            if ((!teams && GUILayout.Button(content, GUIStyle.none)) || (teams && GUILayout.Button(content))) {
                TeamChangeEvent evnt = TeamChangeEvent.Create(Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
                evnt.NewTeam = i;
                evnt.Send();
            }
            GUI.backgroundColor = bgRestore;
            GUI.contentColor = restore;
        }
        GUILayout.EndHorizontal();
    }

    public override void Disconnected(BoltConnection connection) {
        Bolt.IProtocolToken token = connection.DisconnectToken;
        if (token != null && token is DisconnectReason) {
            DisconnectReason reason = (DisconnectReason)token;
            Debug.Log("Disconnected from server: "+ reason.Reason + (reason.Message=="" ? "" : ": "+reason.Message));
        }
    }
}
