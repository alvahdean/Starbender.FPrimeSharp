using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using Volo.Abp.DependencyInjection;

namespace Starbender.FPrimeSharp.Gds.Topology;

public class TopologyBuilder : ISingletonDependency
{
    
    public TopologyValidationResult Validate(TopologyDictionary _)
    {
        var result = new TopologyValidationResult()
        {
            Path = "/"
        };

        throw new NotImplementedException();
    }

    public TopologyDictionary FromJson(string json)
    {
        var topology = JsonSerializer.Deserialize<TopologyDictionary>(json)
            ?? throw new Exception("Topology is empty");

        return topology;
    }

    public string ToJson(TopologyDictionary topology)
    {
        var json = JsonSerializer.Serialize(topology);

        return json;
    }

    private TopologyValidationResult Validate(TelemetryPacketDefinition p, string parentPath)
    {
        var result = new TopologyValidationResult()
        {
            Path = $"{parentPath}:TelemetryPackets:{p.Name}"
        };

        if (p.Apid > 0x7FF)
        {
            result.IsValid = false;
            result.Messages.Add(new TopologyValidationMessage()
            {
                Severity = TopologyValidationSeverity.Error,
                Message = $"Invalid CCSDS APID:{p.Apid}"
            });
        }

        if (p.Channels.Count == 0)
        {
            result.IsValid = false;
            result.Messages.Add(new TopologyValidationMessage()
            {
                Severity = TopologyValidationSeverity.Error,
                Message = $"No telemetry channels found"
            });
        }

        result.IsValid ??= true;

        return result;
    }
}
