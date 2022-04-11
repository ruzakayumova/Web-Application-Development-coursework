using ATECA.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace ATECA.DAL
{
    public class RoomsRepository : IRoomsRepository
    {

        private string ConnStr
        {
            get { return WebConfigurationManager.ConnectionStrings["atecaConnStr"].ConnectionString; }
        }

        public void Delete(int id)
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"delete from rooms where id = @id";
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IList<Rooms> Filter(string Name, int? Price, string Category, int page, int pageSize, string sortField, bool sortDesc, out int totalCount)
        {
            IList<Rooms> rooms = new List<Rooms>();
            using (var conn = new SqlConnection(ConnStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    string fields = @"[Id]
	                                        ,[Name]
	                                        ,[Sleep]
	                                        ,[Beds]
	                                        ,[Description]
	                                        ,[Price]
	                                        ,[Category]";
                    string sql = @"select 
                                        {0}
                                        from rooms
                                    ";
                    string whereSql = "";
                    if (!string.IsNullOrWhiteSpace(Name))
                    {
                        whereSql += (whereSql.Length == 0 ? "" : " AND ") + " Name like @Name + '%' ";
                        cmd.Parameters.AddWithValue("@Name", Name);
                    }
                    if (Price.HasValue)
                    {
                        whereSql += (whereSql.Length == 0 ? "" : " AND ") + " Price = @Price ";
                        cmd.Parameters.AddWithValue("@Price", Price);
                    }
                    if (!string.IsNullOrWhiteSpace(Category))
                    {
                        whereSql += (whereSql.Length == 0 ? "" : " AND ") + " Category like @Category + '%' ";
                        cmd.Parameters.AddWithValue("@Category", Category);
                    }

                    if (!string.IsNullOrWhiteSpace(whereSql))
                    {
                        whereSql = " WHERE " + whereSql;
                    }

                    conn.Open();
                    cmd.CommandText = string.Format(sql, "count(*) ") + whereSql;
                    totalCount = (int)cmd.ExecuteScalar();

                    string pageSql = " OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);

                    if (!string.IsNullOrWhiteSpace(sortField))
                    {
                        sortField = "Name";
                    }

                    cmd.CommandText = string.Format(sql, fields)+ whereSql + 
                        $" ORDER BY {sortField} {(sortDesc ? "DESC" : "ASC")}" + pageSql;

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var room = new Rooms();
                            room.Id = rdr.GetInt32(rdr.GetOrdinal("Id"));
                            room.Name = rdr.GetString(rdr.GetOrdinal("Name"));
                            room.Sleep = rdr.GetInt32(rdr.GetOrdinal("Sleep"));
                            room.Beds = rdr.GetString(rdr.GetOrdinal("Beds"));
                            room.Description = rdr.GetString(rdr.GetOrdinal("Description"));
                            room.Price = rdr.GetInt32(rdr.GetOrdinal("Price"));
                            room.Category = rdr.GetString(rdr.GetOrdinal("Category"));

                            rooms.Add(room);
                        }
                    }
                }
            }
            return rooms;
        }

        public IList<Rooms> GetAll()
        {
            IList <Rooms> rooms = new List<Rooms>();
            using (var conn = new SqlConnection(ConnStr))
            {
                using(var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select [Id]
	                                        ,[Name]
	                                        ,[Sleep]
	                                        ,[Beds]
	                                        ,[Description]
	                                        ,[Price]
	                                        ,[Category]
                                        from rooms
                                    ";
                    conn.Open();
                    using(var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var room = new Rooms();
                            room.Id = rdr.GetInt32(rdr.GetOrdinal("Id"));
                            room.Name = rdr.GetString(rdr.GetOrdinal("Name"));
                            room.Sleep = rdr.GetInt32(rdr.GetOrdinal("Sleep"));
                            room.Beds = rdr.GetString(rdr.GetOrdinal("Beds"));
                            room.Description = rdr.GetString(rdr.GetOrdinal("Description"));
                            room.Price = rdr.GetInt32(rdr.GetOrdinal("Price"));
                            room.Category = rdr.GetString(rdr.GetOrdinal("Category"));

                            rooms.Add(room);
                        }
                    }
                }
            }
            return rooms;
        }

        public Rooms GetById(int id)
        {
            Rooms room = null;
            using(var conn = new SqlConnection(ConnStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select [Id]
	                                        ,[Name]
	                                        ,[Sleep]
	                                        ,[Beds]
	                                        ,[Description]
	                                        ,[Price]
	                                        ,[Category]
                                        from rooms
                                        where Id = @Id
                                    ";
                    cmd.Parameters.AddWithValue("@Id", id);
                    
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            room = new Rooms()
                            {
                                Id = id,
                                Name = rdr.GetString(rdr.GetOrdinal("Name")),
                                Sleep = rdr.GetInt32(rdr.GetOrdinal("Sleep")),
                                Beds = rdr.GetString(rdr.GetOrdinal("Beds")),
                                Description = rdr.GetString(rdr.GetOrdinal("Description")),
                                Price = rdr.GetInt32(rdr.GetOrdinal("Price")),
                                Category = rdr.GetString(rdr.GetOrdinal("Category")),

                            };
                        }
                    }
                }
            }
            return room;
        }

        public void Insert(Rooms room)
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO rooms
				                                        ([Name]
				                                        ,[Sleep]
				                                        ,[Beds]
				                                        ,[Description]
				                                        ,[Price]
				                                        ,[category]
				                                        )
			                                        VALUES
				                                        (
				                                         @Name
				                                        ,@Sleep
				                                        ,@Beds
				                                        ,@Description
				                                        ,@Price
				                                        ,@Category
				                                        )";
                    cmd.Parameters.AddWithValue("@Name", room.Name);
                    cmd.Parameters.AddWithValue("@Sleep", room.Sleep);
                    cmd.Parameters.AddWithValue("@Beds", room.Beds);
                    cmd.Parameters.AddWithValue("@Description", room.Description);
                    cmd.Parameters.AddWithValue("@Price", room.Price);
                    cmd.Parameters.AddWithValue("@Category", room.Category);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Rooms room)
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE [dbo].[Rooms]
                                    SET [Name] = @Name
                                        ,[Sleep] = @Sleep
                                        ,[Beds] = @Beds
                                        ,[Description] = @Description
                                        ,[Price] = @Price
                                        ,[Category] = @Category

                                    WHERE Id = @Id";

                    cmd.Parameters.AddWithValue("@Name", room.Name);
                    cmd.Parameters.AddWithValue("@Sleep", room.Sleep);
                    cmd.Parameters.AddWithValue("@Beds", room.Beds);
                    cmd.Parameters.AddWithValue("@Description", room.Description);
                    cmd.Parameters.AddWithValue("@Price", room.Price);
                    cmd.Parameters.AddWithValue("@Category", room.Category);
                    cmd.Parameters.AddWithValue("@Id", room.Id);

                    conn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}