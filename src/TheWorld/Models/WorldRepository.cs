using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddStop(string tripName, Stop newStop, string username)
        {
            var trip = GetUserTripByName(tripName, username);

            if (trip != null)
            {
                trip.Stops.Add(newStop);
                _context.Stops.Add(newStop); // Requires both lines. First sets FK, 2nd adds to DB
            }
        }

        public void AddTrip(Trip trip)
        {
            _context.Add(trip);
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            _logger.LogInformation("Getting all Trips");
            var trips = _context.Trips.ToList();
            return trips;
        }

        public IEnumerable<Trip> GetTripsByUsername(string name)
        {
            return _context
                .Trips
                .Include(x => x.Stops)
                .Where(q => q.UserName.Equals(name))
                .ToList();
        }

        public Trip GetTripByName(string tripName)
        {
            return _context.Trips.Include(t => t.Stops).FirstOrDefault(t => t.Name.Equals(tripName));
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0; // Value returned by SaveChanges shows number of rows affected
        }

        public Trip GetUserTripByName(string tripName, string name)
        {
            return _context
                .Trips
                .Include(t => t.Stops)
                .FirstOrDefault(t => t.Name.Equals(tripName) && 
                                     t.UserName.Equals(name));
        }
    }
}
