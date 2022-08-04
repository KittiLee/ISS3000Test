using System;
using System.Linq;
using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AirlineWeb.MessageBus;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class FlightDetailController : ControllerBase
    {
        private readonly AirlineDbContext _context; 
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBus;
        public FlightDetailController(AirlineDbContext context,IMapper mapper,IMessageBusClient messageBus)
        {
            _context=context;
            _mapper=mapper;
            _messageBus=messageBus;
        }

        [HttpGet("{flightCode}", Name = "GetFlightDetailsByCode")]
        public ActionResult<FlightDetailReadDto> GetFlightDetailsByCode(string flightCode)
        {
            var flight = _context.FlightDetails.FirstOrDefault(f => f.FlightCode == flightCode);

            if (flight == null)
            {
                return NotFound();
            }

            //This mapping won't work as I have not done the Profiles section Duh!!!
            return Ok(_mapper.Map<FlightDetailReadDto>(flight));
        }


        [HttpPost]
        public ActionResult<FlightDetailCreateDto> CreateSubsription(FlightDetailCreateDto flightDetailCreateDtos)
        {
           var flight = _context.FlightDetails.FirstOrDefault(s => s.FlightCode == flightDetailCreateDtos.FlightCode);
  

           if (flight == null)
           {
               var flightDetail = _mapper.Map<FlightDetail>(flightDetailCreateDtos);
               try
               {
                   _context.FlightDetails.Add(flightDetail);
                   _context.SaveChanges();
               }
               catch(Exception ex)
               {
                   return BadRequest(ex.Message);
               }
               //return flightDetail;
               var flightDetailReadDto = _mapper.Map<FlightDetailReadDto>(flightDetail);

               return CreatedAtRoute(nameof(GetFlightDetailsByCode), new { flightCode = flightDetailReadDto.FlightCode}, flightDetailReadDto);
                
           }
           else
           {
               return NoContent();
           }
        }

        
        [HttpPut("{id}")]
        public ActionResult UpdateFlightDetail(int id, FlightDetailUpdateDto flightDetailUpdateDto)
        {
            var flight = _context.FlightDetails.FirstOrDefault(f => f.Id == id);

            if(flight == null)
            {
                return NotFound();
            }

            decimal oldPrice = flight.Price;

            _mapper.Map(flightDetailUpdateDto, flight);

            try
            {
                _context.SaveChanges();
                
                if(oldPrice != flight.Price)
                {
                    Console.WriteLine("Price Changed - Place message on bus");

                    var message = new NotificationMessageDto{
                        WebhookType = "pricechange",
                        OldPrice = oldPrice,
                        NewPrice = flight.Price,
                        FlightCode = flight.FlightCode
                    };
                    _messageBus.SendMessage(message);
                }
                else
                {
                    Console.WriteLine("No Price change");
                }
                
                return NoContent();
                
              

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }                   
        } 


    }
}