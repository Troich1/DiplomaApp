using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Collections.Generic;
using System.Text.Json;

namespace ServerDiplom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class ItemsController : ControllerBase
    {
        private readonly string _connectionString;

        public ItemsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult GetItems()
        {
            var items = new List<Item>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = "SELECT * FROM item";
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
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

            return Ok(JsonSerializer.Serialize(items));
        }


        [HttpGet("{itemId}")]
        public IActionResult GetItemById(int itemId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = "SELECT * FROM item WHERE id = @itemId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@itemId", itemId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var item = new Item
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
                            };
                            return Ok(JsonSerializer.Serialize(item));
                        }
                        else
                        {
                            return NotFound(new { message = "Предмет не найден." });
                        }
                    }
                }
            }
        }

        [HttpGet("check/{itemId}/{classroomId}")]
        public IActionResult CheckItemInClassroom(int itemId, int classroomId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = "SELECT i.id, i.name, i.quantity, i.price, i.state, i.inventorycode, i.factorycode, i.idClass, i.idType, i.nametype, cl.name AS className " +
                            "FROM item i INNER JOIN class cl ON i.idClass = cl.id " +
                            "WHERE i.id = @itemId AND cl.id = @classroomId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@itemId", itemId);
                    command.Parameters.AddWithValue("@classroomId", classroomId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var item = new Item
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
                            };
                            return Ok(JsonSerializer.Serialize(item));
                        }
                        else
                        {
                            return NotFound(new { message = "Предмет не найден или не принадлежит указанному классу." });
                        }
                    }
                }
            }
        }

        [HttpPost]
        public IActionResult CreateItem([FromBody] JsonElement Item)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                // Попытка извлечь свойства ClassName и NameType из JSON объекта
                if (!Item.TryGetProperty("ClassName", out JsonElement classNameElement) ||
                    !Item.TryGetProperty("NameType", out JsonElement nameTypeElement))
                {
                    return BadRequest(new { message = "JSON не содержит необходимых свойств ClassName и NameType." });
                }

                string className = classNameElement.GetString();
                string nameType = nameTypeElement.GetString();

                // Получение idclass на основе ClassName
                var queryGetIdClass = "SELECT id FROM class WHERE name = @name";
                int idClass;
                using (var commandGetIdClass = new MySqlCommand(queryGetIdClass, connection))
                {
                    commandGetIdClass.Parameters.AddWithValue("@name", className);
                    connection.Open();
                    var classResult = commandGetIdClass.ExecuteScalar();
                    idClass = classResult != null ? Convert.ToInt32(classResult) : 0;
                    connection.Close();
                }

                // Получение idtype на основе NameType
                var queryGetIdType = "SELECT id FROM typeItem WHERE name = @name";
                int idType;
                using (var commandGetIdType = new MySqlCommand(queryGetIdType, connection))
                {
                    commandGetIdType.Parameters.AddWithValue("@name", nameType);
                    connection.Open();
                    var typeResult = commandGetIdType.ExecuteScalar();
                    idType = typeResult != null ? Convert.ToInt32(typeResult) : 0;
                    connection.Close();
                }

                // Вставка нового элемента с полученными idclass и idtype
                var queryInsertItem = "INSERT INTO item (name, quantity, price, state, inventorycode, factorycode, idclass, idtype, nameType, classname) " +
                      "VALUES (@name, @quantity, @price, @state, @inventorycode, @factorycode, @idclass, @idtype, @nameType, @classname)";
                using (var commandInsertItem = new MySqlCommand(queryInsertItem, connection))
                {
                    // Предполагается, что Item это JsonElement и у него есть соответствующие свойства
                    commandInsertItem.Parameters.AddWithValue("@name", Item.GetProperty("Name").GetString() ?? (object)DBNull.Value);
                    commandInsertItem.Parameters.AddWithValue("@quantity", Item.GetProperty("Quantity").GetInt32());
                    commandInsertItem.Parameters.AddWithValue("@price", Item.GetProperty("Price").GetSingle());
                    commandInsertItem.Parameters.AddWithValue("@state", Item.GetProperty("State").GetString() ?? (object)DBNull.Value);
                    commandInsertItem.Parameters.AddWithValue("@inventorycode", Item.GetProperty("InventoryCode").GetString() ?? (object)DBNull.Value);
                    commandInsertItem.Parameters.AddWithValue("@nameType", nameType);
                    commandInsertItem.Parameters.AddWithValue("@factorycode", Item.GetProperty("FactoryCode").GetString() ?? (object)DBNull.Value);
                    commandInsertItem.Parameters.AddWithValue("@idclass", idClass);
                    commandInsertItem.Parameters.AddWithValue("@idtype", idType);
                    commandInsertItem.Parameters.AddWithValue("@classname", className);

                    connection.Open();
                    int result = commandInsertItem.ExecuteNonQuery();
                    connection.Close();

                    if (result > 0)
                    {
                        // Получение и возврат ID вставленной записи
                        long insertedId = commandInsertItem.LastInsertedId;
                        return Ok(new
                        {
                            ID = insertedId,
                            Name = Item.GetProperty("Name").GetString(),
                            Quantity = Item.GetProperty("Quantity").GetInt32(),
                            Price = Item.GetProperty("Price").GetSingle(),
                            State = Item.GetProperty("State").GetString(),
                            InventoryCode = Item.GetProperty("InventoryCode").GetString(),
                            FactoryCode = Item.GetProperty("FactoryCode").GetString(),
                            ClassName = className,
                            NameType = nameType
                        });
                    }
                    else
                    {
                        return BadRequest(new { message = "Не удалось добавить элемент." });
                    }
                }
            }
        }



        [HttpPut("update/{id}")]
        public IActionResult UpdateItemClass(int id, [FromBody] UpdateItemClassDto updateDto)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                // Обновляем idClass и ClassName
                var query = "UPDATE item SET idClass = @newIdClass, classname = @newClassName WHERE id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@newIdClass", updateDto.NewClassroomId);
                    command.Parameters.AddWithValue("@newClassName", updateDto.NewClassName); // Добавляем параметр для нового ClassName

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        // После успешного обновления, получаем обновленный Item
                        var selectQuery = "SELECT * FROM item WHERE id = @id";
                        using (var selectCommand = new MySqlCommand(selectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@id", id);
                            using (var reader = selectCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var updatedItem = new Item
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
                                    };
                                    var jsonResult = JsonSerializer.Serialize(updatedItem);
                                    return Ok(jsonResult);
                                }
                                else
                                {
                                    return NotFound(new { message = "Item not found after update." });
                                }
                            }
                        }
                    }
                    else
                    {
                        return NotFound(new { message = "Item not found." });
                    }
                }
            }
        }

        [HttpPut("update/state/{id}")]
        public IActionResult UpdateItemState(int id, [FromBody] JsonElement body)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                // Десериализация тела запроса для получения нового состояния
                string newState;
                try
                {
                    newState = body.GetProperty("newState").GetString();
                }
                catch (JsonException)
                {
                    return BadRequest(new { message = "Invalid JSON format." });
                }

                var query = "UPDATE item SET state = @newState WHERE id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@newState", newState);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        // Сериализация сообщения об успешном обновлении
                        var successMessage = JsonSerializer.Serialize(new { message = "Item state updated successfully." });
                        return Ok(successMessage);
                    }
                    else
                    {
                        // Сериализация сообщения об ошибке
                        var errorMessage = JsonSerializer.Serialize(new { message = "Item not found." });
                        return NotFound(errorMessage);
                    }
                }
            }
        }

        public class UpdateItemClassDto
        {
            public int NewClassroomId { get; set; }
            public string NewClassName { get; set; }
        }

    }
}
