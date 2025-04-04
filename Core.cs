using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Trash;
using HarmonyLib;
using Il2CppScheduleOne.Money;
using System.Collections;

[assembly: MelonInfo(typeof(TrashGrabPlus.Core), "TrashDestroyer", "1.0.0", "heimy", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace TrashGrabPlus
{
    [HarmonyPatch(typeof(TrashItem))]
    [HarmonyPatch("AddTrash")]
    [HarmonyPatch(new Type[]
        {
        typeof(TrashItem),
    })]

    public class Core : MelonMod
    {
        private MoneyManager moneyManager;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.RightBracket)) // ']' key
            {
                UseTrashGrabber();
            }
        }

        private void UseTrashGrabber()
        {

            GameObject player = GameObject.Find("Player_Local");
            Vector3 pos = player.transform.position;
            MelonLoader.MelonLogger.Msg($"Player Position: {player.transform.position}");
            CleanWithinVicinity(pos);

        }

        //-68.56, 0.05, -63.53 // top of dumpster
        //-68.56, 0.89, -63.53 // jumped
        //-68.61, -1.37, -63.40 //bottom of dumpster
        //-69.91, -2.29, -63.54 //item


        private void CleanWithinVicinity(Vector3 pos)
        {
            Vector3 docksDumpster = new Vector3(-69.39f, -0.9f, -63.50f); // center of dumpster
            Vector3 docksPosition = new Vector3(-74.0796f, -1.163f, -64.8039f); // where the player stands near the docks

            float playerVicinityRadius = 4f;
            float length = 3.03f;
            float width = 1.15f;

            // Adjust the bottom Y value to be slightly lower, since the item is near the bottom
            float minY = -2.4f; // Set to -2.5f to ensure that items near the bottom (like at -2.29) are included
            float maxY = 0.89f; // Keep this as the top of the dumpster

            // If the player is near the dumpster
            if (Vector3.Distance(pos, docksPosition) <= playerVicinityRadius)
            {
                MelonLoader.MelonLogger.Msg("Player is near the docks.");
                // Use the adjusted minY and maxY values
                PickUpTrashItems(docksDumpster, length, width, minY, maxY);
            }
        }

        private void PickUpTrashItems(Vector3 center, float length, float width, float minY, float maxY)
        {
            LoggerInstance.Msg($"Checking trash items near position: {center}");

            int trashDeleted = 0;
            TrashItem[] allTrashItems = GameObject.FindObjectsOfType<TrashItem>();
            foreach (var trashItem in allTrashItems)
            {
                if (IsWithinRectangle(trashItem.transform.position, center, length, width, minY, maxY))
                {
                    LoggerInstance.Msg($"Found trash item: {trashItem.name} at position: {trashItem.transform.position}");
                    GameObject.Destroy(trashItem.gameObject);
                    trashDeleted++;
                }
            }

            LoggerInstance.Msg($"Total trash deleted: {trashDeleted}");
            this.moneyManager.ChangeCashBalance(trashDeleted, true, false);
        }

        private bool IsWithinRectangle(Vector3 itemPos, Vector3 center, float length, float width, float minY, float maxY)
        {
            float minX = center.x - length / 2f;
            float maxX = center.x + length / 2f;
            float minZ = center.z - width / 2f;
            float maxZ = center.z + width / 2f;

            // Logging the bounds for better understanding
            MelonLoader.MelonLogger.Msg($"Checking item position: {itemPos}");
            MelonLoader.MelonLogger.Msg($"Rectangle bounds: x: [{minX}, {maxX}], y: [{minY}, {maxY}], z: [{minZ}, {maxZ}]");

            return itemPos.x >= minX && itemPos.x <= maxX &&
                   itemPos.y >= minY && itemPos.y <= maxY &&
                   itemPos.z >= minZ && itemPos.z <= maxZ;
        }


        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MelonCoroutines.Start(this.WaitForMoneyManager());
        }

        // Token: 0x06000002 RID: 2 RVA: 0x0000205F File Offset: 0x0000025F
        private IEnumerator WaitForMoneyManager()
        {
            while (this.moneyManager == null)
            {
                GameObject moneyManagerObject = GameObject.Find("Managers/@Money");
                bool flag = moneyManagerObject != null;
                if (flag)
                {
                    this.moneyManager = moneyManagerObject.GetComponent<MoneyManager>();
                }
                yield return new WaitForSeconds(1f);
                moneyManagerObject = null;
            }
            yield break;
        }

    }
}

