using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemParent;
    [SerializeField] private LobbyItem lobbyItemPrefab;

    private bool isJoining = false;
    private bool isRefreshing = false;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if( isRefreshing ) { return; }
        isRefreshing = true;

        try
        {
            // Query lobbies with available slots and not locked
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter
                (
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                ),
                new QueryFilter
                (
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"
                ),
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            // Clear the list
            foreach(Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            // Add the lobbies to the list
            foreach(Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyItem.Initialise(this, lobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        isRefreshing = false;
    }

    // Called by LobbyItem
    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) { return; }
        isJoining = true;

        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["joinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        isJoining = false;
    }
}
