using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour {
  public void StopAll() {
    var bgm = GetComponent<AudioSource> ();
    StartCoroutine (bgm.LazyStop (() => {}));
  }
}
