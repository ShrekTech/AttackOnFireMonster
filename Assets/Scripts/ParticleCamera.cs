using UnityEngine;
using System.Collections;

/// <summary>
/// Renders particles to a RenderTexture for use in a canvas by a RawImage, see ParticleLayer
/// </summary>
/// <remarks> Author: jhuffman </remarks>
[RequireComponent(typeof(Camera))]
public class ParticleCamera : MonoBehaviour
{
  private RenderTexture _renderTexture;

  void Awake()
  {
    if (camera.isOrthoGraphic == false)
    {
      Debug.LogError("The particle camera must be orthographic in order to work.");
    }

    //Set the camera to the same size as the canvas
    camera.orthographicSize = Screen.height / 2;

    //Create the rexnderTexture at half the resolution of the screen to reduce memory use and stay below 2048x2048. If artifacting this should be switched to full res.
    _renderTexture = new RenderTexture(Screen.width / 2, Screen.height / 2, 16);

    camera.targetTexture = _renderTexture;

    //NOTE: I run my coroutine on another game object dedicated to the purpose as I deactive the camera below and don;t want to run into ppotential issues because of it.
    //MCKUtils.RunCoroutine(RenderParticles());
    StartCoroutine(RenderParticles());
    gameObject.SetActive(false);
  }

  /// <summary>
  /// Manually calls render on the camera once per frame instead of relying on the camera to update itself which causes issues on iOS devices
  /// </summary>
  private IEnumerator RenderParticles()
  {
    while (true)
    {
      RenderTexture currentRT = RenderTexture.active;
      RenderTexture.active = camera.targetTexture;
      camera.Render();
      RenderTexture.active = currentRT;
      yield return new WaitForEndOfFrame();
    }
  }
}