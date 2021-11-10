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
        public bool mirror { get; init; }
        public bool flip { get; init; }
        public bool rotation90 { get; init; }
        public double zoom { get; init; }
        public double tilt { get; init; }
        public double shift { get; init; }
        public double rotation { get; init; }
        public double contrast { get; init; }
        public double brightness { get; init; }
        public double smoothFiltering { get; init; }
        public double noise { get; init; }
        public double colorNoise { get; init; }
        public double partialFocus { get; init; }
        public double shade { get; init; }
        public double hue { get; init; }
        public double saturation { get; init; }
        public int maxRandomAugmentCount { get; init; }
        public double probability { get; init; }
        public int borderMode { get; init; }

        public Augmentation() { }
    }
}
