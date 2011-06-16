/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.IO;

namespace MCForge
{
    public class Group
    {
        public string name;
        public string trueName;
        public string color;
        public LevelPermission Permission;
        public int maxBlocks;
        public CommandList commands;
        public string fileName;
        public PlayerList playerList;

        public Group()
        {
            Permission = LevelPermission.Null;
        }

        public Group(LevelPermission Perm, int maxB, string fullName, char newColor, string file) {
            Permission = Perm;
            maxBlocks = maxB;
            trueName = fullName;
            name = trueName.ToLower();
            color = "&" + newColor;
            fileName = file;
            if (name != "nobody")
                playerList = PlayerList.Load(fileName, this);
            else
                playerList = new PlayerList();
        }

        public void fillCommands()
        {
            CommandList _commands = new CommandList();
            GrpCommands.AddCommands(out _commands, Permission);
            commands = _commands;
        }

        public bool CanExecute(Command cmd) { return commands.Contains(cmd); }

        public static List<Group> GroupList = new List<Group>();
        public static Group standard;
        public static void InitAll()
        {
            GroupList = new List<Group>();

            if (File.Exists("properties/ranks.properties"))
            {
                string[] lines = File.ReadAllLines("properties/ranks.properties");

                Group thisGroup = new Group();
                int gots = 0;

                foreach (string s in lines)
                {
                    try
                    {
                        if (s != "" && s[0] != '#')
                        {
                            if (s.Split('=').Length == 2)
                            {
                                string property = s.Split('=')[0].Trim();
                                string value = s.Split('=')[1].Trim();

                                if (thisGroup.name == "" && property.ToLower() != "rankname")
                                {
                                    Server.s.Log("Hitting an error at " + s + " of ranks.properties");
                                }
                                else
                                {
                                    switch (property.ToLower())
                                    {
                                        case "rankname":
                                            gots = 0;
                                            thisGroup = new Group();

                                            if (value.ToLower() == "developers" || value.ToLower() == "devs")
                                                Server.s.Log("You are not a developer. Stop pretending you are.");
                                            else if (GroupList.Find(grp => grp.name == value.ToLower()) == null)
                                                thisGroup.trueName = value;
                                            else
                                                Server.s.Log("Cannot add the rank " + value + " twice");
                                            break;
                                        case "permission":
                                            int foundPermission;

                                            try
                                            {
                                                foundPermission = int.Parse(value);
                                            }
                                            catch { Server.s.Log("Invalid permission on " + s); break; }

                                            if (thisGroup.Permission != LevelPermission.Null)
                                            {
                                                Server.s.Log("Setting permission again on " + s);
                                                gots--;
                                            }

                                            bool allowed = true;
                                            if (GroupList.Find(grp => grp.Permission == (LevelPermission)foundPermission) != null)
                                                allowed = false;

                                            if (foundPermission > 119 || foundPermission < -50)
                                            {
                                                Server.s.Log("Permission must be between -50 and 119 for ranks");
                                                break;
                                            }

                                            if (allowed)
                                            {
                                                gots++;
                                                thisGroup.Permission = (LevelPermission)foundPermission;
                                            }
                                            else
                                            {
                                                Server.s.Log("Cannot have 2 ranks set at permission level " + value);
                                            }
                                            break;
                                        case "limit":
                                            int foundLimit;

                                            try
                                            {
                                                foundLimit = int.Parse(value);
                                            }
                                            catch { Server.s.Log("Invalid limit on " + s); break; }

                                            gots++;
                                            thisGroup.maxBlocks = foundLimit;
                                            break;
                                        case "color":
                                            char foundChar;

                                            try
                                            {
                                                foundChar = char.Parse(value);
                                            }
                                            catch { Server.s.Log("Incorrect color on " + s); break; }

                                            if ((foundChar >= '0' && foundChar <= '9') || (foundChar >= 'a' && foundChar <= 'f'))
                                            {
                                                gots++;
                                                thisGroup.color = foundChar.ToString();
                                            }
                                            else
                                            {
                                                Server.s.Log("Invalid color code at " + s);
                                            }
                                            break;
                                        case "filename":
                                            if (value.Contains("\\") || value.Contains("/"))
                                            {
                                                Server.s.Log("Invalid filename on " + s);
                                                break;
                                            }

                                            gots++;
                                            thisGroup.fileName = value;
                                            break;
                                    }

                                    if (gots >= 4)
                                    {
                                        GroupList.Add(new Group(thisGroup.Permission, thisGroup.maxBlocks, thisGroup.trueName, thisGroup.color[0], thisGroup.fileName));
                                    }
                                }
                            }
                            else
                            {
                                Server.s.Log("In ranks.properties, the line " + s + " is wrongly formatted");
                            }
                        }
                    }
                    catch { }
                }
            }

            if (GroupList.Find(grp => grp.Permission == LevelPermission.Banned) == null) GroupList.Add(new Group(LevelPermission.Banned, 1, "Banned", '8', "banned.txt"));
            if (GroupList.Find(grp => grp.Permission == LevelPermission.Guest) == null) GroupList.Add(new Group(LevelPermission.Guest, 1, "Guest", '7', "guest.txt"));
            if (GroupList.Find(grp => grp.Permission == LevelPermission.Builder) == null) GroupList.Add(new Group(LevelPermission.Builder, 400, "Builder", '2', "builders.txt"));
            if (GroupList.Find(grp => grp.Permission == LevelPermission.AdvBuilder) == null) GroupList.Add(new Group(LevelPermission.AdvBuilder, 1200, "AdvBuilder", '3', "advbuilders.txt"));
            if (GroupList.Find(grp => grp.Permission == LevelPermission.Operator) == null) GroupList.Add(new Group(LevelPermission.Operator, 2500, "Operator", 'c', "operators.txt"));
            if (GroupList.Find(grp => grp.Permission == LevelPermission.Admin) == null) GroupList.Add(new Group(LevelPermission.Admin, 65536, "SuperOP", 'e', "uberOps.txt"));
            GroupList.Add(new Group(LevelPermission.Nobody, 65536, "Nobody", '0', "nobody.txt"));

            bool swap = true; Group storedGroup;
            while (swap)
            {
                swap = false;
                for (int i = 0; i < GroupList.Count - 1; i++)
                    if (GroupList[i].Permission > GroupList[i + 1].Permission)
                    {
                        swap = true;
                        storedGroup = GroupList[i];
                        GroupList[i] = GroupList[i + 1];
                        GroupList[i + 1] = storedGroup;
                    }
            }

            if (Group.Find(Server.defaultRank) != null) standard = Group.Find(Server.defaultRank);
            else standard = Group.findPerm(LevelPermission.Guest);

            foreach (Player pl in Player.players)
            {
                pl.group = GroupList.Find(g => g.name == pl.group.name);
            }

            saveGroups(GroupList);
        }
        public static void saveGroups(List<Group> givenList)
        {
            StreamWriter SW = new StreamWriter(File.Create("properties/ranks.properties"));
            SW.WriteLine("#RankName = string");
            SW.WriteLine("#     The name of the rank, use capitalization.");
            SW.WriteLine("#");
            SW.WriteLine("#Permission = num");
            SW.WriteLine("#     The \"permission\" of the rank. It's a number.");
            SW.WriteLine("#		There are pre-defined permissions already set. (for the old ranks)");
            SW.WriteLine("#		Banned = -20, Guest = 0, Builder = 30, AdvBuilder = 50, Operator = 80");
            SW.WriteLine("#		SuperOP = 100, Nobody = 120");
            SW.WriteLine("#		Must be greater than -50 and less than 120");
            SW.WriteLine("#		The higher the number, the more commands do (such as undo allowing more seconds)");
            SW.WriteLine("#Limit = num");
            SW.WriteLine("#     The command limit for the rank (can be changed in-game with /limit)");
            SW.WriteLine("#		Must be greater than 0 and less than 10000000");
            SW.WriteLine("#Color = char");
            SW.WriteLine("#     A single letter or number denoting the color of the rank");
            SW.WriteLine("#	    Possibilities:");
            SW.WriteLine("#		    0, 1, 2, 3, 4, 5, 6, 7, 8, 9, a, b, c, d, e, f");
            SW.WriteLine("#FileName = string.txt");
            SW.WriteLine("#     The file which players of this rank will be stored in");
            SW.WriteLine("#		It doesn't need to be a .txt file, but you may as well");
            SW.WriteLine("#		Generally a good idea to just use the same file name as the rank name");
            SW.WriteLine();
            SW.WriteLine();

            foreach (Group grp in givenList)
            {
                if (grp.name != "nobody")
                {
                    SW.WriteLine("RankName = " + grp.trueName);
                    SW.WriteLine("Permission = " + (int)grp.Permission);
                    SW.WriteLine("Limit = " + grp.maxBlocks);
                    SW.WriteLine("Color = " + grp.color[1]);
                    SW.WriteLine("FileName = " + grp.fileName);
                    SW.WriteLine();
                }
            }

            SW.Flush();
            SW.Close();
        }

