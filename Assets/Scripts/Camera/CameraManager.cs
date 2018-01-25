using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CameraManager : MonoBehaviour {
  public IObservable<Camera> OnUpdated { get { return cameraSubject; } }
  Subject<Camera> cameraSubject = new Subject<Camera>();

  public Camera mainCamera;
  public Transform anchor;

  public ICamera current { private set; get; }
  public CameraUpShot upShot { private set; get; }
  Shaker shaker;

  public void Build() {
    current = new CameraRegular ();
    upShot = new CameraUpShot ();

    shaker = new Shaker ();
  }

  public void UpdateCamera() {
    shaker.Update ();

    if (upShot.IsActive) {
      anchor.forward = upShot.ForwardBy (current);
      anchor.position = upShot.PositionBy(current);
    } else {
      anchor.forward = current.forward;
      anchor.position = current.position + shaker.offset;
    }

    cameraSubject.OnNext (mainCamera);
  }

  public void Shake() {
    shaker.Invoke ();
  }

  public void UpShot(Transform tgt) {
    if (!upShot.IsActive) {
      upShot.Initialize (tgt, mainCamera.transform);
      StartCoroutine (upShot.UpShot ());
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
