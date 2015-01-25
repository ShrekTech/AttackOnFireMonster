using UnityEngine;
using System.Collections;

public class PlayerActionVoteCounter : MonoBehaviour {
    public static bool[] PlayerUsedGamepad = new bool[Voting.MaxPlayers];

    [System.Serializable]
    public struct Control
    {
        public KeyCode keyCode;
        public string gamepadButton;
    }

    [System.Serializable]
    public struct PlayerControls
    {
        public Control[] controls;

        public PlayerControls(Control one, Control two, Control three)
        {
            controls = new Control[3] { one, two, three };
        }
    }

    public PlayerControls[] playerControlsConfig = new PlayerControls[Voting.MaxPlayers] {
        new PlayerControls(
            new Control() { keyCode = KeyCode.A, gamepadButton = "P0 Option1" },
            new Control() { keyCode = KeyCode.S, gamepadButton = "P0 Option2" },
            new Control() { keyCode = KeyCode.D, gamepadButton = "P0 Option3" }),
        new PlayerControls(
            new Control() { keyCode = KeyCode.R, gamepadButton = "P1 Option1" },
            new Control() { keyCode = KeyCode.T, gamepadButton = "P1 Option2" },
            new Control() { keyCode = KeyCode.Y, gamepadButton = "P1 Option3" }),
        new PlayerControls(
            new Control() { keyCode = KeyCode.J, gamepadButton = "P2 Option1" },
            new Control() { keyCode = KeyCode.K, gamepadButton = "P2 Option2" },
            new Control() { keyCode = KeyCode.L, gamepadButton = "P2 Option3" }),
        new PlayerControls(
            new Control() { keyCode = KeyCode.Keypad4, gamepadButton = "P3 Option1" },
            new Control() { keyCode = KeyCode.Keypad5, gamepadButton = "P3 Option2" },
            new Control() { keyCode = KeyCode.Keypad6, gamepadButton = "P3 Option3" }),
    };

	public void Update () {
        if (playerControlsConfig.Length > Voting.MaxPlayers)
        {
            Debug.LogError("Incorrect controls configuration");
            this.enabled = false;
            return;
        }
        for (int playerIndex = 0; playerIndex < playerControlsConfig.Length; ++playerIndex) {
            for (int optionIndex = 0; optionIndex < playerControlsConfig[playerIndex].controls.Length; ++optionIndex) {
                if (Input.GetKeyDown(playerControlsConfig[playerIndex].controls[optionIndex].keyCode)) {
                    PlayerUsedGamepad[playerIndex] = false;
                    Voting.ChosenOption[playerIndex] = optionIndex + 1;
                }
                else if (Input.GetButtonDown(playerControlsConfig[playerIndex].controls[optionIndex].gamepadButton)) {
                    PlayerUsedGamepad[playerIndex] = true;
                    Voting.ChosenOption[playerIndex] = optionIndex + 1;
                }
            }
        }

        //if (Input.GetButtonDown("P0 Option1"))  {
        //    PlayerUsedGamepad[0] = true;
        //    Voting.ChosenOption[Voting.RED] = 1;
        //}
        //if (Input.GetKeyDown(KeyCode.A)) {
        //    PlayerUsedGamepad[0] = false;
        //    Voting.ChosenOption[Voting.RED] = 1;
        //}

        //if (Input.GetKeyDown(KeyCode.S) || Input.GetButtonDown("P0 Option2")) {
        //    Voting.ChosenOption[Voting.RED] = (int)BattleStateHandler.PlayerAction.COLDBALL;
        //}
        //if (Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("P0 Option3")) {
        //    Voting.ChosenOption[Voting.RED] = (int)BattleStateHandler.PlayerAction.DEFEND;
        //}

        //if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("P1 Option2")) {
        //    Voting.ChosenOption[Voting.BLUE] = (int)BattleStateHandler.PlayerAction.FIREBALL;
        //}
        //if (Input.GetKeyDown(KeyCode.T)) {
        //    Voting.ChosenOption[Voting.BLUE] = (int)BattleStateHandler.PlayerAction.COLDBALL;
        //}
        //if (Input.GetKeyDown(KeyCode.Y)) {
        //    Voting.ChosenOption[Voting.BLUE] = (int)BattleStateHandler.PlayerAction.DEFEND;
        //}

        //if (Input.GetKeyDown(KeyCode.J)) {
        //    Voting.ChosenOption[Voting.WHITE] = (int)BattleStateHandler.PlayerAction.FIREBALL;
        //}
        //if (Input.GetKeyDown(KeyCode.K)) {
        //    Voting.ChosenOption[Voting.WHITE] = (int)BattleStateHandler.PlayerAction.COLDBALL;
        //}
        //if (Input.GetKeyDown(KeyCode.L)) {
        //    Voting.ChosenOption[Voting.WHITE] = (int)BattleStateHandler.PlayerAction.DEFEND;
        //}

        //if (Input.GetKeyDown(KeyCode.Keypad4)) {
        //    Voting.ChosenOption[Voting.GREEN] = (int)BattleStateHandler.PlayerAction.FIREBALL;
        //}
        //if (Input.GetKeyDown(KeyCode.Keypad5)) {
        //    Voting.ChosenOption[Voting.GREEN] = (int)BattleStateHandler.PlayerAction.COLDBALL;
        //}
        //if (Input.GetKeyDown(KeyCode.Keypad6)) {
        //    Voting.ChosenOption[Voting.GREEN] = (int)BattleStateHandler.PlayerAction.DEFEND;
        //}
	}
}
