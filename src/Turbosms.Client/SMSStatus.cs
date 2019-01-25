using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurboSMSua
{
    public enum SMSStatus
    {
        NULL,
        ACCEPTD,
        ENROUTE,
        DELIVRD,
        EXPIRED,
        DELETED,
        UNDELIV,
        REJECTD,
        UNKNOWN
    }
}
