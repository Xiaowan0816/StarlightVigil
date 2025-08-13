using UnityEngine;
using StarlightVigil;

namespace StarlightVigil.Managers
{
    public class SaveManager : Singleton<SaveManager>
    {
        protected override void Awake()
        {
            base.Awake();
            GameDebug.Log("SaveManager Initialized");
        }

        public void SaveGame()
        {
            // TODO: Week 16实现
            GameDebug.Log("SaveGame - Coming in Week 16");
        }

        public void LoadGame()
        {
            // TODO: Week 16实现
            GameDebug.Log("LoadGame - Coming in Week 16");
        }
    }
}