using System;
using System.Collections.Generic;
using System.Text;

namespace eazy.logger.EfCore.Entity
{
    public class LogModel
    {
        public virtual int RowNo { get; set; }

        public virtual string Level { get; set; }

        public virtual string Message { get; set; }

        public virtual DateTime Timestamp { get; set; }

        public virtual string Exception { get; set; }

        public virtual string Properties { get; set; }

        public virtual string PropertyType { get; set; }
    }

    
    internal abstract class SqlServerLogModel : LogModel
    {
        public override string PropertyType => "xml";
    }
}
