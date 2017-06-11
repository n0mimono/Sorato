using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour {
  [Serializable]
  public class SoundEffect {
    public int type;
    public int no;
    public AudioClip clip;
    public bool loop;
    public float volume = 1f;
    public float blend = 1f;
  }
  public List<SoundEffect> seList;

  SoundEffect latest;

  List<AudioSource> pool;

  void Awake() {
    pool = new List<AudioSource> ();
  }

  public AudioSource Play(int type, int no, Transform parent) {
    var s = seList.FirstOrDefault (a => a.type == type && a.no == no);
    if (s == null) {
      Debug.LogWarning (type + ", " + no);
    }

    var player = pool.FirstOrDefault (p => !p.gameObject.activeInHierarchy);
    if (player == null) {
      GameObject go = new GameObject ("SE");
      player = go.AddComponent<AudioSource> ();
      pool.Add (player);
    }

    player.transform.SetParent (parent);
    player.transform.localPosition = Vector3.zero;

    player.clip = s.clip;
    player.volume = s.volume;
    player.loop = s.loop;
    player.spatialBlend = s.blend;
    player.gameObject.SetActive (true);
    if (s.loop) {
      player.Play ();
    } else {
      StartCoroutine (PlayAudio (player));
    }

    return player;
  }

  IEnumerator PlayAudio(AudioSource player) {
    player.Play ();

    yield return new WaitForSeconds (player.clip.length);
    player.Stop ();

    player.transform.SetParent (transform);
    player.gameObject.SetActive (false);
  }

  public void StopAll() {
    foreach (var p in pool) {
      StartCoroutine (p.LazyStop (() => p.gameObject.SetActive(false)));
    }
  }

}

public static class SoundEffect {

  static SoundEffectManager instance {
    get {
      return GameObject.FindObjectOfType<SoundEffectManager>();
    }
  }

  public static AudioSource Play(SE type, int no, Transform parent) {
    return instance.Play ((int)type, no, parent);
  }
 
}

public enum SE {
  Move,
  Water,
  Attack
}

public enum SE_Move {
  Engine,
  Boost,
  Air
}

public enum SE_Water {
  Run,
  Pillar,
}

public enum SE_Attack {
  Fire,
  Hit,
  Kill_0,
  Kill_1
}
