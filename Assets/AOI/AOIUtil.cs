namespace AOI
{
    /// <summary>
    /// the Utility of AOI for user
    /// </summary>

    public static class AOIUtil
    {
        public static AOIManager CreateBlackBox(string name)
        {
            var blackBox = new AOIManager(name);
            return blackBox;
        }

        public static void DestroyBlackBox(AOIManager box)
        {
            box.Destroy();
        }

    }
}
