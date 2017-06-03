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
  Subject<BooledVariable<Quaternion>> rotSbj;

  [Header("Dash")]
  public Image spdGauge;
  public Text spdText;
  public Text heightText;

  public Button dashBtn;
  public Image dashGauge;
  public Text dashText;
  public UiFlash dashFlash;

  [Header("Engine")]
  public List<Button> engBtns;
  public IObservable<int> engChanged { private set; get; }
  Subject<int> engSbj;

  [Header("Fire")]
  public Button fireBtn;
  public Image fireGauge;
  public Text fireText;
  public UiFlash fireFlash;

  [Header("Target")]
  public UiTarget targetPrefab;
  public List<UiTarget> candidates { private set; get; }

  [Header("Extra")]
  public Button extra;

  [Header("Player")]
  public Image hpBar;
  public UiCharacter chara;

  bool useKeyBoard = false;

  public IEnumerator Build() {
    BuildBase ();
    BuildRotater ();
    BuildEngine ();
    BuildTargets ();

    if (useKeyBoard) {
      BuildKeyboard ();
    }
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

    rotSbj = new Subject<BooledVariable<Quaternion>> ();
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
    engSbj = new Subject<int> ();
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

  public void BuildTargets() {
    candidates = new List<UiTarget> ();
  }

  public void BuildKeyboard() {
    var keyboard = new UiKeyboard ();
    keyboard.Build ();

    keyboard.OnFire
      .Subscribe (_ => fireBtn.onClick.Invoke ());
    keyboard.OnDash
      .Subscribe (_ => dashBtn.onClick.Invoke ());

    keyboard.OnRot
      .Select (v => {
        var rot = Quaternion.AngleAxis(v.item, Vector3.forward);
        return new BooledVariable<Quaternion>() { item = rot, b = v.b };
      })
      .Subscribe (v => rotSbj.OnNext(v));

    var keyGes = keyboard.OnGesture
      .Select (v => v * 30f);
    baseChanged = baseChanged.Merge (keyGes);

    keyboard.OnEngine
      .Subscribe (n => engSbj .OnNext(n));
  }

  public UiTarget CreateTarget() {
    var target = Instantiate (targetPrefab, targetPrefab.transform.parent);
    target.Build ();

    target.name = targetPrefab.name;

    candidates.Add (target);
    return target;
  }

}
