using Data;
using GlobalSpace;
using TMPro;
using UniRx;
using UnityEngine;

namespace View
{
    public class BuildingStatusView : MonoBehaviour
    {
        [SerializeField] private TMP_Text statusText;
    
        private CompositeDisposable _disposables = new CompositeDisposable();
        private BuildingInstance _myBuilding;
        
        public void Init(BuildingInstance instance)
        {
            _myBuilding = instance;
            
            G.Events.Ticked
                .Where(b => b != null && b.instanceId == _myBuilding.instanceId)
                .Subscribe(UpdateStatus)
                .AddTo(_disposables);
            
            UpdateStatus(_myBuilding);
        }
    
        public void UpdateStatus(BuildingInstance instance)
        {
            if (instance == null || instance.instanceId != _myBuilding.instanceId) 
                return;
            
            string stageName = instance.stage.ToString();
            int timeLeft = instance.remaingTime;
        
            statusText.text = $"{stageName}\n осталось времени: {timeLeft}";
        }
    
        public void Clear()
        {
            _disposables.Clear();
            if (statusText != null) statusText.text = "";
        }
    }
}
