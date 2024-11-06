using System;

namespace AudioSwitcher.AudioApi
{
    /// <summary>
    ///     The ERole enumeration defines constants that indicate the role
    ///     that the system has assigned to an audio endpoint device
    /// </summary>
    [Flags]
    public enum Role
    {
        /// <summary>
        ///     Games, system notification sounds, and voice commands.
        /// </summary>
#pragma warning disable CA1008 // Enums should have zero value
        Console,
#pragma warning restore CA1008 // Enums should have zero value

        /// <summary>
        ///     Music, movies, narration, and live music recording
        /// </summary>
        Multimedia,

        /// <summary>
        ///     Voice communications (talking to another person).
        /// </summary>
        Communications
    }
}