using System;
using System.Collections.Generic;
using System.IO;

namespace MCForge
{
    public class GrpCommands
    {
        public static List<RankAllowance> allowedCommands;
        public static List<string> foundCommands = new List<string>();

        public static LevelPermission defaultRanks(string command)
        {
            Command cmd = Command.all.Find(command);

            if (cmd != null) return cmd.defaultRank;
            else return LevelPermission.Null;
        }

        public static void fillRanks()
        {
            foundCommands = Command.all.commandNames();
            allowedCommands = new List<RankAllowance>();

            RankAllowance allowVar;

            foreach (Command cmd in Command.all.All())
            {
                allowVar = new RankAllowance();
                allowVar.commandName = cmd.name;
                allowVar.lowestRank = cmd.defaultRank;
                allowedCommands.Add(allowVar);
            }

            if (File.Exists("properties/command.properties"))
            {
                string[] lines = File.ReadAllLines("properties/command.properties");

                if (lines.Length == 0) ;
                else if (lines[0] == "#Version 2")
                {
                    string[] colon = new string[] { " : " };
                    foreach (string line in lines)
                    {
                        allowVar = new RankAllowance();
                        if (line != "" && line[0] != '#')
                        {
                            //Name : Lowest : Disallow : Allow
                            string[] command = line.Split(colon, StringSplitOptions.None);

                            if (!foundCommands.Contains(command[0]))
                            {
                                Server.s.Log("Incorrect command name: " + command[0]);
                                continue;
                            }
                            allowVar.commandName = command[0];

                            string[] disallow = new string[0];
                            if (command[2] != "")
                                disallow = command[2].Split(',');
                            string[] allow = new string[0];
                            if (command[3] != "")
                                allow = command[3].Split(',');

                            try
                            {
                                allowVar.lowestRank = (LevelPermission)int.Parse(command[1]);
                                foreach (string s in disallow) { allowVar.disallow.Add((LevelPermission)int.Parse(s)); }
                                foreach (string s in allow) { allowVar.allow.Add((LevelPermission)int.Parse(s)); }
                            }
                            catch
                            {
                                Server.s.Log("Hit an error on the command " + line);
                                continue;
                            }

                            int current = 0;
                            foreach (RankAllowance aV in allowedCommands)
                            {
                                if (command[0] == aV.commandName)
                                {
                                    allowedCommands[current] = allowVar;
                                    break;
                                }
                                current++;
                            }
                        }
                    }
                }
                else
                {
                    foreach (string line in lines)
                    {
                        if (line != "" && line[0] != '#')
                        {
                            allowVar = new RankAllowance();
                            string key = line.Split('=')[0].Trim().ToLower();
                            string value = line.Split('=')[1].Trim().ToLower();

                            if (!foundCommands.Contains(key))
                            {
                                Server.s.Log("Incorrect command name: " + key);
                            }
                            else if (Level.PermissionFromName(value) == LevelPermission.Null)
                            {
                                Server.s.Log("Incorrect value given for " + key + ", using default value.");
                            }
                            else
                            {
                                allowVar.commandName = key;
                                allowVar.lowestRank = Level.PermissionFromName(value);

                                int current = 0;
                                foreach (RankAllowance aV in allowedCommands)
                                {
                                    if (key == aV.commandName)
                                    {
                                        allowedCommands[current] = allowVar;
                                        break;
                                    }
                                    current++;
                                }
                            }
                        }
                    }
                }
                Save(allowedCommands);
            }
            else Save(allowedCommands);

            foreach (Group grp in Group.GroupList)
            {
                grp.fillCommands();
            }
        }

        public static void Save(List<RankAllowance> givenList)
        {
            try
            {
                StreamWriter w = new StreamWriter(File.Create("properties/command.properties"));
                w.WriteLine("#Version 2");
                w.WriteLine("#   This file contains a reference to every command found in the server software");
                w.WriteLine("#   Use this file to specify which ranks get which commands");
                w.WriteLine("#   Current ranks: " + Group.concatList(false, false, true));
                w.WriteLine("#   Disallow and allow can be left empty, just make sure there's 2 spaces between the colons");
                w.WriteLine("#   This works entirely on permission values, not names. Do not enter a rank name. Use it's permission value");
                w.WriteLine("#   CommandName : LowestRank : Disallow : Allow");
                w.WriteLine("#   gun : 60 : 80,67 : 40,41,55");
                w.WriteLine("");
                foreach (RankAllowance aV in givenList)
                {
                    w.WriteLine(aV.commandName + " : " + (int)aV.lowestRank + " : " + getInts(aV.disallow) + " : " + getInts(aV.allow));
                }
                w.Flush();
                w.Close();
            }
            catch
            {
                Server.s.Log("SAVE FAILED! command.properties");
            }
        }
        public static string getInts(List<LevelPermission> givenList)
        {
            string returnString = ""; bool foundOne = false;
            foreach (LevelPermission Perm in givenList)
            {
                foundOne = true;
                returnString += "," + (int)Perm;
            }
            if (foundOne) returnString = returnString.Remove(0, 1);
            return returnString;
        }
        public static void AddCommands(out CommandList commands, LevelPermission perm)
        {
            commands = new CommandList();

            foreach (RankAllowance aV in allowedCommands)
                if ((aV.lowestRank <= perm && !aV.disallow.Contains(perm)) || aV.allow.Contains(perm)) commands.Add(Command.all.Find(aV.commandName));
        }
    }

}
