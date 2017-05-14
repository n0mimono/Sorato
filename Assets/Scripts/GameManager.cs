using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

  ResourceManager resource;
  UiManager ui;

  IEnumerator Start() {
    resource = new ResourceManager ();  
    yield return StartCoroutine (resource.LoadSceneAsync ("SoratoUi"));

    ui = GameObject.FindObjectOfType<UiManager> ();
    yield return StartCoroutine(ui.Build());
  }

}
