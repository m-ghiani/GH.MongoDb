using System.Data.SqlTypes;
using System.Text;

namespace GH.MongoDb.Repository.Options
{
    public class PagingSettings : INullable
    {
        public PagingSettings()
        {

        }

        public PagingSettings(int? skip, int? limit)
        {
            this.Skip = skip;
            this.Limit = limit;
        }
        public int? Skip { get; set; }
        public int? Limit { get; set; }
        public bool IsNull => Skip == null && Limit == null;
    }
}
