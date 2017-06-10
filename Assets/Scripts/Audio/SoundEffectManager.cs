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
    public AudioSource source;
  }
  public List<SoundEffect> seList;

  SoundEffect latest;

  public void Play(int type, int no) {
    var s = seList.FirstOrDefault (a => a.type == type && a.no == no);
    if (s == null) {
      Debug.LogWarning (type + ", " + no);
    }

    s.source.Play ();
  }

}

public static class SoundEffect {

  static SoundEffectManager instance {
    get {
      return GameObject.FindObjectOfType<SoundEffectManager>();
    }
  }

  public static void Play(int type, int no) {
    instance.Play (type, no);
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
