using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disintegrator : MonoBehaviour {

  public void Build(Transform parent, Vector3 point, Vector3 angles) {
    transform.SetParent (parent.parent);
    transform.position = point;
    transform.eulerAngles = angles;

    gameObject.SetActive (true);
    StartCoroutine (Proc ());
  }

  IEnumerator Proc() {
    foreach (var p in GetComponentsInChildren<ParticleSystem>()) {
      p.Play ();
    }

    yield return new WaitForSeconds (5f);
    Destroy (gameObject);
  }

  public Disintegrator Create() {
    return Instantiate (this);
  }

}
