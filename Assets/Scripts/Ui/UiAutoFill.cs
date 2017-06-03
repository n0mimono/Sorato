using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiAutoFill : MonoBehaviour {

  public void StartFill() {
    StartCoroutine (Fill ());
  }

  IEnumerator Fill() {
    var image = GetComponent<Image> ();
    yield return StartCoroutine (Utility.Clock (3f, t => image.fillAmount = t / 3f));
  }
}
