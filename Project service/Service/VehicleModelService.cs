﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Project_service.Models;
using Microsoft.EntityFrameworkCore;
using Project_service.PagingFIlteringSorting;

namespace Project_service.Service
{
    public class VehicleModelService : IVehicleModelService
    {
        private readonly VehicleContext _db = new VehicleContext();

        public VehicleModelService(VehicleContext db)
        {
            _db = db;
        }

      

        //GET - VehicleModel
        public async Task<IVehicleModel> GetVehicleModel(int? Id)
        {
            var vehicleModel = await _db.VehicleModels
               .Include(v => v.Make)
               .FirstOrDefaultAsync(m => m.Id == Id);
            if (_db != null)
            {
                return  vehicleModel;
            }

            return null;
        }

        //GETALL - VehicleModels
        public async Task<IPaginatedList<IVehicleModel>> GetVehicleModels(ISorting sort, IFiltering filter, int? page)
        {
            var vehicleModel = from v in _db.VehicleModels.Include(v => v.Make)
                                 select v;
            
            if (filter.SearchString != null)
            {
                page = 1;
            }
            else
            {
                filter.SearchString = filter.CurrentFilter;
            }

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                vehicleModel = vehicleModel.Where(v => v.Make.Name.Contains(filter.SearchString));
            }

            vehicleModel = sort.SortOrder switch
            {
                "name_desc" => vehicleModel.OrderByDescending(x => x.Name),
                "abrv_desc" => vehicleModel.OrderByDescending(x => x.Abrv),
                "abrv_asc" => vehicleModel.OrderBy(x => x.Abrv),
                "make_asc" => vehicleModel.OrderBy(x => x.Make.Name),
                "make_desc" => vehicleModel.OrderByDescending(x => x.Make.Name),
                _ => vehicleModel.OrderBy(x => x.Name),
            };

            IPaginatedList<IVehicleModel> paginatedList = new PaginatedList<IVehicleModel>();
            int pageSize = 3;
            return await paginatedList.CreateAsync(vehicleModel.AsNoTracking(), page ?? 1, pageSize);

        }



        //CREATE - VehicleModel
        public async Task<IVehicleModel> CreateVehicleModel(VehicleModel _vehicleModel)
        {

            _db.VehicleModels.Add(_vehicleModel);
            await _db.SaveChangesAsync();
            return _vehicleModel;
        }

        //UPDATE - VehicleModel
        public async Task<IVehicleModel> EditVehicleModel(VehicleModel _vehicleModel)
        {
            var vehicleModel = _db.VehicleModels.Attach(_vehicleModel);
            vehicleModel.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _db.SaveChangesAsync();
            return _vehicleModel;
        }

        //DELETE - VehicleModel
        public async Task<IVehicleModel> DeleteVehicleModel(int id)
        {

            VehicleModel vehicleModel = _db.VehicleModels.Find(id);
            if (vehicleModel != null)
            {
                _db.VehicleModels.Remove(vehicleModel);
                await _db.SaveChangesAsync();
            }
            return vehicleModel;
        }
    }
}
