using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace managers
{
    public enum QuestType
    {
        AnnoyHuman,
        FindNest,
        EntratainHuman,
        FindFood
    }

    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [SerializeField] private GameObject questUIPrefab;
        [SerializeField] private Transform questListContainer;

        private Dictionary<QuestType, Quest> quests = new Dictionary<QuestType, Quest>();
        private Dictionary<QuestType, GameObject> questUIElements = new Dictionary<QuestType, GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            AddQuest(new Quest(QuestType.AnnoyHuman, "Steal coffee from a coffee guy", 1));
            AddQuest(new Quest(QuestType.FindNest, "Have a relaxing nap", 1));
            AddQuest(new Quest(QuestType.EntratainHuman, "Perform for bread crumbs", 1));
            AddQuest(new Quest(QuestType.FindFood, "Fat sparrow", 100));
        }

        public void AddQuest(Quest quest)
        {
            if (!quests.ContainsKey(quest.Id))
            {
                quests.Add(quest.Id, quest);
                
                if (questUIPrefab != null && questListContainer != null)
                {
                    var questUI = Instantiate(questUIPrefab, questListContainer);
                    questUI.name = $"QuestUI_{quest.Id}";
                    var titleText = questUI.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                    
                    if (titleText != null){
                        titleText.text = quest.Title;
                        titleText.fontStyle = quest.IsComplete ? FontStyles.Strikethrough : FontStyles.Normal;
                    }
                    questUIElements.Add(quest.Id, questUI);
                    UpdateQuestUI(quest);
                }
            }
        }

        public void ProgressQuest(QuestType questType, int progress)
        {
            if (quests.TryGetValue(questType, out Quest quest))
            {
                if(quest.CheckProgress(progress)){
                    quest.Complete();
                    if(AllQuestsCompleted())
                        MainGameManager.Instance.GameFinished(true);
                }
                UpdateQuestUI(quest);
            }
        }

        private void UpdateQuestUI(Quest quest)
        {
            if (questUIElements.TryGetValue(quest.Id, out GameObject questUI))
            {
                var titleText = questUI.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                if (titleText != null)
                        titleText.fontStyle = quest.IsComplete ? FontStyles.Strikethrough : FontStyles.Normal;
                var progressText = questUI.transform.Find("Status").GetComponent<TextMeshProUGUI>();
                if (progressText != null)  
                    progressText.text = quest.CurrentCount + "/" + quest.Count;
            }
        }

        public Quest GetQuest(QuestType questType)
        {
            quests.TryGetValue(questType, out Quest quest);
            return quest;
        }

        public IEnumerable<Quest> GetAllQuests()
        {
            return quests.Values;
        }

        private bool AllQuestsCompleted()
        {
            foreach (var quest in quests.Values)
            {
                if (!quest.IsComplete)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
