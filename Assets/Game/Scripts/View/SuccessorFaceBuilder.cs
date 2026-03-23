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
        public List<Sprite> _faceSprites;
        public List<Sprite> _bodySprites;

        public SuccessorFaceBuilder(List<Sprite> headSprites, List<Sprite> faceSprites, List<Sprite> bodySprites)
        {
           
            _headSprites= new List<Sprite>(headSprites);
            _faceSprites= new List<Sprite>(faceSprites);
            _bodySprites= new List<Sprite>(bodySprites);

        }
        
        public GameObject BuildSuccessor(SuccessorProfile successorProfile)
        {
            _rng = new System.Random(successorProfile.FaceSeed);
            Sprite headSprite = _headSprites[_rng.Next(_headSprites.Count)];
            Sprite faceSprite = _faceSprites[_rng.Next(_faceSprites.Count)];
            Sprite bodySprite = _bodySprites[_rng.Next(_bodySprites.Count)];
            
            
            GameObject successorObj = new GameObject("Successor");
            successorObj.AddComponent<RectTransform>();

            GameObject headObj = new GameObject("Head");
            GameObject faceObj = new GameObject("Face");
            GameObject bodyObj = new GameObject("Body");

            headObj.transform.SetParent(successorObj.transform);
            faceObj.transform.SetParent(successorObj.transform);
            bodyObj.transform.SetParent(successorObj.transform);
            
            headObj.transform.localPosition = new Vector3(0, 50f, 0);
            faceObj.transform.localPosition = new Vector3(0, 15f, 0);
            bodyObj.transform.localPosition = new Vector3(0, -10f, 0);
            
            headObj.transform.localScale = new Vector3(1, 1, 1);
            faceObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            bodyObj.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            
            Image headRenderer = headObj.AddComponent<Image>();
            headRenderer.sprite = headSprite;
    
            Image faceRenderer = faceObj.AddComponent<Image>();
            faceRenderer.sprite = faceSprite;
    
            Image bodyRenderer = bodyObj.AddComponent<Image>();
            bodyRenderer.sprite = bodySprite;

           // bodyRenderer.sortingOrder = 0;
           // headRenderer.sortingOrder = 1;
           // faceRenderer.sortingOrder = 2;
    
            return successorObj;
        }
    }
}
