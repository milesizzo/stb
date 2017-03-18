using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine
{
    public interface IFixture
    {
        PhysicsShape Shape { get; }

        IBody Body { get; set; }
    }
}
