using System;
using System.Collections.Generic;
using UnityEngine;

public class SporeCollision : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (Time.time < SporeCollision.timeOfLastEffect + this.delayBetweenEffects)
		{
			return;
		}
		if (other.gameObject.tag == "Player")
		{
			base.gameObject.SetActive(false);
			int numFishRotted = 0;
			List<FishItemInstance> allItemsOfType = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH);
			allItemsOfType.ForEach(delegate(FishItemInstance fishItemInstance)
			{
				if (fishItemInstance.freshness > 0.99f)
				{
					fishItemInstance.freshness = 0.99f;
					int numFishRotted2 = numFishRotted;
					numFishRotted = numFishRotted2 + 1;
				}
			});
			if (numFishRotted > 0)
			{
				GameManager.Instance.UI.ShowNotificationWithColor(NotificationType.ROT, "notification.mushroom-spore-rot", GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL));
				SporeCollision.timeOfLastEffect = Time.time;
				return;
			}
			if (allItemsOfType.Count > 0)
			{
				FishItemInstance fishItemInstance2 = allItemsOfType.PickRandom<FishItemInstance>();
				GameManager.Instance.ItemManager.ReplaceFishWithRot(fishItemInstance2, GameManager.Instance.SaveData.Inventory, true);
				SporeCollision.timeOfLastEffect = Time.time;
			}
		}
	}

	private static float timeOfLastEffect;

	[SerializeField]
	private float delayBetweenEffects;
}
