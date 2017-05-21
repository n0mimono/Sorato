using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetFormController : MonoBehaviour {
  public Transform origin;

  ParticleSystem[] particles;
  MaterialPropertyBlock prop;

  float speed;

  void Awake() {
    particles = GetComponentsInChildren<ParticleSystem> ();
  }

  void Update() {
    foreach (var p in particles) {
      var c = p.colorBySpeed;
      c.enabled = true;
      var colorModule = c.color;
      colorModule.mode = ParticleSystemGradientMode.Color;
      var alpha = Mathf.Clamp01 (1 - origin.position.y * 0.5f);
      colorModule.color = new Color (1f, 1f, 1f, alpha);
      c.color = colorModule;
    }

    Vector3 position = origin.position;
    position.y = 0.5f;
    transform.position = position;

  }

}
