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
  public CameraUpShot upShot { private set; get; }

  Shaker shaker;

  public void Build() {
    current = new CameraBase ();
    upShot = new CameraUpShot ();

    shaker = new Shaker ();

    cameraSubject = new Subject<Camera> ();
    OnUpdated = cameraSubject;
  }

  public void UpdateCamera() {
    shaker.Update ();

    if (upShot.IsActive) {
      upShot.Update (Time.deltaTime);
      mainCamera.transform.forward = Vector3.Lerp (current.trans.forward, upShot.forward, upShot.blend).normalized;
      mainCamera.transform.position = Vector3.Lerp (current.trans.position, upShot.position, upShot.blend);
    } else {
      mainCamera.transform.forward = current.trans.forward;
      mainCamera.transform.position = current.trans.position + shaker.offset;
    }

    cameraSubject.OnNext (mainCamera);
  }

  public void Shake() {
    shaker.Invoke ();
  }

  public void UpShot(Transform tgt) {
    if (!upShot.IsActive) {
      upShot.StartUpShot (tgt, mainCamera.transform);
    }
  }

  public void FlipPostProcess() {
    var post = GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour> ();
    post.enabled = !post.enabled;
  }

  public ScreenPoint ScreenPosition(Vector3 world) {
    var viewPoint = mainCamera.WorldToViewportPoint (world);
    var scrPoint = mainCamera.WorldToScreenPoint (world);
    if (viewPoint.z > 0f) {
      return new ScreenPoint () {
        pos = new Vector2 (scrPoint.x, scrPoint.y),
        isForward = true
      };
    } else {
      return new ScreenPoint() {
        pos = new Vector2 (scrPoint.x, scrPoint.y),
        isForward = false
      };
    }
  }

}

public struct ScreenPoint {
  public Vector2 pos;
  public bool isForward;
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
