#nullable enable
using RuniEngine.Resource.Texts;
using System;

namespace RuniEngine.Settings
{
    public class BasicDataNotLoadedException : Exception
    {
        public static void Exception()
        {
            if (!SettingManager.isDataLoaded)
                throw new BasicDataNotLoadedException();
        }

        public BasicDataNotLoadedException() : base(LanguageLoader.TryGetText("exception.basic_data_not_loaded"))
        {

        }
    }
}
