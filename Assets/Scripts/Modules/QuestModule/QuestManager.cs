using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private const float QUEST_COMPLETED_MESSAGE_SECONDS = 1.5f;

    public QuestData data;
    public GameObject mainQuestTrigger;

    private int m_playerLvl = 1;
    private float m_playerXP = 0;

    [Header("Experience")]
    public float maxLvlXP = 750;

    [Range(1.1f, 5.0f)]
    public float XPModificator = 1.1f;

    public AchivementsData achivementsData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetQuestData();
        SpawnStartQuestTrigger();
        HideAllQuestUI();
        GetQuestsAvailable();
        UpdatePlayerData();

        // Reset achivements status
        foreach (AchivementsItem v in achivementsData.items)
        {
            v.isUnblocked = false;
        }
    }

    public void HideAllQuestUI()
    {
        QuestInfoUI.Instance.Hide();
        QuestCompletedUI.Instance.Hide();
        QuestConfirmUI.Instance.Hide();
        QuestNavigateUI.Instance.Hide();
    }

    public void Update()
    {
        if (IsQuestConfirm())
        {
            GetHeightObjective();
            QuestNavigateUI.Instance.UpdateDistance(GetDistanceToObjective().ToString());
        }
    }

    public int QuestID => data.currentQuestID;

    public int GetQuestSubID()
    {
        return data.questItems[QuestID].currentObjectivesID;
    }

    public string GetQuestName()
    {
        return data.questItems[QuestID].questName;
    }
    public string GetQuestSubName()
    {
        int currentSubTargetID = data.questItems[QuestID].currentObjectivesID;
        return data.questItems[QuestID].objectives[currentSubTargetID].name;
    }

    public bool IsQuestConfirm()
    {
        return data.questItems[QuestID].isQuestConfirm;
    }

    public void GetQuestInfoData()
    {
        QuestConfirmUI.Instance.UpdateQuestName(GetQuestName());
        QuestInfoUI.Instance.UpdateQuestName(GetQuestName());
        QuestInfoUI.Instance.UpdateQuestObjectives(GetQuestSubName());
    }

    public void SetFirstQuest()
    {
        GetQuestInfoData();
    }

    public void NextQuest()
    {
        GetQuestInfoData();
        data.currentQuestID++;
        GetQuestsAvailable();
        SpawnStartQuestTrigger();
    }

    public void NextQuestSub()
    {
        int currentobjectives = data.questItems[QuestID].objectives.Length;
        if (GetQuestSubID() < currentobjectives)
            data.questItems[QuestID].currentObjectivesID++;

        GetQuestInfoData();
        SpawnObjectiveTrigger();
    }

    public void QuestStart()
    {
        QuestConfirmUI.Instance.Hide();
        QuestInfoUI.Instance.Show();
        QuestNavigateUI.Instance.Show();
    }

    public void GetQuestsAvailable()
    {
        int allQuestsNum = data.questItems.Length;
        int completedQuestsNum = data.currentQuestID;

        QuestAvailableUI.Instance.UpdateQuestsNum($"{allQuestsNum - completedQuestsNum}");
    }

    public void UpdatePlayerData()
    {
        if (XPModificator > 1.0f)
        {
            while (m_playerXP >= maxLvlXP)
            {
                m_playerLvl++;
                m_playerXP -= maxLvlXP;
                maxLvlXP *= XPModificator;
                QuestLvlUI.Instance.ShowLvlUpPanel(true);
                UpdateLvlData();
            }
        }
        else
        {
            Debug.LogError("Setting a value less than 1.0 for the 'XPModificator' will result in an infinite loop during the completion of the quest");
        }
        UpdateLvlData();
    }

    public void UpdateLvlData()
    {
        float _playerXP = Math.Abs((float)Math.Round(m_playerXP, 1));
        float _maxLvlXP = (float)Math.Round(maxLvlXP, 1);
        QuestLvlUI.Instance.UpdateLvl(m_playerLvl.ToString());
        QuestLvlUI.Instance.UpdateXP($"{_playerXP} / {_maxLvlXP}", Math.Abs(_playerXP) / _maxLvlXP);
    }

    public void QuestEnd(bool isDecline = false)
    {
        QuestConfirmUI.Instance.Hide();
        QuestInfoUI.Instance.Hide();
        QuestNavigateUI.Instance.Hide();

        if (!isDecline)
        {
            GetQuestsAvailable();
            m_playerXP += data.questItems[QuestID].xp;
            UpdatePlayerData();
            StartCoroutine(QuestCompleted());
        }
    }

    IEnumerator QuestCompleted()
    {
        QuestCompletedUI.Instance.Show();
        QuestCompletedUI.Instance.UpdateQuestName(GetQuestName());
        QuestCompletedUI.Instance.UpdateQuestXP(data.questItems[QuestID].xp);

        yield return new WaitForSeconds(QUEST_COMPLETED_MESSAGE_SECONDS);

        QuestCompletedUI.Instance.UpdateQuestName("");
        QuestCompletedUI.Instance.Hide();
        QuestLvlUI.Instance.ShowLvlUpPanel(false);
    }

    public void DeclineQuest()
    {
        QuestEnd(true);
    }

    public void SpawnStartQuestTrigger()
    {
        Vector3 position = data.questItems[QuestID].triggerPosition;
        Quaternion rotation = data.questItems[QuestID].triggerRotation;

        Instantiate(mainQuestTrigger, position, rotation);
    }

    public void SpawnObjectiveTrigger()
    {
        Vector3 position = data.questItems[QuestID].objectives[GetQuestSubID()].triggerPosition;
        Quaternion rotation = data.questItems[QuestID].objectives[GetQuestSubID()].triggerRotation;
        Vector3 scale = data.questItems[QuestID].objectives[GetQuestSubID()].triggerScale;
        GameObject spawnObjective = data.questItems[QuestID].objectives[GetQuestSubID()].spawnObjective;

        spawnObjective.transform.localScale = scale;

        Instantiate(spawnObjective, position, rotation);
    }


    // Reset currentQuestID, currentObjectivesID and isQuestConfirm on ScriptableObject in play mode start
    public void ResetQuestData()
    {
        data.currentQuestID = 0;
        for (int i = 0; i < data.questItems[QuestID].objectives.Length - 1; i++)
        {
            data.questItems[QuestID].currentObjectivesID = 0;
        }

        for (int i = 0; i < data.questItems.Length; i++)
        {
            data.questItems[i].isQuestConfirm = false;
        }
    }

    public void SetCursor(bool isShow = false)
    {
        if (isShow)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public int GetDistanceToObjective()
    {
        Vector3 objectivePosition = data.questItems[QuestID].objectives[GetQuestSubID()].triggerPosition;
        Vector3 playerPosition = GameSystem.Instance.player.transform.position;

        float distance = Vector3.Distance(playerPosition, objectivePosition);
        return Mathf.RoundToInt(distance);
    }

    public void GetHeightObjective()
    {
        Vector3 objectivePosition = data.questItems[QuestID].objectives[GetQuestSubID()].triggerPosition;

        if (objectivePosition.y >= GameSystem.Instance.player.transform.position.y + 2)
        {
            QuestNavigateUI.Instance.UpdateHeightObjective("Up");
        }
        else if (objectivePosition.y + 2 <= GameSystem.Instance.player.transform.position.y)
        {
            QuestNavigateUI.Instance.UpdateHeightObjective("Down");
        }
        else
        {
            QuestNavigateUI.Instance.UpdateHeightObjective("Same level");
        }
    }
}