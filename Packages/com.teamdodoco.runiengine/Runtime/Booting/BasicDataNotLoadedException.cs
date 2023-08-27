#nullable enable
using RuniEngine.Resource;
using System;

namespace RuniEngine.Booting
{
    public class BasicDataNotLoadedException : Exception
    {
        public static void Exception()
        {
            if (!BootLoader.basicDataLoaded)
                throw new BasicDataNotLoadedException();
        }

        public BasicDataNotLoadedException() : base(LanguageLoader.TryGetText("exception.basic_data_not_loaded"))
        {

        }
    }
}
