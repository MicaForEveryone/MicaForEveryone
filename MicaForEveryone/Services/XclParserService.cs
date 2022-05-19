using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using XclParser;
using XclParser.Reflection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;

namespace MicaForEveryone.Services
{
    internal class XclParserService : IConfigParser
    {
        private Context _xclContext = new();
        private XclDocument _configDocument;

        public IRule[] Rules { get; private set; }

        public XclParserService()
        {
            _xclContext.TypeMap.RegisterEnum(typeof(TitlebarColorMode));
            _xclContext.TypeMap.RegisterEnum(typeof(BackdropType));
            //_xclContext.TypeMap.RegisterByAttribute();
            _xclContext.TypeMap.RegisterClass(typeof(GlobalRule));
            _xclContext.TypeMap.RegisterClass(typeof(ProcessRule));
            _xclContext.TypeMap.RegisterClass(typeof(ClassRule));
        }

        public async Task LoadAsync(StreamReader reader)
        {
            _configDocument = await _xclContext.ParseDocumentAsync(reader);
            Rules = _configDocument.Instances.Select(instance => (IRule)instance.Value).ToArray();
        }

        public async Task SaveAsync(StreamWriter writer)
        {
            await _configDocument.SaveAsync(writer);
        }

        public void AddRule(IRule rule)
        {
            if (_configDocument == null)
                throw new Exception("Config document not loaded.");

            if (_configDocument.Instances.Any(section => section.Name == rule.Name))
                throw new Exception("Rule already exists.");

            var xclClass = _xclContext.TypeMap.GetXclType(rule.GetType()) as XclClass;
            var instance = xclClass.ToXclInstance(rule);

            _configDocument.Instances.Add(instance);

            Rules = _configDocument.Instances.Select(instance => (IRule)instance.Value).ToArray();
        }

        public void SetRule(IRule rule)
        {
            var target = _configDocument.Instances.FirstOrDefault(
                instance => instance.Value == rule);

            if (target == null)
                throw new Exception($"Rule {rule.Name} not found in config file.");

            target.UpdateData();
        }

        public void RemoveRule(IRule rule)
        {
            var instance = _configDocument.Instances.First(instance => instance.Value == rule);
            _configDocument.Instances.Remove(instance);

            Rules = _configDocument.Instances.Select(instance => (IRule)instance.Value).ToArray();
        }
    }
}
