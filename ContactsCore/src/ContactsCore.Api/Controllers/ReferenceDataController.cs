using ContactsCore.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContactsCore.Api.Controllers
{
    [Route("api/[controller]")]
    public class ReferenceDataController : Controller
    {
        private static readonly Type[] ReferenceDataEnums = 
        {
            typeof(ContactDetailType)
        };

        private static readonly Dictionary<string, Dictionary<string, int>> ReferenceData;
        
        static ReferenceDataController()
        {
            ReferenceData = GetReferenceData();
        }

        private static Dictionary<string, Dictionary<string, int>> GetReferenceData()
        {
            var enums = new Dictionary<string, Dictionary<string, int>>();

            foreach (var type in ReferenceDataEnums)
            {
                var members = Enum.GetValues(type)
                    .Cast<int>()
                    .ToDictionary(val => Enum.GetName(type, val));
                enums.Add(type.Name, members);
            }

            return enums;
        }

        [HttpGet]
        public IActionResult Get()
        {            
            return Ok(ReferenceData);
        }

        [HttpGet("{typeName}")]
        public IActionResult Get(string typeName)
        {
            if (!ReferenceData.ContainsKey(typeName))
                return NotFound();

            return Ok(ReferenceData[typeName]);
        }
    }
}