        public static bool Exists(string name)
        {
            name = name.ToLower();
            foreach (Group gr in GroupList)
            {
                if (gr.name == name.ToLower()) { return true; }
            } return false;
        }
        public static Group Find(string name)
        {
            name = name.ToLower();

            if (name == "adv") name = "advbuilder";
            if (name == "op") name = "operator";
            if (name == "super" || name == "admin") name = "superop";
            if (name == "noone") name = "nobody";

            foreach (Group gr in GroupList)
            {
                if (gr.name == name.ToLower()) { return gr; }
            } return null;
        }
        public static Group findPerm(LevelPermission Perm)
        {
            foreach (Group grp in GroupList)
            {
                if (grp.Permission == Perm) return grp;
            }
            return null;
        }

        public static string findPlayer(string playerName)
        {
            foreach (Group grp in Group.GroupList)
            {
                if (grp.playerList.Contains(playerName)) return grp.name;
            }
            return Group.standard.name;
        }
        public static Group findPlayerGroup(string playerName)
        {
            foreach (Group grp in Group.GroupList)
            {
                if (grp.playerList.Contains(playerName)) return grp;
            }
            return Group.standard;
        }

        public static string concatList(bool includeColor = true, bool skipExtra = false, bool permissions = false)
        {
            string returnString = "";
            foreach (Group grp in Group.GroupList)
            {
                if (!skipExtra || (grp.Permission > LevelPermission.Guest && grp.Permission < LevelPermission.Nobody))
                    if (includeColor) {
                        returnString += ", " + grp.color + grp.name + Server.DefaultColor;
                    } else if (permissions) {
                        returnString += ", " + ((int)grp.Permission).ToString();
                    } else
                        returnString += ", " + grp.name;
            }

            if (includeColor) returnString = returnString.Remove(returnString.Length - 2);

            return returnString.Remove(0, 2);
        }
    }


}