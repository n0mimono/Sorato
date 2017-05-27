using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Arrow : MonoBehaviour {
  public bool waterable;
  public int explosionNo;

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

  PoolObject pooled;

  public void Build(Params p) {
    this.p = p;
    source = GetComponent<DamageSource> ();
    pooled = GetComponent<PoolObject> ();
  }

  public void Shoot() {
    isActive = true;
    elapse = 0f;

    transform.position = p.origin.position;
    transform.forward = p.origin.up * -1f;
    pooled.SetActive (true);
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

      var splash = ObjectPool.GetInstance (PoolType.Splash, 0);
      splash.Activate (hit.point, a);

      if (!waterable) {
        Kill ();
      }
    } else if (elapse >= 0.5f) {
      var explosion = ObjectPool.GetInstance (PoolType.Explosion, explosionNo);
      explosion.Activate (hit.point, transform.eulerAngles);

      var recep = hit.collider.GetComponent<DamageReceptor> ();
      if (recep != null) {
        recep.Receive (source);
      }

      Kill ();
    }
  }

  void Kill() {
    isActive = false;

    pooled.Kill ();
  }

}
