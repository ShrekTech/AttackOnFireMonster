using UnityEngine;
using System.Collections;

public class AttackOfTheFireMonster : MonoBehaviour
{
    public const int MaxPlayers = 4;
    [System.NonSerialized]
    public int[] ChosenOption = new int[MaxPlayers];

    IEnumerator Start()
    {
        MasterServer.ipAddress = "127.0.0.1";
        MasterServer.RequestHostList("Cool");

        // Try to find an existing host
        {
            for (int attempt = 0; attempt < 4; ++attempt) {
                var hosts = MasterServer.PollHostList();
                if (hosts.Length > 0) {
                    Debug.Log(string.Format("Connecting to {0} on {1}:{2}", hosts[0].gameName, string.Join(".", hosts[0].ip), hosts[0].port));
                    var error = Network.Connect(hosts[0]);
                    if (error != NetworkConnectionError.NoError) {
                        Debug.LogError("Failed to connect");
                    }
                    else {
                        yield break;
                    }
                }
                else {
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        // Start a server
        {
            var error = Network.InitializeServer(10, 43212, false);
            if (error != NetworkConnectionError.NoError) {
                Debug.LogError("Failed to start server");
            }
            else {
                MasterServer.RegisterHost("Cool", "Game Namee");
            }
        }
    }

    void OnApplicationExit()
    {
        MasterServer.UnregisterHost();
    }


    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Debug.Log("Disconnected from server: " + info);
        // TODO: Host migration?
    }
}
