using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyFixedTransform : MonoBehaviour {
  public float scale;

  void Update() {
    transform.localPosition = Vector3.Lerp (transform.localPosition, Vector3.zero, Time.deltaTime * scale);
  }

}
