﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces;

public interface IToolboxItem
{
    Type PlacementToolType { get; set; }
    Type Type { get; set; }
}