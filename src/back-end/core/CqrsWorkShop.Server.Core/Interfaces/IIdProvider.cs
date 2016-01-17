using System;

namespace Hse.CqrsWorkShop
{
    public interface IIdProvider<out TId> where TId : IEquatable<TId>
    {
        TId GenerateId();
    }

    public interface ILongIdProvider : IIdProvider<long>
    {
    }
}