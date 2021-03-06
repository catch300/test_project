﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project_service.Models;

namespace test_project.Models.ViewModels
{
    public class VehicleModelVM
    {
        public int Id { get; set; }
    
        public string Name { get; set; }

        public string Abrv { get; set; }
        public VehicleMake Make{ get; set; }
        public int MakeId { get; set; }
    }
}
