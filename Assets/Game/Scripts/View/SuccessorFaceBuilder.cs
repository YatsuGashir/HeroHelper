using System.Collections.Generic;
using Core.Successors;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class SuccessorFaceBuilder
    {
        private System.Random _rng;
        private List<Sprite> _headSprites;
        public List<Sprite> _eyeSprites;
        public List<Sprite> _mounthSprites;
        public List<Sprite> _bodySprites;

        public SuccessorFaceBuilder(List<Sprite> headSprites, List<Sprite> eyeSprites, List<Sprite> mounthSprite, List<Sprite> bodySprites)
        {
           
            _headSprites= new List<Sprite>(headSprites);
            _eyeSprites= new List<Sprite>(eyeSprites);
            _mounthSprites= new List<Sprite>(mounthSprite);
            _bodySprites= new List<Sprite>(bodySprites);

        }
        
        public GameObject BuildSuccessor(SuccessorProfile successorProfile)
        {
            _rng = new System.Random(successorProfile.FaceSeed);
            Sprite headSprite = _headSprites[_rng.Next(_headSprites.Count)];
            Sprite eyeSprite = _eyeSprites[_rng.Next(_eyeSprites.Count)];
            Sprite mouthSprite = _mounthSprites[_rng.Next(_mounthSprites.Count)];
            Sprite bodySprite = _bodySprites[_rng.Next(_bodySprites.Count)];
            
            
            GameObject successorObj = new GameObject("Successor");
            successorObj.AddComponent<RectTransform>();
            successorObj.transform.localScale = new Vector3(0.23f, 0.23f, 0.23f);
            
            var tooltip = successorObj.AddComponent<TooltipTrigerForUi>();
            var prepare = new PrepareSuccessorForTooltip();
            prepare.tooltipTriger = tooltip;
            prepare.successorStatus = successorProfile;
            
            prepare.InitializeTooltip();
            
            GameObject bodyContainer = new GameObject("BodyContainer");
            bodyContainer.transform.SetParent(successorObj.transform);

            RectTransform containerRect = bodyContainer.AddComponent<RectTransform>();
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = new Vector2(400, 400);
            containerRect.pivot = new Vector2(0.5f, 0.5f);
            containerRect.localScale = Vector3.one;

            var rectMask = bodyContainer.AddComponent<RectMask2D>();
            var i = rectMask.softness;
            i.y = 80;
            rectMask.softness = i;
            
            
            GameObject bodyObj = new GameObject("Body");
            GameObject headObj = new GameObject("Head");
            GameObject eyeObj = new GameObject("Eye");
            GameObject mouthObj = new GameObject("Mouth");
            
            Image headRenderer = headObj.AddComponent<Image>();
            headRenderer.sprite = headSprite;
            ApplySpritePivot(headRenderer);

            Image eyeRenderer = eyeObj.AddComponent<Image>();
            eyeRenderer.sprite = eyeSprite;
            ApplySpritePivot(eyeRenderer);

            Image mounthRenderer = mouthObj.AddComponent<Image>();
            mounthRenderer.sprite = mouthSprite;
            ApplySpritePivot(mounthRenderer);

            Image bodyRenderer = bodyObj.AddComponent<Image>();
            bodyRenderer.sprite = bodySprite;
            ApplySpritePivot(bodyRenderer);
            
            
            bodyObj.transform.SetParent(bodyContainer.transform);
            headObj.transform.SetParent(successorObj.transform);
            eyeObj.transform.SetParent(successorObj.transform);
            mouthObj.transform.SetParent(successorObj.transform);
            
            headRenderer.SetNativeSize();
            eyeRenderer.SetNativeSize();
            mounthRenderer.SetNativeSize();
            bodyRenderer.SetNativeSize();
            
            RectTransform headRect = headObj.GetComponent<RectTransform>();
            headRect.anchoredPosition = new Vector2(-22f, 259f);
            
            RectTransform eyeRect = eyeObj.GetComponent<RectTransform>();
            eyeRect.anchoredPosition = new Vector2(0f, 134f);
            
            RectTransform mounthRect = mouthObj.GetComponent<RectTransform>();
            mounthRect.anchoredPosition = new Vector2(0f, 71f);
            
            RectTransform bodyRect = bodyObj.GetComponent<RectTransform>();
            bodyRect.anchoredPosition = new Vector2(0f, 0f);
            
            headObj.transform.localScale = new Vector3(1f, 1f, 1);
            eyeObj.transform.localScale = new Vector3(1f, 1f, 1f);
            mouthObj.transform.localScale = new Vector3(1f, 1f, 1f);
            bodyObj.transform.localScale = new Vector3(1f, 1f, 1f);
            
            
            return successorObj;
        }
        
        private void ApplySpritePivot(Image image)
        {
            if (image.sprite == null) return;

            RectTransform rt = image.rectTransform;
            Sprite sprite = image.sprite;

            Vector2 normalizedPivot = new Vector2(
                sprite.pivot.x / sprite.rect.width,
                sprite.pivot.y / sprite.rect.height
            );

            rt.pivot = normalizedPivot;
        }
    }
}
