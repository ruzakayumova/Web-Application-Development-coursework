using ATECA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATECA.DAL
{
    public interface IRoomsRepository
    {
        IList<Rooms> GetAll();
        IList<Rooms> Filter(string Name, int? Price, string Category, int page, int pageSize, string sortField, bool sortDesc, out int totalCount);
        void Insert(Rooms room);
        Rooms GetById(int id);
        void Update(Rooms room);
        void Delete(int id);
    }
}
