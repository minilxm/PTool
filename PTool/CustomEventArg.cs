using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTool
{

    public class StartOrStopArgs : EventArgs
    {
        private bool m_IsStart = false;

        public bool IsStart
        {
            get { return m_IsStart; }
            set { m_IsStart = value; }
        }

        public StartOrStopArgs(bool isStart)
        {
            m_IsStart = isStart;
        }
    }
}
 
