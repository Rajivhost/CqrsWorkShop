using System;

namespace Hse.CqrsWorkShop.Domain
{
    public static class GuidIdGenerator
    {
        private static IGuidIdProvider _idProvider;

        public static void SetProvider(IGuidIdProvider idProvider)
        {
            _idProvider = idProvider;
        }

        public static Guid GetNextId()
        {
            return _idProvider.GenerateId();
        }

    }
}