using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour {
  public Button button;

  IEnumerator Start() {
    button.onClick.AddListener (() => {
      StartCoroutine(Move());
    });

    yield return StartCoroutine (SceneStack.Open ());
    SceneStack.SetActive (true);
  }

  IEnumerator Move() {
    button.interactable = false;

    SceneStack.SetActive (false);
    yield return StartCoroutine (SceneStack.Close ());

    SceneStack.MoveScene ("Sorato");
  }

}
