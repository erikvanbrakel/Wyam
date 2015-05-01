﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wyam.Extensibility;

namespace Wyam.Core
{
    public interface IPipelineCollection
    {
        IPipeline Add(params IModule[] modules);
    }
}