using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace Perceptron
{
    static class NetworkSerializer
    {
        class NetworkModel
        {
            public float LearningCoefficient { get; set; }
            public List<float> Biases { get; set; }
            public List<List<float>> Weights { get; set; }
        }

        static NetworkModel CreateNetworkModel(Network.Network network)
        {
            var weights = new List<List<float>>();
            for (int i = 0; i < network.Weights.GetLength(0); i++)
            {
                var floats = new List<float>();
                for (int j = 0; j < network.Weights.GetLength(1); j++)
                {
                    floats.Add(network.Weights[i, j]);
                }
                weights.Add(floats);
            }
            var model = new NetworkModel()
            {
                LearningCoefficient = network.LearningCoeficient,
                Weights = weights,
                Biases = network.Biases.ToList()
            };
            return model;
        }

        static Network.Network CreateNetwork(NetworkModel model)
        {
            var network = new Network.Network();
            network.LearningCoeficient = model.LearningCoefficient;
            network.Neurons = model.Biases.Count;
            var weights = new float[model.Weights.Count, model.Weights[0].Count];
            for (int i = 0; i < model.Weights.Count; i++)
            {
                var floats = new List<float>();
                for (int j = 0; j < model.Weights[0].Count; j++)
                {
                    weights[i, j] = model.Weights[i][j];
                }
            }
            network.Weights = weights;
            network.Biases = model.Biases.ToArray();
            network.InputLayer = new Network.InputLayer(model.Weights.Count);
            bool ok = network.Run();
            network.Stop();
            if (ok)
                return network;
            else
                return null;
        }

        static string Serialize(Network.Network network)
        {
            var model = CreateNetworkModel(network);
            return JsonSerializer.Serialize(model);
        }

        static Network.Network Deserialize(string json)
        {
            try
            {
                var model = JsonSerializer.Deserialize<NetworkModel>(json);
                return CreateNetwork(model);
            }
            catch(Exception e)
            {
                MessageBox.Show("Corrupted data.", "Data error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static void Save(Network.Network network)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "perceptron"; // Default file name
            dlg.DefaultExt = ".perc"; // Default file extension
            dlg.Filter = "Json documents (.perc)|*.perc"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                string json = Serialize(network);
                try
                {
                    File.WriteAllText(filename, json);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to save file, try again.", "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static Network.Network Load()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".perc";
            openFileDialog.Filter = "Json documents (.perc)|*.perc";
            if (openFileDialog.ShowDialog() == true)
                return Deserialize(File.ReadAllText(openFileDialog.FileName));
            return null;
        }
    }
}
