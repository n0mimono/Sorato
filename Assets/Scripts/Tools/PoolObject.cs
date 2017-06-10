using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PoolObject : MonoBehaviour {
  public bool IsActive { private set; get; }

  public PoolProperty Prop { private set; get; }

  public void Initialize(PoolProperty prop) {
    this.Prop = prop;

    IsActive = false;

    gameObject.SetActive (false);
  }

  public void SetActive(bool isActive) {
    IsActive = isActive;

    gameObject.SetActive (isActive);
  }

  public void Activate(Vector3 pos, Vector3 ang) {
    SetActive (true);
    transform.position = pos;
    transform.eulerAngles = ang;

    if (Prop.autoKill) {
      Kill ();
    }
  }

  public void Kill() {
    StartCoroutine (ProcKill ());
  }

  IEnumerator ProcKill() {
    yield return new WaitForSeconds (Prop.killDelay);
    SetActive (false);
  }

}

public enum PoolType {
  Arrow,
  Explosion,
  Splash,
  Boost,
  Kill,
}

[Serializable]
public class PoolProperty {
  public PoolType type;
  public int no;
  public bool autoKill;
  public float killDelay;
}
