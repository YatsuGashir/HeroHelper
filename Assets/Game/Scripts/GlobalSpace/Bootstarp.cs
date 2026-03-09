using Core;
using UnityEngine;

namespace GlobalSpace
{
    public class Bootstarp : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        private void Awake()
        {
            Global.gameManager = gameManager;
        }
    }
}
