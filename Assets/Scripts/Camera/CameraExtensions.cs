using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface ICamera {
  Vector3 position { get; }
  Vector3 forward { get; }

  void SetTargetPosition (Vector3 point);
  void Rotate (Vector3 angs);
}

public class CameraRegular : ICamera {
  public class Target {
    public Vector3 position;

    public void Update(Vector3 point) {
      position = point;
    }
  }
  public Target target { private set; get; }

  public class Transform {
    public float speed;
    public float length;
    public float delay;

    public Vector3 angs;

    public Vector3 forward;
    public Vector3 position;

    public void Update(Target target, float dt) {
      forward = Vector3.Lerp(forward, Quaternion.Euler(angs) * Vector3.forward, dt * delay);
      position = Vector3.Lerp(position, target.position - forward * length, dt * delay);
      position.y = Mathf.Clamp (position.y, GearBoard.Kinematics.MinHeight, GearBoard.Kinematics.MaxHeight);
    }
    public void Update(Vector3 delta) {
      angs += delta * speed;

      if (angs.x < 0f && angs.x >= 70f) {
        angs.x = 70f;
      }
      if (angs.x < 0f && angs.x <= -70f) {
        angs.x = -70f;
      }
    }
  }
  public Transform trans;

  public Vector3 position { get { return trans.position; } }
  public Vector3 forward { get { return trans.forward; } }

  public CameraRegular() {
    target = new Target();
    trans = new Transform () { length = 7.5f, speed = 0.2f, delay = 5f };
  }

  public void SetTargetPosition(Vector3 point) {
    target.Update (point);
    trans.Update (target, Time.deltaTime);
  }

  public void Rotate(Vector3 angs) {
    trans.Update (angs);
  }

}
  
public class CameraUpShot  : ICamera {
  public bool IsActive { private set; get; }
  public float blend { private set; get; }

  public Vector3 forward  { private set; get; }
  public Vector3 position  { private set; get; }

  Transform target;
  Transform origin;

  Action OnComplete;

  public void Initialize(Transform target, Transform origin) {
    this.target = target;
    this.origin = origin;

    position = origin.position;
    forward = origin.forward;
  }

  public IEnumerator UpShot() {
    IsActive = true;

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

    IsActive = false;
  }

  public void SetTargetPosition(Vector3 point) {
  }

  public void Rotate(Vector3 angs) {
  }

  public Vector3 PositionBy(ICamera camera) {
    return Vector3.Lerp (camera.position, position, blend);
  }

  public Vector3 ForwardBy(ICamera camera) {
    return Vector3.Lerp (camera.forward, forward, blend).normalized;
  }

}
