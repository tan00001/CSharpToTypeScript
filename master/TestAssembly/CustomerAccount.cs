using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly
{
    [DataContract]
    public class CustomerAccount
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public decimal? Balance { get; set; }
    }
}
