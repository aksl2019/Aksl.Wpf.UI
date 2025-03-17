using System;
using System.ComponentModel;

namespace Aksl.Modules.Pipeline
{
    public enum FanStatus
    {
        [Description("转动")]
        Turn,
        [Description("暂停")]
        Pause
    }
}
