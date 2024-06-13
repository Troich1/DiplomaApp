using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Text.Json;

namespace ServerDiplom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private readonly string _connectionString;

        public ClassroomController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Получение списка всех кабинетов
        [HttpGet]
        public IActionResult GetClassrooms()
        {
            var classrooms = new List<object>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                // Получение информации о всех кабинетах
                var queryClassrooms = "SELECT * FROM class";
                using (var commandClassrooms = new MySqlCommand(queryClassrooms, connection))
                {
                    using (var reader = commandClassrooms.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var classroomId = reader.IsDBNull(reader.GetOrdinal("id")) ? default : reader.GetInt32("id");
                            var items = GetItemsForClassroom(classroomId); // Вызов метода для получения предметов кабинета

                            classrooms.Add(new
                            {
                                Id = classroomId,
                                Name = reader.IsDBNull(reader.GetOrdinal("name")) ? "Неизвестно" : reader.GetString("name"),
                                Type = reader.IsDBNull(reader.GetOrdinal("type")) ? "Неопределен" : reader.GetString("type"),
                                IdKafedra = reader.IsDBNull(reader.GetOrdinal("IdKafedra")) ? default : reader.GetInt32("IdKafedra"),
                                Items = items // Добавление списка предметов к информации о кабинете
                            });
                        }
                    }
                }
            }

            // Проверка наличия кабинетов и возврат результата
            if (classrooms.Any())
            {
                return Ok(JsonSerializer.Serialize(classrooms));
            }
            else
            {
                return NotFound("Кабинеты не найдены.");
            }
        }

        // Метод для получения списка предметов для конкретного кабинета
        private List<object> GetItemsForClassroom(int classroomId)
        {
            var items = new List<object>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var queryItems = "SELECT * FROM item WHERE idClass = @idClass";
                using (var commandItems = new MySqlCommand(queryItems, connection))
                {
                    commandItems.Parameters.AddWithValue("@idClass", classroomId);
                    connection.Open();
                    using (var reader = commandItems.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString("name"),
                                NameType = reader.IsDBNull(reader.GetOrdinal("nametype")) ? null : reader.GetString("nametype"),
                                Quantity = reader.IsDBNull(reader.GetOrdinal("quantity")) ? 0 : reader.GetInt32("quantity"),
                                Price = reader.IsDBNull(reader.GetOrdinal("price")) ? 0.0f : reader.GetFloat("price"),
                                State = reader.IsDBNull(reader.GetOrdinal("state")) ? null : reader.GetString("state"),
                                ClassName = reader.IsDBNull(reader.GetOrdinal("classname")) ? null : reader.GetString("classname"),
                                InventoryCode = reader.IsDBNull(reader.GetOrdinal("inventorycode")) ? null : reader.GetString("inventorycode"),
                                FactoryCode = reader.IsDBNull(reader.GetOrdinal("factorycode")) ? null : reader.GetString("factorycode"),
                                IdClass = reader.IsDBNull(reader.GetOrdinal("idclass")) ? 0 : reader.GetInt32("idclass"),
                                IdType = reader.IsDBNull(reader.GetOrdinal("idtype")) ? 0 : reader.GetInt32("idtype")
                            });
                        }
                    }
                }
            }
            return items;
        }


        // Получение детальной информации о кабинете по ID
        [HttpGet("{id}")]
        public IActionResult GetClassroom(int id)
        {
            object classroom = null;
            var items = new List<object>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                // Получение информации о кабинете
                var queryClassroom = "SELECT * FROM class WHERE id = @id";
                using (var commandClassroom = new MySqlCommand(queryClassroom, connection))
                {
                    commandClassroom.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = commandClassroom.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            classroom = new
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("id")) ? default : reader.GetInt32("id"),
                                Name = reader.IsDBNull(reader.GetOrdinal("name")) ? "Неизвестно" : reader.GetString("name"),
                                Type = reader.IsDBNull(reader.GetOrdinal("type")) ? "Неопределен" : reader.GetString("type"),
                                IdKafedra = reader.IsDBNull(reader.GetOrdinal("IdKafedra")) ? default : reader.GetInt32("IdKafedra")
                            };
                        }
                    }
                }

                // Получение списка имущества в кабинете
                var queryItems = "SELECT * FROM item WHERE idClass = @idClass";
                using (var commandItems = new MySqlCommand(queryItems, connection))
                {
                    commandItems.Parameters.AddWithValue("@idClass", id);
                    using (var reader = commandItems.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString("name"),
                                NameType = reader.IsDBNull(reader.GetOrdinal("nametype")) ? null : reader.GetString("nametype"),
                                Quantity = reader.IsDBNull(reader.GetOrdinal("quantity")) ? 0 : reader.GetInt32("quantity"),
                                Price = reader.IsDBNull(reader.GetOrdinal("price")) ? 0.0f : reader.GetFloat("price"),
                                State = reader.IsDBNull(reader.GetOrdinal("state")) ? null : reader.GetString("state"),
                                ClassName = reader.IsDBNull(reader.GetOrdinal("classname")) ? null : reader.GetString("classname"),
                                InventoryCode = reader.IsDBNull(reader.GetOrdinal("inventorycode")) ? null : reader.GetString("inventorycode"),
                                FactoryCode = reader.IsDBNull(reader.GetOrdinal("factorycode")) ? null : reader.GetString("factorycode"),
                                IdClass = reader.IsDBNull(reader.GetOrdinal("idclass")) ? 0 : reader.GetInt32("idclass"),
                                IdType = reader.IsDBNull(reader.GetOrdinal("idtype")) ? 0 : reader.GetInt32("idtype")
                            });
                        }
                    }
                }
            }

            if (classroom != null)
            {
                var result = new { Classroom = classroom, Items = items };
                return Ok(JsonSerializer.Serialize(result));
            }
            else
            {
                return NotFound($"Кабинет с ID {id} не найден.");
            }
        }

        [HttpPost]
        public IActionResult CreateClassroom([FromBody] Classroom classroom)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = "INSERT INTO class (Name, Type, IdKafedra) VALUES (@Name, @Type, @IdKafedra)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", classroom.Name);
                    command.Parameters.AddWithValue("@Type", classroom.Type);
                    command.Parameters.AddWithValue("@IdKafedra", classroom.IdKafedra);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return Ok(new { message = "Кабинет успешно добавлен.", classroomId = command.LastInsertedId });
                    }
                    else
                    {
                        return BadRequest("Не удалось добавить кабинет.");
                    }
                }
            }


        }
    }
}

