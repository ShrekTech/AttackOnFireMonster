using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Takes the renderTexture object from the camera and applies it to the Rawimage component allowing it to be rendered in the Canvas
/// </summary>
/// <remarks> Author: jhuffman </remarks>
[RequireComponent(typeof(RawImage))]
public class ParticleLayer : MonoBehaviour
{
  [SerializeField]
  private Camera _particleCamera;

  private RawImage _particleImage;

  void Awake()
  {
    if (_particleCamera == null)
    {
      Debug.LogError(string.Format("Particle camera on {0} is null", gameObject.name));
    }

    _particleImage = GetComponent<RawImage>();
    _particleImage.enabled = true;
  }

  void LateUpdate()
  {
    _particleImage.texture = _particleCamera.targetTexture;
  }
}