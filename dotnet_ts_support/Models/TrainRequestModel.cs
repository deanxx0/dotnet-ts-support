using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_ts_support.Models
{
    public class TrainRequestModel
    {
        public string target_type { get; init; }
        public string[] directories { get; init; }
        public Train_params train_params { get; init; }
        public Dictionary<string, string> class_list { get; init; }
        
        public TrainRequestModel() { }
    }

    public class Train_params
    {
        public int gpu_id { get; init; }
        public int iterations { get; init; }
        public Network network { get; init; }
        public Patchmode patchmode { get; init; }
        public Roi roi { get; init; }
        public Solver_param solver_param { get; init; }
        public Augmentation augmentation { get; init; }

        public Train_params() { }
    }

    public class Network
    {
        public int batch_size { get; init; }
        public string pretrain_data { get; init; }
        public int width { get; init; }
        public int heigth { get; init; }
        public int channels { get; init; }

        public Network() { }
    }

    public class Patchmode
    {
        public int enabled { get; init; }
        public int width { get; init; }
        public int height { get; init; }

        public Patchmode() { }
    }

    public class Roi
    {
        public int enabled { get; init; }
        public int x { get; init; }
        public int y { get; init; }
        public int width { get; init; }
        public int height { get; init; }

        public Roi() { }
    }

    public class Solver_param
    {
        public double base_learning_rate { get; init; }
        public double gamma { get; init; }
        public int step_count { get; init; }

        public Solver_param() { }
    }

    public class Augmentation
    {
        public bool Mirror { get; init; }
        public bool Flip { get; init; }
        public bool Rotation90 { get; init; }
        public double Zoom { get; init; }
        public double Tilt { get; init; }
        public double Shift { get; init; }
        public double Rotation { get; init; }
        public double Contrast { get; init; }
        public double Brightness { get; init; }
        public double SmoothFiltering { get; init; }
        public double Noise { get; init; }
        public double ColorNoise { get; init; }
        public double PartialFocus { get; init; }
        public double Shade { get; init; }
        public double Hue { get; init; }
        public double Saturation { get; init; }
        public int MaxRandomAugmentCount { get; init; }
        public double Probability { get; init; }
        public int BorderMode { get; init; }

        public Augmentation() { }
    }
}
