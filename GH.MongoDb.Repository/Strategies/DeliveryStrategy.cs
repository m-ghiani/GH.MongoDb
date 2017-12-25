using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GH.MongoDb.Repository.Options;

namespace GH.MongoDb.Repository.Strategies
{
    public class DeliveryStrategy
    {
        public List<string> IncludedFields { get; set; } = new List<string>();
        public bool IncludeId { get; set; } = true;
        public PagingSettings PagingSettings { get; set; }
        public List<SortingField> SortingSettings { get; set; }
        public string ProjectionString
        {
            get
            {

                var sb = new StringBuilder("");
                if (!IncludedFields.Any()) return sb.ToString();
                sb.Append("{");
                if (!IncludeId) sb.Append("_id: 0,");
                for (var i = 0; i < IncludedFields.Count(); i++)
                {
                    sb.Append(IncludedFields.ElementAt(i));
                    sb.Append(": 1");
                    if (i < IncludedFields.Count() - 1) sb.Append(",");
                }
                sb.Append("}");
                return sb.ToString();
            }
        }

        public static DeliveryStrategy DefaultDeliveryStrategy()
        {
            return DefaultDeliveryStrategy(100);
        }

        public static DeliveryStrategy DefaultDeliveryStrategy(int limit)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));
            return new DeliveryStrategy { PagingSettings = new PagingSettings(0, limit) };
        }
    }
}
