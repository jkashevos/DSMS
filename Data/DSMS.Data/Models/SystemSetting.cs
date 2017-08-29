using System;

namespace DSMS.Data.Models
{
    [Serializable]
    public class SystemSetting : BaseEntity
    {
        public SystemSetting(){}
        public SystemSetting(string Name, string Value = null) : this()
        {
            this.Name = Name;
            if (!string.IsNullOrEmpty(Value))
                this.Value = Value;
        }
        
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Value { get; set; }
        
    }
}