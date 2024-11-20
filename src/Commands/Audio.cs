using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AudioSwitcher.CoreAudio;

namespace Media.Commands;
internal class Audio : Command
{
    public override int Execute(CommandContext context)
    {
        using var controller = new CoreAudioController();
        var devices = controller.GetDevices().Where(d => d.DeviceType == AudioSwitcher.AudioApi.DeviceType.Playback);

        foreach (var device in devices)
        {
            var chr = device.IsDefaultDevice ? '*' : ' ';
            Console.WriteLine($"{chr}{device.FullName}");
            
        }

        return 0;
    }
}
