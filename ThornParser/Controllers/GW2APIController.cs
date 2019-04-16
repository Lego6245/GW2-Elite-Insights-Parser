using ThornParser.Models;
using ThornParser.Models.ParseModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ThornParser.Controllers
{
    public class GW2APIController
    {
        private SkillList GetSkillList()
        {
            if (_listOfSkills.Items.Count == 0)
            {
                SetSkillList();
            }
            return _listOfSkills;
        }

        private void SetSkillList()
        {

            if (_listOfSkills.Items.Count == 0)
            {

                if (Properties.Resources.skillList.Length != 0)
                {
                    Console.WriteLine("Reading Skilllist");
                    using (StringReader reader = new StringReader(Properties.Resources.skillList))
                    {
                        JsonSerializer serializer = new JsonSerializer()
                        {
                            ContractResolver = new DefaultContractResolver()
                            {
                                NamingStrategy = new CamelCaseNamingStrategy()
                            }
                        };
                        _listOfSkills.Items = (List<GW2APISkill>)serializer.Deserialize(reader, typeof(List<GW2APISkill>));
                        reader.Close();
                    }
                }

            }
            return;
        }

        public class SkillList
        {
            public SkillList() { Items = new List<GW2APISkill>(); }
            public List<GW2APISkill> Items { get; set; }
        }

        static SkillList _listOfSkills = new SkillList();

        public GW2APISkill GetSkill(long id)
        {
            GW2APISkill skill = GetSkillList().Items.FirstOrDefault(x => x.Id == id);
            return skill;
        }

        private SpecList GetSpecList()
        {
            if (_listofSpecs.Items.Count == 0)
            {
                SetSpecList(); 
            }
            return _listofSpecs;
        }
        
        private void SetSpecList()
        {

            if (_listofSpecs.Items.Count == 0)
            {

                if (Properties.Resources.specList.Length != 0)
                {
                    Console.WriteLine("Reading SpecList");
                    using (StringReader reader = new StringReader(Properties.Resources.specList))
                    {
                        JsonSerializer serializer = new JsonSerializer()
                        {
                            ContractResolver = new DefaultContractResolver()
                            {
                                NamingStrategy = new CamelCaseNamingStrategy()
                            }
                        };
                        _listofSpecs.Items = (List<GW2APISpec>)serializer.Deserialize(reader, typeof(List<GW2APISpec>));
                        reader.Close();
                    }
                }
                if (_listofSpecs.Items.Count == 0)//if nothing in file or fail write new file
                {
                    // something went really hyucking wrong
                }

            }
            return;
        }

        public class SpecList
        {
            public SpecList() { Items = new List<GW2APISpec>(); }

            public List<GW2APISpec> Items { get; set; }
        }
     
        static SpecList _listofSpecs = new SpecList();

        public GW2APISpec GetSpec(int id)
        {
            GW2APISpec spec = GetSpecList().Items.FirstOrDefault(x => x.Id == id);
            
            return spec;
        }
    }
}
