using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pegasus
{
    public interface IWorker
    {
        /// <summary>
        /// 启用工作
        /// </summary>
        void Start();

        /// <summary>
        /// 是否等待
        /// </summary>
        bool IsWait();

        /// <summary>
        /// 执行过程
        /// </summary>
        void Process(Func<string, dynamic, bool> filter = null);
    }
}
