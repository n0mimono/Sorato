using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiFillToColor : MonoBehaviour {
  public Image target;
  public Color color;

  Image image;
  Color orgColor;

  void Start() {
    image = GetComponent<Image> ();
    orgColor = target.color;
  }

  void Update () {
    image.color = Color.Lerp (color, orgColor, target.fillAmount);
  }

}
