using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace managers
{
    public class Quest
    {
        public QuestType Id { get; private set; }
        public string Title { get; private set; }
        public int Count { get; private set; }
        public bool IsComplete { get; private set; }
        public int CurrentCount{ get; private set; }

        public Quest(QuestType id, string title, int count)
        {
            Id = id;
            Title = title;
            Count = count;
            IsComplete = false;
            CurrentCount = 0;
        }
        public void Complete()
        {
            IsComplete = true;
        }

        public bool CheckProgress(int count)
        {
            CurrentCount += count;
            return CurrentCount >= Count;
        }
    }
}