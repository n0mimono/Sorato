using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
  public List<PoolPrefab> prefabs;

  [Serializable]
  public class PoolPrefab {
    public GameObject obj;
    public PoolProperty prop;
  }

  List<PoolObject> pool;
  static ObjectPool instance; 

  void Awake() {
    instance = this;
    pool = new List<PoolObject> ();
  }

  public static PoolObject GetInstance(PoolType type, int no) {
    var obj = instance.pool
      .FirstOrDefault (p =>
        p.Prop.type == type &&
        p.Prop.no == no &&
        !p.IsActive
      );

    if (obj == null) {
      var entity = instance.prefabs.FirstOrDefault(p => 
        p.prop.type == type &&
        p.prop.no == no
      );
      if (entity == null) {
        Debug.LogWarning("No object: " + type + ", " + no);
      }

      var prefab = entity.obj;
      var prop = entity.prop;

      obj = Instantiate (prefab, instance.transform)
        .AddComponent<PoolObject> ();
      obj.name = prefab.name + "_" + instance.pool.Count;

      obj.Initialize (prop);
      instance.pool.Add (obj);
    }

    return obj;
  }

}
