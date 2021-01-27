using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Stock
{
    public class EAN : BaseEntity
    {
        public UInt64 LastCode { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        private const UInt64 _LastNumber = 9999999999999;

        public IEnumerable<string> Generate(UInt64 quantity)
        {
            if (!CanGenereate(quantity))
                throw new ArgumentOutOfRangeException($"Cant generate {quantity} EAN Codes.");
            var codes = Enumerable.Range((int)LastCode + 1, (int)quantity);
            LastCode = Convert.ToUInt32(codes.Last());
            return codes.Select(it => it.ToString().PadLeft(13, '0'));
        }

        public bool CanGenereate(UInt64 quantity)
        {
            return HowManyNumbersAreLeft() >= quantity;
        }

        public UInt64 HowManyNumbersAreLeft()
        {
            return _LastNumber - LastCode;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
