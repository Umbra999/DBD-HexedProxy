namespace HexedProxy.DBDObjects
{
    internal class OnboardingChallanges
    {
        public class ResponseRoot
        {
            public Progress[] progress { get; set; }
        }

        public class Progress
        {
            public bool completed { get; set; }
            public string step { get; set; }
            public Tutorial[] tutorials { get; set; }
        }

        public class Tutorial
        {
            public bool completed { get; set; }
            public bool mainRewardClaimed { get; set; }
            public string tutorialId { get; set; }
        }
    }
}
