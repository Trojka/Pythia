using System;
using System.Collections.Generic;
using System.Text;

namespace SceneBuilderBot.Logic.Conversations
{
    public static class BotAnswers
    {
        public static string WelcomeMessageForNewUserTemplate => "Welcome {0}! I see you are new around here. Would you like to take a tour first?";
        public static string WelcomeMessageForKnownUserTemplate => "Welcome {0}! Would you like to proceed with you last project?";

        public static string WelcomeMessageForNewUser(string userName) => string.Format(WelcomeMessageForNewUserTemplate, userName);
        public static string WelcomeMessageForKnownUser(string userName) => string.Format(WelcomeMessageForKnownUserTemplate, userName);
    }
}
