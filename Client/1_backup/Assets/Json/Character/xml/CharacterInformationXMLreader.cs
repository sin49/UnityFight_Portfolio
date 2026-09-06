using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

//namespace DataExecute
//{
//    public class CharacterInformationXMLreader
//    {
//        string this_Directory = Directory.GetCurrentDirectory();
//        private static CharacterInformationXMLreader _instnace;
//        public static CharacterInformationXMLreader instance { get { if (_instnace == null) _instnace = new CharacterInformationXMLreader(); return _instnace; } }

//        public GameCharacter Read(string xml_name, GameCharacter chr)
//        {
//            if (chr.normal_attack == null)
//            {
//                CharacterSkillSetting.instance.SetPlayerActInterface(xml_name, chr);
//            }
//            XmlDocument xml = new XmlDocument();
//            xml.Load(this_Directory + "\\Assets\\DB\\Character\\" + xml_name + "\\" + xml_name + ".xml");
//            CharacterInformation info = Read_status(xml);


//            Character_action normal = Read_skill_setting(xml, chr.normal_attack, chr.T);
//            List<Character_action> skills = new List<Character_action>();
//            foreach (iAct Act in chr.skill)
//            {
//                skills.Add(Read_skill_setting(xml, Act, chr.T));
//            }
//            List<int> skill_cycle = Read_skillcycle(xml);
//            foreach (int n in skill_cycle)
//            {
//                if (n == 0)
//                {
//                    chr.get_skillinformation(normal);
//                    continue;
//                }
//                foreach (Character_action action in skills)
//                {
//                    if (action.action_name == "Skill" + n.ToString())
//                    {
//                        chr.get_skillinformation(action);
//                        break;
//                    }

//                    Debug.Log("Can't find skill information Please check XML");
//                    chr.get_skillinformation(normal);
//                }
//            }
//            Character_action LB = Read_LimitBurst(xml, chr.LB, chr.T);
//            chr.get_LB(LB);
//            return chr;
//        }
//        public CharacterInformation Read(CharacterData Data)
//        {

//            XmlDocument xml = new XmlDocument();
//            xml.Load(this_Directory + "\\Assets\\DB\\Character\\" + Data.Name + "\\" + Data.Name + ".xml");
//            CharacterInformation info = Read_status(xml);


//            return info;
//        }
//        CharacterInformation Read_status(XmlDocument xml)
//        {
//            XmlNodeList node_list = xml.SelectNodes("/characterinfo/status");
//            CharacterInformation ch = new CharacterInformation();
//            foreach (XmlNode node in node_list)
//            {

//                ch.DistanaceNumber = XmlConvert.ToInt32(node["distancepoint"].InnerText);

//                ch.HP_Grownth = XmlConvert.ToInt32(node["grownth"]["HP_grownth"].InnerText);
//                ch.ATK_Grownth = XmlConvert.ToInt32(node["grownth"]["ATK_grownth"].InnerText);
//            }
//            return ch;
//        }
//        public CharacterInformation Read_status(String xml_name)
//        {
//            XmlDocument xml = new XmlDocument();
//            xml.Load(this_Directory + "\\Assets\\DB\\Character\\" + xml_name + "\\" + xml_name + ".xml");
//            XmlNodeList node_list = xml.SelectNodes("/characterinfo/status");
//            CharacterInformation ch = new CharacterInformation();
//            foreach (XmlNode node in node_list)
//            {

//                ch.DistanaceNumber = XmlConvert.ToInt32(node["distancepoint"].InnerText);

//                ch.HP_Grownth += XmlConvert.ToInt32(node["grownth"]["HP_grownth"].InnerText);
//                ch.ATK_Grownth += XmlConvert.ToInt32(node["grownth"]["ATK_grownth"].InnerText);
//            }
//            return ch;
//        }
//        public Character_action Read_skill_setting(XmlDocument xml, iAct act, Team t)
//        {
//            string skill_string = act.ToString();
//            int skill_string_index = skill_string.IndexOf("_") + 1;
//            string s = skill_string.Substring(skill_string_index, skill_string.Length - skill_string_index);
//            XmlNodeList node_list = xml.SelectNodes("/characterinfo/Skill/" + s);
//            return ReadCharacterAction(s, node_list, act, t);
//        }
//        public List<int> Read_skillcycle(XmlDocument xml)
//        {

//            string str = xml.SelectSingleNode("/characterinfo/Skillcycle").InnerText;
//            List<int> skillcycle = ConvertStringtoIntArray(str);


//            return skillcycle;
//        }
//        List<int> ConvertStringtoIntArray(string s)
//        {
//            List<int> array = new List<int>();
//            for (int n = 0; n < s.Length; n++)
//            {
//                if (s[n] >= '0' && s[n] <= '9')
//                {
//                    array.Add(s[n] - '0');
//                }
//                else
//                    continue;
//            }
//            return array;
//        }
//        Character_action Read_LimitBurst(XmlDocument xml, iAct act, Team t)
//        {
//            if (act == null)
//                return new Character_action() { execution = true };
//            XmlNodeList node_list = xml.SelectNodes("/characterinfo/LimitBurst");

//            return ReadCharacterAction("LimitBurst", node_list, act, t);
//        }
//        Character_action ReadCharacterAction(string s, XmlNodeList nodeList, iAct act, Team t)
//        {
//            Team Target_team = Team.Enemy;
//            List<int> target_array = new List<int>();
//            Func<Team, Vector3, List<int>, List<GameCharacter>> priority_func = null;
//            foreach (XmlNode setting in nodeList)
//            {
//                switch (setting["targeting"].InnerText.ToLower())
//                {
//                    case "enemy":
//                        if (t == Team.Enemy)
//                            Target_team = Team.Player;
//                        else
//                            Target_team = Team.Enemy;
//                        break;
//                    case "player":
//                        if (t == Team.Enemy)
//                            Target_team = Team.Enemy;
//                        else
//                            Target_team = Team.Player;
//                        break;
//                    default:
//                        break;
//                }
//                switch (setting["priority"].InnerText.ToLower())
//                {
//                    case "distance":
//                        priority_func = new Func<Team, Vector3, List<int>, List<GameCharacter>>
//                            (Character_Priority.instance.get_enemy_by_distance);
//                        break;
//                    default:
//                        break;
//                }
//                string targetnumber = setting["TargetNumber"].InnerText;
//                target_array = ConvertStringtoIntArray(targetnumber);
//            }
//            return new Character_action(s, act, Target_team, priority_func, target_array);
//        }
//    }
    

