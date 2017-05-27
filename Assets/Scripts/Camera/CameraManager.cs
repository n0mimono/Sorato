using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CameraManager : MonoBehaviour {
  public IObservable<Camera> OnUpdated { private set; get; }
  Subject<Camera> cameraSubject;

  public Camera mainCamera;

  public CameraBase current { private set; get; }
  Shaker shaker;

  public void Build() {
    current = new CameraBase ();
    shaker = new Shaker ();

    cameraSubject = new Subject<Camera> ();
    OnUpdated = cameraSubject;
  }

  public void UpdateCamera() {
    shaker.Update ();

    mainCamera.transform.forward = current.trans.forward;
    mainCamera.transform.position = current.trans.position + shaker.offset;

    cameraSubject.OnNext (mainCamera);
  }

  public void Shake() {
    shaker.Invoke ();
  }

  public Vector3 ScreenPosition(Vector3 world) {
    Vector3 viewPoint = mainCamera.WorldToViewportPoint (world);
    if (viewPoint.z > 0f) {
      Vector3 scrPoint = mainCamera.WorldToScreenPoint (world);
      return new Vector2 (scrPoint.x, scrPoint.y);
    } else {
      return new Vector2 (-1000f, -1000f);
    }
  }

}

public class CameraBase {
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

  public CameraBase() {
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

public class Shaker {
  float decay = 0.01f;
  float coef  = 0.2f;

  float intensity = 0f;
  public Vector3 offset { private set; get; }

  public void Update() {
    offset = Random.insideUnitSphere * intensity;
    intensity = Mathf.Max (0f, intensity - decay);
  }

  public void Invoke() {
    intensity = coef;
  }

}
