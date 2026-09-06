public static class RaceResultData
{
    public static float FinalTime { get; private set; }

    public static void SetFinalTime(float time)
    {
        FinalTime = time;
    }

    public static void Clear()
    {
        FinalTime = 0f;
    }
}