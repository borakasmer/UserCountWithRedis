namespace RedisSqlService
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserCount")]
    public partial class UserCount
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string ServerName { get; set; }

        [Column("UserCount")]
        public int? UserCount1 { get; set; }

        public DateTime? DateTime { get; set; }
    }
}
