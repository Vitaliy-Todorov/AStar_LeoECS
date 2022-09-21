namespace Assets.Scripts.FindingPath.Grid
{
    public struct Cost
    {
        public int G;
        public int H;
        public int F => G + H;
    }
}