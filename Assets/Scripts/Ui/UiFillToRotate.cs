using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiFillToRotate : MonoBehaviour {
  public Rotater rotater;
  public Image target;
  public float amplitude;
	
  Vector3 orgSpeed;

  void Start() {
    orgSpeed = rotater.speed;
  }

	void Update () {
    var scale = 1f + (1f - target.fillAmount) * (amplitude - 1f);
    rotater.speed = orgSpeed * scale;
	}

}
