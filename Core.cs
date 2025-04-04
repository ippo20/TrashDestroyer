using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Trash;
using HarmonyLib;

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
            Vector3 barnPosition = new Vector3(190.2984f, 1.065f, -11.6897f);
            Vector3 docksPosition = new Vector3(-86.7962f, -1.255f, -48.1173f);
            Vector3 sweatshopPosition = new Vector3(-61.8378f, 0.715f, 138.1508f);
            Vector3 housePosition = new Vector3(-172.1976f, -2.735f, 114.9906f);
            float radius = 20.0f;

            LoggerInstance.Msg($"Checking for trash items within radius: {radius}");

            CheckAndPickUpTrashItems(barnPosition, radius);
            CheckAndPickUpTrashItems(docksPosition, radius);
            CheckAndPickUpTrashItems(sweatshopPosition, radius);
            CheckAndPickUpTrashItems(housePosition, radius);
        }

        private void CheckAndPickUpTrashItems(Vector3 position, float radius)
        {
            LoggerInstance.Msg($"Checking position: {position}");

            TrashItem[] allTrashItems = GameObject.FindObjectsOfType<TrashItem>();
            foreach (var trashItem in allTrashItems)
            {
                float distance = Vector3.Distance(position, trashItem.transform.position);
                if (distance <= radius)
                {
                    LoggerInstance.Msg($"Found trash item: {trashItem.name} at distance: {distance}");
                    PickUpTrashItem(trashItem);
                }
            }
        }

        private void PickUpTrashItem(TrashItem trashItem)
        {
            LoggerInstance.Msg($"Picked up: {trashItem.name}");
            GameObject.Destroy(trashItem.gameObject);
        }
    }
}

