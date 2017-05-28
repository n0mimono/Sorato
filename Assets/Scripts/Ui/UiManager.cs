using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class UiManager : MonoBehaviour {
  [Header("Base")]
  public Image basePanel;
  public IObservable<Vector2> baseChanged { private set; get; }

  [Header("Rotation")]
  public Image rot;
  public Image rotSub;
  public IObservable<BooledVariable<Quaternion>> rotChanged { private set; get; }

  [Header("Dash")]
  public Image spdGauge;
  public Text spdText;
  public Text heightText;

  public Button dashBtn;
  public Image dashGauge;
  public Text dashText;

  [Header("Engine")]
  public List<Button> engBtns;
  public IObservable<int> engChanged { private set; get; }

  [Header("Fire")]
  public Button fireBtn;
  public Image fireGauge;
  public Text fireText;

  [Header("Target")]
  public Image playerTarget;
  public Text playerTargetTxt;

  [Header("Extra")]
  public Button extra;

  [Header("Player")]
  public UiCharacter chara;

  public IEnumerator Build() {
    BuildBase ();
    BuildRotater ();
    BuildEngine ();
    yield return null;
  }

  void BuildBase() {
    var baseGes = basePanel.GetComponent<UiGesture> ();
    baseChanged = baseGes.OnSwipe.Select (s => s.delta);
  }

  void BuildRotater() {
    var rotTrans = rot.GetComponent<RectTransform> ();

    var rotGes = rot.GetComponent<UiGesture> ();
    var onRotUpdate = Observable.Merge (rotGes.OnDown, rotGes.OnSwipe);
    var onRotDisabled = rotGes.OnUp;

    var disabledColor = new Color (1f, 1f, 1f, 0.25f);
    var enabledColor = new Color (0.2f, 0.2f, 0.2f, 0.5f);

    Subject<BooledVariable<Quaternion>> rotSbj = new Subject<BooledVariable<Quaternion>> ();
    rotChanged = rotSbj;

    onRotUpdate
      .Select (p => new Vector3 (p.position.x, p.position.y, 0f))
      .Select (p => {
        var dir = (p - rotTrans.position).normalized;
        return Quaternion.FromToRotation (Vector3.up, dir);
      }).Subscribe (q => rotSbj.OnNext (new BooledVariable<Quaternion>() { item = q, b = true }));

    onRotDisabled
      .Subscribe (_ => rotSbj.OnNext (new BooledVariable<Quaternion>() { item = Quaternion.identity, b = false }));

    rotSbj
      .Select(v => v.item)
      .Subscribe (q => {
        rotSub.transform.rotation =
          q * Quaternion.AngleAxis(360f * rotSub.fillAmount * 0.5f, Vector3.forward);
      });

    rotSbj
      .Subscribe (v => {
        if (v.b) {
          rotSub.SetJointEnabled(true);
          rot.color = enabledColor;
        } else {
          rotSub.SetJointEnabled(false);
          rot.color = disabledColor;
        }
      });

    rotSbj.OnNext (new BooledVariable<Quaternion> () { item = Quaternion.identity, b = false });
  }

  void BuildEngine() {
    Subject<int> engSbj = new Subject<int> ();
    engChanged = engSbj;

    engSbj
      .Subscribe (index => {
        engBtns.ForEach (e => e.interactable = true);
        engBtns [index].interactable = false;
    });

    foreach (var e in engBtns.Select((b,i) => new { button = b, index = i })) {
      e.button.onClick.AsObservable()
        .Subscribe (_ => engSbj.OnNext (e.index));
    }

    engSbj.OnNext (2);
  }

  public void UpdateTargetPosition(Vector3 pos, float dist) {
    playerTarget.transform.position = pos;
    playerTargetTxt.text = dist.ToString ("F1") + " m";
  }

}
