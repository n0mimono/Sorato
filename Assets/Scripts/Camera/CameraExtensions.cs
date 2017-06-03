using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CameraUpShot {
  public bool IsActive;
  public float blend;

  public Vector3 forward;
  public Vector3 position;

  IEnumerator routine;

  Transform target;
  Transform origin;

  public void StartUpShot(Transform target, Transform origin) {
    this.target = target;
    this.origin = origin;

    position = origin.position;
    forward = origin.forward;

    IsActive = true;
    routine = Proc ();
  }

  public void Update(float dt) {
    var hasNext = routine.MoveNext ();
    if (!hasNext) {
      IsActive = false;
    }
  }

  IEnumerator Proc() {
    var opt = 25f;

    Action update = () => {
      var dir = (target.position - origin.position).normalized;
      var tgt = target.position - dir * opt;
      position = tgt;
      forward = dir;
    };

    for (float t = 0f; t < 0.2f; t += Time.deltaTime) {
      update ();
      blend = t * 5f;
      yield return null;
    }
    blend = 1f;

    for (float t = 0f; t < 0.5f; t += Time.deltaTime) {
      update ();
      yield return null;
    }

    for (float t = 0f; t < 0.2f; t += Time.deltaTime) {
      update ();
      blend = 1f - t * 5f;
      yield return null;
    }
    blend = 0f;
  }

}

public class Shaker {
  float decay = 0.01f;
  float coef  = 0.2f;

  float intensity = 0f;
  public Vector3 offset { private set; get; }

  public void Update() {
    offset = UnityEngine.Random.insideUnitSphere * intensity;
    intensity = Mathf.Max (0f, intensity - decay);
  }

  public void Invoke() {
    intensity = coef;
  }

}
