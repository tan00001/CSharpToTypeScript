using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestAssembly
{
    [DataContract(Namespace = "TestAssembly.Enums")]
    public enum RegistrationStatus
    {
        [EnumMember(Value = "Unknown")]
        Unknown,
        Registered,
        Unregistered,
        [Display(Name = "Approval Pending")]
        ApprovalPending
    }

}
