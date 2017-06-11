using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
  public Image stop;
  public Image rotater;

  void Start() {
    if (SceneManager.sceneCount == 1) {
      SceneStack.MoveScene ("Title");
    }
  }

  public IEnumerator LoadSceneAsync(string name, bool activate) {
    var scene = SceneManager.GetSceneByName (name);

    if (!scene.isLoaded) {
      yield return SceneManager.LoadSceneAsync (name, LoadSceneMode.Additive);
      scene = SceneManager.GetSceneByName (name);
    }

    if (activate) {
      SceneManager.SetActiveScene (scene);
    }
  }

  IEnumerator UnLoadScenes() {
    var scenes = Enumerable.Range (0, SceneManager.sceneCount)
      .Select (i => SceneManager.GetSceneAt (i))
      .Where (s => s.name != "SceneLoader")
      .ToArray ();

    foreach (var s in scenes) {
      yield return SceneManager.UnloadSceneAsync (s);
    }
  }

  public IEnumerator MoveSceneAsync(string name) {
    yield return null;

    yield return StartCoroutine (UnLoadScenes ());

    yield return null;

    yield return StartCoroutine (LoadSceneAsync (name, true));
  }

}

public class SceneStack {
  public static SceneLoader Instance {
   get {
      if (instance == null) {
        instance = GameObject.FindObjectOfType<SceneLoader> ();
      }
      if (instance == null) {
        SceneManager.LoadScene ("SceneLoader", LoadSceneMode.Additive);
        instance = GameObject.FindObjectOfType<SceneLoader> ();
      }
      return instance;
    }
  }
  static SceneLoader instance;

  public static IEnumerator LoadSceneAsync(string name, bool activate = false) {
    yield return Instance.StartCoroutine (Instance.LoadSceneAsync (name, activate));
  }

  public static void MoveScene(string name) {
    Instance.StartCoroutine (Instance.MoveSceneAsync (name));
  }

  public static IEnumerator Open() {
    Instance.rotater.gameObject.SetActive (false);
    yield return Instance.StartCoroutine (Utility.Clock (2f,
      t => Instance.stop.color = Color.white.WithAlpha (1f - t)));
  }

  public static IEnumerator Close() {
    yield return Instance.StartCoroutine (Utility.Clock (2f,
      t => Instance.stop.color = Color.white.WithAlpha (t)));
    Instance.rotater.gameObject.SetActive (true);
  }

  public static void SetActive(bool isActive) {
    Instance.stop.enabled = !isActive;
  }

}
