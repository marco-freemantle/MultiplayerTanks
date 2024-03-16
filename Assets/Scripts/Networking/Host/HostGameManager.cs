using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Text;
using Unity.Services.Authentication;

public class HostGameManager : IDisposable
{
    private Allocation allocation;
    private string joinCode;
    private string lobbyId;

    public NetworkServer NetworkServer { get; private set; }

    private const string GameSceneName = "Game";

    private const int MaxConnections = 10;
    
    // Called from MainMenu. Starts the host and loads the game scene.
    public async Task StartHostAsync()
    {
        try
        {
            // Create an allocation for the host.
            allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create allocation: {e.Message}");
            return;
        }

        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join code: {joinCode}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        // Set the allocation and transport data.
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        try
        {
            // Create a lobby and set the join code as a lobby data object.
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "joinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode)
                }
            };

            string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Unknwon");
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", MaxConnections, lobbyOptions);
            lobbyId = lobby.Id;

            // Start the heartbeat coroutine to keep the lobby alive.
            HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return;
        }

        // Create a new NetworkServer instance and set the connection approval callback.
        NetworkServer = new NetworkServer(NetworkManager.Singleton);

        // Set the user data to be sent to the server when connecting.
        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "MissingName"),
            userAuthId = AuthenticationService.Instance.PlayerId,
        };
        // Convert the user data to a JSON string and then to a byte array.
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        // Set the connection data to the payload bytes.
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        // Start the host and load the game scene.
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    private IEnumerator HeartbeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    public async void Dispose()
    {
        HostSingleton.Instance.StopCoroutine(nameof(HeartbeatLobby));

        if (!string.IsNullOrEmpty(lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }

            lobbyId = string.Empty;
        }

        NetworkServer?.Dispose();
    }
}
