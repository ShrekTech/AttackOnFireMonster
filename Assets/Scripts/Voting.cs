using UnityEngine;
using System.Collections;

public class Voting : MonoBehaviour
{
    public const int RED = 0;
    public const int BLUE = 1;
    public const int WHITE = 2;
    public const int GREEN = 3;
    public const int MaxPlayers = 4;
    [System.NonSerialized]
    public static int[] ChosenOption = new int[MaxPlayers];

    public int MyPlayerIndex = 0;
    // NOTE: options start at 1 (0 is "no option selected")
    public void SendOption(int option)
    {
        var networkView = GetComponent<NetworkView>();
        networkView.RPC("Vote", RPCMode.All, MyPlayerIndex, option);
    }

    void Start()
    {
        MyPlayerIndex = 0;
        ++currentPlayerCount;
    }

    int currentPlayerCount = 0;

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + currentPlayerCount + " connected from " + player.ipAddress + ":" + player.port);
        if (currentPlayerCount < MaxPlayers) {
            var networkView = GetComponent<NetworkView>();
            networkView.RPC("SetPlayerIndex", player, currentPlayerCount);

            ++currentPlayerCount;
        }
        else {
            Network.CloseConnection(player, true);
        }
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        --currentPlayerCount;

        var networkView = GetComponent<NetworkView>();
        for (int i = 0; i < Network.connections.Length; ++i)
            networkView.RPC("SetPlayerIndex", Network.connections[i], i);
    }

    [RPC]
    void SetPlayerIndex(int value)
    {
        Debug.Log(value);
        MyPlayerIndex = value;
    }

    bool AllVotesAreIn()
    {
        // TODO: unity-test assert currentPlayerCount < ChosenOption.Length
        for (int playerIndex = 0; playerIndex < currentPlayerCount; ++playerIndex) {
            if (ChosenOption[playerIndex] == 0)
                return false;
        }
        return true;
    }

    [RPC]
    void ResetVotes()
    {
        System.Array.Clear(ChosenOption, 0, ChosenOption.Length);
    }

    [RPC]
    void Vote(int playerIndex, int option)
    {
        ChosenOption[playerIndex] = option;
    }
}
