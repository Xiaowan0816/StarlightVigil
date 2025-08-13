using UnityEngine;
using StarlightVigil;

namespace StarlightVigil.Managers
{
    public class CraftingManager : Singleton<CraftingManager>
    {
        public int currentWorkshopLevel = 0;

        protected override void Awake()
        {
            base.Awake();
            GameDebug.Log("CraftingManager Initialized");
        }

        public void StartCrafting()
        {
            // TODO: Week 9-12й╣ож
            GameDebug.Log("Crafting system - Coming in Week 9");
        }
    }
}