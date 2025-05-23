﻿using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace ViolentNight.Systems.Data.DataFileTypes;

/// <summary>
/// A struct generated from faction data files.
/// </summary>
/// <param name="identifier">A string identifier for the faction type.</param>
/// <param name="members">NPC IDs of all members. In the data file, these are encoded as strings - ID name (vanilla) or type name (modded).</param>
/// <param name="enemies">Faction identifiers of all enemy factions.</param>
public struct FactionData(string identifier, int[] members, string[] enemies)
{
    public string Identifier = identifier;
    public int[] Members = members;
    public string[] EnemyFactions = enemies;
}

public sealed class FactionDataManager : IDataManager<FactionData>
{
    public string Extension => ".faction.hjson";

    public void Populate(ReadOnlySpan<JObject> inputs, Span<FactionData> outputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            JObject json = inputs[i];

            FactionData definition = new();

            foreach (var rootPair in json)
            {
                if (rootPair is not { Key: string factionId, Value: JObject setJson })
                {
                    continue;
                }

                definition.Identifier = factionId;

                foreach (var property in setJson.Properties())
                {
                    if (property.Value is not JArray jsonArray)
                    {
                        continue;
                    }

                    switch (property.Name)
                    {
                        case "Members":
                            definition.Members = jsonArray.Values<string>().Select(ViolentNightUtils.StringToNpcId).ToArray();
                            break;
                        case "Enemies":
                            definition.EnemyFactions = jsonArray.Values<string>().ToArray();
                            break;
                    }
                }
            }

            outputs[i] = definition;
        }
    }
}
