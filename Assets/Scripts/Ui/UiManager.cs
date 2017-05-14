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
  public IObservable<Vector2> baseSwiped;

  [Header("Rotation")]
  public Image rot;
  public List<Image> rotSubs;
  public IObservable<int> rotChanged;

  [Header("Dash")]
  public Button dashBtn;
  public Image dashGauge;

  [Header("Engine")]
  public List<Button> engBtns;
  public IObservable<int> engClicked;

  [Header("Fire")]
  public Button fireBtn;
  public Image fireGauge;
  public Text fireText;

  public IEnumerator Build() {
    BuildRotater ();
    BuildEngine ();
    yield return null;
  }

  void BuildBase() {
    var baseGes = basePanel.GetComponent<UiGesture> ();
    baseSwiped = baseGes.OnSwipe.Select (s => s.delta);
  }

  void BuildRotater() {
    var rotTrans = rot.GetComponent<RectTransform> ();

    var rotGes = rot.GetComponent<UiGesture> ();
    var onRotUpdate = Observable.Merge (rotGes.OnDown, rotGes.OnSwipe);
    var onRotDisabled = rotGes.OnUp;

    var enabledColor = new Color (1f, 1f, 1f, 1f);
    var disabledColor = new Color (0.2f, 0.2f, 0.2f, 0.5f);

    Subject<int> rotSbj = new Subject<int> ();
    rotChanged = rotSbj;

    onRotUpdate
      .Select (p => new Vector3 (p.position.x, p.position.y, 0f))
      .Select (p => {
        var dir = (p - rotTrans.position).normalized;
        return Quaternion.FromToRotation (Vector3.up, dir);
      })
      .Select (q => {
        int count = rotSubs.Count;
        int index = (int)(q.eulerAngles.z / 360f * count + 0.5f);
        index = (index + count) % count;
        return index;
      })
      .Subscribe (index => rotSbj.OnNext (index));

    onRotDisabled
      .Subscribe (_ => rotSbj.OnNext (-1));

    rotSbj
      .Subscribe (index => {
        rotSubs.ForEach(s => s.color = disabledColor);
        if (index > -1) {
          rotSubs[index].color = enabledColor;
        }
      });

    rotSubs.ForEach(s => s.color = disabledColor);
  }

  void BuildEngine() {
    Subject<int> engSbj = new Subject<int> ();
    engClicked = engSbj;

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

}
