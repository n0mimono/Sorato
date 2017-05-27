using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class JointImage : MonoBehaviour {
  Image image;
  List<Image> childs;

  public bool jointEnabled {
    set {
      image.enabled = value;
      foreach (var child in childs) {
        child.enabled = value;
      }
    }
    get {
      return image.enabled;
    }
  }

  void Awake() {
    image = GetComponent<Image> ();

    childs = GetComponentsInChildren<Image> ()
      .Where (i => i != image)
      .ToList ();
  }

}

public static class JointImageUtility {
  public static void SetJointEnabled(this Image image, bool enabled) {
    var joint = image.GetComponent<JointImage> ();
    if (joint != null)  {
      joint.jointEnabled = enabled;
    } else {
      image.enabled = enabled;
    }
  }
}
