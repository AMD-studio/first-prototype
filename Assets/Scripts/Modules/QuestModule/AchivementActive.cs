using System.Collections;
using System.Collections.Generic;
using System.Linq; // Добавляем using для использования LINQ
using UnityEngine;

public class AchivementActive : MonoBehaviour
{
    public int id;

    private void OnTriggerEnter(Collider other)
    {
        var achievementsDataItems = QuestManager.Instance.achivementsData.items;

        var foundAchievement = 
            achievementsDataItems
            .FirstOrDefault(item => item.id == id && !item.isUnblocked);

        if (foundAchievement != null)
        {
            string title = foundAchievement.title;
            string desc = foundAchievement.description;

            AchivementUI.Instance.Show();
            AchivementUI.Instance.UpdateTitle(title);
            AchivementUI.Instance.UpdateDescription(desc);

            foundAchievement.isUnblocked = true;

            Destroy(gameObject);
        }
    }
}
