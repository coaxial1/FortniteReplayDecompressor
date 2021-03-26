﻿using FortniteReplayReader;
using FortniteReplayReader.Extensions;
using FortniteReplayReader.Models;
using FortniteReplayReader.Models.NetFieldExports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unreal.Core;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;
using Unreal.Encryption;

namespace ConsoleReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(loggingBuilder => loggingBuilder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Warning));
            var provider = serviceCollection.BuildServiceProvider();
            var logger = provider.GetService<ILogger<Program>>();

            //var localAppDataFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
            ////var replayFilesFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");
            //var replayFilesFolder = @"D:\Projects\FortniteReplayCollection";
            //var replayFiles = Directory.EnumerateFiles(replayFilesFolder, "*.replay");

            //foreach (var replayFile in replayFiles)
            //{
            //    var reader = new ReplayReader(logger);
            //    var replay = reader.ReadReplay(replayFile);
            //}

            //var replayFile = "Replays/season12_arena.replay";
            //var replayFile = "Replays/season11.31.replay
            var replayFile = "Replays/vehicle.replay"; //Used for testing
            //var replayFile = "Replays/season11.11.replay"; //Used for testing
            //var replayFile = "Replays/Test.replay"; //Used for testing
            //var replayFile = "Replays/shoottest.replay"; 
            //var replayFile = "Replays/tournament2.replay";
            //var replayFile = "Replays/creative-season11.21.replay";
            //var replayFile = "Replays/creative.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.12.11-02.43.14.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.12.10-17.47.46.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.12.11-01.49.22.replay";
            //var replayFile = "Replays/123.replay";
            //var replayFile = "Replays/WCReplay.replay";
            //var replayFile = "Replays/00769AB3D5F45A5ED7B01553227A8A82E07CC592.replay";
            //var replayFile = "Replays/season12_arena.replay";
            //var replayFile = "Replays/1.replay";
            //var replayFile = "Replays/weapons2.replay";
            //var replayFile = "Replays/iceblocks2.replay";
            //var replayFile = "Replays/creativeShooting.replay";

            Stopwatch sw = new Stopwatch();

            double totalTime = 0;

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            int count = 0;

            List<double> times = new List<double>();

            var reader = new ReplayReader(logger, new FortniteReplaySettings
            {
                PlayerLocationType = LocationTypes.User
            });

            string demoPath = Path.Combine(appData, "FortniteGame", "Saved", "Demos");

            foreach (string path in Directory.GetFiles("Replays"))
            {
                for (int i = 0; i < 5; i++)
                {
                    ++count;

                    sw.Restart();
                    var replay = reader.ReadReplay(replayFile, ParseType.Full);

                    sw.Stop();

                    var aff = replay.GameInformation.Players.OrderByDescending(x => x.Locations.Count);

                    var player = replay.GameInformation.Players.FirstOrDefault(x => x.IsPlayersReplay);

                    for(int z = 1; z < player.Locations.Count; z++)
                    {
                        PlayerLocationRepMovement lastLocation = player.Locations[z - 1];
                        PlayerLocationRepMovement currentLocation = player.Locations[z];

                        if(currentLocation.DeltaGameTimeSeconds - lastLocation.DeltaGameTimeSeconds > 1)
                        {
                            Console.WriteLine($"{currentLocation.Location} - {lastLocation.DeltaGameTimeSeconds} - {currentLocation.DeltaGameTimeSeconds - lastLocation.DeltaGameTimeSeconds}s");
                        }
                    }

                    var aaa = replay.GameInformation.Players.SelectMany(y => y.Shots);

                    Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds}ms. Total Groups Read: {reader?.TotalGroupsRead}. Failed Bunches: {reader?.TotalFailedBunches}. Failed Replicator: {reader?.TotalFailedReplicatorReceives} Null Exports: {reader?.NullHandles} Property Errors: {reader?.PropertyError} Failed Property Reads: {reader?.FailedToRead}");

                    var killFeed = replay.GameInformation.KillFeed.Where(x => x.FinisherOrDowner?.IsPlayersReplay == true).ToList();

                    var bots = replay.GameInformation.Players.Where(x => !String.IsNullOrEmpty(x.BotId)).ToList();
                    var nonBots = replay.GameInformation.Players.Where(x => !x.IsBot).ToList();
                    var asdfa = replay.GameInformation.Teams.OrderByDescending(x => x.Players.Count);

                    //Console.WriteLine($"\t - Properties Read: {reader.TotalPropertiesRead}");

                    //Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds}ms. Total Llamas: {reader.GameInformation.Llamas.Count}. Unknown Fields: {NetFieldParser.UnknownNetFields.Count}");

                    totalTime += sw.Elapsed.TotalMilliseconds;
                    times.Add(sw.Elapsed.TotalMilliseconds);

                    /*
                    foreach (Llama llama in replay.GameInformation.Llamas)
                    {
                        //Console.WriteLine($"\t -{llama}");
                    }


                    var c = replay.GameInformation.Players.Where(x => x.IsPlayersReplay);
                    var d = replay.GameInformation.Teams.OrderByDescending(x => x.Players.Count);
                    //var a = NetFieldParser.UnknownNetFields;
                    var kills = d.FirstOrDefault().Players.Sum(x => x.TotalKills);

                    var asdfas = replay.GameInformation.Teams.OrderByDescending(x => x.Players.Count);
                    var totalBots = replay.GameInformation.Players.Count(x => x.IsBot);*/
                }
            }

            var fastest5 = times.OrderBy(x => x).Take(10);



            Console.WriteLine($"Total Time: {totalTime}ms. Average: {((double)totalTime / count):0.00}ms");
            Console.WriteLine($"Fastest 10 Time: {fastest5.Sum()}ms. Average: {(fastest5.Sum() / fastest5.Count()):0.00}ms");

            Console.WriteLine("---- done ----");
            //Console.WriteLine($"Total Errors: {reader?.TotalErrors}");
            Console.ReadLine();
        }
    }
}
