using UnityEngine;

namespace Source.MapSource.MapObjs.lever
{
    [CreateAssetMenu(fileName = "LeverData", menuName = "Scriptable Object/LeverData", order = int.MaxValue)]
    public class LeverData : ScriptableObject
    {
        [SerializeField] private Sprite beforeActive;
        public Sprite BeforeActive => beforeActive;
        [SerializeField] private Sprite afterActive;
        public Sprite AfterActive => afterActive;
    }
}
