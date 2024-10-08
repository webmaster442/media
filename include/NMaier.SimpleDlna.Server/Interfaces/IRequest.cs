﻿using System.Net;

namespace NMaier.SimpleDlna.Server.Interfaces;

public interface IRequest
{
    string? Body { get; }

    IHeaders Headers { get; }

    IPEndPoint LocalEndPoint { get; }

    string Method { get; }

    string Path { get; }

    IPEndPoint RemoteEndpoint { get; }
}