using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;
using WebApi.Model;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    public class VehiclesController : Controller
    {
        private IVehicleService _vehicleService;
        private IMapper _mapper;

        public VehiclesController(IVehicleService productService, IMapper mapper)
        {
            _vehicleService = productService;
            _mapper = mapper;
        }
        [Authorize(Roles = "Admin,User")]

        [HttpGet("{userid}/{q?}")]
        public IActionResult GetVehicles(string userid, string q = "")
        {
            if (q == null ||q == "undefined")  q = "";
            List<Vehicle> vehicles=default(List<Vehicle>) ;
            try{
                var claims = User.Claims.Select(x => new {Type = x.Type, Value = x.Value});
                vehicles = _vehicleService.Get(userid,q);
            }
            catch(AppException){
                
            }
            return Ok(vehicles);
        }

        // [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Vehicle))]
        [ProducesResponseType(404)]
        public IActionResult GetVehicle(long id)
        {
            Vehicle vehicle=default(Vehicle);
            try{
                vehicle = _vehicleService.GetById(id);
                if (vehicle == null) return NotFound();
            }
            catch(AppException){

            }
            return Ok(vehicle);
        }

        // [Authorize(Roles = "Admin")]   
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Vehicle))]
        [ProducesResponseType(400)]
        public IActionResult PostVehicle([FromBody][Required]VehicleDto vehicleDto)
        {
            if (!ModelState.IsValid || vehicleDto == null || string.IsNullOrEmpty(vehicleDto.Name)) return BadRequest(ModelState);
            Vehicle vehicle=default(Vehicle);
            try{
                vehicle=_mapper.Map<Vehicle>(vehicleDto);
                vehicle=_vehicleService.Add(vehicle);
            }
            catch(AppException){

            }
            return Ok(new {Vehicle=vehicle,StatusCode="Vehicle added"});
        }

        [HttpPut("{id}")]
        public IActionResult PutVehicle(string id, [FromBody]VehicleDto vehicleDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Vehicle vehicle=default(Vehicle);
            try{
                vehicle=_mapper.Map<Vehicle>(vehicleDto);

            if (!_vehicleService.Update(vehicle)) return NotFound();
            }
            catch(AppException) {

            }
            return Ok(new { Status = "Vehicle updated" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteVehicle(long id)
        {
            try{
                if (!_vehicleService.Delete(id)) return BadRequest();
            }
            catch(AppException){

            }
            return Ok(new { Status = "Vehicle deleted" });
        }
    }
}