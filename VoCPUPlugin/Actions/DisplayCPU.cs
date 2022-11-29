namespace Loupedeck.VoCPUPlugin.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DisplayCPU: PluginDynamicCommand{
        private readonly System.Timers.Timer _timer;
        private readonly PerformanceCounter _perfCounter = new System.Diagnostics.PerformanceCounter();
        private readonly Queue<Double> _cpuavg = new Queue<Double>();
        private readonly BitmapImage _background;

        public DisplayCPU()
        : base(displayName: "Display CPU %", description: "Displays the CPU utilization.", groupName: "")
        {
            this._background = BitmapImage.FromFile("assets\\CPU-xxsmallc.png");
            this._timer = new System.Timers.Timer(1000);
            this._timer.Elapsed += (sender, e) =>
            {
                this.ActionImageChanged();
            };
            this._timer.AutoReset = true;
            this._timer.Start();
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            this._perfCounter.CategoryName = "Processor";
            this._perfCounter.CounterName = "% Processor Time";
            this._perfCounter.InstanceName = "_Total";
            var cpuval = this._perfCounter.NextValue();
            Thread.Sleep(100);

            this._cpuavg.Enqueue(cpuval);
            if (this._cpuavg.Count < 10)
            {
                return "CPU\n\n" + cpuval.ToString("F2") + "%";
            }
            else
            {
                this._cpuavg.Dequeue();
                return "CPU\n\n" + (_cpuavg.Sum(x => x) / 10).ToString("F2") + "%";
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var bitmap = new BitmapBuilder(imageSize);
            bitmap.FillRectangle(0, 0, 90, 90, BitmapColor.Black);
            bitmap.DrawImage(this._background, 0, 0);
            bitmap.DrawText(this.GetCommandDisplayName(actionParameter, imageSize), BitmapColor.White, 16);
            return bitmap.ToImage();
        }
    }
}
