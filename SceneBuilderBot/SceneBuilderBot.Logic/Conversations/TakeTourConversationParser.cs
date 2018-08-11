using System;
using System.Collections.Generic;
using System.Text;

namespace SceneBuilderBot.Logic.Conversations
{
    public class TakeTourConversationParser
    {
        public static bool TakeTour(string takeTourAnswer) {
            return takeTourAnswer.ToUpper().Contains("YES");
        }
    }
}
