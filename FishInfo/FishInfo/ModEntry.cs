﻿using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FishInfo
{
    public class ModEntry : StardewModdingAPI.Mod
    {
        internal static Dictionary<int, FishData> FishInfo = new Dictionary<int, FishData>();

        internal static IModHelper helper;

        public override void Entry(IModHelper helper)
        {
            ModEntry.helper = helper;
            Patches.DoPatches();

            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            
            if (Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            {
                object api = Helper.ModRegistry.GetApi("spacechase0.JsonAssets");
                EventInfo EventIdsAssigned = api.GetType().GetEvent("IdsAssigned");

                Delegate d = Delegate.CreateDelegate(EventIdsAssigned.EventHandlerType, this, "LoadData");

                EventIdsAssigned.AddEventHandler(api, d);
            }
            else
            {
                Helper.Events.GameLoop.SaveLoaded += LoadData;
            }
        }

        private static FishData GetOrCreateData(int fishID)
        {
            if (FishInfo.TryGetValue(fishID, out FishData data))
            {
                return data;
            }
            else
            {
                FishInfo.Add(fishID, new FishData());
                return GetOrCreateData(fishID); //lol recursion
            }
        }

        public void LoadData(object sender, EventArgs e)
        {
            FishInfo.Clear();
            Dictionary<string, string> LocationData = helper.Content.Load<Dictionary<string, string>>("Data\\Locations", ContentSource.GameContent);
            foreach (KeyValuePair<string, string> locdata in LocationData)
            {
                string[] data = locdata.Value.Split('/');

                string locationName = locdata.Key;

                if (locationName == "fishingGame" || locationName == "Temp") continue; //don't want these - what the fuck even is temp
                if (locationName == "BugLand") locationName = "MutantBugLair"; //fucking bugland lmao
                
                string[] seasonData;
                for (int i = 4; i <= 7; i++)
                {
                    if (data[i] == "-1")
                    {
                        continue;
                    }

                    seasonData = data[i].Split(' ');

                    for (int fish = 0; fish < seasonData.Length; fish += 2)
                    {

                        
                        int FishID = int.Parse(seasonData[fish]);
                        FishData fd = GetOrCreateData(FishID);

                        if (seasonData[fish + 1] == "0")
                        {
                            locationName = "ForestRiver";
                        }
                        else if (seasonData[fish + 1] == "1")
                        {
                            locationName = "ForestPond";
                        }
                        
                        //locationName = Regex.Replace(locationName, "  ", " ");

                        fd.AddLocation(locationName);
                        fd.AddSeason((Season)(1 << (i - 4)));
                    }
                }

            }

            Dictionary<int, string> FishData = helper.Content.Load<Dictionary<int, string>>("Data\\Fish", ContentSource.GameContent);
            foreach (KeyValuePair<int, string> fishData in FishData)
            {
                int FishID = fishData.Key;
                string[] fishInfo = fishData.Value.Split('/');
                FishData fd = GetOrCreateData(FishID);

                if (fishInfo[1] == "trap") //crabpot
                {
                    fd.IsCrabPot = true;
                    fd.AddLocation(fishInfo[4]);
                }
                else
                {
                    string[] times = fishInfo[5].Split(' ');

                    for (int time = 0; time < times.Length; time += 2)
                    {
                        fd.AddTimes(int.Parse(times[time]), int.Parse(times[time + 1]));
                    }

                    if (fishInfo[7] == "sunny" || fishInfo[7] == "both")
                    {
                        fd.AddWeather(Weather.Sun);
                    }
                    if (fishInfo[7] == "rainy" || fishInfo[7] == "both")
                    {
                        fd.AddWeather(Weather.Rain);
                    }
                }
            }
        }
    }
}