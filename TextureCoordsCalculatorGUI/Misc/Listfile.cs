using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using System.IO;
using System.Collections.Frozen;

namespace TextureCoordsCalculatorGUI.Misc
{
    internal readonly record struct ListfileRecord(
        [property: Index(0)] uint FileDataId, 
        [property: Index(1)] string FilePath);
    internal sealed class Listfile
    {
        private static readonly Lazy<Listfile> _instance = new(() => new Listfile());
        public static Listfile Instance => _instance.Value;

        private readonly Dictionary<string, uint> _listFile = [];
        private bool _isLoaded;

        public void Initialize(Stream listfileStream)
        {
            if (_isLoaded)
                return;

            using var reader = new StreamReader(listfileStream);
            using var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = ";"
            });

            while (csvReader.Read())
            {
                var record = csvReader.GetRecord<ListfileRecord>();

                if (record.FilePath.EndsWith(".blp") && record.FilePath.StartsWith("interface/"))
                {
                    _listFile.TryAdd(record.FilePath, record.FileDataId);
                }
            }

            _isLoaded = true;
        }


        public uint GetFileDataId(string filePath)
        {
            _ = _listFile.TryGetValue(filePath, out var fileDataId);
            return fileDataId;
        }

        public FrozenDictionary<string, uint> Textures => _listFile.ToFrozenDictionary();

    }
}
