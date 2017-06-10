using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetFormController : MonoBehaviour {
  public Transform origin;

  ParticleSystem[] particles;
  MaterialPropertyBlock prop;

  float speed;
  float orgVol;

  AudioSource source;

  void Awake() {
    particles = GetComponentsInChildren<ParticleSystem> ();

    source = SoundEffect.Play (SE.Water, 0, transform);
    orgVol = source.volume;
  }

  void Update() {
    var alpha = Mathf.Clamp01 (1 - origin.position.y * 0.5f);

    foreach (var p in particles) {
      var c = p.colorBySpeed;
      c.enabled = true;
      var colorModule = c.color;
      colorModule.mode = ParticleSystemGradientMode.Color;
      colorModule.color = new Color (1f, 1f, 1f, alpha);
      c.color = colorModule;
    }

    source.volume = alpha * orgVol;

    Vector3 position = origin.position;
    position.y = 0.5f;
    transform.position = position;

  }

}
