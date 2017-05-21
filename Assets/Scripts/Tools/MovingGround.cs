using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGround : MonoBehaviour {
  public Transform target;
	
  void Update () {
    Vector3 position = target.position;
    position.y = 0f;
    transform.position = position;
  }
}
