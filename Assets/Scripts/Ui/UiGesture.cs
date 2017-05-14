using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class UiGesture : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler, IDragHandler {
  Subject<PointerEventData> swiper = new Subject<PointerEventData> ();
  Subject<PointerEventData> upper = new Subject<PointerEventData> ();
  Subject<PointerEventData> downer = new Subject<PointerEventData> ();

  public IObservable<PointerEventData> OnSwipe {
    get {
      return swiper;
    }
  }
  public IObservable<PointerEventData> OnUp {
    get {
      return upper;
    }
  }
  public IObservable<PointerEventData> OnDown {
    get {
      return downer;
    }
  }

  bool isSwiping;

  public void OnDrag (PointerEventData eventData) {
    if (isSwiping) {
      swiper.OnNext (eventData);
    }
  }

  public void OnPointerUp (PointerEventData eventData) {
    if (isSwiping) {
      upper.OnNext (eventData);
    }
    isSwiping = false;
  }

  public void OnPointerExit (PointerEventData eventData) {
    if (isSwiping) {
      upper.OnNext (eventData);
    }
    isSwiping = false;
  }

  public void OnPointerDown (PointerEventData eventData) {
    if (!isSwiping) {
      downer.OnNext (eventData);
    }
    isSwiping = true;
  }

}
