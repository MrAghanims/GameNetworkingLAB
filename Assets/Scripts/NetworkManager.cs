using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager :
    MonoBehaviour,
    INetworkRunnerCallbacks
{
    public NetworkRunner runner;

    public NetworkPrefabRef playerPrefab;

    public TMP_Text roomInfoText;
    public TMP_Text playerListText;
    public static string LocalPlayerName;
    async void Start()
    {
        runner = GetComponent<NetworkRunner>();

        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,

            SessionName = "ArenaRoom",

            Scene = SceneRef.FromIndex(
                SceneManager.GetActiveScene().buildIndex),

            SceneManager =
                GetComponent<NetworkSceneManagerDefault>()
        });
    }
    void Update()
    {
        if (runner != null &&
        runner.IsRunning &&
        roomInfoText != null)
        {
            roomInfoText.text =
                "Room: Room1\nPlayers: " +
                runner.ActivePlayers.Count();
        }
        string list = "";

        foreach (PlayerRef player in runner.ActivePlayers)
        {
            string host =
                runner.IsServer &&
                player == runner.LocalPlayer
                ? " (HOST)"
                : "";

            list +=
                "Player " +
                player.RawEncoded +
                host +
                "\n";
        }

        playerListText.text = list;
    }

    public void OnPlayerJoined(
        NetworkRunner runner,
        PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPos =
                new Vector3(
                    Random.Range(-5, 5),
                    1,
                    Random.Range(-5, 5));

            NetworkObject obj =
     runner.Spawn(
         playerPrefab,
         spawnPos,
         Quaternion.identity,
         player);

            obj.GetComponent<PlayerNetwork>().PlayerName =
       LocalPlayerName;
        }
    }

    public void OnInput(
        NetworkRunner runner,
        NetworkInput input)
    {
    }

    public void OnPlayerLeft(
        NetworkRunner runner,
        PlayerRef player)
    { }

    public void OnShutdown(
        NetworkRunner runner,
        ShutdownReason shutdownReason)
    { }

    public void OnConnectedToServer(
        NetworkRunner runner)
    { }

    public void OnDisconnectedFromServer(
        NetworkRunner runner,
        NetDisconnectReason reason)
    { }

    public void OnConnectRequest(
        NetworkRunner runner,
        NetworkRunnerCallbackArgs.ConnectRequest request,
        byte[] token)
    { }

    public void OnConnectFailed(
        NetworkRunner runner,
        NetAddress remoteAddress,
        NetConnectFailedReason reason)
    { }

    public void OnUserSimulationMessage(
        NetworkRunner runner,
        SimulationMessagePtr message)
    { }

    public void OnSessionListUpdated(
        NetworkRunner runner,
        List<SessionInfo> sessionList)
    { }

    public void OnCustomAuthenticationResponse(
        NetworkRunner runner,
        Dictionary<string, object> data)
    { }

    public void OnHostMigration(
        NetworkRunner runner,
        HostMigrationToken hostMigrationToken)
    { }

    public void OnReliableDataReceived(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        System.ArraySegment<byte> data)
    { }

    public void OnSceneLoadDone(
        NetworkRunner runner)
    { }

    public void OnSceneLoadStart(
        NetworkRunner runner)
    { }

    public void OnInputMissing(
        NetworkRunner runner,
        PlayerRef player,
        NetworkInput input)
    { }
    public void OnObjectEnterAOI(
    NetworkRunner runner,
    NetworkObject obj,
    PlayerRef player)
    {
    }

    public void OnObjectExitAOI(
        NetworkRunner runner,
        NetworkObject obj,
        PlayerRef player)
    {
    }

    public void OnReliableDataProgress(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        float progress)
    {
    }
}