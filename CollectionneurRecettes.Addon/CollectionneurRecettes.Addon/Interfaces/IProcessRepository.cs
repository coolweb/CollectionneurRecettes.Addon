﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Interfaces
{
    public interface IProcessRepository
    {
        Process RunningProcess(string name);
    }
}
