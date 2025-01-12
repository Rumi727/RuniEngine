#nullable enable
using RuniEngine.Resource.Texts;
using System;

namespace RuniEngine.Booting
{
    public class ResourceDataNotLoadedException : Exception
    {
        public static void Exception()
        {
            if (!BootLoader.isAllLoaded)
                throw new ResourceDataNotLoadedException();
        }

        public ResourceDataNotLoadedException() : base(LanguageLoader.GetText("exception.resource_not_loaded"))
        {

        }
    }
}
