namespace HexedProxy.Modules
{
    internal class Misc
    {
        public static void UnlockTutorials()
        {
            Task.Run(async () =>
            {
                DBDObjects.OnboardingChallanges.ResponseRoot AllChallanges = await RequestSender.GetOnboardingChallenges();

                foreach (var Challenge in AllChallanges.progress)
                {
                    foreach (var tutorial in Challenge.tutorials)
                    {
                        if (!tutorial.completed) await RequestSender.FinishTutorial(Challenge.step, tutorial.tutorialId);
                    }
                }
            });
        }
    }
}
