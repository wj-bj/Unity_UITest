using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.EventSystems;


public class MyUIButtonEffect : MonoBehaviour
    {
        public int soundId = 201;

        public string fx;

        private Vector3 oriScale = Vector3.one;
        private float duration = 0.2f;

        public bool runOnlyOnce = false;
        public bool needScale = true;

        private bool hasInit = false;
        void Start()
        {
            hasInit = true;
            oriScale = transform.localScale;
            MyUIEventTrigger.AddListener(MyUIEventType.onDown, this.OnMouseDown, this.transform);
            MyUIEventTrigger.AddListener(MyUIEventType.onUp, this.OnMouseUp, this.transform);
        }

        void OnMouseDown(PointerEventData eventData)
        {
            this.transform.localScale = oriScale;
            if (needScale)
                this.transform.DOScale(oriScale * 0.9f, duration);
            
            if(soundId > 0)
            {
                // SoundCfg soundCfg = MHCfgMgr.GetConfigById<SoundCfg>(soundId);
                // MHSoundMgr.instance.PlaySfx(soundCfg.Res);
            }

            // MHFxInfo fx = MHFxMgr.Instance.CreateFxFromPool("ui_fx_btn", 1f);
            // fx.SetParent(this.transform);
        }

        void OnMouseUp(PointerEventData eventData)
        {
            if (needScale)
            {
                Tweener tween = this.transform.DOScale(oriScale, duration);
                tween.SetEase(Ease.OutBounce);
            }
            
        }

        private void OnDisable()
        {
            if(hasInit)
            {
                transform.localScale = oriScale;
            }
        }
    }

