namespace RaceBike.Model
{
    public class GameStatsEventArgs : EventArgs
    {
        public string Title { get; private set; }
        public string Description01 { get; private set; }
        public string Description02 { get; private set; }
        public string NewButtonText { get; private set; }

        public GameStatsEventArgs(string title, string description01, string description02, string newButtonText)
        {
            Title = title;
            Description01 = description01;
            Description02 = description02;
            NewButtonText = newButtonText;
        }
    }
}