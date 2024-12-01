using System.Diagnostics;

using AudioSwitcher.CoreAudio;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Interfaces;
using Media.Ui.Controls;

namespace Media.Ui.Gui;

internal sealed partial class AudioViewModel : ObservableObject, IViewModel
{
    internal sealed class DeviceViewModel
    {
        public required bool IsDefault { get; init; }
        public required string Name { get; init; }
        public required Guid Guid { get; init; }
    }

    public ObservableRangeCollection<DeviceViewModel> Devices { get; }

    public AudioViewModel()
    {
        Devices = new ObservableRangeCollection<DeviceViewModel>();
    }

    private void RefreshdevicesCore(CoreAudioController controller)
    {
        var devices = controller.GetDevices()
            .Where(d => d.DeviceType == AudioSwitcher.AudioApi.DeviceType.Playback
                && d.State == AudioSwitcher.AudioApi.DeviceState.Active)
            .Select(d => new DeviceViewModel
            {
                IsDefault = d.IsDefaultDevice,
                Name = (string)d.FullName.Clone(),
                Guid = d.Id
            }).ToList();

        Devices.Clear();
        Devices.AddRange(devices);
    }

    [RelayCommand]
    private void Refreshdevices()
    {
        using (var controller = new CoreAudioController())
        {
            RefreshdevicesCore(controller);
        }
    }

    [RelayCommand]
    private void SetDefault(DeviceViewModel deviceViewModel)
    {
        using (var controller = new CoreAudioController())
        {
            var device = controller.GetDevice(deviceViewModel.Guid);
            device.SetAsDefault();
            RefreshdevicesCore(controller);
        }
    }

    [RelayCommand]
    private async Task Mute()
    {
        using (var controller = new CoreAudioController())
        {
            var device = controller.GetDevices()
            .Where(d => d.DeviceType == AudioSwitcher.AudioApi.DeviceType.Playback
                   && d.State == AudioSwitcher.AudioApi.DeviceState.Active
                   && d.IsDefaultDevice)
            .FirstOrDefault();

            if (device != null)
            {
                await device.ToggleMuteAsync();
            }
        }
    }

    [RelayCommand]
    private void StartVolMixer()
    {
        using var process = new Process();
        process.StartInfo.FileName = "SndVol.exe";
        process.StartInfo.UseShellExecute = true;
        process.Start();
    }

    public void Initialize()
    {
        Refreshdevices();
    }
}
