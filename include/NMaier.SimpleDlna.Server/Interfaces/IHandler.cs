﻿namespace NMaier.SimpleDlna.Server.Interfaces;

internal interface IHandler
{
    IResponse HandleRequest(IRequest request);
}