﻿using ProjectCardboardBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ICommandHandler
{
    void ReceiveCommand(List<Command> commands);
    void ReceiveChips(List<Chip> chips);
}