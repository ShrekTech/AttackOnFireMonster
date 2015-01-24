using UnityEngine;
using System.Collections;

public class AttackOfTheFireMonster : MonoBehaviour
{
    public const int MaxPlayers = 4;
    [System.NonSerialized]
    public int[] ChosenOption = new int[MaxPlayers];

    public int Progress = 0;

    public GameObject SpritePrefab;

    public struct ConvoChoice : IEnumerable
    {
        public string Words;
        public string OptionA;
        public string OptionB;
        public string OptionC;

        // Some hot object oriented programming
        public IEnumerator GetEnumerator()
        {
            yield return Words;
            yield return OptionA;
            yield return OptionB;
            yield return OptionC;
        }
        int what;
        public void Add(string item)
        {
            switch (what) {
                case 0:
                    Words = item;
                    break;
                case 1:
                    OptionA = item;
                    break;
                case 2:
                    OptionB = item;
                    break;
                case 3:
                    OptionC = item;
                    break;
            }
            ++what;
        }
    }

    public ConvoChoice[] ConvoParts = new ConvoChoice[] {
        new ConvoChoice() { "Let me sing you a song of cold and fireballs.", "Sing to me, my firebeast.", "That sounds stupid. Lets fight.", "Okaaay?" },
        new ConvoChoice() { 
@"~Oh my God, we're back again
Brothers, sisters, everybody sing\n
Gonna bring you the flavor show you how
Got a gotta question for you better answer now, yeah
(Yeah)
Everybody, rock your body right
Backstreet's back alright~", "(Sing along)", "Are you singing right now? Because it sounds like you're singing. Please stop singing.", "What streets?" },
        new ConvoChoice() { "Placeholder words.", "Placeholder!", "Placeholder.", "Placeholder?" },
        new ConvoChoice() { "Placeholder words.", "Placeholder!", "Placeholder.", "Placeholder?" },
        new ConvoChoice() { "Placeholder words.", "Placeholder!", "Placeholder.", "Placeholder?" },
        new ConvoChoice() { "Placeholder words.", "Placeholder!", "Placeholder.", "Placeholder?" },
        new ConvoChoice() { "Placeholder words.", "Placeholder!", "Placeholder.", "Placeholder?" },
        new ConvoChoice() { "Placeholder words.", "Placeholder!", "Placeholder.", "Placeholder?" },
        new ConvoChoice() { "Placeholder words.", "Placeholder!", "Placeholder.", "Placeholder?" },
        new ConvoChoice() { "Placeholder words.", "Placeholder!", "Placeholder.", "Placeholder?" },
        new ConvoChoice() { "Placeholder words.", "Placeholder!", "Placeholder.", "Placeholder?" },
    };

    void OnGUI()
    {
        GUI.skin.label.richText = true;
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 10, 200));
        {
            GUILayout.BeginVertical(GUI.skin.window);
            {
                GUILayout.Label(string.Format("<color=lime>{0}</color>", ConvoParts[Progress].Words));

                GUILayout.Label(string.Format("[Q] {0}", ConvoParts[Progress].OptionA));
                GUILayout.Label(string.Format("[W] {0}", ConvoParts[Progress].OptionB));
                GUILayout.Label(string.Format("[E] {0}", ConvoParts[Progress].OptionC));
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndArea();
    }

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

                Network.Instantiate(SpritePrefab, Vector3.up, Quaternion.identity, 0);
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
