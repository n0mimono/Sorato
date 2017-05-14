using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour {
  public Vector3 speed;

  void Update() {
    transform.Rotate (speed * Time.deltaTime);
  }

}
