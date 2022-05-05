
[System.Serializable]
public class Quest 
{
    public bool isActive;
    
    public string title;
    public string description;
    public int experienceReward;

    public QuestGoal[] questGoal;

    public void Complete()
    {
        isActive = false;
    }
}
