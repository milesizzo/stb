﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats.Templates
{
    public interface IGameObjectTemplate : ITemplate { }

    public class GameObjectTemplateStore : TemplateStore<IGameObjectTemplate>
    {
        //
    }
}
