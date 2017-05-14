using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceManager {

  public IEnumerator LoadSceneAsync(string name, bool activate = false) {
    Scene scene = SceneManager.GetSceneByName (name);

    if (!scene.isLoaded) {
      yield return SceneManager.LoadSceneAsync (name, LoadSceneMode.Additive);
      scene = SceneManager.GetSceneByName (name);
    }

    if (activate) {
      SceneManager.SetActiveScene (scene);
    }
  }

}
