namespace Assets.Scripts.Infrastructure.Systems.GridFolder
{
    public struct Cost
    {
        public int G;
        public int H;
        public int F => G + H;
    }
}