using System;

namespace Hse.CqrsWorkShop.Domain
{
    public class LongIdProvider : ILongIdProvider
    {
        public static readonly long TimestampMask = 0x1FFFFFFFFFF;
        public static readonly int TimestampShift = 22;

        public static readonly long ScopeMask = 0x3FF;
        public static readonly int ScopeShift = 12;

        public static readonly long SequenceMask = 0xFFF;
        public static readonly int SequenceShift = 0;

        private long _lastCounter;
        private long _currentSequence;

        private long _scopeBits;

        private readonly object _lockObject = new object();
        private readonly DateTime _commonEpoch = new DateTime(2016, 1, 17, 0, 0, 0, 0, DateTimeKind.Utc);

        public LongIdProvider() : this(0) { }
        public LongIdProvider(int scope)
        {
            SetScope(scope);
        }

        public long GenerateId()
        {
            var counter = CalculateCounter();
            lock (_lockObject)
            {
                return CalculateId(counter);
            }
        }

        public void SetScope(int scope)
        {
            long idScope = scope;
            _scopeBits = (idScope & ScopeMask) << ScopeShift;
        }

        public void Dispose() { }

        private long CalculateId(long currentCounter)
        {
            if (currentCounter > _lastCounter)
            {
                _lastCounter = currentCounter;
                _currentSequence = 0;
            }
            else
            {
                _currentSequence = (_currentSequence + 1) & SequenceMask;
                if (_currentSequence == 0)
                {
                    _lastCounter++;  // rolled over, add 1 millisecond
                }
            }

            return (_lastCounter << TimestampShift) |
                   (_scopeBits) |
                   (_currentSequence << SequenceShift);
        }
        private long CalculateCounter()
        {
            return (long)(DateTime.UtcNow - _commonEpoch).TotalMilliseconds & TimestampMask;
        }

        
    }
}