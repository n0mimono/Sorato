using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
  public Camera mainCamera;

  public CameraBase current { private set; get; }

  public void Build() {
    current = new CameraBase ();
  }

  public void UpdateCamera() {
    mainCamera.transform.forward = current.trans.forward;
    mainCamera.transform.position = current.trans.position;
  }

}

public class CameraBase {
  public class Target {
    public Vector3 position;

    public void Update(Vector3 point, float scl) {
      position = Vector3.Lerp (position, point, scl);
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

    public void Update(Target target) {
      forward = Quaternion.Euler(angs) * Vector3.forward;
      position = target.position - forward * length;
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
    trans = new Transform () { length = 5f, speed = 0.1f, delay = 10f };
  }

  public void SetTargetPosition(Vector3 point) {
    target.Update (point, trans.delay * Time.deltaTime);
    trans.Update (target);
  }

  public void Rotate(Vector3 angs) {
    trans.Update (angs);
  }

}
