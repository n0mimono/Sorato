using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Arrow : MonoBehaviour {
  public Disintegrator explosion;
  public Disintegrator splash;
  public bool waterable;

  public struct Params {
    public DamageReceptor target;
    public Transform origin;
  }
  Params p;

  [Serializable]
  public class Status {
    public float v;
    public float w;
  }
  public Status status;

  DamageSource source;
  bool isActive;
  float elapse;

  public void Build(Params p) {
    this.p = p;
    source = GetComponent<DamageSource> ();
  }

  public void Shoot() {
    isActive = true;
    elapse = 0f;

    transform.position = p.origin.position;
    transform.forward = p.origin.up * -1f;

    gameObject.SetActive (true);
  }

  public Arrow Create() {
    Arrow a = Instantiate (this, transform.parent);
    return a;
  }

  void Update() {
    if (isActive) {
      UpdateKinematics ();
    }
  }

  void UpdateKinematics() {
    elapse += Time.deltaTime;

    if (Vector3.Distance (p.target.transform.position, transform.position) > 20f) {
      var dir = (p.target.transform.position - transform.position).normalized;
      transform.forward = Vector3.RotateTowards (
        transform.forward,
        dir,
        status.w * Mathf.Deg2Rad * Time.deltaTime,
        0f
      );
    }

    var prev = transform.position;
    var cur = prev + transform.forward * status.v * Time.deltaTime;
    transform.position = cur;

    RaycastHit hit;
    bool isHit = Physics.Linecast (prev, cur, out hit);

    if (isHit) {
      Hit (hit);
    }
  }

  void Hit(RaycastHit hit) {
    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water")) {
      Vector3 a = Vector3.up * transform.eulerAngles.y;
      splash.Create().Build(transform, hit.point, a);

      if (!waterable) {
        StartCoroutine (ProcDestroy ());
      }
    } else if (elapse >= 0.5f) {
      explosion.Build (transform, hit.point, transform.eulerAngles);
      StartCoroutine (ProcDestroy ());

      var recep = hit.collider.GetComponent<DamageReceptor> ();
      if (recep != null) {
        recep.Receive (source);
      }
    }
  }

  IEnumerator ProcDestroy() {
    isActive = false;
    yield return new WaitForSeconds (5f);
    Destroy (gameObject);
  }

}
