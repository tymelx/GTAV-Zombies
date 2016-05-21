using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zombies.Core.Interfaces
{
    public interface IGameMode
    {
        void Start(bool notify = true);
        void Stop(bool notify = true);
        void ProcessTick();
    }
}
