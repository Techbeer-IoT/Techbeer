using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Util {
    public interface ILog {
        void LogInfo(string message);
        void LogError(string message);
        void LogWarning(string message);
    }
}
