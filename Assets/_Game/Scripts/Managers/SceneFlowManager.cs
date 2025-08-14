using UnityEngine;
using UnityEngine.SceneManagement;
using StarlightVigil;

namespace StarlightVigil.Managers
{
    public class SceneFlowManager : Singleton<SceneFlowManager>
    {
        public string currentSceneName = "";

        protected override void Awake()
        {
            base.Awake();
            currentSceneName = SceneManager.GetActiveScene().name;
            GameDebug.Log("SceneFlowManager Initialized");
        }

        public void LoadScene(string sceneName)
        {
            // TODO: Week 14 µœ÷≥°æ∞«–ªª
            GameDebug.Log($"LoadScene {sceneName} - Coming in Week 14");
        }
    }
}