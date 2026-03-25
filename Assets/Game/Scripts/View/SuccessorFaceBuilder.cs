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
            
            GameObject bodyObj = new GameObject("Body");
            GameObject headObj = new GameObject("Head");
            GameObject eyeObj = new GameObject("Eye");
            GameObject mouthObj = new GameObject("Mouth");
            
            Image headRenderer = headObj.AddComponent<Image>();
            headRenderer.sprite = headSprite;
            headRenderer.SetNativeSize();
            
            
            Image eyeRenderer = eyeObj.AddComponent<Image>();
            eyeRenderer.sprite = eyeSprite;
            eyeRenderer.SetNativeSize();
            
            Image mounthRenderer = mouthObj.AddComponent<Image>();
            mounthRenderer.sprite = mouthSprite;
            mounthRenderer.SetNativeSize();
    
            Image bodyRenderer = bodyObj.AddComponent<Image>();
            bodyRenderer.sprite = bodySprite;
            bodyRenderer.SetNativeSize();
            
            
            bodyObj.transform.SetParent(successorObj.transform);
            headObj.transform.SetParent(successorObj.transform);
            eyeObj.transform.SetParent(successorObj.transform);
            mouthObj.transform.SetParent(successorObj.transform);
            
            
            headObj.transform.localPosition = new Vector3(-22f, 259f, 0);
            eyeObj.transform.localPosition = new Vector3(0f, 134f, 0);
            mouthObj.transform.localPosition = new Vector3(0f, 71f, 0);
            bodyObj.transform.localPosition = new Vector3(0f, 0f, 0);
            
            headObj.transform.localScale = new Vector3(1f, 1f, 1);
            eyeObj.transform.localScale = new Vector3(1f, 1f, 1f);
            mouthObj.transform.localScale = new Vector3(1f, 1f, 1f);
            bodyObj.transform.localScale = new Vector3(1f, 1f, 1f);
            


           // bodyRenderer.sortingOrder = 0;
           // headRenderer.sortingOrder = 1;
           // faceRenderer.sortingOrder = 2;
    
            return successorObj;
        }
    }
}
