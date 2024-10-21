using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;
using EmployeesLoaderPlugin;
using System.Net;
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using NLog.Targets;
using Microsoft.Win32;
using Newtonsoft.Json.Serialization;
using System.Threading;

namespace EmployeesAddPlugin
{

    [Author(Name = "Soleev Maksim")]
    public class Plugin : IPluggable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        const string URL = "https://dummyjson.com/users";
        
        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
        {
            WebClient webClient = new WebClient();
            string json = webClient.DownloadString(URL);

            logger.Info("Loading new employees");
            var employeesList = args.Cast<EmployeesDTO>().ToList();

            List<EmployeesDTO> result = new List<EmployeesDTO>();

            var employeesJson = JObject.Parse(json);
            
            foreach (var employeeData in employeesJson.SelectToken("users"))
            {
                EmployeesDTO employeesDTO = new EmployeesDTO();
                employeesDTO.Name = (string)employeeData.SelectToken("firstName") + " " + (string)employeeData.SelectToken("lastName");
                employeesDTO.AddPhone((string)employeeData.SelectToken("phone"));
                result.Add(employeesDTO);
            }
            employeesList = result;
            logger.Info($"Loaded {employeesList.Count()} employees");

            return employeesList.Cast<DataTransferObject>();
        }
    }
}