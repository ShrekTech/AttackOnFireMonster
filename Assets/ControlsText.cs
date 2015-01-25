using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControlsText : MonoBehaviour {
    public int playerIndex;
    public int Option;

    Text graphic;

    public void Awake()
    {
        graphic = GetComponent<Text>();
    }

    public void Start()
    {
        var controlsConfigComponent = GetComponentInParent<PlayerActionVoteCounter>();
        if (controlsConfigComponent) {
            graphic.text = controlsConfigComponent.playerControlsConfig[playerIndex].controls[Option].keyCode.ToString();
        }
    }

    public void Update()
    {
        if (playerIndex < 0 || playerIndex > PlayerActionVoteCounter.PlayerUsedGamepad.Length) {
            Debug.LogError("Wrong");
            this.enabled = false;
            return;
        }

        this.graphic.enabled = !PlayerActionVoteCounter.PlayerUsedGamepad[playerIndex];
    }
}
