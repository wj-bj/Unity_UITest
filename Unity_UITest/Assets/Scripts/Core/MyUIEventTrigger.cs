using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;


    public enum MyUIEventType
    {
        onClickWithNone = 1,
        onClickWithObject = 2,
        onClickWithName = 3,
        onClick = 4,
        onDown = 5,
        onEnter = 6,
        onExit = 7,
        onUp = 8,
    }
    public class MyUIEventTrigger : EventTrigger
    {
        public Action onClickWithNone;
        public Action<GameObject> onClickWithObject;
        public Action<string> onClickWithName;
        public Action<PointerEventData> onClick;
        public Action<PointerEventData> onDown;
        public Action<PointerEventData> onEnter;
        public Action<PointerEventData> onExit;
        public Action<PointerEventData> onUp;
        public Action<PointerEventData> onBeginDrag;
        public Action<PointerEventData> onDrag;
        public Action<PointerEventData> onEndDrag;
        public Action<BaseEventData> onSelect;
        public Action<BaseEventData> onUpdateSelect;

        public static MyUIEventTrigger Get(GameObject go)
        {
            MyUIEventTrigger listener = go.GetComponent<MyUIEventTrigger>();
            if (listener == null) listener = go.AddComponent<MyUIEventTrigger>();
            return listener;
        }

        public static MyUIEventTrigger Get(UIBehaviour ui)
        {
            MyUIEventTrigger listener = ui.gameObject.GetComponent<MyUIEventTrigger>();
            if (listener == null) listener = ui.gameObject.AddComponent<MyUIEventTrigger>();
            return listener;
        }

        public static MyUIEventTrigger Get(Transform transform)
        {
            MyUIEventTrigger listener = transform.gameObject.GetComponent<MyUIEventTrigger>();
            if (listener == null) listener = transform.gameObject.AddComponent<MyUIEventTrigger>();
            return listener;
        }

        public static bool ContainsTriggger(Transform transform)
        {
            MyUIEventTrigger listener = transform.gameObject.GetComponent<MyUIEventTrigger>();
            return (listener != null);
        }

        public static void AddListener( MyUIEventType eventType, Action<PointerEventData> callback, Transform transform)
        {
            if (transform == null || callback == null)
            {
                return;
            }

            MyUIEventTrigger et = Get(transform);
            switch (eventType)
            {
                case MyUIEventType.onDown:
                    et.onDown += callback;
                    break;
                case MyUIEventType.onUp:
                    et.onUp += callback;
                    break;
            }
        }

        public static void RemoveListener(MyUIEventType eventType, Action<PointerEventData> callback, Transform transform)
        {
            if (transform == null || callback == null)
            {
                return;
            }

            MyUIEventTrigger et = Get(transform);
            switch (eventType)
            {
                case MyUIEventType.onDown:
                    et.onDown -= callback;
                    break;
                case MyUIEventType.onUp:
                    et.onUp -= callback;
                    break;
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            onClickWithObject?.Invoke(gameObject);
            onClickWithNone?.Invoke();
            onClick?.Invoke(eventData);
            onClickWithName?.Invoke(gameObject.name);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            onDown?.Invoke(eventData);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            onEnter?.Invoke(eventData);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            onExit?.Invoke(eventData);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            onUp?.Invoke(eventData);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            onSelect?.Invoke(eventData);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            onUpdateSelect?.Invoke(eventData);
        }
        public override void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.Invoke(eventData);
        }
        public override void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke(eventData);
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke(eventData);
        }
    }


