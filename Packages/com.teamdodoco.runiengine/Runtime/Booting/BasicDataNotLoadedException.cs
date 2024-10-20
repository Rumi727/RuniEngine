using RuniEngine.Resource.Texts;
using System;

namespace RuniEngine.Booting
{
    public class BasicDataNotLoadedException : Exception
    {
        public static void Exception()
        {
            if (!BootLoader.isDataLoaded)
                throw new BasicDataNotLoadedException();
        }

        public BasicDataNotLoadedException() : base(LanguageLoader.TryGetText("exception.basic_data_not_loaded"))
        {

        }
    }
}
