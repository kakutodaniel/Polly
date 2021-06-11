namespace HelloPolly.API
{
    public static class StaticData
    {

        public static int Count { get; set; } = 1;

        public static void IncrementCount()
        {
            Count += 1;
        }
    }
}
